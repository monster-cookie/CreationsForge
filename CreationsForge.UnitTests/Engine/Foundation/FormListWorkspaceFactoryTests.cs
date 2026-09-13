using Autofac;
using CreationsForge.Core.Engine;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using Moq;
using Mutagen.Bethesda;
using Serilog;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Foundation;

/// <summary>
/// Verifies explicit workspace input validation, native lifetime ownership, and isolated engine composition.
/// </summary>
public sealed class FormListWorkspaceFactoryTests
{
    /// <summary>Verifies that a missing source plugin is rejected before native acquisition.</summary>
    [Fact]
    public async Task OpenAsync_WhenSourcePluginIsMissing_ReturnsInvalidRequestWithoutOpeningAdapter()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var dataDirectory = tempDirectory.CreateSubdirectory("Data");
            var stringDirectory = tempDirectory.CreateSubdirectory("Strings");
            var missingSourcePath = Path.Combine(dataDirectory.FullName, "Missing.esm");
            var request = new WorkspaceOpenRequest(
                Guid.NewGuid(),
                SupportedGame.Starfield,
                GameRelease.Starfield,
                missingSourcePath,
                [missingSourcePath],
                dataDirectory.FullName,
                [stringDirectory.FullName]);
            var adapter = CreateSupportingAdapter();
            var factory = CreateFactory(adapter.Object);

            var result = await factory.OpenAsync(request);

