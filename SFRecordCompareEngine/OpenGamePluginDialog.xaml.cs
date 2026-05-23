using SFRecordCompareEngine.ViewModels;
using System.Windows.Controls;

namespace SFRecordCompareEngine;

public partial class OpenGamePluginDialog
{
    public OpenGamePluginDialog(OpenGamePluginDialogViewModel viewModel)
    {
        InitializeComponent();

        ViewModel = viewModel;
        DataContext = ViewModel;
    }

    public OpenGamePluginDialogViewModel ViewModel { get; }

    private void OpenButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        if (ViewModel.TryConfirmOpen())
        {
            DialogResult = true;
        }
    }

    private void PluginComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (PluginComboBox.SelectedItem is string pluginName)
        {
            ViewModel.SelectPluginSearchResult(pluginName);
        }
    }
}
