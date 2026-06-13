namespace CreationsForge.Core.DTOs.Assets;

public class AssetPreviewModelDTO
{
    public required string DisplayName { get; set; }

    public required string SourcePath { get; set; }

    public bool AllowFallbackRender { get; set; } = true;

    public IList<AssetPreviewMeshDTO> Meshes { get; set; } = new List<AssetPreviewMeshDTO>();
}
