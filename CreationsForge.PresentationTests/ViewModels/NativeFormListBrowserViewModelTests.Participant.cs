using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using CreationsForge.NativeEditing.Drafts;
using CreationsForge.PresentationTests.Support;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;
using CreationsForge.Starfield.Native.Wire;
using CreationsForge.ViewModels;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Shouldly;

namespace CreationsForge.PresentationTests.ViewModels;

public sealed partial class NativeFormListBrowserViewModelTests
{
    /// <summary>Verifies the participant forwards draft state and discards only request-local input under transition admission.</summary>
    /// <returns>A task that completes after the local draft is discarded.</returns>
    [Fact]
    public async Task EditParticipant_DiscardLocalDraft_RetainsSessionRevisionAndStagedState()
    {
        var context = await CreateParticipantContextAsync();
        using var viewModel = context.ViewModel;
        var editor = viewModel.Editor;
        var session = editor.Session.ShouldNotBeNull();
        var sessionRevision = session.ExpectedRevision;
        var stagedKnown = editor.IsStagedChangesKnown;
        var hasStagedChanges = editor.HasStagedChanges;
        var changedProperties = new List<string?>();
        viewModel.PropertyChanged += (_, eventArgs) => changedProperties.Add(eventArgs.PropertyName);
        var command = editor.AvailableCommands.Single(candidate => candidate.CommandName == "form-list.set-editor-id");
        var draft = editor.CreateDraft(command, NativeFormListDraftSeedSelection.CurrentValue()).Value.ShouldNotBeNull();

        draft.Root.ShouldBeOfType<NativeWireObjectDraftNode>()
            .FindProperty("editorId").ShouldBeOfType<NativeWireStringDraftNode>().Value = "ParticipantDraft";

        viewModel.HasDraftChanges.ShouldBeTrue();
        changedProperties.ShouldContain(nameof(INativeWorkspaceEditParticipant.HasDraftChanges));
        using var transitionLease = await context.OperationArbiter.ReserveWorkspaceTransitionAsync(
            NativeWorkspaceTransitionDrainMode.WaitForCurrentOperation);

        viewModel.DiscardRequestLocalFormChanges();

        viewModel.HasDraftChanges.ShouldBeFalse();
        editor.Draft.ShouldBeNull();
        editor.SelectedCommand.ShouldBeNull();
        editor.Session.ShouldBeSameAs(session);
        editor.Session.ShouldNotBeNull().ExpectedRevision.ShouldBe(sessionRevision);
        editor.IsStagedChangesKnown.ShouldBe(stagedKnown);
        editor.HasStagedChanges.ShouldBe(hasStagedChanges);
        changedProperties.Count(property => property == nameof(INativeWorkspaceEditParticipant.HasDraftChanges)).ShouldBeGreaterThanOrEqualTo(2);
    }

