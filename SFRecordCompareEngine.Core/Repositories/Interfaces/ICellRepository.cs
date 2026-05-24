using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface ICellRepository
{
    void Upsert(IDatabase database, CellDTO cell);
    void ReplaceGroupLocations(IDatabase database, string modKey, string cellFormId, IList<CellGroupLocationDTO> locations);
    void ReplacePlacedRecords(IDatabase database, string modKey, string cellFormId, IList<CellPlacedRecordDTO> placedRecords);
}
