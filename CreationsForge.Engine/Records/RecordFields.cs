using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Engine.Records;

/// <summary>Builds bounded field registrations for reusable scalar, link, list, enum, and translation shapes.</summary>
public static partial class RecordFields
{
    private static readonly RecordCollectionOperation[] ScalarOperations = [RecordCollectionOperation.Set];
    private static readonly RecordCollectionOperation[] ListOperations =
    [
        RecordCollectionOperation.Set,
        RecordCollectionOperation.Append,
        RecordCollectionOperation.Insert,
        RecordCollectionOperation.RemoveAt,
        RecordCollectionOperation.Clear,
    ];

    /// <summary>Creates a nullable or required record string field.</summary>
    /// <typeparam name="TRecord">The exact mutable record type.</typeparam>
    /// <typeparam name="TGetter">The exact Mutagen getter interface.</typeparam>
    /// <param name="path">The stable field path.</param>
    /// <param name="isNullable">Whether the field accepts an explicit null.</param>
    /// <param name="read">The typed Mutagen read operation.</param>
    /// <param name="write">The typed Mutagen write operation.</param>
    /// <returns>The bounded field registration.</returns>
    public static RecordField<TRecord, TGetter> String<TRecord, TGetter>(
        string path,
        bool isNullable,
        Func<TGetter, string?> read,
        Action<TRecord, string?> write)
        where TRecord : class, IMajorRecord
        where TGetter : class, IMajorRecordGetter
    {
        return new RecordField<TRecord, TGetter>(
            ScalarDescriptor(path, RecordValueKind.String, isNullable),
            getter => read(getter) is { } value ? RecordValue.FromString(value) : RecordValue.Null,
            change => ValidateScalar<StringValue>(change, path, isNullable),
            (record, change) => write(record, change.Value is RecordValue.NullRecordValue ? null : ((RecordValue.StringRecordValue)change.Value!).Value));
    }

    /// <summary>Creates a nullable single-precision field.</summary>
    /// <typeparam name="TRecord">The exact mutable record type.</typeparam>
    /// <typeparam name="TGetter">The exact Mutagen getter interface.</typeparam>
    /// <param name="path">The stable field path.</param>
    /// <param name="read">The typed Mutagen read operation.</param>
    /// <param name="write">The typed Mutagen write operation.</param>
    /// <returns>The bounded field registration.</returns>
    public static RecordField<TRecord, TGetter> NullableSingle<TRecord, TGetter>(
        string path,
        Func<TGetter, float?> read,
        Action<TRecord, float?> write)
        where TRecord : class, IMajorRecord
        where TGetter : class, IMajorRecordGetter
    {
        return FloatingPoint(path, isNullable: true, read, write);
    }

    /// <summary>Creates a required single-precision field.</summary>
    /// <typeparam name="TRecord">The exact mutable record type.</typeparam>
    /// <typeparam name="TGetter">The exact Mutagen getter interface.</typeparam>
    /// <param name="path">The stable field path.</param>
    /// <param name="read">The typed Mutagen read operation.</param>
    /// <param name="write">The typed Mutagen write operation.</param>
    /// <returns>The bounded field registration.</returns>
    public static RecordField<TRecord, TGetter> Single<TRecord, TGetter>(
        string path,
        Func<TGetter, float> read,
        Action<TRecord, float> write)
        where TRecord : class, IMajorRecord
        where TGetter : class, IMajorRecordGetter
    {
        return FloatingPoint<TRecord, TGetter>(
            path,
            isNullable: false,
            getter => read(getter),
            (record, value) => write(record, value!.Value));
    }

    /// <summary>Creates a nullable unsigned 16-bit field.</summary>
    /// <typeparam name="TRecord">The exact mutable record type.</typeparam>
    /// <typeparam name="TGetter">The exact Mutagen getter interface.</typeparam>
    /// <param name="path">The stable field path.</param>
    /// <param name="read">The typed Mutagen read operation.</param>
    /// <param name="write">The typed Mutagen write operation.</param>
    /// <returns>The bounded field registration.</returns>
    public static RecordField<TRecord, TGetter> NullableUInt16<TRecord, TGetter>(
        string path,
        Func<TGetter, ushort?> read,
        Action<TRecord, ushort?> write)
        where TRecord : class, IMajorRecord
        where TGetter : class, IMajorRecordGetter
    {
        return UnsignedInteger<TRecord, TGetter>(path, ushort.MaxValue, isNullable: true, getter => read(getter), (record, value) => write(record, value is null ? null : (ushort)value.Value));
    }

