using CreationsForge.Core.Engine.Contracts;
using CreationsForge.NativeEditing;
using CreationsForge.NativeEditing.Drafts;
using CreationsForge.Services;

namespace CreationsForge.ViewModels;

public sealed partial class NativeFormListEditorViewModel
{
    /// <summary>Replays the exact immutable mutation retained after an uncertain outcome.</summary>
    /// <returns>A task that completes after exact replay succeeds, is definitively rejected, or remains unresolved.</returns>
    public Task RetryPendingOperationAsync()
    {
        return RetryPendingOperationAsync(CancellationToken.None);
    }

    /// <summary>Replays the exact immutable mutation retained after an uncertain outcome.</summary>
    /// <param name="cancellationToken">A token that may leave replay unresolved while preserving the same envelope.</param>
    /// <returns>A task that completes after exact replay succeeds, is definitively rejected, or remains unresolved.</returns>
    public async Task RetryPendingOperationAsync(CancellationToken cancellationToken)
    {
        if (PendingOperationValue is not { } pending ||
            IsBusy ||
            !IsEditorAdmissionOpen)
        {
            return;
        }

        using var operationLease = TryEnterOperation(NativeFormListEditorOperationState.RetryingPendingOperation);
        if (operationLease is null)
        {
            return;
        }

        var generation = Volatile.Read(ref WorkspaceGeneration);
        using var linkedCancellation = CreateGenerationLinkedCancellation(
            cancellationToken,
            operationLease.CancellationToken);
        try
        {
            switch (pending)
            {
                case NativeFormListEditorPendingBegin begin:
                    await ReplayBeginAsync(generation, begin, linkedCancellation.Token).ConfigureAwait(false);
                    break;
                case NativeFormListEditorPendingApply apply:
                    await ReplayApplyAsync(generation, apply, linkedCancellation.Token).ConfigureAwait(false);
                    break;
                default:
                    await PublishRetryBlockedAsync(generation, pending, "The retained editor operation kind is unsupported.").ConfigureAwait(false);
                    break;
            }
        }
        finally
        {
            await ExitOperationAsync(generation).ConfigureAwait(false);
        }
    }

