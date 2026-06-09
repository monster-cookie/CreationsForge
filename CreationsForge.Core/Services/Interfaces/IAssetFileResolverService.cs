using CreationsForge.Assets.Files;
using CreationsForge.Core.DTOs.Assets;

namespace CreationsForge.Core.Services.Interfaces;

public interface IAssetFileResolverService
{
    AssetFileResolutionDTO ResolveAssetFile(AssetPreviewCandidateDTO candidate);
}
