using VoicevoxCoreSharp.Core;
using VoicevoxCoreSharp.Core.Enum;
using VoicevoxCoreSharp.Core.Struct;
using VoicevoxCoreSharp.Experimental;

namespace NotifyAgentActMCP.Services;

public class VoicevoxTtsService
{
    private readonly uint _styleId;

    private readonly OpenJtalk _openJtalk;
    private readonly Onnxruntime _onnxruntime;
    private readonly Synthesizer _synthesizer;

    private VoicevoxTtsService(OpenJtalk openJtalk, Onnxruntime onnxruntime, Synthesizer synthesizer, uint styleId)
    {
        _openJtalk = openJtalk;
        _onnxruntime = onnxruntime;
        _synthesizer = synthesizer;
        _styleId = styleId;
    }

    public static async Task<VoicevoxTtsService> Build(string openJTalkDictPath, string voiceModelPath, uint styleId, string onnxruntimePath)
    {
        var initializeOptions = InitializeOptions.Default();
        var openJtalk = await OpenJtalkExtensions.NewAsync(openJTalkDictPath);

        var loadOnnxruntimeOptions = new LoadOnnxruntimeOptions(onnxruntimePath);
        var onnxruntime = await OnnxruntimeExtensions.LoadOnceAsync(loadOnnxruntimeOptions);

        var result = Synthesizer.New(onnxruntime, openJtalk, initializeOptions, out var synthesizer);
        if (result != ResultCode.RESULT_OK)
        {
            throw new Exception(result.ToMessage());
        }

        using var voiceModel = await VoiceModelFileExtensions.NewAsync(voiceModelPath);
        await synthesizer.LoadVoiceModelAsync(voiceModel);

        return new VoicevoxTtsService(openJtalk, onnxruntime, synthesizer, styleId);
    }

    public async Task<byte[]> Synthesize(string speechText)
    {
        var (_, wav) = await _synthesizer.TtsAsync(speechText, _styleId, TtsOptions.Default());

        return wav!;
    }
}
