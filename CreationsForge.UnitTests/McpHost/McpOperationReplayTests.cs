using System.Text.Json;
using CreationsForge.Mcp.Protocol;
using CreationsForge.Mcp.Sessions;
using ModelContextProtocol.Protocol;

namespace CreationsForge.UnitTests.McpHost;

/// <summary>Verifies process-lifetime operation replay without starting a host.</summary>
public sealed class McpOperationReplayTests
{
    /// <summary>Replays an identical operation and rejects a different payload for the same identifier.</summary>
    [Fact]
    public async Task IdenticalReplayDoesNotExecuteAgainAndConflictDoesNotReplaceIt()
    {
        var replay = new McpOperationReplay();
        var calls = 0;

        var first = await replay.ExecuteAsync(
            "create-keyword",
            "fingerprint-a",
            _ =>
            {
                calls++;
                return Task.FromResult(McpToolResults.Success(new { ok = true }));
            },
            CancellationToken.None);
        var second = await replay.ExecuteAsync(
            "create-keyword",
            "fingerprint-a",
            _ => throw new InvalidOperationException("Identical replay executed again."),
            CancellationToken.None);
        var conflict = await replay.ExecuteAsync(
            "create-keyword",
            "fingerprint-b",
            _ => throw new InvalidOperationException("A conflicting replay executed."),
            CancellationToken.None);
        var stillOriginal = await replay.ExecuteAsync(
            "create-keyword",
            "fingerprint-a",
            _ => throw new InvalidOperationException("The original payload was replaced."),
            CancellationToken.None);

        Assert.Equal(1, calls);
        Assert.False(first.IsError);
        Assert.False(second.IsError);
        Assert.Equal("true", Structured(second).GetProperty("ok").GetRawText());
        Assert.Equal("replay_conflict", ErrorCode(conflict));
        Assert.False(stillOriginal.IsError);
    }

    /// <summary>Expires the oldest cached identifier after the closed cache limit and does not execute it again.</summary>
    [Fact]
    public async Task OldestIdentifierExpiresAfterCacheLimit()
    {
        var replay = new McpOperationReplay();
        for (var index = 0; index < 128; index++)
        {
            var cached = await replay.ExecuteAsync(
                $"op-{index}",
                "same",
                _ => Task.FromResult(McpToolResults.Success(new { index })),
                CancellationToken.None);
            Assert.False(cached.IsError);
        }

        var newest = await replay.ExecuteAsync(
            "op-128",
            "same",
            _ => Task.FromResult(McpToolResults.Success(new { index = 128 })),
            CancellationToken.None);
        Assert.False(newest.IsError);

        var calls = 0;
        var expired = await replay.ExecuteAsync(
            "op-0",
            "same",
            _ =>
            {
                calls++;
                return Task.FromResult(McpToolResults.Success(new { ok = true }));
            },
            CancellationToken.None);

        Assert.Equal(0, calls);
        Assert.Equal("replay_expired", ErrorCode(expired));
    }

    /// <summary>Returns cancellation to a joiner without stopping an owner that is still waiting.</summary>
    [Fact]
    public async Task JoinerCancellationDoesNotStopOwner()
    {
        var replay = new McpOperationReplay();
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        CancellationToken observed = default;
        var calls = 0;

        var owner = replay.ExecuteAsync(
            "join",
            "same",
            async token =>
            {
                Interlocked.Increment(ref calls);
                observed = token;
                started.TrySetResult();
                await release.Task.ConfigureAwait(false);
                return McpToolResults.Success(new { ok = true });
            },
            CancellationToken.None);
        await started.Task.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

        using var canceled = new CancellationTokenSource();
        await canceled.CancelAsync();
        var joined = await replay.ExecuteAsync(
            "join",
            "same",
            _ => throw new InvalidOperationException("A joiner executed the operation."),
            canceled.Token);

        Assert.Equal("canceled", ErrorCode(joined));
        Assert.False(observed.IsCancellationRequested);
        release.TrySetResult();
        var ownerResult = await owner.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);
        Assert.False(ownerResult.IsError);
        Assert.Equal(1, calls);
    }

    /// <summary>Returns cancellation only to the canceled owner and keeps a cacheable result replayable.</summary>
    [Fact]
    public async Task OwnerCancellationKeepsACacheableResult()
    {
        var replay = new McpOperationReplay();
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var calls = 0;
        using var canceled = new CancellationTokenSource();

        var owner = replay.ExecuteAsync(
            "owner",
            "same",
            async _ =>
            {
                Interlocked.Increment(ref calls);
                started.TrySetResult();
                await release.Task.ConfigureAwait(false);
                return McpToolResults.Success(new { ok = true });
            },
            canceled.Token);
        await started.Task.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);
        var joiner = replay.ExecuteAsync(
            "owner",
            "same",
            _ => throw new InvalidOperationException("A joiner executed the operation."),
            CancellationToken.None);
        await canceled.CancelAsync();
        release.TrySetResult();

        var canceledResult = await owner.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);
        var joined = await joiner.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);
        var replayed = await replay.ExecuteAsync(
            "owner",
            "same",
            _ =>
            {
                Interlocked.Increment(ref calls);
                return Task.FromResult(McpToolResults.Success(new { ok = false }));
            },
            CancellationToken.None);

        Assert.Equal("canceled", ErrorCode(canceledResult));
        Assert.False(joined.IsError);
        Assert.Equal("true", Structured(joined).GetProperty("ok").GetRawText());
        Assert.False(replayed.IsError);
        Assert.Equal("true", Structured(replayed).GetProperty("ok").GetRawText());
        Assert.Equal(1, calls);
    }

    /// <summary>Refuses a new operation at the admission ceiling without expiring an identifier that never ran.</summary>
    [Fact]
    public async Task DistinctOperationsStopAtTheAdmissionCeiling()
    {
        var replay = new McpOperationReplay();
        var limit = McpOperationReplay.MaximumCachedResults + McpOperationReplay.MaximumExpiredIds;
        for (var index = 0; index < limit; index++)
        {
            var cached = await replay.ExecuteAsync(
                index.ToString(),
                "same",
                _ => Task.FromResult(McpToolResults.Success(new { index })),
                CancellationToken.None);
            Assert.False(cached.IsError);
        }

        var refused = await replay.ExecuteAsync(
            "overflow",
            "same",
            _ => throw new InvalidOperationException("A refused operation executed."),
            CancellationToken.None);
        var expired = await replay.ExecuteAsync(
            "0",
            "same",
            _ => throw new InvalidOperationException("An expired identifier executed."),
            CancellationToken.None);
        var replayed = await replay.ExecuteAsync(
            (limit - 1).ToString(),
            "same",
            _ => throw new InvalidOperationException("A cached identifier executed again."),
            CancellationToken.None);

        Assert.Equal("replay_capacity", ErrorCode(refused));
        Assert.Equal("replay_expired", ErrorCode(expired));
        Assert.False(replayed.IsError);
    }

    private static string ErrorCode(CallToolResult result)
    {
        Assert.True(result.IsError);
        return Structured(result).GetProperty("code").GetString()
            ?? throw new InvalidOperationException("The structured error did not contain a code.");
    }

    private static JsonElement Structured(CallToolResult result)
    {
        return result.StructuredContent
            ?? throw new InvalidOperationException("The tool result did not contain structured content.");
    }
}
