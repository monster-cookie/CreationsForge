using CreationsForge.Core.DTOs.Records;

namespace CreationsForge.Core.DTOs.Records.Interfaces;

public interface IHasName
{
    TranslatedStringDTO? Name { get; set; }
}
