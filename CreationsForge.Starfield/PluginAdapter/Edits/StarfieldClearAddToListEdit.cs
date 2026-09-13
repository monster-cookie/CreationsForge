using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Starfield.PluginAdapter.Edits;

/// <summary>Clears the optional AddToList link from a Starfield FormList.</summary>
public sealed class StarfieldClearAddToListEdit : FormListEdit
{
    /// <summary>Initializes an explicit Mutagen AddToList clear command.</summary>
    public StarfieldClearAddToListEdit()
        : base("starfield.form-list.clear-add-to-list")
    { }
}
