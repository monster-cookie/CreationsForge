using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
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

    public ObservableCollection<RecordTreeItemViewModel> RecordTreeItems { get; } = new();
    public ObservableCollection<RecordComparisonFieldViewModel> RecordComparisonFields { get; } = new();
    public ObservableCollection<RecordComparisonColumnViewModel> RecordComparisonColumns { get; } = new();
    public ObservableCollection<RecordComparisonScriptViewModel> RecordComparisonScripts { get; } = new();

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

    public bool HasRecordComparison
    {
        get;
        private set
        {
            if (!SetProperty(ref field, value))
            {
                return;
            }

            RecordComparisonVisibility = value ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    public Visibility RecordComparisonVisibility
    {
        get;
        private set => SetProperty(ref field, value);
    } = Visibility.Collapsed;

    public bool HasRecordComparisonScripts
    {
        get;
        private set
        {
            if (!SetProperty(ref field, value))
            {
                return;
            }

            RecordComparisonScriptsVisibility = value ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    public Visibility RecordComparisonScriptsVisibility
    {
        get;
        private set => SetProperty(ref field, value);
    } = Visibility.Collapsed;

    public bool ShowChangedOnlyScripts
    {
        get;
        private set
        {
            if (!SetProperty(ref field, value))
            {
                return;
            }

            foreach (var script in RecordComparisonScripts)
            {
                script.SetShowChangedOnly(value);
            }

            ChangedOnlyScriptsButtonText = value ? "Show all" : "Changed only";
        }
    }

    public string ChangedOnlyScriptsButtonText
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

        if (item.RecordType == RecordTypeCatalog.Global.RecordType)
        {
            LoadGlobalComparison(formKey);
        }

        if (item.RecordType == RecordTypeCatalog.MiscItem.RecordType)
        {
            LoadMiscItemComparison(formKey);
        }

        if (item.RecordType == RecordTypeCatalog.Keyword.RecordType)
        {
            LoadKeywordComparison(formKey);
        }

        if (item.RecordType == RecordTypeCatalog.NPC.RecordType)
        {
            LoadNPCComparison(formKey);
        }

        if (item.RecordType == RecordTypeCatalog.ActorValueInformation.RecordType)
        {
            LoadActorValueInformationComparison(formKey);
        }

        if (item.RecordType == RecordTypeCatalog.MagicEffect.RecordType)
        {
            LoadMagicEffectComparison(formKey);
        }

        if (item.RecordType == RecordTypeCatalog.Perk.RecordType)
        {
            LoadPerkComparison(formKey);
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
        var fields = CreateHeaderFields("Name", "ShortName", "Value", "Weight");
        SetRecordComparisonWithScripting(fields, records,
            record => new List<string> { RecordTypeCatalog.MiscItem.RecordID, record.EditorID, record.FormKey.ToString(), FormatStarfieldMajorRecordFlags(record.StarfieldMajorRecordFlags), record.Name ?? string.Empty, record.ShortName ?? string.Empty, record.Value?.ToString() ?? string.Empty, record.Weight?.ToString() ?? string.Empty });
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
        var fields = CreateHeaderFields("Name", "Description", "Flags", "SkillGroup", "CrewAssignment", "PerkIcon");
        SetRecordComparisonWithScripting(fields, records,
            record => new List<string> { RecordTypeCatalog.Perk.RecordID, record.EditorID, record.FormKey.ToString(), FormatStarfieldMajorRecordFlags(record.StarfieldMajorRecordFlags), record.Name ?? string.Empty, record.Description ?? string.Empty, record.Flags, record.SkillGroup ?? string.Empty, record.CrewAssignment ?? string.Empty, record.PerkIcon ?? string.Empty });
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
        if (orderedModKeys.Count == 0 || records.Count == 0)
        {
            return;
        }

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
        if (!isComparable || values.Count <= 1)
        {
            return RecordComparisonValueState.Neutral;
        }

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
        if (property == null)
        {
            return string.Empty;
        }

        if (property.ObjectFormKey != null)
        {
            return $"{property.ObjectFormKey} | Alias={property.ObjectAlias?.ToString() ?? string.Empty} | Unused={property.ObjectUnused?.ToString() ?? string.Empty}";
        }

        if (property.DataBool.HasValue)
        {
            return property.DataBool.Value.ToString();
        }

        if (property.DataInt.HasValue)
        {
            return property.DataInt.Value.ToString();
        }

        if (property.DataFloat.HasValue)
        {
            return property.DataFloat.Value.ToString();
        }

        return property.DataString ?? string.Empty;
    }

    private static string FormatScriptingAdapterListItemValue(ScriptingAdapterPropertyListItemDTO? listItem)
    {
        if (listItem == null)
        {
            return string.Empty;
        }

        if (listItem.ObjectFormKey != null)
        {
            return $"{listItem.ObjectFormKey} | Alias={listItem.ObjectAlias?.ToString() ?? string.Empty} | Unused={listItem.ObjectUnused?.ToString() ?? string.Empty}";
        }

        if (listItem.DataBool.HasValue)
        {
            return listItem.DataBool.Value.ToString();
        }

        if (listItem.DataInt.HasValue)
        {
            return listItem.DataInt.Value.ToString();
        }

        if (listItem.DataFloat.HasValue)
        {
            return listItem.DataFloat.Value.ToString();
        }

        return listItem.DataString ?? string.Empty;
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
        RecordComparisonScripts.Clear();
        HasRecordComparison = false;
        HasRecordComparisonScripts = false;
        ScriptingAdapterSectionHeader = "Scripts";
    }

    private void CollapseAllScripts()
    {
        foreach (var script in RecordComparisonScripts)
        {
            script.IsExpanded = false;
        }
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
            : $"{selectedRecord.RecordType} {selectedRecord.FormIDText}";
        SelectedRecordContextVisibility = selectedRecord == null ? Visibility.Collapsed : Visibility.Visible;
        StatusText = GetStatusText();
    }

    private void ExpandChangedScripts()
    {
        foreach (var script in RecordComparisonScripts)
        {
            script.IsExpanded = script.HasChanges;
        }
    }

    private void ToggleChangedOnlyScripts()
    {
        ShowChangedOnlyScripts = !ShowChangedOnlyScripts;
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
