using CreationsForge.Core.Database.Interfaces;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Importers;
using CreationsForge.Core.Services.Interfaces;
using Serilog;

namespace CreationsForge.Core.Services;

public class AllGamesImportWorkflowService : IAllGamesImportWorkflowService
{
    private readonly IDatabaseResetService DatabaseResetService;
    private readonly IDatabaseSchemaInitializer DatabaseSchemaInitializer;
    private readonly GameImportDispatcher GameImportDispatcher;
    private readonly ILogger Logger = Log.ForContext<AllGamesImportWorkflowService>();

    public AllGamesImportWorkflowService(
        IDatabaseResetService databaseResetService,
        IDatabaseSchemaInitializer databaseSchemaInitializer,
        GameImportDispatcher gameImportDispatcher)
    {
        DatabaseResetService = databaseResetService;
        DatabaseSchemaInitializer = databaseSchemaInitializer;
        GameImportDispatcher = gameImportDispatcher;
    }

    public async Task<AllGamesImportWorkflowResultDTO> ImportAllAsync(
        bool resetDatabase,
        IProgress<GameImportProgressDTO>? progress = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            return await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (resetDatabase)
                {
                    progress?.Report(new GameImportProgressDTO
                    {
                        StatusText = "Resetting database...",
                        DetailText = "Deleting the SQLite database and sidecar files.",
                        ProgressValue = 0,
                        IsIndeterminate = true
                    });
                    DatabaseResetService.Reset();
                }

                cancellationToken.ThrowIfCancellationRequested();
                progress?.Report(new GameImportProgressDTO
                {
                    StatusText = "Initializing database...",
                    DetailText = "Applying migrations before all-games import.",
                    ProgressValue = 10,
                    ProgressMaximum = 100,
                    IsIndeterminate = true
                });
                var migrationsApplied = DatabaseSchemaInitializer.Initialize();
                var importResults = new List<GameImportResultDTO>();
                var games = Enum.GetValues<SupportedGame>();

                Logger.Information("Starting all-games import workflow; reset database: {ResetDatabase}", resetDatabase);
                for (var index = 0; index < games.Length; index++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var game = games[index];
                    progress?.Report(new GameImportProgressDTO
                    {
                        StatusText = $"Importing {game}...",
                        DetailText = $"Game {index + 1} of {games.Length}.",
                        ProgressValue = 10 + (index * 90 / games.Length),
                        ProgressMaximum = 100,
                        IsIndeterminate = true
                    });

                    var result = GameImportDispatcher.Import(game, true, progress, cancellationToken);
                    importResults.Add(result);
                    Logger.Information("Completed all-games import for {Game}; plugins imported: {PluginsImported}", game, result.PluginsImported);
                }

                var totalPluginsImported = importResults.Sum(result => result.PluginsImported);
                progress?.Report(new GameImportProgressDTO
                {
                    StatusText = "Completed all-games import.",
                    DetailText = $"Imported {totalPluginsImported} plugins across {importResults.Count} games.",
                    ProgressValue = 100,
                    ProgressMaximum = 100,
                    IsIndeterminate = false
                });
                Logger.Information("Completed all-games import workflow; plugins imported: {PluginsImported}", totalPluginsImported);

                return new AllGamesImportWorkflowResultDTO
                {
                    DatabaseReset = resetDatabase,
                    MigrationsApplied = migrationsApplied,
                    ImportResults = importResults
                };
            }, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            Logger.Information("All-games import workflow was canceled; reset database: {ResetDatabase}", resetDatabase);
            throw;
        }
    }
}
