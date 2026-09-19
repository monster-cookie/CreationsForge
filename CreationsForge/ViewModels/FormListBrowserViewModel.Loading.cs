using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Services;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.ViewModels;

public sealed partial class FormListBrowserViewModel
{
    /// <summary>Starts a new workspace generation and clears every stale record and comparison projection.</summary>
    /// <param name="workspace">The newly published workspace descriptor, or <see langword="null"/> after close.</param>
    /// <param name="select">The FormList to reselect after loading, or <see langword="null"/>.</param>
    /// <param name="cancellationToken">An optional caller token linked to this new generation.</param>
    /// <returns>A task that completes after this generation loads or is superseded.</returns>
    private Task BeginWorkspaceGeneration(
        WorkspaceDescriptor? workspace,
        FormKey? select = null,
        CancellationToken cancellationToken = default)
    {
        ResetWorkspaceGeneration();
        if (workspace is null)
        {
            SetStatus("No workspace is open.");
            return Task.CompletedTask;
        }

        var loadCancellation = CreateWorkspaceLoadCancellation(cancellationToken);
        var generation = WorkspaceGeneration;
        SetStatus("Loading plugins and FormLists...");
        return LoadWorkspaceAndDisposeCancellationAsync(
            workspace,
            generation,
            select,
            loadCancellation);
    }

    /// <summary>Cancels older browser work and clears all state derived from its workspace generation.</summary>
    private void ResetWorkspaceGeneration()
    {
        WorkspaceGeneration++;
        ComparisonGeneration++;
        CancelAndDispose(ref ComparisonCancellation);
        CancelAndDispose(ref WorkspaceCancellation);
        ClearEditorSelection();
        ClearWorkspacePresentation();
        OnPropertyChanged(nameof(HasWorkspace));
        RetryRelayCommand.RaiseCanExecuteChanged();
        PickReferenceRelayCommand.RaiseCanExecuteChanged();
        RefreshRelayCommand.RaiseCanExecuteChanged();
    }

    /// <summary>Creates a new workspace-generation token and links it to one caller-owned refresh token.</summary>
    /// <param name="cancellationToken">The caller token linked only to this load.</param>
    /// <returns>The task-owned linked cancellation source.</returns>
    private CancellationTokenSource CreateWorkspaceLoadCancellation(CancellationToken cancellationToken)
    {
        WorkspaceCancellation = new CancellationTokenSource();
        return CancellationTokenSource.CreateLinkedTokenSource(
            WorkspaceCancellation.Token,
            cancellationToken);
    }

    /// <summary>Loads one workspace generation and releases its caller-linked refresh token registration.</summary>
    /// <param name="workspace">The workspace identity captured for this generation.</param>
    /// <param name="generation">The workspace generation captured for this load.</param>
    /// <param name="select">The FormList to reselect after a successful load, or <see langword="null"/>.</param>
    /// <param name="loadCancellation">The task-owned token source linked to the workspace generation and refresh caller.</param>
    /// <returns>A task that completes after load publication or stale-result suppression and token disposal.</returns>
    private async Task LoadWorkspaceAndDisposeCancellationAsync(
        WorkspaceDescriptor workspace,
        long generation,
        FormKey? select,
        CancellationTokenSource loadCancellation)
    {
        using (loadCancellation)
        {
            await LoadWorkspaceAsync(
                workspace,
                generation,
                select,
                loadCancellation.Token).ConfigureAwait(false);
        }
    }

