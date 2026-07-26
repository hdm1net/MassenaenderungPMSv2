using System.Xml.Serialization;

namespace MassenaenderungPMSv2.ImpSpezXMLClasses
{

    [XmlRoot("Uebersicht")]
    public class Uebersicht
    {
        [XmlElement("Importspezifikation")]
        public Importspezifikation? Importspezifikation { get; set; }
    }

    public class Importspezifikation
    {
        [XmlAttribute("Version")]
        public string? Version { get; set; }

        [XmlAttribute("Beschreibung")]
        public string? Beschreibung { get; set; }

        [XmlAttribute("Importdatei")]
        public string? Importdatei { get; set; }

        [XmlAttribute("Trennzeichen")]
        public string? Trennzeichen { get; set; }

        [XmlAttribute("Textqualifizierer")]
        public string? Textqualifizierer { get; set; }

        [XmlElement("Prozess")]
        public Prozess? Prozess { get; set; }
    }

    public class Prozess
    {
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("schnittstellen_art")]
        public string? SchnittstellenArt { get; set; }

        [XmlAttribute("service_operation")]
        public string? ServiceOperation { get; set; }

        [XmlAttribute("aufrufvariantennummer")]
        public string? AufrufvariantenNummer { get; set; }

        [XmlAttribute("logdatei")]
        public string? Logdatei { get; set; }

        [XmlElement("EingabeParameterCollection")]
        public EingabeParameterCollection? EingabeParameterCollection { get; set; }
    }

    public class EingabeParameterCollection
    {
        [XmlElement("Eingabeparameter")]
        public List<Eingabeparameter>? Eingabeparameter { get; set; }

        [XmlAttribute("eingabekardinalität")]
        public string? EingabeKardinalitaet { get; set; }
        
    }

    public class Eingabeparameter
    {
        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("eingabe-fo")]
        public string? EingabeFo { get; set; }

        [XmlAttribute("lfdn")]
        public string? Lfdn { get; set; }

        [XmlAttribute("isarray")]
        public string? IsArray { get; set; }

        [XmlText]
        public string? Datenspaltenname { get; set; }

    }

}
