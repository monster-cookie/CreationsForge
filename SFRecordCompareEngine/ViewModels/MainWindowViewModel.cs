using System.Configuration;
using Serilog;
using SFRecordCompareEngine.Core.Configuration.Interfaces;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly ILogger Logger;
    private readonly IGameConfigurationStore GameConfigurationStore;
    private readonly IPluginImportService PluginImportService;
    private bool DatabaseImportCompleted;

    public MainWindowViewModel(IPluginImportService pluginImportService,
        IGameConfigurationStore gameConfigurationStore,
        ILogger logger)
    {
        PluginImportService = pluginImportService;
        GameConfigurationStore = gameConfigurationStore;
        Logger = logger.ForContext<MainWindowViewModel>() ?? logger;
        LoadedGameText = "None";
        LoadedPluginText = "None";
        StatusText = "Use File > Open to choose a game and plugin.";
    }

    public string LoadedGameText { get; private set; }
    public string LoadedPluginText { get; private set; }

    public bool CanUseApplication
    {
        get;
        private set => SetProperty(ref field, value);
    } = true;

    public bool IsDatabaseImportRunning
    {
        get;
        private set
        {
            if (SetProperty(ref field, value))
            {
                CanUseApplication = !value;
            }
        }
    }

    public string DatabaseImportStatusText
    {
        get;
        private set => SetProperty(ref field, value);
    } = string.Empty;

    public string DatabaseImportCurrentPluginText
    {
        get;
        private set => SetProperty(ref field, value);
    } = string.Empty;

    public double DatabaseImportProgressValue
    {
        get;
        private set => SetProperty(ref field, value);
    }

    public double DatabaseImportProgressMaximum
    {
        get;
        private set => SetProperty(ref field, value);
    } = 100;

    public bool IsDatabaseImportIndeterminate
    {
        get;
        private set => SetProperty(ref field, value);
    } = true;

    public string StatusText
    {
        get;
        private set => SetProperty(ref field, value);
    }

    public async Task InitializeDatabaseImportAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(GameConfigurationStore.SelectedGame)) throw new ConfigurationErrorsException("Select a game before initializing the plugin database.");

        IsDatabaseImportRunning = true;
        DatabaseImportCompleted = false;
        IsDatabaseImportIndeterminate = true;
        DatabaseImportStatusText = "Preparing plugin database import...";
        DatabaseImportCurrentPluginText = string.Empty;
        DatabaseImportProgressValue = 0;
        DatabaseImportProgressMaximum = 100;
        StatusText = "Preparing plugin database import...";

        try
        {
            // TODO: Reimplement after mutagen safe import process setup
            var progress = new Progress<PluginImportProgressDTO>(UpdateDatabaseImportProgress);
            var importResult = await PluginImportService.InitializeAndImportAsync(progress, cancellationToken);
            
            DatabaseImportCompleted = true;
            StatusText = $"Plugin database import completed. Imported {importResult.PluginsImported} plugins.";
            Logger.Information("Plugin database import completed with {PluginCount} imported plugins", importResult.PluginsImported);
        }
        catch (OperationCanceledException)
        {
            DatabaseImportCompleted = true;
            StatusText = "Plugin database import was canceled.";
            Logger.Information("Plugin database import was canceled");
            throw;
        }
        catch (Exception ex)
        {
            DatabaseImportCompleted = true;
            StatusText = "Unable to initialize the plugin database.";
            DatabaseImportStatusText = "Unable to initialize the plugin database.";
            Logger.Error(ex, "Unable to initialize the plugin database");
            throw;
        }
        finally
        {
            IsDatabaseImportRunning = false;
        }
    }

    public void LoadPlugin(string selectedGame, string selectedPluginName)
    {
        LoadedGameText = selectedGame;
        OnPropertyChanged(nameof(LoadedGameText));
        LoadedPluginText = selectedPluginName;
        OnPropertyChanged(nameof(LoadedPluginText));
        StatusText = $"Loaded {selectedPluginName}.";

        Logger.Information("Opened {PluginName} for {Game}", selectedPluginName, selectedGame);
    }

    private void UpdateDatabaseImportProgress(PluginImportProgressDTO progress)
    {
        if (DatabaseImportCompleted) return;

        DatabaseImportStatusText = progress.StatusText;
        DatabaseImportCurrentPluginText = string.IsNullOrWhiteSpace(progress.CurrentPluginName) ? string.Empty : $"Current plugin: {progress.CurrentPluginName}";
        IsDatabaseImportIndeterminate = progress.IsIndeterminate || progress.PluginCount <= 0;
        
        if (progress.PluginCount <= 0) return;
        
        DatabaseImportProgressMaximum = progress.PluginCount;
        DatabaseImportProgressValue = progress.PluginIndex;
    }
}
