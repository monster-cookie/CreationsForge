using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Serilog;
using SFRecordCompareEngine.Core.Configuration.Interfaces;
using SFRecordCompareEngine.Core.DTOs.Cache;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Models.Records;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine;

public partial class MainWindow : Window
{
    private const string BasePluginName = "Starfield.esm";
    private readonly ICacheService CacheService;
    private readonly IGameConfigurationStore GameConfigurationStore;
    private readonly ILogger Logger = Log.ForContext<MainWindow>();
    private readonly IPluginService PluginService;
    private string? LoadedPluginName;

    public MainWindow(
        IGameConfigurationStore gameConfigurationStore,
        IPluginService pluginService,
        ICacheService cacheService)
    {
        InitializeComponent();

        GameConfigurationStore = gameConfigurationStore;
        PluginService = pluginService;
        CacheService = cacheService;

        ClearLoadedPlugin();
    }

    private async void OpenMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenGamePluginDialog(GameConfigurationStore, PluginService)
        {
            Owner = this
        };

        if (dialog.ShowDialog() != true) return;
        if (string.IsNullOrWhiteSpace(dialog.SelectedGame) || string.IsNullOrWhiteSpace(dialog.SelectedPluginName)) return;

        LoadedGameTextBlock.Text = dialog.SelectedGame;
        LoadedPluginTextBlock.Text = dialog.SelectedPluginName;
        LoadedPluginName = dialog.SelectedPluginName;

        try
        {
            ShowCacheProgress("Preparing reference cache...", 0, 0);
            var progress = new Progress<CacheBuildProgressDTO>(UpdateCacheProgress);
            await CacheService.BuildOrUpdateReferenceCacheAsync(
                GetCachePluginNames(dialog.SelectedPluginName),
                progress,
                CancellationToken.None);

            StatusTextBlock.Text = $"Loaded {dialog.SelectedPluginName}.";
            LoadRecordTree();
        }
        finally
        {
            HideCacheProgress();
        }

