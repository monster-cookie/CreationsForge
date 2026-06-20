namespace CreationsForge.Core.DTOs.Records;

public class TranslatedStringDTO
{
    public string TargetLanguage { get; set; } = "English";

    public IList<TranslatedStringValueDTO> Strings { get; set; } = new List<TranslatedStringValueDTO>();
}
