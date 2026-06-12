namespace CreationsForge.Bethesda.Assets.Nif;

public class NifPreviewMesh
{
    public required string Name { get; set; }

    public required string MaterialName { get; set; }

    public string? TexturePath { get; set; }

    public string? OverlayTexturePath { get; set; }

    public string? DecalOpacityTexturePath { get; set; }

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

    public IList<string> Diagnostics { get; set; } = new List<string>();

    public IList<NifPreviewVertex> Vertices { get; set; } = new List<NifPreviewVertex>();

    public IList<int> Indices { get; set; } = new List<int>();
}