    /// <summary>Creates a nullable unsigned 32-bit field.</summary>
    /// <typeparam name="TRecord">The exact mutable record type.</typeparam>
    /// <typeparam name="TGetter">The exact Mutagen getter interface.</typeparam>
    /// <param name="path">The stable field path.</param>
    /// <param name="read">The typed Mutagen read operation.</param>
    /// <param name="write">The typed Mutagen write operation.</param>
    /// <returns>The bounded field registration.</returns>
    public static RecordField<TRecord, TGetter> NullableUInt32<TRecord, TGetter>(
        string path,
        Func<TGetter, uint?> read,
        Action<TRecord, uint?> write)
        where TRecord : class, IMajorRecord
        where TGetter : class, IMajorRecordGetter
    {
        return UnsignedInteger<TRecord, TGetter>(path, uint.MaxValue, isNullable: true, getter => read(getter), (record, value) => write(record, value is null ? null : (uint)value.Value));
    }

    /// <summary>Creates a required unsigned 32-bit field.</summary>
    /// <typeparam name="TRecord">The exact mutable record type.</typeparam>
    /// <typeparam name="TGetter">The exact Mutagen getter interface.</typeparam>
    /// <param name="path">The stable field path.</param>
    /// <param name="read">The typed Mutagen read operation.</param>
    /// <param name="write">The typed Mutagen write operation.</param>
    /// <returns>The bounded field registration.</returns>
    public static RecordField<TRecord, TGetter> UInt32<TRecord, TGetter>(
        string path,
        Func<TGetter, uint> read,
        Action<TRecord, uint> write)
        where TRecord : class, IMajorRecord
        where TGetter : class, IMajorRecordGetter
    {
        return UnsignedInteger<TRecord, TGetter>(path, uint.MaxValue, isNullable: false, getter => read(getter), (record, value) => write(record, (uint)value!.Value));
    }

    /// <summary>Creates a required unsigned byte field.</summary>
    /// <typeparam name="TRecord">The exact mutable record type.</typeparam>
    /// <typeparam name="TGetter">The exact Mutagen getter interface.</typeparam>
    /// <param name="path">The stable field path.</param>
    /// <param name="read">The typed Mutagen read operation.</param>
    /// <param name="write">The typed Mutagen write operation.</param>
    /// <returns>The bounded field registration.</returns>
    public static RecordField<TRecord, TGetter> Byte<TRecord, TGetter>(
        string path,
        Func<TGetter, byte> read,
        Action<TRecord, byte> write)
        where TRecord : class, IMajorRecord
        where TGetter : class, IMajorRecordGetter
    {
        return UnsignedInteger<TRecord, TGetter>(path, byte.MaxValue, isNullable: false, getter => read(getter), (record, value) => write(record, (byte)value!.Value));
    }

    /// <summary>Creates a required signed 32-bit field.</summary>
    /// <typeparam name="TRecord">The exact mutable record type.</typeparam>
    /// <typeparam name="TGetter">The exact Mutagen getter interface.</typeparam>
    /// <param name="path">The stable field path.</param>
    /// <param name="read">The typed Mutagen read operation.</param>
    /// <param name="write">The typed Mutagen write operation.</param>
    /// <returns>The bounded field registration.</returns>
    public static RecordField<TRecord, TGetter> Int32<TRecord, TGetter>(
        string path,
        Func<TGetter, int> read,
        Action<TRecord, int> write)
        where TRecord : class, IMajorRecord
        where TGetter : class, IMajorRecordGetter
    {
        return new RecordField<TRecord, TGetter>(
            ScalarDescriptor(path, RecordValueKind.SignedInteger, isNullable: false),
            getter => RecordValue.FromSignedInteger(read(getter)),
            change =>
            {
                if (change.Operation != RecordCollectionOperation.Set || change.Index is not null || change.Value is not RecordValue.SignedIntegerRecordValue value)
                {
                    throw new RecordEditingException($"Field '{path}' requires a Set operation with one signed integer and no collection index.");
                }

                if (value.Value is < int.MinValue or > int.MaxValue)
                {
                    throw new RecordEditingException($"Field '{path}' value '{value.Value}' is outside the Int32 range.");
                }
            },
            (record, change) => write(record, (int)((RecordValue.SignedIntegerRecordValue)change.Value!).Value));
    }

