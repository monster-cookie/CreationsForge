using CreationsForge.Core.Engine.Contracts;
using CreationsForge.NativeEditing;
using CreationsForge.NativeEditing.Drafts;
using CreationsForge.Services;

namespace CreationsForge.ViewModels;

public sealed partial class NativeFormListEditorViewModel
{
    /// <summary>Begins a new FormList edit from a fresh atomic workspace revision.</summary>
    /// <returns>A task that completes after session publication or failure.</returns>
    public Task BeginNewAsync()
    {
        return BeginNewAsync(CancellationToken.None);
    }

    /// <summary>Begins a new FormList edit from a fresh atomic workspace revision.</summary>
    /// <param name="cancellationToken">A token that may leave an entered mutation outcome uncertain.</param>
    /// <returns>A task that completes after session publication or failure.</returns>
    public Task BeginNewAsync(CancellationToken cancellationToken)
    {
        return BeginAsync(FormListEditRole.New, cancellationToken);
    }

    /// <summary>Begins an override of the exact revision-bound non-output browser selection.</summary>
    /// <returns>A task that completes after session publication or failure.</returns>
    public Task BeginOverrideAsync()
    {
        return BeginOverrideAsync(CancellationToken.None);
    }

    /// <summary>Begins an override of the exact revision-bound non-output browser selection.</summary>
    /// <param name="cancellationToken">A token that may leave an entered mutation outcome uncertain.</param>
    /// <returns>A task that completes after session publication or failure.</returns>
    public Task BeginOverrideAsync(CancellationToken cancellationToken)
    {
        return BeginAsync(FormListEditRole.Override, cancellationToken);
    }

    /// <summary>Begins editing the exact revision-bound staged-output browser selection.</summary>
    /// <returns>A task that completes after session publication or failure.</returns>
    public Task BeginExistingOutputAsync()
    {
        return BeginExistingOutputAsync(CancellationToken.None);
    }

    /// <summary>Begins editing the exact revision-bound staged-output browser selection.</summary>
    /// <param name="cancellationToken">A token that may leave an entered mutation outcome uncertain.</param>
    /// <returns>A task that completes after session publication or failure.</returns>
    public Task BeginExistingOutputAsync(CancellationToken cancellationToken)
    {
        return BeginAsync(FormListEditRole.ExistingOutput, cancellationToken);
    }

