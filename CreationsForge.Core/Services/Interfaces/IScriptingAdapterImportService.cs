using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.Services.Interfaces;

public interface IScriptingAdapterImportService
{
    void ReplaceRecordScriptingAdapters(IHasScriptingAdaptersRecordDTO record, string recordType);
}
