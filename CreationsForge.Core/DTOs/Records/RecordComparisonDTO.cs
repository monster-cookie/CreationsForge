using CreationsForge.Core.DTOs.Plugins;

namespace CreationsForge.Core.DTOs.Records;

public class RecordComparisonDTO
{
    public string RecordType { get; set; } = string.Empty;

    public FormKeyDTO? FormKey { get; set; }

    public string EditorID { get; set; } = string.Empty;

    public IReadOnlyList<RecordComparisonColumnDTO> Columns { get; set; } = [];

    public IReadOnlyList<RecordComparisonFieldDTO> Fields { get; set; } = [];
}
