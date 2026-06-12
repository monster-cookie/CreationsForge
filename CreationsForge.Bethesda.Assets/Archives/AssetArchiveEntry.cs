namespace CreationsForge.Bethesda.Assets.Archives;

public class AssetArchiveEntry
{
    public required string ArchivePath { get; set; }

    public required string EntryPath { get; set; }

    public long UnpackedSize { get; set; }

    public long PackedSize { get; set; }
}
