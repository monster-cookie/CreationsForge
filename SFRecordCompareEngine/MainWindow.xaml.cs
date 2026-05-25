using System.Windows;
using SFRecordCompareEngine.Core.Configuration.Interfaces;
using SFRecordCompareEngine.ViewModels;

namespace SFRecordCompareEngine;

public partial class MainWindow
{
    private readonly Func<OpenGamePluginDialog> OpenGamePluginDialogFactory;
    private readonly Func<DatabaseImportConfirmationDialog> DatabaseImportConfirmationDialogFactory;
    private readonly Func<StartupConfigurationDialog> StartupConfigurationDialogFactory;
    private readonly IApplicationConfigurationStore ApplicationConfigurationStore;
    private readonly IGameConfigurationStore GameConfigurationStore;
    private readonly MainWindowViewModel ViewModel;
    private readonly CancellationTokenSource StartupImportCancellationTokenSource = new();
    private bool HasPromptedForDatabaseImport;

    public MainWindow(
        MainWindowViewModel viewModel,
        Func<OpenGamePluginDialog> openGamePluginDialogFactory,
        Func<DatabaseImportConfirmationDialog> databaseImportConfirmationDialogFactory,
        Func<StartupConfigurationDialog> startupConfigurationDialogFactory,
        IApplicationConfigurationStore applicationConfigurationStore,
        IGameConfigurationStore gameConfigurationStore)
    {
        InitializeComponent();

        ViewModel = viewModel;
        OpenGamePluginDialogFactory = openGamePluginDialogFactory;
        DatabaseImportConfirmationDialogFactory = databaseImportConfirmationDialogFactory;
        StartupConfigurationDialogFactory = startupConfigurationDialogFactory;
        ApplicationConfigurationStore = applicationConfigurationStore;
        GameConfigurationStore = gameConfigurationStore;
        DataContext = ViewModel;

        Loaded += MainWindow_Loaded;
        Closing += MainWindow_Closing;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (HasPromptedForDatabaseImport) return;
        HasPromptedForDatabaseImport = true;

        if (ApplicationConfigurationStore.IsConfigurationRequired)
        {
            var configurationDialog = StartupConfigurationDialogFactory();
            configurationDialog.Owner = this;
            if (configurationDialog.ShowDialog() != true)
            {
                Close();
                return;
            }
        }
        else
        {
            GameConfigurationStore.SelectGame(ApplicationConfigurationStore.Current.SelectedGame);
        }

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
        if (!ViewModel.CanUseApplication) return;

        var dialog = OpenGamePluginDialogFactory();
        dialog.Owner = this;

        if (dialog.ShowDialog() != true) return;
        if (string.IsNullOrWhiteSpace(GameConfigurationStore.SelectedGame) ||
            string.IsNullOrWhiteSpace(dialog.ViewModel.SelectedPluginName)) return;

        ViewModel.LoadPlugin(GameConfigurationStore.SelectedGame, dialog.ViewModel.SelectedPluginName);
    }

    private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
