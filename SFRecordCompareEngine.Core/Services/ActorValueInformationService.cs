using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class ActorValueInformationService : IActorValueInformationService
{
    private readonly IActorValueInformationRepository ActorValueInformationRepository;

    public ActorValueInformationService(IActorValueInformationRepository repository)
    {
        ActorValueInformationRepository = repository;
    }

    public IList<ActorValueInformationDTO> GetByModKey(ModKey modKey) => ActorValueInformationRepository.GetByModKey(modKey);
    public IList<ActorValueInformationDTO> GetByFormKeyID(uint formKeyID) => ActorValueInformationRepository.GetByFormKeyID(formKeyID);
}
