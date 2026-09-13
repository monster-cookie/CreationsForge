using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using CreationsForge.PresentationTests.Support;
using CreationsForge.Services;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Serilog;
using Shouldly;

namespace CreationsForge.PresentationTests.Services;

/// <summary>
/// Verifies transactional native workspace ownership and serialized borrowing at the presentation root lifetime.
/// </summary>
public sealed class NativeWorkspaceCoordinatorTests
{
    /// <summary>Verifies exact game and release forwarding for every supported presentation option.</summary>
    /// <param name="game">The selected CreationsForge game.</param>
    /// <param name="release">The exact native release expected by the engine factory.</param>
    [Theory]
    [InlineData(SupportedGame.Starfield, GameRelease.Starfield)]
    [InlineData(SupportedGame.Fallout4, GameRelease.Fallout4)]
    [InlineData(SupportedGame.Skyrim, GameRelease.SkyrimSE)]
    public async Task OpenAsync_WithSupportedGame_ForwardsCompleteRequestAndPublishesDescriptor(
        SupportedGame game,
        GameRelease release)
    {
        var workspace = CreateSuccessfulWorkspace();
        var factory = CreateFactory(workspace);
        var dispatcher = new InlineUiDispatcher();
        await using var coordinator = CreateCoordinator(factory, dispatcher);
        var request = CreateRequest(game, release, OutputMasterStyle.Full);

        var result = await coordinator.OpenAsync(request);

        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Game.ShouldBe(game);
        result.Value.Release.ShouldBe(release);
        coordinator.CurrentWorkspace.ShouldBeSameAs(result.Value);
        factory.Requests.Single().ShouldBeSameAs(request.Sources);
        workspace.LastSelectOutputRequest.ShouldNotBeNull();
        workspace.LastSelectOutputRequest.Mode.ShouldBe(request.OutputMode);
        workspace.LastSelectOutputRequest.Output.ShouldBeSameAs(request.Output);
        dispatcher.InvokeCount.ShouldBe(1);
    }

    /// <summary>Verifies source-only activation publishes a read-only workspace without selecting an output.</summary>
    [Fact]
    public async Task OpenAsync_WithSourceOnlyRequest_PublishesReadOnlyDescriptor()
    {
        var workspace = CreateSuccessfulWorkspace();
        var factory = CreateFactory(workspace);
        await using var coordinator = CreateCoordinator(factory, new InlineUiDispatcher());
        var editableRequest = CreateRequest(SupportedGame.Starfield, GameRelease.Starfield);

        var result = await coordinator.OpenAsync(new NativeWorkspaceOpenRequest(editableRequest.Sources));

        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Output.ShouldBeNull();
        result.Value.SourcePluginPath.ShouldBe(editableRequest.Sources.SourcePluginPath);
        result.Value.Revision.ShouldBe(workspace.Revision);
        workspace.LastSelectOutputRequest.ShouldBeNull();
    }

    /// <summary>Verifies a failed replacement disposes only its candidate and leaves the prior workspace active.</summary>
    [Fact]
    public async Task OpenAsync_WhenReplacementOutputFails_PreservesPriorWorkspaceAndDisposesCandidate()
    {
        var original = CreateSuccessfulWorkspace();
        var failedCandidate = CreateFailedOutputWorkspace();
        var results = new Queue<IFormListWorkspace>([original, failedCandidate]);
        var factory = new FakeFormListWorkspaceFactory((_, _) =>
            ValueTask.FromResult(EngineResult<IFormListWorkspace>.Success(results.Dequeue())));
        await using var coordinator = CreateCoordinator(factory, new InlineUiDispatcher());
        var firstResult = await coordinator.OpenAsync(CreateRequest(SupportedGame.Starfield, GameRelease.Starfield));

        var secondResult = await coordinator.OpenAsync(CreateRequest(SupportedGame.Fallout4, GameRelease.Fallout4));

        firstResult.Succeeded.ShouldBeTrue();
        secondResult.Succeeded.ShouldBeFalse();
        secondResult.Error!.Code.ShouldBe(EngineErrorCode.OutputOpenFailed);
        coordinator.CurrentWorkspace.ShouldBeSameAs(firstResult.Value);
        original.DisposeCount.ShouldBe(0);
        failedCandidate.DisposeCount.ShouldBe(1);
    }

