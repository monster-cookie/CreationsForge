using CreationsForge.Engine.Records;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Mcp.Protocol;

/// <summary>Converts closed transport values to and from engine record values.</summary>
internal static class McpValueMapper
{
    private const int MaximumDepth = 32;

    /// <summary>Converts a transport value into an engine value.</summary>
    /// <param name="dto">The transport value.</param>
    /// <returns>The engine value.</returns>
    /// <exception cref="McpContractException">Thrown when the transport shape does not match its kind.</exception>
    public static RecordValue ToRecordValue(McpRecordValueDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        return ToRecordValue(dto, 0);
    }

    /// <summary>Converts an engine value into a transport value.</summary>
    /// <param name="value">The engine value.</param>
    /// <returns>The transport value.</returns>
    public static McpRecordValueDto FromRecordValue(RecordValue value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return value switch
        {
            RecordValue.NullRecordValue => new McpRecordValueDto { Kind = nameof(RecordValueKind.Null) },
            RecordValue.StringRecordValue item => new McpRecordValueDto
            {
                Kind = nameof(RecordValueKind.String),
                String = item.Value,
            },
            RecordValue.BooleanRecordValue item => new McpRecordValueDto
            {
                Kind = nameof(RecordValueKind.Boolean),
                Boolean = item.Value,
            },
            RecordValue.SignedIntegerRecordValue item => new McpRecordValueDto
            {
                Kind = nameof(RecordValueKind.SignedInteger),
                SignedInteger = item.Value,
            },
            RecordValue.UnsignedIntegerRecordValue item => new McpRecordValueDto
            {
                Kind = nameof(RecordValueKind.UnsignedInteger),
                UnsignedInteger = item.Value,
            },
            RecordValue.FloatingPointRecordValue item => new McpRecordValueDto
            {
                Kind = nameof(RecordValueKind.FloatingPoint),
                FloatingPoint = item.Value,
            },
            RecordValue.EnumRecordValue item => new McpRecordValueDto
            {
                Kind = nameof(RecordValueKind.Enum),
                EnumName = item.Value,
            },
            RecordValue.FormLinkRecordValue item => new McpRecordValueDto
            {
                Kind = nameof(RecordValueKind.FormLink),
                FormKey = item.FormKey.ToString(),
            },
            RecordValue.ListRecordValue item => new McpRecordValueDto
            {
                Kind = nameof(RecordValueKind.List),
                Items = item.Values.Select(FromRecordValue).ToList(),
            },
            RecordValue.TranslatedStringRecordValue item => new McpRecordValueDto
            {
                Kind = nameof(RecordValueKind.TranslatedString),
                TargetLanguage = item.TargetLanguage.ToString(),
                Translations = item.Values.ToDictionary(pair => pair.Key.ToString(), pair => pair.Value),
            },
            RecordValue.ObjectRecordValue item => new McpRecordValueDto
            {
                Kind = nameof(RecordValueKind.Object),
                Alternative = item.Alternative,
                Fields = item.Fields.ToDictionary(pair => pair.Key, pair => FromRecordValue(pair.Value)),
            },
            _ => throw new McpContractException(
                "unsupported_shape",
                $"Record value kind '{value.Kind}' cannot be transported."),
        };
    }

    private static RecordValue ToRecordValue(McpRecordValueDto dto, int depth)
    {
        if (dto is null)
        {
            throw new McpContractException("invalid_input", "A nested record value is required.");
        }

        if (depth > MaximumDepth)
        {
            throw new McpContractException("invalid_input", "A record value is nested more than 32 levels deep.");
        }

        if (!McpAuthoringArguments.TryParseDefined<RecordValueKind>(dto.Kind, ignoreCase: false, out var kind))
        {
            throw new McpContractException("invalid_input", $"Record value kind '{dto.Kind}' is not recognized.");
        }

        return kind switch
        {
            RecordValueKind.Null => RecordValue.Null,
            RecordValueKind.String => RecordValue.FromString(Require(dto.String, "String")),
            RecordValueKind.Boolean => RecordValue.FromBoolean(Require(dto.Boolean, "Boolean")),
            RecordValueKind.SignedInteger => RecordValue.FromSignedInteger(Require(dto.SignedInteger, "SignedInteger")),
            RecordValueKind.UnsignedInteger => RecordValue.FromUnsignedInteger(
                Require(dto.UnsignedInteger, "UnsignedInteger")),
            RecordValueKind.FloatingPoint => RecordValue.FromFloatingPoint(Require(dto.FloatingPoint, "FloatingPoint")),
            RecordValueKind.Enum => RecordValue.FromEnum(Require(dto.EnumName, "EnumName")),
            RecordValueKind.FormLink => RecordValue.FromFormLink(ParseFormKey(Require(dto.FormKey, "FormKey"))),
            RecordValueKind.List => RecordValue.FromList(
                Require(dto.Items, "Items").Select(item => ToRecordValue(item, depth + 1))),
            RecordValueKind.TranslatedString => RecordValue.FromTranslatedString(
                ParseLanguage(Require(dto.TargetLanguage, "TargetLanguage")),
                Require(dto.Translations, "Translations").Select(pair =>
                    new KeyValuePair<Language, string>(ParseLanguage(pair.Key), pair.Value))),
            RecordValueKind.Object => RecordValue.FromObject(
                Require(dto.Alternative, "Alternative"),
                Require(dto.Fields, "Fields").Select(pair =>
                    new KeyValuePair<string, RecordValue>(pair.Key, ToRecordValue(pair.Value, depth + 1)))),
            _ => throw new McpContractException("invalid_input", $"Record value kind '{kind}' is not recognized."),
        };
    }

    private static FormKey ParseFormKey(string text)
    {
        if (!FormKey.TryFactory(text.AsSpan(), out var formKey) || formKey.IsNull)
        {
            throw new McpContractException("invalid_input", $"FormKey '{text}' is not valid.");
        }

        return formKey;
    }

    private static Language ParseLanguage(string text)
    {
        if (!McpAuthoringArguments.TryParseDefined<Language>(text, ignoreCase: true, out var language))
        {
            throw new McpContractException("invalid_input", $"Language '{text}' is not recognized.");
        }

        return language;
    }

    private static T Require<T>(T? value, string property)
        where T : class
    {
        if (value is null)
        {
            throw new McpContractException("invalid_input", $"Record value kind requires '{property}'.");
        }

        return value;
    }

    private static T Require<T>(T? value, string property)
        where T : struct
    {
        if (value is null)
        {
            throw new McpContractException("invalid_input", $"Record value kind requires '{property}'.");
        }

        return value.Value;
    }
}

/// <summary>Reports a closed authoring-contract failure before an engine call.</summary>
internal sealed class McpContractException : Exception
{
    /// <summary>Initializes a contract failure.</summary>
    /// <param name="code">The stable domain error code.</param>
    /// <param name="message">The actionable failure message.</param>
    public McpContractException(string code, string message)
        : base(message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        Code = code;
    }

    /// <summary>Gets the stable domain error code.</summary>
    public string Code { get; }
}
