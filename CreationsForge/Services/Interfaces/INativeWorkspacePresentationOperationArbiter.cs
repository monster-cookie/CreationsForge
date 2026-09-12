using System.ComponentModel;
using CreationsForge.Services;

namespace CreationsForge.Services.Interfaces;

/// <summary>Atomically coordinates editor operations with workspace-wide presentation transitions.</summary>
public interface INativeWorkspacePresentationOperationArbiter : INotifyPropertyChanged
{
    /// <summary>Gets whether one editor operation currently owns admission.</summary>
    bool IsEditorOperationActive { get; }

    /// <summary>Gets whether at least one workspace transition is waiting for or owns exclusive admission.</summary>
    bool IsWorkspaceTransitionPendingOrReserved { get; }

    /// <summary>Attempts to reserve editor admission without waiting.</summary>
    /// <returns>An editor-operation lease, or <see langword="null"/> when an editor operation or workspace transition already owns admission.</returns>
    NativeWorkspaceEditorOperationLease? TryBeginEditorOperation();

    /// <summary>Closes editor admission and acquires a queued transition position without draining the captured editor operation.</summary>
    /// <param name="cancellationToken">A token that cancels acquisition before the pending request is returned.</param>
    /// <returns>A pending transition request that owns admission and can be completed with an explicit drain choice or abandoned.</returns>
    /// <exception cref="OperationCanceledException">Thrown when acquisition is canceled before the pending request is returned.</exception>
    ValueTask<NativeWorkspaceTransitionRequest> BeginWorkspaceTransitionRequestAsync(
        CancellationToken cancellationToken = default);

    /// <summary>Reserves exclusive workspace-transition admission after the current editor operation reaches terminal drain.</summary>
    /// <param name="drainMode">Whether to wait naturally or request cancellation of the active editor operation before waiting.</param>
    /// <param name="cancellationToken">A token that cancels this reservation request without revoking an already requested editor cancellation.</param>
    /// <returns>The exclusive transition lease after all earlier admission has drained.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="drainMode"/> is not a defined mode.</exception>
    /// <exception cref="OperationCanceledException">Thrown when this reservation request is canceled before acquisition.</exception>
    ValueTask<NativeWorkspaceTransitionLease> ReserveWorkspaceTransitionAsync(
        NativeWorkspaceTransitionDrainMode drainMode,
        CancellationToken cancellationToken = default);
}
