using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.DTOs.Results;
using SFRecordCompareEngine.Core.Helpers;
using SFRecordCompareEngine.Core.Importers.Interfaces;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Importers.Starfield;

public class ActorValueInformationImporter : ITypedRecordDetailImporter
{
    private readonly IActorValueInformationRepository ActorValueInformationRepository;

    public GameRelease GameRelease => GameRelease.Starfield;
    public RecordType RecordType => new(RecordTypeCatalog.ActorValueInformation.RecordID);
    public string TableName => RecordTypeCatalog.ActorValueInformation.TableName;

    public ActorValueInformationImporter(IActorValueInformationRepository actorValueInformationRepository)
    {
        ActorValueInformationRepository = actorValueInformationRepository;
    }

    public void Import(object recordDTO, RecordTypeImportResultDTO resultDTO)
    {
        var record = (ActorValueInformationDTO)recordDTO;
        record.ImportedAtUTC = DateTime.UtcNow;
        ActorValueInformationRepository.Save(record);
        resultDTO.DetailRowsImported++;
    }
}
