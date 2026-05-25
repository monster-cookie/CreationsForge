using Mutagen.Bethesda.Plugins;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class CellGroupLocationDTO
{
    public required ModKey ModKey { get; set; }
    public required string CellFormID { get; set; }
    public int LocationIndex { get; set; }
    public required string LocationKind { get; set; }
    public string? WorldspaceFormID { get; set; }
    public int? BlockNumber { get; set; }
    public int? SubBlockNumber { get; set; }
    public int? BlockX { get; set; }
    public int? BlockY { get; set; }
    public int? SubBlockX { get; set; }
    public int? SubBlockY { get; set; }
    public int? CellIndex { get; set; }
    public string? BlockGroupType { get; set; }
    public string? SubBlockGroupType { get; set; }
    public int? BlockLastModified { get; set; }
    public int? SubBlockLastModified { get; set; }
    public int? BlockUnknown { get; set; }
    public int? SubBlockUnknown { get; set; }
    public required string ImportedAtUtc { get; set; }
}
