using SFRecordCompareEngine.ViewModels;

namespace SFRecordCompareEngine;

public partial class StartupConfigurationDialog
{
    public StartupConfigurationDialog(StartupConfigurationDialogViewModel viewModel)
    {
        InitializeComponent();

        ViewModel = viewModel;
        DataContext = ViewModel;
    }

    public StartupConfigurationDialogViewModel ViewModel { get; }

    private void ContinueButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        if (ViewModel.TrySave())
        {
            DialogResult = true;
        }
    }

    private void ExitButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