    /// <summary>Verifies a failed post-persistence refresh retains stale editor state and a later exact refresh alone clears it.</summary>
    /// <returns>A task that completes after the failed and successful refresh attempts.</returns>
    [Fact]
    public async Task EditParticipant_PostPersistenceRefresh_FailureRetainsEditorAndExactRetryClearsIt()
    {
        var context = await CreateParticipantContextAsync();
        using var viewModel = context.ViewModel;
        var editor = viewModel.Editor;
        var session = editor.Session.ShouldNotBeNull();
        var command = editor.AvailableCommands.Single(candidate => candidate.CommandName == "form-list.set-editor-id");
        var draft = editor.CreateDraft(command, NativeFormListDraftSeedSelection.CurrentValue()).Value.ShouldNotBeNull();
        context.Workspace.AdvanceRevision();
        var persistedRevision = context.Workspace.CurrentRevision;
        context.Workspace.FailPluginRead = true;
        using var transitionLease = await context.OperationArbiter.ReserveWorkspaceTransitionAsync(
            NativeWorkspaceTransitionDrainMode.WaitForCurrentOperation);

        var failed = await viewModel.RefreshAfterWorkspacePersistenceAsync(
            context.Workspace.WorkspaceId,
            persistedRevision);

        failed.Succeeded.ShouldBeFalse();
        failed.Error.ShouldNotBeNull().Code.ShouldBe(EngineErrorCode.UnexpectedFailure);
        editor.Session.ShouldBeSameAs(session);
        editor.Draft.ShouldBeSameAs(draft);
        viewModel.IsEditorBusy.ShouldBeFalse();
        editor.CanMutateDraft.ShouldBeFalse();
        viewModel.RetryCommand.CanExecute(null).ShouldBeFalse();

        context.Workspace.FailPluginRead = false;
        var refreshed = await viewModel.RefreshAfterWorkspacePersistenceAsync(
            context.Workspace.WorkspaceId,
            persistedRevision);

        refreshed.Succeeded.ShouldBeTrue();
        refreshed.Value.ShouldNotBeNull().Revision.ShouldBe(persistedRevision);
        refreshed.WorkspaceId.ShouldBe(context.Workspace.WorkspaceId);
        refreshed.ResultRevision.ShouldBe(persistedRevision);
        editor.Session.ShouldBeNull();
        editor.Draft.ShouldBeNull();
        viewModel.HasPendingOperation.ShouldBeFalse();
    }

    /// <summary>Verifies a successful snapshot at a different revision is rejected without clearing stale editor state.</summary>
    /// <returns>A task that completes after the mismatched refresh is rejected.</returns>
    [Fact]
    public async Task EditParticipant_PostPersistenceRefresh_MismatchedRevisionRetainsEditor()
    {
        var context = await CreateParticipantContextAsync();
        using var viewModel = context.ViewModel;
        var editor = viewModel.Editor;
        var session = editor.Session.ShouldNotBeNull();
        var command = editor.AvailableCommands.Single(candidate => candidate.CommandName == "form-list.set-editor-id");
        var draft = editor.CreateDraft(command, NativeFormListDraftSeedSelection.CurrentValue()).Value.ShouldNotBeNull();
        var acceptedRevision = context.Workspace.CurrentRevision.Next();
        context.Workspace.AdvanceRevision();
        context.Workspace.AdvanceRevision();
        using var transitionLease = await context.OperationArbiter.ReserveWorkspaceTransitionAsync(
            NativeWorkspaceTransitionDrainMode.WaitForCurrentOperation);

        var result = await viewModel.RefreshAfterWorkspacePersistenceAsync(
            context.Workspace.WorkspaceId,
            acceptedRevision);

        result.Succeeded.ShouldBeFalse();
        result.Error.ShouldNotBeNull().Code.ShouldBe(EngineErrorCode.RevisionConflict);
        editor.Session.ShouldBeSameAs(session);
        editor.Draft.ShouldBeSameAs(draft);
        editor.CanMutateDraft.ShouldBeFalse();
        viewModel.RetryCommand.CanExecute(null).ShouldBeFalse();
    }

    /// <summary>Verifies a successful native read carrying another workspace identity is rejected without clearing stale editor state.</summary>
    /// <returns>A task that completes after the mismatched refresh is rejected.</returns>
    [Fact]
    public async Task EditParticipant_PostPersistenceRefresh_MismatchedWorkspaceRetainsEditor()
    {
        var context = await CreateParticipantContextAsync();
        using var viewModel = context.ViewModel;
        var editor = viewModel.Editor;
        var session = editor.Session.ShouldNotBeNull();
        var command = editor.AvailableCommands.Single(candidate => candidate.CommandName == "form-list.set-editor-id");
        var draft = editor.CreateDraft(command, NativeFormListDraftSeedSelection.CurrentValue()).Value.ShouldNotBeNull();
        context.Workspace.AdvanceRevision();
        var acceptedRevision = context.Workspace.CurrentRevision;
        context.Workspace.ResultWorkspaceId = Guid.NewGuid();
        using var transitionLease = await context.OperationArbiter.ReserveWorkspaceTransitionAsync(
            NativeWorkspaceTransitionDrainMode.WaitForCurrentOperation);

        var result = await viewModel.RefreshAfterWorkspacePersistenceAsync(
            context.Workspace.WorkspaceId,
            acceptedRevision);

        result.Succeeded.ShouldBeFalse();
        result.Error.ShouldNotBeNull().Code.ShouldBe(EngineErrorCode.RevisionConflict);
        result.WorkspaceId.ShouldBe(context.Workspace.WorkspaceId);
        editor.Session.ShouldBeSameAs(session);
        editor.Draft.ShouldBeSameAs(draft);
        editor.CanMutateDraft.ShouldBeFalse();
        viewModel.RetryCommand.CanExecute(null).ShouldBeFalse();
    }

