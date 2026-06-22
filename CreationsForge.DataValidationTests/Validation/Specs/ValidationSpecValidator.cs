namespace CreationsForge.DataValidationTests.Validation.Specs;

public static class ValidationSpecValidator
{
    public static IReadOnlyList<string> Validate(ValidationSpec spec)
    {
        var diagnostics = new List<string>();

        if (spec.RecordType == null)
        {
            diagnostics.Add("Validation spec is missing a record type.");
        }

        if (string.IsNullOrWhiteSpace(spec.SampleName))
        {
            diagnostics.Add("Validation spec is missing a Spriggit sample name.");
        }

        if (string.IsNullOrWhiteSpace(spec.FormKey))
        {
            diagnostics.Add("Validation spec is missing a form key.");
        }

        ValidateRules(spec, diagnostics);
        return diagnostics;
    }

    private static void ValidateRules(ValidationSpec spec, IList<string> diagnostics)
    {
        var fieldMappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        for (var index = 0; index < spec.Rules.Count; index++)
        {
            var rule = spec.Rules[index];
            var ruleName = "Rule " + index.ToString(System.Globalization.CultureInfo.InvariantCulture) + " (" + rule.Kind + ")";

            switch (rule.Kind)
            {
                case ValidationRuleKind.Field:
                    RequireSpriggitPath(rule, ruleName, diagnostics);
                    RequireDtoPath(rule, ruleName, diagnostics);
                    ValidateDuplicateFieldMapping(rule, ruleName, fieldMappings, diagnostics);
                    break;
                case ValidationRuleKind.FormKeyObjectField:
                    RequireSpriggitPath(rule, ruleName, diagnostics);
                    RequireDtoPath(rule, ruleName, diagnostics);
                    ValidateDuplicateFieldMapping(rule, ruleName, fieldMappings, diagnostics);
                    break;
                case ValidationRuleKind.PathPrefix:
                    RequireSpriggitPath(rule, ruleName, diagnostics);
                    RequireDtoPath(rule, ruleName, diagnostics);
                    break;
                case ValidationRuleKind.FormKeyList:
                    RequireSpriggitPath(rule, ruleName, diagnostics);
                    RequireDtoPath(rule, ruleName, diagnostics);
                    RequireExpectedValue(rule, ruleName, "DTO leaf path", diagnostics);
                    break;
                case ValidationRuleKind.TranslatedField:
                    RequireSpriggitPath(rule, ruleName, diagnostics);
                    RequireDtoPath(rule, ruleName, diagnostics);
                    break;
                case ValidationRuleKind.SoundSlot:
                    RequireSpriggitPath(rule, ruleName, diagnostics);
                    RequireDtoPath(rule, ruleName, diagnostics);
                    RequireExpectedValue(rule, ruleName, "DTO sound field name", diagnostics);
                    break;
                case ValidationRuleKind.DtoExpectedValue:
                    RequireDtoPath(rule, ruleName, diagnostics);
                    RequireExpectedValue(rule, ruleName, "expected value", diagnostics);
                    break;
                case ValidationRuleKind.DtoDefaultWhenSpriggitAbsent:
                    RequireSpriggitPath(rule, ruleName, diagnostics);
                    RequireDtoPath(rule, ruleName, diagnostics);
                    RequireExpectedValue(rule, ruleName, "expected value", diagnostics);
                    RequireReason(rule, ruleName, diagnostics);
                    break;
                case ValidationRuleKind.DtoNonEmpty:
                    RequireDtoPath(rule, ruleName, diagnostics);
                    break;
                case ValidationRuleKind.SpriggitAbsent:
                    RequireSpriggitPath(rule, ruleName, diagnostics);
                    break;
                case ValidationRuleKind.IgnoreSpriggit:
                    RequireSpriggitPath(rule, ruleName, diagnostics);
                    RequireReason(rule, ruleName, diagnostics);
                    break;
                case ValidationRuleKind.IgnoreDto:
                    RequireDtoPath(rule, ruleName, diagnostics);
                    RequireReason(rule, ruleName, diagnostics);
                    break;
                case ValidationRuleKind.IgnoreDtoPrefix:
                    RequireDtoPath(rule, ruleName, diagnostics);
                    RequireReason(rule, ruleName, diagnostics);
                    break;
                default:
                    diagnostics.Add(ruleName + " has unsupported kind '" + rule.Kind + "'.");
                    break;
            }
        }
    }

    private static void ValidateDuplicateFieldMapping(
        ValidationFieldRule rule,
        string ruleName,
        IDictionary<string, string> fieldMappings,
        IList<string> diagnostics)
    {
        if (string.IsNullOrWhiteSpace(rule.SpriggitPath) || string.IsNullOrWhiteSpace(rule.DtoPath))
        {
            return;
        }

        if (!fieldMappings.TryGetValue(rule.SpriggitPath, out var existingDtoPath))
        {
            fieldMappings[rule.SpriggitPath] = rule.DtoPath;
            return;
        }

        if (!string.Equals(existingDtoPath, rule.DtoPath, StringComparison.OrdinalIgnoreCase))
        {
            diagnostics.Add(
                ruleName + " maps Spriggit path '" + rule.SpriggitPath +
                "' to DTO path '" + rule.DtoPath +
                "', but it was already mapped to '" + existingDtoPath + "'.");
        }
    }

    private static void RequireSpriggitPath(ValidationFieldRule rule, string ruleName, IList<string> diagnostics)
    {
        if (string.IsNullOrWhiteSpace(rule.SpriggitPath))
        {
            diagnostics.Add(ruleName + " is missing a Spriggit path.");
        }
    }

    private static void RequireDtoPath(ValidationFieldRule rule, string ruleName, IList<string> diagnostics)
    {
        if (string.IsNullOrWhiteSpace(rule.DtoPath))
        {
            diagnostics.Add(ruleName + " is missing a DTO path.");
        }
    }

    private static void RequireExpectedValue(
        ValidationFieldRule rule,
        string ruleName,
        string valueName,
        IList<string> diagnostics)
    {
        if (string.IsNullOrWhiteSpace(rule.ExpectedValue))
        {
            diagnostics.Add(ruleName + " is missing a " + valueName + ".");
        }
    }

    private static void RequireReason(ValidationFieldRule rule, string ruleName, IList<string> diagnostics)
    {
        if (string.IsNullOrWhiteSpace(rule.Reason))
        {
            diagnostics.Add(ruleName + " is missing an ignore reason.");
        }
    }
}
