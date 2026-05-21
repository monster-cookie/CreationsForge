namespace SFRecordCompareEngine.Core.Models.Records;

public class RecordComparisonRecordTypeOptions
{
    private static readonly RecordComparisonRecordTypeOptions DefaultOptions = new()
    {
        HiddenFieldNames =
        {
            "FormVersion",
            "FormKey",
            "StarfieldMajorRecordFlags",
            "Version2",
            "VersionControl"
        }
    };

    private static readonly IDictionary<string, RecordComparisonRecordTypeOptions> OptionsByRecordType =
        new Dictionary<string, RecordComparisonRecordTypeOptions>(StringComparer.OrdinalIgnoreCase)
        {
            ["FormList"] = new()
            {
                HiddenFieldNames =
                {
                    "FormKey"
                },
                TreeFieldNames =
                {
                    "Items"
                }
            },
            ["GameSetting"] = new()
            {
                HiddenFieldNames =
                {
                    "XALG"
                }
            }
        };

    public ISet<string> HiddenFieldNames { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    public ISet<string> TreeFieldNames { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public static RecordComparisonRecordTypeOptions For(string recordType)
    {
        if (!OptionsByRecordType.TryGetValue(recordType, out var options))
        {
            return DefaultOptions;
        }

        var mergedOptions = new RecordComparisonRecordTypeOptions();
        foreach (var hiddenFieldName in DefaultOptions.HiddenFieldNames.Concat(options.HiddenFieldNames))
        {
            mergedOptions.HiddenFieldNames.Add(hiddenFieldName);
        }

        foreach (var treeFieldName in options.TreeFieldNames)
        {
            mergedOptions.TreeFieldNames.Add(treeFieldName);
        }

        return mergedOptions;
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
