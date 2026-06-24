using System.Globalization;
using System.Collections;
using System.Reflection;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Utilities;
using CreationsForge.Fallout4.Interfaces;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Fallout4;

public class Fallout4RecordReaderService : IFallout4RecordReaderService
{
    private readonly Fallout4GameMetadataService GameMetadataService;

    public Fallout4RecordReaderService(Fallout4GameMetadataService gameMetadataService)
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
        var miscItems = MapMiscItems(plugin, mod);
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
        var books = MapBooks(plugin, mod);
        cancellationToken.ThrowIfCancellationRequested();
        var doors = MapDoors(plugin, mod);
        cancellationToken.ThrowIfCancellationRequested();
        var containers = MapContainers(plugin, mod);
        cancellationToken.ThrowIfCancellationRequested();
        var constructibleObjects = MapConstructibleObjects(plugin, mod);
        cancellationToken.ThrowIfCancellationRequested();
        var terminals = MapTerminals(plugin, mod);

        return new PluginRecordSetDTO
        {
            FormLists = formLists,
            GameSettings = gameSettings,
            Globals = globals,
            Classes = classes,
            Factions = factions,
            MiscItems = miscItems,
            Keywords = keywords,
            ActorValueInformation = actorValueInformation,
            NPCs = npcs,
            MagicEffects = magicEffects,
            Perks = perks,
            Statics = statics,
            Books = books,
            Doors = doors,
            Containers = containers,
            ConstructibleObjects = constructibleObjects,
            Terminals = terminals
        };
    }

    public IReadOnlyList<FormListDTO> ReadFormLists(PluginDTO plugin)
    {
        var mod = LoadMod(plugin);
        return MapFormLists(plugin, mod);
    }

    private static IReadOnlyList<FormListDTO> MapFormLists(PluginDTO plugin, IFallout4ModGetter mod)
    {
        return mod.FormLists
            .Select(record => LocalizedStringDTOMapper.AddLocalizedStrings(new FormListDTO
            {
                Game = SupportedGame.Fallout4,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.Fallout4MajorRecordFlags,
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                Name = LocalizedStringDTOMapper.ToTranslatedStringDTO(GetPropertyValue(record, "Name")),
                ImportedAtUTC = DateTime.UtcNow,
                Items = record.Items.Select((item, itemIndex) => new FormListItemDTO
                {
                    Game = SupportedGame.Fallout4,
                    ModKey = plugin.ModKey,
                    FormKey = MapFormKey(record.FormKey),
                    Item = MapFormKey(item.FormKey),
                    ItemIndex = itemIndex,
                    ImportedAtUTC = DateTime.UtcNow
                }).ToList()
            }, record))
            .ToList();
    }

    public IReadOnlyList<GameSettingDTO> ReadGameSettings(PluginDTO plugin)
    {
        var mod = LoadMod(plugin);
        return MapGameSettings(plugin, mod);
    }

    private static IReadOnlyList<GameSettingDTO> MapGameSettings(PluginDTO plugin, IFallout4ModGetter mod)
    {
        return mod.GameSettings
            .Select(record => LocalizedStringDTOMapper.AddLocalizedStrings(new GameSettingDTO
            {
                Game = SupportedGame.Fallout4,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.Fallout4MajorRecordFlags,
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                ImportedAtUTC = DateTime.UtcNow,
                DataType = GetGameSettingDataType(record),
                Data = GetGameSettingData(record)
            }, record))
            .ToList();
    }

    public IReadOnlyList<GlobalDTO> ReadGlobals(PluginDTO plugin)
    {
        var mod = LoadMod(plugin);
        return MapGlobals(plugin, mod);
    }

    private static IReadOnlyList<GlobalDTO> MapGlobals(PluginDTO plugin, IFallout4ModGetter mod)
    {
        return mod.Globals
            .Select(record => new GlobalDTO
            {
                Game = SupportedGame.Fallout4,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.Fallout4MajorRecordFlags,
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                MutagenObjectType = GetSpriggitMutagenObjectType(record),
                MajorFlags = FormatEnumerable(GetPropertyValue(record, "MajorFlags")),
                ImportedAtUTC = DateTime.UtcNow,
                Data = GetGlobalData(record)
            })
            .ToList();
    }

    private static IReadOnlyList<KeywordDTO> MapKeywords(PluginDTO plugin, IFallout4ModGetter mod)
    {
        return GetRecordCollection(mod, "Keywords")
            .Select(record => LocalizedStringDTOMapper.AddLocalizedStrings(new KeywordDTO
            {
                Game = SupportedGame.Fallout4,
                ModKey = plugin.ModKey,
                FormKey = GetRequiredFormKey(record),
                EditorID = GetPropertyString(record, "EditorID"),
                FormVersion = GetPropertyInt(record, "FormVersion"),
                MajorRecordFlags = GetPropertyInt(record, "Fallout4MajorRecordFlags"),
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                ImportedAtUTC = DateTime.UtcNow,
                Name = GetTranslatedString(record, "Name"),
                Color = FormatSpriggitColor(GetPropertyValue(record, "Color")),
                Type = GetPropertyStringOrNull(record, "Type"),
                Notes = GetPropertyStringOrNull(record, "Notes"),
                FlashLinkageName = GetPropertyStringOrNull(record, "FlashLinkageName"),
                AttractionRule = GetLinkedFormKey(record, "AttractionRule"),
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.Keyword.RecordID, record)
            }, record))
            .ToList();
    }

    private static IReadOnlyList<ClassDTO> MapClasses(PluginDTO plugin, IFallout4ModGetter mod)
    {
        return GetRecordCollection(mod, "Classes")
            .Select(record => CreateClass(plugin, SupportedGame.Fallout4, record, "Fallout4MajorRecordFlags"))
            .ToList();
    }

    private static IReadOnlyList<FactionDTO> MapFactions(PluginDTO plugin, IFallout4ModGetter mod)
    {
        return GetRecordCollection(mod, "Factions")
            .Select(record => CreateFaction(plugin, SupportedGame.Fallout4, record, "Fallout4MajorRecordFlags"))
            .ToList();
    }

    private static IReadOnlyList<MiscItemDTO> MapMiscItems(PluginDTO plugin, IFallout4ModGetter mod)
    {
        return GetRecordCollection(mod, "MiscItems")
            .Select(record => LocalizedStringDTOMapper.AddLocalizedStrings(new MiscItemDTO
            {
                Game = SupportedGame.Fallout4,
                ModKey = plugin.ModKey,
                FormKey = GetRequiredFormKey(record),
                EditorID = GetPropertyString(record, "EditorID"),
                FormVersion = GetPropertyInt(record, "FormVersion"),
                MajorRecordFlags = GetPropertyInt(record, "Fallout4MajorRecordFlags"),
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                ImportedAtUTC = DateTime.UtcNow,
                ObjectBounds = new ObjectBoundsDTO
                {
                    First = FormatObjectBoundsPoint(GetPropertyValue(record, "ObjectBounds"), "First"),
                    Second = FormatObjectBoundsPoint(GetPropertyValue(record, "ObjectBounds"), "Second")
                },
                PreviewTransform = GetLinkedFormKey(record, "PreviewTransform"),
                Name = GetTranslatedString(record, "Name"),
                ShortName = GetTranslatedString(record, "ShortName"),
                Value = GetPropertyNullableInt(record, "Value"),
                Weight = GetPropertyNullableFloat(record, "Weight"),
                DirtinessScale = GetPropertyNullableFloat(record, "DirtinessScale"),
                FeaturedItemMessage = GetLinkedFormKey(record, "FeaturedItemMessage"),
                Flag = FormatHexValue(GetPropertyValue(record, "FLAG")),
                Destructible = GetMiscItemDestructible(record),
                Models = GetModels(plugin, RecordTypeCatalog.MiscItem.RecordID, GetRequiredRawFormKey(record), GetPropertyValue(record, "Model")),
                Keywords = GetKeywordMappings(plugin, RecordTypeCatalog.MiscItem.RecordID, GetRequiredRawFormKey(record), GetPropertyValue(record, "Keywords")),
                Sounds = GetNamedSounds(plugin, RecordTypeCatalog.MiscItem.RecordID, GetRequiredRawFormKey(record), record, "CraftingSound", "PickupSound", "PutdownSound", "DropdownSound"),
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.MiscItem.RecordID, record),
                Components = GetMiscItemComponents(plugin, GetRequiredRawFormKey(record), record)
            }, record))
            .ToList();
    }

    private static IReadOnlyList<ActorValueInformationDTO> MapActorValueInformation(PluginDTO plugin, IFallout4ModGetter mod)
    {
        return GetRecordCollection(mod, "ActorValueInformation", "ActorValues")
            .Select(record => LocalizedStringDTOMapper.AddLocalizedStrings(new ActorValueInformationDTO
            {
                Game = SupportedGame.Fallout4,
                ModKey = plugin.ModKey,
                FormKey = GetRequiredFormKey(record),
                EditorID = GetPropertyString(record, "EditorID"),
                FormVersion = GetPropertyInt(record, "FormVersion"),
                MajorRecordFlags = GetPropertyInt(record, "Fallout4MajorRecordFlags"),
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                ImportedAtUTC = DateTime.UtcNow,
                Name = GetTranslatedString(record, "Name"),
                Abbreviation = GetTranslatedString(record, "Abbreviation"),
                Description = GetTranslatedString(record, "Description"),
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

    private static IReadOnlyList<NPCDTO> MapNPCs(PluginDTO plugin, IFallout4ModGetter mod)
    {
        return GetRecordCollection(mod, "Npcs", "NPCs")
            .Select(record => LocalizedStringDTOMapper.AddLocalizedStrings(new NPCDTO
            {
                Game = SupportedGame.Fallout4,
                ModKey = plugin.ModKey,
                FormKey = GetRequiredFormKey(record),
                EditorID = GetPropertyString(record, "EditorID"),
                FormVersion = GetPropertyInt(record, "FormVersion"),
                MajorRecordFlags = GetPropertyInt(record, "Fallout4MajorRecordFlags"),
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                ImportedAtUTC = DateTime.UtcNow,
                Name = GetTranslatedString(record, "Name"),
                ShortName = GetTranslatedString(record, "ShortName"),
                LongName = GetTranslatedString(record, "LongName"),
                DispositionBase = GetPropertyInt(GetPropertyValue(record, "Configuration") ?? record, "DispositionBase"),
                Aggression = GetPropertyString(GetPropertyValue(record, "AIData") ?? record, "Aggression"),
                Confidence = GetPropertyString(GetPropertyValue(record, "AIData") ?? record, "Confidence"),
                EnergyLevel = GetPropertyInt(GetPropertyValue(record, "AIData") ?? record, "EnergyLevel"),
                Responsibility = GetPropertyString(GetPropertyValue(record, "AIData") ?? record, "Responsibility"),
                Assistance = GetPropertyString(GetPropertyValue(record, "AIData") ?? record, "Assistance"),
                GearedUpWeapons = GetPropertyInt(GetPropertyValue(record, "PlayerSkills") ?? record, "GearedUpWeapons"),
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
                Keywords = GetKeywordMappings(plugin, RecordTypeCatalog.NPC.RecordID, GetRequiredRawFormKey(record), GetPropertyValue(record, "Keywords")),
                Sounds = GetNamedSounds(plugin, RecordTypeCatalog.NPC.RecordID, GetRequiredRawFormKey(record), record, "Sound"),
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.NPC.RecordID, record),
                Template = SpriggitValueFormatter.Format(GetPropertyValue(record, "Template")),
                DefaultTemplate = SpriggitValueFormatter.Format(GetPropertyValue(record, "DefaultTemplate")),
                TemplateActors = SpriggitValueFormatter.Format(GetPropertyValue(record, "TemplateActors")),
                WornArmor = SpriggitValueFormatter.Format(GetPropertyValue(record, "WornArmor")),
                FaceMorph = SpriggitValueFormatter.Format(GetPropertyValue(record, "FaceMorph")),
                FaceParts = SpriggitValueFormatter.Format(GetPropertyValue(record, "FaceParts")),
                HeadParts = SpriggitValueFormatter.Format(GetPropertyValue(record, "HeadParts")),
                HeadTexture = SpriggitValueFormatter.Format(GetPropertyValue(record, "HeadTexture")),
                SleepingOutfit = SpriggitValueFormatter.Format(GetPropertyValue(record, "SleepingOutfit")),
                TintLayers = SpriggitValueFormatter.Format(GetPropertyValue(record, "TintLayers")),
                Tints = SpriggitValueFormatter.Format(GetPropertyValue(record, "Tints")),
                SpaceOutfit = SpriggitValueFormatter.Format(GetPropertyValue(record, "SpaceOutfit")),
                BodyMorphRegionValues = SpriggitValueFormatter.Format(GetPropertyValue(record, "BodyMorphRegionValues")),
                ObjectTemplates = SpriggitValueFormatter.Format(GetPropertyValue(record, "ObjectTemplates")),
                AIData = SpriggitValueFormatter.Format(GetPropertyValue(record, "AIData"))
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
            Properties = GetClassProperties(plugin, game, formKey, GetPropertyValue(record, "Properties"))
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
            Keyword = GetFormKeyFromObject(GetPropertyValue(record, "Keyword")),
            Herd = GetFormKeyFromObject(GetPropertyValue(record, "Herd")),
            VoiceType = GetFormKeyFromObject(GetPropertyValue(record, "VoiceType")),
            SharedCrimeFactionList = GetFormKeyFromObject(GetPropertyValue(record, "SharedCrimeFactionList")),
            VendorBuySellList = GetFormKeyFromObject(GetPropertyValue(record, "VendorBuySellList")),
            MerchantContainer = GetFormKeyFromObject(GetPropertyValue(record, "MerchantContainer")),
            ExteriorJailMarker = GetFormKeyFromObject(GetPropertyValue(record, "ExteriorJailMarker")),
            FollowerWaitMarker = GetFormKeyFromObject(GetPropertyValue(record, "FollowerWaitMarker")),
            StolenGoodsContainer = GetFormKeyFromObject(GetPropertyValue(record, "StolenGoodsContainer")),
            PlayerInventoryContainer = GetFormKeyFromObject(GetPropertyValue(record, "PlayerInventoryContainer")),
            JailOutfit = GetFormKeyFromObject(GetPropertyValue(record, "JailOutfit")),
            CrimeValues = CreateFactionCrimeValues(crimeValues),
            VendorValues = CreateFactionVendorValues(vendorValues),
            VendorLocation = CreateFactionVendorLocation(vendorLocation, vendorLocationTarget),
            Relations = GetFactionRelations(plugin, game, formKey, GetPropertyValue(record, "Relations")),
            Ranks = GetFactionRanks(plugin, game, formKey, GetPropertyValue(record, "Ranks")),
            Conditions = GetConditionRules(plugin, game, formKey, GetPropertyValue(record, "Conditions")),
            Keywords = GetKeywordMappings(plugin, RecordTypeCatalog.Faction.RecordID, formKey, GetPropertyValue(record, "Keyword") is null ? null : new[] { GetPropertyValue(record, "Keyword")! })
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
                Target = GetFormKeyFromObject(GetPropertyValue(relation, "Target")),
                Reaction = GetFactionOptionalString(relation, "Reaction", "Neutral"),
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
                Number = GetPropertyNullableInt(rank, "Number") ?? GetPropertyNullableInt(rank, "Rank"),
                Title = new FactionRankDTO.TitleDTO
                {
                    Male = GetTranslatedString(rank, "MaleTitle"),
                    Female = GetTranslatedString(rank, "FemaleTitle")
                },
                ImportedAtUTC = DateTime.UtcNow
            }).ToList();
    }

    private static FactionDTO.CrimeValuesDTO? CreateFactionCrimeValues(object? crimeValues)
    {
        if (crimeValues is null)
        {
            return null;
        }

        var dto = new FactionDTO.CrimeValuesDTO
        {
            Arrest = GetFactionOptionalBool(crimeValues, "Arrest"),
            AttackOnSight = GetFactionOptionalBool(crimeValues, "AttackOnSight"),
            Murder = GetFactionOptionalInt(crimeValues, "Murder"),
            Assault = GetFactionOptionalInt(crimeValues, "Assault"),
            Trespass = GetFactionOptionalInt(crimeValues, "Trespass"),
            Pickpocket = GetFactionOptionalInt(crimeValues, "Pickpocket"),
            Steal = GetFactionOptionalInt(crimeValues, "Steal"),
            StealMult = GetFactionOptionalDouble(crimeValues, "StealMult"),
            StealMultiplier = GetFactionOptionalDouble(crimeValues, "StealMultiplier"),
            Escape = GetFactionOptionalInt(crimeValues, "Escape"),
            Werewolf = GetFactionOptionalInt(crimeValues, "Werewolf"),
            WerewolfUnused = GetFactionOptionalInt(crimeValues, "WerewolfUnused"),
            Unknown = GetFactionOptionalInt(crimeValues, "Unknown"),
            Piracy = GetFactionOptionalInt(crimeValues, "Piracy"),
            SmuggleMultiplier = GetFactionOptionalDouble(crimeValues, "SmuggleMultiplier")
        };
        return HasFactionCrimeValues(dto) ? dto : null;
    }

    private static FactionDTO.VendorValuesDTO? CreateFactionVendorValues(object? vendorValues)
    {
        if (vendorValues is null)
        {
            return null;
        }

        var dto = new FactionDTO.VendorValuesDTO
        {
            StartHour = GetFactionOptionalDouble(vendorValues, "StartHour"),
            EndHour = GetFactionOptionalDouble(vendorValues, "EndHour"),
            Radius = GetFactionOptionalInt(vendorValues, "Radius"),
            BuysStolenItems = GetFactionOptionalBool(vendorValues, "BuysStolenItems"),
            BuysNonStolenItems = GetFactionOptionalBool(vendorValues, "BuysNonStolenItems"),
            BuySellEverythingNotInList = GetFactionOptionalBool(vendorValues, "BuySellEverythingNotInList")
        };
        return HasFactionVendorValues(dto) ? dto : null;
    }

    private static int? GetFactionOptionalInt(object? source, string propertyName)
    {
        var value = GetPropertyNullableInt(source, propertyName);
        return value == 0 ? null : value;
    }

    private static double? GetFactionOptionalDouble(object? source, string propertyName)
    {
        var value = GetPropertyNullableDouble(source, propertyName);
        return value == 0 ? null : value;
    }

    private static bool? GetFactionOptionalBool(object? source, string propertyName)
    {
        var value = GetPropertyNullableBool(source, propertyName);
        return value == false ? null : value;
    }

    private static string? GetFactionOptionalString(object? source, string propertyName, string defaultValue)
    {
        var value = GetPropertyValue(source, propertyName)?.ToString();
        return string.Equals(value, defaultValue, StringComparison.OrdinalIgnoreCase) ? null : value;
    }

    private static bool HasFactionCrimeValues(FactionDTO.CrimeValuesDTO dto)
    {
        return dto.Arrest.HasValue ||
            dto.AttackOnSight.HasValue ||
            dto.Murder.HasValue ||
            dto.Assault.HasValue ||
            dto.Trespass.HasValue ||
            dto.Pickpocket.HasValue ||
            dto.Steal.HasValue ||
            dto.StealMult.HasValue ||
            dto.StealMultiplier.HasValue ||
            dto.Escape.HasValue ||
            dto.Werewolf.HasValue ||
            dto.WerewolfUnused.HasValue ||
            dto.Unknown.HasValue ||
            dto.Piracy.HasValue ||
            dto.SmuggleMultiplier.HasValue;
    }

    private static bool HasFactionVendorValues(FactionDTO.VendorValuesDTO dto)
    {
        return dto.StartHour.HasValue ||
            dto.EndHour.HasValue ||
            dto.Radius.HasValue ||
            dto.BuysStolenItems.HasValue ||
            dto.BuysNonStolenItems.HasValue ||
            dto.BuySellEverythingNotInList.HasValue;
    }

    private static FactionDTO.VendorLocationDTO? CreateFactionVendorLocation(object? vendorLocation, object? vendorLocationTarget)
    {
        return vendorLocation is null
            ? null
            : new FactionDTO.VendorLocationDTO
            {
                MutagenObjectType = GetPropertyValue(vendorLocation, "MutagenObjectType")?.ToString(),
                Target = vendorLocationTarget is null
                    ? null
                    : new FactionDTO.VendorLocationTargetDTO
                    {
                        MutagenObjectType = GetPropertyValue(vendorLocationTarget, "MutagenObjectType")?.ToString() ??
                            GetSpriggitMutagenObjectType(vendorLocationTarget),
                        Type = GetPropertyValue(vendorLocationTarget, "Type")?.ToString(),
                        Link = GetFormKeyFromObject(GetPropertyValue(vendorLocationTarget, "Link"))
                    }
        };
    }

    private static List<ConditionFormConditionDTO> GetConditionRules(PluginDTO plugin, SupportedGame game, FormKey formKey, object? conditions, string conditionSlot = "Conditions")
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
                ConditionSlot = conditionSlot,
                MutagenObjectType = condition.GetType().Name,
                DataMutagenObjectType = data?.GetType().Name,
                CompareOperator = GetPropertyValue(condition, "CompareOperator")?.ToString(),
                Flags = FormatEnumerable(GetPropertyValue(condition, "Flags")),
                Unknown2 = GetPropertyNullableInt(condition, "Unknown2"),
                ComparisonValue = FormatConditionValue(comparisonValue),
                ComparisonValueFormKey = GetFormKeyFromObject(comparisonValue),
                ImportedAtUTC = importedAtUTC,
                Parameters = GetConditionRuleParameters(plugin, game, formKey, conditionSlot, conditionIndex, data, importedAtUTC)
            };
        }).ToList();
    }

    private static List<ConditionFormConditionParameterDTO> GetConditionRuleParameters(PluginDTO plugin, SupportedGame game, FormKey formKey, string conditionSlot, int conditionIndex, object? data, DateTime importedAtUTC)
    {
        return data?.GetType().GetProperties()
            .Where(property => property.GetIndexParameters().Length == 0)
            .Where(property => !string.Equals(property.Name, "MutagenObjectType", StringComparison.Ordinal))
            .Select(property => new ConditionFormConditionParameterDTO
            {
                Game = game,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(formKey),
                ConditionIndex = conditionIndex,
                ConditionSlot = conditionSlot,
                ParameterName = property.Name,
                ParameterValue = FormatConditionValue(property.GetValue(data)),
                ParameterFormKey = GetFormKeyFromObject(property.GetValue(data)),
                ImportedAtUTC = importedAtUTC
            })
            .Where(parameter => !string.IsNullOrWhiteSpace(parameter.ParameterValue) || parameter.ParameterFormKey != null)
            .ToList() ?? new List<ConditionFormConditionParameterDTO>();
    }

    private static IReadOnlyList<MagicEffectDTO> MapMagicEffects(PluginDTO plugin, IFallout4ModGetter mod)
    {
        return GetRecordCollection(mod, "MagicEffects")
            .Select(record => LocalizedStringDTOMapper.AddLocalizedStrings(new MagicEffectDTO
            {
                Game = SupportedGame.Fallout4,
                ModKey = plugin.ModKey,
                FormKey = GetRequiredFormKey(record),
                EditorID = GetPropertyString(record, "EditorID"),
                FormVersion = GetPropertyInt(record, "FormVersion"),
                MajorRecordFlags = GetPropertyInt(record, "Fallout4MajorRecordFlags"),
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                ImportedAtUTC = DateTime.UtcNow,
                Name = GetTranslatedString(record, "Name"),
                Description = GetTranslatedString(record, "Description"),
                Flags = GetPropertyString(record, "Flags"),
                CastType = GetPropertyStringOrNull(record, "CastType"),
                TargetType = GetNonDefaultString(GetPropertyValue(record, "TargetType")),
                CastingSoundLevel = GetPropertyStringOrNull(record, "CastingSoundLevel"),
                DualCastScale = GetPropertyValue(record, "DualCastScale")?.ToString(),
                Unknown1 = GetNonDefaultString(GetPropertyValue(record, "Unknown1")),
                BaseCost = GetNonDefaultString(GetPropertyValue(record, "BaseCost")),
                MagicSkill = GetNonDefaultFormKeyOrString(record, "MagicSkill"),
                CastingLightFormKey = GetLinkedFormKey(record, "CastingLight"),
                MenuDisplayObjectFormKey = GetLinkedFormKey(record, "MenuDisplayObject"),
                MinimumSkillLevel = GetPropertyNonZeroInt(record, "MinimumSkillLevel"),
                SkillUsageMultiplier = GetNonDefaultString(GetPropertyValue(record, "SkillUsageMultiplier")),
                SpellmakingCastingTime = GetNonDefaultString(GetPropertyValue(record, "SpellmakingCastingTime")),
                TaperWeight = GetNonDefaultString(GetPropertyValue(record, "TaperWeight")),
                SecondActorValue = GetNonDefaultFormKeyOrString(record, "SecondActorValue"),
                SecondActorValueWeight = GetNonDefaultString(GetPropertyValue(record, "SecondActorValueWeight")),
                SpellmakingArea = GetPropertyNonZeroInt(record, "SpellmakingArea"),
                EnchantShaderFormKey = GetLinkedFormKey(record, "EnchantShader"),
                ActorValue2FormKey = GetLinkedFormKey(record, "ActorValue2"),
                ResistValueFormKey = GetLinkedFormKey(record, "ResistValue"),
                ResistValue = GetFormKeyOrString(record, "ResistValue"),
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
                ArchetypeActorValue = GetFormKeyOrString(GetPropertyValue(record, "Archetype"), "ActorValue"),
                ArchetypeAssociationFormKey = GetLinkedFormKey(GetPropertyValue(record, "Archetype"), "Association"),
                UnknownFloat1 = GetPropertyNonZeroFloat(record, "UnknownFloat1"),
                UnknownFloat3 = GetPropertyNonZeroFloat(record, "UnknownFloat3"),
                UnknownFloat4 = GetPropertyNonZeroFloat(record, "UnknownFloat4"),
                UnknownInt2 = GetPropertyNullableInt(record, "UnknownInt2"),
                UnknownInt3 = GetPropertyNonZeroLong(record, "UnknownInt3"),
                Unknown = FormatHexValue(GetPropertyValue(record, "Unknown")),
                Unknown2 = FormatHexValue(GetPropertyValue(record, "Unknown2")),
                DataTypeState = GetPropertyStringOrNull(record, "DATADataTypeState"),
                Keywords = GetKeywordMappings(plugin, RecordTypeCatalog.MagicEffect.RecordID, GetRequiredRawFormKey(record), GetPropertyValue(record, "Keywords")),
                Sounds = GetIndexedSounds(plugin, RecordTypeCatalog.MagicEffect.RecordID, GetRequiredRawFormKey(record), record),
                Conditions = GetConditionRules(plugin, SupportedGame.Fallout4, GetRequiredRawFormKey(record), GetPropertyValue(record, "Conditions")),
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.MagicEffect.RecordID, record)
            }, record))
            .ToList();
    }

    private static IReadOnlyList<PerkDTO> MapPerks(PluginDTO plugin, IFallout4ModGetter mod)
    {
        return GetRecordCollection(mod, "Perks")
            .Select(record => LocalizedStringDTOMapper.AddLocalizedStrings(new PerkDTO
            {
                Game = SupportedGame.Fallout4,
                ModKey = plugin.ModKey,
                FormKey = GetRequiredFormKey(record),
                EditorID = GetPropertyString(record, "EditorID"),
                FormVersion = GetPropertyInt(record, "FormVersion"),
                MajorRecordFlags = GetPropertyInt(record, "Fallout4MajorRecordFlags"),
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
                Level = GetPropertyNullableInt(record, "Level"),
                NumRanks = GetPropertyNullableInt(record, "NumRanks"),
                Playable = GetPropertyNullableBool(record, "Playable"),
                Hidden = GetPropertyNullableBool(record, "Hidden"),
                NextPerk = GetLinkedFormKey(record, "NextPerk"),
                Effects = GetPerkEffects(plugin, SupportedGame.Fallout4, GetRequiredRawFormKey(record), record),
                Ranks = GetPerkRanks(plugin, record),
                BackgroundSkills = GetPerkBackgroundSkills(plugin, record),
                Conditions = GetPerkConditionRules(plugin, SupportedGame.Fallout4, GetRequiredRawFormKey(record), record),
                Sounds = GetNamedSounds(plugin, RecordTypeCatalog.Perk.RecordID, GetRequiredRawFormKey(record), record, "Sound"),
                ScriptFragments = GetScriptFragments(SupportedGame.Fallout4, plugin, RecordTypeCatalog.Perk.RecordID, GetRequiredRawFormKey(record), record),
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.Perk.RecordID, record)
            }, record))
            .ToList();
    }

    private static IReadOnlyList<StaticDTO> MapStatics(PluginDTO plugin, IFallout4ModGetter mod)
    {
        return GetRecordCollection(mod, "Statics")
            .Select(record => LocalizedStringDTOMapper.AddLocalizedStrings(new StaticDTO
            {
                Game = SupportedGame.Fallout4,
                ModKey = plugin.ModKey,
                FormKey = GetRequiredFormKey(record),
                EditorID = GetPropertyString(record, "EditorID"),
                FormVersion = GetPropertyInt(record, "FormVersion"),
                MajorRecordFlags = GetPropertyInt(record, "Fallout4MajorRecordFlags"),
                Name = GetTranslatedString(record, "Name"),
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                ImportedAtUTC = DateTime.UtcNow,
                ObjectBoundsFirst = FormatObjectBoundsPoint(GetPropertyValue(record, "ObjectBounds"), "First"),
                ObjectBoundsSecond = FormatObjectBoundsPoint(GetPropertyValue(record, "ObjectBounds"), "Second"),
                MaxAngle = GetPropertyNullableDouble(record, "MaxAngle"),
                LeafAmplitude = GetPropertyNullableDouble(record, "LeafAmplitude"),
                LeafFrequency = GetPropertyNullableDouble(record, "LeafFrequency"),
                DNAMDataTypeState = FormatEnumerable(GetPropertyValue(record, "DNAMDataTypeState")),
                PreviewTransform = GetLinkedFormKey(record, "PreviewTransform"),
                Material = GetLinkedFormKey(record, "Material"),
                LodLevel0 = GetPropertyStringOrNull(GetPropertyValue(record, "Lod"), "Level0"),
                LodLevel1 = GetPropertyStringOrNull(GetPropertyValue(record, "Lod"), "Level1"),
                LodLevel2 = GetPropertyStringOrNull(GetPropertyValue(record, "Lod"), "Level2"),
                LodLevel3 = GetPropertyStringOrNull(GetPropertyValue(record, "Lod"), "Level3"),
                NavmeshGeometry = SpriggitValueFormatter.Format(GetPropertyValue(record, "NavmeshGeometry")),
                Properties = GetStaticProperties(plugin, GetRequiredRawFormKey(record), GetPropertyValue(record, "Properties")),
                Models = GetModels(plugin, RecordTypeCatalog.Static.RecordID, GetRequiredRawFormKey(record), GetPropertyValue(record, "Model"))
            }, record))
            .ToList();
    }

    private static List<StaticPropertyDTO> GetStaticProperties(PluginDTO plugin, FormKey formKey, object? properties)
    {
        var importedAtUTC = DateTime.UtcNow;
        return (properties as IEnumerable)?.Cast<object>()
            .Select((property, propertyIndex) => new StaticPropertyDTO
            {
                Game = SupportedGame.Fallout4,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(formKey),
                PropertyIndex = propertyIndex,
                ActorValue = GetLinkedFormKey(property, "ActorValue"),
                Value = GetPropertyNullableDouble(property, "Value"),
                ImportedAtUTC = importedAtUTC
            })
            .ToList() ?? new List<StaticPropertyDTO>();
    }

    private static IReadOnlyList<ContainerDTO> MapContainers(PluginDTO plugin, IFallout4ModGetter mod)
    {
        return GetRecordCollection(mod, "Containers")
            .Select(record => LocalizedStringDTOMapper.AddLocalizedStrings(new ContainerDTO
            {
                Game = SupportedGame.Fallout4,
                ModKey = plugin.ModKey,
                FormKey = GetRequiredFormKey(record),
                EditorID = GetPropertyString(record, "EditorID"),
                FormVersion = GetPropertyInt(record, "FormVersion"),
                MajorRecordFlags = GetPropertyInt(record, "Fallout4MajorRecordFlags"),
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                ImportedAtUTC = DateTime.UtcNow,
                ObjectBoundsFirst = FormatObjectBoundsPoint(GetPropertyValue(record, "ObjectBounds"), "First"),
                ObjectBoundsSecond = FormatObjectBoundsPoint(GetPropertyValue(record, "ObjectBounds"), "Second"),
                Name = GetTranslatedString(record, "Name"),
                Flags = FormatEnumerable(GetPropertyValue(record, "Flags")),
                MajorFlags = GetPropertyStringOrNull(record, "MajorFlags"),
                NativeTerminalFormKey = GetLinkedFormKey(record, "NativeTerminal"),
                Items = GetContainerItems(plugin, GetRequiredRawFormKey(record), GetPropertyValue(record, "Items")),
                Models = GetModels(plugin, RecordTypeCatalog.Container.RecordID, GetRequiredRawFormKey(record), GetPropertyValue(record, "Model")),
                Keywords = GetKeywordMappings(plugin, RecordTypeCatalog.Container.RecordID, GetRequiredRawFormKey(record), GetPropertyValue(record, "Keywords")),
                Sounds = GetNamedSounds(plugin, RecordTypeCatalog.Container.RecordID, GetRequiredRawFormKey(record), record, "OpenSound", "CloseSound", "TakeAllSound"),
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.Container.RecordID, record)
            }, record))
            .ToList();
    }

    private static IReadOnlyList<BookDTO> MapBooks(PluginDTO plugin, IFallout4ModGetter mod)
    {
        return GetRecordCollection(mod, "Books")
            .Select(record => LocalizedStringDTOMapper.AddLocalizedStrings(new BookDTO
            {
                Game = SupportedGame.Fallout4,
                ModKey = plugin.ModKey,
                FormKey = GetRequiredFormKey(record),
                EditorID = GetPropertyString(record, "EditorID"),
                FormVersion = GetPropertyInt(record, "FormVersion"),
                MajorRecordFlags = GetPropertyInt(record, "Fallout4MajorRecordFlags"),
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                ImportedAtUTC = DateTime.UtcNow,
                ObjectBounds = new ObjectBoundsDTO
                {
                    First = FormatObjectBoundsPoint(GetPropertyValue(record, "ObjectBounds"), "First"),
                    Second = FormatObjectBoundsPoint(GetPropertyValue(record, "ObjectBounds"), "Second")
                },
                InventoryArt = GetLinkedFormKey(record, "InventoryArt"),
                PreviewTransform = GetLinkedFormKey(record, "PreviewTransform"),
                FeaturedItemMessage = GetLinkedFormKey(record, "FeaturedItemMessage"),
                Name = GetTranslatedString(record, "Name"),
                Text = GetTranslatedString(record, "BookText") ?? GetTranslatedString(record, "Text"),
                Value = GetPropertyNullableInt(record, "Value"),
                Weight = GetPropertyNullableFloat(record, "Weight"),
                Flags = FormatEnumerable(GetPropertyValue(record, "Flags")),
                Teaches = new BookTeachesDTO
                {
                    MutagenObjectType = GetPropertyStringOrNull(GetPropertyValue(record, "Teaches"), "Type") ?? GetPropertyValue(record, "Teaches")?.GetType().Name,
                    Perk = GetFormKeyFromObject(GetPropertyValue(GetPropertyValue(record, "Teaches"), "Perk")),
                    RawContent = FormatEnumerable(GetPropertyValue(GetPropertyValue(record, "Teaches"), "RawContent"))
                },
                Description = GetTranslatedString(record, "Description"),
                Models = GetModels(plugin, RecordTypeCatalog.Book.RecordID, GetRequiredRawFormKey(record), GetPropertyValue(record, "Model")),
                Keywords = GetKeywordMappings(plugin, RecordTypeCatalog.Book.RecordID, GetRequiredRawFormKey(record), GetPropertyValue(record, "Keywords")),
                Sounds = GetNamedSounds(plugin, RecordTypeCatalog.Book.RecordID, GetRequiredRawFormKey(record), record, "PickupSound", "PickUpSound", "DropdownSound", "PutdownSound"),
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.Book.RecordID, record)
            }, record))
            .ToList();
    }

    private static IReadOnlyList<DoorDTO> MapDoors(PluginDTO plugin, IFallout4ModGetter mod)
    {
        return GetRecordCollection(mod, "Doors")
            .Select(record => LocalizedStringDTOMapper.AddLocalizedStrings(new DoorDTO
            {
                Game = SupportedGame.Fallout4,
                ModKey = plugin.ModKey,
                FormKey = GetRequiredFormKey(record),
                EditorID = GetPropertyString(record, "EditorID"),
                FormVersion = GetPropertyInt(record, "FormVersion"),
                MajorRecordFlags = GetPropertyInt(record, "Fallout4MajorRecordFlags"),
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                ImportedAtUTC = DateTime.UtcNow,
                ObjectBoundsFirst = FormatObjectBoundsPoint(GetPropertyValue(record, "ObjectBounds"), "First"),
                ObjectBoundsSecond = FormatObjectBoundsPoint(GetPropertyValue(record, "ObjectBounds"), "Second"),
                Name = GetTranslatedString(record, "Name"),
                Flags = FormatEnumerable(GetPropertyValue(record, "Flags")),
                NativeTerminalFormKey = GetLinkedFormKey(record, "NativeTerminal"),
                SoundLevel = GetPropertyStringOrNull(record, "SoundLevel"),
                FacingAxisOverride = GetPropertyStringOrNull(record, "FacingAxisOverride"),
                Models = GetModels(plugin, RecordTypeCatalog.Door.RecordID, GetRequiredRawFormKey(record), GetPropertyValue(record, "Model")),
                Keywords = GetKeywordMappings(plugin, RecordTypeCatalog.Door.RecordID, GetRequiredRawFormKey(record), GetPropertyValue(record, "Keywords")),
                Sounds = GetNamedSounds(plugin, RecordTypeCatalog.Door.RecordID, GetRequiredRawFormKey(record), record, "OpenSound", "CloseSound"),
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.Door.RecordID, record)
            }, record))
            .ToList();
    }

    private static IReadOnlyList<TerminalDTO> MapTerminals(PluginDTO plugin, IFallout4ModGetter mod)
    {
        return GetRecordCollection(mod, "Terminals")
            .Select(record => LocalizedStringDTOMapper.AddLocalizedStrings(CreateTerminal(plugin, record), record))
            .ToList();
    }

    private static TerminalDTO CreateTerminal(PluginDTO plugin, object record)
    {
        var formKey = GetRequiredRawFormKey(record);
        var model = GetPropertyValue(record, "Model");
        var bodyTexts = GetTerminalBodyTexts(plugin, formKey, GetPropertyValue(record, "BodyTexts"));
        var menuItems = GetTerminalMenuItems(plugin, formKey, GetPropertyValue(record, "MenuItems"));
        var dto = new TerminalDTO
        {
            Game = SupportedGame.Fallout4,
            ModKey = plugin.ModKey,
            FormKey = MapFormKey(formKey),
            EditorID = GetPropertyString(record, "EditorID"),
            FormVersion = GetPropertyInt(record, "FormVersion"),
            MajorRecordFlags = GetPropertyInt(record, "Fallout4MajorRecordFlags"),
            Version2 = GetPropertyNullableInt(record, "Version2"),
            VersionControl = GetPropertyNullableInt(record, "VersionControl"),
            ImportedAtUTC = DateTime.UtcNow,
            ObjectBoundsFirst = FormatObjectBoundsPoint(GetPropertyValue(record, "ObjectBounds"), "First"),
            ObjectBoundsSecond = FormatObjectBoundsPoint(GetPropertyValue(record, "ObjectBounds"), "Second"),
            HeaderText = GetTranslatedString(record, "HeaderText"),
            WelcomeText = GetTranslatedString(record, "WelcomeText"),
            Name = GetTranslatedString(record, "Name"),
            Pnam = FormatHexValue(GetPropertyValue(record, "PNAM")),
            Fnam = FormatHexValue(GetPropertyValue(record, "FNAM")),
            Flags = FormatEnumerable(GetPropertyValue(record, "Flags")),
            MajorFlags = FormatMajorFlags(GetPropertyValue(record, "MajorFlags")),
            WorkbenchData = FormatHexValue(GetPropertyValue(record, "WorkbenchData")),
            Models = GetModels(plugin, RecordTypeCatalog.Terminal.RecordID, formKey, model),
            Keywords = GetKeywordMappings(plugin, RecordTypeCatalog.Terminal.RecordID, formKey, GetPropertyValue(record, "Keywords")),
            ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.Terminal.RecordID, record),
            MarkerParameters = GetTerminalMarkerParameters(plugin, formKey, GetPropertyValue(record, "MarkerParameters")),
            ForcedLocations = GetFormKeys(GetPropertyValue(record, "ForcedLocations")),
            BodyTexts = bodyTexts,
            MenuItems = menuItems,
            Conditions = GetTerminalConditionRules(plugin, SupportedGame.Fallout4, formKey, record),
            ScriptFragments = GetScriptFragments(SupportedGame.Fallout4, plugin, RecordTypeCatalog.Terminal.RecordID, formKey, record)
        };

        return dto;
    }

    private static List<TerminalMarkerParameterDTO> GetTerminalMarkerParameters(PluginDTO plugin, FormKey formKey, object? markerParameters)
    {
        var importedAtUTC = DateTime.UtcNow;
        return (markerParameters as IEnumerable)?.Cast<object>()
            .Select((parameter, parameterIndex) => new TerminalMarkerParameterDTO
            {
                Game = SupportedGame.Fallout4,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(formKey),
                ParameterIndex = parameterIndex,
                Enabled = GetPropertyNullableBool(parameter, "Enabled"),
                Offset = GetPropertyValue(parameter, "Offset")?.ToString(),
                EntryTypes = FormatEnumerable(GetPropertyValue(parameter, "EntryTypes")),
                ExitTypes = FormatEnumerable(GetPropertyValue(parameter, "ExitTypes")),
                Unknown = FormatHexValue(GetPropertyValue(parameter, "Unknown")),
                ImportedAtUTC = importedAtUTC
            })
            .ToList() ?? new List<TerminalMarkerParameterDTO>();
    }

    private static List<TerminalBodyTextDTO> GetTerminalBodyTexts(PluginDTO plugin, FormKey formKey, object? bodyTexts)
    {
        var importedAtUTC = DateTime.UtcNow;
        return (bodyTexts as IEnumerable)?.Cast<object>()
            .Select((bodyText, bodyTextIndex) => new TerminalBodyTextDTO
            {
                Game = SupportedGame.Fallout4,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(formKey),
                BodyTextIndex = bodyTextIndex,
                Text = GetTranslatedString(bodyText, "Text"),
                ImportedAtUTC = importedAtUTC
            })
            .ToList() ?? new List<TerminalBodyTextDTO>();
    }

    private static List<TerminalMenuItemDTO> GetTerminalMenuItems(PluginDTO plugin, FormKey formKey, object? menuItems)
    {
        var importedAtUTC = DateTime.UtcNow;
        return (menuItems as IEnumerable)?.Cast<object>()
            .Select((menuItem, menuItemIndex) => new TerminalMenuItemDTO
            {
                Game = SupportedGame.Fallout4,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(formKey),
                MenuItemIndex = menuItemIndex,
                ItemText = GetTranslatedString(menuItem, "ItemText"),
                Type = GetPropertyStringOrNull(menuItem, "Type"),
                ItemId = GetPropertyNullableInt(menuItem, "ItemId") ?? GetPropertyNullableInt(menuItem, "ItemID"),
                Submenu = GetLinkedFormKey(menuItem, "Submenu"),
                DisplayText = GetTranslatedString(menuItem, "DisplayText"),
                ImportedAtUTC = importedAtUTC
            })
            .ToList() ?? new List<TerminalMenuItemDTO>();
    }

    private static List<ConditionFormConditionDTO> GetTerminalConditionRules(
        PluginDTO plugin,
        SupportedGame game,
        FormKey formKey,
        object record)
    {
        var conditions = new List<ConditionFormConditionDTO>();
        AddTerminalConditionRules(conditions, plugin, game, formKey, "BodyTexts", GetPropertyValue(record, "BodyTexts"));
        AddTerminalConditionRules(conditions, plugin, game, formKey, "MenuItems", GetPropertyValue(record, "MenuItems"));
        return conditions;
    }

    private static void AddTerminalConditionRules(
        ICollection<ConditionFormConditionDTO> conditions,
        PluginDTO plugin,
        SupportedGame game,
        FormKey formKey,
        string collectionName,
        object? collection)
    {
        if (collection is not IEnumerable enumerable) return;
        foreach (var item in enumerable.Cast<object>().Select((value, index) => new { value, index }))
        {
            foreach (var condition in GetConditionRules(plugin, game, formKey, GetPropertyValue(item.value, "Conditions"), $"{collectionName}[{item.index}].Conditions"))
            {
                conditions.Add(condition);
            }
        }
    }

    private static IReadOnlyList<ConstructibleObjectDTO> MapConstructibleObjects(PluginDTO plugin, IFallout4ModGetter mod)
    {
        return GetRecordCollection(mod, "ConstructibleObjects")
            .Select(record => LocalizedStringDTOMapper.AddLocalizedStrings(new ConstructibleObjectDTO
            {
                Game = SupportedGame.Fallout4,
                ModKey = plugin.ModKey,
                FormKey = GetRequiredFormKey(record),
                EditorID = GetPropertyString(record, "EditorID"),
                FormVersion = GetPropertyInt(record, "FormVersion"),
                MajorRecordFlags = GetPropertyInt(record, "Fallout4MajorRecordFlags"),
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                ImportedAtUTC = DateTime.UtcNow,
                Description = GetTranslatedString(record, "Description"),
                CreatedObjectFormKey = GetLinkedFormKey(record, "CreatedObject"),
                WorkbenchKeywordFormKey = GetLinkedFormKey(record, "WorkbenchKeyword"),
                CreatedObjectCount = GetFirstCount(GetPropertyValue(record, "CreatedObjectCounts")),
                Value = GetPropertyNullableInt(record, "Value"),
                MajorFlags = FormatMajorFlags(GetPropertyValue(record, "MajorFlags")),
                Components = GetConstructibleObjectComponents(plugin, GetRequiredRawFormKey(record), GetPropertyValue(record, "Components")),
                Categories = GetConstructibleObjectCategories(plugin, GetRequiredRawFormKey(record), GetPropertyValue(record, "Categories")),
                Conditions = GetConditionRules(plugin, SupportedGame.Fallout4, GetRequiredRawFormKey(record), GetPropertyValue(record, "Conditions")),
                Sounds = GetNamedSounds(plugin, RecordTypeCatalog.ConstructibleObject.RecordID, GetRequiredRawFormKey(record), record, "PickUpSound", "PutDownSound"),
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
                Conditions = GetConditionRules(plugin, SupportedGame.Fallout4, formKey, GetPropertyValue(rank, "Conditions"), GetPerkRankConditionSlot(rankIndex)),
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
                Flags = FormatEnumerable(GetPropertyValue(effect, "Flags")),
                ButtonLabel = GetTranslatedString(effect, "ButtonLabel"),
                ConditionCount = GetEnumerableCount(GetPropertyValue(effect, "Conditions")),
                EntryPoint = GetPropertyStringOrNull(effect, "EntryPoint"),
                PerkConditionTabCount = GetPropertyNullableInt(effect, "PerkConditionTabCount"),
                Modification = GetPropertyStringOrNull(effect, "Modification"),
                Value = GetPropertyNullableDouble(effect, "Value"),
                ActorValue = GetFormKeyOrString(effect, "ActorValue"),
                Spell = GetFormKeyOrString(effect, "Spell"),
                Quest = GetFormKeyOrString(effect, "Quest"),
                Stage = GetPropertyNullableInt(effect, "Stage"),
                Conditions = GetPerkEffectConditionTabs(plugin, SupportedGame.Fallout4, formKey, rankIndex, effectIndex, effect, importedAtUTC),
                ImportedAtUTC = importedAtUTC
            })
            .ToList() ?? new List<PerkRankEffectDTO>();
    }

    private static List<PerkEffectDTO> GetPerkEffects(PluginDTO plugin, SupportedGame game, FormKey formKey, object record)
    {
        var effects = GetOrderedPerkEffects(GetPropertyValue(record, "Effects"));
        if (effects.Count == 0) return new List<PerkEffectDTO>();

        var importedAtUTC = DateTime.UtcNow;
        return effects
            .Select((effect, effectIndex) => new PerkEffectDTO
            {
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(formKey),
                EffectIndex = effectIndex,
                MutagenObjectType = effect.GetType().Name,
                Rank = GetPropertyNullableInt(effect, "Rank"),
                Priority = GetPropertyNullableInt(effect, "Priority"),
                PerkEntryId = GetPropertyNullableInt(effect, "PerkEntryID"),
                Flags = FormatEnumerable(GetPropertyValue(effect, "Flags")),
                ButtonLabel = GetTranslatedString(effect, "ButtonLabel"),
                ConditionCount = GetEnumerableCount(GetPropertyValue(effect, "Conditions")),
                EntryPoint = GetPropertyStringOrNull(effect, "EntryPoint"),
                PerkConditionTabCount = GetPropertyNullableInt(effect, "PerkConditionTabCount"),
                Modification = GetPropertyStringOrNull(effect, "Modification"),
                Value = GetPropertyNullableDouble(effect, "Value"),
                ActorValue = GetFormKeyOrString(effect, "ActorValue"),
                Spell = GetFormKeyOrString(effect, "Spell"),
                Quest = GetFormKeyOrString(effect, "Quest"),
                Stage = GetPropertyNullableInt(effect, "Stage"),
                Conditions = GetPerkEffectConditionTabs(plugin, game, formKey, null, effectIndex, effect, importedAtUTC),
                ImportedAtUTC = importedAtUTC
            })
            .ToList();
    }

    private static List<PerkEffectConditionTabDTO> GetPerkEffectConditionTabs(
        PluginDTO plugin,
        SupportedGame game,
        FormKey formKey,
        int? rankIndex,
        int effectIndex,
        object effect,
        DateTime importedAtUTC)
    {
        return (GetPropertyValue(effect, "Conditions") as IEnumerable)?.Cast<object>()
            .Select((conditionTab, conditionTabIndex) =>
            {
                var slot = GetPerkEffectConditionSlot(rankIndex, effectIndex, conditionTabIndex);
                return new PerkEffectConditionTabDTO
                {
                    ModKey = plugin.ModKey,
                    FormKey = MapFormKey(formKey),
                    RankIndex = rankIndex,
                    EffectIndex = effectIndex,
                    ConditionTabIndex = conditionTabIndex,
                    RunOnTabIndex = GetPropertyNullableInt(conditionTab, "RunOnTabIndex"),
                    ConditionCount = GetEnumerableCount(GetPropertyValue(conditionTab, "Conditions")),
                    Conditions = GetConditionRules(plugin, game, formKey, GetPropertyValue(conditionTab, "Conditions"), slot),
                    ImportedAtUTC = importedAtUTC
                };
            })
            .ToList() ?? new List<PerkEffectConditionTabDTO>();
    }

    private static List<ConditionFormConditionDTO> GetPerkConditionRules(PluginDTO plugin, SupportedGame game, FormKey formKey, object record)
    {
        var conditions = GetConditionRules(plugin, game, formKey, GetPropertyValue(record, "Conditions"));
        AddPerkEffectConditionRules(conditions, plugin, game, formKey, null, GetPropertyValue(record, "Effects"));

        var ranks = GetPropertyValue(record, "Ranks") as IEnumerable;
        if (ranks != null)
        {
            foreach (var rank in ranks.Cast<object>().Select((value, index) => new { value, index }))
            {
                conditions.AddRange(GetConditionRules(plugin, game, formKey, GetPropertyValue(rank.value, "Conditions"), GetPerkRankConditionSlot(rank.index)));
                AddPerkEffectConditionRules(conditions, plugin, game, formKey, rank.index, GetPropertyValue(rank.value, "Effects"));
            }
        }

        return conditions;
    }

    private static void AddPerkEffectConditionRules(
        ICollection<ConditionFormConditionDTO> conditions,
        PluginDTO plugin,
        SupportedGame game,
        FormKey formKey,
        int? rankIndex,
        object? effects)
    {
        var effectList = rankIndex.HasValue
            ? (effects as IEnumerable)?.Cast<object>().ToList() ?? new List<object>()
            : GetOrderedPerkEffects(effects);
        if (effectList.Count == 0)
        {
            return;
        }

        foreach (var effect in effectList.Select((value, index) => new { value, index }))
        {
            var conditionTabs = GetPropertyValue(effect.value, "Conditions") as IEnumerable;
            if (conditionTabs == null)
            {
                continue;
            }

            foreach (var conditionTab in conditionTabs.Cast<object>().Select((value, index) => new { value, index }))
            {
                var slot = GetPerkEffectConditionSlot(rankIndex, effect.index, conditionTab.index);
                foreach (var condition in GetConditionRules(plugin, game, formKey, GetPropertyValue(conditionTab.value, "Conditions"), slot))
                {
                    conditions.Add(condition);
                }
            }
        }
    }

    private static string GetPerkEffectConditionSlot(int? rankIndex, int effectIndex, int conditionTabIndex)
    {
        return rankIndex.HasValue
            ? $"Ranks[{rankIndex.Value}].Effects[{effectIndex}].Conditions[{conditionTabIndex}].Conditions"
            : $"Effects[{effectIndex}].Conditions[{conditionTabIndex}].Conditions";
    }

    private static List<object> GetOrderedPerkEffects(object? effects)
    {
        return (effects as IEnumerable)?.Cast<object>()
            .Select((value, originalIndex) => new { value, originalIndex })
            .OrderBy(effect => GetPropertyNullableInt(effect.value, "Priority") ?? 0)
            .ThenBy(effect => effect.originalIndex)
            .Select(effect => effect.value)
            .ToList() ?? new List<object>();
    }

    private static string GetPerkRankConditionSlot(int rankIndex)
    {
        return $"Ranks[{rankIndex}].Conditions";
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
                Game = SupportedGame.Fallout4,
                ModKey = plugin.ModKey,
                RecordType = recordType,
                FormKey = MapFormKey(formKey),
                ModelSlot = "Model",
                ModelGender = string.Empty,
                File = FormatSpriggitModelFilePath(GetPropertyValue(model, "File")?.ToString()),
                Data = FormatHexValue(GetPropertyValue(model, "Data")),
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

    private static string? FormatSpriggitModelFilePath(string? file)
    {
        if (file == null)
        {
            return null;
        }

        return file.StartsWith("Meshes\\", StringComparison.OrdinalIgnoreCase) ||
               file.StartsWith("Meshes/", StringComparison.OrdinalIgnoreCase)
            ? file[7..]
            : file;
    }

    private static List<ModelMaterialSwapDTO> GetModelMaterialSwaps(PluginDTO plugin, string recordType, FormKey formKey, object model, DateTime importedAtUTC)
    {
        var materialSwaps = (GetPropertyValue(model, "MaterialSwaps") as IEnumerable)?.Cast<object>().ToList() ?? new List<object>();
        if (GetPropertyValue(model, "AlternateTextures") is IEnumerable alternateTextures)
        {
            materialSwaps.AddRange(alternateTextures.Cast<object>());
        }

        var materialSwap = GetPropertyValue(model, "MaterialSwap");
        if (materialSwap != null)
        {
            materialSwaps.Add(materialSwap);
        }

        return materialSwaps
            .Select((materialSwap, materialSwapIndex) => (GetFormKeyFromObject(GetPropertyValue(materialSwap, "NewTexture")) ?? GetFormKeyFromObject(materialSwap)) is { } materialSwapFormKey
                ? new ModelMaterialSwapDTO
                {
                    Game = SupportedGame.Fallout4,
                    ModKey = plugin.ModKey,
                    RecordType = recordType,
                    FormKey = MapFormKey(formKey),
                    ModelSlot = "Model",
                    ModelGender = string.Empty,
                    Name = GetPropertyStringOrNull(materialSwap, "Name"),
                    MaterialSwapFormKey = materialSwapFormKey,
                    MaterialSwapIndex = materialSwapIndex,
                    ImportedAtUTC = importedAtUTC
                }
                : null)
            .Where(materialSwap => materialSwap != null)
            .Cast<ModelMaterialSwapDTO>()
            .ToList();
    }

    private static List<KeywordMappingDTO> GetKeywordMappings(PluginDTO plugin, string recordType, FormKey formKey, object? keywords)
    {
        if (keywords is not IEnumerable enumerable) return new List<KeywordMappingDTO>();

        var importedAtUTC = DateTime.UtcNow;
        return enumerable
            .Cast<object>()
            .Select((keyword, keywordIndex) => GetFormKeyFromObject(keyword) is { } keywordFormKey
                ? new KeywordMappingDTO
                {
                    Game = SupportedGame.Fallout4,
                    ModKey = plugin.ModKey,
                    RecordType = recordType,
                    FormKey = MapFormKey(formKey),
                    Keyword = keywordFormKey,
                    KeywordIndex = keywordIndex,
                    ImportedAtUTC = importedAtUTC
                }
                : null)
            .Where(keyword => keyword != null)
            .Cast<KeywordMappingDTO>()
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
            Game = SupportedGame.Fallout4,
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
        var componentData = GetPropertyValue(component, "Component") ?? component;
        var componentFormKey = GetFormKeyFromObject(GetPropertyValue(componentData, "Component")) ?? GetFormKeyFromObject(GetPropertyValue(componentData, "Item")) ?? GetFormKeyFromObject(componentData);
        if (componentFormKey == null)
        {
            return null;
        }

        return new ConstructibleObjectComponentDTO
        {
            Game = SupportedGame.Fallout4,
            ModKey = plugin.ModKey,
            FormKey = MapFormKey(formKey),
            ComponentFormKey = componentFormKey,
            ComponentIndex = componentIndex,
            Count = GetPropertyNullableInt(component, "Count") ?? GetPropertyNullableInt(componentData, "Count") ?? GetPropertyNullableInt(component, "RequiredCount"),
            ImportedAtUTC = importedAtUTC
        };
    }

    private static List<MiscItemComponentDTO> GetMiscItemComponents(PluginDTO plugin, FormKey formKey, object record)
    {
        var importedAtUTC = DateTime.UtcNow;
        var displayIndices = GetIndexedNullableInts(GetPropertyValue(record, "ComponentDisplayIndices"));
        return GetChildObjects(record, "Components")
            .Select((component, componentIndex) => CreateMiscItemComponent(
                plugin,
                formKey,
                component,
                componentIndex,
                displayIndices.TryGetValue(componentIndex, out var displayIndex) ? displayIndex : null,
                importedAtUTC))
            .Where(component => component != null)
            .Cast<MiscItemComponentDTO>()
            .ToList();
    }

    private static MiscItemComponentDTO? CreateMiscItemComponent(
        PluginDTO plugin,
        FormKey formKey,
        object component,
        int componentIndex,
        int? displayIndex,
        DateTime importedAtUTC)
    {
        var componentData = GetPropertyValue(component, "Component") ?? component;
        var componentFormKey = GetFormKeyFromObject(GetPropertyValue(componentData, "Component")) ?? GetFormKeyFromObject(componentData);
        if (componentFormKey == null)
        {
            return null;
        }

        return new MiscItemComponentDTO
        {
            Game = SupportedGame.Fallout4,
            ModKey = plugin.ModKey,
            FormKey = MapFormKey(formKey),
            Component = componentFormKey,
            ComponentIndex = componentIndex,
            DisplayIndex = displayIndex,
            Count = GetPropertyNullableInt(component, "Count") ?? GetPropertyNullableInt(componentData, "Count"),
            ImportedAtUTC = importedAtUTC
        };
    }

    private static MiscItemDestructibleDTO? GetMiscItemDestructible(object record)
    {
        var destructible = GetPropertyValue(record, "Destructible");
        if (destructible == null)
        {
            return null;
        }

        var data = GetPropertyValue(destructible, "Data");
        var stages = GetEnumerableObjects(GetPropertyValue(destructible, "Stages"))
            .Select((stage, stageIndex) => new MiscItemDestructibleStageDTO
            {
                StageIndex = stageIndex,
                Index = GetPropertyNullableInt(stage, "Index"),
                HealthPercent = GetPropertyNullableInt(stage, "HealthPercent"),
                ModelDamageStage = GetPropertyNullableInt(stage, "ModelDamageStage"),
                Flags = FormatEnumerable(GetPropertyValue(stage, "Flags")),
                SelfDamagePerSecond = GetPropertyNullableInt(stage, "SelfDamagePerSecond"),
                Explosion = GetFormKeyFromObject(GetPropertyValue(stage, "Explosion")),
                Model = GetMiscItemDestructibleStageModel(stage)
            })
            .ToList();

        return new MiscItemDestructibleDTO
        {
            Data = data == null
                ? null
                : new MiscItemDestructibleDataDTO
                {
                    Health = GetPropertyNullableInt(data, "Health"),
                    DESTCount = GetPropertyNullableInt(data, "DESTCount")
                },
            Stages = stages
        };
    }

    private static MiscItemDestructibleStageModelDTO? GetMiscItemDestructibleStageModel(object stage)
    {
        var model = GetPropertyValue(stage, "Model");
        if (model == null)
        {
            return null;
        }

        return new MiscItemDestructibleStageModelDTO
        {
            File = FormatSpriggitModelFilePath(GetPropertyValue(model, "File")?.ToString()),
            Data = FormatHexValue(GetPropertyValue(model, "Data"))
        };
    }

    private static IReadOnlyDictionary<int, int?> GetIndexedNullableInts(object? value)
    {
        return GetEnumerableObjects(value)
            .Select((item, index) => new { Index = index, Value = item == null ? (int?)null : Convert.ToInt32(item, CultureInfo.InvariantCulture) })
            .ToDictionary(item => item.Index, item => item.Value);
    }

    private static IReadOnlyList<object> GetChildObjects(object record, string preferredPropertyName)
    {
        var objects = new List<object>();
        AddEnumerablePropertyObjects(objects, GetPropertyValue(record, preferredPropertyName));
        if (objects.Count > 0)
        {
            return objects;
        }

        AddMatchingChildObjects(objects, record, item => GetPropertyValue(item, "Component") != null && GetPropertyValue(item, "Count") != null, 0);

        return objects;
    }

    private static void AddMatchingChildObjects(ICollection<object> objects, object? source, Func<object, bool> isMatch, int depth)
    {
        if (source == null || source is string || IsSimpleReflectionValue(source) || depth > 3)
        {
            return;
        }

        foreach (var item in GetEnumerableObjects(source))
        {
            AddMatchingChildObject(objects, item, isMatch, depth);
        }

        foreach (var property in source.GetType().GetProperties())
        {
            if (property.GetIndexParameters().Length > 0)
            {
                continue;
            }

            object? value;
            try
            {
                value = property.GetValue(source);
            }
            catch (TargetInvocationException)
            {
                continue;
            }

            foreach (var item in GetEnumerableObjects(value))
            {
                AddMatchingChildObject(objects, item, isMatch, depth + 1);
            }
        }
    }

    private static void AddMatchingChildObject(ICollection<object> objects, object item, Func<object, bool> isMatch, int depth)
    {
        if (IsSimpleReflectionValue(item))
        {
            return;
        }

        if (isMatch(item))
        {
            objects.Add(item);
            return;
        }

        AddMatchingChildObjects(objects, item, isMatch, depth + 1);
    }

    private static void AddEnumerablePropertyObjects(ICollection<object> objects, object? value)
    {
        foreach (var item in GetEnumerableObjects(value))
        {
            objects.Add(item);
        }
    }

    private static IEnumerable<object> GetEnumerableObjects(object? value)
    {
        return value is IEnumerable enumerable && value is not string
            ? enumerable.Cast<object>()
            : [];
    }

    private static bool IsSimpleReflectionValue(object value)
    {
        var type = value.GetType();
        return type.IsPrimitive ||
               type.IsEnum ||
               type == typeof(decimal) ||
               type == typeof(DateTime) ||
               type == typeof(Guid) ||
               type == typeof(FormKey) ||
               type == typeof(ModKey);
    }

    private static List<ConstructibleObjectCategoryDTO> GetConstructibleObjectCategories(PluginDTO plugin, FormKey formKey, object? categories)
    {
        if (categories is not IEnumerable enumerable) return new List<ConstructibleObjectCategoryDTO>();

        var importedAtUTC = DateTime.UtcNow;
        return enumerable
            .Cast<object>()
            .Select((category, categoryIndex) => GetFormKeyFromObject(category) is { } categoryFormKey
                ? new ConstructibleObjectCategoryDTO
                {
                    Game = SupportedGame.Fallout4,
                    ModKey = plugin.ModKey,
                    FormKey = MapFormKey(formKey),
                    CategoryFormKey = categoryFormKey,
                    CategoryIndex = categoryIndex,
                    ImportedAtUTC = importedAtUTC
                }
                : null)
            .Where(category => category != null)
            .Cast<ConstructibleObjectCategoryDTO>()
            .ToList();
    }

    private static int? GetFirstCount(object? counts)
    {
        if (counts is not IEnumerable enumerable)
        {
            return null;
        }

        return enumerable.Cast<object>().Select(count => GetPropertyNullableInt(count, "Count")).FirstOrDefault(count => count.HasValue);
    }

    private static List<ScriptFragmentDTO> GetScriptFragments(
        SupportedGame game,
        PluginDTO plugin,
        string recordType,
        FormKey formKey,
        object record)
    {
        var importedAtUTC = DateTime.UtcNow;
        var scriptFragments = GetPropertyValue(GetPropertyValue(record, "VirtualMachineAdapter"), "ScriptFragments");
        return ScriptFragmentDTOMapper.FromScriptFragments(game, plugin.ModKey, recordType, formKey, scriptFragments, importedAtUTC);
    }

    private static List<SoundMappingDTO> GetNamedSounds(PluginDTO plugin, string recordType, FormKey formKey, object record, params string[] soundSlots)
    {
        var importedAtUTC = DateTime.UtcNow;
        return soundSlots
            .Select((soundSlot, soundIndex) => CreateSoundMapping(plugin, recordType, formKey, soundSlot, soundIndex, GetPropertyValue(record, soundSlot), importedAtUTC))
            .Where(sound => sound != null)
            .Cast<SoundMappingDTO>()
            .ToList();
    }

    private static List<SoundMappingDTO> GetIndexedSounds(PluginDTO plugin, string recordType, FormKey formKey, object record)
    {
        var sounds = GetPropertyValue(record, "Sounds") as IEnumerable;
        if (sounds == null) return new List<SoundMappingDTO>();

        var importedAtUTC = DateTime.UtcNow;
        return sounds
            .Cast<object>()
            .Select((sound, soundIndex) => CreateSoundMapping(plugin, recordType, formKey, GetPropertyValue(sound, "Type")?.ToString() ?? $"Sound [{soundIndex}]", soundIndex, sound, importedAtUTC))
            .Where(sound => sound != null)
            .Cast<SoundMappingDTO>()
            .ToList();
    }

    private static SoundMappingDTO? CreateSoundMapping(PluginDTO plugin, string recordType, FormKey formKey, string soundSlot, int soundIndex, object? soundSource, DateTime importedAtUTC)
    {
        if (soundSource == null) return null;

        var start = GetSoundStart(soundSource);
        if (string.IsNullOrWhiteSpace(start)) return null;

        return new SoundMappingDTO
        {
            Game = SupportedGame.Fallout4,
            ModKey = plugin.ModKey,
            RecordType = recordType,
            FormKey = MapFormKey(formKey),
            SoundSlot = soundSlot,
            SoundIndex = soundIndex,
            Start = start,
            Stop = GetSoundStop(soundSource),
            Versioning = FormatEnumerable(GetPropertyValue(soundSource, "Versioning")),
            Unknown = FormatHexValue(GetPropertyValue(soundSource, "Unknown")),
            ImportedAtUTC = importedAtUTC
        };
    }

    private static string? GetSoundStart(object soundSource)
    {
        if (GetFormKeyFromObject(soundSource) is { } formKey)
        {
            return $"{formKey.Id:X6}:{formKey.ModKey.FileName}";
        }

        var directStart = GetPropertyValue(soundSource, "Start")?.ToString();
        if (!string.IsNullOrWhiteSpace(directStart)) return directStart;

        var sound = GetPropertyValue(soundSource, "Sound");
        if (sound == null) return null;

        if (GetFormKeyFromObject(sound) is { } soundFormKey)
        {
            return $"{soundFormKey.Id:X6}:{soundFormKey.ModKey.FileName}";
        }

        return GetPropertyValue(sound, "Start")?.ToString();
    }

    private static string? GetSoundStop(object soundSource)
    {
        var directStop = GetPropertyValue(soundSource, "Stop")?.ToString();
        if (!string.IsNullOrWhiteSpace(directStop)) return IsEmptyGuidText(directStop) ? null : directStop;

        var sound = GetPropertyValue(soundSource, "Sound");
        var stop = sound == null ? null : GetPropertyValue(sound, "Stop")?.ToString();
        return IsEmptyGuidText(stop) ? null : stop;
    }

    private static bool IsEmptyGuidText(string? value)
    {
        return string.Equals(value, "00000000-0000-0000-0000-000000000000", StringComparison.OrdinalIgnoreCase);
    }

    private static List<ScriptingAdapterDTO> GetScriptingAdapters(PluginDTO plugin, string recordType, object record)
    {
        var virtualMachineAdapter = record is IHaveVirtualMachineAdapterGetter scriptedRecord
            ? scriptedRecord.VirtualMachineAdapter
            : GetPropertyValue(record, "VirtualMachineAdapter");
        var scripts = GetPropertyValue(virtualMachineAdapter, "Scripts") as IEnumerable;
        if (scripts == null) return new List<ScriptingAdapterDTO>();

        var formKey = GetRequiredRawFormKey(record);
        var importedAtUTC = DateTime.UtcNow;
        return scripts
            .Cast<object>()
            .Select((script, scriptIndex) => new ScriptingAdapterDTO
            {
                Game = SupportedGame.Fallout4,
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
            .OrderBy(property => GetPropertyString(property, "Name"), StringComparer.OrdinalIgnoreCase)
            .Select((property, propertyIndex) => CreateScriptingAdapterProperty(plugin, recordType, formKey, GetPropertyString(script, "Name"), property, propertyIndex, importedAtUTC))
            .Where(property => property != null)
            .Cast<ScriptingAdapterPropertyDTO>()
            .ToList() ?? new List<ScriptingAdapterPropertyDTO>();
    }

    private static ScriptingAdapterPropertyDTO? CreateScriptingAdapterProperty(PluginDTO plugin, string recordType, FormKey formKey, string scriptName, object property, int propertyIndex, DateTime importedAtUTC)
    {
        var dto = new ScriptingAdapterPropertyDTO
        {
            Game = SupportedGame.Fallout4,
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
                Game = SupportedGame.Fallout4,
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
                Game = SupportedGame.Fallout4,
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

    protected virtual IFallout4ModGetter LoadMod(PluginDTO plugin)
    {
        var dataFolderPath = GetDataFolderPath();
        return Fallout4Mod.Create(Fallout4Release.Fallout4)
            .FromPath(Path.Combine(dataFolderPath, plugin.ModKey.FileName))
            .WithDataFolder(dataFolderPath)
            .Construct();
    }

    private string GetDataFolderPath()
    {
        var environment = GameEnvironment.Typical.Fallout4(Fallout4Release.Fallout4);
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

    private static FormKeyDTO? GetLinkedFormKey(object? source, string propertyName)
    {
        return GetFormKeyFromObject(GetPropertyValue(source, propertyName));
    }

    private static FormKeyDTO? GetFormKeyFromObject(object? value)
    {
        return GetFormKeyFromObject(value, 0);
    }

    private static List<FormKeyDTO> GetFormKeys(object? value)
    {
        return (value as IEnumerable)?.Cast<object>()
            .Select(item => GetFormKeyFromObject(item))
            .Where(formKey => formKey != null)
            .Cast<FormKeyDTO>()
            .ToList() ?? new List<FormKeyDTO>();
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
        if (property != null && property.GetIndexParameters().Length == 0)
        {
            return property.GetValue(source);
        }

        foreach (var interfaceType in sourceType.GetInterfaces())
        {
            property = interfaceType.GetProperty(propertyName);
            if (property != null && property.GetIndexParameters().Length == 0)
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

    private static string? GetPropertyStringOrNull(object? source, string propertyName)
    {
        return GetPropertyValue(source, propertyName)?.ToString();
    }

    private static string? GetNonDefaultFormKeyOrString(object? source, string propertyName)
    {
        return GetNonDefaultString(GetFormKeyOrString(source, propertyName));
    }

    private static string? GetNonDefaultString(object? value)
    {
        var text = value?.ToString();
        return string.IsNullOrWhiteSpace(text) ||
               text.StartsWith("Null<", StringComparison.Ordinal) ||
               string.Equals(text, "0", StringComparison.Ordinal) ||
               string.Equals(text, "0.0", StringComparison.Ordinal) ||
               string.Equals(text, "None", StringComparison.Ordinal) ||
               string.Equals(text, "Self", StringComparison.Ordinal)
            ? null
            : text;
    }

    private static int? GetPropertyNonZeroInt(object? source, string propertyName)
    {
        var value = GetPropertyNullableInt(source, propertyName);
        return value == 0 ? null : value;
    }

    private static long? GetPropertyNonZeroLong(object? source, string propertyName)
    {
        var value = GetPropertyValue(source, propertyName);
        if (value == null)
        {
            return null;
        }

        var longValue = Convert.ToInt64(value, CultureInfo.InvariantCulture);
        return longValue == 0 ? null : longValue;
    }

    private static float? GetPropertyNonZeroFloat(object? source, string propertyName)
    {
        var value = GetPropertyNullableFloat(source, propertyName);
        return value == 0 ? null : value;
    }

    private static string? GetFormKeyOrString(object? source, string propertyName)
    {
        var value = GetPropertyValue(source, propertyName);
        if (GetFormKeyFromObject(value) is { } formKey)
        {
            return FormatFormKey(formKey);
        }

        var text = value?.ToString();
        return text != null && (text.StartsWith("Null<", StringComparison.Ordinal) || string.Equals(text, "None", StringComparison.Ordinal))
            ? null
            : text;
    }

    private static string FormatFormKey(FormKeyDTO formKey)
    {
        return formKey.Id.ToString("X6", CultureInfo.InvariantCulture) + ":" + formKey.ModKey.FileName;
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

    private static float? GetPropertyNullableFloat(object? source, string propertyName)
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
        var text = type?.ToString();
        return string.Equals(text, "ValueModifier", StringComparison.Ordinal) ||
               string.Equals(text, "PeakValueModifier", StringComparison.Ordinal)
            ? null
            : text;
    }

    private static string? FormatEnumerable(object? value)
    {
        if (value is string text) return text;
        if (TryFormatFlagObject(value, out var flagText)) return flagText;
        return value is IEnumerable enumerable
            ? string.Join(", ", enumerable.Cast<object>().Select(item => item.ToString()))
            : value?.ToString();
    }

    private static bool TryFormatFlagObject(object? value, out string? flagText)
    {
        flagText = null;
        if (value == null || value is string || value is IEnumerable || value.GetType().IsEnum || value.GetType().IsPrimitive)
        {
            return false;
        }

        var type = value.GetType();
        if (!type.Name.Contains("Flag", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var flags = new List<string>();
        foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(property => property.GetIndexParameters().Length == 0))
        {
            var propertyValue = property.GetValue(value);
            if (propertyValue is bool boolValue)
            {
                if (boolValue)
                {
                    flags.Add(property.Name);
                }

                continue;
            }

            if (propertyValue is Enum enumValue && Convert.ToUInt64(enumValue, CultureInfo.InvariantCulture) != 0)
            {
                flags.Add(enumValue.ToString());
                continue;
            }

            if (propertyValue is IEnumerable enumerableValue and not string)
            {
                foreach (var item in enumerableValue.Cast<object>().Select(item => item.ToString()).Where(item => !string.IsNullOrWhiteSpace(item)))
                {
                    flags.Add(item!);
                }
            }
        }

        flagText = flags.Count == 0 ? null : string.Join(", ", flags);
        return true;
    }

    private static string GetSpriggitMutagenObjectType(object record)
    {
        var typeName = record.GetType().Name;
        const string binaryOverlaySuffix = "BinaryOverlay";
        return typeName.EndsWith(binaryOverlaySuffix, StringComparison.Ordinal)
            ? typeName[..^binaryOverlaySuffix.Length]
            : typeName;
    }

    private static string? FormatMajorFlags(object? value)
    {
        if (value == null)
        {
            return null;
        }

        if (value is IEnumerable enumerable && value is not string)
        {
            var flags = enumerable
                .Cast<object>()
                .Select(FormatSingleMajorFlag)
                .Where(flag => !string.IsNullOrWhiteSpace(flag))
                .ToList();
            return flags.Count == 0 ? null : string.Join(", ", flags);
        }

        if (!TryConvertToUInt64(value, out var numericValue) || numericValue == 0)
        {
            return null;
        }

        return FormatMajorFlagBits(numericValue);
    }

    private static string? FormatMajorFlagBits(ulong value)
    {
        var flags = new List<string>();
        for (var bit = 0; bit < 64; bit++)
        {
            var flag = 1UL << bit;
            if ((value & flag) != 0)
            {
                flags.Add("0x" + flag.ToString("X8", CultureInfo.InvariantCulture));
            }
        }

        return flags.Count == 0 ? null : string.Join(", ", flags);
    }

    private static string? FormatSingleMajorFlag(object value)
    {
        if (!TryConvertToUInt64(value, out var numericValue) || numericValue == 0)
        {
            return null;
        }

        return "0x" + numericValue.ToString("X8", CultureInfo.InvariantCulture);
    }

    private static bool TryConvertToUInt64(object value, out ulong result)
    {
        if (value is string text)
        {
            if (text.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                return ulong.TryParse(text[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result);
            }

            return ulong.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
        }

        try
        {
            result = Convert.ToUInt64(value, CultureInfo.InvariantCulture);
            return true;
        }
        catch (InvalidCastException)
        {
            result = 0;
            return false;
        }
        catch (FormatException)
        {
            result = 0;
            return false;
        }
        catch (OverflowException)
        {
            result = 0;
            return false;
        }
    }

    private static string FormatSpriggitColor(object? value)
    {
        var text = value?.ToString() ?? string.Empty;
        if (text.Length == 9 &&
            text[0] == '#' &&
            byte.TryParse(text.AsSpan(1, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var alpha) &&
            byte.TryParse(text.AsSpan(3, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var red) &&
            byte.TryParse(text.AsSpan(5, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var green) &&
            byte.TryParse(text.AsSpan(7, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var blue))
        {
            return $"Color [A={alpha}, R={red}, G={green}, B={blue}]";
        }

        return text;
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

    private static string? FormatReflectionPayload(object? value, int depth = 0)
    {
        if (value == null) return null;
        if (depth > 3) return value.GetType().Name;
        if (GetFormKeyFromObject(value) is { } formKey) return $"{formKey.ModKey.FileName}:{formKey.Id:X8}";
        if (value is string text) return text;
        if (value is byte[] bytes) return Convert.ToHexString(bytes);

        var type = value.GetType();
        if (type.IsPrimitive || value is decimal || value is DateTime || value is Guid || type.IsEnum)
        {
            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        if (value is IEnumerable enumerable)
        {
            return string.Join(
                "; ",
                enumerable.Cast<object>()
                    .Select((item, index) => "[" + index.ToString(CultureInfo.InvariantCulture) + "]=" + (FormatReflectionPayload(item, depth + 1) ?? string.Empty)));
        }

        var parts = new List<string>();
        foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                     .Where(property => property.GetIndexParameters().Length == 0)
                     .OrderBy(property => property.Name, StringComparer.Ordinal))
        {
            object? propertyValue;
            try
            {
                propertyValue = property.GetValue(value);
            }
            catch (TargetInvocationException)
            {
                continue;
            }
            catch (TargetParameterCountException)
            {
                continue;
            }

            var formattedValue = FormatReflectionPayload(propertyValue, depth + 1);
            if (!string.IsNullOrWhiteSpace(formattedValue))
            {
                parts.Add(property.Name + "=" + formattedValue);
            }
        }

        return parts.Count == 0 ? value.ToString() : string.Join("; ", parts);
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

    private static GameSettingDataType GetGameSettingDataType(IGameSettingGetter record)
    {
        return record switch
        {
            IGameSettingBoolGetter => GameSettingDataType.Boolean,
            IGameSettingFloatGetter => GameSettingDataType.Float,
            IGameSettingIntGetter => GameSettingDataType.Integer,
            IGameSettingStringGetter => GameSettingDataType.String,
            IGameSettingUIntGetter => GameSettingDataType.UnsignedInteger,
            _ => throw new NotSupportedException($"Unsupported game setting type '{record.GetType().Name}'.")
        };
    }

    private static GameSettingDataDTO GetGameSettingData(IGameSettingGetter record)
    {
        var dataType = GetGameSettingDataType(record);
        return record switch
        {
            IGameSettingBoolGetter gameSetting => new GameSettingDataDTO { DataType = dataType, Boolean = gameSetting.Data },
            IGameSettingFloatGetter gameSetting => new GameSettingDataDTO { DataType = dataType, Float = gameSetting.Data },
            IGameSettingIntGetter gameSetting => new GameSettingDataDTO { DataType = dataType, Integer = gameSetting.Data },
            IGameSettingStringGetter gameSetting => new GameSettingDataDTO { DataType = dataType, String = LocalizedStringDTOMapper.ToTranslatedStringDTO(gameSetting.Data) },
            IGameSettingUIntGetter gameSetting => new GameSettingDataDTO { DataType = dataType, UnsignedInteger = gameSetting.Data },
            _ => new GameSettingDataDTO { DataType = dataType }
        };
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