    /// <summary>Creates a nullable or required Mutagen enum field with exact named choices.</summary>
    /// <typeparam name="TRecord">The exact mutable record type.</typeparam>
    /// <typeparam name="TGetter">The exact Mutagen getter interface.</typeparam>
    /// <typeparam name="TEnum">The closed Mutagen enum type.</typeparam>
    /// <param name="path">The stable field path.</param>
    /// <param name="isNullable">Whether the field accepts an explicit null.</param>
    /// <param name="read">The typed Mutagen read operation.</param>
    /// <param name="write">The typed Mutagen write operation.</param>
    /// <returns>The bounded field registration.</returns>
    public static RecordField<TRecord, TGetter> Enum<TRecord, TGetter, TEnum>(
        string path,
        bool isNullable,
        Func<TGetter, TEnum?> read,
        Action<TRecord, TEnum?> write)
        where TRecord : class, IMajorRecord
        where TGetter : class, IMajorRecordGetter
        where TEnum : struct, Enum
    {
        var choices = System.Enum.GetNames<TEnum>();
        var isFlags = typeof(TEnum).IsDefined(typeof(FlagsAttribute), inherit: false);
        var descriptor = new RecordFieldDescriptor(
            path,
            RecordValueKind.Enum,
            isNullable,
            isFlags,
            choices,
            null,
            ScalarOperations);
        return new RecordField<TRecord, TGetter>(
            descriptor,
            getter => read(getter) is { } value ? RecordValue.FromEnum(value.ToString()) : RecordValue.Null,
            change => ValidateEnum<TEnum>(change, path, isNullable, isFlags, choices),
            (record, change) => write(record, change.Value is RecordValue.NullRecordValue ? null : System.Enum.Parse<TEnum>(((RecordValue.EnumRecordValue)change.Value!).Value, ignoreCase: false)));
    }

    /// <summary>Creates a nullable FormKey link with explicit target constraints.</summary>
    /// <typeparam name="TRecord">The exact mutable record type.</typeparam>
    /// <typeparam name="TGetter">The exact Mutagen getter interface.</typeparam>
    /// <param name="path">The stable field path.</param>
    /// <param name="referenceTargets">The permitted Mutagen getter interfaces.</param>
    /// <param name="read">The typed Mutagen read operation.</param>
    /// <param name="write">The typed Mutagen write operation.</param>
    /// <returns>The bounded field registration.</returns>
    public static RecordField<TRecord, TGetter> FormLink<TRecord, TGetter>(
        string path,
        IEnumerable<Type> referenceTargets,
        Func<TGetter, FormKey?> read,
        Action<TRecord, FormKey?> write)
        where TRecord : class, IMajorRecord
        where TGetter : class, IMajorRecordGetter
    {
        var targetTypes = referenceTargets.ToArray();
        var targets = targetTypes.Select(type => type.FullName ?? type.Name).ToArray();
        var descriptor = new RecordFieldDescriptor(path, RecordValueKind.FormLink, true, false, null, targets, ScalarOperations);
        return new RecordField<TRecord, TGetter>(
            descriptor,
            getter => read(getter) is { } value && !value.IsNull ? RecordValue.FromFormLink(value) : RecordValue.Null,
            change => ValidateScalar<FormLinkValue>(change, path, isNullable: true),
            (record, change) => write(record, change.Value is RecordValue.NullRecordValue ? null : ((RecordValue.FormLinkRecordValue)change.Value!).FormKey),
            change => change.Value is RecordValue.FormLinkRecordValue value ? [new RecordFormLinkReference(value.FormKey, targetTypes)] : []);
    }

    /// <summary>Creates a nullable or required Mutagen translated-string field with one authoritative language map.</summary>
    /// <typeparam name="TRecord">The exact mutable record type.</typeparam>
    /// <typeparam name="TGetter">The exact Mutagen getter interface.</typeparam>
    /// <param name="path">The stable field path.</param>
    /// <param name="isNullable">Whether the field accepts an explicit null.</param>
    /// <param name="read">The typed Mutagen read operation.</param>
    /// <param name="write">The typed Mutagen write operation.</param>
    /// <returns>The bounded field registration.</returns>
    public static RecordField<TRecord, TGetter> TranslatedString<TRecord, TGetter>(
        string path,
        bool isNullable,
        Func<TGetter, ITranslatedStringGetter?> read,
        Action<TRecord, TranslatedString?> write)
        where TRecord : class, IMajorRecord
        where TGetter : class, IMajorRecordGetter
    {
        var descriptor = ScalarDescriptor(path, RecordValueKind.TranslatedString, isNullable);
        return new RecordField<TRecord, TGetter>(
            descriptor,
            getter => ToRecordValue(read(getter)),
            change => ValidateScalar<TranslatedStringValue>(change, path, isNullable),
            (record, change) => write(record, ToTranslatedString(change.Value)));
    }

