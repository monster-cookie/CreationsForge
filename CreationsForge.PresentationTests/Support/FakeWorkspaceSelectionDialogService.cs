using CreationsForge.Services.Interfaces;
using CreationsForge.ViewModels;

namespace CreationsForge.PresentationTests.Support;

/// <summary>
/// Records complete workspace selection workflows without opening a modal window.
/// </summary>
internal sealed class FakeWorkspaceSelectionDialogService : IWorkspaceSelectionDialogService
{
    /// <summary>Gets each selection view model supplied to the fake dialog.</summary>
    public IList<WorkspaceSelectionViewModel> ViewModels { get; } = [];

    /// <summary>Gets or sets the modal result returned by the fake.</summary>
    public bool Result { get; set; }

    /// <summary>Gets or sets optional awaited behavior that runs while the caller still owns its leave reservation.</summary>
    public Func<WorkspaceSelectionViewModel, Task<bool>>? ShowAction { get; set; }

    /// <inheritdoc />
    public Task<bool> ShowAsync(WorkspaceSelectionViewModel viewModel)
    {
        ViewModels.Add(viewModel);
        return ShowAction?.Invoke(viewModel) ?? Task.FromResult(Result);
    }
}
