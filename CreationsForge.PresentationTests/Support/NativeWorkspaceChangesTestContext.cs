using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;
using CreationsForge.ViewModels;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Serilog;

namespace CreationsForge.PresentationTests.Support;

/// <summary>Creates a complete deterministic presentation change-lifecycle dependency graph.</summary>
internal sealed class NativeWorkspaceChangesTestContext : IDisposable, IAsyncDisposable
{
    /// <summary>Initializes one context from its fully constructed dependencies.</summary>
    /// <param name="viewModel">The change-lifecycle system under test.</param>
    /// <param name="coordinator">The recording root workspace coordinator.</param>
    /// <param name="workspace">The optional active recording workspace.</param>
    /// <param name="editParticipant">The recording editor participant.</param>
    /// <param name="saveCoordinator">The recording save recovery coordinator.</param>
    /// <param name="dialogService">The recording modal dialog service.</param>
    /// <param name="arbiter">The real presentation operation arbiter.</param>
    /// <param name="dispatcher">The synchronous recording dispatcher.</param>
    private NativeWorkspaceChangesTestContext(
        NativeWorkspaceChangesViewModel viewModel,
        RecordingWorkspaceChangesCoordinator coordinator,
        RecordingWorkspaceChangesWorkspace? workspace,
        RecordingWorkspaceEditParticipant editParticipant,
        RecordingWorkspaceSaveCoordinator saveCoordinator,
        RecordingWorkspaceChangesDialogService dialogService,
        NativeWorkspacePresentationOperationArbiter arbiter,
        InlineUiDispatcher dispatcher)
    {
        ViewModel = viewModel;
        Coordinator = coordinator;
        Workspace = workspace;
        EditParticipant = editParticipant;
        SaveCoordinator = saveCoordinator;
        DialogService = dialogService;
        Arbiter = arbiter;
        Dispatcher = dispatcher;
    }

    /// <summary>Gets the system under test.</summary>
    internal NativeWorkspaceChangesViewModel ViewModel { get; }

    /// <summary>Gets the recording root workspace coordinator.</summary>
    internal RecordingWorkspaceChangesCoordinator Coordinator { get; }

    /// <summary>Gets the recording workspace, or <see langword="null"/> for an empty shell.</summary>
    internal RecordingWorkspaceChangesWorkspace? Workspace { get; }

    /// <summary>Gets the recording browser and editor participant.</summary>
    internal RecordingWorkspaceEditParticipant EditParticipant { get; }

    /// <summary>Gets the recording save recovery coordinator.</summary>
    internal RecordingWorkspaceSaveCoordinator SaveCoordinator { get; }

    /// <summary>Gets the recording modal dialog service.</summary>
    internal RecordingWorkspaceChangesDialogService DialogService { get; }

    /// <summary>Gets the real presentation operation arbiter.</summary>
    internal NativeWorkspacePresentationOperationArbiter Arbiter { get; }

    /// <summary>Gets the synchronous recording dispatcher.</summary>
    internal InlineUiDispatcher Dispatcher { get; }

    /// <summary>Creates a context with no active native workspace.</summary>
    /// <returns>The empty-shell test context.</returns>
    internal static NativeWorkspaceChangesTestContext CreateNoWorkspace()
    {
        var coordinator = new RecordingWorkspaceChangesCoordinator();
        return Create(coordinator, workspace: null, hasDraftChanges: false);
    }

    /// <summary>Creates a ready Starfield workspace with optional staged and request-local changes.</summary>
    /// <param name="hasStagedChanges">Whether preview returns one detached FormList comparison.</param>
    /// <param name="hasDraftChanges">Whether request-local editor controls are changed.</param>
    /// <returns>The configured ready-workspace test context.</returns>
    internal static NativeWorkspaceChangesTestContext CreateReady(
        bool hasStagedChanges = false,
        bool hasDraftChanges = false)
    {
        return CreateReadyCore(
            hasStagedChanges,
            hasDraftChanges,
            dialogService: null,
            uiDispatcher: null);
    }

