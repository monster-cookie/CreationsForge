namespace CreationsForge.Bethesda.Assets.Resources;

public enum BethesdaAssetReadStatus
{
    ReadLooseFile,
    ReadArchiveEntry,
    MissingAbsoluteFile,
    MissingDataFolder,
    MissingLooseFile,
    ArchiveReaderUnavailable,
    ArchiveEntryMissing,
    AssetTooLarge
}
