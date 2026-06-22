using System.Globalization;
using System.Text.RegularExpressions;
using CreationsForge.Core.DTOs.Records;

namespace CreationsForge.DataValidationTests.Validation.Specs;

public class ValidationSpecRunner
{
    private static readonly SpriggitSampleResolver SpriggitSampleResolver = new();
    private static readonly DtoReflectionFieldReader DtoFieldReader = new();

    public static IReadOnlyList<string> Validate<TRecord>(ValidationSpec spec, TRecord dto)
        where TRecord : RecordDTO
    {
        var evaluation = Evaluate(spec, dto);
        return evaluation.Diagnostics
            .Concat(evaluation.AssertionCases
                .Where(assertion => !string.Equals(assertion.Actual, assertion.Expected, StringComparison.Ordinal))
                .Select(assertion => assertion.Message))
            .ToList();
    }

    public static IReadOnlyList<ValidationAssertionCase> GetAssertionCases<TRecord>(ValidationSpec spec, TRecord dto)
        where TRecord : RecordDTO
    {
        return Evaluate(spec, dto).AssertionCases;
    }

    public static IReadOnlyList<string> GetCoverageDiagnostics<TRecord>(ValidationSpec spec, TRecord dto)
        where TRecord : RecordDTO
    {
        return Evaluate(spec, dto).Diagnostics;
    }

    private static ValidationEvaluation Evaluate<TRecord>(ValidationSpec spec, TRecord dto)
        where TRecord : RecordDTO
    {
        var specDiagnostics = ValidationSpecValidator.Validate(spec);
        if (specDiagnostics.Count > 0)
        {
            return new ValidationEvaluation(Array.Empty<ValidationAssertionCase>(), specDiagnostics);
        }

        var spriggitFields = SpriggitSampleResolver.LoadFields(spec);
        var dtoFields = DtoFieldReader.Read(dto);
        var matchedSpriggitFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var matchedDtoFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var ignoredSpriggitFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var ignoredDtoFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var diagnostics = new List<string>();
        var assertionCases = new List<ValidationAssertionCase>();

        foreach (var rule in spec.Rules)
        {
            switch (rule.Kind)
            {
                case ValidationRuleKind.Field:
                    ApplyFieldRule(rule, spriggitFields, dtoFields, matchedSpriggitFields, matchedDtoFields, diagnostics, assertionCases);
                    break;
                case ValidationRuleKind.PathPrefix:
                    ApplyPathPrefixRule(rule, spriggitFields, dtoFields, matchedSpriggitFields, matchedDtoFields, diagnostics, assertionCases);
                    break;
                case ValidationRuleKind.FormKeyList:
                    ApplyFormKeyListRule(rule, spriggitFields, dtoFields, matchedSpriggitFields, matchedDtoFields, diagnostics, assertionCases);
                    break;
                case ValidationRuleKind.TranslatedField:
                    ApplyTranslatedFieldRule(rule, spriggitFields, dtoFields, matchedSpriggitFields, matchedDtoFields, diagnostics, assertionCases);
                    break;
                case ValidationRuleKind.SoundSlot:
                    ApplySoundSlotRule(rule, spriggitFields, dtoFields, matchedSpriggitFields, matchedDtoFields, diagnostics, assertionCases);
                    break;
                case ValidationRuleKind.DtoExpectedValue:
                    ApplyDtoExpectedValueRule(rule, dtoFields, matchedDtoFields, diagnostics, assertionCases);
                    break;
                case ValidationRuleKind.DtoDefaultWhenSpriggitAbsent:
                    ApplyDtoDefaultWhenSpriggitAbsentRule(rule, spriggitFields, dtoFields, matchedDtoFields, diagnostics, assertionCases);
                    break;
                case ValidationRuleKind.DtoNonEmpty:
                    ApplyDtoNonEmptyRule(rule, spriggitFields, dtoFields, matchedSpriggitFields, matchedDtoFields, diagnostics);
                    break;
                case ValidationRuleKind.SpriggitAbsent:
                    ApplySpriggitAbsentRule(rule, spriggitFields, diagnostics);
                    break;
                case ValidationRuleKind.IgnoreSpriggit:
                    ignoredSpriggitFields.Add(rule.SpriggitPath);
                    break;
                case ValidationRuleKind.IgnoreDto:
                    ignoredDtoFields.Add(rule.DtoPath);
                    break;
                case ValidationRuleKind.IgnoreDtoPrefix:
                    AddIgnoredDtoPrefix(rule, dtoFields, ignoredDtoFields);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported validation rule kind '" + rule.Kind + "'.");
            }
        }

        ApplySamePathRules(
            spriggitFields,
            dtoFields,
            matchedSpriggitFields,
            matchedDtoFields,
            ignoredSpriggitFields,
            ignoredDtoFields,
            diagnostics,
            assertionCases);
        AddUnmatchedSpriggitDiagnostics(spec, spriggitFields, matchedSpriggitFields, ignoredSpriggitFields, diagnostics);
        AddUnmatchedDtoDiagnostics(spec, dtoFields, spriggitFields, matchedDtoFields, ignoredDtoFields, diagnostics);

        return new ValidationEvaluation(assertionCases, diagnostics);
    }

