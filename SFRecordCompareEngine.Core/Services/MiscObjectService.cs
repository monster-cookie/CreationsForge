using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class MiscItemService : IMiscItemService
{
    private readonly IMiscItemRepository Repository;
    private readonly IScriptingAdapterHydrationService ScriptingAdapterHydrationService;

    public MiscItemService(IMiscItemRepository repository, IScriptingAdapterHydrationService scriptingAdapterHydrationService)
    {
        Repository = repository;
        ScriptingAdapterHydrationService = scriptingAdapterHydrationService;
    }

    public IList<MiscItemDTO> GetByModKey(ModKey modKey)
    {
        return ScriptingAdapterHydrationService.Hydrate(Repository.GetByModKey(modKey), Helpers.RecordTypeCatalog.MiscItem.RecordType);
    }

    public IList<MiscItemDTO> GetByFormKeyID(uint formKeyID)
    {
        return ScriptingAdapterHydrationService.Hydrate(Repository.GetByFormKeyID(formKeyID), Helpers.RecordTypeCatalog.MiscItem.RecordType);
    }
}
