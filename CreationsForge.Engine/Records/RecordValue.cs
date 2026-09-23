using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Engine.Records;

/// <summary>Represents one closed, UI-neutral value transported to or from a registered field.</summary>
public abstract record RecordValue
{
    private static readonly NullRecordValue NullInstance = new();

    /// <summary>Gets the value's closed transport kind.</summary>
    public abstract RecordValueKind Kind { get; }

    /// <summary>Gets the singleton value representing an explicit null.</summary>
    public static RecordValue Null => NullInstance;

    /// <summary>Creates a string value.</summary>
    /// <param name="value">The non-null string.</param>
    /// <returns>The closed string value.</returns>
    public static RecordValue FromString(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new StringRecordValue(value);
    }

    /// <summary>Creates a Boolean value.</summary>
    /// <param name="value">The Boolean value.</param>
    /// <returns>The closed Boolean value.</returns>
    public static RecordValue FromBoolean(bool value) => new BooleanRecordValue(value);

    /// <summary>Creates a signed integer value.</summary>
    /// <param name="value">The signed integer.</param>
    /// <returns>The closed signed integer value.</returns>
    public static RecordValue FromSignedInteger(long value) => new SignedIntegerRecordValue(value);

    /// <summary>Creates an unsigned integer value.</summary>
    /// <param name="value">The unsigned integer.</param>
    /// <returns>The closed unsigned integer value.</returns>
    public static RecordValue FromUnsignedInteger(ulong value) => new UnsignedIntegerRecordValue(value);

