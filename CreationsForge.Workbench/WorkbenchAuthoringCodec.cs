using System.Globalization;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace CreationsForge.Workbench;

/// <summary>Reads production authoring results and builds the guided editor's tool arguments.</summary>
internal static class WorkbenchAuthoringCodec
{
    private static readonly string[] SupportedKinds =
    [
        "String",
        "Boolean",
        "SignedInteger",
        "UnsignedInteger",
        "FloatingPoint",
        "Enum",
        "FormLink",
        "TranslatedString",
    ];

    private static readonly Regex FormKeyPattern = new(
        "^[0-9A-Fa-f]{1,8}:[^\\\\/:*?\"<>|]+\\.(esm|esp|esl)$",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);

    /// <summary>Reads family names from a <c>record_types</c> result.</summary>
    /// <param name="result">The tool result, including <c>structuredContent</c> when the host sent it.</param>
    /// <returns>The family identifiers. The list is empty when the result has none.</returns>
    public static IReadOnlyList<string> ReadFamilies(JsonObject result)
    {
        if (Content(result)?["families"] is not JsonArray families)
        {
            return [];
        }

        return families.Select(Text).Where(name => !string.IsNullOrWhiteSpace(name)).Select(name => name!).ToArray();
    }

    /// <summary>Creates field editors from one <c>record_schema</c> page.</summary>
    /// <param name="result">The schema page.</param>
    /// <param name="languages">The language names offered for translated strings.</param>
    /// <returns>The fields on this page.</returns>
    public static IReadOnlyList<WorkbenchFieldDraft> ReadSchemaFields(JsonObject result, IReadOnlyList<string> languages)
    {
        if (Content(result)?["fields"] is not JsonArray fields)
        {
            return [];
        }

        var drafts = new List<WorkbenchFieldDraft>();
        foreach (var field in fields.OfType<JsonObject>())
        {
            var path = Text(field["path"]);
            if (string.IsNullOrWhiteSpace(path))
            {
                continue;
            }

            var kind = Text(field["kind"]) ?? string.Empty;
            var supported = SupportedKinds.Contains(kind, StringComparer.Ordinal);
            var choices = ReadStrings(field["choices"]);
            var targets = ReadStrings(field["referenceTargets"]);
            var summary = targets.Count == 0 ? string.Empty : "Link targets: " + string.Join(", ", targets);
            if (summary.Length > 180)
            {
                summary = summary[..180] + "...";
            }

            drafts.Add(new WorkbenchFieldDraft(
                path,
                kind,
                ReadBool(field["isNullable"]),
                ReadBool(field["isFlags"]),
                supported,
                choices,
                languages,
                summary,
                supported ? string.Empty : "Guided editing does not include " + kind + " fields. This value stays unchanged."));
        }

        return drafts;
    }

    /// <summary>Copies one record-read page onto matching field editors and refreshes their unchanged snapshot.</summary>
    /// <param name="fields">The editors created from the schema.</param>
    /// <param name="result">One <c>record_read</c> page.</param>
    public static void ApplyReadPage(IReadOnlyList<WorkbenchFieldDraft> fields, JsonObject result)
    {
        if (Content(result)?["record"] is not JsonObject record || record["fields"] is not JsonArray page)
        {
            return;
        }

        foreach (var item in page.OfType<JsonObject>())
        {
            var path = Text(item["path"]);
            var field = fields.FirstOrDefault(candidate => string.Equals(candidate.Path, path, StringComparison.Ordinal));
            if (field is null)
            {
                continue;
            }

            ApplyValue(field, item["value"] as JsonObject);
            field.OriginalCanonical = Canonical(field);
        }
    }

    /// <summary>Snapshots every field's current value as unchanged.</summary>
    /// <param name="fields">The editors to snapshot.</param>
    public static void Snapshot(IReadOnlyList<WorkbenchFieldDraft> fields)
    {
        foreach (var field in fields)
        {
            field.OriginalCanonical = Canonical(field);
        }
    }

