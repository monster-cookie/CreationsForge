namespace CreationsForge.Core.DTOs.Assets;

public class AssetPreviewMeshDTO
{
    public required string Name { get; set; }

    public required string MaterialName { get; set; }

    public string? TexturePath { get; set; }

    public AssetPreviewTextureDTO? Texture { get; set; }

    public string? OverlayTexturePath { get; set; }

    public AssetPreviewTextureDTO? OverlayTexture { get; set; }

    public string? DecalOpacityTexturePath { get; set; }

    public AssetPreviewTextureDTO? DecalOpacityTexture { get; set; }

    public float MaterialTintRed { get; set; } = 1f;

    public float MaterialTintGreen { get; set; } = 1f;

    public float MaterialTintBlue { get; set; } = 1f;

    public float MaterialTintAlpha { get; set; } = 1f;

    public float DecalTintRed { get; set; } = 1f;

    public float DecalTintGreen { get; set; } = 1f;

    public float DecalTintBlue { get; set; } = 1f;

    public float DecalOpacity { get; set; } = 1f;

    public float DecalUvScaleU { get; set; } = 1f;

    public float DecalUvScaleV { get; set; } = 1f;

    public float DecalUvOffsetU { get; set; }

    public float DecalUvOffsetV { get; set; }

    public bool IsDecal { get; set; }

    public bool IsInvisible { get; set; }

    public bool UseAdditiveBlend { get; set; }

    public IList<AssetPreviewVertexDTO> Vertices { get; set; } = new List<AssetPreviewVertexDTO>();

    public IList<int> Indices { get; set; } = new List<int>();
}
