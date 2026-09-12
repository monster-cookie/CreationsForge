using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Starfield.Native.Edits;

/// <summary>Clears the optional native translated name from a Starfield FormList.</summary>
public sealed class StarfieldClearNameEdit : FormListEdit
{
    /// <summary>Initializes an explicit native name-clear command.</summary>
    public StarfieldClearNameEdit()
        : base("starfield.form-list.clear-name")
    { }
}
