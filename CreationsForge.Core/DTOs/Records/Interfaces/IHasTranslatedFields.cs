namespace CreationsForge.Core.DTOs.Records.Interfaces;

public interface IHasTranslatedFields
{
    IEnumerable<TranslatedFieldDTO> GetTranslatedFields();
}
