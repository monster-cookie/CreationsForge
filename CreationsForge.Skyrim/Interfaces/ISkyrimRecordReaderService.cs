using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;

namespace CreationsForge.Skyrim.Interfaces;

public interface ISkyrimRecordReaderService
{
    PluginRecordSetDTO ReadPluginRecords(PluginDTO plugin, CancellationToken cancellationToken = default);
}
