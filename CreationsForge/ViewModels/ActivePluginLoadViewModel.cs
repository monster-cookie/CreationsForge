using System.Diagnostics;
using System.Collections.Concurrent;
using Autofac;
using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Repositories.Interfaces;
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
        using var scope = RootScope.BeginLifetimeScope();
        var recordTypes = scope.Resolve<IEnumerable<IRecordTreeRepository>>()
            .Select(repository => repository.RecordType)
            .ToList();
        var indexedRecordTypes = recordTypes
            .Select((recordType, index) => new RecordTreeLoadRequest(index, recordType))
            .ToList();
        var results = new ConcurrentBag<RecordTreeLoadResult>();
        var parallelOptions = new ParallelOptions
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = Math.Max(1, Math.Min(Environment.ProcessorCount, indexedRecordTypes.Count))
        };

        Parallel.ForEach(indexedRecordTypes, parallelOptions, request =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var recordTypeScope = RootScope.BeginLifetimeScope();
            var repository = recordTypeScope.Resolve<IEnumerable<IRecordTreeRepository>>()
                .Single(candidate => string.Equals(candidate.RecordType, request.RecordType, StringComparison.Ordinal));
            var stopwatch = Stopwatch.StartNew();
            var entries = repository.GetRecordTreeEntriesByPlugin(game, modKey);
            stopwatch.Stop();

            Logger.Information(
                "Loaded active plugin record tree group {RecordType} with {RecordTreeEntryCount} entries for {Game} plugin {PluginFileName} in {ElapsedMilliseconds} ms",
                request.RecordType,
                entries.Count,
                gameDisplayName,
                pluginFileName,
                stopwatch.ElapsedMilliseconds);

            results.Add(new RecordTreeLoadResult(request.Index, entries));
        });

        return results
            .OrderBy(result => result.Index)
            .SelectMany(result => result.Entries)
            .ToList();
    }

    private sealed record RecordTreeLoadRequest(int Index, string RecordType);

    private sealed record RecordTreeLoadResult(int Index, IReadOnlyList<RecordTreeEntryDTO> Entries);
}
