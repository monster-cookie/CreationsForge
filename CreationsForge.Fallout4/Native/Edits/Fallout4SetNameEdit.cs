using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Fallout4.Native.Edits;

/// <summary>Sets the localized Fallout 4 FormList name from a typed native translated-string value.</summary>
public sealed class Fallout4SetNameEdit : FormListEdit
{
    /// <summary>Initializes a native localized-name replacement command.</summary>
    /// <param name="name">The caller-owned translated string that the Fallout 4 adapter must snapshot before asynchronous work.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <see langword="null"/>.</exception>
    public Fallout4SetNameEdit(ITranslatedStringGetter name)
        : base("fallout4.form-list.set-name")
    {
        ArgumentNullException.ThrowIfNull(name);
        Name = name;
    }

    /// <summary>Gets the caller-owned translated string to copy during native command preparation.</summary>
    public ITranslatedStringGetter Name { get; }
}
