using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordWire;
using CreationsForge.Services;

namespace CreationsForge.RecordEditing.Schema;

/// <summary>Resolves bounded presentation descriptors from one exact immutable schema catalog.</summary>
public sealed class RecordWireSchemaIndex
{
    /// <summary>The exact catalog context.</summary>
    private readonly FormListWireCatalogContext Context;

    /// <summary>Initializes an index bound to one complete catalog identity.</summary>
    /// <param name="context">The exact resolved wire context.</param>
    public RecordWireSchemaIndex(FormListWireCatalogContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        Context = context;
        Identity = context.Identity;
    }

    /// <summary>Gets the complete catalog identity that keys this index.</summary>
    public RecordWireSchemaCatalogIdentity Identity { get; }

    /// <summary>Gets all exact catalog node keys in deterministic command-first order without resolving graphs.</summary>
    public IReadOnlyList<RecordWireSchemaNodeKey> Nodes => Context.SchemaCatalog.Nodes;

    /// <summary>Resolves one exact command or concrete-type descriptor under operation limits.</summary>
    /// <param name="key">The exact catalog-bound node key.</param>
    /// <param name="limits">The per-operation traversal limits.</param>
    /// <param name="cancellationToken">A token observed throughout schema traversal.</param>
    /// <returns>The resolved closed descriptor or a typed reference, cycle, limit, or schema failure.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    public EngineResult<RecordWireSchemaDescriptor> Resolve(RecordWireSchemaNodeKey key, RecordWireReadLimits limits, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(limits);
        return RecordWireSchemaParser.Resolve(Context.SchemaCatalog, key, limits, cancellationToken);
    }
}
