namespace CreationsForge.Core.DTOs.Plugins;

public class PluginSourceInfoDTO
{
    public required bool Exists { get; set; }

    public required long LastWriteUTCTicks { get; set; }

    public required long FileSizeBytes { get; set; }
}
