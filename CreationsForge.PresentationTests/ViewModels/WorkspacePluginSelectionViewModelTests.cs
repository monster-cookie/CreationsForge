using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using CreationsForge.PresentationTests.Support;
using CreationsForge.Services;
using CreationsForge.ViewModels;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Serilog;
using Shouldly;

namespace CreationsForge.PresentationTests.ViewModels;

/// <summary>Verifies the product plugin-selection workflow translates installed plugins into guarded engine requests.</summary>
public sealed class WorkspacePluginSelectionViewModelTests
{
    /// <summary>Verifies discovery preserves load-order display order and applies filename filtering locally.</summary>
    [Fact]
    public async Task RefreshPluginsAsync_WithCatalog_PublishesAndFiltersInstalledPlugins()
    {
        var root = CreateTestRoot();
        var discovery = new FakePluginDiscoveryService
        {
            Result = EngineResult<PluginCatalog>.Success(CreateCatalog(root))
        };
        await using var coordinator = CreateCoordinator(CreateSuccessfulWorkspace());
        var viewModel = CreateViewModel(coordinator, new FakeWorkspacePathPicker(), discovery);

        await viewModel.RefreshPluginsAsync();

        discovery.RequestedGames.ShouldBe([SupportedGame.Starfield]);
        viewModel.PluginRows.Select(row => row.FileName).ShouldBe(["Base.esm", "Editable.esp"]);
        viewModel.DetectedDataDirectoryText.ShouldBe(root);
        var editable = viewModel.PluginRows.Single(row => row.FileName == "Editable.esp");
        editable.FileTypeText.ShouldBe("ESP");
        editable.MasterStyleText.ShouldBe("Small");
        editable.PluginTypeText.ShouldBe("ESP");
        editable.ParentMastersText.ShouldBe("Base.esm");
        editable.AuthorText.ShouldBe("Venworks");
        editable.DescriptionText.ShouldBe("Editable test plugin");
        editable.AvailabilityText.ShouldBe("Available");
        viewModel.SelectedPlugin = viewModel.PluginRows.Single(row => row.FileName == "Base.esm");
        viewModel.CanOpenSelectedPluginReadOnly.ShouldBeTrue();
        viewModel.CanOpenSelectedPlugin.ShouldBeFalse();
        viewModel.PluginSearchText = "edit";
        viewModel.PluginRows.Select(row => row.FileName).ShouldBe(["Editable.esp"]);
    }

    /// <summary>Verifies a newer game discovery cancels and supersedes an older result without publishing stale rows.</summary>
    [Fact]
    public async Task RefreshPluginsAsync_WhenGameChanges_PublishesOnlyNewestCatalog()
    {
        var firstStarted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var secondRoot = CreateTestRoot();
        var discovery = new FakePluginDiscoveryService
        {
            Handler = async (game, token) =>
            {
                if (game == SupportedGame.Starfield)
                {
                    firstStarted.SetResult(true);
                    await Task.Delay(Timeout.InfiniteTimeSpan, token);
                }

                return EngineResult<PluginCatalog>.Success(CreateCatalog(secondRoot));
            }
        };
        await using var coordinator = CreateCoordinator(CreateSuccessfulWorkspace());
        var viewModel = CreateViewModel(coordinator, new FakeWorkspacePathPicker(), discovery);

        var first = viewModel.RefreshPluginsAsync();
        await firstStarted.Task;
        viewModel.SelectedGame = viewModel.Games.Single(option => option.Game == SupportedGame.Fallout4);
        var second = viewModel.RefreshPluginsAsync();
        await Task.WhenAll(first, second);

        discovery.RequestedGames.ShouldBe([SupportedGame.Starfield, SupportedGame.Fallout4]);
        viewModel.DetectedDataDirectoryText.ShouldBe(secondRoot);
        viewModel.PluginRows.Count.ShouldBe(2);
        viewModel.StatusText.ShouldBe("2 installed plugin(s) found.");
        viewModel.IsBusy.ShouldBeFalse();
    }

