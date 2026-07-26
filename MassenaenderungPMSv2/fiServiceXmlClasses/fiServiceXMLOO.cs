using System.Xml.Serialization;

namespace MassenaenderungPMSv2.fiServiceXmlClasses.OO
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

        [XmlAttribute("mdbAliasSc")]
        public string MdbAliasSc { get; set; }

        [XmlAttribute("implementierungseinheit")]
        public string Implementierungseinheit { get; set; }

        [XmlAttribute("gruppe")]
        public string Gruppe { get; set; }

        [XmlAttribute("schnittstellen_art")]
        public string SchnittstellenArt { get; set; }

        [XmlElement("PROZESS_INFO")]
        public ProzessInfo ProzessInfo { get; set; }

        [XmlArray("PARAMETER_UEBERSICHT")]
        [XmlArrayItem("PARAMETER")]
        public List<Parameter> ParameterUebersicht { get; set; }

        [XmlArray("RETURNCODELISTE")]
        [XmlArrayItem("MELDUNG")]
        public List<Meldung> ReturnCodeListe { get; set; }

        [XmlArray("OPERATIONEN")]
        [XmlArrayItem("OP")]
        public List<Operation> Operationen { get; set; }

        [XmlArray("FACHOBJEKTE")]
        [XmlArrayItem("FO")]
        public List<FachObjekt> FachObjekte { get; set; }
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

        [XmlElement("PROTOKOLLIERUNG")]
        public string Protokollierung { get; set; }

        [XmlElement("MIGRATIONS_KZ")]
        public string MigrationsKz { get; set; }

        [XmlElement("ANSPRECHPARTNER")]
        public string Ansprechpartner { get; set; }

        [XmlElement("OO_SCHNITTSTELLE_VEROEFFENTLICHT")]
        public string OoSchnittstelleVeroeffentlicht { get; set; }
    }

    public class Parameter
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("typ")]
        public string Typ { get; set; }

        [XmlAttribute("laenge")]
        public string Laenge { get; set; }

        [XmlAttribute("modelldatentyp")]
        public string ModellDatentyp { get; set; }

        [XmlAttribute("svz_id")]
        public string SvzId { get; set; }

        [XmlAttribute("svz_optional_kz")]
        public string SvzOptionalKz { get; set; }

        [XmlAttribute("beschreibung")]
        public string Beschreibung { get; set; }
    }

    public class Meldung
    {
        [XmlAttribute("rc")]
        public string Rc { get; set; }

        [XmlAttribute("fk")]
        public string Fk { get; set; }

        [XmlAttribute("text")]
        public string Text { get; set; }
    }

    public class Operation
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("eingabe-fo-refid")]
        public string EingabeFoRefId { get; set; }

        [XmlAttribute("ausgabe-fo-refid")]
        public string AusgabeFoRefId { get; set; }

        [XmlElement("BESCHREIBUNG")]
        public string Beschreibung { get; set; }
    }

    public class FachObjekt
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("basistyp")]
        public string BasisTyp { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("BESCHREIBUNG")]
        public string Beschreibung { get; set; }

        [XmlElement("PARAMETER_REF")]
        public List<ParameterRef> ParameterRefs { get; set; }

        [XmlElement("FO_REF")]
        public List<FoRef> FoRefs { get; set; }
    }

    public class ParameterRef
    {
        [XmlAttribute("parameter-refid")]
        public string ParameterRefId { get; set; }

        [XmlAttribute("isMandatory")]
        public bool IsMandatory { get; set; }

        [XmlAttribute("default")]
        public string Default { get; set; }
    }

    public class FoRef
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("isPrimaereListe")]
        public bool IsPrimaereListe { get; set; }

        [XmlAttribute("maximaleKardinalitaet")]
        public int MaximaleKardinalitaet { get; set; }

        [XmlAttribute("isMandatory")]
        public bool IsMandatory { get; set; }

        [XmlAttribute("fo-refid")]
        public string FoRefId { get; set; }
    }

}