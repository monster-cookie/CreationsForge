using Moq;
using SFRecordCompareEngine.Core.Database;
using SFRecordCompareEngine.Core.Database.Interfaces;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Core.Repositories;
using SFRecordCompareEngine.Core.Services;
using SFRecordCompareEngine.Core.Services.Interfaces;
using SFRecordCompareEngine.Migrations;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Services;

public class PluginImportServiceTests : IDisposable
{
    private readonly string DatabaseDirectory = Path.Combine(Path.GetTempPath(), "SFRecordCompareEngineTests", Guid.NewGuid().ToString("N"));
    private readonly ISqliteConnectionFactory ConnectionFactory;
    private readonly PluginRepository PluginRepository = new();
    private readonly Mock<IPluginService> PluginService = new();
    private readonly DatabaseSchemaInitializer SchemaInitializer;

    public PluginImportServiceTests()
    {
        var options = new SqliteDatabaseOptions
        {
            DatabaseDirectory = DatabaseDirectory
        };

        ConnectionFactory = new SqliteConnectionFactory(options);
        SchemaInitializer = new DatabaseSchemaInitializer(ConnectionFactory, new DatabaseMigrationRunner());
    }

    public void Dispose()
    {
        if (Directory.Exists(DatabaseDirectory))
        {
            Directory.Delete(DatabaseDirectory, true);
        }
    }

    [Fact]
    public async Task InitializeAndImportAsync_WhenPluginIsNew_ImportsPluginAndMasterReferences()
    {
        var parentPath = CreatePluginFile("Parent.esm", "parent");
        var childPath = CreatePluginFile("Child.esm", "child");
        PluginService.Setup(service => service.GetLoadOrder()).Returns([
            CreateLoadOrderEntry("Parent.esm", parentPath, 0),
            CreateLoadOrderEntry("Child.esm", childPath, 1)
        ]);
        PluginService.Setup(service => service.ReadHeader(parentPath)).Returns(CreateHeader("Parent.esm"));
        PluginService.Setup(service => service.ReadHeader(childPath)).Returns(CreateHeader("Child.esm", "Parent.esm"));
        var sut = CreateSut();

        var result = await sut.InitializeAndImportAsync(CancellationToken.None);

        result.PluginsImported.ShouldBe(2);
        result.MasterReferencesImported.ShouldBe(1);
        using var database = ConnectionFactory.OpenDatabase();
        PluginRepository.GetMasterReferences(database, "Child.esm").Single().ParentModKey.ShouldBe("Parent.esm");
    }

    [Fact]
    public async Task InitializeAndImportAsync_WhenPluginFingerprintIsUnchanged_DoesNotReadHeaderAgainAndRefreshesLoadOrder()
    {
        var pluginPath = CreatePluginFile("Example.esm", "plugin");
        SeedImportedPlugin("Example.esm", pluginPath, 1);
        PluginService.Setup(service => service.GetLoadOrder()).Returns([CreateLoadOrderEntry("Example.esm", pluginPath, 9)]);
        var sut = CreateSut();

        var result = await sut.InitializeAndImportAsync(CancellationToken.None);

        result.PluginsUnchanged.ShouldBe(1);
        PluginService.Verify(service => service.ReadHeader(It.IsAny<string>()), Times.Never);
        using var database = ConnectionFactory.OpenDatabase();
        PluginRepository.GetByModKey(database, "Example.esm")!.LoadOrderIndex.ShouldBe(9);
    }

    [Fact]
    public async Task InitializeAndImportAsync_WhenPluginFileIsMissing_MarksPluginMissing()
    {
        var missingPath = Path.Combine(DatabaseDirectory, "Missing.esm");
        PluginService.Setup(service => service.GetLoadOrder()).Returns([CreateLoadOrderEntry("Missing.esm", missingPath, 1)]);
        var sut = CreateSut();

        await sut.InitializeAndImportAsync(CancellationToken.None);

        using var database = ConnectionFactory.OpenDatabase();
        var plugin = PluginRepository.GetByModKey(database, "Missing.esm");
        plugin.ShouldNotBeNull();
        plugin.ExistsOnDisk.ShouldBeFalse();
        plugin.ImportState.ShouldBe(PluginImportState.Missing.ToString());
    }

    [Fact]
    public async Task InitializeAndImportAsync_WhenPluginFileSizeChanges_ReimportsPlugin()
    {
        var pluginPath = CreatePluginFile("Example.esm", "plugin");
        SeedImportedPlugin("Example.esm", pluginPath, 1);
        File.AppendAllText(pluginPath, "changed");
        PluginService.Setup(service => service.GetLoadOrder()).Returns([CreateLoadOrderEntry("Example.esm", pluginPath, 1)]);
        PluginService.Setup(service => service.ReadHeader(pluginPath)).Returns(CreateHeader("Example.esm"));
        var sut = CreateSut();

        var result = await sut.InitializeAndImportAsync(CancellationToken.None);

        result.PluginsChanged.ShouldBe(1);
        result.PluginsImported.ShouldBe(1);
        PluginService.Verify(service => service.ReadHeader(pluginPath), Times.Once);
    }

    private PluginImportService CreateSut()
    {
        return new PluginImportService(
            SchemaInitializer,
            ConnectionFactory,
            PluginRepository,
            PluginService.Object);
    }

    private void SeedImportedPlugin(string modKey, string pluginPath, int loadOrderIndex)
    {
        SchemaInitializer.Initialize();
        var fileInfo = new FileInfo(pluginPath);
        using var database = ConnectionFactory.OpenDatabase();
        PluginRepository.UpsertPlugin(database, new PluginMetadataDTO
        {
            ModKey = modKey,
            GameRelease = "Starfield",
            LoadOrderIndex = loadOrderIndex,
            PluginFileName = modKey,
            PluginPath = pluginPath,
            Enabled = true,
            ExistsOnDisk = true,
            ImportState = PluginImportState.Current.ToString(),
            SourceFileSizeBytes = fileInfo.Length,
            SourceLastWriteUtcTicks = fileInfo.LastWriteTimeUtc.Ticks,
            LastCheckedUtc = DateTimeOffset.UtcNow.ToString("O"),
            LastImportedUtc = DateTimeOffset.UtcNow.ToString("O")
        });
    }

    private string CreatePluginFile(string fileName, string content)
    {
        Directory.CreateDirectory(DatabaseDirectory);
        var pluginPath = Path.Combine(DatabaseDirectory, fileName);
        File.WriteAllText(pluginPath, content);
        return pluginPath;
    }

    private static PluginLoadOrderEntryDTO CreateLoadOrderEntry(string modKey, string pluginPath, int loadOrderIndex)
    {
        return new PluginLoadOrderEntryDTO
        {
            ModKey = modKey,
            PluginFileName = modKey,
            PluginPath = pluginPath,
            LoadOrderIndex = loadOrderIndex
        };
    }

    private static PluginHeaderMetadataDTO CreateHeader(string modKey, params string[] masterModKeys)
    {
        return new PluginHeaderMetadataDTO
        {
            ModKey = modKey,
            Author = "Test",
            FormVersion = 582,
            HeaderFlags = 1,
            MasterModKeys = masterModKeys
        };
    }
}