    private static void ApplyFieldRule(
        ValidationFieldRule rule,
        IReadOnlyDictionary<string, string> spriggitFields,
        IReadOnlyDictionary<string, string> dtoFields,
        ISet<string> matchedSpriggitFields,
        ISet<string> matchedDtoFields,
        IList<string> diagnostics,
        IList<ValidationAssertionCase> assertionCases)
    {
        var hasSpriggitValue = spriggitFields.TryGetValue(rule.SpriggitPath, out var spriggitValue);
        var hasDtoValue = dtoFields.TryGetValue(rule.DtoPath, out var dtoValue);
        if (!hasSpriggitValue && !hasDtoValue)
        {
            return;
        }

        if (!hasSpriggitValue)
        {
            diagnostics.Add("Spriggit field '" + rule.SpriggitPath + "' was missing for DTO field '" + rule.DtoPath + "'.");
            return;
        }

        if (!hasDtoValue)
        {
            diagnostics.Add("DTO field '" + rule.DtoPath + "' was missing for Spriggit field '" + rule.SpriggitPath + "'.");
            return;
        }

        MarkMatched(matchedSpriggitFields, rule.SpriggitPath);
        MarkSpriggitFormKeyObjectAlias(spriggitFields, matchedSpriggitFields, rule.SpriggitPath, spriggitValue!);
        MarkMatched(matchedDtoFields, rule.DtoPath);
        AddAssertionCase(rule.SpriggitPath, spriggitValue!, rule.DtoPath, dtoValue!, rule.Normalizer, assertionCases);
    }

    private static void ApplyPathPrefixRule(
        ValidationFieldRule rule,
        IReadOnlyDictionary<string, string> spriggitFields,
        IReadOnlyDictionary<string, string> dtoFields,
        ISet<string> matchedSpriggitFields,
        ISet<string> matchedDtoFields,
        IList<string> diagnostics,
        IList<ValidationAssertionCase> assertionCases)
    {
        foreach (var spriggitField in spriggitFields.OrderBy(field => field.Key, StringComparer.OrdinalIgnoreCase))
        {
            if (!IsUnderPath(spriggitField.Key, rule.SpriggitPath))
            {
                continue;
            }

            var dtoPath = rule.DtoPath + spriggitField.Key[rule.SpriggitPath.Length..];
            foreach (var replacement in rule.PathReplacements)
            {
                dtoPath = dtoPath.Replace(replacement.Key, replacement.Value, StringComparison.Ordinal);
            }

            MarkMatched(matchedSpriggitFields, spriggitField.Key);
            if (!dtoFields.TryGetValue(dtoPath, out var dtoValue))
            {
                if (IsSpriggitScriptingListItemNameWithoutDtoShape(spriggitField.Key))
                {
                    continue;
                }

                diagnostics.Add("DTO field '" + dtoPath + "' was missing for Spriggit field '" + spriggitField.Key + "'.");
                continue;
            }

            MarkMatched(matchedDtoFields, dtoPath);
            AddAssertionCase(spriggitField.Key, spriggitField.Value, dtoPath, dtoValue, ValidationValueNormalizer.None, assertionCases);
        }
    }

