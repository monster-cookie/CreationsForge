using CreationsForge.Core.Database.Interfaces;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Importers;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Core.Services;
using CreationsForge.Core.Services.Interfaces;
using Shouldly;

namespace CreationsForge.UnitTests.Services;

public class AllGamesImportWorkflowServiceTests
{
    [Fact]
    public async Task ImportAllAsync_WithReset_ResetsInitializesSchemaAndImportsEveryGame()
    {
        var callOrder = new List<string>();
        var resetService = new TestDatabaseResetService(callOrder);
        var schemaInitializer = new TestDatabaseSchemaInitializer(callOrder) { MigrationsApplied = true };
        var importers = Enum.GetValues<SupportedGame>()
            .Select(game => new TestGameImporter(game, callOrder))
            .ToList();
        var dispatcher = new GameImportDispatcher(importers);
        var memoryPressureService = new TestMemoryPressureService();
        var workflowService = new AllGamesImportWorkflowService(resetService, schemaInitializer, dispatcher, memoryPressureService);

        var result = await workflowService.ImportAllAsync(resetDatabase: true);

        resetService.ResetWasCalled.ShouldBeTrue();
        schemaInitializer.InitializeWasCalled.ShouldBeTrue();
        callOrder[0].ShouldBe("reset");
        callOrder[1].ShouldBe("initialize");
        foreach (var importer in importers)
        {
            importer.ImportWasCalled.ShouldBeTrue();
            importer.ForceFullReimport.ShouldBeTrue();
        }

        result.DatabaseReset.ShouldBeTrue();
        result.MigrationsApplied.ShouldBeTrue();
        result.ImportResults.Count.ShouldBe(importers.Count);
        result.ImportResults.Select(importResult => importResult.Game).ShouldBe(Enum.GetValues<SupportedGame>());
        memoryPressureService.PhaseNames.ShouldBe(Enum.GetValues<SupportedGame>().Select(game => $"{game} import"));
    }

    [Fact]
    public async Task ImportAllAsync_WithoutReset_DoesNotResetDatabase()
    {
        var callOrder = new List<string>();
        var resetService = new TestDatabaseResetService(callOrder);
        var schemaInitializer = new TestDatabaseSchemaInitializer(callOrder);
        var dispatcher = new GameImportDispatcher(Enum.GetValues<SupportedGame>()
            .Select(game => new TestGameImporter(game, callOrder)));
        var workflowService = new AllGamesImportWorkflowService(resetService, schemaInitializer, dispatcher);

        var result = await workflowService.ImportAllAsync(resetDatabase: false);

        resetService.ResetWasCalled.ShouldBeFalse();
        schemaInitializer.InitializeWasCalled.ShouldBeTrue();
        result.DatabaseReset.ShouldBeFalse();
    }

    [Fact]
    public async Task ImportAllAsync_ReportsCurrentGameForGamePhases()
    {
        var progressReports = new List<GameImportProgressDTO>();
        var callOrder = new List<string>();
        var resetService = new TestDatabaseResetService(callOrder);
        var schemaInitializer = new TestDatabaseSchemaInitializer(callOrder);
        var dispatcher = new GameImportDispatcher(Enum.GetValues<SupportedGame>()
            .Select(game => new TestGameImporter(game, callOrder)));
        var workflowService = new AllGamesImportWorkflowService(resetService, schemaInitializer, dispatcher);

        await workflowService.ImportAllAsync(
            resetDatabase: false,
            progress: new TestProgress<GameImportProgressDTO>(progressReports));

        var gameReports = progressReports
            .Where(progress => progress.Game.HasValue)
            .Select(progress => progress.Game!.Value)
            .ToList();
        gameReports.ShouldBe(Enum.GetValues<SupportedGame>());
    }

    private sealed class TestDatabaseResetService : IDatabaseResetService
    {
        private readonly IList<string> CallOrder;

        public TestDatabaseResetService(IList<string> callOrder)
        {
            CallOrder = callOrder;
        }

        public bool ResetWasCalled { get; private set; }

        public void Reset()
        {
            ResetWasCalled = true;
            CallOrder.Add("reset");
        }
    }

    private sealed class TestDatabaseSchemaInitializer : IDatabaseSchemaInitializer
    {
        private readonly IList<string> CallOrder;

        public TestDatabaseSchemaInitializer(IList<string> callOrder)
        {
            CallOrder = callOrder;
        }

        public bool InitializeWasCalled { get; private set; }

        public bool MigrationsApplied { get; set; }

        public bool Initialize()
        {
            InitializeWasCalled = true;
            CallOrder.Add("initialize");
            return MigrationsApplied;
        }
    }

    private sealed class TestGameImporter : IGameImporter
    {
        private readonly IList<string> CallOrder;

        public TestGameImporter(SupportedGame game, IList<string> callOrder)
        {
            Game = game;
            CallOrder = callOrder;
        }

        public SupportedGame Game { get; }

        public bool ImportWasCalled { get; private set; }

        public bool ForceFullReimport { get; private set; }

        public GameImportResultDTO Import(bool forceFullReimport = false, IProgress<GameImportProgressDTO>? progress = null, CancellationToken cancellationToken = default)
        {
            ImportWasCalled = true;
            ForceFullReimport = forceFullReimport;
            CallOrder.Add(Game.ToString());
            return new GameImportResultDTO
            {
                Game = Game,
                PluginsImported = 1
            };
        }
    }

    private sealed class TestMemoryPressureService : IMemoryPressureService
    {
        public List<string> PhaseNames { get; } = new();

        public void CollectAfterBulkImportPhase(string phaseName)
        {
            PhaseNames.Add(phaseName);
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
}
