using CreationsForge.Core.Configuration.Interfaces;
using CreationsForge.Core.Database.Interfaces;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Importers;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Core.Models.Configuration;
using CreationsForge.Core.Services;
using Shouldly;

namespace CreationsForge.UnitTests.Services;

public class GameImportWorkflowServiceTests
{
    [Fact]
    public async Task ImportAsync_InitializesSchema_SavesActiveGame_AndDispatchesImport()
    {
        var configurationStore = new TestApplicationConfigurationStore();
        var schemaInitializer = new TestDatabaseSchemaInitializer { MigrationsApplied = true };
        var importer = new TestGameImporter(SupportedGame.Starfield);
        var dispatcher = new GameImportDispatcher([importer]);
        var selectionService = new GameSelectionService(configurationStore);
        var workflowService = new GameImportWorkflowService(schemaInitializer, dispatcher, selectionService);

        var result = await workflowService.ImportAsync(SupportedGame.Starfield, true);

        schemaInitializer.InitializeWasCalled.ShouldBeTrue();
        configurationStore.Current.ActiveGame.ShouldBe("Starfield");
        importer.ImportWasCalled.ShouldBeTrue();
        importer.ForceFullReimport.ShouldBeTrue();
        result.MigrationsApplied.ShouldBeTrue();
        result.ImportResult.Game.ShouldBe(SupportedGame.Starfield);
    }

    [Fact]
    public async Task ImportAsync_WithCanceledToken_ThrowsBeforeImport()
    {
        var configurationStore = new TestApplicationConfigurationStore();
        var schemaInitializer = new TestDatabaseSchemaInitializer();
        var importer = new TestGameImporter(SupportedGame.Skyrim);
        var dispatcher = new GameImportDispatcher([importer]);
        var selectionService = new GameSelectionService(configurationStore);
        var workflowService = new GameImportWorkflowService(schemaInitializer, dispatcher, selectionService);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Should.ThrowAsync<OperationCanceledException>(() => workflowService.ImportAsync(SupportedGame.Skyrim, cancellationToken: cancellationTokenSource.Token));

        schemaInitializer.InitializeWasCalled.ShouldBeFalse();
        importer.ImportWasCalled.ShouldBeFalse();
    }

    private sealed class TestApplicationConfigurationStore : IApplicationConfigurationStore
    {
        public string ConfigurationPath => "test.json";

        public ApplicationConfiguration Current { get; set; } = new();

        public void Load()
        { }

        public void Save(ApplicationConfiguration configuration)
        {
            Current = configuration;
        }
    }

    private sealed class TestDatabaseSchemaInitializer : IDatabaseSchemaInitializer
    {
        public bool InitializeWasCalled { get; private set; }

        public bool MigrationsApplied { get; set; }

        public bool Initialize()
        {
            InitializeWasCalled = true;
            return MigrationsApplied;
        }
    }

    private sealed class TestGameImporter : IGameImporter
    {
        public TestGameImporter(SupportedGame game)
        {
            Game = game;
        }

        public SupportedGame Game { get; }

        public bool ImportWasCalled { get; private set; }

        public bool ForceFullReimport { get; private set; }

        public CancellationToken CancellationToken { get; private set; }

        public GameImportResultDTO Import(bool forceFullReimport = false, IProgress<GameImportProgressDTO>? progress = null, CancellationToken cancellationToken = default)
        {
            ImportWasCalled = true;
            ForceFullReimport = forceFullReimport;
            CancellationToken = cancellationToken;
            return new GameImportResultDTO
            {
                Game = Game,
                PluginsDiscovered = 3,
                PluginsImported = 2
            };
        }
    }
}
