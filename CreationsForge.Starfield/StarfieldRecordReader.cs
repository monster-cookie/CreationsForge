using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Starfield.Interfaces;

namespace CreationsForge.Starfield;

public class StarfieldRecordReader : IGameRecordReader
{
    private readonly IStarfieldRecordReaderService RecordReaderService;

    public StarfieldRecordReader(IStarfieldRecordReaderService recordReaderService)
    {
        RecordReaderService = recordReaderService;
    }

    public SupportedGame Game => SupportedGame.Starfield;

    public PluginRecordSetDTO ReadPluginRecords(PluginDTO plugin, CancellationToken cancellationToken = default)
    {
        return RecordReaderService.ReadPluginRecords(plugin, cancellationToken);
    }
}
