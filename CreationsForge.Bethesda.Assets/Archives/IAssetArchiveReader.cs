namespace CreationsForge.Bethesda.Assets.Archives;

public interface IAssetArchiveReader
{
    bool CanRead(string archivePath);

    IReadOnlyList<AssetArchiveEntry> ListEntries(string archivePath);

    AssetArchiveReadResult TryReadEntry(string archivePath, string entryPath);
}
