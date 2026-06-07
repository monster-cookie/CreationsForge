using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Services;

public class RecordComparisonService : IRecordComparisonService
{
    private readonly IFormListRepository FormListRepository;
    private readonly IGameSettingRepository GameSettingRepository;
    private readonly IGlobalRepository GlobalRepository;
    private readonly IMiscObjectRepository MiscObjectRepository;
    private readonly IKeywordRepository KeywordRepository;
    private readonly IActorValueInformationRepository ActorValueInformationRepository;
    private readonly INPCRepository NPCRepository;
    private readonly IMagicEffectRepository MagicEffectRepository;
    private readonly IPerkRepository PerkRepository;
    private readonly IModelRepository ModelRepository;
    private readonly IRecordKeywordRepository RecordKeywordRepository;
    private readonly IRecordSoundRepository RecordSoundRepository;
    private readonly IScriptingAdapterRepository ScriptingAdapterRepository;

    public RecordComparisonService(
        IFormListRepository formListRepository,
        IGameSettingRepository gameSettingRepository,
        IGlobalRepository globalRepository,
        IMiscObjectRepository miscObjectRepository,
        IKeywordRepository keywordRepository,
        IActorValueInformationRepository actorValueInformationRepository,
        INPCRepository npcRepository,
        IMagicEffectRepository magicEffectRepository,
        IPerkRepository perkRepository,
        IModelRepository modelRepository,
        IRecordKeywordRepository recordKeywordRepository,
        IRecordSoundRepository recordSoundRepository,
        IScriptingAdapterRepository scriptingAdapterRepository)
    {
        FormListRepository = formListRepository;
        GameSettingRepository = gameSettingRepository;
        GlobalRepository = globalRepository;
        MiscObjectRepository = miscObjectRepository;
        KeywordRepository = keywordRepository;
        ActorValueInformationRepository = actorValueInformationRepository;
        NPCRepository = npcRepository;
        MagicEffectRepository = magicEffectRepository;
        PerkRepository = perkRepository;
        ModelRepository = modelRepository;
        RecordKeywordRepository = recordKeywordRepository;
        RecordSoundRepository = recordSoundRepository;
        ScriptingAdapterRepository = scriptingAdapterRepository;
    }

    public RecordComparisonDTO GetRecordComparison(SupportedGame game, string recordType, FormKeyDTO formKey)
    {
        if (recordType == RecordTypeCatalog.FormList.RecordID)
        {
            return CreateFormListComparison(game, formKey);
        }

        if (recordType == RecordTypeCatalog.GameSetting.RecordID)
        {
            return CreateGameSettingComparison(game, formKey);
        }

        if (recordType == RecordTypeCatalog.Global.RecordID)
        {
            return CreateGlobalComparison(game, formKey);
        }

        if (recordType == RecordTypeCatalog.MiscObject.RecordID)
        {
            return CreateMiscObjectComparison(game, formKey);
        }

        if (recordType == RecordTypeCatalog.Keyword.RecordID)
        {
            return CreateKeywordComparison(game, formKey);
        }

        if (recordType == RecordTypeCatalog.ActorValueInformation.RecordID)
        {
            return CreateActorValueInformationComparison(game, formKey);
        }

        if (recordType == RecordTypeCatalog.NPC.RecordID)
        {
            return CreateNPCComparison(game, formKey);
        }

        if (recordType == RecordTypeCatalog.MagicEffect.RecordID)
        {
            return CreateMagicEffectComparison(game, formKey);
        }

        if (recordType == RecordTypeCatalog.Perk.RecordID)
        {
            return CreatePerkComparison(game, formKey);
        }

        return new RecordComparisonDTO
        {
            RecordType = recordType,
            FormKey = formKey
        };
    }

