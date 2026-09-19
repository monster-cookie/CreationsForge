using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.ViewModels;

public sealed partial class FormListBrowserViewModel
{
    /// <inheritdoc />
    /// <exception cref="OperationCanceledException">Thrown when this refresh attempt is canceled before exact publication.</exception>
    public async Task<EngineResult<WorkspaceState>> RefreshAfterWorkspacePersistenceAsync(
        Guid expectedWorkspaceId,
        WorkspaceRevision expectedRevision,
        CancellationToken cancellationToken = default)
    {
        if (expectedWorkspaceId == Guid.Empty)
        {
            return PersistenceRefreshFailure(
                new EngineError(EngineErrorCode.InvalidRequest, "A post-persistence refresh requires a non-empty workspace identity."),
                expectedWorkspaceId,
                expectedRevision);
        }

        if (IsDisposed ||
            !IsStarted ||
            !OperationArbiter.IsWorkspaceTransitionPendingOrReserved ||
            WorkspaceCoordinator.CurrentWorkspace is not { } workspace ||
            workspace.WorkspaceId != expectedWorkspaceId)
        {
            return PersistenceRefreshFailure(
                new EngineError(
                    EngineErrorCode.InvalidRequest,
                    "A post-persistence refresh requires the matching active workspace and an exclusive presentation transition."),
                expectedWorkspaceId,
                expectedRevision);
        }

        cancellationToken.ThrowIfCancellationRequested();
        long generation = 0;
        CancellationTokenSource? refreshCancellation = null;
        await UiDispatcher.InvokeAsync(() =>
        {
            ResetWorkspaceGeneration();
            generation = WorkspaceGeneration;
            refreshCancellation = CreateWorkspaceLoadCancellation(cancellationToken);
            SetStatus("Refreshing FormLists after workspace persistence...");
        }).ConfigureAwait(false);
        using var loadCancellation = refreshCancellation!;
        try
        {
            var result = await WorkspaceCoordinator.ExecuteAsync(
                ReadBrowserSnapshotAsync,
                loadCancellation.Token).ConfigureAwait(false);
            loadCancellation.Token.ThrowIfCancellationRequested();
            if (!result.Succeeded || result.Value is null)
            {
                var failure = PersistenceRefreshFailure(
                    result.Error ?? new EngineError(
                        EngineErrorCode.UnexpectedFailure,
                        "The post-persistence browser refresh returned no value or failure reason."),
                    expectedWorkspaceId,
                    result.ResultRevision ?? expectedRevision,
                    result.Warnings);
                await PublishPersistenceRefreshFailureAsync(
                    workspace.WorkspaceId,
                    generation,
                    failure.Error,
                    failure.Warnings).ConfigureAwait(false);
                return failure;
            }

            var state = result.Value.State;
            var identityError = ValidatePersistenceRefreshIdentity(
                result,
                state,
                expectedWorkspaceId,
                expectedRevision);
            if (identityError is not null)
            {
                var failure = PersistenceRefreshFailure(
                    identityError,
                    expectedWorkspaceId,
                    result.ResultRevision,
                    result.Warnings);
                await PublishPersistenceRefreshFailureAsync(
                    workspace.WorkspaceId,
                    generation,
                    identityError,
                    result.Warnings).ConfigureAwait(false);
                return failure;
            }

            var records = await Task.Run(
                () => BuildRecordTree(result.Value.FormLists, loadCancellation.Token),
                loadCancellation.Token).ConfigureAwait(false);
            EngineResult<WorkspaceState>? publishedResult = null;
            await UiDispatcher.InvokeAsync(() =>
            {
                loadCancellation.Token.ThrowIfCancellationRequested();
                if (!IsCurrentWorkspaceGeneration(expectedWorkspaceId, generation))
                {
                    publishedResult = PersistenceRefreshFailure(
                        CreateRevisionError("The post-persistence browser refresh was superseded before publication."),
                        expectedWorkspaceId,
                        expectedRevision,
                        result.Warnings);
                    return;
                }

                PublishSnapshot(
                    result.Value,
                    records,
                    result.Warnings,
                    select: null,
                    preferStagedOutput: false);
                Editor.CompleteWorkspacePersistenceRefresh(expectedWorkspaceId, expectedRevision);
                publishedResult = EngineResult<WorkspaceState>.Success(
                    state,
                    expectedWorkspaceId,
                    baseRevision: expectedRevision,
                    resultRevision: expectedRevision,
                    warnings: result.Warnings);
                PersistenceRefreshed?.Invoke();
            }).ConfigureAwait(false);

            return publishedResult ?? PersistenceRefreshFailure(
                new EngineError(
                    EngineErrorCode.UnexpectedFailure,
                    "The post-persistence browser refresh completed without publishing a result."),
                expectedWorkspaceId,
                expectedRevision,
                result.Warnings);
        }
        catch (OperationCanceledException) when (loadCancellation.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            var failure = PersistenceRefreshFailure(
                new EngineError(
                    EngineErrorCode.UnexpectedFailure,
                    $"The post-persistence browser refresh failed: {exception.Message}"),
                expectedWorkspaceId,
                expectedRevision);
            await PublishPersistenceRefreshFailureAsync(
                workspace.WorkspaceId,
                generation,
                failure.Error,
                failure.Warnings).ConfigureAwait(false);
            return failure;
        }
    }

