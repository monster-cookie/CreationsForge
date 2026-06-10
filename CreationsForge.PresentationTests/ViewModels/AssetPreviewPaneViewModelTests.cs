using CreationsForge.Core.DTOs.Assets;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.Services.Interfaces;
using CreationsForge.ViewModels;
using Serilog;
using Shouldly;

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
    public void LoadPreviewForRecord_RebuildsMeshSelectionsAndResetsToAll()
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

        viewModel.MeshSelections.Count.ShouldBe(3);
        viewModel.SelectedMeshSelection.ShouldNotBeNull();
        viewModel.SelectedMeshSelection.Value.MeshIndex.ShouldBeNull();
        viewModel.SelectedMeshSelection.Value.DisplayName.ShouldBe("All meshes");
    }

    [Fact]
    public void ClearPreview_ResetsMeshSelectionsToAll()
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

        viewModel.ClearPreview();

        viewModel.MeshSelections.Count.ShouldBe(1);
        viewModel.SelectedMeshSelection.ShouldNotBeNull();
        viewModel.SelectedMeshSelection.Value.MeshIndex.ShouldBeNull();
        viewModel.SelectedMeshSelection.Value.DisplayName.ShouldBe("All meshes");
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

    private class FakeExternalAssetOpenService : IExternalAssetOpenService
    {
        public bool OpenExternally(string assetPath)
        {
            return true;
        }
    }
}
