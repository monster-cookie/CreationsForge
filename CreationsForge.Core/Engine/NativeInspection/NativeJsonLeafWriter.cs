using System.Globalization;
using System.Text.Json;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Core.Engine.NativeInspection;

/// <summary>Writes reusable typed native leaf values without reflection or lossy scalar conversion.</summary>
public static class NativeJsonLeafWriter
{
    /// <summary>Writes a nullable native string using unchanged read-view JSON or lossless canonical malformed-UTF-16 encoding.</summary>
    /// <param name="writer">The caller-owned JSON writer.</param>
    /// <param name="value">The exact native string, or <see langword="null"/>.</param>
    /// <param name="context">The optional write context; <see langword="null"/> preserves read-view behavior without allocating a context.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="writer"/> is <see langword="null"/>.</exception>
    public static void WriteString(
        Utf8JsonWriter writer,
        string? value,
        NativeJsonWriteContext? context)
    {
        ArgumentNullException.ThrowIfNull(writer);
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        if (context is null || context.Mode != NativeJsonWriteMode.CanonicalFingerprintV1 || IsWellFormedUtf16(value))
        {
            writer.WriteStringValue(value);
            return;
        }

        context.RecordMalformedUtf16();
        writer.WriteStartObject();
        writer.WriteStartArray("$invalidUtf16");
        foreach (var codeUnit in value)
        {
            writer.WriteNumberValue((ushort)codeUnit);
        }

        writer.WriteEndArray();
        writer.WriteEndObject();
    }

    /// <summary>Writes a single-precision value with invariant text, exact IEEE-754 bits, and a JSON number when finite.</summary>
    /// <param name="writer">The caller-owned JSON writer.</param>
    /// <param name="value">The native single-precision value.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="writer"/> is <see langword="null"/>.</exception>
    public static void WriteSingle(Utf8JsonWriter writer, float value)
    {
        ArgumentNullException.ThrowIfNull(writer);
        writer.WriteStartObject();
        writer.WriteString("text", value.ToString("R", CultureInfo.InvariantCulture));
        writer.WriteString("bits", $"0x{BitConverter.SingleToUInt32Bits(value):X8}");
        if (float.IsFinite(value))
        {
            writer.WriteNumber("number", value);
        }

        writer.WriteEndObject();
    }

    /// <summary>Writes a double-precision value with invariant text, exact IEEE-754 bits, and a JSON number when finite.</summary>
    /// <param name="writer">The caller-owned JSON writer.</param>
    /// <param name="value">The native double-precision value.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="writer"/> is <see langword="null"/>.</exception>
    public static void WriteDouble(Utf8JsonWriter writer, double value)
    {
        ArgumentNullException.ThrowIfNull(writer);
        writer.WriteStartObject();
        writer.WriteString("text", value.ToString("R", CultureInfo.InvariantCulture));
        writer.WriteString("bits", $"0x{BitConverter.DoubleToUInt64Bits(value):X16}");
        if (double.IsFinite(value))
        {
            writer.WriteNumber("number", value);
        }

        writer.WriteEndObject();
    }

    /// <summary>Writes nullable binary data with its exact length and Base64 payload.</summary>
    /// <param name="writer">The caller-owned JSON writer.</param>
    /// <param name="value">The native byte sequence, or <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token checked before copying a potentially long sequence.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="writer"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public static void WriteBytes(
        Utf8JsonWriter writer,
        IReadOnlyList<byte>? value,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(writer);
        cancellationToken.ThrowIfCancellationRequested();
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        var bytes = new byte[value.Count];
        for (var index = 0; index < value.Count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            bytes[index] = value[index];
        }