    private RecordComparisonDTO CreateFormListComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = FormListRepository.GetByFormKey(game, formKey);
        var maxItemCount = records
            .Select(record => record.Items.Count)
            .DefaultIfEmpty()
            .Max();
        var fields = CreateCommonFields(records.Cast<RecordDTO>().ToList());
        fields.Add(CreateField("AddToListFormKey", records, record => FormatFormKey(record.AddToListFormKey)));
        for (var itemIndex = 0; itemIndex < maxItemCount; itemIndex++)
        {
            var currentIndex = itemIndex;
            fields.Add(CreateField($"Items[{itemIndex}]", records, record => FormatFormKey(record.Items.FirstOrDefault(item => item.ItemIndex == currentIndex)?.ItemFormKey)));
        }

        return CreateComparison(RecordTypeCatalog.FormList.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
    }

    private RecordComparisonDTO CreateGameSettingComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = GameSettingRepository.GetByFormKey(game, formKey);
        var fields = CreateCommonFields(records.Cast<RecordDTO>().ToList());
        fields.Add(CreateField("SettingType", records, record => record.SettingType ?? string.Empty));
        fields.Add(CreateField("Data", records, record => record.Data ?? string.Empty));

        return CreateComparison(RecordTypeCatalog.GameSetting.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
    }

    private RecordComparisonDTO CreateGlobalComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = GlobalRepository.GetByFormKey(game, formKey);
        var fields = CreateCommonFields(records.Cast<RecordDTO>().ToList());
        fields.Add(CreateField("Data", records, record => record.Data?.ToString() ?? string.Empty));

        return CreateComparison(RecordTypeCatalog.Global.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
    }

    private RecordComparisonDTO CreateMiscObjectComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = MiscObjectRepository.GetByFormKey(game, formKey);
        var fields = CreateCommonFields(records.Cast<RecordDTO>().ToList());
        fields.Add(CreateField("Name", records, record => record.Name ?? string.Empty));
        fields.Add(CreateField("ShortName", records, record => record.ShortName ?? string.Empty));
        fields.Add(CreateField("Value", records, record => record.Value?.ToString() ?? string.Empty));
        fields.Add(CreateField("Weight", records, record => record.Weight?.ToString() ?? string.Empty));
        fields.Add(CreateField("DirtinessScale", records, record => record.DirtinessScale?.ToString() ?? string.Empty));
        fields.Add(CreateField("FeaturedItemMessageFormKey", records, record => FormatFormKey(record.FeaturedItemMessageFormKey)));
        fields.Add(CreateField("Flag", records, record => record.Flag ?? string.Empty));
        AddKeywordGroup(fields, records.Cast<RecordDTO>().ToList(), RecordKeywordRepository.GetByFormKey(game, RecordTypeCatalog.MiscObject.RecordID, formKey));
        AddModelGroups(fields, records.Cast<RecordDTO>().ToList(), ModelRepository.GetByFormKey(game, RecordTypeCatalog.MiscObject.RecordID, formKey));
        AddSoundGroups(fields, records.Cast<RecordDTO>().ToList(), RecordSoundRepository.GetByFormKey(game, RecordTypeCatalog.MiscObject.RecordID, formKey));
        AddScriptingAdapterGroups(fields, records.Cast<RecordDTO>().ToList(), ScriptingAdapterRepository.GetByFormKey(game, RecordTypeCatalog.MiscObject.RecordID, formKey));

        return CreateComparison(RecordTypeCatalog.MiscObject.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
    }

    private RecordComparisonDTO CreateKeywordComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = KeywordRepository.GetByFormKey(game, formKey);
        var fields = CreateCommonFields(records.Cast<RecordDTO>().ToList());
        fields.Add(CreateField("Name", records, record => record.Name ?? string.Empty));
        fields.Add(CreateField("Color", records, record => record.Color));
        fields.Add(CreateField("Type", records, record => record.Type));
        fields.Add(CreateField("Notes", records, record => record.Notes ?? string.Empty));
        fields.Add(CreateField("FlashLinkageName", records, record => record.FlashLinkageName ?? string.Empty));
        fields.Add(CreateField("AttractionRuleFormKey", records, record => FormatFormKey(record.AttractionRuleFormKey)));

