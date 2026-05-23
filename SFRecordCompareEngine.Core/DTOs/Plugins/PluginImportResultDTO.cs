namespace SFRecordCompareEngine.Core.DTOs.Plugins;

public class PluginImportResultDTO
{
    public int PluginsDiscovered { get; set; }
    public int PluginsUnchanged { get; set; }
    public int PluginsChanged { get; set; }
    public int PluginsImported { get; set; }
    public int PluginsMissing { get; set; }
    public int PluginsFailed { get; set; }
    public int PluginsInvalidated { get; set; }
    public int MasterReferencesImported { get; set; }
    public int RecordHeadersImported { get; set; }
    public int TypedRecordDetailRowsImported { get; set; }
    public int FormListItemsImported { get; set; }
    public int RecordImportFailures { get; set; }
    public int UnsupportedRecordTypes { get; set; }
}
