namespace CreationsForge.Bethesda.Assets.Nif;

public class NifPreviewVertex
{
    public required NifPreviewVector3 Position { get; set; }

    public required NifPreviewVector3 Normal { get; set; }

    public required NifPreviewUV UV { get; set; }

    public float Alpha { get; set; } = 1f;
}
