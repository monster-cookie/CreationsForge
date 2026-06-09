using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using CreationsForge.Commands;
using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.Services.Interfaces;
using Serilog;

namespace CreationsForge.ViewModels;

public class MainViewModel : ViewModelBase
{
    private const int LargePluginRecordThreshold = 2000;
    private readonly IApplicationNavigationService ApplicationNavigationService;
    private readonly IGameImportReadinessService GameImportReadinessService;
    private readonly IGameSelectionService GameSelectionService;
    private readonly ILogger Logger;
    private readonly IPluginSelectionService PluginSelectionService;
    private readonly IRecordComparisonService RecordComparisonService;
    private readonly IRecordTreeService RecordTreeService;
    private readonly IUserDialogService UserDialogService;
    private IList<RecordTreeItemViewModel> AllRecordTreeItems = [];
    private IReadOnlyList<PluginDTO> MatchingPlugins = [];
    private bool RunConfiguredGameImport;
    private bool Started;
    private SupportedGameDTO? SelectedGame;
    private PluginDTO? SelectedPlugin;
    private IList<string> GameSuggestionList = [];
    private IList<string> PluginSuggestionList = [];
    private string? SelectedGameDisplayNameValue;
    private string? SelectedPluginFileNameValue;
    private string ActiveGameText = string.Empty;
    private string ActivePluginTextValue = string.Empty;
    private string ImportedRecordCountTextValue = "Imported records: 0";
    private string RecordComparisonTitleTextValue = "Select a record to compare.";
    private string ActiveGameStatusTextValue = "Active game: None";
    private string ActivePluginStatusTextValue = "Active plugin: None";
    private string ActiveRecordCountTextValue = "Active records: 0";
    private string FormIDFilterValue = string.Empty;
    private string EditorIDFilterValue = string.Empty;
    private bool RecordTreePaneExpanded = true;
    private string StatusTextValue = "Select an active game to import plugins.";
    private bool HasSelectedGameValue;
    private bool IsUpdatingSelectedPlugin;
    private bool IsActivePluginLoadingValue;
    private int ActivePluginLoadVersion;
    private string? LoadedRecordTreePluginKey;
    private string? LoadingRecordTreePluginKey;
    private string ActivePluginLoadingTextValue = string.Empty;

    public MainViewModel(
        IGameSelectionService gameSelectionService,
        IGameImportReadinessService gameImportReadinessService,
        IPluginSelectionService pluginSelectionService,
        IRecordComparisonService recordComparisonService,
        IRecordTreeService recordTreeService,
        AssetPreviewPaneViewModel assetPreviewPane,
        IApplicationNavigationService applicationNavigationService,
        IUserDialogService userDialogService,
        ILogger logger)
    {
        GameSelectionService = gameSelectionService;
        GameImportReadinessService = gameImportReadinessService;
        PluginSelectionService = pluginSelectionService;
        RecordComparisonService = recordComparisonService;
        RecordTreeService = recordTreeService;
        AssetPreviewPane = assetPreviewPane;
        ApplicationNavigationService = applicationNavigationService;
        UserDialogService = userDialogService;
        Logger = logger.ForContext<MainViewModel>();
        ShowSettingsCommand = new RelayCommand(ShowSettings);
        ReimportSelectedGameCommand = new AsyncRelayCommand(ReimportSelectedGameAsync, () => SelectedGame is not null);
        ResetAndImportAllCommand = new AsyncRelayCommand(ResetAndImportAllAsync);
        ToggleRecordTreePaneCommand = new RelayCommand(ToggleRecordTreePane);
        SupportedGames = GameSelectionService.GetSupportedGames();
        GameSuggestions = SupportedGames.Select(game => game.DisplayName).ToList();
        PluginSuggestions = [];
        RecordTreeItems = new ObservableCollection<RecordTreeItemViewModel>();
        RecordComparisonColumns = new ObservableCollection<RecordComparisonColumnViewModel>();
        RecordComparisonRows = new ObservableCollection<RecordComparisonRowViewModel>();
        RecordComparisonSource = CreateRecordComparisonSource();

        var activeGame = GameSelectionService.GetActiveGame();
        SelectedGame = SupportedGames.FirstOrDefault(game => game.Game == activeGame);
        if (SelectedGame is not null)
        {
            ActiveGameText = SelectedGame.DisplayName;
            SelectedGameDisplayNameValue = SelectedGame.DisplayName;
        }

        UpdateSelectedGameState();
        RefreshPluginSuggestions(string.Empty);
        UpdateStatusBar();
    }

