namespace CreationsForge.ViewModels;

/// <summary>Defines the presentation fields and hierarchy shared by record-type groups and selectable record rows.</summary>
public interface IRecordTreeNodeViewModel
{
    /// <summary>Gets the primary tree-column text, such as a record-type label or FormKey.</summary>
    string PrimaryText { get; }

    /// <summary>Gets the EditorID column text, or an empty value for a record-type group.</summary>
    string EditorIdText { get; }

    /// <summary>Gets the record context column text, or an empty value for a record-type group.</summary>
    string ContextText { get; }

    /// <summary>Gets the override-count column text, or an empty value for a record-type group.</summary>
    string OverrideCountText { get; }

    /// <summary>Gets the ordered child nodes displayed beneath this node.</summary>
    IReadOnlyList<IRecordTreeNodeViewModel> TreeChildren { get; }

    /// <summary>Gets whether this node has children that can be expanded.</summary>
    bool HasChildren { get; }

    /// <summary>Gets or sets whether the node's children are expanded.</summary>
    bool IsExpanded { get; set; }
}
