using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeWire;
using CreationsForge.Services;

namespace CreationsForge.NativeEditing.Schema;

/// <summary>Resolves bounded presentation descriptors from one exact immutable schema catalog.</summary>
public sealed class NativeWireSchemaIndex
{
    /// <summary>The exact catalog context.</summary>
    private readonly NativeFormListWireCatalogContext Context;

    /// <summary>Initializes an index bound to one complete catalog identity.</summary>
    /// <param name="context">The exact resolved wire context.</param>
    public NativeWireSchemaIndex(NativeFormListWireCatalogContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        Context = context;
        Identity = context.Identity;
    }

    /// <summary>Gets the complete catalog identity that keys this index.</summary>
    public NativeWireSchemaCatalogIdentity Identity { get; }

    /// <summary>Gets all exact catalog node keys in deterministic command-first order without resolving graphs.</summary>
    public IReadOnlyList<NativeWireSchemaNodeKey> Nodes => Context.SchemaCatalog.Nodes;

    /// <summary>Resolves one exact command or concrete-type descriptor under operation limits.</summary>
    /// <param name="key">The exact catalog-bound node key.</param>
    /// <param name="limits">The per-operation traversal limits.</param>
    /// <param name="cancellationToken">A token observed throughout schema traversal.</param>
    /// <returns>The resolved closed descriptor or a typed reference, cycle, limit, or schema failure.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    public EngineResult<NativeWireSchemaDescriptor> Resolve(NativeWireSchemaNodeKey key, NativeWireReadLimits limits, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(limits);
        return NativeWireSchemaParser.Resolve(Context.SchemaCatalog, key, limits, cancellationToken);
    }
}
