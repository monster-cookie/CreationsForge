using System.Collections.ObjectModel;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Starfield;
using Noggog;
using SFRecordCompareEngine.Commands;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.DTOs.Records.Interfaces;
using SFRecordCompareEngine.Core.Helpers;
using SFRecordCompareEngine.Core.Services.Interfaces;
using SFRecordCompareEngine.Services.Interfaces;

namespace SFRecordCompareEngine.ViewModels;

public class MainPageViewModel : ViewModelBase
{
    private readonly IActivePluginSelectionService ActivePluginSelectionService;
    private readonly IActorValueInformationService ActorValueInformationService;
    private readonly IApplicationNavigationService ApplicationNavigationService;
    private readonly IFormListService FormListService;
    private readonly IGameSettingService GameSettingService;
    private readonly IGlobalService GlobalService;
    private readonly IKeywordService KeywordService;
    private readonly IMagicEffectService MagicEffectService;
    private readonly IMiscItemService MiscItemService;
    private readonly INPCService NPCService;
    private readonly IPerkService PerkService;
    private readonly IPluginService PluginService;
    private IList<RecordTreeItemViewModel> AllRecordTreeItems = new List<RecordTreeItemViewModel>();
    private IReadOnlySeparatedMasterPackage? MasterPackage;

    public MainPageViewModel(
        IApplicationNavigationService applicationNavigationService,
        IActivePluginSelectionService activePluginSelectionService,
        IActorValueInformationService actorValueInformationService,
        IFormListService formListService,
        IGameSettingService gameSettingService,
        IGlobalService globalService,
        IKeywordService keywordService,
        IMagicEffectService magicEffectService,
        IMiscItemService miscItemService,
        INPCService npcService,
        IPerkService perkService,
        IPluginService pluginService)
    {
        ApplicationNavigationService = applicationNavigationService;
        ActivePluginSelectionService = activePluginSelectionService;
        ActorValueInformationService = actorValueInformationService;
        FormListService = formListService;
        GameSettingService = gameSettingService;
        GlobalService = globalService;
        KeywordService = keywordService;
        MagicEffectService = magicEffectService;
        MiscItemService = miscItemService;
        NPCService = npcService;
        PerkService = perkService;
        PluginService = pluginService;
        OpenCommand = new AsyncRelayCommand(OpenAsync);
        OptionsCommand = new AsyncRelayCommand(ShowOptionsAsync);
        ReimportAllPluginsCommand = new AsyncRelayCommand(ReimportAllPluginsAsync);
        ExitCommand = new RelayCommand(ApplicationNavigationService.Quit);
        CollapseAllScriptsCommand = new RelayCommand(CollapseAllScripts);
        ExpandChangedScriptsCommand = new RelayCommand(ExpandChangedScripts);
        ToggleChangedOnlyScriptsCommand = new RelayCommand(ToggleChangedOnlyScripts);
        CollapseAllPerkRanksCommand = new RelayCommand(CollapseAllPerkRanks);
        ExpandChangedPerkRanksCommand = new RelayCommand(ExpandChangedPerkRanks);
        ToggleChangedOnlyPerkRanksCommand = new RelayCommand(ToggleChangedOnlyPerkRanks);
        UpdateStatusBar(null);
        ActivePluginSelectionService.ActivePluginChanged += OnActivePluginChanged;
    }

    public AsyncRelayCommand OpenCommand { get; }
    public AsyncRelayCommand OptionsCommand { get; }
    public AsyncRelayCommand ReimportAllPluginsCommand { get; }
    public RelayCommand ExitCommand { get; }
    public RelayCommand CollapseAllScriptsCommand { get; }
    public RelayCommand ExpandChangedScriptsCommand { get; }
    public RelayCommand ToggleChangedOnlyScriptsCommand { get; }
    public RelayCommand CollapseAllPerkRanksCommand { get; }
    public RelayCommand ExpandChangedPerkRanksCommand { get; }
    public RelayCommand ToggleChangedOnlyPerkRanksCommand { get; }

