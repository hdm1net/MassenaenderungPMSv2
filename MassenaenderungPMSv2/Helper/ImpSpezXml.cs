using MassenaenderungPMSv2.ImpSpezXMLClasses;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MassenaenderungPMSv2.Helper.ImpSpezXml
{

    internal class Global
    {

        /// <summary>
        /// Deserialisiert die XML-Datei zur Importspezifikation und gibt diese als Uebersichtklasse zurueck
        /// </summary>
        /// <param name="ImpSpezXmlFilePath"></param>
        /// <returns>ImpSpezXMLClasses.OO.Uebersicht</returns>
        /// <exception cref="Exception"></exception>
        internal static ImpSpezXMLClasses.Uebersicht GetImpSpezUebersicht(string ImpSpezXmlFilePath)
        {

            return GetImpSpezUebersichtAsync(ImpSpezXmlFilePath).Result;

        }


        /// <summary>
        /// Deserialisiert die XML-Datei zur Importspezifikation und gibt diese als Uebersichtklasse zurueck
        /// </summary>
        /// <param name="ImpSpezXmlFilePath"></param>
        /// <returns>ImpSpezXMLClasses.OO.Uebersicht</returns>
        /// <exception cref="Exception"></exception>
        internal static async Task<ImpSpezXMLClasses.Uebersicht> GetImpSpezUebersichtAsync(string ImpSpezXmlFilePath)
        {
            try
            {
                // Return-Object
                ImpSpezXMLClasses.Uebersicht rcUebersicht = new ImpSpezXMLClasses.Uebersicht();

                await Task.Run(action: () =>
                {

                    if (File.Exists(ImpSpezXmlFilePath))
                    {

                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImpSpezXMLClasses.Uebersicht));

                        using (TextReader textReader = new StreamReader(ImpSpezXmlFilePath, Encoding.GetEncoding("iso-8859-1")))
                        {

                            var objDeserialized = xmlSerializer.Deserialize(textReader);

                            if (objDeserialized != null)
                            {
                                rcUebersicht = (ImpSpezXMLClasses.Uebersicht)objDeserialized;
                            }

                            textReader.Close();

                        }
                    }
                });

                return rcUebersicht;

            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Error GetImpSpezUebersichtAsync()", e);
            }
        }

        /// <summary>
        /// Serialisiert ein <c>ImpSpezXMLClasses.Uebersicht</c>-Objekt in eine XML Datei.
        /// Das Ergebnis verwendet ISO 8859 1 und setzt Attribut Quotes auf einfache Anführungszeichen (').
        /// </summary>
        /// <param name="Uebersicht">Das zu serialisierende Objekt.</param>
        /// <param name="ImpSpezXmlFilePath">Zielpfad inkl. Dateiname.</param>
        internal static void WriteImpSpezUebersicht(ImpSpezXMLClasses.Uebersicht Uebersicht, string ImpSpezXmlFilePath)
        {

            WritetImpSpezUebersichtAsync(Uebersicht, ImpSpezXmlFilePath).Wait();

        }

        /// <summary>
        /// Serialisiert ein <c>ImpSpezXMLClasses.Uebersicht</c>-Objekt in eine XML Datei.
        /// Das Ergebnis verwendet ISO 8859 1 und setzt Attribut Quotes auf einfache Anführungszeichen (').
        /// </summary>
        /// <param name="Uebersicht">Das zu serialisierende Objekt.</param>
        /// <param name="ImpSpezXmlFilePath">Zielpfad inkl. Dateiname.</param>
        internal static async Task WritetImpSpezUebersichtAsync(ImpSpezXMLClasses.Uebersicht Uebersicht, string ImpSpezFileName)
        {

            if (String.IsNullOrEmpty(ImpSpezFileName)
                || Uebersicht?.Importspezifikation?.Prozess?.EingabeParameterCollection?.Eingabeparameter == null) { return; }

            try
            {
                var targetEncoding = Encoding.GetEncoding("iso-8859-1");

                var xmlSerializer = new XmlSerializer(typeof(ImpSpezXMLClasses.Uebersicht));

                var settings = new System.Xml.XmlWriterSettings
                {
                    Encoding = targetEncoding,
                    Indent = true,
                    NewLineOnAttributes = false,
                    OmitXmlDeclaration = false
                };

                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add(string.Empty, string.Empty);

                string xmlString;
                using (var ms = new MemoryStream())
                {
                    
                    using (var writer = XmlWriter.Create(ms, settings))
                    {
                        writer.WriteStartDocument(true);
                        xmlSerializer.Serialize(writer, Uebersicht, namespaces);
                        writer.Flush();
                    }
                    
                    xmlString = targetEncoding.GetString(ms.ToArray());

                }

               // Doppelte Anführungszeichen bei Attributen durch einfache ersetzen
                //string xmlWithSingleQuotes = System.Text.RegularExpressions.Regex.Replace(xmlString, "(\\s\\w+)=\\\"([^\"]*)\\\"", "$1='$2'");
                string xmlWithSingleQuotes = System.Text.RegularExpressions.Regex.Replace(xmlString, @"""", "'");
                // Falls &quote in den Attributen vorkommt, wieder in " umwandeln
                xmlWithSingleQuotes = xmlWithSingleQuotes.Replace("&quot;", "\"");

                // Ergebnis in die Datei schreiben (ISO 8859 1)
                await File.WriteAllTextAsync(ImpSpezFileName, xmlWithSingleQuotes, targetEncoding);

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error WritetImpSpezUebersichtAsync()", ex);
            }
        }

        /// <summary>
        /// Wandelt die CSV-Datei in ein DataTable mit Spaltennamen um
        /// </summary>
        /// <param name="Uebersicht"></param>
        /// <returns>System.Data.DataTable/returns>
        /// <exception cref="Exception"></exception>
        internal static System.Data.DataTable ReadAndConvertCsvToDataTable(ImpSpezXMLClasses.Uebersicht Uebersicht)
        {

            return ReadAndConvertCsvToDataTableAsync(Uebersicht).Result;

        }

        /// <summary>
        /// Wandelt die CSV-Datei in ein DataTable mit Spaltennamen um
        /// </summary>
        /// <param name="Uebersicht"></param>
        /// <returns>System.Data.DataTable/returns>
        /// <exception cref="Exception"></exception>
        internal static async Task<System.Data.DataTable> ReadAndConvertCsvToDataTableAsync(ImpSpezXMLClasses.Uebersicht Uebersicht)
        {
            return await Helper.ImpSpezXml.ImpSpezXmlFile.ReadAndConvertCsvToDataTableAsync(String.Concat(Uebersicht.Importspezifikation?.Importdatei,""),
                                                                                            String.Concat(Uebersicht.Importspezifikation?.Trennzeichen, ""),
                                                                                            String.Concat(Uebersicht.Importspezifikation?.Textqualifizierer, ""));
        }

        /// <summary>
        /// Wandelt den Header der CSV-Datei in ein DataTable mit Spaltennamen um
        /// </summary>
        /// <param name="Uebersicht"></param>
        /// <returns>System.Data.DataTable/returns>
        /// <exception cref="Exception"></exception>
        internal static System.Data.DataTable ReadAndConvertCsvToDataTableHeader(ImpSpezXMLClasses.Uebersicht Uebersicht)
        {

            return ReadAndConvertCsvToDataTableHeaderAsync(Uebersicht).Result;

        }

        /// <summary>
        /// Wandelt den Header der CSV-Datei in ein DataTable mit Spaltennamen um
        /// </summary>
        /// <param name="Uebersicht"></param>
        /// <returns>System.Data.DataTable/returns>
        /// <exception cref="Exception"></exception>
        internal static async Task<System.Data.DataTable> ReadAndConvertCsvToDataTableHeaderAsync(ImpSpezXMLClasses.Uebersicht Uebersicht)
        {
            return await Helper.ImpSpezXml.ImpSpezXmlFile.ReadAndConvertCsvToDataTableHeaderAsync(String.Concat(Uebersicht.Importspezifikation?.Importdatei, ""),
                                                                                                  String.Concat(Uebersicht.Importspezifikation?.Trennzeichen, ""),
                                                                                                  String.Concat(Uebersicht.Importspezifikation?.Textqualifizierer, ""));
        }

        /// <summary>
        /// Wandelt eine DataTable in eine CSV-Datei um
        /// </summary>
        /// <param name="DataTable"></param>
        /// <param name="Exportdatei"></param>
        /// <param name="delimiter"></param>
        /// <param name="textqualifier"></param>
        internal static void WriteAndConvertDataTableToCsv(System.Data.DataTable DataTable, string Exportdatei, string delimiter, string textqualifier)
        {
            Helper.ImpSpezXml.ImpSpezXmlFile.WriteAndConvertDataTableToCsvAsync(DataTable, Exportdatei, delimiter, textqualifier).Wait();
        }

        internal static bool CheckAndExpandCsvFilePMSColumns(ImpSpezXMLClasses.Uebersicht Uebersicht)
        {
            
            // Der Header der Importdatei in eine DataTable
            System.Data.DataTable dtCsvHeader = ReadAndConvertCsvToDataTableHeader(Uebersicht);

            if (dtCsvHeader.Columns.Contains("HOSTEINGABE_ID") && 
                dtCsvHeader.Columns.Contains("HOSTEINGABE_DATUM") &&
                dtCsvHeader.Columns.Contains("HOSTEINGABE_STATUSCODE") &&
                dtCsvHeader.Columns.Contains("HOSTEINGABE_MELDUNG")) {

                // Felder fuer die PMS-Hosteingabe sind bereits vorhanden                
                return true;

            }


            // Felder fuer die PMS-Hosteingabe sind nicht vorhanden, also die Importdatei erweitern
            System.Data.DataTable dtCsvFile = ReadAndConvertCsvToDataTable(Uebersicht);
            if (dtCsvFile == null ||dtCsvFile.Rows.Count == 0)
            {
                return false;
            }

            // Spalten an die DataTable anhaengen
            dtCsvFile.Columns.Add("HOSTEINGABE_ID", typeof(Int64));
            dtCsvFile.Columns.Add("HOSTEINGABE_DATUM", typeof(string));
            dtCsvFile.Columns.Add("HOSTEINGABE_STATUSCODE", typeof(int));
            dtCsvFile.Columns.Add("HOSTEINGABE_MELDUNG", typeof(string));

            System.Threading.Thread.Sleep(250); // Kurze Pause, damit die Datei nicht gesperrt ist

            // Neue CSV-Datei mit den erweiterten Spalten schreiben
            WriteAndConvertDataTableToCsv(dtCsvFile,
                                          String.Concat(Uebersicht.Importspezifikation?.Importdatei,""),
                                          String.Concat(Uebersicht.Importspezifikation?.Trennzeichen,""),
                                          String.Concat(Uebersicht.Importspezifikation?.Textqualifizierer,""));

            return true;

        }

        /// <summary>
        /// Schreibe eine XML-Datei zur Importspezifikation basierend auf der Uebersicht-Klasse
        /// </summary>
        /// <param name="Uebersicht"></param>
        /// <param name="ImpSpezFileName"></param>
        /// <returns>bool</returns>
        /// <exception cref="InvalidDataException"></exception>
        internal static bool WriteImportSpezifikationFile(ImpSpezXMLClasses.Uebersicht Uebersicht, string ImpSpezFilePath)
        {
            return WriteImportSpezifikationFileAsync(Uebersicht, ImpSpezFilePath).Result;
        }

        /// <summary>
        /// Schreibe eine XML-Datei zur Importspezifikation basierend auf der Uebersicht-Klasse
        /// </summary>
        /// <param name="Uebersicht"></param>
        /// <param name="ImpSpezFileName"></param>
        /// <returns>bool</returns>
        /// <exception cref="InvalidDataException"></exception>
        internal static async Task<bool> WriteImportSpezifikationFileAsync(ImpSpezXMLClasses.Uebersicht Uebersicht, string ImpSpezFileName)
        {
            try
            {

                if (String.IsNullOrEmpty(ImpSpezFileName) ||
                    Uebersicht == null ||
                    Uebersicht.Importspezifikation == null ||
                    Uebersicht.Importspezifikation.Prozess == null ||
                    Uebersicht.Importspezifikation.Prozess.EingabeParameterCollection == null ||
                    Uebersicht.Importspezifikation.Prozess.EingabeParameterCollection.Eingabeparameter == null) { return false; }


                // XML zusammenbauen
                // Importspezifikation erstellen
                string xmlFileImpSpez = String.Empty;
                xmlFileImpSpez += "<?xml version=\"1.0\" encoding=\"iso-8859-1\" standalone=\"yes\"?>" + System.Environment.NewLine;
                xmlFileImpSpez += "<Uebersicht>" + System.Environment.NewLine;
                xmlFileImpSpez += String.Format(" <Importspezifikation Version=\"{0}\"", Uebersicht.Importspezifikation?.Version) + System.Environment.NewLine;
                if (String.IsNullOrEmpty(Uebersicht.Importspezifikation?.Beschreibung))
                {
                    xmlFileImpSpez += String.Format("                      Beschreibung=\"Prozess '{0}' Aufrufvariante '{1}'\"", Uebersicht.Importspezifikation?.Prozess?.Name, Uebersicht.Importspezifikation?.Prozess?.AufrufvariantenNummer) + System.Environment.NewLine;
                }
                else
                {
                    xmlFileImpSpez += String.Format("                      Beschreibung=\"{0}\"", Uebersicht.Importspezifikation?.Beschreibung) + System.Environment.NewLine;
                }
                xmlFileImpSpez += String.Format("                      Importdatei=\"{0}\"", Uebersicht.Importspezifikation?.Importdatei) + System.Environment.NewLine;
                xmlFileImpSpez += String.Format("                      Trennzeichen=\"{0}\"", Uebersicht.Importspezifikation?.Trennzeichen) + System.Environment.NewLine;
                xmlFileImpSpez += String.Format("                      Textqualifizierer='{0}'>", Uebersicht.Importspezifikation?.Textqualifizierer) + System.Environment.NewLine;
                xmlFileImpSpez += String.Format("  <Prozess name=\"{0}\"", Uebersicht.Importspezifikation?.Prozess?.Name?.ToUpper()) + System.Environment.NewLine;
                xmlFileImpSpez += String.Format("           schnittstellen_art=\"{0}\"", Uebersicht.Importspezifikation?.Prozess?.SchnittstellenArt) + System.Environment.NewLine;
                xmlFileImpSpez += String.Format("           service_operation=\"{0}\"", Uebersicht.Importspezifikation?.Prozess?.ServiceOperation) + System.Environment.NewLine;
                xmlFileImpSpez += String.Format("           aufrufvariantennummer=\"{0}\"", Uebersicht.Importspezifikation?.Prozess?.AufrufvariantenNummer) + System.Environment.NewLine;
                xmlFileImpSpez += String.Format("           logdatei=\"{0}\">", Uebersicht.Importspezifikation?.Prozess?.Logdatei) + System.Environment.NewLine;
                xmlFileImpSpez += String.Format("   <EingabeParameterCollection eingabekardinalität=\"{0}\">", Uebersicht.Importspezifikation?.Prozess?.EingabeParameterCollection?.EingabeKardinalitaet) + System.Environment.NewLine;

                foreach (var eingabeparameter in Uebersicht.Importspezifikation?.Prozess?.EingabeParameterCollection?.Eingabeparameter ?? Enumerable.Empty<ImpSpezXMLClasses.Eingabeparameter>())
                {
                    xmlFileImpSpez += String.Format("    <Eingabeparameter Name=\"{0}\" eingabe_fo=\"{1}\" lfdn=\"{2}\" isarray=\"{3}\">", eingabeparameter.Name, eingabeparameter.EingabeFo, eingabeparameter.Lfdn, eingabeparameter.IsArray) + eingabeparameter.Datenspaltenname + "</Eingabeparameter>" + System.Environment.NewLine;
                }

                xmlFileImpSpez += "   </EingabeParameterCollection>" + System.Environment.NewLine;
                xmlFileImpSpez += "  </Prozess>" + System.Environment.NewLine;
                xmlFileImpSpez += " </Importspezifikation>" + System.Environment.NewLine;
                xmlFileImpSpez += "</Uebersicht>" + System.Environment.NewLine;

                // Datei schreiben(synchron, aber ohne Task.Run)
                await File.WriteAllTextAsync(ImpSpezFileName, xmlFileImpSpez, Encoding.GetEncoding("iso-8859-1")).ConfigureAwait(false);

                return File.Exists(ImpSpezFileName);

            }
            catch (Exception e)
            {
                throw new InvalidDataException(e.ToString(), e);
            }
        }

    }


    /// <summary>
    /// Klasse fuer Importspezifikationsdatei fuer parameterorientierte Prozesse
    /// </summary>
    internal class PO
    {

        /// <summary>
        /// Prueft, ob die Importspezifikation gueltig ist und verwendet werden kann
        /// </summary>
        /// <param name="Uebersicht"></param>
        /// <returns>bool</returns>
        internal static bool IsImportspezifikationValid(ImpSpezXMLClasses.Uebersicht Uebersicht)
        {
            return IsImportspezifikationValidAsync(Uebersicht).Result;
        }


        /// <summary>
        /// Prueft, ob die Importspezifikation gueltig ist und verwendet werden kann
        /// </summary>
        /// <param name="Uebersicht"></param>
        /// <returns>bool</returns>
        internal static async Task<bool> IsImportspezifikationValidAsync(ImpSpezXMLClasses.Uebersicht Uebersicht)
        {
            try
            {
                bool isValid = true;

                await Task.Run(action: () =>
                {

                    if (!String.Equals(Uebersicht.Importspezifikation?.Prozess?.SchnittstellenArt, "PO", StringComparison.OrdinalIgnoreCase)) { 
                    
                        isValid = false;

                    }

                    // Die Aufrufvariante ermitteln, hier brauchen wir die Parameter-Referenzen fuer die Ermittlung der Eingabeparameter
                    fiServiceXmlClasses.PO.AufrufVariante av = Helper.fiServiceXml.PO.GetAufrufvariante(String.Concat(Uebersicht.Importspezifikation?.Prozess?.Name,""),
                                                                                                        String.Concat(Uebersicht.Importspezifikation?.Prozess?.AufrufvariantenNummer,""));

                    // Der Header der Importdatei in eine DataTable
                    System.Data.DataTable dtImportFileHeader = Helper.ImpSpezXml.Global.ReadAndConvertCsvToDataTableHeader(Uebersicht);


                    // 1.) Pruefen, ob alle verpflichtenden Parameter fuer die Aufrufvariante in der Importspezifikation vorhanden sind
                    if (av != null)
                    {
                        
                        foreach (fiServiceXmlClasses.PO.VarianteParameter avPARAMETER in av.Parameter.Where(i => string.Equals(i.OptionalKz, "M", StringComparison.OrdinalIgnoreCase)))
                        {
                            
                            bool parameterFound = false; // wird true, wenn der Parameter gefunden wurde 

                            // Parameter in der Importspezifikation suchen
                            foreach (ImpSpezXMLClasses.Eingabeparameter impspezparameter in (Uebersicht.Importspezifikation?.Prozess?.EingabeParameterCollection?.Eingabeparameter) ?? Enumerable.Empty<ImpSpezXMLClasses.Eingabeparameter>())
                            {
                                if (String.Equals(avPARAMETER.Name, impspezparameter.Name, StringComparison.OrdinalIgnoreCase)) { parameterFound = true; break; }
                            }

                            // Pruefen, ob der Parameter gefunden wurde, wenn nicht ist die Pruefung hier schon fehlgeschlagen
                            if (!parameterFound)
                            {
                                isValid = false;
                                break;
                            }

                        }
                    }
                    else {

                        isValid = false;

                    }


                    // 2.) Pruefen, ob alle in der Importspezifikation enthaltenen Parameter auch gueltige Eingabeparameter sind
                    if (av != null) {
                        
                        foreach (ImpSpezXMLClasses.Eingabeparameter impspezparameter in (Uebersicht.Importspezifikation?.Prozess?.EingabeParameterCollection?.Eingabeparameter) ?? Enumerable.Empty<ImpSpezXMLClasses.Eingabeparameter>())
                        {

                            bool parameterFound = false; // wird true, wenn der Parameter gefunden wurde

                            // Parameter im Eingabefachobjekt suchen
                            foreach (var avParameter in av.Parameter)
                            {
                                if (String.Equals(impspezparameter.Name, avParameter.Name, StringComparison.OrdinalIgnoreCase)) { parameterFound = true; break; }
                            }

                            // Pruefen, ob der Parameter gefunden wurde, wenn nicht ist die Pruefung hier schon fehlgeschlagen
                            if (!parameterFound)
                            {
                                isValid = false;
                                break;
                            }

                        }
                    } else
                    {

                        isValid = false;

                    }


                    // 3.) Pruefen, ob alle Felder (Spalten) der Importspezifikation auch in der Importdatei vorhanden sind
                    foreach (ImpSpezXMLClasses.Eingabeparameter impspezparameter in (Uebersicht.Importspezifikation?.Prozess?.EingabeParameterCollection?.Eingabeparameter) ?? Enumerable.Empty<ImpSpezXMLClasses.Eingabeparameter>())
                    {

                        bool parameterFound = false; // wird true, wenn der Parameter gefunden wurde

                        // Parameter (Spalte) in der Importdatei suchen
                        foreach (System.Data.DataColumn column in dtImportFileHeader.Columns)
                        {
                            if (String.Equals(impspezparameter.Datenspaltenname, column.ColumnName, StringComparison.OrdinalIgnoreCase)) { parameterFound = true; break; } ;
                        }

                        // Pruefen, ob der Parameter gefunden wurde, wenn nicht ist die Pruefung hier schon fehlgeschlagen
                        if (!parameterFound)
                        {
                            isValid = false;
                            break;
                        }

                    }


                    // 4.) Eingabekardinalität > 1 definiert aber kein Eingabeparameter vom Typ Array mit entsprechender Laenge in der Importspezifikation vorhanden
                    int intEingabekardinalitaet = 0;
                    if (int.TryParse(Uebersicht.Importspezifikation?.Prozess?.EingabeParameterCollection?.EingabeKardinalitaet, out intEingabekardinalitaet))
                    {
                        if (intEingabekardinalitaet > 1)
                        {
                            bool arrayParameterFound = false; // wird true, wenn ein Array-Parameter gefunden wurde

                            foreach (ImpSpezXMLClasses.Eingabeparameter impspezparameter in (Uebersicht.Importspezifikation?.Prozess?.EingabeParameterCollection?.Eingabeparameter) ?? Enumerable.Empty<ImpSpezXMLClasses.Eingabeparameter>())
                            {
                                if (String.Equals(impspezparameter.IsArray, "true", StringComparison.OrdinalIgnoreCase) 
                                 && String.Equals(impspezparameter.Lfdn,intEingabekardinalitaet.ToString(), StringComparison.OrdinalIgnoreCase))
                                {
                                    arrayParameterFound = true;
                                    break;
                                }
                            }
                            
                            if (!arrayParameterFound)
                            {
                                isValid = false;
                            }
                        }
                    }

                });

                return isValid;

            }
            catch (Exception e)
            {
                throw new InvalidDataException(e.ToString(),e);
            }
        }


      

    }


    /// <summary>
    /// Klasse fuer Importspezifikationsdatei fuer objektorientierte Prozesse
    /// </summary>
    internal class OO
    {

        /// <summary>
        /// Prueft, ob die Importspezifikation gueltig ist und verwendet werden kann
        /// </summary>
        /// <param name="Uebersicht"></param>
        /// <returns>bool</returns>
        internal static bool IsImportspezifikationValid(ImpSpezXMLClasses.Uebersicht Uebersicht)
        {
            return IsImportspezifikationValidAsync(Uebersicht).Result;
        }


        /// <summary>
        /// Prueft, ob die Importspezifikation gueltig ist und verwendet werden kann
        /// </summary>
        /// <param name="Uebersicht"></param>
        /// <returns>bool</returns>
        internal static async Task<bool> IsImportspezifikationValidAsync(ImpSpezXMLClasses.Uebersicht Uebersicht)
        {
            try
            {
                bool isValid = true;

                await Task.Run(action: () =>
                {

                    if (!String.Equals(Uebersicht.Importspezifikation?.Prozess?.SchnittstellenArt, "OO", StringComparison.OrdinalIgnoreCase))
                    {

                        isValid = false;

                    }

                    // Das Eingabefachobjekt ermitteln, hier brauchen wir die Parameter-Referenzen fuer die Ermittlung der Eingabeparameter
                    fiServiceXmlClasses.OO.FachObjekt fo = Helper.fiServiceXml.OO.GetFachObjektEingabeByServiceOperator(String.Concat(Uebersicht.Importspezifikation?.Prozess?.Name, ""),
                                                                                                                        String.Concat(Uebersicht.Importspezifikation?.Prozess?.ServiceOperation, ""));

                    // Die Eingabeparameter in ein Dictionary fuer einen Vergleich Parameter laden 
                    var dictFachobjektParameter = Helper.fiServiceXml.OO.GetFachobjektParameter(fo, String.Concat(Uebersicht.Importspezifikation?.Prozess?.Name,""));

                    // Der Header der Importdatei in eine DataTable
                    System.Data.DataTable dtImportFileHeader =  Helper.ImpSpezXml.Global.ReadAndConvertCsvToDataTableHeader(Uebersicht);


                    // 1.) Pruefen, ob alle verpflichtenden Parameter fuer das Eingabefachobjekt in der Importspezifikation vorhanden sind
                    if (fo != null && dictFachobjektParameter != null) {
                        
                        foreach (var foParameter in dictFachobjektParameter.Where(i => i.Value.IsMandatory))
                        {

                            bool parameterFound = false; // wird true, wenn der Parameter gefunden wurde

                            // Parameter in der Importspezifikation suchen
                            foreach (ImpSpezXMLClasses.Eingabeparameter impspezparameter in (Uebersicht.Importspezifikation?.Prozess?.EingabeParameterCollection?.Eingabeparameter ?? Enumerable.Empty<ImpSpezXMLClasses.Eingabeparameter>()))
                            {
                                if (String.Equals(foParameter.Value.ParameterName, impspezparameter.Name, StringComparison.OrdinalIgnoreCase)) { parameterFound = true; break; }
                            }

                            // Pruefen, ob der Parameter gefunden wurde, wenn nicht ist die Pruefung hier schon fehlgeschlagen
                            if (!parameterFound)
                            {
                                isValid = false;
                                break;
                            }

                        }

                    }
                    else
                    {

                        isValid = false; 

                    }


                    // 2.) Pruefen, ob alle in der Importspezifikation enthaltenen Parameter auch gueltige Eingabeparameter fuer das Fachobjekt sind
                    if (fo != null && dictFachobjektParameter != null) {
                        
                        foreach (ImpSpezXMLClasses.Eingabeparameter impspezparameter in (Uebersicht.Importspezifikation?.Prozess?.EingabeParameterCollection?.Eingabeparameter ?? Enumerable.Empty<ImpSpezXMLClasses.Eingabeparameter>()))
                        {

                            bool parameterFound = false; // wird true, wenn der Parameter gefunden wurde

                            // Parameter im Eingabefachobjekt suchen
                            foreach (var foParameter in dictFachobjektParameter)
                            {
                                if (String.Equals(impspezparameter.Name, foParameter.Value.ParameterName, StringComparison.OrdinalIgnoreCase)) { parameterFound = true; break; }
                            }

                            // Pruefen, ob der Parameter gefunden wurde, wenn nicht ist die Pruefung hier schon fehlgeschlagen
                            if (!parameterFound)
                            {
                                isValid = false;
                                break;
                            }

                        }
                    } else
                    {
                        
                        isValid = false;

                    }
    


                    // 3.) Pruefen, ob alle Felder (Spalten) der Importspezifikation auch in der Importdatei vorhanden sind
                    foreach (ImpSpezXMLClasses.Eingabeparameter impspezparameter in (Uebersicht.Importspezifikation?.Prozess?.EingabeParameterCollection?.Eingabeparameter ?? Enumerable.Empty<ImpSpezXMLClasses.Eingabeparameter>()))
                    {

                        bool parameterFound = false; // wird true, wenn der Parameter gefunden wurde

                        // Parameter (Spalte) in der Importdatei suchen
                        foreach (System.Data.DataColumn column in dtImportFileHeader.Columns)
                        {
                            if (String.Equals(impspezparameter.Datenspaltenname, column.ColumnName, StringComparison.OrdinalIgnoreCase)) { parameterFound = true; break; };
                        }

                        // Pruefen, ob der Parameter gefunden wurde, wenn nicht ist die Pruefung hier schon fehlgeschlagen
                        if (!parameterFound)
                        {
                            isValid = false;
                            break;
                        }

                    }


                });

                return isValid;

             }
            catch (Exception e)
            {
                throw new InvalidDataException(e.ToString(), e);
            }
        }

    }

    /// <summary>
    /// Klasse fuer die Dateibehandlung der Importspezifikation
    /// </summary>
    internal class ImpSpezXmlFile
    {

        /// <summary>
        /// Liest die Importdatei und konvertiert diese als DataTable
        /// </summary>
        /// <param name="Importdatei"></param>
        /// <param name="delimiter"></param>
        /// <param name="textqualifier"></param>
        /// <returns>System.Data.DataTable</returns>
        /// <exception cref="Exception"></exception>
        internal static async Task<System.Data.DataTable> ReadAndConvertCsvToDataTableAsync(string Importdatei, string delimiter, string textqualifier)
        {

            bool isFirstRowHeader = true;

            try
            {

                // Datatable erstellen
                System.Data.DataTable objDataTable = new System.Data.DataTable();

                await Task.Run(action: () =>
                {

                    // Datei lesen und Felder teilen
                    using (TextReader textReader = new StreamReader(Importdatei, Encoding.GetEncoding("iso-8859-1")))
                    {

                        string? line;

                        while ((line = textReader.ReadLine()) != null)
                        {

                            // Zeile nach Trennzeichen splitten
                            string[] arrItems = line.Split(new string[] { delimiter }, StringSplitOptions.None);

                            // Textqualifizierer verarbeiten
                            if (!String.IsNullOrWhiteSpace(textqualifier))
                            {
                                for (int i = 0; i < arrItems.Length; i++)
                                {
                                    if (textqualifier.Length == 1)
                                    {
                                        arrItems[i] = arrItems[i].Trim(textqualifier[0]);
                                    }
                                }
                            }

                            // Headerzeile bearbeiten/festlegen
                            if (objDataTable.Columns.Count == 0)
                            {

                                // Zeile mit Spaltenbezeichnung in die DataTable einfuegen
                                if (isFirstRowHeader)
                                {
                                    for (int i = 0; i < arrItems.Length; i++)
                                    {
                                        objDataTable.Columns.Add(new System.Data.DataColumn(Convert.ToString(arrItems[i]), typeof(string)));
                                    }

                                    continue;

                                }
                                else
                                {

                                    // Ohne Header die Spalten einfach durchnummerieren
                                    for (int i = 0; i < arrItems.Length; i++)
                                    {
                                        objDataTable.Columns.Add(new System.Data.DataColumn("Column" + Convert.ToString(i), typeof(string)));
                                    }

                                }
                            }

                            // Hier nun die Zeile in die DataTable einfuegen
                            objDataTable.Rows.Add(arrItems);

                        }
                    }

                });

                return objDataTable;

            }
            catch (Exception e)
            {

                throw new FileLoadException(e.ToString(), e);

            }

        }

        /// <summary>
        /// Liest nur den Header (Erste Zeile) der Importdatei und konvertiert diesen als DataTable
        /// </summary>
        /// <param name="Importdatei"></param>
        /// <param name="delimiter"></param>
        /// <param name="textqualifier"></param>
        /// <returns>System.Data.DataTable</returns>
        /// <exception cref="Exception"></exception>
        internal static async Task<System.Data.DataTable> ReadAndConvertCsvToDataTableHeaderAsync(string Importdatei, string delimiter, string textqualifier)
        {

            bool isFirstRowHeader = true;

            try
            {

                // Datatable erstellen
                System.Data.DataTable objDataTable = new System.Data.DataTable();

                await Task.Run(action: () =>
                {

                    // Datei lesen und Felder teilen
                    using (TextReader textReader = new StreamReader(Importdatei, Encoding.GetEncoding("iso-8859-1")))
                    {

                        string? line;

                        // Erste Zeile lesen
                        line = textReader.ReadLine();

                        if (line != null)
                        {

                            // Zeile nach Trennzeichen splitten
                            string[] arrItems = line.Split(new string[] { delimiter }, StringSplitOptions.None);

                            // Textqualifizierer verarbeiten
                            if (!String.IsNullOrWhiteSpace(textqualifier))
                            {

                                for (int i = 0; i < arrItems.Length; i++)
                                {

                                    if (textqualifier.Length == 1)
                                    {
                                        arrItems[i] = arrItems[i].Trim(textqualifier[0]);
                                    }

                                }

                            }

                            // Headerzeile bearbeiten/festlegen
                            if (objDataTable.Columns.Count == 0)
                            {

                                // Zeile mit Spaltenbezeichnung in die DataTable einfuegen
                                if (isFirstRowHeader)
                                {

                                    for (int i = 0; i < arrItems.Length; i++)
                                    {
                                        objDataTable.Columns.Add(new System.Data.DataColumn(Convert.ToString(arrItems[i]), typeof(string)));
                                    }

                                }

                            }

                            // Hier nun die Zeile in die DataTable einfuegen
                            objDataTable.Rows.Add(arrItems);

                        }

                    }
                });

                return objDataTable;

            }
            catch (Exception e)
            {

                throw new FileLoadException(e.ToString(), e);
            }

        }

        internal static async Task WriteAndConvertDataTableToCsvAsync(System.Data.DataTable DataTable, string Exportdatei, string delimiter, string textqualifier)
        {

            bool isFirstRowHeader = true;

            await Task.Run(action: () =>
            {

                // Datei lesen und Felder teilen
                using (TextWriter textWriter = new StreamWriter(Exportdatei, false, Encoding.GetEncoding("iso-8859-1")))
                {

                    if (isFirstRowHeader)
                    {
                        // Header schreiben
                        for (int i = 0; i < DataTable.Columns.Count; i++)
                        {
                            textWriter.Write(textqualifier + DataTable.Columns[i].ColumnName + textqualifier);
                            if (i < DataTable.Columns.Count - 1)
                                textWriter.Write(delimiter);
                        }
                        textWriter.WriteLine();
                    }


                    // Datenzeilen schreiben
                    foreach (System.Data.DataRow row in DataTable.Rows)
                    {
                        for (int i = 0; i < DataTable.Columns.Count; i++)
                        {
                            var value = row[i]?.ToString();

                            // Werte escapen, falls nötig
                            if (value != null && (value.Contains(";") || value.Contains("\"")))
                            {
                                value = "\"" + value.Replace("\"", "\"\"") + "\"";
                            }

                            textWriter.Write(textqualifier + value + textqualifier);
                            if (i < DataTable.Columns.Count - 1)
                                textWriter.Write(delimiter);
                        }
                        textWriter.WriteLine();
                    }
                }
            });
        }

    }
}

