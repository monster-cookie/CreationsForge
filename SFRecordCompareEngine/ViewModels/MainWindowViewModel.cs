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
    private readonly IPluginService PluginService;
    private bool DatabaseImportCompleted;
    private string? LoadedPluginName;

    public MainWindowViewModel(
        IPluginService pluginService,
        IPluginImportService pluginImportService,
        IGameConfigurationStore gameConfigurationStore,
        ILogger logger)
    {
        PluginService = pluginService;
        PluginImportService = pluginImportService;
        GameConfigurationStore = gameConfigurationStore;
        Logger = logger.ForContext<MainWindowViewModel>() ?? logger;
        LoadedGameText = "None";
        LoadedPluginText = "None";
        StatusText = "Use File > Open to choose a game and plugin.";
    }

    public string LoadedGameText { get; private set; }
    public string LoadedPluginText { get; private set; }

    private bool _canUseApplication = true;
    public bool CanUseApplication
    {
        get => _canUseApplication;
        private set => SetProperty(ref _canUseApplication, value);
    }

    private bool _isDatabaseImportRunning;
    public bool IsDatabaseImportRunning
    {
        get => _isDatabaseImportRunning;
        private set
        {
            if (SetProperty(ref _isDatabaseImportRunning, value))
            {
                CanUseApplication = !value;
            }
        }
    }

    private string _databaseImportStatusText = string.Empty;
    public string DatabaseImportStatusText
    {
        get => _databaseImportStatusText;
        private set => SetProperty(ref _databaseImportStatusText, value);
    }

    private string _databaseImportCurrentPluginText = string.Empty;
    public string DatabaseImportCurrentPluginText
    {
        get => _databaseImportCurrentPluginText;
        private set => SetProperty(ref _databaseImportCurrentPluginText, value);
    }

    private double _databaseImportProgressValue;
    public double DatabaseImportProgressValue
    {
        get => _databaseImportProgressValue;
        private set => SetProperty(ref _databaseImportProgressValue, value);
    }

    private double _databaseImportProgressMaximum = 100;
    public double DatabaseImportProgressMaximum
    {
        get => _databaseImportProgressMaximum;
        private set => SetProperty(ref _databaseImportProgressMaximum, value);
    }

    private bool _isDatabaseImportIndeterminate = true;
    public bool IsDatabaseImportIndeterminate
    {
        get => _isDatabaseImportIndeterminate;
        private set => SetProperty(ref _isDatabaseImportIndeterminate, value);
    }

    private string _statusText = string.Empty;
    public string StatusText
    {
        get => _statusText;
        private set => SetProperty(ref _statusText, value);
    }

    private IList<RecordTypeTreeNode> _recordTypeNodes = new List<RecordTypeTreeNode>();
    public IList<RecordTypeTreeNode> RecordTypeNodes
    {
        get => _recordTypeNodes;
        private set => SetProperty(ref _recordTypeNodes, value);
    }

    private object? _recordsGridItems;
    public object? RecordsGridItems
    {
        get => _recordsGridItems;
        private set => SetProperty(ref _recordsGridItems, value);
    }

    public IList<string> ComparisonPluginNames { get; private set; } = new List<string>();
    public bool IsComparisonMode { get; private set; }

    public async Task InitializeDatabaseImportAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(GameConfigurationStore.SelectedGame))
        {
            throw new InvalidOperationException("Select a game before initializing the plugin database.");
        }

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
        LoadedPluginName = selectedPluginName;
        StatusText = $"Loaded {selectedPluginName}.";

        Logger.Information("Opened {PluginName} for {Game}", selectedPluginName, selectedGame);
    }

    private void UpdateDatabaseImportProgress(PluginImportProgressDTO progress)
    {
        if (DatabaseImportCompleted) return;

        DatabaseImportStatusText = progress.StatusText;
        DatabaseImportCurrentPluginText = string.IsNullOrWhiteSpace(progress.CurrentPluginName)
            ? string.Empty
            : $"Current plugin: {progress.CurrentPluginName}";
        IsDatabaseImportIndeterminate = progress.IsIndeterminate || progress.PluginCount <= 0;
        if (progress.PluginCount > 0)
        {
            DatabaseImportProgressMaximum = progress.PluginCount;
            DatabaseImportProgressValue = progress.PluginIndex;
        }
    }
}