    /// <summary>Reads search hits from one <c>records_search</c> page.</summary>
    /// <param name="result">The search page.</param>
    /// <returns>The hits on this page.</returns>
    public static IReadOnlyList<WorkbenchRecordHit> ReadHits(JsonObject result)
    {
        if (Content(result)?["records"] is not JsonArray records)
        {
            return [];
        }

        var hits = new List<WorkbenchRecordHit>();
        foreach (var record in records.OfType<JsonObject>())
        {
            var formKey = Text(record["formKey"]);
            var containing = Text(record["containingModKey"]);
            if (string.IsNullOrWhiteSpace(formKey) || string.IsNullOrWhiteSpace(containing))
            {
                continue;
            }

            hits.Add(new WorkbenchRecordHit(
                formKey,
                containing,
                Text(record["winningModKey"]) ?? containing,
                Text(record["editorId"]) ?? string.Empty));
        }

        return hits;
    }

    /// <summary>Reads the next-page cursor from a schema, search, read, preview, or compare result.</summary>
    /// <param name="result">The tool result.</param>
    /// <returns>The cursor, or <see langword="null"/> when this is the last page.</returns>
    public static string? NextCursor(JsonObject result)
    {
        var content = Content(result);
        var cursor = Text(content?["nextCursor"]);
        if (!string.IsNullOrWhiteSpace(cursor))
        {
            return cursor;
        }

        return content?["record"] is JsonObject record ? Text(record["nextFieldCursor"]) : null;
    }

    /// <summary>Reads the published record identity from a create or override result.</summary>
    /// <param name="result">The apply result.</param>
    /// <param name="formKey">The origin FormKey when this method returns <see langword="true"/>.</param>
    /// <param name="containingModKey">The containing plugin when this method returns <see langword="true"/>.</param>
    /// <returns><see langword="true"/> when the result contains one record identity.</returns>
    public static bool TryReadPublishedRecord(JsonObject result, out string formKey, out string containingModKey)
    {
        formKey = string.Empty;
        containingModKey = string.Empty;
        if (Content(result)?["records"] is not JsonArray records)
        {
            return false;
        }

        var record = records.OfType<JsonObject>().FirstOrDefault();
        formKey = Text(record?["formKey"]) ?? string.Empty;
        containingModKey = Text(record?["containingModKey"]) ?? string.Empty;
        return formKey.Length > 0 && containingModKey.Length > 0;
    }

    /// <summary>Reads whether a workspace-state or preview result says the workspace is dirty.</summary>
    /// <param name="result">The tool result.</param>
    /// <returns><see langword="true"/> when <c>isDirty</c> is true.</returns>
    public static bool IsDirty(JsonObject result)
    {
        return ReadBool(Content(result)?["isDirty"]);
    }

    /// <summary>Reads a successful save path.</summary>
    /// <param name="result">The <c>workspace_save</c> result.</param>
    /// <param name="path">The destination path when this method returns <see langword="true"/>.</param>
    /// <returns><see langword="true"/> only when the status is <c>Succeeded</c> and a path is present.</returns>
    public static bool TryReadSuccessfulSave(JsonObject result, out string path)
    {
        var content = Content(result);
        path = Text(content?["destinationPath"]) ?? string.Empty;
        return string.Equals(Text(content?["status"]), "Succeeded", StringComparison.Ordinal) && path.Length > 0;
    }

    /// <summary>Builds Set changes for fields whose values differ from the loaded snapshot.</summary>
    /// <param name="fields">The current editors.</param>
    /// <param name="changes">The changes to send when this method returns <see langword="null"/>.</param>
    /// <returns>A validation message when a changed field is illegal; otherwise <see langword="null"/>.</returns>
    public static string? TryBuildChanges(IReadOnlyList<WorkbenchFieldDraft> fields, out JsonArray changes)
    {
        changes = [];
        foreach (var field in fields)
        {
            if (!field.IsSupported)
            {
                continue;
            }

            var current = Canonical(field);
            if (string.Equals(current, field.OriginalCanonical, StringComparison.Ordinal))
            {
                continue;
            }

            var error = Validate(field);
            if (error is not null)
            {
                changes = [];
                return error;
            }

            changes.Add(new JsonObject
            {
                ["path"] = field.Path,
                ["operation"] = "Set",
                ["value"] = Value(field),
            });
        }

        return null;
    }

