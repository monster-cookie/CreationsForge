using System.Windows;
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
                RecordsDataGrid.ItemsSource = node.Records;
                StatusTextBlock.Text = $"Loaded {node.Records.Count} {node.Name} records.";
                break;
            case RecordSummaryDTO record:
                RecordsDataGrid.SelectedItem = record;
                RecordsDataGrid.ScrollIntoView(record);
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
        StatusTextBlock.Text = nodes.Count == 1
            ? "Loaded 1 record type."
            : $"Loaded {nodes.Count} record types.";
    }
}
