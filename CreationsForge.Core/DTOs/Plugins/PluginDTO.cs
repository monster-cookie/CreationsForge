using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Plugins;

public class PluginDTO
{
    public required SupportedGame Game { get; set; }

    public required ModKeyDTO ModKey { get; set; }

    public required int LoadOrderIndex { get; set; }

    public required bool Enabled { get; set; }

    public required bool ExistsOnDisk { get; set; }

    public required PluginImportState ImportState { get; set; }

    public required int HeaderFlags { get; set; }

    public required int FormVersion { get; set; }

    public string? Author { get; set; }

    public string? Description { get; set; }

    public string? ImportMessage { get; set; }

    public string? ImportDetails { get; set; }

    public required int RecordCount { get; set; }

    public required long SourceLastWriteUTCTicks { get; set; }

    public required long SourceFileSizeBytes { get; set; }

    public required DateTime LastCheckedUTC { get; set; }

    public DateTime? LastImportedUTC { get; set; }

    public DateTime? InvalidatedAtUTC { get; set; }
}
