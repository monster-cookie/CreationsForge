using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.ViewModels;

/// <summary>Presents one source-plugin major-record context as a selectable browser row.</summary>
public sealed class MajorRecordViewModel : ViewModelBase, IRecordTreeNodeViewModel
{
    /// <summary>Tracks the interface expansion value; records expose no navigation children.</summary>
    private bool IsExpandedValue;

    /// <summary>Initializes one source-plugin major-record row.</summary>
    /// <param name="match">The lightweight source context supplied by the engine.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="match"/> is <see langword="null"/>.</exception>
    public MajorRecordViewModel(ReferenceSearchMatch match)
    {
        ArgumentNullException.ThrowIfNull(match);
        FormKey = match.FormKey;
        RecordType = match.RecordType;
        EditorId = match.EditorId;
        ContainingModKey = match.ContainingModKey;
        SourcePath = match.SourcePath;
        LoadOrderIndex = match.LoadOrderIndex;
        Role = match.Role;
        IsDeleted = match.IsDeleted;
    }

    /// <summary>Gets the canonical record identity.</summary>
    public FormKey FormKey { get; }

    /// <summary>Gets the stable registered record family.</summary>
    public string RecordType { get; }

    /// <summary>Gets the EditorID observed in the listed plugin context, or <see langword="null"/>.</summary>
    public string? EditorId { get; }

    /// <summary>Gets the plugin containing the listed context, or <see langword="null"/> when unavailable.</summary>
    public ModKey? ContainingModKey { get; }

    /// <summary>Gets the canonical plugin path containing the listed context, or <see langword="null"/>.</summary>
    public string? SourcePath { get; }

    /// <summary>Gets the listed context's explicit load-order index, or <see langword="null"/>.</summary>
    public int? LoadOrderIndex { get; }

    /// <summary>Gets the listed context's workspace role, or <see langword="null"/>.</summary>
    public PluginRole? Role { get; }

    /// <summary>Gets whether the listed context carries the deletion flag.</summary>
    public bool IsDeleted { get; }

    /// <inheritdoc />
    public string PrimaryText => FormKey.ID.ToString("X8");

    /// <inheritdoc />
    public string EditorIdText => EditorId ?? "(no EditorID)";

    /// <inheritdoc />
    public string ContextText => ContainingModKey.HasValue
        ? $"{ContainingModKey.Value.FileName}{(IsDeleted ? " (deleted)" : string.Empty)}"
        : IsDeleted ? "Deleted" : "Plugin context";

    /// <inheritdoc />
    public string OverrideCountText => string.Empty;

    /// <inheritdoc />
    public IReadOnlyList<IRecordTreeNodeViewModel> TreeChildren => Array.Empty<IRecordTreeNodeViewModel>();

    /// <inheritdoc />
    public bool HasChildren => false;

    /// <inheritdoc />
    public bool IsExpanded
    {
        get => IsExpandedValue;
        set => SetProperty(ref IsExpandedValue, value);
    }
}
