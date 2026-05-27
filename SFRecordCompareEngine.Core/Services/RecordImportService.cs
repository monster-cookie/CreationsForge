using System.Configuration;
using System.Data;
using System.IO;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using NPoco;
using Serilog;
using SFRecordCompareEngine.Core.Configuration.Interfaces;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Results;
using SFRecordCompareEngine.Core.Importers.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

using StarfieldGameEnvironment = Mutagen.Bethesda.Environments.IGameEnvironment<Mutagen.Bethesda.Starfield.IStarfieldMod, Mutagen.Bethesda.Starfield.IStarfieldModGetter>;

namespace SFRecordCompareEngine.Core.Services;

public class RecordImportService : IRecordImportService
{
    private readonly ILogger Logger = Log.ForContext<RecordImportService>();

    private readonly Dictionary<(GameRelease GameRelease, RecordType RecordType), ITypedRecordDetailImporter> TypedRecordDetailImporters;

    public RecordImportService(IEnumerable<ITypedRecordDetailImporter> typedRecordDetailImporters)
    {
        TypedRecordDetailImporters = typedRecordDetailImporters.ToDictionary(importer => (importer.GameRelease, importer.RecordType));
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
        var mod = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(Path.Join(GameEnvironment.Typical.Starfield(StarfieldRelease.Starfield).DataFolderPath.Directory.ToString(), plugin.ModKey.FileName))
            .WithLoadOrderFromHeaderMasters()
            .WithDataFolder(GameEnvironment.Typical.Starfield(StarfieldRelease.Starfield).DataFolderPath)
            .Construct();
        if (mod == null) throw new DataException($"No plugin found in Starfield with ModKey {plugin.ModKey}");
        if (!mod.FormLists.Any()) return;

        var key = (GameRelease.Starfield, new RecordType("FLST"));
        if (!TypedRecordDetailImporters.TryGetValue(key, out var importer) || importer == null) return;

        foreach (var formListEntry in mod.FormLists)
        {
            importer.Import(mod.ModKey, formListEntry.FormKey, resultDTO);
        }
    }
}
