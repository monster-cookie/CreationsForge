using CreationsForge.Engine;
using CreationsForge.Fallout4;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;

if (args.Length != 2)
{
    Console.Error.WriteLine("Usage: CreationsForge.LockProbe <data-directory> <ready-path>");
    return 2;
}

var dataDirectory = Path.GetFullPath(args[0]);
var readyPath = Path.GetFullPath(args[1]);
var outputModKey = ModKey.FromNameAndExtension("CrossProcess.esp");
var output = new NativeOutputDefinition(
    Path.Combine(dataDirectory, outputModKey.ToString()),
    outputModKey,
    MasterStyle.Full,
    NativeTextStorageMode.Embedded,
    createNew: true);
var request = new NativeWorkspaceOpenRequest(GameRelease.Fallout4, dataDirectory, [], output);
var factory = new NativeWorkspaceFactory([new Fallout4GameIntegration()]);
using var workspace = factory.Open(request);
await File.WriteAllTextAsync(readyPath, string.Empty);
return await Console.In.ReadLineAsync() is null ? 3 : 0;
