using System.ComponentModel;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;
using CreationsForge.ViewModels;

namespace CreationsForge.PresentationTests.Composition;

/// <summary>Delays delivery of one real successful plugin Apply result without altering the engine's mutation or receipt.</summary>
internal sealed class DesktopApplyResultGate : IWorkspaceCoordinator
{
    /// <summary>The production coordinator whose workspace ownership remains with the root container.</summary>
    private readonly IWorkspaceCoordinator Inner;

    /// <summary>Signals that the plugin operation succeeded and its exact receipt is waiting for presentation.</summary>
    private readonly TaskCompletionSource<OperationReceipt> HeldReceiptCompletion = new(TaskCreationOptions.RunContinuationsAsynchronously);

    /// <summary>Signals cancellation of the real editor token after the successful response was held.</summary>
    private readonly TaskCompletionSource CancellationCompletion = new(TaskCreationOptions.RunContinuationsAsynchronously);

    /// <summary>Allows the held successful response to reach the production editor.</summary>
    private readonly TaskCompletionSource ReleaseCompletion = new(TaskCreationOptions.RunContinuationsAsynchronously);

    /// <summary>Ensures only the first successful Apply result is delayed.</summary>
    private int HasCapturedApply;

    /// <summary>Creates a non-owning adapter around the production coordinator.</summary>
    /// <param name="inner">The root-owned coordinator that performs every plugin operation unchanged.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="inner"/> is <see langword="null"/>.</exception>
    internal DesktopApplyResultGate(IWorkspaceCoordinator inner)
    {
        ArgumentNullException.ThrowIfNull(inner);
        Inner = inner;
    }

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged
    {
        add => Inner.PropertyChanged += value;
        remove => Inner.PropertyChanged -= value;
    }

    /// <inheritdoc />
    public WorkspaceDescriptor? CurrentWorkspace => Inner.CurrentWorkspace;

    /// <summary>Gets the exact successful plugin receipt retained before presentation publication.</summary>
    internal Task<OperationReceipt> HeldReceipt => HeldReceiptCompletion.Task;

    /// <summary>Gets completion when the captured editor operation receives cancellation.</summary>
    internal Task CancellationObserved => CancellationCompletion.Task;

    /// <inheritdoc />
    public ValueTask<EngineResult<WorkspaceDescriptor>> OpenAsync(
        WorkspaceLaunchRequest request,
        CancellationToken cancellationToken = default)
    {
        return Inner.OpenAsync(request, cancellationToken);
    }

    /// <summary>Runs the real operation, then delays only one successful Apply response until the test releases it.</summary>
    /// <typeparam name="T">The unchanged successful engine result type.</typeparam>
    /// <param name="operation">The operation forwarded to the production coordinator without modification.</param>
    /// <param name="cancellationToken">The real caller token forwarded to plugin execution and observed during the response delay.</param>
    /// <returns>The exact production result after any controlled response delay.</returns>
    /// <exception cref="OperationCanceledException">Propagated when the production coordinator cancels before returning a result.</exception>
    public async ValueTask<EngineResult<T>> ExecuteAsync<T>(
        Func<IFormListWorkspace, CancellationToken, ValueTask<EngineResult<T>>> operation,
        CancellationToken cancellationToken = default)
    {
        var result = await Inner.ExecuteAsync(operation, cancellationToken).ConfigureAwait(false);
        if (result.Succeeded
            && result.Value is FormListEditorApplyOutcome outcome
            && Interlocked.CompareExchange(ref HasCapturedApply, 1, 0) == 0)
        {
            using var cancellationRegistration = cancellationToken.Register(() => CancellationCompletion.TrySetResult());
            HeldReceiptCompletion.TrySetResult(outcome.Receipt);
            await ReleaseCompletion.Task.ConfigureAwait(false);
        }

        return result;
    }

    /// <inheritdoc />
    public ValueTask CloseAsync()
    {
        return Inner.CloseAsync();
    }

    /// <summary>Releases any held response without disposing the separately owned production coordinator.</summary>
    /// <returns>An already-completed value task after the response gate is opened.</returns>
    public ValueTask DisposeAsync()
    {
        ReleaseHeldResult();
        return ValueTask.CompletedTask;
    }

    /// <summary>Idempotently releases the exact successful response so editor terminal publication can drain.</summary>
    internal void ReleaseHeldResult()
    {
        ReleaseCompletion.TrySetResult();
    }
}