        return CreateComparison(RecordTypeCatalog.Keyword.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
    }

    private RecordComparisonDTO CreateActorValueInformationComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = ActorValueInformationRepository.GetByFormKey(game, formKey);
        var fields = CreateCommonFields(records.Cast<RecordDTO>().ToList());
        fields.Add(CreateField("Name", records, record => record.Name ?? string.Empty));
        fields.Add(CreateField("Abbreviation", records, record => record.Abbreviation ?? string.Empty));
        fields.Add(CreateField("ContextNotes", records, record => record.ContextNotes ?? string.Empty));
        fields.Add(CreateField("DefaultValue", records, record => record.DefaultValue?.ToString() ?? string.Empty));
        fields.Add(CreateField("Flags", records, record => record.Flags ?? string.Empty));
        fields.Add(CreateField("Type", records, record => record.Type ?? string.Empty));
        fields.Add(CreateField("Min", records, record => record.Min?.ToString() ?? string.Empty));
        fields.Add(CreateField("Max", records, record => record.Max?.ToString() ?? string.Empty));

        return CreateComparison(RecordTypeCatalog.ActorValueInformation.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
    }

    private RecordComparisonDTO CreateNPCComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = NPCRepository.GetByFormKey(game, formKey);
        var fields = CreateCommonFields(records.Cast<RecordDTO>().ToList());
        fields.Add(CreateField("Name", records, record => record.Name ?? string.Empty));
        fields.Add(CreateField("ShortName", records, record => record.ShortName ?? string.Empty));
        fields.Add(CreateField("LongName", records, record => record.LongName ?? string.Empty));
        fields.Add(CreateField("DispositionBase", records, record => record.DispositionBase.ToString()));
        fields.Add(CreateField("Aggression", records, record => record.Aggression));
        fields.Add(CreateField("Confidence", records, record => record.Confidence));
        fields.Add(CreateField("EnergyLevel", records, record => record.EnergyLevel.ToString()));
        fields.Add(CreateField("Responsibility", records, record => record.Responsibility));
        fields.Add(CreateField("Assistance", records, record => record.Assistance));
        fields.Add(CreateField("GearedUpWeapons", records, record => record.GearedUpWeapons.ToString()));
        fields.Add(CreateField("HeightMin", records, record => record.HeightMin.ToString()));
        fields.Add(CreateField("HeightMax", records, record => record.HeightMax.ToString()));
        fields.Add(CreateField("SkinToneIndex", records, record => record.SkinToneIndex?.ToString() ?? string.Empty));
        fields.Add(CreateField("Pronoun", records, record => record.Pronoun ?? string.Empty));
        fields.Add(CreateField("VoiceFormKey", records, record => FormatFormKey(record.VoiceFormKey)));
        fields.Add(CreateField("RaceFormKey", records, record => FormatFormKey(record.RaceFormKey)));
        fields.Add(CreateField("CombatOverridePackageListFormKey", records, record => FormatFormKey(record.CombatOverridePackageListFormKey)));
        fields.Add(CreateField("CombatStyleFormKey", records, record => FormatFormKey(record.CombatStyleFormKey)));
        fields.Add(CreateField("DefaultPackageListFormKey", records, record => FormatFormKey(record.DefaultPackageListFormKey)));
        fields.Add(CreateField("CrimeFactionFormKey", records, record => FormatFormKey(record.CrimeFactionFormKey)));
        AddKeywordGroup(fields, records.Cast<RecordDTO>().ToList(), RecordKeywordRepository.GetByFormKey(game, RecordTypeCatalog.NPC.RecordID, formKey));

        return CreateComparison(RecordTypeCatalog.NPC.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
    }

