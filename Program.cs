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

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

builder.Services
    .AddSingleton(sp =>
    {
        // REPLACE_HERE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        var openJTalkDictPath = "/Users/yamachu/Projects/github.com/yamachu/NotifyAgentActMCP/voicevox_core/dict/open_jtalk_dic_utf_8-1.11";
        var voiceModelPath = "/Users/yamachu/Projects/github.com/yamachu/NotifyAgentActMCP/voicevox_core/models/vvms/0.vvm";
        var onnxruntimePath = "/Users/yamachu/Projects/github.com/yamachu/NotifyAgentActMCP/bin/Debug/net9.0/libvoicevox_onnxruntime.1.17.3.dylib";
        uint styleId = 1; // REPLACE_HERE

        var service = VoicevoxTtsService.Build(openJTalkDictPath, voiceModelPath, styleId, onnxruntimePath).Result;
        return service;
    })
    .AddSingleton<AudioPlayService>();

await builder.Build().RunAsync();
