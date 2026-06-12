using CreationsForge.Core.DTOs.Assets;

namespace CreationsForge.Services.Interfaces;

public interface IAssetPreviewSceneService
{
    AssetPreviewModelDTO CreatePreview(AssetPreviewCandidateDTO candidate, out string statusMessage);
}
