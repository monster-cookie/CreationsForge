using CreationsForge.Core.Engine.Contracts;
using CreationsForge.RecordEditing;
using CreationsForge.RecordEditing.Drafts;
using CreationsForge.Services;

namespace CreationsForge.ViewModels;

public sealed partial class FormListEditorViewModel
{
    /// <summary>Creates and publishes one typed draft with an explicit closed seed intent.</summary>
    /// <param name="command">The exact catalog-bound command presentation.</param>
    /// <param name="selection">The fixed seed intent for this command workflow.</param>
    /// <returns>The created typed draft or a typed local failure.</returns>
    public EngineResult<FormListDraft> CreateDraft(
        FormListCommandPresentation command,
        FormListDraftSeedSelection selection)
    {
        return CreateDraft(command, selection, allowDuringBegin: false);
    }

    /// <summary>Creates a typed draft through either the public idle guard or the successful Begin publication path.</summary>
    /// <param name="command">The exact catalog-bound command presentation.</param>
    /// <param name="selection">The fixed seed intent for this command workflow.</param>
    /// <param name="allowDuringBegin">Whether the current successful Begin operation may create its initial draft before releasing the single-flight gate.</param>
    /// <returns>The created typed draft or a typed local failure.</returns>
    private EngineResult<FormListDraft> CreateDraft(
        FormListCommandPresentation command,
        FormListDraftSeedSelection selection,
        bool allowDuringBegin)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(selection);
        if (!allowDuringBegin && HasDraftChanges)
        {
            var changedDraft = Failure<FormListDraft>(
                EngineErrorCode.InvalidRequest,
                "Discard the current local form changes before opening another editor command.");
            PublishError(changedDraft.Error);
            return changedDraft;
        }

        var creationAllowed = allowDuringBegin
            ? !IsDisposed &&
                (OperationStateValue is FormListEditorOperationState.Beginning or FormListEditorOperationState.RetryingPendingOperation) &&
                !HasPendingOperation
            : CanMutateDraft;
        if (!creationAllowed || SessionValue is null || CatalogContextValue is null)
        {
            var unavailable = Failure<FormListDraft>(
                EngineErrorCode.InvalidRequest,
                "A command draft requires an active idle editor session with no unresolved operation.");
            PublishError(unavailable.Error);
            return unavailable;
        }

        var available = AvailableCommandsValue.SingleOrDefault(candidate =>
            ReferenceEquals(candidate, command) ||
            SameSchemaKey(candidate.Key, command.Key));
        if (available is null)
        {
            var foreign = Failure<FormListDraft>(
                EngineErrorCode.InvalidRequest,
                "The selected command does not belong to this session's exact catalog.");
            PublishError(foreign.Error);
            return foreign;
        }

        var policyResult = FormListCommandSeedCatalog.Resolve(CatalogContextValue, available.CommandName);
        if (!policyResult.Succeeded)
        {
            var failure = CopyFailure<FormListCommandSeedPolicy, FormListDraft>(policyResult);
            PublishError(failure.Error);
            return failure;
        }

        var result = DraftFactory.Create(
            CatalogContextValue,
            available.Key,
            IsCurrentSeed(SessionValue, SeedValue) ? SeedValue : null,
            selection,
            ReadLimits);
        if (!result.Succeeded || result.Value is null)
        {
            PublishError(result.Error);
            return result;
        }

