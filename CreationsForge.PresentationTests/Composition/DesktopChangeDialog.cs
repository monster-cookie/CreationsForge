using Avalonia.Threading;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;
using CreationsForge.ViewModels;
using Shouldly;

namespace CreationsForge.PresentationTests.Composition;

/// <summary>Drives explicitly queued user choices through the real change view model and preserves callback assertion failures.</summary>
internal sealed class DesktopChangeDialog : IWorkspaceChangesDialogService
{
    /// <summary>The expected modal purposes and deterministic user interactions in invocation order.</summary>
    private readonly Queue<(WorkspaceChangesDialogPurpose Purpose,
        Func<WorkspaceChangesViewModel, CancellationToken, Task<WorkspaceChangesDialogResult>> Callback)> Interactions = new();

    /// <summary>Retains assertion failures even when presentation error handling catches the dialog exception.</summary>
    private readonly List<Exception> Failures = [];

    /// <summary>Queues one explicit modal interaction without issuing any workspace operation.</summary>
    /// <param name="purpose">The exact expected dialog purpose.</param>
    /// <param name="callback">The awaited user interaction that drives real public view-model operations.</param>
    public void Enqueue(WorkspaceChangesDialogPurpose purpose,
        Func<WorkspaceChangesViewModel, CancellationToken, Task<WorkspaceChangesDialogResult>> callback)
    {
        Interactions.Enqueue((purpose, callback));
    }

    /// <inheritdoc />
    public Task<WorkspaceChangesDialogResult> ShowAsync(
        WorkspaceChangesViewModel viewModel,
        WorkspaceChangesDialogRequest request,
        CancellationToken cancellationToken = default)
    {
        if (Dispatcher.UIThread.CheckAccess())
        {
            return ShowOnUiThreadAsync(viewModel, request, cancellationToken);
        }

        return Dispatcher.UIThread.InvokeAsync(
            () => ShowOnUiThreadAsync(viewModel, request, cancellationToken));
    }

    /// <summary>Runs the queued interaction on Avalonia's UI thread and records callback assertion failures.</summary>
    /// <param name="viewModel">The production change lifecycle supplied by the caller.</param>
    /// <param name="request">The exact dialog request being tested.</param>
    /// <param name="cancellationToken">The token that bounds the queued interaction.</param>
    /// <returns>The explicit result selected by the queued interaction.</returns>
    private async Task<WorkspaceChangesDialogResult> ShowOnUiThreadAsync(
        WorkspaceChangesViewModel viewModel,
        WorkspaceChangesDialogRequest request,
        CancellationToken cancellationToken)
    {
        Dispatcher.UIThread.VerifyAccess();
        try
        {
            Interactions.Count.ShouldBeGreaterThan(0, "An unexpected changes dialog was opened.");
            var interaction = Interactions.Dequeue();
            request.Purpose.ShouldBe(interaction.Purpose);
            return await interaction.Callback(viewModel, cancellationToken);
        }
        catch (Exception exception)
        {
            Failures.Add(exception);
            throw;
        }
    }

    /// <summary>Requires every queued interaction to have completed without a swallowed callback assertion.</summary>
    /// <exception cref="AggregateException">Thrown when an interaction raised an assertion or unexpected error.</exception>
    public void AssertDrained()
    {
        if (Failures.Count > 0)
        {
            throw new AggregateException("A plugin changes dialog interaction failed.", Failures);
        }

        Interactions.ShouldBeEmpty("The expected changes dialog was never opened.");
    }
}
