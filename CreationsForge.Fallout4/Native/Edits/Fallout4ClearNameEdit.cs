using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Fallout4.Native.Edits;

/// <summary>Clears the optional localized name from a Fallout 4 FormList.</summary>
public sealed class Fallout4ClearNameEdit : FormListEdit
{
    /// <summary>Initializes an explicit native localized-name clear command.</summary>
    public Fallout4ClearNameEdit()
        : base("fallout4.form-list.clear-name")
    { }
}
