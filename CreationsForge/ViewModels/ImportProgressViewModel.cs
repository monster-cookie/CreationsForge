using System.Windows.Input;
using CreationsForge.Commands;
using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.Services.Interfaces;
using Serilog;

namespace CreationsForge.ViewModels;

public class ImportProgressViewModel : ViewModelBase
{
    private readonly IApplicationNavigationService ApplicationNavigationService;
    private readonly IAllGamesImportWorkflowService AllGamesImportWorkflowService;
    private readonly IGameImportWorkflowService GameImportWorkflowService;
    private readonly ILogger Logger;
    private readonly IProcessTerminationDiagnosticsService ProcessTerminationDiagnosticsService;
    private readonly IUserDialogService UserDialogService;
    private readonly CancellationTokenSource CancellationTokenSource = new();
    private SupportedGameDTO? SelectedGame;
    private bool ForceFullReimport;
    private bool ResetAndImportAll;
    private bool ImportCompleted;
    private bool ImportStarted;
    private string StatusText = "Preparing import...";
    private string DetailText = string.Empty;
    private string GameText = string.Empty;
    private double ProgressValue;
    private double ProgressMaximum = 100;
    private bool IsIndeterminate = true;

    public ImportProgressViewModel(
        IGameImportWorkflowService gameImportWorkflowService,
        IAllGamesImportWorkflowService allGamesImportWorkflowService,
        IApplicationNavigationService applicationNavigationService,
        IUserDialogService userDialogService,
        IProcessTerminationDiagnosticsService processTerminationDiagnosticsService,
        ILogger logger)
    {
        GameImportWorkflowService = gameImportWorkflowService;
        AllGamesImportWorkflowService = allGamesImportWorkflowService;
        ApplicationNavigationService = applicationNavigationService;
        UserDialogService = userDialogService;
        ProcessTerminationDiagnosticsService = processTerminationDiagnosticsService;
        Logger = logger.ForContext<ImportProgressViewModel>();
        CancelImportCommand = new RelayCommand(CancelImport);
    }

    public ICommand CancelImportCommand { get; }

    public string CurrentStatusText
    {
        get => StatusText;
        private set => SetProperty(ref StatusText, value);
    }

    public string CurrentDetailText
    {
        get => DetailText;
        private set => SetProperty(ref DetailText, value);
    }

    public string CurrentGameText
    {
        get => GameText;
        private set => SetProperty(ref GameText, value);
    }

    public double CurrentProgressValue
    {
        get => ProgressValue;
        private set => SetProperty(ref ProgressValue, value);
    }

    public double CurrentProgressMaximum
    {
        get => ProgressMaximum;
        private set => SetProperty(ref ProgressMaximum, value);
    }

    public bool CurrentIsIndeterminate
    {
        get => IsIndeterminate;
        private set => SetProperty(ref IsIndeterminate, value);
    }

    public void Configure(SupportedGameDTO selectedGame, bool forceFullReimport)
    {
        SelectedGame = selectedGame;
        ForceFullReimport = forceFullReimport;
        ResetAndImportAll = false;
        CurrentStatusText = forceFullReimport
            ? $"Preparing full {selectedGame.DisplayName} import..."
            : $"Preparing {selectedGame.DisplayName} import...";
        CurrentGameText = FormatGameText(selectedGame.DisplayName);
        CurrentDetailText = string.Empty;
        CurrentProgressValue = 0;
        CurrentProgressMaximum = 100;
        CurrentIsIndeterminate = true;
    }

    public void ConfigureResetAndImportAll()
    {
        SelectedGame = null;
        ForceFullReimport = true;
        ResetAndImportAll = true;
        CurrentStatusText = "Preparing Reset & Import All...";
        CurrentGameText = "Current game: All games";
        CurrentDetailText = string.Empty;
        CurrentProgressValue = 0;
        CurrentProgressMaximum = 100;
        CurrentIsIndeterminate = true;
    }

    public async Task StartImportAsync()
    {
        if (ImportStarted || (!ResetAndImportAll && SelectedGame is null))
        {
            return;
        }

        ImportStarted = true;
        ImportCompleted = false;

        try
        {
            using var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                CancellationTokenSource.Token,
                ProcessTerminationDiagnosticsService.TerminationToken);
            var progress = new Progress<GameImportProgressDTO>(UpdateProgress);
            if (ResetAndImportAll)
            {
                await AllGamesImportWorkflowService.ImportAllAsync(
                    resetDatabase: true,
                    progress: progress,
                    cancellationToken: linkedCancellationTokenSource.Token);
            }
            else
            {
                await GameImportWorkflowService.ImportAsync(
                    SelectedGame!.Game,
                    ForceFullReimport,
                    progress,
                    linkedCancellationTokenSource.Token);
            }

            ImportCompleted = true;
            await ApplicationNavigationService.ShowMainViewAsync(SelectedGame, runConfiguredGameImport: false);
        }
        catch (OperationCanceledException)
        {
            ImportCompleted = true;
            if (ResetAndImportAll)
            {
                Logger.Information("Reset & Import All was canceled");
                CurrentStatusText = "Reset & Import All canceled.";
            }
            else
            {
                Logger.Information("Import for {Game} was canceled", SelectedGame!.Game);
                CurrentStatusText = $"Import for {SelectedGame.DisplayName} canceled.";
            }

            CurrentDetailText = "Returning to the main workspace.";
            CurrentIsIndeterminate = false;
            await ApplicationNavigationService.ShowMainViewAsync(SelectedGame, runConfiguredGameImport: false);
        }
        catch (Exception ex)
        {
            ImportCompleted = true;
            if (ResetAndImportAll)
            {
                Logger.Error(ex, "Reset & Import All failed");
            }
            else
            {
                Logger.Error(ex, "Import for {Game} failed", SelectedGame!.Game);
            }

            await UserDialogService.ShowErrorAsync("Unable to complete the import. Details were written to the log file.");
            await ApplicationNavigationService.ShowMainViewAsync(SelectedGame, runConfiguredGameImport: false);
        }
    }

    public void CancelImport()
    {
        if (!ImportCompleted)
        {
            CancellationTokenSource.Cancel();
        }
    }

    private void UpdateProgress(GameImportProgressDTO progress)
    {
        CurrentStatusText = progress.StatusText;
        CurrentGameText = GetProgressGameText(progress);
        CurrentDetailText = progress.DetailText;
        CurrentProgressValue = progress.ProgressValue;
        CurrentProgressMaximum = progress.ProgressMaximum;
        CurrentIsIndeterminate = progress.IsIndeterminate;
    }

    private string GetProgressGameText(GameImportProgressDTO progress)
    {
        if (progress.Game.HasValue)
        {
            return FormatGameText(GetDisplayName(progress.Game.Value));
        }

        if (ResetAndImportAll)
        {
            return "Current game: All games";
        }

        return SelectedGame is null ? string.Empty : FormatGameText(SelectedGame.DisplayName);
    }

    private string GetDisplayName(Core.Enums.SupportedGame game)
    {
        if (SelectedGame is not null && SelectedGame.Game == game)
        {
            return SelectedGame.DisplayName;
        }

        return game.ToString();
    }

    private static string FormatGameText(string displayName)
    {
        return $"Current game: {displayName}";
    }
}
