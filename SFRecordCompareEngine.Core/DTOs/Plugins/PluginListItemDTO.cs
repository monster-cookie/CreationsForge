namespace SFRecordCompareEngine.Core.DTOs.Plugins;

public class PluginListItemDTO
{
    public required string PluginFileName { get; set; }

    public string ImportState { get; set; } = PluginImportState.Current.ToString();

    public bool IsFailed => string.Equals(ImportState, PluginImportState.Failed.ToString(), StringComparison.OrdinalIgnoreCase);
}
