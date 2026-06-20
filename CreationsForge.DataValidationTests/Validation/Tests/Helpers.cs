using System.Globalization;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.DataValidationTests.Validation.Environment;
using CreationsForge.DataValidationTests.Validation.Parsing;
using CreationsForge.DataValidationTests.Validation.Services;
using Shouldly;

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
        record.ShouldBeOfType<TRecord>("Reader DTO should match the expected test DTO type for " + game + " " + recordType.RecordID + " " + formKey + ".");
        return (TRecord)record;
    }

    private static GlobalSpriggitDTO GetGlobalSpriggit(SupportedGame game, RecordTypeData recordType, string sampleName)
    {
        recordType.RecordID.ShouldBe(RecordTypeCatalog.Global.RecordID, "This Spriggit helper currently supports Global records.");
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
        var fields = document.FlattenScalars();

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

    public static void AssertNoUnmatchedSpriggitFields(GlobalSpriggitDTO spriggit, GlobalDTO dto)
    {
        _ = dto;
        foreach (var field in spriggit.Fields.OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase))
        {
            GlobalSpriggitToDtoFields.ContainsKey(field.Key).ShouldBeTrue(
                "No matching CreationsForge reader DTO data was found for Spriggit field '" + field.Key + "'." +
                System.Environment.NewLine +
                "Spriggit value: " + field.Value +
                System.Environment.NewLine +
                "Record: " + spriggit.FormKey);
        }
    }

    public static void AssertNoUnmatchedSpriggitFields(SpriggitRecordDTO spriggit, params string[] assertedSpriggitFields)
    {
        var assertedFields = new HashSet<string>(assertedSpriggitFields, StringComparer.OrdinalIgnoreCase);

        foreach (var field in spriggit.Fields.OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase))
        {
            assertedFields.Contains(field.Key).ShouldBeTrue(
                "No explicit assertion was found for Spriggit field '" + field.Key + "'." +
                System.Environment.NewLine +
                "Spriggit value: " + field.Value +
                System.Environment.NewLine +
                "Record: " + spriggit.FormKey);
        }
    }

    public static void AssertNoUnmatchedDtoFields(GlobalSpriggitDTO spriggit, GlobalDTO dto)
    {
        var dtoFields = DtoFlattener.Flatten(dto);
        var matchedDtoFields = new HashSet<string>(GlobalSpriggitToDtoFields.Values, StringComparer.OrdinalIgnoreCase);

        foreach (var field in dtoFields.OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase))
        {
            matchedDtoFields.Contains(field.Key).ShouldBeTrue(
                "No matching Spriggit field was found for CreationsForge reader DTO field '" + field.Key + "'." +
                System.Environment.NewLine +
                "DTO value: " + field.Value +
                System.Environment.NewLine +
                "Record: " + spriggit.FormKey);
        }
    }

    public static void AssertNoUnmatchedDtoFields<TRecord>(SpriggitRecordDTO spriggit, TRecord dto, params string[] assertedDtoFields)
        where TRecord : RecordDTO
    {
        var dtoFields = DtoFlattener.Flatten(dto);
        var assertedFields = new HashSet<string>(assertedDtoFields, StringComparer.OrdinalIgnoreCase);

        foreach (var field in dtoFields.OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase))
        {
            assertedFields.Contains(field.Key).ShouldBeTrue(
                "No explicit assertion was found for CreationsForge reader DTO field '" + field.Key + "'." +
                System.Environment.NewLine +
                "DTO value: " + field.Value +
                System.Environment.NewLine +
                "Record: " + spriggit.FormKey);
        }
    }

    public static string GetSpriggitField(SpriggitRecordDTO spriggit, string fieldPath)
    {
        spriggit.Fields.TryGetValue(fieldPath, out var value).ShouldBeTrue(
            "Spriggit record should contain field '" + fieldPath + "'." +
            System.Environment.NewLine +
            "Record: " + spriggit.FormKey);

        return value!;
    }

    public static string GetDTOField<TRecord>(TRecord dto, string fieldPath)
        where TRecord : RecordDTO
    {
        var dtoFields = DtoFlattener.Flatten(dto);
        dtoFields.TryGetValue(fieldPath, out var value).ShouldBeTrue(
            "CreationsForge reader DTO should contain field '" + fieldPath + "'." +
            System.Environment.NewLine +
            "Record: " + FormatFormKey(dto.FormKey));

        return value!;
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
        Directory.Exists(root).ShouldBeTrue("Spriggit extraction root should exist: " + root);

        var matchingPath = Directory.GetFiles(root, "*.yaml", SearchOption.AllDirectories)
            .FirstOrDefault(path =>
                string.Equals(Path.GetFileNameWithoutExtension(path), sampleName, StringComparison.OrdinalIgnoreCase) ||
                Path.GetFileNameWithoutExtension(path).StartsWith(sampleName + " - ", StringComparison.OrdinalIgnoreCase));

        matchingPath.ShouldNotBeNull("Unable to find Spriggit sample '" + sampleName + "' under " + root + ".");
        return matchingPath;
    }

    private static string GetRequiredString(IReadOnlyDictionary<string, string> fields, string fieldName, string path)
    {
        fields.TryGetValue(fieldName, out var value).ShouldBeTrue("Spriggit file should contain field '" + fieldName + "': " + path);
        return value!;
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
