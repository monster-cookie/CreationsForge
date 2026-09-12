namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Describes how a native file participates in a source or output set.
/// </summary>
public enum NativeArtifactRole
{
    /// <summary>The primary plugin file.</summary>
    Plugin,

    /// <summary>A localized STRINGS sidecar.</summary>
    Strings,

    /// <summary>A localized DLSTRINGS sidecar.</summary>
    DlStrings,

    /// <summary>A localized ILSTRINGS sidecar.</summary>
    IlStrings,

    /// <summary>A native archive selected by Mutagen as applicable localized-string input.</summary>
    StringsArchive
}