    public IReadOnlyList<SupportedGameDTO> SupportedGames { get; }

    public IList<string> GameSuggestions
    {
        get => GameSuggestionList;
        private set => SetProperty(ref GameSuggestionList, value);
    }

    public IList<string> PluginSuggestions
    {
        get => PluginSuggestionList;
        private set => SetProperty(ref PluginSuggestionList, value);
    }

    public string? SelectedGameDisplayName
    {
        get => SelectedGameDisplayNameValue;
        set
        {
            if (!SetProperty(ref SelectedGameDisplayNameValue, value) || value is null)
            {
                return;
            }

            _ = SelectGameByTextAsync(value);
        }
    }

    public string? SelectedPluginFileName
    {
        get => SelectedPluginFileNameValue;
        set
        {
            if (!SetProperty(ref SelectedPluginFileNameValue, value) || value is null)
            {
                return;
            }

            _ = SelectPluginByFileNameAsync(value);
        }
    }

    public ObservableCollection<RecordTreeItemViewModel> RecordTreeItems { get; }

    public ObservableCollection<RecordComparisonColumnViewModel> RecordComparisonColumns { get; }

    public ObservableCollection<RecordComparisonRowViewModel> RecordComparisonRows { get; }

    public HierarchicalTreeDataGridSource<RecordComparisonRowViewModel> RecordComparisonSource { get; private set; }

    public AssetPreviewPaneViewModel AssetPreviewPane { get; }

    public ICommand ToggleRecordTreePaneCommand { get; }

    public ICommand ShowSettingsCommand { get; }

    public AsyncRelayCommand ReimportSelectedGameCommand { get; }

    public AsyncRelayCommand ResetAndImportAllCommand { get; }

    public bool IsRecordTreePaneExpanded
    {
        get => RecordTreePaneExpanded;
        private set
        {
            if (!SetProperty(ref RecordTreePaneExpanded, value))
            {
                return;
            }

            OnPropertyChanged(nameof(IsRecordTreePaneContentVisible));
            OnPropertyChanged(nameof(RecordTreePaneToggleText));
            OnPropertyChanged(nameof(RecordTreePaneWidth));
        }
    }

    public bool IsRecordTreePaneContentVisible => IsRecordTreePaneExpanded;

    public string RecordTreePaneToggleText => IsRecordTreePaneExpanded
        ? "<"
        : ">";

    public double RecordTreePaneWidth => IsRecordTreePaneExpanded
        ? 1000
        : 44;

    public string FormIDFilter
    {
        get => FormIDFilterValue;
        set
        {
            if (SetProperty(ref FormIDFilterValue, value))
            {
                ApplyRecordTreeFilters();
            }
        }
    }

    public string EditorIDFilter
    {
        get => EditorIDFilterValue;
        set
        {
            if (SetProperty(ref EditorIDFilterValue, value))
            {
                ApplyRecordTreeFilters();
            }
        }
    }

    public string ActiveGameSearchText
    {
        get => ActiveGameText;
        set => SetProperty(ref ActiveGameText, value);
    }

    public string ActivePluginSearchText
    {
        get => ActivePluginTextValue;
        set
        {
            if (!SetProperty(ref ActivePluginTextValue, value))
            {
                return;
            }

            RefreshPluginSuggestions(value);
            if (!IsUpdatingSelectedPlugin &&
                !string.Equals(SelectedPluginFileName, value, StringComparison.OrdinalIgnoreCase))
            {
                SetSelectedPluginFileName(null);
            }
        }
    }