    /// <summary>Verifies opening a selected plugin hides engine setup while preserving exact dependency and header metadata.</summary>
    [Fact]
    public async Task OpenSelectedPluginAsync_WithEditableEntry_UsesDeclaredMastersAsReadOnlySources()
    {
        var root = CreateTestRoot();
        var workspace = CreateSuccessfulWorkspace();
        var factory = CreateFactory(workspace);
        await using var coordinator = CreateCoordinator(factory);
        var discovery = new FakePluginDiscoveryService
        {
            Result = EngineResult<PluginCatalog>.Success(CreateCatalog(root))
        };
        var viewModel = CreateViewModel(coordinator, new FakeWorkspacePathPicker(), discovery);
        await viewModel.RefreshPluginsAsync();
        viewModel.SelectedPlugin = viewModel.PluginRows.Single(row => row.FileName == "Editable.esp");

        var opened = await viewModel.OpenSelectedPluginAsync();

        opened.ShouldBeTrue();
        factory.Requests.Single().SourcePluginPath.ShouldBe(Path.Combine(root, "Base.esm"));
        factory.Requests.Single().LoadOrderPluginPaths.ShouldBe([Path.Combine(root, "Base.esm")]);
        factory.Requests.Single().DataDirectoryPath.ShouldBe(root);
        factory.Requests.Single().StringDirectoryPaths.ShouldBe([root]);
        workspace.LastSelectOutputRequest.ShouldNotBeNull();
        workspace.LastSelectOutputRequest.Mode.ShouldBe(OutputSelectionMode.OpenExisting);
        workspace.LastSelectOutputRequest.Output.PluginPath.ShouldBe(Path.Combine(root, "Editable.esp"));
        workspace.LastSelectOutputRequest.Output.LocalizedOutputMode.ShouldBe(LocalizedOutputMode.SeparateStringFiles);
        workspace.LastSelectOutputRequest.Output.MasterStyle.ShouldBe(OutputMasterStyle.Small);
    }

    /// <summary>Verifies read-only opening keeps the selected plugin in immutable source inputs and admits no output.</summary>
    [Fact]
    public async Task OpenSelectedPluginReadOnlyAsync_WithSelectedEntry_UsesPluginAndMastersAsSources()
    {
        var root = CreateTestRoot();
        var workspace = CreateSuccessfulWorkspace();
        var factory = CreateFactory(workspace);
        await using var coordinator = CreateCoordinator(factory);
        var discovery = new FakePluginDiscoveryService
        {
            Result = EngineResult<PluginCatalog>.Success(CreateCatalog(root))
        };
        var viewModel = CreateViewModel(coordinator, new FakeWorkspacePathPicker(), discovery);
        await viewModel.RefreshPluginsAsync();
        viewModel.SelectedPlugin = viewModel.PluginRows.Single(row => row.FileName == "Editable.esp");

        var opened = await viewModel.OpenSelectedPluginReadOnlyAsync();

        opened.ShouldBeTrue();
        factory.Requests.Single().SourcePluginPath.ShouldBe(Path.Combine(root, "Editable.esp"));
        factory.Requests.Single().LoadOrderPluginPaths.ShouldBe([
            Path.Combine(root, "Base.esm"),
            Path.Combine(root, "Editable.esp")]);
        factory.Requests.Single().StringDirectoryPaths.ShouldBe([root]);
        workspace.LastSelectOutputRequest.ShouldBeNull();
        var descriptor = coordinator.CurrentWorkspace.ShouldNotBeNull();
        descriptor.Output.ShouldBeNull();
    }

    /// <summary>Verifies new-plugin creation uses the detected enabled load order and the existing save-file picker.</summary>
    [Fact]
    public async Task CreateNewPluginAsync_WithDetectedCatalog_UsesEnabledPluginsAsReadOnlySources()
    {
        var root = CreateTestRoot();
        var outputPath = Path.Combine(root, "NewPatch.esl");
        var picker = new FakeWorkspacePathPicker { OutputPluginPath = outputPath };
        var workspace = CreateSuccessfulWorkspace();
        var factory = CreateFactory(workspace);
        await using var coordinator = CreateCoordinator(factory);
        var discovery = new FakePluginDiscoveryService
        {
            Result = EngineResult<PluginCatalog>.Success(CreateCatalog(root))
        };
        var viewModel = CreateViewModel(coordinator, picker, discovery);
        await viewModel.RefreshPluginsAsync();

        var opened = await viewModel.CreateNewPluginAsync();

        opened.ShouldBeTrue();
        picker.RequestedOutputMode.ShouldBe(OutputSelectionMode.CreateNew);
        factory.Requests.Single().SourcePluginPath.ShouldBe(Path.Combine(root, "Editable.esp"));
        factory.Requests.Single().LoadOrderPluginPaths.ShouldBe([
            Path.Combine(root, "Base.esm"),
            Path.Combine(root, "Editable.esp")]);
        workspace.LastSelectOutputRequest.ShouldNotBeNull();
        workspace.LastSelectOutputRequest.Mode.ShouldBe(OutputSelectionMode.CreateNew);
        workspace.LastSelectOutputRequest.Output.PluginPath.ShouldBe(outputPath);
        workspace.LastSelectOutputRequest.Output.MasterStyle.ShouldBe(OutputMasterStyle.Small);
    }

