using CreationsForge.Bethesda.Assets.Resources;
using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Importers;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;
using Moq;
using NPoco;
using Shouldly;

namespace CreationsForge.UnitTests.Importers;

public class GameImporterTests
{
    [Fact]
    public void Import_SavesGameBeforePlugin()
    {
        var events = new List<string>();
        var plugin = CreatePlugin(SupportedGame.Starfield);
        var importer = CreateImporter(
            plugin,
            new TestGameRepository(events),
            new TestPluginRepository(events),
            new TestPluginMasterReferenceRepository(),
            [],
            new TestRecordImportService(),
            assetArchiveIndexService: new TestAssetArchiveIndexService(events));

        importer.Import();

        events.ShouldBe(["game", "asset-index", "base-plugin"]);
    }

    [Fact]
    public void Import_OpensPluginTransactionAfterAssetIndexing()
    {
        var events = new List<string>();
        var plugin = CreatePlugin(SupportedGame.Starfield);
        var transaction = new Mock<ITransaction>();
        transaction.Setup(current => current.Complete()).Callback(() => events.Add("transaction-complete"));
        transaction.Setup(current => current.Dispose()).Callback(() => events.Add("transaction-dispose"));
        var database = new Mock<IDatabase>();
        database.Setup(current => current.GetTransaction()).Returns(transaction.Object).Callback(() => events.Add("transaction-begin"));
        var importer = new GameImporter(
            new TestGamePluginReader(plugin),
            new TestGameRecordReader(plugin.Game),
            new TestGameRepository(events),
            new TestPluginRepository(events),
            new TestPluginMasterReferenceRepository(),
            [],
            new TestRecordImportService(),
            new TestAssetArchiveIndexService(events),
            database.Object);

        importer.Import();

        events.ShouldBe(["game", "asset-index", "transaction-begin", "base-plugin", "transaction-complete", "transaction-dispose"]);
    }

    [Fact]
    public void Import_WhenRecordImportFails_RollsBackPluginTransactionAndSavesFailedState()
    {
        var events = new List<string>();
        var plugin = CreatePlugin(SupportedGame.Skyrim);
        var transaction = new Mock<ITransaction>();
        transaction.Setup(current => current.Dispose()).Callback(() => events.Add("transaction-dispose"));
        var database = new Mock<IDatabase>();
        database.Setup(current => current.GetTransaction()).Returns(transaction.Object).Callback(() => events.Add("transaction-begin"));
        var pluginRepository = new TestPluginRepository(events);
        var importer = new GameImporter(
            new TestGamePluginReader(plugin),
            new TestGameRecordReader(plugin.Game),
            new TestGameRepository(events),
            pluginRepository,
            new TestPluginMasterReferenceRepository(),
            [],
            new ThrowingRecordImportService(),
            new TestAssetArchiveIndexService(events),
            database.Object);

        var result = importer.Import();

        result.PluginsFailed.ShouldBe(1);
        pluginRepository.Saved.Last().ImportState.ShouldBe(PluginImportState.Failed);
        pluginRepository.Saved.Last().ImportDetails.ShouldNotBeNull();
        pluginRepository.Saved.Last().ImportDetails!.ShouldContain("Record import failed.");
        events.ShouldContain("transaction-begin");
        events.ShouldContain("transaction-dispose");
    }

    [Fact]
    public void Import_ReportsGameForPluginProgress()
    {
        var progressReports = new List<GameImportProgressDTO>();
        var plugin = CreatePlugin(SupportedGame.Fallout4);
        var importer = CreateImporter(
            plugin,
            new TestGameRepository(),
            new TestPluginRepository(),
            new TestPluginMasterReferenceRepository(),
            [],
            new TestRecordImportService());

        importer.Import(progress: new TestProgress<GameImportProgressDTO>(progressReports));

        progressReports.ShouldNotBeEmpty();
        progressReports.ShouldAllBe(progress => progress.Game == SupportedGame.Fallout4);
    }

