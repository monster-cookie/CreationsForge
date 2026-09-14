namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Selects the plugin master style where a game supports more than one style.</summary>
public enum OutputMasterStyle
{
    /// <summary>Use a full plugin master.</summary>
    Full,

    /// <summary>Use a small or light plugin master when the selected game supports it.</summary>
    Small,

    /// <summary>Use a medium plugin master when the selected game supports it.</summary>
    Medium
}
