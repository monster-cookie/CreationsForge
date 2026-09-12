using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.ViewModels;

public sealed partial class NativeFormListBrowserViewModel
{
    /// <summary>The exact revision-bound context currently available to editor actions.</summary>
    private NativeFormListEditorSelection? EditorSelectionValue;

    /// <summary>Gets the browser-owned editor that operates through detached presentation contracts.</summary>
    public NativeFormListEditorViewModel Editor { get; }

    /// <inheritdoc />
    public bool HasDraftChanges => Editor.HasDraftChanges;

    /// <inheritdoc />
    public bool HasPendingOperation => Editor.HasPendingOperation;

    /// <inheritdoc />
    public bool IsEditorBusy => Editor.IsBusy;

    /// <inheritdoc />
    public NativeFormListEditorOperationState EditorOperationState => Editor.OperationState;

    /// <inheritdoc />
    public NativeFormListEditorSelection? Selection => EditorSelectionValue;

    /// <inheritdoc />
    public void DiscardRequestLocalFormChanges()
    {
        if (!OperationArbiter.IsWorkspaceTransitionPendingOrReserved)
        {
            throw new InvalidOperationException(
                "Request-local form changes require an active workspace-transition reservation before discard.");
        }

        Editor.DiscardRequestLocalFormChangesForWorkspaceTransition();
    }

    /// <summary>Forwards browser-owned editor lifecycle changes through the narrow Task 21 participant contract.</summary>
    /// <param name="sender">The browser-owned editor.</param>
    /// <param name="eventArgs">The changed editor property.</param>
    private void OnEditorPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs eventArgs)
    {
        var participantProperty = eventArgs.PropertyName switch
        {
            nameof(NativeFormListEditorViewModel.HasDraftChanges) => nameof(HasDraftChanges),
            nameof(NativeFormListEditorViewModel.HasPendingOperation) => nameof(HasPendingOperation),
            nameof(NativeFormListEditorViewModel.IsBusy) => nameof(IsEditorBusy),
            nameof(NativeFormListEditorViewModel.OperationState) => nameof(EditorOperationState),
            _ => null,
        };
        if (participantProperty is not null)
        {
            OnPropertyChanged(participantProperty);
        }
    }

    /// <summary>Publishes an atomic editor selection only for an exact context child.</summary>
    /// <param name="record">The tree node selected by the user.</param>
    private void PublishEditorSelection(NativeFormListRecordViewModel record)
    {
        var descriptor = WorkspaceCoordinator.CurrentWorkspace;
        if (record.Context.IsWinningOverride ||
            WorkspaceStateValue is not { } state ||
            descriptor is null ||
            descriptor.WorkspaceId == Guid.Empty)
        {
            ClearEditorSelection();
            return;
        }

        var isStagedOutput = record.Context.Role == PluginRole.Output;
        var selection = new NativeFormListEditorSelection(
            descriptor.WorkspaceId,
            state.Revision,
            record.FormKey,
            isStagedOutput ? null : record.Context.Selection,
            isStagedOutput);
        if (ReferenceEquals(EditorSelectionValue, selection))
        {
            return;
        }

        EditorSelectionValue = selection;
        OnPropertyChanged(nameof(Selection));
    }

    /// <summary>Clears the atomic editor selection when its browser context is no longer current.</summary>
    private void ClearEditorSelection()
    {
        if (EditorSelectionValue is null)
        {
            return;
        }

        EditorSelectionValue = null;
        OnPropertyChanged(nameof(Selection));
    }
}
