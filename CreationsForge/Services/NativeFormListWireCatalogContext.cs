using CreationsForge.Core.Engine.NativeWire;

namespace CreationsForge.Services;

/// <summary>Pairs one exact immutable wire schema catalog with the codec that decodes its commands.</summary>
public sealed class NativeFormListWireCatalogContext
{
    /// <summary>Initializes a verified exact catalog and codec pair.</summary>
    /// <param name="schemaCatalog">The immutable schema catalog.</param>
    /// <param name="codec">The exact command codec for the same game and release.</param>
    /// <exception cref="ArgumentException">Thrown when the catalog and codec game or release differ.</exception>
    public NativeFormListWireCatalogContext(
        IFormListEditWireSchemaCatalog schemaCatalog,
        IFormListEditWireCodec codec)
    {
        ArgumentNullException.ThrowIfNull(schemaCatalog);
        ArgumentNullException.ThrowIfNull(codec);
        if (schemaCatalog.Identity.Game != codec.Game || schemaCatalog.Identity.Release != codec.Release)
        {
            throw new ArgumentException("The native wire schema catalog and codec must identify the same game and release.");
        }

        SchemaCatalog = schemaCatalog;
        Codec = codec;
        Identity = schemaCatalog.Identity;
    }

    /// <summary>Gets the immutable exact command and native-type schema catalog.</summary>
    public IFormListEditWireSchemaCatalog SchemaCatalog { get; }

    /// <summary>Gets the exact typed command codec paired with the schema catalog.</summary>
    public IFormListEditWireCodec Codec { get; }

    /// <summary>Gets the complete game, release, schema-version, and catalog-content identity.</summary>
    public NativeWireSchemaCatalogIdentity Identity { get; }
}
