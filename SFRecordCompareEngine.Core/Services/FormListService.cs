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

    public IList<FormListDTO> GetByFormKeyID(uint formKeyID)
    {
        return FormListRepository.GetByFormKeyID(formKeyID);
    }

    public IList<FormListItemDTO> GetItems(ModKey modKey, FormKey formKey)
    {
        return FormListItemRepository.GetByFormList(modKey, formKey);
    }
}