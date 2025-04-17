using System.Diagnostics;

namespace NotifyAgentActMCP.Services;

#if MACOS
public partial class AudioPlayService
{
    public partial Task Play(string audioFilePath)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "afplay",
            Arguments = audioFilePath,
            RedirectStandardOutput = false,
            RedirectStandardError = false,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var process = Process.Start(psi))
        {
            process.WaitForExit();
        }

        return Task.CompletedTask;
    }
}
#endif
