namespace CreationsForge.Core.DTOs.Assets;

public class AssetPreviewVertexDTO
{
    public required AssetPreviewVector3DTO Position { get; set; }

    public required AssetPreviewVector3DTO Normal { get; set; }

    public required AssetPreviewUVDTO UV { get; set; }
}
