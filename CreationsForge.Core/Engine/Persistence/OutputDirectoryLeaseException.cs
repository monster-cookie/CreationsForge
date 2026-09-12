using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Core.Engine.Persistence;

/// <summary>Carries a typed output-directory lease failure across private acquisition helpers.</summary>
internal sealed class OutputDirectoryLeaseException : Exception
{
    /// <summary>Initializes a typed output-directory lease failure.</summary>
    /// <param name="code">The stable engine failure category.</param>
    /// <param name="message">The diagnostic failure description.</param>
    /// <param name="innerException">The platform exception that caused the failure, when available.</param>
    internal OutputDirectoryLeaseException(
        EngineErrorCode code,
        string message,
        Exception? innerException = null)
        : base(message, innerException)
    {
        Code = code;
    }

    /// <summary>Gets the stable engine failure category.</summary>
    internal EngineErrorCode Code { get; }
}
