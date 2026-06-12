using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Assets;

public class AssetArchiveEntryDTO
{
    public required SupportedGame Game { get; set; }

    public required string ArchivePath { get; set; }

    public required string NormalizedEntryPath { get; set; }

    public required string RootFolder { get; set; }

    public required string Extension { get; set; }

    public long PackedSize { get; set; }

    public long UnpackedSize { get; set; }
}
