using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Helpers;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class ActorValueInformationRepository : RecordHeaderRepository<ActorValueInformation, ActorValueInformationDTO>, IActorValueInformationRepository
{
    public ActorValueInformationRepository(IDatabase database)
        : base(database, RecordTypeCatalog.ActorValueInformation.TableName)
    { }

    protected override ActorValueInformationDTO CreateDTO(ActorValueInformation model) => new(model);
    protected override ActorValueInformation CreateModel(ActorValueInformationDTO dto) => new(dto);
}
