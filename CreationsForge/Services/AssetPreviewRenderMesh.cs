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
}
