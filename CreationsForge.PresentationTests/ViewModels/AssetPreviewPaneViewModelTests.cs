using Autofac;
using CreationsForge.Core.DTOs.Assets;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.Services.Interfaces;
using CreationsForge.ViewModels;
using Serilog;
using Shouldly;
using System.Threading;

namespace CreationsForge.PresentationTests.ViewModels;

public class AssetPreviewPaneViewModelTests
{
    [Fact]
    public void SelectedMeshSelection_AllowsNullBindingReset()
    {
        var viewModel = CreateViewModel();
        viewModel.SelectedMeshSelection = new AssetPreviewPaneViewModel.AssetPreviewMeshSelectionOption(0, "Mesh 1");

        viewModel.SelectedMeshSelection = null;

        viewModel.SelectedMeshSelection.ShouldNotBeNull();
        viewModel.SelectedMeshSelection.Value.MeshIndex.ShouldBeNull();
        viewModel.SelectedMeshSelection.Value.DisplayName.ShouldBe("All meshes");
    }

    [Fact]
    public void Constructor_ExposesRenderModesForSingleInteractivePreview()
    {
        var viewModel = CreateViewModel();

        viewModel.RenderModes.ShouldBe(
            [
                AssetPreviewRenderMode.Solid,
                AssetPreviewRenderMode.Wireframe,
                AssetPreviewRenderMode.Points
            ]);
        viewModel.SelectedRenderMode.ShouldBe(AssetPreviewRenderMode.Solid);
    }

    [Fact]
    public async Task LoadPreviewForRecord_RebuildsMeshSelectionsAndResetsToAll()
    {
        var pathResolver = new FakeAssetPreviewPathResolverService
        {
            Candidates =
            [
                CreateCandidate()
            ]
        };
        var sceneService = new FakeAssetPreviewSceneService
        {
            PreviewModel = CreatePreviewModel()
        };
        var viewModel = CreateViewModel(pathResolver, sceneService);

        viewModel.LoadPreviewForRecord(SupportedGame.Fallout4, "MISC", CreateFormKey());
        await WaitUntil(() => !viewModel.IsPreviewLoading);

        viewModel.MeshSelections.Count.ShouldBe(3);
        viewModel.SelectedMeshSelection.ShouldNotBeNull();
        viewModel.SelectedMeshSelection.Value.MeshIndex.ShouldBeNull();
        viewModel.SelectedMeshSelection.Value.DisplayName.ShouldBe("All meshes");
    }

    [Fact]
    public async Task ClearPreview_ResetsMeshSelectionsToAll()
    {
        var pathResolver = new FakeAssetPreviewPathResolverService
        {
            Candidates =
            [
                CreateCandidate()
            ]
        };
        var sceneService = new FakeAssetPreviewSceneService
        {
            PreviewModel = CreatePreviewModel()
        };
        var viewModel = CreateViewModel(pathResolver, sceneService);
        viewModel.LoadPreviewForRecord(SupportedGame.Fallout4, "MISC", CreateFormKey());
        await WaitUntil(() => !viewModel.IsPreviewLoading);

        viewModel.ClearPreview();

        viewModel.MeshSelections.Count.ShouldBe(1);
        viewModel.SelectedMeshSelection.ShouldNotBeNull();
        viewModel.SelectedMeshSelection.Value.MeshIndex.ShouldBeNull();
        viewModel.SelectedMeshSelection.Value.DisplayName.ShouldBe("All meshes");
    }

    [Fact]
    public async Task LoadPreviewForRecord_ShowsLoadingStateWhilePreviewLoads()
    {
        var pathResolver = new FakeAssetPreviewPathResolverService
        {
            Candidates =
            [
                CreateCandidate()
            ]
        };
        var sceneService = new BlockingAssetPreviewSceneService();
        var viewModel = CreateViewModel(pathResolver, sceneService);

        viewModel.LoadPreviewForRecord(SupportedGame.Fallout4, "MISC", CreateFormKey());
        await WaitForTask(sceneService.Started.Task);

        viewModel.IsPreviewLoading.ShouldBeTrue();
        viewModel.PreviewModel.ShouldBeNull();
        viewModel.PreviewStatusText.ShouldBe("Loading asset preview...");

        sceneService.Release();
        await WaitUntil(() => !viewModel.IsPreviewLoading);

        viewModel.PreviewModel.ShouldNotBeNull();
        viewModel.PreviewStatusText.ShouldBe("Loaded preview.");
    }

