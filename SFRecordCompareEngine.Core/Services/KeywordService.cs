using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class KeywordService : IKeywordService
{
    private readonly IKeywordRepository Repository;
    private readonly IScriptingAdapterHydrationService ScriptingAdapterHydrationService;

    public KeywordService(IKeywordRepository repository, IScriptingAdapterHydrationService scriptingAdapterHydrationService)
    {
        Repository = repository;
        ScriptingAdapterHydrationService = scriptingAdapterHydrationService;
    }

    public IList<KeywordDTO> GetByModKey(ModKey modKey)
    {
        return ScriptingAdapterHydrationService.Hydrate(Repository.GetByModKey(modKey), Helpers.RecordTypeCatalog.Keyword.RecordType);
    }

    public IList<KeywordDTO> GetByFormKeyID(uint formKeyID)
    {
        return ScriptingAdapterHydrationService.Hydrate(Repository.GetByFormKeyID(formKeyID), Helpers.RecordTypeCatalog.Keyword.RecordType);
    }
}
