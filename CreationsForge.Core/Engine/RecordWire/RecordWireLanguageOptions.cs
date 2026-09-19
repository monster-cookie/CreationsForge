using Mutagen.Bethesda.Strings;

namespace CreationsForge.Core.Engine.RecordWire;

/// <summary>Exposes the exact installed Mutagen language names accepted by translated-string wire codecs.</summary>
public static class RecordWireLanguageOptions
{
    /// <summary>Gets the known language names in stable display order without accepting arbitrary text.</summary>
    /// <returns>The exact symbolic names of the installed language enum.</returns>
    public static IReadOnlyList<string> GetNames()
    {
        return Enum.GetValues<Language>()
            .Select(language => language.ToString())
            .Distinct(StringComparer.Ordinal)
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}
