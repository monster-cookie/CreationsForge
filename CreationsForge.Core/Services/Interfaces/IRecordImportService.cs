using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Importers.Interfaces;

namespace CreationsForge.Core.Services.Interfaces;

public interface IRecordImportService
{
    RecordImportResultDTO ImportPluginRecords(
        PluginDTO plugin,
        IGameRecordReader recordReader,
        IProgress<GameImportProgressDTO>? progress = null,
        int pluginIndex = 0,
        int pluginCount = 0,
        CancellationToken cancellationToken = default);
}
