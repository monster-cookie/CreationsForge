using System.Collections.ObjectModel;
using System.IO;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Starfield;
using Noggog;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Helpers;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Commands;
using SFRecordCompareEngine.Services.Interfaces;

namespace SFRecordCompareEngine.ViewModels;

public class MainPageViewModel : ViewModelBase
{
    private readonly IApplicationNavigationService ApplicationNavigationService;
    private readonly IActivePluginSelectionService ActivePluginSelectionService;
    private readonly IFormListRepository FormListRepository;
    private readonly IGameSettingRepository GameSettingRepository;
    private IList<RecordTreeItemViewModel> AllRecordTreeItems = new List<RecordTreeItemViewModel>();
    private IReadOnlySeparatedMasterPackage? MasterPackage;

    public MainPageViewModel(
        IApplicationNavigationService applicationNavigationService,
        IActivePluginSelectionService activePluginSelectionService,
        IFormListRepository formListRepository,
        IGameSettingRepository gameSettingRepository)
    {
        ApplicationNavigationService = applicationNavigationService;
        ActivePluginSelectionService = activePluginSelectionService;
        FormListRepository = formListRepository;
        GameSettingRepository = gameSettingRepository;
        OpenCommand = new AsyncRelayCommand(OpenAsync);
        OptionsCommand = new AsyncRelayCommand(ShowOptionsAsync);
        ExitCommand = new RelayCommand(ApplicationNavigationService.Quit);
        StatusText = GetStatusText();
        ActivePluginSelectionService.ActivePluginChanged += OnActivePluginChanged;
    }

    public AsyncRelayCommand OpenCommand { get; }
    public AsyncRelayCommand OptionsCommand { get; }
    public RelayCommand ExitCommand { get; }

    public ObservableCollection<RecordTreeItemViewModel> RecordTreeItems { get; } = new();

    public string FormIDFilter
    {
        get;
        private set
        {
            if (!SetProperty(ref field, value)) return;
            ApplyFilters();
        }
    } = string.Empty;

    public string EditorIDFilter
    {
        get;
        private set
        {
            if (!SetProperty(ref field, value)) return;
            ApplyFilters();
        }
    } = string.Empty;

    public string StatusText
    {
        get;
        private set => SetProperty(ref field, value);
    }

    private async Task OpenAsync()
    {
        await ApplicationNavigationService.ShowOpenDialogAsync();
    }

    private async Task ShowOptionsAsync()
    {
        await ApplicationNavigationService.ShowSettingsDialogAsync();
    }

    private async void OnActivePluginChanged(object? sender, EventArgs e)
    {
        StatusText = GetStatusText();
        await RefreshRecordTreeAsync();
    }

    private string GetStatusText()
    {
        return ActivePluginSelectionService.ActivePlugin == null
            ? "No active plugin selected."
            : $"Active plugin: {ActivePluginSelectionService.ActivePlugin.ModKey.FileName}";
    }

    private async Task RefreshRecordTreeAsync()
    {
        var activePlugin = ActivePluginSelectionService.ActivePlugin;
        if (activePlugin == null)
        {
            AllRecordTreeItems = new List<RecordTreeItemViewModel>();
            MasterPackage = null;
            ApplyFilters();
            return;
        }

        var tree = await Task.Run(() => BuildRecordTree(activePlugin));
        MasterPackage = tree.MasterPackage;
        AllRecordTreeItems = tree.RecordTreeItems;
        ApplyFilters();
    }