    /// <summary>Replays one exact immutable Begin request without adopting current revision state.</summary>
    /// <param name="generation">The captured workspace generation.</param>
    /// <param name="pending">The exact retained Begin envelope.</param>
    /// <param name="cancellationToken">The caller cancellation token.</param>
    /// <returns>A task that completes after replay handling.</returns>
    private async Task ReplayBeginAsync(
        long generation,
        NativeFormListEditorPendingBegin pending,
        CancellationToken cancellationToken)
    {
        var descriptor = WorkspaceCoordinator.CurrentWorkspace;
        if (descriptor is null || descriptor.WorkspaceId != pending.WorkspaceId)
        {
            await PublishRetryBlockedAsync(generation, pending, "The active workspace does not match the retained Begin operation.").ConfigureAwait(false);
            return;
        }

        var coreResponded = false;
        try
        {
            var result = await WorkspaceCoordinator.ExecuteAsync(
                async (workspace, token) =>
                {
                    var stateResult = await workspace.ReadStateAsync(token).ConfigureAwait(false);
                    if (!stateResult.Succeeded || stateResult.Value is null)
                    {
                        return CopyFailure<WorkspaceState, NativeFormListEditorBeginOutcome>(stateResult);
                    }

                    var stateError = ValidateWorkspaceState(workspace, stateResult.Value, descriptor);
                    if (stateError is not null)
                    {
                        return EngineResult<NativeFormListEditorBeginOutcome>.Failure(stateError, workspace.WorkspaceId, resultRevision: stateResult.Value.Revision);
                    }

                    var catalogResult = CatalogResolver.Resolve(stateResult.Value.Game, stateResult.Value.Release);
                    if (!catalogResult.Succeeded || catalogResult.Value is null)
                    {
                        return CopyFailure<NativeFormListWireCatalogContext, NativeFormListEditorBeginOutcome>(catalogResult);
                    }

                    var commandsResult = NativeFormListCommandPresentationCatalog.Resolve(catalogResult.Value, token);
                    if (!commandsResult.Succeeded || commandsResult.Value is null)
                    {
                        return CopyFailure<IReadOnlyList<NativeFormListCommandPresentation>, NativeFormListEditorBeginOutcome>(commandsResult);
                    }

                    var seedPoliciesResult = NativeFormListCommandSeedCatalog.ResolveAll(catalogResult.Value);
                    if (!seedPoliciesResult.Succeeded)
                    {
                        return CopyFailure<IReadOnlyList<NativeFormListCommandSeedPolicy>, NativeFormListEditorBeginOutcome>(seedPoliciesResult);
                    }

                    var beginResult = await workspace.BeginEditAsync(pending.Request, token).ConfigureAwait(false);
                    coreResponded = true;
                    if (!beginResult.Succeeded || beginResult.Value is null)
                    {
                        return CopyFailure<EditReceipt, NativeFormListEditorBeginOutcome>(beginResult);
                    }

                    var outcome = await ReadBeginFollowUpAsync(
                        workspace,
                        descriptor,
                        catalogResult.Value,
                        commandsResult.Value,
                        beginResult.Value,
                        stateResult.Warnings.Concat(beginResult.Warnings),
                        token).ConfigureAwait(false);
                    return EngineResult<NativeFormListEditorBeginOutcome>.Success(
                        outcome,
                        workspace.WorkspaceId,
                        pending.OperationId,
                        pending.ExpectedRevision,
                        beginResult.Value.Revision,
                        outcome.Warnings);
                },
                cancellationToken).ConfigureAwait(false);

            if (!result.Succeeded || result.Value is null)
            {
                if (coreResponded)
                {
                    await PublishBeginFailureAsync(generation, pending.WorkspaceId, result.Error).ConfigureAwait(false);
                }
                else
                {
                    await PublishRetryBlockedAsync(generation, pending, result.Error?.Message ?? "The exact Begin replay did not reach Core.").ConfigureAwait(false);
                }

                return;
            }

            await PublishBeginSuccessAsync(generation, descriptor, result.Value, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            await PublishRetryBlockedAsync(generation, pending, exception.Message).ConfigureAwait(false);
        }
    }

    /// <summary>Re-decodes and replays one exact immutable Apply request without reserialization or revision adoption.</summary>
    /// <param name="generation">The captured workspace generation.</param>
    /// <param name="pending">The exact retained Apply envelope.</param>
    /// <param name="cancellationToken">The caller cancellation token.</param>
    /// <returns>A task that completes after replay handling.</returns>
    private async Task ReplayApplyAsync(
        long generation,
        NativeFormListEditorPendingApply pending,
        CancellationToken cancellationToken)
    {
        var descriptor = WorkspaceCoordinator.CurrentWorkspace;
        var session = SessionValue;
        var draft = DraftValue;
        if (descriptor is null ||
            session is null ||
            draft is null ||
            descriptor.WorkspaceId != pending.WorkspaceId ||
            session.WorkspaceId != pending.WorkspaceId ||
            session.EditId != pending.EditId ||
            session.ExpectedRevision != pending.ExpectedRevision)
        {
            await PublishRetryBlockedAsync(generation, pending, "The active editor session does not match the retained Apply operation.").ConfigureAwait(false);
            return;
        }

        var coreResponded = false;
        try
        {
            var result = await WorkspaceCoordinator.ExecuteAsync(
                async (workspace, token) =>
                {
                    var stateResult = await workspace.ReadStateAsync(token).ConfigureAwait(false);
                    if (!stateResult.Succeeded || stateResult.Value is null)
                    {
                        return CopyFailure<WorkspaceState, NativeFormListEditorApplyOutcome>(stateResult);
                    }

                    var stateError = ValidateSessionState(workspace, stateResult.Value, descriptor, session);
                    if (stateError is not null)
                    {
                        return EngineResult<NativeFormListEditorApplyOutcome>.Failure(stateError, workspace.WorkspaceId, resultRevision: stateResult.Value.Revision);
                    }

                    var catalogResult = CatalogResolver.Resolve(stateResult.Value.Game, stateResult.Value.Release);
                    if (!catalogResult.Succeeded || catalogResult.Value is null)
                    {
                        return CopyFailure<NativeFormListWireCatalogContext, NativeFormListEditorApplyOutcome>(catalogResult);
                    }

                    if (!SameCatalogIdentity(catalogResult.Value.Identity, pending.CatalogIdentity))
                    {
                        return Failure<NativeFormListEditorApplyOutcome>(EngineErrorCode.ExternalChangeDetected, "The exact retained Apply catalog is no longer available.");
                    }

                    var decode = catalogResult.Value.Codec.Decode(
                        pending.CommandName,
                        pending.Arguments,
                        pending.Limits,
                        token);
                    if (!decode.Succeeded || decode.Value is null)
                    {
                        return EngineResult<NativeFormListEditorApplyOutcome>.Failure(
                            decode.Error ?? new EngineError(EngineErrorCode.ValidationFailed, "The exact retained Apply payload could not be decoded."),
                            workspace.WorkspaceId,
                            pending.OperationId,
                            pending.ExpectedRevision,
                            stateResult.Value.Revision);
                    }

                    var request = new FormListEditRequest(
                        pending.OperationId,
                        pending.ExpectedRevision,
                        pending.EditId,
                        decode.Value);
                    var applyResult = await workspace.ApplyFormListEditAsync(request, token).ConfigureAwait(false);
                    coreResponded = true;
                    if (!applyResult.Succeeded || applyResult.Value is null)
                    {
                        return CopyFailure<OperationReceipt, NativeFormListEditorApplyOutcome>(applyResult);
                    }

                    var outcome = await ReadApplyFollowUpAsync(
                        workspace,
                        descriptor,
                        catalogResult.Value,
                        session,
                        applyResult.Value,
                        stateResult.Warnings.Concat(applyResult.Warnings),
                        token).ConfigureAwait(false);
                    return EngineResult<NativeFormListEditorApplyOutcome>.Success(
                        outcome,
                        workspace.WorkspaceId,
                        pending.OperationId,
                        pending.ExpectedRevision,
                        applyResult.Value.Revision,
                        outcome.Warnings);
                },
                cancellationToken).ConfigureAwait(false);

            if (!result.Succeeded || result.Value is null)
            {
                if (coreResponded)
                {
                    await PublishApplyFailureAsync(generation, pending.WorkspaceId, draft, result.Error).ConfigureAwait(false);
                }
                else
                {
                    await PublishRetryBlockedAsync(generation, pending, result.Error?.Message ?? "The exact Apply replay did not reach Core.").ConfigureAwait(false);
                }

                return;
            }

            await PublishApplySuccessAsync(generation, descriptor, session, draft, result.Value, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            await PublishRetryBlockedAsync(generation, pending, exception.Message).ConfigureAwait(false);
        }
    }

    /// <summary>Keeps an unresolved envelope frozen when an exact replay cannot reach a definitive Core response.</summary>
    /// <param name="generation">The captured workspace generation.</param>
    /// <param name="pending">The retained exact operation.</param>
    /// <param name="reason">The complete retry failure reason.</param>
    /// <returns>A task that completes after current-generation publication.</returns>
    private Task PublishRetryBlockedAsync(
        long generation,
        NativeFormListEditorPendingOperation pending,
        string reason)
    {
        return UiDispatcher.InvokeAsync(() =>
        {
            if (!IsCurrentGeneration(generation, pending.WorkspaceId) || !ReferenceEquals(PendingOperationValue, pending))
            {
                return;
            }

            SetOperationState(NativeFormListEditorOperationState.PendingOutcome);
            PublishError(new EngineError(
                EngineErrorCode.UnexpectedFailure,
                $"Operation outcome unresolved for {pending.ActionName}; exact retry did not complete: {reason}"));
            RaiseSessionProperties();
        });
    }
}
