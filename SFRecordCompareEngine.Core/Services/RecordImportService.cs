using System.Configuration;
using System.Data;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Mapping;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Starfield.Assets;
using NPoco;
using Serilog;
using SFRecordCompareEngine.Core.Configuration.Interfaces;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Importers.Interfaces;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class RecordImportService : IRecordImportService
{
    private readonly ILogger Logger = Log.ForContext<RecordImportService>();

    private readonly IDatabase Database;
    private readonly IGameConfigurationStore GameConfigurationStore;
    private readonly IRecordHeaderRepository RecordHeaderRepository;

    private readonly Dictionary<(GameRelease GameRelease, RecordType RecordType), ITypedRecordDetailImporter> TypedRecordDetailImporters;

    public RecordImportService(
        IGameConfigurationStore gameConfigurationStore,
        IDatabase database,
        IRecordHeaderRepository recordHeaderRepository,
        IEnumerable<ITypedRecordDetailImporter> typedRecordDetailImporters
    )
    {
        GameConfigurationStore = gameConfigurationStore ?? throw new ArgumentNullException(nameof(gameConfigurationStore));
        Database = database ?? throw new ArgumentNullException(nameof(database));
        RecordHeaderRepository = recordHeaderRepository ?? throw new ArgumentNullException(nameof(recordHeaderRepository));

        TypedRecordDetailImporters = typedRecordDetailImporters.ToDictionary(importer => (importer.GameRelease, importer.RecordType));
    }

    public RecordImportResultDTO ImportPluginRecords(PluginDTO plugin, CancellationToken cancellationToken)
    {
        if (GameConfigurationStore.SelectedGame == null) throw new ConfigurationErrorsException("No game selected in configuration (SelectedGame is null)");
        if (GameConfigurationStore.Game == null) throw new ConfigurationErrorsException("No game selected in configuration (Game is null)");
        if (GameConfigurationStore.Release == null) throw new ConfigurationErrorsException("No game selected in configuration (Release is null)");

        RecordImportResultDTO importResult = new RecordImportResultDTO
        {
            ModKey = plugin.ModKey
        };
        switch (GameConfigurationStore.SelectedGame)
        {
            case "Skyrim":
                throw new NotImplementedException();

            case "Starfield":
                ImportStarfieldPluginRecords(plugin, importResult, cancellationToken);
                break;
            case "Fallout4":

                break;
            default:
                throw new ConfigurationErrorsException($"Unsupported game: {GameConfigurationStore.SelectedGame}");
        }

        return importResult;
    }

    private void ImportStarfieldPluginRecords(PluginDTO plugin, RecordImportResultDTO resultDTO, CancellationToken cancellationToken)
    {
        if (GameConfigurationStore.SelectedGame == null) throw new ConfigurationErrorsException("No game selected in configuration (SelectedGame is null)");
        if (GameConfigurationStore.Game == null) throw new ConfigurationErrorsException("No game selected in configuration (Game is null)");
        if (GameConfigurationStore.Release == null) throw new ConfigurationErrorsException("No game selected in configuration (Release is null)");

        var mod = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(plugin.PluginPath)
            .WithLoadOrderFromHeaderMasters()
            .WithDataFolder(GameConfigurationStore.Game.DataFolderPath)
            .Construct();
        if (mod == null) throw new DataException($"No plugin found in Starfield with ModKey {plugin.ModKey}");

        if (mod.FormLists.Any())
        {
            var key = (GameRelease.Starfield, new RecordType("FLST"));
            if (TypedRecordDetailImporters.TryGetValue(key, out var importer) && importer != null)
            {
                foreach (var formListEntry in mod.FormLists)
                {
                    importer.Import(mod.ModKey, formListEntry.FormKey);
                }
            }

        }
    }
}
