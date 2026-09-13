using System.ComponentModel;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Services.Interfaces;
using CreationsForge.ViewModels;

namespace CreationsForge.PresentationTests.Support;

/// <summary>Exposes configurable editor state and records post-persistence refresh requests.</summary>
internal sealed class RecordingWorkspaceEditParticipant : IWorkspaceEditParticipant
{
    /// <summary>Initializes an idle recording editor participant.</summary>
    internal RecordingWorkspaceEditParticipant()
    {
    }

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public bool HasDraftChanges { get; private set; }

    /// <inheritdoc />
    public bool HasPendingOperation { get; private set; }

    /// <inheritdoc />
    public bool IsEditorBusy { get; private set; }

    /// <inheritdoc />
    public FormListEditorOperationState EditorOperationState { get; private set; } = FormListEditorOperationState.Idle;

    /// <summary>Gets the number of request-local discard calls.</summary>
    internal int DiscardRequestLocalCount { get; private set; }

    /// <summary>Gets the exact post-persistence refresh requests in invocation order.</summary>
    internal List<(Guid WorkspaceId, WorkspaceRevision Revision)> RefreshRequests { get; } = [];

    /// <summary>Gets or sets a custom post-persistence refresh implementation.</summary>
    internal Func<Guid, WorkspaceRevision, CancellationToken, Task<EngineResult<WorkspaceState>>>? OnRefreshAsync { get; set; }

    /// <summary>Sets all editor-owned state and publishes the affected properties.</summary>
    /// <param name="hasDraftChanges">Whether request-local controls are changed.</param>
    /// <param name="hasPendingOperation">Whether an uncertain edit operation awaits replay.</param>
    /// <param name="isEditorBusy">Whether an editor operation is active.</param>
    /// <param name="operationState">The exact editor operation state.</param>
    internal void SetState(
        bool hasDraftChanges,
        bool hasPendingOperation = false,
        bool isEditorBusy = false,
        FormListEditorOperationState operationState = FormListEditorOperationState.Idle)
    {
        HasDraftChanges = hasDraftChanges;
        HasPendingOperation = hasPendingOperation;
        IsEditorBusy = isEditorBusy;
        EditorOperationState = operationState;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasDraftChanges)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasPendingOperation)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEditorBusy)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EditorOperationState)));
    }

    /// <inheritdoc />
    public void DiscardRequestLocalFormChanges()
    {
        DiscardRequestLocalCount++;
        SetState(false, HasPendingOperation, IsEditorBusy, EditorOperationState);
    }

    /// <inheritdoc />
    public Task<EngineResult<WorkspaceState>> RefreshAfterWorkspacePersistenceAsync(
        Guid expectedWorkspaceId,
        WorkspaceRevision expectedRevision,
        CancellationToken cancellationToken = default)
    {
        RefreshRequests.Add((expectedWorkspaceId, expectedRevision));
        cancellationToken.ThrowIfCancellationRequested();
        return OnRefreshAsync?.Invoke(expectedWorkspaceId, expectedRevision, cancellationToken)
            ?? Task.FromResult(EngineResult<WorkspaceState>.Failure(
                new EngineError(EngineErrorCode.UnexpectedFailure, "The recording participant refresh was not configured."),
                expectedWorkspaceId,
                baseRevision: expectedRevision,
                resultRevision: expectedRevision));
    }
}
