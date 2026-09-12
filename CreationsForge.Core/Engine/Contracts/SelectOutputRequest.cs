namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Requests selection or creation of a separate native output for a workspace.</summary>
public sealed class SelectOutputRequest
{
    /// <summary>Initializes an output-selection request.</summary>
    /// <param name="operationId">The non-empty idempotency identifier.</param>
    /// <param name="expectedRevision">The exact workspace revision expected by the caller.</param>
    /// <param name="mode">Whether the destination must be newly created or already exist.</param>
    /// <param name="output">The requested output association.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="operationId"/> is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="output"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="mode"/> is undefined.</exception>
    public SelectOutputRequest(
        Guid operationId,
        WorkspaceRevision expectedRevision,
        OutputSelectionMode mode,
        OutputAssociation output)
    {
        if (operationId == Guid.Empty)
        {
            throw new ArgumentException("An output-selection operation requires a non-empty identifier.", nameof(operationId));
        }

        ArgumentNullException.ThrowIfNull(output);
        if (!Enum.IsDefined(mode))
        {
            throw new ArgumentOutOfRangeException(nameof(mode));
        }

        OperationId = operationId;
        ExpectedRevision = expectedRevision;
        Mode = mode;
        Output = output;
    }

    /// <summary>Gets the idempotency identifier.</summary>
    public Guid OperationId { get; }

    /// <summary>Gets the exact expected workspace revision.</summary>
    public WorkspaceRevision ExpectedRevision { get; }

    /// <summary>Gets whether the destination must be newly created or already exist.</summary>
    public OutputSelectionMode Mode { get; }

    /// <summary>Gets the requested output association.</summary>
    public OutputAssociation Output { get; }
}