    /// <summary>Copies field values from one read page into a canonical map.</summary>
    /// <param name="map">The map to update. Keys are field paths.</param>
    /// <param name="result">One <c>record_read</c> page.</param>
    public static void AddCanonicalFields(Dictionary<string, string> map, JsonObject result)
    {
        if (Content(result)?["record"] is not JsonObject record || record["fields"] is not JsonArray page)
        {
            return;
        }

        foreach (var item in page.OfType<JsonObject>())
        {
            var path = Text(item["path"]);
            if (string.IsNullOrWhiteSpace(path))
            {
                continue;
            }

            map[path] = CanonicalValue(item["value"] as JsonObject);
        }
    }

    /// <summary>Describes a translated-string value for the observation log.</summary>
    /// <param name="result">A <c>record_read</c> page that may contain <c>Name</c>.</param>
    /// <returns>A one-line description, or an empty string when this page has no <c>Name</c>.</returns>
    public static string DescribeName(JsonObject result)
    {
        if (Content(result)?["record"] is not JsonObject record || record["fields"] is not JsonArray page)
        {
            return string.Empty;
        }

        foreach (var item in page.OfType<JsonObject>())
        {
            if (!string.Equals(Text(item["path"]), "Name", StringComparison.Ordinal))
            {
                continue;
            }

            return DescribeTranslation(item["value"] as JsonObject);
        }

        return string.Empty;
    }

    /// <summary>Compares two field snapshots of one unrelated record.</summary>
    /// <param name="editorId">The record's EditorID, used in the sentence.</param>
    /// <param name="before">The fields captured before save.</param>
    /// <param name="after">The fields read in the fresh session.</param>
    /// <returns>A sentence that says the record is unchanged or names the paths that differ.</returns>
    public static string CompareMaps(
        string editorId,
        IReadOnlyDictionary<string, string> before,
        IReadOnlyDictionary<string, string> after)
    {
        var label = editorId.Length == 0 ? "the unrelated record" : editorId;
        var changed = new List<string>();
        foreach (var pair in before)
        {
            if (!after.TryGetValue(pair.Key, out var value) || !string.Equals(value, pair.Value, StringComparison.Ordinal))
            {
                changed.Add(pair.Key);
            }
        }

        foreach (var pair in after)
        {
            if (!before.ContainsKey(pair.Key))
            {
                changed.Add(pair.Key);
            }
        }

        if (changed.Count == 0)
        {
            return "Unrelated record " + label + " is unchanged.";
        }

        return "Unrelated record " + label + " changed: " + string.Join(", ", changed.Distinct(StringComparer.Ordinal)) + ".";
    }

    /// <summary>Summarizes a compare page for the observation log.</summary>
    /// <param name="result">The <c>record_compare</c> result.</param>
    /// <returns>A short description of the difference page.</returns>
    public static string SummarizeCompare(JsonObject result)
    {
        if (Content(result)?["differences"] is not JsonArray differences || differences.Count == 0)
        {
            return "Compare returned no registered field differences on this page.";
        }

        var names = differences.OfType<JsonObject>()
            .Select(difference => Text(difference["path"]))
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .Take(6)
            .ToArray();
        return "Compare found " + differences.Count + " difference(s): " + string.Join(", ", names) + ".";
    }

    /// <summary>Summarizes a preview page.</summary>
    /// <param name="result">The <c>workspace_preview</c> result.</param>
    /// <returns>A sentence that does not claim the output file was written.</returns>
    public static string SummarizePreview(JsonObject result)
    {
        var content = Content(result);
        var count = content?["records"] is JsonArray records ? records.Count : 0;
        var dirty = ReadBool(content?["isDirty"]);
        return "Preview: dirty=" + dirty.ToString().ToLowerInvariant() + ", " + count + " pending record(s) on this page. Preview does not write the output file.";
    }

    /// <summary>Summarizes a discard result.</summary>
    /// <param name="result">The <c>workspace_discard</c> result.</param>
    /// <returns>A sentence that says discard did not write the output file.</returns>
    public static string SummarizeDiscard(JsonObject result)
    {
        var content = Content(result);
        var changed = ReadBool(content?["changed"]);
        var dirty = ReadBool(content?["isDirty"]);
        return "Discard restored the saved baseline (changed=" + changed.ToString().ToLowerInvariant()
            + ", dirty=" + dirty.ToString().ToLowerInvariant() + "). Discard does not write the output file.";
    }

