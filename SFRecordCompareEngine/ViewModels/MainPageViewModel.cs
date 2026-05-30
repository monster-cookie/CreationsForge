using System.Collections.ObjectModel;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Starfield;
using Noggog;
using SFRecordCompareEngine.Commands;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.Helpers;
using SFRecordCompareEngine.Core.Services.Interfaces;
using SFRecordCompareEngine.Services.Interfaces;

namespace SFRecordCompareEngine.ViewModels;

public class MainPageViewModel : ViewModelBase
{
    private readonly IApplicationNavigationService ApplicationNavigationService;
    private readonly IActivePluginSelectionService ActivePluginSelectionService;
    private readonly IFormListService FormListService;
    private readonly IGameSettingService GameSettingService;
    private readonly IPluginService PluginService;
    private IList<RecordTreeItemViewModel> AllRecordTreeItems = new List<RecordTreeItemViewModel>();
    private IReadOnlySeparatedMasterPackage? MasterPackage;

    public MainPageViewModel(
        IApplicationNavigationService applicationNavigationService,
        IActivePluginSelectionService activePluginSelectionService,
        IFormListService formListService,
        IGameSettingService gameSettingService,
        IPluginService pluginService)
    {
        ApplicationNavigationService = applicationNavigationService;
        ActivePluginSelectionService = activePluginSelectionService;
        FormListService = formListService;
        GameSettingService = gameSettingService;
        PluginService = pluginService;
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
    public ObservableCollection<RecordComparisonFieldViewModel> RecordComparisonFields { get; } = new();
    public ObservableCollection<RecordComparisonColumnViewModel> RecordComparisonColumns { get; } = new();

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
        ClearRecordComparison();
        await RefreshRecordTreeAsync();
    }

