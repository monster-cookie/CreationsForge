using Mutagen.Bethesda.Strings;

namespace CreationsForge.Core.Engine.PluginInputs;

/// <summary>
/// Combines record string lookups in caller-supplied directory order while preserving Mutagen string keys and languages.
/// </summary>
internal sealed class PluginStringsFolderLookup : IStringsFolderLookup
{
    /// <summary>Plugin directory lookups in first-match priority order.</summary>
    private readonly IReadOnlyList<StringsFolderLookupOverlay> Lookups;

    /// <summary>Initializes an ordered record strings lookup.</summary>
    /// <param name="lookups">At least one concrete plugin lookup in priority order.</param>
    /// <exception cref="ArgumentException">Thrown when the collection is empty or contains a <see langword="null"/> entry.</exception>
    internal PluginStringsFolderLookup(IReadOnlyList<StringsFolderLookupOverlay> lookups)
    {
        ArgumentNullException.ThrowIfNull(lookups);
        if (lookups.Count == 0 || lookups.Any(lookup => lookup is null))
        {
            throw new ArgumentException("At least one non-null record strings lookup is required.", nameof(lookups));
        }

        Lookups = Array.AsReadOnly(lookups.ToArray());
    }

    /// <inheritdoc />
    public IReadOnlyCollection<Language> AvailableLanguages(StringsSource source)
    {
        return Array.AsReadOnly(Lookups
            .SelectMany(lookup => lookup.AvailableLanguages(source))
            .Distinct()
            .OrderBy(language => language)
            .ToArray());
    }

    /// <inheritdoc />
    public bool TryLookup(StringsSource source, Language language, uint key, out string str)
    {
        return TryLookup(source, language, key, out str, out _);
    }

    /// <summary>Looks up one localized value in directory priority order and returns the winning plugin physical source path.</summary>
    /// <param name="source">The record strings file category.</param>
    /// <param name="language">The requested plugin language.</param>
    /// <param name="key">The record string key.</param>
    /// <param name="value">The resolved string value, or an empty string when absent.</param>
    /// <param name="sourcePath">The plugin lookup's source path, or an empty string when absent.</param>
    /// <returns><see langword="true"/> when a configured plugin lookup resolves the key.</returns>
    internal bool TryLookup(
        StringsSource source,
        Language language,
        uint key,
        out string value,
        out string sourcePath)
    {
        foreach (var lookup in Lookups)
        {
            if (lookup.TryLookup(source, language, key, out value!, out sourcePath!))
            {
                return true;
            }
        }

        value = string.Empty;
        sourcePath = string.Empty;
        return false;
    }

    /// <inheritdoc />
    public string? Lookup(StringsSource source, Language language, uint key)
    {
        return TryLookup(source, language, key, out var value) ? value : null;
    }

    /// <inheritdoc />
    public TranslatedString CreateString(StringsSource source, uint key, Language targetLanguage)
    {
        var baseLookup = Lookups.FirstOrDefault(lookup => lookup.AvailableLanguages(source).Count > 0)
            ?? Lookups[0];
        var translated = baseLookup.CreateString(source, key, targetLanguage);
        foreach (var language in AvailableLanguages(source))
        {
            if (TryLookup(source, language, key, out var value))
            {
                translated.Set(language, value);
            }
        }

        return translated;
    }
}