    /// <summary>Creates a two-plugin detected catalog with one read-only base and one editable plugin.</summary>
    /// <param name="root">The synthetic data directory.</param>
    /// <returns>The detached catalog.</returns>
    private static PluginCatalog CreateCatalog(string root)
    {
        var basePath = Path.Combine(root, "Base.esm");
        var editablePath = Path.Combine(root, "Editable.esp");
        return new PluginCatalog(
            SupportedGame.Starfield,
            GameRelease.Starfield,
            root,
            [
                new PluginCatalogEntry(
                    ModKey.FromNameAndExtension("Base.esm"),
                    basePath,
                    0,
                    true,
                    [],
                    LocalizedOutputMode.Embedded,
                    OutputMasterStyle.Full,
                    false,
                    "Bethesda-supplied game plugins are read-only."),
                new PluginCatalogEntry(
                    ModKey.FromNameAndExtension("Editable.esp"),
                    editablePath,
                    1,
                    true,
                    [basePath],
                    LocalizedOutputMode.SeparateStringFiles,
                    OutputMasterStyle.Small,
                    true,
                    null,
                    [ModKey.FromNameAndExtension("Base.esm")],
                    "Venworks",
                    "Editable test plugin")
            ]);
    }

    /// <summary>Creates the plugin-selection view model with deterministic dependencies.</summary>
    /// <param name="coordinator">The guarded presentation workspace owner.</param>
    /// <param name="picker">The deterministic path picker.</param>
    /// <param name="discovery">The deterministic plugin catalog provider.</param>
    /// <returns>The configured view model.</returns>
    private static WorkspaceSelectionViewModel CreateViewModel(
        WorkspaceCoordinator coordinator,
        FakeWorkspacePathPicker picker,
        FakePluginDiscoveryService discovery)
    {
        return new WorkspaceSelectionViewModel(
            coordinator,
            picker,
            new FakeGameSelectionService(),
            new InlineUiDispatcher(),
            new LoggerConfiguration().CreateLogger(),
            discovery);
    }

    /// <summary>Creates a coordinator over one successful fake workspace.</summary>
    /// <param name="workspace">The workspace returned from the factory.</param>
    /// <returns>The coordinator.</returns>
    private static WorkspaceCoordinator CreateCoordinator(FakeFormListWorkspace workspace)
    {
        return CreateCoordinator(CreateFactory(workspace));
    }

    /// <summary>Creates a coordinator over a recording factory.</summary>
    /// <param name="factory">The recording engine factory.</param>
    /// <returns>The coordinator.</returns>
    private static WorkspaceCoordinator CreateCoordinator(FakeFormListWorkspaceFactory factory)
    {
        return new WorkspaceCoordinator(
            factory,
            new InlineUiDispatcher(),
            new LoggerConfiguration().CreateLogger());
    }

    /// <summary>Creates a recording factory for one workspace.</summary>
    /// <param name="workspace">The workspace returned by the factory.</param>
    /// <returns>The recording factory.</returns>
    private static FakeFormListWorkspaceFactory CreateFactory(FakeFormListWorkspace workspace)
    {
        return new FakeFormListWorkspaceFactory((_, _) =>
            ValueTask.FromResult(EngineResult<IPluginWorkspace>.Success(workspace)));
    }

    /// <summary>Creates a workspace whose output selection succeeds with the requested association.</summary>
    /// <returns>The successful fake workspace.</returns>
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
                    [new PluginArtifactAssociation(
                        Path.GetFullPath(request.Output.PluginPath),
                        PluginArtifactRole.Plugin,
                        null,
                        new PluginArtifactFingerprint(false, 0, null))]);
                return ValueTask.FromResult(EngineResult<OutputSelectionReceipt>.Success(
                    new OutputSelectionReceipt(request.Output, baseline, resultRevision),
                    operationId: request.OperationId,
                    baseRevision: request.ExpectedRevision,
                    resultRevision: resultRevision));
            });
    }

    /// <summary>Creates a unique synthetic data path without writing files.</summary>
    /// <returns>The absolute path.</returns>
    private static string CreateTestRoot()
    {
        return Path.Combine(Path.GetTempPath(), "CreationsForge-PresentationTests", Guid.NewGuid().ToString("N"));
    }
}
