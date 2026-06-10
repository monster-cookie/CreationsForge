namespace CreationsForge.Services;

public class AssetPreviewRenderMesh
{
    public IList<float> Vertices { get; set; } = new List<float>();

    public IList<uint> Indices { get; set; } = new List<uint>();

    public IList<uint> LineIndices { get; set; } = new List<uint>();

    public IList<string> TexturePaths { get; set; } = new List<string>();
}
