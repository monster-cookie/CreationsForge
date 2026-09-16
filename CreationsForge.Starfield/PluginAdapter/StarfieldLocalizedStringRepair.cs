using System.Collections;
using System.Reflection;
using System.Text;
using CreationsForge.Core.Engine.PluginInputs;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Starfield.PluginAdapter;

/// <summary>Restores localized strings that the Starfield binary overlay materializes as direct encoded string keys.</summary>
internal static class StarfieldLocalizedStringRepair
{
    /// <summary>The Mutagen field that retains the generated record field's localized-string source category.</summary>
    private static readonly FieldInfo? StringsSourceField = typeof(TranslatedString).GetField(
        "StringsSource",
        BindingFlags.Instance | BindingFlags.NonPublic);

    /// <summary>The encoding used by Mutagen when a localized Starfield string key is incorrectly read as direct text.</summary>
    private static readonly Encoding EmbeddedStringEncoding = CreateEmbeddedStringEncoding();

    /// <summary>Restores every reachable dropped localized-string key on one detached Starfield record.</summary>
    /// <param name="record">The detached mutable record whose translated strings may require repair.</param>
    /// <param name="inputs">The validated plugin and localized-string input lifetime.</param>
    /// <param name="plugin">The containing localized plugin descriptor.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is <see langword="null"/>.</exception>
    internal static void Repair(
        IMajorRecordGetter record,
        PluginSourceInputs inputs,
        PluginSourcePluginInput plugin)
    {
        ArgumentNullException.ThrowIfNull(record);
        ArgumentNullException.ThrowIfNull(inputs);
        ArgumentNullException.ThrowIfNull(plugin);
        if (!plugin.UsesLocalization)
        {
            return;
        }

        var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);
        RepairReachableValue(record, inputs, plugin, visited);
    }

    /// <summary>Traverses one detached Starfield value graph and repairs each translated-string instance once.</summary>
    /// <param name="value">The current reachable value.</param>
    /// <param name="inputs">The validated plugin and localized-string input lifetime.</param>
    /// <param name="plugin">The containing localized plugin descriptor.</param>
    /// <param name="visited">Reference identities already traversed.</param>
    private static void RepairReachableValue(
        object? value,
        PluginSourceInputs inputs,
        PluginSourcePluginInput plugin,
        HashSet<object> visited)
    {
        if (value is null || value is string)
        {
            return;
        }

        if (value is TranslatedString translatedString)
        {
            RepairTranslatedString(translatedString, inputs, plugin);
            return;
        }

        var valueType = value.GetType();
        if (valueType.IsPrimitive || valueType.IsEnum ||
            (!valueType.IsValueType && !visited.Add(value)))
        {
            return;
        }

        if (value is IEnumerable values)
        {
            foreach (var item in values)
            {
                RepairReachableValue(item, inputs, plugin, visited);
            }

            return;
        }

        if (valueType.Assembly != typeof(StarfieldMod).Assembly)
        {
            return;
        }

        foreach (var property in valueType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (!property.CanRead || property.GetIndexParameters().Length != 0)
            {
                continue;
            }

            RepairReachableValue(property.GetValue(value), inputs, plugin, visited);
        }
    }

    /// <summary>Replaces one dropped direct key with all translations available from its original string-file category.</summary>
    /// <param name="value">The translated string materialized by Mutagen.</param>
    /// <param name="inputs">The validated plugin and localized-string input lifetime.</param>
    /// <param name="plugin">The containing localized plugin descriptor.</param>
    private static void RepairTranslatedString(
        TranslatedString value,
        PluginSourceInputs inputs,
        PluginSourcePluginInput plugin)
    {
        if (value.StringsKey.HasValue || !TryDecodeStringKey(value.String, out var key))
        {
            return;
        }

        if (StringsSourceField?.GetValue(value) is not StringsSource source)
        {
            return;
        }

        var translations = new List<KeyValuePair<Language, string>>();
        foreach (var language in Enum.GetValues<Language>().Distinct())
        {
            if (inputs.TryLookupString(plugin, source, language, key, out var translation, out _))
            {
                translations.Add(new KeyValuePair<Language, string>(language, translation));
            }
        }

        if (translations.Count == 0)
        {
            return;
        }

        value.Clear();
        foreach (var translation in translations)
        {
            value.Set(translation.Key, translation.Value);
        }
    }

    /// <summary>Reconstructs one little-endian 32-bit string key from Mutagen's direct Windows-1252 decoding.</summary>
    /// <param name="value">The direct string produced from the four-byte key payload.</param>
    /// <param name="key">The reconstructed string-table key.</param>
    /// <returns><see langword="true"/> when the value can represent one nonzero 32-bit key.</returns>
    private static bool TryDecodeStringKey(string? value, out uint key)
    {
        key = 0;
        if (string.IsNullOrEmpty(value))
        {
            return false;
        }

        try
        {
            var bytes = EmbeddedStringEncoding.GetBytes(value);
            if (bytes.Length is < 1 or > sizeof(uint))
            {
                return false;
            }

            for (var index = 0; index < bytes.Length; index++)
            {
                key |= (uint)bytes[index] << (index * 8);
            }

            return key != 0;
        }
        catch (EncoderFallbackException)
        {
            return false;
        }
    }

    /// <summary>Creates the strict encoding used by Mutagen for embedded nonlocalized Starfield strings.</summary>
    /// <returns>A Windows-1252 encoding that rejects values which cannot have originated from one byte.</returns>
    private static Encoding CreateEmbeddedStringEncoding()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        return Encoding.GetEncoding(1252, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
    }
}
