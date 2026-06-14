using System.Diagnostics;
using Autofac;
using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.Services.Interfaces;
using Serilog;

namespace CreationsForge.ViewModels;

public class ActivePluginLoadViewModel : ViewModelBase
{
    private readonly IApplicationNavigationService ApplicationNavigationService;
    private readonly ILifetimeScope RootScope;
    private readonly ILogger Logger;
    private readonly IUserDialogService UserDialogService;
    private SupportedGameDTO? SelectedGame;
    private PluginDTO? SelectedPlugin;
    private bool LoadStarted;
    private string StatusText = "Preparing record browser...";
    private string DetailText = string.Empty;

    public ActivePluginLoadViewModel(
        IApplicationNavigationService applicationNavigationService,
        ILifetimeScope rootScope,
        IUserDialogService userDialogService,
        ILogger logger)
    {
        ApplicationNavigationService = applicationNavigationService;
        RootScope = rootScope;
        UserDialogService = userDialogService;
        Logger = logger.ForContext<ActivePluginLoadViewModel>();
    }

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

    public void Configure(SupportedGameDTO selectedGame, PluginDTO selectedPlugin)
    {
        SelectedGame = selectedGame;
        SelectedPlugin = selectedPlugin;
        LoadStarted = false;
        CurrentStatusText = $"Loading record browser for {selectedPlugin.ModKey.FileName}...";
        CurrentDetailText = $"{selectedPlugin.RecordCount:N0} records. This can take a minute for large master files.";
    }

    public async Task StartLoadAsync(CancellationToken cancellationToken = default)
    {
        if (LoadStarted || SelectedGame is null || SelectedPlugin is null)
        {
            return;
        }

        LoadStarted = true;
        var selectedGame = SelectedGame;
        var selectedPlugin = SelectedPlugin;
        var stopwatch = Stopwatch.StartNew();

        try
        {
            Logger.Information(
                "Starting active plugin record browser load for {Game} plugin {PluginFileName} with {RecordCount} records",
                selectedGame.DisplayName,
                selectedPlugin.ModKey.FileName,
                selectedPlugin.RecordCount);

            var recordTreeItems = await Task.Run(() => LoadRecordTreeItems(selectedGame, selectedPlugin, cancellationToken), cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            stopwatch.Stop();

            Logger.Information(
                "Loaded active plugin record browser data for {Game} plugin {PluginFileName}: {RecordTypeCount} record type groups in {ElapsedMilliseconds} ms",
                selectedGame.DisplayName,
                selectedPlugin.ModKey.FileName,
                recordTreeItems.Count,
                stopwatch.ElapsedMilliseconds);

            await ApplicationNavigationService.ShowMainViewAsync(selectedGame, runConfiguredGameImport: false, selectedPlugin, recordTreeItems);
        }
        catch (OperationCanceledException)
        {
            stopwatch.Stop();
            Logger.Information(
                "Canceled active plugin record browser load for {Game} plugin {PluginFileName} after {ElapsedMilliseconds} ms",
                selectedGame.DisplayName,
                selectedPlugin.ModKey.FileName,
                stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Logger.Error(
                ex,
                "Unable to load active plugin record browser data for {Game} plugin {PluginFileName} after {ElapsedMilliseconds} ms",
                selectedGame.DisplayName,
                selectedPlugin.ModKey.FileName,
                stopwatch.ElapsedMilliseconds);
            await UserDialogService.ShowErrorAsync("Unable to load the active plugin record browser. Details were written to the log file.");
            await ApplicationNavigationService.ShowMainViewAsync(selectedGame, runConfiguredGameImport: false);
        }
    }

    private IList<RecordTreeItemViewModel> LoadRecordTreeItems(SupportedGameDTO selectedGame, PluginDTO selectedPlugin, CancellationToken cancellationToken)
    {
        var recordTreeEntries = LoadRecordTreeEntries(selectedGame.Game, selectedPlugin.ModKey, selectedGame.DisplayName, selectedPlugin.ModKey.FileName, cancellationToken);
        return MainViewModel.BuildRecordTree(recordTreeEntries);
    }

    private IReadOnlyList<RecordTreeEntryDTO> LoadRecordTreeEntries(SupportedGame game, ModKeyDTO modKey, string gameDisplayName, string pluginFileName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using var scope = RootScope.BeginLifetimeScope();
        var recordTreeService = scope.Resolve<IRecordTreeService>();
        var stopwatch = Stopwatch.StartNew();
        var entries = recordTreeService.GetRecordTreeEntries(game, modKey);
        stopwatch.Stop();
        cancellationToken.ThrowIfCancellationRequested();

        Logger.Information(
            "Loaded active plugin record tree with {RecordTreeEntryCount} entries for {Game} plugin {PluginFileName} in {ElapsedMilliseconds} ms",
            entries.Count,
            gameDisplayName,
            pluginFileName,
            stopwatch.ElapsedMilliseconds);

        return entries;
    }
}
