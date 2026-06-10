namespace CreationsForge.Core.DTOs.Assets;

public class AssetPreviewMeshDTO
{
    public required string Name { get; set; }

    public required string MaterialName { get; set; }

    public string? TexturePath { get; set; }

    public AssetPreviewTextureDTO? Texture { get; set; }

    public IList<AssetPreviewVertexDTO> Vertices { get; set; } = new List<AssetPreviewVertexDTO>();

    public IList<int> Indices { get; set; } = new List<int>();
}
