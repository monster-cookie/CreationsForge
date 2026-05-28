using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Results;
using SFRecordCompareEngine.Core.Helpers;
using SFRecordCompareEngine.Core.Importers.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class RecordImportService : IRecordImportService
{
    private readonly Dictionary<(GameRelease GameRelease, RecordType RecordType), ITypedRecordDetailImporter> TypedRecordDetailImporters;
    private readonly IStarfieldRecordReaderService StarfieldRecordReaderService;

    public RecordImportService(
        IEnumerable<ITypedRecordDetailImporter> typedRecordDetailImporters,
        IStarfieldRecordReaderService starfieldRecordReaderService)
    {
        TypedRecordDetailImporters = typedRecordDetailImporters.ToDictionary(importer => (importer.GameRelease, importer.RecordType));
        StarfieldRecordReaderService = starfieldRecordReaderService;
    }

    public RecordImportResultDTO ImportPluginRecords(PluginDTO plugin, CancellationToken cancellationToken)
    {
        var importResult = new RecordImportResultDTO
        {
            ModKey = plugin.ModKey
        };
        
        ImportStarfieldPluginRecords(plugin, importResult, cancellationToken);

        return importResult;
    }

    private void ImportStarfieldPluginRecords(PluginDTO plugin, RecordImportResultDTO resultDTO, CancellationToken cancellationToken)
    {
        ImportStarfieldPluginRecordType(plugin, resultDTO, RecordTypeCatalog.FormList.RecordID, StarfieldRecordReaderService.GetFormListFormKeys(plugin), cancellationToken);
        ImportStarfieldPluginRecordType(plugin, resultDTO, RecordTypeCatalog.GameSetting.RecordID, StarfieldRecordReaderService.GetGameSettingFormKeys(plugin), cancellationToken);
    }

    private void ImportStarfieldPluginRecordType(PluginDTO plugin, RecordImportResultDTO resultDTO, string recordID, IReadOnlyList<FormKey> formKeys, CancellationToken cancellationToken)
    {
        if (!formKeys.Any()) return;

        var key = (GameRelease.Starfield, new RecordType(recordID));
        if (!TypedRecordDetailImporters.TryGetValue(key, out var importer) || importer == null) return;

        foreach (var formKey in formKeys)
        {
            cancellationToken.ThrowIfCancellationRequested();
            importer.Import(plugin.ModKey, formKey, resultDTO);
        }
    }
}