    /// <summary>Creates a bounded nested record object or polymorphic field with explicitly named alternatives.</summary>
    /// <typeparam name="TRecord">The exact mutable record type.</typeparam>
    /// <typeparam name="TGetter">The exact Mutagen getter interface.</typeparam>
    /// <param name="path">The stable field path.</param>
    /// <param name="isNullable">Whether the field accepts an explicit null.</param>
    /// <param name="alternatives">The permitted record object alternatives.</param>
    /// <param name="read">The typed Mutagen read operation.</param>
    /// <param name="validateValue">The record-shape validator for a selected alternative.</param>
    /// <param name="write">The typed Mutagen write operation.</param>
    /// <param name="referenceAlternatives">Optional alternative-to-getter-type constraints for contained FormKey values.</param>
    /// <returns>The bounded field registration.</returns>
    public static RecordField<TRecord, TGetter> Object<TRecord, TGetter>(
        string path,
        bool isNullable,
        IEnumerable<string> alternatives,
        Func<TGetter, RecordValue> read,
        Action<RecordValue> validateValue,
        Action<TRecord, RecordValue> write,
        IReadOnlyDictionary<string, Type>? referenceAlternatives = null)
        where TRecord : class, IMajorRecord
        where TGetter : class, IMajorRecordGetter
    {
        var alternativeSnapshot = alternatives.ToArray();
        var referenceTargets = referenceAlternatives?.Values
            .Distinct()
            .Select(type => type.FullName ?? type.Name);
        var descriptor = new RecordFieldDescriptor(
            path,
            RecordValueKind.Object,
            isNullable,
            false,
            null,
            referenceTargets,
            ScalarOperations,
            alternativeSnapshot);
        return new RecordField<TRecord, TGetter>(
            descriptor,
            read,
            change =>
            {
                if (change.Operation != RecordCollectionOperation.Set || change.Index is not null || change.Value is null)
                {
                    throw new RecordEditingException($"Field '{path}' requires a Set operation with one object value and no collection index.");
                }

                if (change.Value is RecordValue.NullRecordValue)
                {
                    if (!isNullable)
                    {
                        throw new RecordEditingException($"Field '{path}' does not accept a null.");
                    }

                    return;
                }

                if (change.Value is not RecordValue.ObjectRecordValue objectValue
                    || !alternativeSnapshot.Contains(objectValue.Alternative, StringComparer.Ordinal))
                {
                    throw new RecordEditingException($"Field '{path}' requires one of its registered record object alternatives.");
                }

                validateValue(objectValue);
            },
            (record, change) => write(record, change.Value!),
            change => GetObjectReferences(change, referenceAlternatives));
    }

    private static RecordField<TRecord, TGetter> FloatingPoint<TRecord, TGetter>(
        string path,
        bool isNullable,
        Func<TGetter, float?> read,
        Action<TRecord, float?> write)
        where TRecord : class, IMajorRecord
        where TGetter : class, IMajorRecordGetter
    {
        return new RecordField<TRecord, TGetter>(
            ScalarDescriptor(path, RecordValueKind.FloatingPoint, isNullable),
            getter => read(getter) is { } value ? RecordValue.FromFloatingPoint(value) : RecordValue.Null,
            change =>
            {
                ValidateScalar<FloatingPointValue>(change, path, isNullable);
                if (change.Value is RecordValue.FloatingPointRecordValue value && (value.Value < -float.MaxValue || value.Value > float.MaxValue))
                {
                    throw new RecordEditingException($"Field '{path}' value '{value.Value}' is outside the Single range.");
                }
            },
            (record, change) => write(record, change.Value is RecordValue.NullRecordValue ? null : (float)((RecordValue.FloatingPointRecordValue)change.Value!).Value));
    }

