using CreationsForge.Core.DTOs.Assets;

namespace CreationsForge.Services.Interfaces;

public interface IAssetPreviewGeometryReader
{
    bool TryRead(AssetPreviewCandidateDTO candidate, out AssetPreviewModelDTO? previewModel, out string statusMessage);
}