    private static void ApplyValue(WorkbenchFieldDraft field, JsonObject? value)
    {
        field.ClearValue = false;
        field.StringValue = string.Empty;
        field.NumberText = string.Empty;
        field.BooleanValue = false;
        field.EnumValue = string.Empty;
        field.FormKeyText = string.Empty;
        field.Translations.Clear();
        foreach (var flag in field.Flags)
        {
            flag.IsSelected = false;
        }

        var kind = Text(value?["kind"]);
        if (value is null || kind is null or "Null")
        {
            field.ClearValue = field.IsNullable;
            return;
        }

        switch (kind)
        {
            case "String":
                field.StringValue = Text(value["string"]) ?? string.Empty;
                break;
            case "Boolean":
                field.BooleanValue = ReadBool(value["boolean"]);
                break;
            case "SignedInteger":
                field.NumberText = Text(value["signedInteger"]) ?? string.Empty;
                break;
            case "UnsignedInteger":
                field.NumberText = Text(value["unsignedInteger"]) ?? string.Empty;
                break;
            case "FloatingPoint":
                field.NumberText = Text(value["floatingPoint"]) ?? string.Empty;
                break;
            case "Enum":
                var name = Text(value["enumName"]) ?? string.Empty;
                if (field.IsFlags)
                {
                    var selected = name.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    foreach (var flag in field.Flags)
                    {
                        flag.IsSelected = selected.Contains(flag.Name, StringComparer.Ordinal);
                    }
                }
                else
                {
                    field.EnumValue = name;
                }

                break;
            case "FormLink":
                field.FormKeyText = Text(value["formKey"]) ?? string.Empty;
                break;
            case "TranslatedString":
                field.TargetLanguage = Text(value["targetLanguage"]) ?? field.TargetLanguage;
                if (value["translations"] is JsonObject translations)
                {
                    foreach (var pair in translations.OrderBy(pair => pair.Key, StringComparer.Ordinal))
                    {
                        field.Translations.Add(new WorkbenchTranslationRow(
                            pair.Key,
                            Text(pair.Value) ?? string.Empty,
                            field.Languages,
                            field.RemoveTranslation));
                    }
                }

                break;
        }
    }

    private static string? Validate(WorkbenchFieldDraft field)
    {
        if (field.ClearValue)
        {
            return field.IsNullable ? null : "Field '" + field.Path + "' does not accept null.";
        }

        switch (field.Kind)
        {
            case "String":
                return field.IsNullable || field.StringValue.Length > 0
                    ? null
                    : "Field '" + field.Path + "' requires a value.";
            case "SignedInteger":
                return field.NumberText.Length == 0
                    ? NullableOrRequired(field)
                    : long.TryParse(field.NumberText, NumberStyles.Integer, CultureInfo.InvariantCulture, out _)
                        ? null
                        : "Field '" + field.Path + "' value '" + field.NumberText + "' is not a signed integer.";
            case "UnsignedInteger":
                return field.NumberText.Length == 0
                    ? NullableOrRequired(field)
                    : ulong.TryParse(field.NumberText, NumberStyles.Integer, CultureInfo.InvariantCulture, out _)
                        ? null
                        : "Field '" + field.Path + "' value '" + field.NumberText + "' is not an unsigned integer.";
            case "FloatingPoint":
                return field.NumberText.Length == 0
                    ? NullableOrRequired(field)
                    : double.TryParse(field.NumberText, NumberStyles.Float, CultureInfo.InvariantCulture, out var number) && double.IsFinite(number)
                        ? null
                        : "Field '" + field.Path + "' value '" + field.NumberText + "' is not a finite number.";
            case "Enum":
                var names = EnumNames(field);
                if (names.Count == 0)
                {
                    return NullableOrRequired(field);
                }

                if (!field.IsFlags && names.Count != 1)
                {
                    return "Field '" + field.Path + "' accepts one enum name.";
                }

                var illegal = names.FirstOrDefault(name => !field.Choices.Contains(name, StringComparer.Ordinal));
                return illegal is null
                    ? null
                    : "Field '" + field.Path + "' value '" + illegal + "' is not one of the schema choices.";
            case "FormLink":
                if (field.FormKeyText.Length == 0)
                {
                    return null;
                }

                return FormKeyPattern.IsMatch(field.FormKeyText)
                    ? null
                    : "Field '" + field.Path + "' value '" + field.FormKeyText + "' is not a FormKey.";
            case "TranslatedString":
                return ValidateTranslation(field);
            default:
                return null;
        }
    }

