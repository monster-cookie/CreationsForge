using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Core.Engine.RecordReading;

/// <summary>Identifies one direct record link and its optional typed field path for diagnostics.</summary>
public sealed class FormLinkReference
{
    /// <summary>Initializes direct record link diagnostic metadata.</summary>
    /// <param name="formKey">The non-null record target identity.</param>
    /// <param name="fieldPath">The stable typed field path, or <see langword="null"/> when the record visitor cannot provide one.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="formKey"/> is null or <paramref name="fieldPath"/> is empty or whitespace.</exception>
    public FormLinkReference(FormKey formKey, string? fieldPath)
    {
        if (formKey.IsNull)
        {
            throw new ArgumentException("A direct record link diagnostic requires a non-null FormKey.", nameof(formKey));
        }

        if (fieldPath is not null && string.IsNullOrWhiteSpace(fieldPath))
        {
            throw new ArgumentException("A direct record link field path cannot be empty or whitespace.", nameof(fieldPath));
        }

        FormKey = formKey;
        FieldPath = fieldPath;
    }

    /// <summary>Gets the linked record target identity.</summary>
    public FormKey FormKey { get; }

    /// <summary>Gets the stable typed field path, or <see langword="null"/> when unavailable.</summary>
    public string? FieldPath { get; }
}
