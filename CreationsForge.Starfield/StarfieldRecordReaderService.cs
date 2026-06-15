using System.Globalization;
using System.Collections;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Utilities;
using CreationsForge.Starfield.Interfaces;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Starfield;

public class StarfieldRecordReaderService : IStarfieldRecordReaderService
{
    private readonly StarfieldGameMetadataService GameMetadataService;

    public StarfieldRecordReaderService(StarfieldGameMetadataService gameMetadataService)
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
        var statics = MapStaticModelRecords(plugin, mod);
        cancellationToken.ThrowIfCancellationRequested();
        var books = MapBooks(plugin, mod);
        cancellationToken.ThrowIfCancellationRequested();
        var doors = MapDoors(plugin, mod);
        cancellationToken.ThrowIfCancellationRequested();
        var containers = MapContainers(plugin, mod);
        cancellationToken.ThrowIfCancellationRequested();
        var conditionForms = MapConditionForms(plugin, mod);
        cancellationToken.ThrowIfCancellationRequested();
        var constructibleObjects = MapConstructibleObjects(plugin, mod);
        cancellationToken.ThrowIfCancellationRequested();
        var terminals = MapTerminals(plugin, mod);

        return new PluginRecordSetDTO
        {
            FormLists = formLists,
            GameSettings = gameSettings,
            Globals = globals,
            MiscObjects = miscObjects,
            Keywords = keywords,
            ActorValueInformation = actorValueInformation,
            NPCs = npcs,
            MagicEffects = magicEffects,
            Perks = perks,
            Statics = statics,
            Books = books,
            Doors = doors,
            Containers = containers,
            ConditionForms = conditionForms,
            ConstructibleObjects = constructibleObjects,
            Terminals = terminals
        };
    }

    public IReadOnlyList<FormListDTO> ReadFormLists(PluginDTO plugin)
    {
        var mod = LoadMod(plugin);
        return MapFormLists(plugin, mod);
    }

    private static IReadOnlyList<FormListDTO> MapFormLists(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return mod.FormLists
            .Select(record => new FormListDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.StarfieldMajorRecordFlags,
                ImportedAtUTC = DateTime.UtcNow,
                AddToListFormKey = GetFormKeyFromObject(GetPropertyValue(record, "AddToList")),
                Items = record.Items.Select((item, itemIndex) => new FormListItemDTO
                {
                    Game = SupportedGame.Starfield,
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

    private static IReadOnlyList<GameSettingDTO> MapGameSettings(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return mod.GameSettings
            .Select(record => new GameSettingDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.StarfieldMajorRecordFlags,
                ImportedAtUTC = DateTime.UtcNow,
                SettingType = GetGameSettingType(record),
                Data = GetGameSettingData(record),
                NumericData = GetGameSettingNumericData(record),
                IntegerData = GetGameSettingIntegerData(record),
                BooleanData = GetGameSettingBooleanData(record)
            })
            .ToList();
    }

    public IReadOnlyList<GlobalDTO> ReadGlobals(PluginDTO plugin)
    {
        var mod = LoadMod(plugin);
        return MapGlobals(plugin, mod);
    }

    private static IReadOnlyList<GlobalDTO> MapGlobals(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return mod.Globals
            .Select(record => new GlobalDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.StarfieldMajorRecordFlags,
                ImportedAtUTC = DateTime.UtcNow,
                Data = record.Data,
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.Global.RecordID, record)
            })
            .ToList();
    }

    private static IReadOnlyList<KeywordDTO> MapKeywords(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return mod.Keywords
            .Select(record => new KeywordDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.StarfieldMajorRecordFlags,
                ImportedAtUTC = DateTime.UtcNow,
                Name = record.Name?.Lookup(Language.English),
                Color = record.Color.ToString() ?? string.Empty,
                Type = record.Type.ToString() ?? string.Empty,
                Notes = record.Notes,
                FlashLinkageName = record.FlashLinkageName,
                AttractionRuleFormKey = record.AttractionRule.IsNull ? null : MapFormKey(record.AttractionRule.FormKey),
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.Keyword.RecordID, record)
            })
            .ToList();
    }

    private static IReadOnlyList<MiscObjectDTO> MapMiscObjects(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return mod.MiscItems
            .Select(record => new MiscObjectDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.StarfieldMajorRecordFlags,
                ImportedAtUTC = DateTime.UtcNow,
                Name = record.Name?.Lookup(Language.English),
                ShortName = record.ShortName?.Lookup(Language.English),
                Value = record.Value,
                Weight = record.Weight,
                DirtinessScale = (float)record.DirtinessScale,
                FeaturedItemMessageFormKey = record.FeaturedItemMessage.IsNull ? null : MapFormKey(record.FeaturedItemMessage.FormKey),
                Flag = record.FLAG == null ? null : Convert.ToHexString(record.FLAG.Value.ToArray()),
                Models = GetModels(plugin, RecordTypeCatalog.MiscObject.RecordID, record.FormKey, record.Model),
                Keywords = GetRecordKeywords(plugin, RecordTypeCatalog.MiscObject.RecordID, record.FormKey, record.Keywords),
                Sounds = GetNamedSounds(plugin, RecordTypeCatalog.MiscObject.RecordID, record.FormKey, record, "CraftingSound", "PickupSound", "DropdownSound"),
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.MiscObject.RecordID, record)
            })
            .ToList();
    }

    private static IReadOnlyList<ActorValueInformationDTO> MapActorValueInformation(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return mod.ActorValueInformation
            .Select(record => new ActorValueInformationDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.StarfieldMajorRecordFlags,
                ImportedAtUTC = DateTime.UtcNow,
                Name = record.Name?.Lookup(Language.English),
                Abbreviation = record.Abbreviation?.Lookup(Language.English),
                ContextNotes = record.ContextNotes,
                DefaultValue = record.DefaultValue,
                Flags = record.Flags.ToString(),
                Type = record.Type?.ToString(),
                Min = record.Min,
                Max = record.Max,
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.ActorValueInformation.RecordID, record)
            })
            .ToList();
    }

    private static IReadOnlyList<StaticDTO> MapStaticModelRecords(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return mod.Statics
            .Select(record => new StaticDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.StarfieldMajorRecordFlags,
                ImportedAtUTC = DateTime.UtcNow,
                Version2 = GetPropertyNullableInt(record, "Version2"),
                ObjectBoundsFirst = FormatObjectBoundsPoint(GetPropertyValue(record, "ObjectBounds"), "First"),
                ObjectBoundsSecond = FormatObjectBoundsPoint(GetPropertyValue(record, "ObjectBounds"), "Second"),
                MaxAngle = GetPropertyNullableDouble(record, "MaxAngle"),
                UnknownDNAMFloat = GetPropertyNullableDouble(record, "UnknownDNAMFloat"),
                DNAMDataTypeState = FormatEnumerable(GetPropertyValue(record, "DNAMDataTypeState")),
                Models = GetModels(plugin, RecordTypeCatalog.Static.RecordID, record.FormKey, record.Model),
                Keywords = GetRecordKeywordsFromNestedKeywordLists(plugin, RecordTypeCatalog.Static.RecordID, record.FormKey, GetPropertyValue(record, "Components")),
                RawPayloads = GetStaticRawPayloads(plugin, record.FormKey, record.Model, GetPropertyValue(record, "Components"))
            })
            .ToList();
    }

    private static IReadOnlyList<BookDTO> MapBooks(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return mod.Books
            .Select(record => new BookDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.StarfieldMajorRecordFlags,
                ImportedAtUTC = DateTime.UtcNow,
                Version2 = GetPropertyNullableInt(record, "Version2"),
                ObjectBoundsFirst = FormatObjectBoundsPoint(GetPropertyValue(record, "ObjectBounds"), "First"),
                ObjectBoundsSecond = FormatObjectBoundsPoint(GetPropertyValue(record, "ObjectBounds"), "Second"),
                InventoryTransformFormKey = GetFormKeyFromObject(GetPropertyValue(GetPropertyValue(record, "Transforms"), "Inventory")),
                Xalg = GetPropertyNullableInt(record, "XALG"),
                Name = record.Name?.Lookup(Language.English),
                Text = GetLocalizedEnglishText(GetPropertyValue(record, "BookTextOverride"))
                    ?? GetLocalizedEnglishText(GetPropertyValue(record, "Text")),
                Value = GetPropertyNullableInt(record, "Value"),
                Weight = GetPropertyNullableFloat(record, "Weight"),
                Flags = FormatEnumerable(GetPropertyValue(record, "Flags")),
                TeachesType = GetPropertyValue(GetPropertyValue(record, "Teaches"), "Type")?.ToString(),
                TeachesRawContent = FormatEnumerable(GetPropertyValue(record, "Teaches")),
                DataSlateType = GetPropertyValue(record, "DataSlateType")?.ToString(),
                Description = GetLocalizedEnglishText(GetPropertyValue(record, "Description")),
                DataSlateHeaderLeft = GetLocalizedEnglishText(GetPropertyValue(record, "DataSlateHeaderLeft")),
                DataSlateHeaderRight = GetLocalizedEnglishText(GetPropertyValue(record, "DataSlateHeaderRight")),
                Models = GetModels(plugin, RecordTypeCatalog.Book.RecordID, record.FormKey, record.Model),
                Keywords = GetRecordKeywords(plugin, RecordTypeCatalog.Book.RecordID, record.FormKey, record.Keywords),
                Sounds = GetNamedSounds(plugin, RecordTypeCatalog.Book.RecordID, record.FormKey, record, "PickupSound", "DropdownSound"),
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.Book.RecordID, record),
                RawPayloads = GetBookRawPayloads(plugin, record.FormKey, record)
            })
            .ToList();
    }

    private static IReadOnlyList<DoorDTO> MapDoors(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return mod.Doors
            .Select(record => new DoorDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.StarfieldMajorRecordFlags,
                ImportedAtUTC = DateTime.UtcNow,
                Version2 = GetPropertyNullableInt(record, "Version2"),
                ObjectBoundsFirst = FormatObjectBoundsPoint(GetPropertyValue(record, "ObjectBounds"), "First"),
                ObjectBoundsSecond = FormatObjectBoundsPoint(GetPropertyValue(record, "ObjectBounds"), "Second"),
                Name = record.Name?.Lookup(Language.English),
                Flags = FormatEnumerable(GetPropertyValue(record, "Flags")),
                NativeTerminalFormKey = record.NativeTerminal.IsNull ? null : MapFormKey(record.NativeTerminal.FormKey),
                SoundLevel = GetPropertyValue(record, "SoundLevel")?.ToString(),
                FacingAxisOverride = GetPropertyValue(record, "FacingAxisOverride")?.ToString(),
                Models = GetModels(plugin, RecordTypeCatalog.Door.RecordID, record.FormKey, record.Model),
                Keywords = GetRecordKeywords(plugin, RecordTypeCatalog.Door.RecordID, record.FormKey, record.Keywords),
                Sounds = GetNamedSounds(plugin, RecordTypeCatalog.Door.RecordID, record.FormKey, record, "OpenSound", "CloseSound"),
                RawPayloads = GetDoorRawPayloads(plugin, record.FormKey, record.Model, GetPropertyValue(record, "Components"))
            })
            .ToList();
    }

    private static IReadOnlyList<ContainerDTO> MapContainers(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return mod.Containers
            .Select(record => new ContainerDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.StarfieldMajorRecordFlags,
                ImportedAtUTC = DateTime.UtcNow,
                Version2 = GetPropertyNullableInt(record, "Version2"),
                ObjectBoundsFirst = FormatObjectBoundsPoint(GetPropertyValue(record, "ObjectBounds"), "First"),
                ObjectBoundsSecond = FormatObjectBoundsPoint(GetPropertyValue(record, "ObjectBounds"), "Second"),
                Name = record.Name?.Lookup(Language.English),
                Flags = FormatEnumerable(GetPropertyValue(record, "Flags")),
                MajorFlags = FormatEnumerable(GetPropertyValue(record, "MajorFlags")),
                NativeTerminalFormKey = record.NativeTerminal.IsNull ? null : MapFormKey(record.NativeTerminal.FormKey),
                Items = GetContainerItems(plugin, record.FormKey, GetPropertyValue(record, "Items")),
                Models = GetModels(plugin, RecordTypeCatalog.Container.RecordID, record.FormKey, record.Model),
                Keywords = GetRecordKeywords(plugin, RecordTypeCatalog.Container.RecordID, record.FormKey, record.Keywords)
                    .Concat(GetRecordKeywordsFromNestedKeywordLists(plugin, RecordTypeCatalog.Container.RecordID, record.FormKey, GetPropertyValue(record, "Components")))
                    .Select((keyword, keywordIndex) =>
                    {
                        keyword.KeywordIndex = keywordIndex;
                        return keyword;
                    })
                    .ToList(),
                RawPayloads = GetContainerRawPayloads(plugin, record.FormKey, record.Model, GetPropertyValue(record, "Components"))
            })
            .ToList();
    }

    private static IReadOnlyList<ConstructibleObjectDTO> MapConstructibleObjects(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return mod.ConstructibleObjects
            .Select(record => new ConstructibleObjectDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.StarfieldMajorRecordFlags,
                ImportedAtUTC = DateTime.UtcNow,
                Version2 = GetPropertyNullableInt(record, "Version2"),
                Description = GetLocalizedEnglishText(GetPropertyValue(record, "Description")),
                CreatedObjectFormKey = GetFormKeyFromObject(GetPropertyValue(record, "CreatedObject")),
                WorkbenchKeywordFormKey = GetFormKeyFromObject(GetPropertyValue(record, "WorkbenchKeyword")),
                AmountProduced = GetPropertyNullableInt(record, "AmountProduced"),
                MenuSortOrder = GetPropertyNullableInt(record, "MenuSortOrder"),
                LearnMethod = GetPropertyValue(record, "LearnMethod")?.ToString(),
                Flags = FormatEnumerable(GetPropertyValue(record, "Flags")),
                Components = GetConstructibleObjectComponents(plugin, record.FormKey, GetPropertyValue(record, "ConstructableComponents")),
                RecipeFilters = GetConstructibleObjectRecipeFilters(plugin, record.FormKey, GetPropertyValue(record, "RecipeFilters")),
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.ConstructibleObject.RecordID, record),
                RawPayloads = GetConstructibleObjectRawPayloads(plugin, record.FormKey, record)
            })
            .ToList();
    }

    private static IReadOnlyList<ConditionFormDTO> MapConditionForms(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return GetRecordCollection(mod, "ConditionRecords")
            .Select(record =>
            {
                var formKey = GetRequiredFormKey(record);
                return new ConditionFormDTO
                {
                    Game = SupportedGame.Starfield,
                    ModKey = plugin.ModKey,
                    FormKey = MapFormKey(formKey),
                    EditorID = GetPropertyValue(record, "EditorID")?.ToString() ?? string.Empty,
                    FormVersion = GetPropertyNullableInt(record, "FormVersion") ?? 0,
                    MajorRecordFlags = GetPropertyNullableInt(record, "StarfieldMajorRecordFlags") ?? 0,
                    ImportedAtUTC = DateTime.UtcNow,
                    Version2 = GetPropertyNullableInt(record, "Version2"),
                    RawPayloads = GetConditionFormRawPayloads(plugin, formKey, record)
                };
            })
            .ToList();
    }

    private static IReadOnlyList<TerminalDTO> MapTerminals(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return mod.Terminals
            .Select(record => new TerminalDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.StarfieldMajorRecordFlags,
                ImportedAtUTC = DateTime.UtcNow,
                Version2 = GetPropertyNullableInt(record, "Version2"),
                ObjectBoundsFirst = FormatObjectBoundsPoint(GetPropertyValue(record, "ObjectBounds"), "First"),
                ObjectBoundsSecond = FormatObjectBoundsPoint(GetPropertyValue(record, "ObjectBounds"), "Second"),
                MenuFormKey = GetFormKeyFromObject(GetPropertyValue(record, "Menu")),
                Background = GetPropertyValue(record, "Background")?.ToString(),
                Name = record.Name?.Lookup(Language.English),
                Pnam = GetPropertyValue(record, "PNAM")?.ToString(),
                Fnam = GetPropertyValue(record, "FNAM")?.ToString(),
                Jnam = GetPropertyValue(record, "JNAM")?.ToString(),
                MarkerFlags = GetPropertyNullableLong(record, "MarkerFlags"),
                Gnam = GetPropertyValue(record, "GNAM")?.ToString(),
                WorkbenchData = FormatEnumerable(GetPropertyValue(record, "WorkbenchData")),
                FurnitureTemplateFormKey = GetFormKeyFromObject(GetPropertyValue(record, "FurnitureTemplate")),
                MarkerModel = GetPropertyValue(record, "MarkerModel")?.ToString(),
                Models = GetModels(plugin, RecordTypeCatalog.Terminal.RecordID, record.FormKey, record.Model),
                Keywords = GetRecordKeywords(plugin, RecordTypeCatalog.Terminal.RecordID, record.FormKey, record.Keywords),
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.Terminal.RecordID, record),
                RawPayloads = GetTerminalRawPayloads(plugin, record.FormKey, record.Model, GetPropertyValue(record, "Components")),
                MarkerParameters = GetTerminalMarkerParameters(plugin, record.FormKey, GetPropertyValue(record, "MarkerParameters"))
            })
            .ToList();
    }

    private static IReadOnlyList<NPCDTO> MapNPCs(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return mod.Npcs
            .Select(record => new NPCDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.StarfieldMajorRecordFlags,
                ImportedAtUTC = DateTime.UtcNow,
                Name = record.Name?.Lookup(Language.English),
                ShortName = record.ShortName?.Lookup(Language.English),
                LongName = record.LongName?.Lookup(Language.English),
                DispositionBase = record.DispositionBase,
                Aggression = record.Aggression.ToString(),
                Confidence = record.Confidence.ToString(),
                EnergyLevel = record.EnergyLevel,
                Responsibility = record.Responsibility.ToString(),
                Assistance = record.Assistance.ToString(),
                GearedUpWeapons = record.GearedUpWeapons,
                HeightMin = record.HeightMin,
                HeightMax = record.HeightMax,
                SkinToneIndex = record.SkinToneIndex,
                Pronoun = record.Pronoun?.ToString(),
                VoiceFormKey = record.Voice.IsNull ? null : MapFormKey(record.Voice.FormKey),
                RaceFormKey = record.Race.IsNull ? null : MapFormKey(record.Race.FormKey),
                CombatOverridePackageListFormKey = record.CombatOverridePackageList.IsNull ? null : MapFormKey(record.CombatOverridePackageList.FormKey),
                CombatStyleFormKey = record.CombatStyle.IsNull ? null : MapFormKey(record.CombatStyle.FormKey),
                DefaultPackageListFormKey = record.DefaultPackageList.IsNull ? null : MapFormKey(record.DefaultPackageList.FormKey),
                CrimeFactionFormKey = record.CrimeFaction.IsNull ? null : MapFormKey(record.CrimeFaction.FormKey),
                Keywords = GetRecordKeywords(plugin, RecordTypeCatalog.NPC.RecordID, record.FormKey, record.Keywords),
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.NPC.RecordID, record)
            })
            .ToList();
    }

    private static IReadOnlyList<MagicEffectDTO> MapMagicEffects(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return mod.MagicEffects
            .Select(record => new MagicEffectDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.StarfieldMajorRecordFlags,
                ImportedAtUTC = DateTime.UtcNow,
                Name = GetLocalizedEnglishText(() => record.Name),
                Description = GetLocalizedEnglishText(() => record.Description),
                Flags = record.Flags.ToString(),
                CastType = record.CastType.ToString(),
                TargetType = record.TargetType.ToString(),
                ActorValue2FormKey = record.ActorValue2.IsNull ? null : MapFormKey(record.ActorValue2.FormKey),
                ResistValueFormKey = record.ResistValue.IsNull ? null : MapFormKey(record.ResistValue.FormKey),
                PerkToApplyFormKey = record.PerkToApply.IsNull ? null : MapFormKey(record.PerkToApply.FormKey),
                EquipAbilityFormKey = record.EquipAbility.IsNull ? null : MapFormKey(record.EquipAbility.FormKey),
                ExplosionFormKey = record.Explosion.IsNull ? null : MapFormKey(record.Explosion.FormKey),
                CastingArtFormKey = record.CastingArt.IsNull ? null : MapFormKey(record.CastingArt.FormKey),
                HitEffectArtFormKey = record.HitEffectArt.IsNull ? null : MapFormKey(record.HitEffectArt.FormKey),
                HitShaderFormKey = record.HitShader.IsNull ? null : MapFormKey(record.HitShader.FormKey),
                ImageSpaceModifierFormKey = record.ImageSpaceModifier.IsNull ? null : MapFormKey(record.ImageSpaceModifier.FormKey),
                ImpactDataFormKey = record.ImpactData.IsNull ? null : MapFormKey(record.ImpactData.FormKey),
                ProjectileFormKey = record.Projectile.IsNull ? null : MapFormKey(record.Projectile.FormKey),
                Archetype = GetMagicEffectArchetype(record),
                UnknownFloat3 = record.UnknownFloat3,
                UnknownInt2 = unchecked((int)record.UnknownInt2),
                Unknown = Convert.ToHexString(record.Unknown.ToArray()),
                Unknown2 = Convert.ToHexString(record.Unknown2.ToArray()),
                DataTypeState = record.DATADataTypeState.ToString(),
                Keywords = GetRecordKeywords(plugin, RecordTypeCatalog.MagicEffect.RecordID, record.FormKey, record.Keywords),
                Sounds = GetIndexedSounds(plugin, RecordTypeCatalog.MagicEffect.RecordID, record.FormKey, record),
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.MagicEffect.RecordID, record)
            })
            .ToList();
    }

    private static IReadOnlyList<PerkDTO> MapPerks(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return mod.Perks
            .Select(record => new PerkDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.StarfieldMajorRecordFlags,
                ImportedAtUTC = DateTime.UtcNow,
                Name = record.Name?.Lookup(Language.English),
                Description = record.Description?.Lookup(Language.English),
                Flags = record.Flags.ToString(),
                SkillGroup = record.SkillGroup.ToString(),
                CrewAssignment = record.CrewAssignment.ToString(),
                PerkIcon = record.PerkIcon,
                Category = record.Categroy.ToString(),
                RestrictionFormKey = record.Restriction.IsNull ? null : MapFormKey(record.Restriction.FormKey),
                TrainingFormKey = record.Training.IsNull ? null : MapFormKey(record.Training.FormKey),
                MajorFlags = record.MajorFlags.ToString(),
                Ranks = GetPerkRanks(plugin, record),
                BackgroundSkills = GetPerkBackgroundSkills(plugin, record),
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.Perk.RecordID, record)
            })
            .ToList();
    }

    private static List<PerkRankDTO> GetPerkRanks(PluginDTO plugin, IPerkGetter record)
    {
        var importedAtUTC = DateTime.UtcNow;
        return record.Ranks
            .Select((rank, rankIndex) => new PerkRankDTO
            {
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                RankIndex = rankIndex,
                Description = GetLocalizedEnglishText(() => rank.Description),
                UnknownStaticFormKey = rank.UnknownStatic.IsNull ? null : MapFormKey(rank.UnknownStatic.FormKey),
                ConditionCount = rank.Conditions?.Count ?? 0,
                ActivityCount = rank.Activities?.Count ?? 0,
                ImportedAtUTC = importedAtUTC,
                Effects = GetPerkRankEffects(plugin, record.FormKey, rank, rankIndex, importedAtUTC)
            })
            .ToList();
    }

    private static List<PerkRankEffectDTO> GetPerkRankEffects(PluginDTO plugin, FormKey formKey, IPerkRankGetter rank, int rankIndex, DateTime importedAtUTC)
    {
        return rank.Effects
            .Select((effect, effectIndex) =>
            {
                var dto = new PerkRankEffectDTO
                {
                    ModKey = plugin.ModKey,
                    FormKey = MapFormKey(formKey),
                    RankIndex = rankIndex,
                    EffectIndex = effectIndex,
                    MutagenObjectType = effect.GetType().Name,
                    Rank = effect.Rank,
                    Priority = effect.Priority,
                    PerkEntryId = effect.PerkEntryID,
                    Flags = effect.Flags?.ToString(),
                    ButtonLabel = GetLocalizedEnglishText(() => effect.ButtonLabel),
                    ConditionCount = effect.Conditions.Count,
                    ImportedAtUTC = importedAtUTC
                };

                if (effect is IAPerkEntryPointEffectGetter entryPointEffect)
                {
                    dto.EntryPoint = entryPointEffect.EntryPoint.ToString();
                    dto.PerkConditionTabCount = entryPointEffect.PerkConditionTabCount;
                }

                if (effect is IPerkEntryPointModifyValueGetter modifyValueEffect)
                {
                    dto.Modification = modifyValueEffect.Modification.ToString();
                    dto.Value = modifyValueEffect.Value;
                }

                return dto;
            })
            .ToList();
    }

    private static List<PerkBackgroundSkillDTO> GetPerkBackgroundSkills(PluginDTO plugin, IPerkGetter record)
    {
        var importedAtUTC = DateTime.UtcNow;
        return record.BackgroundSkills
            .Select((skill, skillIndex) => new PerkBackgroundSkillDTO
            {
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                SkillFormKey = MapFormKey(skill.FormKey),
                SkillIndex = skillIndex,
                ImportedAtUTC = importedAtUTC
            })
            .ToList();
    }

    private static string? GetLocalizedEnglishText(Func<ITranslatedStringGetter?> valueFactory)
    {
        try
        {
            return valueFactory()?.Lookup(Language.English);
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    private static string? GetLocalizedEnglishText(object? value)
    {
        return value is ITranslatedStringGetter translatedString
            ? GetLocalizedEnglishText(() => translatedString)
            : value?.ToString();
    }

    private static List<ScriptingAdapterDTO> GetScriptingAdapters(PluginDTO plugin, string recordType, IHaveVirtualMachineAdapterGetter record)
    {
        if (record.VirtualMachineAdapter == null) return new List<ScriptingAdapterDTO>();

        var importedAtUTC = DateTime.UtcNow;
        return record.VirtualMachineAdapter.Scripts
            .Select((script, scriptIndex) => new ScriptingAdapterDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                RecordType = recordType,
                FormKey = MapFormKey(record.FormKey),
                Name = script.Name,
                ScriptIndex = scriptIndex,
                ImportedAtUTC = importedAtUTC,
                Properties = GetScriptingAdapterProperties(plugin, recordType, record.FormKey, script, importedAtUTC)
            })
            .ToList();
    }

    private static List<ModelDTO> GetModels(PluginDTO plugin, string recordType, FormKey formKey, IModelGetter? model)
    {
        if (model == null) return new List<ModelDTO>();

        var importedAtUTC = DateTime.UtcNow;
        return new List<ModelDTO>
        {
            new ModelDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                RecordType = recordType,
                FormKey = MapFormKey(formKey),
                ModelSlot = "Model",
                ModelGender = string.Empty,
                  File = model.File?.ToString(),
                TextureFileHashes = model.TextureFileHashes == null ? null : Convert.ToHexString(model.TextureFileHashes.Value.ToArray()),
                LightLayer = model.LightLayer,
                Flags = model.Flags?.ToString(),
                ColorRemappingIndex = model.ColorRemappingIndex,
                FlagsVestigial = model.FlagsVestigial?.ToString(),
                ImportedAtUTC = importedAtUTC,
                MaterialSwaps = (model.MaterialSwaps ?? []).Select((materialSwap, materialSwapIndex) => new ModelMaterialSwapDTO
                {
                    Game = SupportedGame.Starfield,
                    ModKey = plugin.ModKey,
                    RecordType = recordType,
                    FormKey = MapFormKey(formKey),
                    ModelSlot = "Model",
                    ModelGender = string.Empty,
                    MaterialSwapFormKey = MapFormKey(materialSwap.FormKey),
                    MaterialSwapIndex = materialSwapIndex,
                    ImportedAtUTC = importedAtUTC
                }).ToList()
            }
        };
    }

    private static List<RecordKeywordDTO> GetRecordKeywords(PluginDTO plugin, string recordType, FormKey formKey, IEnumerable<IFormLinkGetter<IKeywordGetter>>? keywords)
    {
        if (keywords == null) return new List<RecordKeywordDTO>();

        var importedAtUTC = DateTime.UtcNow;
        return keywords
            .Select((keyword, keywordIndex) => new RecordKeywordDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                RecordType = recordType,
                FormKey = MapFormKey(formKey),
                KeywordFormKey = MapFormKey(keyword.FormKey),
                KeywordIndex = keywordIndex,
                ImportedAtUTC = importedAtUTC
            })
            .ToList();
    }

    private static List<RecordKeywordDTO> GetRecordKeywordsFromNestedKeywordLists(PluginDTO plugin, string recordType, FormKey formKey, object? keywordSources)
    {
        if (keywordSources is not IEnumerable enumerable) return new List<RecordKeywordDTO>();

        var importedAtUTC = DateTime.UtcNow;
        var keywords = new List<RecordKeywordDTO>();
        foreach (var keywordSource in enumerable.Cast<object>())
        {
            var nestedKeywords = GetPropertyValue(keywordSource, "Keywords") as IEnumerable;
            if (nestedKeywords == null)
            {
                continue;
            }

            foreach (var keyword in nestedKeywords.Cast<object>())
            {
                if (GetFormKeyFromObject(keyword) is not { } keywordFormKey)
                {
                    continue;
                }

                keywords.Add(new RecordKeywordDTO
                {
                    Game = SupportedGame.Starfield,
                    ModKey = plugin.ModKey,
                    RecordType = recordType,
                    FormKey = MapFormKey(formKey),
                    KeywordFormKey = keywordFormKey,
                    KeywordIndex = keywords.Count,
                    ImportedAtUTC = importedAtUTC
                });
            }
        }

        return keywords;
    }

    private static List<RawRecordPayloadDTO> GetStaticRawPayloads(PluginDTO plugin, FormKey formKey, object? model, object? components)
    {
        var importedAtUTC = DateTime.UtcNow;
        var payloads = new List<RawRecordPayloadDTO>();
        AddRawPayload(payloads, plugin, RecordTypeCatalog.Static.RecordID, formKey, "Model.Data", 0, model?.GetType().Name ?? "Model", FormatHexValue(GetPropertyValue(model, "Data")), importedAtUTC);

        if (components is IEnumerable enumerable)
        {
            foreach (var component in enumerable.Cast<object>().Select((value, index) => new { value, index }))
            {
                AddRawPayload(
                    payloads,
                    plugin,
                    RecordTypeCatalog.Static.RecordID,
                    formKey,
                    "BaseFormComponents.REFL",
                    component.index,
                    component.value.GetType().Name,
                    FormatHexValue(GetPropertyValue(component.value, "REFL")),
                    importedAtUTC,
                    "Components.REFL");
            }
        }

        return payloads;
    }

    private static List<RawRecordPayloadDTO> GetBookRawPayloads(PluginDTO plugin, FormKey formKey, object record)
    {
        var importedAtUTC = DateTime.UtcNow;
        var payloads = new List<RawRecordPayloadDTO>();
        AddRawPayload(payloads, plugin, RecordTypeCatalog.Book.RecordID, formKey, "Model.Data", 0, record.GetType().Name, FormatHexValue(GetPropertyValue(GetPropertyValue(record, "Model"), "Data")), importedAtUTC);
        AddRawPayload(payloads, plugin, RecordTypeCatalog.Book.RecordID, formKey, "BaseFormComponents", 0, "Components", FormatEnumerable(GetPropertyValue(record, "Components")), importedAtUTC, "Components");
        return payloads;
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
            Game = SupportedGame.Starfield,
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
        var componentFormKey = GetFormKeyFromObject(GetPropertyValue(component, "Component")) ?? GetFormKeyFromObject(component);
        if (componentFormKey == null)
        {
            return null;
        }

        return new ConstructibleObjectComponentDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = plugin.ModKey,
            FormKey = MapFormKey(formKey),
            ComponentFormKey = componentFormKey,
            ComponentIndex = componentIndex,
            Count = GetPropertyNullableInt(component, "RequiredCount") ?? GetPropertyNullableInt(component, "Count"),
            ImportedAtUTC = importedAtUTC
        };
    }

    private static List<ConstructibleObjectRecipeFilterDTO> GetConstructibleObjectRecipeFilters(PluginDTO plugin, FormKey formKey, object? recipeFilters)
    {
        if (recipeFilters is not IEnumerable enumerable) return new List<ConstructibleObjectRecipeFilterDTO>();

        var importedAtUTC = DateTime.UtcNow;
        return enumerable
            .Cast<object>()
            .Select((recipeFilter, recipeFilterIndex) => GetFormKeyFromObject(recipeFilter) is { } recipeFilterFormKey
                ? new ConstructibleObjectRecipeFilterDTO
                {
                    Game = SupportedGame.Starfield,
                    ModKey = plugin.ModKey,
                    FormKey = MapFormKey(formKey),
                    RecipeFilterFormKey = recipeFilterFormKey,
                    RecipeFilterIndex = recipeFilterIndex,
                    ImportedAtUTC = importedAtUTC
                }
                : null)
            .Where(recipeFilter => recipeFilter != null)
            .Cast<ConstructibleObjectRecipeFilterDTO>()
            .ToList();
    }

    private static List<RawRecordPayloadDTO> GetConstructibleObjectRawPayloads(PluginDTO plugin, FormKey formKey, object record)
    {
        var importedAtUTC = DateTime.UtcNow;
        var payloads = new List<RawRecordPayloadDTO>();
        AddRawPayload(payloads, plugin, RecordTypeCatalog.ConstructibleObject.RecordID, formKey, "Conditions", 0, "Conditions", FormatEnumerable(GetPropertyValue(record, "Conditions")), importedAtUTC);
        return payloads;
    }

    private static List<RawRecordPayloadDTO> GetConditionFormRawPayloads(PluginDTO plugin, FormKey formKey, object record)
    {
        var importedAtUTC = DateTime.UtcNow;
        var payloads = new List<RawRecordPayloadDTO>();
        AddRawPayload(payloads, plugin, RecordTypeCatalog.ConditionForm.RecordID, formKey, "Conditions", 0, "Conditions", FormatEnumerable(GetPropertyValue(record, "Conditions")), importedAtUTC);
        return payloads;
    }

    private static List<RawRecordPayloadDTO> GetContainerRawPayloads(PluginDTO plugin, FormKey formKey, object? model, object? components)
    {
        var importedAtUTC = DateTime.UtcNow;
        var payloads = new List<RawRecordPayloadDTO>();
        AddRawPayload(payloads, plugin, RecordTypeCatalog.Container.RecordID, formKey, "Model.Data", 0, model?.GetType().Name ?? "Model", FormatHexValue(GetPropertyValue(model, "Data")), importedAtUTC);
        AddBaseFormComponentRawPayloads(payloads, plugin, RecordTypeCatalog.Container.RecordID, formKey, components, importedAtUTC);
        return payloads;
    }

    private static List<RawRecordPayloadDTO> GetDoorRawPayloads(PluginDTO plugin, FormKey formKey, object? model, object? components)
    {
        var importedAtUTC = DateTime.UtcNow;
        var payloads = new List<RawRecordPayloadDTO>();
        AddRawPayload(payloads, plugin, RecordTypeCatalog.Door.RecordID, formKey, "Model.Data", 0, model?.GetType().Name ?? "Model", FormatHexValue(GetPropertyValue(model, "Data")), importedAtUTC);
        AddBaseFormComponentRawPayloads(payloads, plugin, RecordTypeCatalog.Door.RecordID, formKey, components, importedAtUTC);
        return payloads;
    }

    private static List<RawRecordPayloadDTO> GetTerminalRawPayloads(PluginDTO plugin, FormKey formKey, object? model, object? components)
    {
        var importedAtUTC = DateTime.UtcNow;
        var payloads = new List<RawRecordPayloadDTO>();
        AddRawPayload(payloads, plugin, RecordTypeCatalog.Terminal.RecordID, formKey, "Model.Data", 0, model?.GetType().Name ?? "Model", FormatHexValue(GetPropertyValue(model, "Data")), importedAtUTC);
        AddBaseFormComponentRawPayloads(payloads, plugin, RecordTypeCatalog.Terminal.RecordID, formKey, components, importedAtUTC);
        return payloads;
    }

    private static List<TerminalMarkerParameterDTO> GetTerminalMarkerParameters(PluginDTO plugin, FormKey formKey, object? markerParameters)
    {
        if (markerParameters is not IEnumerable enumerable)
        {
            return new List<TerminalMarkerParameterDTO>();
        }

        var importedAtUTC = DateTime.UtcNow;
        return enumerable
            .Cast<object>()
            .Select((parameter, parameterIndex) => new TerminalMarkerParameterDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(formKey),
                ParameterIndex = parameterIndex,
                Offset = GetPropertyValue(parameter, "Offset")?.ToString(),
                EntryTypes = FormatEnumerable(GetPropertyValue(parameter, "EntryTypes")),
                ExitTypes = FormatEnumerable(GetPropertyValue(parameter, "ExitTypes")),
                ImportedAtUTC = importedAtUTC
            })
            .ToList();
    }

    private static void AddBaseFormComponentRawPayloads(
        ICollection<RawRecordPayloadDTO> payloads,
        PluginDTO plugin,
        string recordType,
        FormKey formKey,
        object? components,
        DateTime importedAtUTC)
    {
        if (components is not IEnumerable enumerable)
        {
            return;
        }

        foreach (var component in enumerable.Cast<object>().Select((value, index) => new { value, index }))
        {
            var componentType = component.value.GetType().Name;
            foreach (var property in component.value.GetType().GetProperties())
            {
                var propertyName = property.Name;
                if (!IsRawBaseFormComponentPayloadProperty(propertyName))
                {
                    continue;
                }

                AddRawPayload(
                    payloads,
                    plugin,
                    recordType,
                    formKey,
                    $"BaseFormComponents.{componentType}.{propertyName}",
                    component.index,
                    componentType,
                    FormatHexValue(property.GetValue(component.value)),
                    importedAtUTC,
                    $"Components.{componentType}.{propertyName}");
            }
        }
    }

    private static bool IsRawBaseFormComponentPayloadProperty(string propertyName)
    {
        return propertyName is "ANAM" or "BNAM" or "CNAM" or "REFL";
    }

    private static void AddRawPayload(
        ICollection<RawRecordPayloadDTO> payloads,
        PluginDTO plugin,
        string recordType,
        FormKey formKey,
        string payloadSlot,
        int payloadIndex,
        string payloadType,
        string? payloadValue,
        DateTime importedAtUTC,
        string? sourcePath = null)
    {
        if (string.IsNullOrWhiteSpace(payloadValue))
        {
            return;
        }

        payloads.Add(new RawRecordPayloadDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = plugin.ModKey,
            RecordType = recordType,
            FormKey = MapFormKey(formKey),
            PayloadSlot = payloadSlot,
            PayloadIndex = payloadIndex,
            PayloadType = payloadType,
            SourcePath = sourcePath ?? payloadSlot,
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
        if (soundSource == null)
        {
            return null;
        }

        var start = GetSoundStart(soundSource);
        if (string.IsNullOrWhiteSpace(start))
        {
            return null;
        }

        return new RecordSoundDTO
        {
            Game = SupportedGame.Starfield,
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
        if (!string.IsNullOrWhiteSpace(directStart))
        {
            return directStart;
        }

        var sound = GetPropertyValue(soundSource, "Sound");
        return sound == null ? null : GetPropertyValue(sound, "Start")?.ToString();
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

    private static IEnumerable<object> GetRecordCollection(object source, string propertyName)
    {
        return GetPropertyValue(source, propertyName) is IEnumerable enumerable
            ? enumerable.Cast<object>()
            : [];
    }

    private static FormKey GetRequiredFormKey(object record)
    {
        return GetPropertyValue(record, "FormKey") is FormKey formKey
            ? formKey
            : throw new InvalidOperationException($"Record '{record.GetType().Name}' did not expose a FormKey.");
    }

    private static FormKeyDTO? GetFormKeyFromObject(object? value)
    {
        if (value == null) return null;
        if (value is FormKey formKey) return MapFormKey(formKey);
        if (GetPropertyValue(value, "IsNull") is bool isNull && isNull) return null;
        if (GetPropertyValue(value, "FormKey") is FormKey linkedFormKey) return MapFormKey(linkedFormKey);
        if (GetPropertyValue(value, "FormKeyNullable") is FormKey nullableFormKey) return MapFormKey(nullableFormKey);
        return null;
    }

    private static int? GetPropertyNullableInt(object? source, string propertyName)
    {
        var value = GetPropertyValue(source, propertyName);
        return value == null ? null : Convert.ToInt32(value, CultureInfo.InvariantCulture);
    }

    private static long? GetPropertyNullableLong(object? source, string propertyName)
    {
        var value = GetPropertyValue(source, propertyName);
        return value == null ? null : Convert.ToInt64(value, CultureInfo.InvariantCulture);
    }

    private static double? GetPropertyNullableDouble(object? source, string propertyName)
    {
        var value = GetPropertyValue(source, propertyName);
        return value == null ? null : Convert.ToDouble(value, CultureInfo.InvariantCulture);
    }

    private static float? GetPropertyNullableFloat(object? source, string propertyName)
    {
        var value = GetPropertyValue(source, propertyName);
        return value == null ? null : Convert.ToSingle(value, CultureInfo.InvariantCulture);
    }

    private static string? FormatObjectBoundsPoint(object? objectBounds, string propertyName)
    {
        return GetPropertyValue(objectBounds, propertyName)?.ToString();
    }

    private static string? FormatEnumerable(object? value)
    {
        if (value is string text)
        {
            return text;
        }

        return value is IEnumerable enumerable
            ? string.Join(", ", enumerable.Cast<object>().Select(item => item.ToString()))
            : value?.ToString();
    }

    private static string? FormatHexValue(object? value)
    {
        if (value == null)
        {
            return null;
        }

        if (value is string text)
        {
            return text;
        }

        if (value is byte[] bytes)
        {
            return Convert.ToHexString(bytes);
        }

        var toArray = value.GetType().GetMethod("ToArray", Type.EmptyTypes);
        if (toArray?.Invoke(value, null) is byte[] arrayBytes)
        {
            return Convert.ToHexString(arrayBytes);
        }

        return value.ToString();
    }

    private static string? GetMagicEffectArchetype(IMagicEffectGetter record)
    {
        return record.Archetype == null
            ? null
            : Convert.ToInt64(record.Archetype.Type, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
    }

    private static List<ScriptingAdapterPropertyDTO> GetScriptingAdapterProperties(PluginDTO plugin, string recordType, FormKey formKey, IScriptEntryGetter script, DateTime importedAtUTC)
    {
        return script.Properties
            .Select((property, propertyIndex) => CreateScriptingAdapterProperty(plugin, recordType, formKey, script.Name, property, propertyIndex, importedAtUTC))
            .Where(property => property != null)
            .Cast<ScriptingAdapterPropertyDTO>()
            .ToList();
    }

    private static ScriptingAdapterPropertyDTO? CreateScriptingAdapterProperty(PluginDTO plugin, string recordType, FormKey formKey, string scriptName, IScriptPropertyGetter property, int propertyIndex, DateTime importedAtUTC)
    {
        var dto = new ScriptingAdapterPropertyDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = plugin.ModKey,
            RecordType = recordType,
            FormKey = MapFormKey(formKey),
            ScriptingAdapterName = scriptName,
            PropertyIndex = propertyIndex,
            Name = property.Name,
            MutagenObjectType = property.GetType().Name,
            ImportedAtUTC = importedAtUTC
        };

        switch (property)
        {
            case ScriptBoolProperty boolProperty:
                dto.DataBool = boolProperty.Data;
                return dto;
            case ScriptIntProperty intProperty:
                dto.DataInt = intProperty.Data;
                return dto;
            case ScriptFloatProperty floatProperty:
                dto.DataFloat = floatProperty.Data;
                return dto;
            case ScriptStringProperty stringProperty:
                dto.DataString = stringProperty.Data;
                return dto;
            case ScriptObjectProperty objectProperty:
                dto.ObjectFormKey = objectProperty.Object.FormKeyNullable is { } objectFormKey ? MapFormKey(objectFormKey) : null;
                dto.ObjectAlias = objectProperty.Alias;
                dto.ObjectUnused = objectProperty.Unused;
                return dto;
            case ScriptBoolListProperty boolListProperty:
                dto.ListItems = boolListProperty.Data.Select((value, listItemIndex) => CreateScriptingAdapterPropertyListItem(plugin, recordType, formKey, scriptName, propertyIndex, listItemIndex, nameof(ScriptBoolProperty), importedAtUTC, dataBool: value)).ToList();
                return dto;
            case ScriptIntListProperty intListProperty:
                dto.ListItems = intListProperty.Data.Select((value, listItemIndex) => CreateScriptingAdapterPropertyListItem(plugin, recordType, formKey, scriptName, propertyIndex, listItemIndex, nameof(ScriptIntProperty), importedAtUTC, dataInt: value)).ToList();
                return dto;
            case ScriptFloatListProperty floatListProperty:
                dto.ListItems = floatListProperty.Data.Select((value, listItemIndex) => CreateScriptingAdapterPropertyListItem(plugin, recordType, formKey, scriptName, propertyIndex, listItemIndex, nameof(ScriptFloatProperty), importedAtUTC, dataFloat: value)).ToList();
                return dto;
            case ScriptStringListProperty stringListProperty:
                dto.ListItems = stringListProperty.Data.Select((value, listItemIndex) => CreateScriptingAdapterPropertyListItem(plugin, recordType, formKey, scriptName, propertyIndex, listItemIndex, nameof(ScriptStringProperty), importedAtUTC, dataString: value)).ToList();
                return dto;
            case ScriptObjectListProperty objectListProperty:
                dto.ListItems = objectListProperty.Objects.Select((value, listItemIndex) => CreateScriptingAdapterPropertyListItem(plugin, recordType, formKey, scriptName, propertyIndex, listItemIndex, nameof(ScriptObjectProperty), importedAtUTC, objectFormKey: value.Object.FormKeyNullable is { } objectFormKey ? MapFormKey(objectFormKey) : null, objectAlias: value.Alias, objectUnused: value.Unused)).ToList();
                return dto;
            case ScriptProperty:
                return dto;
            default:
                return null;
        }
    }

    private static ScriptingAdapterPropertyListItemDTO CreateScriptingAdapterPropertyListItem(
        PluginDTO plugin,
        string recordType,
        FormKey formKey,
        string scriptName,
        int propertyIndex,
        int listItemIndex,
        string mutagenObjectType,
        DateTime importedAtUTC,
        bool? dataBool = null,
        int? dataInt = null,
        double? dataFloat = null,
        string? dataString = null,
        FormKeyDTO? objectFormKey = null,
        short? objectAlias = null,
        ushort? objectUnused = null)
    {
        return new ScriptingAdapterPropertyListItemDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = plugin.ModKey,
            RecordType = recordType,
            FormKey = MapFormKey(formKey),
            ScriptingAdapterName = scriptName,
            PropertyIndex = propertyIndex,
            ListItemIndex = listItemIndex,
            MutagenObjectType = mutagenObjectType,
            DataBool = dataBool,
            DataInt = dataInt,
            DataFloat = dataFloat,
            DataString = dataString,
            ObjectFormKey = objectFormKey,
            ObjectAlias = objectAlias,
            ObjectUnused = objectUnused,
            ImportedAtUTC = importedAtUTC
        };
    }

    protected virtual IStarfieldModGetter LoadMod(PluginDTO plugin)
    {
        return StarfieldModConstruction.Load(plugin.ModKey);
    }

    private string GetDataFolderPath()
    {
        return StarfieldModConstruction.GetDataFolderPath();
    }

    private static FormKeyDTO MapFormKey(FormKey formKey)
    {
        return new FormKeyDTO
        {
            ModKey = ModKeyDTOMapper.FromModKey(formKey.ModKey),
            Id = formKey.ID
        };
    }

    private static string GetGameSettingType(IGameSettingGetter record)
    {
        return record switch
        {
            IGameSettingBoolGetter => "GameSettingBool",
            IGameSettingFloatGetter => "GameSettingFloat",
            IGameSettingIntGetter => "GameSettingInt",
            IGameSettingStringGetter => "GameSettingString",
            IGameSettingUIntGetter => "GameSettingUInt",
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
            IGameSettingStringGetter gameSetting => gameSetting.Data?.ToString(),
            IGameSettingUIntGetter gameSetting => Convert.ToString(gameSetting.Data, CultureInfo.InvariantCulture),
            _ => null
        };
    }

    private static double? GetGameSettingNumericData(IGameSettingGetter record)
    {
        return record switch
        {
            IGameSettingFloatGetter gameSetting => gameSetting.Data,
            IGameSettingIntGetter gameSetting => gameSetting.Data,
            IGameSettingUIntGetter gameSetting => gameSetting.Data,
            _ => null
        };
    }

    private static int? GetGameSettingIntegerData(IGameSettingGetter record)
    {
        return record switch
        {
            IGameSettingIntGetter gameSetting => gameSetting.Data,
            IGameSettingUIntGetter gameSetting when gameSetting.Data <= int.MaxValue => (int)gameSetting.Data,
            _ => null
        };
    }

    private static bool? GetGameSettingBooleanData(IGameSettingGetter record)
    {
        return record is IGameSettingBoolGetter gameSetting ? gameSetting.Data : null;
    }
}