    [Fact]
    public void Import_WithMatchingPluginExtensionImporter_ImportsExtensionAfterBasePlugin()
    {
        var events = new List<string>();
        var plugin = CreatePlugin(SupportedGame.Starfield);
        var importer = CreateImporter(
            plugin,
            new TestGameRepository(),
            new TestPluginRepository(events),
            new TestPluginMasterReferenceRepository(),
            [new TestPluginExtensionImporter(events, true)],
            new TestRecordImportService());

        importer.Import();

        events.ShouldBe(["base-plugin", "extension-plugin"]);
    }

    [Fact]
    public void Import_WithNonMatchingPluginExtensionImporter_DoesNotImportExtension()
    {
        var events = new List<string>();
        var plugin = CreatePlugin(SupportedGame.Starfield);
        var extensionImporter = new TestPluginExtensionImporter(events, false);
        var importer = CreateImporter(
            plugin,
            new TestGameRepository(),
            new TestPluginRepository(events),
            new TestPluginMasterReferenceRepository(),
            [extensionImporter],
            new TestRecordImportService());

        importer.Import();

        events.ShouldBe(["base-plugin"]);
        extensionImporter.ImportWasCalled.ShouldBeFalse();
    }

    [Fact]
    public void Import_WithMasterReferences_SavesMasterReferencesAndIncrementsCount()
    {
        var plugin = CreatePlugin(SupportedGame.Starfield);
        var masterPlugin = CreatePlugin(SupportedGame.Starfield, "Master", "Master.esm");
        var masterReference = new PluginMasterReferenceDTO
        {
            Game = plugin.Game,
            MasterModKey = masterPlugin.ModKey,
            PluginModKey = plugin.ModKey,
            ImportedAtUTC = DateTime.UtcNow
        };
        var masterReferenceRepository = new TestPluginMasterReferenceRepository();
        var pluginRepository = new TestPluginRepository(existingPlugins: [masterPlugin]);
        var importer = CreateImporter(
            plugin,
            new TestGameRepository(),
            pluginRepository,
            masterReferenceRepository,
            [],
            new TestRecordImportService(),
            [masterReference]);

        var result = importer.Import();

        masterReferenceRepository.Saved.ShouldBe([masterReference]);
        result.MasterReferencesImported.ShouldBe(1);
        masterReferenceRepository.StaleCleanupRequests.ShouldBe([(plugin.Game, plugin.ModKey)]);
    }

    [Fact]
    public void Import_WithMasterReferenceDifferentCasing_SavesMasterReference()
    {
        var plugin = CreatePlugin(SupportedGame.Starfield);
        var masterPlugin = CreatePlugin(SupportedGame.Starfield, "Starfield", "Starfield.esm");
        var masterReference = new PluginMasterReferenceDTO
        {
            Game = plugin.Game,
            MasterModKey = new ModKeyDTO
            {
                Name = "starfield",
                Type = masterPlugin.ModKey.Type,
                FileName = "starfield.esm"
            },
            PluginModKey = plugin.ModKey,
            ImportedAtUTC = DateTime.UtcNow
        };
        var masterReferenceRepository = new TestPluginMasterReferenceRepository();
        var pluginRepository = new TestPluginRepository(existingPlugins: [masterPlugin]);
        var importer = CreateImporter(
            plugin,
            new TestGameRepository(),
            pluginRepository,
            masterReferenceRepository,
            [],
            new TestRecordImportService(),
            [masterReference]);

        var result = importer.Import();

        masterReferenceRepository.Saved.Count.ShouldBe(1);
        masterReferenceRepository.Saved[0].MasterModKey.ShouldBe(masterPlugin.ModKey);
        masterReferenceRepository.Saved[0].PluginModKey.ShouldBe(plugin.ModKey);
        result.MasterReferencesImported.ShouldBe(1);
    }

    [Fact]
    public void Import_WithMissingMasterReference_SkipsMasterReference()
    {
        var plugin = CreatePlugin(SupportedGame.Starfield);
        var masterReference = new PluginMasterReferenceDTO
        {
            Game = plugin.Game,
            MasterModKey = new ModKeyDTO
            {
                Name = "Missing",
                Type = 0,
                FileName = "Missing.esm"
            },
            PluginModKey = plugin.ModKey,
            ImportedAtUTC = DateTime.UtcNow
        };
        var masterReferenceRepository = new TestPluginMasterReferenceRepository();
        var importer = CreateImporter(
            plugin,
            new TestGameRepository(),
            new TestPluginRepository(),
            masterReferenceRepository,
            [],
            new TestRecordImportService(),
            [masterReference]);

        var result = importer.Import();

        masterReferenceRepository.Saved.ShouldBeEmpty();
        result.MasterReferencesImported.ShouldBe(0);
    }

