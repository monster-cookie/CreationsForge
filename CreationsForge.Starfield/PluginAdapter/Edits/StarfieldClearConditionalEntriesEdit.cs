using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Starfield.PluginAdapter.Edits;

/// <summary>Clears every conditional entry while retaining the Starfield FormList record.</summary>
public sealed class StarfieldClearConditionalEntriesEdit : FormListEdit
{
    /// <summary>Initializes an explicit conditional-entry clear command.</summary>
    public StarfieldClearConditionalEntriesEdit()
        : base("starfield.form-list.clear-conditional-entries")
    { }
}
