using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using CreationsForge.PresentationTests.Support;
using CreationsForge.Services;
using CreationsForge.ViewModels;
using Mutagen.Bethesda;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Shouldly;

namespace CreationsForge.PresentationTests.ViewModels;

/// <summary>
/// Verifies picker projection, game capabilities, validation, progress, and engine request forwarding for native workspace selection.
/// </summary>
public sealed class NativeWorkspaceSelectionViewModelTests
{
    /// <summary>Verifies the persisted active game selects the exact native release and valid output styles.</summary>
    /// <param name="game">The configured CreationsForge game.</param>
    /// <param name="release">The expected Mutagen release.</param>
    /// <param name="supportsMedium">Whether the release supports a medium master output.</param>
    [Theory]
    [InlineData(SupportedGame.Starfield, GameRelease.Starfield, true)]
    [InlineData(SupportedGame.Fallout4, GameRelease.Fallout4, false)]
    [InlineData(SupportedGame.Skyrim, GameRelease.SkyrimSE, false)]
    public void Constructor_WithConfiguredGame_SelectsExactReleaseAndOutputCapabilities(
        SupportedGame game,
        GameRelease release,
        bool supportsMedium)
    {
        var viewModel = CreateViewModel(
            CreateCoordinator(CreateFactory(CreateSuccessfulWorkspace()), new InlineUiDispatcher()),
            new FakeNativeWorkspacePathPicker(),
            new FakeGameSelectionService { ActiveGame = game },
            new InlineUiDispatcher());

        viewModel.SelectedGame.Game.ShouldBe(game);
        viewModel.SelectedGame.Release.ShouldBe(release);
        viewModel.OutputMasterStyleOptions.ShouldContain(OutputMasterStyle.Full);
        viewModel.OutputMasterStyleOptions.ShouldContain(OutputMasterStyle.Small);
        viewModel.OutputMasterStyleOptions.Contains(OutputMasterStyle.Medium).ShouldBe(supportsMedium);
    }

    /// <summary>Verifies picker results preserve the explicit caller order, including masters before the source.</summary>
    [Fact]
    public async Task BrowsePathsAsync_WithExplicitSelections_PreservesOrderedInputsAndOutputMode()
    {
        var root = CreateTestRoot();
        var source = Path.Combine(root, "Source.esm");
        var dependencyA = Path.Combine(root, "A.esm");
        var dependencyB = Path.Combine(root, "B.esm");
        var picker = new FakeNativeWorkspacePathPicker
        {
            SourcePluginPath = source,
            LoadOrderPluginPaths = [dependencyB, source, dependencyA, dependencyB],
            DataDirectoryPath = root,
            StringDirectoryPaths = [Path.Combine(root, "Strings"), Path.Combine(root, "Strings")],
            OutputPluginPath = Path.Combine(root, "Output.esp")
        };
        var dispatcher = new InlineUiDispatcher();
        var viewModel = CreateViewModel(
            CreateCoordinator(CreateFactory(CreateSuccessfulWorkspace()), dispatcher),
            picker,
            new FakeGameSelectionService(),
            dispatcher);
        viewModel.OutputMode = OutputSelectionMode.OpenExisting;

        await viewModel.BrowseSourcePluginAsync();
        await viewModel.BrowseLoadOrderAsync();
        await viewModel.BrowseDataDirectoryAsync();
        await viewModel.BrowseStringDirectoriesAsync();
        await viewModel.BrowseOutputPluginAsync();

        viewModel.SourcePluginPath.ShouldBe(source);
        viewModel.LoadOrderPluginPaths.ShouldBe([dependencyB, source, dependencyA]);
        viewModel.DataDirectoryPath.ShouldBe(root);
        viewModel.StringDirectoryPaths.ShouldBe([Path.Combine(root, "Strings")]);
        viewModel.OutputPluginPath.ShouldBe(Path.Combine(root, "Output.esp"));
        picker.RequestedOutputMode.ShouldBe(OutputSelectionMode.OpenExisting);
    }

