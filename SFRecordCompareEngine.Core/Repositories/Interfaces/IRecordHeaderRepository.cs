using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IRecordHeaderRepository
{
    RecordHeaderDTO? GetCurrentByFormKey(IDatabase database, string formKey);
    void Upsert(IDatabase database, RecordHeaderDTO recordHeader);
    void DeleteByModKey(IDatabase database, ModKey modKey);
}