            result.Succeeded.ShouldBeFalse();
            result.Error.ShouldNotBeNull();
            result.Error.Code.ShouldBe(EngineErrorCode.InvalidRequest);
            adapter.Verify(
                candidate => candidate.OpenSourcesAsync(
                    It.IsAny<WorkspaceOpenRequest>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    /// <summary>Verifies that the source must occur in the explicit load order by canonical path.</summary>
    [Fact]
    public async Task OpenAsync_WhenSourceIsAbsentFromLoadOrder_ReturnsInvalidRequest()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var request = CreateValidRequest(tempDirectory);
            var sourceAbsentRequest = new WorkspaceOpenRequest(
                request.WorkspaceId,
                request.Game,
                request.Release,
                request.SourcePluginPath,
                [request.LoadOrderPluginPaths[0]],
                request.DataDirectoryPath,
                request.StringDirectoryPaths);
            var adapter = CreateSupportingAdapter();
            var factory = CreateFactory(adapter.Object);

            var result = await factory.OpenAsync(sourceAbsentRequest);

            result.Succeeded.ShouldBeFalse();
            result.Error.ShouldNotBeNull();
            result.Error.Code.ShouldBe(EngineErrorCode.InvalidRequest);
            adapter.Verify(
                candidate => candidate.OpenSourcesAsync(
                    It.IsAny<WorkspaceOpenRequest>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    /// <summary>Verifies that equivalent path spellings cannot repeat one canonical load-order entry.</summary>
    [Fact]
    public async Task OpenAsync_WhenLoadOrderRepeatsCanonicalPath_ReturnsInvalidRequest()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var request = CreateValidRequest(tempDirectory);
            var repeatedSourcePath = Path.Combine(
                Path.GetDirectoryName(request.SourcePluginPath)!,
                ".",
                Path.GetFileName(request.SourcePluginPath));
            var repeatedPathRequest = new WorkspaceOpenRequest(
                request.WorkspaceId,
                request.Game,
                request.Release,
                request.SourcePluginPath,
                [.. request.LoadOrderPluginPaths, repeatedSourcePath],
                request.DataDirectoryPath,
                request.StringDirectoryPaths);
            var adapter = CreateSupportingAdapter();
            var factory = CreateFactory(adapter.Object);

            var result = await factory.OpenAsync(repeatedPathRequest);

            result.Succeeded.ShouldBeFalse();
            result.Error.ShouldNotBeNull();
            result.Error.Code.ShouldBe(EngineErrorCode.InvalidRequest);
            adapter.Verify(
                candidate => candidate.OpenSourcesAsync(
                    It.IsAny<WorkspaceOpenRequest>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    /// <summary>Verifies that Linux path casing cannot make a different file satisfy source membership.</summary>
    [Fact]
    public async Task OpenAsync_OnLinux_WhenOnlyCaseVariantPathIsInLoadOrder_ReturnsInvalidRequest()
    {
        Assert.SkipUnless(
            OperatingSystem.IsLinux(),
            "This regression requires a case-sensitive Linux file system.");

        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var dataDirectory = tempDirectory.CreateSubdirectory("Data");
            var stringDirectory = tempDirectory.CreateSubdirectory("Strings");
            var upperCaseDirectory = tempDirectory.CreateSubdirectory("SourceDirectory");
            var lowerCaseDirectory = tempDirectory.CreateSubdirectory("sourcedirectory");
            var sourcePath = Path.Combine(upperCaseDirectory.FullName, "Source.esm");
            var caseVariantPath = Path.Combine(lowerCaseDirectory.FullName, "Source.esm");
            await File.WriteAllBytesAsync(sourcePath, []);
            await File.WriteAllBytesAsync(caseVariantPath, []);
            var request = new WorkspaceOpenRequest(
                Guid.NewGuid(),
                SupportedGame.Starfield,
                GameRelease.Starfield,
                sourcePath,
                [caseVariantPath],
                dataDirectory.FullName,
                [stringDirectory.FullName]);
            var adapter = CreateSupportingAdapter();
            var factory = CreateFactory(adapter.Object);

            var result = await factory.OpenAsync(request);

            result.Succeeded.ShouldBeFalse();
            result.Error.ShouldNotBeNull();
            result.Error.Code.ShouldBe(EngineErrorCode.InvalidRequest);
            adapter.Verify(
                candidate => candidate.OpenSourcesAsync(
                    It.IsAny<WorkspaceOpenRequest>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    /// <summary>Verifies that two paths cannot represent the same native plugin identity.</summary>
    [Fact]
    public async Task OpenAsync_WhenLoadOrderRepeatsModKey_ReturnsInvalidRequest()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var request = CreateValidRequest(tempDirectory);
            var alternateDirectory = tempDirectory.CreateSubdirectory("Alternate");
            var repeatedModKeyPath = Path.Combine(alternateDirectory.FullName, Path.GetFileName(request.SourcePluginPath));
            await File.WriteAllBytesAsync(repeatedModKeyPath, []);
            var repeatedModKeyRequest = new WorkspaceOpenRequest(
                request.WorkspaceId,
                request.Game,
                request.Release,
                request.SourcePluginPath,
                [.. request.LoadOrderPluginPaths, repeatedModKeyPath],
                request.DataDirectoryPath,
                request.StringDirectoryPaths);
            var adapter = CreateSupportingAdapter();
            var factory = CreateFactory(adapter.Object);

            var result = await factory.OpenAsync(repeatedModKeyRequest);

            result.Succeeded.ShouldBeFalse();
            result.Error.ShouldNotBeNull();
            result.Error.Code.ShouldBe(EngineErrorCode.InvalidRequest);
            adapter.Verify(
                candidate => candidate.OpenSourcesAsync(
                    It.IsAny<WorkspaceOpenRequest>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    /// <summary>Verifies that adapter release support is required for the requested game and release pair.</summary>
    [Fact]
    public async Task OpenAsync_WhenGameReleaseIsUnsupported_ReturnsTypedFailure()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var request = CreateValidRequest(tempDirectory);
            var adapter = new Mock<IFormListGameAdapter>();
            adapter.SetupGet(candidate => candidate.Game).Returns(SupportedGame.Starfield);
            adapter.Setup(candidate => candidate.SupportsRelease(GameRelease.Starfield)).Returns(false);
            var factory = CreateFactory(adapter.Object);

            var result = await factory.OpenAsync(request);

            result.Succeeded.ShouldBeFalse();
            result.Error.ShouldNotBeNull();
            result.Error.Code.ShouldBe(EngineErrorCode.UnsupportedGameRelease);
            adapter.Verify(candidate => candidate.SupportsRelease(GameRelease.Starfield), Times.Once);
            adapter.Verify(
                candidate => candidate.OpenSourcesAsync(
                    It.IsAny<WorkspaceOpenRequest>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    /// <summary>Verifies that an adapter's stable native-open failure is preserved.</summary>
    [Fact]
    public async Task OpenAsync_WhenAdapterReturnsFailure_PreservesFailure()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var request = CreateValidRequest(tempDirectory);
            var expectedError = new EngineError(
                EngineErrorCode.SourceOpenFailed,
                "The native test source could not be opened.");
            var adapter = CreateSupportingAdapter();
            adapter.Setup(candidate => candidate.OpenSourcesAsync(
                    It.IsAny<WorkspaceOpenRequest>(),
                    It.IsAny<CancellationToken>()))
                .Returns(ValueTask.FromResult(
                    EngineResult<NativeSourceOpenResult>.Failure(expectedError)));
            var factory = CreateFactory(adapter.Object);

            var result = await factory.OpenAsync(request);

            result.Succeeded.ShouldBeFalse();
            result.Error.ShouldBeSameAs(expectedError);
            result.WorkspaceId.ShouldBe(request.WorkspaceId);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    /// <summary>Verifies that cancellation already requested is observed before native acquisition.</summary>
    [Fact]
    public async Task OpenAsync_WhenCancelledBeforeAcquisition_DoesNotOpenAdapter()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var request = CreateValidRequest(tempDirectory);
            var adapter = CreateSupportingAdapter();
            var factory = CreateFactory(adapter.Object);
            using var cancellationSource = new CancellationTokenSource();
            cancellationSource.Cancel();

            await Should.ThrowAsync<OperationCanceledException>(async () =>
                await factory.OpenAsync(request, cancellationSource.Token));

            adapter.Verify(
                candidate => candidate.OpenSourcesAsync(
                    It.IsAny<WorkspaceOpenRequest>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    /// <summary>Verifies that cancellation observed after acquisition disposes the acquired native sources.</summary>
    [Fact]
    public async Task OpenAsync_WhenCancelledAfterAcquisition_DisposesNativeSources()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var request = CreateValidRequest(tempDirectory);
            var sourceSet = CreateSourceSet();
            var sourceOpenResult = CreateSourceOpenResult(sourceSet.Object);
            using var cancellationSource = new CancellationTokenSource();
            var adapter = CreateSupportingAdapter();
            adapter.Setup(candidate => candidate.OpenSourcesAsync(
                    It.IsAny<WorkspaceOpenRequest>(),
                    cancellationSource.Token))
                .Returns(() =>
                {
                    cancellationSource.Cancel();
                    return ValueTask.FromResult(EngineResult<NativeSourceOpenResult>.Success(sourceOpenResult));
                });
            var factory = CreateFactory(adapter.Object);

            await Should.ThrowAsync<OperationCanceledException>(async () =>
                await factory.OpenAsync(request, cancellationSource.Token));

            sourceSet.Verify(source => source.DisposeAsync(), Times.Once);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    /// <summary>Verifies that a progress observer failure after workspace construction disposes its native lifetime.</summary>
    [Fact]
    public async Task OpenAsync_WhenCompletedProgressObserverThrows_DisposesNativeSources()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var reportedStages = new List<WorkspaceOpenStage>();
            var progress = new Mock<IProgress<WorkspaceOpenProgress>>();
            progress.Setup(observer => observer.Report(It.IsAny<WorkspaceOpenProgress>()))
                .Callback<WorkspaceOpenProgress>(update =>
                {
                    reportedStages.Add(update.Stage);
                    if (update.Stage == WorkspaceOpenStage.Completed)
                    {
                        throw new InvalidOperationException("The test progress observer failed.");
                    }
                });
            var request = CreateValidRequest(tempDirectory, progress: progress.Object);
            var sourceSet = CreateSourceSet();
            var adapter = CreateSupportingAdapter();
            adapter.Setup(candidate => candidate.OpenSourcesAsync(
                    It.IsAny<WorkspaceOpenRequest>(),
                    It.IsAny<CancellationToken>()))
                .Returns(ValueTask.FromResult(EngineResult<NativeSourceOpenResult>.Success(
                    CreateSourceOpenResult(sourceSet.Object))));
            var factory = CreateFactory(adapter.Object);

            var result = await factory.OpenAsync(request);

            result.Succeeded.ShouldBeFalse();
            result.Error.ShouldNotBeNull();
            result.Error.Code.ShouldBe(EngineErrorCode.UnexpectedFailure);
            reportedStages.ShouldBe([
                WorkspaceOpenStage.Validating,
                WorkspaceOpenStage.SelectingAdapter,
                WorkspaceOpenStage.PreparingInputs,
                WorkspaceOpenStage.Completed]);
            sourceSet.Verify(source => source.DisposeAsync(), Times.Once);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    /// <summary>Verifies canonical path forwarding, explicit load-order preservation, and successful progress.</summary>
    [Fact]
    public async Task OpenAsync_WhenInputsAreValid_ForwardsCanonicalOrderedSnapshot()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var stages = new List<WorkspaceOpenStage>();
            var progress = new Mock<IProgress<WorkspaceOpenProgress>>();
            progress.Setup(observer => observer.Report(It.IsAny<WorkspaceOpenProgress>()))
                .Callback<WorkspaceOpenProgress>(update => stages.Add(update.Stage));
            var baseRequest = CreateValidRequest(tempDirectory);
            var sourceInput = Path.Combine(
                Path.GetDirectoryName(baseRequest.SourcePluginPath)!,
                ".",
                Path.GetFileName(baseRequest.SourcePluginPath));
            var loadOrderInputs = baseRequest.LoadOrderPluginPaths
                .Select(path => Path.Combine(Path.GetDirectoryName(path)!, ".", Path.GetFileName(path)))
                .ToList();
            var request = new WorkspaceOpenRequest(
                baseRequest.WorkspaceId,
                baseRequest.Game,
                baseRequest.Release,
                sourceInput,
                loadOrderInputs,
                Path.Combine(baseRequest.DataDirectoryPath, "."),
                [baseRequest.StringDirectoryPaths[0] + Path.DirectorySeparatorChar],
                progress.Object);
            WorkspaceOpenRequest? forwardedRequest = null;
            var sourceSet = CreateSourceSet();
            var adapter = CreateSupportingAdapter();
            adapter.Setup(candidate => candidate.OpenSourcesAsync(
                    It.IsAny<WorkspaceOpenRequest>(),
                    It.IsAny<CancellationToken>()))
                .Callback<WorkspaceOpenRequest, CancellationToken>((canonicalRequest, _) =>
                    forwardedRequest = canonicalRequest)
                .Returns(ValueTask.FromResult(EngineResult<NativeSourceOpenResult>.Success(
                    CreateSourceOpenResult(sourceSet.Object))));
            var factory = CreateFactory(adapter.Object);

            var result = await factory.OpenAsync(request);

            result.Succeeded.ShouldBeTrue();
            result.Value.ShouldNotBeNull();
            result.Value.Revision.BaselineId.ShouldBe(new Guid("6195dcd8-7cab-49ba-b25a-56af1a86a4e7"));
            result.Value.Revision.Sequence.ShouldBe(0UL);
            forwardedRequest.ShouldNotBeNull();
            forwardedRequest.ShouldNotBeSameAs(request);
            forwardedRequest.SourcePluginPath.ShouldBe(Path.GetFullPath(sourceInput));
            forwardedRequest.LoadOrderPluginPaths.ShouldBe(
                loadOrderInputs.Select(Path.GetFullPath).ToArray());
            forwardedRequest.DataDirectoryPath.ShouldBe(
                Path.TrimEndingDirectorySeparator(Path.GetFullPath(request.DataDirectoryPath)));
            forwardedRequest.StringDirectoryPaths.ShouldBe([
                Path.TrimEndingDirectorySeparator(Path.GetFullPath(request.StringDirectoryPaths[0]))]);
            stages.ShouldBe([
                WorkspaceOpenStage.Validating,
                WorkspaceOpenStage.SelectingAdapter,
                WorkspaceOpenStage.PreparingInputs,
                WorkspaceOpenStage.Completed]);

            await result.Value.DisposeAsync();
            sourceSet.Verify(source => source.DisposeAsync(), Times.Once);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    /// <summary>Verifies that the isolated engine module creates transient factories and independent workspaces.</summary>
    [Fact]
    public async Task EngineModule_OpensTwoIndependentWorkspaceLifetimes()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var firstRequest = CreateValidRequest(tempDirectory, workspaceId: Guid.NewGuid());
            var secondRequest = CreateValidRequest(tempDirectory, workspaceId: Guid.NewGuid());
            var firstSourceSet = CreateSourceSet();
            var secondSourceSet = CreateSourceSet();
            var sourceResults = new Queue<NativeSourceOpenResult>([
                CreateSourceOpenResult(firstSourceSet.Object),
                CreateSourceOpenResult(secondSourceSet.Object)]);
            var adapter = CreateSupportingAdapter();
            adapter.Setup(candidate => candidate.OpenSourcesAsync(
                    It.IsAny<WorkspaceOpenRequest>(),
                    It.IsAny<CancellationToken>()))
                .Returns(() => ValueTask.FromResult(EngineResult<NativeSourceOpenResult>.Success(
                    sourceResults.Dequeue())));
            var saveCoordinator = new Mock<IWorkspaceSaveCoordinator>();
            var logger = new Mock<ILogger>();
            var builder = new ContainerBuilder();
            builder.RegisterInstance(adapter.Object).As<IFormListGameAdapter>();
            builder.RegisterInstance(saveCoordinator.Object).As<IWorkspaceSaveCoordinator>();
            builder.RegisterInstance(new Mock<IOutputDirectoryLeaseProvider>().Object).As<IOutputDirectoryLeaseProvider>();
            builder.RegisterInstance(logger.Object).As<ILogger>();
            builder.RegisterModule<EngineModule>();
            using var container = builder.Build();

            var firstFactory = container.Resolve<IFormListWorkspaceFactory>();
            var secondFactory = container.Resolve<IFormListWorkspaceFactory>();
            var firstResult = await firstFactory.OpenAsync(firstRequest);
            var secondResult = await secondFactory.OpenAsync(secondRequest);

            firstFactory.ShouldNotBeSameAs(secondFactory);
            firstResult.Succeeded.ShouldBeTrue();
            secondResult.Succeeded.ShouldBeTrue();
            firstResult.Value.ShouldNotBeNull();
            secondResult.Value.ShouldNotBeNull();
            firstResult.Value.ShouldNotBeSameAs(secondResult.Value);
            firstResult.Value.WorkspaceId.ShouldBe(firstRequest.WorkspaceId);
            secondResult.Value.WorkspaceId.ShouldBe(secondRequest.WorkspaceId);

            await firstResult.Value.DisposeAsync();
            firstSourceSet.Verify(source => source.DisposeAsync(), Times.Once);
            secondSourceSet.Verify(source => source.DisposeAsync(), Times.Never);

            await secondResult.Value.DisposeAsync();
            secondSourceSet.Verify(source => source.DisposeAsync(), Times.Once);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    /// <summary>Creates a complete explicit request backed by disposable test files.</summary>
    /// <param name="tempDirectory">The disposable root directory owned by the calling test.</param>
    /// <param name="workspaceId">An optional deterministic workspace identifier.</param>
    /// <param name="progress">An optional progress observer.</param>
    /// <returns>A valid Starfield workspace-open request.</returns>
    private static WorkspaceOpenRequest CreateValidRequest(
        DirectoryInfo tempDirectory,
        Guid? workspaceId = null,
        IProgress<WorkspaceOpenProgress>? progress = null)
    {
        var dataDirectory = tempDirectory.CreateSubdirectory("Data");
        var stringDirectory = tempDirectory.CreateSubdirectory("Strings");
        var masterPath = Path.Combine(dataDirectory.FullName, "Master.esm");
        var sourcePath = Path.Combine(dataDirectory.FullName, "Source.esp");
        File.WriteAllBytes(masterPath, []);
        File.WriteAllBytes(sourcePath, []);
        return new WorkspaceOpenRequest(
            workspaceId ?? Guid.NewGuid(),
            SupportedGame.Starfield,
            GameRelease.Starfield,
            sourcePath,
            [masterPath, sourcePath],
            dataDirectory.FullName,
            [stringDirectory.FullName],
            progress);
    }

    /// <summary>Creates a Starfield adapter mock that supports the exact test release.</summary>
    /// <returns>A configured loose adapter mock.</returns>
    private static Mock<IFormListGameAdapter> CreateSupportingAdapter()
    {
        var adapter = new Mock<IFormListGameAdapter>();
        adapter.SetupGet(candidate => candidate.Game).Returns(SupportedGame.Starfield);
        adapter.Setup(candidate => candidate.SupportsRelease(GameRelease.Starfield)).Returns(true);
        return adapter;
    }

    /// <summary>Creates a factory with externally supplied test infrastructure.</summary>
    /// <param name="adapter">The single adapter available to the factory.</param>
    /// <returns>A workspace factory isolated from legacy Core composition.</returns>
    private static FormListWorkspaceFactory CreateFactory(IFormListGameAdapter adapter)
    {
        var saveCoordinator = Mock.Of<IWorkspaceSaveCoordinator>();
        TestWorkspaceInfrastructure.ConfigureReadyAdmission(saveCoordinator);
        return new FormListWorkspaceFactory(
            [adapter],
            saveCoordinator,
            TestWorkspaceInfrastructure.CreateLeaseProvider(),
            Mock.Of<ILogger>());
    }

    /// <summary>Creates an independently verifiable native source lifetime.</summary>
    /// <returns>A loose native source mock with an awaitable disposal operation.</returns>
    private static Mock<INativeSourceSet> CreateSourceSet()
    {
        var sourceSet = new Mock<INativeSourceSet>();
        sourceSet.Setup(source => source.DisposeAsync()).Returns(ValueTask.CompletedTask);
        return sourceSet;
    }

    /// <summary>Creates a successful native source result with a deterministic adapter baseline.</summary>
    /// <param name="sourceSet">The independently owned native source lifetime.</param>
    /// <returns>A source result suitable for constructing a workspace.</returns>
    private static NativeSourceOpenResult CreateSourceOpenResult(INativeSourceSet sourceSet)
    {
        var baselineId = new Guid("6195dcd8-7cab-49ba-b25a-56af1a86a4e7");
        TestWorkspaceInfrastructure.ConfigureSourceBaseline(
            sourceSet,
            baselineId,
            Path.Combine(Path.GetTempPath(), "Source.esp"));
        return new NativeSourceOpenResult(
            sourceSet,
            baselineId,
            Array.Empty<NativeArtifactAssociation>());
    }
}
