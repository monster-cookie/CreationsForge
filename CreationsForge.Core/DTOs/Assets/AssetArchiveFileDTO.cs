using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Assets;

public class AssetArchiveFileDTO
{
    public required SupportedGame Game { get; set; }

    public required string DataFolder { get; set; }

    public required string ArchivePath { get; set; }

    public required string ArchiveFileName { get; set; }

    public required string ArchiveExtension { get; set; }

    public required string ArchiveType { get; set; }

    public long SourceLastWriteUTCTicks { get; set; }

    public long SourceFileSizeBytes { get; set; }

    public required DateTime IndexedAtUTC { get; set; }
}
