using CreationsForge.Core.DTOs.Assets;

namespace CreationsForge.Services.Interfaces;

public interface IAssetPreviewSceneService
{
    AssetPreviewModelDTO CreateSamplePreview(AssetPreviewCandidateDTO candidate);
}
