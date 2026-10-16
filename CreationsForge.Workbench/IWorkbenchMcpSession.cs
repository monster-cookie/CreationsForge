using System.Text.Json.Nodes;

namespace CreationsForge.Workbench;

/// <summary>One initialized MCP session owned by the Workbench.</summary>
internal interface IWorkbenchMcpSession : IAsyncDisposable
{
    /// <summary>Gets the protocol version agreed during initialize.</summary>
    string ProtocolVersion { get; }

    /// <summary>Gets the server name reported by MCP initialize.</summary>
    string ServerName { get; }

    /// <summary>Gets the server version reported by MCP initialize.</summary>
    string ServerVersion { get; }

    /// <summary>Gets the capability object returned by MCP initialize.</summary>
    JsonObject Capabilities { get; }

    /// <summary>Gets the tools and input schemas returned by <c>tools/list</c>.</summary>
    IReadOnlyList<WorkbenchMcpTool> Tools { get; }

    /// <summary>Gets the session identifier carried from the latest successful tool result, or <see langword="null"/> when none is active.</summary>
    string? WorkspaceId { get; }

    /// <summary>Gets the revision carried from the latest successful tool result. Zero is a real revision; <see langword="null"/> means no revision has been observed.</summary>
    ulong? Revision { get; }

    /// <summary>Gets whether the child process has exited.</summary>
    bool HasExited { get; }

    /// <summary>Raised when the child exits before the Workbench disposes the session.</summary>
    event EventHandler? HostExited;

    /// <summary>Sends one tool call and returns its JSON-RPC result object.</summary>
    /// <param name="tool">The production tool name.</param>
    /// <param name="arguments">The tool arguments. The session does not retain this instance. Missing session and revision arguments are filled from earlier results when the discovered schema names them.</param>
    /// <param name="cancellationToken">Cancels the wait and asks the server to cancel the request.</param>
    /// <returns>The tool result object. Domain failures are thrown instead of returned.</returns>
    /// <exception cref="WorkbenchMcpException">Thrown when the transport, protocol, or tool reports a failure.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the transport or the tool reports a failure.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    Task<JsonObject> CallAsync(string tool, JsonObject arguments, CancellationToken cancellationToken);
}
