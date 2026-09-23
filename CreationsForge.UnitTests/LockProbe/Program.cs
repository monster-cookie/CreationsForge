using CreationsForge.Engine.Workspaces;
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
var output = new PluginOutputDefinition(
    Path.Combine(dataDirectory, outputModKey.ToString()),
    outputModKey,
    MasterStyle.Full,
    PluginTextStorageMode.Embedded,
    createNew: true);
var request = new PluginWorkspaceOpenRequest(GameRelease.Fallout4, dataDirectory, [], output);
var factory = new PluginWorkspaceFactory([new Fallout4GameIntegration()]);
using var workspace = factory.Open(request);
await File.WriteAllTextAsync(readyPath, string.Empty);
return await Console.In.ReadLineAsync() is null ? 3 : 0;
