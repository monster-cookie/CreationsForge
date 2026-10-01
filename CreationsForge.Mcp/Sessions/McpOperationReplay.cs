using CreationsForge.Mcp.Protocol;
using ModelContextProtocol.Protocol;

namespace CreationsForge.Mcp.Sessions;

/// <summary>
/// Replays identical operation identifiers for the life of this process.
/// The cache is not durable across a host restart.
/// </summary>
internal sealed class McpOperationReplay
{
    internal const int MaximumCachedResults = 128;
    internal const int MaximumExpiredIds = 1024;

    private readonly object _gate = new();
    private readonly Dictionary<string, Flight> _inFlight = new(StringComparer.Ordinal);
    private readonly Dictionary<string, CompletedOperation> _completed = new(StringComparer.Ordinal);
    private readonly Queue<string> _completedOrder = new();
    private readonly HashSet<string> _expired = new(StringComparer.Ordinal);

    /// <summary>Runs an operation once and replays the cached result for an identical identifier and payload.</summary>
    /// <param name="operationId">The client operation identifier.</param>
    /// <param name="fingerprint">The canonical tool name and arguments.</param>
    /// <param name="execute">The operation. It must not run under the replay lock.</param>
    /// <param name="cancellationToken">The caller's cancellation token. Shared execution cancels only when every waiter cancels.</param>
    /// <returns>The original or replayed tool result.</returns>
    public async Task<CallToolResult> ExecuteAsync(
        string operationId,
        string fingerprint,
        Func<CancellationToken, Task<McpInvocation>> execute,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(operationId);
        ArgumentException.ThrowIfNullOrWhiteSpace(fingerprint);
        ArgumentNullException.ThrowIfNull(execute);

        Flight? joined = null;
        Flight? owned = null;
        lock (_gate)
        {
            if (_expired.Contains(operationId))
            {
                return Expired().Replay();
            }

            if (_completed.TryGetValue(operationId, out var completed))
            {
                if (!string.Equals(completed.Fingerprint, fingerprint, StringComparison.Ordinal))
                {
                    return Conflict().Replay();
                }

                return completed.Invocation.Replay();
            }

            if (_inFlight.TryGetValue(operationId, out var existing))
            {
                if (!string.Equals(existing.Fingerprint, fingerprint, StringComparison.Ordinal))
                {
                    return Conflict().Replay();
                }

                existing.WaiterCount++;
                joined = existing;
            }
            else if (!CanAdmit())
            {
                return Capacity().Replay();
            }
            else
            {
                owned = new Flight(fingerprint);
                _inFlight.Add(operationId, owned);
            }
        }

        if (joined is not null)
        {
            return await JoinAsync(joined, cancellationToken).ConfigureAwait(false);
        }

        return await OwnAsync(operationId, owned!, execute, cancellationToken).ConfigureAwait(false);
    }

    private async Task<CallToolResult> OwnAsync(
        string operationId,
        Flight flight,
        Func<CancellationToken, Task<McpInvocation>> execute,
        CancellationToken cancellationToken)
    {
        var abandoned = 0;
        using var registration = cancellationToken.Register(() =>
        {
            if (Interlocked.Exchange(ref abandoned, 1) == 0)
            {
                Abandon(flight);
            }
        });
        if (cancellationToken.IsCancellationRequested && Interlocked.Exchange(ref abandoned, 1) == 0)
        {
            Abandon(flight);
        }

        McpInvocation invocation;
        try
        {
            invocation = await execute(flight.Execution.Token).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            invocation = McpErrorMapper.Map(exception);
        }

        Complete(operationId, flight, invocation);
        if (cancellationToken.IsCancellationRequested)
        {
            // Waiters already observed the shared result. A cacheable completion stays replayable.
            return McpErrorMapper.Canceled().Replay();
        }

        return invocation.Replay();
    }

    private async Task<CallToolResult> JoinAsync(Flight flight, CancellationToken cancellationToken)
    {
        var canceled = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        using var registration = cancellationToken.Register(() => canceled.TrySetResult());
        if (cancellationToken.IsCancellationRequested)
        {
            canceled.TrySetResult();
        }

        var winner = await Task.WhenAny(flight.Completion.Task, canceled.Task).ConfigureAwait(false);
        if (winner != flight.Completion.Task || cancellationToken.IsCancellationRequested)
        {
            Abandon(flight);
            return McpErrorMapper.Canceled().Replay();
        }

        var invocation = await flight.Completion.Task.ConfigureAwait(false);
        return invocation.Replay();
    }

    private void Abandon(Flight flight)
    {
        lock (_gate)
        {
            if (flight.Finished || flight.WaiterCount == 0)
            {
                return;
            }

            flight.WaiterCount--;
            if (flight.WaiterCount == 0)
            {
                flight.Execution.Cancel();
            }
        }
    }

    private void Complete(string operationId, Flight flight, McpInvocation invocation)
    {
        lock (_gate)
        {
            flight.Finished = true;
            flight.Completed = invocation;
            flight.Execution.Dispose();
            _inFlight.Remove(operationId);
            if (invocation.Cacheable)
            {
                Remember(operationId, flight.Fingerprint, invocation);
            }

            flight.Completion.TrySetResult(invocation);
        }
    }

    private bool CanAdmit()
    {
        return _completed.Count + _expired.Count + _inFlight.Count < MaximumCachedResults + MaximumExpiredIds;
    }

    private void Remember(string operationId, string fingerprint, McpInvocation invocation)
    {
        while (_completed.Count >= MaximumCachedResults)
        {
            if (_expired.Count >= MaximumExpiredIds || _completedOrder.Count == 0)
            {
                return;
            }

            var oldest = _completedOrder.Dequeue();
            if (_completed.Remove(oldest))
            {
                _expired.Add(oldest);
            }
        }

        _completed[operationId] = new CompletedOperation(fingerprint, invocation);
        _completedOrder.Enqueue(operationId);
    }

    private static McpInvocation Capacity()
    {
        return McpToolResults.Failure(
            "replay_capacity",
            "The operation replay cache is full and will not run a new operation.",
            cacheable: false);
    }

    private static McpInvocation Expired()
    {
        return McpToolResults.Failure(
            "replay_expired",
            "The operation identifier expired and will not be executed again.");
    }

    private static McpInvocation Conflict()
    {
        return McpToolResults.Failure(
            "replay_conflict",
            "The operation identifier was already used with a different payload.");
    }

    private sealed class Flight
    {
        public Flight(string fingerprint)
        {
            Fingerprint = fingerprint;
            Execution = new CancellationTokenSource();
            Completion = new TaskCompletionSource<McpInvocation>(TaskCreationOptions.RunContinuationsAsynchronously);
            WaiterCount = 1;
        }

        public string Fingerprint { get; }

        public CancellationTokenSource Execution { get; }

        public TaskCompletionSource<McpInvocation> Completion { get; }

        public int WaiterCount { get; set; }

        public bool Finished { get; set; }

        public McpInvocation? Completed { get; set; }
    }

    private sealed class CompletedOperation
    {
        public CompletedOperation(string fingerprint, McpInvocation invocation)
        {
            Fingerprint = fingerprint;
            Invocation = invocation;
        }

        public string Fingerprint { get; }

        public McpInvocation Invocation { get; }
    }
}
