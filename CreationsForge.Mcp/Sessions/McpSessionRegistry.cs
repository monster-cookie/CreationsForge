using CreationsForge.Engine.Workspaces;
using CreationsForge.Mcp.Protocol;

namespace CreationsForge.Mcp.Sessions;

/// <summary>Guards the one authoring session without holding its lock across save, discard, or dispose.</summary>
internal sealed class McpSessionRegistry
{
    private readonly object _gate = new();
    private McpAuthoringSession? _session;

    /// <summary>Creates the pending session when none exists.</summary>
    /// <param name="session">The created session when creation succeeds.</param>
    /// <returns>A failure when a session already exists; otherwise <see langword="null"/>.</returns>
    public McpInvocation? TryCreate(McpAuthoringSession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        lock (_gate)
        {
            if (_session is not null)
            {
                return AlreadyOpen();
            }

            _session = session;
            return null;
        }
    }

    /// <summary>Copies the matching session when it still exists.</summary>
    /// <param name="workspaceId">The client session identifier.</param>
    /// <param name="session">The matching session.</param>
    /// <returns>A not-found failure, or <see langword="null"/> when the session matches.</returns>
    public McpInvocation? TryGet(string workspaceId, out McpAuthoringSession? session)
    {
        lock (_gate)
        {
            if (_session is null || !string.Equals(_session.Id, workspaceId, StringComparison.Ordinal))
            {
                session = null;
                return NotFound(workspaceId);
            }

            session = _session;
            return null;
        }
    }

    /// <summary>Returns the native workspace when the matching session is open.</summary>
    /// <param name="workspaceId">The client session identifier.</param>
    /// <param name="workspace">The open native workspace.</param>
    /// <returns>A failure when the session is missing or not open; otherwise <see langword="null"/>.</returns>
    public McpInvocation? TryGetOpen(string workspaceId, out PluginWorkspace? workspace)
    {
        lock (_gate)
        {
            if (_session is null || !string.Equals(_session.Id, workspaceId, StringComparison.Ordinal))
            {
                workspace = null;
                return NotFound(workspaceId);
            }

            if (_session.Phase != McpSessionPhase.Open || _session.Workspace is null)
            {
                workspace = null;
                return NotOpen(_session.Phase);
            }

            workspace = _session.Workspace;
            return null;
        }
    }

    /// <summary>Marks a pending session as opening so a second output request cannot overlap it.</summary>
    /// <param name="workspaceId">The client session identifier.</param>
    /// <param name="session">The session that may call the workspace factory.</param>
    /// <returns>A failure when the session cannot begin opening; otherwise <see langword="null"/>.</returns>
    public McpInvocation? TryBeginOutput(string workspaceId, out McpAuthoringSession? session)
    {
        lock (_gate)
        {
            if (_session is null || !string.Equals(_session.Id, workspaceId, StringComparison.Ordinal))
            {
                session = null;
                return NotFound(workspaceId);
            }

            if (_session.Phase != McpSessionPhase.Pending || _session.Workspace is not null)
            {
                session = null;
                return AlreadyOpen();
            }

            _session.Phase = McpSessionPhase.Opening;
            session = _session;
            return null;
        }
    }

    /// <summary>Attaches a successfully opened native workspace.</summary>
    /// <param name="session">The session that began opening.</param>
    /// <param name="workspace">The native workspace.</param>
    public void CompleteOutput(McpAuthoringSession session, PluginWorkspace workspace)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(workspace);
        lock (_gate)
        {
            if (!ReferenceEquals(_session, session))
            {
                throw new InvalidOperationException("The authoring session changed while output open was completing.");
            }

            session.Workspace = workspace;
            session.Phase = McpSessionPhase.Open;
        }
    }

    /// <summary>Returns a failed output open to the pending phase.</summary>
    /// <param name="session">The session that began opening.</param>
    public void FailOutput(McpAuthoringSession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        lock (_gate)
        {
            if (ReferenceEquals(_session, session) && session.Phase == McpSessionPhase.Opening)
            {
                session.Phase = McpSessionPhase.Pending;
            }
        }
    }

    /// <summary>Marks the matching session closing and returns its native workspace, if any.</summary>
    /// <param name="workspaceId">The client session identifier.</param>
    /// <param name="session">The session being closed.</param>
    /// <param name="workspace">The native workspace, or <see langword="null"/> when the session is still pending.</param>
    /// <returns>A failure when close cannot start; otherwise <see langword="null"/>.</returns>
    public McpInvocation? TryBeginClose(string workspaceId, out McpAuthoringSession? session, out PluginWorkspace? workspace)
    {
        lock (_gate)
        {
            if (_session is null || !string.Equals(_session.Id, workspaceId, StringComparison.Ordinal))
            {
                session = null;
                workspace = null;
                return NotFound(workspaceId);
            }

            if (_session.Phase is McpSessionPhase.Opening or McpSessionPhase.Closing)
            {
                session = null;
                workspace = null;
                return McpToolResults.Failure(
                    "invalid_input",
                    "The workspace session cannot close while another open or close is in progress.",
                    new { phase = _session.Phase.ToString() });
            }

            _session.Phase = McpSessionPhase.Closing;
            session = _session;
            workspace = _session.Workspace;
            return null;
        }
    }

    /// <summary>Restores an open or pending phase when close stops before dispose.</summary>
    /// <param name="session">The session whose close was aborted.</param>
    public void AbortClose(McpAuthoringSession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        lock (_gate)
        {
            if (ReferenceEquals(_session, session) && session.Phase == McpSessionPhase.Closing)
            {
                session.Phase = session.Workspace is null ? McpSessionPhase.Pending : McpSessionPhase.Open;
            }
        }
    }

    /// <summary>Removes the session after close has released its native workspace.</summary>
    /// <param name="session">The closed session.</param>
    public void CompleteClose(McpAuthoringSession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        lock (_gate)
        {
            if (ReferenceEquals(_session, session))
            {
                _session = null;
            }
        }
    }

    private static McpInvocation NotFound(string workspaceId)
    {
        return McpToolResults.Failure(
            "workspace_not_found",
            $"Workspace '{workspaceId}' was not found.",
            new { workspaceId });
    }

    private static McpInvocation NotOpen(McpSessionPhase phase)
    {
        return McpToolResults.Failure(
            "workspace_not_open",
            "The workspace session does not have an open native workspace.",
            new { phase = phase.ToString() });
    }

    private static McpInvocation AlreadyOpen()
    {
        return McpToolResults.Failure(
            "workspace_already_open",
            "A workspace session is already open.");
    }
}
