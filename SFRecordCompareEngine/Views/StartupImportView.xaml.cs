using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SFRecordCompareEngine.ViewModels;

namespace SFRecordCompareEngine.Views;

public sealed partial class StartupImportView : UserControl
{
    private readonly StartupImportViewModel ViewModel;

    public StartupImportView(StartupImportViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        DataContext = viewModel;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.StartImportAsync();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        ViewModel.CancelImport();
    }
}
