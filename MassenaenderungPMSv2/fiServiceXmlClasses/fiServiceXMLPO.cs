using System.Xml.Serialization;

namespace MassenaenderungPMSv2.fiServiceXmlClasses.PO
{

    [XmlRoot("PROZESS_UEBERSICHT")]
    public class ProzessUebersicht
    {
        [XmlElement("TOP_ENTITAET")]
        public TopEntitaet TopEntitaet { get; set; }
    }

    public class TopEntitaet
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("untergruppe")]
        public string Untergruppe { get; set; }

        [XmlAttribute("java-package")]
        public string JavaPackage { get; set; }

        [XmlElement("T_PROZESS")]
        public TProzess Prozess { get; set; }
    }

    public class TProzess
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("ergebniskardinalitaet")]
        public string Ergebniskardinalitaet { get; set; }

        [XmlAttribute("ressource")]
        public string Ressource { get; set; }

        [XmlAttribute("ts_aenderung")]
        public string TsAenderung { get; set; }

        [XmlAttribute("implementierungseinheit")]
        public string Implementierungseinheit { get; set; }

        [XmlAttribute("gruppe")]
        public string Gruppe { get; set; }

        [XmlAttribute("schnittstellen_art")]
        public string SchnittstellenArt { get; set; }

        [XmlAttribute("prozess_nr")]
        public int ProzessNr { get; set; }

        [XmlElement("PROZESS_INFO")]
        public ProzessInfo ProzessInfo { get; set; }

        [XmlElement("PARAMETER_UEBERSICHT")]
        public ParameterUebersicht ParameterUebersicht { get; set; }

        [XmlElement("AUFRUFVARIANTEN")]
        public AufrufVarianten AufrufVarianten { get; set; }

        [XmlElement("AUSGABEPARAMETER")]
        public AusgabeParameter AusgabeParameter { get; set; }

        [XmlElement("RETURNCODELISTE")]
        public ReturnCodeListe ReturnCodeListe { get; set; }
    }

    public class ProzessInfo
    {
        [XmlElement("HIERARCHIE-STUFE")]
        public string HierarchieStufe { get; set; }

        [XmlElement("VISIBLE")]
        public string Visible { get; set; }

        [XmlElement("FACHLICHE_BESCHREIBUNG")]
        public string FachlicheBeschreibung { get; set; }

        [XmlElement("VORBEDINGUNGEN")]
        public string Vorbedingungen { get; set; }

        [XmlElement("NACHBEDINGUNGEN")]
        public string Nachbedingungen { get; set; }

        [XmlElement("BATCHFAEHIG_KZ")]
        public string BatchfaehigKz { get; set; }

        [XmlElement("ASYNC_KZ")]
        public string AsyncKz { get; set; }

        [XmlElement("IST_AUSFUEHRBAR_UNTER_CICS")]
        public string IstAusfuehrbarUnterCics { get; set; }

        [XmlElement("IST_AUSFUEHRBAR_UNTER_JEE")]
        public string IstAusfuehrbarUnterJee { get; set; }

        [XmlElement("ZUGRIFF")]
        public string Zugriff { get; set; }

        [XmlElement("PREIS")]
        public string Preis { get; set; }

        [XmlElement("VERFUEGBARKEIT")]
        public string Verfuegbarkeit { get; set; }

        [XmlElement("ANSPRECHPARTNER")]
        public string Ansprechpartner { get; set; }
    }

    public class ParameterUebersicht
    {
        [XmlElement("PARAMETER")]
        public List<Parameter> Parameter { get; set; }
    }

    public class Parameter
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("typ")]
        public string Typ { get; set; }

        [XmlAttribute("laenge")]
        public string Laenge { get; set; }

        [XmlAttribute("modelldatentyp")]
        public string ModellDatentyp { get; set; }

        [XmlAttribute("richtung")]
        public string Richtung { get; set; }

        [XmlAttribute("beschreibung")]
        public string Beschreibung { get; set; }
    }

    public class AufrufVarianten
    {
        [XmlElement("AV")]
        public List<AufrufVariante> Varianten { get; set; }
    }

    public class AufrufVariante
    {
        [XmlAttribute("nummer")]
        public int Nummer { get; set; }

        [XmlElement("PARAMETER")]
        public List<VarianteParameter> Parameter { get; set; }
    }

    public class VarianteParameter
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("kardinalität")]
        public string Kardinalitaet { get; set; }

        [XmlAttribute("optional_kz")]
        public string OptionalKz { get; set; }

        [XmlAttribute("default")]
        public string Default { get; set; }
    }

    public class AusgabeParameter
    {
        [XmlElement("PARAMETER")]
        public List<VarianteParameter> Parameter { get; set; }
    }

    public class ReturnCodeListe
    {
        [XmlElement("MELDUNG")]
        public List<Meldung> Meldungen { get; set; }
    }

    public class Meldung
    {
        [XmlAttribute("rc")]
        public int Rc { get; set; }

        [XmlAttribute("fk")]
        public int Fk { get; set; }

        [XmlAttribute("text")]
        public string Text { get; set; }
    }


}