using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.DTOs.Results;
using SFRecordCompareEngine.Core.Helpers;
using SFRecordCompareEngine.Core.Importers.Interfaces;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Importers.Starfield;

public class PerkImporter : ITypedRecordDetailImporter
{
    private readonly IPerkRepository PerkRepository;

    public GameRelease GameRelease => GameRelease.Starfield;
    public RecordType RecordType => new(RecordTypeCatalog.Perk.RecordID);
    public string TableName => RecordTypeCatalog.Perk.TableName;

    public PerkImporter(IPerkRepository perkRepository)
    {
        PerkRepository = perkRepository;
    }

    public void Import(object recordDTO, RecordTypeImportResultDTO resultDTO)
    {
        var record = (PerkDTO)recordDTO;
        record.ImportedAtUTC = DateTime.UtcNow;
        PerkRepository.Save(record);
        resultDTO.DetailRowsImported++;
    }
}