    /// <summary>Checks that a fresh browser snapshot exactly matches the accepted persistence result.</summary>
    /// <param name="result">The revision-consistent browser snapshot result.</param>
    /// <param name="state">The detached workspace state contained in the result.</param>
    /// <param name="expectedWorkspaceId">The workspace identity returned by persistence.</param>
    /// <param name="expectedRevision">The resulting revision returned by persistence.</param>
    /// <returns>A typed mismatch error, or <see langword="null"/> when every identity matches.</returns>
    private static EngineError? ValidatePersistenceRefreshIdentity(
        EngineResult<FormListBrowserSnapshot> result,
        WorkspaceState state,
        Guid expectedWorkspaceId,
        WorkspaceRevision expectedRevision)
    {
        if (result.WorkspaceId != expectedWorkspaceId)
        {
            return CreateRevisionError("The post-persistence browser refresh returned a different workspace identity.");
        }

        if (state.Revision != expectedRevision ||
            result.BaseRevision != expectedRevision ||
            result.ResultRevision != expectedRevision)
        {
            return CreateRevisionError("The post-persistence browser refresh did not return the accepted workspace revision.");
        }

        return null;
    }

    /// <summary>Publishes a typed post-persistence refresh failure without enabling the ordinary browser retry path.</summary>
    /// <param name="workspaceId">The workspace identity captured for this refresh.</param>
    /// <param name="generation">The browser generation captured for this refresh.</param>
    /// <param name="error">The typed failure, or <see langword="null"/> for a malformed result.</param>
    /// <param name="warnings">The exact warnings returned with the failed refresh.</param>
    /// <returns>A task that completes after current-generation failure publication.</returns>
    private Task PublishPersistenceRefreshFailureAsync(
        Guid workspaceId,
        long generation,
        EngineError? error,
        IReadOnlyList<EngineWarning> warnings)
    {
        return UiDispatcher.InvokeAsync(() =>
        {
            if (!IsCurrentWorkspaceGeneration(workspaceId, generation))
            {
                return;
            }

            PublishFailure(
                error ?? new EngineError(
                    EngineErrorCode.UnexpectedFailure,
                    "The post-persistence browser refresh returned no value or failure reason."),
                RetryKind.None);
            SetWarnings(warnings);
        });
    }

    /// <summary>Creates a typed post-persistence refresh failure with exact contextual identity.</summary>
    /// <param name="error">The typed refresh failure.</param>
    /// <param name="workspaceId">The expected workspace identity.</param>
    /// <param name="resultRevision">The latest known result revision.</param>
    /// <param name="warnings">Optional warnings returned with the refresh.</param>
    /// <returns>The contextual typed failure.</returns>
    private static EngineResult<WorkspaceState> PersistenceRefreshFailure(
        EngineError error,
        Guid workspaceId,
        WorkspaceRevision? resultRevision,
        IReadOnlyList<EngineWarning>? warnings = null)
    {
        return EngineResult<WorkspaceState>.Failure(
            error,
            workspaceId,
            resultRevision: resultRevision,
            warnings: warnings);
    }
}