    /// <summary>Creates a floating-point value.</summary>
    /// <param name="value">The finite floating-point value.</param>
    /// <returns>The closed floating-point value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="value"/> is not finite.</exception>
    public static RecordValue FromFloatingPoint(double value)
    {
        if (!double.IsFinite(value))
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "Record floating-point values must be finite.");
        }

        return new FloatingPointRecordValue(value);
    }

    /// <summary>Creates a named enum or flags value.</summary>
    /// <param name="value">The case-sensitive Mutagen enum name, or comma-separated flag names.</param>
    /// <returns>The closed enum value.</returns>
    public static RecordValue FromEnum(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return new EnumRecordValue(value);
    }

    /// <summary>Creates a FormKey reference.</summary>
    /// <param name="formKey">The referenced record identity.</param>
    /// <returns>The closed FormLink value.</returns>
    public static RecordValue FromFormLink(FormKey formKey)
    {
        if (formKey.IsNull)
        {
            throw new ArgumentException("A null FormKey must be represented by RecordValue.Null.", nameof(formKey));
        }

        return new FormLinkRecordValue(formKey);
    }

    /// <summary>Creates an ordered list value without removing duplicates.</summary>
    /// <param name="values">The ordered values.</param>
    /// <returns>The closed list value.</returns>
    public static RecordValue FromList(IEnumerable<RecordValue> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        var snapshot = values.ToArray();
        if (snapshot.Any(value => value is null))
        {
            throw new ArgumentException("Record lists cannot contain a null object; use RecordValue.Null explicitly.", nameof(values));
        }

        return new ListRecordValue(snapshot);
    }

    /// <summary>Creates a translated string whose selected target is represented by the same language map.</summary>
    /// <param name="targetLanguage">The active target language.</param>
    /// <param name="values">All language-to-value entries.</param>
    /// <returns>The closed translated value.</returns>
    public static RecordValue FromTranslatedString(Language targetLanguage, IEnumerable<KeyValuePair<Language, string>> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        var snapshot = values.ToDictionary(pair => pair.Key, pair => pair.Value);
        if (!snapshot.TryGetValue(targetLanguage, out var targetValue) || targetValue is null)
        {
            throw new ArgumentException("The translated language map must contain a non-null value for its target language.", nameof(values));
        }

        if (snapshot.Values.Any(value => value is null))
        {
            throw new ArgumentException("Translated language values cannot be null.", nameof(values));
        }

        return new TranslatedStringRecordValue(targetLanguage, snapshot);
    }

    /// <summary>Creates a bounded nested object or polymorphic alternative.</summary>
    /// <param name="alternative">The registered record object or alternative name.</param>
    /// <param name="fields">The registered child field values.</param>
    /// <returns>The closed object value.</returns>
    public static RecordValue FromObject(string alternative, IEnumerable<KeyValuePair<string, RecordValue>> fields)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(alternative);
        ArgumentNullException.ThrowIfNull(fields);
        var snapshot = fields.ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.Ordinal);
        if (snapshot.Any(pair => string.IsNullOrWhiteSpace(pair.Key) || pair.Value is null))
        {
            throw new ArgumentException("Object fields require non-empty names and explicit non-null RecordValue instances.", nameof(fields));
        }

        return new ObjectRecordValue(alternative, snapshot);
    }

    /// <summary>Represents an explicit null.</summary>
    public sealed record NullRecordValue : RecordValue
    {
        internal NullRecordValue()
        {
        }

        /// <inheritdoc />
        public override RecordValueKind Kind => RecordValueKind.Null;
    }

    /// <summary>Represents a non-null string.</summary>
    public sealed record StringRecordValue : RecordValue
    {
        internal StringRecordValue(string value)
        {
            Value = value;
        }

        /// <summary>Gets the string.</summary>
        public string Value { get; }

        /// <inheritdoc />
        public override RecordValueKind Kind => RecordValueKind.String;
    }

    /// <summary>Represents a Boolean value.</summary>
    public sealed record BooleanRecordValue : RecordValue
    {
        internal BooleanRecordValue(bool value)
        {
            Value = value;
        }

        /// <summary>Gets the Boolean value.</summary>
        public bool Value { get; }

        /// <inheritdoc />
        public override RecordValueKind Kind => RecordValueKind.Boolean;
    }

    /// <summary>Represents a signed integer.</summary>
    public sealed record SignedIntegerRecordValue : RecordValue
    {
        internal SignedIntegerRecordValue(long value)
        {
            Value = value;
        }

        /// <summary>Gets the signed integer.</summary>
        public long Value { get; }

        /// <inheritdoc />
        public override RecordValueKind Kind => RecordValueKind.SignedInteger;
    }

    /// <summary>Represents an unsigned integer.</summary>
    public sealed record UnsignedIntegerRecordValue : RecordValue
    {
        internal UnsignedIntegerRecordValue(ulong value)
        {
            Value = value;
        }

        /// <summary>Gets the unsigned integer.</summary>
        public ulong Value { get; }

        /// <inheritdoc />
        public override RecordValueKind Kind => RecordValueKind.UnsignedInteger;
    }

    /// <summary>Represents a finite floating-point value.</summary>
    public sealed record FloatingPointRecordValue : RecordValue
    {
        internal FloatingPointRecordValue(double value)
        {
            Value = value;
        }

        /// <summary>Gets the floating-point value.</summary>
        public double Value { get; }

        /// <inheritdoc />
        public override RecordValueKind Kind => RecordValueKind.FloatingPoint;
    }

    /// <summary>Represents a named Mutagen enum or flags value.</summary>
    public sealed record EnumRecordValue : RecordValue
    {
        internal EnumRecordValue(string value)
        {
            Value = value;
        }

        /// <summary>Gets the Mutagen enum name or comma-separated flag names.</summary>
        public string Value { get; }

        /// <inheritdoc />
        public override RecordValueKind Kind => RecordValueKind.Enum;
    }

    /// <summary>Represents a FormKey link.</summary>
    public sealed record FormLinkRecordValue : RecordValue
    {
        internal FormLinkRecordValue(FormKey formKey)
        {
            FormKey = formKey;
        }

        /// <summary>Gets the referenced record identity.</summary>
        public FormKey FormKey { get; }

        /// <inheritdoc />
        public override RecordValueKind Kind => RecordValueKind.FormLink;
    }

    /// <summary>Represents an ordered collection with duplicate preservation.</summary>
    public sealed record ListRecordValue : RecordValue
    {
        internal ListRecordValue(IReadOnlyList<RecordValue> values)
        {
            Values = values;
        }

        /// <summary>Gets the ordered values.</summary>
        public IReadOnlyList<RecordValue> Values { get; }

        /// <inheritdoc />
        public override RecordValueKind Kind => RecordValueKind.List;
    }

    /// <summary>Represents one language map and its selected target language.</summary>
    public sealed record TranslatedStringRecordValue : RecordValue
    {
        internal TranslatedStringRecordValue(Language targetLanguage, IReadOnlyDictionary<Language, string> values)
        {
            TargetLanguage = targetLanguage;
            Values = values;
        }

        /// <summary>Gets the selected target language.</summary>
        public Language TargetLanguage { get; }

        /// <summary>Gets all language-to-value entries, including the target language.</summary>
        public IReadOnlyDictionary<Language, string> Values { get; }

        /// <inheritdoc />
        public override RecordValueKind Kind => RecordValueKind.TranslatedString;
    }

    /// <summary>Represents a bounded nested record object or polymorphic alternative.</summary>
    public sealed record ObjectRecordValue : RecordValue
    {
        internal ObjectRecordValue(string alternative, IReadOnlyDictionary<string, RecordValue> fields)
        {
            Alternative = alternative;
            Fields = fields;
        }

        /// <summary>Gets the registered record object or alternative name.</summary>
        public string Alternative { get; }

        /// <summary>Gets the registered child fields.</summary>
        public IReadOnlyDictionary<string, RecordValue> Fields { get; }

        /// <inheritdoc />
        public override RecordValueKind Kind => RecordValueKind.Object;
    }
}
