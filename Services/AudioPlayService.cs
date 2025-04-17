using System.Diagnostics;

namespace NotifyAgentActMCP.Services;

public partial class AudioPlayService
{
    public AudioPlayService() { }

    public partial Task Play(string audioFilePath);

#if !MACOS && NET
    public partial Task Play(string audioFilePath)
    {
        // Need install sox command, play is a part of sox
        var psi = new ProcessStartInfo
        {
            FileName = "play",
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
#endif
}
