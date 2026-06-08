using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Fallout4.Interfaces;

namespace CreationsForge.Fallout4;

public class Fallout4RecordReader : IGameRecordReader
{
    private readonly IFallout4RecordReaderService RecordReaderService;

    public Fallout4RecordReader(IFallout4RecordReaderService recordReaderService)
    {
        RecordReaderService = recordReaderService;
    }

    public SupportedGame Game => SupportedGame.Fallout4;

    public PluginRecordSetDTO ReadPluginRecords(PluginDTO plugin, CancellationToken cancellationToken = default)
    {
        return RecordReaderService.ReadPluginRecords(plugin, cancellationToken);
    }
}
