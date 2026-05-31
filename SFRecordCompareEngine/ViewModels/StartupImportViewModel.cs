using Serilog;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.Services.Interfaces;
using SFRecordCompareEngine.Services.Interfaces;

namespace SFRecordCompareEngine.ViewModels;

public class StartupImportViewModel : ViewModelBase
{
    private readonly IApplicationNavigationService ApplicationNavigationService;
    private readonly CancellationTokenSource CancellationTokenSource = new();
    private readonly ILogger Logger;
    private readonly IPluginImportService PluginImportService;
    private readonly IUserDialogService UserDialogService;
    private bool ImportCompleted;
    private bool ImportStarted;

    public StartupImportViewModel(
        IPluginImportService pluginImportService,
        IApplicationNavigationService applicationNavigationService,
        IUserDialogService userDialogService,
        ILogger logger
    )
    {
        PluginImportService = pluginImportService;
        ApplicationNavigationService = applicationNavigationService;
        UserDialogService = userDialogService;
        Logger = logger.ForContext<StartupImportViewModel>() ?? logger;
        StatusText = "Preparing plugin database import...";
        CurrentPluginText = string.Empty;
    }

    public string StatusText
    {
        get;
        private set => SetProperty(ref field, value);
    }

    public string CurrentPluginText
    {
        get;
        private set => SetProperty(ref field, value);
    }

    public double ProgressValue
    {
        get;
        private set
        {
            if (SetProperty(ref field, value))
            {
                OnPropertyChanged(nameof(ProgressPercentage));
            }
        }
    }

    public double ProgressMaximum
    {
        get;
        private set
        {
            if (SetProperty(ref field, value))
            {
                OnPropertyChanged(nameof(ProgressPercentage));
            }
        }
    } = 100;

    public double ProgressPercentage
    {
        get
        {
            if (ProgressMaximum <= 0)
            {
                return 0;
            }

            return Math.Clamp(ProgressValue / ProgressMaximum, 0, 1);
        }
    }

    public bool IsIndeterminate
    {
        get;
        private set => SetProperty(ref field, value);
    } = true;

    public async Task StartImportAsync()
    {
        if (ImportStarted)
        {
            return;
        }

        ImportStarted = true;
        ImportCompleted = false;
        StatusText = "Preparing plugin database import...";
        CurrentPluginText = string.Empty;
        ProgressValue = 0;
        ProgressMaximum = 100;
        IsIndeterminate = true;

        try
        {
            var progress = new Progress<PluginImportProgressDTO>(UpdateProgress);
            var importResult = await PluginImportService.InitializeAndImportAsync(progress, CancellationTokenSource.Token);

            ImportCompleted = true;
            StatusText = $"Plugin database import completed. Imported {importResult.PluginsImported} plugins.";
            Logger.Information("Plugin database import completed with {PluginCount} imported plugins", importResult.PluginsImported);
            await ApplicationNavigationService.ShowMainPageAsync();
        }
        catch (OperationCanceledException)
        {
            ImportCompleted = true;
            StatusText = "Plugin database import was canceled.";
            Logger.Information("Plugin database import was canceled");
            ApplicationNavigationService.Quit();
        }
        catch (Exception ex)
        {
            ImportCompleted = true;
            StatusText = "Unable to initialize the plugin database.";
            Logger.Error(ex, "Unable to initialize the plugin database");
            await UserDialogService.ShowErrorAsync("Unable to initialize the plugin database. Details were written to the log file.");
        }
    }

    public void CancelImport()
    {
        CancellationTokenSource.Cancel();
    }

    private void UpdateProgress(PluginImportProgressDTO progress)
    {
        if (ImportCompleted)
        {
            return;
        }

        StatusText = progress.StatusText;
        if (string.IsNullOrWhiteSpace(progress.CurrentPluginName))
        {
            CurrentPluginText = string.Empty;
        }
        else if (!string.IsNullOrWhiteSpace(progress.CurrentRecordType) && progress.RecordCount > 0)
        {
            CurrentPluginText = $"Current plugin: {progress.CurrentPluginName} - {progress.CurrentRecordType} {progress.RecordIndex} of {progress.RecordCount}";
        }
        else if (!string.IsNullOrWhiteSpace(progress.CurrentRecordType))
        {
            CurrentPluginText = $"Current plugin: {progress.CurrentPluginName} - {progress.CurrentRecordType}";
        }
        else
        {
            CurrentPluginText = $"Current plugin: {progress.CurrentPluginName}";
        }

        IsIndeterminate = progress.IsIndeterminate || progress.PluginCount <= 0;

        if (progress.PluginCount <= 0)
        {
            return;
        }

        ProgressMaximum = progress.PluginCount;
        ProgressValue = progress.PluginIndex;
        OnPropertyChanged(nameof(ProgressPercentage));
    }
}