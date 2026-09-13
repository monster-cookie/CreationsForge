using CreationsForge.Services;
using Shouldly;

namespace CreationsForge.PresentationTests.Services;

/// <summary>Verifies atomic editor admission, transition queuing, cancellation, and terminal drain.</summary>
public sealed class WorkspacePresentationOperationArbiterTests
{
    /// <summary>Verifies one editor lease excludes every competing editor until idempotent release.</summary>
    [Fact]
    public void TryBeginEditorOperation_ActiveLeaseExcludesCompetingEditor()
    {
        var arbiter = new WorkspacePresentationOperationArbiter();
        var changedProperties = new List<string?>();
        arbiter.PropertyChanged += (_, eventArgs) => changedProperties.Add(eventArgs.PropertyName);

        var lease = arbiter.TryBeginEditorOperation().ShouldNotBeNull();

        arbiter.IsEditorOperationActive.ShouldBeTrue();
        arbiter.TryBeginEditorOperation().ShouldBeNull();
        lease.IsActive.ShouldBeTrue();

        lease.Dispose();
        lease.Dispose();

        lease.IsActive.ShouldBeFalse();
        arbiter.IsEditorOperationActive.ShouldBeFalse();
        changedProperties.ShouldBe([
            nameof(WorkspacePresentationOperationArbiter.IsEditorOperationActive),
            nameof(WorkspacePresentationOperationArbiter.IsEditorOperationActive)]);
    }

    /// <summary>Verifies a pending request closes admission and can be abandoned without canceling or draining its captured editor.</summary>
    /// <returns>A task that completes after request abandonment releases transition ownership.</returns>
    [Fact]
    public async Task BeginWorkspaceTransitionRequestAsync_AbandonPreservesCapturedEditor()
    {
        var arbiter = new WorkspacePresentationOperationArbiter();
        using var editorLease = arbiter.TryBeginEditorOperation().ShouldNotBeNull();
        var request = await arbiter.BeginWorkspaceTransitionRequestAsync();

        request.IsEditorOperationActive.ShouldBeTrue();
        arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeTrue();
        arbiter.TryBeginEditorOperation().ShouldBeNull();
        editorLease.CancellationToken.IsCancellationRequested.ShouldBeFalse();

        await request.DisposeAsync();
        await request.DisposeAsync();

        request.IsEditorOperationActive.ShouldBeTrue();
        editorLease.IsActive.ShouldBeTrue();
        editorLease.CancellationToken.IsCancellationRequested.ShouldBeFalse();
        arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
    }

    /// <summary>Verifies wait-mode completion drains the exact captured editor and transfers admission without reopening it.</summary>
    /// <returns>A task that completes after the transferred lease releases continuous admission.</returns>
    [Fact]
    public async Task WorkspaceTransitionRequest_WaitCompletionTransfersContinuousAdmission()
    {
        var arbiter = new WorkspacePresentationOperationArbiter();
        var editorLease = arbiter.TryBeginEditorOperation().ShouldNotBeNull();
        var request = await arbiter.BeginWorkspaceTransitionRequestAsync();

        var completionTask = request.CompleteAsync(
            WorkspaceTransitionDrainMode.WaitForCurrentOperation).AsTask();

        completionTask.IsCompleted.ShouldBeFalse();
        editorLease.CancellationToken.IsCancellationRequested.ShouldBeFalse();
        editorLease.Dispose();
        var transitionLease = await completionTask;
        await request.DisposeAsync();

        transitionLease.IsActive.ShouldBeTrue();
        arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeTrue();
        arbiter.TryBeginEditorOperation().ShouldBeNull();

        transitionLease.Dispose();

        arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
        using var nextEditor = arbiter.TryBeginEditorOperation().ShouldNotBeNull();
    }

