using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.ViewModels;

/// <summary>Supplies revision-consistent complete record loading and exact context discovery.</summary>
public sealed partial class MajorRecordBrowserViewModel
{
    /// <summary>Replaces browser state and loads every admitted record at a fresh workspace revision.</summary>
    /// <returns>A task that completes after the complete tree publishes or the workspace is found closed.</returns>
    private Task BeginWorkspaceGenerationAsync()
    {
        WorkspaceGeneration++;
        SelectionGeneration++;
        CancelAndDispose(ref WorkspaceCancellation);
        CancelAndDispose(ref SelectionCancellation);
        ClearWorkspacePresentation();
        var workspace = WorkspaceCoordinator.CurrentWorkspace;
        OnPropertyChanged(nameof(HasWorkspace));
        if (workspace is null || IsDisposed)
        {
            SetBusy(false);
            SetComparisonBusy(false);
            SetStatus("No workspace is open.");
            return Task.CompletedTask;
        }

        WorkspaceCancellation = new CancellationTokenSource();
        SetBusy(true);
        SetStatus("Loading major records...");
        return LoadAllRecordsAsync(
            workspace.WorkspaceId,
            WorkspaceGeneration,
            WorkspaceCancellation.Token);
    }

    /// <summary>Reads all revision-bound engine pages off the UI thread and publishes one complete tree.</summary>
    /// <param name="workspaceId">The captured workspace identity.</param>
    /// <param name="generation">The captured workspace generation.</param>
    /// <param name="cancellationToken">The workspace generation token.</param>
    /// <returns>A task that completes after conditional publication or cancellation.</returns>
    private async Task LoadAllRecordsAsync(
        Guid workspaceId,
        long generation,
        CancellationToken cancellationToken)
    {
        try
        {
            await Task.Run(async () =>
            {
                var records = new List<MajorRecordViewModel>();
                var warnings = new List<EngineWarning>();
                var seenTokens = new HashSet<string>(StringComparer.Ordinal);
                WorkspaceRevision? revision = null;
                string? continuationToken = null;
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var result = await WorkspaceCoordinator.ExecuteAsync(
                        (workspace, token) => ReadRecordPageAsync(workspace, revision, continuationToken, token),
                        cancellationToken).ConfigureAwait(false);
                    cancellationToken.ThrowIfCancellationRequested();
                    if (!result.Succeeded || result.Value is null || !result.ResultRevision.HasValue)
                    {
                        await UiDispatcher.InvokeAsync(() =>
                        {
                            if (IsCurrentWorkspaceGeneration(workspaceId, generation, null))
                            {
                                PublishFailure(result.Error, RetryKind.Page);
                                SetWarnings(CombineWarnings(warnings, result.Warnings));
                            }
                        }).ConfigureAwait(false);
                        return;
                    }

                    revision = result.ResultRevision.Value;
                    warnings.AddRange(result.Warnings);
                    records.AddRange(result.Value.Records.Select(record => new MajorRecordViewModel(record)));
                    var currentPlugin = result.Value.Records.LastOrDefault()?.ContainingModKey?.FileName.String;
                    var loadedCount = records.Count;
                    await UiDispatcher.InvokeAsync(() =>
                    {
                        if (IsCurrentWorkspaceGeneration(workspaceId, generation, null))
                        {
                            LoadingRecordCountValue = loadedCount;
                            OnPropertyChanged(nameof(LoadedRecordCountText));
                            SetStatus(currentPlugin is null
                                ? $"Loading major records... {loadedCount:N0} found."
                                : $"Loading {currentPlugin}... {loadedCount:N0} records found.");
                        }
                    }).ConfigureAwait(false);

                    continuationToken = result.Value.ContinuationToken;
                    if (continuationToken is null)
                    {
                        break;
                    }

                    if (!seenTokens.Add(continuationToken))
                    {
                        throw new InvalidOperationException("The major-record listing repeated a continuation token.");
                    }
                }

                await UiDispatcher.InvokeAsync(() =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (IsCurrentWorkspaceGeneration(workspaceId, generation, null))
                    {
                        PublishRecords(records, revision!.Value, warnings);
                    }
                }).ConfigureAwait(false);
            }, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            await UiDispatcher.InvokeAsync(() =>
            {
                if (IsCurrentWorkspaceGeneration(workspaceId, generation, null))
                {
                    PublishFailure(
                        new EngineError(EngineErrorCode.UnexpectedFailure, $"The major records could not be loaded: {exception.Message}"),
                        RetryKind.Page);
                }
            }).ConfigureAwait(false);
        }
        finally
        {
            UiDispatcher.Post(() =>
            {
                if (IsCurrentWorkspaceGeneration(workspaceId, generation, null))
                {
                    SetBusy(false);
                }
            });
        }
    }

    /// <summary>Reads state and one listing page under one coordinator borrow.</summary>
    /// <param name="workspace">The borrowed workspace.</param>
    /// <param name="expectedRevision">The accepted revision, or <see langword="null"/> for the first page.</param>
    /// <param name="continuationToken">The engine-issued next-page token, or <see langword="null"/>.</param>
    /// <param name="cancellationToken">The operation token.</param>
    /// <returns>The page with a revision proven against the current workspace state.</returns>
    private static async ValueTask<EngineResult<MajorRecordListPage>> ReadRecordPageAsync(
        IPluginWorkspace workspace,
        WorkspaceRevision? expectedRevision,
        string? continuationToken,
        CancellationToken cancellationToken)
    {
        var stateResult = await workspace.ReadStateAsync(cancellationToken).ConfigureAwait(false);
        if (!stateResult.Succeeded || stateResult.Value is null)
        {
            return CopyFailure<WorkspaceState, MajorRecordListPage>(stateResult);
        }

        var revision = stateResult.Value.Revision;
        if (stateResult.ResultRevision != revision || (expectedRevision.HasValue && expectedRevision.Value != revision))
        {
            return RevisionFailure<MajorRecordListPage>(
                workspace,
                expectedRevision,
                revision,
                "The workspace changed before the major-record page could be read.",
                stateResult.Warnings);
        }

        var pageResult = await workspace.ListMajorRecordsAsync(
            new MajorRecordListRequest(PageSize, continuationToken, RecordScope.WinningOverrides),
            cancellationToken).ConfigureAwait(false);
        if (!pageResult.Succeeded || pageResult.Value is null)
        {
            return CopyFailure<MajorRecordListPage, MajorRecordListPage>(pageResult);
        }

        var warnings = CombineWarnings(stateResult.Warnings, pageResult.Warnings);
        if (pageResult.ResultRevision != revision)
        {
            return RevisionFailure<MajorRecordListPage>(
                workspace,
                expectedRevision ?? revision,
                pageResult.ResultRevision,
                "The workspace changed while the major-record page was being read.",
                warnings);
        }

        return EngineResult<MajorRecordListPage>.Success(
            pageResult.Value,
            workspace.WorkspaceId,
            baseRevision: expectedRevision ?? revision,
            resultRevision: revision,
            warnings: warnings);
    }

    /// <summary>Clears prior selection work and loads exact contexts for the selected winning record.</summary>
    /// <returns>A task that completes after context options and their default comparison publish.</returns>
    private Task BeginSelectionGenerationAsync()
    {
        SelectionGeneration++;
        CancelAndDispose(ref SelectionCancellation);
        ClearContextPresentation();
        if (SelectedRecordValue is null || !RevisionValue.HasValue || WorkspaceCoordinator.CurrentWorkspace is not { } workspace)
        {
            SetComparisonBusy(false);
            return Task.CompletedTask;
        }

        SelectionCancellation = CancellationTokenSource.CreateLinkedTokenSource(WorkspaceCancellation?.Token ?? CancellationToken.None);
        SetComparisonBusy(true);
        SetStatus($"Loading contexts for {SelectedRecordValue.FormKey}...");
        return LoadContextsAsync(
            workspace.WorkspaceId,
            WorkspaceGeneration,
            SelectionGeneration,
            RevisionValue.Value,
            SelectedRecordValue,
            SelectionCancellation.Token);
    }

    /// <summary>Loads all exact contexts that match one selected FormKey.</summary>
    /// <param name="workspaceId">The captured workspace identity.</param>
    /// <param name="workspaceGeneration">The captured workspace generation.</param>
    /// <param name="selectionGeneration">The captured selection generation.</param>
    /// <param name="revision">The exact record-page revision.</param>
    /// <param name="record">The selected source-plugin record.</param>
    /// <param name="cancellationToken">The selection generation token.</param>
    /// <returns>A task that completes after conditional publication and default comparison.</returns>
    private async Task LoadContextsAsync(
        Guid workspaceId,
        long workspaceGeneration,
        long selectionGeneration,
        WorkspaceRevision revision,
        MajorRecordViewModel record,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await WorkspaceCoordinator.ExecuteAsync(
                (workspace, token) => ReadContextsAtRevisionAsync(workspace, record.FormKey, revision, token),
                cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            var options = result.Succeeded && result.Value is not null
                ? CreateContextOptions(record, result.Value)
                : Array.Empty<MajorRecordContextOption>();
            var published = false;
            await UiDispatcher.InvokeAsync(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!IsCurrentSelectionGeneration(workspaceId, workspaceGeneration, selectionGeneration, revision, record.FormKey))
                {
                    return;
                }

                if (!result.Succeeded || result.Value is null)
                {
                    PublishFailure(result.Error, RetryKind.Contexts);
                    SetWarnings(CombineWarnings(PageWarnings, result.Warnings));
                    return;
                }

                PublishContextOptions(options, result.Warnings);
                published = true;
            }).ConfigureAwait(false);
            if (published)
            {
                await CompareSelectedAsync(
                    workspaceId,
                    workspaceGeneration,
                    selectionGeneration,
                    revision,
                    record.FormKey,
                    cancellationToken).ConfigureAwait(false);
            }
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
                record.FormKey,
                exception,
                RetryKind.Contexts).ConfigureAwait(false);
        }
        finally
        {
            UiDispatcher.Post(() =>
            {
                if (IsCurrentSelectionGeneration(workspaceId, workspaceGeneration, selectionGeneration, revision, record.FormKey))
                {
                    SetComparisonBusy(false);
                }
            });
        }
    }

    /// <summary>Searches all contexts for one exact FormKey while enforcing a fixed workspace revision.</summary>
    /// <param name="workspace">The borrowed workspace.</param>
    /// <param name="formKey">The selected record identity.</param>
    /// <param name="expectedRevision">The accepted record-page revision.</param>
    /// <param name="cancellationToken">The operation token.</param>
    /// <returns>Every exact context match in deterministic engine order.</returns>
    private static async ValueTask<EngineResult<IReadOnlyList<ReferenceSearchMatch>>> ReadContextsAtRevisionAsync(
        IPluginWorkspace workspace,
        FormKey formKey,
        WorkspaceRevision expectedRevision,
        CancellationToken cancellationToken)
    {
        var stateResult = await workspace.ReadStateAsync(cancellationToken).ConfigureAwait(false);
        if (!stateResult.Succeeded || stateResult.Value is null)
        {
            return CopyFailure<WorkspaceState, IReadOnlyList<ReferenceSearchMatch>>(stateResult);
        }

        if (stateResult.Value.Revision != expectedRevision || stateResult.ResultRevision != expectedRevision)
        {
            return RevisionFailure<IReadOnlyList<ReferenceSearchMatch>>(
                workspace,
                expectedRevision,
                stateResult.Value.Revision,
                "The workspace changed before record contexts could be listed.",
                stateResult.Warnings);
        }

        var matches = new List<ReferenceSearchMatch>();
        var warnings = new List<EngineWarning>(stateResult.Warnings);
        string? cursor = null;
        do
        {
            cancellationToken.ThrowIfCancellationRequested();
            var page = await workspace.SearchReferencesAsync(
                new ReferenceSearchRequest(
                    formKey.ToString(),
                    ReferenceSearchRequest.MaximumPageSize,
                    cursor,
                    RecordScope.AllContexts),
                cancellationToken).ConfigureAwait(false);
            if (!page.Succeeded || page.Value is null)
            {
                return CopyFailure<ReferenceSearchPage, IReadOnlyList<ReferenceSearchMatch>>(page);
            }
            if (page.ResultRevision != expectedRevision)
            {
                return RevisionFailure<IReadOnlyList<ReferenceSearchMatch>>(
                    workspace,
                    expectedRevision,
                    page.ResultRevision,
                    "The workspace changed while record contexts were being listed.",
                    CombineWarnings(warnings, page.Warnings));
            }

            warnings.AddRange(page.Warnings);
            matches.AddRange(page.Value.Matches.Where(match => match.FormKey == formKey));
            cursor = page.Value.ContinuationToken;
        }
        while (cursor is not null);

        return EngineResult<IReadOnlyList<ReferenceSearchMatch>>.Success(
            Array.AsReadOnly(matches.ToArray()),
            workspace.WorkspaceId,
            baseRevision: expectedRevision,
            resultRevision: expectedRevision,
            warnings: Array.AsReadOnly(warnings.ToArray()));
    }

    /// <summary>Creates a winning option followed by exact context options.</summary>
    /// <param name="record">The selected source-plugin record.</param>
    /// <param name="contexts">The exact context matches.</param>
    /// <returns>Immutable selector options in display order.</returns>
    private static IReadOnlyList<MajorRecordContextOption> CreateContextOptions(
        MajorRecordViewModel record,
        IReadOnlyList<ReferenceSearchMatch> contexts)
    {
        var winningContext = contexts
            .Where(context => context.ContainingModKey.HasValue)
            .OrderBy(context => context.LoadOrderIndex ?? int.MinValue)
            .LastOrDefault();
        var winningModKey = winningContext?.ContainingModKey ?? record.ContainingModKey;
        var winnerLabel = winningModKey.HasValue
            ? $"Winning override ({winningModKey.Value.FileName})"
            : "Winning override";
        var options = new List<MajorRecordContextOption>(contexts.Count + 1)
        {
            new(
                new ReferenceRequest(record.FormKey, RecordScope.WinningOverrides),
                winnerLabel,
                winningContext?.SourcePath ?? record.SourcePath,
                winningContext?.LoadOrderIndex ?? record.LoadOrderIndex,
                winningContext?.Role ?? record.Role,
                winningContext?.IsDeleted ?? record.IsDeleted)
        };
        foreach (var context in contexts)
        {
            if (!context.ContainingModKey.HasValue)
            {
                continue;
            }

            var role = context.Role?.ToString() ?? "Context";
            var index = context.LoadOrderIndex?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "?";
            options.Add(new MajorRecordContextOption(
                new ReferenceRequest(record.FormKey, RecordScope.AllContexts, context.ContainingModKey),
                $"[{index}] {context.ContainingModKey.Value.FileName} ({role}){(context.IsDeleted ? " - deleted" : string.Empty)}",
                context.SourcePath,
                context.LoadOrderIndex,
                context.Role,
                context.IsDeleted));
        }

        return Array.AsReadOnly(options.ToArray());
    }
}
