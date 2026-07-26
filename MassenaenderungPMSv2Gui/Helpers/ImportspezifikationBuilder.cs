using MassenaenderungPMSv2.ImpSpezXMLClasses;

namespace MassenaenderungPMSv2Gui.Helpers
{
    /*
 
var spezifikation = new ImportspezifikationBuilder()
    .WithVersion("1.0")
    .WithBeschreibung("Import für Kunden")
    .WithImportdatei(Importdatei)
    .WithTrennzeichen(Trennzeichen)
    .WithTextqualifizierer(Textqualifizierer)
    .WithProzessName(SelectedProzess?.Name)
    .WithSchnittstellenArt(SelectedDynsSchnittstellenart?.Name)
    .WithServiceOperation("ImportCustomer")
    .WithAufrufvariantenNummer("1")
    .WithLogdatei("import.log")
    .WithEingabeKardinalitaet("1..n")
    .AddEingabeparameter("KundenNr", "KNR")
    .AddEingabeparameter("Name", "NAME")
    .Build();

*/

    public class ImportspezifikationBuilder
    {
        private readonly Uebersicht _uebersicht = new();
        private readonly Importspezifikation _importspezifikation = new();
        private readonly Prozess _prozess = new();
        private readonly EingabeParameterCollection _eingabeParameterCollection = new()
        {
            Eingabeparameter = new List<Eingabeparameter>()
        };

        public ImportspezifikationBuilder()
        {
            _prozess.EingabeParameterCollection = _eingabeParameterCollection;
            _uebersicht.Importspezifikation = _importspezifikation;
            _importspezifikation.Prozess = _prozess;
        }

        //
        // Importspezifikation
        //
        public ImportspezifikationBuilder WithVersion(string version)
        {
            _importspezifikation.Version = version;
            return this;
        }

        public ImportspezifikationBuilder WithBeschreibung(string beschreibung)
        {
            _importspezifikation.Beschreibung = beschreibung;
            return this;
        }

        public ImportspezifikationBuilder WithImportdatei(string importdatei)
        {
            _importspezifikation.Importdatei = importdatei;
            return this;
        }

        public ImportspezifikationBuilder WithTrennzeichen(string trennzeichen)
        {
            _importspezifikation.Trennzeichen = trennzeichen;
            return this;
        }

        public ImportspezifikationBuilder WithTextqualifizierer(string textqualifizierer)
        {
            _importspezifikation.Textqualifizierer = textqualifizierer;
            return this;
        }

        //
        // Prozess
        //
        public ImportspezifikationBuilder WithProzessName(string name)
        {
            _prozess.Name = name;
            return this;
        }

        public ImportspezifikationBuilder WithSchnittstellenArt(string art)
        {
            _prozess.SchnittstellenArt = art;
            return this;
        }

        public ImportspezifikationBuilder WithServiceOperation(string operation)
        {
            _prozess.ServiceOperation = operation;
            return this;
        }

        public ImportspezifikationBuilder WithAufrufvariantenNummer(string nummer)
        {
            _prozess.AufrufvariantenNummer = nummer;
            return this;
        }

        public ImportspezifikationBuilder WithLogdatei(string logdatei)
        {
            _prozess.Logdatei = logdatei;
            return this;
        }

        //
        // EingabeParameterCollection
        //
        public ImportspezifikationBuilder WithEingabeKardinalitaet(string kardinalitaet)
        {
            _eingabeParameterCollection.EingabeKardinalitaet = kardinalitaet;
            return this;
        }

        public ImportspezifikationBuilder AddEingabeparameter(
            string name,
            string datenspaltenname,
            string eingabeFo = "",
            string lfdn = "",
            string isArray = "false")
        {
            _eingabeParameterCollection.Eingabeparameter!.Add(new Eingabeparameter
            {
                Name = name,
                Datenspaltenname = datenspaltenname,
                EingabeFo = eingabeFo,
                Lfdn = lfdn,
                IsArray = isArray
            });

            return this;
        }

        //
        // Build
        //
        public Uebersicht Build()
        {
            return _uebersicht;
        }
    }

}
