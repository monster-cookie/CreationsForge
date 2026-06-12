namespace CreationsForge.Bethesda.Assets.Archives;

public class AssetArchiveReadResult
{
    public required bool IsSuccess { get; set; }

    public byte[]? Data { get; set; }

    public bool IsTooLarge { get; set; }

    public string? ArchivePath { get; set; }

    public string? EntryPath { get; set; }

    public string? StatusMessage { get; set; }
}