    /// <summary>Runs one exact New, Override, or ExistingOutput Begin lifecycle.</summary>
    /// <param name="role">The requested Core edit role.</param>
    /// <param name="cancellationToken">The optional caller cancellation token.</param>
    /// <returns>A task that completes after known success, definitive rejection, cancellation, or uncertain-outcome publication.</returns>
    private async Task BeginAsync(FormListEditRole role, CancellationToken cancellationToken)
    {
        if (!CanBegin)
        {
            return;
        }

        using var operationLease = TryEnterOperation(NativeFormListEditorOperationState.Beginning);
        if (operationLease is null)
        {
            return;
        }

        var generation = Volatile.Read(ref WorkspaceGeneration);
        var descriptor = WorkspaceCoordinator.CurrentWorkspace;
        var selection = Host.Selection;
        NativeFormListEditorPendingBegin? attemptedOperation = null;
        var coreResponded = false;
        try
        {
            if (descriptor is null)
            {
                await PublishBeginFailureAsync(generation, Guid.Empty, new EngineError(EngineErrorCode.InvalidRequest, "No native workspace is open.")).ConfigureAwait(false);
                return;
            }

            var targetError = ValidateBeginSelection(role, descriptor, selection);
            if (targetError is not null)
            {
                await PublishBeginFailureAsync(generation, descriptor.WorkspaceId, targetError).ConfigureAwait(false);
                return;
            }

            using var linkedCancellation = CreateGenerationLinkedCancellation(
                cancellationToken,
                operationLease.CancellationToken);
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

                    var target = CreateBeginTarget(role, descriptor, stateResult.Value, selection);
                    var request = target.CreateRequest(Guid.NewGuid());
                    attemptedOperation = new NativeFormListEditorPendingBegin(descriptor.WorkspaceId, request, DescribeBeginRole(role));
                    var beginResult = await workspace.BeginEditAsync(request, token).ConfigureAwait(false);
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
                        request.OperationId,
                        request.ExpectedRevision,
                        beginResult.Value.Revision,
                        outcome.Warnings);
                },
                linkedCancellation.Token).ConfigureAwait(false);

            if (!result.Succeeded || result.Value is null)
            {
                if (attemptedOperation is not null && !coreResponded)
                {
                    await PublishUncertainOperationAsync(generation, descriptor.WorkspaceId, attemptedOperation).ConfigureAwait(false);
                }
                else
                {
                    await PublishBeginFailureAsync(generation, descriptor.WorkspaceId, result.Error).ConfigureAwait(false);
                }

                return;
            }

            await PublishBeginSuccessAsync(generation, descriptor, result.Value, linkedCancellation.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            if (descriptor is not null && IsCurrentGeneration(generation, descriptor.WorkspaceId))
            {
                if (attemptedOperation is not null && !coreResponded)
                {
                    await PublishUncertainOperationAsync(generation, descriptor.WorkspaceId, attemptedOperation).ConfigureAwait(false);
                }
                else
                {
                    await PublishBeginFailureAsync(generation, descriptor.WorkspaceId, new EngineError(EngineErrorCode.InvalidRequest, "The editor Begin operation was canceled before mutation.")).ConfigureAwait(false);
                }
            }
        }
        catch (Exception exception)
        {
            if (descriptor is not null && IsCurrentGeneration(generation, descriptor.WorkspaceId))
            {
                if (attemptedOperation is not null && !coreResponded)
                {
                    await PublishUncertainOperationAsync(generation, descriptor.WorkspaceId, attemptedOperation).ConfigureAwait(false);
                }
                else
                {
                    await PublishBeginFailureAsync(generation, descriptor.WorkspaceId, new EngineError(EngineErrorCode.UnexpectedFailure, exception.Message)).ConfigureAwait(false);
                }
            }
        }
        finally
        {
            await ExitOperationAsync(generation).ConfigureAwait(false);
        }
    }

    /// <summary>Validates role-specific browser selection before borrowing the native workspace.</summary>
    /// <param name="role">The requested edit role.</param>
    /// <param name="descriptor">The current active workspace descriptor.</param>
    /// <param name="selection">The atomic browser selection, when available.</param>
    /// <returns>A typed local failure, or <see langword="null"/>.</returns>
    private static EngineError? ValidateBeginSelection(
        FormListEditRole role,
        NativeWorkspaceDescriptor descriptor,
        NativeFormListEditorSelection? selection)
    {
        if (role == FormListEditRole.New)
        {
            return null;
        }

        if (selection is null || selection.WorkspaceId != descriptor.WorkspaceId)
        {
            return new EngineError(EngineErrorCode.InvalidRequest, "Select one exact FormList context from the current workspace before beginning this edit.");
        }

        if (role == FormListEditRole.Override && (selection.IsStagedOutput || selection.ExactReferenceRequest is null))
        {
            return new EngineError(EngineErrorCode.InvalidRequest, "Override requires an exact non-output FormList context.");
        }

        if (role == FormListEditRole.ExistingOutput && !selection.IsStagedOutput)
        {
            return new EngineError(EngineErrorCode.InvalidRequest, "Edit staged output requires an exact output FormList context.");
        }

        return null;
    }

    /// <summary>Creates the immutable exact Begin target while preserving selected revisions unchanged.</summary>
    /// <param name="role">The requested edit role.</param>
    /// <param name="descriptor">The active workspace descriptor.</param>
    /// <param name="state">The fresh atomic workspace state used only by New.</param>
    /// <param name="selection">The captured exact browser selection used by Override or ExistingOutput.</param>
    /// <returns>The immutable validated editor target.</returns>
    private static NativeFormListEditorTarget CreateBeginTarget(
        FormListEditRole role,
        NativeWorkspaceDescriptor descriptor,
        WorkspaceState state,
        NativeFormListEditorSelection? selection)
    {
        return role switch
        {
            FormListEditRole.New => new NativeFormListEditorTarget(descriptor.WorkspaceId, state.Revision, role, null, null, null),
            FormListEditRole.Override => new NativeFormListEditorTarget(descriptor.WorkspaceId, selection!.Revision, role, selection.FormKey, selection.ExactReferenceRequest, null),
            FormListEditRole.ExistingOutput => new NativeFormListEditorTarget(descriptor.WorkspaceId, selection!.Revision, role, null, null, selection.FormKey),
            _ => throw new ArgumentOutOfRangeException(nameof(role)),
        };
    }

    /// <summary>Reads the exact staged seed and preview after Core has returned a successful Begin receipt.</summary>
    /// <param name="workspace">The still-borrowed native workspace.</param>
    /// <param name="descriptor">The captured workspace descriptor.</param>
    /// <param name="catalogContext">The exact catalog and codec pair.</param>
    /// <param name="commands">Every admitted command presentation.</param>
    /// <param name="receipt">The successful Begin receipt.</param>
    /// <param name="initialWarnings">Warnings already returned by state and Begin.</param>
    /// <param name="cancellationToken">The operation token used only for post-mutation reads.</param>
    /// <returns>A known-success outcome even when seed or preview capture fails.</returns>
    private async ValueTask<NativeFormListEditorBeginOutcome> ReadBeginFollowUpAsync(
        IFormListWorkspace workspace,
        NativeWorkspaceDescriptor descriptor,
        NativeFormListWireCatalogContext catalogContext,
        IReadOnlyList<NativeFormListCommandPresentation> commands,
        EditReceipt receipt,
        IEnumerable<EngineWarning> initialWarnings,
        CancellationToken cancellationToken)
    {
        var warnings = new List<EngineWarning>(initialWarnings);
        NativeFormListDraftSeed? seed = null;
        var messages = new List<string>();
        try
        {
            var viewResult = await workspace.ReadFormListViewAsync(
                new ReferenceRequest(receipt.FormKey, RecordScope.StagedOutput, descriptor.Output.ModKey),
                cancellationToken).ConfigureAwait(false);
            warnings.AddRange(viewResult.Warnings);
            if (viewResult.Succeeded &&
                viewResult.Value?.Record is { } record &&
                viewResult.ResultRevision == receipt.Revision)
            {
                var seedResult = DraftFactory.CaptureSeed(
                    catalogContext,
                    descriptor.WorkspaceId,
                    receipt.Revision,
                    viewResult.Value.Context,
                    record,
                    ReadLimits,
                    cancellationToken);
                if (seedResult.Succeeded)
                {
                    seed = seedResult.Value;
                }
                else
                {
                    messages.Add($"staged seed failed: {seedResult.Error?.Message ?? "no diagnostic reason"}");
                }
            }
            else
            {
                messages.Add($"staged seed failed: {viewResult.Error?.Message ?? "the staged record was not returned at the receipt revision"}");
            }
        }
        catch (Exception exception)
        {
            messages.Add($"staged seed failed: {exception.Message}");
        }

        var previewKnown = false;
        var hasStagedChanges = false;
        try
        {
            var previewResult = await workspace.PreviewAsync(cancellationToken).ConfigureAwait(false);
            warnings.AddRange(previewResult.Warnings);
            if (previewResult.Succeeded && previewResult.Value is not null && previewResult.ResultRevision == receipt.Revision)
            {
                previewKnown = true;
                var comparison = previewResult.Value.Comparisons.FirstOrDefault(candidate => candidate.FormKey == receipt.FormKey);
                hasStagedChanges = receipt.Role is FormListEditRole.New or FormListEditRole.Override || comparison?.Changes.Count > 0;
                warnings.AddRange(previewResult.Value.Warnings);
            }
            else
            {
                messages.Add($"preview failed: {previewResult.Error?.Message ?? "the preview did not match the Begin receipt revision"}");
            }
        }
        catch (Exception exception)
        {
            messages.Add($"preview failed: {exception.Message}");
        }

        return new NativeFormListEditorBeginOutcome(
            receipt,
            catalogContext,
            commands,
            seed,
            previewKnown,
            hasStagedChanges,
            warnings,
            messages.Count == 0 ? null : string.Join("; ", messages));
    }

    /// <summary>Publishes a successful Begin receipt before attempting browser refresh.</summary>
    /// <param name="generation">The captured workspace generation.</param>
    /// <param name="descriptor">The captured workspace descriptor.</param>
    /// <param name="outcome">The known successful Begin outcome.</param>
    /// <param name="cancellationToken">The post-mutation refresh token.</param>
    /// <returns>A task that completes after publication and browser refresh handling.</returns>
    private async Task PublishBeginSuccessAsync(
        long generation,
        NativeWorkspaceDescriptor descriptor,
        NativeFormListEditorBeginOutcome outcome,
        CancellationToken cancellationToken)
    {
        await UiDispatcher.InvokeAsync(() =>
        {
            if (!IsCurrentGeneration(generation, descriptor.WorkspaceId))
            {
                return;
            }

            PendingOperationValue = null;
            SessionValue = new NativeFormListEditorSession(
                descriptor.WorkspaceId,
                descriptor.Game,
                descriptor.Release,
                descriptor.Output,
                outcome.CatalogContext.Identity,
                outcome.Receipt);
            CatalogContextValue = outcome.CatalogContext;
            SeedValue = outcome.Seed;
            AvailableCommandsValue = outcome.Commands;
            SelectedCommandValue = null;
            DetachDraft();
            IsStagedChangesKnownValue = outcome.PreviewKnown;
            HasStagedChangesValue = outcome.HasStagedChanges;
            SetWarnings(outcome.Warnings);
            ClearError();
            RaiseSessionProperties();
            EngineError? draftError = null;
            if (outcome.Commands.Count > 0)
            {
                var draftResult = CreateDraft(
                    outcome.Commands[0],
                    NativeFormListDraftSeedSelection.CurrentValue(),
                    allowDuringBegin: true);
                draftError = draftResult.Succeeded
                    ? null
                    : draftResult.Error ?? new EngineError(EngineErrorCode.ValidationFailed, "The initial command draft could not be created.");
            }
            else
            {
                draftError = new EngineError(EngineErrorCode.ValidationFailed, "The exact native wire catalog contains no editor commands.");
            }

            if (draftError is not null)
            {
                var message = outcome.PostMutationMessage is null
                    ? $"{DescribeBeginRole(outcome.Receipt.Role)} succeeded; initial draft failed: {draftError.Message}"
                    : $"{DescribeBeginRole(outcome.Receipt.Role)} succeeded; {outcome.PostMutationMessage}; initial draft failed: {draftError.Message}";
                PublishError(new EngineError(draftError.Code, message));
            }
            else if (outcome.PostMutationMessage is not null)
            {
                PublishError(new EngineError(EngineErrorCode.UnexpectedFailure, $"{DescribeBeginRole(outcome.Receipt.Role)} succeeded; {outcome.PostMutationMessage}."));
            }
            else
            {
                SetStatus($"{DescribeBeginRole(outcome.Receipt.Role)} succeeded for {outcome.Receipt.FormKey}.");
            }
        }).ConfigureAwait(false);

        if (!IsCurrentGeneration(generation, descriptor.WorkspaceId))
        {
            return;
        }

        try
        {
            await Host.RefreshAsync(outcome.Receipt.FormKey, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            await UiDispatcher.InvokeAsync(() =>
            {
                if (IsCurrentGeneration(generation, descriptor.WorkspaceId))
                {
                    PublishError(new EngineError(EngineErrorCode.UnexpectedFailure, $"{DescribeBeginRole(outcome.Receipt.Role)} succeeded; refresh failed: {exception.Message}"));
                }
            }).ConfigureAwait(false);
        }
    }

    /// <summary>Publishes a definitive local or Core Begin failure without discarding an existing session or draft.</summary>
    /// <param name="generation">The captured workspace generation.</param>
    /// <param name="workspaceId">The captured workspace identity.</param>
    /// <param name="error">The typed failure.</param>
    /// <returns>A task that completes after current-generation publication.</returns>
    private Task PublishBeginFailureAsync(long generation, Guid workspaceId, EngineError? error)
    {
        return UiDispatcher.InvokeAsync(() =>
        {
            if (workspaceId != Guid.Empty && !IsCurrentGeneration(generation, workspaceId))
            {
                return;
            }

            PendingOperationValue = null;
            PublishError(error);
            RaiseSessionProperties();
        });
    }

    /// <summary>Returns the stable user-facing name for one Begin role.</summary>
    /// <param name="role">The Core edit role.</param>
    /// <returns>The corresponding editor action name.</returns>
    private static string DescribeBeginRole(FormListEditRole role)
    {
        return role switch
        {
            FormListEditRole.New => "New",
            FormListEditRole.Override => "Override selected",
            FormListEditRole.ExistingOutput => "Edit staged output",
            _ => throw new ArgumentOutOfRangeException(nameof(role)),
        };
    }
}
