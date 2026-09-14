namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Describes how a plugin participates in the current workspace.
/// </summary>
public enum PluginRole
{
    /// <summary>The plugin is the selected read-only source.</summary>
    Source,

    /// <summary>The plugin participates in the explicitly ordered read-only load order.</summary>
    LoadOrder,

    /// <summary>The plugin is the selected mutable output.</summary>
    Output
}
