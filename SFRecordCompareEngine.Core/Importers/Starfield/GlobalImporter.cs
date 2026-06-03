using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.DTOs.Results;
using SFRecordCompareEngine.Core.Helpers;
using SFRecordCompareEngine.Core.Importers.Interfaces;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Importers.Starfield;

public class GlobalImporter : ITypedRecordDetailImporter
{
    private readonly IGlobalRepository Repository;
    private readonly IScriptingAdapterImportService ScriptingAdapterImportService;

    public GlobalImporter(IGlobalRepository repository, IScriptingAdapterImportService scriptingAdapterImportService)
    {
        Repository = repository;
        ScriptingAdapterImportService = scriptingAdapterImportService;
    }

    public GameRelease GameRelease => GameRelease.Starfield;
    public RecordType RecordType => new(RecordTypeCatalog.Global.RecordID);
    public string TableName => RecordTypeCatalog.Global.TableName;

    public void Import(object recordDTO, RecordTypeImportResultDTO resultDTO)
    {
        var record = (GlobalDTO)recordDTO;
        record.ImportedAtUTC = DateTime.UtcNow;
        Repository.Save(record);
        ScriptingAdapterImportService.ReplaceRecordScriptingAdapters(record, RecordTypeCatalog.Global.RecordType);
        resultDTO.DetailRowsImported++;
    }
}