    /// <summary>Verifies cancel-mode completion signals only the captured editor and still waits for its terminal drain.</summary>
    /// <returns>A task that completes after the captured editor releases and admission transfers.</returns>
    [Fact]
    public async Task WorkspaceTransitionRequest_CancelCompletionSignalsCapturedEditorAndWaits()
    {
        var arbiter = new WorkspacePresentationOperationArbiter();
        var editorLease = arbiter.TryBeginEditorOperation().ShouldNotBeNull();
        var request = await arbiter.BeginWorkspaceTransitionRequestAsync();

        var completionTask = request.CompleteAsync(
            WorkspaceTransitionDrainMode.CancelAndWaitForCurrentOperation).AsTask();

        editorLease.CancellationToken.IsCancellationRequested.ShouldBeTrue();
        completionTask.IsCompleted.ShouldBeFalse();
        editorLease.Dispose();
        using var transitionLease = await completionTask;

        transitionLease.IsActive.ShouldBeTrue();
        arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeTrue();
    }

    /// <summary>Verifies an editor that finishes before the choice cannot admit a replacement and its captured drain still transfers continuously.</summary>
    /// <returns>A task that completes after the already-drained request transfers admission.</returns>
    [Fact]
    public async Task WorkspaceTransitionRequest_EditorFinishesBeforeChoiceKeepsAdmissionClosed()
    {
        var arbiter = new WorkspacePresentationOperationArbiter();
        var editorLease = arbiter.TryBeginEditorOperation().ShouldNotBeNull();
        var request = await arbiter.BeginWorkspaceTransitionRequestAsync();

        editorLease.Dispose();

        request.IsEditorOperationActive.ShouldBeFalse();
        arbiter.IsEditorOperationActive.ShouldBeFalse();
        arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeTrue();
        arbiter.TryBeginEditorOperation().ShouldBeNull();

        using var transitionLease = await request.CompleteAsync(
            WorkspaceTransitionDrainMode.WaitForCurrentOperation);

        transitionLease.IsActive.ShouldBeTrue();
        arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeTrue();
    }

    /// <summary>Verifies asynchronous disposal can win during natural drain and consumes the request without canceling the captured editor.</summary>
    /// <returns>A task that completes after request cleanup releases transition ownership.</returns>
    [Fact]
    public async Task WorkspaceTransitionRequest_DisposeDuringWaitAbandonsWithoutTransfer()
    {
        var arbiter = new WorkspacePresentationOperationArbiter();
        using var editorLease = arbiter.TryBeginEditorOperation().ShouldNotBeNull();
        var request = await arbiter.BeginWorkspaceTransitionRequestAsync();
        var completionTask = request.CompleteAsync(
            WorkspaceTransitionDrainMode.WaitForCurrentOperation).AsTask();

        await request.DisposeAsync();

        await Should.ThrowAsync<OperationCanceledException>(async () => await completionTask);
        editorLease.IsActive.ShouldBeTrue();
        editorLease.CancellationToken.IsCancellationRequested.ShouldBeFalse();
        arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
        await Should.ThrowAsync<InvalidOperationException>(async () =>
            await request.CompleteAsync(WorkspaceTransitionDrainMode.WaitForCurrentOperation));
    }

    /// <summary>Verifies disposal after cancel-mode completion starts cannot revoke cancellation already delivered to the captured editor.</summary>
    /// <returns>A task that completes after concurrent disposal consumes the cancel-mode request.</returns>
    [Fact]
    public async Task WorkspaceTransitionRequest_DisposeDuringCancelWaitPreservesEditorCancellation()
    {
        var arbiter = new WorkspacePresentationOperationArbiter();
        using var editorLease = arbiter.TryBeginEditorOperation().ShouldNotBeNull();
        var request = await arbiter.BeginWorkspaceTransitionRequestAsync();
        var completionTask = request.CompleteAsync(
            WorkspaceTransitionDrainMode.CancelAndWaitForCurrentOperation).AsTask();

        editorLease.CancellationToken.IsCancellationRequested.ShouldBeTrue();
        await request.DisposeAsync();

        await Should.ThrowAsync<OperationCanceledException>(async () => await completionTask);
        editorLease.IsActive.ShouldBeTrue();
        editorLease.CancellationToken.IsCancellationRequested.ShouldBeTrue();
        arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
    }

