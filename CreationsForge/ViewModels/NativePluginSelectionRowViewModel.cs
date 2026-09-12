using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.ViewModels;

/// <summary>Projects one installed native plugin as a selectable row without owning its engine state.</summary>
public sealed class NativePluginSelectionRowViewModel
{
    /// <summary>Initializes a row from one detached discovery result.</summary>
    /// <param name="entry">The discovered plugin entry.</param>
    public NativePluginSelectionRowViewModel(NativePluginCatalogEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        Entry = entry;
    }

    /// <summary>Gets the detached native discovery entry.</summary>
    public NativePluginCatalogEntry Entry { get; }

    /// <summary>Gets the plugin filename.</summary>
    public string FileName => Entry.ModKey.FileName.String;

    /// <summary>Gets the one-based load-order position shown to the user.</summary>
    public string LoadOrderText => (Entry.LoadOrderIndex + 1).ToString();

    /// <summary>Gets the native plugin extension type.</summary>
    public string PluginTypeText => Entry.ModKey.Type.ToString();

    /// <summary>Gets whether the installed load order enables the plugin.</summary>
    public string EnabledText => Entry.Enabled ? "Enabled" : "Disabled";

    /// <summary>Gets whether the plugin can be opened for guarded editing.</summary>
    public bool CanEdit => Entry.CanEdit;

    /// <summary>Gets a concise availability description.</summary>
    public string AvailabilityText => Entry.CanEdit ? "Editable" : "Read-only";

    /// <summary>Gets the reason the plugin cannot be edited, or an editable-plugin summary.</summary>
    public string DetailsText => Entry.UnavailableReason
        ?? $"{Entry.DependencyPluginPaths.Count} declared master plugin(s) will be opened read-only.";
}
