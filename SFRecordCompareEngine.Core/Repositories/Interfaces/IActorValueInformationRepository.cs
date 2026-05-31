using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IActorValueInformationRepository
{
    IList<ActorValueInformationDTO> GetByModKey(ModKey modKey);
    IList<ActorValueInformationDTO> GetByFormKeyID(uint formKeyID);
    void Save(ActorValueInformationDTO dto);
}