    [Fact]
    public async Task ClearPreview_IgnoresInFlightPreviewResult()
    {
        var pathResolver = new FakeAssetPreviewPathResolverService
        {
            Candidates =
            [
                CreateCandidate()
            ]
        };
        var sceneService = new BlockingAssetPreviewSceneService();
        var viewModel = CreateViewModel(pathResolver, sceneService);

        viewModel.LoadPreviewForRecord(SupportedGame.Fallout4, "MISC", CreateFormKey());
        await WaitForTask(sceneService.Started.Task);

        viewModel.ClearPreview();
        sceneService.Release();
        await WaitForTask(sceneService.Completed.Task);
        await Task.Delay(20);

        viewModel.IsPreviewLoading.ShouldBeFalse();
        viewModel.PreviewModel.ShouldBeNull();
        viewModel.MeshSelections.Count.ShouldBe(1);
        viewModel.PreviewStatusText.ShouldBe("Select a model-bearing record to preview assets.");
    }

    [Fact]
    public async Task LoadPreviewForRecord_IgnoresPreviousPreviewWhenSelectionChanges()
    {
        var firstCandidate = CreateCandidate(@"Meshes\First.nif", 0x100);
        var secondCandidate = CreateCandidate(@"Meshes\Second.nif", 0x200);
        var pathResolver = new FakeAssetPreviewPathResolverService
        {
            Candidates =
            [
                firstCandidate
            ]
        };
        var firstSceneService = new BlockingAssetPreviewSceneService
        {
            PreviewModel = CreatePreviewModel("First preview")
        };
        var secondSceneService = new FakeAssetPreviewSceneService
        {
            PreviewModel = CreatePreviewModel("Second preview")
        };
        var viewModel = CreateViewModel(
            pathResolver,
            CreateQueuedLifetimeScope(firstSceneService, secondSceneService));

        viewModel.LoadPreviewForRecord(SupportedGame.Fallout4, "MISC", firstCandidate.FormKey);
        await WaitForTask(firstSceneService.Started.Task);

        pathResolver.Candidates =
        [
            secondCandidate
        ];
        viewModel.LoadPreviewForRecord(SupportedGame.Fallout4, "MISC", secondCandidate.FormKey);
        await WaitUntil(() => !viewModel.IsPreviewLoading);

        viewModel.PreviewModel.ShouldNotBeNull();
        viewModel.PreviewModel.DisplayName.ShouldBe("Second preview");

        firstSceneService.Release();
        await WaitForTask(firstSceneService.Completed.Task);
        await Task.Delay(20);

        viewModel.PreviewModel.ShouldNotBeNull();
        viewModel.PreviewModel.DisplayName.ShouldBe("Second preview");
    }

    [Fact]
    public void OpenExternallyCommand_OpensResolvedPath()
    {
        var pathResolver = new FakeAssetPreviewPathResolverService
        {
            Candidates =
            [
                CreateCandidate()
            ],
            ExternalOpenPath = @"C:\Games\Data\Meshes\Preview.nif"
        };
        var externalOpenService = new FakeExternalAssetOpenService();
        var viewModel = CreateViewModel(pathResolver, externalAssetOpenService: externalOpenService);

        viewModel.LoadPreviewForRecord(SupportedGame.Fallout4, "MISC", CreateFormKey());
        viewModel.OpenExternallyCommand.Execute(null);

        externalOpenService.OpenedPath.ShouldBe(@"C:\Games\Data\Meshes\Preview.nif");
    }

    private static AssetPreviewPaneViewModel CreateViewModel(
        IAssetPreviewPathResolverService? pathResolver = null,
        IAssetPreviewSceneService? sceneService = null,
        IExternalAssetOpenService? externalAssetOpenService = null)
    {
        return CreateViewModel(
            pathResolver,
            CreateLifetimeScope(sceneService ?? new FakeAssetPreviewSceneService()),
            externalAssetOpenService);
    }

    private static AssetPreviewPaneViewModel CreateViewModel(
        IAssetPreviewPathResolverService? pathResolver,
        ILifetimeScope lifetimeScope,
        IExternalAssetOpenService? externalAssetOpenService = null)
    {
        return new AssetPreviewPaneViewModel(
            pathResolver ?? new FakeAssetPreviewPathResolverService(),
            lifetimeScope,
            externalAssetOpenService ?? new FakeExternalAssetOpenService(),
            new LoggerConfiguration().CreateLogger());
    }

    private static ILifetimeScope CreateLifetimeScope(IAssetPreviewSceneService sceneService)
    {
        var builder = new ContainerBuilder();
        builder.RegisterInstance(sceneService).As<IAssetPreviewSceneService>();
        return builder.Build();
    }

