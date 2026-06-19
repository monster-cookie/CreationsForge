using CreationsForge.Core.Database.Interfaces;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Importers;
using CreationsForge.Core.Services.Interfaces;
using Serilog;

namespace CreationsForge.Core.Services;

public class GameImportWorkflowService : IGameImportWorkflowService
{
    private readonly IDatabaseSchemaInitializer DatabaseSchemaInitializer;
    private readonly GameImportDispatcher GameImportDispatcher;
    private readonly IGameSelectionService GameSelectionService;
    private readonly IProcessTerminationDiagnosticsService? ProcessTerminationDiagnosticsService;
    private readonly ILogger Logger = Log.ForContext<GameImportWorkflowService>();

    public GameImportWorkflowService(
        IDatabaseSchemaInitializer databaseSchemaInitializer,
        GameImportDispatcher gameImportDispatcher,
        IGameSelectionService gameSelectionService,
        IProcessTerminationDiagnosticsService? processTerminationDiagnosticsService = null)
    {
        DatabaseSchemaInitializer = databaseSchemaInitializer;
        GameImportDispatcher = gameImportDispatcher;
        GameSelectionService = gameSelectionService;
        ProcessTerminationDiagnosticsService = processTerminationDiagnosticsService;
    }

    public async Task<GameImportWorkflowResultDTO> ImportAsync(
        SupportedGame game,
        bool forceFullReimport = false,
        IProgress<GameImportProgressDTO>? progress = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var reportingProgress = CreateHeartbeatProgress($"Import {game}", progress);
        reportingProgress.Report(new GameImportProgressDTO
        {
            Game = game,
            StatusText = $"Preparing {game} import...",
            DetailText = "Saving selected game.",
            ProgressValue = 0,
            IsIndeterminate = true
        });
        GameSelectionService.SetActiveGame(game);

        try
        {
            return await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                Logger.Information("Starting UI import workflow for {Game}", game);
                reportingProgress.Report(new GameImportProgressDTO
                {
                    Game = game,
                    StatusText = $"Initializing {game} database...",
                    DetailText = "Applying any pending migrations.",
                    ProgressValue = 25,
                    IsIndeterminate = true
                });
                var migrationsApplied = DatabaseSchemaInitializer.Initialize();
                var forceImport = forceFullReimport || migrationsApplied;

                cancellationToken.ThrowIfCancellationRequested();
                reportingProgress.Report(new GameImportProgressDTO
                {
                    Game = game,
                    StatusText = $"Importing {game} plugins and records...",
                    DetailText = forceImport ? "Running a full reimport. This may take several minutes." : "Unchanged plugins will be skipped.",
                    ProgressValue = 50,
                    IsIndeterminate = true
                });
                var importResult = GameImportDispatcher.Import(game, forceImport, reportingProgress, cancellationToken);
                Logger.Information("Completed UI import workflow for {Game}; plugins imported: {PluginsImported}", game, importResult.PluginsImported);
                reportingProgress.Report(new GameImportProgressDTO
                {
                    Game = game,
                    StatusText = $"Completed {game} import.",
                    DetailText = $"Imported {importResult.PluginsImported} plugins.",
                    ProgressValue = 100,
                    IsIndeterminate = false
                });

                return new GameImportWorkflowResultDTO
                {
                    MigrationsApplied = migrationsApplied,
                    ImportResult = importResult
                };
            }, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            Logger.Information("UI import workflow for {Game} was canceled", game);
            throw;
        }
    }

    private IProgress<GameImportProgressDTO> CreateHeartbeatProgress(
        string phaseName,
        IProgress<GameImportProgressDTO>? progress)
    {
        return new HeartbeatProgress(phaseName, progress, ProcessTerminationDiagnosticsService);
    }

    private sealed class HeartbeatProgress : IProgress<GameImportProgressDTO>
    {
        private readonly string PhaseName;
        private readonly IProgress<GameImportProgressDTO>? InnerProgress;
        private readonly IProcessTerminationDiagnosticsService? ProcessTerminationDiagnosticsService;

        public HeartbeatProgress(
            string phaseName,
            IProgress<GameImportProgressDTO>? innerProgress,
            IProcessTerminationDiagnosticsService? processTerminationDiagnosticsService)
        {
            PhaseName = phaseName;
            InnerProgress = innerProgress;
            ProcessTerminationDiagnosticsService = processTerminationDiagnosticsService;
        }

        public void Report(GameImportProgressDTO value)
        {
            ProcessTerminationDiagnosticsService?.UpdateHeartbeat(PhaseName, value);
            InnerProgress?.Report(value);
        }
    }
}