    private static void ApplyFormKeyListRule(
        ValidationFieldRule rule,
        IReadOnlyDictionary<string, string> spriggitFields,
        IReadOnlyDictionary<string, string> dtoFields,
        ISet<string> matchedSpriggitFields,
        ISet<string> matchedDtoFields,
        IList<string> diagnostics,
        IList<ValidationAssertionCase> assertionCases)
    {
        if (spriggitFields.TryGetValue(rule.SpriggitPath + ".Count", out var spriggitCount) &&
            dtoFields.TryGetValue(rule.DtoPath + ".Count", out var dtoCount))
        {
            MarkMatched(matchedSpriggitFields, rule.SpriggitPath + ".Count");
            MarkMatched(matchedDtoFields, rule.DtoPath + ".Count");
            AddAssertionCase(
                rule.SpriggitPath + ".Count",
                spriggitCount,
                rule.DtoPath + ".Count",
                dtoCount,
                ValidationValueNormalizer.None,
                assertionCases);
        }

        foreach (var spriggitField in spriggitFields.OrderBy(field => field.Key, StringComparer.OrdinalIgnoreCase))
        {
            if (!TryGetListIndex(spriggitField.Key, rule.SpriggitPath, out var index))
            {
                continue;
            }

            var dtoPath = rule.DtoPath + "[" + index.ToString(CultureInfo.InvariantCulture) + "]." + rule.ExpectedValue;
            MarkMatched(matchedSpriggitFields, spriggitField.Key);
            MarkSpriggitFormKeyObjectAlias(spriggitFields, matchedSpriggitFields, spriggitField.Key, spriggitField.Value);
            if (!dtoFields.TryGetValue(dtoPath, out var dtoValue))
            {
                diagnostics.Add("DTO field '" + dtoPath + "' was missing for Spriggit field '" + spriggitField.Key + "'.");
                continue;
            }

            MarkMatched(matchedDtoFields, dtoPath);
            AddAssertionCase(spriggitField.Key, spriggitField.Value, dtoPath, dtoValue, ValidationValueNormalizer.None, assertionCases);
        }
    }

