using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IActorValueInformationRepository
{
    IList<ActorValueInformationDTO> GetByModKey(ModKey modKey);
    IList<ActorValueInformationDTO> GetByFormKey(FormKey formKey);
    void Save(ActorValueInformationDTO dto);
}