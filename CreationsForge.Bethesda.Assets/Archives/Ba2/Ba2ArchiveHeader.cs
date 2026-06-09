namespace CreationsForge.Bethesda.Assets.Archives.Ba2;

internal readonly struct Ba2ArchiveHeader
{
    public Ba2ArchiveHeader(uint version, long headerSize, uint fileCount, ulong nameTableOffset)
    {
        Version = version;
        HeaderSize = headerSize;
        FileCount = fileCount;
        NameTableOffset = nameTableOffset;
    }

    public uint Version { get; }

    public long HeaderSize { get; }

    public uint FileCount { get; }

    public ulong NameTableOffset { get; }
}
