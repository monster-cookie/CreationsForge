namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Requests explicit evidence-validated recovery adoption by a live workspace.</summary>
public sealed class ResolveOutputRecoveryRequest
{
    /// <summary>Initializes an immutable output-recovery adoption request.</summary>
    /// <param name="operationId">The new non-empty idempotency identifier for adoption.</param>
    /// <param name="expectedRevision">The current live workspace revision expected by the caller.</param>
    /// <param name="mode">Whether to resume the original candidate or reopen the terminal output.</param>
    /// <param name="evidence">The terminal recovery evidence to revalidate against coordinator-owned metadata.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="operationId"/> is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="evidence"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="mode"/> is undefined.</exception>
    public ResolveOutputRecoveryRequest(
        Guid operationId,
        WorkspaceRevision expectedRevision,
        OutputRecoveryAdoptionMode mode,
        ResolvedOutputEvidence evidence)
    {
        if (operationId == Guid.Empty)
        {
            throw new ArgumentException("Output recovery adoption requires a non-empty operation identifier.", nameof(operationId));
        }

        if (!Enum.IsDefined(mode))
        {
            throw new ArgumentOutOfRangeException(nameof(mode));
        }

        ArgumentNullException.ThrowIfNull(evidence);
        OperationId = operationId;
        ExpectedRevision = expectedRevision;
        Mode = mode;
        Evidence = evidence;
    }

    /// <summary>Gets the idempotency identifier for this adoption decision.</summary>
    public Guid OperationId { get; }

    /// <summary>Gets the exact current live workspace revision expected by the caller.</summary>
    public WorkspaceRevision ExpectedRevision { get; }

    /// <summary>Gets whether to resume the original candidate or reopen the terminal output.</summary>
    public OutputRecoveryAdoptionMode Mode { get; }

    /// <summary>Gets the terminal recovery evidence to revalidate against coordinator-owned metadata.</summary>
    public ResolvedOutputEvidence Evidence { get; }
}