    private static string? ValidateTranslation(WorkbenchFieldDraft field)
    {
        var rows = field.Translations.Where(row => row.Value.Length > 0).ToArray();
        if (rows.Length == 0)
        {
            return NullableOrRequired(field);
        }

        if (rows.Select(row => row.Language).Distinct(StringComparer.Ordinal).Count() != rows.Length)
        {
            return "Field '" + field.Path + "' uses the same language more than once.";
        }

        foreach (var row in rows)
        {
            if (!field.Languages.Contains(row.Language, StringComparer.Ordinal))
            {
                return "Field '" + field.Path + "' language '" + row.Language + "' is not recognized.";
            }
        }

        if (!field.Languages.Contains(field.TargetLanguage, StringComparer.Ordinal))
        {
            return "Field '" + field.Path + "' target language '" + field.TargetLanguage + "' is not recognized.";
        }

        if (!rows.Any(row => string.Equals(row.Language, field.TargetLanguage, StringComparison.Ordinal)))
        {
            return "Field '" + field.Path + "' needs a value for target language " + field.TargetLanguage + ".";
        }

        return null;
    }

    private static string? NullableOrRequired(WorkbenchFieldDraft field)
    {
        return field.IsNullable ? null : "Field '" + field.Path + "' requires a value.";
    }

    private static string Canonical(WorkbenchFieldDraft field)
    {
        return Value(field).ToJsonString();
    }

    private static JsonObject Value(WorkbenchFieldDraft field)
    {
        if (!field.IsSupported || field.ClearValue)
        {
            return NullValue();
        }

        switch (field.Kind)
        {
            case "String":
                return field.StringValue.Length == 0
                    ? NullValue()
                    : new JsonObject { ["kind"] = "String", ["string"] = field.StringValue };
            case "Boolean":
                return new JsonObject { ["kind"] = "Boolean", ["boolean"] = field.BooleanValue };
            case "SignedInteger":
                return long.TryParse(field.NumberText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var signed)
                    ? new JsonObject { ["kind"] = "SignedInteger", ["signedInteger"] = signed }
                    : NullValue();
            case "UnsignedInteger":
                return ulong.TryParse(field.NumberText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var unsigned)
                    ? new JsonObject { ["kind"] = "UnsignedInteger", ["unsignedInteger"] = unsigned }
                    : NullValue();
            case "FloatingPoint":
                return double.TryParse(field.NumberText, NumberStyles.Float, CultureInfo.InvariantCulture, out var floating) && double.IsFinite(floating)
                    ? new JsonObject { ["kind"] = "FloatingPoint", ["floatingPoint"] = floating }
                    : NullValue();
            case "Enum":
                var names = EnumNames(field);
                return names.Count == 0
                    ? NullValue()
                    : new JsonObject { ["kind"] = "Enum", ["enumName"] = string.Join(", ", names) };
            case "FormLink":
                return field.FormKeyText.Length == 0
                    ? NullValue()
                    : new JsonObject { ["kind"] = "FormLink", ["formKey"] = field.FormKeyText };
            case "TranslatedString":
                var rows = field.Translations.Where(row => row.Value.Length > 0)
                    .OrderBy(row => row.Language, StringComparer.Ordinal)
                    .ToArray();
                if (rows.Length == 0)
                {
                    return NullValue();
                }

                var translations = new JsonObject();
                foreach (var row in rows)
                {
                    translations[row.Language] = row.Value;
                }

                return new JsonObject
                {
                    ["kind"] = "TranslatedString",
                    ["targetLanguage"] = field.TargetLanguage,
                    ["translations"] = translations,
                };
            default:
                return NullValue();
        }
    }