    [Fact]
    public void Import_WithUnchangedPlugin_SkipsMetadataMasterReferencesAndRecords()
    {
        var plugin = CreatePlugin(SupportedGame.Skyrim);
        var recordImportService = new TestRecordImportService();
        var pluginReader = new TestGamePluginReader(plugin);
        var importer = new GameImporter(
            pluginReader,
            new TestGameRecordReader(plugin.Game),
            new TestGameRepository(),
            new TestPluginRepository(existingPlugins: [plugin]),
            new TestPluginMasterReferenceRepository(),
            [],
            recordImportService,
            new TestAssetArchiveIndexService());

        var result = importer.Import();

        result.PluginsUnchanged.ShouldBe(1);
        result.PluginsImported.ShouldBe(0);
        pluginReader.ReadPluginMetadataWasCalled.ShouldBeFalse();
        recordImportService.ImportWasCalled.ShouldBeFalse();
    }

    [Fact]
    public void Import_WithInvalidatedUnchangedPlugin_ReimportsMetadataMasterReferencesAndRecords()
    {
        var plugin = CreatePlugin(SupportedGame.Skyrim);
        var invalidatedPlugin = CreatePlugin(SupportedGame.Skyrim);
        invalidatedPlugin.ImportState = PluginImportState.Changed;
        invalidatedPlugin.InvalidatedAtUTC = DateTime.UtcNow;
        var recordImportService = new TestRecordImportService();
        var pluginReader = new TestGamePluginReader(plugin);
        var importer = new GameImporter(
            pluginReader,
            new TestGameRecordReader(plugin.Game),
            new TestGameRepository(),
            new TestPluginRepository(existingPlugins: [invalidatedPlugin]),
            new TestPluginMasterReferenceRepository(),
            [],
            recordImportService,
            new TestAssetArchiveIndexService());

        var result = importer.Import();

        result.PluginsUnchanged.ShouldBe(0);
        result.PluginsChanged.ShouldBe(1);
        result.PluginsImported.ShouldBe(1);
        pluginReader.ReadPluginMetadataWasCalled.ShouldBeTrue();
        recordImportService.ImportWasCalled.ShouldBeTrue();
    }

    [Fact]
    public void Import_InvokesRecordImportService()
    {
        var plugin = CreatePlugin(SupportedGame.Skyrim);
        var recordImportService = new TestRecordImportService();
        var importer = CreateImporter(
            plugin,
            new TestGameRepository(),
            new TestPluginRepository(),
            new TestPluginMasterReferenceRepository(),
            [],
            recordImportService);

        var result = importer.Import();

        recordImportService.ImportedPlugin.ShouldBe(plugin);
        recordImportService.ImportWasCalled.ShouldBeTrue();
        result.Records.GlobalsImported.ShouldBe(1);
    }

    [Fact]
    public void Import_WithRecordFailures_SavesPluginAsPartiallyImported()
    {
        var plugin = CreatePlugin(SupportedGame.Skyrim);
        var pluginRepository = new TestPluginRepository();
        var recordImportService = new TestRecordImportService(new RecordImportResultDTO
        {
            RecordTypes =
            [
                new RecordTypeImportResultDTO
                {
                    RecordType = "GLOB",
                    HeaderImportSupported = true,
                    TypedDetailImportSupported = true,
                    RecordsFailed = 1
                }
            ]
        });
        var importer = CreateImporter(
            plugin,
            new TestGameRepository(),
            pluginRepository,
            new TestPluginMasterReferenceRepository(),
            [],
            recordImportService);

        var result = importer.Import();

        result.Records.RecordsFailed.ShouldBe(1);
        pluginRepository.Saved.Last().ImportState.ShouldBe(PluginImportState.PartiallyImported);
        pluginRepository.Saved.Last().InvalidatedAtUTC.ShouldNotBeNull();
    }

