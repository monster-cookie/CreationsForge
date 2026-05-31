using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.DTOs.Results;
using SFRecordCompareEngine.Core.Helpers;
using SFRecordCompareEngine.Core.Importers.Interfaces;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Importers.Starfield;

public class MiscItemImporter : ITypedRecordDetailImporter
{
    private readonly IMiscItemRepository MiscItemRepository;

    public GameRelease GameRelease => GameRelease.Starfield;
    public RecordType RecordType => new(RecordTypeCatalog.MiscItem.RecordID);
    public string TableName => RecordTypeCatalog.MiscItem.TableName;

    public MiscItemImporter(IMiscItemRepository miscItemRepository)
    {
        MiscItemRepository = miscItemRepository;
    }

    public void Import(object recordDTO, RecordTypeImportResultDTO resultDTO)
    {
        var record = (MiscItemDTO)recordDTO;
        record.ImportedAtUTC = DateTime.UtcNow;
        MiscItemRepository.Save(record);
        resultDTO.DetailRowsImported++;
    }
}
