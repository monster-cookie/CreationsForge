using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.Services.Interfaces;

public interface IKeywordMappingImportService
{
    void ReplaceKeywordMappings(IKeywords record, string recordType);
}
