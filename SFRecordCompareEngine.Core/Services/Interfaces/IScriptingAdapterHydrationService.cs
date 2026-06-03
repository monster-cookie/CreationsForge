using SFRecordCompareEngine.Core.DTOs.Records.Interfaces;

namespace SFRecordCompareEngine.Core.Services.Interfaces;

public interface IScriptingAdapterHydrationService
{
    IList<TRecord> Hydrate<TRecord>(IList<TRecord> records, string recordType) where TRecord : IHasScriptingAdaptersRecordDTO;
}
