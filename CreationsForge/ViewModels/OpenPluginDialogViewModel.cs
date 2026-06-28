using System.Collections.ObjectModel;
using System.Windows.Input;
using CreationsForge.Commands;
using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.ViewModels;

public class OpenPluginDialogViewModel : ViewModelBase
{
    private readonly IPluginSelectionService PluginSelectionService;
    private IReadOnlyList<OpenPluginRowViewModel> AllPluginRows = [];
    private SupportedGameDTO SelectedGameValue;
    private OpenPluginRowViewModel? SelectedPluginRowValue;
    private string SearchTextValue = string.Empty;
    private string SelectedImportStateFilterValue = "All";
    private string SelectedSortOptionValue = "Load order";
    private string PluginSummaryTextValue = "0 plugins";

    public OpenPluginDialogViewModel(
        IReadOnlyList<SupportedGameDTO> supportedGames,
        SupportedGameDTO? selectedGame,
        IPluginSelectionService pluginSelectionService)
    {
        if (supportedGames.Count == 0)
        {
            throw new ArgumentException("At least one supported game is required.", nameof(supportedGames));
        }

        SupportedGames = supportedGames;
        PluginSelectionService = pluginSelectionService;
        SelectedGameValue = selectedGame ?? supportedGames[0];
        PluginRows = new ObservableCollection<OpenPluginRowViewModel>();
        ImportStateFilters = ["All", nameof(PluginImportState.Current), nameof(PluginImportState.Changed), nameof(PluginImportState.PartiallyImported), nameof(PluginImportState.Failed), nameof(PluginImportState.Missing), nameof(PluginImportState.Unsupported)];
        SortOptions = ["Load order", "Plugin", "Records", "State", "Last imported"];
        RefreshCommand = new RelayCommand(RefreshPlugins);
        RefreshPlugins();
    }

    public IReadOnlyList<SupportedGameDTO> SupportedGames { get; }

    public ObservableCollection<OpenPluginRowViewModel> PluginRows { get; }

    public IReadOnlyList<string> ImportStateFilters { get; }

    public IReadOnlyList<string> SortOptions { get; }

    public ICommand RefreshCommand { get; }

    public SupportedGameDTO SelectedGame
    {
        get => SelectedGameValue;
        private set
        {
            if (!SetProperty(ref SelectedGameValue, value))
            {
                return;
            }

            RefreshPlugins();
        }
    }

    public OpenPluginRowViewModel? SelectedPluginRow
    {
        get => SelectedPluginRowValue;
        set
        {
            if (!SetProperty(ref SelectedPluginRowValue, value))
            {
                return;
            }

            OnPropertyChanged(nameof(SelectedPlugin));
            OnPropertyChanged(nameof(CanOpenSelectedPlugin));
            OnPropertyChanged(nameof(CanRunPrimaryAction));
            OnPropertyChanged(nameof(HasSelectedPlugin));
            OnPropertyChanged(nameof(HasSelectedDiagnostics));
            OnPropertyChanged(nameof(SelectedPluginDiagnosticSummary));
            OnPropertyChanged(nameof(SelectedPluginDiagnosticDetails));
        }
    }

    public PluginDTO? SelectedPlugin => SelectedPluginRow?.Plugin;

    public bool CanOpenSelectedPlugin => SelectedPluginRow?.CanOpen == true;

    public bool CanRunPrimaryAction => HasNoPlugins || CanOpenSelectedPlugin;

    public bool HasSelectedPlugin => SelectedPluginRow is not null;

    public bool HasNoPlugins => AllPluginRows.Count == 0;

    public string PrimaryActionText => HasNoPlugins ? "Import" : "Open";

    public string EmptyPluginListText => $"{SelectedGame.DisplayName} has not been imported yet.";

    public bool HasSelectedDiagnostics => SelectedPluginRow?.HasDiagnostics == true;

    public string SelectedPluginDiagnosticSummary => SelectedPluginRow?.DiagnosticSummary ?? string.Empty;

