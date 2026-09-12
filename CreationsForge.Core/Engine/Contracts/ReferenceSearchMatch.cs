using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Identifies one bounded native reference-search match.
/// </summary>
public sealed class ReferenceSearchMatch
{
    /// <summary>Initializes a native reference-search match.</summary>
    /// <param name="formKey">The native record identity.</param>
    /// <param name="recordType">The stable native record-type identifier.</param>
    /// <param name="editorId">The native EditorID, or <see langword="null"/> when absent.</param>
    /// <param name="containingModKey">The plugin that contains this native record context.</param>
    /// <param name="sourcePath">The canonical path from which the containing plugin was opened.</param>
    /// <param name="loadOrderIndex">The zero-based explicit load-order position of the containing plugin.</param>
    /// <param name="role">The containing plugin's workspace role.</param>
    /// <param name="isDeleted">Whether this exact native context carries the deletion flag.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="recordType"/> is empty or whitespace.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="loadOrderIndex"/> is negative or <paramref name="role"/> is undefined.</exception>
    public ReferenceSearchMatch(
        FormKey formKey,
        string recordType,
        string? editorId,
        ModKey? containingModKey = null,
        string? sourcePath = null,
        int? loadOrderIndex = null,
        PluginRole? role = null,
        bool isDeleted = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(recordType);
        if (loadOrderIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(loadOrderIndex));
        }
        if (role.HasValue && !Enum.IsDefined(role.Value))
        {
            throw new ArgumentOutOfRangeException(nameof(role));
        }

        FormKey = formKey;
        RecordType = recordType;
        EditorId = editorId;
        ContainingModKey = containingModKey;
        SourcePath = sourcePath;
        LoadOrderIndex = loadOrderIndex;
        Role = role;
        IsDeleted = isDeleted;
    }

    /// <summary>Gets the native record identity.</summary>
    public FormKey FormKey { get; }

    /// <summary>Gets the stable native record-type identifier.</summary>
    public string RecordType { get; }

    /// <summary>Gets the native EditorID, or <see langword="null"/> when absent.</summary>
    public string? EditorId { get; }

    /// <summary>Gets the plugin containing this context, distinct from the record's origin <see cref="FormKey"/>.</summary>
    public ModKey? ContainingModKey { get; }

    /// <summary>Gets the canonical path of the containing plugin, or <see langword="null"/> for legacy callers.</summary>
    public string? SourcePath { get; }

    /// <summary>Gets the containing plugin's explicit load-order position, or <see langword="null"/> for legacy callers.</summary>
    public int? LoadOrderIndex { get; }

    /// <summary>Gets the containing plugin's workspace role, or <see langword="null"/> for legacy callers.</summary>
    public PluginRole? Role { get; }

    /// <summary>Gets a value indicating whether this native record context is deleted.</summary>
    public bool IsDeleted { get; }
}