    /// <summary>Creates a source-only Starfield workspace with no mutable output.</summary>
    /// <returns>The configured read-only workspace test context.</returns>
    internal static NativeWorkspaceChangesTestContext CreateReadOnly()
    {
        var workspaceId = Guid.NewGuid();
        var sourceModKey = ModKey.FromNameAndExtension("ChangesSource.esm");
        var sourcePath = AbsolutePath(sourceModKey.FileName);
        var revision = new WorkspaceRevision(Guid.NewGuid(), 0);
        var state = new WorkspaceState(
            SupportedGame.Starfield,
            GameRelease.Starfield,
            output: null,
            outputBaseline: null,
            new OutputSynchronizationState(OutputSynchronizationStatus.Ready, null),
            revision);
        var workspace = new RecordingWorkspaceChangesWorkspace(workspaceId, state)
        {
            Preview = new WorkspacePreview([], 0, [])
        };
        var descriptor = new NativeWorkspaceDescriptor(
            workspaceId,
            state.Game,
            state.Release,
            sourcePath,
            [sourcePath],
            output: null,
            revision);
        var coordinator = new RecordingWorkspaceChangesCoordinator();
        coordinator.SetWorkspace(workspace, descriptor);
        return Create(coordinator, workspace, hasDraftChanges: false);
    }

    /// <summary>Creates a ready Starfield workspace with explicit production-facing presentation services.</summary>
    /// <param name="dialogService">The dialog service used by the change lifecycle.</param>
    /// <param name="uiDispatcher">The dispatcher used for bound presentation state.</param>
    /// <param name="hasStagedChanges">Whether preview returns one detached FormList comparison.</param>
    /// <param name="hasDraftChanges">Whether request-local editor controls are changed.</param>
    /// <returns>The configured ready-workspace test context.</returns>
    internal static NativeWorkspaceChangesTestContext CreateReadyWithPresentationServices(
        INativeWorkspaceChangesDialogService dialogService,
        IUiDispatcher uiDispatcher,
        bool hasStagedChanges = false,
        bool hasDraftChanges = false)
    {
        ArgumentNullException.ThrowIfNull(dialogService);
        ArgumentNullException.ThrowIfNull(uiDispatcher);
        return CreateReadyCore(hasStagedChanges, hasDraftChanges, dialogService, uiDispatcher);
    }

    /// <summary>Creates the shared ready-workspace graph around optional presentation-service overrides.</summary>
    /// <param name="hasStagedChanges">Whether preview returns one detached FormList comparison.</param>
    /// <param name="hasDraftChanges">Whether request-local editor controls are changed.</param>
    /// <param name="dialogService">The optional dialog service override.</param>
    /// <param name="uiDispatcher">The optional presentation dispatcher override.</param>
    /// <returns>The configured ready-workspace test context.</returns>
    private static NativeWorkspaceChangesTestContext CreateReadyCore(
        bool hasStagedChanges,
        bool hasDraftChanges,
        INativeWorkspaceChangesDialogService? dialogService,
        IUiDispatcher? uiDispatcher)
    {
        var workspaceId = Guid.NewGuid();
        var sourceModKey = ModKey.FromNameAndExtension("ChangesSource.esm");
        var outputModKey = ModKey.FromNameAndExtension("ChangesOutput.esp");
        var sourcePath = AbsolutePath(sourceModKey.FileName);
        var outputPath = AbsolutePath(outputModKey.FileName);
        var output = new OutputAssociation(
            outputPath,
            outputModKey,
            LocalizedOutputMode.Embedded,
            OutputMasterStyle.Full);
        var baseline = new OutputArtifactSetBaseline(
            Guid.NewGuid(),
            [new NativeArtifactAssociation(
                outputPath,
                NativeArtifactRole.Plugin,
                language: null,
                new NativeArtifactFingerprint(false, 0, null))]);
        var revision = new WorkspaceRevision(Guid.NewGuid(), 3);
        var state = new WorkspaceState(
            SupportedGame.Starfield,
            GameRelease.Starfield,
            output,
            baseline,
            new OutputSynchronizationState(OutputSynchronizationStatus.Ready, null),
            revision);
        var workspace = new RecordingWorkspaceChangesWorkspace(workspaceId, state)
        {
            Preview = hasStagedChanges
                ? new WorkspacePreview([CreateComparison(sourceModKey, sourcePath, output)], 0, [])
                : new WorkspacePreview([], 0, [])
        };
        var descriptor = new NativeWorkspaceDescriptor(
            workspaceId,
            state.Game,
            state.Release,
            sourcePath,
            [sourcePath],
            output,
            revision);
        var coordinator = new RecordingWorkspaceChangesCoordinator();
        coordinator.SetWorkspace(workspace, descriptor);
        return Create(coordinator, workspace, hasDraftChanges, dialogService, uiDispatcher);
    }

