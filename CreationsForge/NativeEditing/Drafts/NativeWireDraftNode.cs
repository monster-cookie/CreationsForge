using System.ComponentModel;
using System.Runtime.CompilerServices;
using CreationsForge.NativeEditing.Schema;

namespace CreationsForge.NativeEditing.Drafts;

/// <summary>Identifies the closed typed draft-node set consumed by the native editor controls.</summary>
public enum NativeWireDraftNodeKind
{
    /// <summary>A closed JSON object.</summary>
    Object,
    /// <summary>An ordered mutable JSON array.</summary>
    Array,
    /// <summary>A lazily selected schema union.</summary>
    Union,
    /// <summary>A whole-value nullable wrapper.</summary>
    Nullable,
    /// <summary>A Boolean value.</summary>
    Boolean,
    /// <summary>A canonical signed or unsigned integer.</summary>
    Integer,
    /// <summary>A bounded string.</summary>
    String,
    /// <summary>A known or unknown native enum integer.</summary>
    Enum,
    /// <summary>A native FormLink.</summary>
    FormLink,
    /// <summary>An owner-mode-sensitive native FormLink-or-index.</summary>
    FormLinkOrIndex,
    /// <summary>An exact floating-point bit representation.</summary>
    FloatBits,
    /// <summary>A Base64-encoded byte sequence.</summary>
    ByteArray,
    /// <summary>A translated string graph.</summary>
    TranslatedString,
    /// <summary>A native asset graph.</summary>
    Asset,
    /// <summary>A native color graph.</summary>
    Color,
    /// <summary>A shape-preserving two-dimensional array graph.</summary>
    Array2D,
}

/// <summary>Identifies how a draft node obtained its current initial value.</summary>
public enum NativeWireDraftValueState
{
    /// <summary>The value comes from an explicit schema <c>const</c>.</summary>
    SchemaConstant,
    /// <summary>The value was hydrated from a revision-bound record seed.</summary>
    Seeded,
    /// <summary>The value comes from an explicit schema default or complete validated type template.</summary>
    Defaulted,
    /// <summary>A required value has no safe explicit default and must be supplied by the user.</summary>
    RequiredUnset,
}

/// <summary>Provides the common observable state for one request-local typed native wire draft value.</summary>
public abstract class NativeWireDraftNode : INotifyPropertyChanged
{
    /// <summary>The immutable issues currently assigned to this node.</summary>
    private IReadOnlyList<NativeWireDraftIssue> CurrentIssues = Array.Empty<NativeWireDraftIssue>();

    /// <summary>Whether this node or a descendant changed after initialization.</summary>
    private bool IsLocallyChanged;

