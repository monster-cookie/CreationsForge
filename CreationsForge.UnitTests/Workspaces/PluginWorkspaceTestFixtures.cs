using CreationsForge.Engine.Workspaces;
using CreationsForge.Fallout4;
using CreationsForge.Skyrim;
using CreationsForge.Starfield;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using Fallout4FormList = Mutagen.Bethesda.Fallout4.FormList;
using Fallout4FormListGetter = Mutagen.Bethesda.Fallout4.IFormListGetter;
using Fallout4Mod = Mutagen.Bethesda.Fallout4.Fallout4Mod;
using Fallout4Release = Mutagen.Bethesda.Fallout4.Fallout4Release;
using SkyrimFormList = Mutagen.Bethesda.Skyrim.FormList;
using SkyrimFormListGetter = Mutagen.Bethesda.Skyrim.IFormListGetter;
using SkyrimMod = Mutagen.Bethesda.Skyrim.SkyrimMod;
using SkyrimRelease = Mutagen.Bethesda.Skyrim.SkyrimRelease;
using StarfieldFormList = Mutagen.Bethesda.Starfield.FormList;
using StarfieldFormListGetter = Mutagen.Bethesda.Starfield.IFormListGetter;
using StarfieldMod = Mutagen.Bethesda.Starfield.StarfieldMod;
using StarfieldRelease = Mutagen.Bethesda.Starfield.StarfieldRelease;

namespace CreationsForge.UnitTests.Workspaces;

public sealed partial class PluginWorkspaceTests
{
    private static PluginWorkspaceFactory CreateFactory()
    {
        return new PluginWorkspaceFactory(
        [
            new StarfieldGameIntegration(),
            new Fallout4GameIntegration(),
            new SkyrimGameIntegration(),
        ]);
    }

    private static PluginWorkspaceOpenRequest CreateNewRequest(
        string dataDirectory,
        GameRelease release,
        IEnumerable<ModKey> selectedPlugins,
        ModKey outputModKey)
    {
        var output = new PluginOutputDefinition(
            Path.Combine(dataDirectory, outputModKey.ToString()),
            outputModKey,
            MasterStyle.Full,
            PluginTextStorageMode.Embedded,
            createNew: true);
        return new PluginWorkspaceOpenRequest(release, dataDirectory, selectedPlugins, output);
    }

    private static Fixture CreateFixture(string directory, GameRelease release)
    {
        return release switch
        {
            GameRelease.Starfield => CreateStarfieldFixture(directory),
            GameRelease.Fallout4 => CreateFallout4Fixture(directory),
            GameRelease.SkyrimSE => CreateSkyrimFixture(directory),
            _ => throw new ArgumentOutOfRangeException(nameof(release), release, null),
        };
    }

    private static Fixture CreateStarfieldFixture(string directory)
    {
        var masterModKey = ModKey.FromNameAndExtension("Starfield.esm");
        var selectedModKey = ModKey.FromNameAndExtension("Selected.esp");
        var master = new StarfieldMod(masterModKey, StarfieldRelease.Starfield);
        var record = master.FormLists.AddNew();
        record.EditorID = "WorkspaceFixture";
        var formKey = record.FormKey;
        WritePlugin(master, Path.Combine(directory, masterModKey.ToString()));

        var selected = new StarfieldMod(selectedModKey, StarfieldRelease.Starfield);
        ((IMod)selected).MasterReferences.Add(new MasterReference { Master = masterModKey });
        selected.FormLists.Add((StarfieldFormList)record.DeepCopy());
        WritePlugin(selected, Path.Combine(directory, selectedModKey.ToString()), master);
        return new Fixture(masterModKey, selectedModKey, formKey, typeof(StarfieldFormListGetter));
    }

    private static Fixture CreateFallout4Fixture(string directory)
    {
        var masterModKey = ModKey.FromNameAndExtension("Base.esm");
        var selectedModKey = ModKey.FromNameAndExtension("Selected.esp");
        var master = new Fallout4Mod(masterModKey, Fallout4Release.Fallout4);
        var record = master.FormLists.AddNew();
        record.EditorID = "WorkspaceFixture";
        var formKey = record.FormKey;
        WritePlugin(master, Path.Combine(directory, masterModKey.ToString()));

        var selected = new Fallout4Mod(selectedModKey, Fallout4Release.Fallout4);
        ((IMod)selected).MasterReferences.Add(new MasterReference { Master = masterModKey });
        selected.FormLists.Add((Fallout4FormList)record.DeepCopy());
        WritePlugin(selected, Path.Combine(directory, selectedModKey.ToString()));
        return new Fixture(masterModKey, selectedModKey, formKey, typeof(Fallout4FormListGetter));
    }

    private static Fixture CreateSkyrimFixture(string directory)
    {
        var masterModKey = ModKey.FromNameAndExtension("Base.esm");
        var selectedModKey = ModKey.FromNameAndExtension("Selected.esp");
        var master = new SkyrimMod(masterModKey, SkyrimRelease.SkyrimSE);
        var record = master.FormLists.AddNew();
        record.EditorID = "WorkspaceFixture";
        var formKey = record.FormKey;
        WritePlugin(master, Path.Combine(directory, masterModKey.ToString()));

        var selected = new SkyrimMod(selectedModKey, SkyrimRelease.SkyrimSE);
        ((IMod)selected).MasterReferences.Add(new MasterReference { Master = masterModKey });
        selected.FormLists.Add((SkyrimFormList)record.DeepCopy());
        WritePlugin(selected, Path.Combine(directory, selectedModKey.ToString()));
        return new Fixture(masterModKey, selectedModKey, formKey, typeof(SkyrimFormListGetter));
    }

    private static void WriteEmptyPlugin(string path, ModKey modKey, GameRelease release)
    {
        IMod plugin = release switch
        {
            GameRelease.Starfield => new StarfieldMod(modKey, StarfieldRelease.Starfield),
            GameRelease.Fallout4 => new Fallout4Mod(modKey, Fallout4Release.Fallout4),
            GameRelease.SkyrimSE => new SkyrimMod(modKey, SkyrimRelease.SkyrimSE),
            _ => throw new ArgumentOutOfRangeException(nameof(release), release, null),
        };
        WritePlugin(plugin, path);
    }

    private static void WritePlugin(IModGetter plugin, string path, params IModMasterStyledGetter[] masters)
    {
        var masterFlags = new Cache<IModMasterStyledGetter, ModKey>(
            master => master.ModKey,
            EqualityComparer<ModKey>.Default);
        foreach (var master in masters)
        {
            masterFlags.Add(master);
        }

        plugin.WriteToBinary(path, new BinaryWriteParameters
        {
            MastersListContent = MastersListContentOption.NoCheck,
            MasterFlagsLookup = masterFlags,
        });
    }

    private sealed class Fixture
    {
        public Fixture(ModKey masterModKey, ModKey selectedModKey, FormKey formKey, Type recordGetterType)
        {
            MasterModKey = masterModKey;
            SelectedModKey = selectedModKey;
            FormKey = formKey;
            RecordGetterType = recordGetterType;
        }

        public ModKey MasterModKey { get; }

        public ModKey SelectedModKey { get; }

        public FormKey FormKey { get; }

        public Type RecordGetterType { get; }
    }

    private sealed class TemporaryDirectory : IDisposable
    {
        public TemporaryDirectory()
        {
            Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"CreationsForge-{Guid.NewGuid():N}");
            Directory.CreateDirectory(Path);
        }

        public string Path { get; }

        public void Dispose()
        {
            Directory.Delete(Path, recursive: true);
        }
    }
}
