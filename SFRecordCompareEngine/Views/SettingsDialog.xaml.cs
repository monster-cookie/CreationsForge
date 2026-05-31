using Microsoft.UI.Xaml.Controls;
using SFRecordCompareEngine.ViewModels;

namespace SFRecordCompareEngine.Views;

public sealed partial class SettingsDialog : ContentDialog
{
    private readonly SettingsViewModel ViewModel;

    public SettingsDialog(SettingsViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        DataContext = viewModel;
    }

    private void OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        ViewModel.Save();
    }
}