    private RecordComparisonDTO CreateMagicEffectComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = MagicEffectRepository.GetByFormKey(game, formKey);
        var fields = CreateCommonFields(records.Cast<RecordDTO>().ToList());
        fields.Add(CreateField("Name", records, record => record.Name ?? string.Empty));
        fields.Add(CreateField("Description", records, record => record.Description ?? string.Empty));
        fields.Add(CreateField("Flags", records, record => record.Flags));
        fields.Add(CreateField("CastType", records, record => record.CastType ?? string.Empty));
        fields.Add(CreateField("TargetType", records, record => record.TargetType ?? string.Empty));
        fields.Add(CreateField("ActorValue2FormKey", records, record => FormatFormKey(record.ActorValue2FormKey)));
        fields.Add(CreateField("ResistValueFormKey", records, record => FormatFormKey(record.ResistValueFormKey)));
        fields.Add(CreateField("PerkToApplyFormKey", records, record => FormatFormKey(record.PerkToApplyFormKey)));
        fields.Add(CreateField("EquipAbilityFormKey", records, record => FormatFormKey(record.EquipAbilityFormKey)));
        fields.Add(CreateField("ExplosionFormKey", records, record => FormatFormKey(record.ExplosionFormKey)));
        fields.Add(CreateField("CastingArtFormKey", records, record => FormatFormKey(record.CastingArtFormKey)));
        fields.Add(CreateField("HitEffectArtFormKey", records, record => FormatFormKey(record.HitEffectArtFormKey)));
        fields.Add(CreateField("HitShaderFormKey", records, record => FormatFormKey(record.HitShaderFormKey)));
        fields.Add(CreateField("ImageSpaceModifierFormKey", records, record => FormatFormKey(record.ImageSpaceModifierFormKey)));
        fields.Add(CreateField("ImpactDataFormKey", records, record => FormatFormKey(record.ImpactDataFormKey)));
        fields.Add(CreateField("ProjectileFormKey", records, record => FormatFormKey(record.ProjectileFormKey)));
        fields.Add(CreateField("Archetype", records, record => record.Archetype ?? string.Empty));
        fields.Add(CreateField("UnknownFloat3", records, record => record.UnknownFloat3?.ToString() ?? string.Empty));
        fields.Add(CreateField("UnknownInt2", records, record => record.UnknownInt2?.ToString() ?? string.Empty));
        fields.Add(CreateField("Unknown", records, record => record.Unknown ?? string.Empty));
        fields.Add(CreateField("Unknown2", records, record => record.Unknown2 ?? string.Empty));
        fields.Add(CreateField("DataTypeState", records, record => record.DataTypeState ?? string.Empty));
        AddKeywordGroup(fields, records.Cast<RecordDTO>().ToList(), RecordKeywordRepository.GetByFormKey(game, RecordTypeCatalog.MagicEffect.RecordID, formKey));
        AddSoundGroups(fields, records.Cast<RecordDTO>().ToList(), RecordSoundRepository.GetByFormKey(game, RecordTypeCatalog.MagicEffect.RecordID, formKey));
        AddScriptingAdapterGroups(fields, records.Cast<RecordDTO>().ToList(), ScriptingAdapterRepository.GetByFormKey(game, RecordTypeCatalog.MagicEffect.RecordID, formKey));

        return CreateComparison(RecordTypeCatalog.MagicEffect.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
    }

    private RecordComparisonDTO CreatePerkComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = PerkRepository.GetByFormKey(game, formKey);
        var fields = CreateCommonFields(records.Cast<RecordDTO>().ToList());
        fields.Add(CreateField("Name", records, record => record.Name ?? string.Empty));
        fields.Add(CreateField("Description", records, record => record.Description ?? string.Empty));
        fields.Add(CreateField("Flags", records, record => record.Flags));
        fields.Add(CreateField("SkillGroup", records, record => record.SkillGroup ?? string.Empty));
        fields.Add(CreateField("CrewAssignment", records, record => record.CrewAssignment ?? string.Empty));
        fields.Add(CreateField("PerkIcon", records, record => record.PerkIcon ?? string.Empty));
        fields.Add(CreateField("Category", records, record => record.Category ?? string.Empty));
        fields.Add(CreateField("RestrictionFormKey", records, record => FormatFormKey(record.RestrictionFormKey)));
        fields.Add(CreateField("TrainingFormKey", records, record => FormatFormKey(record.TrainingFormKey)));
        fields.Add(CreateField("MajorFlags", records, record => record.MajorFlags ?? string.Empty));

