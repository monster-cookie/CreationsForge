using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.DTOs.Results;
using SFRecordCompareEngine.Core.Helpers;
using SFRecordCompareEngine.Core.Importers.Interfaces;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Importers.Starfield;

public class PerkImporter : ITypedRecordDetailImporter
{
    private readonly IPerkRepository Repository;
    private readonly IScriptingAdapterImportService ScriptingAdapterImportService;

    public PerkImporter(IPerkRepository repository, IScriptingAdapterImportService scriptingAdapterImportService)
    {
        Repository = repository;
        ScriptingAdapterImportService = scriptingAdapterImportService;
    }

    public GameRelease GameRelease => GameRelease.Starfield;
    public RecordType RecordType => new(RecordTypeCatalog.Perk.RecordID);
    public string TableName => RecordTypeCatalog.Perk.TableName;

    public void Import(object recordDTO, RecordTypeImportResultDTO resultDTO)
    {
        var record = (PerkDTO)recordDTO;
        record.ImportedAtUTC = DateTime.UtcNow;
        Repository.Save(record);
        ScriptingAdapterImportService.ReplaceRecordScriptingAdapters(record, RecordTypeCatalog.Perk.RecordType);
        resultDTO.DetailRowsImported++;
    }
}
