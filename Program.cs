using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotifyAgentActMCP.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.AddConsole(consoleLogOptions =>
{
    // Configure all logs to go to stderr
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
});
builder.Configuration.AddEnvironmentVariables();

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

builder.Services
    .AddSingleton(_ =>
    {
        var config = builder.Configuration;
        var openJTalkDictPath = config["OPEN_JTALK_DICT_PATH"]!;
        var voiceModelPath = config["VOICE_MODEL_PATH"]!;
        var onnxruntimePath = config["ONNXRUNTIME_PATH"]!;
        uint styleId = uint.TryParse(config["VOICEVOX_STYLE_ID"], out var sid) ? sid : 1;

        var service = VoicevoxTtsService.Build(openJTalkDictPath, voiceModelPath, styleId, onnxruntimePath).Result;
        return service;
    })
    .AddSingleton<AudioPlayService>();

await builder.Build().RunAsync();
