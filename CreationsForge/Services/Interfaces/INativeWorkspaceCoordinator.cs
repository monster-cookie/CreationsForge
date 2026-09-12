using System.ComponentModel;
using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Services.Interfaces;

/// <summary>
/// Owns the application-wide native workspace and serializes operations that borrow it with replacement and shutdown.
/// </summary>
public interface INativeWorkspaceCoordinator : INotifyPropertyChanged, IAsyncDisposable
{
    /// <summary>Gets immutable identity for the active workspace, or <see langword="null"/> when none is open.</summary>
    NativeWorkspaceDescriptor? CurrentWorkspace { get; }

    /// <summary>Opens source and output state completely before transactionally replacing the active workspace.</summary>
    /// <param name="request">The complete explicit source and output selection.</param>
    /// <param name="cancellationToken">A token that cancels acquisition before a new workspace is published.</param>
    /// <returns>The new active workspace descriptor, or a typed failure that leaves the prior workspace active.</returns>
    /// <exception cref="OperationCanceledException">Thrown when acquisition is canceled before publication.</exception>
    ValueTask<EngineResult<NativeWorkspaceDescriptor>> OpenAsync(
        NativeWorkspaceOpenRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Runs one operation while replacement and close are excluded from the borrowed workspace lifetime.</summary>
    /// <typeparam name="T">The successful engine value type.</typeparam>
    /// <param name="operation">The operation to run. It must not retain, return, or dispose the borrowed workspace.</param>
    /// <param name="cancellationToken">A token that cancels waiting for or running the operation.</param>
    /// <returns>The operation result, or a typed failure when no workspace is active.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    ValueTask<EngineResult<T>> ExecuteAsync<T>(
        Func<IFormListWorkspace, CancellationToken, ValueTask<EngineResult<T>>> operation,
        CancellationToken cancellationToken = default);

    /// <summary>Closes and disposes the active workspace, if any.</summary>
    /// <returns>A task that completes after native ownership has been released.</returns>
    ValueTask CloseAsync();
}
