using System.Windows;
using SFRecordCompareEngine.Core.Configuration.Interfaces;
using SFRecordCompareEngine.ViewModels;

namespace SFRecordCompareEngine;

public partial class MainWindow
{
    private readonly Func<DatabaseImportConfirmationDialog> DatabaseImportConfirmationDialogFactory;
    private readonly IApplicationConfigurationStore ApplicationConfigurationStore;
    private readonly MainWindowViewModel ViewModel;
    private readonly CancellationTokenSource StartupImportCancellationTokenSource = new();
    private bool HasPromptedForDatabaseImport;

    public MainWindow(
        MainWindowViewModel viewModel,
        Func<DatabaseImportConfirmationDialog> databaseImportConfirmationDialogFactory,
        IApplicationConfigurationStore applicationConfigurationStore
    )
    {
        InitializeComponent();

        ViewModel = viewModel;
        DatabaseImportConfirmationDialogFactory = databaseImportConfirmationDialogFactory;
        ApplicationConfigurationStore = applicationConfigurationStore;
        DataContext = ViewModel;

        Loaded += MainWindow_Loaded;
        Closing += MainWindow_Closing;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (HasPromptedForDatabaseImport) return;
        HasPromptedForDatabaseImport = true;

        var dialog = DatabaseImportConfirmationDialogFactory();
        dialog.Owner = this;
        var result = dialog.ShowDialog();

        if (result != true)
        {
            Close();
            return;
        }

        try
        {
            await ViewModel.InitializeDatabaseImportAsync(StartupImportCancellationTokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Unable to initialize the plugin database. Details were written to the log file.{Environment.NewLine}{ex.Message}",
                "SF Record Compare Engine",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        StartupImportCancellationTokenSource.Cancel();
        StartupImportCancellationTokenSource.Dispose();
    }

    private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
    {
    }

    private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
