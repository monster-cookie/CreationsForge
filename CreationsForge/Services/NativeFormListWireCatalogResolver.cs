using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeWire;
using CreationsForge.Core.Enums;
using CreationsForge.Services.Interfaces;
using Mutagen.Bethesda;

namespace CreationsForge.Services;

/// <summary>Resolves registered wire services by exact game and native release.</summary>
public sealed class NativeFormListWireCatalogResolver : INativeFormListWireCatalogResolver
{
    /// <summary>The immutable registered codecs.</summary>
    private readonly IReadOnlyList<IFormListEditWireCodec> Codecs;

    /// <summary>The immutable registered schema catalogs.</summary>
    private readonly IReadOnlyList<IFormListEditWireSchemaCatalog> SchemaCatalogs;

    /// <summary>Initializes a resolver from the complete registered wire service sets.</summary>
    /// <param name="codecs">The registered exact command codecs.</param>
    /// <param name="schemaCatalogs">The registered immutable schema catalogs.</param>
    public NativeFormListWireCatalogResolver(
        IEnumerable<IFormListEditWireCodec> codecs,
        IEnumerable<IFormListEditWireSchemaCatalog> schemaCatalogs)
    {
        ArgumentNullException.ThrowIfNull(codecs);
        ArgumentNullException.ThrowIfNull(schemaCatalogs);
        Codecs = Array.AsReadOnly(codecs.ToArray());
        SchemaCatalogs = Array.AsReadOnly(schemaCatalogs.ToArray());
    }

    /// <inheritdoc />
    public EngineResult<NativeFormListWireCatalogContext> Resolve(
        SupportedGame game,
        GameRelease release)
    {
        if (!Enum.IsDefined(game) || !Enum.IsDefined(release))
        {
            return Failure($"Native wire catalog resolution requires defined game and release values; received {game}/{release}.");
        }

        var codecs = Codecs.Where(candidate => candidate.Game == game && candidate.Release == release).ToArray();
        var catalogs = SchemaCatalogs.Where(candidate => candidate.Identity.Game == game && candidate.Identity.Release == release).ToArray();
        if (codecs.Length != 1 || catalogs.Length != 1)
        {
            return Failure($"Native wire catalog resolution requires exactly one codec and one schema catalog for {game}/{release}; found {codecs.Length} codec(s) and {catalogs.Length} catalog(s).");
        }

        try
        {
            return EngineResult<NativeFormListWireCatalogContext>.Success(
                new NativeFormListWireCatalogContext(catalogs[0], codecs[0]));
        }
        catch (ArgumentException exception)
        {
            return Failure(exception.Message);
        }
    }

    /// <summary>Creates a typed unsupported-registration result.</summary>
    /// <param name="message">The complete diagnostic message.</param>
    /// <returns>A failed immutable engine result.</returns>
    private static EngineResult<NativeFormListWireCatalogContext> Failure(string message)
    {
        return EngineResult<NativeFormListWireCatalogContext>.Failure(
            new EngineError(EngineErrorCode.UnsupportedGameRelease, message));
    }
}
