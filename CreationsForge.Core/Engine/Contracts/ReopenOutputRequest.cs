namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Requests replacement of staged output state with a fresh native reopen from its selected association.</summary>
public sealed class ReopenOutputRequest
{
    /// <summary>Initializes an output-reopen request.</summary>
    /// <param name="operationId">The non-empty idempotency identifier.</param>
    /// <param name="expectedRevision">The exact expected workspace revision.</param>
    /// <param name="expectedBaseline">The exact selected-output baseline the caller observed.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="operationId"/> is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="expectedBaseline"/> is <see langword="null"/>.</exception>
    public ReopenOutputRequest(Guid operationId, WorkspaceRevision expectedRevision, OutputArtifactSetBaseline expectedBaseline)
    {
        if (operationId == Guid.Empty)
        {
            throw new ArgumentException("An output-reopen operation requires a non-empty identifier.", nameof(operationId));
        }

        ArgumentNullException.ThrowIfNull(expectedBaseline);
        OperationId = operationId;
        ExpectedRevision = expectedRevision;
        ExpectedBaseline = expectedBaseline;
    }

    /// <summary>Gets the idempotency identifier.</summary>
    public Guid OperationId { get; }

    /// <summary>Gets the exact expected workspace revision.</summary>
    public WorkspaceRevision ExpectedRevision { get; }

    /// <summary>Gets the exact selected-output baseline observed by the caller.</summary>
    public OutputArtifactSetBaseline ExpectedBaseline { get; }
}
