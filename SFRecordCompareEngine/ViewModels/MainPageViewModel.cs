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
        StatusText = GetStatusText();
        ActivePluginSelectionService.ActivePluginChanged += OnActivePluginChanged;
    }

    public AsyncRelayCommand OpenCommand { get; }
    public AsyncRelayCommand OptionsCommand { get; }
    public AsyncRelayCommand ReimportAllPluginsCommand { get; }
    public RelayCommand ExitCommand { get; }

    public ObservableCollection<RecordTreeItemViewModel> RecordTreeItems { get; } = new();
    public ObservableCollection<RecordComparisonFieldViewModel> RecordComparisonFields { get; } = new();
    public ObservableCollection<RecordComparisonColumnViewModel> RecordComparisonColumns { get; } = new();

    public string FormIDFilter
    {
        get;
        private set
        {
            if (!SetProperty(ref field, value))
            {
                return;
            }

            ApplyFilters();
        }
    } = string.Empty;

    public string EditorIDFilter
    {
        get;
        private set
        {
            if (!SetProperty(ref field, value))
            {
                return;
            }

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

    private async Task ReimportAllPluginsAsync()
    {
        ActivePluginSelectionService.ClearActivePlugin();
        await ApplicationNavigationService.ShowStartupImportAsync(true);
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
        if (item?.FormKey == null || item.RecordType == null)
        {
            return;
        }

        if (item.RecordType == RecordTypeCatalog.FormList.RecordType)
        {
            LoadFormListComparison(item.FormKey.Value.ID);
            return;
        }

        if (item.RecordType == RecordTypeCatalog.GameSetting.RecordType)
        {
            LoadGameSettingComparison(item.FormKey.Value.ID);
            return;
        }

        if (item.RecordType == RecordTypeCatalog.Global.RecordType)
        {
            LoadGlobalComparison(item.FormKey.Value.ID);
        }

        if (item.RecordType == RecordTypeCatalog.MiscItem.RecordType)
        {
            LoadMiscItemComparison(item.FormKey.Value.ID);
        }

        if (item.RecordType == RecordTypeCatalog.Keyword.RecordType)
        {
            LoadKeywordComparison(item.FormKey.Value.ID);
        }

        if (item.RecordType == RecordTypeCatalog.NPC.RecordType)
        {
            LoadNPCComparison(item.FormKey.Value.ID);
        }

        if (item.RecordType == RecordTypeCatalog.ActorValueInformation.RecordType)
        {
            LoadActorValueInformationComparison(item.FormKey.Value.ID);
        }

        if (item.RecordType == RecordTypeCatalog.MagicEffect.RecordType)
        {
            LoadMagicEffectComparison(item.FormKey.Value.ID);
        }

        if (item.RecordType == RecordTypeCatalog.Perk.RecordType)
        {
            LoadPerkComparison(item.FormKey.Value.ID);
        }
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
        if (plugin.HeaderFlags.HasFlag(StarfieldModHeader.HeaderFlag.Overlay))
        {
            pluginTypes.Add("overlay");
        }

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
        AddRecordType(recordTreeItems, RecordTypeCatalog.Global.RecordType, GlobalService.GetByModKey(activePlugin.ModKey).Select(record => CreateRecordTreeItem(masterPackage, record.FormKey, record.EditorID, RecordTypeCatalog.Global.RecordType)));
        AddRecordType(recordTreeItems, RecordTypeCatalog.MiscItem.RecordType, MiscItemService.GetByModKey(activePlugin.ModKey).Select(record => CreateRecordTreeItem(masterPackage, record.FormKey, record.EditorID, RecordTypeCatalog.MiscItem.RecordType)));
        AddRecordType(recordTreeItems, RecordTypeCatalog.Keyword.RecordType, KeywordService.GetByModKey(activePlugin.ModKey).Select(record => CreateRecordTreeItem(masterPackage, record.FormKey, record.EditorID, RecordTypeCatalog.Keyword.RecordType)));
        AddRecordType(recordTreeItems, RecordTypeCatalog.NPC.RecordType, NPCService.GetByModKey(activePlugin.ModKey).Select(record => CreateRecordTreeItem(masterPackage, record.FormKey, record.EditorID, RecordTypeCatalog.NPC.RecordType)));
        AddRecordType(recordTreeItems, RecordTypeCatalog.ActorValueInformation.RecordType, ActorValueInformationService.GetByModKey(activePlugin.ModKey).Select(record => CreateRecordTreeItem(masterPackage, record.FormKey, record.EditorID, RecordTypeCatalog.ActorValueInformation.RecordType)));
        AddRecordType(recordTreeItems, RecordTypeCatalog.MagicEffect.RecordType, MagicEffectService.GetByModKey(activePlugin.ModKey).Select(record => CreateRecordTreeItem(masterPackage, record.FormKey, record.EditorID, RecordTypeCatalog.MagicEffect.RecordType)));
        AddRecordType(recordTreeItems, RecordTypeCatalog.Perk.RecordType, PerkService.GetByModKey(activePlugin.ModKey).Select(record => CreateRecordTreeItem(masterPackage, record.FormKey, record.EditorID, RecordTypeCatalog.Perk.RecordType)));
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
            return filteredItem.Children.Count > 0 || (string.IsNullOrWhiteSpace(FormIDFilter) && string.IsNullOrWhiteSpace(EditorIDFilter))
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
                return item.FormKey == MasterPackage.GetFormKey(formID, false);
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

    private void LoadGlobalComparison(uint formKeyID)
    {
        var records = GlobalService.GetByFormKeyID(formKeyID);
        var fields = CreateHeaderFields("Data");
        SetRecordComparison(fields, records.Select(record => (record.ModKey, Values: (IReadOnlyList<string>)new List<string> { RecordTypeCatalog.Global.RecordID, record.EditorID, record.FormKey.ToString(), FormatStarfieldMajorRecordFlags(record.StarfieldMajorRecordFlags), record.Data?.ToString() ?? string.Empty })));
    }

    private void LoadMiscItemComparison(uint formKeyID)
    {
        var records = MiscItemService.GetByFormKeyID(formKeyID);
        var fields = CreateHeaderFields("Name", "ShortName", "Value", "Weight");
        SetRecordComparison(fields,
            records.Select(record => (record.ModKey,
                Values: (IReadOnlyList<string>)new List<string> { RecordTypeCatalog.MiscItem.RecordID, record.EditorID, record.FormKey.ToString(), FormatStarfieldMajorRecordFlags(record.StarfieldMajorRecordFlags), record.Name ?? string.Empty, record.ShortName ?? string.Empty, record.Value?.ToString() ?? string.Empty, record.Weight?.ToString() ?? string.Empty })));
    }

    private void LoadKeywordComparison(uint formKeyID)
    {
        var records = KeywordService.GetByFormKeyID(formKeyID);
        var fields = CreateHeaderFields("Name", "Color", "Type", "Notes", "FlashLinkageName", "AttractionRuleFormKey");
        SetRecordComparison(fields,
            records.Select(record => (record.ModKey,
                Values: (IReadOnlyList<string>)new List<string>
                    { RecordTypeCatalog.Keyword.RecordID, record.EditorID, record.FormKey.ToString(), FormatStarfieldMajorRecordFlags(record.StarfieldMajorRecordFlags), record.Name ?? string.Empty, record.Color, record.Type, record.Notes ?? string.Empty, record.FlashLinkageName ?? string.Empty, record.AttractionRuleFormKey?.ToString() ?? string.Empty })));
    }

    private void LoadNPCComparison(uint formKeyID)
    {
        var records = NPCService.GetByFormKeyID(formKeyID);
        var fields = CreateHeaderFields("Name", "ShortName", "LongName", "DispositionBase", "Aggression", "Confidence", "EnergyLevel", "Responsibility", "Assistance", "GearedUpWeapons", "HeightMin", "HeightMax", "SkinToneIndex", "Pronoun", "VoiceFormKey", "RaceFormKey", "CombatOverridePackageListFormKey", "CombatStyleFormKey", "DefaultPackageListFormKey", "CrimeFactionFormKey");
        SetRecordComparison(fields,
            records.Select(record => (record.ModKey,
                Values: (IReadOnlyList<string>)new List<string>
                {
                    RecordTypeCatalog.NPC.RecordID, record.EditorID, record.FormKey.ToString(), FormatStarfieldMajorRecordFlags(record.StarfieldMajorRecordFlags), record.Name ?? string.Empty, record.ShortName ?? string.Empty, record.LongName ?? string.Empty, record.DispositionBase.ToString(), record.Aggression, record.Confidence, record.EnergyLevel.ToString(), record.Responsibility,
                    record.Assistance, record.GearedUpWeapons.ToString(), record.HeightMin.ToString(), record.HeightMax.ToString(), record.SkinToneIndex?.ToString() ?? string.Empty, record.Pronoun ?? string.Empty, record.VoiceFormKey?.ToString() ?? string.Empty, record.RaceFormKey?.ToString() ?? string.Empty, record.CombatOverridePackageListFormKey?.ToString() ?? string.Empty,
                    record.CombatStyleFormKey?.ToString() ?? string.Empty, record.DefaultPackageListFormKey?.ToString() ?? string.Empty, record.CrimeFactionFormKey?.ToString() ?? string.Empty
                })));
    }

    private void LoadActorValueInformationComparison(uint formKeyID)
    {
        var records = ActorValueInformationService.GetByFormKeyID(formKeyID);
        var fields = CreateHeaderFields("Name", "Abbreviation", "ContextNotes", "DefaultValue", "Flags", "Type", "Min", "Max");
        SetRecordComparison(fields,
            records.Select(record => (record.ModKey,
                Values: (IReadOnlyList<string>)new List<string>
                {
                    RecordTypeCatalog.ActorValueInformation.RecordID, record.EditorID, record.FormKey.ToString(), FormatStarfieldMajorRecordFlags(record.StarfieldMajorRecordFlags), record.Name ?? string.Empty, record.Abbreviation ?? string.Empty, record.ContextNotes ?? string.Empty, record.DefaultValue?.ToString() ?? string.Empty, record.Flags ?? string.Empty, record.Type ?? string.Empty,
                    record.Min?.ToString() ?? string.Empty, record.Max?.ToString() ?? string.Empty
                })));
    }

    private void LoadMagicEffectComparison(uint formKeyID)
    {
        var records = MagicEffectService.GetByFormKeyID(formKeyID);
        var fields = CreateHeaderFields("Name", "Description", "Flags", "CastType", "TargetType", "ActorValue2FormKey", "ResistValueFormKey", "PerkToApplyFormKey", "EquipAbilityFormKey", "ExplosionFormKey", "CastingArtFormKey", "HitEffectArtFormKey", "HitShaderFormKey", "ImageSpaceModifierFormKey", "ImpactDataFormKey", "ProjectileFormKey");
        SetRecordComparison(fields,
            records.Select(record => (record.ModKey,
                Values: (IReadOnlyList<string>)new List<string>
                {
                    RecordTypeCatalog.MagicEffect.RecordID, record.EditorID, record.FormKey.ToString(), FormatStarfieldMajorRecordFlags(record.StarfieldMajorRecordFlags), record.Name ?? string.Empty, record.Description ?? string.Empty, record.Flags, record.CastType ?? string.Empty, record.TargetType ?? string.Empty, record.ActorValue2FormKey?.ToString() ?? string.Empty,
                    record.ResistValueFormKey?.ToString() ?? string.Empty, record.PerkToApplyFormKey?.ToString() ?? string.Empty, record.EquipAbilityFormKey?.ToString() ?? string.Empty, record.ExplosionFormKey?.ToString() ?? string.Empty, record.CastingArtFormKey?.ToString() ?? string.Empty, record.HitEffectArtFormKey?.ToString() ?? string.Empty,
                    record.HitShaderFormKey?.ToString() ?? string.Empty, record.ImageSpaceModifierFormKey?.ToString() ?? string.Empty, record.ImpactDataFormKey?.ToString() ?? string.Empty, record.ProjectileFormKey?.ToString() ?? string.Empty
                })));
    }

    private void LoadPerkComparison(uint formKeyID)
    {
        var records = PerkService.GetByFormKeyID(formKeyID);
        var fields = CreateHeaderFields("Name", "Description", "Flags", "SkillGroup", "CrewAssignment", "PerkIcon");
        SetRecordComparison(fields,
            records.Select(record => (record.ModKey,
                Values: (IReadOnlyList<string>)new List<string> { RecordTypeCatalog.Perk.RecordID, record.EditorID, record.FormKey.ToString(), FormatStarfieldMajorRecordFlags(record.StarfieldMajorRecordFlags), record.Name ?? string.Empty, record.Description ?? string.Empty, record.Flags, record.SkillGroup ?? string.Empty, record.CrewAssignment ?? string.Empty, record.PerkIcon ?? string.Empty })));
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

    private static string FormatStarfieldMajorRecordFlags(StarfieldMajorRecord.StarfieldMajorRecordFlag flags)
    {
        var value = Convert.ToUInt64(flags);
        if (value == 0)
        {
            return string.Empty;
        }

        var names = new List<string>();
        var knownFlags = 0UL;
        foreach (var flag in Enum.GetValues<StarfieldMajorRecord.StarfieldMajorRecordFlag>())
        {
            var flagValue = Convert.ToUInt64(flag);
            if (flagValue == 0 || (flagValue & (flagValue - 1)) != 0 || (value & flagValue) == 0)
            {
                continue;
            }

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
