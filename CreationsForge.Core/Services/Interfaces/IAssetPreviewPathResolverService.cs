using CreationsForge.Core.DTOs.Assets;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.Services.Interfaces;

public interface IAssetPreviewPathResolverService
{
    IReadOnlyList<AssetPreviewCandidateDTO> GetPreviewCandidates(SupportedGame game, string recordType, FormKeyDTO formKey);

    bool CanPreviewPath(string? meshPath);

    bool CanOpenExternally(string? meshPath);

    string? ResolveExternalOpenPath(AssetPreviewCandidateDTO candidate);
}
