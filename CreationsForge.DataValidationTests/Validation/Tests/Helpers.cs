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
        ["MajorRecordFlagsRaw"] = "MajorRecordFlags"
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

    public static IReadOnlyList<string> GetUnmatchedSpriggitFields<TRecord>(SpriggitRecordDTO spriggit, TRecord dto, params string[] ignoredSpriggitFieldRoots)
        where TRecord : RecordDTO
    {
        var unmatchedFields = new List<string>();
        var dtoFields = DtoFlattener.Flatten(dto);

        foreach (var field in spriggit.Fields.OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase))
        {
            if (!IsIgnoredField(field.Key, ignoredSpriggitFieldRoots) && !IsMatchedSpriggitField(field.Key, spriggit.Fields, dtoFields))
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
        var dtoFields = DtoFlattener.Flatten(dto);
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

    public static IReadOnlyList<string> GetUnmatchedDtoFields<TRecord>(SpriggitRecordDTO spriggit, TRecord dto, params string[] ignoredDtoFieldRoots)
        where TRecord : RecordDTO
    {
        var unmatchedFields = new List<string>();
        var dtoFields = DtoFlattener.Flatten(dto);

        foreach (var field in dtoFields.OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase))
        {
            if (!IsIgnoredField(field.Key, ignoredDtoFieldRoots) && !IsMatchedDtoField(field.Key, field.Value, spriggit.Fields, dtoFields))
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
        return DtoFlattener.Flatten(dto);
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

        if (string.Equals(fieldName, "MajorRecordFlags", StringComparison.OrdinalIgnoreCase) && IsZero(fieldValue))
        {
            return true;
        }

        if (IsMissingZeroCount(fieldName, fieldValue, spriggitFields))
        {
            return true;
        }

        return IsSpriggitListBackedDtoScalar(fieldName, spriggitFields, dtoFields, "Flags");
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
        return !HasSpriggitPath(spriggitFields, rootFieldName);
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

    private static bool IsZero(string value)
    {
        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var number) && number == 0;
    }

    private static bool IsIgnoredField(string fieldName, IReadOnlyList<string> ignoredFieldRoots)
    {
        return ignoredFieldRoots.Any(root =>
            string.Equals(fieldName, root, StringComparison.OrdinalIgnoreCase) ||
            fieldName.StartsWith(root + ".", StringComparison.OrdinalIgnoreCase) ||
            fieldName.StartsWith(root + "[", StringComparison.OrdinalIgnoreCase));
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
