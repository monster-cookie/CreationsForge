namespace SFRecordCompareEngine.Core.DTOs.Plugins;

public class PluginImportResultDTO
{
    public int SchemaVersion { get; set; }
    public int PluginsDiscovered { get; set; }
    public int PluginsUnchanged { get; set; }
    public int PluginsChanged { get; set; }
    public int PluginsImported { get; set; }
    public int PluginsMissing { get; set; }
    public int PluginsFailed { get; set; }
    public int PluginsInvalidated { get; set; }
    public int MasterReferencesImported { get; set; }
}
