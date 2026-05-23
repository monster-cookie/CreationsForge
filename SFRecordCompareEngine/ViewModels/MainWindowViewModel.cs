using Serilog;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly ILogger Logger;
    private readonly IPluginService PluginService;
    private object? _recordsGridItems;
    private IList<RecordTypeTreeNode> _recordTypeNodes = new List<RecordTypeTreeNode>();
    private string _statusText = string.Empty;
    private string? LoadedPluginName;

    public MainWindowViewModel(
        IPluginService pluginService,
        ILogger logger)
    {
        PluginService = pluginService;
        Logger = logger.ForContext<MainWindowViewModel>() ?? logger;
        LoadedGameText = "None";
        LoadedPluginText = "None";
        StatusText = "Use File > Open to choose a game and plugin.";
    }

    public string LoadedGameText { get; private set; }
    public string LoadedPluginText { get; private set; }

    public string StatusText
    {
        get => _statusText;
        private set => SetProperty(ref _statusText, value);
    }

    public IList<RecordTypeTreeNode> RecordTypeNodes
    {
        get => _recordTypeNodes;
        private set => SetProperty(ref _recordTypeNodes, value);
    }

    public object? RecordsGridItems
    {
        get => _recordsGridItems;
        private set => SetProperty(ref _recordsGridItems, value);
    }

    public IList<string> ComparisonPluginNames { get; private set; } = new List<string>();
    public bool IsComparisonMode { get; private set; }

    public void LoadPlugin(string selectedGame, string selectedPluginName)
    {
        LoadedGameText = selectedGame;
        OnPropertyChanged(nameof(LoadedGameText));
        LoadedPluginText = selectedPluginName;
        OnPropertyChanged(nameof(LoadedPluginText));
        LoadedPluginName = selectedPluginName;
        StatusText = $"Loaded {selectedPluginName}.";
        LoadRecordTree();

        Logger.Information("Opened {PluginName} for {Game}", selectedPluginName, selectedGame);
    }

    public void SelectRecordTreeItem(object? selectedItem)
    {
        switch (selectedItem)
        {
            case RecordTypeTreeNode node:
                ShowRecordSummaries(node.Records);
                StatusText = $"Loaded {node.Records.Count} {node.Name} records.";
                break;
            case RecordSummaryDTO record:
                ShowRecordComparison(record);
                break;
        }
    }

    private void LoadRecordTree()
    {
        if (LoadedPluginName is null)
        {
            RecordTypeNodes = new List<RecordTypeTreeNode>();
            RecordsGridItems = null;
            SetSummaryMode();
            return;
        }

        var nodes = PluginService.GetRecordTypes()
            .Select(recordType => new RecordTypeTreeNode
            {
                Name = recordType,
                Records = PluginService.GetRecords(LoadedPluginName, recordType)
            })
            .Where(node => node.Records.Count > 0)
            .ToList();

        RecordTypeNodes = nodes;
        RecordsGridItems = null;
        SetSummaryMode();
        StatusText = nodes.Count == 1
            ? "Loaded 1 record type."
            : $"Loaded {nodes.Count} record types.";
    }

    private void ShowRecordSummaries(IList<RecordSummaryDTO> records)
    {
        SetSummaryMode();
        RecordsGridItems = records;
    }

    private void ShowRecordComparison(RecordSummaryDTO record)
    {
        if (LoadedPluginName is null || string.IsNullOrWhiteSpace(record.RecordType) || string.IsNullOrWhiteSpace(record.FormID))
        {
            StatusText = "Unable to load comparison for the selected record.";
            return;
        }

        var comparison = PluginService.GetRecordComparison(LoadedPluginName, record.RecordType, record.FormID);
        SetComparisonMode(comparison.Plugins.Select(plugin => plugin.PluginName).ToList());

        RecordsGridItems = comparison.Fields
            .Select(field => new RecordComparisonRowViewModel
            {
                FieldName = field.FieldName,
                Cells = comparison.Plugins.ToDictionary(
                    plugin => plugin.PluginName,
                    plugin => new RecordComparisonCellViewModel
                    {
                        DisplayKind = field.DisplayKind,
                        TextValue = field.ValuesByPlugin.TryGetValue(plugin.PluginName, out var textValue)
                            ? textValue ?? string.Empty
                            : string.Empty,
                        BooleanValue = field.BooleanValuesByPlugin.TryGetValue(plugin.PluginName, out var booleanValue)
                            ? booleanValue
                            : null,
                        TreeNodes = field.TreeValuesByPlugin.TryGetValue(plugin.PluginName, out var treeNodes)
                            ? treeNodes
                            : new List<RecordComparisonFieldNodeDTO>()
                    },
                    StringComparer.OrdinalIgnoreCase)
            })
            .ToList();
        StatusText = $"Loaded comparison for {record.EditorID ?? record.FormID}.";
    }

    private void SetSummaryMode()
    {
        IsComparisonMode = false;
        OnPropertyChanged(nameof(IsComparisonMode));
        ComparisonPluginNames = new List<string>();
        OnPropertyChanged(nameof(ComparisonPluginNames));
    }

    private void SetComparisonMode(IList<string> pluginNames)
    {
        IsComparisonMode = true;
        OnPropertyChanged(nameof(IsComparisonMode));
        ComparisonPluginNames = pluginNames;
        OnPropertyChanged(nameof(ComparisonPluginNames));
    }
}
