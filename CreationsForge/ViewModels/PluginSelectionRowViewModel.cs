using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.ViewModels;

/// <summary>Projects one installed plugin as a selectable row without owning its engine state.</summary>
public sealed class PluginSelectionRowViewModel
{
    /// <summary>Initializes a row from one detached discovery result.</summary>
    /// <param name="entry">The discovered plugin entry.</param>
    public PluginSelectionRowViewModel(PluginCatalogEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        Entry = entry;
    }

    /// <summary>Gets the detached plugin discovery entry.</summary>
    public PluginCatalogEntry Entry { get; }

    /// <summary>Gets the plugin filename.</summary>
    public string FileName => Entry.ModKey.FileName.String;

    /// <summary>Gets the one-based load-order position shown to the user.</summary>
    public string LoadOrderText => (Entry.LoadOrderIndex + 1).ToString();

    /// <summary>Gets the plugin type, including the master style for ESM files.</summary>
    public string PluginTypeText => Entry.ModKey.Type == ModType.Master
        ? $"Master ({MasterStyleText})"
        : FileTypeText;

    /// <summary>Gets the plugin filename extension type.</summary>
    public string FileTypeText => Entry.ModKey.Type switch
    {
        ModType.Master => "ESM",
        ModType.Light => "ESL",
        ModType.Plugin => "ESP",
        _ => Entry.ModKey.Type.ToString()
    };

    /// <summary>Gets the plugin master style.</summary>
    public string MasterStyleText => Entry.MasterStyle.ToString();

    /// <summary>Gets whether the installed load order enables the plugin.</summary>
    public string EnabledText => Entry.Enabled ? "Enabled" : "Disabled";

    /// <summary>Gets whether the plugin can be opened for guarded editing.</summary>
    public bool CanEdit => Entry.CanEdit;

    /// <summary>Gets a concise availability description.</summary>
    public string AvailabilityText => Entry.CanEdit ? "Available" : "Read-only";

    /// <summary>Gets the direct parent masters in plugin declaration order.</summary>
    public string ParentMastersText => Entry.DeclaredMasters.Count == 0
        ? "None"
        : string.Join(Environment.NewLine, Entry.DeclaredMasters.Select(master => master.FileName.String));

    /// <summary>Gets the plugin author or an explicit empty-state value.</summary>
    public string AuthorText => string.IsNullOrWhiteSpace(Entry.Author) ? "Not specified" : Entry.Author;

    /// <summary>Gets the plugin description or an explicit empty-state value.</summary>
    public string DescriptionText => string.IsNullOrWhiteSpace(Entry.Description) ? "Not specified" : Entry.Description;

    /// <summary>Gets whether the plugin header selects separate localized strings.</summary>
    public string LocalizationText => Entry.LocalizedOutputMode == LocalizedOutputMode.SeparateStringFiles
        ? "Separate strings"
        : "Embedded";

    /// <summary>Gets the absolute plugin path.</summary>
    public string PluginPathText => Entry.PluginPath;

    /// <summary>Gets the reason the plugin cannot be edited, or an editable-plugin summary.</summary>
    public string DetailsText => Entry.UnavailableReason
        ?? $"{Entry.DependencyPluginPaths.Count} declared master plugin(s) will be opened read-only.";
}
