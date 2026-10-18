namespace CreationsForge.Workbench;

/// <summary>Classifies a failure reported by the production MCP child.</summary>
internal enum WorkbenchMcpFailureKind
{
    /// <summary>The child process ended, closed a stream, or wrote text that is not a protocol frame.</summary>
    Transport,

    /// <summary>The host rejected a protocol request or negotiated a capability set this Workbench cannot use.</summary>
    Protocol,

    /// <summary>A tool ran and returned a domain failure.</summary>
    Tool,
}
