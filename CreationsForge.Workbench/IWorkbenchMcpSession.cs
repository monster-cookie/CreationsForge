using System.Text.Json.Nodes;

namespace CreationsForge.Workbench;

/// <summary>One initialized MCP session owned by the Workbench.</summary>
internal interface IWorkbenchMcpSession : IAsyncDisposable
{
    /// <summary>Gets the server version reported by MCP initialize.</summary>
    string ServerVersion { get; }

    /// <summary>Sends one tool call and returns its JSON-RPC result object.</summary>
    /// <param name="tool">The production tool name.</param>
    /// <param name="arguments">The tool arguments. The session does not retain this instance.</param>
    /// <param name="cancellationToken">Cancels the wait and asks the server to cancel the request.</param>
    /// <returns>The tool result object. Domain failures are thrown instead of returned.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the transport or the tool reports a failure.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    Task<JsonObject> CallAsync(string tool, JsonObject arguments, CancellationToken cancellationToken);
}
