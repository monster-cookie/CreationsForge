using CreationsForge.Services;
using CreationsForge.Services.Interfaces;
using CreationsForge.ViewModels;

namespace CreationsForge.PresentationTests.Support;

/// <summary>Records plugin changes dialogs and optionally runs deterministic user choices.</summary>
internal sealed class RecordingWorkspaceChangesDialogService : IWorkspaceChangesDialogService
{
    /// <summary>Initializes a dialog recorder that keeps editing by default.</summary>
    internal RecordingWorkspaceChangesDialogService()
    {
    }

    /// <summary>Gets or sets the terminal result returned when no custom callback is configured.</summary>
    internal WorkspaceChangesDialogResult Result { get; set; } = WorkspaceChangesDialogResult.KeepEditing;

    /// <summary>Gets or sets a custom modal callback.</summary>
    internal Func<WorkspaceChangesViewModel, WorkspaceChangesDialogRequest, Task<WorkspaceChangesDialogResult>>? OnShowAsync { get; set; }

    /// <summary>Gets the view models shown in invocation order.</summary>
    internal List<WorkspaceChangesViewModel> ViewModels { get; } = [];

    /// <summary>Gets the immutable requests shown in invocation order.</summary>
    internal List<WorkspaceChangesDialogRequest> Requests { get; } = [];

    /// <inheritdoc />
    public Task<WorkspaceChangesDialogResult> ShowAsync(
        WorkspaceChangesViewModel viewModel,
        WorkspaceChangesDialogRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ViewModels.Add(viewModel);
        Requests.Add(request);
        return OnShowAsync?.Invoke(viewModel, request) ?? Task.FromResult(Result);
    }
}