    [Fact]
    public void ImportedRecordCount_IncludesPartiallyImportedPlugins()
    {
        var current = CreatePlugin(SupportedGame.Starfield, "Current", "Current.esm");
        current.RecordCount = 10;
        var partial = CreatePlugin(SupportedGame.Starfield, "Partial", "Partial.esm");
        partial.RecordCount = 5;
        partial.ImportState = PluginImportState.PartiallyImported;
        var failed = CreatePlugin(SupportedGame.Starfield, "Failed", "Failed.esm");
        failed.RecordCount = 20;
        failed.ImportState = PluginImportState.Failed;
        var pluginRepository = new TestPluginRepository(existingPlugins: [current, partial, failed]);

        var count = pluginRepository.GetImportedRecordCountByGame(SupportedGame.Starfield);

        count.ShouldBe(15);
    }

    [Fact]
    public void Import_WhenPluginReaderAndRecordReaderGamesDoNotMatch_Throws()
    {
        var plugin = CreatePlugin(SupportedGame.Starfield);
        var importer = new GameImporter(
            new TestGamePluginReader(plugin),
            new TestGameRecordReader(SupportedGame.Fallout4),
            new TestGameRepository(),
            new TestPluginRepository(),
            new TestPluginMasterReferenceRepository(),
            [],
            new TestRecordImportService(),
            new TestAssetArchiveIndexService());

        Should.Throw<InvalidOperationException>(() => importer.Import())
            .Message.ShouldContain("does not match");
    }

    private static GameImporter CreateImporter(
        PluginDTO plugin,
        IGameRepository gameRepository,
        IPluginRepository pluginRepository,
        IPluginMasterReferenceRepository pluginMasterReferenceRepository,
        IEnumerable<IPluginExtensionImporter> pluginExtensionImporters,
        IRecordImportService recordImportService,
        IReadOnlyList<PluginMasterReferenceDTO>? masterReferences = null,
        IAssetArchiveIndexService? assetArchiveIndexService = null)
    {
        return new GameImporter(
            new TestGamePluginReader(plugin, masterReferences ?? []),
            new TestGameRecordReader(plugin.Game),
            gameRepository,
            pluginRepository,
            pluginMasterReferenceRepository,
            pluginExtensionImporters,
            recordImportService,
            assetArchiveIndexService ?? new TestAssetArchiveIndexService());
    }

    private static PluginDTO CreatePlugin(SupportedGame game, string name = "Test", string fileName = "Test.esm")
    {
        return new PluginDTO
        {
            Game = game,
            ModKey = new ModKeyDTO
            {
                Name = name,
                Type = 0,
                FileName = fileName
            },
            LoadOrderIndex = 0,
            Enabled = true,
            ExistsOnDisk = true,
            ImportState = PluginImportState.Current,
            HeaderFlags = 0,
            FormVersion = 1,
            RecordCount = 0,
            SourceLastWriteUTCTicks = 0,
            SourceFileSizeBytes = 0,
            LastCheckedUTC = DateTime.UtcNow
        };
    }

    private sealed class TestGamePluginReader : IGamePluginReader
    {
        private readonly PluginDTO Plugin;
        private readonly IReadOnlyList<PluginMasterReferenceDTO> MasterReferences;

        public TestGamePluginReader(PluginDTO plugin, IReadOnlyList<PluginMasterReferenceDTO>? masterReferences = null)
        {
            Plugin = plugin;
            MasterReferences = masterReferences ?? [];
        }

        public SupportedGame Game => Plugin.Game;

        public bool ReadPluginMetadataWasCalled { get; private set; }

        public GameDTO ReadGame()
        {
            return new GameDTO
            {
                Game = Game,
                DisplayName = Game.ToString()
            };
        }

        public IReadOnlyList<PluginLoadOrderEntryDTO> ReadLoadOrder()
        {
            return
            [
                new PluginLoadOrderEntryDTO
                {
                    Game = Game,
                    ModKey = Plugin.ModKey,
                    LoadOrderIndex = Plugin.LoadOrderIndex,
                    Enabled = Plugin.Enabled
                }
            ];
        }