    /// <summary>Verifies a completed transfer wins ownership before later request disposal and leaves release solely to the returned lease.</summary>
    /// <returns>A task that completes after the transferred lease releases admission.</returns>
    [Fact]
    public async Task WorkspaceTransitionRequest_TransferBeforeDisposeLeavesLeaseAsSoleOwner()
    {
        var arbiter = new WorkspacePresentationOperationArbiter();
        var request = await arbiter.BeginWorkspaceTransitionRequestAsync();
        var transitionLease = await request.CompleteAsync(
            WorkspaceTransitionDrainMode.WaitForCurrentOperation);

        await Should.ThrowAsync<InvalidOperationException>(async () =>
        {
            await request.CompleteAsync(WorkspaceTransitionDrainMode.WaitForCurrentOperation);
        });
        await request.DisposeAsync();
        await request.DisposeAsync();

        transitionLease.IsActive.ShouldBeTrue();
        arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeTrue();
        arbiter.TryBeginEditorOperation().ShouldBeNull();

        transitionLease.Dispose();

        arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
        using var nextEditor = arbiter.TryBeginEditorOperation().ShouldNotBeNull();
    }

    /// <summary>Verifies caller cancellation during completion consumes the request and releases its gate without signaling natural-wait cancellation to the editor.</summary>
    /// <returns>A task that completes after the canceled completion cleans up request ownership.</returns>
    [Fact]
    public async Task WorkspaceTransitionRequest_CallerCancellationDuringWaitConsumesRequest()
    {
        var arbiter = new WorkspacePresentationOperationArbiter();
        using var editorLease = arbiter.TryBeginEditorOperation().ShouldNotBeNull();
        var request = await arbiter.BeginWorkspaceTransitionRequestAsync();
        using var cancellation = new CancellationTokenSource();
        var completionTask = request.CompleteAsync(
            WorkspaceTransitionDrainMode.WaitForCurrentOperation,
            cancellation.Token).AsTask();

        cancellation.Cancel();
        var disposeTask = request.DisposeAsync().AsTask();

        await Should.ThrowAsync<OperationCanceledException>(async () => await completionTask);
        await disposeTask;
        editorLease.IsActive.ShouldBeTrue();
        editorLease.CancellationToken.IsCancellationRequested.ShouldBeFalse();
        arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
    }

