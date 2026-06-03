using SFRecordCompareEngine.Core.DTOs.Records.Interfaces;

namespace SFRecordCompareEngine.Core.Services.Interfaces;

public interface IScriptingAdapterImportService
{
    void ReplaceRecordScriptingAdapters(IHasScriptingAdaptersRecordDTO record, string recordType);
}
