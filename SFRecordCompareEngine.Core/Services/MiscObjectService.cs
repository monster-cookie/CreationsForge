using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class MiscItemService : IMiscItemService
{
    private readonly IMiscItemRepository Repository;

    public MiscItemService(IMiscItemRepository repository)
    {
        Repository = repository;
    }

    public IList<MiscItemDTO> GetByModKey(ModKey modKey)
    {
        return Repository.GetByModKey(modKey);
    }

    public IList<MiscItemDTO> GetByFormKeyID(uint formKeyID)
    {
        return Repository.GetByFormKeyID(formKeyID);
    }
}