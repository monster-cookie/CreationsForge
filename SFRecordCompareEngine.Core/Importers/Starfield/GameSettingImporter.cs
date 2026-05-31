using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.DTOs.Results;
using SFRecordCompareEngine.Core.Helpers;
using SFRecordCompareEngine.Core.Importers.Interfaces;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Importers.Starfield;

public class GameSettingImporter : ITypedRecordDetailImporter
{
    private readonly IGameSettingRepository GameSettingRepository;

    public GameSettingImporter(IGameSettingRepository gameSettingRepository)
    {
        GameSettingRepository = gameSettingRepository;
    }

    public GameRelease GameRelease => GameRelease.Starfield;

    public RecordType RecordType => new(RecordTypeCatalog.GameSetting.RecordID);

    public string TableName => RecordTypeCatalog.GameSetting.TableName;

    public void Import(object recordDTO, RecordTypeImportResultDTO resultDTO)
    {
        var record = (GameSettingDTO)recordDTO;
        record.ImportedAtUTC = DateTime.UtcNow;

        GameSettingRepository.Save(record);
        resultDTO.DetailRowsImported++;
    }
}