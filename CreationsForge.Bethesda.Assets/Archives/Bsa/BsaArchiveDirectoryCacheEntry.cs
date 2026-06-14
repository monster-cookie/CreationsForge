namespace CreationsForge.Bethesda.Assets.Archives.Bsa;

internal class BsaArchiveDirectoryCacheEntry
{
    public BsaArchiveDirectoryCacheEntry(BsaArchiveDirectoryCacheKey cacheKey, BsaArchiveDirectory directory, long lastAccess)
    {
        CacheKey = cacheKey;
        Directory = directory;
        LastAccess = lastAccess;
    }

    public BsaArchiveDirectoryCacheKey CacheKey { get; }

    public BsaArchiveDirectory Directory { get; }

    public long LastAccess { get; set; }
}
