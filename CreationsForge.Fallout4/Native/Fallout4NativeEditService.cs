using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInspection;
using CreationsForge.Fallout4.Native.Edits;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Fallout4.Native;

/// <summary>Prepares, applies, and previews typed Fallout 4 FormList edits against unpublished complete output state.</summary>
public sealed partial class Fallout4NativeEditService
{
    /// <summary>The stateless complete native field writer and semantic comparer.</summary>
    private readonly NativeInspection.Fallout4FormListNativeInspector _inspector;

    /// <summary>Initializes a Fallout 4 native edit service.</summary>
    /// <param name="inspector">The complete native FormList inspector shared with read operations.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="inspector"/> is <see langword="null"/>.</exception>
    public Fallout4NativeEditService(NativeInspection.Fallout4FormListNativeInspector inspector)
    {
        ArgumentNullException.ThrowIfNull(inspector);
        _inspector = inspector;
    }
}
