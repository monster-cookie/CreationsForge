using CreationsForge.Services.Interfaces;

namespace CreationsForge.PresentationTests.Support;

/// <summary>
/// Queues awaited presentation state changes so tests can control the exact UI-thread publication boundary.
/// </summary>
internal sealed class QueuedUiDispatcher : IUiDispatcher
{
    /// <summary>The queued state changes and their completion signals.</summary>
    private readonly Queue<(Action Action, TaskCompletionSource<bool> Completion)> Invocations = [];

    /// <summary>Signals each newly queued awaited invocation.</summary>
    private readonly SemaphoreSlim InvocationSignal = new(0);

    /// <summary>The completion belonging to an action run without completing its awaiting caller.</summary>
    private TaskCompletionSource<bool>? RunningCompletion;

    /// <inheritdoc />
    public void Post(Action action)
    {
        action();
    }

    /// <inheritdoc />
    public Task InvokeAsync(Action action)
    {
        var completion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        lock (Invocations)
        {
            Invocations.Enqueue((action, completion));
        }

        InvocationSignal.Release();
        return completion.Task;
    }

    /// <summary>Waits until one awaited state change has been queued.</summary>
    /// <param name="cancellationToken">A token that bounds how long the test waits for an invocation.</param>
    /// <returns>A task that completes when a queued invocation is available.</returns>
    public Task WaitForInvocationAsync(CancellationToken cancellationToken = default)
    {
        return InvocationSignal.WaitAsync(cancellationToken);
    }

    /// <summary>Runs the next queued invocation and completes its awaiting caller with the resulting outcome.</summary>
    public void RunNextInvocation()
    {
        var invocation = DequeueInvocation();
        try
        {
            invocation.Action();
            invocation.Completion.SetResult(true);
        }
        catch (Exception exception)
        {
            invocation.Completion.SetException(exception);
        }
    }

    /// <summary>Runs the next queued action while keeping its awaiting caller suspended at the publication boundary.</summary>
    public void RunNextActionWithoutCompleting()
    {
        if (RunningCompletion is not null)
        {
            throw new InvalidOperationException("A previously run invocation is still awaiting completion.");
        }

        var invocation = DequeueInvocation();
        try
        {
            invocation.Action();
            RunningCompletion = invocation.Completion;
        }
        catch (Exception exception)
        {
            invocation.Completion.SetException(exception);
        }
    }

    /// <summary>Completes the caller awaiting the action most recently run without completion.</summary>
    public void CompleteRunningInvocation()
    {
        var completion = RunningCompletion ?? throw new InvalidOperationException("No invocation is awaiting completion.");
        RunningCompletion = null;
        completion.SetResult(true);
    }

    /// <summary>Removes the next queued invocation under the dispatcher's synchronization lock.</summary>
    /// <returns>The next queued action and its completion signal.</returns>
    private (Action Action, TaskCompletionSource<bool> Completion) DequeueInvocation()
    {
        lock (Invocations)
        {
            return Invocations.Dequeue();
        }
    }
}
