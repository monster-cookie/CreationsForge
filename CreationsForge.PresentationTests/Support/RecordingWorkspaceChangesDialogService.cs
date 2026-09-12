using CreationsForge.Services;
using CreationsForge.Services.Interfaces;
using CreationsForge.ViewModels;

namespace CreationsForge.PresentationTests.Support;

/// <summary>Records native changes dialogs and optionally runs deterministic user choices.</summary>
internal sealed class RecordingWorkspaceChangesDialogService : INativeWorkspaceChangesDialogService
{
    /// <summary>Initializes a dialog recorder that keeps editing by default.</summary>
    internal RecordingWorkspaceChangesDialogService()
    {
    }

    /// <summary>Gets or sets the terminal result returned when no custom callback is configured.</summary>
    internal NativeWorkspaceChangesDialogResult Result { get; set; } = NativeWorkspaceChangesDialogResult.KeepEditing;

    /// <summary>Gets or sets a custom modal callback.</summary>
    internal Func<NativeWorkspaceChangesViewModel, NativeWorkspaceChangesDialogRequest, Task<NativeWorkspaceChangesDialogResult>>? OnShowAsync { get; set; }

    /// <summary>Gets the view models shown in invocation order.</summary>
    internal List<NativeWorkspaceChangesViewModel> ViewModels { get; } = [];

    /// <summary>Gets the immutable requests shown in invocation order.</summary>
    internal List<NativeWorkspaceChangesDialogRequest> Requests { get; } = [];

    /// <inheritdoc />
    public Task<NativeWorkspaceChangesDialogResult> ShowAsync(
        NativeWorkspaceChangesViewModel viewModel,
        NativeWorkspaceChangesDialogRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ViewModels.Add(viewModel);
        Requests.Add(request);
        return OnShowAsync?.Invoke(viewModel, request) ?? Task.FromResult(Result);
    }
}
