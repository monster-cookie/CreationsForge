using System.Collections.ObjectModel;
using System.Windows.Input;
using Serilog;
using SFRecordCompareEngine.Commands;
using SFRecordCompareEngine.Core.Configuration.Interfaces;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.ViewModels;

public class OpenGamePluginDialogViewModel : ViewModelBase
{
    private readonly IGameConfigurationStore GameConfigurationStore;
    private readonly ILogger Logger;
    private readonly IPluginService PluginService;
    private string _pluginAuthor = string.Empty;
    private string _pluginDescription = string.Empty;
    private string _pluginMasters = string.Empty;
    private string _pluginName = string.Empty;
    private string _pluginSearchText = string.Empty;
    private string _pluginVersion = string.Empty;
    private string? _selectedGame;
    private PluginHeaderDTO? _selectedPluginHeader;
    private string? _selectedPluginName;
    private string _statusText = string.Empty;
    private IList<string> AllPluginItems = new List<string>();
    private bool IsLoadingPlugins;
    private bool IsSelectingPluginSearchResult;
    private bool IsUpdatingPluginItems;

    public OpenGamePluginDialogViewModel(
        IGameConfigurationStore gameConfigurationStore,
        IPluginService pluginService,
        ILogger logger)
    {
        GameConfigurationStore = gameConfigurationStore;
        PluginService = pluginService;
        Logger = logger.ForContext<OpenGamePluginDialogViewModel>() ?? logger;

        SupportedGames = gameConfigurationStore.SupportedGames;
        LoadPluginHeaderCommand = new RelayCommand(LoadPluginHeader, CanLoadPluginHeader);
        RefreshPluginsCommand = new RelayCommand(LoadPlugins);

        SelectedGame = SupportedGames.FirstOrDefault();
    }

    public string[] SupportedGames { get; }
    public ObservableCollection<string> PluginItems { get; } = new();
    public ICommand LoadPluginHeaderCommand { get; }
    public ICommand RefreshPluginsCommand { get; }

    public string? SelectedGame
    {
        get => _selectedGame;
        set
        {
            if (!SetProperty(ref _selectedGame, value))
            {
                return;
            }

            ApplySelectedGame();
            ClearPluginHeader();
            LoadPlugins();
        }
    }

    public string PluginSearchText
    {
        get => _pluginSearchText;
        set
        {
            if (!SetProperty(ref _pluginSearchText, value))
            {
                return;
            }

            ClearPluginHeader();
            if (!IsLoadingPlugins && !IsUpdatingPluginItems && !IsSelectingPluginSearchResult)
            {
                FilterPlugins();
            }

            RaiseCommandStates();
        }
    }

    public string? SelectedPluginName
    {
        get => _selectedPluginName;
        private set => SetProperty(ref _selectedPluginName, value);
    }

    public string PluginName
    {
        get => _pluginName;
        private set => SetProperty(ref _pluginName, value);
    }

    public string PluginAuthor
    {
        get => _pluginAuthor;
        private set => SetProperty(ref _pluginAuthor, value);
    }

    public string PluginVersion
    {
        get => _pluginVersion;
        private set => SetProperty(ref _pluginVersion, value);
    }

    public string PluginDescription
    {
        get => _pluginDescription;
        private set => SetProperty(ref _pluginDescription, value);
    }

    public string PluginMasters
    {
        get => _pluginMasters;
        private set => SetProperty(ref _pluginMasters, value);
    }

    public string StatusText
    {
        get => _statusText;
        private set => SetProperty(ref _statusText, value);
    }

    public bool CanOpen => _selectedPluginHeader is not null
        && string.Equals(SelectedPluginName, PluginSearchText.Trim(), StringComparison.OrdinalIgnoreCase);

    public bool TryConfirmOpen()
    {
        var pluginName = PluginSearchText.Trim();
        if (_selectedPluginHeader is null || !string.Equals(SelectedPluginName, pluginName, StringComparison.OrdinalIgnoreCase))
        {
            LoadPluginHeader();
        }

        return _selectedPluginHeader is not null;
    }

    public void SelectPluginSearchResult(string pluginName)
    {
        if (string.IsNullOrWhiteSpace(pluginName))
        {
            return;
        }

        IsSelectingPluginSearchResult = true;
        try
        {
            PluginSearchText = pluginName;
        }
        finally
        {
            IsSelectingPluginSearchResult = false;
        }

        ClearPluginHeader();
        RaiseCommandStates();
    }

