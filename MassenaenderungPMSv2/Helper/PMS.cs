
namespace MassenaenderungPMSv2.Helper
{
    internal class PMS
    {

        public BWSPS.PS_Session Session { get; }

        public DynSKontext DynSKontext { get; set; }

        private Helper.Logging logging = new Helper.Logging();

        private MyConfiguration myConfiguration = MyConfiguration.GetConfiguration();

        public PMS()
        {

            Session = new BWSPS.PS_Session();
            DynSKontext = new DynSKontext();
            logging = new Helper.Logging();
            myConfiguration = MyConfiguration.GetConfiguration();

        }

        public bool Login()
        {
            try
            {
                // Session-Eigenschaften setzen
                Session.DynsLoadbalancerUrl = DynSKontext.BaseURL;
                Session.Aufrufweg = DynSKontext.VERTRIEBSWEG; // 1 Stationär
                Session.InstitutsNr = DynSKontext.INR; // 3 Stellen Zahl
                Session.VRZ = DynSKontext.VRZ; // 5 Stellen
                Session.ApplikationsId = DynSKontext.PRODUKTID; // 20 Stellen
                Session.ProduktId = DynSKontext.PRODUKTID; // 20 Stellen
                Session.VersionsNummer = 1;
                Session.UserId = DynSKontext.PS_USERID; // 8 Stellen

                // Prozess-Instanz erzeugen
                BWSPS.PS_Prozess prozess = Session.NeuerProzess("LOGIN");

                // Prozess-Eigenschaften setzen
                prozess.LogSize = 20;

                // Input-Parameter setzen
                prozess.ParameterHinzufuegen("PS_LOKATION", BWSPS.EnumParameterArt.PsIn, BWSPS.EnumParameterTyp.PsString, 24, 1, DynSKontext.PS_LOKATION);
                prozess.ParameterHinzufuegen("PS_USERID", BWSPS.EnumParameterArt.PsIn, BWSPS.EnumParameterTyp.PsString, 8, 1, DynSKontext.PS_USERID);
                prozess.ParameterHinzufuegen("PS_PASSWORT", BWSPS.EnumParameterArt.PsIn, BWSPS.EnumParameterTyp.PsString, 73, 1, DynSKontext.PS_PASSWORT);
                prozess.ParameterHinzufuegen("PS_SESSION_INDEX", BWSPS.EnumParameterArt.PsIn, BWSPS.EnumParameterTyp.PsString, 24, 1, DynSKontext.PS_SESSION_INDEX);

                // Output-Parameter setzen
                prozess.ParameterHinzufuegen("PS_SESSION_TOKEN", BWSPS.EnumParameterArt.PsOut, BWSPS.EnumParameterTyp.PsString, 20, 1, "");
                prozess.ParameterHinzufuegen("PS_LOGIN_STATUS", BWSPS.EnumParameterArt.PsOut, BWSPS.EnumParameterTyp.PsSignedLong, 4, 1, "");
                prozess.ParameterHinzufuegen("HOST_RELEASE", BWSPS.EnumParameterArt.PsOut, BWSPS.EnumParameterTyp.PsString, 6, 1, "");

                // Prozess aufrufen/ausfuehren
                BWSPS.PS_Result result = prozess.Aktiviere();

                if (Convert.ToInt16(result.Status()) < 8)
                {
                    // Session-Token im Kontext und der Session speichern
                    DynSKontext.PS_SESSION_TOKEN = result.FeldWert("PS_SESSION_TOKEN", 1);
                    Session.SetzeProperty("PS_SESSION_TOKEN", result.FeldWert("PS_SESSION_TOKEN", 1));
                    Session.SetzeProperty("PS_SESSION_INDEX", DynSKontext.PS_SESSION_INDEX); // max. 24 Stellen

                    //logging.Add("PMS-Login erfolgreich: " + result.LogCode(0) + ": " + result.LogText(0), myConfiguration.Global_AppLogfile);

                    return true;

                }
                else
                {

                    string message = string.Empty;
                    for (short i = 0; i < result.LogAnzahl(); i++)
                    {
                        logging.Add("ERROR: PMS-Login fehlgeschlagen: " + result.LogCode(i) + ": " + result.LogText(i), myConfiguration.Global_AppLogfile);
                    }

                    return false;

                }

            }
            catch (Exception e)
            {

                throw new ApplicationException("PMS-Login fehlgeschlagen", e);
            }

        }

