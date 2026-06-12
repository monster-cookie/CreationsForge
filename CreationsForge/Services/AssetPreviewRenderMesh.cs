namespace CreationsForge.Services;

public class AssetPreviewRenderMesh
{
    public IList<float> Vertices { get; set; } = new List<float>();

    public IList<uint> Indices { get; set; } = new List<uint>();

    public IList<uint> LineIndices { get; set; } = new List<uint>();

    public IList<string> TexturePaths { get; set; } = new List<string>();

    public IList<AssetPreviewRenderTexture> Textures { get; set; } = new List<AssetPreviewRenderTexture>();

    public IList<AssetPreviewRenderMeshPart> MeshParts { get; set; } = new List<AssetPreviewRenderMeshPart>();
}

public class AssetPreviewRenderTexture
{
    public required string Path { get; set; }

    public required byte[] Data { get; set; }
}

public class AssetPreviewRenderMeshPart
{
    public required int IndexOffset { get; set; }

    public required int IndexCount { get; set; }

    public int? TextureIndex { get; set; }

    public int? OverlayTextureIndex { get; set; }

    public int? DecalOpacityTextureIndex { get; set; }

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

    public bool UseAdditiveBlend { get; set; }
}
