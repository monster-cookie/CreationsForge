using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Core.Engine.PluginInputs;

/// <summary>Represents a stable source-input boundary failure before it is projected into an engine result.</summary>
internal sealed class PluginSourceInputException : Exception
{
    /// <summary>Initializes a source-input failure.</summary>
    /// <param name="code">The stable engine failure category.</param>
    /// <param name="message">The diagnostic failure description.</param>
    /// <param name="innerException">The plugin or file-system exception that caused the failure, when available.</param>
    internal PluginSourceInputException(EngineErrorCode code, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        Code = code;
    }

    /// <summary>Gets the stable engine failure category.</summary>
    internal EngineErrorCode Code { get; }
}
