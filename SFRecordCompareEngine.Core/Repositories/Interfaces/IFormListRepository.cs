using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IFormListRepository
{
    void UpsertFormList(IDatabase database, FormListDTO formList);
    void ReplaceItems(IDatabase database, ModKey modKey, string formId, IList<FormListItemDTO> items);
}
