using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class MiscItemService : IMiscItemService
{
    private readonly IMiscItemRepository MiscItemRepository;

    public MiscItemService(IMiscItemRepository repository)
    {
        MiscItemRepository = repository;
    }

    public IList<MiscItemDTO> GetByModKey(ModKey modKey) => MiscItemRepository.GetByModKey(modKey);
    public IList<MiscItemDTO> GetByFormKeyID(uint formKeyID) => MiscItemRepository.GetByFormKeyID(formKeyID);
}
