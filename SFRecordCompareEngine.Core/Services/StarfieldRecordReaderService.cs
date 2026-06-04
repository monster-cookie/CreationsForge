using System.Globalization;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Strings;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Helpers;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class StarfieldRecordReaderService : IStarfieldRecordReaderService
{
    #region FormList

    public IReadOnlyList<FormListDTO> GetFormLists(PluginDTO plugin)
    {
        var mod = LoadMod(plugin.ModKey);
        return MapRecords(plugin, mod.FormLists, record => new FormListDTO
        {
            ModKey = plugin.ModKey,
            FormKey = record.FormKey,
            EditorID = record.EditorID ?? string.Empty,
            FormVersion = record.FormVersion,
            StarfieldMajorRecordFlags = record.StarfieldMajorRecordFlags,
            Version2 = record.Version2,
            VersionControl = (int)record.VersionControl,
            ImportedAtUTC = DateTime.UtcNow,
            AddToListFormKey = record.AddToList.FormKey,
            Items = record.Items.Select(item =>
            {
                item.TryGetModKey(out var itemModKey);
                return new FormListItemDataDTO
                {
                    ItemModKey = itemModKey,
                    ItemFormKey = item.FormKey
                };
            }).ToList()
        });
    }

    #endregion

    #region GameSettings

    public IReadOnlyList<GameSettingDTO> GetGameSettings(PluginDTO plugin)
    {
        var mod = LoadMod(plugin.ModKey);
        return MapRecords(plugin, mod.GameSettings, record => new GameSettingDTO
        {
            ModKey = plugin.ModKey,
            FormKey = record.FormKey,
            EditorID = record.EditorID ?? string.Empty,
            FormVersion = record.FormVersion,
            StarfieldMajorRecordFlags = record.StarfieldMajorRecordFlags,
            Version2 = record.Version2,
            VersionControl = (int)record.VersionControl,
            ImportedAtUTC = DateTime.UtcNow,
            SettingType = GetGameSettingType(record),
            Data = GetGameSettingData(record),
            RawData = GetGameSettingRawData(record),
            IsCompressed = 0,
            IsDeleted = 0
        });
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

    private static double? GetGameSettingRawData(IGameSettingGetter record)
    {
        return record switch
        {
            IGameSettingBoolGetter gameSetting => gameSetting.Data == true ? 1 : 0,
            IGameSettingFloatGetter gameSetting => gameSetting.Data,
            IGameSettingIntGetter gameSetting => gameSetting.Data,
            IGameSettingUIntGetter gameSetting => gameSetting.Data,
            _ => null
        };
    }

    public IReadOnlyList<GlobalDTO> GetGlobals(PluginDTO plugin)
    {
        return MapRecords(plugin, LoadMod(plugin.ModKey).Globals, record => new GlobalDTO
        {
            ModKey = plugin.ModKey, FormKey = record.FormKey, EditorID = record.EditorID ?? string.Empty, FormVersion = record.FormVersion,
            StarfieldMajorRecordFlags = record.StarfieldMajorRecordFlags, Version2 = record.Version2, VersionControl = (int)record.VersionControl,
            ImportedAtUTC = DateTime.UtcNow, Data = record.Data, ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.Global.RecordType, record)
        });
    }

    public IReadOnlyList<MiscItemDTO> GetMiscItems(PluginDTO plugin)
    {
        return MapRecords(plugin, LoadMod(plugin.ModKey).MiscItems, record => new MiscItemDTO
        {
            ModKey = plugin.ModKey, FormKey = record.FormKey, EditorID = record.EditorID ?? string.Empty, FormVersion = record.FormVersion,
            StarfieldMajorRecordFlags = record.StarfieldMajorRecordFlags, Version2 = record.Version2, VersionControl = (int)record.VersionControl,
            ImportedAtUTC = DateTime.UtcNow, Name = record.Name?.Lookup(Language.English), ShortName = record.ShortName?.Lookup(Language.English), Value = record.Value, Weight = record.Weight,
            DirtinessScale = (float)record.DirtinessScale,
            FeaturedItemMessageFormKey = record.FeaturedItemMessage.FormKey,
            Flag = record.FLAG == null ? null : Convert.ToHexString(record.FLAG.Value.ToArray()),
            ObjectBounds = GetMiscItemObjectBounds(record.ObjectBounds),
            ObjectPaletteDefaults = GetMiscItemObjectPaletteDefaults(record.ObjectPaletteDefaults),
            Transforms = GetMiscItemTransforms(record.Transforms),
            Model = GetMiscItemModel(record.Model),
            CraftingSound = GetMiscItemSound(record.CraftingSound),
            PickupSound = GetMiscItemSound(record.PickupSound),
            DropdownSound = GetMiscItemSound(record.DropdownSound),
            Keywords = record.Keywords?.Select(keyword => keyword.FormKey).ToList() ?? new List<FormKey>(),
            Destructible = GetMiscItemDestructible(record.Destructible),
            ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.MiscItem.RecordType, record)
        });
    }

    private static MiscItemObjectBoundsDTO? GetMiscItemObjectBounds(IObjectBoundsGetter value)
    {
        if (value.First.X == 0 && value.First.Y == 0 && value.First.Z == 0 && value.Second.X == 0 && value.Second.Y == 0 && value.Second.Z == 0) return null;

        return new MiscItemObjectBoundsDTO
        {
            FirstX = value.First.X,
            FirstY = value.First.Y,
            FirstZ = value.First.Z,
            SecondX = value.Second.X,
            SecondY = value.Second.Y,
            SecondZ = value.Second.Z
        };
    }

    private static MiscItemObjectPaletteDefaultsDTO? GetMiscItemObjectPaletteDefaults(IObjectPaletteDefaultsGetter? value)
    {
        return value == null
            ? null
            : new MiscItemObjectPaletteDefaultsDTO
            {
                Flags = value.Flags.ToString(),
                SinkMeters = value.SinkMeters,
                SinkVariance = value.SinkVariance,
                XYOffsetVariance = value.XYOffsetVariance,
                FootprintSize = value.FootprintSize.ToString(),
                ScalePercent = value.ScalePercent,
                ScaleVariance = value.ScaleVariance,
                AngleXDegrees = value.AngleXDegrees,
                AngleXVariance = value.AngleXVariance,
                AngleYDegrees = value.AngleYDegrees,
                AngleYVariance = value.AngleYVariance,
                AngleZDegrees = value.AngleZDegrees,
                AngleZVariance = value.AngleZVariance,
                SlopePercent = value.SlopePercent,
                SlopePercentVariance = value.SlopePercentVariance,
                Density = value.Density,
                FrequencyPercent = value.FrequencyPercent,
                SlopeLimit = value.SlopeLimit,
                DistanceBelowWater = value.DistanceBelowWater,
                DistanceAboveWater = value.DistanceAboveWater
            };
    }

    private static MiscItemTransformsDTO? GetMiscItemTransforms(ITransformsGetter? value)
    {
        return value == null
            ? null
            : new MiscItemTransformsDTO
            {
                InventoryIconFormKey = value.InventoryIcon.FormKey,
                OutpostFormKey = value.Outpost.FormKey,
                ShipFormKey = value.Ship.FormKey,
                PreviewFormKey = value.Preview.FormKey,
                InventoryFormKey = value.Inventory.FormKey,
                WorkbenchFormKey = value.Workbench.FormKey,
                MainGameUIFormKey = value.MainGameUI.FormKey
            };
    }

    private static MiscItemModelDTO? GetMiscItemModel(IModelGetter? value)
    {
        return value == null
            ? null
            : new MiscItemModelDTO
            {
                File = value.File?.ToString(),
                TextureFileHashes = value.TextureFileHashes == null ? null : Convert.ToHexString(value.TextureFileHashes.Value.ToArray()),
                LightLayer = value.LightLayer,
                Flags = value.Flags.ToString(),
                ColorRemappingIndex = value.ColorRemappingIndex,
                FlagsVestigial = value.FlagsVestigial.ToString(),
                MaterialSwaps = value.MaterialSwaps?.Select(materialSwap => materialSwap.FormKey).ToList() ?? new List<FormKey>()
            };
    }

    private static MiscItemSoundDTO? GetMiscItemSound(ISoundReferenceGetter? value)
    {
        return value == null
            ? null
            : new MiscItemSoundDTO
            {
                Start = value.Start == Guid.Empty ? null : value.Start.ToString(),
                Stop = value.Stop == Guid.Empty ? null : value.Stop.ToString(),
                ConditionFormKey = value.Condition.FormKey,
                EventMappingFormKey = value.EventMapping.FormKey
            };
    }

    private static MiscItemDestructibleDTO? GetMiscItemDestructible(IDestructibleGetter? value)
    {
        if (value == null) return null;

        return new MiscItemDestructibleDTO
        {
            Health = value.Data?.Health,
            Count = value.Data?.DESTCount,
            Flags = value.Data?.Flags.ToString(),
            Resistances = value.Resistances?.Select((resistance, resistanceIndex) => new MiscItemDestructibleResistanceDTO
            {
                DamageTypeFormKey = resistance.DamageType.FormKey,
                Value = resistance.Value,
                ResistanceIndex = resistanceIndex
            }).ToList() ?? new List<MiscItemDestructibleResistanceDTO>(),
            Stages = value.Stages.Select((stage, stageIndex) => new MiscItemDestructionStageDTO
            {
                StageIndex = stageIndex,
                HealthPercent = stage.HealthPercent,
                Index = stage.Index,
                ModelDamageStage = stage.ModelDamageStage,
                Flags = stage.Flags.ToString(),
                SelfDamagePerSecond = stage.SelfDamagePerSecond,
                ExplosionFormKey = stage.Explosion.FormKey,
                DebrisFormKey = stage.Debris.FormKey,
                DebrisCount = stage.DebrisCount,
                SequenceName = stage.SequenceName,
                ModelFile = stage.Model?.File?.ToString(),
                ModelLightLayer = stage.Model?.LightLayer,
                ModelFlags = stage.Model?.Flags.ToString(),
                ModelMaterialSwaps = stage.Model?.MaterialSwaps?.Select(materialSwap => materialSwap.FormKey).ToList() ?? new List<FormKey>()
            }).ToList()
        };
    }

    public IReadOnlyList<KeywordDTO> GetKeywords(PluginDTO plugin)
    {
        return MapRecords(plugin, LoadMod(plugin.ModKey).Keywords, record => new KeywordDTO
        {
            ModKey = plugin.ModKey, FormKey = record.FormKey, EditorID = record.EditorID ?? string.Empty, FormVersion = record.FormVersion,
            StarfieldMajorRecordFlags = record.StarfieldMajorRecordFlags, Version2 = record.Version2, VersionControl = (int)record.VersionControl,
            ImportedAtUTC = DateTime.UtcNow, Name = record.Name?.Lookup(Language.English), Color = record.Color.ToString() ?? string.Empty, Type = record.Type.ToString() ?? string.Empty,
            Notes = record.Notes, FlashLinkageName = record.FlashLinkageName, AttractionRuleFormKey = record.AttractionRule.FormKey,
            ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.Keyword.RecordType, record)
        });
    }

    public IReadOnlyList<NPCDTO> GetNPCs(PluginDTO plugin)
    {
        return MapRecords(plugin, LoadMod(plugin.ModKey).Npcs, record => new NPCDTO
        {
            ModKey = plugin.ModKey, FormKey = record.FormKey, EditorID = record.EditorID ?? string.Empty, FormVersion = record.FormVersion,
            StarfieldMajorRecordFlags = record.StarfieldMajorRecordFlags, Version2 = record.Version2, VersionControl = (int)record.VersionControl,
            ImportedAtUTC = DateTime.UtcNow, Name = record.Name?.Lookup(Language.English), ShortName = record.ShortName?.Lookup(Language.English), LongName = record.LongName?.Lookup(Language.English),
            DispositionBase = record.DispositionBase, Aggression = record.Aggression.ToString(), Confidence = record.Confidence.ToString(),
            EnergyLevel = record.EnergyLevel, Responsibility = record.Responsibility.ToString(), Assistance = record.Assistance.ToString(),
            GearedUpWeapons = record.GearedUpWeapons, HeightMin = record.HeightMin, HeightMax = record.HeightMax, SkinToneIndex = record.SkinToneIndex,
            Pronoun = record.Pronoun?.ToString(), VoiceFormKey = record.Voice.FormKey, RaceFormKey = record.Race.FormKey,
            CombatOverridePackageListFormKey = record.CombatOverridePackageList.FormKey, CombatStyleFormKey = record.CombatStyle.FormKey,
            DefaultPackageListFormKey = record.DefaultPackageList.FormKey, CrimeFactionFormKey = record.CrimeFaction.FormKey,
            ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.NPC.RecordType, record)
        });
    }

    public IReadOnlyList<ActorValueInformationDTO> GetActorValueInformation(PluginDTO plugin)
    {
        return MapRecords(plugin, LoadMod(plugin.ModKey).ActorValueInformation, record => new ActorValueInformationDTO
        {
            ModKey = plugin.ModKey, FormKey = record.FormKey, EditorID = record.EditorID ?? string.Empty, FormVersion = record.FormVersion,
            StarfieldMajorRecordFlags = record.StarfieldMajorRecordFlags, Version2 = record.Version2, VersionControl = (int)record.VersionControl,
            ImportedAtUTC = DateTime.UtcNow, Name = record.Name?.Lookup(Language.English), Abbreviation = record.Abbreviation?.Lookup(Language.English), ContextNotes = record.ContextNotes,
            DefaultValue = record.DefaultValue, Flags = record.Flags.ToString(), Type = record.Type?.ToString(), Min = record.Min, Max = record.Max,
            ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.ActorValueInformation.RecordType, record)
        });
    }

    public IReadOnlyList<MagicEffectDTO> GetMagicEffects(PluginDTO plugin)
    {
        return MapRecords(plugin, LoadMod(plugin.ModKey).MagicEffects, record => new MagicEffectDTO
        {
            ModKey = plugin.ModKey, FormKey = record.FormKey, EditorID = record.EditorID ?? string.Empty, FormVersion = record.FormVersion,
            StarfieldMajorRecordFlags = record.StarfieldMajorRecordFlags, Version2 = record.Version2, VersionControl = (int)record.VersionControl,
            ImportedAtUTC = DateTime.UtcNow, Name = GetLocalizedEnglishText(() => record.Name), Description = GetLocalizedEnglishText(() => record.Description), Flags = record.Flags.ToString(),
            CastType = record.CastType.ToString(), TargetType = record.TargetType.ToString(), ActorValue2FormKey = record.ActorValue2.FormKey,
            ResistValueFormKey = record.ResistValue.FormKey, PerkToApplyFormKey = record.PerkToApply.FormKey, EquipAbilityFormKey = record.EquipAbility.FormKey,
            ExplosionFormKey = record.Explosion.FormKey, CastingArtFormKey = record.CastingArt.FormKey, HitEffectArtFormKey = record.HitEffectArt.FormKey,
            HitShaderFormKey = record.HitShader.FormKey, ImageSpaceModifierFormKey = record.ImageSpaceModifier.FormKey,
            ImpactDataFormKey = record.ImpactData.FormKey, ProjectileFormKey = record.Projectile.FormKey,
            ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.MagicEffect.RecordType, record)
        });
    }

    public IReadOnlyList<PerkDTO> GetPerks(PluginDTO plugin)
    {
        return MapRecords(plugin, LoadMod(plugin.ModKey).Perks, record => new PerkDTO
        {
            ModKey = plugin.ModKey, FormKey = record.FormKey, EditorID = record.EditorID ?? string.Empty, FormVersion = record.FormVersion,
            StarfieldMajorRecordFlags = record.StarfieldMajorRecordFlags, Version2 = record.Version2, VersionControl = (int)record.VersionControl,
            ImportedAtUTC = DateTime.UtcNow, Name = record.Name?.Lookup(Language.English), Description = record.Description?.Lookup(Language.English), Flags = record.Flags.ToString(),
            SkillGroup = record.SkillGroup.ToString(), CrewAssignment = record.CrewAssignment.ToString(), PerkIcon = record.PerkIcon,
            Category = record.Categroy.ToString(), RestrictionFormKey = record.Restriction.FormKey, TrainingFormKey = record.Training.FormKey, MajorFlags = record.MajorFlags.ToString(),
            Ranks = GetPerkRanks(plugin, record),
            BackgroundSkills = GetPerkBackgroundSkills(plugin, record),
            ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.Perk.RecordType, record)
        });
    }

    private static List<PerkRankDTO> GetPerkRanks(PluginDTO plugin, IPerkGetter record)
    {
        var importedAtUtc = DateTime.UtcNow;
        return record.Ranks
            .Select((rank, rankIndex) => new PerkRankDTO
            {
                ModKey = plugin.ModKey,
                FormKey = record.FormKey,
                RankIndex = rankIndex,
                Description = GetLocalizedEnglishText(() => rank.Description),
                UnknownStaticFormKey = rank.UnknownStatic?.FormKey,
                ConditionCount = rank.Conditions?.Count ?? 0,
                ActivityCount = rank.Activities?.Count ?? 0,
                ImportedAtUTC = importedAtUtc,
                Effects = GetPerkRankEffects(plugin, record.FormKey, rank, rankIndex, importedAtUtc)
            })
            .ToList();
    }

    private static List<PerkRankEffectDTO> GetPerkRankEffects(PluginDTO plugin, FormKey formKey, IPerkRankGetter rank, int rankIndex, DateTime importedAtUtc)
    {
        return rank.Effects
            .Select((effect, effectIndex) =>
            {
                var dto = new PerkRankEffectDTO
                {
                    ModKey = plugin.ModKey,
                    FormKey = formKey,
                    RankIndex = rankIndex,
                    EffectIndex = effectIndex,
                    MutagenObjectType = effect.GetType().Name,
                    Rank = effect.Rank,
                    Priority = effect.Priority,
                    PerkEntryId = effect.PerkEntryID,
                    Flags = effect.Flags?.ToString(),
                    ButtonLabel = GetLocalizedEnglishText(() => effect.ButtonLabel),
                    ConditionCount = effect.Conditions.Count,
                    ImportedAtUTC = importedAtUtc
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
        var importedAtUtc = DateTime.UtcNow;
        return record.BackgroundSkills
            .Select((skill, skillIndex) => new PerkBackgroundSkillDTO
            {
                ModKey = plugin.ModKey,
                FormKey = record.FormKey,
                SkillFormKey = skill.FormKey,
                SkillIndex = skillIndex,
                ImportedAtUTC = importedAtUtc
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

    private static List<ScriptingAdapterDTO> GetScriptingAdapters(PluginDTO plugin, string recordType, IHaveVirtualMachineAdapterGetter record)
    {
        if (record.VirtualMachineAdapter == null) return new List<ScriptingAdapterDTO>();

        var importedAtUtc = DateTime.UtcNow;
        return record.VirtualMachineAdapter.Scripts
            .Select((script, scriptIndex) => new ScriptingAdapterDTO
            {
                ModKey = plugin.ModKey,
                RecordType = recordType,
                FormKey = record.FormKey,
                Name = script.Name,
                ScriptIndex = scriptIndex,
                ImportedAtUTC = importedAtUtc,
                Properties = GetScriptingAdapterProperties(plugin, recordType, record.FormKey, script, importedAtUtc)
            })
            .ToList();
    }

    private static List<ScriptingAdapterPropertyDTO> GetScriptingAdapterProperties(PluginDTO plugin, string recordType, FormKey formKey, IScriptEntryGetter script, DateTime importedAtUtc)
    {
        return script.Properties
            .Select((property, propertyIndex) => CreateScriptingAdapterProperty(plugin, recordType, formKey, script.Name, property, propertyIndex, importedAtUtc))
            .Where(property => property != null)
            .Cast<ScriptingAdapterPropertyDTO>()
            .ToList();
    }

    private static ScriptingAdapterPropertyDTO? CreateScriptingAdapterProperty(PluginDTO plugin, string recordType, FormKey formKey, string scriptName, IScriptPropertyGetter property, int propertyIndex, DateTime importedAtUtc)
    {
        var dto = new ScriptingAdapterPropertyDTO
        {
            ModKey = plugin.ModKey,
            RecordType = recordType,
            FormKey = formKey,
            ScriptingAdapterName = scriptName,
            PropertyIndex = propertyIndex,
            Name = property.Name,
            MutagenObjectType = property.GetType().Name,
            ImportedAtUTC = importedAtUtc
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
                dto.ObjectFormKey = objectProperty.Object.FormKeyNullable;
                dto.ObjectAlias = objectProperty.Alias;
                dto.ObjectUnused = objectProperty.Unused;
                return dto;
            case ScriptBoolListProperty boolListProperty:
                dto.ListItems = boolListProperty.Data.Select((value, listItemIndex) => new ScriptingAdapterPropertyListItemDTO
                {
                    ModKey = plugin.ModKey,
                    RecordType = recordType,
                    FormKey = formKey,
                    ScriptingAdapterName = scriptName,
                    PropertyIndex = propertyIndex,
                    ListItemIndex = listItemIndex,
                    MutagenObjectType = nameof(ScriptBoolProperty),
                    DataBool = value,
                    ImportedAtUTC = importedAtUtc
                }).ToList();
                return dto;
            case ScriptIntListProperty intListProperty:
                dto.ListItems = intListProperty.Data.Select((value, listItemIndex) => new ScriptingAdapterPropertyListItemDTO
                {
                    ModKey = plugin.ModKey,
                    RecordType = recordType,
                    FormKey = formKey,
                    ScriptingAdapterName = scriptName,
                    PropertyIndex = propertyIndex,
                    ListItemIndex = listItemIndex,
                    MutagenObjectType = nameof(ScriptIntProperty),
                    DataInt = value,
                    ImportedAtUTC = importedAtUtc
                }).ToList();
                return dto;
            case ScriptFloatListProperty floatListProperty:
                dto.ListItems = floatListProperty.Data.Select((value, listItemIndex) => new ScriptingAdapterPropertyListItemDTO
                {
                    ModKey = plugin.ModKey,
                    RecordType = recordType,
                    FormKey = formKey,
                    ScriptingAdapterName = scriptName,
                    PropertyIndex = propertyIndex,
                    ListItemIndex = listItemIndex,
                    MutagenObjectType = nameof(ScriptFloatProperty),
                    DataFloat = value,
                    ImportedAtUTC = importedAtUtc
                }).ToList();
                return dto;
            case ScriptStringListProperty stringListProperty:
                dto.ListItems = stringListProperty.Data.Select((value, listItemIndex) => new ScriptingAdapterPropertyListItemDTO
                {
                    ModKey = plugin.ModKey,
                    RecordType = recordType,
                    FormKey = formKey,
                    ScriptingAdapterName = scriptName,
                    PropertyIndex = propertyIndex,
                    ListItemIndex = listItemIndex,
                    MutagenObjectType = nameof(ScriptStringProperty),
                    DataString = value,
                    ImportedAtUTC = importedAtUtc
                }).ToList();
                return dto;
            case ScriptObjectListProperty objectListProperty:
                dto.ListItems = objectListProperty.Objects.Select((value, listItemIndex) => new ScriptingAdapterPropertyListItemDTO
                {
                    ModKey = plugin.ModKey,
                    RecordType = recordType,
                    FormKey = formKey,
                    ScriptingAdapterName = scriptName,
                    PropertyIndex = propertyIndex,
                    ListItemIndex = listItemIndex,
                    MutagenObjectType = nameof(ScriptObjectProperty),
                    ObjectFormKey = value.Object.FormKeyNullable,
                    ObjectAlias = value.Alias,
                    ObjectUnused = value.Unused,
                    ImportedAtUTC = importedAtUtc
                }).ToList();
                return dto;
            case ScriptProperty:
                return dto;
            default:
                return null;
        }
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

    private static IReadOnlyList<TDTO> MapRecords<TRecord, TDTO>(PluginDTO plugin, IEnumerable<TRecord> records, Func<TRecord, TDTO> mapRecord)
        where TRecord : IMajorRecordGetter
    {
        return records.Select(record => MapRecord(plugin, record, mapRecord)).ToList();
    }

    private static TDTO MapRecord<TRecord, TDTO>(PluginDTO plugin, TRecord record, Func<TRecord, TDTO> mapRecord)
        where TRecord : IMajorRecordGetter
    {
        try
        {
            return mapRecord(record);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            RecordException.EnrichAndThrow(ex, plugin.ModKey, record);
            throw;
        }
    }

    #endregion
}