        return CreateComparison(RecordTypeCatalog.Perk.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
    }

    private static RecordComparisonDTO CreateComparison(
        string recordType,
        FormKeyDTO formKey,
        IReadOnlyList<RecordDTO> records,
        IReadOnlyList<RecordComparisonFieldDTO> fields)
    {
        return new RecordComparisonDTO
        {
            RecordType = recordType,
            FormKey = formKey,
            EditorID = records.FirstOrDefault()?.EditorID ?? string.Empty,
            Columns = records.Select(record => new RecordComparisonColumnDTO
            {
                ModKey = record.ModKey,
                Header = record.ModKey.FileName
            }).ToList(),
            Fields = fields
        };
    }

    private static List<RecordComparisonFieldDTO> CreateCommonFields(IReadOnlyList<RecordDTO> records)
    {
        return
        [
            CreateField("EditorID", records, record => record.EditorID),
            CreateField("FormVersion", records, record => record.FormVersion.ToString(), isComparable: false),
            CreateField("MajorRecordFlags", records, record => record.MajorRecordFlags.ToString(), isComparable: false)
        ];
    }

    private static RecordComparisonFieldDTO CreateField<TRecord>(
        string fieldName,
        IReadOnlyList<TRecord> records,
        Func<TRecord, string> valueFactory,
        bool isComparable = true)
        where TRecord : RecordDTO
    {
        var values = records.Select(record => new RecordComparisonValueDTO
            {
                ModKey = record.ModKey,
                DisplayValue = valueFactory(record)
            })
            .ToList();
        var state = GetComparisonValueState(values.Select(value => value.DisplayValue).ToList(), isComparable);
        for (var valueIndex = 0; valueIndex < values.Count; valueIndex++)
        {
            values[valueIndex].State = state == RecordComparisonValueState.Conflict && valueIndex == values.Count - 1
                ? RecordComparisonValueState.WinningOverride
                : state;
        }

        return new RecordComparisonFieldDTO
        {
            FieldName = fieldName,
            IsComparable = isComparable,
            State = state,
            Values = values
        };
    }

    private static void AddModelGroups(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<RecordDTO> records,
        IReadOnlyList<ModelDTO> models)
    {
        foreach (var modelKey in models.Select(model => new ModelKey(model.ModelSlot, model.ModelGender)).Distinct().OrderBy(key => key.Slot, StringComparer.Ordinal).ThenBy(key => key.Gender, StringComparer.Ordinal))
        {
            var modelFields = new List<RecordComparisonFieldDTO>
            {
                CreateChildField("File", records, record => FindModel(models, record.ModKey, modelKey)?.File ?? string.Empty),
                CreateChildField("Texture Hashes", records, record => FindModel(models, record.ModKey, modelKey)?.TextureFileHashes ?? string.Empty),
                CreateChildField("Light Layer", records, record => FindModel(models, record.ModKey, modelKey)?.LightLayer?.ToString() ?? string.Empty),
                CreateChildField("Flags", records, record => FindModel(models, record.ModKey, modelKey)?.Flags ?? string.Empty),
                CreateChildField("Color Remap", records, record => FindModel(models, record.ModKey, modelKey)?.ColorRemappingIndex?.ToString() ?? string.Empty),
                CreateChildField("Vestigial Flags", records, record => FindModel(models, record.ModKey, modelKey)?.FlagsVestigial ?? string.Empty)
            };

            var materialSwapIndexes = models
                .Where(model => IsModelKey(model, modelKey))
                .SelectMany(model => model.MaterialSwaps)
                .Select(materialSwap => materialSwap.MaterialSwapIndex)
                .Distinct()
                .Order()
                .ToList();
            foreach (var materialSwapIndex in materialSwapIndexes)
            {
                var currentIndex = materialSwapIndex;
                modelFields.Add(CreateChildField(
                    $"Material Swap [{materialSwapIndex}]",
                    records,
                    record => FormatFormKey(FindModel(models, record.ModKey, modelKey)?.MaterialSwaps.FirstOrDefault(materialSwap => materialSwap.MaterialSwapIndex == currentIndex)?.MaterialSwapFormKey)));
            }

            var visibleModelFields = modelFields
                .Where(HasVisibleValue)
                .ToList();
            if (visibleModelFields.Count > 0)
            {
                fields.Add(CreateGroupField(GetModelGroupName(modelKey), records, visibleModelFields));
            }
        }
    }

