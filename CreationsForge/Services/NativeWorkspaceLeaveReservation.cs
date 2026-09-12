namespace CreationsForge.Services;

/// <summary>Owns the exact presentation transition lease transferred after one safe leave disposition is proved.</summary>
public sealed class NativeWorkspaceLeaveReservation : IDisposable
{
    /// <summary>The transferred presentation lease, or <see langword="null"/> after disposal.</summary>
    private NativeWorkspaceTransitionLease? TransitionLease;

    /// <summary>Initializes one non-fabricable leave reservation from the exact transferred transition lease.</summary>
    /// <param name="lease">The active transition lease owned exclusively by this reservation.</param>
    /// <param name="reason">The exact final action authorized by the reservation.</param>
    /// <param name="disposition">The proof established while the lease remained active.</param>
    /// <param name="expectedWorkspaceId">The expected live workspace identity, or <see langword="null"/> for a no-workspace proof.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="lease"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when the reason, disposition, and workspace identity are incompatible.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="reason"/> or <paramref name="disposition"/> is undefined.</exception>
    internal NativeWorkspaceLeaveReservation(
        NativeWorkspaceTransitionLease lease,
        NativeWorkspaceLeaveReason reason,
        NativeWorkspaceLeaveDisposition disposition,
        Guid? expectedWorkspaceId)
    {
        ArgumentNullException.ThrowIfNull(lease);
        if (!lease.IsActive)
        {
            throw new ArgumentException("A leave reservation requires an active transition lease.", nameof(lease));
        }

        Validate(reason, disposition, expectedWorkspaceId);
        TransitionLease = lease;
        Reason = reason;
        Disposition = disposition;
        ExpectedWorkspaceId = expectedWorkspaceId;
    }

    /// <summary>Gets the exact final action authorized by this reservation.</summary>
    public NativeWorkspaceLeaveReason Reason { get; }

    /// <summary>Gets the closed proof established before this reservation was transferred.</summary>
    public NativeWorkspaceLeaveDisposition Disposition { get; }

    /// <summary>Gets the expected live workspace identity, or <see langword="null"/> for a no-workspace proof.</summary>
    public Guid? ExpectedWorkspaceId { get; }

    /// <summary>Gets whether this reservation still owns its presentation transition lease.</summary>
    public bool IsActive => Volatile.Read(ref TransitionLease) is not null;

    /// <summary>Releases presentation admission exactly once after the permitted final action finishes or is canceled.</summary>
    public void Dispose()
    {
        Interlocked.Exchange(ref TransitionLease, null)?.Dispose();
    }

    /// <summary>Validates that a reservation cannot authorize a broader action than its proof permits.</summary>
    /// <param name="reason">The requested final action.</param>
    /// <param name="disposition">The proof established by the leave state machine.</param>
    /// <param name="expectedWorkspaceId">The expected workspace identity.</param>
    /// <exception cref="ArgumentException">Thrown when the supplied values are incompatible.</exception>
    private static void Validate(
        NativeWorkspaceLeaveReason reason,
        NativeWorkspaceLeaveDisposition disposition,
        Guid? expectedWorkspaceId)
    {
        if (!Enum.IsDefined(reason))
        {
            throw new ArgumentOutOfRangeException(nameof(reason));
        }

        if (!Enum.IsDefined(disposition))
        {
            throw new ArgumentOutOfRangeException(nameof(disposition));
        }

        if (disposition == NativeWorkspaceLeaveDisposition.NoWorkspace && expectedWorkspaceId is not null)
        {
            throw new ArgumentException("A no-workspace reservation cannot carry a workspace identity.", nameof(expectedWorkspaceId));
        }

        if (disposition != NativeWorkspaceLeaveDisposition.NoWorkspace && expectedWorkspaceId is null)
        {
            throw new ArgumentException("A reservation for a live workspace requires its exact identity.", nameof(expectedWorkspaceId));
        }

        if (disposition == NativeWorkspaceLeaveDisposition.ConfirmedAbandonmentForOpen &&
            reason != NativeWorkspaceLeaveReason.OpenWorkspace)
        {
            throw new ArgumentException("Confirmed abandonment authorizes only workspace replacement.", nameof(disposition));
        }
    }
}
