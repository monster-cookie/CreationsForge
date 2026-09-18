using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Core.Engine;

/// <summary>Stages complete native GameSettingFloat edits through the shared transaction and replay gate.</summary>
public sealed partial class PluginWorkspace
{
    /// <inheritdoc />
    public async ValueTask<EngineResult<OperationReceipt>> ApplyGameSettingFloatEditAsync(
        GameSettingFloatEditRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var fingerprint = FingerprintFactory.Create(WorkspaceId, request);
        await OperationGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (TryReplay(request.OperationId, fingerprint, out EngineResult<OperationReceipt>? replay, out var conflict, out var expired))
            {
                return replay!;
            }

            if (expired)
            {
                return CreateOperationReplayExpiredFailure<OperationReceipt>(request.OperationId, request.ExpectedRevision);
            }

            if (conflict)
            {
                return CreateReuseFailure<OperationReceipt>(request.OperationId, request.ExpectedRevision);
            }

            if (!CanStoreOperation(request.OperationId))
            {
                return CreateOperationCapacityFailure<OperationReceipt>(request.OperationId, request.ExpectedRevision);
            }

            var guardFailure = ValidateMutation(request.OperationId, request.ExpectedRevision);
            if (guardFailure is not null)
            {
                return Store(request.OperationId, fingerprint, EngineResult<OperationReceipt>.Failure(
                    guardFailure,
                    WorkspaceId,
                    request.OperationId,
                    request.ExpectedRevision,
                    CurrentRevision));
            }

            if (Output is null)
            {
                return Store(request.OperationId, fingerprint, EngineResult<OperationReceipt>.Failure(
                    new EngineError(EngineErrorCode.OutputNotSelected, "Select an output before editing a GameSettingFloat."),
                    WorkspaceId,
                    request.OperationId,
                    request.ExpectedRevision,
                    CurrentRevision));
            }

            if (!Edits.TryGetValue(request.EditId, out var editIdentity) ||
                !string.Equals(editIdentity.RecordType, "GameSettingFloat", StringComparison.Ordinal))
            {
                return Store(request.OperationId, fingerprint, EngineResult<OperationReceipt>.Failure(
                    new EngineError(EngineErrorCode.EditNotFound, "The staged GameSettingFloat edit identifier is not part of this workspace."),
                    WorkspaceId,
                    request.OperationId,
                    request.ExpectedRevision,
                    CurrentRevision));
            }

            IPluginOutputState? candidate = null;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                candidate = await Task.Run(() => Adapter.CloneOutput(Output, cancellationToken)).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
                var adapterResult = await Task.Run(
                    () => Adapter.ApplyGameSettingFloatEdit(Sources!, candidate, editIdentity.FormKey, request, cancellationToken))
                    .ConfigureAwait(false);
                if (!adapterResult.Succeeded)
                {
                    await candidate.DisposeAsync().ConfigureAwait(false);
                    candidate = null;
                    return Store(request.OperationId, fingerprint, EngineResult<OperationReceipt>.Failure(
                        adapterResult.Error ?? new EngineError(EngineErrorCode.ValidationFailed, "The game adapter rejected the GameSettingFloat fields."),
                        WorkspaceId,
                        request.OperationId,
                        request.ExpectedRevision,
                        CurrentRevision,
                        adapterResult.Warnings));
                }

                cancellationToken.ThrowIfCancellationRequested();
                var baseRevision = CurrentRevision;
                var resultRevision = baseRevision.Next();
                var disposalWarnings = await PublishCandidateAsync(candidate, resultRevision).ConfigureAwait(false);
                candidate = null;
                var receipt = new OperationReceipt(request.OperationId, resultRevision);
                return Store(request.OperationId, fingerprint, EngineResult<OperationReceipt>.Success(
                    receipt,
                    WorkspaceId,
                    request.OperationId,
                    baseRevision,
                    resultRevision,
                    CombineWarnings(adapterResult.Warnings, disposalWarnings)));
            }
            catch (OperationCanceledException exception)
            {
                if (candidate is not null)
                {
                    await DisposeCandidateAfterFailureAsync(candidate, exception).ConfigureAwait(false);
                }

                throw;
            }
            catch (Exception exception)
            {
                if (candidate is not null)
                {
                    await DisposeCandidateAfterFailureAsync(candidate, exception).ConfigureAwait(false);
                }

                Logger.Error(exception, "Failed to apply GameSettingFloat edit in workspace {WorkspaceId} for operation {OperationId}", WorkspaceId, request.OperationId);
                return Store(request.OperationId, fingerprint, EngineResult<OperationReceipt>.Failure(
                    new EngineError(EngineErrorCode.UnexpectedFailure, "The GameSettingFloat edit could not be applied."),
                    WorkspaceId,
                    request.OperationId,
                    request.ExpectedRevision,
                    CurrentRevision));
            }
        }
        finally
        {
            OperationGate.Release();
        }
    }
}
