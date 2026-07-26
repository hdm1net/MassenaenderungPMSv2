namespace MassenaenderungPMSv2.HosteingabePMS
{
    internal class PO
    {
        private Helper.Logging logging { get; set; }

        private MyConfiguration myConfiguration { get; set; }

        private fiServiceXmlClasses.PO.ProzessUebersicht prozessUebersicht { get; set; }

        internal PO()
        {

            logging = new Helper.Logging();
            myConfiguration = MyConfiguration.GetConfiguration();
            prozessUebersicht = new fiServiceXmlClasses.PO.ProzessUebersicht();

        }

        internal bool HostEingabeDurchfuehren(ImpSpezXMLClasses.Uebersicht impSpezUebersicht, bool testlauf, int? delay)
        {

            //
            // Falls keine Logdatei angegeben, dann einen Default setzen
            //
            if (String.IsNullOrEmpty(impSpezUebersicht.Importspezifikation?.Prozess?.Logdatei))
            {
                impSpezUebersicht.Importspezifikation!.Prozess!.Logdatei = "PMSProzessEingabeLog.log";
            }

            //
            // Vorabchecks der Importspezifikation
            //
            if (!PruefeImportspezifikation(impSpezUebersicht)) return false;

            //
            // Import-Datei lesen 
            //
            System.Data.DataTable? dataTableImportdatei = LeseImportdatei(impSpezUebersicht);
            if (dataTableImportdatei == null || dataTableImportdatei.Rows.Count < 1) return false;

            //
            // Protokollierung der Auftragsdaten
            //
            ProtokolliereAuftragskopf(impSpezUebersicht, dataTableImportdatei);
            
            //
            // Prozess-Uebersicht fuer den Eingabe-Prozess ermitteln
            //
            prozessUebersicht = Helper.fiServiceXml.PO.GetProzessUebersicht(impSpezUebersicht.Importspezifikation.Prozess.Name ?? String.Empty);
            if (prozessUebersicht == null || prozessUebersicht.TopEntitaet == null || prozessUebersicht.TopEntitaet.Prozess.Name == null)
            {
                LogError("Konnte Prozessdaten aus " + (impSpezUebersicht.Importspezifikation.Prozess.Name ?? string.Empty) + ".xml nicht ermitteln.", impSpezUebersicht.Importspezifikation.Prozess.Logdatei);
                return false;
            }

            //
            // Importdatei um techn. Spalten fuer die Hosteingabe erweitern
            //
            if (!Helper.ImpSpezXml.Global.CheckAndExpandCsvFilePMSColumns(impSpezUebersicht))
            {
                LogError("Fehler bei der Erweiterung der Eingabedatei um die techn. PMS-Felder", impSpezUebersicht.Importspezifikation.Prozess.Logdatei);
                return false;
            }
            if (!dataTableImportdatei.Columns.Contains("HOSTEINGABE_ID"))
            {
                // Falls die Spalten nicht vorhanden waren, jetzt nochmal neu einlesen
                dataTableImportdatei = Helper.ImpSpezXml.Global.ReadAndConvertCsvToDataTable(impSpezUebersicht);
            }

            //
            // Daten der notwendigen Aufrufparameter gem. Importspezifikation ermitteln
            //
            List<fiServiceXmlClasses.PO.Parameter> prozessaufrufParameterListe = Helper.fiServiceXml.PO.GetAufrufParameter(impSpezUebersicht);
            if (prozessaufrufParameterListe.Count < 1)
            {
                LogError("Aufrufparameter fuer den Prozessaufruf konnten nicht ermittelt werden!", impSpezUebersicht.Importspezifikation.Prozess.Logdatei);
                return false;
            }

            //
            // PMS Login durchfuehren
            //
            var pmsHelper = StartePMSSession();
            if (pmsHelper == null) return false;

            //
            // PMS Session erzeugen
            //
            BWSPS.PS_Session psSession = pmsHelper.Session; // Session-Objekt aus dem Login-Helper holen
            if (psSession == null) return false;

            //
            // Hosteingabe starten protokollieren
            //
            Log("Verarbeitung gestartet", impSpezUebersicht.Importspezifikation.Prozess.Logdatei);

            //
            // PMS Prozess erzeugen
            //
            BWSPS.PS_Prozess psProzess = ErzeugePMSProzess(pmsHelper, impSpezUebersicht);
            if (psProzess == null) return false;

            //
            // Eingabekardinalität der EingabeparameterCollection ermitteln
            //
            int eingabekardinalitaet;
            if (!int.TryParse(impSpezUebersicht.Importspezifikation.Prozess.EingabeParameterCollection?.EingabeKardinalitaet, out eingabekardinalitaet))
            {
                eingabekardinalitaet = 1; // default
                Log("EingabeParameterCollectionKardinalitaet konnte nicht geparst werden, Default=1 gesetzt", impSpezUebersicht.Importspezifikation.Prozess.Logdatei);
            }
        
            //
            // Schleife ueber alle Datensaetze der Importdatei
            // 
            bool firstRun = true;
            for (int iRowIndex = 0; iRowIndex < dataTableImportdatei.Rows.Count; iRowIndex++)
            {

                //
                // Aktueller Datensatz aus der Importdatei
                //
                System.Data.DataRow dataRowImportdatei = dataTableImportdatei.Rows[iRowIndex];

                //
                // Uberspringen, falls der Datensatz bereits verarbeitet wurde
                // 
                if (!String.IsNullOrEmpty(dataRowImportdatei["HOSTEINGABE_STATUSCODE"].ToString())) { continue; }

                //
                // Prozessparameter setzen
                // 
                string parameterName = string.Empty;
                // parameterWert hier als String-Array, wird vor dem Setzen in ein PMS-Input-Array mit den korrekten inneren Datentypen konvertiert
                string[] parameterWert;
                BWSPS.EnumParameterTyp parameterTyp;
                BWSPS.EnumParameterArt parameterArt;
                int parameterLaenge = 1; // default
                int parameterLfdn = 1;  // default


                // Schleife über alle Eingabeparameter mit der Lfdn 1
                // Falls der Eingabeparameter als Array in der Importspezifikation definiert ist, dann wird das gesamte Array beim Aufruf des Array-Parameters mit der Lfdn Nr gefuellt
                if (impSpezUebersicht.Importspezifikation.Prozess.EingabeParameterCollection?.Eingabeparameter == null)
                {
                    LogError("Die EingabeparameterCollection der Importspezifikation ist leer!", impSpezUebersicht.Importspezifikation.Prozess.Logdatei);
                    break;
                }

                foreach (ImpSpezXMLClasses.Eingabeparameter impSpezEingabeparameter in impSpezUebersicht.Importspezifikation.Prozess.EingabeParameterCollection.Eingabeparameter)
                {

                    // Aufrufparameterdaten aus der Prozessdokumentation der FI (XML) lesen
                    fiServiceXmlClasses.PO.Parameter? prozessaufrufParameter = prozessaufrufParameterListe.Find(p => String.Equals(p.Name, impSpezEingabeparameter.Name, StringComparison.OrdinalIgnoreCase));

                    // Parameter Lfdn aus der Importspezifikation 
                    if (!int.TryParse(impSpezEingabeparameter.Lfdn, out parameterLfdn))
                    {
                        parameterLfdn = 1;
                    }

                    // Den Parameter der Importspezifikation überspringen, falls es ein Array ist und es NICHT der Parameter mit der Lfdn 1 ist.
                    if (!String.IsNullOrEmpty(impSpezEingabeparameter.IsArray))
                    {
                        if (impSpezEingabeparameter.IsArray.ToLower().Equals("true") && parameterLfdn > 1) { continue; }
                    }

                    // Prüfen, ob die Spalte existiert und der Wert nicht null ist
                    if (dataTableImportdatei.Columns.Contains(impSpezEingabeparameter.Datenspaltenname ?? string.Empty) &&
                        dataRowImportdatei[(impSpezEingabeparameter.Datenspaltenname ?? string.Empty)] != DBNull.Value)
                    {

                        // Parameternamen 
                        parameterName = impSpezEingabeparameter.Name ?? string.Empty;

                        // Parameterart
                        switch (prozessaufrufParameter?.Richtung.ToUpper())
                        {
                            case "IN":
                                parameterArt = BWSPS.EnumParameterArt.PsIn;
                                break;

                            case "OUT":
                                parameterArt = BWSPS.EnumParameterArt.PsOut;
                                break;

                            default:
                                parameterArt = BWSPS.EnumParameterArt.PsIn; // Bei Massenänderung in der Regel nur IN-Parameter
                                break;
                        }

                        // Parametertyp
                        int pTypParse = 1; // default String
                        if (int.TryParse(prozessaufrufParameter?.Typ, out pTypParse))
                        {
                            parameterTyp = (BWSPS.EnumParameterTyp)pTypParse;
                        }
                        else
                        {
                            { parameterTyp = BWSPS.EnumParameterTyp.PsString; }
                        }

                        // Parameterlaenge
                        int pLaengeParse = 0; // default 0
                        if (int.TryParse(prozessaufrufParameter?.Laenge, out pLaengeParse))
                        {
                            parameterLaenge = pLaengeParse;
                        }
                        else
                        {
                            parameterLaenge = 1;
                        }

                        // Groesse EingabeParameterWertArrays bestimmen und setzen
                        // Hier als String - Array, wird vor dem Setzen in ein PMS-Input-Array mit den korrekten inneren Datentypen konvertiert
                        if (String.Equals(impSpezEingabeparameter.IsArray, "true", StringComparison.OrdinalIgnoreCase) && parameterLfdn == 1)
                        {
                            parameterWert = new string[eingabekardinalitaet];
                        }
                        else
                        {
                            parameterWert = new string[parameterLfdn];
                        }

                        // Ersten Wert im Parameter-Wert-Array fuellen
                        parameterWert[parameterLfdn - 1] = dataRowImportdatei[(impSpezEingabeparameter.Datenspaltenname ?? string.Empty)].ToString() ?? string.Empty;

                        if (!String.IsNullOrEmpty(impSpezEingabeparameter.IsArray))
                        {

                            // Wenn erster Array-Parameter, dann fuellen wir nachfolgend nun das ganze Werte-Array
                            if (String.Equals(impSpezEingabeparameter.IsArray, "true", StringComparison.OrdinalIgnoreCase) && parameterLfdn == 1)
                            {

                                // Das Parameter-Wert-Array hier nun mit den weiteren Werten befuellen
                                // Dazu eine Schleife über alle weiteren Eingabeparamter mit dem gleichen Namen und damit das Werte-Array auffuellen
                                foreach (ImpSpezXMLClasses.Eingabeparameter impSpezEingabeparameterWeiterer in impSpezUebersicht.Importspezifikation.Prozess.EingabeParameterCollection.Eingabeparameter.Where(pWeitere => pWeitere != null && String.Equals(pWeitere.Name, impSpezEingabeparameter.Name, StringComparison.OrdinalIgnoreCase) && pWeitere.Lfdn != "1"))
                                {

                                    int parameterLfdnWeiterer;
                                    int parseResult_parameterLfdnWeiterer;
                                    if (int.TryParse(impSpezEingabeparameterWeiterer.Lfdn, out parseResult_parameterLfdnWeiterer))
                                    {

                                        // Ldfn des Eingabeparameters aus der Importspezifikation
                                        parameterLfdnWeiterer = parseResult_parameterLfdnWeiterer;

                                        // Prüfen, ob die Spalte existiert und der Wert nicht null ist
                                        if (dataTableImportdatei.Columns.Contains((impSpezEingabeparameterWeiterer.Datenspaltenname ?? string.Empty)) &&
                                            dataRowImportdatei[(impSpezEingabeparameterWeiterer.Datenspaltenname ?? string.Empty)] != DBNull.Value)
                                        {

                                            // weiteren Einzelwert nun an die "richtige" Stelle im Parameterarray hinzufuegen
                                            parameterWert[parameterLfdnWeiterer - 1] = dataRowImportdatei[(impSpezEingabeparameterWeiterer.Datenspaltenname ?? string.Empty)].ToString() ?? string.Empty;

                                        }

                                    }

                                }

                            }

                        }

                        // Hier nun die einmalige Konvertierung des gesamten parameterWert-Arrays in das PMS-Input-Array mit den korrekten inneren Datentypen
                        object[] objPMSParameterWertArray = Global.ConvertToPMSInputArray(parameterWert, parameterTyp);

                        if (firstRun)
                        {

                            // Neuen Aufrufparameter hinzufuegen / Parameter immer in Grossbuchstaben
                            if (parameterWert.Length == 1)
                            {
                                // aus irgendwelchen Gründen funktioniert das Hinzufügen eines einzelnen Wertes als Array nicht, wenn es sich nicht um einen String handelt
                                psProzess.ParameterHinzufuegen(parameterName.ToUpper(), parameterArt, parameterTyp, parameterLaenge, parameterWert.Length, objPMSParameterWertArray[0]);
                            }
                            else
                            {
                                psProzess.ParameterHinzufuegen(parameterName.ToUpper(), parameterArt, parameterTyp, parameterLaenge, parameterWert.Length, objPMSParameterWertArray);
                            }

                        }
                        else
                        {

                            // Aufrufparameter aendern (weitere Datensaetze) / Parameter immer in Grossbuchstaben
                            if (parameterWert.Length == 1)
                            {
                                // aus irgendwelchen Gründen funktioniert das Hinzufügen eines einzelnen Wertes als Array nicht, wenn es sich nicht um einen String handelt
                                psProzess.ParameterÄndern(parameterName.ToUpper(), parameterArt, parameterTyp, parameterLaenge, parameterWert.Length, objPMSParameterWertArray[0]);
                            } else
                            {
                                psProzess.ParameterÄndern(parameterName.ToUpper(), parameterArt, parameterTyp, parameterLaenge, parameterWert.Length, objPMSParameterWertArray);
                            }
                                
                        }

                    }

                }

                //
                // Prozess aktivieren (aufrufen/ausführen)
                //
                BWSPS.PS_Result psResult = psProzess.Aktiviere();

                //
                // Ergebnis der Hosteingabe auswerten und in die DataTable schreiben
                //
                Global.HosteingabeDokumentieren(ref dataRowImportdatei, psResult, iRowIndex + 1);

                //
                // Das SessionToken läuft um 24:00 Uhr ab, daher hier prüfen ob das Tagesende erreicht ist.
                //
                if (DateTime.Now.Hour == 23 && DateTime.Now.Minute >= 45)
                {
                    Log("Verarbeitung durch erreichen des Tagesendes (Ablauf SessionToken) beendet", impSpezUebersicht.Importspezifikation.Prozess.Logdatei);
                    pmsHelper.Logoff();
                    break;
                }

                //
                // Zwischenprotokollierung alle 10 Datensätze
                //
                if ((iRowIndex > 0 && iRowIndex % 10 == 0))
                {
                    Log("Verarbeitete Datensätze: " + (iRowIndex).ToString(), impSpezUebersicht.Importspezifikation.Prozess.Logdatei);

                    //
                    // Importdatei mit den Ergebnissen der Hosteingabe aktualisieren
                    //
                    Global.SchreibeHosteingabeCsv(dataTableImportdatei, impSpezUebersicht);
                }

                //
                // Zeitverzögerung einbauen (Throttling)
                //
                System.Threading.Thread.Sleep(delay ?? Helper.PMS.GetPMSWaitTime());

                //
                // Testausfuehrung?
                //
                if (testlauf)
                {
                    Log("Testlauf-Flag gesetzt - Verarbeitung von nur einem Datensatz ausgefuehrt", impSpezUebersicht.Importspezifikation.Prozess.Logdatei);
                    Log("Alle PMS-Meldungen:", impSpezUebersicht.Importspezifikation.Prozess.Logdatei);
                    for (short i = 0; i < psResult.LogAnzahl(); i++)
                    {
                        Log(psResult.LogCode(i).ToString() + " " + psResult.LogText(i).ToString(), impSpezUebersicht.Importspezifikation.Prozess.Logdatei);
                    }
                    break;
                }

                firstRun = false;

            }

            //
            // Importdatei mit den Ergebnissen der Hosteingabe final schreiben
            //
            Global.SchreibeHosteingabeCsv(dataTableImportdatei, impSpezUebersicht);

            //
            // Verarbeitung beenden protokollieren
            //
            Log("Verarbeitung beendet", impSpezUebersicht.Importspezifikation.Prozess.Logdatei);

            //
            // Logoff
            //
            pmsHelper.Logoff();

            return true;

        }

        private void Log(string msg, string logfile) => logging.Add(msg, logfile);

        private void LogError(string msg, string logfile) => logging.Add("ERROR: " + msg, logfile);

        private bool PruefeImportspezifikation(ImpSpezXMLClasses.Uebersicht uebersicht)
        {
            if (Helper.ImpSpezXml.PO.IsImportspezifikationValid(uebersicht)) return true;

            LogError("Importspezifikation ungueltig.", myConfiguration.Global_AppLogfile);
            LogError("Moegliche Fehler:", myConfiguration.Global_AppLogfile);
            LogError(" - verpflichtende Parameter fehlen", myConfiguration.Global_AppLogfile);
            LogError(" - ungueltige Eingabeparameter", myConfiguration.Global_AppLogfile);
            LogError(" - nicht alle Felder vorhanden", myConfiguration.Global_AppLogfile);

            return false;
        }

        private System.Data.DataTable? LeseImportdatei(ImpSpezXMLClasses.Uebersicht uebersicht)
        {
            var datatable = Helper.ImpSpezXml.Global.ReadAndConvertCsvToDataTable(uebersicht);
            if (datatable.Rows.Count < 1)
            {
                LogError("Konnte Importdatei nicht lesen oder Importdatei ist leer.", uebersicht.Importspezifikation?.Prozess?.Logdatei!);
                return null;
            }
            return datatable;
        }

        private void ProtokolliereAuftragskopf(ImpSpezXMLClasses.Uebersicht impSpezUebersicht, System.Data.DataTable dataTable)
        {

            string logfile = impSpezUebersicht.Importspezifikation?.Prozess?.Logdatei!;

            Log("", logfile);
            Log("*** Technisches Protokoll OSPlus Massenänderung ***", logfile);
            Log("", logfile);
            Log("Auftragsdaten", logfile);
            Log("-------------", logfile);
            Log("Importspezifikation: " + impSpezUebersicht.Importspezifikation?.Beschreibung + " (Version: " + impSpezUebersicht.Importspezifikation?.Version + ")", logfile);
            Log("Importdatei        : " + impSpezUebersicht.Importspezifikation?.Importdatei, logfile);
            Log("Anzahl Datensätze  : " + (dataTable.Rows.Count).ToString(), logfile);
            Log("Spaltentrennzeichen: " + impSpezUebersicht.Importspezifikation?.Trennzeichen, logfile);
            Log("Textqualifizierer  : " + impSpezUebersicht.Importspezifikation?.Textqualifizierer, logfile);
            Log("Eingabe-Prozess    : " + impSpezUebersicht.Importspezifikation?.Prozess?.Name, logfile);
            Log("Aufrufvariante(ARV): " + impSpezUebersicht.Importspezifikation?.Prozess?.AufrufvariantenNummer, logfile);
            Log("Eingabe-Logdatei   : " + logfile, logfile);
            Log("PMS-API-Throttling : " + Helper.PMS.GetPMSWaitTime() + "ms", logfile);

        }

        private Helper.PMS? StartePMSSession()
        {
            var pmsHelper = new Helper.PMS { 
                DynSKontext = myConfiguration.DynSKontext
            };
            pmsHelper.DynSKontext.PS_SESSION_INDEX += DateTime.Now.ToString("HH:mm:ss:fffff");

            if (!pmsHelper.Login())
            {
                LogError("Der PMS-Login konnte nicht durchgeführt werden", myConfiguration.Global_AppLogfile);
                return null;
            }

            return pmsHelper;
        }

        private BWSPS.PS_Prozess ErzeugePMSProzess(Helper.PMS pmsHelper, ImpSpezXMLClasses.Uebersicht uebersicht)
        {
            var psProzess = pmsHelper.Session.NeuerProzess((uebersicht.Importspezifikation?.Prozess?.Name ?? string.Empty).ToUpper());
            psProzess.LogSize = 20;
            return psProzess;
        }
    }

    internal class OO
    {

        private Helper.Logging logging { get; set; }

        private MyConfiguration myConfiguration { get; set; }

        private fiServiceXmlClasses.OO.ProzessUebersicht prozessUebersicht { get; set; }

        internal OO()
        {
            
            logging = new Helper.Logging();
            myConfiguration = MyConfiguration.GetConfiguration();
            prozessUebersicht = new fiServiceXmlClasses.OO.ProzessUebersicht();

        }


        internal bool HostEingabeDurchfuehren(ImpSpezXMLClasses.Uebersicht impSpezUebersicht, bool testlauf, int? delay)
        {

            //
            // Falls keine Logdatei angegeben, dann einen Default setzen
            //
            if (String.IsNullOrEmpty(impSpezUebersicht.Importspezifikation?.Prozess?.Logdatei))
            {
                impSpezUebersicht.Importspezifikation!.Prozess!.Logdatei = "PMSProzessEingabeLog.log";
            }

            //
            // Vorabchecks der Importspezifikation
            //
            if (!PruefeImportspezifikation(impSpezUebersicht)) return false;

            //
            // Vorabcheck auf noch nicht implementierte Funktionen
            //
            if (!PruefeAufNochNichtImplementierteFunktionen(impSpezUebersicht)) return false;

            //
            // Import-Datei lesen 
            //
            System.Data.DataTable? dataTableImportdatei = LeseImportdatei(impSpezUebersicht);
            if (dataTableImportdatei == null || dataTableImportdatei.Rows.Count < 1) return false;

            //
            // Protokollierung der Auftragsdaten
            //
            ProtokolliereAuftragskopf(impSpezUebersicht, dataTableImportdatei);

            //
            // Importdatei um techn. Spalten fuer die Hosteingabe erweitern
            //
            if (!Helper.ImpSpezXml.Global.CheckAndExpandCsvFilePMSColumns(impSpezUebersicht))
            {
                LogError("Fehler bei der Erweiterung der Eingabedatei um die techn. PMS-Felder", impSpezUebersicht.Importspezifikation.Prozess.Logdatei);
                return false;
            }
            if (!dataTableImportdatei.Columns.Contains("HOSTEINGABE_ID"))
            {
                // Falls die Spalten nicht vorhanden waren, jetzt nochmal neu einlesen
                dataTableImportdatei = Helper.ImpSpezXml.Global.ReadAndConvertCsvToDataTable(impSpezUebersicht);
            }

            //
            // Prozess-Uebersicht fuer den Eingabe-Prozess ermitteln
            //
            prozessUebersicht = Helper.fiServiceXml.OO.GetProzessUebersicht(impSpezUebersicht.Importspezifikation.Prozess.Name ?? string.Empty);
            if (prozessUebersicht == null || prozessUebersicht.TopEntitaet == null || prozessUebersicht.TopEntitaet.Prozess.Name == null) 
            {
                LogError("Konnte Prozessdaten aus " + (impSpezUebersicht.Importspezifikation.Prozess.Name ?? string.Empty) + ".xml nicht ermitteln.", impSpezUebersicht.Importspezifikation.Prozess.Logdatei);
                return false; 
            }

            //
            // Den Service-Operator zum Prozessaufruf ermitteln
            //
            fiServiceXmlClasses.OO.Operation fiServiceOperation = Helper.fiServiceXml.OO.GetServiceOperation((impSpezUebersicht.Importspezifikation.Prozess.Name ?? string.Empty), (impSpezUebersicht.Importspezifikation.Prozess.ServiceOperation ?? string.Empty));
            if (fiServiceOperation.Name == null)
            {
                LogError("Konnte Service-Operation (OP) zum Prozess nicht ermitteln.", impSpezUebersicht.Importspezifikation.Prozess.Logdatei);
                return false;
            }

            //
            // Namen des Fachobjekts (Service-Namen) ermitteln, daran hängt, ob es auch noch Unterfachobjekte gibt
            // Auch die Eingabekardinalität hängt davon ab. Wenn das implementiert werden soll, dann müsste an dieser Stelle wohl ein Array von Unterfachobjekten angelegt und befüllt werden.
            // Keine Ahnung, ob man das wirklich braucht. Daher lassen wir das erstmal sein.
            //
            fiServiceXmlClasses.OO.FachObjekt fiServiceFachobjekt = Helper.fiServiceXml.OO.GetFachObjektEingabeByServiceOperator((impSpezUebersicht.Importspezifikation.Prozess.Name ?? string.Empty), (impSpezUebersicht.Importspezifikation.Prozess.ServiceOperation ?? string.Empty));
            if (fiServiceFachobjekt.Name == null)
            {
                LogError("Konnte Eingabe-Fachobjekt (FO) zur Operation (OP) nicht ermitteln.", impSpezUebersicht.Importspezifikation.Prozess.Logdatei);
                return false;
            }

            //
            // PMS Login durchfuehren
            //
            var pmsHelper = StartePMSSession();
            if (pmsHelper == null) return false;

            //
            // PMS Session erzeugen
            //
            BWSPS.PS_Session psSession = pmsHelper.Session; // Session-Objekt aus dem Login-Helper holen
            if (psSession == null) return false;

            //
            // Hosteingabe starten protokollieren
            //
            Log("Verarbeitung gestartet", impSpezUebersicht.Importspezifikation.Prozess.Logdatei);

            // 
            // Eingabekardinalität ermitteln
            //
            int eingabekardinalitaet;
            if (!int.TryParse(impSpezUebersicht.Importspezifikation.Prozess.EingabeParameterCollection?.EingabeKardinalitaet, out eingabekardinalitaet))
            {
                eingabekardinalitaet = 1; // default
                Log("EingabeParameterCollectionKardinalitaet konnte nicht geparst werden, Default=1 gesetzt", impSpezUebersicht.Importspezifikation.Prozess.Logdatei);
            }
      
            //
            // Schleife ueber alle Datensaetze der Importdatei
            // 
            for (int iRowIndex = 0; iRowIndex < dataTableImportdatei.Rows.Count; iRowIndex++)
            {

                //
                // Aktueller Datensatz aus der Importdatei
                //
                System.Data.DataRow dataRowImportdatei = dataTableImportdatei.Rows[iRowIndex];

                //
                // Uberspringen, falls der Datensatz bereits verarbeitet wurde
                // 
                if (!String.IsNullOrEmpty(dataRowImportdatei["HOSTEINGABE_STATUSCODE"].ToString())) 
                { 
                    continue;
                }
                  
                //
                // PMS Prozess erzeugen
                //
                BWSPS.PS_Prozess psProzess = ErzeugePMSProzess(pmsHelper, impSpezUebersicht);
                if (psProzess == null) return false;

                //
                // Service-Operation (OP Fachobjekt) fuer die Prozess-Aufruf hinzufuegen
                //
                BWSPS.PS_Fachobjekt prozessOperation = psProzess.FachobjektHinzufuegen(impSpezUebersicht.Importspezifikation.Prozess.ServiceOperation);

                //
                // Falls es ein Unterfachobjekt (FO) gibt, dann muss es ebenfalls erzeugt werden und die Parameter dort hinzugefügt werden
                //
                BWSPS.PS_Fachobjekt? prozessUnterFachobjekt = null;
                if (fiServiceFachobjekt.FoRefs.Count == 1)
                {
                    prozessUnterFachobjekt = prozessOperation.FachobjektHinzufuegen(fiServiceFachobjekt.FoRefs[0].Name);
                }

                //
                // Werte fuer die Parameter mit den Werten aus der Importdatei befuellen
                //
                // Falls es ein Unterfachobjekt gibt, dann muesen die Parameter hier ran, ansonsten direkt an die Operation
                // Der Eingabeparameter kann auch selbst ein Fachobjekt sein, das ist hier aber (noch) nicht implementiert. 
                //
                if (impSpezUebersicht.Importspezifikation.Prozess.EingabeParameterCollection?.Eingabeparameter == null)
                {
                    LogError("Die EingabeparameterCollection der Importspezifikation ist leer!", impSpezUebersicht.Importspezifikation.Prozess.Logdatei);
                    break;
                }

                foreach (ImpSpezXMLClasses.Eingabeparameter impSpezEingabeparameter in impSpezUebersicht.Importspezifikation.Prozess.EingabeParameterCollection.Eingabeparameter)
                {

                    // Parametername und -wert initialisieren
                    string parameterName = string.Empty;
                    string parameterWert = string.Empty;

                    // Prüfen, ob die Spalte existiert und der Wert nicht null ist
                    if (dataTableImportdatei.Columns.Contains(impSpezEingabeparameter.Datenspaltenname ?? string.Empty) &&
                        dataRowImportdatei[impSpezEingabeparameter.Datenspaltenname ?? string.Empty] != DBNull.Value)
                    {
                        parameterName = impSpezEingabeparameter.Name ?? string.Empty;
                        parameterWert = dataRowImportdatei[impSpezEingabeparameter.Datenspaltenname ?? string.Empty].ToString() ?? string.Empty;
                    }

                    // Prozess-Parameter hinzufuegen (wir gehen mal davon aus, dass es max. ein Unterfachobjekt gibt)
                    if (fiServiceFachobjekt.FoRefs.Count == 1)
                    {
                        prozessUnterFachobjekt?.ParameterHinzufuegen(parameterName, parameterWert);
                    }
                    else
                    {
                        prozessOperation.ParameterHinzufuegen(parameterName, parameterWert);
                    }
                };
                
                //
                // Prozess aktivieren (aufrufen/ausführen)
                //
                BWSPS.PS_Result psResult = psProzess.Aktiviere();

                //
                // Ergebnis der Hosteingabe auswerten und in die DataTable schreiben
                //
                Global.HosteingabeDokumentieren(ref dataRowImportdatei, psResult, iRowIndex + 1);
                
                //
                // Das SessionToken läuft um 24:00 Uhr ab, daher hier prüfen ob das Tagesende erreicht ist.
                //
                if (DateTime.Now.Hour == 23 && DateTime.Now.Minute >= 45)
                {
                    Log("Verarbeitung durch erreichen des Tagesendes (Ablauf SessionToken) beendet", impSpezUebersicht.Importspezifikation.Prozess.Logdatei);
                    pmsHelper.Logoff();
                    break;

                }

                //
                // Zwischenprotokollierung alle 10 Datensätze
                //
                if ((iRowIndex > 0 && iRowIndex % 10 == 0))
                {
                    Log("Verarbeitete Datensätze: " + (iRowIndex).ToString(), impSpezUebersicht.Importspezifikation.Prozess.Logdatei);

                    //
                    // Importdatei mit den Ergebnissen der Hosteingabe aktualisieren
                    //
                    Global.SchreibeHosteingabeCsv(dataTableImportdatei, impSpezUebersicht);
                }

                //
                // Zeitverzögerung einbauen (Throttling)
                //
                System.Threading.Thread.Sleep(delay ?? Helper.PMS.GetPMSWaitTime());

                //
                // Testausfuehrung?
                //
                if (testlauf)
                {
                    Log("Testlauf-Flag gesetzt - Verarbeitung von nur einem Datensatz ausgefuehrt", impSpezUebersicht.Importspezifikation.Prozess.Logdatei);
                    Log("Alle PMS-Meldungen:", impSpezUebersicht.Importspezifikation.Prozess.Logdatei);
                    for (short i = 0; i < psResult.LogAnzahl(); i++)
                    {
                        Log(psResult.LogCode(i).ToString() + " " + psResult.LogText(i).ToString(), impSpezUebersicht.Importspezifikation.Prozess.Logdatei);
                    }
                    break;
                }

            }

            //
            // Importdatei mit den Ergebnissen der Hosteingabe final schreiben
            //
            Global.SchreibeHosteingabeCsv(dataTableImportdatei, impSpezUebersicht);

            //
            // Verarbeitung beenden protokollieren
            //
            Log("Verarbeitung beendet", impSpezUebersicht.Importspezifikation.Prozess.Logdatei);

            //
            // Logoff
            //
            pmsHelper.Logoff();

            return true;

        }
        
        private void Log(string msg, string logfile) => logging.Add(msg, logfile);
        
        private void LogError(string msg, string logfile) => logging.Add("ERROR: " + msg, logfile);
        
        private bool PruefeImportspezifikation(ImpSpezXMLClasses.Uebersicht uebersicht)
        {
            if (Helper.ImpSpezXml.OO.IsImportspezifikationValid(uebersicht)) return true;

            LogError("Importspezifikation ungueltig.", myConfiguration.Global_AppLogfile);
            LogError("Moegliche Fehler:", myConfiguration.Global_AppLogfile);
            LogError(" - verpflichtende Parameter fuer das Eingabefachobjekt fehlen", myConfiguration.Global_AppLogfile);
            LogError(" - ungueltige Eingabeparameter fuer das Eingabefachobjekt", myConfiguration.Global_AppLogfile);
            LogError(" - nicht alle Eingabefelder in der Importdatei vorhanden", myConfiguration.Global_AppLogfile);

            return false;
        }

        private bool PruefeAufNochNichtImplementierteFunktionen(ImpSpezXMLClasses.Uebersicht uebersicht) 
        {

            // 
            // Eingabekardinalität > 1 wird (derzeit) nicht unterstützt
            //
            int eingabekardinalitaet;
            int.TryParse(uebersicht.Importspezifikation?.Prozess?.EingabeParameterCollection?.EingabeKardinalitaet, out eingabekardinalitaet);
            if (eingabekardinalitaet > 1)
            {
                LogError("Eine EingabeParameterCollectionKardinalitaet > 1 wird derzeit bei objektorientieren Prozessen nicht unterstützt!", uebersicht.Importspezifikation?.Prozess?.Logdatei!);
                return false;
            }

            //
            // Eingabeparameter als Fachobjekt wird (noch) nicht unterstützt    
            //
            foreach (ImpSpezXMLClasses.Eingabeparameter impSpezEingabeparameter in uebersicht.Importspezifikation?.Prozess?.EingabeParameterCollection?.Eingabeparameter!)
            {
                if (!String.IsNullOrEmpty(impSpezEingabeparameter.EingabeFo))
                {
                    LogError("Eingabeparameter " + (impSpezEingabeparameter.Name ?? string.Empty) + " als Eingabefachobjekt (" + impSpezEingabeparameter.EingabeFo + ") wird derzeit leider (noch) nicht unterstützt!", uebersicht.Importspezifikation?.Prozess?.Logdatei!);
                    return false;
                }
            }

            //
            // Eingabeparameter als Array ist bei OO-Prozessen nicht erlaubt
            //
            foreach (ImpSpezXMLClasses.Eingabeparameter impSpezEingabeparameter in uebersicht.Importspezifikation?.Prozess?.EingabeParameterCollection?.Eingabeparameter!)
            {
                if (String.Equals(impSpezEingabeparameter.IsArray, "true", StringComparison.OrdinalIgnoreCase))
                {
                    LogError("Eingabeparameter " + (impSpezEingabeparameter.Name ?? string.Empty) + " als Array ist bei objektorientierten Prozessen nicht erlaubt.", uebersicht.Importspezifikation?.Prozess?.Logdatei!);
                    return false;
                }
            }

            return true;

        }

        private System.Data.DataTable? LeseImportdatei(ImpSpezXMLClasses.Uebersicht uebersicht)
        {
            var datatable = Helper.ImpSpezXml.Global.ReadAndConvertCsvToDataTable(uebersicht);
            if (datatable.Rows.Count < 1)
            {
                LogError("Konnte Importdatei nicht lesen oder Importdatei ist leer.", uebersicht.Importspezifikation?.Prozess?.Logdatei!);
                return null;
            }
            return datatable;
        }

        private void ProtokolliereAuftragskopf(ImpSpezXMLClasses.Uebersicht impSpezUebersicht, System.Data.DataTable dataTable)
        {

            string logfile = impSpezUebersicht.Importspezifikation?.Prozess?.Logdatei!;

            Log("", logfile);
            Log("*** Technisches Protokoll OSPlus Massenänderung ***", logfile);
            Log("", logfile);
            Log("Auftragsdaten", logfile);
            Log("-------------", logfile);
            Log("Importspezifikation: " + impSpezUebersicht.Importspezifikation?.Beschreibung + " (Version: " + impSpezUebersicht.Importspezifikation?.Version + ")", logfile);
            Log("Importdatei        : " + impSpezUebersicht.Importspezifikation?.Importdatei, logfile);
            Log("Anzahl Datensätze  : " + (dataTable.Rows.Count).ToString(), logfile);
            Log("Spaltentrennzeichen: " + impSpezUebersicht.Importspezifikation?.Trennzeichen, logfile);
            Log("Textqualifizierer  : " + impSpezUebersicht.Importspezifikation?.Textqualifizierer, logfile);
            Log("Eingabe-Prozess    : " + impSpezUebersicht.Importspezifikation?.Prozess?.Name, logfile);
            Log("Eingabe-Operator   : " + impSpezUebersicht.Importspezifikation?.Prozess?.ServiceOperation, logfile);
            Log("Eingabe-Logdatei   : " + logfile, logfile);
            Log("PMS-API-Throttling : " + Helper.PMS.GetPMSWaitTime() + "ms", logfile);

        }

        private Helper.PMS? StartePMSSession()
        {
            var pmsHelper = new Helper.PMS
            {
                DynSKontext = myConfiguration.DynSKontext
            };
            pmsHelper.DynSKontext.PS_SESSION_INDEX += DateTime.Now.ToString("HH:mm:ss:fffff");

            if (!pmsHelper.Login())
            {
                LogError("Der PMS-Login konnte nicht durchgeführt werden", myConfiguration.Global_AppLogfile);
                return null;
            }

            return pmsHelper;
        }

        private BWSPS.PS_Prozess ErzeugePMSProzess(Helper.PMS pmsHelper, ImpSpezXMLClasses.Uebersicht uebersicht)
        {
            var psProzess = pmsHelper.Session.NeuerProzess((uebersicht.Importspezifikation?.Prozess?.Name ?? string.Empty).ToUpper());
            psProzess.LogSize = 20;
            return psProzess;
        }
    }

    internal partial class Global
    {

        internal static void HosteingabeDokumentieren(ref System.Data.DataRow dataRow, BWSPS.PS_Result psResult, int rowIndex)
        {

            dataRow["HOSTEINGABE_ID"] = rowIndex;
            dataRow["HOSTEINGABE_DATUM"] = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            dataRow["HOSTEINGABE_STATUSCODE"] = (int)psResult.Status();
            dataRow["HOSTEINGABE_MELDUNG"] = psResult.LogCode(0).ToString() + " " + psResult.LogText(0).ToString();

        }

        internal static void SchreibeHosteingabeCsv(System.Data.DataTable table, ImpSpezXMLClasses.Uebersicht uebersicht)
        {
            Helper.ImpSpezXml.Global.WriteAndConvertDataTableToCsv(table,
                                                                   uebersicht.Importspezifikation?.Importdatei ?? string.Empty,
                                                                   uebersicht.Importspezifikation?.Trennzeichen ?? string.Empty,
                                                                   uebersicht.Importspezifikation?.Textqualifizierer ?? string.Empty);
        }


         internal static Object[] ConvertToPMSInputArray(Array sourceArray, BWSPS.EnumParameterTyp targetBWSPSType)
        {
            if (sourceArray == null)
                throw new ArgumentNullException(nameof(sourceArray));

            // Rückgabeobjekt basierend auf dem Ziel-BWSPS-Typ erstellen und mit konvertierten Werten füllen
            object[] result = new object[sourceArray.Length];

            switch (targetBWSPSType)
            {
                case BWSPS.EnumParameterTyp.PsString:
                case BWSPS.EnumParameterTyp.PsVarChar2:
                    for (int i = 0; i < sourceArray.Length; i++)
                    {
                        result[i] = sourceArray.GetValue(i)?.ToString() ?? string.Empty;
                    }
                    break;
                case BWSPS.EnumParameterTyp.PsSignedShort:
                case BWSPS.EnumParameterTyp.PsUnsignedShort:
                    for (int i = 0; i < sourceArray.Length; i++)
                    {
                        result[i] = Convert.ToInt16(sourceArray.GetValue(i));
                    }
                    break;
                case BWSPS.EnumParameterTyp.PsSignedLong:
                case BWSPS.EnumParameterTyp.PsUnsignedLong:
                    for (int i = 0; i < sourceArray.Length; i++)
                    {
                        result[i] = Convert.ToInt64(sourceArray.GetValue(i));
                    }
                    break;
                case BWSPS.EnumParameterTyp.PsSignedInt:
                case BWSPS.EnumParameterTyp.PsUnsignedInt:
                    for (int i = 0; i < sourceArray.Length; i++)
                    {
                        result[i] = Convert.ToInt32(sourceArray.GetValue(i));
                    }
                    break;
                case BWSPS.EnumParameterTyp.PsDouble:
                    for (int i = 0; i < sourceArray.Length; i++)
                    {
                        result[i] = Convert.ToDouble(sourceArray.GetValue(i));
                    }
                    break;
                case BWSPS.EnumParameterTyp.PsChar:
                    for (int i = 0; i < sourceArray.Length; i++)
                    {
                        string strValue = sourceArray.GetValue(i)?.ToString() ?? string.Empty;
                        if (strValue.Length >= 1)
                        {
                            result[i] = strValue.Substring(0, 1); // Erstes Zeichen nehmen
                        }
                    }
                    break;
                case BWSPS.EnumParameterTyp.PsFloat:
                    for (int i = 0; i < sourceArray.Length; i++)
                    {
                        result[i] = Convert.ToSingle(sourceArray.GetValue(i));
                    }
                    break;
                default:
                    throw new ArgumentException("Unsupported target BWSPS type: " + targetBWSPSType);
            }

            return result;

        }

    }
}