using System.ComponentModel;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.ViewModels;

namespace CreationsForge.Services.Interfaces;

/// <summary>Exposes the browser-owned editor state and its guarded post-persistence refresh boundary.</summary>
public interface INativeWorkspaceEditParticipant : INotifyPropertyChanged
{
    /// <summary>Gets whether request-local form controls contain unapplied changes.</summary>
    bool HasDraftChanges { get; }

    /// <summary>Gets whether an uncertain editor mutation awaits exact replay.</summary>
    bool HasPendingOperation { get; }

    /// <summary>Gets whether a Begin, Apply, retry, or editor reference-picker operation is active.</summary>
    bool IsEditorBusy { get; }

    /// <summary>Gets the exact browser-owned editor operation state.</summary>
    NativeFormListEditorOperationState EditorOperationState { get; }

    /// <summary>Discards only request-local form input while preserving the edit session and engine-staged work.</summary>
    /// <exception cref="InvalidOperationException">Thrown when no active workspace-transition lease protects an idle changed draft.</exception>
    void DiscardRequestLocalFormChanges();

    /// <summary>Publishes a fresh exact browser snapshot after a known successful workspace persistence transition.</summary>
    /// <param name="expectedWorkspaceId">The workspace identity returned by the accepted Core result.</param>
    /// <param name="expectedRevision">The resulting revision returned by the accepted Core result.</param>
    /// <param name="cancellationToken">A token that cancels only this refresh attempt.</param>
    /// <returns>The matching freshly published workspace state or a typed failure that leaves the stale editor disabled.</returns>
    Task<EngineResult<WorkspaceState>> RefreshAfterWorkspacePersistenceAsync(
        Guid expectedWorkspaceId,
        WorkspaceRevision expectedRevision,
        CancellationToken cancellationToken = default);
}
