namespace CreationsForge.Bethesda.Assets.Files;

public class AssetFileResolutionDTO
{
    public required string OriginalPath { get; set; }

    public string? ResolvedPath { get; set; }

    public string? DataFolder { get; set; }

    public required AssetFileResolutionStatus Status { get; set; }

    public required string StatusMessage { get; set; }

    public List<string> SearchedPaths { get; } = new();

    public bool IsResolved => Status == AssetFileResolutionStatus.ResolvedLooseFile && !string.IsNullOrWhiteSpace(ResolvedPath);
}
