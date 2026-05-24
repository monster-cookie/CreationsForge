using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IFactionRepository
{
    void Upsert(IDatabase database, FactionDTO faction);
    void ReplaceRelations(IDatabase database, string modKey, string formId, IList<FactionRelationDTO> relations);
}