    private static List<string> EnumNames(WorkbenchFieldDraft field)
    {
        if (field.IsFlags)
        {
            return field.Flags.Where(flag => flag.IsSelected).Select(flag => flag.Name).Order(StringComparer.Ordinal).ToList();
        }

        return field.EnumValue.Length == 0 ? [] : [field.EnumValue];
    }

    private static JsonObject NullValue()
    {
        return new JsonObject { ["kind"] = "Null" };
    }

    private static string CanonicalValue(JsonObject? value)
    {
        if (value is null || Text(value["kind"]) is null or "Null")
        {
            return NullValue().ToJsonString();
        }

        var kind = Text(value["kind"]) ?? string.Empty;
        var normalized = new JsonObject { ["kind"] = kind };
        switch (kind)
        {
            case "String":
                normalized["string"] = Text(value["string"]) ?? string.Empty;
                break;
            case "Boolean":
                normalized["boolean"] = ReadBool(value["boolean"]);
                break;
            case "SignedInteger":
                normalized["signedInteger"] = Text(value["signedInteger"]) ?? "0";
                break;
            case "UnsignedInteger":
                normalized["unsignedInteger"] = Text(value["unsignedInteger"]) ?? "0";
                break;
            case "FloatingPoint":
                normalized["floatingPoint"] = Text(value["floatingPoint"]) ?? "0";
                break;
            case "Enum":
                var names = (Text(value["enumName"]) ?? string.Empty)
                    .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                    .Order(StringComparer.Ordinal);
                normalized["enumName"] = string.Join(", ", names);
                break;
            case "FormLink":
                normalized["formKey"] = Text(value["formKey"]) ?? string.Empty;
                break;
            case "TranslatedString":
                normalized["targetLanguage"] = Text(value["targetLanguage"]) ?? string.Empty;
                var translations = new JsonObject();
                if (value["translations"] is JsonObject map)
                {
                    foreach (var pair in map.OrderBy(pair => pair.Key, StringComparer.Ordinal))
                    {
                        translations[pair.Key] = Text(pair.Value) ?? string.Empty;
                    }
                }

                normalized["translations"] = translations;
                break;
            default:
                return value.ToJsonString();
        }

        return normalized.ToJsonString();
    }

    private static string DescribeTranslation(JsonObject? value)
    {
        if (value is null)
        {
            return "Name was not returned.";
        }

        if (value["translations"] is not JsonObject translations)
        {
            return "Name kind " + (Text(value["kind"]) ?? "missing") + ".";
        }

        var parts = translations.OrderBy(pair => pair.Key, StringComparer.Ordinal)
            .Select(pair => pair.Key + "=\"" + (Text(pair.Value) ?? string.Empty) + "\"");
        return "Name target " + (Text(value["targetLanguage"]) ?? "missing") + "; " + string.Join("; ", parts) + ".";
    }

    private static JsonObject? Content(JsonObject result)
    {
        return result["structuredContent"] as JsonObject ?? result;
    }

    private static IReadOnlyList<string> ReadStrings(JsonNode? node)
    {
        if (node is not JsonArray array)
        {
            return [];
        }

        return array.Select(Text).Where(value => !string.IsNullOrWhiteSpace(value)).Select(value => value!).ToArray();
    }

    private static bool ReadBool(JsonNode? node)
    {
        return node is JsonValue value && value.TryGetValue<bool>(out var flag) && flag;
    }

    private static string? Text(JsonNode? node)
    {
        if (node is not JsonValue value)
        {
            return null;
        }

        if (value.TryGetValue<string>(out var text))
        {
            return text;
        }

        if (value.TryGetValue<long>(out var signed))
        {
            return signed.ToString(CultureInfo.InvariantCulture);
        }

        if (value.TryGetValue<ulong>(out var unsigned))
        {
            return unsigned.ToString(CultureInfo.InvariantCulture);
        }

        if (value.TryGetValue<double>(out var number))
        {
            return number.ToString(CultureInfo.InvariantCulture);
        }

        if (value.TryGetValue<int>(out var integer))
        {
            return integer.ToString(CultureInfo.InvariantCulture);
        }

        return null;
    }
}