    /// <summary>Verifies cancellation after source acquisition cleans the candidate and preserves the active workspace.</summary>
    [Fact]
    public async Task OpenAsync_WhenOutputSelectionIsCanceled_PreservesPriorWorkspaceAndDisposesCandidate()
    {
        var original = CreateSuccessfulWorkspace();
        using var cancellation = new CancellationTokenSource();
        var canceledCandidate = new FakeFormListWorkspace(
            Guid.NewGuid(),
            new WorkspaceRevision(Guid.NewGuid(), 0),
            (_, token) =>
            {
                cancellation.Cancel();
                token.ThrowIfCancellationRequested();
                throw new InvalidOperationException("Cancellation should have been observed.");
            });
        var results = new Queue<IFormListWorkspace>([original, canceledCandidate]);
        var factory = new FakeFormListWorkspaceFactory((_, _) =>
            ValueTask.FromResult(EngineResult<IFormListWorkspace>.Success(results.Dequeue())));
        await using var coordinator = CreateCoordinator(factory, new InlineUiDispatcher());
        var firstResult = await coordinator.OpenAsync(CreateRequest(SupportedGame.Starfield, GameRelease.Starfield));

        await Should.ThrowAsync<OperationCanceledException>(async () =>
        {
            await coordinator.OpenAsync(
                CreateRequest(SupportedGame.Fallout4, GameRelease.Fallout4),
                cancellation.Token);
        });

        coordinator.CurrentWorkspace.ShouldBeSameAs(firstResult.Value);
        original.DisposeCount.ShouldBe(0);
        canceledCandidate.DisposeCount.ShouldBe(1);
    }

    /// <summary>Verifies a borrowed operation excludes replacement until it releases the coordinator-owned workspace.</summary>
    [Fact]
    public async Task ExecuteAsync_WhileBorrowed_SerializesWorkspaceReplacement()
    {
        var original = CreateSuccessfulWorkspace();
        var replacement = CreateSuccessfulWorkspace();
        var results = new Queue<IFormListWorkspace>([original, replacement]);
        var factory = new FakeFormListWorkspaceFactory((_, _) =>
            ValueTask.FromResult(EngineResult<IFormListWorkspace>.Success(results.Dequeue())));
        await using var coordinator = CreateCoordinator(factory, new InlineUiDispatcher());
        await coordinator.OpenAsync(CreateRequest(SupportedGame.Starfield, GameRelease.Starfield));
        var borrowStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var releaseBorrow = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        var borrowTask = coordinator.ExecuteAsync<string>(async (workspace, _) =>
        {
            workspace.ShouldBeSameAs(original);
            borrowStarted.SetResult(true);
            await releaseBorrow.Task;
            return EngineResult<string>.Success("complete", workspaceId: workspace.WorkspaceId);
        }).AsTask();
        await borrowStarted.Task;
        var replaceTask = coordinator.OpenAsync(CreateRequest(SupportedGame.Skyrim, GameRelease.SkyrimSE)).AsTask();
        await Task.Yield();

        factory.Requests.Count.ShouldBe(1);
        replacement.DisposeCount.ShouldBe(0);
        releaseBorrow.SetResult(true);
        (await borrowTask).Succeeded.ShouldBeTrue();
        (await replaceTask).Succeeded.ShouldBeTrue();
        factory.Requests.Count.ShouldBe(2);
        original.DisposeCount.ShouldBe(1);
    }

