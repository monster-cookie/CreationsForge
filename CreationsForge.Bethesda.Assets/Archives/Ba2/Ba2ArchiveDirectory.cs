namespace CreationsForge.Bethesda.Assets.Archives.Ba2;

internal class Ba2ArchiveDirectory
{
    private Ba2ArchiveDirectory(
        Ba2ArchiveHeader header,
        IReadOnlyList<string> names,
        IReadOnlyList<Ba2ArchiveFileRecord> fileRecords,
        IReadOnlyList<Ba2ArchiveTextureRecord> textureRecords)
    {
        Header = header;
        Names = names;
        FileRecords = fileRecords;
        TextureRecords = textureRecords;
        EntryIndexes = CreateEntryIndexes(names);
    }

    public Ba2ArchiveHeader Header { get; }

    public IReadOnlyList<string> Names { get; }

    public IReadOnlyList<Ba2ArchiveFileRecord> FileRecords { get; }

    public IReadOnlyList<Ba2ArchiveTextureRecord> TextureRecords { get; }

    public IReadOnlyDictionary<string, int> EntryIndexes { get; }

    public static Ba2ArchiveDirectory CreateGeneral(Ba2ArchiveHeader header, IReadOnlyList<string> names, IReadOnlyList<Ba2ArchiveFileRecord> records)
    {
        return new Ba2ArchiveDirectory(header, names, records, Array.Empty<Ba2ArchiveTextureRecord>());
    }

    public static Ba2ArchiveDirectory CreateTexture(Ba2ArchiveHeader header, IReadOnlyList<string> names, IReadOnlyList<Ba2ArchiveTextureRecord> records)
    {
        return new Ba2ArchiveDirectory(header, names, Array.Empty<Ba2ArchiveFileRecord>(), records);
    }

    private static IReadOnlyDictionary<string, int> CreateEntryIndexes(IReadOnlyList<string> names)
    {
        var entryIndexes = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (var index = 0; index < names.Count; index++)
        {
            entryIndexes.TryAdd(names[index], index);
        }

        return entryIndexes;
    }
}