    private (IReadOnlySeparatedMasterPackage MasterPackage, IList<RecordTreeItemViewModel> RecordTreeItems) BuildRecordTree(PluginDTO activePlugin)
    {
        var contextMod = LoadMod(activePlugin.ModKey);
        var masterFlagLookup = new Cache<IModMasterStyledGetter, ModKey>(mod => mod.ModKey);
        masterFlagLookup.Add(contextMod);
        foreach (var masterReference in contextMod.MasterReferences)
        {
            masterFlagLookup.Add(LoadMod(masterReference.Master));
        }
        var masterPackage = SeparatedMasterPackage.Factory(
            GameRelease.Starfield,
            contextMod.ModKey,
            contextMod.GetMasterStyle(),
            new MasterReferenceCollection(contextMod.ModKey, contextMod.MasterReferences),
            masterFlagLookup);
        var recordTreeItems = new List<RecordTreeItemViewModel>();
        AddRecordType(
            recordTreeItems,
            RecordTypeCatalog.FormList.RecordType,
            FormListRepository.GetByModKey(activePlugin.ModKey)
                .Select(record => CreateRecordTreeItem(masterPackage, record.FormKey, record.EditorID)));
        AddRecordType(
            recordTreeItems,
            RecordTypeCatalog.GameSetting.RecordType,
            GameSettingRepository.GetByModKey(activePlugin.ModKey)
                .Select(record => CreateRecordTreeItem(masterPackage, record.FormKey, record.EditorID)));
        return (masterPackage, recordTreeItems);
    }

    private static RecordTreeItemViewModel CreateRecordTreeItem(IReadOnlySeparatedMasterPackage masterPackage, FormKey formKey, string editorID)
    {
        var formID = masterPackage.GetFormID(formKey);
        return new RecordTreeItemViewModel(formID.ToString(), editorID, formKey);
    }

    private static void AddRecordType(
        IList<RecordTreeItemViewModel> recordTreeItems,
        string recordType,
        IEnumerable<RecordTreeItemViewModel> records)
    {
        var recordTypeItem = new RecordTreeItemViewModel(recordType, string.Empty);
        foreach (var record in records)
        {
            recordTypeItem.Children.Add(record);
        }

        if (recordTypeItem.Children.Count > 0)
        {
            recordTreeItems.Add(recordTypeItem);
        }
    }

    private void ApplyFilters()
    {
        RecordTreeItems.Clear();
        foreach (var item in AllRecordTreeItems)
        {
            var filteredItem = FilterItem(item);
            if (filteredItem != null)
            {
                RecordTreeItems.Add(filteredItem);
            }
        }
    }

    private RecordTreeItemViewModel? FilterItem(RecordTreeItemViewModel item)
    {
        var filteredItem = new RecordTreeItemViewModel(item.FormIDText, item.EditorID, item.FormKey);
        foreach (var child in item.Children)
        {
            var filteredChild = FilterItem(child);
            if (filteredChild != null)
            {
                filteredItem.Children.Add(filteredChild);
            }
        }

        if (item.FormKey == null)
        {
            return filteredItem.Children.Count > 0 || string.IsNullOrWhiteSpace(FormIDFilter) && string.IsNullOrWhiteSpace(EditorIDFilter)
                ? filteredItem
                : null;
        }

        return MatchesFormIDFilter(item)
               && item.EditorID.Contains(EditorIDFilter.Trim(), StringComparison.OrdinalIgnoreCase)
            ? filteredItem
            : null;
    }

    private bool MatchesFormIDFilter(RecordTreeItemViewModel item)
    {
        var filter = FormIDFilter.Trim();
        if (string.IsNullOrWhiteSpace(filter))
        {
            return true;
        }

        if (MasterPackage != null && filter.Length == 8 && FormID.TryFactory(filter, out var formID, false))
        {
            try
            {
                return item.FormKey == MasterPackage.GetFormKey(formID, reference: false);
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        return item.FormIDText.Contains(filter, StringComparison.OrdinalIgnoreCase);
    }

    private static IStarfieldModGetter LoadMod(ModKey modKey)
    {
        var environment = GameEnvironment.Typical.Starfield(StarfieldRelease.Starfield);
        return StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(Path.Join(environment.DataFolderPath, modKey.FileName))
            .WithLoadOrderFromHeaderMasters()
            .WithDataFolder(environment.DataFolderPath)
            .Construct();
    }
}
