using System.Collections.ObjectModel;
using CreationsForge.Core.DTOs.Plugins;

namespace CreationsForge.ViewModels;

public class RecordTreeItemViewModel
{
    public RecordTreeItemViewModel(
        string formIDText,
        string editorID,
        FormKeyDTO? formKey = null,
        string? recordType = null,
        int? pluginCount = null)
    {
        FormIDText = formIDText;
        EditorID = editorID;
        FormKey = formKey;
        RecordType = recordType;
        PluginCount = pluginCount;
    }

    public ObservableCollection<RecordTreeItemViewModel> Children { get; } = new();

    public string FormIDText { get; }

    public string DisplayFormIDText => FormKey is null
        ? $"{FormIDText} ({Children.Count:N0})"
        : FormIDText;

    public string EditorID { get; }

    public FormKeyDTO? FormKey { get; }

    public string? RecordType { get; }

    public int? PluginCount { get; }

    public string PluginCountText => PluginCount.HasValue
        ? PluginCount.Value.ToString("N0")
        : string.Empty;
}
