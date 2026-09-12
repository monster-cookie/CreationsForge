using System.Text.Json;

namespace CreationsForge.ViewModels;

/// <summary>
/// Presents one ordered node from a detached native JSON record without normalizing its structure or scalar representation.
/// </summary>
public sealed class NativeJsonFieldNodeViewModel : ViewModelBase
{
    /// <summary>Tracks whether the node's children are expanded in a hierarchical presentation.</summary>
    private bool IsExpandedValue;

    /// <summary>Tracks the field's presentation-only comparison state.</summary>
    private NativeComparisonFieldState ComparisonStateValue;

    /// <summary>Initializes one immutable JSON field projection.</summary>
    /// <param name="name">The exact property name, array index label, or root label.</param>
    /// <param name="valueKind">The original JSON value kind.</param>
    /// <param name="valueText">The exact scalar JSON spelling or a concise container marker.</param>
    /// <param name="children">The children in their original property or array order.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="valueText"/> is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> or <paramref name="children"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="valueKind"/> is undefined.</exception>
    public NativeJsonFieldNodeViewModel(
        string name,
        JsonValueKind valueKind,
        string valueText,
        IReadOnlyList<NativeJsonFieldNodeViewModel> children)
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
    public IReadOnlyList<NativeJsonFieldNodeViewModel> Children { get; }

    /// <summary>Gets whether this field is identical, conflicting, or part of the explicitly selected winning override.</summary>
    internal NativeComparisonFieldState ComparisonState => ComparisonStateValue;

    /// <summary>Gets whether the node contains one or more projected children.</summary>
    public bool HasChildren => Children.Count > 0;

    /// <summary>Gets or sets whether the node's children are expanded in a hierarchical presentation.</summary>
    public bool IsExpanded
    {
        get => IsExpandedValue;
        set => SetProperty(ref IsExpandedValue, value);
    }

    /// <summary>Assigns presentation-only comparison state without changing the detached JSON projection.</summary>
    /// <param name="state">The exact state derived from the paired detached JSON trees.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="state"/> is undefined.</exception>
    internal void SetComparisonState(NativeComparisonFieldState state)
    {
        if (!Enum.IsDefined(state))
        {
            throw new ArgumentOutOfRangeException(nameof(state));
        }

        SetProperty(ref ComparisonStateValue, state, nameof(ComparisonState));
    }
}
