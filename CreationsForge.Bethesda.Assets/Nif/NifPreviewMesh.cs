namespace CreationsForge.Bethesda.Assets.Nif;

public class NifPreviewMesh
{
    public required string Name { get; set; }

    public required string MaterialName { get; set; }

    public string? TexturePath { get; set; }

    public IList<string> Diagnostics { get; set; } = new List<string>();

    public IList<NifPreviewVertex> Vertices { get; set; } = new List<NifPreviewVertex>();

    public IList<int> Indices { get; set; } = new List<int>();
}
