using CreationsForge.Services;
using CreationsForge.ViewModels;

namespace CreationsForge.Services.Interfaces;

/// <summary>Presents one owner-bound modal surface for plugin change review, persistence, and guarded leave choices.</summary>
public interface IWorkspaceChangesDialogService
{
    /// <summary>Shows the supplied change lifecycle until the user keeps editing or a proved workflow may proceed.</summary>
    /// <param name="viewModel">The navigation-scope change lifecycle state.</param>
    /// <param name="request">The immutable closed dialog purpose.</param>
    /// <param name="cancellationToken">A token that requests cancellation of modal admission or cancellable read-only work.</param>
    /// <returns>The terminal dialog choice accepted by the change lifecycle.</returns>
    Task<WorkspaceChangesDialogResult> ShowAsync(
        WorkspaceChangesViewModel viewModel,
        WorkspaceChangesDialogRequest request,
        CancellationToken cancellationToken = default);
}