    /// <summary>Verifies canceling the optional string-directory picker preserves the current explicit list.</summary>
    [Fact]
    public async Task BrowseStringDirectoriesAsync_WhenPickerIsCanceled_PreservesExistingDirectories()
    {
        var root = CreateTestRoot();
        var existing = Path.Combine(root, "Strings");
        var picker = new FakeNativeWorkspacePathPicker { StringDirectoryPaths = [] };
        var dispatcher = new InlineUiDispatcher();
        var viewModel = CreateViewModel(
            CreateCoordinator(CreateFactory(CreateSuccessfulWorkspace()), dispatcher),
            picker,
            new FakeGameSelectionService(),
            dispatcher);
        viewModel.StringDirectoryPaths.Add(existing);

        await viewModel.BrowseStringDirectoriesAsync();

        viewModel.StringDirectoryPaths.ShouldBe([existing]);
    }

    /// <summary>Verifies the first explicit entry can move later so the selected source is not treated as a fixed first plugin.</summary>
    [Fact]
    public void MoveLoadOrderPluginDown_WithFirstEntry_MovesItLaterInTheExplicitOrder()
    {
        var dispatcher = new InlineUiDispatcher();
        var viewModel = CreateViewModel(
            CreateCoordinator(CreateFactory(CreateSuccessfulWorkspace()), dispatcher),
            new FakeNativeWorkspacePathPicker(),
            new FakeGameSelectionService(),
            dispatcher);
        viewModel.LoadOrderPluginPaths.Add("Source.esm");
        viewModel.LoadOrderPluginPaths.Add("Master.esm");

        viewModel.MoveLoadOrderPluginDown(0);

        viewModel.LoadOrderPluginPaths.ShouldBe(["Master.esm", "Source.esm"]);
    }

    /// <summary>Verifies complete UI choices become exact engine acquisition and output-selection requests.</summary>
    [Fact]
    public async Task OpenWorkspaceAsync_WithValidSelection_ForwardsAllInputsReportsProgressAndPersistsGame()
    {
        var root = CreateTestRoot();
        var workspace = CreateSuccessfulWorkspace();
        var factory = new FakeFormListWorkspaceFactory((request, _) =>
        {
            request.Progress?.Report(new WorkspaceOpenProgress(WorkspaceOpenStage.OpeningSources, "Opening test sources."));
            return ValueTask.FromResult(EngineResult<IFormListWorkspace>.Success(workspace));
        });
        var dispatcher = new InlineUiDispatcher();
        var coordinator = CreateCoordinator(factory, dispatcher);
        var gameSelection = new FakeGameSelectionService();
        var viewModel = CreateViewModel(coordinator, new FakeNativeWorkspacePathPicker(), gameSelection, dispatcher);
        var skyrim = viewModel.Games.Single(option => option.Game == SupportedGame.Skyrim);
        var source = Path.Combine(root, "Skyrim.esm");
        viewModel.SelectedGame = skyrim;
        viewModel.SourcePluginPath = source;
        viewModel.LoadOrderPluginPaths.Add(source);
        viewModel.LoadOrderPluginPaths.Add(Path.Combine(root, "Update.esm"));
        viewModel.DataDirectoryPath = root;
        viewModel.StringDirectoryPaths.Add(Path.Combine(root, "Strings"));
        viewModel.OutputMode = OutputSelectionMode.OpenExisting;
        viewModel.OutputPluginPath = Path.Combine(root, "Patch.esl");
        viewModel.LocalizedOutputMode = LocalizedOutputMode.SeparateStringFiles;
        viewModel.OutputMasterStyle = OutputMasterStyle.Small;

        var succeeded = await viewModel.OpenWorkspaceAsync();

        succeeded.ShouldBeTrue();
        var request = factory.Requests.Single();
        request.Game.ShouldBe(SupportedGame.Skyrim);
        request.Release.ShouldBe(GameRelease.SkyrimSE);
        request.SourcePluginPath.ShouldBe(source);
        request.LoadOrderPluginPaths.ShouldBe([source, Path.Combine(root, "Update.esm")]);
        request.DataDirectoryPath.ShouldBe(root);
        request.StringDirectoryPaths.ShouldBe([Path.Combine(root, "Strings")]);
        workspace.LastSelectOutputRequest!.Mode.ShouldBe(OutputSelectionMode.OpenExisting);
        workspace.LastSelectOutputRequest.Output.LocalizedOutputMode.ShouldBe(LocalizedOutputMode.SeparateStringFiles);
        workspace.LastSelectOutputRequest.Output.MasterStyle.ShouldBe(OutputMasterStyle.Small);
        workspace.LastSelectOutputRequest.Output.ModKey.FileName.ToString().ShouldBe("Patch.esl");
        gameSelection.SavedGame.ShouldBe(SupportedGame.Skyrim);
        dispatcher.PostCount.ShouldBe(1);
        viewModel.StatusText.ShouldBe("Editing workspace ready: Patch.esl.");
        viewModel.HasError.ShouldBeFalse();
    }

