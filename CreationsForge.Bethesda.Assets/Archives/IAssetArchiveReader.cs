namespace CreationsForge.Bethesda.Assets.Archives;

public interface IAssetArchiveReader
{
    bool CanRead(string archivePath);

    IReadOnlyList<AssetArchiveEntry> ListEntries(string archivePath);

    AssetArchiveReadResult ExtractEntry(string archivePath, string entryPath, string destinationDirectory);
}
