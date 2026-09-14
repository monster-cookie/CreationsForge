using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordInspection;
using CreationsForge.Fallout4.PluginAdapter.Edits;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Fallout4.PluginAdapter;

/// <summary>Prepares, applies, and previews typed Fallout 4 FormList edits against unpublished complete output state.</summary>
public sealed partial class Fallout4RecordEditService
{
    /// <summary>The stateless complete record field writer and semantic comparer.</summary>
    private readonly RecordInspection.Fallout4FormListInspector _inspector;

    /// <summary>Initializes a Fallout 4 record edit service.</summary>
    /// <param name="inspector">The complete FormList inspector shared with read operations.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="inspector"/> is <see langword="null"/>.</exception>
    public Fallout4RecordEditService(RecordInspection.Fallout4FormListInspector inspector)
    {
        ArgumentNullException.ThrowIfNull(inspector);
        _inspector = inspector;
    }
}