    public void SelectRecord(RecordTreeItemViewModel? item)
    {
        ClearRecordComparison();
        if (item?.FormKey == null || item.RecordType == null) return;

        if (item.RecordType == RecordTypeCatalog.FormList.RecordType)
        {
            LoadFormListComparison(item.FormKey.Value.ID);
            return;
        }

        if (item.RecordType == RecordTypeCatalog.GameSetting.RecordType)
        {
            LoadGameSettingComparison(item.FormKey.Value.ID);
        }
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
            FormListService.GetByModKey(activePlugin.ModKey)
                .Select(record => CreateRecordTreeItem(masterPackage, record.FormKey, record.EditorID, RecordTypeCatalog.FormList.RecordType)));
        AddRecordType(
            recordTreeItems,
            RecordTypeCatalog.GameSetting.RecordType,
            GameSettingService.GetByModKey(activePlugin.ModKey)
                .Select(record => CreateRecordTreeItem(masterPackage, record.FormKey, record.EditorID, RecordTypeCatalog.GameSetting.RecordType)));
        return (masterPackage, recordTreeItems);
    }

    private static RecordTreeItemViewModel CreateRecordTreeItem(IReadOnlySeparatedMasterPackage masterPackage, FormKey formKey, string editorID, string recordType)
    {
        var formID = masterPackage.GetFormID(formKey);
        return new RecordTreeItemViewModel(formID.ToString(), editorID, formKey, recordType);
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
        var filteredItem = new RecordTreeItemViewModel(item.FormIDText, item.EditorID, item.FormKey, item.RecordType);
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

    private void LoadFormListComparison(uint formKeyID)
    {
        var records = FormListService.GetByFormKeyID(formKeyID);
        var itemLookup = records.ToDictionary(
            record => record.ModKey,
            record => FormListService.GetItems(record.ModKey, record.FormKey));
        var maxItemCount = itemLookup.Count == 0 ? 0 : itemLookup.Max(pair => pair.Value.Count);
        var fields = new List<RecordComparisonFieldViewModel>
        {
            new("Record Header", false),
            new("EditorID"),
            new("FormKey", false),
            new("StarfieldMajorRecordFlags"),
            new("AddToListFormKey")
        };
        for (var itemIndex = 0; itemIndex < maxItemCount; itemIndex++)
        {
            fields.Add(new RecordComparisonFieldViewModel($"Items[{itemIndex}]"));
        }

        SetRecordComparison(
            fields,
            records.Select(record =>
            {
                var values = new List<string>
                {
                    RecordTypeCatalog.FormList.RecordID,
                    record.EditorID,
                    record.FormKey.ToString(),
                    FormatStarfieldMajorRecordFlags(record.StarfieldMajorRecordFlags),
                    record.AddToListFormKey?.ToString() ?? string.Empty
                };
                values.AddRange(itemLookup[record.ModKey].Select(item => item.ItemFormKey.ToString()));
                while (values.Count < fields.Count)
                {
                    values.Add(string.Empty);
                }

                return (record.ModKey, Values: (IReadOnlyList<string>)values);
            }));
    }

    private void LoadGameSettingComparison(uint formKeyID)
    {
        var records = GameSettingService.GetByFormKeyID(formKeyID);
        var fields = new List<RecordComparisonFieldViewModel>
        {
            new("Record Header", false),
            new("EditorID"),
            new("FormKey", false),
            new("StarfieldMajorRecordFlags"),
            new("SettingType"),
            new("Data"),
            new("IsCompressed"),
            new("IsDeleted")
        };
        SetRecordComparison(
            fields,
            records.Select(record => (
                record.ModKey,
                Values: (IReadOnlyList<string>)new List<string>
                {
                    RecordTypeCatalog.GameSetting.RecordID,
                    record.EditorID,
                    record.FormKey.ToString(),
                    FormatStarfieldMajorRecordFlags(record.StarfieldMajorRecordFlags),
                    record.SettingType ?? string.Empty,
                    record.Data ?? string.Empty,
                    record.IsCompressed.ToString(),
                    record.IsDeleted.ToString()
                })));
    }

    private static string FormatStarfieldMajorRecordFlags(StarfieldMajorRecord.StarfieldMajorRecordFlag flags)
    {
        var value = Convert.ToUInt64(flags);
        if (value == 0) return string.Empty;

        var names = new List<string>();
        var knownFlags = 0UL;
        foreach (var flag in Enum.GetValues<StarfieldMajorRecord.StarfieldMajorRecordFlag>())
        {
            var flagValue = Convert.ToUInt64(flag);
            if (flagValue == 0 || (flagValue & (flagValue - 1)) != 0 || (value & flagValue) == 0) continue;

            names.Add(flag.ToString());
            knownFlags |= flagValue;
        }

        var unknownFlags = value & ~knownFlags;
        if (unknownFlags != 0)
        {
            names.Add($"0x{unknownFlags:X}");
        }

        return string.Join(", ", names);
    }

    private void SetRecordComparison(
        IEnumerable<RecordComparisonFieldViewModel> fields,
        IEnumerable<(ModKey ModKey, IReadOnlyList<string> Values)> columns)
    {
        var activeModKey = ActivePluginSelectionService.ActivePlugin?.ModKey;
        var pluginLookup = PluginService.GetImportedPlugins().ToDictionary(plugin => plugin.ModKey);
        var fieldList = fields.ToList();
        var columnList = columns
            .Where(column => pluginLookup.ContainsKey(column.ModKey))
            .OrderBy(column => pluginLookup[column.ModKey].LoadOrderIndex)
            .ToList();
        var states = GetRecordComparisonValueStates(fieldList, columnList);
        foreach (var field in fieldList)
        {
            RecordComparisonFields.Add(field);
        }

        for (var columnIndex = 0; columnIndex < columnList.Count; columnIndex++)
        {
            var column = columnList[columnIndex];
            var plugin = pluginLookup[column.ModKey];
            RecordComparisonColumns.Add(new RecordComparisonColumnViewModel(column.ModKey, plugin.LoadOrderIndex, column.ModKey == activeModKey, column.Values, states, columnIndex == columnList.Count - 1));
        }
    }

    private static IReadOnlyList<RecordComparisonValueState> GetRecordComparisonValueStates(
        IReadOnlyList<RecordComparisonFieldViewModel> fields,
        IReadOnlyList<(ModKey ModKey, IReadOnlyList<string> Values)> columns)
    {
        var states = new List<RecordComparisonValueState>();
        for (var fieldIndex = 0; fieldIndex < fields.Count; fieldIndex++)
        {
            var state = RecordComparisonValueState.Neutral;
            if (fields[fieldIndex].IsComparable && columns.Count > 1)
            {
                state = columns
                    .Select(column => column.Values[fieldIndex])
                    .Distinct(StringComparer.Ordinal)
                    .Count() == 1
                    ? RecordComparisonValueState.Identical
                    : RecordComparisonValueState.Conflict;
            }

            fields[fieldIndex].State = state;
            states.Add(state);
        }

        return states;
    }

    private void ClearRecordComparison()
    {
        RecordComparisonFields.Clear();
        RecordComparisonColumns.Clear();
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