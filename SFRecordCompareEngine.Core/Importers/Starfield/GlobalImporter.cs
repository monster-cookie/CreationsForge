using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.DTOs.Results;
using SFRecordCompareEngine.Core.Helpers;
using SFRecordCompareEngine.Core.Importers.Interfaces;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Importers.Starfield;

public class GlobalImporter : ITypedRecordDetailImporter
{
    private readonly IGlobalRepository GlobalRepository;

    public GameRelease GameRelease => GameRelease.Starfield;
    public RecordType RecordType => new(RecordTypeCatalog.Global.RecordID);
    public string TableName => RecordTypeCatalog.Global.TableName;

    public GlobalImporter(IGlobalRepository globalRepository)
    {
        GlobalRepository = globalRepository;
    }

    public void Import(object recordDTO, RecordTypeImportResultDTO resultDTO)
    {
        var record = (GlobalDTO)recordDTO;
        record.ImportedAtUTC = DateTime.UtcNow;
        GlobalRepository.Save(record);
        resultDTO.DetailRowsImported++;
    }
}
