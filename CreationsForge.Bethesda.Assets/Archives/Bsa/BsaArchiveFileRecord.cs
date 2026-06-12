namespace CreationsForge.Bethesda.Assets.Archives.Bsa;

internal readonly struct BsaArchiveFileRecord
{
    public BsaArchiveFileRecord(string entryPath, uint storedSize, uint dataOffset, bool compressionToggled)
    {
        EntryPath = entryPath;
        StoredSize = storedSize;
        DataOffset = dataOffset;
        CompressionToggled = compressionToggled;
    }

    public string EntryPath { get; }

    public uint StoredSize { get; }

    public uint DataOffset { get; }

    public bool CompressionToggled { get; }
}
