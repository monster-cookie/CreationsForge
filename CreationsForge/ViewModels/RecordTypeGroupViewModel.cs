namespace CreationsForge.ViewModels;

/// <summary>Presents one collapsible major-record group above its records in the selected display order.</summary>
public sealed class RecordTypeGroupViewModel : ViewModelBase, IRecordTreeNodeViewModel
{
    /// <summary>Tracks whether the record-type group is expanded.</summary>
    private bool IsExpandedValue;

    /// <summary>Initializes one record-type group.</summary>
    /// <param name="label">The friendly major-record type and plugin signature displayed in the primary column.</param>
    /// <param name="children">The selectable record rows in display order.</param>
    /// <param name="isExpanded">Whether the family is initially expanded in the tree.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="label"/> is empty or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="children"/> is <see langword="null"/>.</exception>
    public RecordTypeGroupViewModel(
        string label,
        IReadOnlyList<IRecordTreeNodeViewModel> children,
        bool isExpanded = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(label);
        ArgumentNullException.ThrowIfNull(children);
        Label = label;
        Children = Array.AsReadOnly(children.ToArray());
        IsExpandedValue = isExpanded;
    }

    /// <summary>Gets the friendly major-record type and plugin signature.</summary>
    public string Label { get; }

    /// <inheritdoc />
    public string PrimaryText => Label;

    /// <inheritdoc />
    public string EditorIdText => string.Empty;

    /// <inheritdoc />
    public string ContextText => string.Empty;

    /// <inheritdoc />
    public string OverrideCountText => string.Empty;

    /// <summary>Gets the selectable record rows in display order.</summary>
    public IReadOnlyList<IRecordTreeNodeViewModel> Children { get; }

    /// <inheritdoc />
    public IReadOnlyList<IRecordTreeNodeViewModel> TreeChildren => Children;

    /// <inheritdoc />
    public bool HasChildren => Children.Count > 0;

    /// <inheritdoc />
    public bool IsExpanded
    {
        get => IsExpandedValue;
        set => SetProperty(ref IsExpandedValue, value);
    }
}
