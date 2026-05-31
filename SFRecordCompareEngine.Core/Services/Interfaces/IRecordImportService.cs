using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Results;

namespace SFRecordCompareEngine.Core.Services.Interfaces;

public interface IRecordImportService
{
    RecordImportResultDTO ImportPluginRecords(PluginDTO plugin, IProgress<PluginImportProgressDTO>? progress, int pluginIndex, int pluginCount, CancellationToken cancellationToken);
}