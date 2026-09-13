using CreationsForge.Mcp;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using Moq;
using Mutagen.Bethesda;
using Shouldly;

namespace CreationsForge.UnitTests.McpHost;

/// <summary>
/// Verifies MCP ownership of independent engine workspace lifetimes.
/// </summary>
public sealed class McpWorkspaceRegistryTests
{
    /// <summary>Verifies that a successful open is published and closed exactly once.</summary>
    [Fact]
    public async Task OpenAsync_WhenFactorySucceeds_PublishesAndClosesWorkspace()
    {
        await using var registry = new McpWorkspaceRegistry();
        var request = CreateRequest();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 3);
        var workspace = CreateWorkspace(request.WorkspaceId, revision);
        var factory = CreateFactory(EngineResult<IFormListWorkspace>.Success(workspace.Object));

        var result = await registry.OpenAsync(factory.Object, request);

        result.Succeeded.ShouldBeTrue();
        result.Value.ShouldBe(revision);
        result.WorkspaceId.ShouldBe(request.WorkspaceId);
        registry.ActiveWorkspaceCount.ShouldBe(1);
        registry.GetWorkspaceIds().ShouldBe([request.WorkspaceId]);

        (await registry.CloseAsync(request.WorkspaceId)).ShouldBeTrue();
        (await registry.CloseAsync(request.WorkspaceId)).ShouldBeFalse();
        registry.ActiveWorkspaceCount.ShouldBe(0);
        workspace.Verify(candidate => candidate.DisposeAsync(), Times.Once);
    }

    /// <summary>Verifies that an identifier reservation prevents duplicate factory acquisition.</summary>
    [Fact]
    public async Task OpenAsync_WhenWorkspaceAlreadyExists_RejectsDuplicateWithoutOpeningFactoryAgain()
    {
        await using var registry = new McpWorkspaceRegistry();
        var request = CreateRequest();
        var workspace = CreateWorkspace(request.WorkspaceId, new WorkspaceRevision(Guid.NewGuid(), 0));
        var factory = CreateFactory(EngineResult<IFormListWorkspace>.Success(workspace.Object));

        var firstResult = await registry.OpenAsync(factory.Object, request);
        var duplicateResult = await registry.OpenAsync(factory.Object, request);

        firstResult.Succeeded.ShouldBeTrue();
        duplicateResult.Succeeded.ShouldBeFalse();
        duplicateResult.Error.ShouldNotBeNull();
        duplicateResult.Error.Code.ShouldBe(EngineErrorCode.InvalidRequest);
        factory.Verify(
            candidate => candidate.OpenAsync(request, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>Verifies that typed factory failures do not publish registry state.</summary>
    [Fact]
    public async Task OpenAsync_WhenFactoryFails_PreservesFailureWithoutPublishingWorkspace()
    {
        await using var registry = new McpWorkspaceRegistry();
        var request = CreateRequest();
        var expectedError = new EngineError(
            EngineErrorCode.UnsupportedGameRelease,
            "No plugin adapter supports the requested release.");
        var factory = CreateFactory(
            EngineResult<IFormListWorkspace>.Failure(expectedError, workspaceId: request.WorkspaceId));

        var result = await registry.OpenAsync(factory.Object, request);

        result.Succeeded.ShouldBeFalse();
        result.Error.ShouldBeSameAs(expectedError);
        result.WorkspaceId.ShouldBe(request.WorkspaceId);
        registry.ActiveWorkspaceCount.ShouldBe(0);
    }

    /// <summary>Verifies that cancellation after factory acquisition disposes plugin state before releasing the reservation.</summary>
    [Fact]
    public async Task OpenAsync_WhenCancelledAfterAcquisition_DisposesUnpublishedWorkspace()
    {
        await using var registry = new McpWorkspaceRegistry();
        var request = CreateRequest();
        var workspace = CreateWorkspace(request.WorkspaceId, new WorkspaceRevision(Guid.NewGuid(), 0));
        using var cancellationSource = new CancellationTokenSource();
        var factory = new Mock<IFormListWorkspaceFactory>();
        factory.Setup(candidate => candidate.OpenAsync(request, cancellationSource.Token))
            .Returns(() =>
            {
                cancellationSource.Cancel();
                return ValueTask.FromResult(
                    EngineResult<IFormListWorkspace>.Success(workspace.Object));
            });

        await Should.ThrowAsync<OperationCanceledException>(async () =>
            await registry.OpenAsync(factory.Object, request, cancellationSource.Token));

        registry.ActiveWorkspaceCount.ShouldBe(0);
        workspace.Verify(candidate => candidate.DisposeAsync(), Times.Once);
    }

    /// <summary>Verifies that host shutdown waits for a pending open to release acquired plugin state.</summary>
    [Fact]
    public async Task DisposeAsync_WhenOpenIsPending_WaitsForOpenCleanup()
    {
        var registry = new McpWorkspaceRegistry();
        var request = CreateRequest();
        var workspace = CreateWorkspace(request.WorkspaceId, new WorkspaceRevision(Guid.NewGuid(), 0));
        var openCompletion = new TaskCompletionSource<EngineResult<IFormListWorkspace>>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        var factory = new Mock<IFormListWorkspaceFactory>();
        factory.Setup(candidate => candidate.OpenAsync(request, It.IsAny<CancellationToken>()))
            .Returns(new ValueTask<EngineResult<IFormListWorkspace>>(openCompletion.Task));

        var openTask = registry.OpenAsync(factory.Object, request).AsTask();
        factory.Verify(
            candidate => candidate.OpenAsync(request, It.IsAny<CancellationToken>()),
            Times.Once);
        var disposeTask = registry.DisposeAsync().AsTask();
        disposeTask.IsCompleted.ShouldBeFalse();

        openCompletion.SetResult(EngineResult<IFormListWorkspace>.Success(workspace.Object));

        await Should.ThrowAsync<ObjectDisposedException>(async () => await openTask);
        await disposeTask;
        workspace.Verify(candidate => candidate.DisposeAsync(), Times.Once);
    }

    /// <summary>Verifies that registry disposal attempts every active workspace even when one disposal fails.</summary>
    [Fact]
    public async Task DisposeAsync_WhenOneWorkspaceFails_StillDisposesRemainingWorkspaces()
    {
        var registry = new McpWorkspaceRegistry();
        var firstRequest = CreateRequest(Guid.NewGuid());
        var secondRequest = CreateRequest(Guid.NewGuid());
        var firstWorkspace = CreateWorkspace(firstRequest.WorkspaceId, new WorkspaceRevision(Guid.NewGuid(), 0));
        firstWorkspace.Setup(candidate => candidate.DisposeAsync())
            .Returns(ValueTask.FromException(new InvalidOperationException("First disposal failed.")));
        var secondWorkspace = CreateWorkspace(secondRequest.WorkspaceId, new WorkspaceRevision(Guid.NewGuid(), 0));
        var firstFactory = CreateFactory(EngineResult<IFormListWorkspace>.Success(firstWorkspace.Object));
        var secondFactory = CreateFactory(EngineResult<IFormListWorkspace>.Success(secondWorkspace.Object));
        (await registry.OpenAsync(firstFactory.Object, firstRequest)).Succeeded.ShouldBeTrue();
        (await registry.OpenAsync(secondFactory.Object, secondRequest)).Succeeded.ShouldBeTrue();

        var exception = await Should.ThrowAsync<AggregateException>(async () =>
            await registry.DisposeAsync());

        exception.InnerExceptions.Count.ShouldBe(1);
        firstWorkspace.Verify(candidate => candidate.DisposeAsync(), Times.Once);
        secondWorkspace.Verify(candidate => candidate.DisposeAsync(), Times.Once);
        registry.ActiveWorkspaceCount.ShouldBe(0);
    }

    /// <summary>Verifies close reservation, same-ID reopen rejection, and shutdown ordering around an active operation.</summary>
    [Fact]
    public async Task CloseAsync_WhenOperationIsActive_ReservesIdAndShutdownAwaitsCleanup()
    {
        var registry = new McpWorkspaceRegistry();
        var firstRequest = CreateRequest(Guid.NewGuid());
        var secondRequest = CreateRequest(Guid.NewGuid());
        var firstWorkspace = CreateWorkspace(firstRequest.WorkspaceId, new WorkspaceRevision(Guid.NewGuid(), 0));
        var secondWorkspace = CreateWorkspace(secondRequest.WorkspaceId, new WorkspaceRevision(Guid.NewGuid(), 0));
        var firstFactory = CreateFactory(EngineResult<IFormListWorkspace>.Success(firstWorkspace.Object));
        var secondFactory = CreateFactory(EngineResult<IFormListWorkspace>.Success(secondWorkspace.Object));
        (await registry.OpenAsync(firstFactory.Object, firstRequest)).Succeeded.ShouldBeTrue();
        (await registry.OpenAsync(secondFactory.Object, secondRequest)).Succeeded.ShouldBeTrue();
        var operationStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var releaseOperation = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        var operationTask = registry.ExecuteAsync(
            firstRequest.WorkspaceId,
            async (workspace, _) =>
            {
                workspace.ShouldBeSameAs(firstWorkspace.Object);
                operationStarted.SetResult();
                await releaseOperation.Task;
                return EngineResult<string>.Success("complete", workspaceId: workspace.WorkspaceId);
            }).AsTask();
        await operationStarted.Task;
        var closeTask = registry.CloseAsync(firstRequest.WorkspaceId).AsTask();

        closeTask.IsCompleted.ShouldBeFalse();
        registry.ActiveWorkspaceCount.ShouldBe(1);
        registry.GetWorkspaceIds().ShouldBe([secondRequest.WorkspaceId]);
        firstWorkspace.Verify(candidate => candidate.DisposeAsync(), Times.Never);
        secondWorkspace.Verify(candidate => candidate.DisposeAsync(), Times.Never);
        var reopenFactory = CreateFactory(EngineResult<IFormListWorkspace>.Success(firstWorkspace.Object));
        var reopenResult = await registry.OpenAsync(reopenFactory.Object, firstRequest);
        reopenResult.Succeeded.ShouldBeFalse();
        reopenResult.Error.ShouldNotBeNull();
        reopenResult.Error.Code.ShouldBe(EngineErrorCode.InvalidRequest);
        reopenFactory.Verify(
            candidate => candidate.OpenAsync(
                It.IsAny<WorkspaceOpenRequest>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
        var shutdownTask = registry.DisposeAsync().AsTask();
        shutdownTask.IsCompleted.ShouldBeFalse();
        secondWorkspace.Verify(candidate => candidate.DisposeAsync(), Times.Once);
        releaseOperation.SetResult();

        (await operationTask).Value.ShouldBe("complete");
        (await closeTask).ShouldBeTrue();
        await shutdownTask;
        firstWorkspace.Verify(candidate => candidate.DisposeAsync(), Times.Once);
        secondWorkspace.Verify(candidate => candidate.DisposeAsync(), Times.Once);
    }

    /// <summary>Verifies that close and shutdown observe the same single plugin disposal failure.</summary>
    [Fact]
    public async Task CloseAsync_WhenPluginDisposalFails_PropagatesSameFailureToShutdown()
    {
        var registry = new McpWorkspaceRegistry();
        var request = CreateRequest();
        var workspace = CreateWorkspace(request.WorkspaceId, new WorkspaceRevision(Guid.NewGuid(), 0));
        var disposalCompletion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        workspace.Setup(candidate => candidate.DisposeAsync())
            .Returns(new ValueTask(disposalCompletion.Task));
        var factory = CreateFactory(EngineResult<IFormListWorkspace>.Success(workspace.Object));
        (await registry.OpenAsync(factory.Object, request)).Succeeded.ShouldBeTrue();

        var closeTask = registry.CloseAsync(request.WorkspaceId).AsTask();
        workspace.Verify(candidate => candidate.DisposeAsync(), Times.Once);
        closeTask.IsCompleted.ShouldBeFalse();
        var shutdownTask = registry.DisposeAsync().AsTask();
        shutdownTask.IsCompleted.ShouldBeFalse();
        var expectedException = new InvalidOperationException("Plugin disposal failed.");

        disposalCompletion.SetException(expectedException);

        var closeException = await Should.ThrowAsync<InvalidOperationException>(async () =>
            await closeTask);
        var shutdownException = await Should.ThrowAsync<AggregateException>(async () =>
            await shutdownTask);
        closeException.ShouldBeSameAs(expectedException);
        shutdownException.InnerExceptions.ShouldHaveSingleItem().ShouldBeSameAs(expectedException);
        workspace.Verify(candidate => candidate.DisposeAsync(), Times.Once);
    }

    /// <summary>Creates a complete request suitable for registry tests whose factory is test-controlled.</summary>
    /// <param name="workspaceId">An optional deterministic workspace identifier.</param>
    /// <returns>A request whose paths are opaque to the fake factory.</returns>
    private static WorkspaceOpenRequest CreateRequest(Guid? workspaceId = null)
    {
        return new WorkspaceOpenRequest(
            workspaceId ?? Guid.NewGuid(),
            SupportedGame.Starfield,
            GameRelease.Starfield,
            "Source.esm",
            ["Source.esm"],
            "Data",
            []);
    }

    /// <summary>Creates an engine workspace with deterministic identity, revision, and disposal behavior.</summary>
    /// <param name="workspaceId">The workspace identifier exposed by the fake.</param>
    /// <param name="revision">The workspace revision exposed by the fake.</param>
    /// <returns>A configured workspace mock.</returns>
    private static Mock<IFormListWorkspace> CreateWorkspace(
        Guid workspaceId,
        WorkspaceRevision revision)
    {
        var workspace = new Mock<IFormListWorkspace>();
        workspace.SetupGet(candidate => candidate.WorkspaceId).Returns(workspaceId);
        workspace.SetupGet(candidate => candidate.Revision).Returns(revision);
        workspace.Setup(candidate => candidate.DisposeAsync()).Returns(ValueTask.CompletedTask);
        return workspace;
    }

    /// <summary>Creates a factory that returns one deterministic engine result.</summary>
    /// <param name="result">The result returned by every factory invocation.</param>
    /// <returns>A configured factory mock.</returns>
    private static Mock<IFormListWorkspaceFactory> CreateFactory(
        EngineResult<IFormListWorkspace> result)
    {
        var factory = new Mock<IFormListWorkspaceFactory>();
        factory.Setup(candidate => candidate.OpenAsync(
                It.IsAny<WorkspaceOpenRequest>(),
                It.IsAny<CancellationToken>()))
            .Returns(ValueTask.FromResult(result));
        return factory;
    }
}
