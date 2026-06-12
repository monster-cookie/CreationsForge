namespace CreationsForge.Bethesda.Assets.Nif;

public class NifPreviewReadResult
{
    public required bool IsSuccess { get; set; }

    public NifPreviewModel? Model { get; set; }

    public required string StatusMessage { get; set; }

    public IList<string> Diagnostics { get; set; } = new List<string>();
}