    private static ILifetimeScope CreateQueuedLifetimeScope(params IAssetPreviewSceneService[] sceneServices)
    {
        var queue = new Queue<IAssetPreviewSceneService>(sceneServices);
        var builder = new ContainerBuilder();
        builder.Register(_ => queue.Dequeue()).As<IAssetPreviewSceneService>();
        return builder.Build();
    }

    private static AssetPreviewCandidateDTO CreateCandidate(string meshPath = @"Meshes\Preview.nif", uint formId = 0x1A899B)
    {
        return new AssetPreviewCandidateDTO
        {
            Game = SupportedGame.Fallout4,
            ModKey = CreateModKey(),
            RecordType = "MISC",
            FormKey = CreateFormKey(formId),
            ModelSlot = "Model",
            MeshPath = meshPath,
            DisplayName = $"Model: {Path.GetFileName(meshPath)}",
            CanPreview = true,
            CanOpenExternally = true
        };
    }

    private static AssetPreviewModelDTO CreatePreviewModel(string displayName = "Preview")
    {
        return new AssetPreviewModelDTO
        {
            DisplayName = displayName,
            SourcePath = @"Meshes\Preview.nif",
            Meshes =
            {
                new AssetPreviewMeshDTO
                {
                    Name = "Mesh 1",
                    MaterialName = "Material 1"
                },
                new AssetPreviewMeshDTO
                {
                    Name = "Mesh 2",
                    MaterialName = "Material 2"
                }
            }
        };
    }

    private static FormKeyDTO CreateFormKey(uint formId = 0x1A899B)
    {
        return new FormKeyDTO
        {
            ModKey = CreateModKey(),
            Id = formId
        };
    }

    private static ModKeyDTO CreateModKey()
    {
        return new ModKeyDTO
        {
            Name = "Fallout4",
            Type = 0,
            FileName = "Fallout4.esm"
        };
    }

    private class FakeAssetPreviewPathResolverService : IAssetPreviewPathResolverService
    {
        public IReadOnlyList<AssetPreviewCandidateDTO> Candidates { get; set; } = [];

        public string? ExternalOpenPath { get; set; }

        public IReadOnlyList<AssetPreviewCandidateDTO> GetPreviewCandidates(SupportedGame game, string recordType, FormKeyDTO formKey)
        {
            return Candidates;
        }

        public bool CanPreviewPath(string? meshPath)
        {
            return true;
        }

        public bool CanOpenExternally(string? meshPath)
        {
            return true;
        }

        public string? ResolveExternalOpenPath(AssetPreviewCandidateDTO candidate)
        {
            return ExternalOpenPath;
        }
    }

    private class FakeAssetPreviewSceneService : IAssetPreviewSceneService
    {
        public AssetPreviewModelDTO PreviewModel { get; set; } = CreatePreviewModel();

        public AssetPreviewModelDTO CreatePreview(AssetPreviewCandidateDTO candidate, out string statusMessage)
        {
            statusMessage = "Loaded preview.";
            return PreviewModel;
        }
    }

    private class BlockingAssetPreviewSceneService : IAssetPreviewSceneService
    {
        private readonly ManualResetEventSlim ReleaseGate = new ManualResetEventSlim(false);

        public AssetPreviewModelDTO PreviewModel { get; set; } = CreatePreviewModel();

        public TaskCompletionSource Started { get; } = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        public TaskCompletionSource Completed { get; } = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        public AssetPreviewModelDTO CreatePreview(AssetPreviewCandidateDTO candidate, out string statusMessage)
        {
            Started.TrySetResult();
            try
            {
                ReleaseGate.Wait();
                statusMessage = "Loaded preview.";
                return PreviewModel;
            }
            finally
            {
                Completed.TrySetResult();
            }
        }

        public void Release()
        {
            ReleaseGate.Set();
        }
    }

    private class FakeExternalAssetOpenService : IExternalAssetOpenService
    {
        public string? OpenedPath { get; private set; }

        public bool OpenExternally(string assetPath)
        {
            OpenedPath = assetPath;
            return true;
        }
    }

    private static async Task WaitUntil(Func<bool> condition)
    {
        for (var attempt = 0; attempt < 100; attempt++)
        {
            if (condition())
            {
                return;
            }

            await Task.Delay(20);
        }

        condition().ShouldBeTrue();
    }

    private static async Task WaitForTask(Task task)
    {
        var completedTask = await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(5)));
        completedTask.ShouldBe(task);
        await task;
    }
}
