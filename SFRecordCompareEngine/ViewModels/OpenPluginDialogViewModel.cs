using System.Collections.ObjectModel;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Commands;
using SFRecordCompareEngine.Services.Interfaces;

namespace SFRecordCompareEngine.ViewModels;

public class OpenPluginDialogViewModel : ViewModelBase
{
    private readonly IApplicationNavigationService ApplicationNavigationService;
    private readonly IActivePluginSelectionService ActivePluginSelectionService;
    private readonly IPluginRepository PluginRepository;
    private IList<PluginDTO> MatchingPlugins = new List<PluginDTO>();

    public OpenPluginDialogViewModel(
        IApplicationNavigationService applicationNavigationService,
        IActivePluginSelectionService activePluginSelectionService,
        IPluginRepository pluginRepository)
    {
        ApplicationNavigationService = applicationNavigationService;
        ActivePluginSelectionService = activePluginSelectionService;
        PluginRepository = pluginRepository;
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
            if (!SetProperty(ref field, value)) return;
            RefreshSuggestions(value);
        }
    } = string.Empty;

    public string SelectionStatus
    {
        get;
        private set => SetProperty(ref field, value);
    } = "Select a plugin to load.";

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

    private PluginDTO? SelectedPlugin
    {
        get;
        set
        {
            if (!SetProperty(ref field, value)) return;
            SelectionStatus = value == null ? "Select a plugin to load." : $"Ready to load {value.ModKey.FileName}.";
            LoadCommand.RaiseCanExecuteChanged();
        }
    }

    private void RefreshSuggestions(string searchText)
    {
        MatchingPlugins = string.IsNullOrWhiteSpace(searchText)
            ? PluginRepository.GetOpenablePlugins()
            : PluginRepository.SearchOpenablePluginsByFilename(searchText);

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
        if (SelectedPlugin == null) return;

        ActivePluginSelectionService.SetActivePlugin(SelectedPlugin);
        await ApplicationNavigationService.CloseOpenDialogAsync();
    }

    private async Task CancelAsync()
    {
        await ApplicationNavigationService.CloseOpenDialogAsync();
    }
}
