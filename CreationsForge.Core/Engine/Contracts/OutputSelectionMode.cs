namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Specifies whether output selection must create a new plugin or open an existing plugin.</summary>
public enum OutputSelectionMode
{
    /// <summary>Create a new output and reject an existing destination.</summary>
    CreateNew,

    /// <summary>Open an existing output and reject an absent destination.</summary>
    OpenExisting
}
