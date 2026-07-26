using System.CommandLine;

namespace MassenaenderungPMSv2.Helper.CommandLineParser
{
    internal class CliOptions
    {
        public string Importspezifikation { get; set; } = String.Empty;

        public bool Test { get; set; } = false;

        public int? Delay { get; set; } = null;

        public string? StringToEncrypt { get; set; }

        public CliOptions() { }

        public int Parse(string[] args)
        {

            RootCommand rootCommand = new("Massendatenänderung OSPlus");

            // Auf Required verzichten, da die Option -enc alleine aufgerufen werden kann, um eine Zeichenfolge zu verschlüsseln. In diesem Fall wird die Importspezifikation nicht benötigt.
            // Die Importspezifikation wird in der Program.cs geprüft, ob sie zwingend benötigt wird.
            Option<string> importspezifikation = new("-i", "--impspez") { Description = "Pfad zur XML-Datei mit der Importspezifikation", Required = false };

            Option<bool> test = new("-t", "--test") { Description = "Testlauf mit nur einem Datensatz ausführen", Required = false };

            Option<string> delay = new("-d", "--delay") { Description = "Übersteuerung der Verzögerung in Millisekunden der PMS-API-Aufrufe", Required = false };

            Option<string> encryption = new("-enc", "--encryption") { Description = "Verschlüsselung des Benutzerpassworts zur Hinterlegung in der appsettings.json", Required = false };

            rootCommand.Options.Add(importspezifikation);
            rootCommand.Options.Add(test);
            rootCommand.Options.Add(delay);
            rootCommand.Options.Add(encryption);

            rootCommand.SetAction(parseResult =>
            {
                if (!String.IsNullOrEmpty(parseResult.GetValue(importspezifikation))) { Importspezifikation = parseResult.GetValue(importspezifikation); }
                if (!String.IsNullOrEmpty(parseResult.GetValue(delay))) 
                { 
                    if( int.TryParse(parseResult.GetValue(delay), out int parsedDelay))
                    {
                        Delay = parsedDelay;
                    } else
                    {
                        Delay = null;
                    }
                } 
                Test = parseResult.GetValue(test);
                StringToEncrypt = parseResult.GetValue(encryption);
            });

            return rootCommand.Parse(args).Invoke();
        }

    }
}
