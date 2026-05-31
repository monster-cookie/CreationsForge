using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
namespace SFRecordCompareEngine.Core.Services.Interfaces;
public interface IActorValueInformationService { IList<ActorValueInformationDTO> GetByModKey(ModKey modKey); IList<ActorValueInformationDTO> GetByFormKeyID(uint formKeyID); }
