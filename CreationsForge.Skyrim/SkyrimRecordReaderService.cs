using System.Globalization;
using System.Collections;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Utilities;
using CreationsForge.Skyrim.Interfaces;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Skyrim;

public class SkyrimRecordReaderService : ISkyrimRecordReaderService
{
    private readonly SkyrimGameMetadataService GameMetadataService;

    public SkyrimRecordReaderService(SkyrimGameMetadataService gameMetadataService)
    {
        GameMetadataService = gameMetadataService;
    }

    public PluginRecordSetDTO ReadPluginRecords(PluginDTO plugin, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var mod = LoadMod(plugin);
        cancellationToken.ThrowIfCancellationRequested();
        var formLists = MapFormLists(plugin, mod);
        cancellationToken.ThrowIfCancellationRequested();
        var gameSettings = MapGameSettings(plugin, mod);
        cancellationToken.ThrowIfCancellationRequested();
        var globals = MapGlobals(plugin, mod);
        cancellationToken.ThrowIfCancellationRequested();
        var classes = MapClasses(plugin, mod);
        cancellationToken.ThrowIfCancellationRequested();
        var factions = MapFactions(plugin, mod);
        cancellationToken.ThrowIfCancellationRequested();
        var miscObjects = MapMiscObjects(plugin, mod);
        cancellationToken.ThrowIfCancellationRequested();
        var keywords = MapKeywords(plugin, mod);
        cancellationToken.ThrowIfCancellationRequested();
        var actorValueInformation = MapActorValueInformation(plugin, mod);
        cancellationToken.ThrowIfCancellationRequested();
        var npcs = MapNPCs(plugin, mod);
        cancellationToken.ThrowIfCancellationRequested();
        var magicEffects = MapMagicEffects(plugin, mod);
        cancellationToken.ThrowIfCancellationRequested();
        var perks = MapPerks(plugin, mod);
        cancellationToken.ThrowIfCancellationRequested();
        var statics = MapStatics(plugin, mod);
        cancellationToken.ThrowIfCancellationRequested();
        var containers = MapContainers(plugin, mod);
        cancellationToken.ThrowIfCancellationRequested();
        var constructibleObjects = MapConstructibleObjects(plugin, mod);

        return new PluginRecordSetDTO
        {
            FormLists = formLists,
            GameSettings = gameSettings,
            Globals = globals,
            Classes = classes,
            Factions = factions,
            MiscObjects = miscObjects,
            Keywords = keywords,
            ActorValueInformation = actorValueInformation,
            NPCs = npcs,
            MagicEffects = magicEffects,
            Perks = perks,
            Statics = statics,
            Containers = containers,
            ConstructibleObjects = constructibleObjects
        };
    }

    public IReadOnlyList<FormListDTO> ReadFormLists(PluginDTO plugin)
    {
        var mod = LoadMod(plugin);
        return MapFormLists(plugin, mod);
    }

    private static IReadOnlyList<FormListDTO> MapFormLists(PluginDTO plugin, ISkyrimModGetter mod)
    {
        return mod.FormLists
            .Select(record => new FormListDTO
            {
                Game = SupportedGame.Skyrim,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.SkyrimMajorRecordFlags,
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                ImportedAtUTC = DateTime.UtcNow,
                Items = record.Items.Select((item, itemIndex) => new FormListItemDTO
                {
                    Game = SupportedGame.Skyrim,
                    ModKey = plugin.ModKey,
                    FormKey = MapFormKey(record.FormKey),
                    ItemFormKey = MapFormKey(item.FormKey),
                    ItemIndex = itemIndex,
                    ImportedAtUTC = DateTime.UtcNow
                }).ToList()
            })
            .ToList();
    }

    public IReadOnlyList<GameSettingDTO> ReadGameSettings(PluginDTO plugin)
    {
        var mod = LoadMod(plugin);
        return MapGameSettings(plugin, mod);
    }

    private static IReadOnlyList<GameSettingDTO> MapGameSettings(PluginDTO plugin, ISkyrimModGetter mod)
    {
        return mod.GameSettings
            .Select(record => LocalizedStringDTOMapper.AddLocalizedStrings(new GameSettingDTO
            {
                Game = SupportedGame.Skyrim,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.SkyrimMajorRecordFlags,
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                ImportedAtUTC = DateTime.UtcNow,
                MutagenObjectType = GetGameSettingType(record),
                SettingType = GetGameSettingType(record),
                Data = GetGameSettingData(record),
                NumericData = GetGameSettingNumericData(record),
                IntegerData = GetGameSettingIntegerData(record),
                BooleanData = GetGameSettingBooleanData(record)
            }, record))
            .ToList();
    }

    public IReadOnlyList<GlobalDTO> ReadGlobals(PluginDTO plugin)
    {
        var mod = LoadMod(plugin);
        return MapGlobals(plugin, mod);
    }

    private static IReadOnlyList<GlobalDTO> MapGlobals(PluginDTO plugin, ISkyrimModGetter mod)
    {
        return mod.Globals
            .Select(record => new GlobalDTO
            {
                Game = SupportedGame.Skyrim,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.SkyrimMajorRecordFlags,
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                ImportedAtUTC = DateTime.UtcNow,
                Data = GetGlobalData(record)
            })
            .ToList();
    }

