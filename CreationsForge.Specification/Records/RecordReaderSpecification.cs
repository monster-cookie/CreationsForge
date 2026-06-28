namespace CreationsForge.Specification.Records;

/// <summary>
/// Describes reader-facing metadata for a record family without moving game-specific Mutagen mapping into the
/// specification project.
/// </summary>
public sealed class RecordReaderSpecification
{
    /// <summary>
    /// Gets the <c>PluginRecordSetDTO</c> collection property that receives mapped DTOs for this record family.
    /// </summary>
    public required string PluginRecordSetPropertyName { get; init; }

    /// <summary>
    /// Gets the default Mutagen mod collection property name that game adapters read for this record family.
    /// Game-specific support entries may still describe per-game collection differences when a future adapter needs
    /// them.
    /// </summary>
    public required string DefaultMutagenCollectionName { get; init; }

    /// <summary>
    /// Gets a value indicating whether at least one supported game adapter must use a full binary Mutagen mod for this
    /// record family instead of the overlay-safe mod used by the normal reader path.
    /// </summary>
    public bool RequiresFullBinaryMod => GamesRequiringFullBinaryMod.Count > 0;

    /// <summary>
    /// Gets the supported games that must read this record family from a full binary Mutagen mod. Games absent from
    /// this set continue to use the normal overlay-safe reader path when <see cref="UsesOverlaySafeMod"/> is true.
    /// </summary>
    public IReadOnlySet<SpecificationGame> GamesRequiringFullBinaryMod { get; init; } =
        new HashSet<SpecificationGame>();

    /// <summary>
    /// Gets a value indicating whether the normal game-adapter dispatch path can read this record family through the
    /// overlay-safe Mutagen mod loaded by the reader service.
    /// </summary>
    public bool UsesOverlaySafeMod { get; init; } = true;

    /// <summary>
    /// Gets a value indicating whether a missing reader collection is an expected adapter capability gap rather than a
    /// mapper registration error. Current production specifications keep this false unless a record family has a known
    /// optional collection path.
    /// </summary>
    public bool IsOptionalCollection { get; init; }

    /// <summary>
    /// Gets a value indicating whether current reader behavior still relies on game-adapter mapping code rather than
    /// purely declarative specification metadata.
    /// </summary>
    public bool UsesGameSpecificMapper { get; init; } = true;

    /// <summary>
    /// Determines whether the specified game adapter must use a full binary Mutagen mod for this record family.
    /// </summary>
    /// <param name="game">The game adapter being dispatched by the reader service.</param>
    /// <returns>
    /// <see langword="true"/> when the game adapter should use its full binary mod path; otherwise,
    /// <see langword="false"/>.
    /// </returns>
    public bool RequiresFullBinaryModForGame(SpecificationGame game)
    {
        return GamesRequiringFullBinaryMod.Contains(game);
    }
}
