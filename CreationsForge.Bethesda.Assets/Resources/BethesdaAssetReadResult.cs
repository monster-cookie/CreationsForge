namespace CreationsForge.Bethesda.Assets.Resources;

public class BethesdaAssetReadResult
{
    public required BethesdaAssetReadStatus Status { get; set; }

    public required BethesdaAssetSourceType SourceType { get; set; }

    public required string OriginalPath { get; set; }

    public byte[]? Data { get; set; }

    public string? ResolvedPath { get; set; }

    public string? DataFolder { get; set; }

    public string? SourceArchivePath { get; set; }

    public string? NormalizedEntryPath { get; set; }

    public required string StatusMessage { get; set; }

    public List<string> SearchedPaths { get; } = new();

    public bool IsSuccess => Status is BethesdaAssetReadStatus.ReadLooseFile or BethesdaAssetReadStatus.ReadArchiveEntry;
}
