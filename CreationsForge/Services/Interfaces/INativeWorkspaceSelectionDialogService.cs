using CreationsForge.ViewModels;

namespace CreationsForge.Services.Interfaces;

/// <summary>
/// Presents the native workspace selection workflow in a modal Avalonia window.
/// </summary>
public interface INativeWorkspaceSelectionDialogService
{
    /// <summary>Shows the supplied selection workflow until a workspace opens or the user cancels.</summary>
    /// <param name="viewModel">The selection state and native activation workflow.</param>
    /// <returns><see langword="true"/> when a native workspace became active; otherwise <see langword="false"/>.</returns>
    Task<bool> ShowAsync(NativeWorkspaceSelectionViewModel viewModel);
}
