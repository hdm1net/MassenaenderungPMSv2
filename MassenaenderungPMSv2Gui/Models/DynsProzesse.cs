namespace MassenaenderungPMSv2Gui.Models
{
    public class DynsProzess
    {
        public string ProcessName { get; set; }
        public string XmlFilePath { get; set; }

        public DynsProzess(string processName, string xmlFilePath)
        {
            ProcessName = processName;
            XmlFilePath = xmlFilePath;
        }
    }

    public class DynsSchnittstellenart
    {
        public string Key { get; set; }
        public string Beschreibung { get; set; }

        public DynsSchnittstellenart(string key, string beschreibung)
        {
            Key = key;
            Beschreibung = beschreibung;
        }
    }

    public class DynsAufrufvariante
    {
        public string Key { get; set; }
        public string Beschreibung { get; set; }

        public DynsAufrufvariante(string key, string beschreibung)
        {
            Key = key;
            Beschreibung = beschreibung;
        }
    }

    public class DynsAufrufvariantenParameter
    {
        private MassenaenderungPMSv2.fiServiceXmlClasses.PO.AufrufVariante PO_Aufrufvariante {  get; set; } = new MassenaenderungPMSv2.fiServiceXmlClasses.PO.AufrufVariante();
        private MassenaenderungPMSv2.fiServiceXmlClasses.PO.Parameter PO_Parameter { get; set; } = new MassenaenderungPMSv2.fiServiceXmlClasses.PO.Parameter();
        private MassenaenderungPMSv2.fiServiceXmlClasses.OO.Operation OO_Operation { get; set; } = new MassenaenderungPMSv2.fiServiceXmlClasses.OO.Operation();
        private MassenaenderungPMSv2.fiServiceXmlClasses.OO.FachObjekt OO_Fachobjekt { get; set; } = new MassenaenderungPMSv2.fiServiceXmlClasses.OO.FachObjekt();
        private MassenaenderungPMSv2.fiServiceXmlClasses.OO.Parameter OO_Parameter { get; set; } = new MassenaenderungPMSv2.fiServiceXmlClasses.OO.Parameter();

        public string Id { get; set; }
        public string Name { get; set; }
        public string OptionalKz { get; set; }
        public string EingabeFo { get; set; }
        public string Kardinalitaet {  get; set; }
        public bool IsArray { get; set; }
        public string Beschreibung { get; set; }


        public DynsAufrufvariantenParameter()
        {

            Id = String.Empty;
            Name = String.Empty;
            OptionalKz = String.Empty;
            EingabeFo = String.Empty;
            Kardinalitaet = String.Empty;
            IsArray = false;
            Beschreibung = String.Empty;

        }

        public DynsAufrufvariantenParameter(
            string id, 
            string name, 
            string optionalKz, 
            string eingabeFo, 
            string kardinalitaet, 
            bool isArray, 
            string beschreibung)
        {
            
            Id = id;
            Name = name;
            OptionalKz = optionalKz;
            EingabeFo = eingabeFo;
            Kardinalitaet = kardinalitaet;
            IsArray = isArray;
            Beschreibung = beschreibung;

        }

        public DynsAufrufvariantenParameter(MassenaenderungPMSv2.fiServiceXmlClasses.PO.VarianteParameter parameter)
        {
            Id = parameter.Name;
            Name = parameter.Name + $" [{parameter.Kardinalitaet}][{parameter.OptionalKz}]";
            OptionalKz = parameter.OptionalKz;
            EingabeFo = String.Empty; // bei PO gibt es kein Eingabefachobjekt 
            Kardinalitaet = parameter.Kardinalitaet;
            IsArray  = parameter.Kardinalitaet != "1" ? true : false;  
            Beschreibung = String.Empty;    
        }

        public DynsAufrufvariantenParameter(MassenaenderungPMSv2.fiServiceXmlClasses.OO.Parameter parameter)
        {
            Id = parameter.Name;
            Name = parameter.Name + $" [{parameter.SvzOptionalKz}]";
            OptionalKz = parameter.SvzOptionalKz;
            EingabeFo = "ToDo!";
            Kardinalitaet = "1"; // bei OO-Parametern wird die Kardinalitaet ueber das EingabeFo realisiert
            IsArray = false; // bei OO-Parametern wird die Kardinalitaet ueber das EingabeFo realisiert
            Beschreibung = String.Empty;
        }

        public static string OptionalKzFromBool(bool IsMandatory)
        {
            if (IsMandatory)
            {
                return "M";
            }
            else
            {
                return "K";
            }
        }

    }

}