    private static void AddKeywordGroup(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<RecordDTO> records,
        IReadOnlyList<RecordKeywordDTO> keywords)
    {
        var keywordIndexes = keywords
            .Select(keyword => keyword.KeywordIndex)
            .Distinct()
            .Order()
            .ToList();
        if (keywordIndexes.Count == 0)
        {
            return;
        }

        var keywordFields = new List<RecordComparisonFieldDTO>();
        foreach (var keywordIndex in keywordIndexes)
        {
            var currentIndex = keywordIndex;
            keywordFields.Add(CreateChildField(
                $"Keyword [{keywordIndex}]",
                records,
                record => FormatFormKey(FindKeyword(keywords, record.ModKey, currentIndex)?.KeywordFormKey)));
        }

        fields.Add(CreateGroupField("Keywords", records, keywordFields));
    }

    private static void AddSoundGroups(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<RecordDTO> records,
        IReadOnlyList<RecordSoundDTO> sounds)
    {
        var soundKeys = sounds
            .Select(sound => new SoundKey(sound.SoundSlot, sound.SoundIndex))
            .Distinct()
            .OrderBy(key => key.Slot, StringComparer.Ordinal)
            .ThenBy(key => key.Index)
            .ToList();
        if (soundKeys.Count == 0)
        {
            return;
        }

        var soundFields = new List<RecordComparisonFieldDTO>();
        foreach (var soundKey in soundKeys)
        {
            var soundChildren = new List<RecordComparisonFieldDTO>
            {
                CreateChildField("Start", records, record => FindSound(sounds, record.ModKey, soundKey)?.Start ?? string.Empty),
                CreateChildField("Versioning", records, record => FindSound(sounds, record.ModKey, soundKey)?.Versioning ?? string.Empty),
                CreateChildField("Unknown", records, record => FindSound(sounds, record.ModKey, soundKey)?.Unknown ?? string.Empty)
            }
                .Where(HasVisibleValue)
                .ToList();
            if (soundChildren.Count > 0)
            {
                soundFields.Add(CreateGroupField(GetSoundGroupName(soundKey), records, soundChildren));
            }
        }

        if (soundFields.Count > 0)
        {
            fields.Add(CreateGroupField("Sounds", records, soundFields));
        }
    }

