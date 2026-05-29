using Microsoft.UI.Xaml.Controls;
using SFRecordCompareEngine.ViewModels;

namespace SFRecordCompareEngine.Views;

public sealed partial class OpenPluginDialog : ContentDialog
{
    public OpenPluginDialog(OpenPluginDialogViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
