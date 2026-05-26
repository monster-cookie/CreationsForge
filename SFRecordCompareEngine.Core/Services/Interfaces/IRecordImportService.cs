using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Services.Interfaces;

public interface IRecordImportService
{
    RecordImportResultDTO ImportPluginRecords(PluginDTO plugin, CancellationToken cancellationToken);
}
