using NPoco;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Services.Interfaces;

public interface IRecordImportService
{
    RecordImportResultDTO ImportPluginRecords(IDatabase database, PluginMetadataDTO plugin, string importedAtUtc, CancellationToken cancellationToken);
}
