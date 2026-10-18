using System.Text.Json.Nodes;
using CreationsForge.Workbench;

namespace CreationsForge.UnitTests.Workbench;

/// <summary>Provides the session members that do not start a process.</summary>
internal class WorkbenchSessionDouble : IWorkbenchMcpSession
{
    /// <summary>Gets or sets the negotiated protocol version.</summary>
    public string ProtocolVersion { get; set; } = "2025-06-18";

    /// <summary>Gets or sets the server name shown after connect.</summary>
    public string ServerName { get; set; } = "CreationsForge";

    /// <summary>Gets or sets the server version shown after connect.</summary>
    public string ServerVersion { get; set; } = "test";

    /// <summary>Gets or sets the capability object returned by initialize.</summary>
    public JsonObject Capabilities { get; set; } = new() { ["tools"] = new JsonObject() };

    /// <summary>Gets or sets the discovered tools copied into the shell.</summary>
    public IReadOnlyList<WorkbenchMcpTool> Tools { get; set; } = [];

    /// <summary>Gets or sets the carried session identifier.</summary>
    public string? WorkspaceId { get; set; }

    /// <summary>Gets or sets the carried revision. Zero is a real revision.</summary>
    public ulong? Revision { get; set; }

    /// <summary>Gets or sets whether the child has exited.</summary>
    public bool HasExited { get; set; }

    /// <summary>Gets whether <see cref="DisposeAsync"/> has run.</summary>
    public bool Disposed { get; private set; }

    /// <inheritdoc />
    public event EventHandler? HostExited;

    /// <summary>Raises <see cref="HostExited"/> as an unexpected child exit.</summary>
    public void RaiseHostExited() => HostExited?.Invoke(this, EventArgs.Empty);

    /// <inheritdoc />
    public virtual Task<JsonObject> CallAsync(string tool, JsonObject arguments, CancellationToken cancellationToken) =>
        Task.FromResult(new JsonObject());

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        Disposed = true;
        return ValueTask.CompletedTask;
    }
}
