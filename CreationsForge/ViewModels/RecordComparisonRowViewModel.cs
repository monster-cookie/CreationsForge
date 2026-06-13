using System.Collections.ObjectModel;
using CreationsForge.Core.DTOs.Records;

namespace CreationsForge.ViewModels;

public class RecordComparisonRowViewModel
{
    public RecordComparisonRowViewModel(string fieldName, IReadOnlyList<RecordComparisonValueDTO> values, IReadOnlyList<RecordComparisonFieldDTO> children)
        : this(fieldName, values, children, null)
    { }

    private RecordComparisonRowViewModel(string fieldName, IReadOnlyList<RecordComparisonValueDTO> values, IReadOnlyList<RecordComparisonFieldDTO> children, string? parentFieldName)
    {
        FieldName = fieldName;
        ParentFieldName = parentFieldName;
        Values = values;
        foreach (var child in children)
        {
            Children.Add(new RecordComparisonRowViewModel(child.FieldName, child.Values, child.Children, FieldName));
        }
    }

    public ObservableCollection<RecordComparisonRowViewModel> Children { get; } = new();

    public bool HasChildren => Children.Count > 0;

    public bool IsExpanded => true;

    public string FieldName { get; }

    public string? ParentFieldName { get; }

    public IReadOnlyList<RecordComparisonValueDTO> Values { get; }

    public string this[int index] => GetValue(index);

    public string GetValue(int index)
    {
        return index >= 0 && index < Values.Count
            ? Values[index].DisplayValue
            : string.Empty;
    }

    public RecordComparisonValueDTO? GetComparisonValue(int index)
    {
        return index >= 0 && index < Values.Count
            ? Values[index]
            : null;
    }

    public RecordComparisonValueState GetValueState(int index)
    {
        return index >= 0 && index < Values.Count
            ? Values[index].State
            : RecordComparisonValueState.Neutral;
    }
}
