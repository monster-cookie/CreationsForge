using System.Collections.ObjectModel;
using Mutagen.Bethesda.Plugins;

namespace SFRecordCompareEngine.ViewModels;

public class RecordTreeItemViewModel
{
    public RecordTreeItemViewModel(string formIDText, string editorID, FormKey? formKey = null, string? recordType = null)
    {
        FormIDText = formIDText;
        EditorID = editorID;
        FormKey = formKey;
        RecordType = recordType;
    }

    public ObservableCollection<RecordTreeItemViewModel> Children { get; } = new();

    public string FormIDText { get; }
    public string EditorID { get; }
    public FormKey? FormKey { get; }
    public string? RecordType { get; }
}