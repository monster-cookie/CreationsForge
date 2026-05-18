using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Serilog;
using SFRecordCompareEngine.Core.Configuration;
using SFRecordCompareEngine.Core.Configuration.Interfaces;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine;

public partial class MainWindow : Window
{
    private readonly ILogger Logger = Log.ForContext<MainWindow>();
    private readonly IGameConfigurationStore GameConfigurationStore;
    private readonly IPluginService PluginService;
    private string? LoadedPluginName;

    public MainWindow(
        IGameConfigurationStore gameConfigurationStore,
        IPluginService pluginService)
    {
        InitializeComponent();
        
        GameConfigurationStore = gameConfigurationStore;
        PluginService = pluginService;

        ClearLoadedPlugin();
    }

    private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
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
        StatusTextBlock.Text = $"Loaded {dialog.SelectedPluginName}.";
        LoadRecordTree();

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

        var table = new DataTable();
        table.Columns.Add("Field");
        foreach (var plugin in comparison.Plugins)
        {
            table.Columns.Add(plugin.PluginName);
        }

        foreach (var field in comparison.Fields)
        {
            var row = table.NewRow();
            row["Field"] = field.FieldName;
            foreach (var plugin in comparison.Plugins)
            {
                row[plugin.PluginName] = field.ValuesByPlugin.TryGetValue(plugin.PluginName, out var value)
                    ? value ?? string.Empty
                    : string.Empty;
            }

            table.Rows.Add(row);
        }

        RecordsDataGrid.ItemsSource = table.DefaultView;
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
            Binding = new Binding("[Field]"),
            Width = new DataGridLength(240)
        });

        foreach (var pluginName in pluginNames)
        {
            RecordsDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = pluginName,
                Binding = new Binding($"[{pluginName}]"),
                Width = new DataGridLength(1, DataGridLengthUnitType.Star)
            });
        }
    }
}
