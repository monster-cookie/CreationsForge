using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.RecordEditing.Drafts;
using CreationsForge.Services;

namespace CreationsForge.ViewModels;

public sealed partial class FormListEditorViewModel
{
    /// <summary>Validates, serializes, decodes, and applies the current typed command draft.</summary>
    /// <returns>A task that completes after known success, definitive failure, or uncertain-outcome publication.</returns>
    public Task ApplyAsync()
    {
        return ApplyAsync(CancellationToken.None);
    }

    /// <summary>Validates, serializes, decodes, and applies the current typed command draft.</summary>
    /// <param name="cancellationToken">A token that may leave an entered mutation outcome uncertain.</param>
    /// <returns>A task that completes after known success, definitive failure, or uncertain-outcome publication.</returns>
    public async Task ApplyAsync(CancellationToken cancellationToken)
    {
        if (!CanApply ||
            SessionValue is not { } session ||
            DraftValue is not { } draft)
        {
            return;
        }

        using var operationLease = TryEnterOperation(FormListEditorOperationState.Applying);
        if (operationLease is null)
        {
            return;
        }

        var generation = Volatile.Read(ref WorkspaceGeneration);
        var descriptor = WorkspaceCoordinator.CurrentWorkspace;
        FormListEditorPendingApply? attemptedOperation = null;
        var coreResponded = false;
        try
        {
            if (descriptor is null || descriptor.Output is null || descriptor.WorkspaceId != session.WorkspaceId)
            {
                await PublishApplyFailureAsync(generation, session.WorkspaceId, draft, new EngineError(EngineErrorCode.InvalidRequest, "The active workspace does not match the editor session.")).ConfigureAwait(false);
                return;
            }

            var validation = DraftValidator.Validate(draft, ReadLimits, cancellationToken);
            await UiDispatcher.InvokeAsync(() =>
            {
                if (IsCurrentGeneration(generation, session.WorkspaceId) && ReferenceEquals(DraftValue, draft))
                {
                    ValidationIssuesValue = validation.Issues;
                    OnPropertyChanged(nameof(ValidationIssues));
                    OnPropertyChanged(nameof(IsValid));
                }
            }).ConfigureAwait(false);
            if (!validation.IsValid)
            {
                await PublishApplyFailureAsync(generation, session.WorkspaceId, draft, new EngineError(EngineErrorCode.ValidationFailed, "The typed command draft contains validation errors.")).ConfigureAwait(false);
                return;
            }

            var serialization = DraftSerializer.Serialize(draft, ReadLimits, cancellationToken);
            if (!serialization.Succeeded)
            {
                await UiDispatcher.InvokeAsync(() =>
                {
                    if (IsCurrentGeneration(generation, session.WorkspaceId) && ReferenceEquals(DraftValue, draft))
                    {
                        ValidationIssuesValue = serialization.Issues;
                        OnPropertyChanged(nameof(ValidationIssues));
                        OnPropertyChanged(nameof(IsValid));
                        PublishError(new EngineError(EngineErrorCode.ValidationFailed, "The typed command draft could not be serialized."));
                    }
                }).ConfigureAwait(false);
                return;
            }

            var arguments = serialization.GetArguments();
            var expectedRevision = session.ExpectedRevision;
            using var linkedCancellation = CreateGenerationLinkedCancellation(
                cancellationToken,
                operationLease.CancellationToken);
            var result = await WorkspaceCoordinator.ExecuteAsync(
                async (workspace, token) =>
                {
                    var stateResult = await workspace.ReadStateAsync(token).ConfigureAwait(false);
                    if (!stateResult.Succeeded || stateResult.Value is null)
                    {
                        return CopyFailure<WorkspaceState, FormListEditorApplyOutcome>(stateResult);
                    }

                    var stateError = ValidateSessionState(workspace, stateResult.Value, descriptor, session);
                    if (stateError is not null)
                    {
                        return EngineResult<FormListEditorApplyOutcome>.Failure(stateError, workspace.WorkspaceId, resultRevision: stateResult.Value.Revision);
                    }

                    var catalogResult = CatalogResolver.Resolve(stateResult.Value.Game, stateResult.Value.Release);
                    if (!catalogResult.Succeeded || catalogResult.Value is null)
                    {
                        return CopyFailure<FormListWireCatalogContext, FormListEditorApplyOutcome>(catalogResult);
                    }

                    if (!SameCatalogIdentity(catalogResult.Value.Identity, session.CatalogIdentity) ||
                        !SameCatalogIdentity(draft.CatalogIdentity, session.CatalogIdentity))
                    {
                        return Failure<FormListEditorApplyOutcome>(EngineErrorCode.ExternalChangeDetected, "The plugin wire catalog no longer matches the active editor session.");
                    }

                    var decode = catalogResult.Value.Codec.Decode(draft.CommandName, arguments, ReadLimits, token);
                    if (!decode.Succeeded || decode.Value is null)
                    {
                        return EngineResult<FormListEditorApplyOutcome>.Failure(
                            decode.Error ?? new EngineError(EngineErrorCode.ValidationFailed, "The exact plugin codec rejected the typed command without a diagnostic reason."),
                            workspace.WorkspaceId,
                            baseRevision: expectedRevision,
                            resultRevision: stateResult.Value.Revision);
                    }

                    var operationId = Guid.NewGuid();
                    attemptedOperation = new FormListEditorPendingApply(
                        session.WorkspaceId,
                        expectedRevision,
                        operationId,
                        session.EditId,
                        session.CatalogIdentity,
                        draft.CommandName,
                        arguments,
                        ReadLimits);
                    var request = new FormListEditRequest(operationId, expectedRevision, session.EditId, decode.Value);
                    var applyResult = await workspace.ApplyFormListEditAsync(request, token).ConfigureAwait(false);
                    coreResponded = true;
                    if (!applyResult.Succeeded || applyResult.Value is null)
                    {
                        return CopyFailure<OperationReceipt, FormListEditorApplyOutcome>(applyResult);
                    }

                    var outcome = await ReadApplyFollowUpAsync(
                        workspace,
                        descriptor,
                        catalogResult.Value,
                        session,
                        applyResult.Value,
                        stateResult.Warnings.Concat(applyResult.Warnings),
                        token).ConfigureAwait(false);
                    return EngineResult<FormListEditorApplyOutcome>.Success(
                        outcome,
                        workspace.WorkspaceId,
                        operationId,
                        expectedRevision,
                        applyResult.Value.Revision,
                        outcome.Warnings);
                },
                linkedCancellation.Token).ConfigureAwait(false);

            if (!result.Succeeded || result.Value is null)
            {
                if (attemptedOperation is not null && !coreResponded)
                {
                    await PublishUncertainOperationAsync(generation, session.WorkspaceId, attemptedOperation).ConfigureAwait(false);
                }
                else
                {
                    await PublishApplyFailureAsync(generation, session.WorkspaceId, draft, result.Error).ConfigureAwait(false);
                }

                return;
            }

            await PublishApplySuccessAsync(generation, descriptor, session, draft, result.Value, linkedCancellation.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            if (IsCurrentGeneration(generation, session.WorkspaceId))
            {
                if (attemptedOperation is not null && !coreResponded)
                {
                    await PublishUncertainOperationAsync(generation, session.WorkspaceId, attemptedOperation).ConfigureAwait(false);
                }
                else
                {
                    await PublishApplyFailureAsync(generation, session.WorkspaceId, draft, new EngineError(EngineErrorCode.InvalidRequest, "Apply was canceled before mutation.")).ConfigureAwait(false);
                }
            }
        }
        catch (Exception exception)
        {
            if (IsCurrentGeneration(generation, session.WorkspaceId))
            {
                if (attemptedOperation is not null && !coreResponded)
                {
                    await PublishUncertainOperationAsync(generation, session.WorkspaceId, attemptedOperation).ConfigureAwait(false);
                }
                else
                {
                    await PublishApplyFailureAsync(generation, session.WorkspaceId, draft, new EngineError(EngineErrorCode.UnexpectedFailure, exception.Message)).ConfigureAwait(false);
                }
            }
        }
        finally
        {
            await ExitOperationAsync(generation).ConfigureAwait(false);
        }
    }

    /// <summary>Reads matching preview state and a receipt-bound staged seed after known Apply success.</summary>
    /// <param name="workspace">The still-borrowed workspace.</param>
    /// <param name="descriptor">The captured workspace descriptor.</param>
    /// <param name="catalogContext">The exact catalog and codec pair.</param>
    /// <param name="session">The session whose FormList was mutated.</param>
    /// <param name="receipt">The known successful Apply receipt.</param>
    /// <param name="initialWarnings">Warnings already returned by state and Apply.</param>
    /// <param name="cancellationToken">The post-mutation read token.</param>
    /// <returns>A known-success outcome even when preview or seed capture fails.</returns>
    private async ValueTask<FormListEditorApplyOutcome> ReadApplyFollowUpAsync(
        IPluginWorkspace workspace,
        WorkspaceDescriptor descriptor,
        FormListWireCatalogContext catalogContext,
        FormListEditorSession session,
        OperationReceipt receipt,
        IEnumerable<EngineWarning> initialWarnings,
        CancellationToken cancellationToken)
    {
        var warnings = new List<EngineWarning>(initialWarnings);
        var messages = new List<string>();
        FormListDraftSeed? seed = null;
        var previewKnown = false;
        var hasStagedChanges = false;
        try
        {
            var previewResult = await workspace.PreviewAsync(cancellationToken).ConfigureAwait(false);
            warnings.AddRange(previewResult.Warnings);
            if (previewResult.Succeeded && previewResult.Value is not null && previewResult.ResultRevision == receipt.Revision)
            {
                previewKnown = true;
                var comparison = previewResult.Value.Comparisons.FirstOrDefault(candidate => candidate.FormKey == session.FormKey);
                hasStagedChanges = session.Role is FormListEditRole.New or FormListEditRole.Override || comparison?.Changes.Count > 0;
                warnings.AddRange(previewResult.Value.Warnings);
                if (comparison?.After is { } after)
                {
                    var seedResult = DraftFactory.CaptureSeed(
                        catalogContext,
                        session.WorkspaceId,
                        receipt.Revision,
                        comparison.AfterContext,
                        after,
                        ReadLimits,
                        cancellationToken);
                    if (seedResult.Succeeded)
                    {
                        seed = seedResult.Value;
                    }
                    else
                    {
                        messages.Add($"receipt-bound seed failed: {seedResult.Error?.Message ?? "no diagnostic reason"}");
                    }
                }
            }
            else
            {
                messages.Add($"preview failed: {previewResult.Error?.Message ?? "the preview did not match the Apply receipt revision"}");
            }
        }
        catch (Exception exception)
        {
            messages.Add($"preview failed: {exception.Message}");
        }

        if (seed is null)
        {
            try
            {
                var viewResult = await workspace.ReadFormListViewAsync(
                    new ReferenceRequest(session.FormKey, RecordScope.StagedOutput, descriptor.Output!.ModKey),
                    cancellationToken).ConfigureAwait(false);
                warnings.AddRange(viewResult.Warnings);
                if (viewResult.Succeeded && viewResult.Value?.Record is { } record && viewResult.ResultRevision == receipt.Revision)
                {
                    var seedResult = DraftFactory.CaptureSeed(
                        catalogContext,
                        session.WorkspaceId,
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
                    messages.Add($"staged seed failed: {viewResult.Error?.Message ?? "the staged record was not returned at the Apply receipt revision"}");
                }
            }
            catch (Exception exception)
            {
                messages.Add($"staged seed failed: {exception.Message}");
            }
        }

        return new FormListEditorApplyOutcome(
            receipt,
            seed,
            previewKnown,
            hasStagedChanges,
            warnings,
            messages.Count == 0 ? null : string.Join("; ", messages.Distinct(StringComparer.Ordinal)));
    }

    /// <summary>Publishes known Apply success before attempting browser refresh.</summary>
    /// <param name="generation">The captured workspace generation.</param>
    /// <param name="descriptor">The captured workspace descriptor.</param>
    /// <param name="session">The exact session advanced by the receipt.</param>
    /// <param name="draft">The applied typed draft.</param>
    /// <param name="outcome">The known successful Apply outcome.</param>
    /// <param name="cancellationToken">The post-mutation refresh token.</param>
    /// <returns>A task that completes after publication and browser refresh handling.</returns>
    private async Task PublishApplySuccessAsync(
        long generation,
        WorkspaceDescriptor descriptor,
        FormListEditorSession session,
        FormListDraft draft,
        FormListEditorApplyOutcome outcome,
        CancellationToken cancellationToken)
    {
        await UiDispatcher.InvokeAsync(() =>
        {
            if (!IsCurrentGeneration(generation, session.WorkspaceId) || !ReferenceEquals(SessionValue, session))
            {
                return;
            }

            PendingOperationValue = null;
            session.Advance(outcome.Receipt.Revision);
            SeedValue = outcome.Seed;
            IsStagedChangesKnownValue = outcome.PreviewKnown;
            HasStagedChangesValue = outcome.HasStagedChanges;
            SetWarnings(outcome.Warnings);
            ClearError();
            EngineError? draftRefreshError = null;
            if (outcome.Seed is not null &&
                CatalogContextValue is not null &&
                SelectedCommandValue is not null)
            {
                var refreshedDraft = DraftFactory.Create(
                    CatalogContextValue,
                    SelectedCommandValue.Key,
                    outcome.Seed,
                    DraftSelectionValue,
                    ReadLimits);
                if (refreshedDraft.Succeeded && refreshedDraft.Value is not null)
                {
                    AttachDraft(refreshedDraft.Value);
                    ValidateDraft();
                }
                else
                {
                    draftRefreshError = refreshedDraft.Error ?? new EngineError(
                        EngineErrorCode.ValidationFailed,
                        "The applied command draft could not be recreated from the receipt-bound seed.");
                }
            }

            RaiseSessionProperties();
            if (draftRefreshError is not null)
            {
                var message = outcome.PostMutationMessage is null
                    ? $"Apply succeeded; draft refresh failed: {draftRefreshError.Message}"
                    : $"Apply succeeded; {outcome.PostMutationMessage}; draft refresh failed: {draftRefreshError.Message}";
                PublishError(new EngineError(draftRefreshError.Code, message));
            }
            else if (outcome.PostMutationMessage is null)
            {
                SetStatus($"Apply succeeded for {session.FormKey} at revision {outcome.Receipt.Revision}.");
            }
            else
            {
                PublishError(new EngineError(EngineErrorCode.UnexpectedFailure, $"Apply succeeded; {outcome.PostMutationMessage}."));
            }
        }).ConfigureAwait(false);

        if (!IsCurrentGeneration(generation, session.WorkspaceId))
        {
            return;
        }

        try
        {
            await Host.RefreshAsync(session.FormKey, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            await UiDispatcher.InvokeAsync(() =>
            {
                if (IsCurrentGeneration(generation, session.WorkspaceId))
                {
                    PublishError(new EngineError(EngineErrorCode.UnexpectedFailure, $"Apply succeeded; refresh failed: {exception.Message}"));
                }
            }).ConfigureAwait(false);
        }
        finally
        {
            await UiDispatcher.InvokeAsync(() =>
            {
                if (IsCurrentGeneration(generation, session.WorkspaceId))
                {
                    StagedRecordChanged?.Invoke(session.FormKey);
                }
            }).ConfigureAwait(false);
        }
    }

    /// <summary>Publishes a definitive local, codec, or Core Apply failure while retaining the session and draft.</summary>
    /// <param name="generation">The captured workspace generation.</param>
    /// <param name="workspaceId">The captured workspace identity.</param>
    /// <param name="draft">The exact draft retained for correction.</param>
    /// <param name="error">The typed failure.</param>
    /// <returns>A task that completes after current-generation publication.</returns>
    private Task PublishApplyFailureAsync(
        long generation,
        Guid workspaceId,
        FormListDraft draft,
        EngineError? error)
    {
        return UiDispatcher.InvokeAsync(() =>
        {
            if (!IsCurrentGeneration(generation, workspaceId))
            {
                return;
            }

            PendingOperationValue = null;
            var effective = error ?? new EngineError(EngineErrorCode.UnexpectedFailure, "Apply failed without a diagnostic reason.");
            if (effective.Code == EngineErrorCode.ValidationFailed)
            {
                var issue = RecordWireDraftErrorMapper.Map(draft, effective.Message);
                ValidationIssuesValue = Array.AsReadOnly(ValidationIssuesValue.Concat(new[] { issue }).ToArray());
                OnPropertyChanged(nameof(ValidationIssues));
                OnPropertyChanged(nameof(IsValid));
            }

            PublishError(effective);
            RaiseSessionProperties();
        });
    }

    /// <summary>Freezes editor mutations around one exact immutable operation whose Core outcome is unresolved.</summary>
    /// <param name="generation">The captured workspace generation.</param>
    /// <param name="workspaceId">The captured workspace identity.</param>
    /// <param name="pendingOperation">The exact operation envelope to replay.</param>
    /// <returns>A task that completes after pending-outcome publication.</returns>
    private Task PublishUncertainOperationAsync(
        long generation,
        Guid workspaceId,
        FormListEditorPendingOperation pendingOperation)
    {
        return UiDispatcher.InvokeAsync(() =>
        {
            if (!IsCurrentGeneration(generation, workspaceId))
            {
                return;
            }

            PendingOperationValue = pendingOperation;
            SetOperationState(FormListEditorOperationState.PendingOutcome);
            PublishError(new EngineError(
                EngineErrorCode.UnexpectedFailure,
                $"Operation outcome unresolved for {pendingOperation.ActionName}. Retry the exact pending operation before making another editor change."));
            RaiseSessionProperties();
        });
    }
}
