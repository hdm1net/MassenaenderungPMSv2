namespace MassenaenderungPMSv2Gui.Models.Importspezifikation
{
    public class Eingabeparameter
    {
        public string Name { get; set; }
        public string EingabeFo { get; set; }
        public string Lfdn { get; set; }
        public string IsArray { get; set; }
        public string Datenspaltenname { get; set; }
        
        public Eingabeparameter(string name, string eingabefo, string lfdn, string isarray, string datenspaltenname) 
        { 
            Name = name;
            EingabeFo = eingabefo;
            Lfdn = lfdn;
            IsArray = isarray;  
            Datenspaltenname = datenspaltenname; 
        
        }
    }
}
