using System.ComponentModel;
using System.Diagnostics;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using NotifyAgentActMCP.Services;

namespace NotifyAgentActMCP.Tools;

[McpServerToolType]
public sealed class VoicevoxSpeechTools
{
    [McpServerTool, Description("Speech text with Voicevox")]
    public static Task Speech(
        IMcpServer server,
        VoicevoxTtsService ttsService,
        AudioPlayService audioPlayService,
        [Description("What to speech.")] string speechText)
    {
        Task.Run(async () =>
        {
            var tempFilePath = System.IO.Path.GetTempFileName();

            try
            {
                var wav = await ttsService.Synthesize(speechText);
                using var writer = new BinaryWriter(File.OpenWrite(tempFilePath));
                writer.Write(wav!);

                await audioPlayService.Play(tempFilePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await server.SendNotificationAsync("VoicevoxSpeechTools/SynthesizeError", ex.Message);
            }
            finally
            {
                if (File.Exists(tempFilePath))
                    File.Delete(tempFilePath);
            }
        });

        return Task.CompletedTask;
    }
}
