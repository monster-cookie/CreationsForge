using CreationsForge.Core.Enums;
using Mutagen.Bethesda;

namespace CreationsForge.Core.Engine.NativeWire;

/// <summary>Identifies one immutable compiled native wire schema catalog.</summary>
public sealed class NativeWireSchemaCatalogIdentity
{
    /// <summary>Initializes one game, release, schema-version, and content-bound catalog identity.</summary>
    /// <param name="game">The supported CreationsForge game.</param>
    /// <param name="release">The exact native release used by the compiled constructors.</param>
    /// <param name="schemaVersion">The non-empty human-readable wire schema version.</param>
    /// <param name="catalogId">The SHA-256 hexadecimal identity of the generated catalog content.</param>
    /// <exception cref="ArgumentException">Thrown when the schema version or catalog identity is invalid.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when a game or release value is undefined.</exception>
    public NativeWireSchemaCatalogIdentity(
        SupportedGame game,
        GameRelease release,
        string schemaVersion,
        string catalogId)
    {
        if (!Enum.IsDefined(game))
        {
            throw new ArgumentOutOfRangeException(nameof(game));
        }

        if (!Enum.IsDefined(release))
        {
            throw new ArgumentOutOfRangeException(nameof(release));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(schemaVersion);
        Game = game;
        Release = release;
        SchemaVersion = schemaVersion;
        CatalogId = NormalizeCatalogId(catalogId, nameof(catalogId));
    }

    /// <summary>Gets the supported CreationsForge game.</summary>
    public SupportedGame Game { get; }

    /// <summary>Gets the exact native release used by the compiled constructors.</summary>
    public GameRelease Release { get; }

    /// <summary>Gets the human-readable wire schema version.</summary>
    public string SchemaVersion { get; }

    /// <summary>Gets the canonical uppercase SHA-256 identity of the generated catalog content.</summary>
    public string CatalogId { get; }

    /// <summary>Validates and normalizes one SHA-256 catalog identity.</summary>
    /// <param name="catalogId">The supplied hexadecimal identity.</param>
    /// <param name="parameterName">The constructor parameter reported for invalid input.</param>
    /// <returns>The canonical uppercase identity.</returns>
    /// <exception cref="ArgumentException">Thrown when the identity is not exactly 64 hexadecimal characters.</exception>
    internal static string NormalizeCatalogId(string catalogId, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(catalogId);
        if (catalogId.Length != 64 || catalogId.Any(character => !Uri.IsHexDigit(character)))
        {
            throw new ArgumentException(
                "A native wire schema catalog identity must contain exactly 64 hexadecimal characters.",
                parameterName);
        }

        return catalogId.ToUpperInvariant();
    }
}