    /// <summary>Creates a browser-owned active editor against one deterministic Starfield workspace.</summary>
    /// <returns>The participant context after initial browser load and successful Begin publication.</returns>
    private static async Task<ParticipantTestContext> CreateParticipantContextAsync()
    {
        var workspaceId = Guid.NewGuid();
        var initialRevision = new WorkspaceRevision(Guid.NewGuid(), 4);
        var sourceMod = ModKey.FromNameAndExtension("ParticipantSource.esm");
        var formKey = new FormKey(sourceMod, 0x0100);
        var descriptor = CreateDescriptor(workspaceId, initialRevision);
        var workspace = new ParticipantTestWorkspace(
            workspaceId,
            initialRevision,
            descriptor.Output!,
            sourceMod,
            formKey);
        var coordinator = new RecordingNativeWorkspaceCoordinator();
        coordinator.Publish(descriptor, workspace);
        var dispatcher = new InlineUiDispatcher();
        var operationArbiter = new NativeWorkspacePresentationOperationArbiter();
        var picker = new RecordingNativeReferencePickerService();
        var validator = new NativeFormListDraftValidator();
        var editorFactory = new NativeFormListEditorViewModelFactory(
            coordinator,
            operationArbiter,
            new NativeFormListWireCatalogResolver(
                [new StarfieldFormListEditWireCodec()],
                [new StarfieldFormListEditWireSchemaCatalog()]),
            new NativeFormListDraftFactory(),
            validator,
            new NativeFormListDraftSerializer(validator),
            picker,
            dispatcher);
        var viewModel = new NativeFormListBrowserViewModel(
            coordinator,
            operationArbiter,
            new NativeJsonTreeProjectionService(),
            picker,
            dispatcher,
            editorFactory);
        await viewModel.StartAsync();
        await viewModel.Editor.BeginNewAsync();
        viewModel.Editor.Session.ShouldNotBeNull();
        return new ParticipantTestContext(viewModel, workspace, operationArbiter);
    }

    /// <summary>Groups the browser, mutable native fake, and exact shared arbiter used by participant tests.</summary>
    /// <param name="viewModel">The browser that owns the editor participant.</param>
    /// <param name="workspace">The deterministic native workspace.</param>
    /// <param name="operationArbiter">The exact shared presentation arbiter.</param>
    private sealed class ParticipantTestContext
    {
        /// <summary>Initializes one grouped participant-test context.</summary>
        /// <param name="viewModel">The browser that owns the editor participant.</param>
        /// <param name="workspace">The deterministic native workspace.</param>
        /// <param name="operationArbiter">The exact shared presentation arbiter.</param>
        public ParticipantTestContext(
            NativeFormListBrowserViewModel viewModel,
            ParticipantTestWorkspace workspace,
            NativeWorkspacePresentationOperationArbiter operationArbiter)
        {
            ViewModel = viewModel;
            Workspace = workspace;
            OperationArbiter = operationArbiter;
        }

        /// <summary>Gets the browser under test.</summary>
        public NativeFormListBrowserViewModel ViewModel { get; }

        /// <summary>Gets the deterministic mutable workspace.</summary>
        public ParticipantTestWorkspace Workspace { get; }

        /// <summary>Gets the exact shared presentation arbiter.</summary>
        public NativeWorkspacePresentationOperationArbiter OperationArbiter { get; }
    }

