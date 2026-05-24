using SFRecordCompareEngine.Core.Database;
using SFRecordCompareEngine.Core.Database.Interfaces;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Core.Repositories;
using SFRecordCompareEngine.Migrations;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Repositories;

public class PluginRepositoryTests : IDisposable
{
    private readonly string DatabaseDirectory = Path.Combine(Path.GetTempPath(), "SFRecordCompareEngineTests", Guid.NewGuid().ToString("N"));
    private readonly ISqliteConnectionFactory ConnectionFactory;
    private readonly PluginRepository Sut = new();

    public PluginRepositoryTests()
    {
        var options = new SqliteDatabaseOptions
        {
            DatabaseDirectory = DatabaseDirectory
        };

        ConnectionFactory = new SqliteConnectionFactory(options);
        new DatabaseSchemaInitializer(ConnectionFactory, new DatabaseMigrationRunner()).Initialize();
    }

    public void Dispose()
    {
        if (Directory.Exists(DatabaseDirectory))
        {
            Directory.Delete(DatabaseDirectory, true);
        }
    }

    [Fact]
    public void UpsertPlugin_WhenPluginIsNew_InsertsRow()
    {
        using var database = ConnectionFactory.OpenDatabase();

        Sut.UpsertPlugin(database, CreatePlugin("Example.esm", 7));

        var result = Sut.GetByModKey(database, "Example.esm");
        result.ShouldNotBeNull();
        result.LoadOrderIndex.ShouldBe(7);
    }

    [Fact]
    public void UpsertPlugin_WhenPluginExists_UpdatesRow()
    {
        using var database = ConnectionFactory.OpenDatabase();
        Sut.UpsertPlugin(database, CreatePlugin("Example.esm", 7));

        var updatedPlugin = CreatePlugin("Example.esm", 9);
        updatedPlugin.Author = "Updated";
        Sut.UpsertPlugin(database, updatedPlugin);

        var result = Sut.GetByModKey(database, "Example.esm");
        result.ShouldNotBeNull();
        result.LoadOrderIndex.ShouldBe(9);
        result.Author.ShouldBe("Updated");
    }

    [Fact]
    public void UpsertPlugin_WhenModKeyDiffersOnlyByCase_UpdatesExistingRow()
    {
        using var database = ConnectionFactory.OpenDatabase();
        Sut.UpsertPlugin(database, CreatePlugin("Starfield.esm", 0));

        var updatedPlugin = CreatePlugin("starfield.esm", 5);
        updatedPlugin.PluginFileName = "starfield.esm";
        Sut.UpsertPlugin(database, updatedPlugin);

        var count = database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM Plugins WHERE ModKey = @0 COLLATE NOCASE;",
            "Starfield.esm");
        var result = Sut.GetByModKey(database, "Starfield.esm");

