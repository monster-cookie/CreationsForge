using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.ViewModels;

/// <summary>Supplies revision-checked native major-record comparison and field projection.</summary>
public sealed partial class MajorRecordBrowserViewModel
{
    /// <summary>Starts a fresh comparison generation for the current exact context selections.</summary>
    /// <returns>A task that completes after comparison publication or stale-result suppression.</returns>
    private Task BeginComparisonGenerationAsync()
    {
        SelectionGeneration++;
        CancelAndDispose(ref SelectionCancellation);
        ClearComparisonPresentation();
        if (SelectedRecordValue is null ||
            SelectedBeforeContextValue is null ||
            SelectedAfterContextValue is null ||
            !RevisionValue.HasValue ||
            WorkspaceCoordinator.CurrentWorkspace is not { } workspace)
        {
            SetComparisonBusy(false);
            return Task.CompletedTask;
        }

        SelectionCancellation = CancellationTokenSource.CreateLinkedTokenSource(WorkspaceCancellation?.Token ?? CancellationToken.None);
        SetComparisonBusy(true);
        SetStatus($"Comparing {SelectedRecordValue.FormKey}...");
        return CompareSelectedAsync(
            workspace.WorkspaceId,
            WorkspaceGeneration,
            SelectionGeneration,
            RevisionValue.Value,
            SelectedRecordValue.FormKey,
            SelectionCancellation.Token);
    }

    /// <summary>Compares the currently captured exact contexts and projects both complete field trees.</summary>
    /// <param name="workspaceId">The captured workspace identity.</param>
    /// <param name="workspaceGeneration">The captured workspace generation.</param>
    /// <param name="selectionGeneration">The captured selection generation.</param>
    /// <param name="revision">The exact browser revision.</param>
    /// <param name="formKey">The selected record identity.</param>
    /// <param name="cancellationToken">The selection generation token.</param>
    /// <returns>A task that completes after conditional publication.</returns>
    private async Task CompareSelectedAsync(
        Guid workspaceId,
        long workspaceGeneration,
        long selectionGeneration,
        WorkspaceRevision revision,
        FormKey formKey,
        CancellationToken cancellationToken)
    {
        var before = SelectedBeforeContextValue;
        var after = SelectedAfterContextValue;
        if (before is null || after is null)
        {
            return;
        }

        try
        {
            var request = new CompareMajorRecordRequest(before.Selection, after.Selection);
            var result = await WorkspaceCoordinator.ExecuteAsync(
                (workspace, token) => CompareAtRevisionAsync(workspace, request, revision, token),
                cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            IReadOnlyList<RecordJsonFieldNodeViewModel>? beforeFields = null;
            IReadOnlyList<RecordJsonFieldNodeViewModel>? afterFields = null;
            if (result.Succeeded && result.Value is not null)
            {
                var comparison = result.Value;
                var projected = await Task.Run(() =>
                {
                    var projectedBefore = JsonTreeProjectionService.Project(comparison.Before, cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();
                    var projectedAfter = JsonTreeProjectionService.Project(comparison.After, cancellationToken);
                    FormListBrowserViewModel.ApplyComparisonStates(
                        projectedBefore,
                        projectedAfter,
                        comparison.AfterContext.Selection.Scope == RecordScope.WinningOverrides,
                        cancellationToken);
                    return (projectedBefore, projectedAfter);
                }, cancellationToken).ConfigureAwait(false);
                beforeFields = projected.projectedBefore;
                afterFields = projected.projectedAfter;
            }

            await UiDispatcher.InvokeAsync(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!IsCurrentComparisonGeneration(
                    workspaceId,
                    workspaceGeneration,
                    selectionGeneration,
                    revision,
                    formKey,
                    before.Selection,
                    after.Selection))
                {
                    return;
                }

                if (!result.Succeeded || result.Value is null)
                {
                    PublishFailure(result.Error, RetryKind.Comparison);
                    SetWarnings(CombineWarnings(PageWarnings, result.Warnings));
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
            await PublishUnexpectedSelectionFailureAsync(
                workspaceId,
                workspaceGeneration,
                selectionGeneration,
                revision,
                formKey,
                exception,
                RetryKind.Comparison).ConfigureAwait(false);
        }
        finally
        {
            UiDispatcher.Post(() =>
            {
                if (IsCurrentComparisonGeneration(
                    workspaceId,
                    workspaceGeneration,
                    selectionGeneration,
                    revision,
                    formKey,
                    before.Selection,
                    after.Selection))
                {
                    SetComparisonBusy(false);
                }
            });
        }
    }

    /// <summary>Checks the workspace revision immediately before executing one native comparison.</summary>
    /// <param name="workspace">The borrowed workspace.</param>
    /// <param name="request">The exact two-context request.</param>
    /// <param name="expectedRevision">The accepted browser revision.</param>
    /// <param name="cancellationToken">The comparison token.</param>
    /// <returns>The native comparison or a typed revision failure.</returns>
    private static async ValueTask<EngineResult<MajorRecordComparison>> CompareAtRevisionAsync(
        IPluginWorkspace workspace,
        CompareMajorRecordRequest request,
        WorkspaceRevision expectedRevision,
        CancellationToken cancellationToken)
    {
        var stateResult = await workspace.ReadStateAsync(cancellationToken).ConfigureAwait(false);
        if (!stateResult.Succeeded || stateResult.Value is null)
        {
            return CopyFailure<WorkspaceState, MajorRecordComparison>(stateResult);
        }

        if (stateResult.Value.Revision != expectedRevision || stateResult.ResultRevision != expectedRevision)
        {
            return RevisionFailure<MajorRecordComparison>(
                workspace,
                expectedRevision,
                stateResult.Value.Revision,
                "The workspace changed after this major-record selection was loaded.",
                stateResult.Warnings);
        }

        var comparisonResult = await workspace.CompareMajorRecordAsync(request, cancellationToken).ConfigureAwait(false);
        if (!comparisonResult.Succeeded || comparisonResult.Value is null)
        {
            return comparisonResult;
        }

        var warnings = CombineWarnings(stateResult.Warnings, comparisonResult.Warnings);
        if (comparisonResult.ResultRevision != expectedRevision)
        {
            return RevisionFailure<MajorRecordComparison>(
                workspace,
                expectedRevision,
                comparisonResult.ResultRevision,
                "The workspace changed while the major-record comparison was being read.",
                warnings);
        }

        return EngineResult<MajorRecordComparison>.Success(
            comparisonResult.Value,
            workspace.WorkspaceId,
            baseRevision: expectedRevision,
            resultRevision: expectedRevision,
            warnings: warnings);
    }

    /// <summary>Publishes exact contexts, projected fields, semantic changes, and warnings.</summary>
    /// <param name="comparison">The detached native comparison.</param>
    /// <param name="beforeFields">The complete projected prior fields.</param>
    /// <param name="afterFields">The complete projected resulting fields.</param>
    /// <param name="warnings">The comparison warnings.</param>
    private void PublishComparison(
        MajorRecordComparison comparison,
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
        SetWarnings(CombineWarnings(PageWarnings, warnings));
        ClearError();
        RetryKindValue = RetryKind.None;
        SetStatus(string.Empty);
    }
}
