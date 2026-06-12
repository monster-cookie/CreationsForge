using CreationsForge.Core.DTOs.Assets;
using CreationsForge.Services;

namespace CreationsForge.Services.Interfaces;

public interface IAssetPreviewRenderMeshFactory
{
    AssetPreviewRenderMesh CreateRenderMesh(AssetPreviewModelDTO? previewModel);

    AssetPreviewRenderMesh CreateRenderMesh(AssetPreviewModelDTO? previewModel, AssetPreviewRenderOptions options);
}
