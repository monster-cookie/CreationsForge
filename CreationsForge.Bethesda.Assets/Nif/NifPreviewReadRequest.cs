namespace CreationsForge.Bethesda.Assets.Nif;

public class NifPreviewReadRequest
{
    public required string SourcePath { get; set; }

    public required string DisplayName { get; set; }

    public required byte[] Data { get; set; }
}
