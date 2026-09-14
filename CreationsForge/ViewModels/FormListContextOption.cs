using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.ViewModels;

/// <summary>
/// Presents one exact FormList context selection and its enumeration provenance.
/// </summary>
public sealed class FormListContextOption
{
    /// <summary>Initializes one immutable context selector option.</summary>
    /// <param name="selection">The exact engine request represented by this option.</param>
    /// <param name="label">The concise selector label.</param>
    /// <param name="editorId">The EditorID observed in this enumerated context, or <see langword="null"/>.</param>
    /// <param name="sourcePath">The canonical containing-plugin path, or <see langword="null"/> for the winning selector.</param>
    /// <param name="loadOrderIndex">The containing plugin's load-order position, or <see langword="null"/> for the winning selector.</param>
    /// <param name="role">The containing plugin's workspace role, or <see langword="null"/> for the winning selector.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="selection"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="label"/> is empty.</exception>
    public FormListContextOption(
        ReferenceRequest selection,
        string label,
        string? editorId,
        string? sourcePath,
        int? loadOrderIndex,
        PluginRole? role)
    {
        ArgumentNullException.ThrowIfNull(selection);
        ArgumentException.ThrowIfNullOrWhiteSpace(label);
        Selection = selection;
        Label = label;
        EditorId = editorId;
        SourcePath = sourcePath;
        LoadOrderIndex = loadOrderIndex;
        Role = role;
    }

    /// <summary>Gets the exact engine request represented by this option.</summary>
    public ReferenceRequest Selection { get; }

    /// <summary>Gets the concise selector label.</summary>
    public string Label { get; }

    /// <summary>Gets the EditorID observed in this enumerated context, or <see langword="null"/>.</summary>
    public string? EditorId { get; }

    /// <summary>Gets the containing plugin identity, or <see langword="null"/> for the winning selector.</summary>
    public ModKey? ContainingModKey => Selection.ContainingModKey;

    /// <summary>Gets the canonical containing-plugin path, or <see langword="null"/> for the winning selector.</summary>
    public string? SourcePath { get; }

    /// <summary>Gets the containing plugin's load-order position, or <see langword="null"/> for the winning selector.</summary>
    public int? LoadOrderIndex { get; }

    /// <summary>Gets the containing plugin's workspace role, or <see langword="null"/> for the winning selector.</summary>
    public PluginRole? Role { get; }

    /// <summary>Gets whether this option delegates context selection to the engine's winning-override view.</summary>
    public bool IsWinningOverride => Selection.Scope == RecordScope.WinningOverrides;

    /// <summary>Gets complete enumeration provenance suitable for a context selector.</summary>
    public string ProvenanceText => IsWinningOverride
        ? "Winning override resolved by the plugin load order."
        : $"{ContainingModKey?.FileName} | index {LoadOrderIndex} | {Role} | {SourcePath}";

    /// <inheritdoc />
    public override string ToString()
    {
        return Label;
    }
}
