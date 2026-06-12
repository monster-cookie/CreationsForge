namespace CreationsForge.Bethesda.Assets.Nif;

public class NifPreviewModel
{
    public required string DisplayName { get; set; }

    public required string SourcePath { get; set; }

    public IList<NifPreviewMesh> Meshes { get; set; } = new List<NifPreviewMesh>();
}
