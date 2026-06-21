using System.Globalization;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.DataValidationTests.Validation.Environment;
using CreationsForge.DataValidationTests.Validation.Parsing;
using CreationsForge.DataValidationTests.Validation.Services;

namespace CreationsForge.DataValidationTests.Validation.Tests;

public static class Helpers
{
    private static readonly SpriggitEnvironmentLoader EnvironmentLoader = new();
    private static readonly Lazy<SpriggitEnvironmentConfiguration> SpriggitEnvironment = new(() => EnvironmentLoader.Load());
    private static readonly GameRecordSetProvider RecordSetProvider = new();
    private static readonly DtoFlattener DtoFlattener = new();

    private static readonly IReadOnlyDictionary<string, string> GlobalSpriggitToDtoFields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["FormKey"] = "FormKey",
        ["MajorRecordFlagsRaw"] = "MajorRecordFlags",
        ["FormVersion"] = "FormVersion",
        ["Data"] = "Data"
    };

    private static readonly IReadOnlyDictionary<string, string> SpriggitToDtoFields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["MajorRecordFlagsRaw"] = "MajorRecordFlags",
        ["ObjectBounds.First"] = "ObjectBoundsFirst",
        ["ObjectBounds.Second"] = "ObjectBoundsSecond",
        ["Transforms.Inventory"] = "Transforms.Inventory",
        ["InventoryArt"] = "InventoryArt",
        ["PreviewTransform"] = "PreviewTransform",
        ["NativeTerminal"] = "NativeTerminalFormKey",
        ["Model.File"] = "Models[0].File",
        ["Model.LightLayer"] = "Models[0].LightLayer"
    };

    public static TSpriggit GetSpriggit<TSpriggit>(SupportedGame game, RecordTypeData recordType, string sampleName)
        where TSpriggit : class
    {
        if (typeof(TSpriggit) == typeof(GlobalSpriggitDTO))
        {
            return (TSpriggit)(object)GetGlobalSpriggit(game, recordType, sampleName);
        }

        if (typeof(TSpriggit) == typeof(SpriggitRecordDTO))
        {
            return (TSpriggit)(object)GetSpriggitRecord(game, recordType, sampleName);
        }

        throw new InvalidOperationException("Unsupported Spriggit validation DTO type '" + typeof(TSpriggit).Name + "'.");
    }

    public static TRecord GetDTO<TRecord>(SupportedGame game, RecordTypeData recordType, string formKey)
        where TRecord : RecordDTO
    {
        var record = RecordSetProvider.GetRecord(game, recordType.RecordID, formKey);
        if (record is not TRecord typedRecord)
        {
            throw new InvalidOperationException("Reader DTO should match the expected test DTO type for " + game + " " + recordType.RecordID + " " + formKey + ". Actual type: " + record.GetType().Name + ".");
        }

        return typedRecord;
    }

    private static GlobalSpriggitDTO GetGlobalSpriggit(SupportedGame game, RecordTypeData recordType, string sampleName)
    {
        if (recordType.RecordID != RecordTypeCatalog.Global.RecordID)
        {
            throw new InvalidOperationException("This Spriggit helper currently supports Global records.");
        }

        var path = FindSpriggitFile(game, "Globals", sampleName);
        var document = SpriggitYamlDocument.Load(path);
        var fields = document.FlattenScalars();

        return new GlobalSpriggitDTO
        {
            FormKey = GetRequiredString(fields, "FormKey", path),
            MajorRecordFlagsRaw = GetOptionalInt(fields, "MajorRecordFlagsRaw"),
            FormVersion = GetOptionalInt(fields, "FormVersion"),
            Data = GetOptionalDouble(fields, "Data"),
            Fields = fields
        };
    }

    private static SpriggitRecordDTO GetSpriggitRecord(SupportedGame game, RecordTypeData recordType, string sampleName)
    {
        var path = FindSpriggitFile(game, recordType.TableName, sampleName);
        var document = SpriggitYamlDocument.Load(path);
        var fields = AddRootScalarLists(path, document.FlattenScalars());

        return new SpriggitRecordDTO
        {
            FormKey = GetRequiredString(fields, "FormKey", path),
            Fields = fields
        };
    }

    public static string FormatFormKey(FormKeyDTO formKey)
    {
        return formKey.Id.ToString("X6", CultureInfo.InvariantCulture) + ":" + formKey.ModKey.FileName;
    }

    public static IReadOnlyList<string> GetUnmatchedSpriggitFields(GlobalSpriggitDTO spriggit, GlobalDTO dto)
    {
        _ = dto;
        var unmatchedFields = new List<string>();

        foreach (var field in spriggit.Fields.OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase))
        {
            if (!GlobalSpriggitToDtoFields.ContainsKey(field.Key))
            {
                unmatchedFields.Add(
                    "No matching CreationsForge reader DTO data was found for Spriggit field '" + field.Key + "'." +
                    System.Environment.NewLine +
                    "Spriggit value: " + field.Value +
                    System.Environment.NewLine +
                    "Record: " + spriggit.FormKey);
            }
        }

        return unmatchedFields;
    }

    public static IReadOnlyList<string> GetUnmatchedSpriggitFields<TRecord>(SpriggitRecordDTO spriggit, TRecord dto)
        where TRecord : RecordDTO
    {
        var unmatchedFields = new List<string>();
        var dtoFields = GetDTOFields(dto);

        foreach (var field in spriggit.Fields.OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase))
        {
            if (!IsMatchedSpriggitField(field.Key, field.Value, spriggit.Fields, dtoFields))
            {
                unmatchedFields.Add(
                    "No matching CreationsForge reader DTO field was found for Spriggit field '" + field.Key + "'." +
                    System.Environment.NewLine +
                    "Spriggit value: " + field.Value +
                    System.Environment.NewLine +
                    "Record: " + spriggit.FormKey);
            }
        }

        return unmatchedFields;
    }

    public static IReadOnlyList<string> GetUnmatchedDtoFields(GlobalSpriggitDTO spriggit, GlobalDTO dto)
    {
        var unmatchedFields = new List<string>();
        var dtoFields = GetDTOFields(dto);
        var matchedDtoFields = new HashSet<string>(GlobalSpriggitToDtoFields.Values, StringComparer.OrdinalIgnoreCase);

        foreach (var field in dtoFields.OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase))
        {
            if (!matchedDtoFields.Contains(field.Key))
            {
                unmatchedFields.Add(
                    "No matching Spriggit field was found for CreationsForge reader DTO field '" + field.Key + "'." +
                    System.Environment.NewLine +
                    "DTO value: " + field.Value +
                    System.Environment.NewLine +
                    "Record: " + spriggit.FormKey);
            }
        }

        return unmatchedFields;
    }

    public static IReadOnlyList<string> GetUnmatchedDtoFields<TRecord>(SpriggitRecordDTO spriggit, TRecord dto)
        where TRecord : RecordDTO
    {
        var unmatchedFields = new List<string>();
        var dtoFields = GetDTOFields(dto);

        foreach (var field in dtoFields.OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase))
        {
            if (!IsMatchedDtoField(field.Key, field.Value, spriggit.Fields, dtoFields))
            {
                unmatchedFields.Add(
                    "No matching Spriggit field was found for CreationsForge reader DTO field '" + field.Key + "'." +
                    System.Environment.NewLine +
                    "DTO value: " + field.Value +
                    System.Environment.NewLine +
                    "Record: " + spriggit.FormKey);
            }
        }

        return unmatchedFields;
    }

    public static IReadOnlyDictionary<string, string> GetDTOFields<TRecord>(TRecord dto)
        where TRecord : RecordDTO
    {
        var fields = new Dictionary<string, string>(DtoFlattener.Flatten(dto), StringComparer.OrdinalIgnoreCase);
        AddSpriggitFieldAlias(fields, "ObjectBoundsFirst", "ObjectBounds.First");
        AddSpriggitFieldAlias(fields, "ObjectBoundsSecond", "ObjectBounds.Second");
        AddSpriggitFieldAlias(fields, "XALG", "XALG");
        AddSpriggitFieldAlias(fields, "Models[0].File", "Model.File");
        AddSpriggitFieldAlias(fields, "Models[0].LightLayer", "Model.LightLayer");
        AddSpriggitFieldAlias(fields, "Models[0].Flags", "Model.Flags");
        AddSpriggitScalarListAliases(fields, "Flags");
        AddSpriggitScalarListAliases(fields, "Model.Flags");
        AddSpriggitKeywordAliases(fields);
        AddSpriggitModelMaterialSwapAliases(fields);
        AddSpriggitRawPayloadAliases(fields);
        AddSpriggitSoundAliases(fields);
        AddSpriggitScriptingAdapterAliases(fields);
        AddActorValueInformationAliases(fields);
        return fields;
    }

    private static void AddSpriggitFieldAlias(IDictionary<string, string> fields, string dtoFieldName, string spriggitFieldName)
    {
        if (fields.TryGetValue(dtoFieldName, out var value))
        {
            fields[spriggitFieldName] = value;
        }
    }

    private static void AddSpriggitTranslatedStringAliases(IDictionary<string, string> fields, string dtoRootFieldName, string spriggitRootFieldName)
    {
        foreach (var field in fields.Where(field => field.Key.StartsWith(dtoRootFieldName + ".", StringComparison.OrdinalIgnoreCase) ||
                                                    field.Key.StartsWith(dtoRootFieldName + "[", StringComparison.OrdinalIgnoreCase)).ToList())
        {
            fields[spriggitRootFieldName + field.Key[dtoRootFieldName.Length..]] = field.Value;
        }
    }

    private static void AddSpriggitScalarListAliases(IDictionary<string, string> fields, string fieldName)
    {
        if (!fields.TryGetValue(fieldName, out var value) || string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        if (IsZero(value))
        {
            return;
        }

        var values = value.Split(", ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        fields[fieldName + ".Count"] = values.Length.ToString(CultureInfo.InvariantCulture);
        for (var index = 0; index < values.Length; index++)
        {
            fields[fieldName + "[" + index.ToString(CultureInfo.InvariantCulture) + "]"] = values[index];
        }
    }

    private static void AddSpriggitSoundAliases(IDictionary<string, string> fields)
    {
        if (!fields.TryGetValue("Sounds.Count", out var countValue) ||
            !int.TryParse(countValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var count))
        {
            return;
        }

        for (var index = 0; index < count; index++)
        {
            var soundPath = "Sounds[" + index.ToString(CultureInfo.InvariantCulture) + "]";
            if (!fields.TryGetValue(soundPath + ".SoundSlot", out var soundSlot))
            {
                continue;
            }

            AddSpriggitSoundAlias(fields, soundPath, soundSlot, "Start");
            AddSpriggitSoundAlias(fields, soundPath, soundSlot, "Versioning");
            AddSpriggitSoundAlias(fields, soundPath, soundSlot, "Unknown");
        }
    }

    private static void AddSpriggitKeywordAliases(IDictionary<string, string> fields)
    {
        if (!fields.TryGetValue("Keywords.Count", out var countValue) ||
            !int.TryParse(countValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var count))
        {
            return;
        }

        for (var index = 0; index < count; index++)
        {
            AddSpriggitFieldAlias(
                fields,
                "Keywords[" + index.ToString(CultureInfo.InvariantCulture) + "].KeywordFormKey",
                "Keywords[" + index.ToString(CultureInfo.InvariantCulture) + "]");
        }
    }

    private static void AddSpriggitModelMaterialSwapAliases(IDictionary<string, string> fields)
    {
        if (!fields.TryGetValue("Models[0].MaterialSwaps.Count", out var countValue) ||
            !int.TryParse(countValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var count))
        {
            return;
        }

        fields["Model.MaterialSwaps.Count"] = count.ToString(CultureInfo.InvariantCulture);
        for (var index = 0; index < count; index++)
        {
            AddSpriggitFieldAlias(
                fields,
                "Models[0].MaterialSwaps[" + index.ToString(CultureInfo.InvariantCulture) + "].MaterialSwapFormKey",
                "Model.MaterialSwaps[" + index.ToString(CultureInfo.InvariantCulture) + "]");
        }

        if (count == 1)
        {
            AddSpriggitFieldAlias(fields, "Models[0].MaterialSwaps[0].MaterialSwapFormKey", "Model.MaterialSwap");
        }
    }

    private static void AddActorValueInformationAliases(IDictionary<string, string> fields)
    {
        AddSpriggitFieldAlias(fields, "Cnam", "CNAM");
        AddSpriggitFieldAlias(fields, "SkillImproveMult", "Skill.ImproveMult");
        AddSpriggitFieldAlias(fields, "SkillImproveOffset", "Skill.ImproveOffset");
        AddSpriggitFieldAlias(fields, "SkillUseMult", "Skill.UseMult");

        if (fields.TryGetValue("LayoutEntries.Count", out var layoutCountValue) &&
            int.TryParse(layoutCountValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var layoutCount))
        {
            for (var index = 0; index < layoutCount; index++)
            {
                var layoutPath = "LayoutEntries[" + index.ToString(CultureInfo.InvariantCulture) + "]";
                var spriggitPath = "PerkTree[" + index.ToString(CultureInfo.InvariantCulture) + "]";
                AddSpriggitFieldAlias(fields, layoutPath + ".AssociatedSkillFormKey", spriggitPath + ".AssociatedSkill");
                AddSpriggitFieldAlias(fields, layoutPath + ".Fnam", spriggitPath + ".FNAM");
                AddSpriggitFieldAlias(fields, layoutPath + ".HorizontalPosition", spriggitPath + ".HorizontalPosition");
                AddSpriggitFieldAlias(fields, layoutPath + ".Index", spriggitPath + ".Index");
                AddSpriggitFieldAlias(fields, layoutPath + ".PerkGridX", spriggitPath + ".PerkGridX");
                AddSpriggitFieldAlias(fields, layoutPath + ".PerkGridY", spriggitPath + ".PerkGridY");
                AddSpriggitFieldAlias(fields, layoutPath + ".VerticalPosition", spriggitPath + ".VerticalPosition");
            }
        }

        if (fields.TryGetValue("PerkTree.Count", out var perkTreeCountValue) &&
            int.TryParse(perkTreeCountValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var perkTreeCount))
        {
            for (var index = 0; index < perkTreeCount; index++)
            {
                var perkTreePath = "PerkTree[" + index.ToString(CultureInfo.InvariantCulture) + "]";
                AddSpriggitFieldAlias(fields, perkTreePath + ".Fnam", perkTreePath + ".FNAM");
                AddSpriggitFieldAlias(fields, perkTreePath + ".PerkFormKey", perkTreePath + ".Perk");
            }
        }
    }

    private static void AddSpriggitSoundAlias(IDictionary<string, string> fields, string soundPath, string soundSlot, string fieldName)
    {
        if (fields.TryGetValue(soundPath + "." + fieldName, out var value))
        {
            fields[soundSlot + "." + fieldName] = value;
        }
    }

    private static void AddSpriggitRawPayloadAliases(IDictionary<string, string> fields)
    {
        if (!fields.TryGetValue("RawPayloads.Count", out var countValue) ||
            !int.TryParse(countValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var count))
        {
            return;
        }

        for (var index = 0; index < count; index++)
        {
            var payloadPath = "RawPayloads[" + index.ToString(CultureInfo.InvariantCulture) + "]";
            if (!fields.TryGetValue(payloadPath + ".PayloadValue", out var payloadValue))
            {
                continue;
            }

            var spriggitFieldName = GetSpriggitRawPayloadFieldName(fields, payloadPath);
            if (!string.IsNullOrWhiteSpace(spriggitFieldName))
            {
                fields[spriggitFieldName] = FormatSpriggitHexPayload(payloadValue);
            }
        }
    }

    private static string FormatSpriggitHexPayload(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.StartsWith("0x", StringComparison.OrdinalIgnoreCase) || !IsHexPayloadValue(value))
        {
            return value;
        }

        return "0x" + value;
    }

    private static bool IsHexPayloadValue(string value)
    {
        return value.Length % 2 == 0 &&
               value.All(character =>
                   character is >= '0' and <= '9' ||
                   character is >= 'A' and <= 'F' ||
                   character is >= 'a' and <= 'f');
    }

    private static string GetSpriggitRawPayloadFieldName(IDictionary<string, string> fields, string payloadPath)
    {
        if (fields.TryGetValue(payloadPath + ".SourcePath", out var sourcePath) &&
            TryGetRawPayloadLeafField(sourcePath, out var sourceLeafField))
        {
            return sourceLeafField;
        }

        if (fields.TryGetValue(payloadPath + ".PayloadSlot", out var payloadSlot) &&
            TryGetRawPayloadLeafField(payloadSlot, out var payloadLeafField))
        {
            return payloadLeafField;
        }

        return string.Empty;
    }

    private static string GetSpriggitRawPayloadFieldName(IReadOnlyDictionary<string, string> fields, string payloadPath)
    {
        if (fields.TryGetValue(payloadPath + ".SourcePath", out var sourcePath) &&
            TryGetRawPayloadLeafField(sourcePath, out var sourceLeafField))
        {
            return sourceLeafField;
        }

        if (fields.TryGetValue(payloadPath + ".PayloadSlot", out var payloadSlot) &&
            TryGetRawPayloadLeafField(payloadSlot, out var payloadLeafField))
        {
            return payloadLeafField;
        }

        return string.Empty;
    }

    private static bool HasSpriggitRawPayloadField(IReadOnlyDictionary<string, string> spriggitFields, string fieldName)
    {
        return spriggitFields.ContainsKey(fieldName) ||
               spriggitFields.Keys.Any(field =>
                   field.EndsWith("." + fieldName, StringComparison.OrdinalIgnoreCase) &&
                   TryGetIndexedPath(field, "Components", out _, out _));
    }

    private static bool TryGetRawPayloadLeafField(string path, out string fieldName)
    {
        if (string.Equals(path, "Model.Data", StringComparison.OrdinalIgnoreCase))
        {
            fieldName = "Model.Data";
            return true;
        }

        var separatorIndex = path.LastIndexOf('.');
        fieldName = separatorIndex < 0
            ? path
            : path[(separatorIndex + 1)..];

        return fieldName is "ANAM" or "BNAM" or "CNAM" or "REFL";
    }

    private static void AddSpriggitScriptingAdapterAliases(IDictionary<string, string> fields)
    {
        if (!fields.TryGetValue("ScriptingAdapters.Count", out var countValue) ||
            !int.TryParse(countValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var count))
        {
            return;
        }

        fields["VirtualMachineAdapter.Count"] = count.ToString(CultureInfo.InvariantCulture);
        fields["VirtualMachineAdapter.Scripts.Count"] = count.ToString(CultureInfo.InvariantCulture);
        for (var index = 0; index < count; index++)
        {
            AddSpriggitScriptingAdapterAlias(fields, index);
        }
    }

    private static void AddSpriggitScriptingAdapterAlias(IDictionary<string, string> fields, int scriptIndex)
    {
        var dtoScriptPath = "ScriptingAdapters[" + scriptIndex.ToString(CultureInfo.InvariantCulture) + "]";
        var spriggitScriptPath = "VirtualMachineAdapter[" + scriptIndex.ToString(CultureInfo.InvariantCulture) + "]";
        var spriggitScriptsPath = "VirtualMachineAdapter.Scripts[" + scriptIndex.ToString(CultureInfo.InvariantCulture) + "]";
        AddSpriggitFieldAlias(fields, dtoScriptPath + ".Name", spriggitScriptPath + ".Name");
        AddSpriggitFieldAlias(fields, dtoScriptPath + ".Name", spriggitScriptsPath + ".Name");
        AddSpriggitFieldAlias(fields, dtoScriptPath + ".Properties.Count", spriggitScriptPath + ".Count");
        AddSpriggitFieldAlias(fields, dtoScriptPath + ".Properties.Count", spriggitScriptsPath + ".Properties.Count");

        if (!fields.TryGetValue(dtoScriptPath + ".Properties.Count", out var propertyCountValue) ||
            !int.TryParse(propertyCountValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var propertyCount))
        {
            return;
        }

        for (var propertyIndex = 0; propertyIndex < propertyCount; propertyIndex++)
        {
            AddSpriggitScriptingAdapterPropertyAlias(fields, dtoScriptPath, spriggitScriptPath, propertyIndex);
            AddSpriggitScriptingAdapterPropertyAlias(fields, dtoScriptPath, spriggitScriptsPath + ".Properties", propertyIndex);
        }
    }

    private static void AddSpriggitScriptingAdapterPropertyAlias(IDictionary<string, string> fields, string dtoScriptPath, string spriggitScriptPath, int propertyIndex)
    {
        var dtoPropertyPath = dtoScriptPath + ".Properties[" + propertyIndex.ToString(CultureInfo.InvariantCulture) + "]";
        var spriggitPropertyPath = spriggitScriptPath + "[" + propertyIndex.ToString(CultureInfo.InvariantCulture) + "]";
        AddSpriggitFieldAlias(fields, dtoPropertyPath + ".MutagenObjectType", spriggitPropertyPath + ".MutagenObjectType");
        AddSpriggitFieldAlias(fields, dtoPropertyPath + ".Name", spriggitPropertyPath + ".Name");
        AddSpriggitFieldAlias(fields, dtoPropertyPath + ".ObjectFormKey", spriggitPropertyPath + ".Object");
        AddSpriggitScriptingAdapterDataAlias(fields, dtoPropertyPath, spriggitPropertyPath);
        AddSpriggitFieldAlias(fields, dtoPropertyPath + ".ListItems.Count", spriggitPropertyPath + ".Count");

        if (!fields.TryGetValue(dtoPropertyPath + ".ListItems.Count", out var listItemCountValue) ||
            !int.TryParse(listItemCountValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var listItemCount))
        {
            return;
        }

        fields[spriggitPropertyPath + ".Objects.Count"] = listItemCount.ToString(CultureInfo.InvariantCulture);
        for (var listItemIndex = 0; listItemIndex < listItemCount; listItemIndex++)
        {
            var dtoListItemPath = dtoPropertyPath + ".ListItems[" + listItemIndex.ToString(CultureInfo.InvariantCulture) + "]";
            var spriggitListItemPath = spriggitPropertyPath + "[" + listItemIndex.ToString(CultureInfo.InvariantCulture) + "]";
            var spriggitObjectListItemPath = spriggitPropertyPath + ".Objects[" + listItemIndex.ToString(CultureInfo.InvariantCulture) + "]";
            AddSpriggitFieldAlias(fields, dtoListItemPath + ".MutagenObjectType", spriggitListItemPath + ".MutagenObjectType");
            AddSpriggitFieldAlias(fields, dtoListItemPath + ".ObjectFormKey", spriggitListItemPath + ".Object");
            AddSpriggitFieldAlias(fields, dtoListItemPath + ".MutagenObjectType", spriggitObjectListItemPath + ".MutagenObjectType");
            AddSpriggitFieldAlias(fields, dtoListItemPath + ".ObjectFormKey", spriggitObjectListItemPath + ".Object");
            AddSpriggitScriptingAdapterDataAlias(fields, dtoListItemPath, spriggitListItemPath);
            AddSpriggitScriptingAdapterDataAlias(fields, dtoListItemPath, spriggitObjectListItemPath);
        }
    }

    private static void AddSpriggitScriptingAdapterDataAlias(IDictionary<string, string> fields, string dtoPath, string spriggitPath)
    {
        AddSpriggitFieldAlias(fields, dtoPath + ".DataBool", spriggitPath + ".Data");
        AddSpriggitFieldAlias(fields, dtoPath + ".DataFloat", spriggitPath + ".Data");
        AddSpriggitFieldAlias(fields, dtoPath + ".DataInt", spriggitPath + ".Data");
        AddSpriggitFieldAlias(fields, dtoPath + ".DataString", spriggitPath + ".Data");
    }

    public static IReadOnlyList<string> GetSpriggitListValues(SpriggitRecordDTO spriggit, string fieldName)
    {
        return spriggit.Fields
            .Where(field => TryGetListIndex(field.Key, fieldName, out _))
            .OrderBy(field =>
            {
                TryGetListIndex(field.Key, fieldName, out var index);
                return index;
            })
            .Select(field => field.Value)
            .ToList();
    }

    private static bool IsMatchedSpriggitField(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> spriggitFields,
        IReadOnlyDictionary<string, string> dtoFields)
    {
        if (dtoFields.ContainsKey(fieldName))
        {
            return true;
        }

        if (SpriggitToDtoFields.TryGetValue(fieldName, out var dtoFieldName) && dtoFields.ContainsKey(dtoFieldName))
        {
            return true;
        }

        if (IsCommonMetadataFieldOutsideRepositoryReadback(fieldName, dtoFields))
        {
            return true;
        }

        if (IsSpriggitLocalizedFieldBackedByDtoFallback(fieldName, spriggitFields, dtoFields))
        {
            return true;
        }

        if ((fieldName.EndsWith("Sound", StringComparison.OrdinalIgnoreCase) || fieldName.EndsWith("SoundLevel", StringComparison.OrdinalIgnoreCase)) &&
            dtoFields.ContainsKey(fieldName + ".Start"))
        {
            return true;
        }

        if (IsAlternativeRootFieldBackedByDtoField(fieldName, dtoFields, "BookText", "Text"))
        {
            return true;
        }

        if (IsEmptySpriggitTranslationTargetLanguage(fieldName, spriggitFields, dtoFields))
        {
            return true;
        }

        if (IsEmptySpriggitVirtualMachineAdapterListItemName(fieldName, spriggitFields))
        {
            return true;
        }

        if (IsSpriggitObjectListItemMutagenType(fieldName, spriggitFields, dtoFields, "Components"))
        {
            return true;
        }

        if (IsSpriggitComponentRawPayloadBackedByDtoRawPayload(fieldName, spriggitFields, dtoFields))
        {
            return true;
        }

        if (IsSpriggitActorValueInformationFieldBackedByDtoField(fieldName, spriggitFields, dtoFields))
        {
            return true;
        }

        if (IsSpriggitInlineFormKeyListItemBackedByDtoScalar(fieldName, fieldValue, dtoFields, "Keywords"))
        {
            return true;
        }

        return IsSpriggitListBackedDtoScalar(fieldName, spriggitFields, dtoFields, "Flags");
    }

    private static bool IsMatchedDtoField(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> spriggitFields,
        IReadOnlyDictionary<string, string> dtoFields)
    {
        if (spriggitFields.ContainsKey(fieldName))
        {
            return true;
        }

        if (SpriggitToDtoFields.Any(field => string.Equals(field.Value, fieldName, StringComparison.OrdinalIgnoreCase) && spriggitFields.ContainsKey(field.Key)))
        {
            return true;
        }

        if (IsDtoLocalizedFallbackBackedBySpriggitField(fieldName, fieldValue, spriggitFields, dtoFields))
        {
            return true;
        }

        if (IsDtoFieldBackedByAlternativeRootField(fieldName, spriggitFields, "Text", "BookText"))
        {
            return true;
        }

        if (string.Equals(fieldName, "InventoryArt", StringComparison.OrdinalIgnoreCase) && spriggitFields.ContainsKey("InventoryArt"))
        {
            return true;
        }

        if (string.Equals(fieldName, "InventoryArt", StringComparison.OrdinalIgnoreCase) && spriggitFields.ContainsKey("Transforms.Inventory"))
        {
            return true;
        }

        if (string.Equals(fieldName, "Transforms.Inventory", StringComparison.OrdinalIgnoreCase) && spriggitFields.ContainsKey("InventoryArt"))
        {
            return true;
        }

        if (string.Equals(fieldName, "Teaches.RawContent", StringComparison.OrdinalIgnoreCase) &&
            spriggitFields.ContainsKey("Teaches.RawContent"))
        {
            return true;
        }

        if (string.Equals(fieldName, "Teaches.MutagenObjectType", StringComparison.OrdinalIgnoreCase) &&
            spriggitFields.ContainsKey("Teaches.MutagenObjectType"))
        {
            return true;
        }

        if (fieldName.EndsWith(".Start", StringComparison.OrdinalIgnoreCase) &&
            spriggitFields.TryGetValue(fieldName[..^".Start".Length], out var linkedSound) &&
            string.Equals(fieldValue, linkedSound, StringComparison.Ordinal))
        {
            return true;
        }

        if (string.Equals(fieldName, "MajorRecordFlags", StringComparison.OrdinalIgnoreCase) && IsZero(fieldValue))
        {
            return true;
        }

        if ((string.Equals(fieldName, "Version2", StringComparison.OrdinalIgnoreCase) ||
             string.Equals(fieldName, "VersionControl", StringComparison.OrdinalIgnoreCase)) &&
            !spriggitFields.ContainsKey(fieldName))
        {
            return true;
        }

        if (IsMissingZeroCount(fieldName, fieldValue, spriggitFields))
        {
            return true;
        }

        if (IsMissingDefaultDtoField(fieldName, fieldValue, spriggitFields))
        {
            return true;
        }

        if (IsMissingDefaultActorValueInformationSkillOffset(fieldName, fieldValue, spriggitFields))
        {
            return true;
        }

        if (IsDtoChildInfrastructureField(fieldName))
        {
            return true;
        }

        if (IsDtoIndexedPropertyBackedBySpriggitScalarList(fieldName, spriggitFields, "Keywords", "KeywordFormKey"))
        {
            return true;
        }

        if (IsDtoIndexedMetadataBackedBySpriggitScalarList(fieldName, fieldValue, spriggitFields, "Keywords", "KeywordIndex"))
        {
            return true;
        }

        if (IsDtoIndexedMetadataBackedBySpriggitScalarList(fieldName, fieldValue, spriggitFields, "Components", "ComponentIndex"))
        {
            return true;
        }

        if (IsDtoComponentMutagenTypeBackedBySpriggitField(fieldName, fieldValue, spriggitFields))
        {
            return true;
        }

        if (IsDtoMutagenTypeBackedBySpriggitInlineType(fieldName, fieldValue, spriggitFields))
        {
            return true;
        }

        if (IsDtoMutagenTypeBackedByExistingSpriggitObject(fieldName, spriggitFields))
        {
            return true;
        }

        if (IsDtoSingletonObjectBackedBySpriggitObject(fieldName, fieldValue, spriggitFields, "Models", "Model"))
        {
            return true;
        }

        if (IsDtoModelMaterialSwapBackedBySpriggitScalar(fieldName, fieldValue, spriggitFields))
        {
            return true;
        }

        if (IsDtoScriptingAdapterBackedBySpriggitField(fieldName, fieldValue, spriggitFields, dtoFields))
        {
            return true;
        }

        if (IsDtoVirtualMachineAdapterAliasBackedBySpriggitField(fieldName, fieldValue, spriggitFields))
        {
            return true;
        }

        if (IsDtoActorValueInformationFieldBackedBySpriggitField(fieldName, fieldValue, spriggitFields))
        {
            return true;
        }

        if (IsDtoRawPayloadBackedBySpriggitField(fieldName, spriggitFields, dtoFields))
        {
            return true;
        }

        if (IsDtoSoundFieldBackedBySpriggitNamedSound(fieldName, fieldValue, spriggitFields, dtoFields))
        {
            return true;
        }

        return IsSpriggitListBackedDtoScalar(fieldName, spriggitFields, dtoFields, "Flags");
    }

    private static bool IsDtoModelMaterialSwapBackedBySpriggitScalar(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> spriggitFields)
    {
        if (!spriggitFields.TryGetValue("Model.MaterialSwap", out var materialSwap))
        {
            return false;
        }

        if ((string.Equals(fieldName, "Model.MaterialSwaps.Count", StringComparison.OrdinalIgnoreCase) ||
             string.Equals(fieldName, "Models[0].MaterialSwaps.Count", StringComparison.OrdinalIgnoreCase)) &&
            string.Equals(fieldValue, "1", StringComparison.Ordinal))
        {
            return true;
        }

        if ((string.Equals(fieldName, "Model.MaterialSwaps[0]", StringComparison.OrdinalIgnoreCase) ||
             string.Equals(fieldName, "Models[0].MaterialSwaps[0].MaterialSwapFormKey", StringComparison.OrdinalIgnoreCase)) &&
            string.Equals(fieldValue, materialSwap, StringComparison.Ordinal))
        {
            return true;
        }

        if (string.Equals(fieldName, "Models[0].MaterialSwaps[0].MaterialSwapIndex", StringComparison.OrdinalIgnoreCase) &&
            string.Equals(fieldValue, "0", StringComparison.Ordinal))
        {
            return true;
        }

        if (string.Equals(fieldName, "Models[0].MaterialSwaps[0].ModelSlot", StringComparison.OrdinalIgnoreCase) &&
            string.Equals(fieldValue, "Model", StringComparison.Ordinal))
        {
            return true;
        }

        return string.Equals(fieldName, "Models[0].MaterialSwaps[0].ModelGender", StringComparison.OrdinalIgnoreCase) &&
               string.IsNullOrEmpty(fieldValue);
    }

    private static bool IsDtoActorValueInformationFieldBackedBySpriggitField(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> spriggitFields)
    {
        if (string.Equals(fieldName, "Cnam", StringComparison.OrdinalIgnoreCase))
        {
            return spriggitFields.TryGetValue("CNAM", out var value) &&
                   string.Equals(fieldValue, value, StringComparison.Ordinal);
        }

        var scalarFieldName = fieldName switch
        {
            "SkillImproveMult" => "Skill.ImproveMult",
            "SkillImproveOffset" => "Skill.ImproveOffset",
            "SkillUseMult" => "Skill.UseMult",
            _ => string.Empty
        };
        if (!string.IsNullOrEmpty(scalarFieldName))
        {
            return spriggitFields.TryGetValue(scalarFieldName, out var value) &&
                   AreEquivalentSpriggitValues(fieldValue, value);
        }

        if (string.Equals(fieldName, "LayoutEntries.Count", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (fieldName.StartsWith("LayoutEntries[", StringComparison.OrdinalIgnoreCase) &&
            TryGetIndexedPath(fieldName, "LayoutEntries", out var layoutIndex, out var layoutRemainder))
        {
            if (string.Equals(layoutRemainder, ".LayoutIndex", StringComparison.OrdinalIgnoreCase))
            {
                return string.Equals(fieldValue, layoutIndex.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);
            }

            var spriggitFieldName = layoutRemainder switch
            {
                ".AssociatedSkillFormKey" => "PerkTree[" + layoutIndex.ToString(CultureInfo.InvariantCulture) + "].AssociatedSkill",
                ".Fnam" => "PerkTree[" + layoutIndex.ToString(CultureInfo.InvariantCulture) + "].FNAM",
                ".HorizontalPosition" => "PerkTree[" + layoutIndex.ToString(CultureInfo.InvariantCulture) + "].HorizontalPosition",
                ".Index" => "PerkTree[" + layoutIndex.ToString(CultureInfo.InvariantCulture) + "].Index",
                ".PerkGridX" => "PerkTree[" + layoutIndex.ToString(CultureInfo.InvariantCulture) + "].PerkGridX",
                ".PerkGridY" => "PerkTree[" + layoutIndex.ToString(CultureInfo.InvariantCulture) + "].PerkGridY",
                ".VerticalPosition" => "PerkTree[" + layoutIndex.ToString(CultureInfo.InvariantCulture) + "].VerticalPosition",
                _ => string.Empty
            };

            return !string.IsNullOrEmpty(spriggitFieldName) &&
                   spriggitFields.TryGetValue(spriggitFieldName, out var value) &&
                   AreEquivalentSpriggitValues(fieldValue, value);
        }

        if (fieldName.StartsWith("PerkTree[", StringComparison.OrdinalIgnoreCase) &&
            TryGetIndexedPath(fieldName, "PerkTree", out var perkTreeIndex, out var perkTreeRemainder))
        {
            if (string.Equals(perkTreeRemainder, ".PerkTreeIndex", StringComparison.OrdinalIgnoreCase))
            {
                return string.Equals(fieldValue, perkTreeIndex.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);
            }

            if (string.Equals(perkTreeRemainder, ".PerkFormKey", StringComparison.OrdinalIgnoreCase))
            {
                return spriggitFields.TryGetValue("PerkTree[" + perkTreeIndex.ToString(CultureInfo.InvariantCulture) + "].Perk", out var value) &&
                       string.Equals(fieldValue, value, StringComparison.Ordinal);
            }

            if (string.Equals(perkTreeRemainder, ".Fnam", StringComparison.OrdinalIgnoreCase))
            {
                return spriggitFields.TryGetValue("PerkTree[" + perkTreeIndex.ToString(CultureInfo.InvariantCulture) + "].FNAM", out var value) &&
                       string.Equals(fieldValue, value, StringComparison.Ordinal);
            }

            if (perkTreeRemainder.StartsWith(".ConnectionLineToIndices", StringComparison.OrdinalIgnoreCase))
            {
                return IsDtoActorValueInformationConnectionLineBackedBySpriggitField(
                    fieldName,
                    fieldValue,
                    spriggitFields,
                    perkTreeIndex,
                    perkTreeRemainder);
            }
        }

        return false;
    }

    private static bool IsSpriggitActorValueInformationFieldBackedByDtoField(
        string fieldName,
        IReadOnlyDictionary<string, string> spriggitFields,
        IReadOnlyDictionary<string, string> dtoFields)
    {
        if (!fieldName.StartsWith("PerkTree[", StringComparison.OrdinalIgnoreCase) ||
            !TryGetIndexedPath(fieldName, "PerkTree", out var perkTreeIndex, out var perkTreeRemainder))
        {
            return false;
        }

        if (string.IsNullOrEmpty(perkTreeRemainder))
        {
            var index = perkTreeIndex.ToString(CultureInfo.InvariantCulture);
            return dtoFields.ContainsKey("LayoutEntries[" + index + "].Fnam") ||
                   dtoFields.ContainsKey("PerkTree[" + index + "].Fnam");
        }

        var dtoLayoutFieldName = perkTreeRemainder switch
        {
            ".AssociatedSkill" => "LayoutEntries[" + perkTreeIndex.ToString(CultureInfo.InvariantCulture) + "].AssociatedSkillFormKey",
            ".HorizontalPosition" => "LayoutEntries[" + perkTreeIndex.ToString(CultureInfo.InvariantCulture) + "].HorizontalPosition",
            ".Index" => "LayoutEntries[" + perkTreeIndex.ToString(CultureInfo.InvariantCulture) + "].Index",
            ".PerkGridX" => "LayoutEntries[" + perkTreeIndex.ToString(CultureInfo.InvariantCulture) + "].PerkGridX",
            ".PerkGridY" => "LayoutEntries[" + perkTreeIndex.ToString(CultureInfo.InvariantCulture) + "].PerkGridY",
            ".VerticalPosition" => "LayoutEntries[" + perkTreeIndex.ToString(CultureInfo.InvariantCulture) + "].VerticalPosition",
            _ => string.Empty
        };
        if (!string.IsNullOrEmpty(dtoLayoutFieldName))
        {
            return spriggitFields.TryGetValue(fieldName, out var spriggitValue) &&
                   dtoFields.TryGetValue(dtoLayoutFieldName, out var dtoValue) &&
                   AreEquivalentSpriggitValues(dtoValue, spriggitValue);
        }

        if (string.Equals(perkTreeRemainder, ".Perk", StringComparison.OrdinalIgnoreCase))
        {
            return dtoFields.ContainsKey("PerkTree[" + perkTreeIndex.ToString(CultureInfo.InvariantCulture) + "].PerkFormKey");
        }

        if (perkTreeRemainder.StartsWith(".ConnectionLineToIndices", StringComparison.OrdinalIgnoreCase))
        {
            return IsSpriggitActorValueInformationConnectionLineBackedByDtoField(
                fieldName,
                spriggitFields,
                dtoFields,
                perkTreeIndex,
                perkTreeRemainder);
        }

        return false;
    }

    private static bool IsSpriggitActorValueInformationConnectionLineBackedByDtoField(
        string fieldName,
        IReadOnlyDictionary<string, string> spriggitFields,
        IReadOnlyDictionary<string, string> dtoFields,
        int perkTreeIndex,
        string perkTreeRemainder)
    {
        var connectionLineRoot = "PerkTree[" + perkTreeIndex.ToString(CultureInfo.InvariantCulture) + "].ConnectionLineToIndices";
        if (string.Equals(perkTreeRemainder, ".ConnectionLineToIndices.Count", StringComparison.OrdinalIgnoreCase))
        {
            return dtoFields.ContainsKey(connectionLineRoot + ".Count");
        }

        if (!TryGetIndexedPath(fieldName, connectionLineRoot, out var connectionLineIndex, out var connectionLineRemainder) ||
            !string.IsNullOrEmpty(connectionLineRemainder))
        {
            return false;
        }

        return spriggitFields.TryGetValue(fieldName, out var spriggitValue) &&
               dtoFields.TryGetValue(connectionLineRoot + "[" + connectionLineIndex.ToString(CultureInfo.InvariantCulture) + "].TargetIndex", out var dtoValue) &&
               string.Equals(dtoValue, spriggitValue, StringComparison.Ordinal);
    }

    private static bool IsDtoActorValueInformationConnectionLineBackedBySpriggitField(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> spriggitFields,
        int perkTreeIndex,
        string perkTreeRemainder)
    {
        var connectionLineRoot = "PerkTree[" + perkTreeIndex.ToString(CultureInfo.InvariantCulture) + "].ConnectionLineToIndices";
        if (string.Equals(perkTreeRemainder, ".ConnectionLineToIndices.Count", StringComparison.OrdinalIgnoreCase))
        {
            return spriggitFields.ContainsKey(connectionLineRoot + ".Count");
        }

        if (!TryGetIndexedPath(fieldName, connectionLineRoot, out var connectionLineIndex, out var connectionLineRemainder))
        {
            return false;
        }

        if (string.Equals(connectionLineRemainder, ".ConnectionLineIndex", StringComparison.OrdinalIgnoreCase))
        {
            return string.Equals(fieldValue, connectionLineIndex.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);
        }

        if (string.Equals(connectionLineRemainder, ".PerkTreeIndex", StringComparison.OrdinalIgnoreCase))
        {
            return string.Equals(fieldValue, perkTreeIndex.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);
        }

        return string.Equals(connectionLineRemainder, ".TargetIndex", StringComparison.OrdinalIgnoreCase) &&
               spriggitFields.TryGetValue(connectionLineRoot + "[" + connectionLineIndex.ToString(CultureInfo.InvariantCulture) + "]", out var value) &&
               string.Equals(fieldValue, value, StringComparison.Ordinal);
    }

    private static bool IsDtoChildInfrastructureField(string fieldName)
    {
        return fieldName.Contains('[', StringComparison.Ordinal) &&
               (fieldName.EndsWith(".FormKey", StringComparison.OrdinalIgnoreCase) ||
                fieldName.EndsWith(".Game", StringComparison.OrdinalIgnoreCase) ||
                fieldName.EndsWith(".ImportedAtUTC", StringComparison.OrdinalIgnoreCase) ||
                fieldName.EndsWith(".ModKey", StringComparison.OrdinalIgnoreCase) ||
                fieldName.EndsWith(".RecordType", StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsCommonMetadataFieldOutsideRepositoryReadback(string fieldName, IReadOnlyDictionary<string, string> dtoFields)
    {
        return (string.Equals(fieldName, "Version2", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(fieldName, "VersionControl", StringComparison.OrdinalIgnoreCase)) &&
               !dtoFields.ContainsKey(fieldName);
    }

    private static bool IsSpriggitLocalizedFieldBackedByDtoFallback(
        string fieldName,
        IReadOnlyDictionary<string, string> spriggitFields,
        IReadOnlyDictionary<string, string> dtoFields)
    {
        if (!TryGetLocalizedFieldRoot(fieldName, out var spriggitRootFieldName))
        {
            return false;
        }

        var dtoRootFieldName = string.Equals(spriggitRootFieldName, "BookText", StringComparison.OrdinalIgnoreCase)
            ? "Text"
            : spriggitRootFieldName;

        return HasLocalizedFallbackMatch(spriggitRootFieldName, dtoRootFieldName, spriggitFields, dtoFields);
    }

    private static bool IsDtoLocalizedFallbackBackedBySpriggitField(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> spriggitFields,
        IReadOnlyDictionary<string, string> dtoFields)
    {
        _ = fieldValue;
        if (!TryGetLocalizedFieldRoot(fieldName, out var dtoRootFieldName))
        {
            return false;
        }

        var spriggitRootFieldName = string.Equals(dtoRootFieldName, "Text", StringComparison.OrdinalIgnoreCase) &&
                                    HasSpriggitPath(spriggitFields, "BookText")
            ? "BookText"
            : dtoRootFieldName;

        return HasLocalizedFallbackMatch(spriggitRootFieldName, dtoRootFieldName, spriggitFields, dtoFields);
    }

    private static bool HasLocalizedFallbackMatch(
        string spriggitRootFieldName,
        string dtoRootFieldName,
        IReadOnlyDictionary<string, string> spriggitFields,
        IReadOnlyDictionary<string, string> dtoFields)
    {
        if (!dtoFields.TryGetValue(dtoRootFieldName + ".Count", out var dtoCount) ||
            !string.Equals(dtoCount, "1", StringComparison.Ordinal) ||
            !dtoFields.TryGetValue(dtoRootFieldName + "[0].Language", out var dtoLanguage) ||
            !dtoFields.TryGetValue(dtoRootFieldName + "[0].String", out var dtoString) ||
            !spriggitFields.TryGetValue(spriggitRootFieldName + ".Count", out var spriggitCountValue) ||
            !int.TryParse(spriggitCountValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var spriggitCount) ||
            spriggitCount <= 1)
        {
            return false;
        }

        for (var entryIndex = 0; entryIndex < spriggitCount; entryIndex++)
        {
            var spriggitEntryPath = spriggitRootFieldName + "[" + entryIndex.ToString(CultureInfo.InvariantCulture) + "]";
            if (spriggitFields.TryGetValue(spriggitEntryPath + ".Language", out var spriggitLanguage) &&
                spriggitFields.TryGetValue(spriggitEntryPath + ".String", out var spriggitString) &&
                string.Equals(spriggitLanguage, dtoLanguage, StringComparison.Ordinal) &&
                string.Equals(NormalizeLocalizedText(spriggitString), NormalizeLocalizedText(dtoString), StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryGetLocalizedFieldRoot(string fieldName, out string rootFieldName)
    {
        rootFieldName = string.Empty;

        if (fieldName.EndsWith(".Count", StringComparison.OrdinalIgnoreCase))
        {
            rootFieldName = fieldName[..^".Count".Length];
            return true;
        }

        if (fieldName.EndsWith(".TargetLanguage", StringComparison.OrdinalIgnoreCase))
        {
            rootFieldName = fieldName[..^".TargetLanguage".Length];
            return true;
        }

        var bracketIndex = fieldName.IndexOf('[');
        if (bracketIndex <= 0 || !fieldName.EndsWith(".Language", StringComparison.OrdinalIgnoreCase) && !fieldName.EndsWith(".String", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        rootFieldName = fieldName[..bracketIndex];
        return true;
    }

    private static string NormalizeLocalizedText(string text)
    {
        return text
            .Replace("\\r\\n", "\r\n", StringComparison.Ordinal)
            .Replace("\\\"", "\"", StringComparison.Ordinal);
    }

    private static bool IsSpriggitObjectListItemMutagenType(
        string fieldName,
        IReadOnlyDictionary<string, string> spriggitFields,
        IReadOnlyDictionary<string, string> dtoFields,
        string rootFieldName)
    {
        if (!fieldName.StartsWith(rootFieldName + "[", StringComparison.OrdinalIgnoreCase) ||
            fieldName.Contains('.', StringComparison.Ordinal) ||
            !spriggitFields.TryGetValue(fieldName, out var fieldValue) ||
            !fieldValue.StartsWith("MutagenObjectType:", StringComparison.Ordinal) ||
            !TryGetListIndex(fieldName, rootFieldName, out var componentIndex))
        {
            return false;
        }

        var spriggitTypeName = fieldValue["MutagenObjectType:".Length..].Trim();
        var dtoFieldName = fieldName + ".MutagenObjectType";
        if (dtoFields.TryGetValue(dtoFieldName, out var dtoFieldValue) &&
            IsSameTypeName(spriggitTypeName, dtoFieldValue))
        {
            return true;
        }

        if (IsComponentRawPayloadTypeMatch(dtoFields, componentIndex, spriggitTypeName))
        {
            return true;
        }

        return dtoFields.TryGetValue(rootFieldName + ".Count", out var countValue) &&
               int.TryParse(countValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var count) &&
               componentIndex < count;
    }

    private static bool IsDtoComponentMutagenTypeBackedBySpriggitField(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> spriggitFields)
    {
        if (!fieldName.StartsWith("Components[", StringComparison.OrdinalIgnoreCase) ||
            !fieldName.EndsWith("].MutagenObjectType", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var componentPath = fieldName[..^".MutagenObjectType".Length];
        if (!spriggitFields.TryGetValue(componentPath, out var spriggitValue) ||
            !spriggitValue.StartsWith("MutagenObjectType:", StringComparison.Ordinal))
        {
            return false;
        }

        var spriggitTypeName = spriggitValue["MutagenObjectType:".Length..].Trim();
        return IsSameTypeName(spriggitTypeName, fieldValue) ||
               fieldValue.StartsWith(spriggitTypeName, StringComparison.Ordinal);
    }

    private static bool IsDtoMutagenTypeBackedBySpriggitInlineType(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> spriggitFields)
    {
        if (!fieldName.EndsWith(".MutagenObjectType", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var spriggitInlinePath = fieldName[..^".MutagenObjectType".Length];
        if (!spriggitFields.TryGetValue(spriggitInlinePath, out var spriggitValue) ||
            !spriggitValue.StartsWith("MutagenObjectType:", StringComparison.Ordinal))
        {
            return false;
        }

        var spriggitTypeName = spriggitValue["MutagenObjectType:".Length..].Trim();
        return IsSameTypeName(spriggitTypeName, fieldValue) ||
               fieldValue.StartsWith(spriggitTypeName, StringComparison.Ordinal);
    }

    private static bool IsDtoMutagenTypeBackedByExistingSpriggitObject(
        string fieldName,
        IReadOnlyDictionary<string, string> spriggitFields)
    {
        if ((!fieldName.StartsWith("VirtualMachineAdapter[", StringComparison.OrdinalIgnoreCase) &&
             !fieldName.StartsWith("VirtualMachineAdapter.Scripts[", StringComparison.OrdinalIgnoreCase)) ||
             !fieldName.EndsWith(".MutagenObjectType", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var spriggitObjectPath = fieldName[..^".MutagenObjectType".Length];
        return HasSpriggitPath(spriggitFields, spriggitObjectPath);
    }

    private static bool IsComponentRawPayloadTypeMatch(IReadOnlyDictionary<string, string> dtoFields, int componentIndex, string spriggitTypeName)
    {
        return dtoFields
            .Where(field => field.Key.StartsWith("RawPayloads[", StringComparison.OrdinalIgnoreCase) &&
                            field.Key.EndsWith("].PayloadType", StringComparison.OrdinalIgnoreCase))
            .Any(field =>
            {
                var payloadPath = field.Key[..^".PayloadType".Length];
                return dtoFields.TryGetValue(payloadPath + ".PayloadIndex", out var payloadIndexValue) &&
                       int.TryParse(payloadIndexValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var payloadIndex) &&
                       payloadIndex == componentIndex &&
                       dtoFields.TryGetValue(payloadPath + ".SourcePath", out var sourcePath) &&
                       sourcePath.StartsWith("Components.", StringComparison.OrdinalIgnoreCase) &&
                       (IsSameTypeName(spriggitTypeName, field.Value) ||
                        sourcePath.Contains("." + spriggitTypeName + ".", StringComparison.Ordinal));
            });
    }

    private static bool IsSpriggitComponentRawPayloadBackedByDtoRawPayload(
        string fieldName,
        IReadOnlyDictionary<string, string> spriggitFields,
        IReadOnlyDictionary<string, string> dtoFields)
    {
        if (!TryGetIndexedPath(fieldName, "Components", out var componentIndex, out var componentRemainder) ||
            !componentRemainder.StartsWith(".", StringComparison.Ordinal) ||
            !spriggitFields.TryGetValue("Components[" + componentIndex.ToString(CultureInfo.InvariantCulture) + "]", out var componentTypeValue) ||
            !componentTypeValue.StartsWith("MutagenObjectType:", StringComparison.Ordinal))
        {
            return false;
        }

        var componentFieldName = componentRemainder[1..];
        if (!string.Equals(componentFieldName, "REFL", StringComparison.Ordinal))
        {
            return false;
        }

        return dtoFields
            .Where(field => field.Key.StartsWith("RawPayloads[", StringComparison.OrdinalIgnoreCase) &&
                            field.Key.EndsWith("].PayloadType", StringComparison.OrdinalIgnoreCase))
            .Any(field =>
            {
                var payloadPath = field.Key[..^".PayloadType".Length];
                return dtoFields.TryGetValue(payloadPath + ".PayloadIndex", out var payloadIndexValue) &&
                       int.TryParse(payloadIndexValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var payloadIndex) &&
                       payloadIndex == componentIndex &&
                       dtoFields.TryGetValue(payloadPath + ".SourcePath", out var sourcePath) &&
                       sourcePath.StartsWith("Components.", StringComparison.Ordinal) &&
                       sourcePath.EndsWith("." + componentFieldName, StringComparison.Ordinal);
            });
    }

    private static bool IsSameTypeName(string spriggitTypeName, string dtoTypeName)
    {
        return string.Equals(spriggitTypeName, dtoTypeName, StringComparison.Ordinal) ||
               dtoTypeName.EndsWith("." + spriggitTypeName, StringComparison.Ordinal);
    }

    private static bool IsMissingDefaultDtoField(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> spriggitFields)
    {
        if (HasSpriggitPath(spriggitFields, fieldName))
        {
            return false;
        }

        return (fieldName is "Value" or "Weight" && IsZero(fieldValue)) ||
               string.Equals(fieldName, "FormVersion", StringComparison.OrdinalIgnoreCase) ||
               ((string.Equals(fieldName, "ObjectBoundsFirst", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(fieldName, "ObjectBoundsSecond", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(fieldName, "ObjectBounds.First", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(fieldName, "ObjectBounds.Second", StringComparison.OrdinalIgnoreCase)) &&
                string.Equals(fieldValue, "0, 0, 0", StringComparison.Ordinal)) ||
               (string.Equals(fieldName, "Flags", StringComparison.OrdinalIgnoreCase) && IsZero(fieldValue)) ||
               (string.Equals(fieldName, "Teaches.RawContent", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(fieldValue, uint.MaxValue.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal)) ||
               (string.Equals(fieldName, "DataSlateType", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(fieldValue, "None", StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsMissingDefaultActorValueInformationSkillOffset(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> spriggitFields)
    {
        return (string.Equals(fieldName, "SkillImproveOffset", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(fieldName, "Skill.ImproveOffset", StringComparison.OrdinalIgnoreCase)) &&
               IsZero(fieldValue) &&
               !spriggitFields.ContainsKey("Skill.ImproveOffset");
    }

    private static bool IsAlternativeRootFieldBackedByDtoField(
        string fieldName,
        IReadOnlyDictionary<string, string> dtoFields,
        string spriggitRootFieldName,
        string dtoRootFieldName)
    {
        return TryMapRootField(fieldName, spriggitRootFieldName, dtoRootFieldName, out var dtoFieldName) &&
               dtoFields.ContainsKey(dtoFieldName);
    }

    private static bool IsDtoFieldBackedByAlternativeRootField(
        string fieldName,
        IReadOnlyDictionary<string, string> spriggitFields,
        string dtoRootFieldName,
        string spriggitRootFieldName)
    {
        return TryMapRootField(fieldName, dtoRootFieldName, spriggitRootFieldName, out var spriggitFieldName) &&
               spriggitFields.ContainsKey(spriggitFieldName);
    }

    private static bool TryMapRootField(string fieldName, string fromRootFieldName, string toRootFieldName, out string mappedFieldName)
    {
        if (fieldName.StartsWith(fromRootFieldName + ".", StringComparison.OrdinalIgnoreCase) ||
            fieldName.StartsWith(fromRootFieldName + "[", StringComparison.OrdinalIgnoreCase))
        {
            mappedFieldName = toRootFieldName + fieldName[fromRootFieldName.Length..];
            return true;
        }

        mappedFieldName = string.Empty;
        return false;
    }

    private static bool IsDtoIndexedPropertyBackedBySpriggitScalarList(
        string fieldName,
        IReadOnlyDictionary<string, string> spriggitFields,
        string rootFieldName,
        string dtoPropertyName)
    {
        if (!fieldName.StartsWith(rootFieldName + "[", StringComparison.OrdinalIgnoreCase) ||
            !fieldName.EndsWith("]." + dtoPropertyName, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var indexStart = rootFieldName.Length + 1;
        var indexEnd = fieldName.IndexOf(']', indexStart);
        return indexEnd > indexStart &&
               HasSpriggitPath(spriggitFields, rootFieldName + fieldName[rootFieldName.Length..(indexEnd + 1)]);
    }

    private static bool IsDtoIndexedMetadataBackedBySpriggitScalarList(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> spriggitFields,
        string rootFieldName,
        string metadataFieldName)
    {
        if (!fieldName.StartsWith(rootFieldName + "[", StringComparison.OrdinalIgnoreCase) ||
            !fieldName.EndsWith("]." + metadataFieldName, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var indexStart = rootFieldName.Length + 1;
        var indexEnd = fieldName.IndexOf(']', indexStart);
        return indexEnd > indexStart &&
               int.TryParse(fieldName[indexStart..indexEnd], NumberStyles.Integer, CultureInfo.InvariantCulture, out var index) &&
               int.TryParse(fieldValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var metadataIndex) &&
               index == metadataIndex &&
               HasSpriggitPath(spriggitFields, rootFieldName + fieldName[rootFieldName.Length..(indexEnd + 1)]);
    }

    private static bool IsDtoSingletonObjectBackedBySpriggitObject(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> spriggitFields,
        string dtoRootFieldName,
        string spriggitRootFieldName)
    {
        if (!HasSpriggitPath(spriggitFields, spriggitRootFieldName))
        {
            return false;
        }

        if (string.Equals(fieldName, dtoRootFieldName + ".Count", StringComparison.OrdinalIgnoreCase))
        {
            return string.Equals(fieldValue, "1", StringComparison.Ordinal);
        }

        return string.Equals(fieldName, dtoRootFieldName + "[0].ModelGender", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(fieldName, dtoRootFieldName + "[0].ModelSlot", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsDtoSoundFieldBackedBySpriggitNamedSound(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> spriggitFields,
        IReadOnlyDictionary<string, string> dtoFields)
    {
        if (!fieldName.StartsWith("Sounds", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (string.Equals(fieldName, "Sounds.Count", StringComparison.OrdinalIgnoreCase))
        {
            return int.TryParse(fieldValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var count) &&
                   count == dtoFields.Keys
                       .Where(field => field.StartsWith("Sounds[", StringComparison.OrdinalIgnoreCase) &&
                                       field.EndsWith(".SoundSlot", StringComparison.OrdinalIgnoreCase))
                       .Select(field => dtoFields[field])
                       .Count(soundSlot => HasSpriggitPath(spriggitFields, soundSlot));
        }

        var soundPathEnd = fieldName.IndexOf(']');
        if (soundPathEnd < 0)
        {
            return false;
        }

        var soundPath = fieldName[..(soundPathEnd + 1)];
        if (!dtoFields.TryGetValue(soundPath + ".SoundSlot", out var soundSlot) ||
            !HasSpriggitPath(spriggitFields, soundSlot))
        {
            return false;
        }

        if (fieldName.EndsWith(".SoundIndex", StringComparison.OrdinalIgnoreCase) ||
            fieldName.EndsWith(".SoundSlot", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var fieldSuffix = fieldName[(soundPath.Length + 1)..];
        if (spriggitFields.ContainsKey(soundSlot + "." + fieldSuffix))
        {
            return true;
        }

        return string.Equals(fieldSuffix, "Start", StringComparison.OrdinalIgnoreCase) &&
               spriggitFields.TryGetValue(soundSlot, out var soundValue) &&
               string.Equals(fieldValue, soundValue, StringComparison.Ordinal);
    }

    private static bool IsDtoScriptingAdapterBackedBySpriggitField(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> spriggitFields,
        IReadOnlyDictionary<string, string> dtoFields)
    {
        if (string.Equals(fieldName, "ScriptingAdapters.Count", StringComparison.OrdinalIgnoreCase))
        {
            return TryGetScriptingAdapterCount(spriggitFields, out var scriptCount)
                ? string.Equals(fieldValue, scriptCount, StringComparison.Ordinal)
                : IsZero(fieldValue);
        }

        if (!fieldName.StartsWith("ScriptingAdapters[", StringComparison.OrdinalIgnoreCase) ||
            !TryGetIndexedPath(fieldName, "ScriptingAdapters", out var scriptIndex, out var scriptRemainder))
        {
            return false;
        }

        var spriggitScriptPath = GetScriptingAdapterScriptPath(spriggitFields, scriptIndex);
        if (string.Equals(scriptRemainder, ".ScriptIndex", StringComparison.OrdinalIgnoreCase))
        {
            return string.Equals(fieldValue, scriptIndex.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);
        }

        if (string.Equals(scriptRemainder, ".Name", StringComparison.OrdinalIgnoreCase))
        {
            return spriggitFields.TryGetValue(spriggitScriptPath + ".Name", out var scriptName) &&
                   string.Equals(fieldValue, scriptName, StringComparison.Ordinal);
        }

        if (string.Equals(scriptRemainder, ".Properties.Count", StringComparison.OrdinalIgnoreCase))
        {
            return TryGetScriptingAdapterPropertyCount(spriggitFields, spriggitScriptPath, out var propertyCount) &&
                   string.Equals(fieldValue, propertyCount, StringComparison.Ordinal);
        }

        const string propertiesPrefix = ".Properties";
        if (!scriptRemainder.StartsWith(propertiesPrefix + "[", StringComparison.OrdinalIgnoreCase) ||
            !TryGetIndexedPath(scriptRemainder, propertiesPrefix, out var propertyIndex, out var propertyRemainder))
        {
            return false;
        }

        var spriggitPropertyPath = GetScriptingAdapterPropertyPath(spriggitFields, spriggitScriptPath, propertyIndex);
        if (IsScriptingAdapterPropertyInfrastructureField(propertyRemainder, fieldValue, propertyIndex, spriggitScriptPath, spriggitFields))
        {
            return true;
        }

        if (string.Equals(propertyRemainder, ".ListItems.Count", StringComparison.OrdinalIgnoreCase))
        {
            return TryGetScriptingAdapterListItemCount(spriggitFields, spriggitPropertyPath, out var listItemCount)
                ? string.Equals(fieldValue, listItemCount, StringComparison.Ordinal)
                : IsZero(fieldValue);
        }

        const string listItemsPrefix = ".ListItems";
        if (propertyRemainder.StartsWith(listItemsPrefix + "[", StringComparison.OrdinalIgnoreCase) &&
            TryGetIndexedPath(propertyRemainder, listItemsPrefix, out var listItemIndex, out var listItemRemainder))
        {
            var spriggitListItemPath = GetScriptingAdapterListItemPath(spriggitFields, spriggitPropertyPath, listItemIndex);
            if (IsScriptingAdapterListItemInfrastructureField(listItemRemainder, fieldValue, propertyIndex, listItemIndex))
            {
                return true;
            }

            return IsDtoScriptingAdapterLeafBackedBySpriggitField(listItemRemainder, fieldValue, spriggitListItemPath, spriggitFields);
        }

        return IsDtoScriptingAdapterLeafBackedBySpriggitField(propertyRemainder, fieldValue, spriggitPropertyPath, spriggitFields);
    }

    private static bool IsDtoVirtualMachineAdapterAliasBackedBySpriggitField(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> spriggitFields)
    {
        if (string.Equals(fieldName, "VirtualMachineAdapter.Count", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(fieldName, "VirtualMachineAdapter.Scripts.Count", StringComparison.OrdinalIgnoreCase))
        {
            return TryGetScriptingAdapterCount(spriggitFields, out var scriptCount) &&
                   string.Equals(fieldValue, scriptCount, StringComparison.Ordinal);
        }

        var rootFieldName = fieldName.StartsWith("VirtualMachineAdapter.Scripts[", StringComparison.OrdinalIgnoreCase)
            ? "VirtualMachineAdapter.Scripts"
            : "VirtualMachineAdapter";
        if (!fieldName.StartsWith(rootFieldName + "[", StringComparison.OrdinalIgnoreCase) ||
            !TryGetIndexedPath(fieldName, rootFieldName, out var scriptIndex, out var scriptRemainder))
        {
            return false;
        }

        var spriggitScriptPath = GetScriptingAdapterScriptPath(spriggitFields, scriptIndex);
        if (string.Equals(scriptRemainder, ".Name", StringComparison.OrdinalIgnoreCase))
        {
            return spriggitFields.TryGetValue(spriggitScriptPath + ".Name", out var scriptName) &&
                   string.Equals(fieldValue, scriptName, StringComparison.Ordinal);
        }

        if (string.Equals(scriptRemainder, ".Count", StringComparison.OrdinalIgnoreCase))
        {
            return TryGetScriptingAdapterPropertyCount(spriggitFields, spriggitScriptPath, out var propertyCount) &&
                   string.Equals(fieldValue, propertyCount, StringComparison.Ordinal);
        }

        int propertyIndex;
        string propertyRemainder;
        if (scriptRemainder.StartsWith(".Properties[", StringComparison.OrdinalIgnoreCase))
        {
            if (!TryGetIndexedPath(scriptRemainder, ".Properties", out propertyIndex, out propertyRemainder))
            {
                return false;
            }
        }
        else if (!TryGetIndexedPath(scriptRemainder, string.Empty, out propertyIndex, out propertyRemainder))
        {
            return false;
        }

        var spriggitPropertyPath = GetScriptingAdapterPropertyPath(spriggitFields, spriggitScriptPath, propertyIndex);
        if (string.Equals(propertyRemainder, ".Count", StringComparison.OrdinalIgnoreCase))
        {
            return TryGetScriptingAdapterListItemCount(spriggitFields, spriggitPropertyPath, out var listItemCount) &&
                   string.Equals(fieldValue, listItemCount, StringComparison.Ordinal);
        }

        if (propertyRemainder.StartsWith(".Objects[", StringComparison.OrdinalIgnoreCase) &&
            TryGetIndexedPath(propertyRemainder, ".Objects", out var objectIndex, out var objectRemainder))
        {
            var spriggitListItemPath = GetScriptingAdapterListItemPath(spriggitFields, spriggitPropertyPath, objectIndex);
            return IsDtoScriptingAdapterLeafBackedBySpriggitField(objectRemainder, fieldValue, spriggitListItemPath, spriggitFields);
        }

        if (TryGetIndexedPath(propertyRemainder, string.Empty, out var listItemIndex, out var listItemRemainder))
        {
            var spriggitListItemPath = GetScriptingAdapterListItemPath(spriggitFields, spriggitPropertyPath, listItemIndex);
            return IsDtoScriptingAdapterLeafBackedBySpriggitField(listItemRemainder, fieldValue, spriggitListItemPath, spriggitFields);
        }

        return IsDtoScriptingAdapterLeafBackedBySpriggitField(propertyRemainder, fieldValue, spriggitPropertyPath, spriggitFields);
    }

    private static bool TryGetScriptingAdapterCount(IReadOnlyDictionary<string, string> spriggitFields, out string scriptCount)
    {
        if (spriggitFields.TryGetValue("VirtualMachineAdapter.Scripts.Count", out var nestedScriptCount))
        {
            scriptCount = nestedScriptCount;
            return true;
        }

        if (spriggitFields.TryGetValue("VirtualMachineAdapter.Count", out var rootScriptCount))
        {
            scriptCount = rootScriptCount;
            return true;
        }

        var nestedCount = GetTopLevelIndexedPathCount(spriggitFields, "VirtualMachineAdapter.Scripts");
        if (nestedCount > 0)
        {
            scriptCount = nestedCount.ToString(CultureInfo.InvariantCulture);
            return true;
        }

        var rootCount = GetTopLevelIndexedPathCount(spriggitFields, "VirtualMachineAdapter");
        if (rootCount > 0)
        {
            scriptCount = rootCount.ToString(CultureInfo.InvariantCulture);
            return true;
        }

        scriptCount = string.Empty;
        return false;
    }

    private static string GetScriptingAdapterScriptPath(IReadOnlyDictionary<string, string> spriggitFields, int scriptIndex)
    {
        var nestedPath = "VirtualMachineAdapter.Scripts[" + scriptIndex.ToString(CultureInfo.InvariantCulture) + "]";
        return HasSpriggitPath(spriggitFields, nestedPath)
            ? nestedPath
            : "VirtualMachineAdapter[" + scriptIndex.ToString(CultureInfo.InvariantCulture) + "]";
    }

    private static bool TryGetScriptingAdapterPropertyCount(
        IReadOnlyDictionary<string, string> spriggitFields,
        string spriggitScriptPath,
        out string propertyCount)
    {
        if (spriggitFields.TryGetValue(spriggitScriptPath + ".Properties.Count", out var nestedPropertyCount))
        {
            propertyCount = nestedPropertyCount;
            return true;
        }

        if (spriggitFields.TryGetValue(spriggitScriptPath + ".Count", out var rootPropertyCount))
        {
            propertyCount = rootPropertyCount;
            return true;
        }

        propertyCount = string.Empty;
        return false;
    }

    private static string GetScriptingAdapterPropertyPath(
        IReadOnlyDictionary<string, string> spriggitFields,
        string spriggitScriptPath,
        int propertyIndex)
    {
        var nestedPath = spriggitScriptPath + ".Properties[" + propertyIndex.ToString(CultureInfo.InvariantCulture) + "]";
        return HasSpriggitPath(spriggitFields, nestedPath)
            ? nestedPath
            : spriggitScriptPath + "[" + propertyIndex.ToString(CultureInfo.InvariantCulture) + "]";
    }

    private static string GetScriptingAdapterListItemPath(
        IReadOnlyDictionary<string, string> spriggitFields,
        string spriggitPropertyPath,
        int listItemIndex)
    {
        var objectPath = spriggitPropertyPath + ".Objects[" + listItemIndex.ToString(CultureInfo.InvariantCulture) + "]";
        return HasSpriggitPath(spriggitFields, objectPath)
            ? objectPath
            : spriggitPropertyPath + "[" + listItemIndex.ToString(CultureInfo.InvariantCulture) + "]";
    }

    private static bool TryGetScriptingAdapterListItemCount(
        IReadOnlyDictionary<string, string> spriggitFields,
        string spriggitPropertyPath,
        out string listItemCount)
    {
        if (spriggitFields.TryGetValue(spriggitPropertyPath + ".Objects.Count", out var objectListItemCount))
        {
            listItemCount = objectListItemCount;
            return true;
        }

        if (spriggitFields.TryGetValue(spriggitPropertyPath + ".Count", out var directListItemCount))
        {
            listItemCount = directListItemCount;
            return true;
        }

        listItemCount = string.Empty;
        return false;
    }

    private static bool IsScriptingAdapterPropertyInfrastructureField(
        string propertyRemainder,
        string fieldValue,
        int propertyIndex,
        string spriggitScriptPath,
        IReadOnlyDictionary<string, string> spriggitFields)
    {
        if (string.Equals(propertyRemainder, ".PropertyIndex", StringComparison.OrdinalIgnoreCase))
        {
            return string.Equals(fieldValue, propertyIndex.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);
        }

        if (string.Equals(propertyRemainder, ".ScriptingAdapterName", StringComparison.OrdinalIgnoreCase))
        {
            return spriggitFields.TryGetValue(spriggitScriptPath + ".Name", out var scriptName) &&
                   string.Equals(fieldValue, scriptName, StringComparison.Ordinal);
        }

        return (string.Equals(propertyRemainder, ".ObjectAlias", StringComparison.OrdinalIgnoreCase) && string.Equals(fieldValue, "-1", StringComparison.Ordinal)) ||
               (string.Equals(propertyRemainder, ".ObjectUnused", StringComparison.OrdinalIgnoreCase) && IsZero(fieldValue));
    }

    private static bool IsScriptingAdapterListItemInfrastructureField(
        string listItemRemainder,
        string fieldValue,
        int propertyIndex,
        int listItemIndex)
    {
        return (string.Equals(listItemRemainder, ".PropertyIndex", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(fieldValue, propertyIndex.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal)) ||
               (string.Equals(listItemRemainder, ".ListItemIndex", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(fieldValue, listItemIndex.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal)) ||
               string.Equals(listItemRemainder, ".ScriptingAdapterName", StringComparison.OrdinalIgnoreCase) ||
               (string.Equals(listItemRemainder, ".ObjectAlias", StringComparison.OrdinalIgnoreCase) && string.Equals(fieldValue, "-1", StringComparison.Ordinal)) ||
               (string.Equals(listItemRemainder, ".ObjectUnused", StringComparison.OrdinalIgnoreCase) && IsZero(fieldValue));
    }

    private static bool IsDtoScriptingAdapterLeafBackedBySpriggitField(
        string fieldRemainder,
        string fieldValue,
        string spriggitPath,
        IReadOnlyDictionary<string, string> spriggitFields)
    {
        var spriggitFieldName = fieldRemainder switch
        {
            ".ObjectFormKey" => spriggitPath + ".Object",
            ".DataBool" or ".DataFloat" or ".DataInt" or ".DataString" => spriggitPath + ".Data",
            _ => spriggitPath + fieldRemainder
        };

        if (spriggitFields.TryGetValue(spriggitFieldName, out var spriggitValue) &&
            string.Equals(fieldValue, spriggitValue, StringComparison.Ordinal))
        {
            return true;
        }

        if (string.Equals(fieldRemainder, ".MutagenObjectType", StringComparison.OrdinalIgnoreCase) &&
            HasSpriggitPath(spriggitFields, spriggitPath))
        {
            return true;
        }

        return string.Equals(fieldRemainder, ".MutagenObjectType", StringComparison.OrdinalIgnoreCase) &&
               IsDtoMutagenTypeBackedBySpriggitInlineType(spriggitPath + ".MutagenObjectType", fieldValue, spriggitFields);
    }

    private static bool IsDtoRawPayloadBackedBySpriggitField(
        string fieldName,
        IReadOnlyDictionary<string, string> spriggitFields,
        IReadOnlyDictionary<string, string> dtoFields)
    {
        if (string.Equals(fieldName, "REFL", StringComparison.OrdinalIgnoreCase))
        {
            return HasSpriggitRawPayloadField(spriggitFields, fieldName);
        }

        if (string.Equals(fieldName, "RawPayloads.Count", StringComparison.OrdinalIgnoreCase))
        {
            return dtoFields.Keys.Any(field => field.StartsWith("RawPayloads[", StringComparison.OrdinalIgnoreCase) &&
                                               IsDtoRawPayloadBackedBySpriggitField(field, spriggitFields, dtoFields));
        }

        if (!fieldName.StartsWith("RawPayloads[", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var payloadPathEnd = fieldName.IndexOf(']');
        if (payloadPathEnd < 0)
        {
            return false;
        }

        var payloadPath = fieldName[..(payloadPathEnd + 1)];
        var spriggitFieldName = GetSpriggitRawPayloadFieldName(dtoFields, payloadPath);
        if (string.IsNullOrWhiteSpace(spriggitFieldName) ||
            !HasSpriggitRawPayloadField(spriggitFields, spriggitFieldName))
        {
            return false;
        }

        return fieldName.EndsWith(".PayloadIndex", StringComparison.OrdinalIgnoreCase) ||
               fieldName.EndsWith(".PayloadSlot", StringComparison.OrdinalIgnoreCase) ||
               fieldName.EndsWith(".PayloadType", StringComparison.OrdinalIgnoreCase) ||
               fieldName.EndsWith(".PayloadValue", StringComparison.OrdinalIgnoreCase) ||
               fieldName.EndsWith(".SourcePath", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsSpriggitInlineFormKeyListItemBackedByDtoScalar(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> dtoFields,
        string rootFieldName)
    {
        if (!TryGetIndexedPath(fieldName, rootFieldName, out var index, out var remainder) ||
            !remainder.StartsWith(".", StringComparison.Ordinal) ||
            remainder.Length == 1)
        {
            return false;
        }

        var formKeyId = remainder[1..];
        if (formKeyId.Contains('.', StringComparison.Ordinal) ||
            formKeyId.Contains('[', StringComparison.Ordinal) ||
            formKeyId.Contains(']', StringComparison.Ordinal))
        {
            return false;
        }

        var formKey = formKeyId + ":" + fieldValue;
        var dtoPath = rootFieldName + "[" + index.ToString(CultureInfo.InvariantCulture) + "]";
        return (dtoFields.TryGetValue(dtoPath, out var dtoValue) &&
                string.Equals(formKey, dtoValue, StringComparison.Ordinal)) ||
               (dtoFields.TryGetValue(dtoPath + ".KeywordFormKey", out var dtoKeywordValue) &&
                string.Equals(formKey, dtoKeywordValue, StringComparison.Ordinal));
    }

    private static bool IsSpriggitListBackedDtoScalar(
        string fieldName,
        IReadOnlyDictionary<string, string> spriggitFields,
        IReadOnlyDictionary<string, string> dtoFields,
        string scalarFieldName)
    {
        if (!dtoFields.ContainsKey(scalarFieldName) || GetListValues(spriggitFields, scalarFieldName).Count == 0)
        {
            return false;
        }

        return string.Equals(fieldName, scalarFieldName, StringComparison.OrdinalIgnoreCase) ||
               string.Equals(fieldName, scalarFieldName + ".Count", StringComparison.OrdinalIgnoreCase) ||
               TryGetListIndex(fieldName, scalarFieldName, out _);
    }

    private static bool IsMissingZeroCount(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> spriggitFields)
    {
        if (!fieldName.EndsWith(".Count", StringComparison.OrdinalIgnoreCase) || !IsZero(fieldValue))
        {
            return false;
        }

        var rootFieldName = fieldName[..^".Count".Length];
        return !spriggitFields.ContainsKey(fieldName);
    }

    private static bool IsEmptySpriggitTranslationTargetLanguage(
        string fieldName,
        IReadOnlyDictionary<string, string> spriggitFields,
        IReadOnlyDictionary<string, string> dtoFields)
    {
        const string targetLanguageSuffix = ".TargetLanguage";
        if (!fieldName.EndsWith(targetLanguageSuffix, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var rootFieldName = fieldName[..^targetLanguageSuffix.Length];
        return !HasSpriggitPath(dtoFields, rootFieldName) &&
               !spriggitFields.ContainsKey(rootFieldName + ".Count") &&
               !spriggitFields.Keys.Any(field => field.StartsWith(rootFieldName + "[", StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsEmptySpriggitVirtualMachineAdapterListItemName(
        string fieldName,
        IReadOnlyDictionary<string, string> spriggitFields)
    {
        return (fieldName.StartsWith("VirtualMachineAdapter[", StringComparison.OrdinalIgnoreCase) ||
                fieldName.StartsWith("VirtualMachineAdapter.Scripts[", StringComparison.OrdinalIgnoreCase)) &&
               (fieldName.Count(character => character == '[') == 3 ||
                fieldName.Contains(".Objects[", StringComparison.OrdinalIgnoreCase)) &&
               fieldName.EndsWith("].Name", StringComparison.OrdinalIgnoreCase) &&
               spriggitFields.TryGetValue(fieldName, out var value) &&
               string.IsNullOrEmpty(value);
    }

    private static bool HasSpriggitPath(IReadOnlyDictionary<string, string> spriggitFields, string fieldName)
    {
        return spriggitFields.Keys.Any(field =>
            string.Equals(field, fieldName, StringComparison.OrdinalIgnoreCase) ||
            field.StartsWith(fieldName + ".", StringComparison.OrdinalIgnoreCase) ||
            field.StartsWith(fieldName + "[", StringComparison.OrdinalIgnoreCase));
    }

    private static IReadOnlyList<string> GetListValues(IReadOnlyDictionary<string, string> fields, string fieldName)
    {
        return fields
            .Where(field => TryGetListIndex(field.Key, fieldName, out _))
            .OrderBy(field =>
            {
                TryGetListIndex(field.Key, fieldName, out var index);
                return index;
            })
            .Select(field => field.Value)
            .ToList();
    }

    private static bool TryGetListIndex(string path, string fieldName, out int index)
    {
        index = 0;

        if (!path.StartsWith(fieldName + "[", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var indexStart = fieldName.Length + 1;
        var indexEnd = path.IndexOf(']', indexStart);
        if (indexEnd <= indexStart || path.Length != indexEnd + 1)
        {
            return false;
        }

        return int.TryParse(path[indexStart..indexEnd], NumberStyles.Integer, CultureInfo.InvariantCulture, out index);
    }

    private static bool TryGetIndexedPath(string path, string rootFieldName, out int index, out string remainder)
    {
        index = 0;
        remainder = string.Empty;

        if (!path.StartsWith(rootFieldName + "[", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var indexStart = rootFieldName.Length + 1;
        var indexEnd = path.IndexOf(']', indexStart);
        if (indexEnd <= indexStart ||
            !int.TryParse(path[indexStart..indexEnd], NumberStyles.Integer, CultureInfo.InvariantCulture, out index))
        {
            return false;
        }

        remainder = path[(indexEnd + 1)..];
        return true;
    }

    private static int GetTopLevelIndexedPathCount(IReadOnlyDictionary<string, string> fields, string rootFieldName)
    {
        return fields.Keys
            .Select(field => TryGetTopLevelIndexedPath(field, rootFieldName, out var index) ? index : -1)
            .Where(index => index >= 0)
            .Distinct()
            .Count();
    }

    private static bool TryGetTopLevelIndexedPath(string path, string rootFieldName, out int index)
    {
        index = 0;
        if (!path.StartsWith(rootFieldName + "[", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var indexStart = rootFieldName.Length + 1;
        var indexEnd = path.IndexOf(']', indexStart);
        return indexEnd > indexStart &&
               int.TryParse(path[indexStart..indexEnd], NumberStyles.Integer, CultureInfo.InvariantCulture, out index);
    }

    private static bool IsZero(string value)
    {
        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var number) && number == 0;
    }

    private static bool AreEquivalentSpriggitValues(string dtoValue, string spriggitValue)
    {
        if (string.Equals(dtoValue, spriggitValue, StringComparison.Ordinal))
        {
            return true;
        }

        return double.TryParse(dtoValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var dtoNumber) &&
               double.TryParse(spriggitValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var spriggitNumber) &&
               Math.Abs(dtoNumber - spriggitNumber) <= 0.000001;
    }

    private static IReadOnlyDictionary<string, string> AddRootScalarLists(string path, IReadOnlyDictionary<string, string> fields)
    {
        var mergedFields = new Dictionary<string, string>(fields, StringComparer.OrdinalIgnoreCase);
        var lines = File.ReadAllLines(path);

        for (var lineIndex = 0; lineIndex < lines.Length; lineIndex++)
        {
            var line = lines[lineIndex];
            if (string.IsNullOrWhiteSpace(line) || GetIndent(line) != 0)
            {
                continue;
            }

            var trimmed = line.Trim();
            if (!trimmed.EndsWith(":", StringComparison.Ordinal) || trimmed.StartsWith("- ", StringComparison.Ordinal))
            {
                continue;
            }

            var fieldName = trimmed[..^1];
            var values = new List<string>();
            for (var valueIndex = lineIndex + 1; valueIndex < lines.Length; valueIndex++)
            {
                var valueLine = lines[valueIndex];
                if (string.IsNullOrWhiteSpace(valueLine))
                {
                    continue;
                }

                if (GetIndent(valueLine) != 0 || !valueLine.TrimStart().StartsWith("- ", StringComparison.Ordinal))
                {
                    break;
                }

                values.Add(NormalizeScalar(valueLine.Trim()[2..].Trim()));
            }

            if (values.Count == 0)
            {
                continue;
            }

            mergedFields[fieldName + ".Count"] = values.Count.ToString(CultureInfo.InvariantCulture);
            for (var valueIndex = 0; valueIndex < values.Count; valueIndex++)
            {
                mergedFields[fieldName + "[" + valueIndex.ToString(CultureInfo.InvariantCulture) + "]"] = values[valueIndex];
            }
        }

        return mergedFields;
    }

    private static int GetIndent(string line)
    {
        var index = 0;
        while (index < line.Length && char.IsWhiteSpace(line[index]))
        {
            index++;
        }

        return index;
    }

    private static string NormalizeScalar(string value)
    {
        if (value.Length >= 2 &&
            ((value.StartsWith('\'') && value.EndsWith('\'')) || (value.StartsWith('"') && value.EndsWith('"'))))
        {
            return value[1..^1];
        }

        return value;
    }

    private static string FindSpriggitFile(SupportedGame game, string folder, string sampleName)
    {
        var root = EnvironmentLoader.GetExtractionRoot(game, SpriggitEnvironment.Value);
        var directory = Path.Combine(root, folder);
        if (!Directory.Exists(directory))
        {
            return FindSpriggitFile(root, sampleName);
        }

        var exactFileName = sampleName.EndsWith(".yaml", StringComparison.OrdinalIgnoreCase)
            ? sampleName
            : sampleName + ".yaml";
        var exactPath = Path.Combine(directory, exactFileName);
        if (File.Exists(exactPath))
        {
            return exactPath;
        }

        var matchingPath = Directory.GetFiles(directory, "*.yaml", SearchOption.TopDirectoryOnly)
            .FirstOrDefault(path =>
                string.Equals(Path.GetFileNameWithoutExtension(path), sampleName, StringComparison.OrdinalIgnoreCase) ||
                Path.GetFileNameWithoutExtension(path).StartsWith(sampleName + " - ", StringComparison.OrdinalIgnoreCase));

        return matchingPath ?? FindSpriggitFile(root, sampleName);
    }

    private static string FindSpriggitFile(string root, string sampleName)
    {
        if (!Directory.Exists(root))
        {
            throw new DirectoryNotFoundException("Spriggit extraction root should exist: " + root);
        }

        var matchingPath = Directory.GetFiles(root, "*.yaml", SearchOption.AllDirectories)
            .FirstOrDefault(path =>
                string.Equals(Path.GetFileNameWithoutExtension(path), sampleName, StringComparison.OrdinalIgnoreCase) ||
                Path.GetFileNameWithoutExtension(path).StartsWith(sampleName + " - ", StringComparison.OrdinalIgnoreCase));

        if (matchingPath == null)
        {
            throw new FileNotFoundException("Unable to find Spriggit sample '" + sampleName + "' under " + root + ".");
        }

        return matchingPath;
    }

    private static string GetRequiredString(IReadOnlyDictionary<string, string> fields, string fieldName, string path)
    {
        if (!fields.TryGetValue(fieldName, out var value))
        {
            throw new InvalidDataException("Spriggit file should contain field '" + fieldName + "': " + path);
        }

        return value;
    }

    private static int? GetOptionalInt(IReadOnlyDictionary<string, string> fields, string fieldName)
    {
        return fields.TryGetValue(fieldName, out var value)
            ? int.Parse(value, CultureInfo.InvariantCulture)
            : null;
    }

    private static double? GetOptionalDouble(IReadOnlyDictionary<string, string> fields, string fieldName)
    {
        return fields.TryGetValue(fieldName, out var value)
            ? double.Parse(value, CultureInfo.InvariantCulture)
            : null;
    }
}
