namespace CreationsForge.Bethesda.Assets.Archives.Ba2;

internal readonly struct Ba2ArchiveHeader
{
    public Ba2ArchiveHeader(uint version, Ba2ArchiveType archiveType, long headerSize, uint fileCount, ulong nameTableOffset)
    {
        Version = version;
        ArchiveType = archiveType;
        HeaderSize = headerSize;
        FileCount = fileCount;
        NameTableOffset = nameTableOffset;
    }

    public uint Version { get; }

    public Ba2ArchiveType ArchiveType { get; }

    public long HeaderSize { get; }

    public uint FileCount { get; }

    public ulong NameTableOffset { get; }
}
