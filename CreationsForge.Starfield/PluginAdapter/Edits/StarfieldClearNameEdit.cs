using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Starfield.PluginAdapter.Edits;

/// <summary>Clears the optional record translated name from a Starfield FormList.</summary>
public sealed class StarfieldClearNameEdit : FormListEdit
{
    /// <summary>Initializes an explicit Mutagen name-clear command.</summary>
    public StarfieldClearNameEdit()
        : base("starfield.form-list.clear-name")
    { }
}
