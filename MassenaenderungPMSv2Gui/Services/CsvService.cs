using MassenaenderungPMSv2Gui.Models;
using System.Data;


namespace MassenaenderungPMSv2Gui.Services
{
    public interface ICsvService
    {
        IEnumerable<CsvFileColumns> LoadCsvFileHeader(string path, string delimiter, string qualifier);
    }

    public class CsvService : ICsvService
    {
        private List<CsvFileColumns> _CsvFileHeader { get; set; } = new List<CsvFileColumns>();

        public IEnumerable<CsvFileColumns> LoadCsvFileHeader(string path, string delimiter, string qualifier)
        {
            try
            {
                _CsvFileHeader.Clear();
                var dt = new DataTable();

                dt = MassenaenderungPMSv2.Helper.ImpSpezXml.ImpSpezXmlFile.ReadAndConvertCsvToDataTableHeaderAsync(path, delimiter, qualifier).Result;

                foreach (DataColumn col in dt.Columns) 
                {
                    _CsvFileHeader.Add(new CsvFileColumns(col.ColumnName, col.ColumnName));
                }

                return _CsvFileHeader;

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.ToString(), ex);
            }
        }

    }

}
