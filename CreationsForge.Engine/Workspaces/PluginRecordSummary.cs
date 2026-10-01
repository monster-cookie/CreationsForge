using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Engine.Workspaces;

/// <summary>Projects one winning record into the identities needed for bounded search.</summary>
public sealed class PluginRecordSummary
{
    /// <summary>Initializes a winning-record summary.</summary>
    /// <param name="familyId">The declared family identifier.</param>
    /// <param name="originFormKey">The record's origin identity.</param>
    /// <param name="containingModKey">The plugin containing this winning version.</param>
    /// <param name="winningModKey">The plugin that wins for the origin identity.</param>
    /// <param name="editorId">The EditorID, or <see langword="null"/> when the winning record has none.</param>
    public PluginRecordSummary(
        string familyId,
        FormKey originFormKey,
        ModKey containingModKey,
        ModKey winningModKey,
        string? editorId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(familyId);
        FamilyId = familyId;
        OriginFormKey = originFormKey;
        ContainingModKey = containingModKey;
        WinningModKey = winningModKey;
        EditorId = editorId;
    }

    /// <summary>Gets the declared family identifier.</summary>
    public string FamilyId { get; }

    /// <summary>Gets the record's origin identity. Its mod key may differ from the winning plugin.</summary>
    public FormKey OriginFormKey { get; }

    /// <summary>Gets the plugin containing this winning version.</summary>
    public ModKey ContainingModKey { get; }

    /// <summary>Gets the plugin containing the winning version of the origin identity.</summary>
    public ModKey WinningModKey { get; }

    /// <summary>Gets the EditorID, or <see langword="null"/> when the winning record has none.</summary>
    public string? EditorId { get; }
}
