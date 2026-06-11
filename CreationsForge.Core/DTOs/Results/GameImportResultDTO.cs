using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Results;

public class GameImportResultDTO
{
    public required SupportedGame Game { get; set; }

    public int PluginsDiscovered { get; set; }

    public int PluginsUnchanged { get; set; }

    public int PluginsChanged { get; set; }

    public int PluginsImported { get; set; }

    public int PluginsMissing { get; set; }

    public int PluginsFailed { get; set; }

    public int PluginsUnsupported { get; set; }

    public int PluginsInvalidated { get; set; }

    public int MasterReferencesImported { get; set; }

    public AssetArchiveIndexResultDTO AssetArchiveIndex { get; set; } = new();

    public RecordImportResultDTO Records { get; set; } = new();
}