    /// <summary>Disposes the change view model synchronously for conventional test cleanup.</summary>
    public void Dispose()
    {
        ViewModel.Dispose();
    }

    /// <summary>Drains and disposes the change view model asynchronously.</summary>
    /// <returns>A task that completes after lifecycle shutdown.</returns>
    public ValueTask DisposeAsync()
    {
        return ViewModel.DisposeAsync();
    }

    /// <summary>Creates shared presentation dependencies around an optional recording workspace.</summary>
    /// <param name="coordinator">The recording coordinator to compose.</param>
    /// <param name="workspace">The optional active recording workspace.</param>
    /// <param name="hasDraftChanges">Whether the editor participant begins with changed request-local controls.</param>
    /// <param name="dialogService">The optional dialog service override used by the view model.</param>
    /// <param name="uiDispatcher">The optional presentation dispatcher override used by the view model.</param>
    /// <returns>The composed disposable test context.</returns>
    private static NativeWorkspaceChangesTestContext Create(
        RecordingWorkspaceChangesCoordinator coordinator,
        RecordingWorkspaceChangesWorkspace? workspace,
        bool hasDraftChanges,
        INativeWorkspaceChangesDialogService? dialogService = null,
        IUiDispatcher? uiDispatcher = null)
    {
        var editParticipant = new RecordingWorkspaceEditParticipant();
        editParticipant.SetState(hasDraftChanges);
        var saveCoordinator = new RecordingWorkspaceSaveCoordinator();
        var recordingDialogService = new RecordingWorkspaceChangesDialogService();
        var arbiter = new NativeWorkspacePresentationOperationArbiter();
        var dispatcher = new InlineUiDispatcher();
        var viewModel = new NativeWorkspaceChangesViewModel(
            coordinator,
            saveCoordinator,
            arbiter,
            editParticipant,
            new NativeJsonTreeProjectionService(),
            dialogService ?? recordingDialogService,
            uiDispatcher ?? dispatcher,
            new LoggerConfiguration().CreateLogger());
        return new NativeWorkspaceChangesTestContext(
            viewModel,
            coordinator,
            workspace,
            editParticipant,
            saveCoordinator,
            recordingDialogService,
            arbiter,
            dispatcher);
    }

    /// <summary>Creates one detached staged FormList comparison.</summary>
    /// <param name="sourceModKey">The source plugin identity.</param>
    /// <param name="sourcePath">The fully qualified source path.</param>
    /// <param name="output">The selected output association.</param>
    /// <returns>The detached comparison.</returns>
    private static FormListComparison CreateComparison(
        ModKey sourceModKey,
        string sourcePath,
        OutputAssociation output)
    {
        var formKey = new FormKey(sourceModKey, 0x123);
        var beforeSelection = new ReferenceRequest(formKey, RecordScope.AllContexts, sourceModKey);
        var afterSelection = new ReferenceRequest(formKey, RecordScope.StagedOutput, output.ModKey);
        var beforeContext = new FormListContext(
            beforeSelection,
            ReferenceResolutionStatus.Resolved,
            sourceModKey,
            sourcePath,
            0,
            PluginRole.Source);
        var afterContext = new FormListContext(
            afterSelection,
            ReferenceResolutionStatus.Resolved,
            output.ModKey,
            output.PluginPath,
            1,
            PluginRole.Output);
        return new FormListComparison(
            beforeContext,
            afterContext,
            Json("{\"$type\":\"FormList\",\"EditorID\":\"Before\",\"Items\":[]}"),
            Json("{\"$type\":\"FormList\",\"EditorID\":\"After\",\"Items\":[]}"),
            [new SemanticChangeDescriptor("EditorID", SemanticChangeKind.ValueChanged)],
            []);
    }

    /// <summary>Creates a detached JSON value.</summary>
    /// <param name="value">The complete JSON text.</param>
    /// <returns>A cloned JSON element independent of the parser document.</returns>
    private static JsonElement Json(string value)
    {
        using var document = JsonDocument.Parse(value);
        return document.RootElement.Clone();
    }

    /// <summary>Creates a fully qualified test-only path without retaining a machine-specific literal.</summary>
    /// <param name="fileName">The test artifact file name.</param>
    /// <returns>A fully qualified path under the process temporary directory.</returns>
    private static string AbsolutePath(string fileName)
    {
        return Path.GetFullPath(Path.Combine(Path.GetTempPath(), "CreationsForgePresentationTests", fileName));
    }
}
