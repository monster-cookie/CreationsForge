using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

public class RecordComponentDTO
{
    public SupportedGame Game { get; set; }

    public ModKeyDTO ModKey { get; set; } = new() { Name = string.Empty, Type = 0, FileName = string.Empty };

    public FormKeyDTO FormKey { get; set; } = new()
    {
        ModKey = new ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty },
        Id = 0
    };

    public string RecordType { get; set; } = string.Empty;

    public int ComponentIndex { get; set; }

    public string MutagenObjectType { get; set; } = string.Empty;

    public DateTime ImportedAtUTC { get; set; }

    public IList<RecordComponentItemDTO> Items { get; set; } = new List<RecordComponentItemDTO>();
}
