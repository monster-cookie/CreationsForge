namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Selects how localized strings are represented in a native output.</summary>
public enum LocalizedOutputMode
{
    /// <summary>Store supported strings directly in the plugin.</summary>
    Embedded,

    /// <summary>Store translated strings in the complete native plugin-and-strings output set.</summary>
    SeparateStringFiles
}
