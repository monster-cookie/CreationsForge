namespace CreationsForge.Workbench;

/// <summary>Reports a transport, protocol, or tool failure from the production MCP host.</summary>
internal sealed class WorkbenchMcpException : InvalidOperationException
{
    /// <summary>Initializes a failure that the shell can show and that tests can branch on.</summary>
    /// <param name="kind">Whether the failure came from the process, the protocol, or a tool result.</param>
    /// <param name="message">The copyable failure text. It does not include binary payloads.</param>
    /// <param name="errorCode">The host error code when one was reported; otherwise <see langword="null"/>.</param>
    /// <param name="innerException">The stream or parser exception when this failure wraps one.</param>
    public WorkbenchMcpException(
        WorkbenchMcpFailureKind kind,
        string message,
        string? errorCode = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        Kind = kind;
        ErrorCode = errorCode;
    }

    /// <summary>Gets whether this failure came from the process, the protocol, or a tool result.</summary>
    public WorkbenchMcpFailureKind Kind { get; }

    /// <summary>Gets the host error code, or <see langword="null"/> when the host did not report one.</summary>
    public string? ErrorCode { get; }
}
