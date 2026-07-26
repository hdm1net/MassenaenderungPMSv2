using System.Diagnostics;
using System.Text;

namespace MassenaenderungPMSv2Gui.Services
{
    public interface IExeService
    {
        void StartProcess(string exePath, string arguments, Action<string> onOutput, Action<string> onError);
    }

    public class ExeService : IExeService
    {
        public void StartProcess(string exePath, string arguments, Action<string> onOutput, Action<string> onError)
        {

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C chcp 1252 & \"{exePath}\" {arguments}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.GetEncoding(1252),
                StandardErrorEncoding = Encoding.GetEncoding(1252)
            };

            var process = new Process();
            process.StartInfo = psi;

            process.OutputDataReceived += (s, e) =>
            {
                if (e.Data != null)
                    onOutput?.Invoke(e.Data);
            };

            process.ErrorDataReceived += (s, e) =>
            {
                if (e.Data != null)
                    onError?.Invoke(e.Data);
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }
    }
}
