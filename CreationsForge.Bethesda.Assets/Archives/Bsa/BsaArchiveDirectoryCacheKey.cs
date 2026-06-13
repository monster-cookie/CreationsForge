namespace CreationsForge.Bethesda.Assets.Archives.Bsa;

internal readonly struct BsaArchiveDirectoryCacheKey
{
    public BsaArchiveDirectoryCacheKey(string archivePath, long length, long lastWriteTimeUtcTicks)
    {
        ArchivePath = archivePath;
        Length = length;
        LastWriteTimeUtcTicks = lastWriteTimeUtcTicks;
    }

    public string ArchivePath { get; }

    public long Length { get; }

    public long LastWriteTimeUtcTicks { get; }
}