        Logger.Information("Opened {PluginName} for {Game}", dialog.SelectedPluginName, dialog.SelectedGame);
    }

    private void RecordsTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        switch (e.NewValue)
        {
            case RecordTypeTreeNode node:
                ShowRecordSummaries(node.Records);
                StatusTextBlock.Text = $"Loaded {node.Records.Count} {node.Name} records.";
                break;
            case RecordSummaryDTO record:
                ShowRecordComparison(record);
                break;
        }
    }

    private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ClearLoadedPlugin()
    {
        LoadedPluginName = null;
        LoadedGameTextBlock.Text = "None";
        LoadedPluginTextBlock.Text = "None";
        StatusTextBlock.Text = "Use File > Open to choose a game and plugin.";
        RecordsDataGrid.ItemsSource = null;
        RecordsTreeView.ItemsSource = null;
        ConfigureSummaryGrid();
    }

    private void LoadRecordTree()
    {
        if (LoadedPluginName is null)
        {
            RecordsTreeView.ItemsSource = null;
            RecordsDataGrid.ItemsSource = null;
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

        RecordsTreeView.ItemsSource = nodes;
        RecordsDataGrid.ItemsSource = null;
        ConfigureSummaryGrid();
        StatusTextBlock.Text = nodes.Count == 1
            ? "Loaded 1 record type."
            : $"Loaded {nodes.Count} record types.";
    }

    private void ShowRecordSummaries(IList<RecordSummaryDTO> records)
    {
        ConfigureSummaryGrid();
        RecordsDataGrid.ItemsSource = records;
    }

    private void ShowRecordComparison(RecordSummaryDTO record)
    {
        if (LoadedPluginName is null || string.IsNullOrWhiteSpace(record.RecordType) || string.IsNullOrWhiteSpace(record.FormID))
        {
            StatusTextBlock.Text = "Unable to load comparison for the selected record.";
            return;
        }

        var comparison = PluginService.GetRecordComparison(LoadedPluginName, record.RecordType, record.FormID);
        ConfigureComparisonGrid(comparison.Plugins.Select(plugin => plugin.PluginName).ToList());

        RecordsDataGrid.ItemsSource = comparison.Fields
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
        StatusTextBlock.Text = $"Loaded comparison for {record.EditorID ?? record.FormID}.";
    }

    private void ConfigureSummaryGrid()
    {
        RecordsDataGrid.AutoGenerateColumns = false;
        RecordsDataGrid.Columns.Clear();
        RecordsDataGrid.Columns.Add(new DataGridTextColumn
        {
            Header = "FormID",
            Binding = new Binding(nameof(RecordSummaryDTO.FormID)),
            Width = new DataGridLength(180)
        });
        RecordsDataGrid.Columns.Add(new DataGridTextColumn
        {
            Header = "EditorID",
            Binding = new Binding(nameof(RecordSummaryDTO.EditorID)),
            Width = new DataGridLength(1, DataGridLengthUnitType.Star)
        });
    }

    private void ConfigureComparisonGrid(IList<string> pluginNames)
    {
        RecordsDataGrid.AutoGenerateColumns = false;
        RecordsDataGrid.Columns.Clear();
        RecordsDataGrid.Columns.Add(new DataGridTextColumn
        {
            Header = "Field",
            Binding = new Binding(nameof(RecordComparisonRowViewModel.FieldName)),
            Width = new DataGridLength(240)
        });

        foreach (var pluginName in pluginNames)
        {
            RecordsDataGrid.Columns.Add(new DataGridTemplateColumn
            {
                Header = pluginName,
                CellTemplate = BuildComparisonCellTemplate(pluginName),
                Width = new DataGridLength(1, DataGridLengthUnitType.Star)
            });
        }
    }

    private IList<string> GetCachePluginNames(string pluginName)
    {
        var pluginNames = new List<string> { BasePluginName };
        var pluginHeader = PluginService.GetPluginHeader(pluginName);
        if (pluginHeader is not null)
        {
            pluginNames.AddRange(pluginHeader.Masters
                .Select(master => master.ToString())
                .Where(masterName => !string.IsNullOrWhiteSpace(masterName)));
        }

        pluginNames.Add(pluginName);

        return pluginNames
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private void UpdateCacheProgress(CacheBuildProgressDTO progress)
    {
        ShowCacheProgress(progress.Message, progress.ProcessedPlugins, progress.TotalPlugins);
    }

    private void ShowCacheProgress(string message, int processedPlugins, int totalPlugins)
    {
        CacheProgressOverlay.Visibility = Visibility.Visible;
        CacheProgressTextBlock.Text = message;
        CacheProgressBar.IsIndeterminate = totalPlugins <= 0;
        if (totalPlugins <= 0) return;

        CacheProgressBar.Maximum = totalPlugins;
        CacheProgressBar.Value = Math.Min(processedPlugins, totalPlugins);
    }

    private void HideCacheProgress()
    {
        CacheProgressOverlay.Visibility = Visibility.Collapsed;
        CacheProgressBar.IsIndeterminate = true;
        CacheProgressBar.Value = 0;
    }

    private DataTemplate BuildComparisonCellTemplate(string pluginName)
    {
        var contentControl = new FrameworkElementFactory(typeof(ContentControl));
        contentControl.SetBinding(ContentProperty, new Binding($"Cells[{pluginName}]"));
        contentControl.SetValue(
            ContentTemplateSelectorProperty,
            FindResource("ComparisonCellTemplateSelector"));

        return new DataTemplate
        {
            VisualTree = contentControl
        };
    }
}

public class RecordComparisonRowViewModel
{
    public required string FieldName { get; set; }

    public IDictionary<string, RecordComparisonCellViewModel> Cells { get; set; } =
        new Dictionary<string, RecordComparisonCellViewModel>(StringComparer.OrdinalIgnoreCase);
}

public class RecordComparisonCellViewModel
{
    public RecordComparisonFieldDisplayKind DisplayKind { get; set; }
    public string TextValue { get; set; } = string.Empty;
    public bool? BooleanValue { get; set; }
    public IList<RecordComparisonFieldNodeDTO> TreeNodes { get; set; } = new List<RecordComparisonFieldNodeDTO>();
}

public class RecordComparisonCellTemplateSelector : DataTemplateSelector
{
    public DataTemplate? TextTemplate { get; set; }
    public DataTemplate? BooleanTemplate { get; set; }
    public DataTemplate? TreeTemplate { get; set; }

    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        return item is RecordComparisonCellViewModel cell
            ? cell.DisplayKind switch
            {
                RecordComparisonFieldDisplayKind.Boolean => BooleanTemplate,
                RecordComparisonFieldDisplayKind.Tree => TreeTemplate,
                _ => TextTemplate
            }
            : TextTemplate;
    }
}
