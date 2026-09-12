using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Describes one native FormList for lightweight enumeration without becoming a record authority.
/// </summary>
public sealed class FormListSummary
{
    /// <summary>Initializes an ephemeral FormList summary.</summary>
    /// <param name="formKey">The native FormList identity.</param>
    /// <param name="editorId">The native EditorID, or <see langword="null"/> when absent.</param>
    /// <param name="overrideCount">The number of matching contexts in the complete participating load order whose containing plugin differs from the FormKey's origin plugin, including deleted and staged-output contexts.</param>
    /// <param name="scope">The native record view represented by the summary.</param>
    /// <param name="containingModKey">The plugin containing the represented context, when singular.</param>
    /// <param name="sourcePath">The canonical path of the containing plugin, when singular.</param>
    /// <param name="loadOrderIndex">The explicit load-order position of the containing plugin, when singular.</param>
    /// <param name="role">The workspace role of the containing plugin, when singular.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="overrideCount"/> is negative, <paramref name="scope"/> is undefined, <paramref name="loadOrderIndex"/> is negative, or <paramref name="role"/> is undefined.</exception>
    public FormListSummary(
        FormKey formKey,
        string? editorId,
        int overrideCount,
        RecordScope scope,
        ModKey? containingModKey = null,
        string? sourcePath = null,
        int? loadOrderIndex = null,
        PluginRole? role = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(overrideCount);
        if (!Enum.IsDefined(scope))
        {
            throw new ArgumentOutOfRangeException(nameof(scope));
        }
        if (loadOrderIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(loadOrderIndex));
        }
        if (role.HasValue && !Enum.IsDefined(role.Value))
        {
            throw new ArgumentOutOfRangeException(nameof(role));
        }

        FormKey = formKey;
        EditorId = editorId;
        OverrideCount = overrideCount;
        Scope = scope;
        ContainingModKey = containingModKey;
        SourcePath = sourcePath;
        LoadOrderIndex = loadOrderIndex;
        Role = role;
    }

    /// <summary>Gets the native FormList identity.</summary>
    public FormKey FormKey { get; }

    /// <summary>Gets the native EditorID, or <see langword="null"/> when absent.</summary>
    public string? EditorId { get; }

    /// <summary>Gets the complete participating-load-order count of matching contexts whose containing plugin differs from the FormKey's origin plugin.</summary>
    public int OverrideCount { get; }

    /// <summary>Gets the native record scope represented by the summary.</summary>
    public RecordScope Scope { get; }

    /// <summary>Gets the containing plugin when the summary represents one concrete native context.</summary>
    public ModKey? ContainingModKey { get; }

    /// <summary>Gets the canonical path of the containing plugin, or <see langword="null"/>.</summary>
    public string? SourcePath { get; }

    /// <summary>Gets the containing plugin's explicit load-order position, or <see langword="null"/>.</summary>
    public int? LoadOrderIndex { get; }

    /// <summary>Gets the containing plugin's workspace role, or <see langword="null"/>.</summary>
    public PluginRole? Role { get; }
}
