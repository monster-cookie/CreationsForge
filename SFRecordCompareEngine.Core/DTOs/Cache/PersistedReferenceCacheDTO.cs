namespace SFRecordCompareEngine.Core.DTOs.Cache;

public class PersistedReferenceCacheDTO
{
    public int CacheVersion { get; set; }
    public string? Game { get; set; }
    public IList<PersistedPluginCacheDTO> Plugins { get; set; } = new List<PersistedPluginCacheDTO>();
}
