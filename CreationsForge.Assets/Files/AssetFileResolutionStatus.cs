namespace CreationsForge.Assets.Files;

public enum AssetFileResolutionStatus
{
    ResolvedLooseFile,
    MissingAbsoluteFile,
    MissingDataFolder,
    MissingLooseFile,
    ArchiveExtractionUnsupported
}
