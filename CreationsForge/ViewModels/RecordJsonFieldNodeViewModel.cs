using System.Text.Json;

namespace CreationsForge.ViewModels;

/// <summary>
/// Presents one ordered node from a detached record JSON record without normalizing its structure or scalar representation.
/// </summary>
public sealed class RecordJsonFieldNodeViewModel : ViewModelBase
{
    /// <summary>Tracks whether the node's children are expanded in a hierarchical presentation.</summary>
    private bool IsExpandedValue;

    /// <summary>The structurally paired comparison node whose expansion state mirrors this node.</summary>
    private RecordJsonFieldNodeViewModel? ExpansionPeer;

    /// <summary>Tracks the field's presentation-only comparison state.</summary>
    private ComparisonFieldState ComparisonStateValue;

    /// <summary>Initializes one immutable JSON field projection.</summary>
    /// <param name="name">The exact property name, array index label, or root label.</param>
    /// <param name="valueKind">The original JSON value kind.</param>
    /// <param name="valueText">The exact scalar JSON spelling or a concise container marker.</param>
    /// <param name="children">The children in their original property or array order.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="valueText"/> is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> or <paramref name="children"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="valueKind"/> is undefined.</exception>
    public RecordJsonFieldNodeViewModel(
        string name,
        JsonValueKind valueKind,
        string valueText,
        IReadOnlyList<RecordJsonFieldNodeViewModel> children)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(valueText);
        ArgumentNullException.ThrowIfNull(children);
        if (!Enum.IsDefined(valueKind))
        {
            throw new ArgumentOutOfRangeException(nameof(valueKind));
        }

        Name = name;
        ValueKind = valueKind;
        ValueText = valueText;
        Children = Array.AsReadOnly(children.ToArray());
    }

    /// <summary>Gets the exact property name, array index label, or root label.</summary>
    public string Name { get; }

    /// <summary>Gets the original JSON value kind.</summary>
    public JsonValueKind ValueKind { get; }

    /// <summary>Gets the original JSON value kind as user-facing text.</summary>
    public string KindText => ValueKind.ToString();

    /// <summary>Gets the exact scalar JSON spelling or a concise marker for an object or array.</summary>
    public string ValueText { get; }

    /// <summary>Gets the child nodes in their original property or array order, including duplicate property names.</summary>
    public IReadOnlyList<RecordJsonFieldNodeViewModel> Children { get; }

    /// <summary>Gets whether this field is identical, conflicting, or part of the explicitly selected winning override.</summary>
    internal ComparisonFieldState ComparisonState => ComparisonStateValue;

    /// <summary>Gets whether the node contains one or more projected children.</summary>
    public bool HasChildren => Children.Count > 0;

    /// <summary>Gets or sets whether the node's children are expanded in a hierarchical presentation.</summary>
    public bool IsExpanded
    {
        get => IsExpandedValue;
        set
        {
            if (SetProperty(ref IsExpandedValue, value))
            {
                ExpansionPeer?.SetExpansionFromPeer(value);
            }
        }
    }

    /// <summary>Pairs this node with the corresponding comparison node so either side controls both expansion states.</summary>
    /// <param name="peer">The node at the same structural comparison position.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="peer"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when a node is paired with itself.</exception>
    /// <exception cref="InvalidOperationException">Thrown when either node is already paired with a different node.</exception>
    internal void SynchronizeExpansionWith(RecordJsonFieldNodeViewModel peer)
    {
        ArgumentNullException.ThrowIfNull(peer);
        if (ReferenceEquals(this, peer))
        {
            throw new ArgumentException("A comparison field cannot synchronize expansion with itself.", nameof(peer));
        }

        if ((ExpansionPeer is not null && !ReferenceEquals(ExpansionPeer, peer)) ||
            (peer.ExpansionPeer is not null && !ReferenceEquals(peer.ExpansionPeer, this)))
        {
            throw new InvalidOperationException("A comparison field cannot synchronize expansion with more than one peer.");
        }

        ExpansionPeer = peer;
        peer.ExpansionPeer = this;
        peer.SetExpansionFromPeer(IsExpandedValue);
    }

    /// <summary>Assigns presentation-only comparison state without changing the detached JSON projection.</summary>
    /// <param name="state">The exact state derived from the paired detached JSON trees.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="state"/> is undefined.</exception>
    internal void SetComparisonState(ComparisonFieldState state)
    {
        if (!Enum.IsDefined(state))
        {
            throw new ArgumentOutOfRangeException(nameof(state));
        }

        SetProperty(ref ComparisonStateValue, state, nameof(ComparisonState));
    }

    /// <summary>Applies a paired expansion change without sending the same change back to its origin.</summary>
    /// <param name="value">The expansion state selected on the paired node.</param>
    private void SetExpansionFromPeer(bool value)
    {
        SetProperty(ref IsExpandedValue, value, nameof(IsExpanded));
    }
}
