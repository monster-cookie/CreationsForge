using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Starfield.PluginAdapter.Edits;

/// <summary>Sets a FormList name to one complete record translated-string payload.</summary>
public sealed class StarfieldSetNameEdit : FormListEdit
{
    /// <summary>Initializes a record translated-name replacement.</summary>
    /// <param name="name">The translated string copied during edit preparation, including empty translations when present.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <see langword="null"/>.</exception>
    public StarfieldSetNameEdit(ITranslatedStringGetter name)
        : base("starfield.form-list.set-name")
    {
        ArgumentNullException.ThrowIfNull(name);
        Name = name;
    }

    /// <summary>Gets the caller-owned translated string that preparation must copy before publication.</summary>
    public ITranslatedStringGetter Name { get; }
}