    /// <summary>Verifies a master style unsupported by the selected game is rejected before engine acquisition.</summary>
    [Fact]
    public async Task OpenWorkspaceAsync_WithUnsupportedMasterStyle_ReportsValidationWithoutCallingFactory()
    {
        var factory = CreateFactory(CreateSuccessfulWorkspace());
        var dispatcher = new InlineUiDispatcher();
        var viewModel = CreateViewModel(
            CreateCoordinator(factory, dispatcher),
            new FakeNativeWorkspacePathPicker(),
            new FakeGameSelectionService { ActiveGame = SupportedGame.Fallout4 },
            dispatcher);
        PopulateRequiredPaths(viewModel);
        viewModel.OutputMasterStyle = OutputMasterStyle.Medium;

        var succeeded = await viewModel.OpenWorkspaceAsync();

        succeeded.ShouldBeFalse();
        viewModel.ErrorText.ShouldBe("Fallout 4 does not support the selected output master style.");
        factory.Requests.ShouldBeEmpty();
    }

    /// <summary>Verifies typed engine failures become actionable bound error state without persisting a new active game.</summary>
    [Fact]
    public async Task OpenWorkspaceAsync_WhenEngineRejectsSelection_ReportsErrorAndKeepsSettings()
    {
        var expectedError = new EngineError(EngineErrorCode.MissingMaster, "The explicit load order is missing Master.esm.");
        var factory = new FakeFormListWorkspaceFactory((request, _) =>
            ValueTask.FromResult(EngineResult<IFormListWorkspace>.Failure(expectedError, workspaceId: request.WorkspaceId)));
        var dispatcher = new InlineUiDispatcher();
        var gameSelection = new FakeGameSelectionService();
        var logSink = new CollectingLogSink();
        using var logger = new LoggerConfiguration().WriteTo.Sink(logSink).CreateLogger();
        var viewModel = CreateViewModel(
            CreateCoordinator(factory, dispatcher),
            new FakeNativeWorkspacePathPicker(),
            gameSelection,
            dispatcher,
            logger);
        PopulateRequiredPaths(viewModel);

        var succeeded = await viewModel.OpenWorkspaceAsync();

        succeeded.ShouldBeFalse();
        viewModel.ErrorText.ShouldBe(expectedError.Message);
        viewModel.StatusText.ShouldBe("Native workspace opening failed.");
        gameSelection.SavedGame.ShouldBeNull();
        logSink.Events.ShouldContain(logEvent =>
            logEvent.Level == LogEventLevel.Warning
            && logEvent.RenderMessage().Contains("MissingMaster", StringComparison.Ordinal)
            && logEvent.RenderMessage().Contains(expectedError.Message, StringComparison.Ordinal));
    }

