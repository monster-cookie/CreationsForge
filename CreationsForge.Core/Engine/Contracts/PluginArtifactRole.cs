namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Describes how a plugin file participates in a source or output set.
/// </summary>
public enum PluginArtifactRole
{
    /// <summary>The primary plugin file.</summary>
    Plugin,

    /// <summary>A localized STRINGS sidecar.</summary>
    Strings,

    /// <summary>A localized DLSTRINGS sidecar.</summary>
    DlStrings,

    /// <summary>A localized ILSTRINGS sidecar.</summary>
    IlStrings,

    /// <summary>A plugin archive selected by Mutagen as applicable localized-string input.</summary>
    StringsArchive
}
