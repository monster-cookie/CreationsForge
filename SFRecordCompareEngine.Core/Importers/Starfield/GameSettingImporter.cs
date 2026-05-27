using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Results;
using SFRecordCompareEngine.Core.Helpers;
using SFRecordCompareEngine.Core.Importers.Interfaces;

namespace SFRecordCompareEngine.Core.Importers.Starfield;

public class GameSettingImporter : ITypedRecordDetailImporter
{
    public GameRelease GameRelease => GameRelease.Starfield;

    public RecordType RecordType => new RecordType(RecordTypeCatalog.GameSetting.RecordID);

    public string TableName => RecordTypeCatalog.GameSetting.TableName;

    public void Import(ModKey modKey, FormKey formKey, RecordImportResultDTO resultDTO)
    {
        var test = formKey.ID;
        // TODO: Need to handle the header row table

        // TODO: Need to handle the details data in the FormList table

        throw new NotImplementedException();
    }
}