        public PluginSourceInfoDTO ReadSourceInfo(ModKeyDTO modKey)
        {
            return new PluginSourceInfoDTO
            {
                Exists = Plugin.ExistsOnDisk,
                LastWriteUTCTicks = Plugin.SourceLastWriteUTCTicks,
                FileSizeBytes = Plugin.SourceFileSizeBytes
            };
        }

        public bool IsUnsupported(PluginLoadOrderEntryDTO loadOrderEntry)
        {
            return Plugin.ImportState == PluginImportState.Unsupported;
        }

        public PluginDTO ReadPluginMetadata(PluginLoadOrderEntryDTO loadOrderEntry, PluginSourceInfoDTO sourceInfo)
        {
            ReadPluginMetadataWasCalled = true;
            return Plugin;
        }

        public IReadOnlyList<PluginDTO> ReadPlugins()
        {
            return [Plugin];
        }

        public IReadOnlyList<PluginMasterReferenceDTO> ReadMasterReferences(PluginDTO plugin)
        {
            return MasterReferences;
        }
    }

    private sealed class TestGameRecordReader : IGameRecordReader
    {
        public TestGameRecordReader(SupportedGame game)
        {
            Game = game;
        }

        public SupportedGame Game { get; }

        public PluginRecordSetDTO ReadPluginRecords(PluginDTO plugin, CancellationToken cancellationToken = default)
        {
            return new PluginRecordSetDTO();
        }
    }

    private sealed class TestGameRepository : IGameRepository
    {
        private readonly IList<string>? Events;

        public TestGameRepository(IList<string>? events = null)
        {
            Events = events;
        }

        public void Save(GameDTO dto)
        {
            Events?.Add("game");
        }
    }

    private sealed class TestPluginRepository : IPluginRepository
    {
        private readonly IList<string>? Events;
        private readonly IList<PluginDTO> ExistingPlugins;

        public TestPluginRepository(IList<string>? events = null, IList<PluginDTO>? existingPlugins = null)
        {
            Events = events;
            ExistingPlugins = existingPlugins ?? new List<PluginDTO>();
        }

        public IList<PluginDTO> Saved { get; } = new List<PluginDTO>();

        public PluginDTO? GetByModKey(SupportedGame game, ModKeyDTO modKey)
        {
            return ExistingPlugins.FirstOrDefault(plugin =>
                plugin.Game == game
                && plugin.ModKey.Type == modKey.Type
                && string.Equals(plugin.ModKey.FileName, modKey.FileName, StringComparison.OrdinalIgnoreCase)
                && string.Equals(plugin.ModKey.Name, modKey.Name, StringComparison.OrdinalIgnoreCase));
        }

        public int CountByGame(SupportedGame game)
        {
            return ExistingPlugins.Count(plugin => plugin.Game == game);
        }

        public long GetImportedRecordCountByGame(SupportedGame game)
        {
            return ExistingPlugins
                .Where(plugin => plugin.Game == game &&
                    plugin.ExistsOnDisk &&
                    plugin.ImportState is PluginImportState.Current or PluginImportState.PartiallyImported)
                .Sum(plugin => plugin.RecordCount);
        }

        public IReadOnlyList<PluginDTO> GetOpenablePlugins(SupportedGame game)
        {
            return ExistingPlugins
                .Where(plugin => plugin.Game == game && plugin.ExistsOnDisk)
                .ToList();
        }

