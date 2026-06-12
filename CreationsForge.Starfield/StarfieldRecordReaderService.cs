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
        var books = MapBookModelRecords(plugin, mod);
        cancellationToken.ThrowIfCancellationRequested();
        var doors = MapDoorModelRecords(plugin, mod);
        cancellationToken.ThrowIfCancellationRequested();
        var containers = MapContainerModelRecords(plugin, mod);
        cancellationToken.ThrowIfCancellationRequested();
        var terminals = MapTerminalModelRecords(plugin, mod);

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
                AddToListFormKey = record.AddToList.IsNull ? null : MapFormKey(record.AddToList.FormKey),
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
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.Global.RecordType, record)
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
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.Keyword.RecordType, record)
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
                Models = GetModels(plugin, RecordTypeCatalog.MiscObject.RecordType, record.FormKey, record.Model),
                Keywords = GetRecordKeywords(plugin, RecordTypeCatalog.MiscObject.RecordType, record.FormKey, record.Keywords),
                Sounds = GetNamedSounds(plugin, RecordTypeCatalog.MiscObject.RecordType, record.FormKey, record, "CraftingSound", "PickupSound", "DropdownSound"),
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.MiscObject.RecordType, record)
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
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.ActorValueInformation.RecordType, record)
            })
            .ToList();
    }

    private static IReadOnlyList<ModelRecordDTO> MapStaticModelRecords(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return mod.Statics
            .Select(record => CreateModelRecord(plugin, RecordTypeCatalog.Static.RecordID, record.FormKey, record.EditorID, record.FormVersion, (int)record.StarfieldMajorRecordFlags, record.Model))
            .ToList();
    }

    private static IReadOnlyList<ModelRecordDTO> MapBookModelRecords(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return mod.Books
            .Select(record => CreateModelRecord(plugin, RecordTypeCatalog.Book.RecordID, record.FormKey, record.EditorID, record.FormVersion, (int)record.StarfieldMajorRecordFlags, record.Model))
            .ToList();
    }

    private static IReadOnlyList<ModelRecordDTO> MapDoorModelRecords(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return mod.Doors
            .Select(record => CreateModelRecord(plugin, RecordTypeCatalog.Door.RecordID, record.FormKey, record.EditorID, record.FormVersion, (int)record.StarfieldMajorRecordFlags, record.Model))
            .ToList();
    }

    private static IReadOnlyList<ModelRecordDTO> MapContainerModelRecords(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return mod.Containers
            .Select(record => CreateModelRecord(plugin, RecordTypeCatalog.Container.RecordID, record.FormKey, record.EditorID, record.FormVersion, (int)record.StarfieldMajorRecordFlags, record.Model))
            .ToList();
    }

    private static IReadOnlyList<ModelRecordDTO> MapTerminalModelRecords(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return mod.Terminals
            .Select(record => CreateModelRecord(plugin, RecordTypeCatalog.Terminal.RecordID, record.FormKey, record.EditorID, record.FormVersion, (int)record.StarfieldMajorRecordFlags, record.Model))
            .ToList();
    }

    private static ModelRecordDTO CreateModelRecord(
        PluginDTO plugin,
        string recordType,
        FormKey formKey,
        string? editorID,
        int formVersion,
        int majorRecordFlags,
        IModelGetter? model)
    {
        return new ModelRecordDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = plugin.ModKey,
            FormKey = MapFormKey(formKey),
            EditorID = editorID ?? string.Empty,
            FormVersion = formVersion,
            MajorRecordFlags = majorRecordFlags,
            ImportedAtUTC = DateTime.UtcNow,
            Models = GetModels(plugin, recordType, formKey, model)
        };
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
                Keywords = GetRecordKeywords(plugin, RecordTypeCatalog.NPC.RecordType, record.FormKey, record.Keywords),
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.NPC.RecordType, record)
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
                Keywords = GetRecordKeywords(plugin, RecordTypeCatalog.MagicEffect.RecordType, record.FormKey, record.Keywords),
                Sounds = GetIndexedSounds(plugin, RecordTypeCatalog.MagicEffect.RecordType, record.FormKey, record),
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.MagicEffect.RecordType, record)
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
                Ranks = GetPerkRanks(record),
                BackgroundSkills = GetPerkBackgroundSkills(record),
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.Perk.RecordType, record)
            })
            .ToList();
    }

    private static List<PerkRankDTO> GetPerkRanks(IPerkGetter record)
    {
        var importedAtUTC = DateTime.UtcNow;
        return record.Ranks
            .Select((rank, rankIndex) => new PerkRankDTO
            {
                FormKey = MapFormKey(record.FormKey),
                RankIndex = rankIndex,
                Description = GetLocalizedEnglishText(() => rank.Description),
                UnknownStaticFormKey = rank.UnknownStatic.IsNull ? null : MapFormKey(rank.UnknownStatic.FormKey),
                ConditionCount = rank.Conditions?.Count ?? 0,
                ActivityCount = rank.Activities?.Count ?? 0,
                ImportedAtUTC = importedAtUTC,
                Effects = GetPerkRankEffects(record.FormKey, rank, rankIndex, importedAtUTC)
            })
            .ToList();
    }

    private static List<PerkRankEffectDTO> GetPerkRankEffects(FormKey formKey, IPerkRankGetter rank, int rankIndex, DateTime importedAtUTC)
    {
        return rank.Effects
            .Select((effect, effectIndex) =>
            {
                var dto = new PerkRankEffectDTO
                {
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

    private static List<PerkBackgroundSkillDTO> GetPerkBackgroundSkills(IPerkGetter record)
    {
        var importedAtUTC = DateTime.UtcNow;
        return record.BackgroundSkills
            .Select((skill, skillIndex) => new PerkBackgroundSkillDTO
            {
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

    private static object? GetPropertyValue(object source, string propertyName)
    {
        return source.GetType().GetProperty(propertyName)?.GetValue(source);
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
