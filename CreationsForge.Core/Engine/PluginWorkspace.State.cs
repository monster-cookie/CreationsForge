using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Core.Engine;

/// <content>Provides serialized metadata snapshots that remain available while output recovery blocks record reads.</content>
public sealed partial class PluginWorkspace
{
    /// <inheritdoc />
    public async ValueTask<EngineResult<WorkspaceState>> ReadStateAsync(
        CancellationToken cancellationToken = default)
    {
        await OperationGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (Disposed)
            {
                return EngineResult<WorkspaceState>.Failure(
                    new EngineError(EngineErrorCode.WorkspaceDisposed, "The workspace has already been disposed."),
                    WorkspaceId,
                    resultRevision: CurrentRevision);
            }

            cancellationToken.ThrowIfCancellationRequested();
            var revision = CurrentRevision;
            var state = new WorkspaceState(
                Request.Game,
                Request.Release,
                SelectedOutput,
                SelectedOutputBaseline,
                CurrentOutputSynchronization,
                revision);
            return EngineResult<WorkspaceState>.Success(
                state,
                WorkspaceId,
                baseRevision: revision,
                resultRevision: revision);
        }
        finally
        {
            OperationGate.Release();
        }
    }
}
