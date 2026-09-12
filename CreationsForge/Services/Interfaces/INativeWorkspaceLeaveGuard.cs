using CreationsForge.Services;

namespace CreationsForge.Services.Interfaces;

/// <summary>Reserves a safely drained native workspace transition before application navigation or shutdown.</summary>
public interface INativeWorkspaceLeaveGuard
{
    /// <summary>Attempts to prove and reserve the requested workspace-leave transition.</summary>
    /// <param name="reason">The exact application action that will own the returned reservation.</param>
    /// <param name="cancellationToken">A token that cancels the leave attempt before a reservation is transferred.</param>
    /// <returns>A reservation authorizing only the requested action, or <see langword="null"/> when the user keeps editing.</returns>
    ValueTask<NativeWorkspaceLeaveReservation?> ReserveLeaveAsync(
        NativeWorkspaceLeaveReason reason,
        CancellationToken cancellationToken = default);
}
