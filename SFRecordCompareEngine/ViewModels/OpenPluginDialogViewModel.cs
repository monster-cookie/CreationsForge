using System.Collections.ObjectModel;
using SFRecordCompareEngine.Commands;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.Services.Interfaces;
using SFRecordCompareEngine.Services.Interfaces;

namespace SFRecordCompareEngine.ViewModels;

public class OpenPluginDialogViewModel : ViewModelBase
{
    private readonly IActivePluginSelectionService ActivePluginSelectionService;
    private readonly IApplicationNavigationService ApplicationNavigationService;
    private readonly IPluginService PluginService;
    private IList<PluginDTO> MatchingPlugins = new List<PluginDTO>();

    public OpenPluginDialogViewModel(
        IApplicationNavigationService applicationNavigationService,
        IActivePluginSelectionService activePluginSelectionService,
        IPluginService pluginService)
    {
        ApplicationNavigationService = applicationNavigationService;
        ActivePluginSelectionService = activePluginSelectionService;
        PluginService = pluginService;
        LoadCommand = new AsyncRelayCommand(LoadAsync, CanLoad);
        CancelCommand = new AsyncRelayCommand(CancelAsync);
        RefreshSuggestions(string.Empty);
    }

    public ObservableCollection<string> PluginSuggestions { get; } = new();

    public AsyncRelayCommand LoadCommand { get; }
    public AsyncRelayCommand CancelCommand { get; }

    public string SearchText
    {
        get;
        private set
        {
            if (!SetProperty(ref field, value))
            {
                return;
            }

            RefreshSuggestions(value);
        }
    } = string.Empty;

    public string SelectionStatus
    {
        get;
        private set => SetProperty(ref field, value);
    } = "Select a plugin to load.";

    private PluginDTO? SelectedPlugin
    {
        get;
        set
        {
            if (!SetProperty(ref field, value))
            {
                return;
            }

            SelectionStatus = value == null ? "Select a plugin to load." : $"Ready to load {value.ModKey.FileName}.";
            LoadCommand.RaiseCanExecuteChanged();
        }
    }

    public void UpdateSearchText(string searchText)
    {
        SearchText = searchText;
    }

    public void ChooseSuggestion(string pluginFileName)
    {
        SearchText = pluginFileName;
        SelectPluginByFileName(pluginFileName);
    }

    public void SubmitQuery(string queryText)
    {
        SearchText = queryText;
        SelectPluginByFileName(queryText);
    }

    private bool CanLoad()
    {
        return SelectedPlugin != null;
    }

    private void RefreshSuggestions(string searchText)
    {
        MatchingPlugins = string.IsNullOrWhiteSpace(searchText)
            ? PluginService.GetOpenablePlugins()
            : PluginService.SearchOpenablePluginsByFilename(searchText);

        PluginSuggestions.Clear();
        foreach (var plugin in MatchingPlugins.Take(25))
        {
            PluginSuggestions.Add(plugin.ModKey.FileName.ToString());
        }

        SelectedPlugin = MatchingPlugins.FirstOrDefault(plugin =>
            string.Equals(plugin.ModKey.FileName.ToString(), searchText, StringComparison.OrdinalIgnoreCase));
    }

    private void SelectPluginByFileName(string pluginFileName)
    {
        SelectedPlugin = MatchingPlugins.FirstOrDefault(plugin =>
            string.Equals(plugin.ModKey.FileName.ToString(), pluginFileName, StringComparison.OrdinalIgnoreCase));
    }

    private async Task LoadAsync()
    {
        if (SelectedPlugin == null)
        {
            return;
        }

        ActivePluginSelectionService.SetActivePlugin(SelectedPlugin);
        await ApplicationNavigationService.CloseOpenDialogAsync();
    }

    private async Task CancelAsync()
    {
        await ApplicationNavigationService.CloseOpenDialogAsync();
    }
}