    /// <summary>Initializes shared typed draft state.</summary>
    /// <param name="kind">The closed typed node kind.</param>
    /// <param name="path">The exact root-based JSON path.</param>
    /// <param name="displayName">The user-facing field name.</param>
    /// <param name="descriptor">The resolved schema descriptor.</param>
    /// <param name="isRequired">Whether the containing object requires this value.</param>
    /// <param name="isReadOnly">Whether the value is derived and cannot be directly changed.</param>
    /// <param name="valueState">How the initial value was obtained.</param>
    protected NativeWireDraftNode(
        NativeWireDraftNodeKind kind,
        string path,
        string displayName,
        NativeWireSchemaDescriptor descriptor,
        bool isRequired,
        bool isReadOnly,
        NativeWireDraftValueState valueState)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        ArgumentNullException.ThrowIfNull(descriptor);
        Kind = kind;
        Path = path;
        DisplayName = displayName;
        Descriptor = descriptor;
        IsRequired = isRequired;
        IsReadOnly = isReadOnly;
        ValueState = valueState;
    }

    /// <summary>Raised when observable node state changes.</summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Raised after this node or one of its descendants is mutated.</summary>
    public event EventHandler? Changed;

    /// <summary>Gets the closed node kind used by the control factory.</summary>
    public NativeWireDraftNodeKind Kind { get; }

    /// <summary>Gets the exact root-based JSON path used by validation and codec errors.</summary>
    public string Path { get; private set; }

    /// <summary>Gets the user-facing field name.</summary>
    public string DisplayName { get; }

    /// <summary>Gets the resolved immutable schema descriptor.</summary>
    public NativeWireSchemaDescriptor Descriptor { get; }

    /// <summary>Gets whether the containing object requires this value.</summary>
    public bool IsRequired { get; }

    /// <summary>Gets whether the value is derived and cannot be directly changed.</summary>
    public bool IsReadOnly { get; }

    /// <summary>Gets how the node obtained its initial value.</summary>
    public NativeWireDraftValueState ValueState { get; internal set; }

    /// <summary>Gets whether this node or one of its descendants changed after initialization.</summary>
    public bool HasChanges => IsLocallyChanged;

    /// <summary>Gets this node's immutable local and mapped validation issues.</summary>
    public IReadOnlyList<NativeWireDraftIssue> Issues => CurrentIssues;

    /// <summary>Gets the node's direct typed children without exposing mutable JSON.</summary>
    public abstract IReadOnlyList<NativeWireDraftNode> Children { get; }

    /// <summary>Assigns immutable issues produced by one validation pass.</summary>
    /// <param name="issues">Issues whose exact path resolves to this node.</param>
    internal void SetIssues(IEnumerable<NativeWireDraftIssue> issues)
    {
        ArgumentNullException.ThrowIfNull(issues);
        CurrentIssues = Array.AsReadOnly(issues.ToArray());
        OnPropertyChanged(nameof(Issues));
    }

    /// <summary>Omits one optional read-only projection after its authoritative source changes.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the node is required or editable.</exception>
    internal void InvalidateOptionalReadOnlyProjection()
    {
        if (IsRequired || !IsReadOnly)
        {
            throw new InvalidOperationException($"{Path}: only optional read-only projections can be invalidated.");
        }

        if (ValueState == NativeWireDraftValueState.RequiredUnset)
        {
            return;
        }

        ValueState = NativeWireDraftValueState.RequiredUnset;
        OnPropertyChanged(nameof(ValueState));
    }

    /// <summary>Observes a child so changes propagate to the draft root.</summary>
    /// <param name="child">The owned child node.</param>
    protected void ObserveChild(NativeWireDraftNode child)
    {
        ArgumentNullException.ThrowIfNull(child);
        child.Changed += ChildChanged;
    }

    /// <summary>Stops observing a removed or replaced child.</summary>
    /// <param name="child">The formerly owned child node.</param>
    protected void StopObservingChild(NativeWireDraftNode child)
    {
        ArgumentNullException.ThrowIfNull(child);
        child.Changed -= ChildChanged;
    }

    /// <summary>Marks this node changed and publishes the affected property.</summary>
    /// <param name="propertyName">The changed observable property.</param>
    protected void MarkChanged([CallerMemberName] string? propertyName = null)
    {
        if (ValueState == NativeWireDraftValueState.RequiredUnset)
        {
            ValueState = NativeWireDraftValueState.Defaulted;
            OnPropertyChanged(nameof(ValueState));
        }

        if (!IsLocallyChanged)
        {
            IsLocallyChanged = true;
            OnPropertyChanged(nameof(HasChanges));
        }

        if (propertyName is not null)
        {
            OnPropertyChanged(propertyName);
        }

        Changed?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Raises one observable property notification.</summary>
    /// <param name="propertyName">The changed public property name.</param>
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>Rebases this node and its materialized descendants after an ordered structural edit.</summary>
    /// <param name="path">The new exact structural JSON path.</param>
    internal void RebasePath(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        if (!string.Equals(Path, path, StringComparison.Ordinal))
        {
            Path = path;
            OnPropertyChanged(nameof(Path));
        }

        RebaseChildren();
    }

    /// <summary>Rebases materialized child paths after this node receives a new structural path.</summary>
    protected virtual void RebaseChildren()
    {
    }

    /// <summary>Propagates one descendant mutation through this node.</summary>
    /// <param name="sender">The changed child.</param>
    /// <param name="eventArgs">The empty change event data.</param>
    private void ChildChanged(object? sender, EventArgs eventArgs)
    {
        MarkChanged(nameof(Children));
    }
}
