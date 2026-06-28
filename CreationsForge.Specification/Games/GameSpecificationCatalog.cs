using CreationsForge.Specification.Records;

namespace CreationsForge.Specification.Games;

/// <summary>
/// Exposes the games currently represented by production specification metadata.
/// </summary>
public static class GameSpecificationCatalog
{
    /// <summary>
    /// Gets the Starfield game specification.
    /// </summary>
    public static GameSpecification Starfield { get; } = new()
    {
        Game = SpecificationGame.Starfield,
        Name = nameof(SpecificationGame.Starfield),
        DisplayName = "Starfield"
    };

    /// <summary>
    /// Gets the Fallout 4 game specification.
    /// </summary>
    public static GameSpecification Fallout4 { get; } = new()
    {
        Game = SpecificationGame.Fallout4,
        Name = nameof(SpecificationGame.Fallout4),
        DisplayName = "Fallout 4"
    };

    /// <summary>
    /// Gets the Skyrim game specification.
    /// </summary>
    public static GameSpecification Skyrim { get; } = new()
    {
        Game = SpecificationGame.Skyrim,
        Name = nameof(SpecificationGame.Skyrim),
        DisplayName = "Skyrim"
    };

    /// <summary>
    /// Gets every game specification in deterministic display order.
    /// </summary>
    public static IReadOnlyList<GameSpecification> All { get; } =
    [
        Starfield,
        Fallout4,
        Skyrim
    ];

    /// <summary>
    /// Finds the specification for a game identifier.
    /// </summary>
    /// <param name="game">The game identifier to locate.</param>
    /// <returns>The matching game specification, or <c>null</c> when it is unknown.</returns>
    public static GameSpecification? Find(SpecificationGame game)
    {
        return All.FirstOrDefault(specification => specification.Game == game);
    }
}
