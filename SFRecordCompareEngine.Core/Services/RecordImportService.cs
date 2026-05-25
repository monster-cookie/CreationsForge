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

    private IGameConfigurationStore GameConfigurationStore { get; }
    private IRecordHeaderRepository RecordHeaderRepository { get; }
    private Dictionary<string, ITypedRecordDetailImporter> TypedRecordDetailImporters { get; }
    
    public RecordImportService(IGameConfigurationStore gameConfigurationStore, IRecordHeaderRepository recordHeaderRepository, IEnumerable<ITypedRecordDetailImporter> typedRecordDetailImporters)
    {
        GameConfigurationStore = gameConfigurationStore ?? throw new ArgumentNullException(nameof(gameConfigurationStore));
        RecordHeaderRepository = recordHeaderRepository ?? throw new ArgumentNullException(nameof(recordHeaderRepository));
        TypedRecordDetailImporters = typedRecordDetailImporters.ToDictionary(importer => importer.RecordType, StringComparer.Ordinal);
    }
    
    public RecordImportResultDTO ImportPluginRecords(IDatabase database, PluginDTO plugin, string importedAtUtc, CancellationToken cancellationToken)
    {
        // TODO: Implement record import logic using mutagen safe and multi-threaded.
        throw new NotImplementedException();
    }
}
