using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Starfield.Native.Edits;

/// <summary>Sets the optional AddToList link to a non-null native FormList identity.</summary>
public sealed class StarfieldSetAddToListEdit : FormListEdit
{
    /// <summary>Initializes a typed AddToList link replacement.</summary>
    /// <param name="formList">The non-null FormKey that must resolve to a live FormList when applied.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="formList"/> is <see cref="FormKey.Null"/>.</exception>
    public StarfieldSetAddToListEdit(FormKey formList)
        : base("starfield.form-list.set-add-to-list")
    {
        if (formList.IsNull)
        {
            throw new ArgumentException("AddToList requires a non-null FormList identity; use the clear command to remove the link.", nameof(formList));
        }

        FormList = formList;
    }

    /// <summary>Gets the FormKey that must resolve to a live Starfield FormList.</summary>
    public FormKey FormList { get; }
}
