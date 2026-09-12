using CreationsForge.Core.Engine;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using Moq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Serilog;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Foundation;

/// <summary>Verifies serialized atomic workspace metadata snapshots independently of native record traversal.</summary>
public sealed class FormListWorkspaceStateTests
{
    /// <summary>Verifies output association and baseline cannot be observed as a torn pair.</summary>
    [Fact]
    public void WorkspaceState_WithTornOutputMetadata_RejectsSnapshot()
    {
        var outputPath = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "WorkspaceStateOutput.esp"));
        var output = new OutputAssociation(
            outputPath,
            ModKey.FromNameAndExtension("WorkspaceStateOutput.esp"),
            LocalizedOutputMode.Embedded,
            OutputMasterStyle.Full);

        Should.Throw<ArgumentException>(() => new WorkspaceState(
            SupportedGame.Starfield,
            GameRelease.Starfield,
            output,
            null,
            new OutputSynchronizationState(OutputSynchronizationStatus.Ready, null),
            new WorkspaceRevision(Guid.NewGuid(), 0)));
    }

    /// <summary>Verifies the initial snapshot carries the exact game, release, synchronization, and revision without output metadata.</summary>
    [Fact]
    public async Task ReadStateAsync_BeforeOutputSelection_ReturnsAtomicOpenState()
    {
        await using var fixture = CreateFixture();

        var result = await fixture.Workspace.ReadStateAsync();

        result.Succeeded.ShouldBeTrue();
        result.WorkspaceId.ShouldBe(fixture.Workspace.WorkspaceId);
        result.BaseRevision.ShouldBe(fixture.Workspace.Revision);
        result.ResultRevision.ShouldBe(fixture.Workspace.Revision);
        result.Value!.Game.ShouldBe(SupportedGame.Starfield);
        result.Value.Release.ShouldBe(GameRelease.Starfield);
        result.Value.Output.ShouldBeNull();
        result.Value.OutputBaseline.ShouldBeNull();
        result.Value.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.Ready);
        result.Value.Revision.ShouldBe(fixture.Workspace.Revision);
    }

    /// <summary>Verifies selected output association, complete baseline, and resulting revision are observed together.</summary>
    [Fact]
    public async Task ReadStateAsync_AfterOutputSelection_ReturnsMatchingOutputAndRevision()
    {
        await using var fixture = CreateFixture();
        var outputPath = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "WorkspaceStateOutput.esp"));
        var output = new OutputAssociation(
            outputPath,
            ModKey.FromNameAndExtension("WorkspaceStateOutput.esp"),
            LocalizedOutputMode.Embedded,
            OutputMasterStyle.Full);
        var baseline = new OutputArtifactSetBaseline(
            Guid.NewGuid(),
            new[]
            {
                new NativeArtifactAssociation(
                    outputPath,
                    NativeArtifactRole.Plugin,
                    null,
                    new NativeArtifactFingerprint(false, 0, null))
            });
        var nativeOutput = new Mock<INativeOutputState>();
        nativeOutput.Setup(candidate => candidate.DisposeAsync()).Returns(ValueTask.CompletedTask);
        fixture.Adapter.Setup(candidate => candidate.OpenOutputAsync(
                fixture.Sources.Object,
                It.IsAny<SelectOutputRequest>(),
                It.IsAny<CancellationToken>()))
            .Returns(ValueTask.FromResult(EngineResult<NativeOutputOpenResult>.Success(
                new NativeOutputOpenResult(nativeOutput.Object, output, baseline))));
        var selection = await fixture.Workspace.SelectOutputAsync(new SelectOutputRequest(
            Guid.NewGuid(),
            fixture.Workspace.Revision,
            OutputSelectionMode.CreateNew,
            output));

        var result = await fixture.Workspace.ReadStateAsync();

        selection.Succeeded.ShouldBeTrue();
        result.Succeeded.ShouldBeTrue();
        result.Value!.Output.ShouldBeSameAs(output);
        result.Value.OutputBaseline.ShouldBeSameAs(baseline);
        result.Value.Revision.ShouldBe(selection.Value!.Revision);
        result.Value.Revision.ShouldBe(fixture.Workspace.Revision);
    }

    /// <summary>Verifies state reads preserve the standard typed disposed-workspace failure.</summary>
    [Fact]
    public async Task ReadStateAsync_AfterDisposal_ReturnsWorkspaceDisposed()
    {
        var fixture = CreateFixture();
        await fixture.Workspace.DisposeAsync();

        var result = await fixture.Workspace.ReadStateAsync();

        result.Succeeded.ShouldBeFalse();
        result.Error!.Code.ShouldBe(EngineErrorCode.WorkspaceDisposed);
        result.ResultRevision.ShouldBe(fixture.Workspace.Revision);
    }

    /// <summary>Creates one directly constructed workspace with deterministic source, lease, and admission infrastructure.</summary>
    /// <returns>The disposable workspace fixture.</returns>
    private static WorkspaceFixture CreateFixture()
    {
        var workspaceId = Guid.NewGuid();
        var baselineId = Guid.NewGuid();
        var sourcePath = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "WorkspaceStateSource.esm"));
        var sources = new Mock<INativeSourceSet>();
        var sourceBaseline = TestWorkspaceInfrastructure.ConfigureSourceBaseline(
            sources.Object,
            baselineId,
            sourcePath);
        sources.Setup(candidate => candidate.DisposeAsync()).Returns(ValueTask.CompletedTask);
        var adapter = new Mock<IFormListGameAdapter>();
        adapter.SetupGet(candidate => candidate.Game).Returns(SupportedGame.Starfield);
        var saveCoordinator = new Mock<IWorkspaceSaveCoordinator>();
        TestWorkspaceInfrastructure.ConfigureReadyAdmission(saveCoordinator.Object);
        var request = new WorkspaceOpenRequest(
            workspaceId,
            SupportedGame.Starfield,
            GameRelease.Starfield,
            sourcePath,
            new[] { sourcePath },
            Path.GetDirectoryName(sourcePath)!,
            Array.Empty<string>());
        var sourceOpenResult = new NativeSourceOpenResult(
            sources.Object,
            baselineId,
            sourceBaseline.Artifacts);
        var workspace = new FormListWorkspace(
            request,
            adapter.Object,
            sourceOpenResult,
            saveCoordinator.Object,
            TestWorkspaceInfrastructure.CreateLeaseProvider(),
            new Mock<ILogger>().Object);
        return new WorkspaceFixture(workspace, adapter, sources);
    }

    /// <summary>Owns one directly constructed workspace and the mocks needed by state tests.</summary>
    private sealed class WorkspaceFixture : IAsyncDisposable
    {
        /// <summary>Initializes the disposable workspace fixture.</summary>
        /// <param name="workspace">The directly constructed workspace.</param>
        /// <param name="adapter">The game adapter mock.</param>
        /// <param name="sources">The source lifetime mock.</param>
        internal WorkspaceFixture(
            FormListWorkspace workspace,
            Mock<IFormListGameAdapter> adapter,
            Mock<INativeSourceSet> sources)
        {
            Workspace = workspace;
            Adapter = adapter;
            Sources = sources;
        }

        /// <summary>Gets the directly constructed workspace.</summary>
        internal FormListWorkspace Workspace { get; }

        /// <summary>Gets the game adapter mock.</summary>
        internal Mock<IFormListGameAdapter> Adapter { get; }

        /// <summary>Gets the source lifetime mock.</summary>
        internal Mock<INativeSourceSet> Sources { get; }

        /// <summary>Disposes the owned workspace.</summary>
        /// <returns>A task representing asynchronous disposal.</returns>
        public ValueTask DisposeAsync()
        {
            return Workspace.DisposeAsync();
        }
    }
}
