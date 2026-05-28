using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Serilog;
using SFRecordCompareEngine.Core.DTOs.Results;
using SFRecordCompareEngine.Core.Helpers;
using SFRecordCompareEngine.Core.Importers.Interfaces;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Importers.Starfield;

public class GameSettingImporter : ITypedRecordDetailImporter
{
    private readonly ILogger Logger = Log.ForContext<GameSettingImporter>();

    private readonly IGameSettingRepository GameSettingRepository;

    private readonly IStarfieldRecordReaderService StarfieldRecordReaderService;

    public GameRelease GameRelease => GameRelease.Starfield;

    public RecordType RecordType => new RecordType(RecordTypeCatalog.GameSetting.RecordID);

    public string TableName => RecordTypeCatalog.GameSetting.TableName;

    public GameSettingImporter(
        IGameSettingRepository gameSettingRepository,
        IStarfieldRecordReaderService starfieldRecordReaderService
    )
    {
        GameSettingRepository = gameSettingRepository;
        StarfieldRecordReaderService = starfieldRecordReaderService;
    }

    public void Import(ModKey modKey, FormKey formKey, RecordImportResultDTO resultDTO)
    {
        var record = StarfieldRecordReaderService.GetGameSetting(modKey, formKey);
        if (record == null)
        {
            Logger.Error("Failed to load GameSetting record with FormKey '{FormKey}' from mod '{ModKey}'", formKey, modKey);
            throw new FileNotFoundException($"Failed to load GameSetting record with FormKey '{formKey}' from mod '{modKey}'");
        }
        record.ImportedAtUTC = DateTime.UtcNow;

        GameSettingRepository.Save(record);
    }
}