    /// <summary>Loads one revision-consistent detached snapshot through the coordinator borrowing boundary.</summary>
    /// <param name="workspace">The workspace identity captured for this generation.</param>
    /// <param name="generation">The workspace generation captured for this load.</param>
    /// <param name="select">The FormList to reselect after a successful load, or <see langword="null"/>.</param>
    /// <param name="cancellationToken">The generation cancellation token.</param>
    /// <returns>A task that completes after publication or stale-result suppression.</returns>
    private async Task LoadWorkspaceAsync(
        WorkspaceDescriptor workspace,
        long generation,
        FormKey? select,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await WorkspaceCoordinator.ExecuteAsync(
                ReadBrowserSnapshotAsync,
                cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            IReadOnlyList<FormListRecordViewModel>? records = null;
            if (result.Succeeded && result.Value is not null)
            {
                records = await Task.Run(
                    () => BuildRecordTree(result.Value.FormLists, cancellationToken),
                    cancellationToken).ConfigureAwait(false);
            }

            Task selectionTask = Task.CompletedTask;
            await UiDispatcher.InvokeAsync(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!IsCurrentWorkspaceGeneration(workspace.WorkspaceId, generation))
                {
                    return;
                }

                if (!result.Succeeded || result.Value is null)
                {
                    PublishFailure(result.Error, RetryKind.Load);
                    SetWarnings(result.Warnings);
                    return;
                }

                var selectionTarget = PublishSnapshot(
                    result.Value,
                    records!,
                    result.Warnings,
                    select,
                    preferStagedOutput: true);
                if (selectionTarget is not null)
                {
                    selectionTask = SelectRecordAsync(selectionTarget, cancellationToken);
                }
            }).ConfigureAwait(false);
            await selectionTask.ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            await UiDispatcher.InvokeAsync(() =>
            {
                if (IsCurrentWorkspaceGeneration(workspace.WorkspaceId, generation))
                {
                    SetStatus("FormList refresh canceled.");
                }
            }).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            await PublishUnexpectedFailureAsync(
                workspace.WorkspaceId,
                generation,
                exception,
                RetryKind.Load).ConfigureAwait(false);
        }
    }

    /// <summary>Reads state, plugins, and all FormList contexts while one coordinator borrow excludes replacement.</summary>
    /// <param name="workspace">The currently borrowed workspace.</param>
    /// <param name="cancellationToken">The generation cancellation token.</param>
    /// <returns>A detached revision-consistent browser snapshot or typed engine failure.</returns>
    private static async ValueTask<EngineResult<FormListBrowserSnapshot>> ReadBrowserSnapshotAsync(
        IPluginWorkspace workspace,
        CancellationToken cancellationToken)
    {
        var stateResult = await workspace.ReadStateAsync(cancellationToken).ConfigureAwait(false);
        if (!stateResult.Succeeded || stateResult.Value is null)
        {
            return CopyFailure<WorkspaceState, FormListBrowserSnapshot>(stateResult);
        }

        if (stateResult.WorkspaceId != workspace.WorkspaceId)
        {
            return EngineResult<FormListBrowserSnapshot>.Failure(
                CreateRevisionError("The plugin browser state read returned a different workspace identity."),
                workspace.WorkspaceId,
                resultRevision: stateResult.ResultRevision,
                warnings: stateResult.Warnings);
        }

        var pluginsResult = await workspace.ListPluginsAsync(cancellationToken).ConfigureAwait(false);
        if (!pluginsResult.Succeeded || pluginsResult.Value is null)
        {
            return CopyFailure<IReadOnlyList<PluginSummary>, FormListBrowserSnapshot>(pluginsResult);
        }

        if (pluginsResult.WorkspaceId != workspace.WorkspaceId)
        {
            return EngineResult<FormListBrowserSnapshot>.Failure(
                CreateRevisionError("The plugin browser plugin read returned a different workspace identity."),
                workspace.WorkspaceId,
                resultRevision: pluginsResult.ResultRevision,
                warnings: CombineWarnings(stateResult.Warnings, pluginsResult.Warnings));
        }

        var formListsResult = await workspace.ListFormListsAsync(RecordScope.AllContexts, cancellationToken).ConfigureAwait(false);
        if (!formListsResult.Succeeded || formListsResult.Value is null)
        {
            return CopyFailure<IReadOnlyList<FormListSummary>, FormListBrowserSnapshot>(formListsResult);
        }

        if (formListsResult.WorkspaceId != workspace.WorkspaceId)
        {
            return EngineResult<FormListBrowserSnapshot>.Failure(
                CreateRevisionError("The plugin browser FormList read returned a different workspace identity."),
                workspace.WorkspaceId,
                resultRevision: formListsResult.ResultRevision,
                warnings: CombineWarnings(stateResult.Warnings, pluginsResult.Warnings, formListsResult.Warnings));
        }

        var revision = stateResult.Value.Revision;
        if (stateResult.ResultRevision != revision ||
            pluginsResult.ResultRevision != revision ||
            formListsResult.ResultRevision != revision)
        {
            return EngineResult<FormListBrowserSnapshot>.Failure(
                CreateRevisionError("The plugin browser snapshot changed while it was being read."),
                workspace.WorkspaceId,
                resultRevision: revision,
                warnings: CombineWarnings(stateResult.Warnings, pluginsResult.Warnings, formListsResult.Warnings));
        }

        return EngineResult<FormListBrowserSnapshot>.Success(
            new FormListBrowserSnapshot(stateResult.Value, pluginsResult.Value, formListsResult.Value),
            workspace.WorkspaceId,
            baseRevision: revision,
            resultRevision: revision,
            warnings: CombineWarnings(stateResult.Warnings, pluginsResult.Warnings, formListsResult.Warnings));
    }

    /// <summary>Publishes a successful detached snapshot and its exact plugin ordering.</summary>
    /// <param name="snapshot">The revision-consistent detached snapshot.</param>
    /// <param name="records">The grouped presentation records projected off the UI thread.</param>
    /// <param name="warnings">The combined engine warnings.</param>
    /// <param name="select">The FormList to reselect, or <see langword="null"/>.</param>
    /// <param name="preferStagedOutput">Whether to prefer an exact output context for the requested FormList.</param>
    /// <returns>The current tree node to select after publication, or <see langword="null"/>.</returns>
    private FormListRecordViewModel? PublishSnapshot(
        FormListBrowserSnapshot snapshot,
        IReadOnlyList<FormListRecordViewModel> records,
        IReadOnlyList<EngineWarning> warnings,
        FormKey? select,
        bool preferStagedOutput)
    {
        WorkspaceStateValue = snapshot.State;
        OnPropertyChanged(nameof(HasWorkspace));
        PluginsValue = snapshot.Plugins;
        OnPropertyChanged(nameof(Plugins));
        OnPropertyChanged(nameof(ActivePluginRecordCountText));
        OnPropertyChanged(nameof(LoadedRecordCountText));
        SetAllRecords(records);
        LoadWarnings = Array.AsReadOnly(warnings.ToArray());
        SetWarnings(LoadWarnings);
        ClearError();
        RetryKindValue = RetryKind.None;
        SetStatus(RecordsValue.Count == 0
            ? "The workspace contains no FormLists."
            : $"Loaded {RecordsValue.Count} FormList record(s) from {PluginsValue.Count} plugin(s).");
        PickReferenceRelayCommand.RaiseCanExecuteChanged();
        RefreshRelayCommand.RaiseCanExecuteChanged();
        if (!select.HasValue)
        {
            return null;
        }

        var root = RecordsValue.FirstOrDefault(record => record.FormKey == select.Value);
        if (root is null || !preferStagedOutput)
        {
            return root;
        }

        return root.Contexts.LastOrDefault(context => context.Context.Role == PluginRole.Output) ?? root;
    }

}
