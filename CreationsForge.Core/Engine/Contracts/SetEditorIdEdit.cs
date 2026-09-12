namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Sets a FormList EditorID to an explicit non-empty value.
/// </summary>
public sealed class SetEditorIdEdit : FormListEdit
{
    /// <summary>Initializes an EditorID replacement command.</summary>
    /// <param name="editorId">The non-empty EditorID to assign.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="editorId"/> is empty or whitespace.</exception>
    public SetEditorIdEdit(string editorId)
        : base("form-list.set-editor-id")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(editorId);
        EditorId = editorId;
    }

    /// <summary>Gets the EditorID to assign.</summary>
    public string EditorId { get; }
}