    private void ApplySelectedGame()
    {
        GameConfigurationStore.SelectGame(SelectedGame);
    }

    private void LoadPlugins()
    {
        try
        {
            Logger.Information("Loading plugins for {Game}", GameConfigurationStore.SelectedGame);

            var plugins = PluginService.GetPlugins();
            AllPluginItems = plugins.ToList();
            IsLoadingPlugins = true;
            try
            {
                SetPluginItems(plugins);
                PluginSearchText = plugins.Count > 0 ? plugins[0] : string.Empty;
            }
            finally
            {
                IsLoadingPlugins = false;
            }

            StatusText = GameConfigurationStore.Game is null
                ? $"{GameConfigurationStore.SelectedGame} is not configured yet."
                : plugins.Count == 1
                    ? "Loaded 1 plugin."
                    : $"Loaded {plugins.Count} plugins.";

            Logger.Information("Loaded {PluginCount} plugins for {Game}", plugins.Count, GameConfigurationStore.SelectedGame);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Unable to load plugins");
            SetPluginItems([]);
            StatusText = $"Unable to load plugins: {ex.Message}";
        }
    }

    private void LoadPluginHeader()
    {
        var pluginName = PluginSearchText.Trim();
        if (string.IsNullOrWhiteSpace(pluginName))
        {
            ClearPluginHeader();
            StatusText = "Select a plugin before loading plugin header data.";
            return;
        }

        try
        {
            Logger.Information("Loading plugin header for {PluginName}", pluginName);

            var pluginHeader = PluginService.GetPluginHeader(pluginName);
            if (pluginHeader is null)
            {
                ClearPluginHeader();
                StatusText = $"Unable to load plugin header for {pluginName}.";
                Logger.Warning("Plugin header was not returned for {PluginName}", pluginName);
                return;
            }

            SelectedPluginName = pluginName;
            _selectedPluginHeader = pluginHeader;
            PluginName = pluginHeader.Name;
            PluginAuthor = pluginHeader.Author;
            PluginVersion = pluginHeader.Version.ToString();
            PluginDescription = pluginHeader.Description;
            PluginMasters = pluginHeader.Masters.Count == 0 ? "None" : string.Join(", ", pluginHeader.Masters);
            StatusText = $"Loaded plugin header for {pluginName}.";
            OnPropertyChanged(nameof(CanOpen));
            RaiseCommandStates();

            Logger.Information("Loaded plugin header for {PluginName}", pluginName);
        }
        catch (Exception ex)
        {
            ClearPluginHeader();
            Logger.Error(ex, "Unable to load plugin header for {PluginName}", pluginName);
            StatusText = $"Unable to load plugin header for {pluginName}: {ex.Message}";
        }
    }

    private void ClearPluginHeader()
    {
        SelectedPluginName = null;
        _selectedPluginHeader = null;
        PluginName = string.Empty;
        PluginAuthor = string.Empty;
        PluginVersion = string.Empty;
        PluginDescription = string.Empty;
        PluginMasters = string.Empty;
        OnPropertyChanged(nameof(CanOpen));
        RaiseCommandStates();
    }

    private void FilterPlugins()
    {
        if (GameConfigurationStore.Game is null)
        {
            return;
        }

        var searchText = PluginSearchText.Trim();
        var plugins = string.IsNullOrWhiteSpace(searchText)
            ? AllPluginItems
            : AllPluginItems
                .Where(plugin => plugin.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                .ToList();

        SetPluginItems(plugins);
    }

    private void SetPluginItems(IList<string> plugins)
    {
        IsUpdatingPluginItems = true;
        try
        {
            PluginItems.Clear();
            foreach (var plugin in plugins)
            {
                PluginItems.Add(plugin);
            }
        }
        finally
        {
            IsUpdatingPluginItems = false;
        }
    }

    private bool CanLoadPluginHeader()
    {
        return !string.IsNullOrWhiteSpace(PluginSearchText);
    }

    private void RaiseCommandStates()
    {
        if (LoadPluginHeaderCommand is RelayCommand loadPluginHeaderCommand)
        {
            loadPluginHeaderCommand.RaiseCanExecuteChanged();
        }
    }
}
