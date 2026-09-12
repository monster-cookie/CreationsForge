using Mutagen.Bethesda;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Strings.DI;

namespace CreationsForge.UnitTests.Engine.Starfield;

/// <summary>
/// Supplies Mutagen's public native encodings when generated localized fixtures are written.
/// </summary>
internal sealed class StarfieldTestEncodingProvider : IMutagenEncodingProvider
{
    /// <summary>
    /// Gets the native encoding for a fixture's game release and language.
    /// </summary>
    /// <param name="release">The fixture's native game release.</param>
    /// <param name="language">The localized string language being written.</param>
    /// <returns>The Mutagen encoding registered for the release and language.</returns>
    public IMutagenEncoding GetEncoding(GameRelease release, Language language)
    {
        return MutagenEncoding.GetEncoding(release, language);
    }
}
