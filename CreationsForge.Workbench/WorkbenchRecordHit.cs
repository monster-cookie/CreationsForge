namespace CreationsForge.Workbench;

/// <summary>One winning record returned by <c>records_search</c>.</summary>
public sealed class WorkbenchRecordHit
{
    /// <summary>Initializes a search hit.</summary>
    /// <param name="formKey">The origin FormKey text.</param>
    /// <param name="containingModKey">The plugin that contains this exact version.</param>
    /// <param name="winningModKey">The plugin that wins for the origin identity.</param>
    /// <param name="editorId">The EditorID, or an empty string when the record has none.</param>
    public WorkbenchRecordHit(string formKey, string containingModKey, string winningModKey, string editorId)
    {
        FormKey = formKey ?? string.Empty;
        ContainingModKey = containingModKey ?? string.Empty;
        WinningModKey = winningModKey ?? string.Empty;
        EditorId = editorId ?? string.Empty;
    }

    /// <summary>Gets the origin FormKey text.</summary>
    public string FormKey { get; }

    /// <summary>Gets the plugin that contains this exact version.</summary>
    public string ContainingModKey { get; }

    /// <summary>Gets the plugin that wins for the origin identity.</summary>
    public string WinningModKey { get; }

    /// <summary>Gets the EditorID, or an empty string when the record has none.</summary>
    public string EditorId { get; }

    /// <summary>Gets the text shown in the search list.</summary>
    public string Label
    {
        get
        {
            var name = EditorId.Length == 0 ? "(no EditorID)" : EditorId;
            return name + " — " + FormKey + " in " + ContainingModKey;
        }
    }
}
