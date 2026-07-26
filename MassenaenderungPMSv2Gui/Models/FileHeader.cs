namespace MassenaenderungPMSv2Gui.Models
{
    public class CsvFileColumns
    {
        public string Id { get; set; }
        public string ColumnName { get; set; }

        public CsvFileColumns(string id, string columnname)
        {
            Id = id;
            ColumnName = columnname;    
        }
    }
}
