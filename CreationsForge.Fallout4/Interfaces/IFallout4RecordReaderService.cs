using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;

namespace CreationsForge.Fallout4.Interfaces;

public interface IFallout4RecordReaderService
{
    PluginRecordSetDTO ReadPluginRecords(PluginDTO plugin, CancellationToken cancellationToken = default);
}
