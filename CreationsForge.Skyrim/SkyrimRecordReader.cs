using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Skyrim.Interfaces;

namespace CreationsForge.Skyrim;

public class SkyrimRecordReader : IGameRecordReader
{
    private readonly ISkyrimRecordReaderService RecordReaderService;

    public SkyrimRecordReader(ISkyrimRecordReaderService recordReaderService)
    {
        RecordReaderService = recordReaderService;
    }

    public SupportedGame Game => SupportedGame.Skyrim;

    public PluginRecordSetDTO ReadPluginRecords(PluginDTO plugin, CancellationToken cancellationToken = default)
    {
        return RecordReaderService.ReadPluginRecords(plugin, cancellationToken);
    }
}
