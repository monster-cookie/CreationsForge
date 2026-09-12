namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Confirms a typed workspace mutation and its resulting revision.
/// </summary>
public sealed class OperationReceipt
{
    /// <summary>Initializes an operation receipt.</summary>
    /// <param name="operationId">The completed operation identifier.</param>
    /// <param name="revision">The resulting workspace revision.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="operationId"/> is empty.</exception>
    public OperationReceipt(Guid operationId, WorkspaceRevision revision)
    {
        if (operationId == Guid.Empty)
        {
            throw new ArgumentException("An operation receipt requires a non-empty identifier.", nameof(operationId));
        }

        OperationId = operationId;
        Revision = revision;
    }

    /// <summary>Gets the completed operation identifier.</summary>
    public Guid OperationId { get; }

    /// <summary>Gets the resulting workspace revision.</summary>
    public WorkspaceRevision Revision { get; }
}
