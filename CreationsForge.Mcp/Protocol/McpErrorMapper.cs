using CreationsForge.Engine.Persistence;
using CreationsForge.Engine.Records;
using CreationsForge.Engine.Workspaces;

namespace CreationsForge.Mcp.Protocol;

/// <summary>Maps engine failures onto the closed authoring error codes.</summary>
internal static class McpErrorMapper
{
    /// <summary>Maps an exception thrown by the engine or contract layer.</summary>
    /// <param name="exception">The failure to map.</param>
    /// <returns>A structured tool failure. Unexpected failures do not include a stack trace.</returns>
    public static McpInvocation Map(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);
        if (exception is McpContractException contract)
        {
            return McpToolResults.Failure(contract.Code, contract.Message);
        }

        if (exception is OperationCanceledException)
        {
            return Canceled();
        }

        if (exception is AggregateException aggregate)
        {
            var inner = aggregate.Flatten().InnerExceptions;
            if (inner.Count == 1)
            {
                return Map(inner[0]);
            }

            if (inner.All(item => item is OperationCanceledException))
            {
                return Canceled();
            }
        }

        var busy = FindLock(exception);
        if (busy is not null)
        {
            return McpToolResults.Failure("file_busy", busy.Message, cacheable: false);
        }

        if (exception is RecordEditingException)
        {
            return MapEditing(exception.Message);
        }

        if (exception is PluginWorkspaceException)
        {
            return MapWorkspace(exception.Message);
        }

        if (exception is ArgumentException)
        {
            return McpToolResults.Failure("invalid_input", exception.Message);
        }

        return HostFailure(exception);
    }

    /// <summary>Maps a completed save result that did not throw.</summary>
    /// <param name="result">The bounded save result.</param>
    /// <param name="success">The success payload used when publication was adopted.</param>
    /// <returns>The structured save outcome.</returns>
    public static McpInvocation MapSave(PluginSaveResult result, object success)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(success);
        var details = new
        {
            status = result.Status.ToString(),
            publicationState = result.PublicationState.ToString(),
            revision = result.Revision,
            requiresReopen = result.RequiresWorkspaceReopen,
            destinationPath = result.DestinationPath,
        };

        if (result.Status is PluginSaveStatus.Succeeded or PluginSaveStatus.NoChanges)
        {
            return McpToolResults.Success(success);
        }

        if (result.Status == PluginSaveStatus.Canceled)
        {
            return McpToolResults.Failure("canceled", result.Diagnostic, details, cacheable: false);
        }

        if (result.Status == PluginSaveStatus.PublishedButReopenFailed)
        {
            return McpToolResults.Failure("saved_reopen_failed", result.Diagnostic, details);
        }

        if (result.PublicationState == PluginPublicationState.PartiallyPublished)
        {
            return McpToolResults.Failure("partial_publication", result.Diagnostic, details);
        }

        if (result.RequiresWorkspaceReopen)
        {
            return McpToolResults.Failure("requires_reopen", result.Diagnostic, details);
        }

        if (result.Status == PluginSaveStatus.Failed
            && result.PublicationState is PluginPublicationState.Unchanged or PluginPublicationState.Restored)
        {
            return McpToolResults.Failure("save_failed", result.Diagnostic, details, cacheable: false);
        }

        return McpToolResults.Failure("save_failed", result.Diagnostic, details, cacheable: false);
    }

    /// <summary>Creates an uncached cancellation failure.</summary>
    /// <returns>The cancellation failure.</returns>
    public static McpInvocation Canceled()
    {
        return McpToolResults.Failure("canceled", "The operation was canceled.", cacheable: false);
    }

    /// <summary>Creates a cacheable host failure that does not expose a stack trace.</summary>
    /// <param name="exception">The unexpected exception.</param>
    /// <returns>The host failure.</returns>
    public static McpInvocation HostFailure(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);
        return McpToolResults.Failure(
            "host_failure",
            "The host could not complete the operation.",
            new { exceptionType = exception.GetType().Name });
    }

    private static PluginWorkspaceLockException? FindLock(Exception exception)
    {
        if (exception is PluginWorkspaceLockException busy)
        {
            return busy;
        }

        if (exception is AggregateException aggregate)
        {
            foreach (var inner in aggregate.Flatten().InnerExceptions)
            {
                var found = FindLock(inner);
                if (found is not null)
                {
                    return found;
                }
            }
        }

        return exception.InnerException is null ? null : FindLock(exception.InnerException);
    }

    private static McpInvocation MapEditing(string message)
    {
        if (message.Contains("Workspace revision is", StringComparison.Ordinal))
        {
            return McpToolResults.Failure("stale_revision", message);
        }

        if (message.Contains("requires a fresh reopen", StringComparison.Ordinal))
        {
            return McpToolResults.Failure("requires_reopen", message);
        }

        if (message.Contains("expected getter", StringComparison.Ordinal)
            || message.Contains("expected setter", StringComparison.Ordinal)
            || message.Contains("Mutagen mod type", StringComparison.Ordinal)
            || message.Contains("is not declared family", StringComparison.Ordinal)
            || message.Contains("is not mutable family", StringComparison.Ordinal)
            || message.Contains("is not a Mutagen major record", StringComparison.Ordinal))
        {
            return McpToolResults.Failure("unsupported_shape", message);
        }

        if (message.StartsWith("Field ", StringComparison.Ordinal)
            || message.Contains("Field path", StringComparison.Ordinal)
            || message.Contains("is not legal for field", StringComparison.Ordinal)
            || message.Contains("is not editable", StringComparison.Ordinal))
        {
            return McpToolResults.Failure("invalid_field", message);
        }

        return McpToolResults.Failure("invalid_input", message);
    }

    private static McpInvocation MapWorkspace(string message)
    {
        if (message.Contains("Workspace revision is", StringComparison.Ordinal))
        {
            return McpToolResults.Failure("stale_revision", message);
        }

        if (message.Contains("Required source plugin", StringComparison.Ordinal)
            && message.Contains("is missing", StringComparison.Ordinal))
        {
            return McpToolResults.Failure("missing_master", message);
        }

        if (message.Contains("requires a fresh reopen", StringComparison.Ordinal))
        {
            return McpToolResults.Failure("requires_reopen", message);
        }

        if (message.Contains("changed while the workspace was opening", StringComparison.Ordinal))
        {
            return McpToolResults.Failure("file_busy", message, cacheable: false);
        }

        if (message.StartsWith("Could not open the", StringComparison.Ordinal)
            || message.StartsWith("Could not read", StringComparison.Ordinal))
        {
            return McpToolResults.Failure("host_failure", message);
        }

        return McpToolResults.Failure("invalid_input", message);
    }
}