        writer.WriteStartObject();
        writer.WriteNumber("length", bytes.Length);
        writer.WriteBase64String("base64", bytes);
        writer.WriteEndObject();
    }

    /// <summary>Writes a nullable native FormKey in canonical Mutagen identity form.</summary>
    /// <param name="writer">The caller-owned JSON writer.</param>
    /// <param name="value">The native identity, or <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="writer"/> is <see langword="null"/>.</exception>
    public static void WriteFormKey(Utf8JsonWriter writer, FormKey? value)
    {
        WriteFormKey(writer, value, null);
    }

    /// <summary>Writes a nullable native FormKey in readable or structural canonical identity form.</summary>
    /// <param name="writer">The caller-owned JSON writer.</param>
    /// <param name="value">The native identity, or <see langword="null"/>.</param>
    /// <param name="context">The optional write context; <see langword="null"/> preserves read-view behavior without allocating a context.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="writer"/> is <see langword="null"/>.</exception>
    public static void WriteFormKey(
        Utf8JsonWriter writer,
        FormKey? value,
        NativeJsonWriteContext? context)
    {
        ArgumentNullException.ThrowIfNull(writer);
        if (!value.HasValue)
        {
            writer.WriteNullValue();
            return;
        }

        if (context?.Mode != NativeJsonWriteMode.CanonicalFingerprintV1)
        {
            writer.WriteStringValue(value.Value.ToString());
            return;
        }

        writer.WriteStartObject();
        writer.WritePropertyName("modName");
        WriteString(writer, value.Value.ModKey.Name.ToUpperInvariant(), context);
        writer.WriteNumber("modType", (int)value.Value.ModKey.Type);
        writer.WriteNumber("id", value.Value.ID);
        writer.WriteEndObject();
    }

    /// <summary>Writes a nullable native form link with explicit null-link state and canonical identity.</summary>
    /// <param name="writer">The caller-owned JSON writer.</param>
    /// <param name="value">The native form link, or <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="writer"/> is <see langword="null"/>.</exception>
    public static void WriteFormLink(Utf8JsonWriter writer, IFormLinkGetter? value)
    {
        WriteFormLink(writer, value, null);
    }

    /// <summary>Writes a nullable native form link in readable or structural canonical identity form.</summary>
    /// <param name="writer">The caller-owned JSON writer.</param>
    /// <param name="value">The native form link, or <see langword="null"/>.</param>
    /// <param name="context">The optional write context; <see langword="null"/> preserves read-view behavior without allocating a context.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="writer"/> is <see langword="null"/>.</exception>
    public static void WriteFormLink(
        Utf8JsonWriter writer,
        IFormLinkGetter? value,
        NativeJsonWriteContext? context)
    {
        ArgumentNullException.ThrowIfNull(writer);
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartObject();
        writer.WriteBoolean("isNull", value.IsNull);
        writer.WritePropertyName("formKey");
        WriteFormKey(writer, value.FormKeyNullable, context);
        writer.WriteEndObject();
    }

    /// <summary>Writes a nullable native asset link with its preserved supplied and normalized paths.</summary>
    /// <param name="writer">The caller-owned JSON writer.</param>
    /// <param name="value">The native asset link, or <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="writer"/> is <see langword="null"/>.</exception>
    public static void WriteAssetLink(Utf8JsonWriter writer, IAssetLinkGetter? value)
    {
        WriteAssetLink(writer, value, null);
    }

    /// <summary>Writes a nullable native asset link while preserving every path string under the selected representation.</summary>
    /// <param name="writer">The caller-owned JSON writer.</param>
    /// <param name="value">The native asset link, or <see langword="null"/>.</param>
    /// <param name="context">The optional write context; <see langword="null"/> preserves read-view behavior without allocating a context.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="writer"/> is <see langword="null"/>.</exception>
    public static void WriteAssetLink(
        Utf8JsonWriter writer,
        IAssetLinkGetter? value,
        NativeJsonWriteContext? context)
    {
        ArgumentNullException.ThrowIfNull(writer);
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartObject();
        writer.WriteBoolean("isNull", value.IsNull);
        writer.WritePropertyName("givenPath");
        WriteString(writer, value.GivenPath, context);
        writer.WritePropertyName("dataRelativePath");
        WriteString(writer, value.DataRelativePath.ToString(), context);
        writer.WritePropertyName("extension");
        WriteString(writer, value.Extension, context);
        writer.WriteEndObject();
    }

    /// <summary>Writes a nullable translated string with target language and deterministic language-keyed entries.</summary>
    /// <param name="writer">The caller-owned JSON writer.</param>
    /// <param name="value">The native translated string, or <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token observed while enumerating language entries.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="writer"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public static void WriteTranslatedString(
        Utf8JsonWriter writer,
        ITranslatedStringGetter? value,
        CancellationToken cancellationToken)
    {
        WriteTranslatedString(writer, value, cancellationToken, null);
    }

    /// <summary>Writes a nullable translated string with deterministic language order and representation-aware string values.</summary>
    /// <param name="writer">The caller-owned JSON writer.</param>
    /// <param name="value">The native translated string, or <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token observed while enumerating language entries.</param>
    /// <param name="context">The optional write context; <see langword="null"/> preserves read-view behavior without allocating a context.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="writer"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public static void WriteTranslatedString(
        Utf8JsonWriter writer,
        ITranslatedStringGetter? value,
        CancellationToken cancellationToken,
        NativeJsonWriteContext? context)
    {
        ArgumentNullException.ThrowIfNull(writer);
        cancellationToken.ThrowIfCancellationRequested();
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartObject();
        writer.WriteString("targetLanguage", value.TargetLanguage.ToString());
        writer.WritePropertyName("value");
        WriteString(writer, value.String, context);
        writer.WriteStartArray("translations");
        foreach (var translation in GetOrderedTranslations(value, cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            writer.WriteStartObject();
            writer.WriteString("language", translation.Key.ToString());
            writer.WritePropertyName("value");
            WriteString(writer, translation.Value, context);
            writer.WriteEndObject();
        }

        writer.WriteEndArray();
        writer.WriteEndObject();
    }

    /// <summary>Determines whether every UTF-16 surrogate participates in one well-formed scalar pair.</summary>
    /// <param name="value">The exact string to inspect without normalizing code units.</param>
    /// <returns><see langword="true"/> when the string contains no unpaired surrogate code units.</returns>
    private static bool IsWellFormedUtf16(string value)
    {
        for (var index = 0; index < value.Length; index++)
        {
            var codeUnit = value[index];
            if (char.IsHighSurrogate(codeUnit))
            {
                if (index + 1 >= value.Length || !char.IsLowSurrogate(value[index + 1]))
                {
                    return false;
                }

                index++;
            }
            else if (char.IsLowSurrogate(codeUnit))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>Copies keyed translations into deterministic numeric language order for one response.</summary>
    /// <param name="value">The native translated string to enumerate.</param>
    /// <param name="cancellationToken">A token observed while copying and sorting entries.</param>
    /// <returns>A temporary ordered snapshot that does not retain native state.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    private static IReadOnlyList<KeyValuePair<Language, string>> GetOrderedTranslations(
        ITranslatedStringGetter value,
        CancellationToken cancellationToken)
    {
        var translations = new List<KeyValuePair<Language, string>>(value.NumLanguages);
        foreach (var translation in value)
        {
            cancellationToken.ThrowIfCancellationRequested();
            translations.Add(translation);
        }

        cancellationToken.ThrowIfCancellationRequested();
        translations.Sort(static (left, right) => ((int)left.Key).CompareTo((int)right.Key));
        return translations;
    }
}
