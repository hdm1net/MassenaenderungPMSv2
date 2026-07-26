using System.Text;

namespace MassenaenderungPMSv2.Helper
{
    internal class Logging
    {
        private MyConfiguration configuration = new MyConfiguration();

        internal Logging()
        {
            // Initialisierung
            configuration = MyConfiguration.GetConfiguration();
        }

        internal void Add(string logText, string logFile)
        {

            try
            {

                using (var writer = new StreamWriter(logFile, append: true, Encoding.UTF8))
                {
                    writer.WriteLine($"{DateTime.Now:dd.MM.yyyy HH:mm:ss:ffff} {logText}");
                }

                if (string.Equals(configuration.Global_Log2Console, "true", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"{DateTime.Now:dd.MM.yyyy HH:mm:ss:ffff} {logText}");
                }

            }
            catch (Exception e)
            {
                throw new IOException($"Fehler beim Schreiben in Logdatei {logFile}", e);
            }

        }

        internal void Truncate(int maxLines, string logFile)
        {

            try
            {
                var logFileInfo = new FileInfo(logFile);
                
                if (!logFileInfo.Exists) return;

                var logLines = File.ReadAllLines(logFileInfo.FullName).ToList();

                if (logLines.Count > maxLines)
                {
                    logLines = logLines.Skip(logLines.Count - maxLines).ToList();
                    File.WriteAllLines(logFileInfo.FullName, logLines);
                }

            }
            catch (Exception e)
            {
                throw new IOException($"Fehler beim Schreiben in Logdatei {logFile}", e);
            }
        }
    }
}
