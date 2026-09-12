using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Core.Engine.NativeReading;

/// <summary>Identifies one direct native record link and its optional typed field path for diagnostics.</summary>
public sealed class NativeFormLinkReference
{
    /// <summary>Initializes direct native link diagnostic metadata.</summary>
    /// <param name="formKey">The non-null native target identity.</param>
    /// <param name="fieldPath">The stable typed field path, or <see langword="null"/> when the native visitor cannot provide one.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="formKey"/> is null or <paramref name="fieldPath"/> is empty or whitespace.</exception>
    public NativeFormLinkReference(FormKey formKey, string? fieldPath)
    {
        if (formKey.IsNull)
        {
            throw new ArgumentException("A direct native link diagnostic requires a non-null FormKey.", nameof(formKey));
        }

        if (fieldPath is not null && string.IsNullOrWhiteSpace(fieldPath))
        {
            throw new ArgumentException("A direct native link field path cannot be empty or whitespace.", nameof(fieldPath));
        }

        FormKey = formKey;
        FieldPath = fieldPath;
    }

    /// <summary>Gets the linked native target identity.</summary>
    public FormKey FormKey { get; }

    /// <summary>Gets the stable typed field path, or <see langword="null"/> when unavailable.</summary>
    public string? FieldPath { get; }
}