    /// <summary>Verifies a post-activation preference failure remains a successful activation with truthful warning state.</summary>
    [Fact]
    public async Task OpenWorkspaceAsync_WhenGamePreferenceSaveFails_KeepsActivatedWorkspaceAndReportsWarning()
    {
        var workspace = CreateSuccessfulWorkspace();
        var dispatcher = new InlineUiDispatcher();
        var coordinator = CreateCoordinator(CreateFactory(workspace), dispatcher);
        var gameSelection = new FakeGameSelectionService { ThrowOnSave = true };
        var viewModel = CreateViewModel(
            coordinator,
            new FakeNativeWorkspacePathPicker(),
            gameSelection,
            dispatcher);
        PopulateRequiredPaths(viewModel);

        var succeeded = await viewModel.OpenWorkspaceAsync();

        succeeded.ShouldBeTrue();
        coordinator.CurrentWorkspace.ShouldNotBeNull();
        coordinator.CurrentWorkspace.WorkspaceId.ShouldBe(workspace.WorkspaceId);
        viewModel.ErrorText.ShouldBe("The workspace opened, but the active-game preference could not be saved.");
        viewModel.StatusText.ShouldContain("The game preference was not saved.");
        workspace.DisposeCount.ShouldBe(0);
    }

    /// <summary>Verifies dialog cancellation can drain an in-flight acquisition before dismissal.</summary>
    [Fact]
    public async Task CancelAndWaitForOpenAsync_WithBlockedOutputSelection_DrainsCandidateCleanup()
    {
        var selectionStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var revision = new WorkspaceRevision(Guid.NewGuid(), 0);
        var candidate = new FakeFormListWorkspace(
            Guid.NewGuid(),
            revision,
            async (_, cancellationToken) =>
            {
                selectionStarted.SetResult(true);
                await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
                throw new InvalidOperationException("Cancellation should have interrupted output selection.");
            });
        var dispatcher = new InlineUiDispatcher();
        var coordinator = CreateCoordinator(CreateFactory(candidate), dispatcher);
        var viewModel = CreateViewModel(
            coordinator,
            new FakeNativeWorkspacePathPicker(),
            new FakeGameSelectionService(),
            dispatcher);
        PopulateRequiredPaths(viewModel);
        var openTask = viewModel.OpenWorkspaceAsync();
        await selectionStarted.Task;

        var activated = await viewModel.CancelAndWaitForOpenAsync();

        activated.ShouldBeFalse();
        (await openTask).ShouldBeFalse();
        viewModel.IsBusy.ShouldBeFalse();
        viewModel.StatusText.ShouldBe("Workspace opening canceled.");
        coordinator.CurrentWorkspace.ShouldBeNull();
        candidate.DisposeCount.ShouldBe(1);
    }

    /// <summary>Verifies cancellation after the dispatched commit reports the activation that actually occurred.</summary>
    [Fact]
    public async Task CancelAndWaitForOpenAsync_AfterPublicationCommitted_ReturnsActivatedOutcome()
    {
        var candidate = CreateSuccessfulWorkspace();
        var dispatcher = new QueuedUiDispatcher();
        var factory = CreateFactory(candidate);
        var coordinator = new NativeWorkspaceCoordinator(factory, dispatcher, new LoggerConfiguration().CreateLogger());
        try
        {
            var viewModel = new NativeWorkspaceSelectionViewModel(
                coordinator,
                new FakeNativeWorkspacePathPicker(),
                new FakeGameSelectionService(),
                dispatcher,
                new LoggerConfiguration().CreateLogger());
            PopulateRequiredPaths(viewModel);
            var openTask = viewModel.OpenWorkspaceAsync();
            await dispatcher.WaitForInvocationAsync();
            dispatcher.RunNextActionWithoutCompleting();
            coordinator.CurrentWorkspace.ShouldNotBeNull();

            var drainTask = viewModel.CancelAndWaitForOpenAsync();
            dispatcher.CompleteRunningInvocation();

            (await drainTask).ShouldBeTrue();
            (await openTask).ShouldBeTrue();
            viewModel.StatusText.ShouldContain("Workspace ready:");
            candidate.DisposeCount.ShouldBe(0);
        }
        finally
        {
            var disposal = coordinator.DisposeAsync().AsTask();
            await dispatcher.WaitForInvocationAsync();
            dispatcher.RunNextInvocation();
            await disposal;
        }
    }

