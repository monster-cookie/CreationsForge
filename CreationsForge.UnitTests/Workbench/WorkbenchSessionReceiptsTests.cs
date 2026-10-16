using System.Text.Json.Nodes;
using CreationsForge.Workbench;

namespace CreationsForge.UnitTests.Workbench;

/// <summary>Verifies automatic session and revision carrying without starting a process.</summary>
public sealed class WorkbenchSessionReceiptsTests
{
    /// <summary>Fills only the schema properties that the call left out, including revision zero.</summary>
    [Fact]
    public void ApplyFillsMissingSessionAndRevision()
    {
        var schema = new JsonObject
        {
            ["type"] = "object",
            ["properties"] = new JsonObject
            {
                ["workspaceId"] = new JsonObject { ["type"] = "string" },
                ["expectedRevision"] = new JsonObject { ["type"] = "integer" },
                ["familyId"] = new JsonObject { ["type"] = "string" },
            },
        };
        var arguments = new JsonObject { ["familyId"] = "Keyword" };

        WorkbenchSessionReceipts.Apply(arguments, schema, "session-1", 0);

        Assert.Equal("session-1", arguments["workspaceId"]?.GetValue<string>());
        Assert.Equal(0UL, arguments["expectedRevision"]?.GetValue<ulong>());
        Assert.Equal("Keyword", arguments["familyId"]?.GetValue<string>());
    }

    /// <summary>Leaves explicit identifiers unchanged and ignores tools whose schema does not name them.</summary>
    [Fact]
    public void ApplyPreservesExplicitValuesAndSkipsUnknownSchemas()
    {
        var schema = new JsonObject
        {
            ["properties"] = new JsonObject
            {
                ["workspaceId"] = new JsonObject(),
                ["expectedRevision"] = new JsonObject(),
            },
        };
        var arguments = new JsonObject
        {
            ["workspaceId"] = "kept",
            ["expectedRevision"] = 4,
        };

        WorkbenchSessionReceipts.Apply(arguments, schema, "session-1", 9);
        WorkbenchSessionReceipts.Apply(new JsonObject { ["name"] = "kept" }, inputSchema: null, "session-1", 9);

        Assert.Equal("kept", arguments["workspaceId"]?.GetValue<string>());
        Assert.Equal(4, arguments["expectedRevision"]?.GetValue<int>());
    }

    /// <summary>Stores identifiers from structured content and clears them after workspace close.</summary>
    [Fact]
    public void CaptureStoresReceiptsAndCloseClearsThem()
    {
        string? workspaceId = null;
        ulong? revision = null;
        var opened = new JsonObject
        {
            ["structuredContent"] = new JsonObject
            {
                ["workspaceId"] = "session-1",
                ["revision"] = 0,
            },
        };

        WorkbenchSessionReceipts.Capture("workspace_open", opened, ref workspaceId, ref revision);
        Assert.Equal("session-1", workspaceId);
        Assert.Equal(0UL, revision);

        WorkbenchSessionReceipts.Capture(
            "workspace_close",
            new JsonObject { ["structuredContent"] = new JsonObject { ["workspaceId"] = "session-1", ["revision"] = 3 } },
            ref workspaceId,
            ref revision);
        Assert.Null(workspaceId);
        Assert.Null(revision);
    }
}