        public bool Logoff()
        {

            try
            {

                // Prozess-Instanz erzeugen
                BWSPS.PS_Prozess prozess = Session.NeuerProzess("LOGOFF");

                // Prozess-Eigenschaften setzen
                prozess.LogSize = 20;

                // Input-Parameter setzen
                prozess.ParameterHinzufuegen("PS_LOKATION", BWSPS.EnumParameterArt.PsIn, BWSPS.EnumParameterTyp.PsString, 24, 1, DynSKontext.PS_LOKATION);
                prozess.ParameterHinzufuegen("PS_USERID", BWSPS.EnumParameterArt.PsIn, BWSPS.EnumParameterTyp.PsString, 8, 1, DynSKontext.PS_USERID);

                // Prozess aufrufen/ausfuehren
                BWSPS.PS_Result result = prozess.Aktiviere();
                if (Convert.ToInt16(result.Status()) < 8)
                {
                    //logging.Add("PMS-Logoff erfolgreich: " + result.LogCode(0) + ": " + result.LogText(0), myConfiguration.Global_AppLogfile);
                    return true;
                }
                else
                {
                    string message = string.Empty;
                    for (short i = 0; i < result.LogAnzahl(); i++)
                    {
                        logging.Add("ERROR: PMS-Logoff fehlgeschlagen: " + result.LogCode(i) + ": "  + result.LogText(i), myConfiguration.Global_AppLogfile);
                    }
                    return false;
                }
            }
            catch (Exception e)
            {

                throw new ApplicationException("PMS-Logoff fehlgeschlagen", e);
            }

        }


        internal static int GetPMSWaitTime()
        {
            // PMS-Wartezeit aus Konfiguration lesen
            MyConfiguration myConfiguration = MyConfiguration.GetConfiguration();


            if ((DateTime.Today.DayOfWeek == DayOfWeek.Saturday) || 
                (DateTime.Today.DayOfWeek == DayOfWeek.Sunday) ||
                ((DateTime.Today.DayOfWeek == DayOfWeek.Friday) && (DateTime.Now.Hour >= 13))) 
            {
                return myConfiguration.DynSKontext.DynSMinWaitTime;
            }

            if (DateTime.Now.Hour < 7)
            {
                return myConfiguration.DynSKontext.DynSMinWaitTime;
            }
            else if (DateTime.Now.Hour >= 7 && DateTime.Now.Hour < 13)
            {
                return myConfiguration.DynSKontext.DynSMaxWaitTime;
            }
            else if (DateTime.Now.Hour >= 17)
            {
                return myConfiguration.DynSKontext.DynSMinWaitTime;
            }
            else
            {
                return myConfiguration.DynSKontext.DynSNormalWaitTime;
            }

        }
    }


    internal class PMSParameter
    {
        public string Name { get; set; }
        public BWSPS.EnumParameterArt Art { get; set; }
        public BWSPS.EnumParameterTyp Typ { get; set; }
        public int Laenge { get; set; }
        public int Anzahl { get; set; }
        public string[] Wert { get; set; }
        public string Datenspaltenname { get; set; }


        internal PMSParameter(string name, BWSPS.EnumParameterArt art, BWSPS.EnumParameterTyp typ, int laenge, int anzahl, string[] wert, string datenspaltenname)
        {
            Name = name;
            Art = art;
            Typ = typ;
            Laenge = laenge;
            Anzahl = anzahl;
            Wert = wert;
            Datenspaltenname = datenspaltenname;
        }

    }

}
