using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.ViewModels;

public sealed partial class FormListBrowserViewModel
{
    /// <summary>Starts a comparison generation after immediately clearing previously selected JSON.</summary>
    /// <param name="ownerCancellationToken">An optional token for the operation that owns this comparison.</param>
    /// <returns>A task that completes after this comparison publishes or is superseded.</returns>
    private Task BeginComparisonGeneration(CancellationToken ownerCancellationToken = default)
    {
        ComparisonGeneration++;
        CancelAndDispose(ref ComparisonCancellation);
        ClearComparisonPresentation();
        if (SelectedRecordValue is null ||
            SelectedBeforeContextValue is null ||
            SelectedAfterContextValue is null ||
            WorkspaceStateValue is null ||
            WorkspaceCoordinator.CurrentWorkspace is not { } workspace)
        {
            SetComparisonBusy(false);
            return Task.CompletedTask;
        }

        ComparisonCancellation = CancellationTokenSource.CreateLinkedTokenSource(
            WorkspaceCancellation?.Token ?? CancellationToken.None,
            ownerCancellationToken);
        var workspaceGeneration = WorkspaceGeneration;
        var comparisonGeneration = ComparisonGeneration;
        var expectedRevision = WorkspaceStateValue.Revision;
        var before = SelectedBeforeContextValue;
        var after = SelectedAfterContextValue;
        SetComparisonBusy(true);
        SetStatus($"Comparing {SelectedRecordValue.FormKey}...");
        return CompareAsync(
            workspace.WorkspaceId,
            workspaceGeneration,
            comparisonGeneration,
            expectedRevision,
            before,
            after,
            ComparisonCancellation.Token);
    }