    /// <summary>Verifies cancellation queued before UI publication rolls back acquisition at the actual commit boundary.</summary>
    [Fact]
    public async Task OpenAsync_WhenCanceledBeforeQueuedPublication_PreservesPriorWorkspaceAndDisposesCandidate()
    {
        var original = CreateSuccessfulWorkspace();
        var candidate = CreateSuccessfulWorkspace();
        var results = new Queue<IFormListWorkspace>([original, candidate]);
        var factory = new FakeFormListWorkspaceFactory((_, _) =>
            ValueTask.FromResult(EngineResult<IFormListWorkspace>.Success(results.Dequeue())));
        var dispatcher = new QueuedUiDispatcher();
        var coordinator = new NativeWorkspaceCoordinator(factory, dispatcher, new LoggerConfiguration().CreateLogger());
        try
        {
            var initialOpen = coordinator.OpenAsync(CreateRequest(SupportedGame.Starfield, GameRelease.Starfield)).AsTask();
            await dispatcher.WaitForInvocationAsync();
            dispatcher.RunNextInvocation();
            var initialResult = await initialOpen;
            using var cancellation = new CancellationTokenSource();

            var replacementOpen = coordinator.OpenAsync(
                CreateRequest(SupportedGame.Fallout4, GameRelease.Fallout4),
                cancellation.Token).AsTask();
            await dispatcher.WaitForInvocationAsync();
            cancellation.Cancel();
            dispatcher.RunNextInvocation();

            await Should.ThrowAsync<OperationCanceledException>(async () =>
            {
                await replacementOpen;
            });
            coordinator.CurrentWorkspace.ShouldBeSameAs(initialResult.Value);
            original.DisposeCount.ShouldBe(0);
            candidate.DisposeCount.ShouldBe(1);
        }
        finally
        {
            var disposal = coordinator.DisposeAsync().AsTask();
            await dispatcher.WaitForInvocationAsync();
            dispatcher.RunNextInvocation();
            await disposal;
        }
    }

    /// <summary>Verifies close releases native ownership even when bound state cannot be published.</summary>
    [Fact]
    public async Task CloseAsync_WhenDispatcherRejectsPublication_StillDisposesWorkspace()
    {
        var workspace = CreateSuccessfulWorkspace();
        var dispatcher = new InlineUiDispatcher();
        var coordinator = CreateCoordinator(CreateFactory(workspace), dispatcher);
        await coordinator.OpenAsync(CreateRequest(SupportedGame.Starfield, GameRelease.Starfield));
        dispatcher.InvokeException = new InvalidOperationException("Dispatcher unavailable.");

        await Should.ThrowAsync<InvalidOperationException>(async () => await coordinator.CloseAsync());

        workspace.DisposeCount.ShouldBe(1);
        dispatcher.InvokeException = null;
        await coordinator.DisposeAsync();
    }

    /// <summary>Verifies shutdown disposal releases native ownership even when bound state cannot be published.</summary>
    [Fact]
    public async Task DisposeAsync_WhenDispatcherRejectsPublication_StillDisposesWorkspaceAndRemainsIdempotent()
    {
        var workspace = CreateSuccessfulWorkspace();
        var dispatcher = new InlineUiDispatcher();
        var coordinator = CreateCoordinator(CreateFactory(workspace), dispatcher);
        await coordinator.OpenAsync(CreateRequest(SupportedGame.Starfield, GameRelease.Starfield));
        dispatcher.InvokeException = new InvalidOperationException("Dispatcher unavailable.");

        await Should.ThrowAsync<InvalidOperationException>(async () => await coordinator.DisposeAsync());
        await coordinator.DisposeAsync();

        workspace.DisposeCount.ShouldBe(1);
    }

