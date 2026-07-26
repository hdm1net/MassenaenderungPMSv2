using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace MassenaenderungPMSv2
{  
    public class MyConfiguration
    {
        public string fiServicesXMLPath { get; set; } 

        public string Global_Log2Console {  get; set; }

        public string Global_AppLogfile { get; set; }

        public string Global_AppLogfileLinesMax { get; set; }

        public DynSKontext DynSKontext { get; set; }   

        public MyConfiguration() {

            fiServicesXMLPath = ConfigEntryGet("fiServices:XmlPath");

            Global_Log2Console = ConfigEntryGet("GlobalSettings:Log2Console");
            Global_AppLogfile = ConfigEntryGet("GlobalSettings:AppLog:File");
            Global_AppLogfileLinesMax = ConfigEntryGet("GlobalSettings:AppLog:MaxLines");

            DynSKontext = new DynSKontext();
            LoadDynSKontext(DynSKontext);
        }

        /// <summary>
        /// Liest die Konfigurationseinstellungen der Anwendung aus der appsettings.json
        /// </summary>
        /// <returns>MyConfiguration - Konfigurationseinstellungen der Anwendung</returns>
        public static MyConfiguration GetConfiguration()
        {

            var c = new MyConfiguration();

            c.fiServicesXMLPath = ConfigEntryGet("fiServices:XmlPath");

            c.Global_Log2Console = ConfigEntryGet("GlobalSettings:Log2Console");
            c.Global_AppLogfile = ConfigEntryGet("GlobalSettings:AppLog:File");
            c.Global_AppLogfileLinesMax = ConfigEntryGet("GlobalSettings:AppLog:MaxLines");

            LoadDynSKontext(c.DynSKontext);

            return c;

        }

        /// <summary>
        /// Get the ConfigRoot-Path
        /// </summary>
        /// <returns><c>IConfigurationRoot</c></returns>
        private static IConfigurationRoot GetConfigRoot()
        {
            string assemblyLoc = Assembly.GetExecutingAssembly().Location;
            string directoryPath = Path.GetDirectoryName(assemblyLoc) + "";
            string configFilePath = Path.Combine(directoryPath, "Config", "appsettings.json");

            if (File.Exists(configFilePath) == false)
            {
                throw new FileNotFoundException("Config file Config/appsettings.json not found");
            }

            var configurationBuilder = new ConfigurationBuilder().AddJsonFile(configFilePath);
            
            return configurationBuilder.Build();
        }

        /// <summary>
        /// Liest einen Konfigurationseintrag aus den Anwendungseinstellungen
        /// </summary>
        /// <returns>Wert der Konfigurationseinstellung</returns>
        private static string ConfigEntryGet(string configEntryName)
        {
            if (string.IsNullOrWhiteSpace(configEntryName))
            {
                throw new ArgumentException("param:configEntryName missing", nameof(configEntryName));
            }

            var root = GetConfigRoot();
            var ret = root[configEntryName];

            if (string.IsNullOrWhiteSpace(ret))
            {
                //throw new InvalidOperationException("Config value cannot be empty");
                ret = String.Empty;
            }

            return ret;
        }

        /// <summary>
        /// Liest den DynSKontext aus der Konfiguration
        /// </summary>
        /// <param name="dynSKontext"></param>
        private static void LoadDynSKontext(DynSKontext dynSKontext)
        {
            dynSKontext.BaseURL = ConfigEntryGet("DynSKontext:BaseURL");
            dynSKontext.AnwdKurzname = ConfigEntryGet("DynSKontext:AnwdKurzname");
            dynSKontext.VRZ = ConfigEntryGet("DynSKontext:VRZ");
            dynSKontext.INR = Convert.ToInt16(ConfigEntryGet("DynSKontext:INR"));
            dynSKontext.PRODUKTID = ConfigEntryGet("DynSKontext:PRODUKTID");
            dynSKontext.CLIENTID = ConfigEntryGet("DynSKontext:CLIENTID");
            dynSKontext.VERTRIEBSWEG = Convert.ToInt16(ConfigEntryGet("DynSKontext:VERTRIEBSWEG"));
            dynSKontext.PS_LOKATION = ConfigEntryGet("DynSKontext:PS_LOKATION");
            dynSKontext.PS_USERID = ConfigEntryGet("DynSKontext:PS_USERID");
            dynSKontext.PS_PASSWORT = Enryption.AesOperation.DecryptString(ConfigEntryGet("DynSKontext:PS_PASSWORT"));
            dynSKontext.PS_SESSION_TOKEN = String.Empty;
            dynSKontext.PS_SESSION_INDEX = ConfigEntryGet("DynSKontext:CLIENTID");
            dynSKontext.DynSMinWaitTime = Convert.ToInt16(ConfigEntryGet("DynSKontext:DynSMinWaitTime"));
            dynSKontext.DynSMaxWaitTime = Convert.ToInt16(ConfigEntryGet("DynSKontext:DynSMaxWaitTime"));
            dynSKontext.DynSNormalWaitTime = Convert.ToInt16(ConfigEntryGet("DynSKontext:DynSNormalWaitTime"));

            // DynSKontext.CLIENTID darf an dieser Stelle max. 10 Stellen lang sein, da dieser in der Anwendung um weitere 14 Stellen dynamisch erweitert wird,
            // und der String insgesamt max. 24 Stellen lang sein darf.
            if (dynSKontext.CLIENTID.Length > 10)
            {
                dynSKontext.CLIENTID.Substring(0, 10);
            }
        }


    }

    public class DynSKontext
    {
        public string BaseURL { get; set; } = String.Empty;
        public string AnwdKurzname { get; set; } = String.Empty;
        public string VRZ { get; set; } = String.Empty;
        public int INR { get; set; } = 0;
        public string PRODUKTID { get; set; } = String.Empty;
        public string CLIENTID { get; set; } = String.Empty;
        public int VERTRIEBSWEG { get; set; } = 1;
        public string PS_LOKATION { get; set; } = String.Empty;
        public string PS_USERID { get; set; } = String.Empty;
        public string PS_PASSWORT { get; set; } = String.Empty;
        public string PS_SESSION_TOKEN { get; set; } = String.Empty;
        public string PS_SESSION_INDEX { get; set; } = String.Empty;
        public int DynSMinWaitTime { get; set; } = 0;
        public int DynSNormalWaitTime { get; set; } = 0;
        public int DynSMaxWaitTime { get; set; } = 0;

        public DynSKontext() { }

    }
}