    private static IReadOnlyList<KeywordDTO> MapKeywords(PluginDTO plugin, ISkyrimModGetter mod)
    {
        return GetRecordCollection(mod, "Keywords")
            .Select(record => LocalizedStringDTOMapper.AddLocalizedStrings(new KeywordDTO
            {
                Game = SupportedGame.Skyrim,
                ModKey = plugin.ModKey,
                FormKey = GetRequiredFormKey(record),
                EditorID = GetPropertyString(record, "EditorID"),
                FormVersion = GetPropertyInt(record, "FormVersion"),
                MajorRecordFlags = GetPropertyInt(record, "SkyrimMajorRecordFlags"),
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                ImportedAtUTC = DateTime.UtcNow,
                Name = GetTranslatedString(record, "Name"),
                Color = GetPropertyString(record, "Color"),
                Type = GetPropertyString(record, "Type"),
                Notes = GetPropertyStringOrNull(record, "Notes"),
                FlashLinkageName = GetPropertyStringOrNull(record, "FlashLinkageName"),
                AttractionRuleFormKey = GetLinkedFormKey(record, "AttractionRule"),
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.Keyword.RecordID, record)
            }, record))
            .ToList();
    }

    private static IReadOnlyList<ClassDTO> MapClasses(PluginDTO plugin, ISkyrimModGetter mod)
    {
        return GetRecordCollection(mod, "Classes")
            .Select(record => CreateClass(plugin, SupportedGame.Skyrim, record, "SkyrimMajorRecordFlags"))
            .ToList();
    }

    private static IReadOnlyList<FactionDTO> MapFactions(PluginDTO plugin, ISkyrimModGetter mod)
    {
        return GetRecordCollection(mod, "Factions")
            .Select(record => CreateFaction(plugin, SupportedGame.Skyrim, record, "SkyrimMajorRecordFlags"))
            .ToList();
    }

    private static IReadOnlyList<MiscObjectDTO> MapMiscObjects(PluginDTO plugin, ISkyrimModGetter mod)
    {
        return GetRecordCollection(mod, "MiscItems", "MiscObjects")
            .Select(record => LocalizedStringDTOMapper.AddLocalizedStrings(new MiscObjectDTO
            {
                Game = SupportedGame.Skyrim,
                ModKey = plugin.ModKey,
                FormKey = GetRequiredFormKey(record),
                EditorID = GetPropertyString(record, "EditorID"),
                FormVersion = GetPropertyInt(record, "FormVersion"),
                MajorRecordFlags = GetPropertyInt(record, "SkyrimMajorRecordFlags"),
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                ImportedAtUTC = DateTime.UtcNow,
                Name = GetTranslatedString(record, "Name"),
                ShortName = GetTranslatedString(record, "ShortName"),
                Value = GetPropertyNullableInt(record, "Value"),
                Weight = GetPropertyNullableFloat(record, "Weight"),
                DirtinessScale = GetPropertyNullableFloat(record, "DirtinessScale"),
                FeaturedItemMessageFormKey = GetLinkedFormKey(record, "FeaturedItemMessage"),
                Flag = FormatHexValue(GetPropertyValue(record, "FLAG")),
                Models = GetModels(plugin, RecordTypeCatalog.MiscObject.RecordID, GetRequiredRawFormKey(record), GetPropertyValue(record, "Model")),
                Keywords = GetRecordKeywords(plugin, RecordTypeCatalog.MiscObject.RecordID, GetRequiredRawFormKey(record), GetPropertyValue(record, "Keywords")),
                Sounds = GetNamedSounds(plugin, RecordTypeCatalog.MiscObject.RecordID, GetRequiredRawFormKey(record), record, "CraftingSound", "PickupSound", "PutdownSound", "DropdownSound"),
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.MiscObject.RecordID, record)
            }, record))
            .ToList();
    }

    private static IReadOnlyList<ActorValueInformationDTO> MapActorValueInformation(PluginDTO plugin, ISkyrimModGetter mod)
    {
        return GetRecordCollection(mod, "ActorValueInformation", "ActorValues")
            .Select(record => LocalizedStringDTOMapper.AddLocalizedStrings(new ActorValueInformationDTO
            {
                Game = SupportedGame.Skyrim,
                ModKey = plugin.ModKey,
                FormKey = GetRequiredFormKey(record),
                EditorID = GetPropertyString(record, "EditorID"),
                FormVersion = GetPropertyInt(record, "FormVersion"),
                MajorRecordFlags = GetPropertyInt(record, "SkyrimMajorRecordFlags"),
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                ImportedAtUTC = DateTime.UtcNow,
                Name = GetTranslatedString(record, "Name"),
                Abbreviation = GetTranslatedString(record, "Abbreviation"),
                ContextNotes = GetPropertyStringOrNull(record, "ContextNotes"),
                DefaultValue = GetPropertyNullableDouble(record, "DefaultValue"),
                Flags = GetPropertyStringOrNull(record, "Flags"),
                Type = GetPropertyStringOrNull(record, "Type"),
                Min = GetPropertyNullableDouble(record, "Min"),
                Max = GetPropertyNullableDouble(record, "Max"),
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.ActorValueInformation.RecordID, record)
            }, record))
            .ToList();
    }

    private static IReadOnlyList<NPCDTO> MapNPCs(PluginDTO plugin, ISkyrimModGetter mod)
    {
        return GetRecordCollection(mod, "Npcs", "NPCs")
            .Select(record => LocalizedStringDTOMapper.AddLocalizedStrings(new NPCDTO
            {
                Game = SupportedGame.Skyrim,
                ModKey = plugin.ModKey,
                FormKey = GetRequiredFormKey(record),
                EditorID = GetPropertyString(record, "EditorID"),
                FormVersion = GetPropertyInt(record, "FormVersion"),
                MajorRecordFlags = GetPropertyInt(record, "SkyrimMajorRecordFlags"),
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                ImportedAtUTC = DateTime.UtcNow,
                Name = GetTranslatedString(record, "Name"),
                ShortName = GetTranslatedString(record, "ShortName"),
                LongName = GetTranslatedString(record, "LongName"),
                DispositionBase = GetPropertyInt(record, "DispositionBase"),
                Aggression = GetPropertyString(record, "Aggression"),
                Confidence = GetPropertyString(record, "Confidence"),
                EnergyLevel = GetPropertyInt(record, "EnergyLevel"),
                Responsibility = GetPropertyString(record, "Responsibility"),
                Assistance = GetPropertyString(record, "Assistance"),
                GearedUpWeapons = GetPropertyInt(record, "GearedUpWeapons"),
                HeightMin = GetPropertyDouble(record, "HeightMin"),
                HeightMax = GetPropertyDouble(record, "HeightMax"),
                SkinToneIndex = GetPropertyNullableInt(record, "SkinToneIndex"),
                Pronoun = GetPropertyStringOrNull(record, "Pronoun"),
                VoiceFormKey = GetLinkedFormKey(record, "Voice"),
                RaceFormKey = GetLinkedFormKey(record, "Race"),
                CombatOverridePackageListFormKey = GetLinkedFormKey(record, "CombatOverridePackageList"),
                CombatStyleFormKey = GetLinkedFormKey(record, "CombatStyle"),
                DefaultPackageListFormKey = GetLinkedFormKey(record, "DefaultPackageList"),
                CrimeFactionFormKey = GetLinkedFormKey(record, "CrimeFaction"),
                Keywords = GetRecordKeywords(plugin, RecordTypeCatalog.NPC.RecordID, GetRequiredRawFormKey(record), GetPropertyValue(record, "Keywords")),
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.NPC.RecordID, record)
            }, record))
            .ToList();
    }

    private static ClassDTO CreateClass(PluginDTO plugin, SupportedGame game, object record, string majorFlagsProperty)
    {
        var formKey = GetRequiredRawFormKey(record);
        return LocalizedStringDTOMapper.AddLocalizedStrings(new ClassDTO
        {
            Game = game,
            ModKey = plugin.ModKey,
            FormKey = MapFormKey(formKey),
            EditorID = GetPropertyString(record, "EditorID"),
            FormVersion = GetPropertyInt(record, "FormVersion"),
            MajorRecordFlags = GetPropertyInt(record, majorFlagsProperty),
            Version2 = GetPropertyNullableInt(record, "Version2"),
            VersionControl = GetPropertyNullableInt(record, "VersionControl"),
            ImportedAtUTC = DateTime.UtcNow,
            Name = GetTranslatedString(record, "Name"),
            Description = GetTranslatedString(record, "Description"),
            Teaches = GetPropertyValue(record, "Teaches")?.ToString(),
            MaxTrainingLevel = GetPropertyNullableInt(record, "MaxTrainingLevel"),
            BleedoutDefault = GetPropertyNullableDouble(record, "BleedoutDefault"),
            VoicePoints = GetPropertyNullableDouble(record, "VoicePoints"),
            Unknown = GetPropertyNullableDouble(record, "Unknown"),
            Unknown2 = GetPropertyNullableDouble(record, "Unknown2"),
            Properties = GetClassProperties(plugin, game, formKey, GetPropertyValue(record, "Properties")),
            SkillWeights = GetClassWeights(plugin, game, formKey, "Skill", GetPropertyValue(record, "SkillWeights")),
            StatWeights = GetClassWeights(plugin, game, formKey, "Stat", GetPropertyValue(record, "StatWeights"))
        }, record);
    }

    private static FactionDTO CreateFaction(PluginDTO plugin, SupportedGame game, object record, string majorFlagsProperty)
    {
        var formKey = GetRequiredRawFormKey(record);
        var crimeValues = GetPropertyValue(record, "CrimeValues");
        var vendorValues = GetPropertyValue(record, "VendorValues");
        var vendorLocation = GetPropertyValue(record, "VendorLocation");
        var vendorLocationTarget = GetPropertyValue(vendorLocation, "Target");
        return LocalizedStringDTOMapper.AddLocalizedStrings(new FactionDTO
        {
            Game = game,
            ModKey = plugin.ModKey,
            FormKey = MapFormKey(formKey),
            EditorID = GetPropertyString(record, "EditorID"),
            FormVersion = GetPropertyInt(record, "FormVersion"),
            MajorRecordFlags = GetPropertyInt(record, majorFlagsProperty),
            Version2 = GetPropertyNullableInt(record, "Version2"),
            VersionControl = GetPropertyNullableInt(record, "VersionControl"),
            ImportedAtUTC = DateTime.UtcNow,
            Name = GetTranslatedString(record, "Name"),
            Flags = FormatEnumerable(GetPropertyValue(record, "Flags")),
            FormationRadius = GetPropertyNullableDouble(record, "FormationRadius"),
            KeywordFormKey = GetFormKeyFromObject(GetPropertyValue(record, "Keyword")),
            HerdFormKey = GetFormKeyFromObject(GetPropertyValue(record, "Herd")),
            VoiceTypeFormKey = GetFormKeyFromObject(GetPropertyValue(record, "VoiceType")),
            SharedCrimeFactionListFormKey = GetFormKeyFromObject(GetPropertyValue(record, "SharedCrimeFactionList")),
            VendorBuySellListFormKey = GetFormKeyFromObject(GetPropertyValue(record, "VendorBuySellList")),
            MerchantContainerFormKey = GetFormKeyFromObject(GetPropertyValue(record, "MerchantContainer")),
            ExteriorJailMarkerFormKey = GetFormKeyFromObject(GetPropertyValue(record, "ExteriorJailMarker")),
            FollowerWaitMarkerFormKey = GetFormKeyFromObject(GetPropertyValue(record, "FollowerWaitMarker")),
            StolenGoodsContainerFormKey = GetFormKeyFromObject(GetPropertyValue(record, "StolenGoodsContainer")),
            PlayerInventoryContainerFormKey = GetFormKeyFromObject(GetPropertyValue(record, "PlayerInventoryContainer")),
            JailOutfitFormKey = GetFormKeyFromObject(GetPropertyValue(record, "JailOutfit")),
            CrimeArrest = GetPropertyNullableBool(crimeValues, "Arrest"),
            CrimeAttackOnSight = GetPropertyNullableBool(crimeValues, "AttackOnSight"),
            CrimeMurder = GetPropertyNullableInt(crimeValues, "Murder"),
            CrimeAssault = GetPropertyNullableInt(crimeValues, "Assault"),
            CrimeTrespass = GetPropertyNullableInt(crimeValues, "Trespass"),
            CrimePickpocket = GetPropertyNullableInt(crimeValues, "Pickpocket"),
            CrimeSteal = GetPropertyNullableInt(crimeValues, "Steal"),
            CrimeStealMult = GetPropertyNullableDouble(crimeValues, "StealMult"),
            CrimeEscape = GetPropertyNullableInt(crimeValues, "Escape"),
            CrimeWerewolf = GetPropertyNullableInt(crimeValues, "Werewolf"),
            CrimeUnknown = GetPropertyNullableInt(crimeValues, "Unknown"),
            VendorStartHour = GetPropertyNullableDouble(vendorValues, "StartHour"),
            VendorEndHour = GetPropertyNullableDouble(vendorValues, "EndHour"),
            VendorRadius = GetPropertyNullableInt(vendorValues, "Radius"),
            VendorBuysStolenItems = GetPropertyNullableBool(vendorValues, "BuysStolenItems"),
            VendorBuysNonStolenItems = GetPropertyNullableBool(vendorValues, "BuysNonStolenItems"),
            VendorBuySellEverythingNotInList = GetPropertyNullableBool(vendorValues, "BuySellEverythingNotInList"),
            VendorLocationMutagenObjectType = GetPropertyValue(vendorLocation, "MutagenObjectType")?.ToString(),
            VendorLocationType = GetPropertyValue(vendorLocationTarget, "Type")?.ToString(),
            VendorLocationLinkFormKey = GetFormKeyFromObject(GetPropertyValue(vendorLocationTarget, "Link")),
            Relations = GetFactionRelations(plugin, game, formKey, GetPropertyValue(record, "Relations")),
            Ranks = GetFactionRanks(plugin, game, formKey, GetPropertyValue(record, "Ranks")),
            Conditions = GetConditionRules(plugin, game, formKey, GetPropertyValue(record, "Conditions")),
            Keywords = GetRecordKeywords(plugin, RecordTypeCatalog.Faction.RecordID, formKey, GetPropertyValue(record, "Keyword") is null ? null : new[] { GetPropertyValue(record, "Keyword")! })
        }, record);
    }

    private static List<ClassPropertyDTO> GetClassProperties(PluginDTO plugin, SupportedGame game, FormKey formKey, object? properties)
    {
        return properties is not IEnumerable enumerable
            ? new List<ClassPropertyDTO>()
            : enumerable.Cast<object>().Select((property, propertyIndex) => new ClassPropertyDTO
            {
                Game = game,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(formKey),
                PropertyIndex = propertyIndex,
                ActorValueFormKey = GetFormKeyFromObject(GetPropertyValue(property, "ActorValue")),
                Value = GetPropertyNullableDouble(property, "Value"),
                ImportedAtUTC = DateTime.UtcNow
            }).ToList();
    }

    private static List<ClassWeightDTO> GetClassWeights(PluginDTO plugin, SupportedGame game, FormKey formKey, string weightType, object? weights)
    {
        return weights is not IEnumerable enumerable
            ? new List<ClassWeightDTO>()
            : enumerable.Cast<object>().Select((weight, weightIndex) => new ClassWeightDTO
            {
                Game = game,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(formKey),
                WeightType = weightType,
                WeightIndex = weightIndex,
                Key = GetPropertyValue(weight, "Key")?.ToString() ?? string.Empty,
                Value = GetPropertyNullableDouble(weight, "Value"),
                ImportedAtUTC = DateTime.UtcNow
            }).ToList();
    }

    private static List<FactionRelationDTO> GetFactionRelations(PluginDTO plugin, SupportedGame game, FormKey formKey, object? relations)
    {
        return relations is not IEnumerable enumerable
            ? new List<FactionRelationDTO>()
            : enumerable.Cast<object>().Select((relation, relationIndex) => new FactionRelationDTO
            {
                Game = game,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(formKey),
                RelationIndex = relationIndex,
                TargetFormKey = GetFormKeyFromObject(GetPropertyValue(relation, "Target")),
                Reaction = GetPropertyValue(relation, "Reaction")?.ToString(),
                ImportedAtUTC = DateTime.UtcNow
            }).ToList();
    }

    private static List<FactionRankDTO> GetFactionRanks(PluginDTO plugin, SupportedGame game, FormKey formKey, object? ranks)
    {
        return ranks is not IEnumerable enumerable
            ? new List<FactionRankDTO>()
            : enumerable.Cast<object>().Select((rank, rankIndex) => new FactionRankDTO
            {
                Game = game,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(formKey),
                RankIndex = rankIndex,
                RankNumber = GetPropertyNullableInt(rank, "Rank"),
                MaleTitle = GetTranslatedString(rank, "MaleTitle"),
                FemaleTitle = GetTranslatedString(rank, "FemaleTitle"),
                ImportedAtUTC = DateTime.UtcNow
            }).ToList();
    }

    private static List<ConditionFormConditionDTO> GetConditionRules(PluginDTO plugin, SupportedGame game, FormKey formKey, object? conditions)
    {
        if (conditions is not IEnumerable enumerable) return new List<ConditionFormConditionDTO>();
        var importedAtUTC = DateTime.UtcNow;
        return enumerable.Cast<object>().Select((condition, conditionIndex) =>
        {
            var data = GetPropertyValue(condition, "Data");
            var comparisonValue = GetPropertyValue(condition, "ComparisonValue");
            return new ConditionFormConditionDTO
            {
                Game = game,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(formKey),
                ConditionIndex = conditionIndex,
                MutagenObjectType = condition.GetType().Name,
                DataMutagenObjectType = data?.GetType().Name,
                CompareOperator = GetPropertyValue(condition, "CompareOperator")?.ToString(),
                ComparisonValue = FormatConditionValue(comparisonValue),
                ComparisonValueFormKey = GetFormKeyFromObject(comparisonValue),
                ImportedAtUTC = importedAtUTC,
                Parameters = GetConditionRuleParameters(plugin, game, formKey, conditionIndex, data, importedAtUTC)
            };
        }).ToList();
    }

    private static List<ConditionFormConditionParameterDTO> GetConditionRuleParameters(PluginDTO plugin, SupportedGame game, FormKey formKey, int conditionIndex, object? data, DateTime importedAtUTC)
    {
        return data?.GetType().GetProperties()
            .Select(property => new ConditionFormConditionParameterDTO
            {
                Game = game,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(formKey),
                ConditionIndex = conditionIndex,
                ParameterName = property.Name,
                ParameterValue = FormatConditionValue(property.GetValue(data)),
                ParameterFormKey = GetFormKeyFromObject(property.GetValue(data)),
                ImportedAtUTC = importedAtUTC
            })
            .Where(parameter => parameter.ParameterValue is not null || parameter.ParameterFormKey is not null)
            .ToList() ?? new List<ConditionFormConditionParameterDTO>();
    }

    private static IReadOnlyList<MagicEffectDTO> MapMagicEffects(PluginDTO plugin, ISkyrimModGetter mod)
    {
        return GetRecordCollection(mod, "MagicEffects")
            .Select(record => LocalizedStringDTOMapper.AddLocalizedStrings(new MagicEffectDTO
            {
                Game = SupportedGame.Skyrim,
                ModKey = plugin.ModKey,
                FormKey = GetRequiredFormKey(record),
                EditorID = GetPropertyString(record, "EditorID"),
                FormVersion = GetPropertyInt(record, "FormVersion"),
                MajorRecordFlags = GetPropertyInt(record, "SkyrimMajorRecordFlags"),
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                ImportedAtUTC = DateTime.UtcNow,
                Name = GetTranslatedString(record, "Name"),
                Description = GetTranslatedString(record, "Description"),
                Flags = GetPropertyString(record, "Flags"),
                CastType = GetPropertyStringOrNull(record, "CastType"),
                TargetType = GetPropertyStringOrNull(record, "TargetType"),
                ActorValue2FormKey = GetLinkedFormKey(record, "ActorValue2"),
                ResistValueFormKey = GetLinkedFormKey(record, "ResistValue"),
                PerkToApplyFormKey = GetLinkedFormKey(record, "PerkToApply"),
                EquipAbilityFormKey = GetLinkedFormKey(record, "EquipAbility"),
                ExplosionFormKey = GetLinkedFormKey(record, "Explosion"),
                CastingArtFormKey = GetLinkedFormKey(record, "CastingArt"),
                HitEffectArtFormKey = GetLinkedFormKey(record, "HitEffectArt"),
                HitShaderFormKey = GetLinkedFormKey(record, "HitShader"),
                ImageSpaceModifierFormKey = GetLinkedFormKey(record, "ImageSpaceModifier"),
                ImpactDataFormKey = GetLinkedFormKey(record, "ImpactData"),
                ProjectileFormKey = GetLinkedFormKey(record, "Projectile"),
                Archetype = GetMagicEffectArchetype(record),
                UnknownFloat3 = GetPropertyNullableFloat(record, "UnknownFloat3"),
                UnknownInt2 = GetPropertyNullableInt(record, "UnknownInt2"),
                Unknown = FormatHexValue(GetPropertyValue(record, "Unknown")),
                Unknown2 = FormatHexValue(GetPropertyValue(record, "Unknown2")),
                DataTypeState = GetPropertyStringOrNull(record, "DATADataTypeState"),
                Keywords = GetRecordKeywords(plugin, RecordTypeCatalog.MagicEffect.RecordID, GetRequiredRawFormKey(record), GetPropertyValue(record, "Keywords")),
                Sounds = GetIndexedSounds(plugin, RecordTypeCatalog.MagicEffect.RecordID, GetRequiredRawFormKey(record), record),
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.MagicEffect.RecordID, record)
            }, record))
            .ToList();
    }

    private static IReadOnlyList<PerkDTO> MapPerks(PluginDTO plugin, ISkyrimModGetter mod)
    {
        return GetRecordCollection(mod, "Perks")
            .Select(record => LocalizedStringDTOMapper.AddLocalizedStrings(new PerkDTO
            {
                Game = SupportedGame.Skyrim,
                ModKey = plugin.ModKey,
                FormKey = GetRequiredFormKey(record),
                EditorID = GetPropertyString(record, "EditorID"),
                FormVersion = GetPropertyInt(record, "FormVersion"),
                MajorRecordFlags = GetPropertyInt(record, "SkyrimMajorRecordFlags"),
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                ImportedAtUTC = DateTime.UtcNow,
                Name = GetTranslatedString(record, "Name"),
                Description = GetTranslatedString(record, "Description"),
                Flags = GetPropertyString(record, "Flags"),
                SkillGroup = GetPropertyStringOrNull(record, "SkillGroup"),
                CrewAssignment = GetPropertyStringOrNull(record, "CrewAssignment"),
                PerkIcon = GetPropertyStringOrNull(record, "PerkIcon"),
                Category = GetPropertyStringOrNull(record, "Categroy") ?? GetPropertyStringOrNull(record, "Category"),
                RestrictionFormKey = GetLinkedFormKey(record, "Restriction"),
                TrainingFormKey = GetLinkedFormKey(record, "Training"),
                MajorFlags = GetPropertyStringOrNull(record, "MajorFlags"),
                Ranks = GetPerkRanks(plugin, record),
                BackgroundSkills = GetPerkBackgroundSkills(plugin, record),
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.Perk.RecordID, record)
            }, record))
            .ToList();
    }

    private static IReadOnlyList<StaticDTO> MapStatics(PluginDTO plugin, ISkyrimModGetter mod)
    {
        return GetRecordCollection(mod, "Statics")
            .Select(record => new StaticDTO
            {
                Game = SupportedGame.Skyrim,
                ModKey = plugin.ModKey,
                FormKey = GetRequiredFormKey(record),
                EditorID = GetPropertyString(record, "EditorID"),
                FormVersion = GetPropertyInt(record, "FormVersion"),
                MajorRecordFlags = GetPropertyInt(record, "SkyrimMajorRecordFlags"),
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                ImportedAtUTC = DateTime.UtcNow,
                ObjectBoundsFirst = FormatObjectBoundsPoint(GetPropertyValue(record, "ObjectBounds"), "First"),
                ObjectBoundsSecond = FormatObjectBoundsPoint(GetPropertyValue(record, "ObjectBounds"), "Second"),
                MaxAngle = GetPropertyNullableDouble(record, "MaxAngle"),
                Unused = FormatEnumerable(GetPropertyValue(record, "Unused")),
                DNAMDataTypeState = FormatEnumerable(GetPropertyValue(record, "DNAMDataTypeState")),
                Models = GetModels(plugin, RecordTypeCatalog.Static.RecordID, GetRequiredRawFormKey(record), GetPropertyValue(record, "Model")),
                RawPayloads = GetModelRawPayloads(plugin, RecordTypeCatalog.Static.RecordID, GetRequiredRawFormKey(record), GetPropertyValue(record, "Model"))
            })
            .ToList();
    }

    private static IReadOnlyList<ContainerDTO> MapContainers(PluginDTO plugin, ISkyrimModGetter mod)
    {
        return GetRecordCollection(mod, "Containers")
            .Select(record => LocalizedStringDTOMapper.AddLocalizedStrings(new ContainerDTO
            {
                Game = SupportedGame.Skyrim,
                ModKey = plugin.ModKey,
                FormKey = GetRequiredFormKey(record),
                EditorID = GetPropertyString(record, "EditorID"),
                FormVersion = GetPropertyInt(record, "FormVersion"),
                MajorRecordFlags = GetPropertyInt(record, "SkyrimMajorRecordFlags"),
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                ImportedAtUTC = DateTime.UtcNow,
                ObjectBoundsFirst = FormatObjectBoundsPoint(GetPropertyValue(record, "ObjectBounds"), "First"),
                ObjectBoundsSecond = FormatObjectBoundsPoint(GetPropertyValue(record, "ObjectBounds"), "Second"),
                Name = GetTranslatedString(record, "Name"),
                Flags = FormatEnumerable(GetPropertyValue(record, "Flags")),
                MajorFlags = GetPropertyStringOrNull(record, "MajorFlags"),
                Items = GetContainerItems(plugin, GetRequiredRawFormKey(record), GetPropertyValue(record, "Items")),
                Models = GetModels(plugin, RecordTypeCatalog.Container.RecordID, GetRequiredRawFormKey(record), GetPropertyValue(record, "Model")),
                Keywords = GetRecordKeywords(plugin, RecordTypeCatalog.Container.RecordID, GetRequiredRawFormKey(record), GetPropertyValue(record, "Keywords")),
                Sounds = GetNamedSounds(plugin, RecordTypeCatalog.Container.RecordID, GetRequiredRawFormKey(record), record, "OpenSound", "CloseSound"),
                RawPayloads = GetModelRawPayloads(plugin, RecordTypeCatalog.Container.RecordID, GetRequiredRawFormKey(record), GetPropertyValue(record, "Model"))
            }, record))
            .ToList();
    }

    private static IReadOnlyList<ConstructibleObjectDTO> MapConstructibleObjects(PluginDTO plugin, ISkyrimModGetter mod)
    {
        return GetRecordCollection(mod, "ConstructibleObjects")
            .Select(record => LocalizedStringDTOMapper.AddLocalizedStrings(new ConstructibleObjectDTO
            {
                Game = SupportedGame.Skyrim,
                ModKey = plugin.ModKey,
                FormKey = GetRequiredFormKey(record),
                EditorID = GetPropertyString(record, "EditorID"),
                FormVersion = GetPropertyInt(record, "FormVersion"),
                MajorRecordFlags = GetPropertyInt(record, "SkyrimMajorRecordFlags"),
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                ImportedAtUTC = DateTime.UtcNow,
                Description = GetTranslatedString(record, "Description"),
                CreatedObjectFormKey = GetLinkedFormKey(record, "CreatedObject"),
                WorkbenchKeywordFormKey = GetLinkedFormKey(record, "WorkbenchKeyword"),
                CreatedObjectCount = GetPropertyNullableInt(record, "CreatedObjectCount"),
                Components = GetConstructibleObjectComponents(plugin, GetRequiredRawFormKey(record), GetPropertyValue(record, "Items")),
                Conditions = GetConditionRules(plugin, SupportedGame.Skyrim, GetRequiredRawFormKey(record), GetPropertyValue(record, "Conditions")),
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.ConstructibleObject.RecordID, record)
            }, record))
            .ToList();
    }

    private static List<PerkRankDTO> GetPerkRanks(PluginDTO plugin, object record)
    {
        var ranks = GetPropertyValue(record, "Ranks") as IEnumerable;
        if (ranks == null) return new List<PerkRankDTO>();

        var formKey = GetRequiredRawFormKey(record);
        var importedAtUTC = DateTime.UtcNow;
        return ranks
            .Cast<object>()
            .Select((rank, rankIndex) => new PerkRankDTO
            {
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(formKey),
                RankIndex = rankIndex,
                Description = GetTranslatedString(rank, "Description"),
                UnknownStaticFormKey = GetLinkedFormKey(rank, "UnknownStatic"),
                ConditionCount = GetEnumerableCount(GetPropertyValue(rank, "Conditions")),
                ActivityCount = GetEnumerableCount(GetPropertyValue(rank, "Activities")),
                ImportedAtUTC = importedAtUTC,
                Effects = GetPerkRankEffects(plugin, formKey, rank, rankIndex, importedAtUTC)
            })
            .ToList();
    }

    private static List<PerkRankEffectDTO> GetPerkRankEffects(PluginDTO plugin, FormKey formKey, object rank, int rankIndex, DateTime importedAtUTC)
    {
        return (GetPropertyValue(rank, "Effects") as IEnumerable)?.Cast<object>()
            .Select((effect, effectIndex) => new PerkRankEffectDTO
            {
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(formKey),
                RankIndex = rankIndex,
                EffectIndex = effectIndex,
                MutagenObjectType = effect.GetType().Name,
                Rank = GetPropertyInt(effect, "Rank"),
                Priority = GetPropertyInt(effect, "Priority"),
                PerkEntryId = GetPropertyNullableInt(effect, "PerkEntryID"),
                Flags = GetPropertyStringOrNull(effect, "Flags"),
                ButtonLabel = GetTranslatedString(effect, "ButtonLabel"),
                ConditionCount = GetEnumerableCount(GetPropertyValue(effect, "Conditions")),
                EntryPoint = GetPropertyStringOrNull(effect, "EntryPoint"),
                PerkConditionTabCount = GetPropertyNullableInt(effect, "PerkConditionTabCount"),
                Modification = GetPropertyStringOrNull(effect, "Modification"),
                Value = GetPropertyNullableDouble(effect, "Value"),
                ImportedAtUTC = importedAtUTC
            })
            .ToList() ?? new List<PerkRankEffectDTO>();
    }

    private static List<PerkBackgroundSkillDTO> GetPerkBackgroundSkills(PluginDTO plugin, object record)
    {
        var backgroundSkills = GetPropertyValue(record, "BackgroundSkills") as IEnumerable;
        if (backgroundSkills == null) return new List<PerkBackgroundSkillDTO>();

        var formKey = GetRequiredRawFormKey(record);
        var importedAtUTC = DateTime.UtcNow;
        return backgroundSkills
            .Cast<object>()
            .Select((skill, skillIndex) => GetFormKeyFromObject(skill) is { } skillFormKey
                ? new PerkBackgroundSkillDTO
                {
                    ModKey = plugin.ModKey,
                    FormKey = MapFormKey(formKey),
                    SkillFormKey = skillFormKey,
                    SkillIndex = skillIndex,
                    ImportedAtUTC = importedAtUTC
                }
                : null)
            .Where(skill => skill != null)
            .Cast<PerkBackgroundSkillDTO>()
            .ToList();
    }

    private static List<ModelDTO> GetModels(PluginDTO plugin, string recordType, FormKey formKey, object? model)
    {
        if (model == null) return new List<ModelDTO>();

        var importedAtUTC = DateTime.UtcNow;
        return new List<ModelDTO>
        {
            new ModelDTO
            {
                Game = SupportedGame.Skyrim,
                ModKey = plugin.ModKey,
                RecordType = recordType,
                FormKey = MapFormKey(formKey),
                ModelSlot = "Model",
                ModelGender = string.Empty,
                  File = GetPropertyValue(model, "File")?.ToString(),
                TextureFileHashes = FormatHexValue(GetPropertyValue(model, "TextureFileHashes")),
                LightLayer = GetPropertyNullableUInt(model, "LightLayer"),
                Flags = GetPropertyStringOrNull(model, "Flags"),
                ColorRemappingIndex = GetPropertyNullableFloat(model, "ColorRemappingIndex"),
                FlagsVestigial = GetPropertyStringOrNull(model, "FlagsVestigial"),
                ImportedAtUTC = importedAtUTC,
                MaterialSwaps = GetModelMaterialSwaps(plugin, recordType, formKey, model, importedAtUTC)
            }
        };
    }

    private static List<ModelMaterialSwapDTO> GetModelMaterialSwaps(PluginDTO plugin, string recordType, FormKey formKey, object model, DateTime importedAtUTC)
    {
        return (GetPropertyValue(model, "MaterialSwaps") as IEnumerable)?.Cast<object>()
            .Select((materialSwap, materialSwapIndex) => GetFormKeyFromObject(materialSwap) is { } materialSwapFormKey
                ? new ModelMaterialSwapDTO
                {
                    Game = SupportedGame.Skyrim,
                    ModKey = plugin.ModKey,
                    RecordType = recordType,
                    FormKey = MapFormKey(formKey),
                    ModelSlot = "Model",
                    ModelGender = string.Empty,
                    MaterialSwapFormKey = materialSwapFormKey,
                    MaterialSwapIndex = materialSwapIndex,
                    ImportedAtUTC = importedAtUTC
                }
                : null)
            .Where(materialSwap => materialSwap != null)
            .Cast<ModelMaterialSwapDTO>()
            .ToList() ?? new List<ModelMaterialSwapDTO>();
    }

    private static List<RecordKeywordDTO> GetRecordKeywords(PluginDTO plugin, string recordType, FormKey formKey, object? keywords)
    {
        if (keywords is not IEnumerable enumerable) return new List<RecordKeywordDTO>();

        var importedAtUTC = DateTime.UtcNow;
        return enumerable
            .Cast<object>()
            .Select((keyword, keywordIndex) => GetFormKeyFromObject(keyword) is { } keywordFormKey
                ? new RecordKeywordDTO
                {
                    Game = SupportedGame.Skyrim,
                    ModKey = plugin.ModKey,
                    RecordType = recordType,
                    FormKey = MapFormKey(formKey),
                    KeywordFormKey = keywordFormKey,
                    KeywordIndex = keywordIndex,
                    ImportedAtUTC = importedAtUTC
                }
                : null)
            .Where(keyword => keyword != null)
            .Cast<RecordKeywordDTO>()
            .ToList();
    }

    private static List<ContainerItemDTO> GetContainerItems(PluginDTO plugin, FormKey formKey, object? items)
    {
        if (items is not IEnumerable enumerable) return new List<ContainerItemDTO>();

        var importedAtUTC = DateTime.UtcNow;
        return enumerable
            .Cast<object>()
            .Select((item, itemIndex) => CreateContainerItem(plugin, formKey, item, itemIndex, importedAtUTC))
            .Where(item => item != null)
            .Cast<ContainerItemDTO>()
            .ToList();
    }

    private static ContainerItemDTO? CreateContainerItem(PluginDTO plugin, FormKey formKey, object item, int itemIndex, DateTime importedAtUTC)
    {
        var itemData = GetPropertyValue(item, "Item") ?? item;
        var itemFormKey = GetFormKeyFromObject(GetPropertyValue(itemData, "Item")) ?? GetFormKeyFromObject(itemData);
        if (itemFormKey == null)
        {
            return null;
        }

        return new ContainerItemDTO
        {
            Game = SupportedGame.Skyrim,
            ModKey = plugin.ModKey,
            FormKey = MapFormKey(formKey),
            ItemIndex = itemIndex,
            ItemFormKey = itemFormKey,
            Count = GetPropertyNullableInt(item, "Count") ?? GetPropertyNullableInt(itemData, "Count"),
            ImportedAtUTC = importedAtUTC
        };
    }

    private static List<ConstructibleObjectComponentDTO> GetConstructibleObjectComponents(PluginDTO plugin, FormKey formKey, object? components)
    {
        if (components is not IEnumerable enumerable) return new List<ConstructibleObjectComponentDTO>();

        var importedAtUTC = DateTime.UtcNow;
        return enumerable
            .Cast<object>()
            .Select((component, componentIndex) => CreateConstructibleObjectComponent(plugin, formKey, component, componentIndex, importedAtUTC))
            .Where(component => component != null)
            .Cast<ConstructibleObjectComponentDTO>()
            .ToList();
    }

    private static ConstructibleObjectComponentDTO? CreateConstructibleObjectComponent(PluginDTO plugin, FormKey formKey, object component, int componentIndex, DateTime importedAtUTC)
    {
        var componentData = GetPropertyValue(component, "Item") ?? component;
        var componentFormKey = GetFormKeyFromObject(GetPropertyValue(componentData, "Item")) ?? GetFormKeyFromObject(GetPropertyValue(componentData, "Component")) ?? GetFormKeyFromObject(componentData);
        if (componentFormKey == null)
        {
            return null;
        }

        return new ConstructibleObjectComponentDTO
        {
            Game = SupportedGame.Skyrim,
            ModKey = plugin.ModKey,
            FormKey = MapFormKey(formKey),
            ComponentFormKey = componentFormKey,
            ComponentIndex = componentIndex,
            Count = GetPropertyNullableInt(component, "Count") ?? GetPropertyNullableInt(componentData, "Count") ?? GetPropertyNullableInt(component, "RequiredCount"),
            ImportedAtUTC = importedAtUTC
        };
    }

    private static List<RawRecordPayloadDTO> GetModelRawPayloads(PluginDTO plugin, string recordType, FormKey formKey, object? model)
    {
        var payloads = new List<RawRecordPayloadDTO>();
        var payloadValue = FormatHexValue(GetPropertyValue(model, "Data"));
        if (string.IsNullOrWhiteSpace(payloadValue))
        {
            return payloads;
        }

        payloads.Add(new RawRecordPayloadDTO
        {
            Game = SupportedGame.Skyrim,
            ModKey = plugin.ModKey,
            RecordType = recordType,
            FormKey = MapFormKey(formKey),
            PayloadSlot = "Model.Data",
            PayloadIndex = 0,
            PayloadType = model?.GetType().Name ?? "Model",
            SourcePath = "Model.Data",
            PayloadValue = payloadValue,
            ImportedAtUTC = DateTime.UtcNow
        });
        return payloads;
    }

    private static void AddRawRecordPayload(
        ICollection<RawRecordPayloadDTO> payloads,
        PluginDTO plugin,
        string recordType,
        FormKey formKey,
        string payloadSlot,
        int payloadIndex,
        string payloadType,
        string? payloadValue,
        DateTime importedAtUTC)
    {
        if (string.IsNullOrWhiteSpace(payloadValue))
        {
            return;
        }

        payloads.Add(new RawRecordPayloadDTO
        {
            Game = SupportedGame.Skyrim,
            ModKey = plugin.ModKey,
            RecordType = recordType,
            FormKey = MapFormKey(formKey),
            PayloadSlot = payloadSlot,
            PayloadIndex = payloadIndex,
            PayloadType = payloadType,
            SourcePath = payloadSlot,
            PayloadValue = payloadValue,
            ImportedAtUTC = importedAtUTC
        });
    }

    private static List<RecordSoundDTO> GetNamedSounds(PluginDTO plugin, string recordType, FormKey formKey, object record, params string[] soundSlots)
    {
        var importedAtUTC = DateTime.UtcNow;
        return soundSlots
            .Select((soundSlot, soundIndex) => CreateRecordSound(plugin, recordType, formKey, soundSlot, soundIndex, GetPropertyValue(record, soundSlot), importedAtUTC))
            .Where(sound => sound != null)
            .Cast<RecordSoundDTO>()
            .ToList();
    }

    private static List<RecordSoundDTO> GetIndexedSounds(PluginDTO plugin, string recordType, FormKey formKey, object record)
    {
        var sounds = GetPropertyValue(record, "Sounds") as IEnumerable;
        if (sounds == null) return new List<RecordSoundDTO>();

        var importedAtUTC = DateTime.UtcNow;
        return sounds
            .Cast<object>()
            .Select((sound, soundIndex) => CreateRecordSound(plugin, recordType, formKey, GetPropertyValue(sound, "Type")?.ToString() ?? $"Sound [{soundIndex}]", soundIndex, sound, importedAtUTC))
            .Where(sound => sound != null)
            .Cast<RecordSoundDTO>()
            .ToList();
    }

    private static RecordSoundDTO? CreateRecordSound(PluginDTO plugin, string recordType, FormKey formKey, string soundSlot, int soundIndex, object? soundSource, DateTime importedAtUTC)
    {
        if (soundSource == null) return null;

        var start = GetSoundStart(soundSource);
        if (string.IsNullOrWhiteSpace(start)) return null;

        return new RecordSoundDTO
        {
            Game = SupportedGame.Skyrim,
            ModKey = plugin.ModKey,
            RecordType = recordType,
            FormKey = MapFormKey(formKey),
            SoundSlot = soundSlot,
            SoundIndex = soundIndex,
            Start = start,
            Versioning = FormatEnumerable(GetPropertyValue(soundSource, "Versioning")),
            Unknown = FormatHexValue(GetPropertyValue(soundSource, "Unknown")),
            ImportedAtUTC = importedAtUTC
        };
    }

    private static string? GetSoundStart(object soundSource)
    {
        var directStart = GetPropertyValue(soundSource, "Start")?.ToString();
        if (!string.IsNullOrWhiteSpace(directStart)) return directStart;

        var sound = GetPropertyValue(soundSource, "Sound");
        return sound == null ? null : GetPropertyValue(sound, "Start")?.ToString();
    }

    private static List<ScriptingAdapterDTO> GetScriptingAdapters(PluginDTO plugin, string recordType, object record)
    {
        var virtualMachineAdapter = GetPropertyValue(record, "VirtualMachineAdapter");
        var scripts = GetPropertyValue(virtualMachineAdapter, "Scripts") as IEnumerable;
        if (scripts == null) return new List<ScriptingAdapterDTO>();

        var formKey = GetRequiredRawFormKey(record);
        var importedAtUTC = DateTime.UtcNow;
        return scripts
            .Cast<object>()
            .Select((script, scriptIndex) => new ScriptingAdapterDTO
            {
                Game = SupportedGame.Skyrim,
                ModKey = plugin.ModKey,
                RecordType = recordType,
                FormKey = MapFormKey(formKey),
                Name = GetPropertyString(script, "Name"),
                ScriptIndex = scriptIndex,
                ImportedAtUTC = importedAtUTC,
                Properties = GetScriptingAdapterProperties(plugin, recordType, formKey, script, importedAtUTC)
            })
            .ToList();
    }

    private static List<ScriptingAdapterPropertyDTO> GetScriptingAdapterProperties(PluginDTO plugin, string recordType, FormKey formKey, object script, DateTime importedAtUTC)
    {
        return (GetPropertyValue(script, "Properties") as IEnumerable)?.Cast<object>()
            .Select((property, propertyIndex) => CreateScriptingAdapterProperty(plugin, recordType, formKey, GetPropertyString(script, "Name"), property, propertyIndex, importedAtUTC))
            .Where(property => property != null)
            .Cast<ScriptingAdapterPropertyDTO>()
            .ToList() ?? new List<ScriptingAdapterPropertyDTO>();
    }

    private static ScriptingAdapterPropertyDTO? CreateScriptingAdapterProperty(PluginDTO plugin, string recordType, FormKey formKey, string scriptName, object property, int propertyIndex, DateTime importedAtUTC)
    {
        var dto = new ScriptingAdapterPropertyDTO
        {
            Game = SupportedGame.Skyrim,
            ModKey = plugin.ModKey,
            RecordType = recordType,
            FormKey = MapFormKey(formKey),
            ScriptingAdapterName = scriptName,
            PropertyIndex = propertyIndex,
            Name = GetPropertyString(property, "Name"),
            MutagenObjectType = property.GetType().Name,
            ImportedAtUTC = importedAtUTC
        };

        var typeName = property.GetType().Name;
        if (typeName.Contains("BoolList", StringComparison.OrdinalIgnoreCase))
        {
            dto.ListItems = GetScriptingAdapterPropertyListItems(plugin, recordType, formKey, scriptName, propertyIndex, property, importedAtUTC, nameof(Boolean));
            return dto;
        }

        if (typeName.Contains("IntList", StringComparison.OrdinalIgnoreCase))
        {
            dto.ListItems = GetScriptingAdapterPropertyListItems(plugin, recordType, formKey, scriptName, propertyIndex, property, importedAtUTC, nameof(Int32));
            return dto;
        }

        if (typeName.Contains("FloatList", StringComparison.OrdinalIgnoreCase))
        {
            dto.ListItems = GetScriptingAdapterPropertyListItems(plugin, recordType, formKey, scriptName, propertyIndex, property, importedAtUTC, nameof(Single));
            return dto;
        }

        if (typeName.Contains("StringList", StringComparison.OrdinalIgnoreCase))
        {
            dto.ListItems = GetScriptingAdapterPropertyListItems(plugin, recordType, formKey, scriptName, propertyIndex, property, importedAtUTC, nameof(String));
            return dto;
        }

        if (typeName.Contains("ObjectList", StringComparison.OrdinalIgnoreCase))
        {
            dto.ListItems = GetScriptingAdapterObjectPropertyListItems(plugin, recordType, formKey, scriptName, propertyIndex, property, importedAtUTC);
            return dto;
        }

        if (typeName.Contains("Bool", StringComparison.OrdinalIgnoreCase)) dto.DataBool = GetPropertyValue(property, "Data") as bool?;
        else if (typeName.Contains("Int", StringComparison.OrdinalIgnoreCase)) dto.DataInt = GetPropertyNullableInt(property, "Data");
        else if (typeName.Contains("Float", StringComparison.OrdinalIgnoreCase)) dto.DataFloat = GetPropertyNullableDouble(property, "Data");
        else if (typeName.Contains("String", StringComparison.OrdinalIgnoreCase)) dto.DataString = GetPropertyStringOrNull(property, "Data");
        else if (typeName.Contains("Object", StringComparison.OrdinalIgnoreCase))
        {
            var objectValue = GetPropertyValue(property, "Object");
            dto.ObjectFormKey = GetFormKeyFromObject(objectValue);
            dto.ObjectAlias = GetPropertyNullableShort(property, "Alias");
            dto.ObjectUnused = GetPropertyNullableUShort(property, "Unused");
        }

        return dto;
    }

    private static List<ScriptingAdapterPropertyListItemDTO> GetScriptingAdapterPropertyListItems(PluginDTO plugin, string recordType, FormKey formKey, string scriptName, int propertyIndex, object property, DateTime importedAtUTC, string mutagenObjectType)
    {
        var data = GetPropertyValue(property, "Data") as IEnumerable;
        if (data == null) return new List<ScriptingAdapterPropertyListItemDTO>();

        return data
            .Cast<object>()
            .Select((value, listItemIndex) => new ScriptingAdapterPropertyListItemDTO
            {
                Game = SupportedGame.Skyrim,
                ModKey = plugin.ModKey,
                RecordType = recordType,
                FormKey = MapFormKey(formKey),
                ScriptingAdapterName = scriptName,
                PropertyIndex = propertyIndex,
                ListItemIndex = listItemIndex,
                MutagenObjectType = mutagenObjectType,
                DataBool = value is bool boolValue ? boolValue : null,
                DataInt = value is int intValue ? intValue : null,
                DataFloat = value is float or double or decimal ? Convert.ToDouble(value, CultureInfo.InvariantCulture) : null,
                DataString = value is string stringValue ? stringValue : null,
                ImportedAtUTC = importedAtUTC
            })
            .ToList();
    }

    private static List<ScriptingAdapterPropertyListItemDTO> GetScriptingAdapterObjectPropertyListItems(PluginDTO plugin, string recordType, FormKey formKey, string scriptName, int propertyIndex, object property, DateTime importedAtUTC)
    {
        var objects = GetPropertyValue(property, "Objects") as IEnumerable;
        if (objects == null) return new List<ScriptingAdapterPropertyListItemDTO>();

        return objects
            .Cast<object>()
            .Select((value, listItemIndex) => new ScriptingAdapterPropertyListItemDTO
            {
                Game = SupportedGame.Skyrim,
                ModKey = plugin.ModKey,
                RecordType = recordType,
                FormKey = MapFormKey(formKey),
                ScriptingAdapterName = scriptName,
                PropertyIndex = propertyIndex,
                ListItemIndex = listItemIndex,
                MutagenObjectType = value.GetType().Name,
                ObjectFormKey = GetFormKeyFromObject(GetPropertyValue(value, "Object")),
                ObjectAlias = GetPropertyNullableShort(value, "Alias"),
                ObjectUnused = GetPropertyNullableUShort(value, "Unused"),
                ImportedAtUTC = importedAtUTC
            })
            .ToList();
    }

    protected virtual ISkyrimModGetter LoadMod(PluginDTO plugin)
    {
        var dataFolderPath = GetDataFolderPath();
        return SkyrimMod.Create(SkyrimRelease.SkyrimSE)
            .FromPath(Path.Combine(dataFolderPath, plugin.ModKey.FileName))
            .WithDataFolder(dataFolderPath)
            .Construct();
    }

    private string GetDataFolderPath()
    {
        var environment = GameEnvironment.Typical.Skyrim(SkyrimRelease.SkyrimSE);
        return environment.DataFolderPath;
    }

    private static FormKeyDTO MapFormKey(FormKey formKey)
    {
        return new FormKeyDTO
        {
            ModKey = ModKeyDTOMapper.FromModKey(formKey.ModKey),
            Id = formKey.ID
        };
    }

    private static FormKey GetRequiredRawFormKey(object record)
    {
        return GetPropertyValue(record, "FormKey") is FormKey formKey
            ? formKey
            : throw new InvalidOperationException($"Record type {record.GetType().Name} did not expose a FormKey.");
    }

    private static FormKeyDTO GetRequiredFormKey(object record)
    {
        return MapFormKey(GetRequiredRawFormKey(record));
    }

    private static FormKeyDTO? GetLinkedFormKey(object source, string propertyName)
    {
        return GetFormKeyFromObject(GetPropertyValue(source, propertyName));
    }

    private static FormKeyDTO? GetFormKeyFromObject(object? value)
    {
        return GetFormKeyFromObject(value, 0);
    }

    private static FormKeyDTO? GetFormKeyFromObject(object? value, int depth)
    {
        if (value == null) return null;
        if (value is FormKey formKey) return MapFormKey(formKey);
        if (value is string) return null;
        if (depth > 2) return null;
        if (GetPropertyValue(value, "IsNull") is bool isNull && isNull) return null;
        if (GetPropertyValue(value, "FormKey") is FormKey linkedFormKey) return MapFormKey(linkedFormKey);
        if (GetPropertyValue(value, "FormKeyNullable") is FormKey nullableFormKey) return MapFormKey(nullableFormKey);
        foreach (var propertyName in new[] { "FormKeyOrIndex", "FormLinkOrIndex", "LinkOrIndex", "FormLinkGetter", "FormLink", "Link", "Value", "Reference", "Target", "Object", "Item" })
        {
            if (GetFormKeyFromObject(GetPropertyValue(value, propertyName), depth + 1) is { } nestedFormKey)
            {
                return nestedFormKey;
            }
        }

        return null;
    }

    private static IEnumerable<object> GetRecordCollection(object mod, params string[] propertyNames)
    {
        foreach (var propertyName in propertyNames)
        {
            if (GetPropertyValue(mod, propertyName) is IEnumerable records)
            {
                return records.Cast<object>();
            }
        }

        return Enumerable.Empty<object>();
    }

    private static object? GetPropertyValue(object? source, string propertyName)
    {
        if (source == null)
        {
            return null;
        }

        var sourceType = source.GetType();
        var property = sourceType.GetProperty(propertyName);
        if (property != null)
        {
            return property.GetValue(source);
        }

        foreach (var interfaceType in sourceType.GetInterfaces())
        {
            property = interfaceType.GetProperty(propertyName);
            if (property != null)
            {
                return property.GetValue(source);
            }
        }

        return null;
    }

    private static string GetPropertyString(object source, string propertyName)
    {
        return GetPropertyValue(source, propertyName)?.ToString() ?? string.Empty;
    }

    private static string? GetPropertyStringOrNull(object source, string propertyName)
    {
        return GetPropertyValue(source, propertyName)?.ToString();
    }

    private static int GetPropertyInt(object source, string propertyName)
    {
        var value = GetPropertyValue(source, propertyName);
        return value == null ? 0 : Convert.ToInt32(value, CultureInfo.InvariantCulture);
    }

    private static int? GetPropertyNullableInt(object? source, string propertyName)
    {
        var value = GetPropertyValue(source, propertyName);
        return value == null ? null : Convert.ToInt32(value, CultureInfo.InvariantCulture);
    }

    private static short? GetPropertyNullableShort(object source, string propertyName)
    {
        var value = GetPropertyValue(source, propertyName);
        return value == null ? null : Convert.ToInt16(value, CultureInfo.InvariantCulture);
    }

    private static ushort? GetPropertyNullableUShort(object source, string propertyName)
    {
        var value = GetPropertyValue(source, propertyName);
        return value == null ? null : Convert.ToUInt16(value, CultureInfo.InvariantCulture);
    }

    private static uint? GetPropertyNullableUInt(object source, string propertyName)
    {
        var value = GetPropertyValue(source, propertyName);
        return value == null ? null : Convert.ToUInt32(value, CultureInfo.InvariantCulture);
    }

    private static double GetPropertyDouble(object source, string propertyName)
    {
        var value = GetPropertyValue(source, propertyName);
        return value == null ? 0 : Convert.ToDouble(value, CultureInfo.InvariantCulture);
    }

    private static double? GetPropertyNullableDouble(object? source, string propertyName)
    {
        var value = GetPropertyValue(source, propertyName);
        return value == null ? null : Convert.ToDouble(value, CultureInfo.InvariantCulture);
    }

    private static bool? GetPropertyNullableBool(object? source, string propertyName)
    {
        var value = GetPropertyValue(source, propertyName);
        if (value is bool boolValue) return boolValue;
        return value == null ? null : Convert.ToBoolean(value, CultureInfo.InvariantCulture);
    }

    private static float? GetPropertyNullableFloat(object source, string propertyName)
    {
        var value = GetPropertyValue(source, propertyName);
        return value == null ? null : Convert.ToSingle(value, CultureInfo.InvariantCulture);
    }

    private static int GetEnumerableCount(object? value)
    {
        return value is IEnumerable enumerable ? enumerable.Cast<object>().Count() : 0;
    }

    private static TranslatedStringDTO? GetTranslatedString(object source, string propertyName)
    {
        var value = GetPropertyValue(source, propertyName);
        if (value == null) return null;

        try
        {
            return LocalizedStringDTOMapper.ToTranslatedStringDTO(value);
        }
        catch (ArgumentException)
        {
            return null;
        }
        catch (System.Reflection.TargetInvocationException)
        {
            return null;
        }
    }

    private static string? GetMagicEffectArchetype(object record)
    {
        var archetype = GetPropertyValue(record, "Archetype");
        var type = GetPropertyValue(archetype, "Type");
        return type == null ? null : Convert.ToInt64(type, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
    }

    private static string? FormatEnumerable(object? value)
    {
        if (value is string text) return text;
        return value is IEnumerable enumerable
            ? string.Join(", ", enumerable.Cast<object>().Select(item => item.ToString()))
            : value?.ToString();
    }

    private static string? FormatConditionValue(object? value)
    {
        if (value == null) return null;
        if (GetFormKeyFromObject(value) is { } formKey) return $"{formKey.ModKey.FileName}:{formKey.Id:X8}";
        if (value is string text) return text;
        if (value is byte[] bytes) return Convert.ToHexString(bytes);
        if (value is IEnumerable enumerable) return string.Join(", ", enumerable.Cast<object>().Select(item => FormatConditionValue(item) ?? string.Empty));
        return value.ToString();
    }

    private static string? FormatObjectBoundsPoint(object? objectBounds, string propertyName)
    {
        return GetPropertyValue(objectBounds, propertyName)?.ToString();
    }

    private static string? FormatHexValue(object? value)
    {
        if (value == null) return null;
        if (value is string text) return text;
        if (value is byte[] bytes) return Convert.ToHexString(bytes);

        var toArray = value.GetType().GetMethod("ToArray", Type.EmptyTypes);
        if (toArray?.Invoke(value, null) is byte[] arrayBytes)
        {
            return Convert.ToHexString(arrayBytes);
        }

        return value.ToString();
    }

    private static string GetGameSettingType(IGameSettingGetter record)
    {
        return record switch
        {
            IGameSettingBoolGetter => "GameSettingBool",
            IGameSettingFloatGetter => "GameSettingFloat",
            IGameSettingIntGetter => "GameSettingInt",
            IGameSettingStringGetter => "GameSettingString",
            _ => record.GetType().Name
        };
    }

    private static string? GetGameSettingData(IGameSettingGetter record)
    {
        return record switch
        {
            IGameSettingBoolGetter gameSetting => Convert.ToString(gameSetting.Data, CultureInfo.InvariantCulture),
            IGameSettingFloatGetter gameSetting => Convert.ToString(gameSetting.Data, CultureInfo.InvariantCulture),
            IGameSettingIntGetter gameSetting => Convert.ToString(gameSetting.Data, CultureInfo.InvariantCulture),
            IGameSettingStringGetter gameSetting => LocalizedStringDTOMapper.GetLocalizedText(gameSetting.Data, Language.English),
            _ => null
        };
    }

    private static double? GetGameSettingNumericData(IGameSettingGetter record)
    {
        return record switch
        {
            IGameSettingFloatGetter gameSetting => gameSetting.Data,
            IGameSettingIntGetter gameSetting => gameSetting.Data,
            _ => null
        };
    }

    private static int? GetGameSettingIntegerData(IGameSettingGetter record)
    {
        return record is IGameSettingIntGetter gameSetting ? gameSetting.Data : null;
    }

    private static bool? GetGameSettingBooleanData(IGameSettingGetter record)
    {
        return record is IGameSettingBoolGetter gameSetting ? gameSetting.Data : null;
    }

    private static double? GetGlobalData(IGlobalGetter record)
    {
        var rawFloat = record switch
        {
            GlobalFloat global => global.RawFloat,
            GlobalInt global => global.RawFloat,
            GlobalShort global => global.RawFloat,
            Global global => global.RawFloat,
            _ => null
        };

        if (rawFloat.HasValue) return rawFloat;

        var rawFloatProperty = record.GetType().GetProperty("RawFloat");
        return rawFloatProperty?.GetValue(record) is float value ? value : null;
    }
}
