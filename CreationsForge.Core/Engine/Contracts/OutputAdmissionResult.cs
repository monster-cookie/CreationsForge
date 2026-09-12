namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Reports whether output admission may proceed or an unresolved journal blocks native opening.</summary>
public sealed class OutputAdmissionResult
{
    /// <summary>Initializes an immutable output admission result.</summary>
    /// <param name="status">The ready or recovery-required admission state.</param>
    /// <param name="unresolvedSave">The original unresolved save when recovery is required; otherwise <see langword="null"/>.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="status"/> and <paramref name="unresolvedSave"/> disagree.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="status"/> is undefined or cannot be produced by admission.</exception>
    public OutputAdmissionResult(
        OutputSynchronizationStatus status,
        PendingSaveIdentity? unresolvedSave)
    {
        if (status is not OutputSynchronizationStatus.Ready and not OutputSynchronizationStatus.RecoveryRequired)
        {
            throw new ArgumentOutOfRangeException(nameof(status), "Output admission can report only Ready or RecoveryRequired.");
        }

        if ((status == OutputSynchronizationStatus.Ready) == (unresolvedSave is not null))
        {
            throw new ArgumentException("Recovery-required admission needs its unresolved save, while ready admission cannot carry one.", nameof(unresolvedSave));
        }

        Status = status;
        UnresolvedSave = unresolvedSave;
    }

    /// <summary>Gets whether native output opening may proceed or recovery is required.</summary>
    public OutputSynchronizationStatus Status { get; }

    /// <summary>Gets the original unresolved save when recovery is required.</summary>
    public PendingSaveIdentity? UnresolvedSave { get; }
}