    /// <summary>Supplies browser reads and successful new-edit admission for participant lifecycle tests.</summary>
    private sealed class ParticipantTestWorkspace : IFormListWorkspace
    {
        /// <summary>The source plugin returned by browser enumeration.</summary>
        private readonly ModKey SourceModKey;

        /// <summary>The deterministic FormList allocated by Begin.</summary>
        private readonly FormKey FormKey;

        /// <summary>The exact selected output shared with the coordinator descriptor.</summary>
        private readonly OutputAssociation Output;

        /// <summary>The complete baseline for the selected output.</summary>
        private readonly OutputArtifactSetBaseline OutputBaseline;

        /// <summary>Initializes one deterministic participant workspace.</summary>
        /// <param name="workspaceId">The non-empty workspace identity.</param>
        /// <param name="revision">The initial workspace revision.</param>
        /// <param name="output">The exact selected output.</param>
        /// <param name="sourceModKey">The source plugin identity.</param>
        /// <param name="formKey">The deterministic FormList identity.</param>
        public ParticipantTestWorkspace(
            Guid workspaceId,
            WorkspaceRevision revision,
            OutputAssociation output,
            ModKey sourceModKey,
            FormKey formKey)
        {
            WorkspaceId = workspaceId;
            ResultWorkspaceId = workspaceId;
            CurrentRevision = revision;
            Output = output;
            SourceModKey = sourceModKey;
            FormKey = formKey;
            OutputBaseline = new OutputArtifactSetBaseline(
                revision.BaselineId,
                [new NativeArtifactAssociation(
                    output.PluginPath,
                    NativeArtifactRole.Plugin,
                    language: null,
                    new NativeArtifactFingerprint(false, 0, null))]);
        }

        /// <inheritdoc />
        public Guid WorkspaceId { get; }

        /// <inheritdoc />
        public WorkspaceRevision Revision => CurrentRevision;

        /// <inheritdoc />
        public OutputSynchronizationState OutputSynchronization { get; } = new(OutputSynchronizationStatus.Ready, null);

        /// <summary>Gets the revision currently returned by every exact native read.</summary>
        public WorkspaceRevision CurrentRevision { get; private set; }

        /// <summary>Gets or sets the identity stamped onto deterministic engine result envelopes.</summary>
        public Guid ResultWorkspaceId { get; set; }

        /// <summary>Gets or sets whether plugin enumeration returns a deterministic typed failure.</summary>
        public bool FailPluginRead { get; set; }

