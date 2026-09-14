namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Requests a guarded recoverable save against an exact workspace and selected-output baseline.
/// </summary>
public sealed class SaveRequest
{
    /// <summary>Initializes a guarded save request.</summary>
    /// <param name="operationId">The non-empty idempotency identifier.</param>
    /// <param name="expectedRevision">The exact expected workspace revision.</param>
    /// <param name="expectedOutputBaseline">The exact selected-output baseline the caller observed.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="operationId"/> is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="expectedOutputBaseline"/> is <see langword="null"/>.</exception>
    public SaveRequest(
        Guid operationId,
        WorkspaceRevision expectedRevision,
        OutputArtifactSetBaseline expectedOutputBaseline)
    {
        if (operationId == Guid.Empty)
        {
            throw new ArgumentException("A save operation requires a non-empty identifier.", nameof(operationId));
        }

        ArgumentNullException.ThrowIfNull(expectedOutputBaseline);
        OperationId = operationId;
        ExpectedRevision = expectedRevision;
        ExpectedOutputBaseline = expectedOutputBaseline;
    }

    /// <summary>Gets the idempotency identifier.</summary>
    public Guid OperationId { get; }

    /// <summary>Gets the exact expected workspace revision.</summary>
    public WorkspaceRevision ExpectedRevision { get; }

    /// <summary>Gets the exact selected-output baseline observed by the caller.</summary>
    public OutputArtifactSetBaseline ExpectedOutputBaseline { get; }
}