    private static void AddScriptingAdapterGroups(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<RecordDTO> records,
        IReadOnlyList<ScriptingAdapterDTO> scriptingAdapters)
    {
        var scriptFields = new List<RecordComparisonFieldDTO>();
        foreach (var scriptIndex in scriptingAdapters.Select(adapter => adapter.ScriptIndex).Distinct().Order())
        {
            var currentScriptIndex = scriptIndex;
            var scriptChildren = new List<RecordComparisonFieldDTO>
            {
                CreateChildField("Name", records, record => FindScriptingAdapter(scriptingAdapters, record.ModKey, currentScriptIndex)?.Name ?? string.Empty)
            };

            var propertyIndexes = scriptingAdapters
                .Where(adapter => adapter.ScriptIndex == currentScriptIndex)
                .SelectMany(adapter => adapter.Properties)
                .Select(property => property.PropertyIndex)
                .Distinct()
                .Order()
                .ToList();
            foreach (var propertyIndex in propertyIndexes)
            {
                var currentPropertyIndex = propertyIndex;
                var propertyChildren = new List<RecordComparisonFieldDTO>
                {
                    CreateChildField("Name", records, record => FindScriptingProperty(scriptingAdapters, record.ModKey, currentScriptIndex, currentPropertyIndex)?.Name ?? string.Empty),
                    CreateChildField("Type", records, record => FindScriptingProperty(scriptingAdapters, record.ModKey, currentScriptIndex, currentPropertyIndex)?.MutagenObjectType ?? string.Empty),
                    CreateChildField("Value", records, record => FormatScriptingPropertyValue(FindScriptingProperty(scriptingAdapters, record.ModKey, currentScriptIndex, currentPropertyIndex)))
                }
                    .Where(HasVisibleValue)
                    .ToList();
                if (propertyChildren.Count > 0)
                {
                    scriptChildren.Add(CreateGroupField($"Property [{propertyIndex}]", records, propertyChildren));
                }
            }

            var visibleScriptChildren = scriptChildren
                .Where(field => field.Children.Count > 0 || HasVisibleValue(field))
                .ToList();
            if (visibleScriptChildren.Count > 0)
            {
                scriptFields.Add(CreateGroupField($"Script [{scriptIndex}]", records, visibleScriptChildren));
            }
        }

        if (scriptFields.Count > 0)
        {
            fields.Add(CreateGroupField("Scripts", records, scriptFields));
        }
    }

    private static RecordComparisonFieldDTO CreateChildField(
        string fieldName,
        IReadOnlyList<RecordDTO> records,
        Func<RecordDTO, string> valueFactory)
    {
        return CreateField(fieldName, records, valueFactory);
    }

    private static RecordComparisonFieldDTO CreateGroupField(
        string fieldName,
        IReadOnlyList<RecordDTO> records,
        IReadOnlyList<RecordComparisonFieldDTO> children)
    {
        return new RecordComparisonFieldDTO
        {
            FieldName = fieldName,
            Values = records.Select(record => new RecordComparisonValueDTO
            {
                ModKey = record.ModKey,
                DisplayValue = string.Empty,
                State = RecordComparisonValueState.Neutral
            }).ToList(),
            State = RecordComparisonValueState.Neutral,
            IsComparable = false,
            Children = children
        };
    }