        /// <summary>Advances the native revision by one deterministic sequence.</summary>
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
                    SupportedGame.Starfield,
                    GameRelease.Starfield,
                    Output,
                    OutputBaseline,
                    OutputSynchronization,
                    CurrentRevision),
                ResultWorkspaceId,
                resultRevision: CurrentRevision));
        }

        /// <inheritdoc />
        public ValueTask<EngineResult<IReadOnlyList<PluginSummary>>> ListPluginsAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (FailPluginRead)
            {
                return ValueTask.FromResult(EngineResult<IReadOnlyList<PluginSummary>>.Failure(
                    new EngineError(EngineErrorCode.UnexpectedFailure, "The participant plugin read failed."),
                    ResultWorkspaceId,
                    resultRevision: CurrentRevision));
            }

            IReadOnlyList<PluginSummary> plugins =
            [
                new PluginSummary(
                    SourceModKey,
                    AbsolutePath(SourceModKey.FileName),
                    0,
                    PluginRole.Source),
            ];
            return ValueTask.FromResult(EngineResult<IReadOnlyList<PluginSummary>>.Success(
                plugins,
                ResultWorkspaceId,
                resultRevision: CurrentRevision));
        }

        /// <inheritdoc />
        public ValueTask<EngineResult<IReadOnlyList<FormListSummary>>> ListFormListsAsync(
            RecordScope scope,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return ValueTask.FromResult(EngineResult<IReadOnlyList<FormListSummary>>.Success(
                [],
                ResultWorkspaceId,
                resultRevision: CurrentRevision));
        }

        /// <inheritdoc />
        public ValueTask<EngineResult<EditReceipt>> BeginEditAsync(
            BeginEditRequest request,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            AdvanceRevision();
            return ValueTask.FromResult(EngineResult<EditReceipt>.Success(
                new EditReceipt(Guid.NewGuid(), FormKey, request.OriginFormKey, request.Role, CurrentRevision),
                WorkspaceId,
                request.OperationId,
                request.ExpectedRevision,
                CurrentRevision));
        }

        /// <inheritdoc />
        public ValueTask<EngineResult<FormListReadView>> ReadFormListViewAsync(
            ReferenceRequest request,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var context = new FormListContext(
                request,
                ReferenceResolutionStatus.Resolved,
                Output.ModKey,
                Output.PluginPath,
                1,
                PluginRole.Output);
            using var document = JsonDocument.Parse($"{{\"FormKey\":\"{FormKey}\",\"EditorID\":\"Before\"}}");
            return ValueTask.FromResult(EngineResult<FormListReadView>.Success(
                new FormListReadView(context, document.RootElement.Clone()),
                WorkspaceId,
                resultRevision: CurrentRevision));
        }

        /// <inheritdoc />
        public ValueTask<EngineResult<WorkspacePreview>> PreviewAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return ValueTask.FromResult(EngineResult<WorkspacePreview>.Success(
                new WorkspacePreview([], 0, []),
                WorkspaceId,
                resultRevision: CurrentRevision));
        }

        /// <inheritdoc />
        public ValueTask<EngineResult<OutputSelectionReceipt>> SelectOutputAsync(SelectOutputRequest request, CancellationToken cancellationToken = default) => throw Unsupported();

        /// <inheritdoc />
        public ValueTask<EngineResult<IMajorRecordGetter>> ReadFormListAsync(FormKey formKey, RecordScope scope, CancellationToken cancellationToken = default) => throw Unsupported();

        /// <inheritdoc />
        public ValueTask<EngineResult<ReferenceSearchPage>> SearchReferencesAsync(ReferenceSearchRequest request, CancellationToken cancellationToken = default) => throw Unsupported();

        /// <inheritdoc />
        public ValueTask<EngineResult<ReferenceResolution>> ResolveReferenceAsync(ReferenceRequest request, CancellationToken cancellationToken = default) => throw Unsupported();

        /// <inheritdoc />
        public ValueTask<EngineResult<OperationReceipt>> ApplyFormListEditAsync(FormListEditRequest request, CancellationToken cancellationToken = default) => throw Unsupported();

        /// <inheritdoc />
        public ValueTask<EngineResult<FormListComparison>> CompareFormListAsync(CompareFormListRequest request, CancellationToken cancellationToken = default) => throw Unsupported();

        /// <inheritdoc />
        public ValueTask<SaveResult> SaveAsync(SaveRequest request, CancellationToken cancellationToken = default) => throw Unsupported();

        /// <inheritdoc />
        public ValueTask<EngineResult<OutputSelectionReceipt>> ResolveOutputRecoveryAsync(ResolveOutputRecoveryRequest request, CancellationToken cancellationToken = default) => throw Unsupported();

        /// <inheritdoc />
        public ValueTask<EngineResult<OutputSelectionReceipt>> ReopenOutputAsync(ReopenOutputRequest request, CancellationToken cancellationToken = default) => throw Unsupported();

        /// <inheritdoc />
        public ValueTask<EngineResult<OperationReceipt>> DiscardChangesAsync(DiscardChangesRequest request, CancellationToken cancellationToken = default) => throw Unsupported();

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }

        /// <summary>Creates the deterministic exception for native operations outside participant-test scope.</summary>
        /// <returns>The unsupported-operation exception.</returns>
        private static NotSupportedException Unsupported()
        {
            return new NotSupportedException("This participant workspace supports browser reads and Begin only.");
        }
    }
}
