namespace CreationsForge.Mcp.Protocol;

/// <summary>Transports one closed record value without exposing a Mutagen setter.</summary>
public sealed class McpRecordValueDto
{
    /// <summary>Gets or sets the closed value kind: Null, String, Boolean, SignedInteger, UnsignedInteger, FloatingPoint, Enum, FormLink, List, TranslatedString, or Object.</summary>
    public string Kind { get; set; } = string.Empty;

    /// <summary>Gets or sets the string when <see cref="Kind"/> is String.</summary>
    public string? String { get; set; }

    /// <summary>Gets or sets the Boolean when <see cref="Kind"/> is Boolean.</summary>
    public bool? Boolean { get; set; }

    /// <summary>Gets or sets the signed integer when <see cref="Kind"/> is SignedInteger.</summary>
    public long? SignedInteger { get; set; }

    /// <summary>Gets or sets the unsigned integer when <see cref="Kind"/> is UnsignedInteger.</summary>
    public ulong? UnsignedInteger { get; set; }

    /// <summary>Gets or sets the finite floating-point value when <see cref="Kind"/> is FloatingPoint.</summary>
    public double? FloatingPoint { get; set; }

    /// <summary>Gets or sets the enum or comma-separated flag names when <see cref="Kind"/> is Enum.</summary>
    public string? EnumName { get; set; }

    /// <summary>Gets or sets the FormKey text when <see cref="Kind"/> is FormLink.</summary>
    public string? FormKey { get; set; }

    /// <summary>Gets or sets the ordered items when <see cref="Kind"/> is List.</summary>
    public List<McpRecordValueDto>? Items { get; set; }

    /// <summary>Gets or sets the selected target language when <see cref="Kind"/> is TranslatedString.</summary>
    public string? TargetLanguage { get; set; }

    /// <summary>Gets or sets the language map when <see cref="Kind"/> is TranslatedString.</summary>
    public Dictionary<string, string>? Translations { get; set; }

    /// <summary>Gets or sets the alternative name when <see cref="Kind"/> is Object.</summary>
    public string? Alternative { get; set; }

    /// <summary>Gets or sets the child fields when <see cref="Kind"/> is Object.</summary>
    public Dictionary<string, McpRecordValueDto>? Fields { get; set; }
}
