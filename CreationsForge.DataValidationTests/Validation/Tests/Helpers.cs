using System.Globalization;
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

    private static readonly IReadOnlyDictionary<string, string> SpriggitToDtoFields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["MajorRecordFlagsRaw"] = "MajorRecordFlags",
        ["ObjectBounds.First"] = "ObjectBoundsFirst",
        ["ObjectBounds.Second"] = "ObjectBoundsSecond",
        ["Transforms.Inventory"] = "Transforms.Inventory",
        ["InventoryArt"] = "InventoryArt",
        ["PreviewTransform"] = "PreviewTransform",
        ["NativeTerminal"] = "NativeTerminalFormKey",
        ["Categroy"] = "Category",
        ["Restriction"] = "RestrictionFormKey",
        ["Training"] = "TrainingFormKey",
        ["Model.File"] = "Models[0].File",
        ["Model.Data"] = "Models[0].Data",
        ["Model.LightLayer"] = "Models[0].LightLayer",
        ["Lod.Level0"] = "LodLevel0",
        ["Lod.Level1"] = "LodLevel1",
        ["Lod.Level2"] = "LodLevel2",
        ["Lod.Level3"] = "LodLevel3"
    };

    public static TSpriggit GetSpriggit<TSpriggit>(SupportedGame game, RecordTypeData recordType, string sampleName)
        where TSpriggit : class
    {
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

    private static SpriggitRecordDTO GetSpriggitRecord(SupportedGame game, RecordTypeData recordType, string sampleName)
    {
        var path = FindSpriggitFile(game, recordType.TableName, sampleName);
        var document = SpriggitYamlDocument.Load(path);
        var fields = NormalizeSpriggitFields(game, recordType, AddRootScalarLists(path, document.FlattenScalars()));

        return new SpriggitRecordDTO
        {
            FormKey = GetRequiredString(fields, "FormKey", path),
            Fields = fields
        };
    }

    /// <summary>
    /// Finds Spriggit YAML fields that were not matched by the flattened CreationsForge DTO.
    /// This is an intentional coverage backstop for spec-driven validation tests, used to catch
    /// newly observed Spriggit fields that do not yet have DTO/import/readback coverage.
    /// </summary>
    /// <typeparam name="TRecord">The CreationsForge record DTO type being compared.</typeparam>
    /// <param name="spriggit">The flattened Spriggit YAML record for the validation sample.</param>
    /// <param name="dto">The CreationsForge DTO imported/read back for the same record.</param>
    /// <returns>
    /// Diagnostic messages for unmatched Spriggit fields. An empty list means the backstop found
    /// no Spriggit-side fields missing from the DTO comparison surface.
    /// </returns>
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

    /// <summary>
    /// Finds flattened CreationsForge DTO fields that were not matched by the Spriggit YAML record.
    /// This is an intentional coverage backstop for spec-driven validation tests, used to catch
    /// DTO/import/readback fields that are not represented by the Spriggit comparison surface.
    /// </summary>
    /// <typeparam name="TRecord">The CreationsForge record DTO type being compared.</typeparam>
    /// <param name="spriggit">The flattened Spriggit YAML record for the validation sample.</param>
    /// <param name="dto">The CreationsForge DTO imported/read back for the same record.</param>
    /// <returns>
    /// Diagnostic messages for unmatched DTO fields. An empty list means the backstop found no
    /// DTO-side fields missing from the Spriggit comparison surface.
    /// </returns>
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

    private static IReadOnlyDictionary<string, string> GetDTOFields<TRecord>(TRecord dto)
        where TRecord : RecordDTO
    {
        var fields = new Dictionary<string, string>(DtoFlattener.Flatten(dto), StringComparer.OrdinalIgnoreCase);
        NormalizeDtoModelFields(dto.Game, fields);
        AddSpriggitFieldAlias(fields, "ObjectBoundsFirst", "ObjectBounds.First");
        AddSpriggitFieldAlias(fields, "ObjectBoundsSecond", "ObjectBounds.Second");
        AddSpriggitFieldAlias(fields, "ObjectBounds.First", "ObjectBoundsFirst");
        AddSpriggitFieldAlias(fields, "ObjectBounds.Second", "ObjectBoundsSecond");
        AddSpriggitFieldAlias(fields, "XALG", "XALG");
        AddSpriggitFieldAlias(fields, "Models.Count", "Model.Count");
        AddSpriggitFieldAlias(fields, "Models[0].File", "Model.File");
        AddSpriggitFieldAlias(fields, "Models[0].Data", "Model.Data");
        AddSpriggitFieldAlias(fields, "Models[0].LightLayer", "Model.LightLayer");
        AddSpriggitFieldAlias(fields, "Models[0].Flags", "Model.Flags");
        AddSpriggitFieldAlias(fields, "LodLevel0", "Lod.Level0");
        AddSpriggitFieldAlias(fields, "LodLevel1", "Lod.Level1");
        AddSpriggitFieldAlias(fields, "LodLevel2", "Lod.Level2");
        AddSpriggitFieldAlias(fields, "LodLevel3", "Lod.Level3");
        AddSpriggitScalarListAliases(fields, "Flags");
        AddSpriggitMajorFlagAliases(fields, dto.Game);
        AddSpriggitScalarListAliases(fields, "Model.Flags");
        AddSpriggitScalarListAliases(fields, "DNAMDataTypeState");
        AddSpriggitKeywordAliases(fields);
        AddSpriggitPerkAliases(fields);
        AddSpriggitMiscComponentAliases(fields);
        AddSpriggitMiscResourceAliases(fields);
        AddSpriggitDestructibleAliases(fields);
        AddSpriggitModelMaterialSwapAliases(fields);
        AddSpriggitSoundAliases(fields);
        AddSpriggitScriptingAdapterAliases(fields);
        AddSpriggitScriptFragmentAliases(fields);
        AddSpriggitStaticNavmeshAliases(fields);
        NormalizeConditionFields(fields);
        return fields;
    }

    private static void NormalizeConditionFields(IDictionary<string, string> fields)
    {
        var projectedConditionSlots = new Dictionary<string, HashSet<int>>(StringComparer.OrdinalIgnoreCase);

        foreach (var field in fields.Keys.Where(field => field.EndsWith(".DataMutagenObjectType", StringComparison.OrdinalIgnoreCase)).ToList())
        {
            var basePath = field[..^".DataMutagenObjectType".Length];
            if (fields.TryGetValue(field, out var value) && !IsNull(value))
            {
                fields[basePath + ".Data.MutagenObjectType"] = NormalizeMutagenObjectTypeName(value);
            }

            AddConditionComparisonValueAlias(fields, basePath);
            AddConditionFlagAliases(fields, basePath);
            AddConditionParameterAliases(fields, basePath);
            if (TryProjectConditionSlot(fields, basePath, projectedConditionSlots))
            {
                RemoveConditionFields(fields, basePath);
                continue;
            }

            RemoveConditionInternalFields(fields, basePath);
        }

        AddProjectedConditionSlotCounts(fields, projectedConditionSlots);
        RemoveEmptyConditionCollectionCounts(fields);

        foreach (var field in fields.Keys.Where(field => field.EndsWith(".MutagenObjectType", StringComparison.OrdinalIgnoreCase)).ToList())
        {
            fields[field] = NormalizeMutagenObjectTypeName(fields[field]);
        }
    }

    private static void AddConditionComparisonValueAlias(IDictionary<string, string> fields, string basePath)
    {
        if (fields.TryGetValue(basePath + ".ComparisonValueFormKey", out var formKey) && !IsNull(formKey))
        {
            fields[basePath + ".ComparisonValue"] = formKey;
        }
    }

    private static void AddConditionFlagAliases(IDictionary<string, string> fields, string basePath)
    {
        if (!fields.TryGetValue(basePath + ".Flags", out var flags) || IsNull(flags) || string.IsNullOrWhiteSpace(flags))
        {
            return;
        }

        var flagValues = flags.Split(", ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        fields[basePath + ".Flags.Count"] = flagValues.Length.ToString(CultureInfo.InvariantCulture);
        for (var index = 0; index < flagValues.Length; index++)
        {
            fields[basePath + ".Flags[" + index.ToString(CultureInfo.InvariantCulture) + "]"] = flagValues[index];
        }
    }

    private static void AddConditionParameterAliases(IDictionary<string, string> fields, string basePath)
    {
        if (!fields.TryGetValue(basePath + ".Parameters.Count", out var countValue) ||
            !int.TryParse(countValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var count))
        {
            return;
        }

        for (var index = 0; index < count; index++)
        {
            var parameterPath = basePath + ".Parameters[" + index.ToString(CultureInfo.InvariantCulture) + "]";
            if (!fields.TryGetValue(parameterPath + ".ParameterName", out var parameterName) || string.IsNullOrWhiteSpace(parameterName))
            {
                continue;
            }

            if (fields.TryGetValue(parameterPath + ".ParameterFormKey", out var parameterFormKey) && !IsNull(parameterFormKey))
            {
                fields[basePath + ".Data." + parameterName] = parameterFormKey;
                continue;
            }

            if (fields.TryGetValue(parameterPath + ".ParameterValue", out var parameterValue) && !IsNull(parameterValue))
            {
                fields[basePath + ".Data." + parameterName] = parameterValue;
            }
        }
    }

    private static void RemoveConditionInternalFields(IDictionary<string, string> fields, string basePath)
    {
        foreach (var field in fields.Keys
            .Where(field => string.Equals(field, basePath + ".ConditionSlot", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(field, basePath + ".DataMutagenObjectType", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(field, basePath + ".ComparisonValueFormKey", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(field, basePath + ".Flags", StringComparison.OrdinalIgnoreCase) ||
                field.StartsWith(basePath + ".Parameters", StringComparison.OrdinalIgnoreCase))
            .ToList())
        {
            fields.Remove(field);
        }
    }

    private static bool TryProjectConditionSlot(
        IDictionary<string, string> fields,
        string basePath,
        IDictionary<string, HashSet<int>> projectedConditionSlots)
    {
        if (!fields.TryGetValue(basePath + ".ConditionSlot", out var conditionSlot) ||
            string.IsNullOrWhiteSpace(conditionSlot) ||
            string.Equals(conditionSlot, "Conditions", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var conditionIndex = GetConditionIndex(fields, basePath);
        var projectedBasePath = conditionSlot + "[" + conditionIndex.ToString(CultureInfo.InvariantCulture) + "]";
        if (string.Equals(projectedBasePath, basePath, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        foreach (var field in fields.Where(field => IsUnderPath(field.Key, basePath)).ToList())
        {
            if (IsConditionStorageField(field.Key, basePath))
            {
                continue;
            }

            fields[projectedBasePath + field.Key[basePath.Length..]] = field.Value;
        }

        if (!projectedConditionSlots.TryGetValue(conditionSlot, out var conditionIndexes))
        {
            conditionIndexes = new HashSet<int>();
            projectedConditionSlots[conditionSlot] = conditionIndexes;
        }

        conditionIndexes.Add(conditionIndex);
        return true;
    }

    private static int GetConditionIndex(IDictionary<string, string> fields, string basePath)
    {
        if (fields.TryGetValue(basePath + ".ConditionIndex", out var conditionIndexText) &&
            int.TryParse(conditionIndexText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var conditionIndex))
        {
            return conditionIndex;
        }

        var indexStart = basePath.LastIndexOf('[');
        var indexEnd = basePath.LastIndexOf(']');
        if (indexStart >= 0 &&
            indexEnd > indexStart &&
            int.TryParse(basePath.AsSpan(indexStart + 1, indexEnd - indexStart - 1), NumberStyles.Integer, CultureInfo.InvariantCulture, out conditionIndex))
        {
            return conditionIndex;
        }

        return 0;
    }

    private static void AddProjectedConditionSlotCounts(
        IDictionary<string, string> fields,
        IReadOnlyDictionary<string, HashSet<int>> projectedConditionSlots)
    {
        foreach (var projectedConditionSlot in projectedConditionSlots)
        {
            var count = projectedConditionSlot.Value.Count == 0 ? 0 : projectedConditionSlot.Value.Max() + 1;
            fields[projectedConditionSlot.Key + ".Count"] = count.ToString(CultureInfo.InvariantCulture);
        }
    }

    private static void RemoveConditionFields(IDictionary<string, string> fields, string basePath)
    {
        foreach (var field in fields.Keys.Where(field => IsUnderPath(field, basePath)).ToList())
        {
            fields.Remove(field);
        }
    }

    private static void RemoveEmptyConditionCollectionCounts(IDictionary<string, string> fields)
    {
        foreach (var field in fields.Keys.Where(field => field.EndsWith(".Conditions.Count", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(field, "Conditions.Count", StringComparison.OrdinalIgnoreCase)).ToList())
        {
            var collectionPath = field[..^".Count".Length];
            if (fields.Keys.Any(valueField => valueField.StartsWith(collectionPath + "[", StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            fields.Remove(field);
        }
    }

    private static bool IsConditionStorageField(string fieldName, string basePath)
    {
        return string.Equals(fieldName, basePath + ".ConditionSlot", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(fieldName, basePath + ".ConditionIndex", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(fieldName, basePath + ".DataMutagenObjectType", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(fieldName, basePath + ".ComparisonValueFormKey", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(fieldName, basePath + ".Flags", StringComparison.OrdinalIgnoreCase) ||
               fieldName.StartsWith(basePath + ".Parameters", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsUnderPath(string fieldName, string path)
    {
        return string.Equals(fieldName, path, StringComparison.OrdinalIgnoreCase) ||
               fieldName.StartsWith(path + ".", StringComparison.OrdinalIgnoreCase) ||
               fieldName.StartsWith(path + "[", StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeMutagenObjectTypeName(string value)
    {
        const string binaryOverlaySuffix = "BinaryOverlay";
        return value.EndsWith(binaryOverlaySuffix, StringComparison.Ordinal)
            ? value[..^binaryOverlaySuffix.Length]
            : value;
    }

    private static void NormalizeDtoModelFields(SupportedGame game, Dictionary<string, string> fields)
    {
        var modelCount = GetIndexedPathCount(fields, "Models");
        if (modelCount > 0)
        {
            fields["Models.Count"] = modelCount.ToString(CultureInfo.InvariantCulture);
            fields["Model.Count"] = modelCount.ToString(CultureInfo.InvariantCulture);
        }

    }

    private static void AddSpriggitFieldAlias(IDictionary<string, string> fields, string dtoFieldName, string spriggitFieldName)
    {
        if (fields.TryGetValue(dtoFieldName, out var value))
        {
            fields[spriggitFieldName] = value;
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
            AddSpriggitSoundAlias(fields, soundPath, soundSlot, "MutagenObjectType");
            AddSpriggitSoundAlias(fields, soundPath, soundSlot, "InheritsSoundsFrom");
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
                "Keywords[" + index.ToString(CultureInfo.InvariantCulture) + "].Keyword",
                "Keywords[" + index.ToString(CultureInfo.InvariantCulture) + "]");
        }
    }

    private static void AddSpriggitPerkAliases(Dictionary<string, string> fields)
    {
        var rankCount = GetIndexedPathCount(fields, "Ranks");
        for (var rankIndex = 0; rankIndex < rankCount; rankIndex++)
        {
            AddSpriggitFieldAlias(
                fields,
                "Ranks[" + rankIndex.ToString(CultureInfo.InvariantCulture) + "].UnknownStaticFormKey",
                "Ranks[" + rankIndex.ToString(CultureInfo.InvariantCulture) + "].UnknownStatic");
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
            AddSpriggitFieldAlias(
                fields,
                "Models[0].MaterialSwaps[" + index.ToString(CultureInfo.InvariantCulture) + "].Name",
                "Model[" + index.ToString(CultureInfo.InvariantCulture) + "].Name");
            AddSpriggitFieldAlias(
                fields,
                "Models[0].MaterialSwaps[" + index.ToString(CultureInfo.InvariantCulture) + "].MaterialSwapFormKey",
                "Model[" + index.ToString(CultureInfo.InvariantCulture) + "].NewTexture");

            if (fields.TryGetValue("Models[0].MaterialSwaps[" + index.ToString(CultureInfo.InvariantCulture) + "].MaterialSwapFormKey", out var materialSwapFormKey))
            {
                var formIdSeparator = materialSwapFormKey.IndexOf(':', StringComparison.Ordinal);
                if (formIdSeparator > 0)
                {
                    fields["Model[" + index.ToString(CultureInfo.InvariantCulture) + "]." + materialSwapFormKey[..formIdSeparator]] = materialSwapFormKey;
                }
            }
        }

        if (count == 1)
        {
            AddSpriggitFieldAlias(fields, "Models[0].MaterialSwaps[0].MaterialSwapFormKey", "Model.MaterialSwap");
            AddSpriggitFieldAlias(fields, "Models[0].MaterialSwaps[0].MaterialSwapFormKey", "Model[1]");
        }
    }

    private static void AddSpriggitMiscComponentAliases(IDictionary<string, string> fields)
    {
        if (!fields.TryGetValue("Components.Count", out var countValue) ||
            !int.TryParse(countValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var count))
        {
            return;
        }

        var hasComponentDisplayIndices = fields.Keys.Any(field =>
            field.StartsWith("Components[", StringComparison.OrdinalIgnoreCase) &&
            field.EndsWith("].DisplayIndex", StringComparison.OrdinalIgnoreCase) &&
            fields.TryGetValue(field, out var displayIndex) &&
            !string.Equals(displayIndex, "Null", StringComparison.OrdinalIgnoreCase));
        if (hasComponentDisplayIndices)
        {
            AddSpriggitFieldAlias(fields, "Components.Count", "ComponentDisplayIndices.Count");
        }

        for (var index = 0; index < count; index++)
        {
            AddSpriggitFieldAlias(
                fields,
                "Components[" + index.ToString(CultureInfo.InvariantCulture) + "].Count",
                "Count[" + index.ToString(CultureInfo.InvariantCulture) + "]");
            AddSpriggitFieldAlias(
                fields,
                "Components[" + index.ToString(CultureInfo.InvariantCulture) + "].DisplayIndex",
                "ComponentDisplayIndices[" + index.ToString(CultureInfo.InvariantCulture) + "]");
        }
    }

    private static void AddSpriggitMiscResourceAliases(IDictionary<string, string> fields)
    {
        if (!fields.TryGetValue("Resources.Count", out var countValue) ||
            !int.TryParse(countValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var count))
        {
            return;
        }

        if (count == 1)
        {
            AddSpriggitFieldAlias(fields, "Resources[0].Count", "Count");
        }

        for (var index = 0; index < count; index++)
        {
            AddSpriggitFieldAlias(
                fields,
                "Resources[" + index.ToString(CultureInfo.InvariantCulture) + "].Count",
                "Count[" + index.ToString(CultureInfo.InvariantCulture) + "]");
        }
    }

    private static void AddSpriggitSoundAlias(IDictionary<string, string> fields, string soundPath, string soundSlot, string fieldName)
    {
        if (fields.TryGetValue(soundPath + "." + fieldName, out var value))
        {
            fields[soundSlot + "." + fieldName] = value;
        }
    }

    private static void AddSpriggitScriptingAdapterAliases(IDictionary<string, string> fields)
    {
        if (!fields.TryGetValue("ScriptingAdapters.Count", out var countValue) ||
            !int.TryParse(countValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var count))
        {
            return;
        }

        if (count == 0)
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

    /// <summary>
    /// Adds Spriggit list-shaped aliases for typed major flag fields that preserve the same flag data.
    /// </summary>
    private static void AddSpriggitMajorFlagAliases(IDictionary<string, string> fields, SupportedGame game)
    {
        AddSpriggitScalarListAliases(fields, "MajorFlags");
        if (!fields.TryGetValue("MajorRecordFlags", out var rawFlags) || IsZero(rawFlags))
        {
            return;
        }

        var gameFlagPath = game switch
        {
            SupportedGame.Starfield => "StarfieldMajorRecordFlags",
            SupportedGame.Fallout4 => "Fallout4MajorRecordFlags",
            SupportedGame.Skyrim => "SkyrimMajorRecordFlags",
            _ => null
        };
        if (gameFlagPath == null)
        {
            return;
        }

        fields[gameFlagPath + ".Count"] = "1";
        fields[gameFlagPath + "[0]"] = game == SupportedGame.Fallout4
            ? "0x" + int.Parse(rawFlags, CultureInfo.InvariantCulture).ToString("X", CultureInfo.InvariantCulture)
            : GetGameSpecificMajorFlagAlias(fields, game) ?? rawFlags;
    }

    /// <summary>
    /// Gets a known game-specific major flag alias for a preserved generic major flag value.
    /// </summary>
    private static string? GetGameSpecificMajorFlagAlias(IDictionary<string, string> fields, SupportedGame game)
    {
        if (!fields.TryGetValue("MajorFlags", out var majorFlags))
        {
            return null;
        }

        return game == SupportedGame.Starfield &&
               majorFlags.Split(", ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Contains("HasDistantLOD")
            ? "VisibleWhenDistant"
            : null;
    }

    private static void AddSpriggitScriptFragmentAliases(IDictionary<string, string> fields)
    {
        if (!fields.TryGetValue("ScriptFragments.Count", out var countValue) ||
            !int.TryParse(countValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var count))
        {
            return;
        }

        for (var index = 0; index < count; index++)
        {
            var dtoFragmentPath = "ScriptFragments[" + index.ToString(CultureInfo.InvariantCulture) + "]";
            if (!fields.TryGetValue(dtoFragmentPath + ".FragmentSlot", out var fragmentSlot))
            {
                continue;
            }

            if (string.Equals(fragmentSlot, "ScriptFragments", StringComparison.Ordinal))
            {
                AddSpriggitFieldAlias(fields, dtoFragmentPath + ".ExtraBindDataVersion", "VirtualMachineAdapter.ScriptFragments.ExtraBindDataVersion");
                continue;
            }

            if (string.Equals(fragmentSlot, "ScriptFragments.Fragments", StringComparison.Ordinal))
            {
                var fragmentIndex = GetFragmentIndex(fields, dtoFragmentPath);
                var spriggitFragmentPath = "VirtualMachineAdapter.ScriptFragments.Fragments[" + fragmentIndex.ToString(CultureInfo.InvariantCulture) + "]";
                fields["VirtualMachineAdapter.ScriptFragments.Fragments.Count"] = (fragmentIndex + 1).ToString(CultureInfo.InvariantCulture);
                AddSpriggitFieldAlias(fields, dtoFragmentPath + ".ScriptName", spriggitFragmentPath + ".ScriptName");
                AddSpriggitFieldAlias(fields, dtoFragmentPath + ".FragmentName", spriggitFragmentPath + ".FragmentName");
                AddSpriggitFieldAlias(fields, dtoFragmentPath + ".SourceFragmentIndex", spriggitFragmentPath + ".FragmentIndex");
                AddSpriggitFieldAlias(fields, dtoFragmentPath + ".Unknown2", spriggitFragmentPath + ".Unknown2");
                continue;
            }

            if (string.Equals(fragmentSlot, "ScriptFragments.Script", StringComparison.Ordinal))
            {
                AddSpriggitFieldAlias(fields, dtoFragmentPath + ".ScriptName", "VirtualMachineAdapter.ScriptFragments.Script.Name");
                AddSpriggitScriptFragmentScriptPropertyAliases(fields);
            }
        }
    }

    private static int GetFragmentIndex(IDictionary<string, string> fields, string dtoFragmentPath)
    {
        return fields.TryGetValue(dtoFragmentPath + ".FragmentIndex", out var fragmentIndexValue) &&
               int.TryParse(fragmentIndexValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var fragmentIndex)
            ? fragmentIndex
            : 0;
    }

    private static void AddSpriggitScriptFragmentScriptPropertyAliases(IDictionary<string, string> fields)
    {
        var dtoScriptPath = GetScriptFragmentScriptAdapterPath(fields);
        if (dtoScriptPath == null || !fields.TryGetValue(dtoScriptPath + ".Properties.Count", out var propertyCount))
        {
            return;
        }

        fields["VirtualMachineAdapter.ScriptFragments.Script.Properties.Count"] = propertyCount;
        if (!int.TryParse(propertyCount, NumberStyles.Integer, CultureInfo.InvariantCulture, out var count))
        {
            return;
        }

        for (var propertyIndex = 0; propertyIndex < count; propertyIndex++)
        {
            AddSpriggitScriptingAdapterPropertyAlias(
                fields,
                dtoScriptPath,
                "VirtualMachineAdapter.ScriptFragments.Script.Properties",
                propertyIndex);
        }
    }

    private static string? GetScriptFragmentScriptAdapterPath(IDictionary<string, string> fields)
    {
        if (!fields.TryGetValue("VirtualMachineAdapter.ScriptFragments.Script.Name", out var scriptName))
        {
            return null;
        }

        if (!fields.TryGetValue("ScriptingAdapters.Count", out var scriptCountValue) ||
            !int.TryParse(scriptCountValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var scriptCount))
        {
            return null;
        }

        for (var scriptIndex = 0; scriptIndex < scriptCount; scriptIndex++)
        {
            var dtoScriptPath = "ScriptingAdapters[" + scriptIndex.ToString(CultureInfo.InvariantCulture) + "]";
            if (fields.TryGetValue(dtoScriptPath + ".Name", out var dtoScriptName) &&
                string.Equals(dtoScriptName, scriptName, StringComparison.Ordinal))
            {
                return dtoScriptPath;
            }
        }

        return null;
    }

    private static void AddSpriggitStaticNavmeshAliases(IDictionary<string, string> fields)
    {
        if (fields.TryGetValue("NavmeshGeometry.GridArrays[0].GridCell.Count", out var gridCellCount))
        {
            fields["NavmeshGeometry.GridArrays.GridCell.Count"] = gridCellCount;
            if (int.TryParse(gridCellCount, NumberStyles.Integer, CultureInfo.InvariantCulture, out var count))
            {
                for (var index = 0; index < count; index++)
                {
                    AddSpriggitFieldAlias(
                        fields,
                        "NavmeshGeometry.GridArrays[0].GridCell[" + index.ToString(CultureInfo.InvariantCulture) + "]",
                        "NavmeshGeometry.GridArrays.GridCell[" + index.ToString(CultureInfo.InvariantCulture) + "]");
                }
            }
        }

        if (fields.TryGetValue("NavmeshGeometry.Vertices.Count", out var vertexCount) &&
            int.TryParse(vertexCount, NumberStyles.Integer, CultureInfo.InvariantCulture, out var vertices))
        {
            for (var index = 0; index < vertices; index++)
            {
                AddSpriggitFieldAlias(
                    fields,
                    "NavmeshGeometry.Vertices[" + index.ToString(CultureInfo.InvariantCulture) + "].Point",
                    "NavmeshGeometry.Vertices[" + index.ToString(CultureInfo.InvariantCulture) + "]");
            }
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
        AddSpriggitFieldAlias(fields, dtoPropertyPath + ".ObjectAlias", spriggitPropertyPath + ".Alias");
        AddSpriggitFieldAlias(fields, dtoPropertyPath + ".ObjectUnused", spriggitPropertyPath + ".Unused");
        AddSpriggitScriptingAdapterDataAlias(fields, dtoPropertyPath, spriggitPropertyPath);
        AddSpriggitFieldAlias(fields, dtoPropertyPath + ".ListItems.Count", spriggitPropertyPath + ".Count");
        AddSpriggitFieldAlias(fields, dtoPropertyPath + ".Structs.Count", spriggitPropertyPath + ".Structs.Count");

        if (fields.TryGetValue(dtoPropertyPath + ".Structs.Count", out var structCountValue) &&
            int.TryParse(structCountValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var structCount))
        {
            for (var structIndex = 0; structIndex < structCount; structIndex++)
            {
                AddSpriggitScriptingAdapterStructAlias(fields, dtoPropertyPath, spriggitPropertyPath, structIndex);
            }
        }

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
            AddSpriggitFieldAlias(fields, dtoListItemPath + ".Name", spriggitListItemPath + ".Name");
            AddSpriggitFieldAlias(fields, dtoListItemPath + ".ObjectFormKey", spriggitListItemPath + ".Object");
            AddSpriggitFieldAlias(fields, dtoListItemPath + ".MutagenObjectType", spriggitObjectListItemPath + ".MutagenObjectType");
            AddSpriggitFieldAlias(fields, dtoListItemPath + ".Name", spriggitObjectListItemPath + ".Name");
            AddSpriggitFieldAlias(fields, dtoListItemPath + ".ObjectFormKey", spriggitObjectListItemPath + ".Object");
            AddSpriggitScriptingAdapterDataAlias(fields, dtoListItemPath, spriggitListItemPath);
            AddSpriggitScriptingAdapterDataAlias(fields, dtoListItemPath, spriggitObjectListItemPath);
        }
    }

    /// <summary>
    /// Projects typed VMAD script property struct DTO fields to the equivalent Spriggit struct paths.
    /// </summary>
    private static void AddSpriggitScriptingAdapterStructAlias(
        IDictionary<string, string> fields,
        string dtoPropertyPath,
        string spriggitPropertyPath,
        int structIndex)
    {
        var dtoStructPath = dtoPropertyPath + ".Structs[" + structIndex.ToString(CultureInfo.InvariantCulture) + "]";
        var spriggitStructPath = spriggitPropertyPath + ".Structs[" + structIndex.ToString(CultureInfo.InvariantCulture) + "]";
        AddSpriggitFieldAlias(fields, dtoStructPath + ".Members.Count", spriggitStructPath + ".Members.Count");

        if (!fields.TryGetValue(dtoStructPath + ".Members.Count", out var memberCountValue) ||
            !int.TryParse(memberCountValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var memberCount))
        {
            return;
        }

        for (var memberIndex = 0; memberIndex < memberCount; memberIndex++)
        {
            var dtoMemberPath = dtoStructPath + ".Members[" + memberIndex.ToString(CultureInfo.InvariantCulture) + "]";
            var spriggitMemberPath = spriggitStructPath + ".Members[" + memberIndex.ToString(CultureInfo.InvariantCulture) + "]";
            AddSpriggitFieldAlias(fields, dtoMemberPath + ".MutagenObjectType", spriggitMemberPath + ".MutagenObjectType");
            AddSpriggitFieldAlias(fields, dtoMemberPath + ".Name", spriggitMemberPath + ".Name");
            AddSpriggitFieldAlias(fields, dtoMemberPath + ".ObjectFormKey", spriggitMemberPath + ".Object");
            AddSpriggitFieldAlias(fields, dtoMemberPath + ".ObjectAlias", spriggitMemberPath + ".Alias");
            AddSpriggitFieldAlias(fields, dtoMemberPath + ".ObjectUnused", spriggitMemberPath + ".Unused");
            AddSpriggitScriptingAdapterDataAlias(fields, dtoMemberPath, spriggitMemberPath);
        }
    }

    private static void AddSpriggitScriptingAdapterDataAlias(IDictionary<string, string> fields, string dtoPath, string spriggitPath)
    {
        AddSpriggitDataAlias(fields, dtoPath + ".DataBool", spriggitPath + ".Data");
        AddSpriggitDataAlias(fields, dtoPath + ".DataFloat", spriggitPath + ".Data");
        AddSpriggitDataAlias(fields, dtoPath + ".DataInt", spriggitPath + ".Data");
        AddSpriggitDataAlias(fields, dtoPath + ".DataString", spriggitPath + ".Data");
    }

    private static void AddSpriggitDataAlias(IDictionary<string, string> fields, string dtoFieldName, string spriggitFieldName)
    {
        if (fields.TryGetValue(dtoFieldName, out var value) &&
            !string.Equals(value, "Null", StringComparison.OrdinalIgnoreCase))
        {
            fields[spriggitFieldName] = value;
        }
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

        if (fieldName.Contains('[', StringComparison.Ordinal) &&
            !fieldName.Contains('.', StringComparison.Ordinal) &&
            fieldValue.EndsWith(":", StringComparison.Ordinal))
        {
            return true;
        }

        if (string.Equals(fieldValue, "[]", StringComparison.Ordinal) &&
            dtoFields.TryGetValue(fieldName + ".Count", out var dtoCount) &&
            IsZero(dtoCount))
        {
            return true;
        }

        if (string.Equals(fieldValue, "[]", StringComparison.Ordinal) &&
            fieldName.EndsWith(".Conditions", StringComparison.OrdinalIgnoreCase))
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

        if (IsSpriggitObjectListItemMutagenType(fieldName, spriggitFields, dtoFields, "Components"))
        {
            return true;
        }

        if (IsSpriggitDestructibleFieldBackedByDtoField(fieldName, spriggitFields, dtoFields))
        {
            return true;
        }

        if (IsSpriggitKeywordComponentFieldBackedByDtoScalar(fieldName, spriggitFields, dtoFields))
        {
            return true;
        }

        if (IsSpriggitReflectionFieldBackedByDtoReflection(fieldName, fieldValue, spriggitFields, dtoFields))
        {
            return true;
        }

        if (IsSpriggitModelMaterialSwapFieldBackedByDtoField(fieldName, fieldValue, dtoFields))
        {
            return true;
        }

        if (IsSpriggitResourceInlineObjectBackedByDtoField(fieldName, fieldValue, dtoFields))
        {
            return true;
        }

        if (IsSpriggitActorValueInformationFieldBackedByDtoField(fieldName, spriggitFields, dtoFields))
        {
            return true;
        }

        if (IsSpriggitInlineFormKeyListItemBackedByDtoScalar(fieldName, fieldValue, dtoFields, "Keywords", "Keyword"))
        {
            return true;
        }

        if (IsSpriggitInlineFormKeyListItemBackedByDtoScalar(fieldName, fieldValue, dtoFields, "BackgroundSkills", "SkillFormKey"))
        {
            return true;
        }

        if (IsSpriggitInlineFormKeyListItemBackedByDtoScalar(fieldName, fieldValue, dtoFields, "Items", "Item"))
        {
            return true;
        }

        if (IsSpriggitInlineFormKeyListItemBackedByDtoScalar(fieldName, fieldValue, dtoFields, "ForcedLocations", string.Empty))
        {
            return true;
        }

        if (IsSpriggitNestedListBackedDtoScalar(fieldName, fieldValue, dtoFields, "Flags"))
        {
            return true;
        }

        if (IsSpriggitStaticNavmeshFieldBackedByDtoField(fieldName, fieldValue, dtoFields))
        {
            return true;
        }

        return IsSpriggitListBackedDtoScalar(fieldName, spriggitFields, dtoFields, "Flags") ||
               IsSpriggitListBackedDtoScalar(fieldName, spriggitFields, dtoFields, "MajorFlags");
    }

    private static bool IsSpriggitStaticNavmeshFieldBackedByDtoField(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> dtoFields)
    {
        if (!fieldName.StartsWith("NavmeshGeometry.CoverTriangleMappings[", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (fieldName.EndsWith("]", StringComparison.Ordinal) &&
            string.Equals(fieldValue, "{}", StringComparison.Ordinal))
        {
            return dtoFields.TryGetValue(fieldName + ".Value", out var dtoValue) &&
                   string.Equals(dtoValue, "0, 0", StringComparison.Ordinal);
        }

        return false;
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

        if (IsDtoCollectionMetadataField(fieldName))
        {
            return true;
        }

        if (IsDtoStaticNavmeshFieldBackedBySpriggitField(fieldName, fieldValue, spriggitFields))
        {
            return true;
        }

        if (IsDtoScriptFragmentProjectionBackedBySpriggitField(fieldName, fieldValue, spriggitFields, dtoFields))
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

        if (string.Equals(fieldName, "ScriptingAdapters.Count", StringComparison.OrdinalIgnoreCase) && IsZero(fieldValue))
        {
            return true;
        }

        if (IsMissingDefaultDtoField(fieldName, fieldValue, spriggitFields))
        {
            return true;
        }

        if (IsDtoNullBackedByEmptySpriggitTranslation(fieldName, fieldValue, spriggitFields))
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

        if (IsDtoCollectionIndexFieldBackedByPathIndex(fieldName, fieldValue))
        {
            return true;
        }

        if (IsDtoIndexedPropertyBackedBySpriggitScalarList(fieldName, spriggitFields, "Keywords", "Keyword"))
        {
            return true;
        }

        if (IsDtoIndexedPropertyBackedBySpriggitScalarList(fieldName, spriggitFields, "BackgroundSkills", "SkillFormKey"))
        {
            return true;
        }

        if (IsDtoKeywordBackedBySpriggitComponentKeyword(fieldName, fieldValue, spriggitFields, dtoFields))
        {
            return true;
        }

        if (IsDtoReflectionFieldBackedBySpriggitComponent(fieldName, fieldValue, spriggitFields, dtoFields))
        {
            return true;
        }

        if (IsDtoIndexedPropertyBackedBySpriggitScalarList(fieldName, spriggitFields, "Items", "Item"))
        {
            return true;
        }

        if (IsDtoIndexedMetadataBackedBySpriggitScalarList(fieldName, fieldValue, spriggitFields, "Keywords", "KeywordIndex"))
        {
            return true;
        }

        if (IsDtoIndexedMetadataBackedBySpriggitScalarList(fieldName, fieldValue, spriggitFields, "BackgroundSkills", "SkillIndex"))
        {
            return true;
        }

        if (IsDtoIndexedMetadataBackedBySpriggitScalarList(fieldName, fieldValue, spriggitFields, "Items", "ItemIndex"))
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

        if (IsDtoResourceMetadataBackedBySpriggitField(fieldName, fieldValue, spriggitFields))
        {
            return true;
        }

        if (IsDtoMiscComponentDisplayIndexBackedBySpriggitField(fieldName, fieldValue, spriggitFields))
        {
            return true;
        }

        if (IsDtoDestructibleFieldBackedBySpriggitField(fieldName, fieldValue, spriggitFields))
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

        if (IsDtoNestedScalarBackedBySpriggitList(fieldName, fieldValue, spriggitFields, "Flags"))
        {
            return true;
        }

        if (fieldName.EndsWith(".UnknownStaticFormKey", StringComparison.OrdinalIgnoreCase) &&
            spriggitFields.ContainsKey(fieldName[..^"FormKey".Length]))
        {
            return true;
        }

        if (IsDtoSoundFieldBackedBySpriggitNamedSound(fieldName, fieldValue, spriggitFields, dtoFields))
        {
            return true;
        }

        return IsSpriggitListBackedDtoScalar(fieldName, spriggitFields, dtoFields, "Flags") ||
               IsSpriggitListBackedDtoScalar(fieldName, spriggitFields, dtoFields, "MajorFlags") ||
               IsSpriggitListBackedDtoScalar(fieldName, spriggitFields, dtoFields, "DNAMDataTypeState") ||
               IsSpriggitListBackedDtoScalar(fieldName, spriggitFields, dtoFields, "Model.Flags") ||
               (string.Equals(fieldName, "Models[0].Flags", StringComparison.OrdinalIgnoreCase) &&
                IsSpriggitListBackedDtoScalar("Model.Flags", spriggitFields, dtoFields, "Model.Flags"));
    }

    private static bool IsDtoStaticNavmeshFieldBackedBySpriggitField(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> spriggitFields)
    {
        if (!fieldName.StartsWith("NavmeshGeometry.", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (string.Equals(fieldName, "NavmeshGeometry.GridArrays.Count", StringComparison.OrdinalIgnoreCase) &&
            string.Equals(fieldValue, "1", StringComparison.Ordinal) &&
            (spriggitFields.ContainsKey("NavmeshGeometry.GridArrays.GridCell.Count") ||
             spriggitFields.ContainsKey("NavmeshGeometry.GridArrays[0].GridCell.Count")))
        {
            return true;
        }

        if (TryMatchStaticNavmeshAlternatePath(fieldName, fieldValue, spriggitFields, "NavmeshGeometry.GridArrays[0].GridCell", "NavmeshGeometry.GridArrays.GridCell"))
        {
            return true;
        }

        if (TryMatchStaticNavmeshAlternatePath(fieldName, fieldValue, spriggitFields, "NavmeshGeometry.Vertices", "NavmeshGeometry.Vertices", ".Point"))
        {
            return true;
        }

        if (fieldName.EndsWith(".GridArrayIndex", StringComparison.OrdinalIgnoreCase) ||
            fieldName.EndsWith(".TriangleIndex", StringComparison.OrdinalIgnoreCase) ||
            fieldName.EndsWith(".VertexIndex", StringComparison.OrdinalIgnoreCase) ||
            fieldName.EndsWith(".CoverIndex", StringComparison.OrdinalIgnoreCase) ||
            fieldName.EndsWith(".MappingIndex", StringComparison.OrdinalIgnoreCase))
        {
            return int.TryParse(fieldValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out _);
        }

        if (IsZero(fieldValue) &&
            (fieldName.EndsWith(".CoverFlags", StringComparison.OrdinalIgnoreCase) ||
             fieldName.EndsWith(".EdgeLink_0_1", StringComparison.OrdinalIgnoreCase) ||
             fieldName.EndsWith(".EdgeLink_1_2", StringComparison.OrdinalIgnoreCase) ||
             fieldName.EndsWith(".EdgeLink_2_0", StringComparison.OrdinalIgnoreCase) ||
             (fieldName.StartsWith("NavmeshGeometry.Cover[", StringComparison.OrdinalIgnoreCase) &&
              (fieldName.EndsWith(".Vertex1", StringComparison.OrdinalIgnoreCase) ||
               fieldName.EndsWith(".Vertex2", StringComparison.OrdinalIgnoreCase))) ||
             (fieldName.StartsWith("NavmeshGeometry.CoverTriangleMappings[", StringComparison.OrdinalIgnoreCase) &&
              (fieldName.EndsWith(".Cover", StringComparison.OrdinalIgnoreCase) ||
               fieldName.EndsWith(".Triangle", StringComparison.OrdinalIgnoreCase)))))
        {
            return !spriggitFields.ContainsKey(fieldName);
        }

        if (fieldName.StartsWith("NavmeshGeometry.CoverTriangleMappings[", StringComparison.OrdinalIgnoreCase) &&
            fieldName.EndsWith(".Value", StringComparison.OrdinalIgnoreCase))
        {
            var spriggitPath = fieldName[..^".Value".Length];
            if (spriggitFields.TryGetValue(spriggitPath, out var spriggitValue))
            {
                return string.Equals(fieldValue, spriggitValue, StringComparison.Ordinal) ||
                       (string.Equals(spriggitValue, "{}", StringComparison.Ordinal) &&
                        string.Equals(fieldValue, "0, 0", StringComparison.Ordinal));
            }

            return spriggitFields.TryGetValue(spriggitPath + ".Cover", out var cover) &&
                   TryGetSpriggitValueOrDefault(spriggitFields, spriggitPath + ".Triangle", "0", out var triangle) &&
                   string.Equals(fieldValue, cover + ", " + triangle, StringComparison.Ordinal);
        }

        return false;
    }

    private static bool TryGetSpriggitValueOrDefault(
        IReadOnlyDictionary<string, string> spriggitFields,
        string fieldName,
        string defaultValue,
        out string fieldValue)
    {
        if (spriggitFields.TryGetValue(fieldName, out fieldValue!))
        {
            return true;
        }

        fieldValue = defaultValue;
        return true;
    }

    private static bool TryMatchStaticNavmeshAlternatePath(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> spriggitFields,
        string indexedPath,
        string flattenedPath,
        string indexedLeaf = "")
    {
        var alternatePath = GetStaticNavmeshAlternatePath(fieldName, indexedPath, flattenedPath, indexedLeaf);
        return alternatePath != null &&
               spriggitFields.TryGetValue(alternatePath, out var spriggitValue) &&
               string.Equals(fieldValue, spriggitValue, StringComparison.Ordinal);
    }

    private static string? GetStaticNavmeshAlternatePath(string fieldName, string indexedPath, string flattenedPath, string indexedLeaf)
    {
        if (fieldName.StartsWith(indexedPath, StringComparison.OrdinalIgnoreCase) &&
            fieldName.EndsWith(indexedLeaf, StringComparison.OrdinalIgnoreCase))
        {
            var fieldWithoutLeaf = string.IsNullOrEmpty(indexedLeaf)
                ? fieldName
                : fieldName[..^indexedLeaf.Length];
            return flattenedPath + fieldWithoutLeaf[indexedPath.Length..];
        }

        if (fieldName.StartsWith(flattenedPath, StringComparison.OrdinalIgnoreCase))
        {
            return indexedPath + fieldName[flattenedPath.Length..] + indexedLeaf;
        }

        return null;
    }

    private static bool IsDtoScriptFragmentProjectionBackedBySpriggitField(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> spriggitFields,
        IReadOnlyDictionary<string, string> dtoFields)
    {
        if (!spriggitFields.TryGetValue("VirtualMachineAdapter.ScriptFragments.Script.Name", out var fragmentScriptName))
        {
            return false;
        }

        if (string.Equals(fieldName, "ScriptFragments.Count", StringComparison.OrdinalIgnoreCase))
        {
            return int.TryParse(fieldValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var count) && count > 0;
        }

        if (string.Equals(fieldName, "VirtualMachineAdapter.Count", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(fieldName, "VirtualMachineAdapter.Scripts.Count", StringComparison.OrdinalIgnoreCase))
        {
            return string.Equals(fieldValue, "1", StringComparison.Ordinal);
        }

        if (string.Equals(fieldName, "VirtualMachineAdapter[0].Count", StringComparison.OrdinalIgnoreCase))
        {
            return spriggitFields.TryGetValue("VirtualMachineAdapter.ScriptFragments.Script.Properties.Count", out var propertyCount) &&
                   string.Equals(fieldValue, propertyCount, StringComparison.Ordinal);
        }

        if (fieldName.StartsWith("VirtualMachineAdapter.ScriptFragments.Script.Properties[", StringComparison.OrdinalIgnoreCase) &&
            ((fieldName.EndsWith(".Alias", StringComparison.OrdinalIgnoreCase) && string.Equals(fieldValue, "-1", StringComparison.Ordinal)) ||
             (fieldName.EndsWith(".Unused", StringComparison.OrdinalIgnoreCase) && IsZero(fieldValue))))
        {
            return true;
        }

        if (TryGetIndexedPath(fieldName, "ScriptFragments", out var fragmentRowIndex, out var fragmentRemainder))
        {
            var fragmentPath = "ScriptFragments[" + fragmentRowIndex.ToString(CultureInfo.InvariantCulture) + "]";
            if (fragmentRemainder is ".FragmentIndex" or ".FragmentSlot" or ".MutagenObjectType")
            {
                return true;
            }

            if (dtoFields.TryGetValue(fragmentPath + ".FragmentSlot", out var fragmentSlot) &&
                string.Equals(fragmentSlot, "ScriptFragments", StringComparison.Ordinal) &&
                string.Equals(fragmentRemainder, ".ExtraBindDataVersion", StringComparison.OrdinalIgnoreCase))
            {
                return (spriggitFields.TryGetValue("VirtualMachineAdapter.ScriptFragments.ExtraBindDataVersion", out var version) &&
                        string.Equals(fieldValue, version, StringComparison.Ordinal)) ||
                       string.Equals(fieldValue, "3", StringComparison.Ordinal);
            }

            if (dtoFields.TryGetValue(fragmentPath + ".FragmentSlot", out fragmentSlot) &&
                string.Equals(fragmentSlot, "ScriptFragments.Fragments", StringComparison.Ordinal) &&
                dtoFields.TryGetValue(fragmentPath + ".FragmentIndex", out var fragmentIndex))
            {
                var spriggitFragmentPath = "VirtualMachineAdapter.ScriptFragments.Fragments[" + fragmentIndex + "]";
                var spriggitPath = string.Equals(fragmentRemainder, ".SourceFragmentIndex", StringComparison.OrdinalIgnoreCase)
                    ? spriggitFragmentPath + ".FragmentIndex"
                    : spriggitFragmentPath + fragmentRemainder;
                return spriggitFields.TryGetValue(spriggitPath, out var spriggitValue) &&
                       string.Equals(fieldValue, spriggitValue, StringComparison.Ordinal);
            }

            if (dtoFields.TryGetValue(fragmentPath + ".FragmentSlot", out fragmentSlot) &&
                string.Equals(fragmentSlot, "ScriptFragments.Script", StringComparison.Ordinal) &&
                string.Equals(fragmentRemainder, ".ScriptName", StringComparison.OrdinalIgnoreCase))
            {
                return string.Equals(fieldValue, fragmentScriptName, StringComparison.Ordinal);
            }
        }

        if (TryGetFragmentScriptAdapterPath(dtoFields, fragmentScriptName, out var adapterPath) &&
            IsDtoFragmentScriptingAdapterField(fieldName, fieldValue, spriggitFields, adapterPath))
        {
            return true;
        }

        if (fieldName.StartsWith("VirtualMachineAdapter.Scripts[0]", StringComparison.OrdinalIgnoreCase))
        {
            var fragmentPath = "VirtualMachineAdapter.ScriptFragments.Script" + fieldName["VirtualMachineAdapter.Scripts[0]".Length..];
            return spriggitFields.TryGetValue(fragmentPath, out var spriggitValue) &&
                   string.Equals(fieldValue, spriggitValue, StringComparison.Ordinal);
        }

        if (fieldName.StartsWith("VirtualMachineAdapter[0]", StringComparison.OrdinalIgnoreCase))
        {
            var remainder = fieldName["VirtualMachineAdapter[0]".Length..];
            var fragmentPath = remainder.StartsWith("[", StringComparison.Ordinal)
                ? "VirtualMachineAdapter.ScriptFragments.Script.Properties" + remainder
                : "VirtualMachineAdapter.ScriptFragments.Script" + remainder;
            return spriggitFields.TryGetValue(fragmentPath, out var spriggitValue) &&
                   string.Equals(fieldValue, spriggitValue, StringComparison.Ordinal);
        }

        return false;
    }

    private static bool TryGetFragmentScriptAdapterPath(IReadOnlyDictionary<string, string> dtoFields, string fragmentScriptName, out string adapterPath)
    {
        adapterPath = string.Empty;
        if (!dtoFields.TryGetValue("ScriptingAdapters.Count", out var adapterCountValue) ||
            !int.TryParse(adapterCountValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var adapterCount))
        {
            return false;
        }

        for (var adapterIndex = 0; adapterIndex < adapterCount; adapterIndex++)
        {
            var candidatePath = "ScriptingAdapters[" + adapterIndex.ToString(CultureInfo.InvariantCulture) + "]";
            if (dtoFields.TryGetValue(candidatePath + ".Name", out var adapterName) &&
                string.Equals(adapterName, fragmentScriptName, StringComparison.Ordinal))
            {
                adapterPath = candidatePath;
                return true;
            }
        }

        return false;
    }

    private static bool IsDtoFragmentScriptingAdapterField(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> spriggitFields,
        string adapterPath)
    {
        if (string.Equals(fieldName, "ScriptingAdapters.Count", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(fieldName, adapterPath + ".Name", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (!fieldName.StartsWith(adapterPath + ".Properties", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (fieldName.EndsWith(".ScriptingAdapterName", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if ((fieldName.EndsWith(".ObjectAlias", StringComparison.OrdinalIgnoreCase) && string.Equals(fieldValue, "-1", StringComparison.Ordinal)) ||
            (fieldName.EndsWith(".ObjectUnused", StringComparison.OrdinalIgnoreCase) && IsZero(fieldValue)))
        {
            return true;
        }

        var spriggitPath = "VirtualMachineAdapter.ScriptFragments.Script.Properties" + fieldName[(adapterPath + ".Properties").Length..]
            .Replace(".ObjectFormKey", ".Object", StringComparison.Ordinal);
        return spriggitFields.TryGetValue(spriggitPath, out var spriggitValue) &&
               string.Equals(fieldValue, spriggitValue, StringComparison.Ordinal);
    }

    private static bool IsDtoModelMaterialSwapBackedBySpriggitScalar(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> spriggitFields)
    {
        if ((string.Equals(fieldName, "Model.MaterialSwaps.Count", StringComparison.OrdinalIgnoreCase) ||
             string.Equals(fieldName, "Models[0].MaterialSwaps.Count", StringComparison.OrdinalIgnoreCase)) &&
            string.Equals(fieldValue, "1", StringComparison.Ordinal))
        {
            return spriggitFields.ContainsKey("Model.MaterialSwap") ||
                   spriggitFields.ContainsKey("Model.MaterialSwaps[0]") ||
                   spriggitFields.ContainsKey("Models[0].MaterialSwaps[0].MaterialSwapFormKey");
        }

        if ((string.Equals(fieldName, "Model.MaterialSwap", StringComparison.OrdinalIgnoreCase) ||
             string.Equals(fieldName, "Model.MaterialSwaps[0]", StringComparison.OrdinalIgnoreCase) ||
             string.Equals(fieldName, "Model[0].NewTexture", StringComparison.OrdinalIgnoreCase) ||
             string.Equals(fieldName, "Model[1]", StringComparison.OrdinalIgnoreCase) ||
             IsModelMaterialSwapFormIdAlias(fieldName) ||
             string.Equals(fieldName, "Models[0].MaterialSwaps[0].MaterialSwapFormKey", StringComparison.OrdinalIgnoreCase)) &&
            IsSpriggitMaterialSwapValue(fieldValue, spriggitFields))
        {
            return true;
        }

        if (string.Equals(fieldName, "Models[0].MaterialSwaps[0].Name", StringComparison.OrdinalIgnoreCase))
        {
            return spriggitFields.TryGetValue("Models[0].MaterialSwaps[0].Name", out var name) &&
                   string.Equals(fieldValue, name, StringComparison.Ordinal);
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

    private static bool IsModelMaterialSwapFormIdAlias(string fieldName)
    {
        if (!fieldName.StartsWith("Model[0].", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var formId = fieldName["Model[0].".Length..];
        return formId.Length == 6 &&
               formId.All(character => Uri.IsHexDigit(character));
    }

    private static bool IsSpriggitMaterialSwapValue(string fieldValue, IReadOnlyDictionary<string, string> spriggitFields)
    {
        return (spriggitFields.TryGetValue("Model.MaterialSwap", out var materialSwap) &&
                string.Equals(fieldValue, materialSwap, StringComparison.Ordinal)) ||
               (spriggitFields.TryGetValue("Model.MaterialSwaps[0]", out var materialSwapListItem) &&
                string.Equals(fieldValue, materialSwapListItem, StringComparison.Ordinal)) ||
               (spriggitFields.TryGetValue("Model[0].NewTexture", out var newTexture) &&
                string.Equals(fieldValue, newTexture, StringComparison.Ordinal)) ||
               (spriggitFields.TryGetValue("Models[0].MaterialSwaps[0].MaterialSwapFormKey", out var materialSwapFormKey) &&
                string.Equals(fieldValue, materialSwapFormKey, StringComparison.Ordinal));
    }

    private static bool IsDtoResourceMetadataBackedBySpriggitField(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> spriggitFields)
    {
        return fieldName.StartsWith("Resources[", StringComparison.OrdinalIgnoreCase) &&
               fieldName.EndsWith("].ResourceIndex", StringComparison.OrdinalIgnoreCase) &&
               TryGetIndexedPath(fieldName, "Resources", out var resourceIndex, out _) &&
               string.Equals(fieldValue, resourceIndex.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal) &&
               HasSpriggitPath(spriggitFields, "Resources[" + resourceIndex.ToString(CultureInfo.InvariantCulture) + "]");
    }

    private static bool IsDtoMiscComponentDisplayIndexBackedBySpriggitField(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> spriggitFields)
    {
        return TryGetIndexedPath(fieldName, "Components", out var componentIndex, out var componentRemainder) &&
               string.Equals(componentRemainder, ".DisplayIndex", StringComparison.OrdinalIgnoreCase) &&
               spriggitFields.TryGetValue("ComponentDisplayIndices[" + componentIndex.ToString(CultureInfo.InvariantCulture) + "]", out var displayIndex) &&
               AreEquivalentSpriggitValues(fieldValue, displayIndex);
    }

    private static bool IsDtoDestructibleFieldBackedBySpriggitField(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> spriggitFields)
    {
        if (!TryGetDestructibleStagePath(fieldName, out var stageIndex, out var stageRemainder))
        {
            return false;
        }

        var spriggitStagePath = "Destructible.Stages[" + stageIndex.ToString(CultureInfo.InvariantCulture) + "]";
        if (string.Equals(stageRemainder, ".StageIndex", StringComparison.OrdinalIgnoreCase))
        {
            return string.Equals(fieldValue, stageIndex.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal) &&
                   HasSpriggitPath(spriggitFields, spriggitStagePath);
        }

        if (string.Equals(stageRemainder, ".Flags", StringComparison.OrdinalIgnoreCase))
        {
            return IsDtoScalarValueBackedBySpriggitList(fieldValue, spriggitFields, spriggitStagePath + ".Flags") ||
                   (IsZero(fieldValue) && HasSpriggitPath(spriggitFields, spriggitStagePath));
        }

        if (spriggitFields.TryGetValue(spriggitStagePath + stageRemainder, out var spriggitValue))
        {
            return AreEquivalentSpriggitValues(fieldValue, spriggitValue);
        }

        return IsDefaultDestructibleStageValue(stageRemainder, fieldValue) &&
               HasSpriggitPath(spriggitFields, spriggitStagePath);
    }

    private static bool IsSpriggitDestructibleFieldBackedByDtoField(
        string fieldName,
        IReadOnlyDictionary<string, string> spriggitFields,
        IReadOnlyDictionary<string, string> dtoFields)
    {
        if (!TryGetDestructibleStagePath(fieldName, out var stageIndex, out var stageRemainder))
        {
            return false;
        }

        var dtoStagePath = "Destructible.Stages[" + stageIndex.ToString(CultureInfo.InvariantCulture) + "]";
        if (stageRemainder.StartsWith(".Flags[", StringComparison.OrdinalIgnoreCase) &&
            dtoFields.TryGetValue(dtoStagePath + ".Flags", out var dtoFlags))
        {
            return IsDtoScalarValueBackedBySpriggitList(
                dtoFlags,
                spriggitFields,
                "Destructible.Stages[" + stageIndex.ToString(CultureInfo.InvariantCulture) + "].Flags");
        }

        if (stageRemainder.StartsWith("[", StringComparison.OrdinalIgnoreCase) &&
            dtoFields.TryGetValue(dtoStagePath + ".Flags", out dtoFlags))
        {
            return IsDtoScalarValueBackedBySpriggitList(
                dtoFlags,
                spriggitFields,
                "Destructible.Stages[" + stageIndex.ToString(CultureInfo.InvariantCulture) + "].Flags");
        }

        if ((string.Equals(stageRemainder, ".Flags.Count", StringComparison.OrdinalIgnoreCase) ||
             string.Equals(stageRemainder, ".Count", StringComparison.OrdinalIgnoreCase)) &&
            dtoFields.ContainsKey(dtoStagePath + ".Flags"))
        {
            return GetListValues(
                spriggitFields,
                "Destructible.Stages[" + stageIndex.ToString(CultureInfo.InvariantCulture) + "].Flags").Count > 0;
        }

        return dtoFields.ContainsKey(dtoStagePath + stageRemainder);
    }

    private static bool IsDefaultDestructibleStageValue(string stageRemainder, string fieldValue)
    {
        return (string.Equals(stageRemainder, ".Index", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(stageRemainder, ".HealthPercent", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(stageRemainder, ".SelfDamagePerSecond", StringComparison.OrdinalIgnoreCase)) &&
               IsZero(fieldValue);
    }

    private static bool IsDtoScalarValueBackedBySpriggitList(
        string fieldValue,
        IReadOnlyDictionary<string, string> spriggitFields,
        string spriggitListPath)
    {
        return GetListValues(spriggitFields, spriggitListPath)
            .Any(value => AreEquivalentSpriggitValues(fieldValue, value));
    }

    private static bool TryGetDestructibleStagePath(string fieldName, out int stageIndex, out string stageRemainder)
    {
        return TryGetIndexedPath(fieldName, "Destructible.Stages", out stageIndex, out stageRemainder) ||
               TryGetIndexedPath(fieldName, "Destructible", out stageIndex, out stageRemainder);
    }

    private static bool IsDtoActorValueInformationFieldBackedBySpriggitField(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> spriggitFields)
    {
        if (string.Equals(fieldName, "CNAM", StringComparison.OrdinalIgnoreCase))
        {
            return spriggitFields.TryGetValue("CNAM", out var value) &&
                   string.Equals(fieldValue, value, StringComparison.Ordinal);
        }

        var scalarFieldName = fieldName switch
        {
            "Skill.ImproveMult" => "Skill.ImproveMult",
            "Skill.ImproveOffset" => "Skill.ImproveOffset",
            "Skill.UseMult" => "Skill.UseMult",
            _ => string.Empty
        };
        if (!string.IsNullOrEmpty(scalarFieldName))
        {
            return spriggitFields.TryGetValue(scalarFieldName, out var value) &&
                   AreEquivalentSpriggitValues(fieldValue, value);
        }

        if (fieldName.StartsWith("PerkTree[", StringComparison.OrdinalIgnoreCase) &&
            TryGetIndexedPath(fieldName, "PerkTree", out var perkTreeIndex, out var perkTreeRemainder))
        {
            if (string.Equals(perkTreeRemainder, ".PerkTreeIndex", StringComparison.OrdinalIgnoreCase))
            {
                return string.Equals(fieldValue, perkTreeIndex.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);
            }

            var spriggitFieldName = perkTreeRemainder switch
            {
                ".AssociatedSkill" => "PerkTree[" + perkTreeIndex.ToString(CultureInfo.InvariantCulture) + "].AssociatedSkill",
                ".FNAM" => "PerkTree[" + perkTreeIndex.ToString(CultureInfo.InvariantCulture) + "].FNAM",
                ".HorizontalPosition" => "PerkTree[" + perkTreeIndex.ToString(CultureInfo.InvariantCulture) + "].HorizontalPosition",
                ".Index" => "PerkTree[" + perkTreeIndex.ToString(CultureInfo.InvariantCulture) + "].Index",
                ".PerkGridX" => "PerkTree[" + perkTreeIndex.ToString(CultureInfo.InvariantCulture) + "].PerkGridX",
                ".PerkGridY" => "PerkTree[" + perkTreeIndex.ToString(CultureInfo.InvariantCulture) + "].PerkGridY",
                ".VerticalPosition" => "PerkTree[" + perkTreeIndex.ToString(CultureInfo.InvariantCulture) + "].VerticalPosition",
                ".Perk" => "PerkTree[" + perkTreeIndex.ToString(CultureInfo.InvariantCulture) + "].Perk",
                _ => string.Empty
            };
            if (!string.IsNullOrEmpty(spriggitFieldName))
            {
                return spriggitFields.TryGetValue(spriggitFieldName, out var value) &&
                       AreEquivalentSpriggitValues(fieldValue, value);
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
            return dtoFields.ContainsKey("PerkTree[" + index + "].FNAM");
        }

        var dtoFieldName = perkTreeRemainder switch
        {
            ".AssociatedSkill" => "PerkTree[" + perkTreeIndex.ToString(CultureInfo.InvariantCulture) + "].AssociatedSkill",
            ".HorizontalPosition" => "PerkTree[" + perkTreeIndex.ToString(CultureInfo.InvariantCulture) + "].HorizontalPosition",
            ".Index" => "PerkTree[" + perkTreeIndex.ToString(CultureInfo.InvariantCulture) + "].Index",
            ".PerkGridX" => "PerkTree[" + perkTreeIndex.ToString(CultureInfo.InvariantCulture) + "].PerkGridX",
            ".PerkGridY" => "PerkTree[" + perkTreeIndex.ToString(CultureInfo.InvariantCulture) + "].PerkGridY",
            ".VerticalPosition" => "PerkTree[" + perkTreeIndex.ToString(CultureInfo.InvariantCulture) + "].VerticalPosition",
            ".FNAM" => "PerkTree[" + perkTreeIndex.ToString(CultureInfo.InvariantCulture) + "].FNAM",
            _ => string.Empty
        };
        if (!string.IsNullOrEmpty(dtoFieldName))
        {
            return spriggitFields.TryGetValue(fieldName, out var spriggitValue) &&
                   dtoFields.TryGetValue(dtoFieldName, out var dtoValue) &&
                   AreEquivalentSpriggitValues(dtoValue, spriggitValue);
        }

        if (string.Equals(perkTreeRemainder, ".Perk", StringComparison.OrdinalIgnoreCase))
        {
            return dtoFields.ContainsKey("PerkTree[" + perkTreeIndex.ToString(CultureInfo.InvariantCulture) + "].Perk");
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

    private static bool IsDtoCollectionMetadataField(string fieldName)
    {
        return fieldName.Contains('[', StringComparison.Ordinal) &&
               (fieldName.EndsWith(".RankIndex", StringComparison.OrdinalIgnoreCase) ||
                fieldName.EndsWith(".ActivityIndex", StringComparison.OrdinalIgnoreCase) ||
                fieldName.EndsWith(".EvaluatorIndex", StringComparison.OrdinalIgnoreCase) ||
                fieldName.EndsWith(".EffectIndex", StringComparison.OrdinalIgnoreCase) ||
                fieldName.EndsWith(".PropertyIndex", StringComparison.OrdinalIgnoreCase) ||
                fieldName.EndsWith(".SkillIndex", StringComparison.OrdinalIgnoreCase) ||
                fieldName.EndsWith(".ConditionTabIndex", StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Determines whether a DTO field stores only the collection index already encoded by its flattened path.
    /// </summary>
    /// <param name="fieldName">The flattened DTO field path being evaluated.</param>
    /// <param name="fieldValue">The flattened DTO field value.</param>
    /// <returns>
    /// <c>true</c> when the field value repeats an indexed collection position used for deterministic persistence ordering.
    /// </returns>
    private static bool IsDtoCollectionIndexFieldBackedByPathIndex(string fieldName, string fieldValue)
    {
        if (!int.TryParse(fieldValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
        {
            return false;
        }

        if (fieldName.EndsWith(".StructIndex", StringComparison.OrdinalIgnoreCase) &&
            fieldName.Contains(".Members[", StringComparison.OrdinalIgnoreCase))
        {
            var memberPathIndex = fieldName.IndexOf(".Members[", StringComparison.OrdinalIgnoreCase);
            return TryGetLastIndexedPathIndex(fieldName[..memberPathIndex], out var structIndex) &&
                   value == structIndex;
        }

        return (fieldName.EndsWith(".GridArrayIndex", StringComparison.OrdinalIgnoreCase) ||
                fieldName.EndsWith(".TriangleIndex", StringComparison.OrdinalIgnoreCase) ||
                fieldName.EndsWith(".VertexIndex", StringComparison.OrdinalIgnoreCase) ||
                fieldName.EndsWith(".StructIndex", StringComparison.OrdinalIgnoreCase) ||
                fieldName.EndsWith(".MemberIndex", StringComparison.OrdinalIgnoreCase)) &&
               TryGetLastIndexedPathIndex(fieldName, out var pathIndex) &&
               value == pathIndex;
    }

    /// <summary>
    /// Gets the final bracketed collection index from a flattened DTO path.
    /// </summary>
    /// <param name="path">The flattened DTO path to inspect.</param>
    /// <param name="index">The parsed final collection index, or zero when parsing fails.</param>
    /// <returns><c>true</c> when the path contains a final bracketed integer index.</returns>
    private static bool TryGetLastIndexedPathIndex(string path, out int index)
    {
        index = 0;
        var end = path.LastIndexOf(']');
        if (end < 0)
        {
            return false;
        }

        var start = path.LastIndexOf('[', end);
        return start >= 0 &&
               start < end &&
               int.TryParse(path[(start + 1)..end], NumberStyles.Integer, CultureInfo.InvariantCulture, out index);
    }

    private static bool IsCommonMetadataFieldOutsideRepositoryReadback(string fieldName, IReadOnlyDictionary<string, string> dtoFields)
    {
        if (string.Equals(fieldName, "MajorFlags.Count", StringComparison.OrdinalIgnoreCase) ||
            fieldName.StartsWith("MajorFlags[", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(fieldName, "StarfieldMajorRecordFlags.Count", StringComparison.OrdinalIgnoreCase) ||
            fieldName.StartsWith("StarfieldMajorRecordFlags[", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(fieldName, "Fallout4MajorRecordFlags.Count", StringComparison.OrdinalIgnoreCase) ||
            fieldName.StartsWith("Fallout4MajorRecordFlags[", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(fieldName, "SkyrimMajorRecordFlags.Count", StringComparison.OrdinalIgnoreCase) ||
            fieldName.StartsWith("SkyrimMajorRecordFlags[", StringComparison.OrdinalIgnoreCase))
        {
            return dtoFields.ContainsKey("MajorRecordFlags");
        }

        return (string.Equals(fieldName, "Version2", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(fieldName, "VersionControl", StringComparison.OrdinalIgnoreCase)) &&
               !dtoFields.ContainsKey(fieldName);
    }

    private static bool IsGlobalMetadataFieldOutsideRepositoryReadback(string fieldName)
    {
        return string.Equals(fieldName, "MutagenObjectType", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(fieldName, "MajorFlags.Count", StringComparison.OrdinalIgnoreCase) ||
               fieldName.StartsWith("MajorFlags[", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(fieldName, "StarfieldMajorRecordFlags.Count", StringComparison.OrdinalIgnoreCase) ||
               fieldName.StartsWith("StarfieldMajorRecordFlags[", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(fieldName, "Fallout4MajorRecordFlags.Count", StringComparison.OrdinalIgnoreCase) ||
               fieldName.StartsWith("Fallout4MajorRecordFlags[", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(fieldName, "SkyrimMajorRecordFlags.Count", StringComparison.OrdinalIgnoreCase) ||
               fieldName.StartsWith("SkyrimMajorRecordFlags[", StringComparison.OrdinalIgnoreCase);
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

    /// <summary>
    /// Determines whether a Spriggit keyword component field is preserved by scalar keyword DTO fields.
    /// </summary>
    /// <param name="fieldName">The flattened Spriggit field path being evaluated.</param>
    /// <param name="spriggitFields">All flattened Spriggit fields for the record.</param>
    /// <param name="dtoFields">All flattened DTO fields for the record.</param>
    /// <returns><c>true</c> when the component field is represented by an equivalent DTO scalar or keyword row.</returns>
    private static bool IsSpriggitKeywordComponentFieldBackedByDtoScalar(
        string fieldName,
        IReadOnlyDictionary<string, string> spriggitFields,
        IReadOnlyDictionary<string, string> dtoFields)
    {
        if (!fieldName.StartsWith("Components", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (string.Equals(fieldName, "Components.Count", StringComparison.OrdinalIgnoreCase))
        {
            return spriggitFields.Keys.Any(field =>
                field.StartsWith("Components[", StringComparison.OrdinalIgnoreCase) &&
                (field.EndsWith(".WAIM", StringComparison.OrdinalIgnoreCase) ||
                 field.EndsWith(".WFIR", StringComparison.OrdinalIgnoreCase))) ||
                 spriggitFields.Keys.Any(field =>
                     field.StartsWith("Components[", StringComparison.OrdinalIgnoreCase) &&
                     field.Contains(".Keywords[", StringComparison.OrdinalIgnoreCase));
        }

        if (!TryGetIndexedPath(fieldName, "Components", out _, out var remainder))
        {
            return false;
        }

        if (string.IsNullOrEmpty(remainder) || string.Equals(remainder, ".MutagenObjectType", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (string.Equals(remainder, ".Keywords.Count", StringComparison.OrdinalIgnoreCase))
        {
            return dtoFields.ContainsKey("Keywords.Count");
        }

        if (string.Equals(remainder, ".WAIM", StringComparison.OrdinalIgnoreCase))
        {
            return dtoFields.TryGetValue("WAIM", out var dtoValue) &&
                   spriggitFields.TryGetValue(fieldName, out var spriggitValue) &&
                   HexValuesMatch(dtoValue, spriggitValue);
        }

        if (string.Equals(remainder, ".WFIR", StringComparison.OrdinalIgnoreCase))
        {
            return dtoFields.TryGetValue("WFIR", out var dtoValue) &&
                   spriggitFields.TryGetValue(fieldName, out var spriggitValue) &&
                   HexValuesMatch(dtoValue, spriggitValue);
        }

        return IsSpriggitComponentKeywordBackedByDtoKeyword(remainder, fieldName, spriggitFields, dtoFields);
    }

    /// <summary>
    /// Determines whether a Spriggit component <c>REFL</c> field is represented by a first-class reflection DTO row.
    /// </summary>
    /// <param name="fieldName">The flattened Spriggit field path being evaluated.</param>
    /// <param name="fieldValue">The flattened Spriggit field value.</param>
    /// <param name="spriggitFields">All flattened Spriggit fields for the record.</param>
    /// <param name="dtoFields">All flattened DTO fields for the record.</param>
    /// <returns><c>true</c> when the Spriggit field is covered by reflection DTO data.</returns>
    private static bool IsSpriggitReflectionFieldBackedByDtoReflection(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> spriggitFields,
        IReadOnlyDictionary<string, string> dtoFields)
    {
        if (string.Equals(fieldName, "Components.Count", StringComparison.OrdinalIgnoreCase))
        {
            return GetComponentReflectionPaths(spriggitFields).Count > 0 &&
                   dtoFields.TryGetValue("Reflections.Count", out var reflectionCount) &&
                   !IsZero(reflectionCount);
        }

        if (!TryGetIndexedPath(fieldName, "Components", out var componentIndex, out var remainder) ||
            (!string.Equals(remainder, ".REFL", StringComparison.OrdinalIgnoreCase) &&
             !string.Equals(remainder, ".MutagenObjectType", StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        var sourcePath = "Components[" + componentIndex.ToString(CultureInfo.InvariantCulture) + "].REFL";
        var reflectionIndex = GetComponentReflectionPaths(spriggitFields).IndexOf(sourcePath);
        if (reflectionIndex < 0)
        {
            return false;
        }

        var dtoPath = "Reflections[" + reflectionIndex.ToString(CultureInfo.InvariantCulture) + "]";
        if (string.Equals(remainder, ".REFL", StringComparison.OrdinalIgnoreCase))
        {
            return dtoFields.TryGetValue(dtoPath + ".REFL", out var dtoValue) &&
                   string.Equals(NormalizeHexPrefix(fieldValue), dtoValue, StringComparison.Ordinal);
        }

        return dtoFields.TryGetValue(dtoPath + ".ComponentType", out var componentType) &&
               IsSameTypeName(fieldValue, componentType);
    }

    private static bool IsSpriggitComponentKeywordBackedByDtoKeyword(
        string componentRemainder,
        string fieldName,
        IReadOnlyDictionary<string, string> spriggitFields,
        IReadOnlyDictionary<string, string> dtoFields)
    {
        const string keywordPrefix = ".Keywords[";
        if (!componentRemainder.StartsWith(keywordPrefix, StringComparison.OrdinalIgnoreCase) ||
            !spriggitFields.TryGetValue(fieldName, out var modName))
        {
            return false;
        }

        var formKeySeparator = componentRemainder.LastIndexOf('.');
        if (formKeySeparator < 0)
        {
            return false;
        }

        var formKey = componentRemainder[(formKeySeparator + 1)..] + ":" + modName;
        return dtoFields
            .Where(field => field.Key.StartsWith("Keywords[", StringComparison.OrdinalIgnoreCase) &&
                            field.Key.EndsWith("].Keyword", StringComparison.OrdinalIgnoreCase))
            .Any(field => string.Equals(field.Value, formKey, StringComparison.Ordinal));
    }

    private static bool IsDtoKeywordBackedBySpriggitComponentKeyword(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> spriggitFields,
        IReadOnlyDictionary<string, string> dtoFields)
    {
        if (!fieldName.StartsWith("Keywords", StringComparison.OrdinalIgnoreCase) ||
            !dtoFields.TryGetValue("Keywords.Count", out var keywordCount))
        {
            return false;
        }

        var componentKeywordFields = GetComponentKeywordFormKeys(spriggitFields).ToList();
        if (componentKeywordFields.Count == 0)
        {
            return false;
        }

        if (string.Equals(fieldName, "Keywords.Count", StringComparison.OrdinalIgnoreCase))
        {
            return string.Equals(fieldValue, componentKeywordFields.Count.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal) &&
                   string.Equals(keywordCount, fieldValue, StringComparison.Ordinal);
        }

        if (!TryGetIndexedPath(fieldName, "Keywords", out var keywordIndex, out var remainder) ||
            keywordIndex >= componentKeywordFields.Count)
        {
            return false;
        }

        var formKey = componentKeywordFields[keywordIndex];
        if (string.IsNullOrEmpty(remainder))
        {
            return string.Equals(fieldValue, formKey, StringComparison.Ordinal);
        }

        if (string.Equals(remainder, ".Keyword", StringComparison.OrdinalIgnoreCase))
        {
            return string.Equals(fieldValue, formKey, StringComparison.Ordinal);
        }

        return string.Equals(remainder, ".KeywordIndex", StringComparison.OrdinalIgnoreCase) &&
               string.Equals(fieldValue, keywordIndex.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);
    }

    /// <summary>
    /// Determines whether a flattened reflection DTO field is backed by a Spriggit component <c>REFL</c> field.
    /// </summary>
    /// <param name="fieldName">The flattened DTO field path being evaluated.</param>
    /// <param name="fieldValue">The flattened DTO field value.</param>
    /// <param name="spriggitFields">All flattened Spriggit fields for the record.</param>
    /// <param name="dtoFields">All flattened DTO fields for the record.</param>
    /// <returns><c>true</c> when the DTO field is covered by Spriggit component reflection data.</returns>
    private static bool IsDtoReflectionFieldBackedBySpriggitComponent(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> spriggitFields,
        IReadOnlyDictionary<string, string> dtoFields)
    {
        var reflectionPaths = GetComponentReflectionPaths(spriggitFields);
        if (reflectionPaths.Count == 0)
        {
            return false;
        }

        if (string.Equals(fieldName, "Reflections.Count", StringComparison.OrdinalIgnoreCase))
        {
            return string.Equals(fieldValue, reflectionPaths.Count.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);
        }

        if (!TryGetIndexedPath(fieldName, "Reflections", out var reflectionIndex, out var remainder) ||
            reflectionIndex < 0 ||
            reflectionIndex >= reflectionPaths.Count)
        {
            return false;
        }

        var sourcePath = dtoFields.TryGetValue("Reflections[" + reflectionIndex.ToString(CultureInfo.InvariantCulture) + "].SourcePath", out var dtoSourcePath)
            ? dtoSourcePath
            : reflectionPaths[reflectionIndex];

        if (string.Equals(remainder, ".SourcePath", StringComparison.OrdinalIgnoreCase))
        {
            return string.Equals(fieldValue, reflectionPaths[reflectionIndex], StringComparison.Ordinal);
        }

        if (string.Equals(remainder, ".ComponentIndex", StringComparison.OrdinalIgnoreCase))
        {
            return TryGetIndexedPath(sourcePath, "Components", out var componentIndex, out _) &&
                   string.Equals(fieldValue, componentIndex.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);
        }

        if (string.Equals(remainder, ".REFL", StringComparison.OrdinalIgnoreCase))
        {
            return spriggitFields.TryGetValue(sourcePath, out var spriggitValue) &&
                   string.Equals(fieldValue, NormalizeHexPrefix(spriggitValue), StringComparison.Ordinal);
        }

        if (string.Equals(remainder, ".ComponentType", StringComparison.OrdinalIgnoreCase))
        {
            var componentTypePath = sourcePath[..^".REFL".Length] + ".MutagenObjectType";
            return spriggitFields.TryGetValue(componentTypePath, out var spriggitType) &&
                   IsSameTypeName(spriggitType, fieldValue);
        }

        return false;
    }

    /// <summary>
    /// Gets the ordered Spriggit component paths that contain <c>REFL</c> fields.
    /// </summary>
    /// <param name="spriggitFields">All flattened Spriggit fields for the record.</param>
    /// <returns>The ordered <c>Components[n].REFL</c> paths.</returns>
    private static List<string> GetComponentReflectionPaths(IReadOnlyDictionary<string, string> spriggitFields)
    {
        return spriggitFields.Keys
            .Where(field => TryGetIndexedPath(field, "Components", out _, out var remainder) &&
                string.Equals(remainder, ".REFL", StringComparison.OrdinalIgnoreCase))
            .OrderBy(field =>
            {
                TryGetIndexedPath(field, "Components", out var componentIndex, out _);
                return componentIndex;
            })
            .ToList();
    }

    private static IEnumerable<string> GetComponentKeywordFormKeys(IReadOnlyDictionary<string, string> spriggitFields)
    {
        return spriggitFields
            .Where(field => TryGetComponentKeywordFormKey(field.Key, field.Value, out _))
            .OrderBy(field =>
            {
                TryGetIndexedPath(field.Key, "Components", out var componentIndex, out var componentRemainder);
                TryGetIndexedPath("Keywords" + componentRemainder[".Keywords".Length..], "Keywords", out var keywordIndex, out _);
                return componentIndex * 1000 + keywordIndex;
            })
            .Select(field =>
            {
                TryGetComponentKeywordFormKey(field.Key, field.Value, out var formKey);
                return formKey;
            });
    }

    private static bool TryGetComponentKeywordFormKey(string fieldName, string fieldValue, out string formKey)
    {
        formKey = string.Empty;
        if (!TryGetIndexedPath(fieldName, "Components", out _, out var componentRemainder) ||
            !componentRemainder.StartsWith(".Keywords[", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var separatorIndex = componentRemainder.LastIndexOf('.');
        if (separatorIndex < 0)
        {
            return false;
        }

        var formId = componentRemainder[(separatorIndex + 1)..];
        if (formId.Contains('.', StringComparison.Ordinal) ||
            formId.Contains('[', StringComparison.Ordinal) ||
            formId.Contains(']', StringComparison.Ordinal))
        {
            return false;
        }

        formKey = formId + ":" + fieldValue;
        return true;
    }

    private static bool IsSpriggitModelMaterialSwapFieldBackedByDtoField(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> dtoFields)
    {
        if (string.Equals(fieldName, "Model.MaterialSwaps.Count", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(fieldName, "Model.AlternateTextures.Count", StringComparison.OrdinalIgnoreCase))
        {
            return (dtoFields.TryGetValue("Models[0].MaterialSwaps.Count", out var nestedCount) &&
                    string.Equals(fieldValue, nestedCount, StringComparison.Ordinal)) ||
                   (dtoFields.TryGetValue("Model.MaterialSwaps.Count", out var count) &&
                    string.Equals(fieldValue, count, StringComparison.Ordinal));
        }

        if (TryGetIndexedPath(fieldName, "Model.MaterialSwaps", out var materialSwapIndex, out var materialSwapRemainder) &&
            !string.IsNullOrEmpty(materialSwapRemainder))
        {
            var formKeyId = materialSwapRemainder[1..];
            if (formKeyId.Contains('.', StringComparison.Ordinal) ||
                formKeyId.Contains('[', StringComparison.Ordinal) ||
                formKeyId.Contains(']', StringComparison.Ordinal))
            {
                return false;
            }

            var formKey = formKeyId + ":" + fieldValue;
            return IsDtoMaterialSwapFormKey(materialSwapIndex, formKey, dtoFields);
        }

        if (!TryGetIndexedPath(fieldName, "Model.AlternateTextures", out materialSwapIndex, out materialSwapRemainder))
        {
            return false;
        }

        var dtoPath = "Models[0].MaterialSwaps[" + materialSwapIndex.ToString(CultureInfo.InvariantCulture) + "]";
        if (string.Equals(materialSwapRemainder, ".Name", StringComparison.OrdinalIgnoreCase))
        {
            return dtoFields.TryGetValue(dtoPath + ".Name", out var name) &&
                   string.Equals(fieldValue, name, StringComparison.Ordinal);
        }

        if (string.Equals(materialSwapRemainder, ".NewTexture", StringComparison.OrdinalIgnoreCase))
        {
            return IsDtoMaterialSwapFormKey(materialSwapIndex, fieldValue, dtoFields);
        }

        return false;
    }

    private static bool IsSpriggitResourceInlineObjectBackedByDtoField(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> dtoFields)
    {
        if (!TryGetIndexedPath(fieldName, "Resources", out var resourceIndex, out var resourceRemainder) ||
            !string.IsNullOrEmpty(resourceRemainder) ||
            !fieldValue.StartsWith("Resource: ", StringComparison.Ordinal))
        {
            return false;
        }

        return dtoFields.TryGetValue("Resources[" + resourceIndex.ToString(CultureInfo.InvariantCulture) + "].Resource", out var resource) &&
               string.Equals(fieldValue["Resource: ".Length..], resource, StringComparison.Ordinal);
    }

    private static bool IsDtoMaterialSwapFormKey(
        int materialSwapIndex,
        string formKey,
        IReadOnlyDictionary<string, string> dtoFields)
    {
        var dtoPath = "Models[0].MaterialSwaps[" + materialSwapIndex.ToString(CultureInfo.InvariantCulture) + "]";
        return (dtoFields.TryGetValue(dtoPath + ".MaterialSwapFormKey", out var nestedFormKey) &&
                string.Equals(formKey, nestedFormKey, StringComparison.Ordinal)) ||
               (dtoFields.TryGetValue("Model.MaterialSwaps[" + materialSwapIndex.ToString(CultureInfo.InvariantCulture) + "]", out var flatFormKey) &&
                string.Equals(formKey, flatFormKey, StringComparison.Ordinal));
    }

    /// <summary>
    /// Compares two hex payload strings while allowing either side to include the Spriggit <c>0x</c> prefix.
    /// </summary>
    /// <param name="left">The first hex payload.</param>
    /// <param name="right">The second hex payload.</param>
    /// <returns><c>true</c> when both payloads contain the same hex digits after prefix normalization.</returns>
    private static bool HexValuesMatch(string left, string right)
    {
        return string.Equals(NormalizeHexPrefix(left), NormalizeHexPrefix(right), StringComparison.Ordinal);
    }

    /// <summary>
    /// Removes the optional Spriggit <c>0x</c> prefix from a hex payload.
    /// </summary>
    /// <param name="value">The hex payload text.</param>
    /// <returns>The payload text without a leading <c>0x</c> prefix.</returns>
    private static string NormalizeHexPrefix(string value)
    {
        return value.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
            ? value[2..]
            : value;
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

        if (string.Equals(fieldValue, "Null", StringComparison.OrdinalIgnoreCase) ||
            string.IsNullOrWhiteSpace(fieldValue))
        {
            return true;
        }

        if (fieldName.EndsWith(".ConditionCount", StringComparison.OrdinalIgnoreCase))
        {
            var conditionsCountPath = fieldName[..^".ConditionCount".Length] + ".Conditions.Count";
            if (spriggitFields.ContainsKey(conditionsCountPath))
            {
                return true;
            }
        }

        if (fieldName.EndsWith(".Data.MaleFemaleGender", StringComparison.OrdinalIgnoreCase) &&
            string.Equals(fieldValue, "Male", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (fieldName.EndsWith(".ActivityCount", StringComparison.OrdinalIgnoreCase))
        {
            var activitiesCountPath = fieldName[..^".ActivityCount".Length] + ".Activities.Count";
            if (spriggitFields.ContainsKey(activitiesCountPath))
            {
                return true;
            }
        }

        if ((fieldName.EndsWith(".Data.FirstParameter", StringComparison.OrdinalIgnoreCase) ||
             fieldName.EndsWith(".Data.SecondParameter", StringComparison.OrdinalIgnoreCase)) &&
            string.Equals(fieldValue, "None", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (fieldName.EndsWith(".Modification", StringComparison.OrdinalIgnoreCase) &&
            (string.Equals(fieldValue, "AddAVMult", StringComparison.OrdinalIgnoreCase) ||
             string.Equals(fieldValue, "Set", StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        return (fieldName is "Value" or "Weight" or "DirtinessScale" or "LeafAmplitude" or "LeafFrequency" or "DNAMDataTypeState" && IsZero(fieldValue)) ||
               (string.Equals(fieldName, "MaxAngle", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(fieldValue, "30", StringComparison.Ordinal)) ||
               (string.Equals(fieldName, "EditorID", StringComparison.OrdinalIgnoreCase) &&
                string.IsNullOrWhiteSpace(fieldValue)) ||
               string.Equals(fieldName, "FormVersion", StringComparison.OrdinalIgnoreCase) ||
               ((string.Equals(fieldName, "ObjectBoundsFirst", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(fieldName, "ObjectBoundsSecond", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(fieldName, "ObjectBounds.First", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(fieldName, "ObjectBounds.Second", StringComparison.OrdinalIgnoreCase)) &&
                string.Equals(fieldValue, "0, 0, 0", StringComparison.Ordinal)) ||
               (string.Equals(fieldName, "Color", StringComparison.OrdinalIgnoreCase) &&
                string.IsNullOrEmpty(fieldValue)) ||
               string.Equals(fieldName, "Flags.Count", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(fieldName, "Model.Flags.Count", StringComparison.OrdinalIgnoreCase) ||
               (string.Equals(fieldName, "Flags", StringComparison.OrdinalIgnoreCase) && IsZero(fieldValue)) ||
               (fieldName.EndsWith(".Flags", StringComparison.OrdinalIgnoreCase) &&
                (IsZero(fieldValue) || string.Equals(fieldValue, "None", StringComparison.OrdinalIgnoreCase))) ||
               (fieldName.EndsWith(".Flags.Count", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(fieldValue, "1", StringComparison.Ordinal)) ||
               (fieldName.EndsWith(".Flags[0]", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(fieldValue, "0", StringComparison.Ordinal)) ||
               fieldName.EndsWith(".RankIndex", StringComparison.OrdinalIgnoreCase) ||
               (fieldName.EndsWith(".Rank", StringComparison.OrdinalIgnoreCase) && IsZero(fieldValue)) ||
               (fieldName.EndsWith(".Priority", StringComparison.OrdinalIgnoreCase) && IsZero(fieldValue)) ||
               fieldName.EndsWith(".EffectIndex", StringComparison.OrdinalIgnoreCase) ||
               fieldName.EndsWith(".ConditionIndex", StringComparison.OrdinalIgnoreCase) ||
               fieldName.EndsWith(".RunOnTabIndex", StringComparison.OrdinalIgnoreCase) ||
               (fieldName.EndsWith(".ActivityCount", StringComparison.OrdinalIgnoreCase) && IsZero(fieldValue)) ||
               (fieldName.EndsWith(".ConditionCount", StringComparison.OrdinalIgnoreCase) && IsZero(fieldValue)) ||
               (fieldName.EndsWith(".CompareOperator", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(fieldValue, "EqualTo", StringComparison.OrdinalIgnoreCase)) ||
               (fieldName.EndsWith(".Data.RunOnType", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(fieldValue, "Subject", StringComparison.Ordinal)) ||
               ((fieldName.EndsWith(".Data.RunOnTypeIndex", StringComparison.OrdinalIgnoreCase) ||
                 fieldName.EndsWith(".Data.Unknown3", StringComparison.OrdinalIgnoreCase)) &&
                string.Equals(fieldValue, "-1", StringComparison.Ordinal)) ||
               ((fieldName.EndsWith(".Data.FirstParameter", StringComparison.OrdinalIgnoreCase) ||
                 fieldName.EndsWith(".Data.SecondParameter", StringComparison.OrdinalIgnoreCase) ||
                 fieldName.EndsWith(".ComparisonValue", StringComparison.OrdinalIgnoreCase) ||
                 fieldName.EndsWith(".Data.ParameterOneNumber", StringComparison.OrdinalIgnoreCase) ||
                 fieldName.EndsWith(".Data.ParameterTwoNumber", StringComparison.OrdinalIgnoreCase) ||
                 fieldName.EndsWith(".Data.SecondUnusedIntParameter", StringComparison.OrdinalIgnoreCase) ||
                 fieldName.EndsWith(".Unknown2", StringComparison.OrdinalIgnoreCase)) &&
                IsZero(fieldValue)) ||
               ((fieldName.EndsWith(".Data.UseAliases", StringComparison.OrdinalIgnoreCase) ||
                 fieldName.EndsWith(".Data.UsePackageData", StringComparison.OrdinalIgnoreCase) ||
                 fieldName.EndsWith(".Data.ParameterOneStringIsSet", StringComparison.OrdinalIgnoreCase) ||
                 fieldName.EndsWith(".Data.ParameterTwoStringIsSet", StringComparison.OrdinalIgnoreCase)) &&
               string.Equals(fieldValue, "False", StringComparison.OrdinalIgnoreCase)) ||
               fieldName.EndsWith(".Data.Reference", StringComparison.OrdinalIgnoreCase) ||
               fieldName.EndsWith(".Data.ParameterOneRecord", StringComparison.OrdinalIgnoreCase) ||
               fieldName.EndsWith(".Data.ParameterTwoRecord", StringComparison.OrdinalIgnoreCase) ||
               fieldName.EndsWith(".Data.StaticRegistration", StringComparison.OrdinalIgnoreCase) ||
               ((string.Equals(fieldName, "Hidden", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(fieldName, "Playable", StringComparison.OrdinalIgnoreCase)) &&
                string.Equals(fieldValue, "False", StringComparison.OrdinalIgnoreCase)) ||
               ((string.Equals(fieldName, "Category", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(fieldName, "CrewAssignment", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(fieldName, "SkillGroup", StringComparison.OrdinalIgnoreCase)) &&
                string.Equals(fieldValue, "None", StringComparison.OrdinalIgnoreCase)) ||
               ((string.Equals(fieldName, "Level", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(fieldName, "MajorFlags", StringComparison.OrdinalIgnoreCase)) &&
                IsZero(fieldValue)) ||
               (string.Equals(fieldName, "Teaches.RawContent", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(fieldValue, uint.MaxValue.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal)) ||
               (string.Equals(fieldName, "DataSlateType", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(fieldValue, "None", StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsDtoNullBackedByEmptySpriggitTranslation(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> spriggitFields)
    {
        return string.Equals(fieldValue, "Null", StringComparison.OrdinalIgnoreCase) &&
               spriggitFields.ContainsKey(fieldName + ".TargetLanguage") &&
               !spriggitFields.ContainsKey(fieldName + ".Count") &&
               !spriggitFields.Keys.Any(field => field.StartsWith(fieldName + "[", StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsMissingDefaultActorValueInformationSkillOffset(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> spriggitFields)
    {
        return string.Equals(fieldName, "Skill.ImproveOffset", StringComparison.OrdinalIgnoreCase) &&
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
               (string.Equals(propertyRemainder, ".Alias", StringComparison.OrdinalIgnoreCase) && string.Equals(fieldValue, "-1", StringComparison.Ordinal)) ||
               (string.Equals(propertyRemainder, ".ObjectUnused", StringComparison.OrdinalIgnoreCase) && IsZero(fieldValue)) ||
               (string.Equals(propertyRemainder, ".Unused", StringComparison.OrdinalIgnoreCase) && IsZero(fieldValue));
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
            ".ObjectAlias" => spriggitPath + ".Alias",
            ".ObjectUnused" => spriggitPath + ".Unused",
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

        if ((string.Equals(fieldRemainder, ".Alias", StringComparison.OrdinalIgnoreCase) ||
             string.Equals(fieldRemainder, ".ObjectAlias", StringComparison.OrdinalIgnoreCase)) &&
            string.Equals(fieldValue, "-1", StringComparison.Ordinal))
        {
            return true;
        }

        if ((string.Equals(fieldRemainder, ".Unused", StringComparison.OrdinalIgnoreCase) ||
             string.Equals(fieldRemainder, ".ObjectUnused", StringComparison.OrdinalIgnoreCase)) &&
            IsZero(fieldValue))
        {
            return true;
        }

        return string.Equals(fieldRemainder, ".MutagenObjectType", StringComparison.OrdinalIgnoreCase) &&
               IsDtoMutagenTypeBackedBySpriggitInlineType(spriggitPath + ".MutagenObjectType", fieldValue, spriggitFields);
    }

    private static bool IsDtoNestedScalarBackedBySpriggitList(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> spriggitFields,
        string listFieldName)
    {
        if (!fieldName.EndsWith("." + listFieldName, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var values = GetListValues(spriggitFields, fieldName);
        return values.Count > 0 &&
               string.Equals(fieldValue, string.Join(", ", values), StringComparison.Ordinal);
    }

    private static bool IsSpriggitInlineFormKeyListItemBackedByDtoScalar(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> dtoFields,
        string rootFieldName,
        string dtoPropertyName)
    {
        if (!TryGetIndexedPath(fieldName, rootFieldName, out var index, out var remainder) ||
            (remainder.Length != 0 && !remainder.StartsWith(".", StringComparison.Ordinal)))
        {
            return false;
        }

        if (remainder.Length == 0)
        {
            var dtoScalarPath = rootFieldName + "[" + index.ToString(CultureInfo.InvariantCulture) + "]." + dtoPropertyName;
            return dtoFields.TryGetValue(dtoScalarPath, out var directDtoValue) &&
                   string.Equals(fieldValue, directDtoValue, StringComparison.Ordinal);
        }

        if (remainder.Length == 1)
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
               (dtoFields.TryGetValue(dtoPath + "." + dtoPropertyName, out var dtoScalarValue) &&
                string.Equals(formKey, dtoScalarValue, StringComparison.Ordinal)) ||
               IsSpriggitInlineFormKeyListItemBackedByDtoFormKey(fieldValue, formKeyId, dtoFields, dtoPath);
    }

    /// <summary>
    /// Determines whether an inline Spriggit form-key list item is represented by flattened <see cref="FormKeyDTO"/> members.
    /// </summary>
    /// <param name="modFileName">The Spriggit mod file name stored as the inline field value.</param>
    /// <param name="formKeyId">The Spriggit form identifier portion from the inline field path.</param>
    /// <param name="dtoFields">All flattened DTO fields for the record.</param>
    /// <param name="dtoPath">The flattened DTO collection item path to inspect.</param>
    /// <returns><c>true</c> when the flattened DTO FormKey members match the Spriggit inline form key.</returns>
    private static bool IsSpriggitInlineFormKeyListItemBackedByDtoFormKey(
        string modFileName,
        string formKeyId,
        IReadOnlyDictionary<string, string> dtoFields,
        string dtoPath)
    {
        return dtoFields.TryGetValue(dtoPath + ".ModKey.FileName", out var dtoModFileName) &&
               dtoFields.TryGetValue(dtoPath + ".Id", out var dtoFormKeyId) &&
               string.Equals(modFileName, dtoModFileName, StringComparison.OrdinalIgnoreCase) &&
               AreEquivalentFormKeyIds(formKeyId, dtoFormKeyId);
    }

    /// <summary>
    /// Compares Spriggit hexadecimal form identifiers with DTO numeric or hexadecimal form identifiers.
    /// </summary>
    /// <param name="spriggitFormKeyId">The Spriggit form identifier text, usually hexadecimal without a prefix.</param>
    /// <param name="dtoFormKeyId">The DTO form identifier text, usually decimal after flattening.</param>
    /// <returns><c>true</c> when both values identify the same form.</returns>
    private static bool AreEquivalentFormKeyIds(string spriggitFormKeyId, string dtoFormKeyId)
    {
        var normalizedDtoFormKeyId = dtoFormKeyId.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
            ? dtoFormKeyId[2..]
            : dtoFormKeyId;

        return uint.TryParse(spriggitFormKeyId, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var spriggitId) &&
               (uint.TryParse(normalizedDtoFormKeyId, NumberStyles.Integer, CultureInfo.InvariantCulture, out var dtoDecimalId) ||
                uint.TryParse(normalizedDtoFormKeyId, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out dtoDecimalId)) &&
               spriggitId == dtoDecimalId;
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

    private static bool IsSpriggitNestedListBackedDtoScalar(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> dtoFields,
        string listFieldName)
    {
        var listPath = GetNestedListPath(fieldName, listFieldName);
        if (string.IsNullOrWhiteSpace(listPath) || !dtoFields.TryGetValue(listPath, out var dtoValue))
        {
            return false;
        }

        if (string.Equals(fieldName, listPath + ".Count", StringComparison.OrdinalIgnoreCase))
        {
            var values = dtoValue.Split(", ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            return string.Equals(fieldValue, values.Length.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);
        }

        if (TryGetListIndex(fieldName, listPath, out var listIndex))
        {
            var values = dtoValue.Split(", ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            return listIndex >= 0 &&
                   listIndex < values.Length &&
                   string.Equals(fieldValue, values[listIndex], StringComparison.Ordinal);
        }

        return false;
    }

    private static string GetNestedListPath(string fieldName, string listFieldName)
    {
        var listSuffix = "." + listFieldName;
        var suffixIndex = fieldName.IndexOf(listSuffix, StringComparison.OrdinalIgnoreCase);
        if (suffixIndex < 0)
        {
            return string.Empty;
        }

        return fieldName[..(suffixIndex + listSuffix.Length)];
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
        return !spriggitFields.ContainsKey(rootFieldName + ".Count") &&
               !spriggitFields.Keys.Any(field => field.StartsWith(rootFieldName + "[", StringComparison.OrdinalIgnoreCase));
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

    private static int GetIndexedPathCount(IReadOnlyDictionary<string, string> fields, string rootFieldName)
    {
        return fields.Keys
            .Select(field => TryGetIndexedPath(field, rootFieldName, out var index, out _) ? index : -1)
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

    private static bool IsNull(string value)
    {
        return string.Equals(value, "Null", StringComparison.OrdinalIgnoreCase);
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

                var value = valueLine.Trim()[2..].Trim();
                if (!IsRootScalarListValue(value))
                {
                    values.Clear();
                    break;
                }

                values.Add(NormalizeScalar(value));
            }

            if (values.Count == 0)
            {
                continue;
            }

            if (values.Any(value => value.Contains(": ", StringComparison.Ordinal)))
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

    /// <summary>
    /// Determines whether a top-level YAML list row is a scalar alias candidate rather than an object wrapper.
    /// </summary>
    /// <param name="value">The raw list item text after the leading dash.</param>
    /// <returns><c>true</c> when the row can be safely projected as a scalar list item.</returns>
    private static bool IsRootScalarListValue(string value)
    {
        var isQuoted = value.Length >= 2 &&
                       ((value.StartsWith('\'') && value.EndsWith('\'')) ||
                        (value.StartsWith('"') && value.EndsWith('"')));
        return isQuoted ||
               (!value.EndsWith(":", StringComparison.Ordinal) &&
                !value.Contains(": ", StringComparison.Ordinal));
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

    private static IReadOnlyDictionary<string, string> NormalizeSpriggitFields(SupportedGame game, RecordTypeData recordType, IReadOnlyDictionary<string, string> fields)
    {
        var normalizedFields = new Dictionary<string, string>(fields, StringComparer.OrdinalIgnoreCase);
        if (normalizedFields.TryGetValue("Color", out var color))
        {
            normalizedFields["Color"] = FormatSpriggitColor(color);
        }

        NormalizeSpriggitPathValue(normalizedFields, "Model.File");
        NormalizeSpriggitHexValue(normalizedFields, "FNAM");
        AddSpriggitComponentHexAlias(normalizedFields, "WAIM");
        AddSpriggitComponentHexAlias(normalizedFields, "WFIR");
        NormalizeSpriggitHexValue(normalizedFields, "WAIM");
        NormalizeSpriggitHexValue(normalizedFields, "WFIR");
        NormalizeSpriggitHexValue(normalizedFields, "FLAG");
        AddSpriggitComponentCountAliases(normalizedFields);
        AddSpriggitResourceCountAliases(normalizedFields);
        AddSpriggitDestructibleAliases(normalizedFields);
        AddSpriggitVirtualMachineAdapterAliases(normalizedFields);
        AddSpriggitMaterialSwapAliases(normalizedFields);
        if (normalizedFields.ContainsKey("Model.File") &&
            !normalizedFields.ContainsKey("Model.Count"))
        {
            normalizedFields["Model.Count"] = "1";
        }

        return normalizedFields;
    }

    private static void AddSpriggitComponentHexAlias(IDictionary<string, string> fields, string fieldName)
    {
        if (fields.ContainsKey(fieldName) ||
            !fields.TryGetValue("Components.Count", out var countValue) ||
            !int.TryParse(countValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var count))
        {
            return;
        }

        for (var index = 0; index < count; index++)
        {
            var componentFieldName = "Components[" + index.ToString(CultureInfo.InvariantCulture) + "]." + fieldName;
            if (fields.TryGetValue(componentFieldName, out var value))
            {
                fields[fieldName] = value;
                return;
            }
        }
    }

    private static void AddSpriggitComponentCountAliases(IDictionary<string, string> fields)
    {
        if (!fields.TryGetValue("Components.Count", out var countValue) ||
            !int.TryParse(countValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var count))
        {
            return;
        }

        for (var index = 0; index < count; index++)
        {
            AddSpriggitFieldAlias(
                fields,
                "Components[" + index.ToString(CultureInfo.InvariantCulture) + "].Count",
                "Count[" + index.ToString(CultureInfo.InvariantCulture) + "]");
        }
    }

    private static void AddSpriggitResourceCountAliases(IDictionary<string, string> fields)
    {
        if (!fields.TryGetValue("Resources.Count", out var countValue) ||
            !int.TryParse(countValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var count))
        {
            return;
        }

        if (count == 1)
        {
            AddSpriggitFieldAlias(fields, "Resources[0].Count", "Count");
        }

        for (var index = 0; index < count; index++)
        {
            AddSpriggitFieldAlias(
                fields,
                "Resources[" + index.ToString(CultureInfo.InvariantCulture) + "].Count",
                "Count[" + index.ToString(CultureInfo.InvariantCulture) + "]");
        }
    }

    private static void AddSpriggitDestructibleAliases(IDictionary<string, string> fields)
    {
        AddSpriggitFieldAlias(fields, "Destructible.Stages.Count", "Destructible.Count");
        if (!fields.TryGetValue("Destructible.Stages.Count", out var countValue) ||
            !int.TryParse(countValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var count))
        {
            return;
        }

        for (var index = 0; index < count; index++)
        {
            var stagePath = "Destructible.Stages[" + index.ToString(CultureInfo.InvariantCulture) + "]";
            var aliasPath = "Destructible[" + index.ToString(CultureInfo.InvariantCulture) + "]";
            foreach (var field in fields.Where(field => field.Key.StartsWith(stagePath + ".", StringComparison.OrdinalIgnoreCase)).ToList())
            {
                fields[aliasPath + field.Key[stagePath.Length..]] = field.Value;
            }

            AddSpriggitFieldAlias(fields, stagePath + ".Flags.Count", aliasPath + ".Count");
            if (!fields.TryGetValue(stagePath + ".Flags.Count", out var flagsCountValue) ||
                !int.TryParse(flagsCountValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var flagsCount))
            {
                continue;
            }

            for (var flagIndex = 0; flagIndex < flagsCount; flagIndex++)
            {
                AddSpriggitFieldAlias(
                    fields,
                    stagePath + ".Flags[" + flagIndex.ToString(CultureInfo.InvariantCulture) + "]",
                    aliasPath + "[" + flagIndex.ToString(CultureInfo.InvariantCulture) + "]");
            }
        }
    }

    private static void AddSpriggitVirtualMachineAdapterAliases(IDictionary<string, string> fields)
    {
        if (!fields.TryGetValue("VirtualMachineAdapter.Scripts.Count", out var countValue) ||
            !int.TryParse(countValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var count))
        {
            return;
        }

        fields["VirtualMachineAdapter.Count"] = count.ToString(CultureInfo.InvariantCulture);
        for (var index = 0; index < count; index++)
        {
            var scriptIndex = index.ToString(CultureInfo.InvariantCulture);
            var scriptPath = "VirtualMachineAdapter.Scripts[" + scriptIndex + "]";
            var aliasPath = "VirtualMachineAdapter[" + scriptIndex + "]";
            AddSpriggitFieldAlias(fields, scriptPath + ".Name", aliasPath + ".Name");
            AddSpriggitFieldAlias(fields, scriptPath + ".Properties.Count", aliasPath + ".Count");

            if (!fields.TryGetValue(scriptPath + ".Properties.Count", out var propertyCountValue) ||
                !int.TryParse(propertyCountValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var propertyCount))
            {
                continue;
            }

            for (var propertyIndex = 0; propertyIndex < propertyCount; propertyIndex++)
            {
                var propertyIndexText = propertyIndex.ToString(CultureInfo.InvariantCulture);
                var propertyPath = scriptPath + ".Properties[" + propertyIndexText + "]";
                var propertyAliasPath = aliasPath + "[" + propertyIndexText + "]";
                foreach (var field in fields.Where(field => field.Key.StartsWith(propertyPath + ".", StringComparison.OrdinalIgnoreCase)).ToList())
                {
                    fields[propertyAliasPath + field.Key[propertyPath.Length..]] = field.Value;
                }
            }
        }
    }

    private static void AddSpriggitMaterialSwapAliases(IDictionary<string, string> fields)
    {
        AddSpriggitMaterialSwapListAliases(fields, "Model.MaterialSwaps");
        AddSpriggitMaterialSwapObjectAliases(fields, "Model.AlternateTextures");
    }

    private static void AddSpriggitMaterialSwapListAliases(IDictionary<string, string> fields, string fieldPath)
    {
        if (!fields.TryGetValue(fieldPath + ".Count", out var countValue) ||
            !int.TryParse(countValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var count))
        {
            return;
        }

        fields["Model.MaterialSwaps.Count"] = count.ToString(CultureInfo.InvariantCulture);
        fields["Models[0].MaterialSwaps.Count"] = count.ToString(CultureInfo.InvariantCulture);
        for (var index = 0; index < count; index++)
        {
            var materialSwapPath = fieldPath + "[" + index.ToString(CultureInfo.InvariantCulture) + "]";
            var materialSwapFormKey = string.Empty;
            if (fields.TryGetValue(materialSwapPath, out materialSwapFormKey))
            {
                AddSpriggitMaterialSwapFormKeyAliases(fields, index, materialSwapFormKey);
                continue;
            }

            foreach (var field in fields.Where(field => field.Key.StartsWith(materialSwapPath + ".", StringComparison.OrdinalIgnoreCase)).ToList())
            {
                var materialSwapFormId = field.Key[(materialSwapPath.Length + 1)..];
                if (!materialSwapFormId.Contains('.', StringComparison.Ordinal) &&
                    !materialSwapFormId.Contains('[', StringComparison.Ordinal) &&
                    !materialSwapFormId.Contains(']', StringComparison.Ordinal))
                {
                    materialSwapFormKey = materialSwapFormId + ":" + field.Value;
                    AddSpriggitMaterialSwapFormKeyAliases(fields, index, materialSwapFormKey);
                    break;
                }
            }
        }

        if (count == 1)
        {
            AddSpriggitFieldAlias(fields, fieldPath + "[0]", "Model[1]");
            if (!fields.ContainsKey("Model[1]") &&
                fields.TryGetValue("Model.MaterialSwaps[0]", out var materialSwapFormKey))
            {
                fields["Model[1]"] = materialSwapFormKey;
            }
        }
    }

    private static void AddSpriggitMaterialSwapObjectAliases(IDictionary<string, string> fields, string fieldPath)
    {
        if (!fields.TryGetValue(fieldPath + ".Count", out var countValue) ||
            !int.TryParse(countValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var count))
        {
            return;
        }

        for (var index = 0; index < count; index++)
        {
            var materialSwapPath = fieldPath + "[" + index.ToString(CultureInfo.InvariantCulture) + "]";
            AddSpriggitFieldAlias(fields, materialSwapPath + ".Name", "Model[" + index.ToString(CultureInfo.InvariantCulture) + "].Name");
            AddSpriggitFieldAlias(fields, materialSwapPath + ".Name", "Models[0].MaterialSwaps[" + index.ToString(CultureInfo.InvariantCulture) + "].Name");
            AddSpriggitFieldAlias(fields, materialSwapPath + ".NewTexture", "Model[" + index.ToString(CultureInfo.InvariantCulture) + "].NewTexture");
            if (fields.TryGetValue(materialSwapPath + ".NewTexture", out var materialSwapFormKey))
            {
                AddSpriggitMaterialSwapFormKeyAliases(fields, index, materialSwapFormKey);
            }
        }

        fields["Model.MaterialSwaps.Count"] = count.ToString(CultureInfo.InvariantCulture);
        fields["Models[0].MaterialSwaps.Count"] = count.ToString(CultureInfo.InvariantCulture);
        if (count == 1)
        {
            AddSpriggitFieldAlias(fields, fieldPath + "[0].NewTexture", "Model[1]");
            AddSpriggitFieldAlias(fields, fieldPath + "[0].NewTexture", "Model.MaterialSwap");
            AddSpriggitFieldAlias(fields, fieldPath + "[0].NewTexture", "Model.MaterialSwaps[0]");
        }
    }

    private static void AddSpriggitMaterialSwapFormKeyAliases(IDictionary<string, string> fields, int index, string materialSwapFormKey)
    {
        fields["Model.MaterialSwap"] = materialSwapFormKey;
        fields["Model.MaterialSwaps[" + index.ToString(CultureInfo.InvariantCulture) + "]"] = materialSwapFormKey;
        fields["Model[" + index.ToString(CultureInfo.InvariantCulture) + "].NewTexture"] = materialSwapFormKey;
        fields["Models[0].MaterialSwaps[" + index.ToString(CultureInfo.InvariantCulture) + "].MaterialSwapFormKey"] = materialSwapFormKey;
        fields["Models[0].MaterialSwaps[" + index.ToString(CultureInfo.InvariantCulture) + "].MaterialSwapIndex"] = index.ToString(CultureInfo.InvariantCulture);
        fields["Models[0].MaterialSwaps[" + index.ToString(CultureInfo.InvariantCulture) + "].ModelGender"] = string.Empty;
        fields["Models[0].MaterialSwaps[" + index.ToString(CultureInfo.InvariantCulture) + "].ModelSlot"] = "Model";

        var formIdSeparator = materialSwapFormKey.IndexOf(':', StringComparison.Ordinal);
        if (formIdSeparator > 0)
        {
            fields["Model[" + index.ToString(CultureInfo.InvariantCulture) + "]." + materialSwapFormKey[..formIdSeparator]] = materialSwapFormKey;
        }
    }

    private static void NormalizeSpriggitHexValue(IDictionary<string, string> fields, string fieldName)
    {
        if (fields.TryGetValue(fieldName, out var value) &&
            value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            fields[fieldName] = value[2..];
        }
    }

    private static void NormalizeSpriggitPathValue(IDictionary<string, string> fields, string fieldName)
    {
        if (fields.TryGetValue(fieldName, out var value))
        {
            fields[fieldName] = value.Replace('/', '\\');
        }
    }

    private static string FormatSpriggitColor(string value)
    {
        if (value.Length == 7 &&
            value[0] == '#' &&
            byte.TryParse(value.AsSpan(1, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var redOnly) &&
            byte.TryParse(value.AsSpan(3, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var greenOnly) &&
            byte.TryParse(value.AsSpan(5, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var blueOnly))
        {
            return $"Color [A=255, R={redOnly}, G={greenOnly}, B={blueOnly}]";
        }

        if (value.Length == 9 &&
            value[0] == '#' &&
            byte.TryParse(value.AsSpan(1, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var alpha) &&
            byte.TryParse(value.AsSpan(3, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var red) &&
            byte.TryParse(value.AsSpan(5, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var green) &&
            byte.TryParse(value.AsSpan(7, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var blue))
        {
            return $"Color [A={alpha}, R={red}, G={green}, B={blue}]";
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

}
