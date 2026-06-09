using CreationsForge.Core.DTOs.Assets;

namespace CreationsForge.Services.Interfaces;

public interface IAssetPreviewRenderMeshFactory
{
    AssetPreviewRenderMesh CreateRenderMesh(AssetPreviewModelDTO? previewModel);
}
