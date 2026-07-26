namespace MassenaenderungPMSv2
{
    internal class Program
    {

        static void Main(string[] args)
        {
            
            //
            // Anwendungskonfiguraton lesen
            //
            var myconfig = MyConfiguration.GetConfiguration();  
            if (myconfig == null || String.IsNullOrEmpty(myconfig.fiServicesXMLPath)) {
                Console.WriteLine("Die Anwendungskonfiguration aus Datei Config/appsettings.json konnte nicht gelesen werden!");
                return; 
            }

            //
            // Startprotokollierung Anwendung
            //
            Log("Anwendung gestartet", myconfig.Global_AppLogfile);

            //
            // Aufrufparameter auswerten
            //
            var cliOptions = new Helper.CommandLineParser.CliOptions();
            cliOptions.Parse(args);

            // Anfrage zur Verschlüsselung der Zeichenfolge
            // Gibt nur das Ergebnis aus und beendet dann die Anwendung nach 10 Sek.
            if (!String.IsNullOrEmpty(cliOptions.StringToEncrypt)) {

                Console.WriteLine("Die verschlüsselte Zeichenfolge lautet: " + Enryption.AesOperation.EncryptString(cliOptions.StringToEncrypt));
                Log("Verschlüsselung einer angeforderten Zeichenfolge durchgeführt", myconfig.Global_AppLogfile);
                Log("Anwendung beendet", myconfig.Global_AppLogfile);
                System.Threading.Thread.Sleep(10000);
                
                return;

            }

            //
            // Aufrufparameter -i, --impspez pruefen, dieser wird zwingend benötigt
            //
            if (String.IsNullOrEmpty(cliOptions.Importspezifikation)) {

                LogError("Aufrufparameter -i, --impspez wurde nicht übergeben", myconfig.Global_AppLogfile);
                return;

            };
            Log("Aufrufparameter:", myconfig.Global_AppLogfile);
            Log("  Importspezifikation (-i, --impspez): " + cliOptions.Importspezifikation, myconfig.Global_AppLogfile);
            Log("  Testlauf (-t, --test)              : " + cliOptions.Test, myconfig.Global_AppLogfile);
            Log("  PMS-API-Delay (-d, --delay) ms     : " + cliOptions.Delay, myconfig.Global_AppLogfile);

            // 
            // XML mit der Importspezifikation lesen und auswerten
            //
            ImpSpezXMLClasses.Uebersicht impSpezUebersicht = Helper.ImpSpezXml.Global.GetImpSpezUebersicht(cliOptions.Importspezifikation);
            if (impSpezUebersicht == null || impSpezUebersicht.Importspezifikation == null) {

                LogError("Importspezifikation " + cliOptions.Importspezifikation + " konnte nicht gelesen werden", myconfig.Global_AppLogfile);
                return;

            }

            //
            // Hosteingabe durchfuehren
            //
            if (!String.IsNullOrEmpty(impSpezUebersicht.Importspezifikation.Prozess?.SchnittstellenArt))
            {
                
                switch (impSpezUebersicht.Importspezifikation.Prozess.SchnittstellenArt.ToUpper())
                {
                    case "PO":
                        HosteingabePMS.PO poHosteingabe = new HosteingabePMS.PO();
                        poHosteingabe.HostEingabeDurchfuehren(impSpezUebersicht, cliOptions.Test, cliOptions.Delay);
                        break;

                    case "OO":
                        HosteingabePMS.OO ooHosteingabe = new HosteingabePMS.OO();
                        ooHosteingabe.HostEingabeDurchfuehren(impSpezUebersicht, cliOptions.Test, cliOptions.Delay);
                        break;

                    default:
                        LogError("Schnittstellenart in der Importspezifikation ungueltig: " + impSpezUebersicht.Importspezifikation.Prozess.SchnittstellenArt, myconfig.Global_AppLogfile);
                        return;
                }

            }

            //
            // Habe fertig
            //
            Log("Anwendung beendet", myconfig.Global_AppLogfile);

            //
            // Anwendungslogfile ggf. kürzen
            //
            TruncateLogfile(myconfig.Global_AppLogfile, myconfig.Global_AppLogfileLinesMax);

#if DEBUG
            System.Threading.Thread.Sleep(10000);
#endif

        }
#region "Logging"
        private static void Log(string msg, string logfile)
        {
            Helper.Logging logging = new Helper.Logging();
            logging.Add(msg, logfile);
        } 

        private static void LogError(string msg, string logfile)
        {
            Helper.Logging logging = new Helper.Logging();
            logging.Add("ERROR: " + msg, logfile);
        }

        private static void TruncateLogfile(string logFile, string maxLines)
        {
            int maxlines = 10000;  // default
            int.TryParse(maxLines, out maxlines);
            var logging = new Helper.Logging();
            logging.Truncate(maxlines, logFile);
        }
#endregion
    }
}