    /// <summary>Creates a native workspace selection view model with deterministic test dependencies.</summary>
    /// <param name="coordinator">The native workspace owner.</param>
    /// <param name="picker">The configured path picker.</param>
    /// <param name="gameSelection">The active-game preference fake.</param>
    /// <param name="dispatcher">The synchronous presentation dispatcher.</param>
    /// <param name="logger">The optional logger used to verify structured diagnostics.</param>
    /// <returns>The view model under test.</returns>
    private static NativeWorkspaceSelectionViewModel CreateViewModel(
        NativeWorkspaceCoordinator coordinator,
        FakeNativeWorkspacePathPicker picker,
        FakeGameSelectionService gameSelection,
        InlineUiDispatcher dispatcher,
        ILogger? logger = null)
    {
        return new NativeWorkspaceSelectionViewModel(
            coordinator,
            picker,
            gameSelection,
            dispatcher,
            logger ?? new LoggerConfiguration().CreateLogger());
    }

    /// <summary>Creates a coordinator with deterministic test dependencies.</summary>
    /// <param name="factory">The recording engine factory.</param>
    /// <param name="dispatcher">The synchronous presentation dispatcher.</param>
    /// <returns>The coordinator under test.</returns>
    private static NativeWorkspaceCoordinator CreateCoordinator(
        FakeFormListWorkspaceFactory factory,
        InlineUiDispatcher dispatcher)
    {
        return new NativeWorkspaceCoordinator(factory, dispatcher, new LoggerConfiguration().CreateLogger());
    }

    /// <summary>Creates a factory that returns one successful test workspace.</summary>
    /// <param name="workspace">The workspace to return.</param>
    /// <returns>The recording factory.</returns>
    private static FakeFormListWorkspaceFactory CreateFactory(FakeFormListWorkspace workspace)
    {
        return new FakeFormListWorkspaceFactory((_, _) =>
            ValueTask.FromResult(EngineResult<IFormListWorkspace>.Success(workspace)));
    }

    /// <summary>Creates a workspace whose output selection succeeds using the requested association.</summary>
    /// <returns>The successful test workspace.</returns>
    private static FakeFormListWorkspace CreateSuccessfulWorkspace()
    {
        var revision = new WorkspaceRevision(Guid.NewGuid(), 0);
        return new FakeFormListWorkspace(
            Guid.NewGuid(),
            revision,
            (request, _) =>
            {
                var resultRevision = revision.Next();
                var baseline = new OutputArtifactSetBaseline(
                    Guid.NewGuid(),
                    [new NativeArtifactAssociation(
                        Path.GetFullPath(request.Output.PluginPath),
                        NativeArtifactRole.Plugin,
                        null,
                        new NativeArtifactFingerprint(false, 0, null))]);
                return ValueTask.FromResult(EngineResult<OutputSelectionReceipt>.Success(
                    new OutputSelectionReceipt(request.Output, baseline, resultRevision),
                    operationId: request.OperationId,
                    baseRevision: request.ExpectedRevision,
                    resultRevision: resultRevision));
            });
    }

    /// <summary>Populates structurally complete source, data, and output paths for validation tests.</summary>
    /// <param name="viewModel">The selection view model to populate.</param>
    private static void PopulateRequiredPaths(NativeWorkspaceSelectionViewModel viewModel)
    {
        var root = CreateTestRoot();
        var source = Path.Combine(root, "Source.esm");
        viewModel.SourcePluginPath = source;
        viewModel.LoadOrderPluginPaths.Add(source);
        viewModel.DataDirectoryPath = root;
        viewModel.OutputPluginPath = Path.Combine(root, "Output.esp");
    }

    /// <summary>Creates a unique absolute path without writing machine-specific test state.</summary>
    /// <returns>The unique test path.</returns>
    private static string CreateTestRoot()
    {
        return Path.Combine(Path.GetTempPath(), "CreationsForge-PresentationTests", Guid.NewGuid().ToString("N"));
    }

    /// <summary>Collects structured Serilog events emitted during one isolated test.</summary>
    private sealed class CollectingLogSink : ILogEventSink
    {
        /// <summary>Gets emitted log events in publication order.</summary>
        internal IList<LogEvent> Events { get; } = [];

        /// <inheritdoc />
        public void Emit(LogEvent logEvent)
        {
            Events.Add(logEvent);
        }
    }
}
