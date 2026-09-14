namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Selects how localized strings are represented in a plugin output.</summary>
public enum LocalizedOutputMode
{
    /// <summary>Store supported strings directly in the plugin.</summary>
    Embedded,

    /// <summary>Store translated strings in the complete plugin-and-strings output set.</summary>
    SeparateStringFiles
}
