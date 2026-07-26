using Microsoft.Win32;

namespace MassenaenderungPMSv2Gui.Services
{
    public interface IFileDialogService
    {
        string OpenCsvFile();
        string OpenXmlFile();
    }

    public class FileDialogService : IFileDialogService
    {
        public string OpenCsvFile()
        {
            var dlg = new OpenFileDialog
            {
                Filter = "CSV Dateien (*.csv)|*.csv",
                Title = "CSV-Datei auswählen"
            };

            return dlg.ShowDialog() == true ? dlg.FileName : null;
        }

        public string OpenXmlFile()
        {
            var dlg = new OpenFileDialog
            {
                Filter = "XML Dateien (*.xml)|*.xml",
                Title = "XML-Datei auswählen"
            };

            return dlg.ShowDialog() == true ? dlg.FileName : null;
        }
    }

}
