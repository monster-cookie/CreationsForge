using System.ComponentModel;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using CreationsForge.RecordEditing;
using CreationsForge.RecordEditing.Drafts;
using CreationsForge.PresentationTests.Support;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;
using CreationsForge.Skyrim.PluginAdapter.Wire;
using CreationsForge.ViewModels;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Shouldly;

namespace CreationsForge.PresentationTests.ViewModels;

/// <summary>Verifies revision forwarding, receipt-bound seeds, exact uncertain replay, and editor generation safety.</summary>
public sealed class FormListEditorViewModelTests
{
    /// <summary>Verifies Override forwards the browser revision unchanged while using the successful receipt revision for its staged seed and session.</summary>
    /// <returns>A task that completes after Begin follow-up and browser refresh.</returns>
    [Fact]
    public async Task BeginOverride_ForwardsCapturedSelectionRevisionAndCapturesReceiptSeed()
    {
        var fixture = new EditorFixture();
        var selectedRevision = new WorkspaceRevision(fixture.InitialRevision.BaselineId, fixture.InitialRevision.Sequence - 1);
        var exactSelection = new ReferenceRequest(fixture.FormKey, RecordScope.AllContexts, fixture.SourceModKey);
        fixture.Host.PublishSelection(new FormListEditorSelection(
            fixture.WorkspaceId,
            selectedRevision,
            fixture.FormKey,
            exactSelection,
            isStagedOutput: false));
        using var editor = fixture.CreateEditor();

        await editor.BeginOverrideAsync();

        var request = fixture.Workspace.BeginRequests.ShouldHaveSingleItem();
        request.ExpectedRevision.ShouldBe(selectedRevision);
        request.Role.ShouldBe(FormListEditRole.Override);
        request.OriginFormKey.ShouldBe(fixture.FormKey);
        request.OriginSelection.ShouldBeSameAs(exactSelection);
        editor.Session.ShouldNotBeNull();
        editor.Session!.ExpectedRevision.ShouldBe(fixture.Workspace.CurrentRevision);
        editor.Draft.ShouldNotBeNull();
        editor.Draft!.Seed.ShouldNotBeNull();
        editor.Draft.Seed!.Revision.ShouldBe(fixture.Workspace.CurrentRevision);
        editor.Draft.Seed.FormKey.ShouldBe(fixture.FormKey);
        fixture.Host.RefreshSelections.ShouldBe([fixture.FormKey]);
    }

    /// <summary>Verifies exact replay of an uncertain Begin creates the initial receipt-bound draft for every edit role.</summary>
    /// <param name="role">The New, Override, or ExistingOutput role under test.</param>
    /// <returns>A task that completes after the exact Begin replay succeeds.</returns>
    [Theory]
    [InlineData(FormListEditRole.New)]
    [InlineData(FormListEditRole.Override)]
    [InlineData(FormListEditRole.ExistingOutput)]
    public async Task RetryPendingBegin_KnownSuccessCreatesInitialDraft(FormListEditRole role)
    {
        var fixture = new EditorFixture();
        var selectionRevision = new WorkspaceRevision(
            fixture.InitialRevision.BaselineId,
            fixture.InitialRevision.Sequence - 1);
        if (role == FormListEditRole.Override)
        {
            fixture.Host.PublishSelection(new FormListEditorSelection(
                fixture.WorkspaceId,
                selectionRevision,
                fixture.FormKey,
                new ReferenceRequest(fixture.FormKey, RecordScope.AllContexts, fixture.SourceModKey),
                isStagedOutput: false));
        }
        else if (role == FormListEditRole.ExistingOutput)
        {
            fixture.Host.PublishSelection(new FormListEditorSelection(
                fixture.WorkspaceId,
                selectionRevision,
                fixture.FormKey,
                exactReferenceRequest: null,
                isStagedOutput: true));
        }

        var beginAttempt = 0;
        fixture.Workspace.BeginAction = (request, cancellationToken) =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            fixture.Workspace.BeginRequests.Add(request);
            if (++beginAttempt == 1)
            {
                throw new OperationCanceledException("The first Begin outcome is intentionally uncertain.");
            }

            fixture.Workspace.AdvanceRevision();
            return ValueTask.FromResult(EngineResult<EditReceipt>.Success(
                new EditReceipt(
                    Guid.NewGuid(),
                    fixture.FormKey,
                    request.OriginFormKey,
                    request.Role,
                    fixture.Workspace.CurrentRevision),
                fixture.WorkspaceId,
                request.OperationId,
                request.ExpectedRevision,
                fixture.Workspace.CurrentRevision));
        };
        using var editor = fixture.CreateEditor();

        await (role switch
        {
            FormListEditRole.New => editor.BeginNewAsync(),
            FormListEditRole.Override => editor.BeginOverrideAsync(),
            FormListEditRole.ExistingOutput => editor.BeginExistingOutputAsync(),
            _ => throw new ArgumentOutOfRangeException(nameof(role)),
        });

        editor.HasPendingOperation.ShouldBeTrue();
        editor.Session.ShouldBeNull();
        editor.Draft.ShouldBeNull();
        editor.OperationState.ShouldBe(FormListEditorOperationState.PendingOutcome);

        await editor.RetryPendingOperationAsync();

