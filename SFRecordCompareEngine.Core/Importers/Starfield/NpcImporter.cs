using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.DTOs.Results;
using SFRecordCompareEngine.Core.Helpers;
using SFRecordCompareEngine.Core.Importers.Interfaces;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Importers.Starfield;

public class NpcImporter : ITypedRecordDetailImporter
{
    private readonly INpcRepository NpcRepository;

    public GameRelease GameRelease => GameRelease.Starfield;
    public RecordType RecordType => new(RecordTypeCatalog.Npc.RecordID);
    public string TableName => RecordTypeCatalog.Npc.TableName;

    public NpcImporter(INpcRepository npcRepository)
    {
        NpcRepository = npcRepository;
    }

    public void Import(object recordDTO, RecordTypeImportResultDTO resultDTO)
    {
        var record = (NpcDTO)recordDTO;
        record.ImportedAtUTC = DateTime.UtcNow;
        NpcRepository.Save(record);
        resultDTO.DetailRowsImported++;
    }
}
