using CreationsForge.Engine.Records;
using CreationsForge.Engine.Workspaces;
using CreationsForge.Fallout4;
using CreationsForge.Skyrim;
using CreationsForge.Starfield;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;

if ((args.Length == 6 || args.Length == 11)
    && string.Equals(args[0], "reopen", StringComparison.Ordinal))
{
    var reopenDataDirectory = Path.GetFullPath(args[1]);
    var reopenRelease = Enum.Parse<GameRelease>(args[2], ignoreCase: false);
    var reopenModKey = ModKey.FromNameAndExtension(args[3]);
    var textStorageMode = Enum.Parse<PluginTextStorageMode>(args[4], ignoreCase: false);
    var expectedRecordCount = int.Parse(args[5], System.Globalization.CultureInfo.InvariantCulture);
    var reopenOutput = new PluginOutputDefinition(
        Path.Combine(reopenDataDirectory, reopenModKey.ToString()),
        reopenModKey,
        MasterStyle.Full,
        textStorageMode,
        createNew: false);
    var reopenRequest = new PluginWorkspaceOpenRequest(reopenRelease, reopenDataDirectory, [], reopenOutput);
    var reopenFactory = new PluginWorkspaceFactory(
    [
        new StarfieldGameIntegration(),
        new Fallout4GameIntegration(),
        new SkyrimGameIntegration(),
    ]);
    using var reopenedWorkspace = reopenFactory.Open(reopenRequest);
    if (reopenedWorkspace.Output.EnumerateMajorRecords().Count() != expectedRecordCount)
    {
        return 4;
    }

    if (args.Length == 11)
    {
        var familyId = args[6];
        var originModKey = ModKey.FromNameAndExtension(args[7]);
        var reopenFormKey = new FormKey(
            originModKey,
            uint.Parse(args[8], System.Globalization.CultureInfo.InvariantCulture));
        var snapshot = reopenedWorkspace.Records.Read(new RecordLocator(familyId, reopenFormKey, reopenModKey));
        if (!snapshot.Values.TryGetValue(args[9], out var value)
            || value switch
            {
                RecordValue.StringRecordValue stringValue => !string.Equals(stringValue.Value, args[10], StringComparison.Ordinal),
                RecordValue.TranslatedStringRecordValue translatedValue => !translatedValue.Values.Values.Contains(args[10], StringComparer.Ordinal),
                _ => true,
            })
        {
            return 5;
        }
    }

    return 0;
}

if (args.Length != 2)
{
    Console.Error.WriteLine("Usage: CreationsForge.LockProbe <data-directory> <ready-path> | reopen <data-directory> <release> <output-mod-key> <text-storage> <record-count> [<family> <origin-mod-key> <form-id> <field> <expected-string>]");
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
