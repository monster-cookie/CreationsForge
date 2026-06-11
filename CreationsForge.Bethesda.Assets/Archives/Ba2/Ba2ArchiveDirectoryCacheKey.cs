namespace CreationsForge.Bethesda.Assets.Archives.Ba2;

internal readonly struct Ba2ArchiveDirectoryCacheKey
{
    public Ba2ArchiveDirectoryCacheKey(string archivePath, long length, long lastWriteTimeUtcTicks)
    {
        ArchivePath = archivePath;
        Length = length;
        LastWriteTimeUtcTicks = lastWriteTimeUtcTicks;
    }

    public string ArchivePath { get; }

    public long Length { get; }

    public long LastWriteTimeUtcTicks { get; }
}
