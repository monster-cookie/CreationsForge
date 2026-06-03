using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class ActorValueInformationService : IActorValueInformationService
{
    private readonly IActorValueInformationRepository Repository;
    private readonly IScriptingAdapterHydrationService ScriptingAdapterHydrationService;

    public ActorValueInformationService(IActorValueInformationRepository repository, IScriptingAdapterHydrationService scriptingAdapterHydrationService)
    {
        Repository = repository;
        ScriptingAdapterHydrationService = scriptingAdapterHydrationService;
    }

    public IList<ActorValueInformationDTO> GetByModKey(ModKey modKey)
    {
        return ScriptingAdapterHydrationService.Hydrate(Repository.GetByModKey(modKey), Helpers.RecordTypeCatalog.ActorValueInformation.RecordType);
    }

    public IList<ActorValueInformationDTO> GetByFormKey(FormKey formKey)
    {
        return ScriptingAdapterHydrationService.Hydrate(Repository.GetByFormKey(formKey), Helpers.RecordTypeCatalog.ActorValueInformation.RecordType);
    }
}
