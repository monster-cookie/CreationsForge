using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.Services.Interfaces;

public interface IRecordLocalizedStringImportService
{
    void ReplaceRecordLocalizedStrings(IHasLocalizedStringsRecordDTO record, string recordType);
}
