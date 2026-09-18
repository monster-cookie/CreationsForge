using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.ViewModels;

public sealed partial class FormListBrowserViewModel
{
    /// <summary>The exact revision-bound context currently available to editor actions.</summary>
    private FormListEditorSelection? EditorSelectionValue;

    /// <summary>Gets the browser-owned editor that operates through detached presentation contracts.</summary>
    public FormListEditorViewModel Editor { get; }

    /// <inheritdoc />
    public bool HasDraftChanges => Editor.HasDraftChanges;

    /// <inheritdoc />
    public bool HasPendingOperation => Editor.HasPendingOperation;

    /// <inheritdoc />
    public bool IsEditorBusy => Editor.IsBusy;

    /// <inheritdoc />
    public FormListEditorOperationState EditorOperationState => Editor.OperationState;

    /// <inheritdoc />
    public FormListEditorSelection? Selection => EditorSelectionValue;

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
            nameof(FormListEditorViewModel.HasDraftChanges) => nameof(HasDraftChanges),
            nameof(FormListEditorViewModel.HasPendingOperation) => nameof(HasPendingOperation),
            nameof(FormListEditorViewModel.IsBusy) => nameof(IsEditorBusy),
            nameof(FormListEditorViewModel.OperationState) => nameof(EditorOperationState),
            _ => null,
        };
        if (participantProperty is not null)
        {
            OnPropertyChanged(participantProperty);
        }
    }

    /// <summary>Publishes the exact winning context for a root or the selected exact context child.</summary>
    /// <param name="record">The tree node selected by the user.</param>
    private void PublishEditorSelection(FormListRecordViewModel record)
    {
        var descriptor = WorkspaceCoordinator.CurrentWorkspace;
        var exactRecord = record.Context.IsWinningOverride ? record.Contexts.LastOrDefault() : record;
        if (exactRecord is null ||
            WorkspaceStateValue is not { } state ||
            descriptor is null ||
            descriptor.WorkspaceId == Guid.Empty)
        {
            ClearEditorSelection();
            return;
        }

        var isStagedOutput = exactRecord.Context.Role == PluginRole.Output;
        var selection = new FormListEditorSelection(
            descriptor.WorkspaceId,
            state.Revision,
            exactRecord.FormKey,
            isStagedOutput ? null : exactRecord.Context.Selection,
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
