namespace CreationsForge.Core.DTOs.Records.Interfaces;

public interface IHasLocalizedStringsRecordDTO
{
    IList<LocalizedStringDTO> LocalizedStrings { get; set; }
}
