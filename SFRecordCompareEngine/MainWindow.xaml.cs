using System.Windows;
using Serilog;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine;

public partial class MainWindow : Window
{
    private readonly ILogger Logger = Log.ForContext<MainWindow>();
    private readonly IGameEngineService GameEngineService;
    private readonly IPluginService PluginService;

    public MainWindow(IGameEngineService gameEngineService, IPluginService pluginService)
    {
        InitializeComponent();
        
        GameEngineService = gameEngineService;
        if (!GameEngineService.ValidateStarfieldPluginHeaders(@"C:\Steam\steamapps\common\Starfield\Data"))
        {
            MessageBox.Show("One or more Starfield plugins have malformed headers. Please check the logs for details.", "Plugin Header Validation Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
            Environment.Exit(1);
        }

        PluginService = pluginService;
        LoadDatabases();
    }

    private void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        LoadDatabases();
    }

    private void LoadButton_Click(object sender, RoutedEventArgs e)
    {
        LoadPluginHeader();
    }

    private void LoadDatabases()
    {
        try
        {
            Logger.Information("Loading databases.");

            var databases = PluginService.GetDatabases();
            DatabaseComboBox.ItemsSource = databases;
            DatabaseComboBox.SelectedIndex = databases.Count > 0 ? 0 : -1;
            StatusTextBlock.Text = databases.Count == 1
                ? "Loaded 1 database."
                : $"Loaded {databases.Count} databases.";

            Logger.Information("Loaded {DatabaseCount} databases.", databases.Count);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Unable to load databases.");
            DatabaseComboBox.ItemsSource = null;
            StatusTextBlock.Text = $"Unable to load databases: {ex.Message}";
        }
    }

    private void LoadPluginHeader()
    {
        var pluginName = DatabaseComboBox.Text?.Trim();
        if (string.IsNullOrWhiteSpace(pluginName))
        {
            ClearPluginHeader();
            StatusTextBlock.Text = "Select a database before loading plugin header data.";
            return;
        }

        try
        {
            Logger.Information("Loading plugin header for {PluginName}.", pluginName);

            var pluginHeader = PluginService.GetPluginHeader(pluginName);
            if (pluginHeader is null)
            {
                ClearPluginHeader();
                StatusTextBlock.Text = $"Unable to load plugin header for {pluginName}.";
                Logger.Warning("Plugin header was not returned for {PluginName}.", pluginName);
                return;
            }

            PluginNameTextBlock.Text = pluginHeader.Name;
            PluginAuthorTextBlock.Text = pluginHeader.Author;
            PluginVersionTextBlock.Text = pluginHeader.Version.ToString();
            PluginDescriptionTextBlock.Text = pluginHeader.Description;
            PluginMastersTextBlock.Text = pluginHeader.Masters.Count == 0
                ? "None"
                : string.Join(", ", pluginHeader.Masters.Select(master => master.String));
            StatusTextBlock.Text = $"Loaded plugin header for {pluginName}.";

            Logger.Information("Loaded plugin header for {PluginName}.", pluginName);
        }
        catch (Exception ex)
        {
            ClearPluginHeader();
            Logger.Error(ex, "Unable to load plugin header for {PluginName}.", pluginName);
            StatusTextBlock.Text = $"Unable to load plugin header for {pluginName}: {ex.Message}";
        }
    }

    private void ClearPluginHeader()
    {
        PluginNameTextBlock.Text = string.Empty;
        PluginAuthorTextBlock.Text = string.Empty;
        PluginVersionTextBlock.Text = string.Empty;
        PluginDescriptionTextBlock.Text = string.Empty;
        PluginMastersTextBlock.Text = string.Empty;
    }
}
