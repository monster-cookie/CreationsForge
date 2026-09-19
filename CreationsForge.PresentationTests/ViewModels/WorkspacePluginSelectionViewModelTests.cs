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

    /// <summary>Verifies the selected file type supplies the extension and only Starfield.esm is admitted.</summary>
    [Fact]
    public async Task CreateNewPluginAsync_WithDetectedCatalog_UsesOnlyGameBase()
    {
        var root = CreateTestRoot();
        var outputPath = Path.Combine(root, "NewPatch.esl");
        var picker = new FakeWorkspacePathPicker();
        var workspace = CreateSuccessfulWorkspace();
        var factory = CreateFactory(workspace);
        await using var coordinator = CreateCoordinator(factory);
        var discovery = new FakePluginDiscoveryService
        {
            Result = EngineResult<PluginCatalog>.Success(CreateMisorderedCatalog(root))
        };
        var viewModel = CreateViewModel(coordinator, picker, discovery);
        await viewModel.RefreshPluginsAsync();
        viewModel.NewPluginExtension = ".esl";
        viewModel.NewPluginFileName = "NewPatch";

        var opened = await viewModel.CreateNewPluginAsync();

        opened.ShouldBeTrue();
        picker.RequestedOutputMode.ShouldBeNull();
        factory.Requests.Single().SourcePluginPath.ShouldBe(Path.Combine(root, "Starfield.esm"));
        factory.Requests.Single().LoadOrderPluginPaths.ShouldBe([Path.Combine(root, "Starfield.esm")]);
        workspace.LastSelectOutputRequest.ShouldNotBeNull();
        workspace.LastSelectOutputRequest.Mode.ShouldBe(OutputSelectionMode.CreateNew);
        workspace.LastSelectOutputRequest.Output.PluginPath.ShouldBe(outputPath);
        workspace.LastSelectOutputRequest.Output.MasterStyle.ShouldBe(OutputMasterStyle.Small);
    }

    /// <summary>Verifies a localized new-plugin choice reaches the output association instead of being forced to embedded text.</summary>
    [Fact]
    public async Task CreateNewPluginAsync_WithLocalizedText_PreservesSelectedMode()
    {
        var root = CreateTestRoot();
        var workspace = CreateSuccessfulWorkspace();
        var factory = CreateFactory(workspace);
        await using var coordinator = CreateCoordinator(factory);
        var discovery = new FakePluginDiscoveryService
        {
            Result = EngineResult<PluginCatalog>.Success(CreateMisorderedCatalog(root))
        };
        var viewModel = CreateViewModel(coordinator, new FakeWorkspacePathPicker(), discovery);
        await viewModel.RefreshPluginsAsync();
        viewModel.NewPluginExtension = ".esm";
        viewModel.NewPluginFileName = "LocalizedPatch";
        viewModel.NewPluginLocalizedOutputMode = LocalizedOutputMode.SeparateStringFiles;

        (await viewModel.CreateNewPluginAsync()).ShouldBeTrue();

        workspace.LastSelectOutputRequest.ShouldNotBeNull();
        workspace.LastSelectOutputRequest.Output.LocalizedOutputMode.ShouldBe(LocalizedOutputMode.SeparateStringFiles);
    }

    /// <summary>Verifies selecting a patch never adds it or its declared masters to a new plugin by default.</summary>
    [Fact]
    public async Task CreateNewPluginAsync_WithSelectedPatch_StillUsesOnlyGameBase()
    {
        var root = CreateTestRoot();
        var outputPath = Path.Combine(root, "NewMediumMaster.esm");
        var picker = new FakeWorkspacePathPicker();
        var workspace = CreateSuccessfulWorkspace();
        var factory = CreateFactory(workspace);
        await using var coordinator = CreateCoordinator(factory);
        var discovery = new FakePluginDiscoveryService
        {
            Result = EngineResult<PluginCatalog>.Success(CreateMisorderedCatalog(root))
        };
        var viewModel = CreateViewModel(coordinator, picker, discovery);
        await viewModel.RefreshPluginsAsync();
        viewModel.SelectedPlugin = viewModel.PluginRows.Single(row => row.FileName == "Unofficial Starfield Patch.esm");
        viewModel.NewPluginExtension = ".esm";
        viewModel.OutputMasterStyle = OutputMasterStyle.Medium;
        viewModel.NewPluginFileName = "NewMediumMaster";

        var opened = await viewModel.CreateNewPluginAsync();

        opened.ShouldBeTrue();
        factory.Requests.Single().LoadOrderPluginPaths.ShouldBe([Path.Combine(root, "Starfield.esm")]);
        workspace.LastSelectOutputRequest.ShouldNotBeNull();
        workspace.LastSelectOutputRequest.Output.PluginPath.ShouldBe(outputPath);
        workspace.LastSelectOutputRequest.Output.MasterStyle.ShouldBe(OutputMasterStyle.Medium);
    }

    /// <summary>Verifies the default ESM master size reaches the guarded output selection.</summary>
    [Fact]
    public async Task CreateNewPluginAsync_WithEsmDefault_UsesSmallMaster()
    {
        var root = CreateTestRoot();
        var workspace = CreateSuccessfulWorkspace();
        await using var coordinator = CreateCoordinator(CreateFactory(workspace));
        var discovery = new FakePluginDiscoveryService
        {
            Result = EngineResult<PluginCatalog>.Success(CreateMisorderedCatalog(root))
        };
        var viewModel = CreateViewModel(coordinator, new FakeWorkspacePathPicker(), discovery);
        await viewModel.RefreshPluginsAsync();
        viewModel.NewPluginExtension = ".esm";
        viewModel.NewPluginFileName = "NewSmallMaster";

        (await viewModel.CreateNewPluginAsync()).ShouldBeTrue();

        workspace.LastSelectOutputRequest!.Output.MasterStyle.ShouldBe(OutputMasterStyle.Small);
    }

    /// <summary>Verifies unrelated enabled plugins with incompatible master orders cannot block the default new-plugin context.</summary>
    [Fact]
    public async Task CreateNewPluginAsync_WithUnrelatedConflictingPlugins_OpensBaseContext()
    {
        var root = CreateTestRoot();
        var picker = new FakeWorkspacePathPicker();
        var factory = CreateFactory(CreateSuccessfulWorkspace());
        await using var coordinator = CreateCoordinator(factory);
        var discovery = new FakePluginDiscoveryService
        {
            Result = EngineResult<PluginCatalog>.Success(CreateConflictingCatalog(root))
        };
        var viewModel = CreateViewModel(coordinator, picker, discovery);
        await viewModel.RefreshPluginsAsync();
        viewModel.NewPluginFileName = "NewPatch";

        var opened = await viewModel.CreateNewPluginAsync();

        opened.ShouldBeTrue();
        picker.RequestedOutputMode.ShouldBeNull();
        factory.Requests.Single().LoadOrderPluginPaths.ShouldBe([Path.Combine(root, "Starfield.esm")]);
        viewModel.ErrorText.ShouldBeNull();
    }

    /// <summary>Verifies a typed extension is rejected because the selected file type supplies it.</summary>
    [Fact]
    public async Task CreateNewPluginAsync_WithTypedExtension_RejectsOutput()
    {
        var root = CreateTestRoot();
        var picker = new FakeWorkspacePathPicker();
        var factory = CreateFactory(CreateSuccessfulWorkspace());
        await using var coordinator = CreateCoordinator(factory);
        var discovery = new FakePluginDiscoveryService
        {
            Result = EngineResult<PluginCatalog>.Success(CreateMisorderedCatalog(root))
        };
        var viewModel = CreateViewModel(coordinator, picker, discovery);
        await viewModel.RefreshPluginsAsync();
        viewModel.NewPluginExtension = ".esm";
        viewModel.NewPluginFileName = "WrongType.esp";

        var opened = await viewModel.CreateNewPluginAsync();

        opened.ShouldBeFalse();
        viewModel.ErrorText.ShouldBe("Enter a plugin name without an extension; .esm is added from the selected file type.");
        picker.RequestedOutputMode.ShouldBeNull();
        factory.Requests.ShouldBeEmpty();
    }

    /// <summary>Verifies a missing name cannot start workspace acquisition or invoke a picker.</summary>
    [Fact]
    public async Task CreateNewPluginAsync_WithoutName_RejectsBeforeWorkspace()
    {
        var root = CreateTestRoot();
        var picker = new FakeWorkspacePathPicker();
        var factory = CreateFactory(CreateSuccessfulWorkspace());
        await using var coordinator = CreateCoordinator(factory);
        var discovery = new FakePluginDiscoveryService
        {
            Result = EngineResult<PluginCatalog>.Success(CreateMisorderedCatalog(root))
        };
        var viewModel = CreateViewModel(coordinator, picker, discovery);
        await viewModel.RefreshPluginsAsync();

        (await viewModel.CreateNewPluginAsync()).ShouldBeFalse();

        viewModel.ErrorText.ShouldBe("Enter a name for the new plugin.");
        picker.RequestedOutputMode.ShouldBeNull();
        factory.Requests.ShouldBeEmpty();
    }

    /// <summary>Verifies typed names cannot escape the Data directory or use a Windows device name.</summary>
    /// <param name="name">The unsafe filename stem.</param>
    [Theory]
    [InlineData("../Escape")]
    [InlineData("C:\\Outside")]
    [InlineData("CON")]
    [InlineData("Trailing.")]
    public async Task CreateNewPluginAsync_WithUnsafeName_RejectsBeforeWorkspace(string name)
    {
        var root = CreateTestRoot();
        var factory = CreateFactory(CreateSuccessfulWorkspace());
        await using var coordinator = CreateCoordinator(factory);
        var discovery = new FakePluginDiscoveryService
        {
            Result = EngineResult<PluginCatalog>.Success(CreateMisorderedCatalog(root))
        };
        var viewModel = CreateViewModel(coordinator, new FakeWorkspacePathPicker(), discovery);
        await viewModel.RefreshPluginsAsync();
        viewModel.NewPluginFileName = name;

        (await viewModel.CreateNewPluginAsync()).ShouldBeFalse();

        viewModel.ErrorText.ShouldNotBeNull().ShouldContain("cannot be used in a filename");
        factory.Requests.ShouldBeEmpty();
    }

    /// <summary>Verifies a new filename cannot collide with an installed plugin even in a synthetic catalog.</summary>
    [Fact]
    public async Task CreateNewPluginAsync_WithInstalledName_RejectsBeforeWorkspace()
    {
        var root = CreateTestRoot();
        var factory = CreateFactory(CreateSuccessfulWorkspace());
        await using var coordinator = CreateCoordinator(factory);
        var discovery = new FakePluginDiscoveryService
        {
            Result = EngineResult<PluginCatalog>.Success(CreateMisorderedCatalog(root))
        };
        var viewModel = CreateViewModel(coordinator, new FakeWorkspacePathPicker(), discovery);
        await viewModel.RefreshPluginsAsync();
        viewModel.NewPluginFileName = "starfield";
        viewModel.NewPluginExtension = ".esm";

        (await viewModel.CreateNewPluginAsync()).ShouldBeFalse();

        viewModel.ErrorText.ShouldBe("A plugin named 'starfield.esm' already exists in the game Data directory.");
        factory.Requests.ShouldBeEmpty();
    }

    /// <summary>Verifies an unrelated root plugin cannot silently replace the selected game's missing base master.</summary>
    [Fact]
    public async Task CreateNewPluginAsync_WithoutGameBase_RejectsBeforeWorkspace()
    {
        var root = CreateTestRoot();
        var picker = new FakeWorkspacePathPicker();
        var factory = CreateFactory(CreateSuccessfulWorkspace());
        await using var coordinator = CreateCoordinator(factory);
        var discovery = new FakePluginDiscoveryService
        {
            Result = EngineResult<PluginCatalog>.Success(CreateCatalog(root))
        };
        var viewModel = CreateViewModel(coordinator, picker, discovery);
        await viewModel.RefreshPluginsAsync();
        viewModel.NewPluginFileName = "NewPatch";

        var opened = await viewModel.CreateNewPluginAsync();

        opened.ShouldBeFalse();
        viewModel.ErrorText.ShouldBe("The base plugin 'Starfield.esm' was not found in the detected game Data directory.");
        picker.RequestedOutputMode.ShouldBeNull();
        factory.Requests.ShouldBeEmpty();
    }

    /// <summary>Creates an installed catalog where the patch precedes a master it declares.</summary>
    /// <param name="root">The synthetic game Data directory.</param>
    /// <returns>The detached catalog containing the reported master-order case.</returns>
    private static PluginCatalog CreateMisorderedCatalog(string root)
    {
        var starfield = ModKey.FromNameAndExtension("Starfield.esm");
        var sfbgs004 = ModKey.FromNameAndExtension("SFBGS004.esm");
        var patch = ModKey.FromNameAndExtension("Unofficial Starfield Patch.esm");
        return new PluginCatalog(
            SupportedGame.Starfield,
            GameRelease.Starfield,
            root,
            [
                new PluginCatalogEntry(starfield, Path.Combine(root, "Starfield.esm"), 0, true, [],
                    LocalizedOutputMode.Embedded, OutputMasterStyle.Full, false, "Bethesda-supplied game plugins are read-only."),
                new PluginCatalogEntry(patch, Path.Combine(root, "Unofficial Starfield Patch.esm"), 1, true, [],
                    LocalizedOutputMode.Embedded, OutputMasterStyle.Full, true, null, [starfield, sfbgs004]),
                new PluginCatalogEntry(sfbgs004, Path.Combine(root, "SFBGS004.esm"), 2, false, [],
                    LocalizedOutputMode.Embedded, OutputMasterStyle.Full, false, "Bethesda-supplied game plugins are read-only.", [starfield])
            ]);
    }

    /// <summary>Creates an installed list whose unrelated enabled patches declare opposite master orders.</summary>
    /// <param name="root">The synthetic game Data directory.</param>
    /// <returns>A catalog that cannot be ordered as one global source list.</returns>
    private static PluginCatalog CreateConflictingCatalog(string root)
    {
        var baseKey = ModKey.FromNameAndExtension("Starfield.esm");
        var firstMaster = ModKey.FromNameAndExtension("First.esm");
        var secondMaster = ModKey.FromNameAndExtension("Second.esm");
        var firstPatch = ModKey.FromNameAndExtension("FirstPatch.esp");
        var secondPatch = ModKey.FromNameAndExtension("SecondPatch.esp");
        return new PluginCatalog(
            SupportedGame.Starfield,
            GameRelease.Starfield,
            root,
            [
                new PluginCatalogEntry(baseKey, Path.Combine(root, "Starfield.esm"), 0, true, [],
                    LocalizedOutputMode.Embedded, OutputMasterStyle.Full, false, "Bethesda-supplied game plugins are read-only."),
                new PluginCatalogEntry(firstMaster, Path.Combine(root, "First.esm"), 1, true, [],
                    LocalizedOutputMode.Embedded, OutputMasterStyle.Full, true, null, [baseKey]),
                new PluginCatalogEntry(secondMaster, Path.Combine(root, "Second.esm"), 2, true, [],
                    LocalizedOutputMode.Embedded, OutputMasterStyle.Full, true, null, [baseKey]),
                new PluginCatalogEntry(firstPatch, Path.Combine(root, "FirstPatch.esp"), 3, true, [],
                    LocalizedOutputMode.Embedded, OutputMasterStyle.Full, true, null, [baseKey, firstMaster, secondMaster]),
                new PluginCatalogEntry(secondPatch, Path.Combine(root, "SecondPatch.esp"), 4, true, [],
                    LocalizedOutputMode.Embedded, OutputMasterStyle.Full, true, null, [baseKey, secondMaster, firstMaster])
            ]);
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