    /// <summary>Verifies repeated close and disposal release each owned workspace exactly once.</summary>
    [Fact]
    public async Task CloseAndDisposeAsync_WhenRepeated_DisposesWorkspaceOnceAndClearsObservableState()
    {
        var workspace = CreateSuccessfulWorkspace();
        var dispatcher = new InlineUiDispatcher();
        var coordinator = CreateCoordinator(CreateFactory(workspace), dispatcher);
        var propertyChanges = 0;
        coordinator.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(NativeWorkspaceCoordinator.CurrentWorkspace))
            {
                propertyChanges++;
            }
        };
        await coordinator.OpenAsync(CreateRequest(SupportedGame.Starfield, GameRelease.Starfield));

        await coordinator.CloseAsync();
        await coordinator.CloseAsync();
        await coordinator.DisposeAsync();
        await coordinator.DisposeAsync();

        workspace.DisposeCount.ShouldBe(1);
        coordinator.CurrentWorkspace.ShouldBeNull();
        propertyChanges.ShouldBe(2);
        dispatcher.InvokeCount.ShouldBeGreaterThanOrEqualTo(4);
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

    /// <summary>Creates a factory that returns one successful workspace.</summary>
    /// <param name="workspace">The workspace returned by the factory.</param>
    /// <returns>The recording factory.</returns>
    private static FakeFormListWorkspaceFactory CreateFactory(FakeFormListWorkspace workspace)
    {
        return new FakeFormListWorkspaceFactory((_, _) =>
            ValueTask.FromResult(EngineResult<IFormListWorkspace>.Success(workspace)));
    }

    /// <summary>Creates a workspace whose output selection succeeds with the requested association.</summary>
    /// <returns>The successful test workspace.</returns>
    private static FakeFormListWorkspace CreateSuccessfulWorkspace()
    {
        var initialRevision = new WorkspaceRevision(Guid.NewGuid(), 0);
        return new FakeFormListWorkspace(
            Guid.NewGuid(),
            initialRevision,
            (request, _) =>
            {
                var resultingRevision = initialRevision.Next();
                var baseline = new OutputArtifactSetBaseline(
                    Guid.NewGuid(),
                    [new NativeArtifactAssociation(
                        Path.GetFullPath(request.Output.PluginPath),
                        NativeArtifactRole.Plugin,
                        null,
                        new NativeArtifactFingerprint(false, 0, null))]);
                var receipt = new OutputSelectionReceipt(request.Output, baseline, resultingRevision);
                return ValueTask.FromResult(EngineResult<OutputSelectionReceipt>.Success(
                    receipt,
                    operationId: request.OperationId,
                    baseRevision: request.ExpectedRevision,
                    resultRevision: resultingRevision));
            });
    }

    /// <summary>Creates a workspace whose output selection returns a stable engine failure.</summary>
    /// <returns>The failing test workspace.</returns>
    private static FakeFormListWorkspace CreateFailedOutputWorkspace()
    {
        return new FakeFormListWorkspace(
            Guid.NewGuid(),
            new WorkspaceRevision(Guid.NewGuid(), 0),
            (_, _) => ValueTask.FromResult(EngineResult<OutputSelectionReceipt>.Failure(
                new EngineError(EngineErrorCode.OutputOpenFailed, "The selected output could not be opened."))));
    }

    /// <summary>Creates complete explicit source and output inputs without touching the file system.</summary>
    /// <param name="game">The supported game.</param>
    /// <param name="release">The exact native release.</param>
    /// <param name="masterStyle">The requested output master style.</param>
    /// <returns>The complete coordinator request.</returns>
    private static NativeWorkspaceOpenRequest CreateRequest(
        SupportedGame game,
        GameRelease release,
        OutputMasterStyle masterStyle = OutputMasterStyle.Full)
    {
        var root = Path.Combine(Path.GetTempPath(), "CreationsForge-PresentationTests", Guid.NewGuid().ToString("N"));
        var sourcePath = Path.Combine(root, "Source.esm");
        var outputPath = Path.Combine(root, "Output.esp");
        var sources = new WorkspaceOpenRequest(
            Guid.NewGuid(),
            game,
            release,
            sourcePath,
            [sourcePath, Path.Combine(root, "Dependency.esm")],
            root,
            [Path.Combine(root, "Strings")]);
        var output = new OutputAssociation(
            outputPath,
            ModKey.FromNameAndExtension("Output.esp"),
            LocalizedOutputMode.Embedded,
            masterStyle);
        return new NativeWorkspaceOpenRequest(sources, OutputSelectionMode.CreateNew, output);
    }
}
