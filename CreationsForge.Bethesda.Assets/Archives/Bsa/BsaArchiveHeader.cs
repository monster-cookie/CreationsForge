namespace CreationsForge.Bethesda.Assets.Archives.Bsa;

internal readonly struct BsaArchiveHeader
{
    public BsaArchiveHeader(
        uint version,
        uint folderRecordOffset,
        uint archiveFlags,
        uint folderCount,
        uint fileCount,
        uint totalFolderNameLength,
        uint totalFileNameLength)
    {
        Version = version;
        FolderRecordOffset = folderRecordOffset;
        ArchiveFlags = archiveFlags;
        FolderCount = folderCount;
        FileCount = fileCount;
        TotalFolderNameLength = totalFolderNameLength;
        TotalFileNameLength = totalFileNameLength;
    }

    public uint Version { get; }

    public uint FolderRecordOffset { get; }

    public uint ArchiveFlags { get; }

    public uint FolderCount { get; }

    public uint FileCount { get; }

    public uint TotalFolderNameLength { get; }

    public uint TotalFileNameLength { get; }

    public bool HasDirectoryNames => (ArchiveFlags & 0x1) != 0;

    public bool HasFileNames => (ArchiveFlags & 0x2) != 0;

    public bool IsCompressedByDefault => (ArchiveFlags & 0x4) != 0;

    public bool HasEmbeddedFileNames => (ArchiveFlags & 0x100) != 0;
}
