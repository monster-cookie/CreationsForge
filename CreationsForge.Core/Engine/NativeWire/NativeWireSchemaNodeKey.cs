namespace CreationsForge.Core.Engine.NativeWire;

/// <summary>Identifies one command or concrete-type node within an immutable schema catalog.</summary>
public sealed class NativeWireSchemaNodeKey
{
    /// <summary>Initializes one content-bound schema node key.</summary>
    /// <param name="catalogId">The SHA-256 identity of the containing catalog.</param>
    /// <param name="kind">Whether the node describes a command or concrete native type.</param>
    /// <param name="name">The stable command discriminator or concrete native type name.</param>
    /// <exception cref="ArgumentException">Thrown when the catalog identity or node name is invalid.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="kind"/> is undefined.</exception>
    public NativeWireSchemaNodeKey(
        string catalogId,
        NativeWireSchemaNodeKind kind,
        string name)
    {
        if (!Enum.IsDefined(kind))
        {
            throw new ArgumentOutOfRangeException(nameof(kind));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        CatalogId = NativeWireSchemaCatalogIdentity.NormalizeCatalogId(catalogId, nameof(catalogId));
        Kind = kind;
        Name = name;
    }

    /// <summary>Gets the canonical content identity of the containing catalog.</summary>
    public string CatalogId { get; }

    /// <summary>Gets whether this node describes a command or concrete native type.</summary>
    public NativeWireSchemaNodeKind Kind { get; }

    /// <summary>Gets the stable command discriminator or concrete native type name.</summary>
    public string Name { get; }
}