        public IReadOnlyList<PluginDTO> SearchOpenablePluginsByFilename(SupportedGame game, string searchFilename)
        {
            return ExistingPlugins
                .Where(plugin => plugin.Game == game &&
                    plugin.ExistsOnDisk &&
                    plugin.ModKey.FileName.Contains(searchFilename, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public void Save(PluginDTO dto)
        {
            Events?.Add("base-plugin");
            Saved.Add(dto);
            var existingPlugin = GetByModKey(dto.Game, dto.ModKey);
            if (existingPlugin is not null)
            {
                ExistingPlugins.Remove(existingPlugin);
            }

            ExistingPlugins.Add(dto);
        }
    }

    private sealed class TestPluginMasterReferenceRepository : IPluginMasterReferenceRepository
    {
        public IList<PluginMasterReferenceDTO> Saved { get; } = new List<PluginMasterReferenceDTO>();

        public IList<(SupportedGame Game, ModKeyDTO PluginModKey)> StaleCleanupRequests { get; } = new List<(SupportedGame Game, ModKeyDTO PluginModKey)>();

        public void Save(PluginMasterReferenceDTO dto)
        {
            Saved.Add(dto);
        }

        public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO pluginModKey, DateTime importedAtUTC)
        {
            StaleCleanupRequests.Add((game, pluginModKey));
        }
    }

    private sealed class TestRecordImportService : IRecordImportService
    {
        private readonly RecordImportResultDTO Result;

        public TestRecordImportService(RecordImportResultDTO? result = null)
        {
            Result = result ?? new RecordImportResultDTO
            {
                RecordTypes =
                [
                    new RecordTypeImportResultDTO
                    {
                        RecordType = "GLOB",
                        HeaderImportSupported = true,
                        TypedDetailImportSupported = true,
                        DetailRowsImported = 1
                    }
                ]
            };
        }

        public bool ImportWasCalled { get; private set; }

        public PluginDTO? ImportedPlugin { get; private set; }

        public RecordImportResultDTO ImportPluginRecords(
            PluginDTO plugin,
            IGameRecordReader recordReader,
            IProgress<GameImportProgressDTO>? progress = null,
            int pluginIndex = 0,
            int pluginCount = 0,
            CancellationToken cancellationToken = default)
        {
            ImportWasCalled = true;
            ImportedPlugin = plugin;
            return Result;
        }
    }

    private sealed class ThrowingRecordImportService : IRecordImportService
    {
        public RecordImportResultDTO ImportPluginRecords(
            PluginDTO plugin,
            IGameRecordReader recordReader,
            IProgress<GameImportProgressDTO>? progress = null,
            int pluginIndex = 0,
            int pluginCount = 0,
            CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("Record import failed.");
        }
    }

    private sealed class TestProgress<T> : IProgress<T>
    {
        private readonly IList<T> Reports;

        public TestProgress(IList<T> reports)
        {
            Reports = reports;
        }

        public void Report(T value)
        {
            Reports.Add(value);
        }
    }

    private sealed class TestPluginExtensionImporter : IPluginExtensionImporter
    {
        private readonly IList<string> Events;
        private readonly bool CanImportPlugin;

        public TestPluginExtensionImporter(IList<string> events, bool canImportPlugin)
        {
            Events = events;
            CanImportPlugin = canImportPlugin;
        }

        public bool ImportWasCalled { get; private set; }

        public bool CanImport(PluginDTO plugin)
        {
            return CanImportPlugin;
        }

        public void Import(PluginDTO plugin)
        {
            ImportWasCalled = true;
            Events.Add("extension-plugin");
        }
    }

    private sealed class TestAssetArchiveIndexService : IAssetArchiveIndexService
    {
        private readonly IList<string>? Events;

        public TestAssetArchiveIndexService(IList<string>? events = null)
        {
            Events = events;
        }

        public AssetArchiveIndexResultDTO IndexGameArchives(
            SupportedGame game,
            string? dataFolder,
            IProgress<GameImportProgressDTO>? progress = null,
            CancellationToken cancellationToken = default)
        {
            Events?.Add("asset-index");
            return new AssetArchiveIndexResultDTO
            {
                ArchivesDiscovered = 1,
                ArchivesIndexed = 1,
                EntriesIndexed = 1
            };
        }

        public BethesdaAssetReadResult TryReadArchiveAsset(SupportedGame game, string dataFolder, string assetPath)
        {
            return new BethesdaAssetReadResult
            {
                OriginalPath = assetPath,
                DataFolder = dataFolder,
                SourceType = BethesdaAssetSourceType.Archive,
                Status = BethesdaAssetReadStatus.ArchiveEntryMissing,
                StatusMessage = "Missing indexed asset."
            };
        }
    }
}
