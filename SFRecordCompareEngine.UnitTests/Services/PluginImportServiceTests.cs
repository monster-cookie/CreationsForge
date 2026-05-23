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
    private readonly RecordHeaderRepository RecordHeaderRepository = new();
    private readonly Mock<IPluginService> PluginService = new();
    private readonly Mock<IRecordImportService> RecordImportService = new();
    private readonly DatabaseSchemaInitializer SchemaInitializer;

    public PluginImportServiceTests()
    {
        var options = new SqliteDatabaseOptions
        {
            DatabaseDirectory = DatabaseDirectory
        };

        ConnectionFactory = new SqliteConnectionFactory(options);
        SchemaInitializer = new DatabaseSchemaInitializer(ConnectionFactory, new DatabaseMigrationRunner());
        RecordImportService.Setup(service => service.ImportPluginRecords(
                It.IsAny<NPoco.IDatabase>(),
                It.IsAny<PluginMetadataDTO>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns<NPoco.IDatabase, PluginMetadataDTO, string, CancellationToken>((_, plugin, _, _) => new SFRecordCompareEngine.Core.DTOs.Records.RecordImportResultDTO
            {
                ModKey = plugin.ModKey
            });
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
    public async Task InitializeAndImportAsync_WhenProgressIsProvided_ReportsCurrentPlugin()
    {
        var pluginPath = CreatePluginFile("Example.esm", "plugin");
        PluginService.Setup(service => service.GetLoadOrder()).Returns([CreateLoadOrderEntry("Example.esm", pluginPath, 0)]);
        PluginService.Setup(service => service.ReadHeader(pluginPath)).Returns(CreateHeader("Example.esm"));
        var progressUpdates = new List<PluginImportProgressDTO>();
        var sut = CreateSut();

        await sut.InitializeAndImportAsync(new Progress<PluginImportProgressDTO>(progressUpdates.Add), CancellationToken.None);

        progressUpdates.ShouldContain(update => update.StatusText.Contains("Initializing plugin database schema", StringComparison.Ordinal));
        progressUpdates.ShouldContain(update => update.CurrentPluginName == "Example.esm" && update.PluginIndex == 1 && update.PluginCount == 1);
        progressUpdates.ShouldContain(update => update.StatusText.Contains("Plugin database import completed", StringComparison.Ordinal));
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
        RecordImportService.Verify(service => service.ImportPluginRecords(
            It.IsAny<NPoco.IDatabase>(),
            It.IsAny<PluginMetadataDTO>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Never);
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
    public async Task InitializeAndImportAsync_WhenPluginFileIsMissing_DeletesDerivedRecordRows()
    {
        var pluginPath = CreatePluginFile("Missing.esm", "plugin");
        SeedImportedPlugin("Missing.esm", pluginPath, 1);
        using (var database = ConnectionFactory.OpenDatabase())
        {
            RecordHeaderRepository.Upsert(database, new SFRecordCompareEngine.Core.DTOs.Records.RecordHeaderDTO
            {
                ModKey = "Missing.esm",
                FormID = "000001",
                RecordType = "Keyword",
                FormKey = "000001:Missing.esm",
                PluginFileName = "Missing.esm",
                ImportedAtUtc = DateTimeOffset.UtcNow.ToString("O")
            });
        }

        File.Delete(pluginPath);
        PluginService.Setup(service => service.GetLoadOrder()).Returns([CreateLoadOrderEntry("Missing.esm", pluginPath, 1)]);
        var sut = CreateSut();

        await sut.InitializeAndImportAsync(CancellationToken.None);

        using var resultDatabase = ConnectionFactory.OpenDatabase();
        resultDatabase.ExecuteScalar<int>("SELECT COUNT(*) FROM RecordHeader WHERE ModKey = @0 COLLATE NOCASE;", "Missing.esm").ShouldBe(0);
    }

    [Fact]
    public async Task InitializeAndImportAsync_WhenPluginLeavesLoadOrder_DeletesDerivedRecordRows()
    {
        var pluginPath = CreatePluginFile("Removed.esm", "plugin");
        SeedImportedPlugin("Removed.esm", pluginPath, 1);
        using (var database = ConnectionFactory.OpenDatabase())
        {
            RecordHeaderRepository.Upsert(database, new SFRecordCompareEngine.Core.DTOs.Records.RecordHeaderDTO
            {
                ModKey = "Removed.esm",
                FormID = "000001",
                RecordType = "Keyword",
                FormKey = "000001:Removed.esm",
                PluginFileName = "Removed.esm",
                ImportedAtUtc = DateTimeOffset.UtcNow.ToString("O")
            });
        }

        PluginService.Setup(service => service.GetLoadOrder()).Returns([]);
        var sut = CreateSut();

        await sut.InitializeAndImportAsync(CancellationToken.None);

        using var resultDatabase = ConnectionFactory.OpenDatabase();
        resultDatabase.ExecuteScalar<int>("SELECT COUNT(*) FROM RecordHeader WHERE ModKey = @0 COLLATE NOCASE;", "Removed.esm").ShouldBe(0);
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

    [Fact]
    public async Task InitializeAndImportAsync_WhenPluginFileSizeChanges_DeletesAndRebuildsRecordHeaders()
    {
        var pluginPath = CreatePluginFile("Example.esm", "plugin");
        SeedImportedPlugin("Example.esm", pluginPath, 1);
        using (var database = ConnectionFactory.OpenDatabase())
        {
            RecordHeaderRepository.Upsert(database, new SFRecordCompareEngine.Core.DTOs.Records.RecordHeaderDTO
            {
                ModKey = "Example.esm",
                FormID = "000001",
                RecordType = "Keyword",
                FormKey = "000001:Example.esm",
                PluginFileName = "Example.esm",
                ImportedAtUtc = DateTimeOffset.UtcNow.ToString("O")
            });
        }

        File.AppendAllText(pluginPath, "changed");
        PluginService.Setup(service => service.GetLoadOrder()).Returns([CreateLoadOrderEntry("Example.esm", pluginPath, 1)]);
        PluginService.Setup(service => service.ReadHeader(pluginPath)).Returns(CreateHeader("Example.esm"));
        RecordImportService.Setup(service => service.ImportPluginRecords(
                It.IsAny<NPoco.IDatabase>(),
                It.IsAny<PluginMetadataDTO>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns<NPoco.IDatabase, PluginMetadataDTO, string, CancellationToken>((database, plugin, importedAtUtc, _) =>
            {
                RecordHeaderRepository.Upsert(database, new SFRecordCompareEngine.Core.DTOs.Records.RecordHeaderDTO
                {
                    ModKey = plugin.ModKey,
                    FormID = "000002",
                    RecordType = "Keyword",
                    FormKey = "000002:Example.esm",
                    PluginFileName = plugin.PluginFileName,
                    ImportedAtUtc = importedAtUtc
                });

                return new SFRecordCompareEngine.Core.DTOs.Records.RecordImportResultDTO
                {
                    ModKey = plugin.ModKey,
                    RecordTypes =
                    [
                        new SFRecordCompareEngine.Core.DTOs.Records.RecordTypeImportResultDTO
                        {
                            RecordType = "Keyword",
                            HeaderImportSupported = true,
                            TypedDetailImportSupported = true,
                            HeadersImported = 1,
                            DetailRowsImported = 1
                        }
                    ]
                };
            });
        var sut = CreateSut();

        await sut.InitializeAndImportAsync(CancellationToken.None);

        using var resultDatabase = ConnectionFactory.OpenDatabase();
        resultDatabase.ExecuteScalar<int>("SELECT COUNT(*) FROM RecordHeader WHERE ModKey = @0 COLLATE NOCASE AND FormID = @1;", "Example.esm", "000001")
            .ShouldBe(0);
        resultDatabase.ExecuteScalar<int>("SELECT COUNT(*) FROM RecordHeader WHERE ModKey = @0 COLLATE NOCASE AND FormID = @1;", "Example.esm", "000002")
            .ShouldBe(1);
    }

    [Fact]
    public async Task InitializeAndImportAsync_WhenRecordImportIsCanceled_DoesNotMarkPluginFailed()
    {
        var pluginPath = CreatePluginFile("Example.esm", "plugin");
        PluginService.Setup(service => service.GetLoadOrder()).Returns([CreateLoadOrderEntry("Example.esm", pluginPath, 1)]);
        PluginService.Setup(service => service.ReadHeader(pluginPath)).Returns(CreateHeader("Example.esm"));
        RecordImportService.Setup(service => service.ImportPluginRecords(
                It.IsAny<NPoco.IDatabase>(),
                It.IsAny<PluginMetadataDTO>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Throws(new OperationCanceledException());
        var sut = CreateSut();

        await Should.ThrowAsync<OperationCanceledException>(() => sut.InitializeAndImportAsync(CancellationToken.None));

        using var database = ConnectionFactory.OpenDatabase();
        PluginRepository.GetByModKey(database, "Example.esm")?.ImportState.ShouldNotBe(PluginImportState.Failed.ToString());
    }

    private PluginImportService CreateSut()
    {
        return new PluginImportService(
            SchemaInitializer,
            ConnectionFactory,
            PluginRepository,
            PluginService.Object,
            RecordHeaderRepository,
            RecordImportService.Object);
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
