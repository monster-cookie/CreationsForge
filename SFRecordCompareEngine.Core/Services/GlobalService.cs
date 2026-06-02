using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class GlobalService : IGlobalService
{
    private readonly IGlobalRepository Repository;
    private readonly IScriptingAdapterHydrationService ScriptingAdapterHydrationService;

    public GlobalService(IGlobalRepository repository, IScriptingAdapterHydrationService scriptingAdapterHydrationService)
    {
        Repository = repository;
        ScriptingAdapterHydrationService = scriptingAdapterHydrationService;
    }

    public IList<GlobalDTO> GetByModKey(ModKey modKey)
    {
        return ScriptingAdapterHydrationService.Hydrate(Repository.GetByModKey(modKey), Helpers.RecordTypeCatalog.Global.RecordType);
    }

    public IList<GlobalDTO> GetByFormKeyID(uint formKeyID)
    {
        return ScriptingAdapterHydrationService.Hydrate(Repository.GetByFormKeyID(formKeyID), Helpers.RecordTypeCatalog.Global.RecordType);
    }
}