    private static RecordField<TRecord, TGetter> UnsignedInteger<TRecord, TGetter>(
        string path,
        ulong maximum,
        bool isNullable,
        Func<TGetter, ulong?> read,
        Action<TRecord, ulong?> write)
        where TRecord : class, IMajorRecord
        where TGetter : class, IMajorRecordGetter
    {
        return new RecordField<TRecord, TGetter>(
            ScalarDescriptor(path, RecordValueKind.UnsignedInteger, isNullable),
            getter => read(getter) is { } value ? RecordValue.FromUnsignedInteger(value) : RecordValue.Null,
            change =>
            {
                ValidateScalar<UnsignedIntegerValue>(change, path, isNullable);
                if (change.Value is RecordValue.UnsignedIntegerRecordValue value && value.Value > maximum)
                {
                    throw new RecordEditingException($"Field '{path}' value '{value.Value}' exceeds maximum '{maximum}'.");
                }
            },
            (record, change) => write(record, change.Value is RecordValue.NullRecordValue ? null : ((RecordValue.UnsignedIntegerRecordValue)change.Value!).Value));
    }

    private static RecordFieldDescriptor ScalarDescriptor(string path, RecordValueKind kind, bool isNullable)
    {
        return new RecordFieldDescriptor(path, kind, isNullable, false, null, null, ScalarOperations);
    }

    private static void ValidateScalar<TExpected>(RecordFieldChange change, string path, bool isNullable)
    {
        if (change.Operation != RecordCollectionOperation.Set || change.Index is not null || change.Value is null)
        {
            throw new RecordEditingException($"Field '{path}' requires a Set operation with one value and no collection index.");
        }

        if (change.Value is RecordValue.NullRecordValue)
        {
            if (!isNullable)
            {
                throw new RecordEditingException($"Field '{path}' does not accept a null.");
            }

            return;
        }

        var valid = typeof(TExpected) switch
        {
            var type when type == typeof(StringValue) => change.Value is RecordValue.StringRecordValue,
            var type when type == typeof(FloatingPointValue) => change.Value is RecordValue.FloatingPointRecordValue,
            var type when type == typeof(UnsignedIntegerValue) => change.Value is RecordValue.UnsignedIntegerRecordValue,
            var type when type == typeof(FormLinkValue) => change.Value is RecordValue.FormLinkRecordValue,
            var type when type == typeof(TranslatedStringValue) => change.Value is RecordValue.TranslatedStringRecordValue,
            _ => false,
        };
        if (!valid)
        {
            throw new RecordEditingException($"Field '{path}' expects {typeof(TExpected).Name} instead of '{change.Value.Kind}'.");
        }
    }

    private static void ValidateEnum<TEnum>(
        RecordFieldChange change,
        string path,
        bool isNullable,
        bool isFlags,
        IReadOnlyCollection<string> choices)
        where TEnum : struct, Enum
    {
        if (change.Operation != RecordCollectionOperation.Set || change.Index is not null || change.Value is null)
        {
            throw new RecordEditingException($"Field '{path}' requires a Set operation with one enum value and no collection index.");
        }

        if (change.Value is RecordValue.NullRecordValue)
        {
            if (!isNullable)
            {
                throw new RecordEditingException($"Field '{path}' does not accept a null.");
            }

            return;
        }

        if (change.Value is not RecordValue.EnumRecordValue value)
        {
            throw new RecordEditingException($"Field '{path}' expects a named enum value instead of '{change.Value.Kind}'.");
        }

        var names = value.Value.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (names.Length == 0 || (!isFlags && names.Length != 1) || names.Any(name => !choices.Contains(name, StringComparer.Ordinal)))
        {
            throw new RecordEditingException($"Field '{path}' value '{value.Value}' is not a legal named {typeof(TEnum).Name} value.");
        }
    }

    private static RecordValue ToRecordValue(ITranslatedStringGetter? value)
    {
        if (value is null)
        {
            return RecordValue.Null;
        }

        var values = value.ToArray();
        var targetLanguage = values.Any(pair => pair.Key == value.TargetLanguage && pair.Value is not null)
            ? value.TargetLanguage
            : values
                .Where(pair => pair.Value is not null)
                .OrderBy(pair => pair.Key)
                .Select(pair => (Language?)pair.Key)
                .FirstOrDefault()
                ?? throw new RecordEditingException("The native translated string does not contain any non-null language values.");
        return RecordValue.FromTranslatedString(targetLanguage, values);
    }

    private static TranslatedString? ToTranslatedString(RecordValue? value)
    {
        if (value is RecordValue.NullRecordValue)
        {
            return null;
        }

        var translated = (RecordValue.TranslatedStringRecordValue)value!;
        return new TranslatedString(translated.TargetLanguage, translated.Values);
    }

    private sealed class StringValue;
    private sealed class FloatingPointValue;
    private sealed class UnsignedIntegerValue;
    private sealed class FormLinkValue;
    private sealed class TranslatedStringValue;
}
