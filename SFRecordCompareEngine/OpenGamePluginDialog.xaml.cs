using System.Windows;
using System.Windows.Controls;
using Serilog;
using SFRecordCompareEngine.Core.Configuration.Interfaces;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine;

public partial class OpenGamePluginDialog
{
    private readonly IGameConfigurationStore GameConfigurationStore;
    private readonly ILogger Logger = Log.ForContext<OpenGamePluginDialog>();
    private readonly IPluginService PluginService;
    private bool IsUpdatingPluginItems;

    public OpenGamePluginDialog(IGameConfigurationStore gameConfigurationStore, IPluginService pluginService)
    {
        InitializeComponent();

        GameConfigurationStore = gameConfigurationStore;
        PluginService = pluginService;
        GameComboBox.ItemsSource = gameConfigurationStore.SupportedGames;
        GameComboBox.SelectedIndex = 0;
    }

    public string? SelectedGame { get; private set; }
    public string? SelectedPluginName { get; private set; }
    private PluginHeaderDTO? SelectedPluginHeader { get; set; }

    private void GameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ApplySelectedGame();
        ClearPluginHeader();
        LoadPlugins();
    }

    private void PluginComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!IsUpdatingPluginItems)
        {
            ClearPluginHeader();
        }
    }

    private void PluginComboBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
    {
        FilterPlugins(PluginComboBox.Text);
    }

    private void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        LoadPlugins();
    }

    private void LoadButton_Click(object sender, RoutedEventArgs e)
    {
        LoadPluginHeader();
    }

    private void OpenButton_Click(object sender, RoutedEventArgs e)
    {
        var pluginName = PluginComboBox.Text?.Trim();
        if (SelectedPluginHeader is null || !string.Equals(SelectedPluginName, pluginName, StringComparison.OrdinalIgnoreCase))
        {
            LoadPluginHeader();
        }

        if (SelectedPluginHeader is null)
        {
            return;
        }

        DialogResult = true;
    }

    private void LoadPlugins()
    {
        try
        {
            Logger.Information("Loading plugins for {Game}", GameConfigurationStore.SelectedGame);

            var plugins = PluginService.GetPlugins();
            SetPluginItems(plugins);
            PluginComboBox.SelectedIndex = plugins.Count > 0 ? 0 : -1;
            StatusTextBlock.Text = GameConfigurationStore.Game is null
                ? $"{GameConfigurationStore.SelectedGame} is not configured yet."
                : plugins.Count == 1
                    ? "Loaded 1 plugin."
                    : $"Loaded {plugins.Count} plugins.";

            Logger.Information("Loaded {PluginCount} plugins for {Game}", plugins.Count, GameConfigurationStore.SelectedGame);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Unable to load plugins");
            PluginComboBox.ItemsSource = null;
            StatusTextBlock.Text = $"Unable to load plugins: {ex.Message}";
        }
    }

    private void LoadPluginHeader()
    {
        var pluginName = PluginComboBox.Text?.Trim();
        if (string.IsNullOrWhiteSpace(pluginName))
        {
            ClearPluginHeader();
            StatusTextBlock.Text = "Select a plugin before loading plugin header data.";
            return;
        }

        try
        {
            Logger.Information("Loading plugin header for {PluginName}", pluginName);

            var pluginHeader = PluginService.GetPluginHeader(pluginName);
            if (pluginHeader is null)
            {
                ClearPluginHeader();
                StatusTextBlock.Text = $"Unable to load plugin header for {pluginName}.";
                Logger.Warning("Plugin header was not returned for {PluginName}", pluginName);
                return;
            }

            SelectedGame = GameComboBox.Text?.Trim();
            SelectedPluginName = pluginName;
            SelectedPluginHeader = pluginHeader;

            PluginNameTextBlock.Text = pluginHeader.Name;
            PluginAuthorTextBlock.Text = pluginHeader.Author;
            PluginVersionTextBlock.Text = pluginHeader.Version.ToString();
            PluginDescriptionTextBlock.Text = pluginHeader.Description;
            PluginMastersTextBlock.Text = pluginHeader.Masters.Count == 0 ? "None" : string.Join(", ", pluginHeader.Masters);
            StatusTextBlock.Text = $"Loaded plugin header for {pluginName}.";
            OpenButton.IsEnabled = true;

            Logger.Information("Loaded plugin header for {PluginName}", pluginName);
        }
        catch (Exception ex)
        {
            ClearPluginHeader();
            Logger.Error(ex, "Unable to load plugin header for {PluginName}", pluginName);
            StatusTextBlock.Text = $"Unable to load plugin header for {pluginName}: {ex.Message}";
        }
    }

    private void ClearPluginHeader()
    {
        SelectedPluginName = null;
        SelectedPluginHeader = null;
        OpenButton.IsEnabled = false;
        PluginNameTextBlock.Text = string.Empty;
        PluginAuthorTextBlock.Text = string.Empty;
        PluginVersionTextBlock.Text = string.Empty;
        PluginDescriptionTextBlock.Text = string.Empty;
        PluginMastersTextBlock.Text = string.Empty;
    }

    private void ApplySelectedGame()
    {
        var selectedGame = GameComboBox.SelectedItem as string;
        GameConfigurationStore.SelectGame(selectedGame);
    }

    private void FilterPlugins(string? searchText)
    {
        if (GameConfigurationStore.Game is null)
        {
            return;
        }

        var plugins = PluginService.SearchPlugins(searchText ?? string.Empty);
        SetPluginItems(plugins);
        PluginComboBox.Text = searchText ?? string.Empty;
        PluginComboBox.IsDropDownOpen = plugins.Count > 0;
    }

    private void SetPluginItems(IList<string> plugins)
    {
        IsUpdatingPluginItems = true;
        try
        {
            PluginComboBox.ItemsSource = plugins;
        }
        finally
        {
            IsUpdatingPluginItems = false;
        }
    }
}
