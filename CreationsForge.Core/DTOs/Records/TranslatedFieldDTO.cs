namespace CreationsForge.Core.DTOs.Records;

public class TranslatedFieldDTO
{
    public required string SourceField { get; set; }

    public TranslatedStringDTO? Value { get; set; }
}
