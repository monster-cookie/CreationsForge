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
        ["Transforms.Inventory"] = "InventoryTransformFormKey",
        ["InventoryArt"] = "InventoryTransformFormKey",
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
            if (!IsMatchedSpriggitField(field.Key, spriggit.Fields, dtoFields))
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
        AddSpriggitFieldAlias(fields, "InventoryTransformFormKey", "Transforms.Inventory");
        AddSpriggitFieldAlias(fields, "TeachesType", "Teaches.MutagenObjectType");
        AddSpriggitFieldAlias(fields, "TeachesRawContent", "Teaches.RawContent");
        AddSpriggitFieldAlias(fields, "Xalg", "XALG");
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
        for (var index = 0; index < count; index++)
        {
            AddSpriggitScriptingAdapterAlias(fields, index);
        }
    }

    private static void AddSpriggitScriptingAdapterAlias(IDictionary<string, string> fields, int scriptIndex)
    {
        var dtoScriptPath = "ScriptingAdapters[" + scriptIndex.ToString(CultureInfo.InvariantCulture) + "]";
        var spriggitScriptPath = "VirtualMachineAdapter[" + scriptIndex.ToString(CultureInfo.InvariantCulture) + "]";
        AddSpriggitFieldAlias(fields, dtoScriptPath + ".Name", spriggitScriptPath + ".Name");
        AddSpriggitFieldAlias(fields, dtoScriptPath + ".Properties.Count", spriggitScriptPath + ".Count");

        if (!fields.TryGetValue(dtoScriptPath + ".Properties.Count", out var propertyCountValue) ||
            !int.TryParse(propertyCountValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var propertyCount))
        {
            return;
        }

        for (var propertyIndex = 0; propertyIndex < propertyCount; propertyIndex++)
        {
            AddSpriggitScriptingAdapterPropertyAlias(fields, dtoScriptPath, spriggitScriptPath, propertyIndex);
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

        for (var listItemIndex = 0; listItemIndex < listItemCount; listItemIndex++)
        {
            var dtoListItemPath = dtoPropertyPath + ".ListItems[" + listItemIndex.ToString(CultureInfo.InvariantCulture) + "]";
            var spriggitListItemPath = spriggitPropertyPath + "[" + listItemIndex.ToString(CultureInfo.InvariantCulture) + "]";
            AddSpriggitFieldAlias(fields, dtoListItemPath + ".MutagenObjectType", spriggitListItemPath + ".MutagenObjectType");
            AddSpriggitFieldAlias(fields, dtoListItemPath + ".ObjectFormKey", spriggitListItemPath + ".Object");
            AddSpriggitScriptingAdapterDataAlias(fields, dtoListItemPath, spriggitListItemPath);
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

        if (IsDtoFieldBackedByAlternativeRootField(fieldName, spriggitFields, "Text", "BookText"))
        {
            return true;
        }

        if (string.Equals(fieldName, "InventoryTransformFormKey", StringComparison.OrdinalIgnoreCase) && spriggitFields.ContainsKey("InventoryArt"))
        {
            return true;
        }

        if (string.Equals(fieldName, "Transforms.Inventory", StringComparison.OrdinalIgnoreCase) && spriggitFields.ContainsKey("InventoryArt"))
        {
            return true;
        }

        if ((string.Equals(fieldName, "TeachesRawContent", StringComparison.OrdinalIgnoreCase) ||
             string.Equals(fieldName, "Teaches.RawContent", StringComparison.OrdinalIgnoreCase)) &&
            spriggitFields.ContainsKey("Teaches.RawContent"))
        {
            return true;
        }

        if (string.Equals(fieldName, "TeachesType", StringComparison.OrdinalIgnoreCase) && spriggitFields.ContainsKey("Teaches.MutagenObjectType"))
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

        if (IsMissingZeroCount(fieldName, fieldValue, spriggitFields))
        {
            return true;
        }

        if (IsMissingDefaultDtoField(fieldName, fieldValue, spriggitFields))
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

        if (IsDtoScriptingAdapterBackedBySpriggitField(fieldName, fieldValue, spriggitFields, dtoFields))
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

    private static bool IsDtoChildInfrastructureField(string fieldName)
    {
        return fieldName.Contains('[', StringComparison.Ordinal) &&
               (fieldName.EndsWith(".FormKey", StringComparison.OrdinalIgnoreCase) ||
                fieldName.EndsWith(".Game", StringComparison.OrdinalIgnoreCase) ||
                fieldName.EndsWith(".ImportedAtUTC", StringComparison.OrdinalIgnoreCase) ||
                fieldName.EndsWith(".ModKey", StringComparison.OrdinalIgnoreCase) ||
                fieldName.EndsWith(".RecordType", StringComparison.OrdinalIgnoreCase));
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
        if (!fieldName.StartsWith("VirtualMachineAdapter[", StringComparison.OrdinalIgnoreCase) ||
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

        var spriggitTypeName = componentTypeValue["MutagenObjectType:".Length..].Trim();
        return dtoFields
            .Where(field => field.Key.StartsWith("RawPayloads[", StringComparison.OrdinalIgnoreCase) &&
                            field.Key.EndsWith("].PayloadType", StringComparison.OrdinalIgnoreCase) &&
                            string.Equals(field.Value, spriggitTypeName, StringComparison.Ordinal))
            .Any(field =>
            {
                var payloadPath = field.Key[..^".PayloadType".Length];
                return dtoFields.TryGetValue(payloadPath + ".PayloadIndex", out var payloadIndexValue) &&
                       int.TryParse(payloadIndexValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var payloadIndex) &&
                       payloadIndex == componentIndex &&
                       dtoFields.TryGetValue(payloadPath + ".SourcePath", out var sourcePath) &&
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
               ((string.Equals(fieldName, "TeachesRawContent", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(fieldName, "Teaches.RawContent", StringComparison.OrdinalIgnoreCase)) &&
                string.Equals(fieldValue, uint.MaxValue.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal)) ||
               (string.Equals(fieldName, "DataSlateType", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(fieldValue, "None", StringComparison.OrdinalIgnoreCase));
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
            return spriggitFields.TryGetValue("VirtualMachineAdapter.Count", out var scriptCount)
                ? string.Equals(fieldValue, scriptCount, StringComparison.Ordinal)
                : GetTopLevelIndexedPathCount(spriggitFields, "VirtualMachineAdapter").ToString(CultureInfo.InvariantCulture) == fieldValue;
        }

        if (!fieldName.StartsWith("ScriptingAdapters[", StringComparison.OrdinalIgnoreCase) ||
            !TryGetIndexedPath(fieldName, "ScriptingAdapters", out var scriptIndex, out var scriptRemainder))
        {
            return false;
        }

        var spriggitScriptPath = "VirtualMachineAdapter[" + scriptIndex.ToString(CultureInfo.InvariantCulture) + "]";
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
            return spriggitFields.TryGetValue(spriggitScriptPath + ".Count", out var propertyCount) &&
                   string.Equals(fieldValue, propertyCount, StringComparison.Ordinal);
        }

        const string propertiesPrefix = ".Properties";
        if (!scriptRemainder.StartsWith(propertiesPrefix + "[", StringComparison.OrdinalIgnoreCase) ||
            !TryGetIndexedPath(scriptRemainder, propertiesPrefix, out var propertyIndex, out var propertyRemainder))
        {
            return false;
        }

        var spriggitPropertyPath = spriggitScriptPath + "[" + propertyIndex.ToString(CultureInfo.InvariantCulture) + "]";
        if (IsScriptingAdapterPropertyInfrastructureField(propertyRemainder, fieldValue, propertyIndex, spriggitScriptPath, spriggitFields))
        {
            return true;
        }

        if (string.Equals(propertyRemainder, ".ListItems.Count", StringComparison.OrdinalIgnoreCase))
        {
            return spriggitFields.TryGetValue(spriggitPropertyPath + ".Count", out var listItemCount)
                ? string.Equals(fieldValue, listItemCount, StringComparison.Ordinal)
                : IsZero(fieldValue);
        }

        const string listItemsPrefix = ".ListItems";
        if (propertyRemainder.StartsWith(listItemsPrefix + "[", StringComparison.OrdinalIgnoreCase) &&
            TryGetIndexedPath(propertyRemainder, listItemsPrefix, out var listItemIndex, out var listItemRemainder))
        {
            var spriggitListItemPath = spriggitPropertyPath + "[" + listItemIndex.ToString(CultureInfo.InvariantCulture) + "]";
            if (IsScriptingAdapterListItemInfrastructureField(listItemRemainder, fieldValue, propertyIndex, listItemIndex))
            {
                return true;
            }

            return IsDtoScriptingAdapterLeafBackedBySpriggitField(listItemRemainder, fieldValue, spriggitListItemPath, spriggitFields);
        }

        return IsDtoScriptingAdapterLeafBackedBySpriggitField(propertyRemainder, fieldValue, spriggitPropertyPath, spriggitFields);
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
        return !string.IsNullOrWhiteSpace(spriggitFieldName) &&
               spriggitFields.ContainsKey(spriggitFieldName);
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
        return fieldName.StartsWith("VirtualMachineAdapter[", StringComparison.OrdinalIgnoreCase) &&
               fieldName.Count(character => character == '[') == 3 &&
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