    /// <summary>Compares two exact contexts through one revision-checked coordinator borrow.</summary>
    /// <param name="workspaceId">The workspace identity captured for the comparison.</param>
    /// <param name="workspaceGeneration">The workspace generation captured for the comparison.</param>
    /// <param name="comparisonGeneration">The selection generation captured for the comparison.</param>
    /// <param name="expectedRevision">The exact browser revision that the selections describe.</param>
    /// <param name="before">The exact prior selection.</param>
    /// <param name="after">The exact resulting selection.</param>
    /// <param name="cancellationToken">The linked workspace and selection cancellation token.</param>
    /// <returns>A task that completes after publication or stale-result suppression.</returns>
    private async Task CompareAsync(
        Guid workspaceId,
        long workspaceGeneration,
        long comparisonGeneration,
        WorkspaceRevision expectedRevision,
        FormListContextOption before,
        FormListContextOption after,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new CompareFormListRequest(before.Selection, after.Selection);
            var result = await WorkspaceCoordinator.ExecuteAsync(
                (workspace, token) => CompareAtRevisionAsync(workspace, request, expectedRevision, token),
                cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            IReadOnlyList<RecordJsonFieldNodeViewModel>? beforeFields = null;
            IReadOnlyList<RecordJsonFieldNodeViewModel>? afterFields = null;
            if (result.Succeeded && result.Value is not null)
            {
                var comparison = result.Value;
                var fields = await Task.Run(() =>
                {
                    var projectedBefore = JsonTreeProjectionService.Project(
                        comparison.Before,
                        cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();
                    var projectedAfter = JsonTreeProjectionService.Project(
                        comparison.After,
                        cancellationToken);
                    ApplyComparisonStates(
                        projectedBefore,
                        projectedAfter,
                        comparison.AfterContext.Selection.Scope == RecordScope.WinningOverrides,
                        cancellationToken);
                    return (projectedBefore, projectedAfter);
                }, cancellationToken).ConfigureAwait(false);
                beforeFields = fields.projectedBefore;
                afterFields = fields.projectedAfter;
            }

            await UiDispatcher.InvokeAsync(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!IsCurrentComparisonGeneration(
                    workspaceId,
                    workspaceGeneration,
                    comparisonGeneration,
                    expectedRevision,
                    before.Selection,
                    after.Selection))
                {
                    return;
                }

                if (!result.Succeeded || result.Value is null)
                {
                    PublishFailure(result.Error, RetryKind.Comparison);
                    SetWarnings(CombineWarnings(LoadWarnings, result.Warnings));
                    return;
                }

                PublishComparison(result.Value, beforeFields!, afterFields!, result.Warnings);
            }).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            await PublishUnexpectedComparisonFailureAsync(
                workspaceId,
                workspaceGeneration,
                comparisonGeneration,
                expectedRevision,
                before.Selection,
                after.Selection,
                exception).ConfigureAwait(false);
        }
        finally
        {
            PostClearComparisonBusyIfCurrent(
                workspaceId,
                workspaceGeneration,
                comparisonGeneration,
                expectedRevision,
                before.Selection,
                after.Selection);
        }
    }

    /// <summary>Posts busy-indicator cleanup only when the completed comparison still owns the current selection.</summary>
    /// <param name="workspaceId">The captured workspace identity.</param>
    /// <param name="workspaceGeneration">The captured workspace generation.</param>
    /// <param name="comparisonGeneration">The captured comparison generation.</param>
    /// <param name="revision">The captured workspace revision.</param>
    /// <param name="before">The captured prior selection.</param>
    /// <param name="after">The captured resulting selection.</param>
    private void PostClearComparisonBusyIfCurrent(
        Guid workspaceId,
        long workspaceGeneration,
        long comparisonGeneration,
        WorkspaceRevision revision,
        ReferenceRequest before,
        ReferenceRequest after)
    {
        UiDispatcher.Post(() =>
        {
            if (IsCurrentComparisonGeneration(
                workspaceId,
                workspaceGeneration,
                comparisonGeneration,
                revision,
                before,
                after))
            {
                SetComparisonBusy(false);
            }
        });
    }

    /// <summary>Checks the live workspace revision immediately before executing one plugin comparison.</summary>
    /// <param name="workspace">The currently borrowed workspace.</param>
    /// <param name="request">The exact two-context comparison.</param>
    /// <param name="expectedRevision">The browser revision that produced the selections.</param>
    /// <param name="cancellationToken">The comparison cancellation token.</param>
    /// <returns>The comparison or a typed revision failure without traversing stale records.</returns>
    private static async ValueTask<EngineResult<FormListComparison>> CompareAtRevisionAsync(
        IFormListWorkspace workspace,
        CompareFormListRequest request,
        WorkspaceRevision expectedRevision,
        CancellationToken cancellationToken)
    {
        var stateResult = await workspace.ReadStateAsync(cancellationToken).ConfigureAwait(false);
        if (!stateResult.Succeeded || stateResult.Value is null)
        {
            return CopyFailure<WorkspaceState, FormListComparison>(stateResult);
        }

        if (stateResult.Value.Revision != expectedRevision || stateResult.ResultRevision != expectedRevision)
        {
            return EngineResult<FormListComparison>.Failure(
                CreateRevisionError("The workspace changed after this browser selection was loaded."),
                workspace.WorkspaceId,
                baseRevision: expectedRevision,
                resultRevision: stateResult.Value.Revision,
                warnings: stateResult.Warnings);
        }

        var comparisonResult = await workspace.CompareFormListAsync(request, cancellationToken).ConfigureAwait(false);
        if (!comparisonResult.Succeeded || comparisonResult.Value is null)
        {
            return comparisonResult;
        }

        if (comparisonResult.ResultRevision != expectedRevision)
        {
            return EngineResult<FormListComparison>.Failure(
                CreateRevisionError("The workspace changed while the FormList comparison was being read."),
                workspace.WorkspaceId,
                baseRevision: expectedRevision,
                resultRevision: comparisonResult.ResultRevision,
                warnings: CombineWarnings(stateResult.Warnings, comparisonResult.Warnings));
        }

        return EngineResult<FormListComparison>.Success(
            comparisonResult.Value,
            workspace.WorkspaceId,
            baseRevision: expectedRevision,
            resultRevision: expectedRevision,
            warnings: CombineWarnings(stateResult.Warnings, comparisonResult.Warnings));
    }

    /// <summary>Publishes exact contexts, independent JSON trees, changes, and warnings from one comparison.</summary>
    /// <param name="comparison">The detached plugin comparison.</param>
    /// <param name="beforeFields">The complete prior JSON projection built off the UI thread.</param>
    /// <param name="afterFields">The complete resulting JSON projection built off the UI thread.</param>
    /// <param name="warnings">The comparison warnings.</param>
    private void PublishComparison(
        FormListComparison comparison,
        IReadOnlyList<RecordJsonFieldNodeViewModel> beforeFields,
        IReadOnlyList<RecordJsonFieldNodeViewModel> afterFields,
        IReadOnlyList<EngineWarning> warnings)
    {
        BeforeContextValue = comparison.BeforeContext;
        AfterContextValue = comparison.AfterContext;
        OnPropertyChanged(nameof(BeforeContext));
        OnPropertyChanged(nameof(AfterContext));
        OnPropertyChanged(nameof(BeforeProvenanceText));
        OnPropertyChanged(nameof(AfterProvenanceText));
        SetBeforeFields(beforeFields);
        SetAfterFields(afterFields);
        SemanticChangesValue = Array.AsReadOnly(comparison.Changes.ToArray());
        OnPropertyChanged(nameof(SemanticChanges));
        SetWarnings(CombineWarnings(LoadWarnings, warnings));
        ClearError();
        RetryKindValue = RetryKind.None;
        SetStatus(string.Empty);
    }

}
