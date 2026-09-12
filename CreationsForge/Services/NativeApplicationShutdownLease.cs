namespace CreationsForge.Services;

/// <summary>Owns the optional transferred workspace reservation while application shutdown completes.</summary>
public sealed class NativeApplicationShutdownLease : IDisposable
{
    /// <summary>The transferred workspace reservation, or <see langword="null"/> for Settings and after disposal.</summary>
    private NativeWorkspaceLeaveReservation? LeaveReservation;

    /// <summary>Whether this lease represents a Settings screen with no live editor scope.</summary>
    private readonly bool IsSettingsLease;

    /// <summary>Tracks disposal for a no-op Settings lease.</summary>
    private int IsDisposedValue;

    /// <summary>Initializes a shutdown lease that owns an accepted exit reservation.</summary>
    /// <param name="leaveReservation">The exact reservation transferred by the current shell leave guard.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="leaveReservation"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when the reservation does not authorize application exit.</exception>
    internal NativeApplicationShutdownLease(NativeWorkspaceLeaveReservation leaveReservation)
    {
        ArgumentNullException.ThrowIfNull(leaveReservation);
        if (!leaveReservation.IsActive ||
            leaveReservation.Reason != NativeWorkspaceLeaveReason.ExitApplication ||
            leaveReservation.Disposition == NativeWorkspaceLeaveDisposition.ConfirmedAbandonmentForOpen)
        {
            throw new ArgumentException("The reservation does not authorize application shutdown.", nameof(leaveReservation));
        }

        LeaveReservation = leaveReservation;
    }

    /// <summary>Initializes an independently disposable no-op lease after Settings replaced the native shell safely.</summary>
    private NativeApplicationShutdownLease()
    {
        IsSettingsLease = true;
    }

    /// <summary>Gets whether this lease still authorizes the active shutdown attempt.</summary>
    public bool IsActive => IsSettingsLease
        ? Volatile.Read(ref IsDisposedValue) == 0
        : Volatile.Read(ref LeaveReservation)?.IsActive == true;

    /// <summary>Creates a no-op shutdown lease for a current Settings view that has no native editor scope.</summary>
    /// <returns>An independently disposable shutdown lease.</returns>
    internal static NativeApplicationShutdownLease CreateForSettings()
    {
        return new NativeApplicationShutdownLease();
    }

    /// <summary>Releases the wrapped workspace reservation exactly once after shutdown finishes or is canceled.</summary>
    public void Dispose()
    {
        Interlocked.Exchange(ref IsDisposedValue, 1);
        Interlocked.Exchange(ref LeaveReservation, null)?.Dispose();
    }
}
