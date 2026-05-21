namespace SFRecordCompareEngine.Core.Models.Records;

public class RecordComparisonRecordTypeOptions
{
    private static readonly RecordComparisonRecordTypeOptions DefaultOptions = new();

    private static readonly IDictionary<string, RecordComparisonRecordTypeOptions> OptionsByRecordType =
        new Dictionary<string, RecordComparisonRecordTypeOptions>(StringComparer.OrdinalIgnoreCase)
        {
            ["FormList"] = new()
            {
                HiddenFieldNames =
                {
                    "FormKey",
                    "FormVersion",
                    "StarfieldMajorRecordFlags",
                    "Version2",
                    "VersionControl"
                },
                TreeFieldNames =
                {
                    "Items"
                }
            }
        };

    public ISet<string> HiddenFieldNames { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    public ISet<string> TreeFieldNames { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public static RecordComparisonRecordTypeOptions For(string recordType)
    {
        return OptionsByRecordType.TryGetValue(recordType, out var options)
            ? options
            : DefaultOptions;
    }

    public bool IsHidden(string fieldName)
    {
        return HiddenFieldNames.Any(hiddenFieldName =>
            fieldName.Equals(hiddenFieldName, StringComparison.OrdinalIgnoreCase)
            || fieldName.StartsWith($"{hiddenFieldName}.", StringComparison.OrdinalIgnoreCase));
    }

    public bool IsTree(string fieldName)
    {
        return TreeFieldNames.Any(treeFieldName =>
            fieldName.Equals(treeFieldName, StringComparison.OrdinalIgnoreCase)
            || fieldName.StartsWith($"{treeFieldName}.", StringComparison.OrdinalIgnoreCase));
    }
}