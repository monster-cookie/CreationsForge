using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Services.Interfaces;

public interface IFormListService
{
    IList<FormListDTO> GetByModKey(ModKey modKey);
    IList<FormListDTO> GetByFormKeyID(uint formKeyID);
    IList<FormListItemDTO> GetItems(ModKey modKey, FormKey formKey);
}