        AttachDraft(result.Value);
        SelectedCommandValue = available;
        DraftSelectionValue = selection;
        OnPropertyChanged(nameof(SelectedCommand));
        ClearError();
        ValidateDraft();
        SetStatus($"Editing {available.DisplayName} arguments.");
        RaiseCommandStates();
        return result;
    }

    /// <summary>Discards only the changed request-local draft while retaining the plugin session, receipt-bound seed, revision, and staged work.</summary>
    private void DiscardFormChanges()
    {
        if (!CanDiscardFormChanges)
        {
            return;
        }

        DiscardFormChangesCore();
    }

    /// <summary>Discards request-local form changes while an exclusive workspace transition blocks new editor entry.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the transition, editor, session, or changed-draft preconditions are not satisfied.</exception>
    internal void DiscardRequestLocalFormChangesForWorkspaceTransition()
    {
        if (!OperationArbiter.IsWorkspaceTransitionPendingOrReserved ||
            IsDisposed ||
            IsBusy ||
            HasPendingOperation ||
            SessionValue is null ||
            !HasDraftChanges)
        {
            throw new InvalidOperationException(
                "Request-local form changes can be discarded only from an idle changed editor protected by a workspace transition.");
        }

        DiscardFormChangesCore();
    }

    /// <summary>Clears only request-local draft state after the caller proves ordinary or transition-owned discard admission.</summary>
    private void DiscardFormChangesCore()
    {
        var session = SessionValue ?? throw new InvalidOperationException("The changed draft has no active editor session.");

        SelectedCommandValue = null;
        DraftSelectionValue = FormListDraftSeedSelection.CurrentValue();
        DetachDraft();
        ClearError();
        OnPropertyChanged(nameof(SelectedCommand));
        OnPropertyChanged(nameof(CanBeginNew));
        OnPropertyChanged(nameof(CanBeginOverride));
        OnPropertyChanged(nameof(CanBeginExistingOutput));
        OnPropertyChanged(nameof(CanApply));
        OnPropertyChanged(nameof(CanDiscardFormChanges));
        SetStatus($"Discarded local form changes for {session.FormKey}. Plugin staged changes remain unchanged.");
        RaiseCommandStates();
    }

    /// <summary>Opens the bounded reference picker for one FormLink node in the active typed draft.</summary>
    /// <param name="node">The exact currently materialized FormLink node to update.</param>
    /// <param name="cancellationToken">A token that cancels the picker without changing the node.</param>
    /// <returns>A task that completes after the selected identity is published or the picker is canceled.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> is <see langword="null"/>.</exception>
    public async Task PickFormLinkAsync(
        RecordWireFormLinkDraftNode node,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(node);
        if (!CanMutateDraft ||
            SessionValue is not { } session ||
            DraftValue is not { } draft ||
            !FormListDraft.EnumerateNodes(draft.Root).Any(candidate => ReferenceEquals(candidate, node)))
        {
            return;
        }

        using var operationLease = TryEnterOperation(FormListEditorOperationState.PickingReference);
        if (operationLease is null)
        {
            return;
        }

        var generation = Volatile.Read(ref WorkspaceGeneration);
        try
        {
            using var linkedCancellation = CreateGenerationLinkedCancellation(
                cancellationToken,
                operationLease.CancellationToken);
            var request = new ReferencePickerRequest(
                session.WorkspaceId,
                session.ExpectedRevision,
                session.FormKey,
                RecordScope.WinningOverrides,
                containingModKey: null,
                allowNull: true,
                purpose: $"Select a reference for {node.DisplayName}.");
            var selection = await ReferencePickerService.PickAsync(request, linkedCancellation.Token).ConfigureAwait(false);
            if (selection is null)
            {
                return;
            }

            await UiDispatcher.InvokeAsync(() =>
            {
                if (!IsCurrentGeneration(generation, session.WorkspaceId) ||
                    !ReferenceEquals(SessionValue, session) ||
                    selection.WorkspaceId != session.WorkspaceId ||
                    selection.Revision != session.ExpectedRevision)
                {
                    return;
                }

                if (selection.IsNull)
                {
                    node.FormKey = null;
                    node.IsNull = true;
                    SetStatus($"Selected a null reference for {node.DisplayName}.");
                }
                else
                {
                    node.FormKey = selection.Match!.FormKey.ToString();
                    node.IsNull = false;
                    SetStatus($"Selected {selection.Match.FormKey} for {node.DisplayName}.");
                }
            }).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (
            cancellationToken.IsCancellationRequested ||
            operationLease.CancellationToken.IsCancellationRequested ||
            !IsCurrentGeneration(generation, session.WorkspaceId))
        {
        }
        finally
        {
            await ExitOperationAsync(generation).ConfigureAwait(false);
        }
    }

    /// <summary>Attaches one current draft and observes its aggregate change state.</summary>
    /// <param name="draft">The typed draft to publish.</param>
    private void AttachDraft(FormListDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);
        DetachDraft();
        DraftValue = draft;
        DraftValue.PropertyChanged += OnDraftPropertyChanged;
        OnPropertyChanged(nameof(Draft));
        OnPropertyChanged(nameof(DraftRoot));
        OnPropertyChanged(nameof(HasDraftChanges));
        OnPropertyChanged(nameof(CanDiscardFormChanges));
    }

    /// <summary>Stops observing and clears the current request-local draft.</summary>
    private void DetachDraft()
    {
        if (DraftValue is null)
        {
            return;
        }

        DraftValue.PropertyChanged -= OnDraftPropertyChanged;
        DraftValue = null;
        ValidationIssuesValue = Array.Empty<RecordWireDraftIssue>();
        OnPropertyChanged(nameof(Draft));
        OnPropertyChanged(nameof(DraftRoot));
        OnPropertyChanged(nameof(ValidationIssues));
        OnPropertyChanged(nameof(HasDraftChanges));
        OnPropertyChanged(nameof(IsValid));
        OnPropertyChanged(nameof(CanDiscardFormChanges));
    }

    /// <summary>Revalidates draft state after a typed node mutation.</summary>
    /// <param name="sender">The current draft.</param>
    /// <param name="eventArgs">The changed aggregate property.</param>
    private void OnDraftPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs eventArgs)
    {
        if (eventArgs.PropertyName == nameof(FormListDraft.HasChanges))
        {
            OnPropertyChanged(nameof(HasDraftChanges));
            OnPropertyChanged(nameof(CanBeginNew));
            OnPropertyChanged(nameof(CanBeginOverride));
            OnPropertyChanged(nameof(CanBeginExistingOutput));
            OnPropertyChanged(nameof(CanDiscardFormChanges));
            ValidateDraft();
        }
    }

    /// <summary>Checks whether a detached seed belongs to the active session's exact receipt revision and record.</summary>
    /// <param name="session">The active editor session.</param>
    /// <param name="seed">The candidate detached seed.</param>
    /// <returns><see langword="true"/> when every revision-bound seed identity matches the session.</returns>
    private static bool IsCurrentSeed(FormListEditorSession session, FormListDraftSeed? seed)
    {
        return seed is not null &&
            seed.WorkspaceId == session.WorkspaceId &&
            seed.Revision == session.ExpectedRevision &&
            seed.FormKey == session.FormKey &&
            SameCatalogIdentity(seed.CatalogIdentity, session.CatalogIdentity);
    }

    /// <summary>Runs bounded local validation and publishes every exact typed issue.</summary>
    /// <returns><see langword="true"/> when the current draft is valid.</returns>
    private bool ValidateDraft()
    {
        if (DraftValue is null)
        {
            ValidationIssuesValue = Array.Empty<RecordWireDraftIssue>();
            OnPropertyChanged(nameof(ValidationIssues));
            OnPropertyChanged(nameof(IsValid));
            RaiseCommandStates();
            return false;
        }

        var validation = DraftValidator.Validate(DraftValue, ReadLimits);
        ValidationIssuesValue = validation.Issues;
        OnPropertyChanged(nameof(ValidationIssues));
        OnPropertyChanged(nameof(IsValid));
        RaiseCommandStates();
        return validation.IsValid;
    }
}
