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

    private static AssetPreviewPaneViewModel CreateViewModel(
        IAssetPreviewPathResolverService? pathResolver = null,
        IAssetPreviewSceneService? sceneService = null)
    {
        return new AssetPreviewPaneViewModel(
            pathResolver ?? new FakeAssetPreviewPathResolverService(),
            sceneService ?? new FakeAssetPreviewSceneService(),
            new FakeExternalAssetOpenService(),
            new LoggerConfiguration().CreateLogger());
    }

    private static AssetPreviewCandidateDTO CreateCandidate()
    {
        return new AssetPreviewCandidateDTO
        {
            Game = SupportedGame.Fallout4,
            ModKey = CreateModKey(),
            RecordType = "MISC",
            FormKey = CreateFormKey(),
            ModelSlot = "Model",
            MeshPath = @"Meshes\Preview.nif",
            DisplayName = "Model: Preview.nif",
            CanPreview = true,
            CanOpenExternally = true
        };
    }

    private static AssetPreviewModelDTO CreatePreviewModel()
    {
        return new AssetPreviewModelDTO
        {
            DisplayName = "Preview",
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

    private static FormKeyDTO CreateFormKey()
    {
        return new FormKeyDTO
        {
            ModKey = CreateModKey(),
            Id = 0x1A899B
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

        public TaskCompletionSource Started { get; } = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        public TaskCompletionSource Completed { get; } = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        public AssetPreviewModelDTO CreatePreview(AssetPreviewCandidateDTO candidate, out string statusMessage)
        {
            Started.TrySetResult();
            try
            {
                ReleaseGate.Wait();
                statusMessage = "Loaded preview.";
                return CreatePreviewModel();
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
        public bool OpenExternally(string assetPath)
        {
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
