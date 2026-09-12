using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Core.Engine.NativeInputs;

/// <summary>Represents a stable source-input boundary failure before it is projected into an engine result.</summary>
internal sealed class NativeSourceInputException : Exception
{
    /// <summary>Initializes a source-input failure.</summary>
    /// <param name="code">The stable engine failure category.</param>
    /// <param name="message">The diagnostic failure description.</param>
    /// <param name="innerException">The native or file-system exception that caused the failure, when available.</param>
    internal NativeSourceInputException(EngineErrorCode code, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        Code = code;
    }

    /// <summary>Gets the stable engine failure category.</summary>
    internal EngineErrorCode Code { get; }
}
