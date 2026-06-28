using CreationsForge.Specification.Records;

namespace CreationsForge.Specification.Games;

/// <summary>
/// Describes a supported game as specification metadata without referencing Core runtime types.
/// </summary>
public sealed class GameSpecification
{
    /// <summary>
    /// Gets the specification-layer game identifier used by record and validation metadata.
    /// </summary>
    public required SpecificationGame Game { get; init; }

    /// <summary>
    /// Gets the stable runtime name used when bridging to Core game enums and persisted game values.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the human-readable game name used by diagnostics and display surfaces.
    /// </summary>
    public required string DisplayName { get; init; }
}
