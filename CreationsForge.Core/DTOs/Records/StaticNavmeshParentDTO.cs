using CreationsForge.Core.DTOs.Plugins;

namespace CreationsForge.Core.DTOs.Records;

public class StaticNavmeshParentDTO
{
    public string? MutagenObjectType { get; set; }

    public FormKeyDTO? Parent { get; set; }
}
