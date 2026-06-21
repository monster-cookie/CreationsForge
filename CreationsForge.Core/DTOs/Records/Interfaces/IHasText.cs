using CreationsForge.Core.DTOs.Records;

namespace CreationsForge.Core.DTOs.Records.Interfaces;

public interface IHasText
{
    TranslatedStringDTO? Text { get; set; }
}
