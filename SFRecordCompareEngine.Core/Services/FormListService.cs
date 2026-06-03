using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class FormListService : IFormListService
{
    private readonly IFormListItemRepository FormListItemRepository;
    private readonly IFormListRepository FormListRepository;

    public FormListService(
        IFormListRepository formListRepository,
        IFormListItemRepository formListItemRepository)
    {
        FormListRepository = formListRepository;
        FormListItemRepository = formListItemRepository;
    }

    public IList<FormListDTO> GetByModKey(ModKey modKey)
    {
        return FormListRepository.GetByModKey(modKey);
    }

    public IList<RecordTreeEntryDTO> GetRecordTreeEntriesByModKey(ModKey modKey)
    {
        return FormListRepository.GetRecordTreeEntriesByModKey(modKey);
    }

    public IList<FormListDTO> GetByFormKey(FormKey formKey)
    {
        return FormListRepository.GetByFormKey(formKey);
    }

    public IList<FormListItemDTO> GetItems(ModKey modKey, FormKey formKey)
    {
        return FormListItemRepository.GetByFormList(modKey, formKey);
    }
}