    public bool HasSelectedGame
    {
        get => HasSelectedGameValue;
        private set => SetProperty(ref HasSelectedGameValue, value);
    }

    public string ImportedRecordCountText
    {
        get => ImportedRecordCountTextValue;
        private set => SetProperty(ref ImportedRecordCountTextValue, value);
    }

    public string ActiveGameStatusText
    {
        get => ActiveGameStatusTextValue;
        private set => SetProperty(ref ActiveGameStatusTextValue, value);
    }

    public string ActivePluginStatusText
    {
        get => ActivePluginStatusTextValue;
        private set => SetProperty(ref ActivePluginStatusTextValue, value);
    }

    public string ActiveRecordCountText
    {
        get => ActiveRecordCountTextValue;
        private set => SetProperty(ref ActiveRecordCountTextValue, value);
    }

    public string StatusText
    {
        get => StatusTextValue;
        private set => SetProperty(ref StatusTextValue, value);
    }

    public bool IsActivePluginLoading
    {
        get => IsActivePluginLoadingValue;
        private set => SetProperty(ref IsActivePluginLoadingValue, value);
    }

    public string ActivePluginLoadingText
    {
        get => ActivePluginLoadingTextValue;
        private set => SetProperty(ref ActivePluginLoadingTextValue, value);
    }

    public void Configure(SupportedGameDTO? selectedGame, bool runConfiguredGameImport, PluginDTO? selectedPlugin = null, IList<RecordTreeItemViewModel>? recordTreeItems = null)
    {
        RunConfiguredGameImport = runConfiguredGameImport;
        if (selectedGame is not null)
        {
            SelectedGame = SupportedGames.FirstOrDefault(game => game.Game == selectedGame.Game) ?? selectedGame;
            ActiveGameSearchText = SelectedGame.DisplayName;
            SetSelectedGameDisplayName(SelectedGame.DisplayName);
        }

        ClearActivePlugin();
        UpdateSelectedGameState();
        RefreshPluginSuggestions(string.Empty);
        if (selectedPlugin is not null && recordTreeItems is not null)
        {
            LoadPreloadedRecordTree(selectedPlugin, recordTreeItems);
        }
        else
        {
            RefreshRecordTree();
        }

        UpdateStatusBar();
    }

    public async Task StartAsync()
    {
        if (Started)
        {
            return;
        }

        Started = true;
        if (!RunConfiguredGameImport || SelectedGame is null)
        {
            return;
        }

        await ImportSelectedGameAsync(forceFullReimport: false);
    }

    public void UpdateGameSearchText(string searchText)
    {
        ActiveGameSearchText = searchText;
        RefreshGameSuggestions(searchText);
    }

    public string RecordComparisonTitleText
    {
        get => RecordComparisonTitleTextValue;
        private set => SetProperty(ref RecordComparisonTitleTextValue, value);
    }

