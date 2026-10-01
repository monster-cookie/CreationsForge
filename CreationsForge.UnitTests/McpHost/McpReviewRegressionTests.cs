using CreationsForge.Engine.Workspaces;
using CreationsForge.Mcp;
using CreationsForge.Mcp.Protocol;
using CreationsForge.Mcp.Sessions;
using ModelContextProtocol.Protocol;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.UnitTests.McpHost;

/// <summary>Regression coverage for the VWCF-35 production-contract review.</summary>
public sealed class McpReviewRegressionTests
{
    /// <summary>Restores a pending session when close is canceled before the session is removed.</summary>
    [Fact]
    public async Task PendingCloseCancellationRestoresTheSession()
    {
        var sessions = new McpSessionRegistry();
        var service = CreateService(sessions);
        var session = new McpAuthoringSession("pending-close", GameRelease.Starfield, "C:\\Plugins", []);
        Assert.Null(sessions.TryCreate(session));
        using var canceled = new CancellationTokenSource();
        await canceled.CancelAsync();

        var failed = await service.WorkspaceCloseAsync("close-canceled", session.Id, false, canceled.Token);
        var restored = sessions.TryGet(session.Id, out var stillOpen);

        Assert.Equal("canceled", ErrorCode(failed));
        Assert.Null(restored);
        Assert.NotNull(stillOpen);
        Assert.Equal(McpSessionPhase.Pending, stillOpen.Phase);

        var closed = await service.WorkspaceCloseAsync("close-ok", session.Id, false, CancellationToken.None);
        var removed = sessions.TryGet(session.Id, out var gone);
        Assert.False(closed.IsError);
        Assert.NotNull(removed);
        Assert.Null(gone);
    }

    /// <summary>Fingerprints ignore object key order while cursor serialization preserves it.</summary>
    [Fact]
    public void FingerprintIgnoresObjectKeyOrder()
    {
        var first = new { b = 1, a = 2 };
        var swapped = new { a = 2, b = 1 };

        Assert.Equal(McpToolResults.Fingerprint(first), McpToolResults.Fingerprint(swapped));
        Assert.NotEqual(McpToolResults.Serialize(first), McpToolResults.Serialize(swapped));
    }

    /// <summary>Rejects a null child inside a list or object value.</summary>
    [Fact]
    public void NestedNullRecordValueIsInvalidInput()
    {
        var list = new McpRecordValueDto { Kind = "List", Items = [null!] };
        var map = new McpRecordValueDto
        {
            Kind = "Object",
            Alternative = "Child",
            Fields = new Dictionary<string, McpRecordValueDto> { ["child"] = null! },
        };

        var listError = Assert.Throws<McpContractException>(() => McpValueMapper.ToRecordValue(list));
        var mapError = Assert.Throws<McpContractException>(() => McpValueMapper.ToRecordValue(map));

        Assert.Equal("invalid_input", listError.Code);
        Assert.Contains("nested record value", listError.Message, StringComparison.Ordinal);
        Assert.Equal("invalid_input", mapError.Code);
    }

    /// <summary>Rejects undefined numeric enum text without rejecting a defined member name.</summary>
    [Theory]
    [InlineData("99")]
    [InlineData("-1")]
    public void UndefinedNumericEnumsAreRejected(string value)
    {
        var supported = new HashSet<GameRelease> { GameRelease.Starfield };
        var release = Assert.Throws<McpContractException>(() => McpAuthoringArguments.ParseRelease(value, supported));
        var style = Assert.Throws<McpContractException>(() => McpAuthoringArguments.ParseMasterStyle(value));
        var storage = Assert.Throws<McpContractException>(() => McpAuthoringArguments.ParseTextStorage(value));
        var language = Assert.Throws<McpContractException>(() => McpAuthoringArguments.ParseLanguage(value));
        var operation = Assert.Throws<McpContractException>(() => McpAuthoringArguments.ParseChanges(
        [
            new McpFieldChangeDto { Path = "EditorID", Operation = value },
        ]));
        var kind = Assert.Throws<McpContractException>(() =>
            McpValueMapper.ToRecordValue(new McpRecordValueDto { Kind = value }));

        Assert.Equal(GameRelease.Starfield, McpAuthoringArguments.ParseRelease("Starfield", supported));
        Assert.Equal(MasterStyle.Full, McpAuthoringArguments.ParseMasterStyle("Full"));
        Assert.Equal(PluginTextStorageMode.Embedded, McpAuthoringArguments.ParseTextStorage("Embedded"));
        Assert.Equal(Language.English, McpAuthoringArguments.ParseLanguage("English"));
        Assert.Equal("invalid_input", release.Code);
        Assert.Equal("invalid_input", style.Code);
        Assert.Equal("invalid_input", storage.Code);
        Assert.Equal("invalid_input", language.Code);
        Assert.Equal("invalid_input", operation.Code);
        Assert.Equal("invalid_input", kind.Code);
    }

    /// <summary>Maps a cursor revision mismatch to the stable stale-revision code.</summary>
    [Fact]
    public void WorkspaceRevisionMismatchMapsToStaleRevision()
    {
        var mapped = McpErrorMapper.Map(
            new PluginWorkspaceException("Workspace revision is '4', not cursor revision '3'."));
        var error = mapped.Replay();

        Assert.Equal("stale_revision", ErrorCode(error));
    }

    private static McpAuthoringService CreateService(McpSessionRegistry sessions) =>
        new([], new PluginWorkspaceFactory([]), sessions, new McpOperationReplay());

    private static string ErrorCode(CallToolResult result)
    {
        Assert.True(result.IsError);
        return result.StructuredContent?.GetProperty("code").GetString()
            ?? throw new InvalidOperationException("The structured error did not contain a code.");
    }
}
