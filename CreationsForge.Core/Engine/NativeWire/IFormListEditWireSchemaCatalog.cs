using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Core.Engine.NativeWire;

/// <summary>Exposes immutable compiled command and native-type schema nodes for one game and release.</summary>
public interface IFormListEditWireSchemaCatalog
{
    /// <summary>Gets the exact game, release, schema version, and generated content identity.</summary>
    NativeWireSchemaCatalogIdentity Identity { get; }

    /// <summary>Gets all node keys in deterministic command-first generated order.</summary>
    IReadOnlyList<NativeWireSchemaNodeKey> Nodes { get; }

    /// <summary>Reads one detached schema node from this exact immutable catalog.</summary>
    /// <param name="key">The content-bound command or type key obtained from <see cref="Nodes"/>.</param>
    /// <param name="cancellationToken">A token checked before and after detached JSON materialization.</param>
    /// <returns>The detached node, or a typed failure for a stale, foreign, or unknown key.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    EngineResult<NativeWireSchemaNode> ReadNode(
        NativeWireSchemaNodeKey key,
        CancellationToken cancellationToken = default);
}
