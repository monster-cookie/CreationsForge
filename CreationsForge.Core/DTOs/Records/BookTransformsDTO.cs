using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records.Metadata;

namespace CreationsForge.Core.DTOs.Records;

public class BookTransformsDTO
{
    [FormKeyColumnPrefix("Transforms_Inventory")]
    public FormKeyDTO? Inventory { get; set; }
}
