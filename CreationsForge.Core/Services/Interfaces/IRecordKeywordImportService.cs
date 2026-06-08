using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.Services.Interfaces;

public interface IRecordKeywordImportService
{
    void ReplaceRecordKeywords(IHasKeywordsRecordDTO record, string recordType);
}
