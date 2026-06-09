namespace CreationsForge.Assets.Archives;

public class AssetArchiveReadResult
{
    public required bool IsSuccess { get; set; }

    public string? ExtractedPath { get; set; }

    public string? StatusMessage { get; set; }
}