    public ObservableCollection<RecordTreeItemViewModel> RecordTreeItems { get; } = new();
    public ObservableCollection<RecordComparisonFieldViewModel> RecordComparisonFields { get; } = new();
    public ObservableCollection<RecordComparisonColumnViewModel> RecordComparisonColumns { get; } = new();
    public ObservableCollection<RecordComparisonGroupViewModel> RecordComparisonGroups { get; } = new();
    public ObservableCollection<RecordComparisonScriptViewModel> RecordComparisonScripts { get; } = new();
    public ObservableCollection<RecordComparisonPerkRankViewModel> RecordComparisonPerkRanks { get; } = new();

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
    } = string.Empty;

    public string ImportedRecordCountText
    {
        get;
        private set => SetProperty(ref field, value);
    } = string.Empty;

    public string SelectedRecordContextText
    {
        get;
        private set => SetProperty(ref field, value);
    } = string.Empty;

    public Visibility SelectedRecordContextVisibility
    {
        get;
        private set => SetProperty(ref field, value);
    } = Visibility.Collapsed;

    public string ActivePluginText
    {
        get;
        private set => SetProperty(ref field, value);
    } = string.Empty;

    public string ActiveRecordCountText
    {
        get;
        private set => SetProperty(ref field, value);
    } = string.Empty;

    public string ScriptingAdapterSectionHeader
    {
        get;
        private set => SetProperty(ref field, value);
    } = "Scripts";

    public string PerkRankSectionHeader
    {
        get;
        private set => SetProperty(ref field, value);
    } = "Perk Ranks";

    public bool HasRecordComparison
    {
        get;
        private set
        {
            if (!SetProperty(ref field, value)) return;

            RecordComparisonVisibility = value ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    public Visibility RecordComparisonVisibility
    {
        get;
        private set => SetProperty(ref field, value);
    } = Visibility.Collapsed;

    public bool HasRecordComparisonGroups
    {
        get;
        private set
        {
            if (!SetProperty(ref field, value)) return;

            RecordComparisonGroupsVisibility = value ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    public Visibility RecordComparisonGroupsVisibility
    {
        get;
        private set => SetProperty(ref field, value);
    } = Visibility.Collapsed;

    public bool HasRecordComparisonScripts
    {
        get;
        private set
        {
            if (!SetProperty(ref field, value)) return;

            RecordComparisonScriptsVisibility = value ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    public Visibility RecordComparisonScriptsVisibility
    {
        get;
        private set => SetProperty(ref field, value);
    } = Visibility.Collapsed;

    public bool HasRecordComparisonPerkRanks
    {
        get;
        private set
        {
            if (!SetProperty(ref field, value)) return;

            RecordComparisonPerkRanksVisibility = value ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    public Visibility RecordComparisonPerkRanksVisibility
    {
        get;
        private set => SetProperty(ref field, value);
    } = Visibility.Collapsed;

    public bool ShowChangedOnlyScripts
    {
        get;
        private set
        {
            if (!SetProperty(ref field, value)) return;

            foreach (var script in RecordComparisonScripts) script.SetShowChangedOnly(value);

            ChangedOnlyScriptsButtonText = value ? "Show all" : "Changed only";
        }
    }

    public string ChangedOnlyScriptsButtonText
    {
        get;
        private set => SetProperty(ref field, value);
    } = "Changed only";

    public bool ShowChangedOnlyPerkRanks
    {
        get;
        private set
        {
            if (!SetProperty(ref field, value)) return;

            foreach (var rank in RecordComparisonPerkRanks) rank.SetShowChangedOnly(value);

            ChangedOnlyPerkRanksButtonText = value ? "Show all" : "Changed only";
        }
    }

    public string ChangedOnlyPerkRanksButtonText
    {
        get;
        private set => SetProperty(ref field, value);
    } = "Changed only";

    private async Task OpenAsync()
    {
        await ApplicationNavigationService.ShowOpenDialogAsync();
    }

    private async Task ShowOptionsAsync()
    {
        await ApplicationNavigationService.ShowSettingsDialogAsync();
    }

    private async Task ReimportAllPluginsAsync()
    {
        ActivePluginSelectionService.ClearActivePlugin();
        await ApplicationNavigationService.ShowStartupImportAsync(true);
    }

    private async void OnActivePluginChanged(object? sender, EventArgs e)
    {
        UpdateStatusBar(null);
        ClearRecordComparison();
        await RefreshRecordTreeAsync();
    }

    public void SelectRecord(RecordTreeItemViewModel? item)
    {
        ClearRecordComparison();
        if (item?.FormKey == null || item.RecordType == null)
        {
            UpdateStatusBar(null);
            return;
        }

        UpdateStatusBar(item);

        var formKey = item.FormKey.Value;

        if (item.RecordType == RecordTypeCatalog.FormList.RecordType)
        {
            LoadFormListComparison(formKey);
            return;
        }

        if (item.RecordType == RecordTypeCatalog.GameSetting.RecordType)
        {
            LoadGameSettingComparison(formKey);
            return;
        }

        if (item.RecordType == RecordTypeCatalog.Global.RecordType) LoadGlobalComparison(formKey);

        if (item.RecordType == RecordTypeCatalog.MiscItem.RecordType) LoadMiscItemComparison(formKey);

        if (item.RecordType == RecordTypeCatalog.Keyword.RecordType) LoadKeywordComparison(formKey);

        if (item.RecordType == RecordTypeCatalog.NPC.RecordType) LoadNPCComparison(formKey);

        if (item.RecordType == RecordTypeCatalog.ActorValueInformation.RecordType) LoadActorValueInformationComparison(formKey);

        if (item.RecordType == RecordTypeCatalog.MagicEffect.RecordType) LoadMagicEffectComparison(formKey);

        if (item.RecordType == RecordTypeCatalog.Perk.RecordType) LoadPerkComparison(formKey);
    }

    private string GetStatusText()
    {
        var totalPluginRecords = PluginService.GetImportedPluginRecordCount();
        var activePlugin = ActivePluginSelectionService.ActivePlugin;
        return activePlugin == null
            ? $"Total plugin records: {totalPluginRecords:N0}. No active plugin selected."
            : $"Total plugin records: {totalPluginRecords:N0}. Active plugin: {activePlugin.ModKey.FileName} ({GetPluginType(activePlugin)}, {activePlugin.RecordCount:N0} records).";
    }

    private static string GetPluginType(PluginDTO plugin)
    {
        var pluginTypes = new List<string>();
        if (plugin.HeaderFlags.HasFlag(StarfieldModHeader.HeaderFlag.Overlay)) pluginTypes.Add("overlay");

        if (plugin.ModKey.FileName.String.EndsWith(".esp", StringComparison.OrdinalIgnoreCase))
        {
            pluginTypes.Add("ESP");
            return string.Join(", ", pluginTypes);
        }

        if (plugin.ModKey.FileName.String.EndsWith(".esl", StringComparison.OrdinalIgnoreCase))
        {
            pluginTypes.Add("ESL");
            return string.Join(", ", pluginTypes);
        }

        if (plugin.HeaderFlags.HasFlag(StarfieldModHeader.HeaderFlag.Light))
        {
            pluginTypes.Add("small master");
            return string.Join(", ", pluginTypes);
        }

        if (plugin.HeaderFlags.HasFlag(StarfieldModHeader.HeaderFlag.Medium))
        {
            pluginTypes.Add("medium master");
            return string.Join(", ", pluginTypes);
        }

        pluginTypes.Add(plugin.HeaderFlags.HasFlag(StarfieldModHeader.HeaderFlag.Master)
            ? "full master"
            : "plugin");
        return string.Join(", ", pluginTypes);
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
        foreach (var masterReference in contextMod.MasterReferences) masterFlagLookup.Add(LoadMod(masterReference.Master));

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
            FormListService.GetRecordTreeEntriesByModKey(activePlugin.ModKey)
                .Select(record => CreateRecordTreeItem(masterPackage, record.FormKey, record.EditorID, RecordTypeCatalog.FormList.RecordType)));
        AddRecordType(
            recordTreeItems,
            RecordTypeCatalog.GameSetting.RecordType,
            GameSettingService.GetRecordTreeEntriesByModKey(activePlugin.ModKey)
                .Select(record => CreateRecordTreeItem(masterPackage, record.FormKey, record.EditorID, RecordTypeCatalog.GameSetting.RecordType)));
        AddRecordType(recordTreeItems, RecordTypeCatalog.Global.RecordType, GlobalService.GetRecordTreeEntriesByModKey(activePlugin.ModKey).Select(record => CreateRecordTreeItem(masterPackage, record.FormKey, record.EditorID, RecordTypeCatalog.Global.RecordType)));
        AddRecordType(recordTreeItems, RecordTypeCatalog.MiscItem.RecordType, MiscItemService.GetRecordTreeEntriesByModKey(activePlugin.ModKey).Select(record => CreateRecordTreeItem(masterPackage, record.FormKey, record.EditorID, RecordTypeCatalog.MiscItem.RecordType)));
        AddRecordType(recordTreeItems, RecordTypeCatalog.Keyword.RecordType, KeywordService.GetRecordTreeEntriesByModKey(activePlugin.ModKey).Select(record => CreateRecordTreeItem(masterPackage, record.FormKey, record.EditorID, RecordTypeCatalog.Keyword.RecordType)));
        AddRecordType(recordTreeItems, RecordTypeCatalog.NPC.RecordType, NPCService.GetRecordTreeEntriesByModKey(activePlugin.ModKey).Select(record => CreateRecordTreeItem(masterPackage, record.FormKey, record.EditorID, RecordTypeCatalog.NPC.RecordType)));
        AddRecordType(recordTreeItems, RecordTypeCatalog.ActorValueInformation.RecordType, ActorValueInformationService.GetRecordTreeEntriesByModKey(activePlugin.ModKey).Select(record => CreateRecordTreeItem(masterPackage, record.FormKey, record.EditorID, RecordTypeCatalog.ActorValueInformation.RecordType)));
        AddRecordType(recordTreeItems, RecordTypeCatalog.MagicEffect.RecordType, MagicEffectService.GetRecordTreeEntriesByModKey(activePlugin.ModKey).Select(record => CreateRecordTreeItem(masterPackage, record.FormKey, record.EditorID, RecordTypeCatalog.MagicEffect.RecordType)));
        AddRecordType(recordTreeItems, RecordTypeCatalog.Perk.RecordType, PerkService.GetRecordTreeEntriesByModKey(activePlugin.ModKey).Select(record => CreateRecordTreeItem(masterPackage, record.FormKey, record.EditorID, RecordTypeCatalog.Perk.RecordType)));
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
        foreach (var record in records) recordTypeItem.Children.Add(record);

        if (recordTypeItem.Children.Count > 0) recordTreeItems.Add(recordTypeItem);
    }

    private void ApplyFilters()
    {
        RecordTreeItems.Clear();
        foreach (var item in AllRecordTreeItems)
        {
            var filteredItem = FilterItem(item);
            if (filteredItem != null) RecordTreeItems.Add(filteredItem);
        }
    }

    private RecordTreeItemViewModel? FilterItem(RecordTreeItemViewModel item)
    {
        var filteredItem = new RecordTreeItemViewModel(item.FormIDText, item.EditorID, item.FormKey, item.RecordType);
        foreach (var child in item.Children)
        {
            var filteredChild = FilterItem(child);
            if (filteredChild != null) filteredItem.Children.Add(filteredChild);
        }

        if (item.FormKey == null)
            return filteredItem.Children.Count > 0 || (string.IsNullOrWhiteSpace(FormIDFilter) && string.IsNullOrWhiteSpace(EditorIDFilter))
                ? filteredItem
                : null;

        return MatchesFormIDFilter(item)
               && item.EditorID.Contains(EditorIDFilter.Trim(), StringComparison.OrdinalIgnoreCase)
            ? filteredItem
            : null;
    }

    private bool MatchesFormIDFilter(RecordTreeItemViewModel item)
    {
        var filter = FormIDFilter.Trim();
        if (string.IsNullOrWhiteSpace(filter)) return true;

        if (MasterPackage != null && filter.Length == 8 && FormID.TryFactory(filter, out var formID, false))
            try
            {
                return item.FormKey == MasterPackage.GetFormKey(formID, false);
            }
            catch (ArgumentException)
            {
                return false;
            }

        return item.FormIDText.Contains(filter, StringComparison.OrdinalIgnoreCase);
    }

    private void LoadFormListComparison(FormKey formKey)
    {
        var records = FormListService.GetByFormKey(formKey);
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
        for (var itemIndex = 0; itemIndex < maxItemCount; itemIndex++) fields.Add(new RecordComparisonFieldViewModel($"Items[{itemIndex}]"));

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
                while (values.Count < fields.Count) values.Add(string.Empty);

                return (record.ModKey, Values: (IReadOnlyList<string>)values);
            }));
    }

    private void LoadGameSettingComparison(FormKey formKey)
    {
        var records = GameSettingService.GetByFormKey(formKey);
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

    private void LoadGlobalComparison(FormKey formKey)
    {
        var records = GlobalService.GetByFormKey(formKey);
        var fields = CreateHeaderFields("Data");
        SetRecordComparisonWithScripting(fields, records, record => new List<string> { RecordTypeCatalog.Global.RecordID, record.EditorID, record.FormKey.ToString(), FormatStarfieldMajorRecordFlags(record.StarfieldMajorRecordFlags), record.Data?.ToString() ?? string.Empty });
    }

    private void LoadMiscItemComparison(FormKey formKey)
    {
        var records = MiscItemService.GetByFormKey(formKey);
        var fields = CreateHeaderFields("Name", "ShortName", "Value", "Weight", "DirtinessScale", "FeaturedItemMessageFormKey", "FLAG");
        SetRecordComparisonWithScripting(fields, records,
            record =>
            {
                return new List<string>
                {
                    RecordTypeCatalog.MiscItem.RecordID, record.EditorID, record.FormKey.ToString(), FormatStarfieldMajorRecordFlags(record.StarfieldMajorRecordFlags), record.Name ?? string.Empty, record.ShortName ?? string.Empty, record.Value?.ToString() ?? string.Empty, record.Weight?.ToString() ?? string.Empty, record.DirtinessScale?.ToString() ?? string.Empty,
                    record.FeaturedItemMessageFormKey?.ToString() ?? string.Empty, record.Flag ?? string.Empty
                };
            });
        SetMiscItemGroupComparison(records);
    }

    private void SetMiscItemGroupComparison(IList<MiscItemDTO> records)
    {
        var orderedModKeys = RecordComparisonColumns.Select(column => column.ModKey).ToList();
        var recordLookup = records.ToDictionary(record => record.ModKey);

        if (records.Any(record => record.ObjectBounds != null))
            AddRecordComparisonGroup("Object Bounds", orderedModKeys, new[]
            {
                ("First X", (Func<MiscItemDTO, string>)(record => record.ObjectBounds?.FirstX.ToString() ?? string.Empty)),
                ("First Y", record => record.ObjectBounds?.FirstY.ToString() ?? string.Empty),
                ("First Z", record => record.ObjectBounds?.FirstZ.ToString() ?? string.Empty),
                ("Second X", record => record.ObjectBounds?.SecondX.ToString() ?? string.Empty),
                ("Second Y", record => record.ObjectBounds?.SecondY.ToString() ?? string.Empty),
                ("Second Z", record => record.ObjectBounds?.SecondZ.ToString() ?? string.Empty)
            }, recordLookup);

        if (records.Any(record => record.ObjectPaletteDefaults != null))
            AddRecordComparisonGroup("Object Palette Defaults", orderedModKeys, new[]
            {
                ("Flags", (Func<MiscItemDTO, string>)(record => record.ObjectPaletteDefaults?.Flags ?? string.Empty)),
                ("Sink Meters", record => record.ObjectPaletteDefaults?.SinkMeters?.ToString() ?? string.Empty),
                ("Sink Variance", record => record.ObjectPaletteDefaults?.SinkVariance?.ToString() ?? string.Empty),
                ("XY Offset Variance", record => record.ObjectPaletteDefaults?.XYOffsetVariance?.ToString() ?? string.Empty),
                ("Footprint Size", record => record.ObjectPaletteDefaults?.FootprintSize ?? string.Empty),
                ("Scale Percent", record => record.ObjectPaletteDefaults?.ScalePercent?.ToString() ?? string.Empty),
                ("Scale Variance", record => record.ObjectPaletteDefaults?.ScaleVariance?.ToString() ?? string.Empty),
                ("Angle X Degrees", record => record.ObjectPaletteDefaults?.AngleXDegrees?.ToString() ?? string.Empty),
                ("Angle X Variance", record => record.ObjectPaletteDefaults?.AngleXVariance?.ToString() ?? string.Empty),
                ("Angle Y Degrees", record => record.ObjectPaletteDefaults?.AngleYDegrees?.ToString() ?? string.Empty),
                ("Angle Y Variance", record => record.ObjectPaletteDefaults?.AngleYVariance?.ToString() ?? string.Empty),
                ("Angle Z Degrees", record => record.ObjectPaletteDefaults?.AngleZDegrees?.ToString() ?? string.Empty),
                ("Angle Z Variance", record => record.ObjectPaletteDefaults?.AngleZVariance?.ToString() ?? string.Empty),
                ("Slope Percent", record => record.ObjectPaletteDefaults?.SlopePercent?.ToString() ?? string.Empty),
                ("Slope Percent Variance", record => record.ObjectPaletteDefaults?.SlopePercentVariance?.ToString() ?? string.Empty),
                ("Density", record => record.ObjectPaletteDefaults?.Density?.ToString() ?? string.Empty),
                ("Frequency Percent", record => record.ObjectPaletteDefaults?.FrequencyPercent?.ToString() ?? string.Empty),
                ("Slope Limit", record => record.ObjectPaletteDefaults?.SlopeLimit?.ToString() ?? string.Empty),
                ("Distance Below Water", record => record.ObjectPaletteDefaults?.DistanceBelowWater?.ToString() ?? string.Empty),
                ("Distance Above Water", record => record.ObjectPaletteDefaults?.DistanceAboveWater?.ToString() ?? string.Empty)
            }, recordLookup);

        if (records.Any(record => record.Transforms != null))
            AddRecordComparisonGroup("Transforms", orderedModKeys, new[]
            {
                ("Inventory Icon", (Func<MiscItemDTO, string>)(record => record.Transforms?.InventoryIconFormKey?.ToString() ?? string.Empty)),
                ("Outpost", record => record.Transforms?.OutpostFormKey?.ToString() ?? string.Empty),
                ("Ship", record => record.Transforms?.ShipFormKey?.ToString() ?? string.Empty),
                ("Preview", record => record.Transforms?.PreviewFormKey?.ToString() ?? string.Empty),
                ("Inventory", record => record.Transforms?.InventoryFormKey?.ToString() ?? string.Empty),
                ("Workbench", record => record.Transforms?.WorkbenchFormKey?.ToString() ?? string.Empty),
                ("Main Game UI", record => record.Transforms?.MainGameUIFormKey?.ToString() ?? string.Empty)
            }, recordLookup);

        if (records.Any(record => record.Model != null))
        {
            var modelRows = new List<(string Label, Func<MiscItemDTO, string> ValueFactory)>
            {
                ("File", record => record.Model?.File ?? string.Empty),
                ("Texture File Hashes", record => record.Model?.TextureFileHashes ?? string.Empty),
                ("Light Layer", record => record.Model?.LightLayer?.ToString() ?? string.Empty),
                ("Flags", record => record.Model?.Flags ?? string.Empty),
                ("Color Remapping Index", record => record.Model?.ColorRemappingIndex?.ToString() ?? string.Empty),
                ("Vestigial Flags", record => record.Model?.FlagsVestigial ?? string.Empty)
            };
            var maxMaterialSwapCount = records.Select(record => record.Model?.MaterialSwaps.Count ?? 0).DefaultIfEmpty(0).Max();
            for (var index = 0; index < maxMaterialSwapCount; index++)
            {
                var materialSwapIndex = index;
                modelRows.Add(($"Material Swap {index}", record => record.Model != null && record.Model.MaterialSwaps.Count > materialSwapIndex ? record.Model.MaterialSwaps[materialSwapIndex].ToString() : string.Empty));
            }

            AddRecordComparisonGroup("Model", orderedModKeys, modelRows, recordLookup);
        }

        if (records.Any(record => record.CraftingSound != null || record.PickupSound != null || record.DropdownSound != null))
            AddRecordComparisonGroup("Sounds", orderedModKeys, new[]
            {
                ("Crafting Start", (Func<MiscItemDTO, string>)(record => record.CraftingSound?.Start ?? string.Empty)),
                ("Crafting Stop", record => record.CraftingSound?.Stop ?? string.Empty),
                ("Crafting Condition", record => record.CraftingSound?.ConditionFormKey?.ToString() ?? string.Empty),
                ("Crafting Event Mapping", record => record.CraftingSound?.EventMappingFormKey?.ToString() ?? string.Empty),
                ("Pickup Start", record => record.PickupSound?.Start ?? string.Empty),
                ("Pickup Stop", record => record.PickupSound?.Stop ?? string.Empty),
                ("Pickup Condition", record => record.PickupSound?.ConditionFormKey?.ToString() ?? string.Empty),
                ("Pickup Event Mapping", record => record.PickupSound?.EventMappingFormKey?.ToString() ?? string.Empty),
                ("Dropdown Start", record => record.DropdownSound?.Start ?? string.Empty),
                ("Dropdown Stop", record => record.DropdownSound?.Stop ?? string.Empty),
                ("Dropdown Condition", record => record.DropdownSound?.ConditionFormKey?.ToString() ?? string.Empty),
                ("Dropdown Event Mapping", record => record.DropdownSound?.EventMappingFormKey?.ToString() ?? string.Empty)
            }, recordLookup);

        var keywordRows = new List<(string Label, Func<MiscItemDTO, string> ValueFactory)>();
        var maxKeywordCount = records.Select(record => record.Keywords.Count).DefaultIfEmpty(0).Max();
        for (var index = 0; index < maxKeywordCount; index++)
        {
            var keywordIndex = index;
            keywordRows.Add(($"Keyword {index}", record => record.Keywords.Count > keywordIndex ? record.Keywords[keywordIndex].ToString() : string.Empty));
        }

        AddRecordComparisonGroup("Keywords", orderedModKeys, keywordRows, recordLookup);

        if (records.Any(record => record.Destructible != null))
        {
            var destructibleRows = new List<(string Label, Func<MiscItemDTO, string> ValueFactory)>
            {
                ("Health", record => record.Destructible?.Health?.ToString() ?? string.Empty),
                ("Count", record => record.Destructible?.Count?.ToString() ?? string.Empty),
                ("Flags", record => record.Destructible?.Flags ?? string.Empty)
            };
            AddMiscItemDestructibleResistanceRows(records, destructibleRows);
            AddMiscItemDestructionStageRows(records, destructibleRows);
            AddRecordComparisonGroup("Destructible", orderedModKeys, destructibleRows, recordLookup);
        }

        HasRecordComparisonGroups = RecordComparisonGroups.Count > 0;
    }

    private static void AddMiscItemDestructibleResistanceRows(
        IEnumerable<MiscItemDTO> records,
        ICollection<(string Label, Func<MiscItemDTO, string> ValueFactory)> rows)
    {
        var maxResistanceCount = records.Select(record => record.Destructible?.Resistances.Count ?? 0).DefaultIfEmpty(0).Max();
        for (var index = 0; index < maxResistanceCount; index++)
        {
            var resistanceIndex = index;
            rows.Add(($"Resistance {index} - Resistance Index", record => GetMiscItemDestructibleResistance(record, resistanceIndex)?.ResistanceIndex.ToString() ?? string.Empty));
            rows.Add(($"Resistance {index} - Damage Type", record => GetMiscItemDestructibleResistance(record, resistanceIndex)?.DamageTypeFormKey.ToString() ?? string.Empty));
            rows.Add(($"Resistance {index} - Value", record => GetMiscItemDestructibleResistance(record, resistanceIndex)?.Value.ToString() ?? string.Empty));
        }
    }

    private static void AddMiscItemDestructionStageRows(
        IEnumerable<MiscItemDTO> records,
        ICollection<(string Label, Func<MiscItemDTO, string> ValueFactory)> rows)
    {
        var recordList = records.ToList();
        var maxStageCount = recordList.Select(record => record.Destructible?.Stages.Count ?? 0).DefaultIfEmpty(0).Max();
        for (var index = 0; index < maxStageCount; index++)
        {
            var stageIndex = index;
            rows.Add(($"Stage {index} - Stage Index", record => GetMiscItemDestructionStage(record, stageIndex)?.StageIndex.ToString() ?? string.Empty));
            rows.Add(($"Stage {index} - Health Percent", record => GetMiscItemDestructionStage(record, stageIndex)?.HealthPercent?.ToString() ?? string.Empty));
            rows.Add(($"Stage {index} - Source Index", record => GetMiscItemDestructionStage(record, stageIndex)?.Index?.ToString() ?? string.Empty));
            rows.Add(($"Stage {index} - Model Damage Stage", record => GetMiscItemDestructionStage(record, stageIndex)?.ModelDamageStage?.ToString() ?? string.Empty));
            rows.Add(($"Stage {index} - Flags", record => GetMiscItemDestructionStage(record, stageIndex)?.Flags ?? string.Empty));
            rows.Add(($"Stage {index} - Self Damage Per Second", record => GetMiscItemDestructionStage(record, stageIndex)?.SelfDamagePerSecond?.ToString() ?? string.Empty));
            rows.Add(($"Stage {index} - Explosion", record => GetMiscItemDestructionStage(record, stageIndex)?.ExplosionFormKey?.ToString() ?? string.Empty));
            rows.Add(($"Stage {index} - Debris", record => GetMiscItemDestructionStage(record, stageIndex)?.DebrisFormKey?.ToString() ?? string.Empty));
            rows.Add(($"Stage {index} - Debris Count", record => GetMiscItemDestructionStage(record, stageIndex)?.DebrisCount?.ToString() ?? string.Empty));
            rows.Add(($"Stage {index} - Sequence Name", record => GetMiscItemDestructionStage(record, stageIndex)?.SequenceName ?? string.Empty));
            rows.Add(($"Stage {index} - Model File", record => GetMiscItemDestructionStage(record, stageIndex)?.ModelFile ?? string.Empty));
            rows.Add(($"Stage {index} - Model Light Layer", record => GetMiscItemDestructionStage(record, stageIndex)?.ModelLightLayer?.ToString() ?? string.Empty));
            rows.Add(($"Stage {index} - Model Flags", record => GetMiscItemDestructionStage(record, stageIndex)?.ModelFlags ?? string.Empty));

            var maxMaterialSwapCount = recordList
                .Select(record => GetMiscItemDestructionStage(record, stageIndex)?.ModelMaterialSwaps.Count ?? 0)
                .DefaultIfEmpty(0)
                .Max();
            for (var materialSwapIndex = 0; materialSwapIndex < maxMaterialSwapCount; materialSwapIndex++)
            {
                var capturedMaterialSwapIndex = materialSwapIndex;
                rows.Add(($"Stage {index} - Model Material Swap {materialSwapIndex}", record => GetMiscItemDestructionStageMaterialSwap(record, stageIndex, capturedMaterialSwapIndex)));
            }
        }
    }

    private static MiscItemDestructibleResistanceDTO? GetMiscItemDestructibleResistance(MiscItemDTO record, int resistanceIndex)
    {
        return record.Destructible != null && record.Destructible.Resistances.Count > resistanceIndex
            ? record.Destructible.Resistances[resistanceIndex]
            : null;
    }

    private static MiscItemDestructionStageDTO? GetMiscItemDestructionStage(MiscItemDTO record, int stageIndex)
    {
        return record.Destructible != null && record.Destructible.Stages.Count > stageIndex
            ? record.Destructible.Stages[stageIndex]
            : null;
    }

    private static string GetMiscItemDestructionStageMaterialSwap(MiscItemDTO record, int stageIndex, int materialSwapIndex)
    {
        var stage = GetMiscItemDestructionStage(record, stageIndex);
        return stage != null && stage.ModelMaterialSwaps.Count > materialSwapIndex
            ? stage.ModelMaterialSwaps[materialSwapIndex].ToString()
            : string.Empty;
    }

    private void AddRecordComparisonGroup(
        string headerText,
        IReadOnlyList<ModKey> orderedModKeys,
        IEnumerable<(string Label, Func<MiscItemDTO, string> ValueFactory)> rowDefinitions,
        IReadOnlyDictionary<ModKey, MiscItemDTO> recordLookup)
    {
        if (orderedModKeys.Count == 0) return;

        var rows = rowDefinitions
            .Select(definition =>
            {
                var values = orderedModKeys
                    .Select(modKey => recordLookup.TryGetValue(modKey, out var record) ? definition.ValueFactory(record) : string.Empty)
                    .ToList();
                var state = GetComparisonValueState(values, true);
                return new RecordComparisonGroupRowViewModel(definition.Label, CreateComparisonValues(values, state), state);
            })
            .ToList();
        if (rows.Count == 0) return;

        RecordComparisonGroups.Add(new RecordComparisonGroupViewModel(headerText, RecordComparisonColumns.ToList(), rows));
    }

    private void LoadKeywordComparison(FormKey formKey)
    {
        var records = KeywordService.GetByFormKey(formKey);
        var fields = CreateHeaderFields("Name", "Color", "Type", "Notes", "FlashLinkageName", "AttractionRuleFormKey");
        SetRecordComparisonWithScripting(fields, records,
            record => new List<string>
                { RecordTypeCatalog.Keyword.RecordID, record.EditorID, record.FormKey.ToString(), FormatStarfieldMajorRecordFlags(record.StarfieldMajorRecordFlags), record.Name ?? string.Empty, record.Color, record.Type, record.Notes ?? string.Empty, record.FlashLinkageName ?? string.Empty, record.AttractionRuleFormKey?.ToString() ?? string.Empty });
    }

    private void LoadNPCComparison(FormKey formKey)
    {
        var records = NPCService.GetByFormKey(formKey);
        var fields = CreateHeaderFields("Name", "ShortName", "LongName", "DispositionBase", "Aggression", "Confidence", "EnergyLevel", "Responsibility", "Assistance", "GearedUpWeapons", "HeightMin", "HeightMax", "SkinToneIndex", "Pronoun", "VoiceFormKey", "RaceFormKey", "CombatOverridePackageListFormKey", "CombatStyleFormKey", "DefaultPackageListFormKey", "CrimeFactionFormKey");
        SetRecordComparisonWithScripting(fields, records,
            record => new List<string>
            {
                RecordTypeCatalog.NPC.RecordID, record.EditorID, record.FormKey.ToString(), FormatStarfieldMajorRecordFlags(record.StarfieldMajorRecordFlags), record.Name ?? string.Empty, record.ShortName ?? string.Empty, record.LongName ?? string.Empty, record.DispositionBase.ToString(), record.Aggression, record.Confidence, record.EnergyLevel.ToString(), record.Responsibility,
                record.Assistance, record.GearedUpWeapons.ToString(), record.HeightMin.ToString(), record.HeightMax.ToString(), record.SkinToneIndex?.ToString() ?? string.Empty, record.Pronoun ?? string.Empty, record.VoiceFormKey?.ToString() ?? string.Empty, record.RaceFormKey?.ToString() ?? string.Empty, record.CombatOverridePackageListFormKey?.ToString() ?? string.Empty,
                record.CombatStyleFormKey?.ToString() ?? string.Empty, record.DefaultPackageListFormKey?.ToString() ?? string.Empty, record.CrimeFactionFormKey?.ToString() ?? string.Empty
            });
    }

    private void LoadActorValueInformationComparison(FormKey formKey)
    {
        var records = ActorValueInformationService.GetByFormKey(formKey);
        var fields = CreateHeaderFields("Name", "Abbreviation", "ContextNotes", "DefaultValue", "Flags", "Type", "Min", "Max");
        SetRecordComparisonWithScripting(fields, records,
            record => new List<string>
            {
                RecordTypeCatalog.ActorValueInformation.RecordID, record.EditorID, record.FormKey.ToString(), FormatStarfieldMajorRecordFlags(record.StarfieldMajorRecordFlags), record.Name ?? string.Empty, record.Abbreviation ?? string.Empty, record.ContextNotes ?? string.Empty, record.DefaultValue?.ToString() ?? string.Empty, record.Flags ?? string.Empty, record.Type ?? string.Empty,
                record.Min?.ToString() ?? string.Empty, record.Max?.ToString() ?? string.Empty
            });
    }

    private void LoadMagicEffectComparison(FormKey formKey)
    {
        var records = MagicEffectService.GetByFormKey(formKey);
        var fields = CreateHeaderFields("Name", "Description", "Flags", "CastType", "TargetType", "ActorValue2FormKey", "ResistValueFormKey", "PerkToApplyFormKey", "EquipAbilityFormKey", "ExplosionFormKey", "CastingArtFormKey", "HitEffectArtFormKey", "HitShaderFormKey", "ImageSpaceModifierFormKey", "ImpactDataFormKey", "ProjectileFormKey");
        SetRecordComparisonWithScripting(fields, records,
            record => new List<string>
            {
                RecordTypeCatalog.MagicEffect.RecordID, record.EditorID, record.FormKey.ToString(), FormatStarfieldMajorRecordFlags(record.StarfieldMajorRecordFlags), record.Name ?? string.Empty, record.Description ?? string.Empty, record.Flags, record.CastType ?? string.Empty, record.TargetType ?? string.Empty, record.ActorValue2FormKey?.ToString() ?? string.Empty,
                record.ResistValueFormKey?.ToString() ?? string.Empty, record.PerkToApplyFormKey?.ToString() ?? string.Empty, record.EquipAbilityFormKey?.ToString() ?? string.Empty, record.ExplosionFormKey?.ToString() ?? string.Empty, record.CastingArtFormKey?.ToString() ?? string.Empty, record.HitEffectArtFormKey?.ToString() ?? string.Empty,
                record.HitShaderFormKey?.ToString() ?? string.Empty, record.ImageSpaceModifierFormKey?.ToString() ?? string.Empty, record.ImpactDataFormKey?.ToString() ?? string.Empty, record.ProjectileFormKey?.ToString() ?? string.Empty
            });
    }

    private void LoadPerkComparison(FormKey formKey)
    {
        var records = PerkService.GetByFormKey(formKey);
        var maxBackgroundSkillCount = records.Count == 0 ? 0 : records.Max(record => record.BackgroundSkills.Count);
        var fields = CreateHeaderFields("Name", "Description", "Flags", "SkillGroup", "CrewAssignment", "PerkIcon", "Category", "RestrictionFormKey", "TrainingFormKey", "MajorFlags");
        for (var skillIndex = 0; skillIndex < maxBackgroundSkillCount; skillIndex++) fields.Add(new RecordComparisonFieldViewModel($"BackgroundSkills[{skillIndex}]"));

        SetRecordComparisonWithScripting(fields, records,
            record =>
            {
                var values = new List<string>
                {
                    RecordTypeCatalog.Perk.RecordID, record.EditorID, record.FormKey.ToString(), FormatStarfieldMajorRecordFlags(record.StarfieldMajorRecordFlags), record.Name ?? string.Empty, record.Description ?? string.Empty, record.Flags, record.SkillGroup ?? string.Empty, record.CrewAssignment ?? string.Empty, record.PerkIcon ?? string.Empty,
                    record.Category ?? string.Empty, record.RestrictionFormKey?.ToString() ?? string.Empty, record.TrainingFormKey?.ToString() ?? string.Empty, record.MajorFlags ?? string.Empty
                };
                values.AddRange(record.BackgroundSkills.Select(skill => skill.SkillFormKey.ToString()));
                while (values.Count < fields.Count) values.Add(string.Empty);

                return values;
            });
        SetPerkRankComparison(records);
    }

    private static List<RecordComparisonFieldViewModel> CreateHeaderFields(params string[] recordFields)
    {
        var fields = new List<RecordComparisonFieldViewModel>
        {
            new("Record Header", false),
            new("EditorID"),
            new("FormKey", false),
            new("StarfieldMajorRecordFlags")
        };
        fields.AddRange(recordFields.Select(field => new RecordComparisonFieldViewModel(field)));
        return fields;
    }

    private void SetRecordComparisonWithScripting<TRecord>(List<RecordComparisonFieldViewModel> fields, IList<TRecord> records, Func<TRecord, List<string>> baseValueFactory)
        where TRecord : IHasScriptingAdaptersRecordDTO
    {
        var valuesByModKey = records.ToDictionary(record => record.ModKey, baseValueFactory);
        SetRecordComparison(fields, records.Select(record => (record.ModKey, Values: (IReadOnlyList<string>)valuesByModKey[record.ModKey])));
        SetScriptingAdapterComparison(records);
    }

    private void SetScriptingAdapterComparison<TRecord>(IList<TRecord> records)
        where TRecord : IHasScriptingAdaptersRecordDTO
    {
        var orderedModKeys = RecordComparisonColumns.Select(column => column.ModKey).ToList();
        if (orderedModKeys.Count == 0 || records.Count == 0) return;

        var recordLookup = records.ToDictionary(record => record.ModKey);
        var maxScriptCount = records.Max(record => record.ScriptingAdapters.Count);
        for (var scriptIndex = 0; scriptIndex < maxScriptCount; scriptIndex++)
        {
            var scriptNames = orderedModKeys
                .Select(modKey => GetScriptingAdapter(recordLookup, modKey, scriptIndex)?.Name ?? string.Empty)
                .ToList();
            var scriptName = scriptNames.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value)) ?? "Missing script";
            var scriptState = GetComparisonValueState(scriptNames, true);
            var scriptProperties = CreateScriptingAdapterPropertyRows(recordLookup, orderedModKeys, scriptIndex);
            var script = new RecordComparisonScriptViewModel(
                scriptIndex,
                scriptName,
                RecordComparisonColumns.ToList(),
                CreateComparisonValues(scriptNames, scriptState),
                scriptProperties,
                scriptState);
            script.SetShowChangedOnly(ShowChangedOnlyScripts);
            RecordComparisonScripts.Add(script);
        }

        HasRecordComparisonScripts = RecordComparisonScripts.Count > 0;
        ScriptingAdapterSectionHeader = HasRecordComparisonScripts
            ? $"Scripts ({RecordComparisonScripts.Count})"
            : "Scripts";
    }

    private static IReadOnlyList<RecordComparisonScriptPropertyViewModel> CreateScriptingAdapterPropertyRows<TRecord>(
        IReadOnlyDictionary<ModKey, TRecord> recordLookup,
        IReadOnlyList<ModKey> orderedModKeys,
        int scriptIndex)
        where TRecord : IHasScriptingAdaptersRecordDTO
    {
        var properties = new List<RecordComparisonScriptPropertyViewModel>();
        var maxPropertyCount = recordLookup.Values
            .Select(record => record.ScriptingAdapters.ElementAtOrDefault(scriptIndex)?.Properties.Count ?? 0)
            .DefaultIfEmpty(0)
            .Max();

        for (var propertyIndex = 0; propertyIndex < maxPropertyCount; propertyIndex++)
        {
            var propertyValues = orderedModKeys
                .Select(modKey => GetScriptingAdapterProperty(recordLookup, modKey, scriptIndex, propertyIndex))
                .ToList();
            var propertyName = propertyValues.FirstOrDefault(property => !string.IsNullOrWhiteSpace(property?.Name))?.Name ?? "Missing property";
            var propertyType = propertyValues.FirstOrDefault(property => !string.IsNullOrWhiteSpace(property?.MutagenObjectType))?.MutagenObjectType ?? string.Empty;
            var displayValues = propertyValues.Select(FormatScriptingAdapterPropertyValue).ToList();
            var comparisonValues = propertyValues
                .Select(property => property == null ? string.Empty : $"{property.Name}|{property.MutagenObjectType}|{FormatScriptingAdapterPropertyValue(property)}|{property.ListItems.Count}")
                .ToList();
            var state = GetComparisonValueState(comparisonValues, true);
            var listCountText = GetListCountText(propertyValues);
            properties.Add(new RecordComparisonScriptPropertyViewModel(
                $"Property {propertyIndex} - {propertyName}",
                propertyType,
                listCountText,
                CreateComparisonValues(displayValues, state),
                state));

            var maxListItemCount = propertyValues
                .Select(property => property?.ListItems.Count ?? 0)
                .DefaultIfEmpty(0)
                .Max();

            for (var listItemIndex = 0; listItemIndex < maxListItemCount; listItemIndex++)
            {
                var listItems = propertyValues
                    .Select(property => property?.ListItems.ElementAtOrDefault(listItemIndex))
                    .ToList();
                var listItemType = listItems.FirstOrDefault(listItem => !string.IsNullOrWhiteSpace(listItem?.MutagenObjectType))?.MutagenObjectType ?? string.Empty;
                var listItemValues = listItems.Select(FormatScriptingAdapterListItemValue).ToList();
                var listItemState = GetComparisonValueState(listItemValues, true);
                properties.Add(new RecordComparisonScriptPropertyViewModel(
                    $"List item {listItemIndex}",
                    listItemType,
                    string.Empty,
                    CreateComparisonValues(listItemValues, listItemState),
                    listItemState));
            }
        }

        return properties;
    }

    private static ScriptingAdapterDTO? GetScriptingAdapter<TRecord>(
        IReadOnlyDictionary<ModKey, TRecord> recordLookup,
        ModKey modKey,
        int scriptIndex)
        where TRecord : IHasScriptingAdaptersRecordDTO
    {
        return recordLookup.TryGetValue(modKey, out var record)
            ? record.ScriptingAdapters.ElementAtOrDefault(scriptIndex)
            : null;
    }

    private static ScriptingAdapterPropertyDTO? GetScriptingAdapterProperty<TRecord>(
        IReadOnlyDictionary<ModKey, TRecord> recordLookup,
        ModKey modKey,
        int scriptIndex,
        int propertyIndex)
        where TRecord : IHasScriptingAdaptersRecordDTO
    {
        return GetScriptingAdapter(recordLookup, modKey, scriptIndex)?.Properties.ElementAtOrDefault(propertyIndex);
    }

    private void SetPerkRankComparison(IList<PerkDTO> records)
    {
        var orderedModKeys = RecordComparisonColumns.Select(column => column.ModKey).ToList();
        if (orderedModKeys.Count == 0 || records.Count == 0) return;

        var recordLookup = records.ToDictionary(record => record.ModKey);
        var maxRankCount = records.Max(record => record.Ranks.Count);
        for (var rankIndex = 0; rankIndex < maxRankCount; rankIndex++)
        {
            var rankValues = orderedModKeys
                .Select(modKey => GetPerkRank(recordLookup, modKey, rankIndex))
                .ToList();
            var rankState = GetComparisonValueState(rankValues.Select(FormatPerkRankIdentity).ToList(), true);
            var rows = CreatePerkRankRows(recordLookup, orderedModKeys, rankIndex);
            var rank = new RecordComparisonPerkRankViewModel(
                rankIndex,
                RecordComparisonColumns.ToList(),
                rows,
                rankState);
            rank.SetShowChangedOnly(ShowChangedOnlyPerkRanks);
            RecordComparisonPerkRanks.Add(rank);
        }

        HasRecordComparisonPerkRanks = RecordComparisonPerkRanks.Count > 0;
        PerkRankSectionHeader = HasRecordComparisonPerkRanks
            ? $"Perk Ranks ({RecordComparisonPerkRanks.Count})"
            : "Perk Ranks";
    }

    private static IReadOnlyList<RecordComparisonPerkRankEffectViewModel> CreatePerkRankRows(
        IReadOnlyDictionary<ModKey, PerkDTO> recordLookup,
        IReadOnlyList<ModKey> orderedModKeys,
        int rankIndex)
    {
        var rows = new List<RecordComparisonPerkRankEffectViewModel>();
        AddPerkRankFieldRow(rows, recordLookup, orderedModKeys, rankIndex, "Description", rank => rank?.Description ?? string.Empty);
        AddPerkRankFieldRow(rows, recordLookup, orderedModKeys, rankIndex, "UnknownStaticFormKey", rank => rank?.UnknownStaticFormKey?.ToString() ?? string.Empty);
        AddPerkRankFieldRow(rows, recordLookup, orderedModKeys, rankIndex, "ConditionCount", rank => rank?.ConditionCount.ToString() ?? string.Empty);
        AddPerkRankFieldRow(rows, recordLookup, orderedModKeys, rankIndex, "ActivityCount", rank => rank?.ActivityCount.ToString() ?? string.Empty);

        var maxEffectCount = recordLookup.Values
            .Select(record => record.Ranks.ElementAtOrDefault(rankIndex)?.Effects.Count ?? 0)
            .DefaultIfEmpty(0)
            .Max();
        for (var effectIndex = 0; effectIndex < maxEffectCount; effectIndex++)
        {
            var effectValues = orderedModKeys
                .Select(modKey => GetPerkRankEffect(recordLookup, modKey, rankIndex, effectIndex))
                .ToList();
            var effectType = effectValues.FirstOrDefault(effect => !string.IsNullOrWhiteSpace(effect?.MutagenObjectType))?.MutagenObjectType ?? string.Empty;
            AddPerkRankEffectRow(rows, effectValues, $"Effect {effectIndex}", effectType, FormatPerkRankEffectValue);
        }

        return rows;
    }

    private static void AddPerkRankFieldRow(
        ICollection<RecordComparisonPerkRankEffectViewModel> rows,
        IReadOnlyDictionary<ModKey, PerkDTO> recordLookup,
        IReadOnlyList<ModKey> orderedModKeys,
        int rankIndex,
        string label,
        Func<PerkRankDTO?, string> valueFactory)
    {
        var values = orderedModKeys
            .Select(modKey => valueFactory(GetPerkRank(recordLookup, modKey, rankIndex)))
            .ToList();
        var state = GetComparisonValueState(values, true);
        rows.Add(new RecordComparisonPerkRankEffectViewModel(label, "Rank", CreateComparisonValues(values, state), state));
    }

    private static void AddPerkRankEffectRow(
        ICollection<RecordComparisonPerkRankEffectViewModel> rows,
        IReadOnlyList<PerkRankEffectDTO?> effects,
        string label,
        string type,
        Func<PerkRankEffectDTO?, string> valueFactory)
    {
        var values = effects.Select(valueFactory).ToList();
        var state = GetComparisonValueState(values, true);
        rows.Add(new RecordComparisonPerkRankEffectViewModel(label, type, CreateComparisonValues(values, state), state));
    }

    private static PerkRankDTO? GetPerkRank(
        IReadOnlyDictionary<ModKey, PerkDTO> recordLookup,
        ModKey modKey,
        int rankIndex)
    {
        return recordLookup.TryGetValue(modKey, out var record)
            ? record.Ranks.ElementAtOrDefault(rankIndex)
            : null;
    }

    private static PerkRankEffectDTO? GetPerkRankEffect(
        IReadOnlyDictionary<ModKey, PerkDTO> recordLookup,
        ModKey modKey,
        int rankIndex,
        int effectIndex)
    {
        return GetPerkRank(recordLookup, modKey, rankIndex)?.Effects.ElementAtOrDefault(effectIndex);
    }

    private static string FormatPerkRankIdentity(PerkRankDTO? rank)
    {
        return rank == null
            ? string.Empty
            : $"{rank.Description}|{rank.UnknownStaticFormKey}|{rank.ConditionCount}|{rank.ActivityCount}|{rank.Effects.Count}";
    }

    private static string FormatPerkRankEffectValue(PerkRankEffectDTO? effect)
    {
        if (effect == null) return string.Empty;

        return string.Join(" | ", new[]
        {
            effect.MutagenObjectType,
            $"Rank={effect.Rank}",
            $"Priority={effect.Priority}",
            $"PerkEntryID={effect.PerkEntryId?.ToString() ?? string.Empty}",
            $"Flags={effect.Flags ?? string.Empty}",
            $"Button={effect.ButtonLabel ?? string.Empty}",
            $"Conditions={effect.ConditionCount}",
            $"EntryPoint={effect.EntryPoint ?? string.Empty}",
            $"Tabs={effect.PerkConditionTabCount?.ToString() ?? string.Empty}",
            $"Modification={effect.Modification ?? string.Empty}",
            $"Value={effect.Value?.ToString() ?? string.Empty}"
        });
    }

    private static string GetListCountText(IEnumerable<ScriptingAdapterPropertyDTO?> properties)
    {
        var counts = properties
            .Select(property => property?.ListItems.Count)
            .Where(count => count.HasValue)
            .Select(count => count!.Value)
            .Distinct()
            .OrderBy(count => count)
            .ToList();

        return counts.Count == 0 ? string.Empty : string.Join("/", counts);
    }

    private static RecordComparisonValueState GetComparisonValueState(IReadOnlyList<string> values, bool isComparable)
    {
        if (!isComparable || values.Count <= 1) return RecordComparisonValueState.Neutral;

        return values.Distinct(StringComparer.Ordinal).Count() == 1
            ? RecordComparisonValueState.Identical
            : RecordComparisonValueState.Conflict;
    }

    private static IReadOnlyList<RecordComparisonValueViewModel> CreateComparisonValues(IReadOnlyList<string> values, RecordComparisonValueState state)
    {
        return values
            .Select((value, index) => new RecordComparisonValueViewModel(value, state == RecordComparisonValueState.Conflict && index == values.Count - 1 ? RecordComparisonValueState.WinningOverride : state))
            .ToList();
    }

    private static string FormatScriptingAdapterPropertyValue(ScriptingAdapterPropertyDTO? property)
    {
        if (property == null) return string.Empty;

        if (property.ObjectFormKey != null) return $"{property.ObjectFormKey} | Alias={property.ObjectAlias?.ToString() ?? string.Empty} | Unused={property.ObjectUnused?.ToString() ?? string.Empty}";

        if (property.DataBool.HasValue) return property.DataBool.Value.ToString();

        if (property.DataInt.HasValue) return property.DataInt.Value.ToString();

        if (property.DataFloat.HasValue) return property.DataFloat.Value.ToString();

        return property.DataString ?? string.Empty;
    }

    private static string FormatScriptingAdapterListItemValue(ScriptingAdapterPropertyListItemDTO? listItem)
    {
        if (listItem == null) return string.Empty;

        if (listItem.ObjectFormKey != null) return $"{listItem.ObjectFormKey} | Alias={listItem.ObjectAlias?.ToString() ?? string.Empty} | Unused={listItem.ObjectUnused?.ToString() ?? string.Empty}";

        if (listItem.DataBool.HasValue) return listItem.DataBool.Value.ToString();

        if (listItem.DataInt.HasValue) return listItem.DataInt.Value.ToString();

        if (listItem.DataFloat.HasValue) return listItem.DataFloat.Value.ToString();

        return listItem.DataString ?? string.Empty;
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
        if (unknownFlags != 0) names.Add($"0x{unknownFlags:X}");

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
        foreach (var field in fieldList) RecordComparisonFields.Add(field);

        for (var columnIndex = 0; columnIndex < columnList.Count; columnIndex++)
        {
            var column = columnList[columnIndex];
            var plugin = pluginLookup[column.ModKey];
            RecordComparisonColumns.Add(new RecordComparisonColumnViewModel(column.ModKey, plugin.LoadOrderIndex, column.ModKey == activeModKey, column.Values, states, columnIndex == columnList.Count - 1));
        }

        HasRecordComparison = RecordComparisonFields.Count > 0 && RecordComparisonColumns.Count > 0;
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
                state = columns
                    .Select(column => column.Values[fieldIndex])
                    .Distinct(StringComparer.Ordinal)
                    .Count() == 1
                    ? RecordComparisonValueState.Identical
                    : RecordComparisonValueState.Conflict;

            fields[fieldIndex].State = state;
            states.Add(state);
        }

        return states;
    }

    private void ClearRecordComparison()
    {
        RecordComparisonFields.Clear();
        RecordComparisonColumns.Clear();
        RecordComparisonGroups.Clear();
        RecordComparisonScripts.Clear();
        RecordComparisonPerkRanks.Clear();
        HasRecordComparison = false;
        HasRecordComparisonGroups = false;
        HasRecordComparisonScripts = false;
        HasRecordComparisonPerkRanks = false;
        ScriptingAdapterSectionHeader = "Scripts";
        PerkRankSectionHeader = "Perk Ranks";
    }

    private void CollapseAllScripts()
    {
        foreach (var script in RecordComparisonScripts) script.IsExpanded = false;
    }

    private void CollapseAllPerkRanks()
    {
        foreach (var rank in RecordComparisonPerkRanks) rank.IsExpanded = false;
    }

    private void UpdateStatusBar(RecordTreeItemViewModel? selectedRecord)
    {
        var totalPluginRecords = PluginService.GetImportedPluginRecordCount();
        var activePlugin = ActivePluginSelectionService.ActivePlugin;
        ImportedRecordCountText = $"Imported records: {totalPluginRecords:N0}";
        ActivePluginText = activePlugin == null
            ? "Active plugin: None"
            : $"Active plugin: {activePlugin.ModKey.FileName}";
        ActiveRecordCountText = activePlugin == null
            ? "Active records: 0"
            : $"Active records: {activePlugin.RecordCount:N0}";
        SelectedRecordContextText = selectedRecord == null
            ? string.Empty
            : string.IsNullOrWhiteSpace(selectedRecord.EditorID)
                ? $"{selectedRecord.RecordType} {selectedRecord.FormIDText}"
                : $"{selectedRecord.RecordType} {selectedRecord.EditorID} ({selectedRecord.FormIDText})";
        SelectedRecordContextVisibility = selectedRecord == null ? Visibility.Collapsed : Visibility.Visible;
        StatusText = GetStatusText();
    }

    private void ExpandChangedScripts()
    {
        foreach (var script in RecordComparisonScripts) script.IsExpanded = script.HasChanges;
    }

    private void ExpandChangedPerkRanks()
    {
        foreach (var rank in RecordComparisonPerkRanks) rank.IsExpanded = rank.HasChanges;
    }

    private void ToggleChangedOnlyScripts()
    {
        ShowChangedOnlyScripts = !ShowChangedOnlyScripts;
    }

    private void ToggleChangedOnlyPerkRanks()
    {
        ShowChangedOnlyPerkRanks = !ShowChangedOnlyPerkRanks;
    }

    private static IStarfieldModGetter LoadMod(ModKey modKey)
    {
        try
        {
            var environment = GameEnvironment.Typical.Starfield(StarfieldRelease.Starfield);
            return StarfieldMod.Create(StarfieldRelease.Starfield)
                .FromPath(Path.Join(environment.DataFolderPath, modKey.FileName))
                .WithLoadOrderFromHeaderMasters()
                .WithDataFolder(environment.DataFolderPath)
                .Construct();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            RecordException.EnrichAndThrow(ex, modKey);
            throw;
        }
    }
}