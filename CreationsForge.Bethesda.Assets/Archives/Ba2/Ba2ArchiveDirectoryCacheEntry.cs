namespace CreationsForge.Bethesda.Assets.Archives.Ba2;

internal class Ba2ArchiveDirectoryCacheEntry
{
    public Ba2ArchiveDirectoryCacheEntry(Ba2ArchiveDirectoryCacheKey cacheKey, Ba2ArchiveDirectory directory, long lastAccess)
    {
        CacheKey = cacheKey;
        Directory = directory;
        LastAccess = lastAccess;
    }

    public Ba2ArchiveDirectoryCacheKey CacheKey { get; }

    public Ba2ArchiveDirectory Directory { get; }

    public long LastAccess { get; set; }
}
