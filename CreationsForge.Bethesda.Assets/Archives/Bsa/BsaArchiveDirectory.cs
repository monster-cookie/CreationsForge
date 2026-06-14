namespace CreationsForge.Bethesda.Assets.Archives.Bsa;

internal class BsaArchiveDirectory
{
    public BsaArchiveDirectory(BsaArchiveHeader header, IReadOnlyList<BsaArchiveFileRecord> records)
    {
        Header = header;
        Records = records;
    }

    public BsaArchiveHeader Header { get; }

    public IReadOnlyList<BsaArchiveFileRecord> Records { get; }
}
