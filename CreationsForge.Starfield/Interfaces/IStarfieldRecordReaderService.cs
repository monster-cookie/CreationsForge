using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;

namespace CreationsForge.Starfield.Interfaces;

public interface IStarfieldRecordReaderService
{
    PluginRecordSetDTO ReadPluginRecords(PluginDTO plugin, CancellationToken cancellationToken = default);
}