    private static bool HasVisibleValue(RecordComparisonFieldDTO field)
    {
        return field.Values.Any(value => !string.IsNullOrWhiteSpace(value.DisplayValue));
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

    private static string FormatFormKey(FormKeyDTO? formKey)
    {
        return formKey is null
            ? string.Empty
            : $"{formKey.ModKey.FileName}:{formKey.Id:X8}";
    }

    private static ModelDTO? FindModel(IReadOnlyList<ModelDTO> models, ModKeyDTO modKey, ModelKey modelKey)
    {
        return models.FirstOrDefault(model => IsSameModKey(model.ModKey, modKey) && IsModelKey(model, modelKey));
    }

    private static RecordKeywordDTO? FindKeyword(IReadOnlyList<RecordKeywordDTO> keywords, ModKeyDTO modKey, int keywordIndex)
    {
        return keywords.FirstOrDefault(keyword => IsSameModKey(keyword.ModKey, modKey) && keyword.KeywordIndex == keywordIndex);
    }

    private static RecordSoundDTO? FindSound(IReadOnlyList<RecordSoundDTO> sounds, ModKeyDTO modKey, SoundKey soundKey)
    {
        return sounds.FirstOrDefault(sound => IsSameModKey(sound.ModKey, modKey) &&
            string.Equals(sound.SoundSlot, soundKey.Slot, StringComparison.Ordinal) &&
            sound.SoundIndex == soundKey.Index);
    }

    private static ScriptingAdapterDTO? FindScriptingAdapter(IReadOnlyList<ScriptingAdapterDTO> scriptingAdapters, ModKeyDTO modKey, int scriptIndex)
    {
        return scriptingAdapters.FirstOrDefault(adapter => IsSameModKey(adapter.ModKey, modKey) && adapter.ScriptIndex == scriptIndex);
    }

    private static ScriptingAdapterPropertyDTO? FindScriptingProperty(
        IReadOnlyList<ScriptingAdapterDTO> scriptingAdapters,
        ModKeyDTO modKey,
        int scriptIndex,
        int propertyIndex)
    {
        return FindScriptingAdapter(scriptingAdapters, modKey, scriptIndex)?.Properties.FirstOrDefault(property => property.PropertyIndex == propertyIndex);
    }

    private static string FormatScriptingPropertyValue(ScriptingAdapterPropertyDTO? property)
    {
        if (property is null)
        {
            return string.Empty;
        }

        if (property.ListItems.Count > 0)
        {
            return string.Join(", ", property.ListItems.OrderBy(item => item.ListItemIndex).Select(FormatScriptingPropertyListItemValue));
        }

        return FormatScriptingValue(
            property.DataBool,
            property.DataInt,
            property.DataFloat,
            property.DataString,
            property.ObjectFormKey,
            property.ObjectAlias,
            property.ObjectUnused);
    }

    private static string FormatScriptingPropertyListItemValue(ScriptingAdapterPropertyListItemDTO listItem)
    {
        return FormatScriptingValue(
            listItem.DataBool,
            listItem.DataInt,
            listItem.DataFloat,
            listItem.DataString,
            listItem.ObjectFormKey,
            listItem.ObjectAlias,
            listItem.ObjectUnused);
    }

    private static string FormatScriptingValue(
        bool? dataBool,
        int? dataInt,
        double? dataFloat,
        string? dataString,
        FormKeyDTO? objectFormKey,
        short? objectAlias,
        ushort? objectUnused)
    {
        if (dataBool.HasValue)
        {
            return dataBool.Value.ToString();
        }

        if (dataInt.HasValue)
        {
            return dataInt.Value.ToString();
        }

        if (dataFloat.HasValue)
        {
            return dataFloat.Value.ToString();
        }

        if (!string.IsNullOrEmpty(dataString))
        {
            return dataString;
        }

        if (objectFormKey is not null)
        {
            return FormatFormKey(objectFormKey);
        }

        if (objectAlias.HasValue)
        {
            return objectAlias.Value.ToString();
        }

        return objectUnused?.ToString() ?? string.Empty;
    }

    private static bool IsModelKey(ModelDTO model, ModelKey modelKey)
    {
        return string.Equals(model.ModelSlot, modelKey.Slot, StringComparison.Ordinal) &&
            string.Equals(model.ModelGender, modelKey.Gender, StringComparison.Ordinal);
    }

    private static string GetModelGroupName(ModelKey modelKey)
    {
        if (string.Equals(modelKey.Slot, "Model", StringComparison.Ordinal) && string.IsNullOrWhiteSpace(modelKey.Gender))
        {
            return "Model";
        }

        return string.IsNullOrWhiteSpace(modelKey.Gender)
            ? modelKey.Slot + " Model"
            : $"{modelKey.Slot} Model ({modelKey.Gender})";
    }

    private static string GetSoundGroupName(SoundKey soundKey)
    {
        return soundKey.Index == 0
            ? soundKey.Slot
            : $"{soundKey.Slot} [{soundKey.Index}]";
    }

    private static bool IsSameModKey(ModKeyDTO first, ModKeyDTO second)
    {
        return first.Type == second.Type &&
            string.Equals(first.Name, second.Name, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(first.FileName, second.FileName, StringComparison.OrdinalIgnoreCase);
    }

    private sealed record ModelKey(string Slot, string Gender);

    private sealed record SoundKey(string Slot, int Index);
}