        count.ShouldBe(1);
        result.ShouldNotBeNull();
        result.LoadOrderIndex.ShouldBe(5);
        result.PluginFileName.ShouldBe("starfield.esm");
    }

    [Fact]
    public void UpsertMissingPlaceholder_WhenModKeyDiffersOnlyByCase_DoesNotInsertDuplicate()
    {
        using var database = ConnectionFactory.OpenDatabase();
        Sut.UpsertPlugin(database, CreatePlugin("SFBGS004.esm", 9));

        Sut.UpsertMissingPlaceholder(database, "sfbgs004.esm", DateTimeOffset.UtcNow.ToString("O"));

        var count = database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM Plugins WHERE ModKey = @0 COLLATE NOCASE;",
            "SFBGS004.esm");
        var result = Sut.GetByModKey(database, "sfbgs004.esm");

        count.ShouldBe(1);
        result.ShouldNotBeNull();
        result.LoadOrderIndex.ShouldBe(9);
        result.ExistsOnDisk.ShouldBeTrue();
    }

    [Fact]
    public void GetPlugins_WhenRowsExist_ReturnsPluginsOrderedByLoadOrder()
    {
        using var database = ConnectionFactory.OpenDatabase();
        Sut.UpsertPlugin(database, CreatePlugin("Second.esm", 2));
        Sut.UpsertPlugin(database, CreatePlugin("First.esm", 1));

        var result = Sut.GetPlugins(database);

        result.Select(plugin => plugin.PluginFileName).ShouldBe(["First.esm", "Second.esm"]);
    }

    [Fact]
    public void SearchPlugins_WhenSearchTextMatchesMultipleRows_ReturnsAllMatchesCaseInsensitively()
    {
        using var database = ConnectionFactory.OpenDatabase();
        Sut.UpsertPlugin(database, CreatePlugin("Venworks-MyExperiments.esm", 68));
        Sut.UpsertPlugin(database, CreatePlugin("Venworks-encountersoverhaul.esm", 110));
        Sut.UpsertPlugin(database, CreatePlugin("Other.esm", 111));

        var result = Sut.SearchPlugins(database, "venworks");

        result.Select(plugin => plugin.PluginFileName).ShouldBe([
            "Venworks-MyExperiments.esm",
            "Venworks-encountersoverhaul.esm"
        ]);
    }

    [Fact]
    public void SearchOpenablePlugins_WhenSearchMatchesCurrentAndFailedRows_ReturnsBoth()
    {
        using var database = ConnectionFactory.OpenDatabase();
        Sut.UpsertPlugin(database, CreatePlugin("Venworks-MyExperiments.esm", 68));
        var failedPlugin = CreatePlugin("Venworks-encountersoverhaul.esm", 110);
        failedPlugin.ImportState = PluginImportState.Failed.ToString();
        Sut.UpsertPlugin(database, failedPlugin);
        Sut.UpsertPlugin(database, CreatePlugin("Other.esm", 111));

        var result = Sut.SearchOpenablePlugins(database, "venworks");

        result.Select(plugin => plugin.PluginFileName).ShouldBe([
            "Venworks-MyExperiments.esm",
            "Venworks-encountersoverhaul.esm"
        ]);
        result[1].ImportState.ShouldBe(PluginImportState.Failed.ToString());
    }

    [Fact]
    public void SearchOpenablePlugins_WhenPluginIsMissingOrNotOnDisk_ExcludesPlugin()
    {
        using var database = ConnectionFactory.OpenDatabase();
        var failedPlugin = CreatePlugin("Venworks-Failed.esm", 68);
        failedPlugin.ImportState = PluginImportState.Failed.ToString();
        Sut.UpsertPlugin(database, failedPlugin);
        var missingPlugin = CreatePlugin("Venworks-Missing.esm", 69);
        missingPlugin.ImportState = PluginImportState.Missing.ToString();
        Sut.UpsertPlugin(database, missingPlugin);
        var notOnDiskPlugin = CreatePlugin("Venworks-NotOnDisk.esm", 70);
        notOnDiskPlugin.ExistsOnDisk = false;
        Sut.UpsertPlugin(database, notOnDiskPlugin);

        var result = Sut.SearchOpenablePlugins(database, "venworks");

        result.Select(plugin => plugin.PluginFileName).ShouldBe([
            "Venworks-Failed.esm"
        ]);
    }

    [Fact]
    public void SearchOpenablePlugins_WhenPluginIsUnsupported_ExcludesPlugin()
    {
        using var database = ConnectionFactory.OpenDatabase();
        var unsupportedPlugin = CreatePlugin("BlueprintShips.esm", 68);
        unsupportedPlugin.ImportState = PluginImportState.Unsupported.ToString();
        Sut.UpsertPlugin(database, unsupportedPlugin);
        Sut.UpsertPlugin(database, CreatePlugin("BlueprintShipsPatch.esm", 69));

        var result = Sut.SearchOpenablePlugins(database, "BlueprintShips");

        result.Select(plugin => plugin.PluginFileName).ShouldBe([
            "BlueprintShipsPatch.esm"
        ]);
    }

    [Fact]
    public void ReplaceMasterReferences_WhenPluginIsReimported_ReplacesOnlyThatPluginsRows()
    {
        using var database = ConnectionFactory.OpenDatabase();
        Sut.UpsertPlugin(database, CreatePlugin("Child.esm", 2));
        Sut.UpsertPlugin(database, CreatePlugin("OtherChild.esm", 3));
        Sut.UpsertPlugin(database, CreatePlugin("ParentA.esm", 0));
        Sut.UpsertPlugin(database, CreatePlugin("ParentB.esm", 1));

        Sut.ReplaceMasterReferences(database, "Child.esm", [CreateMaster("Child.esm", "ParentA.esm", 0, 0)]);
        Sut.ReplaceMasterReferences(database, "OtherChild.esm", [CreateMaster("OtherChild.esm", "ParentA.esm", 0, 0)]);

        Sut.ReplaceMasterReferences(database, "Child.esm", [CreateMaster("Child.esm", "ParentB.esm", 0, 1)]);

        Sut.GetMasterReferences(database, "Child.esm").Single().ParentModKey.ShouldBe("ParentB.esm");
        Sut.GetMasterReferences(database, "OtherChild.esm").Single().ParentModKey.ShouldBe("ParentA.esm");
    }

    [Fact]
    public void RefreshParentLoadOrderIndexes_WhenParentModKeyDiffersOnlyByCase_UpdatesParentLoadOrderIndex()
    {
        using var database = ConnectionFactory.OpenDatabase();
        Sut.UpsertPlugin(database, CreatePlugin("Child.esm", 2));
        Sut.UpsertPlugin(database, CreatePlugin("SFBGS004.esm", 9));
        Sut.ReplaceMasterReferences(database, "Child.esm", [CreateMaster("Child.esm", "sfbgs004.esm", 0, 0)]);

        Sut.RefreshParentLoadOrderIndexes(database);

        Sut.GetMasterReferences(database, "Child.esm").Single().ParentLoadOrderIndex.ShouldBe(9);
    }

    [Fact]
    public void GetResolutionHierarchy_WhenRowsExist_ReturnsParentsAndChildOrderedByLoadOrder()
    {
        using var database = ConnectionFactory.OpenDatabase();
        Sut.UpsertPlugin(database, CreatePlugin("ParentA.esm", 1));
        Sut.UpsertPlugin(database, CreatePlugin("ParentB.esm", 2));
        Sut.UpsertPlugin(database, CreatePlugin("Child.esm", 3));
        Sut.ReplaceMasterReferences(database, "Child.esm", [
            CreateMaster("Child.esm", "ParentB.esm", 0, 2),
            CreateMaster("Child.esm", "ParentA.esm", 1, 1)
        ]);

        var result = Sut.GetResolutionHierarchy(database, "Child.esm");

        result.Select(plugin => plugin.HierarchyModKey).ShouldBe(["ParentA.esm", "ParentB.esm", "Child.esm"]);
    }

    private static PluginMetadataDTO CreatePlugin(string modKey, int loadOrderIndex)
    {
        return new PluginMetadataDTO
        {
            ModKey = modKey,
            GameRelease = "Starfield",
            LoadOrderIndex = loadOrderIndex,
            PluginFileName = modKey,
            Enabled = true,
            ExistsOnDisk = true,
            ImportState = PluginImportState.Current.ToString(),
            LastCheckedUtc = DateTimeOffset.UtcNow.ToString("O")
        };
    }

    private static PluginMasterReferenceDTO CreateMaster(string modKey, string parentModKey, int index, int parentLoadOrderIndex)
    {
        return new PluginMasterReferenceDTO
        {
            ModKey = modKey,
            ParentModKey = parentModKey,
            MasterReferenceIndex = index,
            ParentLoadOrderIndex = parentLoadOrderIndex,
            ImportedAtUtc = DateTimeOffset.UtcNow.ToString("O")
        };
    }
}