    public bool IsExactGameSuggestion(string searchText)
    {
        return SupportedGames.Any(game =>
            string.Equals(game.DisplayName, searchText, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(game.Name, searchText, StringComparison.OrdinalIgnoreCase));
    }

    public async Task ChooseGameSuggestionAsync(string gameDisplayName)
    {
        ActiveGameSearchText = gameDisplayName;
        await SelectGameByTextAsync(gameDisplayName);
    }

    public async Task SubmitGameQueryAsync(string queryText)
    {
        ActiveGameSearchText = queryText;
        await SelectGameByTextAsync(queryText);
    }

    public void UpdatePluginSearchText(string searchText)
    {
        ActivePluginSearchText = searchText;
    }

    public bool IsExactPluginSuggestion(string searchText)
    {
        return PluginSuggestions.Any(pluginFileName =>
            string.Equals(pluginFileName, searchText, StringComparison.OrdinalIgnoreCase));
    }

    public void ChoosePluginSuggestion(string pluginFileName)
    {
        ActivePluginSearchText = pluginFileName;
        _ = SelectPluginByFileNameAsync(pluginFileName);
    }

    public void SubmitPluginQuery(string queryText)
    {
        ActivePluginSearchText = queryText;
        if (!PluginSuggestions.Any(pluginFileName => string.Equals(pluginFileName, queryText, StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        _ = SelectPluginByFileNameAsync(queryText);
    }

    public void ToggleRecordTreePane()
    {
        IsRecordTreePaneExpanded = !IsRecordTreePaneExpanded;
    }

    public void SelectRecordForComparison(RecordTreeItemViewModel item)
    {
        if (SelectedGame is null || item.FormKey is null || string.IsNullOrWhiteSpace(item.RecordType))
        {
            ClearRecordComparison();
            AssetPreviewPane.ClearPreview();
            return;
        }

        var comparison = RecordComparisonService.GetRecordComparison(SelectedGame.Game, item.RecordType, item.FormKey);
        RecordComparisonColumns.Clear();
        foreach (var column in comparison.Columns)
        {
            RecordComparisonColumns.Add(new RecordComparisonColumnViewModel(
                column.ModKey,
                column.Header,
                SelectedPlugin is not null && IsSameModKey(column.ModKey, SelectedPlugin.ModKey)));
        }

        RecordComparisonRows.Clear();
        foreach (var field in comparison.Fields)
        {
            RecordComparisonRows.Add(new RecordComparisonRowViewModel(
                field.FieldName,
                field.Values,
                field.Children));
        }

        RecordComparisonTitleText = comparison.FormKey is null
            ? "Select a record to compare."
            : $"{comparison.RecordType} {comparison.EditorID} ({comparison.FormKey.Id:X8})";
        RefreshRecordComparisonSource();
        AssetPreviewPane.LoadPreviewForRecord(SelectedGame.Game, item.RecordType, item.FormKey);
    }

    private void ShowSettings()
    {
        _ = ApplicationNavigationService.ShowSettingsViewAsync();
    }

    private async Task ReimportSelectedGameAsync()
    {
        await ImportSelectedGameAsync(forceFullReimport: true);
    }

    private async Task ResetAndImportAllAsync()
    {
        var importApproved = await UserDialogService.ShowResetAndImportAllWarningAsync();
        if (!importApproved)
        {
            Logger.Information("Reset & Import All was canceled by the user");
            StatusText = "Reset & Import All canceled.";
            return;
        }

        await ApplicationNavigationService.ShowResetAndImportAllProgressViewAsync();
    }

    private async Task SelectGameByTextAsync(string gameText)
    {
        var selectedGame = SupportedGames.FirstOrDefault(game =>
            string.Equals(game.DisplayName, gameText, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(game.Name, gameText, StringComparison.OrdinalIgnoreCase));
        if (selectedGame is null)
        {
            return;
        }

        if (SelectedGame?.Game == selectedGame.Game)
        {
            return;
        }

        SelectedGame = selectedGame;
        ActiveGameSearchText = selectedGame.DisplayName;
        SetSelectedGameDisplayName(selectedGame.DisplayName);
        ClearActivePlugin();
        UpdateSelectedGameState();
        RefreshRecordTree();
        UpdateStatusBar();
        await ImportSelectedGameAsync(forceFullReimport: false);
    }

    private async Task ImportSelectedGameAsync(bool forceFullReimport)
    {
        if (SelectedGame is null)
        {
            return;
        }

        var shouldWarn = forceFullReimport || !GameImportReadinessService.HasImportedData(SelectedGame.Game);
        if (shouldWarn)
        {
            var importApproved = await UserDialogService.ShowImportWarningAsync(SelectedGame, forceFullReimport);
            if (!importApproved)
            {
                Logger.Information("Import for {Game} was canceled by the user", SelectedGame.Game);
                StatusText = $"Import canceled for {SelectedGame.DisplayName}.";
                return;
            }
        }

        await ApplicationNavigationService.ShowImportProgressViewAsync(SelectedGame, forceFullReimport);
    }

    private void RefreshGameSuggestions(string searchText)
    {
        var matchingGames = string.IsNullOrWhiteSpace(searchText)
            ? SupportedGames
            : SupportedGames
                .Where(game => game.DisplayName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    game.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                .ToList();

        GameSuggestions = matchingGames
            .Select(game => game.DisplayName)
            .ToList();
    }

    private void RefreshPluginSuggestions(string searchText)
    {
        if (SelectedGame is null)
        {
            MatchingPlugins = [];
            PluginSuggestions = [];
            return;
        }

        MatchingPlugins = PluginSelectionService.SearchOpenablePluginsByFilename(SelectedGame.Game, searchText);
        PluginSuggestions = MatchingPlugins
            .Take(25)
            .Select(plugin => plugin.ModKey.FileName)
            .ToList();
    }

    private async Task SelectPluginByFileNameAsync(string pluginFileName)
    {
        if (SelectedGame is null)
        {
            return;
        }

        var selectedPlugin = MatchingPlugins.FirstOrDefault(plugin =>
            string.Equals(plugin.ModKey.FileName, pluginFileName, StringComparison.OrdinalIgnoreCase));
        if (selectedPlugin is null)
        {
            MatchingPlugins = PluginSelectionService.SearchOpenablePluginsByFilename(SelectedGame.Game, pluginFileName);
            selectedPlugin = MatchingPlugins.FirstOrDefault(plugin =>
                string.Equals(plugin.ModKey.FileName, pluginFileName, StringComparison.OrdinalIgnoreCase));
        }

        if (selectedPlugin is null)
        {
            return;
        }

        var pluginKey = GetPluginKey(selectedPlugin);
        if (SelectedPlugin is not null &&
            IsSameModKey(SelectedPlugin.ModKey, selectedPlugin.ModKey) &&
            string.Equals(LoadedRecordTreePluginKey, pluginKey, StringComparison.Ordinal) &&
            !IsActivePluginLoading)
        {
            return;
        }

        if (string.Equals(LoadingRecordTreePluginKey, pluginKey, StringComparison.Ordinal) && IsActivePluginLoading)
        {
            return;
        }

        SelectedPlugin = selectedPlugin;
        SetActivePluginSelection(selectedPlugin.ModKey.FileName);
        UpdateStatusBar();
        if (selectedPlugin.RecordCount > LargePluginRecordThreshold)
        {
            await ApplicationNavigationService.ShowActivePluginLoadViewAsync(SelectedGame, selectedPlugin);
            return;
        }

        await RefreshRecordTreeAsync(selectedPlugin);
    }

    private void ClearActivePlugin()
    {
        SelectedPlugin = null;
        LoadedRecordTreePluginKey = null;
        LoadingRecordTreePluginKey = null;
        ActivePluginLoadVersion++;
        IsActivePluginLoading = false;
        ActivePluginLoadingText = string.Empty;
        SetActivePluginSelection(null);
        RefreshRecordTree();
        ClearRecordComparison();
    }

    private void SetSelectedGameDisplayName(string? displayName)
    {
        SelectedGameDisplayNameValue = displayName;
        OnPropertyChanged(nameof(SelectedGameDisplayName));
    }

    private void SetSelectedPluginFileName(string? pluginFileName)
    {
        SelectedPluginFileNameValue = pluginFileName;
        OnPropertyChanged(nameof(SelectedPluginFileName));
    }

    private void SetActivePluginSelection(string? pluginFileName)
    {
        IsUpdatingSelectedPlugin = true;
        ActivePluginSearchText = pluginFileName ?? string.Empty;
        SetSelectedPluginFileName(pluginFileName);
        IsUpdatingSelectedPlugin = false;
    }

    private void UpdateSelectedGameState()
    {
        HasSelectedGame = SelectedGame is not null;
        ReimportSelectedGameCommand.RaiseCanExecuteChanged();
    }

    private void UpdateStatusBar()
    {
        var importedRecordCount = SelectedGame is null
            ? 0
            : PluginSelectionService.GetImportedRecordCount(SelectedGame.Game);
        ImportedRecordCountText = $"Imported records: {importedRecordCount:N0}";
        ActiveGameStatusText = SelectedGame is null
            ? "Active game: None"
            : $"Active game: {SelectedGame.DisplayName}";
        ActivePluginStatusText = SelectedPlugin is null
            ? "Active plugin: None"
            : $"Active plugin: {SelectedPlugin.ModKey.FileName}";
        ActiveRecordCountText = SelectedPlugin is null
            ? "Active records: 0"
            : $"Active records: {SelectedPlugin.RecordCount:N0}";
        StatusText = GetStatusText(importedRecordCount);
    }

    private string GetStatusText(long importedRecordCount)
    {
        if (SelectedGame is null)
        {
            return "No active game selected.";
        }

        return SelectedPlugin is null
            ? $"{SelectedGame.DisplayName}: {importedRecordCount:N0} imported records. No active plugin selected."
            : $"{SelectedGame.DisplayName}: active plugin {SelectedPlugin.ModKey.FileName} ({SelectedPlugin.RecordCount:N0} records).";
    }

    private void RefreshRecordTree()
    {
        if (SelectedGame is null || SelectedPlugin is null)
        {
            AllRecordTreeItems = [];
            ApplyRecordTreeFilters();
            ClearRecordComparison();
            return;
        }

        AllRecordTreeItems = BuildRecordTree(RecordTreeService.GetRecordTreeEntries(SelectedGame.Game, SelectedPlugin.ModKey));
        LogRecordTreeSummary(SelectedGame, SelectedPlugin);
        ApplyRecordTreeFilters();
    }

    private void LoadPreloadedRecordTree(PluginDTO selectedPlugin, IList<RecordTreeItemViewModel> recordTreeItems)
    {
        if (SelectedGame is null)
        {
            return;
        }

        SelectedPlugin = selectedPlugin;
        LoadedRecordTreePluginKey = GetPluginKey(selectedPlugin);
        LoadingRecordTreePluginKey = null;
        ActivePluginLoadVersion++;
        IsActivePluginLoading = false;
        ActivePluginLoadingText = string.Empty;
        SetActivePluginSelection(selectedPlugin.ModKey.FileName);
        AllRecordTreeItems = recordTreeItems;
        LogRecordTreeSummary(SelectedGame, selectedPlugin);
        ApplyRecordTreeFilters();
        ClearRecordComparison();
    }

    private async Task RefreshRecordTreeAsync(PluginDTO selectedPlugin)
    {
        if (SelectedGame is null)
        {
            return;
        }

        var selectedGame = SelectedGame;
        var requestVersion = ++ActivePluginLoadVersion;
        var pluginKey = GetPluginKey(selectedPlugin);
        LoadingRecordTreePluginKey = pluginKey;
        IsActivePluginLoading = true;
        ActivePluginLoadingText = $"Loading records for {selectedPlugin.ModKey.FileName}...";
        StatusText = ActivePluginLoadingText;
        ClearRecordComparison();
        AllRecordTreeItems = [];
        ApplyRecordTreeFilters();

        var stopwatch = Stopwatch.StartNew();
        try
        {
            await Task.Yield();

            var fetchStopwatch = Stopwatch.StartNew();
            var recordTreeEntries = RecordTreeService.GetRecordTreeEntries(selectedGame.Game, selectedPlugin.ModKey);
            fetchStopwatch.Stop();

            if (requestVersion != ActivePluginLoadVersion ||
                SelectedPlugin is null ||
                !IsSameModKey(SelectedPlugin.ModKey, selectedPlugin.ModKey))
            {
                return;
            }

            var buildStopwatch = Stopwatch.StartNew();
            var recordTreeItems = await Task.Run(() => BuildRecordTree(recordTreeEntries));
            buildStopwatch.Stop();

            if (requestVersion != ActivePluginLoadVersion ||
                SelectedPlugin is null ||
                !IsSameModKey(SelectedPlugin.ModKey, selectedPlugin.ModKey))
            {
                return;
            }

            AllRecordTreeItems = recordTreeItems;
            LoadedRecordTreePluginKey = pluginKey;
            LogRecordTreeSummary(selectedGame, selectedPlugin);
            Logger.Information(
                "Active plugin record tree fetched {RecordTreeEntryCount} entries for {Game} plugin {PluginFileName} in {FetchElapsedMilliseconds} ms and built the tree in {BuildElapsedMilliseconds} ms",
                recordTreeEntries.Count,
                selectedGame.DisplayName,
                selectedPlugin.ModKey.FileName,
                fetchStopwatch.ElapsedMilliseconds,
                buildStopwatch.ElapsedMilliseconds);
            ApplyRecordTreeFilters();
            UpdateStatusBar();
        }
        finally
        {
            if (requestVersion == ActivePluginLoadVersion)
            {
                stopwatch.Stop();
                Logger.Information(
                    "Active plugin record tree load finished for {Game} plugin {PluginFileName} in {ElapsedMilliseconds} ms",
                    selectedGame.DisplayName,
                    selectedPlugin.ModKey.FileName,
                    stopwatch.ElapsedMilliseconds);
                LoadingRecordTreePluginKey = null;
                IsActivePluginLoading = false;
                ActivePluginLoadingText = string.Empty;
            }
        }
    }

    private void ClearRecordComparison()
    {
        RecordComparisonColumns.Clear();
        RecordComparisonRows.Clear();
        RecordComparisonTitleText = "Select a record to compare.";
        RefreshRecordComparisonSource();
        AssetPreviewPane.ClearPreview();
    }

    private void LogRecordTreeSummary(SupportedGameDTO selectedGame, PluginDTO selectedPlugin)
    {
        var leafItems = AllRecordTreeItems
            .SelectMany(item => item.Children)
            .ToList();
        var countedLeafItems = leafItems.Count(item => item.PluginCount.HasValue);
        var maxPluginCount = leafItems
            .Select(item => item.PluginCount ?? 0)
            .DefaultIfEmpty()
            .Max();

        Logger.Information(
            "Record tree loaded for {Game} plugin {PluginFileName}: {LeafRecordCount} leaf records, {CountedLeafRecordCount} with plugin counts, max plugin count {MaxPluginCount}",
            selectedGame.DisplayName,
            selectedPlugin.ModKey.FileName,
            leafItems.Count,
            countedLeafItems,
            maxPluginCount);
    }

    public static IList<RecordTreeItemViewModel> BuildRecordTree(IReadOnlyList<RecordTreeEntryDTO> entries)
    {
        var recordTreeItems = new List<RecordTreeItemViewModel>();
        foreach (var recordTypeGroup in entries.GroupBy(entry => entry.RecordType))
        {
            var recordTypeItem = new RecordTreeItemViewModel(recordTypeGroup.Key, string.Empty);
            foreach (var record in recordTypeGroup.OrderBy(entry => entry.EditorID, StringComparer.OrdinalIgnoreCase))
            {
                recordTypeItem.Children.Add(CreateRecordTreeItem(record));
            }

            if (recordTypeItem.Children.Count > 0)
            {
                recordTreeItems.Add(recordTypeItem);
            }
        }

        return recordTreeItems;
    }

    private static RecordTreeItemViewModel CreateRecordTreeItem(RecordTreeEntryDTO entry)
    {
        return new RecordTreeItemViewModel(
            entry.FormKey.Id.ToString("X8"),
            entry.EditorID,
            entry.FormKey,
            entry.RecordType,
            entry.PluginCount);
    }

    private void ApplyRecordTreeFilters()
    {
        RecordTreeItems.Clear();
        foreach (var item in AllRecordTreeItems)
        {
            var filteredItem = FilterRecordTreeItem(item);
            if (filteredItem is not null)
            {
                RecordTreeItems.Add(filteredItem);
            }
        }

    }

    private RecordTreeItemViewModel? FilterRecordTreeItem(RecordTreeItemViewModel item)
    {
        var filteredItem = new RecordTreeItemViewModel(
            item.FormIDText,
            item.EditorID,
            item.FormKey,
            item.RecordType,
            item.PluginCount);
        foreach (var child in item.Children)
        {
            var filteredChild = FilterRecordTreeItem(child);
            if (filteredChild is not null)
            {
                filteredItem.Children.Add(filteredChild);
            }
        }

        if (item.FormKey is null)
        {
            return filteredItem.Children.Count > 0 ||
                (string.IsNullOrWhiteSpace(FormIDFilter) && string.IsNullOrWhiteSpace(EditorIDFilter))
                ? filteredItem
                : null;
        }

        return MatchesFormIDFilter(item) &&
            item.EditorID.Contains(EditorIDFilter.Trim(), StringComparison.OrdinalIgnoreCase)
            ? filteredItem
            : null;
    }

    private bool MatchesFormIDFilter(RecordTreeItemViewModel item)
    {
        var filter = FormIDFilter.Trim();
        return string.IsNullOrWhiteSpace(filter) ||
            item.FormIDText.Contains(filter, StringComparison.OrdinalIgnoreCase);
    }

    private void RefreshRecordComparisonSource()
    {
        RecordComparisonSource = CreateRecordComparisonSource();
        OnPropertyChanged(nameof(RecordComparisonSource));
    }

    private HierarchicalTreeDataGridSource<RecordComparisonRowViewModel> CreateRecordComparisonSource()
    {
        var source = new HierarchicalTreeDataGridSource<RecordComparisonRowViewModel>(RecordComparisonRows)
            .WithHierarchicalExpanderTextColumn(
                "Field",
                row => row.FieldName,
                row => row.Children,
                row => row.IsExpanded,
                row => row.HasChildren);
        for (var columnIndex = 0; columnIndex < RecordComparisonColumns.Count; columnIndex++)
        {
            var currentIndex = columnIndex;
            var column = RecordComparisonColumns[currentIndex];
            source.Columns.Add(new TreeDataGridTemplateColumn
            {
                Header = column.Header,
                CellTemplate = new FuncDataTemplate<RecordComparisonRowViewModel>(
                    (row, _) => row is null
                        ? new TextBlock()
                        : CreateComparisonValueCell(row, currentIndex, column.IsActive))
            });
        }

        return source;
    }

    private static bool IsSameModKey(ModKeyDTO first, ModKeyDTO second)
    {
        return first.Type == second.Type &&
            string.Equals(first.Name, second.Name, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(first.FileName, second.FileName, StringComparison.OrdinalIgnoreCase);
    }

    private static string GetPluginKey(PluginDTO plugin)
    {
        return $"{plugin.Game}|{plugin.ModKey.Name}|{plugin.ModKey.Type}|{plugin.ModKey.FileName}";
    }

    private static Control CreateComparisonValueCell(RecordComparisonRowViewModel row, int columnIndex, bool isActiveColumn)
    {
        var textBlock = new TextBlock
        {
            Text = row.GetValue(columnIndex),
            TextTrimming = TextTrimming.CharacterEllipsis,
            VerticalAlignment = VerticalAlignment.Center
        };
        App.ApplyApplicationTextForeground(textBlock);

        return new Border
        {
            Background = GetComparisonValueBrush(row.GetValueState(columnIndex)),
            BorderBrush = isActiveColumn
                ? new SolidColorBrush(Colors.Goldenrod)
                : new SolidColorBrush(Colors.Transparent),
            BorderThickness = isActiveColumn
                ? new Thickness(1, 0)
                : new Thickness(0),
            Padding = new Thickness(6, 3),
            Child = textBlock
        };
    }

    private static IBrush GetComparisonValueBrush(RecordComparisonValueState state)
    {
        return state switch
        {
            RecordComparisonValueState.Identical => new SolidColorBrush(Color.FromArgb(80, 0, 128, 0)),
            RecordComparisonValueState.Conflict => new SolidColorBrush(Color.FromArgb(80, 192, 0, 0)),
            RecordComparisonValueState.WinningOverride => new SolidColorBrush(Color.FromArgb(80, 192, 160, 0)),
            _ => new SolidColorBrush(Colors.Transparent)
        };
    }
}
