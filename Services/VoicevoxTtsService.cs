using VoicevoxCoreSharp.Core;
using VoicevoxCoreSharp.Core.Enum;
using VoicevoxCoreSharp.Core.Struct;
using VoicevoxCoreSharp.Experimental;

namespace NotifyAgentActMCP.Services;

public class VoicevoxTtsService
{
    private readonly string _openJTalkDictPath;
    private readonly string _voiceModelPath;
    private readonly uint _styleId;
    private readonly string _onnxruntimePath;

    private OpenJtalk? _openJtalk;
    private Onnxruntime? _onnxruntime;
    private Synthesizer? _synthesizer;

    private bool _isInitialized = false;

    public VoicevoxTtsService(string openJTalkDictPath, string voiceModelPath, uint styleId, string onnxruntimePath)
    {
        _openJTalkDictPath = openJTalkDictPath;
        _voiceModelPath = voiceModelPath;
        _styleId = styleId;
        _onnxruntimePath = onnxruntimePath;
    }

    public async Task Setup()
    {
        var initializeOptions = InitializeOptions.Default();
        _openJtalk = await OpenJtalkExtensions.NewAsync(_openJTalkDictPath);

        var loadOnnxruntimeOptions = new LoadOnnxruntimeOptions(_onnxruntimePath);
        _onnxruntime = await OnnxruntimeExtensions.LoadOnceAsync(loadOnnxruntimeOptions);

        var result = Synthesizer.New(_onnxruntime, _openJtalk, initializeOptions, out _synthesizer);
        if (result != ResultCode.RESULT_OK)
        {
            throw new Exception(result.ToMessage());
        }

        using var voiceModel = await VoiceModelFileExtensions.NewAsync(_voiceModelPath);
        await _synthesizer.LoadVoiceModelAsync(voiceModel);

        _isInitialized = true;
    }

    public async Task<byte[]> Synthesize(string speechText)
    {
        if (!_isInitialized)
        {
            throw new InvalidOperationException("VoicevoxTtsService is not initialized. Call Setup() first.");
        }

        var (_, wav) = await _synthesizer!.TtsAsync(speechText, _styleId, TtsOptions.Default());

        return wav!;
    }
}
