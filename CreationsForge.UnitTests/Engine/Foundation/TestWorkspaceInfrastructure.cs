using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.PluginInputs;
using Moq;

namespace CreationsForge.UnitTests.Engine.Foundation;

/// <summary>Provides deterministic default save-admission and output-lease infrastructure for workspace tests.</summary>
internal static class TestWorkspaceInfrastructure
{
    /// <summary>Configures a mocked save coordinator to admit outputs without unresolved journal metadata.</summary>
    /// <param name="saveCoordinator">The mocked coordinator used by a workspace fixture.</param>
    internal static void ConfigureReadyAdmission(IWorkspaceSaveCoordinator saveCoordinator)
    {
        Mock.Get(saveCoordinator)
            .Setup(candidate => candidate.InspectOutputAdmissionAsync(
                It.IsAny<IOutputDirectoryLease>(),
                It.IsAny<OutputAdmissionRequest>(),
                It.IsAny<CancellationToken>()))
            .Returns(ValueTask.FromResult(EngineResult<OutputAdmissionResult>.Success(
                new OutputAdmissionResult(OutputSynchronizationStatus.Ready, null))));
    }

    /// <summary>Creates a provider that returns a fresh independently disposable lease for every requested output directory.</summary>
    /// <returns>The deterministic output-directory lease provider.</returns>
    internal static IOutputDirectoryLeaseProvider CreateLeaseProvider()
    {
        var provider = new Mock<IOutputDirectoryLeaseProvider>();
        provider.Setup(candidate => candidate.AcquireAsync(
                It.IsAny<string>(),
                OutputDirectoryLeaseMode.CreateOrOpen,
                It.IsAny<CancellationToken>()))
            .Returns<string, OutputDirectoryLeaseMode, CancellationToken>((path, _, _) =>
            {
                var lease = new Mock<IOutputDirectoryLease>();
                lease.SetupGet(candidate => candidate.OutputDirectoryPath).Returns(path);
                lease.Setup(candidate => candidate.DisposeAsync()).Returns(ValueTask.CompletedTask);
                return ValueTask.FromResult(EngineResult<OutputDirectoryLeaseAcquisition>.Success(
                    new OutputDirectoryLeaseAcquisition(
                        OutputDirectoryLeaseAcquisitionStatus.Acquired,
                        lease.Object)));
            });
        return provider.Object;
    }

    /// <summary>Configures a mocked plugin source set with a complete immutable and re-verifiable baseline.</summary>
    /// <param name="sources">The mocked plugin source set.</param>
    /// <param name="baselineId">The deterministic source baseline identifier returned by the adapter.</param>
    /// <param name="sourcePluginPath">The canonical synthetic source plugin path.</param>
    /// <returns>The configured source baseline.</returns>
    internal static PluginSourceInputBaseline ConfigureSourceBaseline(
        IPluginSourceSet sources,
        Guid baselineId,
        string sourcePluginPath)
    {
        var baseline = new PluginSourceInputBaseline(
            baselineId,
            Array.AsReadOnly(new[]
            {
                new PluginArtifactAssociation(
                    sourcePluginPath,
                    PluginArtifactRole.Plugin,
                    null,
                    new PluginArtifactFingerprint(true, 0, new string('A', 64)),
                    new ArtifactFileIdentity("test", "volume", "source", 1)),
            }));
        var sourceMock = Mock.Get(sources);
        sourceMock.SetupGet(candidate => candidate.Baseline).Returns(baseline);
        sourceMock.Setup(candidate => candidate.VerifyUnchangedAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(EngineResult<PluginSourceInputBaseline>.Success(baseline)));
        return baseline;
    }
}
