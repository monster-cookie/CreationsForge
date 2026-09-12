namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Selects the native master style where a game supports more than one style.</summary>
public enum OutputMasterStyle
{
    /// <summary>Use a full native master.</summary>
    Full,

    /// <summary>Use a small or light native master when the selected game supports it.</summary>
    Small,

    /// <summary>Use a medium native master when the selected game supports it.</summary>
    Medium
}
