using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Mcp;

/// <summary>Serializes one operation identity and publishes its exact result references from pre-reserved capacity.</summary>
internal sealed class McpMetadataOperationLease : IAsyncDisposable
{
    /// <summary>The owning metadata store.</summary>
    private readonly McpMetadataStore Store;

    /// <summary>The operation reservation held until this lease is disposed.</summary>
    private readonly McpMetadataStore.OperationReservation Reservation;

    /// <summary>Prevents releasing the operation gate more than once.</summary>
    private int IsDisposed;

    /// <summary>Initializes a lease after its operation gate has been acquired.</summary>
    /// <param name="store">The owning metadata store.</param>
    /// <param name="reservation">The acquired operation reservation.</param>
    internal McpMetadataOperationLease(
        McpMetadataStore store,
        McpMetadataStore.OperationReservation reservation)
    {
        Store = store;
        Reservation = reservation;
    }

    /// <summary>Publishes an exact output association using this operation's reserved capacity.</summary>
    /// <param name="value">The immutable Core association.</param>
    /// <returns>The stable handle, or <see langword="null"/> if the reserved publication bound was exceeded.</returns>
    internal McpMetadataReference? Publish(OutputAssociation value)
    {
        return Store.TryPublish(Reservation, value, McpMetadataKind.OutputAssociation);
    }

    /// <summary>Publishes an exact output baseline using this operation's reserved capacity.</summary>
    /// <param name="value">The immutable complete Core baseline.</param>
    /// <returns>The stable handle, or <see langword="null"/> if the reserved publication bound was exceeded.</returns>
    internal McpMetadataReference? Publish(OutputArtifactSetBaseline value)
    {
        return Store.TryPublish(Reservation, value, McpMetadataKind.OutputBaseline);
    }

    /// <summary>Publishes exact terminal recovery evidence using this operation's reserved capacity.</summary>
    /// <param name="value">The immutable Core evidence.</param>
    /// <returns>The stable handle, or <see langword="null"/> if the reserved publication bound was exceeded.</returns>
    internal McpMetadataReference? Publish(ResolvedOutputEvidence value)
    {
        return Store.TryPublish(Reservation, value, McpMetadataKind.ResolvedOutputEvidence);
    }

    /// <summary>Publishes complete recovery-result details using this operation's reserved capacity.</summary>
    /// <param name="value">The immutable Core recovery result.</param>
    /// <returns>The stable handle, or <see langword="null"/> if the reserved publication bound was exceeded.</returns>
    internal McpMetadataReference? Publish(RecoverSaveResult value)
    {
        return Store.TryPublish(Reservation, value, McpMetadataKind.RecoverSaveResult);
    }

    /// <summary>Publishes complete repair-result details using this operation's reserved capacity.</summary>
    /// <param name="value">The immutable Core repair result.</param>
    /// <returns>The stable handle, or <see langword="null"/> if the reserved publication bound was exceeded.</returns>
    internal McpMetadataReference? Publish(RepairSaveResult value)
    {
        return Store.TryPublish(Reservation, value, McpMetadataKind.RepairSaveResult);
    }

    /// <summary>Releases operation serialization and every unused provisional slot while retaining published entries.</summary>
    /// <returns>A completed value task after the operation gate is released.</returns>
    public ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref IsDisposed, 1) == 0)
        {
            Store.ReleaseOperation(Reservation);
        }

        return ValueTask.CompletedTask;
    }
}
