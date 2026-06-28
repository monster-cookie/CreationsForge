using CreationsForge.Core.Enums;
using CreationsForge.Specification.Records;

namespace CreationsForge.Core.Helpers;

/// <summary>
/// Converts between specification-layer game identifiers and Core runtime game identifiers.
/// </summary>
public static class SpecificationGameAdapter
{
    /// <summary>
    /// Converts a specification game identifier to the matching Core runtime game identifier.
    /// </summary>
    /// <param name="game">The specification game identifier.</param>
    /// <returns>The matching Core game identifier.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="game"/> is unknown.</exception>
    public static SupportedGame ToSupportedGame(SpecificationGame game)
    {
        return game switch
        {
            SpecificationGame.Starfield => SupportedGame.Starfield,
            SpecificationGame.Fallout4 => SupportedGame.Fallout4,
            SpecificationGame.Skyrim => SupportedGame.Skyrim,
            _ => throw new ArgumentOutOfRangeException(nameof(game), game, "Unsupported specification game.")
        };
    }

    /// <summary>
    /// Converts a Core runtime game identifier to the matching specification game identifier.
    /// </summary>
    /// <param name="game">The Core game identifier.</param>
    /// <returns>The matching specification game identifier.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="game"/> is unknown.</exception>
    public static SpecificationGame ToSpecificationGame(SupportedGame game)
    {
        return game switch
        {
            SupportedGame.Starfield => SpecificationGame.Starfield,
            SupportedGame.Fallout4 => SpecificationGame.Fallout4,
            SupportedGame.Skyrim => SpecificationGame.Skyrim,
            _ => throw new ArgumentOutOfRangeException(nameof(game), game, "Unsupported Core game.")
        };
    }
}
