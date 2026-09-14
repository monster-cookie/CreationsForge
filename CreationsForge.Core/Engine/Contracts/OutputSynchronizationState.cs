namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Provides one atomic snapshot of output synchronization and its pending save identity.</summary>
public sealed class OutputSynchronizationState
{
    /// <summary>Initializes an immutable output synchronization snapshot.</summary>
    /// <param name="status">Whether ordinary output operations are ready or require recovery or reopen.</param>
    /// <param name="pendingSave">The original pending save for a blocked state; otherwise <see langword="null"/>.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="status"/> and <paramref name="pendingSave"/> disagree.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="status"/> is undefined.</exception>
    public OutputSynchronizationState(
        OutputSynchronizationStatus status,
        PendingSaveIdentity? pendingSave)
    {
        if (!Enum.IsDefined(status))
        {
            throw new ArgumentOutOfRangeException(nameof(status));
        }

        if ((status == OutputSynchronizationStatus.Ready) == (pendingSave is not null))
        {
            throw new ArgumentException("A blocked synchronization state requires its pending save, while a ready state cannot carry one.", nameof(pendingSave));
        }

        Status = status;
        PendingSave = pendingSave;
    }

    /// <summary>Gets whether ordinary output operations are ready or require recovery or reopen.</summary>
    public OutputSynchronizationStatus Status { get; }

    /// <summary>Gets the original pending save for a blocked state.</summary>
    public PendingSaveIdentity? PendingSave { get; }
}