        fixture.Workspace.BeginRequests.Count.ShouldBe(2);
        var original = fixture.Workspace.BeginRequests[0];
        var replay = fixture.Workspace.BeginRequests[1];
        replay.ShouldBeSameAs(original);
        replay.OperationId.ShouldBe(original.OperationId);
        replay.ExpectedRevision.ShouldBe(role == FormListEditRole.New ? fixture.InitialRevision : selectionRevision);
        replay.Role.ShouldBe(role);
        var session = editor.Session.ShouldNotBeNull();
        session.Role.ShouldBe(role);
        session.ExpectedRevision.ShouldBe(fixture.Workspace.CurrentRevision);
        editor.Draft.ShouldNotBeNull();
        editor.Draft!.Seed.ShouldNotBeNull();
        editor.Draft.Seed!.Revision.ShouldBe(session.ExpectedRevision);
        editor.SelectedCommand.ShouldNotBeNull();
        editor.HasPendingOperation.ShouldBeFalse();
        editor.OperationState.ShouldBe(FormListEditorOperationState.Idle);
        editor.HasError.ShouldBeFalse();
        fixture.Host.RefreshSelections.ShouldBe([fixture.FormKey]);
    }

    /// <summary>Verifies known Apply success advances the session and clears replay state before failed preview, seed read, and browser refresh.</summary>
    /// <returns>A task that completes after the failed post-mutation refresh.</returns>
    [Fact]
    public async Task Apply_KnownSuccessWithPostReadFailures_AdvancesWithoutReplay()
    {
        var fixture = new EditorFixture();
        fixture.Host.ThrowOnRefreshCall = 2;
        using var editor = fixture.CreateEditor();
        await editor.BeginNewAsync();
        var draft = editor.Draft.ShouldNotBeNull();
        var expectedRevision = editor.Session!.ExpectedRevision;
        fixture.Workspace.FailPreview = true;
        fixture.Workspace.FailView = true;

        await editor.ApplyAsync();

        var request = fixture.Workspace.ApplyRequests.ShouldHaveSingleItem();
        request.ExpectedRevision.ShouldBe(expectedRevision);
        request.EditId.ShouldBe(editor.Session.EditId);
        editor.Session.ExpectedRevision.ShouldBe(fixture.Workspace.CurrentRevision);
        editor.HasPendingOperation.ShouldBeFalse();
        editor.Draft.ShouldBeSameAs(draft);
        editor.HasError.ShouldBeTrue();
        editor.ErrorMessage.ShouldNotBeNull().ShouldContain("Apply succeeded; refresh failed");
        fixture.Workspace.ApplyRequests.Count.ShouldBe(1);

        var seededCommand = editor.AvailableCommands.Single(command => command.CommandName == "form-list.replace-items");
        var staleSeedAttempt = editor.CreateDraft(seededCommand, FormListDraftSeedSelection.CurrentValue());
        staleSeedAttempt.Succeeded.ShouldBeFalse();
        staleSeedAttempt.Error!.Message.ShouldContain("requires a matching revision-bound record seed");
        editor.Draft.ShouldBeSameAs(draft);
    }

    /// <summary>Verifies a locally changed typed draft blocks every Begin action so it cannot be silently discarded.</summary>
    /// <returns>A task that completes after guarded Begin evaluation.</returns>
    [Fact]
    public async Task ChangedDraft_BlocksEveryBeginAction()
    {
        var fixture = new EditorFixture();
        using var editor = fixture.CreateEditor();
        await editor.BeginNewAsync();
        var command = editor.AvailableCommands.Single(candidate => candidate.CommandName == "form-list.set-editor-id");
        var draftResult = editor.CreateDraft(command, FormListDraftSeedSelection.CurrentValue());
        draftResult.Succeeded.ShouldBeTrue();
        var root = draftResult.Value!.Root.ShouldBeOfType<RecordWireObjectDraftNode>();
        var editorId = root.FindProperty("editorId").ShouldBeOfType<RecordWireStringDraftNode>();

        editorId.Value = "ChangedLocally";

        editor.HasDraftChanges.ShouldBeTrue();
        editor.CanBeginNew.ShouldBeFalse();
        editor.CanBeginOverride.ShouldBeFalse();
        editor.CanBeginExistingOutput.ShouldBeFalse();
        editor.NewCommand.CanExecute(null).ShouldBeFalse();
        await editor.BeginNewAsync();
        fixture.Workspace.BeginRequests.Count.ShouldBe(1);
    }

    /// <summary>Verifies same-command and different-command draft replacement require explicit local discard without changing plugin session or staged state.</summary>
    /// <returns>A task that completes after explicit local discard and new draft creation.</returns>
    [Fact]
    public async Task CreateDraft_ChangedDraftRequiresExplicitDiscardBeforeReplacement()
    {
        var fixture = new EditorFixture();
        using var editor = fixture.CreateEditor();
        await editor.BeginNewAsync();
        var session = editor.Session.ShouldNotBeNull();
        var revision = session.ExpectedRevision;
        var stagedKnown = editor.IsStagedChangesKnown;
        var hasStagedChanges = editor.HasStagedChanges;
        var firstCommand = editor.AvailableCommands.Single(candidate => candidate.CommandName == "form-list.set-editor-id");
        var differentCommand = editor.AvailableCommands.Single(candidate => candidate.CommandName == "form-list.clear-items");
        var firstResult = editor.CreateDraft(firstCommand, FormListDraftSeedSelection.CurrentValue());
        var firstDraft = firstResult.Value.ShouldNotBeNull();
        var receiptSeed = firstDraft.Seed.ShouldNotBeNull();
        var root = firstDraft.Root.ShouldBeOfType<RecordWireObjectDraftNode>();
        var editorId = root.FindProperty("editorId").ShouldBeOfType<RecordWireStringDraftNode>();
        editorId.Value = "PreserveThisValue";

        var sameCommandAttempt = editor.CreateDraft(firstCommand, FormListDraftSeedSelection.CurrentValue());
        AssertChangedDraftReplacementRejected(editor, sameCommandAttempt, firstDraft, firstCommand, editorId, session, revision);

        var differentCommandAttempt = editor.CreateDraft(differentCommand, FormListDraftSeedSelection.CurrentValue());
        AssertChangedDraftReplacementRejected(editor, differentCommandAttempt, firstDraft, firstCommand, editorId, session, revision);

        editor.CanDiscardFormChanges.ShouldBeTrue();
        editor.DiscardFormChangesCommand.CanExecute(null).ShouldBeTrue();
        editor.DiscardFormChangesCommand.Execute(null);

        editor.Draft.ShouldBeNull();
        editor.SelectedCommand.ShouldBeNull();
        editor.HasDraftChanges.ShouldBeFalse();
        editor.CanDiscardFormChanges.ShouldBeFalse();
        editor.Session.ShouldBeSameAs(session);
        editor.Session.ExpectedRevision.ShouldBe(revision);
        editor.IsStagedChangesKnown.ShouldBe(stagedKnown);
        editor.HasStagedChanges.ShouldBe(hasStagedChanges);
        editor.HasError.ShouldBeFalse();
        editor.StatusText.ShouldContain("Plugin staged changes remain unchanged");

        var nextResult = editor.CreateDraft(differentCommand, FormListDraftSeedSelection.CurrentValue());
        nextResult.Succeeded.ShouldBeTrue();
        nextResult.Value.ShouldNotBeNull().ShouldNotBeSameAs(firstDraft);
        nextResult.Value!.Seed.ShouldBeSameAs(receiptSeed);
        editor.Session.ShouldBeSameAs(session);
        editor.Session.ExpectedRevision.ShouldBe(revision);
        editor.IsStagedChangesKnown.ShouldBe(stagedKnown);
        editor.HasStagedChanges.ShouldBe(hasStagedChanges);
    }

    /// <summary>Verifies a running Apply disables local discard and command execution cannot clear the in-flight draft.</summary>
    /// <returns>A task that completes after the held Apply is released.</returns>
    [Fact]
    public async Task DiscardFormChanges_RunningApplyIsDisabledAndPreservesDraft()
    {
        var fixture = new EditorFixture();
        var applyStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var releaseApply = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        fixture.Workspace.ApplyAction = async (request, cancellationToken) =>
        {
            fixture.Workspace.ApplyRequests.Add(request);
            applyStarted.TrySetResult(true);
            await releaseApply.Task.WaitAsync(cancellationToken);
            fixture.Workspace.AdvanceRevision();
            return EngineResult<OperationReceipt>.Success(
                new OperationReceipt(request.OperationId, fixture.Workspace.CurrentRevision),
                fixture.WorkspaceId,
                request.OperationId,
                request.ExpectedRevision,
                fixture.Workspace.CurrentRevision);
        };
        using var editor = fixture.CreateEditor();
        await editor.BeginNewAsync();
        var command = editor.AvailableCommands.Single(candidate => candidate.CommandName == "form-list.set-editor-id");
        var draft = editor.CreateDraft(command, FormListDraftSeedSelection.CurrentValue()).Value.ShouldNotBeNull();
        var root = draft.Root.ShouldBeOfType<RecordWireObjectDraftNode>();
        root.FindProperty("editorId").ShouldBeOfType<RecordWireStringDraftNode>().Value = "BusyValue";
        var session = editor.Session.ShouldNotBeNull();
        var revision = session.ExpectedRevision;

        var applyTask = editor.ApplyAsync();
        await applyStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));
        try
        {
            editor.IsBusy.ShouldBeTrue();
            editor.CanDiscardFormChanges.ShouldBeFalse();
            editor.DiscardFormChangesCommand.CanExecute(null).ShouldBeFalse();
            editor.DiscardFormChangesCommand.Execute(null);
            editor.Draft.ShouldBeSameAs(draft);
            editor.SelectedCommand.ShouldBeSameAs(command);
            editor.Session.ShouldBeSameAs(session);
            editor.Session.ExpectedRevision.ShouldBe(revision);
        }
        finally
        {
            releaseApply.TrySetResult(true);
        }

        await applyTask;
    }

    /// <summary>Verifies a waiting workspace transition closes every editor entry and acquires only after known Apply publication.</summary>
    /// <returns>A task that completes after Apply and transition drain.</returns>
    [Fact]
    public async Task WorkspaceTransition_WaitsForKnownApplyPublicationAndCoordinatorExit()
    {
        var fixture = new EditorFixture();
        var applyStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var releaseApply = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        CancellationToken applyToken = default;
        fixture.Workspace.ApplyAction = async (request, cancellationToken) =>
        {
            applyToken = cancellationToken;
            fixture.Workspace.ApplyRequests.Add(request);
            applyStarted.TrySetResult(true);
            await releaseApply.Task.WaitAsync(cancellationToken);
            fixture.Workspace.AdvanceRevision();
            return EngineResult<OperationReceipt>.Success(
                new OperationReceipt(request.OperationId, fixture.Workspace.CurrentRevision),
                fixture.WorkspaceId,
                request.OperationId,
                request.ExpectedRevision,
                fixture.Workspace.CurrentRevision);
        };
        using var editor = fixture.CreateEditor();
        await editor.BeginNewAsync();
        var command = editor.AvailableCommands.Single(candidate => candidate.CommandName == "form-list.set-editor-id");
        var draft = editor.CreateDraft(command, FormListDraftSeedSelection.CurrentValue()).Value.ShouldNotBeNull();
        draft.Root.ShouldBeOfType<RecordWireObjectDraftNode>()
            .FindProperty("editorId").ShouldBeOfType<RecordWireStringDraftNode>().Value = "DrainKnownApply";
        var applyTask = editor.ApplyAsync();
        await applyStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));

        var transitionTask = fixture.OperationArbiter.ReserveWorkspaceTransitionAsync(
            WorkspaceTransitionDrainMode.WaitForCurrentOperation).AsTask();

        fixture.OperationArbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeTrue();
        applyToken.IsCancellationRequested.ShouldBeFalse();
        transitionTask.IsCompleted.ShouldBeFalse();
        editor.CanBeginNew.ShouldBeFalse();
        editor.CanApply.ShouldBeFalse();
        editor.CanMutateDraft.ShouldBeFalse();
        editor.CanDiscardFormChanges.ShouldBeFalse();
        releaseApply.TrySetResult(true);
        await applyTask;
        using var transitionLease = await transitionTask;

        editor.OperationState.ShouldBe(FormListEditorOperationState.Idle);
        editor.HasPendingOperation.ShouldBeFalse();
        editor.Session.ShouldNotBeNull().ExpectedRevision.ShouldBe(fixture.Workspace.CurrentRevision);
        fixture.Workspace.ApplyRequests.ShouldHaveSingleItem();
        transitionLease.IsActive.ShouldBeTrue();
    }

    /// <summary>Verifies cancel-and-wait retains an uncertain Apply envelope before transition acquisition.</summary>
    /// <returns>A task that completes after cancellation, pending-envelope publication, and transition drain.</returns>
    [Fact]
    public async Task WorkspaceTransition_CancelAndWaitObservesUncertainApplyBeforeAcquisition()
    {
        var fixture = new EditorFixture();
        var applyStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        fixture.Workspace.ApplyAction = async (request, cancellationToken) =>
        {
            fixture.Workspace.ApplyRequests.Add(request);
            applyStarted.TrySetResult(true);
            await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
            throw new InvalidOperationException("The canceled Apply unexpectedly resumed.");
        };
        using var editor = fixture.CreateEditor();
        await editor.BeginNewAsync();
        var command = editor.AvailableCommands.Single(candidate => candidate.CommandName == "form-list.set-editor-id");
        var draft = editor.CreateDraft(command, FormListDraftSeedSelection.CurrentValue()).Value.ShouldNotBeNull();
        draft.Root.ShouldBeOfType<RecordWireObjectDraftNode>()
            .FindProperty("editorId").ShouldBeOfType<RecordWireStringDraftNode>().Value = "DrainUncertainApply";
        var applyTask = editor.ApplyAsync();
        await applyStarted.Task.WaitAsync(TimeSpan.FromSeconds(5));

        var transitionTask = fixture.OperationArbiter.ReserveWorkspaceTransitionAsync(
            WorkspaceTransitionDrainMode.CancelAndWaitForCurrentOperation).AsTask();
        await applyTask;
        using var transitionLease = await transitionTask;

        editor.OperationState.ShouldBe(FormListEditorOperationState.PendingOutcome);
        editor.HasPendingOperation.ShouldBeTrue();
        editor.Draft.ShouldBeSameAs(draft);
        editor.CanMutateDraft.ShouldBeFalse();
        fixture.Workspace.ApplyRequests.ShouldHaveSingleItem();
    }

    /// <summary>Verifies an uncertain Apply freezes mutation and exact retry reuses operation, revision, edit identity, and command type.</summary>
    /// <returns>A task that completes after exact replay succeeds.</returns>
    [Fact]
    public async Task RetryPendingApply_ReusesExactImmutableOperation()
    {
        var fixture = new EditorFixture();
        var applyAttempt = 0;
        fixture.Workspace.ApplyAction = (request, _) =>
        {
            fixture.Workspace.ApplyRequests.Add(request);
            if (++applyAttempt == 1)
            {
                throw new OperationCanceledException("The first Apply outcome is intentionally uncertain.");
            }

            fixture.Workspace.AdvanceRevision();
            return ValueTask.FromResult(EngineResult<OperationReceipt>.Success(
                new OperationReceipt(request.OperationId, fixture.Workspace.CurrentRevision),
                fixture.WorkspaceId,
                request.OperationId,
                request.ExpectedRevision,
                fixture.Workspace.CurrentRevision));
        };
        using var editor = fixture.CreateEditor();
        await editor.BeginNewAsync();
        var command = editor.AvailableCommands.Single(candidate => candidate.CommandName == "form-list.set-editor-id");
        var pendingDraft = editor.CreateDraft(command, FormListDraftSeedSelection.CurrentValue()).Value.ShouldNotBeNull();
        var pendingRoot = pendingDraft.Root.ShouldBeOfType<RecordWireObjectDraftNode>();
        pendingRoot.FindProperty("editorId").ShouldBeOfType<RecordWireStringDraftNode>().Value = "PendingValue";
        var expectedRevision = editor.Session!.ExpectedRevision;

        await editor.ApplyAsync();

        editor.HasPendingOperation.ShouldBeTrue();
        editor.CanApply.ShouldBeFalse();
        editor.CanMutateDraft.ShouldBeFalse();
        editor.CanDiscardFormChanges.ShouldBeFalse();
        editor.DiscardFormChangesCommand.CanExecute(null).ShouldBeFalse();
        editor.DiscardFormChangesCommand.Execute(null);
        editor.Draft.ShouldBeSameAs(pendingDraft);
        editor.SelectedCommand.ShouldBeSameAs(command);
        editor.HasPendingOperation.ShouldBeTrue();
        editor.OperationState.ShouldBe(FormListEditorOperationState.PendingOutcome);

        await editor.RetryPendingOperationAsync();

        fixture.Workspace.ApplyRequests.Count.ShouldBe(2);
        var original = fixture.Workspace.ApplyRequests[0];
        var replay = fixture.Workspace.ApplyRequests[1];
        replay.OperationId.ShouldBe(original.OperationId);
        replay.ExpectedRevision.ShouldBe(expectedRevision);
        replay.EditId.ShouldBe(original.EditId);
        replay.Edit.GetType().ShouldBe(original.Edit.GetType());
        editor.HasPendingOperation.ShouldBeFalse();
        editor.Session.ExpectedRevision.ShouldBe(fixture.Workspace.CurrentRevision);
    }

    /// <summary>Asserts that a changed draft replacement attempt fails without changing any local typed or session identity.</summary>
    /// <param name="editor">The editor under test.</param>
    /// <param name="attempt">The rejected draft creation result.</param>
    /// <param name="expectedDraft">The exact changed draft that must remain active.</param>
    /// <param name="expectedCommand">The exact selected command that must remain active.</param>
    /// <param name="editorId">The changed value node that must retain its input.</param>
    /// <param name="session">The exact edit session that must remain active.</param>
    /// <param name="revision">The unchanged expected revision.</param>
    private static void AssertChangedDraftReplacementRejected(
        FormListEditorViewModel editor,
        EngineResult<FormListDraft> attempt,
        FormListDraft expectedDraft,
        FormListCommandPresentation expectedCommand,
        RecordWireStringDraftNode editorId,
        FormListEditorSession session,
        WorkspaceRevision revision)
    {
        attempt.Succeeded.ShouldBeFalse();
        attempt.Error.ShouldNotBeNull().Code.ShouldBe(EngineErrorCode.InvalidRequest);
        attempt.Error!.Message.ShouldContain("Discard the current local form changes");
        editor.ErrorMessage.ShouldNotBeNull().ShouldContain("Discard the current local form changes");
        editor.Draft.ShouldBeSameAs(expectedDraft);
        editor.SelectedCommand.ShouldBeSameAs(expectedCommand);
        editorId.Value.ShouldBe("PreserveThisValue");
        editor.Session.ShouldBeSameAs(session);
        editor.Session.ShouldNotBeNull().ExpectedRevision.ShouldBe(revision);
    }

    /// <summary>Verifies a definitive Core rejection clears retry state while retaining the exact session and typed draft for correction.</summary>
    /// <returns>A task that completes after Core rejects Apply.</returns>
    [Fact]
    public async Task Apply_DefinitiveRejection_ClearsPendingAndRetainsDraft()
    {
        var fixture = new EditorFixture();
        using var editor = fixture.CreateEditor();
        await editor.BeginNewAsync();
        var draft = editor.Draft.ShouldNotBeNull();
        var revision = editor.Session!.ExpectedRevision;
        fixture.Workspace.ApplyAction = (request, _) =>
        {
            fixture.Workspace.ApplyRequests.Add(request);
            return ValueTask.FromResult(EngineResult<OperationReceipt>.Failure(
                new EngineError(EngineErrorCode.RevisionConflict, "The captured session revision is stale."),
                fixture.WorkspaceId,
                request.OperationId,
                request.ExpectedRevision,
                fixture.Workspace.CurrentRevision));
        };

        await editor.ApplyAsync();

        editor.HasPendingOperation.ShouldBeFalse();
        editor.Draft.ShouldBeSameAs(draft);
        editor.Session.ShouldNotBeNull();
        editor.Session!.ExpectedRevision.ShouldBe(revision);
        editor.ErrorCode.ShouldBe(EngineErrorCode.RevisionConflict);
        editor.ErrorMessage.ShouldBe("The captured session revision is stale.");
    }

    /// <summary>Verifies workspace replacement cancels the editor generation and discards its session, draft, and uncertain replay envelope.</summary>
    /// <returns>A task that completes after replacement publication.</returns>
    [Fact]
    public async Task WorkspaceReplacement_ClearsSessionDraftAndPendingOperation()
    {
        var fixture = new EditorFixture();
        fixture.Workspace.ApplyAction = (request, _) =>
        {
            fixture.Workspace.ApplyRequests.Add(request);
            throw new OperationCanceledException("The Apply outcome is intentionally uncertain.");
        };
        using var editor = fixture.CreateEditor();
        await editor.BeginNewAsync();
        await editor.ApplyAsync();
        editor.HasPendingOperation.ShouldBeTrue();

        var replacement = new EditorTestWorkspace(Guid.NewGuid(), fixture.Output, new WorkspaceRevision(Guid.NewGuid(), 1), fixture.FormKey, fixture.SourceModKey);
        fixture.Coordinator.Publish(
            fixture.CreateDescriptor(replacement.WorkspaceId, replacement.CurrentRevision),
            replacement);

        editor.IsSessionActive.ShouldBeFalse();
        editor.Draft.ShouldBeNull();
        editor.HasPendingOperation.ShouldBeFalse();
        editor.IsStagedChangesKnown.ShouldBeFalse();
        editor.StatusText.ShouldContain("Workspace changed");
    }

    /// <summary>Owns deterministic editor dependencies for one lifecycle test.</summary>
    private sealed class EditorFixture
    {
        /// <summary>Initializes one Skyrim editor fixture with a selected ready output.</summary>
        public EditorFixture()
        {
            WorkspaceId = Guid.NewGuid();
            InitialRevision = new WorkspaceRevision(Guid.NewGuid(), 8);
            SourceModKey = ModKey.FromNameAndExtension("EditorSource.esm");
            FormKey = new FormKey(SourceModKey, 0x0123);
            var outputPath = AbsolutePath("EditorOutput.esp");
            Output = new OutputAssociation(
                outputPath,
                ModKey.FromNameAndExtension("EditorOutput.esp"),
                LocalizedOutputMode.Embedded,
                OutputMasterStyle.Full);
            Workspace = new EditorTestWorkspace(WorkspaceId, Output, InitialRevision, FormKey, SourceModKey);
            Coordinator = new RecordingWorkspaceCoordinator();
            Coordinator.Publish(CreateDescriptor(WorkspaceId, InitialRevision), Workspace);
            Host = new EditorTestHost();
            OperationArbiter = new WorkspacePresentationOperationArbiter();
        }

        /// <summary>Gets the test workspace identity.</summary>
        public Guid WorkspaceId { get; }
        /// <summary>Gets the first live workspace revision.</summary>
        public WorkspaceRevision InitialRevision { get; }
        /// <summary>Gets the source plugin identity.</summary>
        public ModKey SourceModKey { get; }
        /// <summary>Gets the edited FormList identity.</summary>
        public FormKey FormKey { get; }
        /// <summary>Gets the selected output identity.</summary>
        public OutputAssociation Output { get; }
        /// <summary>Gets the configurable borrowed workspace.</summary>
        public EditorTestWorkspace Workspace { get; }
        /// <summary>Gets the recording coordinator.</summary>
        public RecordingWorkspaceCoordinator Coordinator { get; }
        /// <summary>Gets the recording editor host.</summary>
        public EditorTestHost Host { get; }
        /// <summary>Gets the shared editor and workspace-transition admission boundary.</summary>
        public WorkspacePresentationOperationArbiter OperationArbiter { get; }

        /// <summary>Creates the production editor with exact Skyrim wire services and deterministic presentation adapters.</summary>
        /// <returns>The configured editor.</returns>
        public FormListEditorViewModel CreateEditor()
        {
            var validator = new FormListDraftValidator();
            return new FormListEditorViewModel(
                Coordinator,
                Host,
                OperationArbiter,
                new FormListWireCatalogResolver(
                    [new SkyrimFormListEditWireCodec()],
                    [new SkyrimFormListEditWireSchemaCatalog()]),
                new FormListDraftFactory(),
                validator,
                new FormListDraftSerializer(validator),
                new RecordingReferencePickerService(),
                new InlineUiDispatcher());
        }

        /// <summary>Creates a current plugin desktop descriptor for a fixture workspace.</summary>
        /// <param name="workspaceId">The workspace identity.</param>
        /// <param name="revision">The current revision.</param>
        /// <returns>The immutable descriptor.</returns>
        public WorkspaceDescriptor CreateDescriptor(Guid workspaceId, WorkspaceRevision revision)
        {
            var sourcePath = AbsolutePath(SourceModKey.FileName);
            return new WorkspaceDescriptor(
                workspaceId,
                SupportedGame.Skyrim,
                GameRelease.SkyrimSE,
                sourcePath,
                [sourcePath],
                Output,
                revision);
        }
    }

    /// <summary>Publishes an exact editor selection and records post-mutation browser refresh requests.</summary>
    private sealed class EditorTestHost : IFormListEditorHost
    {
        /// <inheritdoc />
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <inheritdoc />
        public FormListEditorSelection? Selection { get; private set; }

        /// <summary>Gets the requested FormList reselections in call order.</summary>
        public List<FormKey?> RefreshSelections { get; } = [];

        /// <summary>Gets or sets the one-based refresh call that throws a deterministic failure.</summary>
        public int? ThrowOnRefreshCall { get; set; }

        /// <summary>Publishes one atomic browser selection.</summary>
        /// <param name="selection">The exact selection, or <see langword="null"/>.</param>
        public void PublishSelection(FormListEditorSelection? selection)
        {
            Selection = selection;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Selection)));
        }

        /// <inheritdoc />
        public Task RefreshAsync(FormKey? reselect = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            RefreshSelections.Add(reselect);
            if (ThrowOnRefreshCall == RefreshSelections.Count)
            {
                throw new InvalidOperationException("The browser refresh failed after mutation success.");
            }

            return Task.CompletedTask;
        }
    }

    /// <summary>Implements only the workspace operations exercised by the editor lifecycle.</summary>
    private sealed class EditorTestWorkspace : IPluginWorkspace
    {
        /// <summary>The selected output identity.</summary>
        private readonly OutputAssociation Output;

        /// <summary>The fixed edited FormList identity.</summary>
        private readonly FormKey FormKey;

        /// <summary>The fixed source plugin identity.</summary>
        private readonly ModKey SourceModKey;

        /// <summary>The complete selected output baseline.</summary>
        private readonly OutputArtifactSetBaseline OutputBaseline;

        /// <summary>Initializes one deterministic ready workspace.</summary>
        /// <param name="workspaceId">The workspace identity.</param>
        /// <param name="output">The selected output.</param>
        /// <param name="revision">The initial revision.</param>
        /// <param name="formKey">The edited FormList identity.</param>
        /// <param name="sourceModKey">The source plugin identity.</param>
        public EditorTestWorkspace(
            Guid workspaceId,
            OutputAssociation output,
            WorkspaceRevision revision,
            FormKey formKey,
            ModKey sourceModKey)
        {
            WorkspaceId = workspaceId;
            Output = output;
            CurrentRevision = revision;
            FormKey = formKey;
            SourceModKey = sourceModKey;
            OutputBaseline = new OutputArtifactSetBaseline(
                revision.BaselineId,
                [new PluginArtifactAssociation(
                    output.PluginPath,
                    PluginArtifactRole.Plugin,
                    language: null,
                    new PluginArtifactFingerprint(false, 0, null))]);
            BeginAction = BeginSuccessfullyAsync;
            ApplyAction = ApplySuccessfullyAsync;
        }

        /// <inheritdoc />
        public Guid WorkspaceId { get; }

        /// <summary>Gets the mutable current revision used by deterministic receipts.</summary>
        public WorkspaceRevision CurrentRevision { get; private set; }

        /// <inheritdoc />
        public WorkspaceRevision Revision => CurrentRevision;

        /// <inheritdoc />
        public OutputSynchronizationState OutputSynchronization { get; } = new(OutputSynchronizationStatus.Ready, null);

        /// <summary>Gets exact Begin requests in call order.</summary>
        public List<BeginEditRequest> BeginRequests { get; } = [];

        /// <summary>Gets exact Apply requests in call order.</summary>
        public List<FormListEditRequest> ApplyRequests { get; } = [];

        /// <summary>Gets or sets the deterministic Begin behavior.</summary>
        public Func<BeginEditRequest, CancellationToken, ValueTask<EngineResult<EditReceipt>>> BeginAction { get; set; }

        /// <summary>Gets or sets the deterministic Apply behavior.</summary>
        public Func<FormListEditRequest, CancellationToken, ValueTask<EngineResult<OperationReceipt>>> ApplyAction { get; set; }

        /// <summary>Gets or sets whether preview returns a typed post-mutation failure.</summary>
        public bool FailPreview { get; set; }

        /// <summary>Gets or sets whether staged record reads return a typed post-mutation failure.</summary>
        public bool FailView { get; set; }

        /// <summary>Advances the deterministic workspace revision by one.</summary>
        public void AdvanceRevision()
        {
            CurrentRevision = CurrentRevision.Next();
        }

        /// <inheritdoc />
        public ValueTask<EngineResult<WorkspaceState>> ReadStateAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return ValueTask.FromResult(EngineResult<WorkspaceState>.Success(
                new WorkspaceState(
                    SupportedGame.Skyrim,
                    GameRelease.SkyrimSE,
                    Output,
                    OutputBaseline,
                    OutputSynchronization,
                    CurrentRevision),
                WorkspaceId,
                resultRevision: CurrentRevision));
        }

        /// <inheritdoc />
        public ValueTask<EngineResult<EditReceipt>> BeginEditAsync(BeginEditRequest request, CancellationToken cancellationToken = default)
        {
            return BeginAction(request, cancellationToken);
        }

        /// <inheritdoc />
        public ValueTask<EngineResult<OperationReceipt>> ApplyFormListEditAsync(FormListEditRequest request, CancellationToken cancellationToken = default)
        {
            return ApplyAction(request, cancellationToken);
        }

        /// <inheritdoc />
        public ValueTask<EngineResult<FormListReadView>> ReadFormListViewAsync(ReferenceRequest request, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (FailView)
            {
                return ValueTask.FromResult(EngineResult<FormListReadView>.Failure(
                    new EngineError(EngineErrorCode.UnexpectedFailure, "The staged record read failed."),
                    WorkspaceId,
                    resultRevision: CurrentRevision));
            }

            var context = new FormListContext(
                request,
                ReferenceResolutionStatus.Resolved,
                Output.ModKey,
                Output.PluginPath,
                1,
                PluginRole.Output);
            return ValueTask.FromResult(EngineResult<FormListReadView>.Success(
                new FormListReadView(context, RecordJson()),
                WorkspaceId,
                resultRevision: CurrentRevision));
        }

        /// <inheritdoc />
        public ValueTask<EngineResult<WorkspacePreview>> PreviewAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (FailPreview)
            {
                return ValueTask.FromResult(EngineResult<WorkspacePreview>.Failure(
                    new EngineError(EngineErrorCode.UnexpectedFailure, "The plugin preview failed."),
                    WorkspaceId,
                    resultRevision: CurrentRevision));
            }

            var beforeRequest = new ReferenceRequest(FormKey, RecordScope.AllContexts, SourceModKey);
            var afterRequest = new ReferenceRequest(FormKey, RecordScope.StagedOutput, Output.ModKey);
            var beforeContext = new FormListContext(
                beforeRequest,
                ReferenceResolutionStatus.Resolved,
                SourceModKey,
                AbsolutePath(SourceModKey.FileName),
                0,
                PluginRole.Source);
            var afterContext = new FormListContext(
                afterRequest,
                ReferenceResolutionStatus.Resolved,
                Output.ModKey,
                Output.PluginPath,
                1,
                PluginRole.Output);
            var comparison = new FormListComparison(
                beforeContext,
                afterContext,
                RecordJson(),
                RecordJson(),
                [new SemanticChangeDescriptor("EditorID", SemanticChangeKind.ValueChanged)],
                []);
            return ValueTask.FromResult(EngineResult<WorkspacePreview>.Success(
                new WorkspacePreview([comparison], 0, []),
                WorkspaceId,
                resultRevision: CurrentRevision));
        }

        /// <summary>Begins one deterministic edit successfully and advances revision.</summary>
        /// <param name="request">The exact Begin request.</param>
        /// <param name="cancellationToken">The operation token.</param>
        /// <returns>The successful edit receipt.</returns>
        private ValueTask<EngineResult<EditReceipt>> BeginSuccessfullyAsync(BeginEditRequest request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            BeginRequests.Add(request);
            AdvanceRevision();
            return ValueTask.FromResult(EngineResult<EditReceipt>.Success(
                new EditReceipt(
                    Guid.NewGuid(),
                    FormKey,
                    request.OriginFormKey,
                    request.Role,
                    CurrentRevision),
                WorkspaceId,
                request.OperationId,
                request.ExpectedRevision,
                CurrentRevision));
        }

        /// <summary>Applies one deterministic command successfully and advances revision.</summary>
        /// <param name="request">The exact Apply request.</param>
        /// <param name="cancellationToken">The operation token.</param>
        /// <returns>The successful operation receipt.</returns>
        private ValueTask<EngineResult<OperationReceipt>> ApplySuccessfullyAsync(FormListEditRequest request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ApplyRequests.Add(request);
            AdvanceRevision();
            return ValueTask.FromResult(EngineResult<OperationReceipt>.Success(
                new OperationReceipt(request.OperationId, CurrentRevision),
                WorkspaceId,
                request.OperationId,
                request.ExpectedRevision,
                CurrentRevision));
        }

        /// <summary>Creates the minimal detached expanded record consumed by presentation seed capture.</summary>
        /// <returns>A cloned JSON object.</returns>
        private JsonElement RecordJson()
        {
            using var document = JsonDocument.Parse($"{{\"FormKey\":\"{FormKey}\",\"EditorID\":\"Before\"}}");
            return document.RootElement.Clone();
        }

        /// <inheritdoc />
        public ValueTask<EngineResult<OutputSelectionReceipt>> SelectOutputAsync(SelectOutputRequest request, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        /// <inheritdoc />
        public ValueTask<EngineResult<IReadOnlyList<PluginSummary>>> ListPluginsAsync(CancellationToken cancellationToken = default) => throw new NotSupportedException();
        /// <inheritdoc />
        public ValueTask<EngineResult<IReadOnlyList<FormListSummary>>> ListFormListsAsync(RecordScope scope, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        /// <inheritdoc />
        public ValueTask<EngineResult<IMajorRecordGetter>> ReadFormListAsync(FormKey formKey, RecordScope scope, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        /// <inheritdoc />
        public ValueTask<EngineResult<ReferenceSearchPage>> SearchReferencesAsync(ReferenceSearchRequest request, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        /// <inheritdoc />
        public ValueTask<EngineResult<ReferenceResolution>> ResolveReferenceAsync(ReferenceRequest request, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        /// <inheritdoc />
        public ValueTask<EngineResult<FormListComparison>> CompareFormListAsync(CompareFormListRequest request, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        /// <inheritdoc />
        public ValueTask<SaveResult> SaveAsync(SaveRequest request, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        /// <inheritdoc />
        public ValueTask<EngineResult<OutputSelectionReceipt>> ResolveOutputRecoveryAsync(ResolveOutputRecoveryRequest request, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        /// <inheritdoc />
        public ValueTask<EngineResult<OutputSelectionReceipt>> ReopenOutputAsync(ReopenOutputRequest request, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        /// <inheritdoc />
        public ValueTask<EngineResult<OperationReceipt>> DiscardChangesAsync(DiscardChangesRequest request, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        /// <inheritdoc />
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }

    /// <summary>Creates a fully qualified task-owned test plugin path.</summary>
    /// <param name="fileName">The plugin file name.</param>
    /// <returns>The fully qualified path.</returns>
    private static string AbsolutePath(string fileName)
    {
        return Path.GetFullPath(Path.Combine(Path.GetTempPath(), "CreationsForge-EditorTests", fileName));
    }
}
