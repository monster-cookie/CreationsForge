using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.DTOs.Cache;

public class PersistedPluginCacheDTO
{
    public string PluginName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime LastWriteTimeUtc { get; set; }
    public IList<RecordReferenceCacheEntryDTO> Entries { get; set; } = new List<RecordReferenceCacheEntryDTO>();
}
