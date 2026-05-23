using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.ViewModels;

namespace SFRecordCompareEngine;

public partial class MainWindow : Window
{
    private readonly Func<OpenGamePluginDialog> OpenGamePluginDialogFactory;
    private readonly MainWindowViewModel ViewModel;

    public MainWindow(
        MainWindowViewModel viewModel,
        Func<OpenGamePluginDialog> openGamePluginDialogFactory)
    {
        InitializeComponent();

        ViewModel = viewModel;
        OpenGamePluginDialogFactory = openGamePluginDialogFactory;
        DataContext = ViewModel;

        ConfigureSummaryGrid();
    }

    private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var dialog = OpenGamePluginDialogFactory();
        dialog.Owner = this;

        if (dialog.ShowDialog() != true) return;
        if (string.IsNullOrWhiteSpace(dialog.ViewModel.SelectedGame) ||
            string.IsNullOrWhiteSpace(dialog.ViewModel.SelectedPluginName)) return;

        ViewModel.LoadPlugin(dialog.ViewModel.SelectedGame, dialog.ViewModel.SelectedPluginName);
        ApplyGridColumns();
    }

    private void RecordsTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        ViewModel.SelectRecordTreeItem(e.NewValue);
        ApplyGridColumns();
    }

    private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ApplyGridColumns()
    {
        if (ViewModel.IsComparisonMode)
        {
            ConfigureComparisonGrid(ViewModel.ComparisonPluginNames);
            return;
        }

        ConfigureSummaryGrid();
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