    private static void ApplyTranslatedFieldRule(
        ValidationFieldRule rule,
        IReadOnlyDictionary<string, string> spriggitFields,
        IReadOnlyDictionary<string, string> dtoFields,
        ISet<string> matchedSpriggitFields,
        ISet<string> matchedDtoFields,
        IList<string> diagnostics,
        IList<ValidationAssertionCase> assertionCases)
    {
        var spriggitEntries = GetTranslatedEntries(spriggitFields, rule.SpriggitPath);
        var dtoEntries = GetTranslatedEntries(dtoFields, rule.DtoPath);
        if (spriggitEntries.Count == 0 && dtoEntries.Count == 0)
        {
            return;
        }

        MarkTranslatedField(matchedSpriggitFields, spriggitFields, rule.SpriggitPath);
        MarkTranslatedField(matchedDtoFields, dtoFields, rule.DtoPath);

        if (rule.RequireAllTranslatedLanguages &&
            spriggitFields.TryGetValue(rule.SpriggitPath + ".Count", out var spriggitCount) &&
            dtoFields.TryGetValue(rule.DtoPath + ".Count", out var dtoCount))
        {
            AddAssertionCase(
                rule.SpriggitPath + ".Count",
                spriggitCount,
                rule.DtoPath + ".Count",
                dtoCount,
                ValidationValueNormalizer.None,
                assertionCases);
        }

        if (spriggitFields.TryGetValue(rule.SpriggitPath + ".TargetLanguage", out var spriggitTargetLanguage) &&
            dtoFields.TryGetValue(rule.DtoPath + ".TargetLanguage", out var dtoTargetLanguage))
        {
            AddAssertionCase(
                rule.SpriggitPath + ".TargetLanguage",
                spriggitTargetLanguage,
                rule.DtoPath + ".TargetLanguage",
                dtoTargetLanguage,
                ValidationValueNormalizer.None,
                assertionCases);
        }

        if (rule.RequireAllTranslatedLanguages)
        {
            foreach (var spriggitEntry in spriggitEntries.OrderBy(entry => entry.Language, StringComparer.OrdinalIgnoreCase))
            {
                var dtoEntry = dtoEntries.FirstOrDefault(entry =>
                    string.Equals(entry.Language, spriggitEntry.Language, StringComparison.OrdinalIgnoreCase));
                if (dtoEntry == null)
                {
                    diagnostics.Add("DTO translated field '" + rule.DtoPath + "' was missing language '" + spriggitEntry.Language + "'.");
                    continue;
                }

                AddAssertionCase(
                    spriggitEntry.StringPath,
                    spriggitEntry.Value,
                    dtoEntry.StringPath,
                    dtoEntry.Value,
                    rule.Normalizer,
                    assertionCases);
            }

            return;
        }

        foreach (var dtoEntry in dtoEntries.OrderBy(entry => entry.Language, StringComparer.OrdinalIgnoreCase))
        {
            if (spriggitEntries.Any(entry => string.Equals(entry.Language, dtoEntry.Language, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            diagnostics.Add("Spriggit translated field '" + rule.SpriggitPath + "' was missing language '" + dtoEntry.Language + "'.");
        }
    }

    private static void ApplySoundSlotRule(
        ValidationFieldRule rule,
        IReadOnlyDictionary<string, string> spriggitFields,
        IReadOnlyDictionary<string, string> dtoFields,
        ISet<string> matchedSpriggitFields,
        ISet<string> matchedDtoFields,
        IList<string> diagnostics,
        IList<ValidationAssertionCase> assertionCases)
    {
        if (!spriggitFields.TryGetValue(rule.SpriggitPath, out var spriggitValue))
        {
            return;
        }

        var soundIndex = FindSoundIndex(dtoFields, rule.DtoPath);
        if (soundIndex < 0)
        {
            diagnostics.Add("DTO sound slot '" + rule.DtoPath + "' was missing for Spriggit field '" + rule.SpriggitPath + "'.");
            return;
        }

        var dtoPath = "Sounds[" + soundIndex.ToString(CultureInfo.InvariantCulture) + "]." + rule.ExpectedValue;
        if (!dtoFields.TryGetValue(dtoPath, out var dtoValue))
        {
            diagnostics.Add("DTO field '" + dtoPath + "' was missing for Spriggit field '" + rule.SpriggitPath + "'.");
            return;
        }

        MarkMatched(matchedSpriggitFields, rule.SpriggitPath);
        MarkMatched(matchedDtoFields, dtoPath);
        MarkMatched(matchedDtoFields, "Sounds[" + soundIndex.ToString(CultureInfo.InvariantCulture) + "].SoundSlot");
        MarkMatched(matchedDtoFields, "Sounds.Count");
        AddAssertionCase(rule.SpriggitPath, spriggitValue, dtoPath, dtoValue, ValidationValueNormalizer.None, assertionCases);
    }

    private static void ApplyDtoExpectedValueRule(
        ValidationFieldRule rule,
        IReadOnlyDictionary<string, string> dtoFields,
        ISet<string> matchedDtoFields,
        IList<string> diagnostics,
        IList<ValidationAssertionCase> assertionCases)
    {
        if (!dtoFields.TryGetValue(rule.DtoPath, out var dtoValue))
        {
            diagnostics.Add("DTO field '" + rule.DtoPath + "' was missing for expected value '" + rule.ExpectedValue + "'.");
            return;
        }

        MarkMatched(matchedDtoFields, rule.DtoPath);
        assertionCases.Add(new ValidationAssertionCase
        {
            SpriggitPath = string.Empty,
            DtoPath = rule.DtoPath,
            Expected = rule.ExpectedValue,
            Actual = dtoValue,
            Message = "DTO field '" + rule.DtoPath + "' should match expected value '" + rule.ExpectedValue + "'."
        });
    }

    private static void ApplyDtoDefaultWhenSpriggitAbsentRule(
        ValidationFieldRule rule,
        IReadOnlyDictionary<string, string> spriggitFields,
        IReadOnlyDictionary<string, string> dtoFields,
        ISet<string> matchedDtoFields,
        IList<string> diagnostics,
        IList<ValidationAssertionCase> assertionCases)
    {
        if (spriggitFields.Keys.Any(field => IsUnderPath(field, rule.SpriggitPath)))
        {
            return;
        }

        if (!dtoFields.TryGetValue(rule.DtoPath, out var dtoValue))
        {
            diagnostics.Add("DTO field '" + rule.DtoPath + "' was missing for omitted Spriggit default field '" + rule.SpriggitPath + "'.");
            return;
        }

        MarkMatched(matchedDtoFields, rule.DtoPath);
        assertionCases.Add(new ValidationAssertionCase
        {
            SpriggitPath = rule.SpriggitPath,
            DtoPath = rule.DtoPath,
            Expected = rule.ExpectedValue,
            Actual = dtoValue,
            Message = "DTO field '" + rule.DtoPath + "' should match default value '" + rule.ExpectedValue +
                      "' because Spriggit field '" + rule.SpriggitPath + "' was omitted." +
                      System.Environment.NewLine +
                      "Reason: " + rule.Reason
        });
    }

    private static void ApplySpriggitAbsentRule(
        ValidationFieldRule rule,
        IReadOnlyDictionary<string, string> spriggitFields,
        IList<string> diagnostics)
    {
        if (spriggitFields.ContainsKey(rule.SpriggitPath))
        {
            diagnostics.Add("Spriggit field '" + rule.SpriggitPath + "' was expected to be absent.");
        }
    }

    private static void ApplyDtoNonEmptyRule(
        ValidationFieldRule rule,
        IReadOnlyDictionary<string, string> spriggitFields,
        IReadOnlyDictionary<string, string> dtoFields,
        ISet<string> matchedSpriggitFields,
        ISet<string> matchedDtoFields,
        IList<string> diagnostics)
    {
        if (!dtoFields.TryGetValue(rule.DtoPath, out var dtoValue))
        {
            diagnostics.Add("DTO field '" + rule.DtoPath + "' was missing.");
            return;
        }

        if (!string.IsNullOrWhiteSpace(rule.SpriggitPath))
        {
            foreach (var field in spriggitFields.Keys.Where(field => IsUnderPath(field, rule.SpriggitPath)))
            {
                MarkMatched(matchedSpriggitFields, field);
            }
        }

        MarkMatched(matchedDtoFields, rule.DtoPath);
        if (string.IsNullOrWhiteSpace(dtoValue))
        {
            diagnostics.Add("DTO field '" + rule.DtoPath + "' was expected to be non-empty.");
        }
    }

    private static void AddIgnoredDtoPrefix(
        ValidationFieldRule rule,
        IReadOnlyDictionary<string, string> dtoFields,
        ISet<string> ignoredDtoFields)
    {
        foreach (var field in dtoFields.Keys.Where(field => IsUnderPath(field, rule.DtoPath)))
        {
            ignoredDtoFields.Add(field);
        }
    }

    private static void ApplySamePathRules(
        IReadOnlyDictionary<string, string> spriggitFields,
        IReadOnlyDictionary<string, string> dtoFields,
        ISet<string> matchedSpriggitFields,
        ISet<string> matchedDtoFields,
        ISet<string> ignoredSpriggitFields,
        ISet<string> ignoredDtoFields,
        IList<string> diagnostics,
        IList<ValidationAssertionCase> assertionCases)
    {
        foreach (var spriggitField in spriggitFields.OrderBy(field => field.Key, StringComparer.OrdinalIgnoreCase))
        {
            if (matchedSpriggitFields.Contains(spriggitField.Key) ||
                ignoredSpriggitFields.Contains(spriggitField.Key) ||
                ignoredDtoFields.Contains(spriggitField.Key) ||
                !dtoFields.TryGetValue(spriggitField.Key, out var dtoValue))
            {
                continue;
            }

            MarkMatched(matchedSpriggitFields, spriggitField.Key);
            MarkMatched(matchedDtoFields, spriggitField.Key);
            AddAssertionCase(
                spriggitField.Key,
                spriggitField.Value,
                spriggitField.Key,
                dtoValue,
                ValidationValueNormalizer.None,
                assertionCases);
        }
    }

    private static void AddUnmatchedSpriggitDiagnostics(
        ValidationSpec spec,
        IReadOnlyDictionary<string, string> spriggitFields,
        ISet<string> matchedSpriggitFields,
        ISet<string> ignoredSpriggitFields,
        IList<string> diagnostics)
    {
        foreach (var field in spriggitFields.OrderBy(field => field.Key, StringComparer.OrdinalIgnoreCase))
        {
            if (matchedSpriggitFields.Contains(field.Key) || ignoredSpriggitFields.Contains(field.Key))
            {
                continue;
            }

            if (IsEmptySpriggitTranslationTargetLanguage(field.Key, spriggitFields))
            {
                continue;
            }

            diagnostics.Add(
                "No matching CreationsForge reader DTO field was found for Spriggit field '" + field.Key + "'." +
                System.Environment.NewLine +
                "Spriggit value: " + field.Value +
                System.Environment.NewLine +
                "Record: " + spec.FormKey);
        }
    }

    private static void AddUnmatchedDtoDiagnostics(
        ValidationSpec spec,
        IReadOnlyDictionary<string, string> dtoFields,
        IReadOnlyDictionary<string, string> spriggitFields,
        ISet<string> matchedDtoFields,
        ISet<string> ignoredDtoFields,
        IList<string> diagnostics)
    {
        foreach (var field in dtoFields.OrderBy(field => field.Key, StringComparer.OrdinalIgnoreCase))
        {
            if (matchedDtoFields.Contains(field.Key) ||
                ignoredDtoFields.Contains(field.Key) ||
                IsUnmatchedDtoFieldAllowed(field.Key, field.Value, spriggitFields))
            {
                continue;
            }

            diagnostics.Add(
                "No matching Spriggit field was found for CreationsForge reader DTO field '" + field.Key + "'." +
                System.Environment.NewLine +
                "DTO value: " + field.Value +
                System.Environment.NewLine +
                "Record: " + spec.FormKey);
        }
    }

    private static bool IsUnmatchedDtoFieldAllowed(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> spriggitFields)
    {
        if (string.Equals(fieldValue, "Null", StringComparison.OrdinalIgnoreCase))
        {
            return !spriggitFields.ContainsKey(fieldName);
        }

        if (fieldName.EndsWith(".Count", StringComparison.OrdinalIgnoreCase) &&
            int.TryParse(fieldValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var count) &&
            count == 0)
        {
            return !spriggitFields.ContainsKey(fieldName);
        }

        if (IsDefaultDtoValueWithoutSpriggitField(fieldName, fieldValue, spriggitFields))
        {
            return true;
        }

        return false;
    }

    private static bool IsDefaultDtoValueWithoutSpriggitField(
        string fieldName,
        string fieldValue,
        IReadOnlyDictionary<string, string> spriggitFields)
    {
        if (spriggitFields.ContainsKey(fieldName))
        {
            return false;
        }

        if (string.Equals(fieldName, "FormVersion", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (fieldName is "ObjectBounds.First" or "ObjectBounds.Second" &&
            string.Equals(fieldValue, "0, 0, 0", StringComparison.Ordinal))
        {
            return true;
        }

        if (string.Equals(fieldName, "DataSlateType", StringComparison.OrdinalIgnoreCase) &&
            string.Equals(fieldValue, "None", StringComparison.Ordinal))
        {
            return true;
        }

        if (string.Equals(fieldName, "Teaches.RawContent", StringComparison.OrdinalIgnoreCase) &&
            string.Equals(fieldValue, uint.MaxValue.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal))
        {
            return true;
        }

        if (fieldName.Contains(".ListItems[", StringComparison.OrdinalIgnoreCase) &&
            fieldName.EndsWith(".MutagenObjectType", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return false;
    }

    private static bool IsEmptySpriggitTranslationTargetLanguage(
        string fieldName,
        IReadOnlyDictionary<string, string> spriggitFields)
    {
        const string targetLanguageSuffix = ".TargetLanguage";
        if (!fieldName.EndsWith(targetLanguageSuffix, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var rootPath = fieldName[..^targetLanguageSuffix.Length];
        return !spriggitFields.ContainsKey(rootPath + ".Count") &&
               !spriggitFields.Keys.Any(field => field.StartsWith(rootPath + "[", StringComparison.OrdinalIgnoreCase));
    }

    private static void AddAssertionCase(
        string spriggitPath,
        string spriggitValue,
        string dtoPath,
        string dtoValue,
        ValidationValueNormalizer normalizer,
        IList<ValidationAssertionCase> assertionCases)
    {
        var normalizedSpriggitValue = NormalizeValue(spriggitValue, normalizer);
        var normalizedDtoValue = NormalizeValue(dtoValue, normalizer);
        var normalizerText = normalizer == ValidationValueNormalizer.None
            ? string.Empty
            : " after " + normalizer + " normalization";

        assertionCases.Add(new ValidationAssertionCase
        {
            SpriggitPath = spriggitPath,
            DtoPath = dtoPath,
            Expected = normalizedSpriggitValue,
            Actual = normalizedDtoValue,
            Message = "Expected Spriggit field '" + spriggitPath + "' to match DTO field '" + dtoPath + "'" + normalizerText + "." +
            System.Environment.NewLine +
            "Spriggit value: " + spriggitValue +
            System.Environment.NewLine +
            "DTO value: " + dtoValue
        });
    }

    private static string NormalizeValue(string value, ValidationValueNormalizer normalizer)
    {
        return normalizer switch
        {
            ValidationValueNormalizer.BookText => value
                .Replace("\\r\\n", "\r\n", StringComparison.Ordinal)
                .Replace("\\\"", "\"", StringComparison.Ordinal),
            ValidationValueNormalizer.HexInteger => value.StartsWith("0x", StringComparison.OrdinalIgnoreCase) &&
                                                    int.TryParse(value.AsSpan(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var number)
                ? number.ToString(CultureInfo.InvariantCulture)
                : value,
            ValidationValueNormalizer.HexPayload => value.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
                ? value[2..]
                : value,
            ValidationValueNormalizer.ModelFile => value.StartsWith("Meshes\\", StringComparison.OrdinalIgnoreCase)
                ? value
                : "Meshes\\" + value,
            ValidationValueNormalizer.DecimalNumber => double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var doubleValue)
                ? Math.Round(doubleValue, 6).ToString("0.######", CultureInfo.InvariantCulture)
                : value,
            _ => value
        };
    }

    private static bool IsUnderPath(string fieldName, string path)
    {
        return string.Equals(fieldName, path, StringComparison.OrdinalIgnoreCase) ||
               fieldName.StartsWith(path + ".", StringComparison.OrdinalIgnoreCase) ||
               fieldName.StartsWith(path + "[", StringComparison.OrdinalIgnoreCase);
    }

    private static void MarkMatched(ISet<string> fields, string path)
    {
        fields.Add(path);

        var startIndex = 0;
        while (true)
        {
            var listIndex = path.IndexOf('[', startIndex);
            if (listIndex <= 0)
            {
                return;
            }

            fields.Add(path[..listIndex] + ".Count");
            startIndex = listIndex + 1;
        }
    }

    private static void MarkSpriggitFormKeyObjectAlias(
        IReadOnlyDictionary<string, string> spriggitFields,
        ISet<string> matchedSpriggitFields,
        string path,
        string value)
    {
        var separatorIndex = value.IndexOf(':', StringComparison.Ordinal);
        if (separatorIndex <= 0)
        {
            return;
        }

        var objectPath = path + "." + value[..separatorIndex];
        if (spriggitFields.ContainsKey(objectPath))
        {
            MarkMatched(matchedSpriggitFields, objectPath);
        }
    }

    private static bool TryGetListIndex(string fieldName, string rootPath, out int index)
    {
        index = 0;
        var prefix = rootPath + "[";
        if (!fieldName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var indexEnd = fieldName.IndexOf(']', prefix.Length);
        if (indexEnd < 0 || indexEnd != fieldName.Length - 1)
        {
            return false;
        }

        return int.TryParse(fieldName.AsSpan(prefix.Length, indexEnd - prefix.Length), NumberStyles.Integer, CultureInfo.InvariantCulture, out index);
    }

    private static bool IsSpriggitScriptingListItemNameWithoutDtoShape(string fieldName)
    {
        return fieldName.Contains(".Objects[", StringComparison.OrdinalIgnoreCase) &&
               fieldName.EndsWith(".Name", StringComparison.OrdinalIgnoreCase);
    }

    private static IReadOnlyList<TranslatedFieldEntry> GetTranslatedEntries(
        IReadOnlyDictionary<string, string> fields,
        string rootPath)
    {
        var entries = new List<TranslatedFieldEntry>();
        var languagePattern = new Regex("^" + Regex.Escape(rootPath) + "\\[(\\d+)\\]\\.Language$", RegexOptions.IgnoreCase);

        foreach (var field in fields)
        {
            var match = languagePattern.Match(field.Key);
            if (!match.Success)
            {
                continue;
            }

            var index = match.Groups[1].Value;
            var stringPath = rootPath + "[" + index + "].String";
            if (!fields.TryGetValue(stringPath, out var value))
            {
                continue;
            }

            entries.Add(new TranslatedFieldEntry(field.Key, stringPath, field.Value, value));
        }

        return entries;
    }

    private static void MarkTranslatedField(
        ISet<string> matchedFields,
        IReadOnlyDictionary<string, string> fields,
        string rootPath)
    {
        foreach (var field in fields.Keys.Where(field => IsUnderPath(field, rootPath)))
        {
            MarkMatched(matchedFields, field);
        }
    }

    private static int FindSoundIndex(IReadOnlyDictionary<string, string> dtoFields, string soundSlot)
    {
        foreach (var field in dtoFields)
        {
            if (!field.Key.StartsWith("Sounds[", StringComparison.OrdinalIgnoreCase) ||
                !field.Key.EndsWith("].SoundSlot", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (!string.Equals(field.Value, soundSlot, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var indexStart = "Sounds[".Length;
            var indexLength = field.Key.IndexOf(']', indexStart) - indexStart;
            if (indexLength <= 0)
            {
                continue;
            }

            if (int.TryParse(field.Key.AsSpan(indexStart, indexLength), NumberStyles.Integer, CultureInfo.InvariantCulture, out var index))
            {
                return index;
            }
        }

        return -1;
    }

    private sealed class TranslatedFieldEntry
    {
        public TranslatedFieldEntry(string languagePath, string stringPath, string language, string value)
        {
            LanguagePath = languagePath;
            StringPath = stringPath;
            Language = language;
            Value = value;
        }

        public string LanguagePath { get; }

        public string StringPath { get; }

        public string Language { get; }

        public string Value { get; }
    }

    private sealed class ValidationEvaluation
    {
        public ValidationEvaluation(
            IReadOnlyList<ValidationAssertionCase> assertionCases,
            IReadOnlyList<string> diagnostics)
        {
            AssertionCases = assertionCases;
            Diagnostics = diagnostics;
        }

        public IReadOnlyList<ValidationAssertionCase> AssertionCases { get; }

        public IReadOnlyList<string> Diagnostics { get; }
    }
}
