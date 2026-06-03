using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class PerkService : IPerkService
{
    private readonly IPerkRepository Repository;
    private readonly IScriptingAdapterHydrationService ScriptingAdapterHydrationService;

    public PerkService(IPerkRepository repository, IScriptingAdapterHydrationService scriptingAdapterHydrationService)
    {
        Repository = repository;
        ScriptingAdapterHydrationService = scriptingAdapterHydrationService;
    }

    public IList<PerkDTO> GetByModKey(ModKey modKey)
    {
        return ScriptingAdapterHydrationService.Hydrate(Repository.GetByModKey(modKey), Helpers.RecordTypeCatalog.Perk.RecordType);
    }

    public IList<RecordTreeEntryDTO> GetRecordTreeEntriesByModKey(ModKey modKey)
    {
        return Repository.GetRecordTreeEntriesByModKey(modKey);
    }

    public IList<PerkDTO> GetByFormKey(FormKey formKey)
    {
        return ScriptingAdapterHydrationService.Hydrate(Repository.GetByFormKey(formKey), Helpers.RecordTypeCatalog.Perk.RecordType);
    }
}
