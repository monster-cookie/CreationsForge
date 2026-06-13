using CreationsForge.Core.DTOs.Assets;

namespace CreationsForge.Services.Interfaces;

public interface IExternalAssetOpenService
{
    bool OpenExternally(string assetPath);

    bool OpenExternally(AssetPreviewCandidateDTO candidate)
    {
        return OpenExternally(candidate.MeshPath);
    }
}
