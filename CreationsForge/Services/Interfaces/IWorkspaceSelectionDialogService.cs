using CreationsForge.ViewModels;

namespace CreationsForge.Services.Interfaces;

/// <summary>
/// Presents the workspace selection workflow in a modal Avalonia window.
/// </summary>
public interface IWorkspaceSelectionDialogService
{
    /// <summary>Shows the supplied selection workflow until a workspace opens or the user cancels.</summary>
    /// <param name="viewModel">The selection state and plugin activation workflow.</param>
    /// <returns><see langword="true"/> when a workspace became active; otherwise <see langword="false"/>.</returns>
    Task<bool> ShowAsync(WorkspaceSelectionViewModel viewModel);
}
