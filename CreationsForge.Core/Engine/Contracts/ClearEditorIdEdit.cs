namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Clears a FormList EditorID through an explicit named command.
/// </summary>
public sealed class ClearEditorIdEdit : FormListEdit
{
    /// <summary>Initializes an EditorID clear command.</summary>
    public ClearEditorIdEdit()
        : base("form-list.clear-editor-id")
    { }
}