    public string SelectedPluginDiagnosticDetails => SelectedPluginRow?.DiagnosticDetails ?? string.Empty;

    public string SearchText
    {
        get => SearchTextValue;
        set
        {
            if (SetProperty(ref SearchTextValue, value))
            {
                ApplyFiltersAndSort();
            }
        }
    }

    public string SelectedImportStateFilter
    {
        get => SelectedImportStateFilterValue;
        set
        {
            if (SetProperty(ref SelectedImportStateFilterValue, value))
            {
                ApplyFiltersAndSort();
            }
        }
    }

    public string SelectedSortOption
    {
        get => SelectedSortOptionValue;
        set
        {
            if (SetProperty(ref SelectedSortOptionValue, value))
            {
                ApplyFiltersAndSort();
            }
        }
    }

    public string PluginSummaryText
    {
        get => PluginSummaryTextValue;
        private set => SetProperty(ref PluginSummaryTextValue, value);
    }

    public void SelectGame(SupportedGameDTO game)
    {
        SelectedGame = game;
    }

    public bool IsSelectedGame(SupportedGameDTO game)
    {
        return SelectedGame.Game == game.Game;
    }

    private void RefreshPlugins()
    {
        AllPluginRows = PluginSelectionService.GetOpenablePlugins(SelectedGame.Game)
            .Select(plugin => new OpenPluginRowViewModel(plugin))
            .ToList();
        SelectedPluginRow = null;
        ApplyFiltersAndSort();
    }

    private void ApplyFiltersAndSort()
    {
        var selectedFileName = SelectedPluginRow?.FileName;
        IEnumerable<OpenPluginRowViewModel> rows = AllPluginRows;
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            rows = rows.Where(row => row.FileName.Contains(SearchText.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        if (!string.Equals(SelectedImportStateFilter, "All", StringComparison.OrdinalIgnoreCase) &&
            Enum.TryParse<PluginImportState>(SelectedImportStateFilter, out var importState))
        {
            rows = rows.Where(row => row.Plugin.ImportState == importState);
        }

        rows = SelectedSortOption switch
        {
            "Plugin" => rows.OrderBy(row => row.FileName, StringComparer.OrdinalIgnoreCase),
            "Records" => rows.OrderByDescending(row => row.Plugin.RecordCount).ThenBy(row => row.Plugin.LoadOrderIndex),
            "State" => rows.OrderBy(row => row.Plugin.ImportState).ThenBy(row => row.Plugin.LoadOrderIndex),
            "Last imported" => rows.OrderByDescending(row => row.Plugin.LastImportedUTC ?? DateTime.MinValue).ThenBy(row => row.Plugin.LoadOrderIndex),
            _ => rows.OrderBy(row => row.Plugin.LoadOrderIndex)
        };

        var filteredRows = rows.ToList();
        PluginRows.Clear();
        foreach (var row in filteredRows)
        {
            PluginRows.Add(row);
        }

        SelectedPluginRow = filteredRows.FirstOrDefault(row => string.Equals(row.FileName, selectedFileName, StringComparison.OrdinalIgnoreCase));
        OnPropertyChanged(nameof(HasNoPlugins));
        OnPropertyChanged(nameof(CanRunPrimaryAction));
        OnPropertyChanged(nameof(PrimaryActionText));
        OnPropertyChanged(nameof(EmptyPluginListText));
        UpdateSummary();
    }

    private void UpdateSummary()
    {
        var failedCount = AllPluginRows.Count(row => row.Plugin.ImportState == PluginImportState.Failed);
        var changedCount = AllPluginRows.Count(row => row.Plugin.ImportState is PluginImportState.Changed or PluginImportState.PartiallyImported);
        var missingCount = AllPluginRows.Count(row => row.Plugin.ImportState == PluginImportState.Missing || !row.Plugin.ExistsOnDisk);
        PluginSummaryText = $"{AllPluginRows.Count:N0} plugins  -  {failedCount:N0} failed  -  {changedCount:N0} changed  -  {missingCount:N0} missing";
    }
}
