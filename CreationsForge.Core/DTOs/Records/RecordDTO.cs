using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

public abstract class RecordDTO : IHasModKey, IHasFormKey, IHasEditorID, IHasLocalizedStringsRecordDTO
{
    public required SupportedGame Game { get; set; }

    public required ModKeyDTO ModKey { get; set; }

    public required FormKeyDTO FormKey { get; set; }

    public required string EditorID { get; set; }

    public required int FormVersion { get; set; }

    public required int MajorRecordFlags { get; set; }

    public int? Version2 { get; set; }

    public int? VersionControl { get; set; }

    public required DateTime ImportedAtUTC { get; set; }

    public IList<LocalizedStringDTO> LocalizedStrings { get; set; } = new List<LocalizedStringDTO>();
}
