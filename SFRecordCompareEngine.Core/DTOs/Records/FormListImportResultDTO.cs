namespace SFRecordCompareEngine.Core.DTOs.Records;

public class FormListImportResultDTO
{
    public required string SelectedModKey { get; set; }
    public IList<string> HierarchyModKeys { get; set; } = new List<string>();
    public IList<string> SkippedModKeys { get; set; } = new List<string>();
    public int PluginsImported { get; set; }
    public int PluginsInvalidated { get; set; }
    public int FormListsImported { get; set; }
    public int FormListItemsImported { get; set; }
    public int FormListsFailed { get; set; }
}