    /// <summary>Verifies transfer linearization wins a deterministic concurrent disposal race and makes the returned lease the only release owner.</summary>
    /// <returns>A task that completes after the transferred test lease releases ownership.</returns>
    [Fact]
    public async Task WorkspaceTransitionRequest_TransferBarrierWinsConcurrentDispose()
    {
        var transferEntered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var allowTransfer = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var disposeStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var abandonedReleaseCount = 0;
        var transferredReleaseCount = 0;
        var request = new WorkspaceTransitionRequest(
            capturedEditorOperation: null,
            () =>
            {
                transferEntered.TrySetResult();
                allowTransfer.Task.GetAwaiter().GetResult();
                return new WorkspaceTransitionLease(_ => Interlocked.Increment(ref transferredReleaseCount));
            },
            () => Interlocked.Increment(ref abandonedReleaseCount));
        var completionTask = Task.Run(async () => await request.CompleteAsync(
            WorkspaceTransitionDrainMode.WaitForCurrentOperation));

        Task? disposeTask = null;
        try
        {
            await transferEntered.Task.WaitAsync(TimeSpan.FromSeconds(5));
            disposeTask = Task.Run(async () =>
            {
                disposeStarted.TrySetResult();
                await request.DisposeAsync();
            });
            await disposeStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));
        }
        finally
        {
            allowTransfer.TrySetResult();
        }

        var transitionLease = await completionTask;
        await disposeTask!;

        abandonedReleaseCount.ShouldBe(0);
        transitionLease.IsActive.ShouldBeTrue();
        transitionLease.Dispose();
        transitionLease.Dispose();
        transferredReleaseCount.ShouldBe(1);
    }

    /// <summary>Verifies canceling queued two-phase acquisition removes only that request while the earlier transition retains admission.</summary>
    /// <returns>A task that completes after queued acquisition observes cancellation.</returns>
    [Fact]
    public async Task BeginWorkspaceTransitionRequestAsync_CanceledQueuedRequestPreservesEarlierLease()
    {
        var arbiter = new WorkspacePresentationOperationArbiter();
        using var first = await arbiter.ReserveWorkspaceTransitionAsync(
            WorkspaceTransitionDrainMode.WaitForCurrentOperation);
        using var cancellation = new CancellationTokenSource();
        var queuedTask = arbiter.BeginWorkspaceTransitionRequestAsync(cancellation.Token).AsTask();

        cancellation.Cancel();

        await Should.ThrowAsync<OperationCanceledException>(async () => await queuedTask);
        first.IsActive.ShouldBeTrue();
        arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeTrue();
        arbiter.TryBeginEditorOperation().ShouldBeNull();
    }

    /// <summary>Verifies wait-mode closes editor admission before waiting and does not cancel the active operation.</summary>
    /// <returns>A task that completes after the editor releases and the transition is acquired.</returns>
    [Fact]
    public async Task ReserveWorkspaceTransitionAsync_WaitModeBlocksNewEditorUntilDrain()
    {
        var arbiter = new WorkspacePresentationOperationArbiter();
        var editorLease = arbiter.TryBeginEditorOperation().ShouldNotBeNull();

        var transitionTask = arbiter.ReserveWorkspaceTransitionAsync(
            WorkspaceTransitionDrainMode.WaitForCurrentOperation).AsTask();

        arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeTrue();
        arbiter.TryBeginEditorOperation().ShouldBeNull();
        editorLease.CancellationToken.IsCancellationRequested.ShouldBeFalse();
        transitionTask.IsCompleted.ShouldBeFalse();

        editorLease.Dispose();
        using var transitionLease = await transitionTask;

        transitionLease.IsActive.ShouldBeTrue();
        arbiter.IsEditorOperationActive.ShouldBeFalse();
        arbiter.TryBeginEditorOperation().ShouldBeNull();
    }

    /// <summary>Verifies cancel-and-wait requests cooperative cancellation but does not return before terminal drain.</summary>
    /// <returns>A task that completes after the canceled editor releases its lease.</returns>
    [Fact]
    public async Task ReserveWorkspaceTransitionAsync_CancelModeCancelsAndWaitsForEditorDrain()
    {
        var arbiter = new WorkspacePresentationOperationArbiter();
        var editorLease = arbiter.TryBeginEditorOperation().ShouldNotBeNull();

        var transitionTask = arbiter.ReserveWorkspaceTransitionAsync(
            WorkspaceTransitionDrainMode.CancelAndWaitForCurrentOperation).AsTask();

        editorLease.CancellationToken.IsCancellationRequested.ShouldBeTrue();
        transitionTask.IsCompleted.ShouldBeFalse();
        editorLease.Dispose();

        var transitionLease = await transitionTask;
        transitionLease.IsActive.ShouldBeTrue();
        transitionLease.Dispose();
        transitionLease.Dispose();

        transitionLease.IsActive.ShouldBeFalse();
        arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
        using var nextEditor = arbiter.TryBeginEditorOperation().ShouldNotBeNull();
    }

    /// <summary>Verifies a failing cancellation observer cannot leak transition admission or prevent terminal drain.</summary>
    /// <returns>A task that completes after the transition drains and releases.</returns>
    [Fact]
    public async Task ReserveWorkspaceTransitionAsync_ThrowingCancellationObserverStillDrainsAndReleases()
    {
        var arbiter = new WorkspacePresentationOperationArbiter();
        var editorLease = arbiter.TryBeginEditorOperation().ShouldNotBeNull();
        using var registration = editorLease.CancellationToken.Register(
            () => throw new InvalidOperationException("The editor cancellation observer failed."));

        var transitionTask = arbiter.ReserveWorkspaceTransitionAsync(
            WorkspaceTransitionDrainMode.CancelAndWaitForCurrentOperation).AsTask();

        editorLease.CancellationToken.IsCancellationRequested.ShouldBeTrue();
        arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeTrue();
        transitionTask.IsCompleted.ShouldBeFalse();
        editorLease.Dispose();
        var transitionLease = await transitionTask;
        transitionLease.Dispose();

        arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
        using var nextEditor = arbiter.TryBeginEditorOperation().ShouldNotBeNull();
    }

    /// <summary>Verifies canceling an unacquired queued transition removes only that request and preserves the active reservation.</summary>
    /// <returns>A task that completes after the queued request observes cancellation.</returns>
    [Fact]
    public async Task ReserveWorkspaceTransitionAsync_CanceledQueuedRequestPreservesCurrentReservation()
    {
        var arbiter = new WorkspacePresentationOperationArbiter();
        using var first = await arbiter.ReserveWorkspaceTransitionAsync(
            WorkspaceTransitionDrainMode.WaitForCurrentOperation);
        using var cancellation = new CancellationTokenSource();
        var queuedTask = arbiter.ReserveWorkspaceTransitionAsync(
            WorkspaceTransitionDrainMode.WaitForCurrentOperation,
            cancellation.Token).AsTask();

        cancellation.Cancel();

        await Should.ThrowAsync<OperationCanceledException>(async () => await queuedTask);
        first.IsActive.ShouldBeTrue();
        arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeTrue();
        arbiter.TryBeginEditorOperation().ShouldBeNull();
    }

    /// <summary>Verifies canceling a cancel-and-wait requester cannot revoke the cancellation already sent to its active editor.</summary>
    /// <returns>A task that completes after the reservation request is canceled.</returns>
    [Fact]
    public async Task ReserveWorkspaceTransitionAsync_CanceledDrainWaitDoesNotUndoEditorCancellation()
    {
        var arbiter = new WorkspacePresentationOperationArbiter();
        using var editorLease = arbiter.TryBeginEditorOperation().ShouldNotBeNull();
        using var cancellation = new CancellationTokenSource();
        var transitionTask = arbiter.ReserveWorkspaceTransitionAsync(
            WorkspaceTransitionDrainMode.CancelAndWaitForCurrentOperation,
            cancellation.Token).AsTask();
        editorLease.CancellationToken.IsCancellationRequested.ShouldBeTrue();

        cancellation.Cancel();

        await Should.ThrowAsync<OperationCanceledException>(async () => await transitionTask);
        editorLease.CancellationToken.IsCancellationRequested.ShouldBeTrue();
        arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
        arbiter.TryBeginEditorOperation().ShouldBeNull();
    }

    /// <summary>Verifies queued transitions keep editor admission continuously closed while reservation ownership changes.</summary>
    /// <returns>A task that completes after the queued transition acquires admission.</returns>
    [Fact]
    public async Task ReserveWorkspaceTransitionAsync_QueuedTransitionKeepsAdmissionClosedAcrossHandoff()
    {
        var arbiter = new WorkspacePresentationOperationArbiter();
        var first = await arbiter.ReserveWorkspaceTransitionAsync(
            WorkspaceTransitionDrainMode.WaitForCurrentOperation);
        var secondTask = arbiter.ReserveWorkspaceTransitionAsync(
            WorkspaceTransitionDrainMode.WaitForCurrentOperation).AsTask();

        first.Dispose();
        using var second = await secondTask;

        arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeTrue();
        arbiter.TryBeginEditorOperation().ShouldBeNull();
        second.IsActive.ShouldBeTrue();
    }
}
