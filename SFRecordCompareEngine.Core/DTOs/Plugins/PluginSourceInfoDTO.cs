namespace SFRecordCompareEngine.Core.DTOs.Plugins;

public class PluginSourceInfoDTO
{
    public bool Exists { get; set; }
    public long LastWriteUTCTicks { get; set; }
    public long FileSizeBytes { get; set; }
}
