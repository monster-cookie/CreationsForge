using System.Text.Json;
using CreationsForge.Engine.Records;
using CreationsForge.Engine.Workspaces;
using CreationsForge.Mcp;
using CreationsForge.Mcp.Protocol;
using CreationsForge.Mcp.Sessions;
using ModelContextProtocol.Protocol;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.UnitTests.Workspaces;

/// <summary>Verifies revision-gated reads and preview paging from the VWCF-35 review.</summary>
public sealed partial class PluginWorkspaceTests
{
    /// <summary>Rejects a stale search revision and a canceled scan without returning a page.</summary>
    [Fact]
    public void SearchHonorsCursorRevisionAndCancellation()
    {
        using var directory = new TemporaryDirectory();
        using var workspace = OpenEmptyStarfield(directory);
        using var canceled = new CancellationTokenSource();
        canceled.Cancel();

        var stale = Assert.Throws<PluginWorkspaceException>(() => workspace.SearchWinningRecords("Keyword", null, 0, 1, expectedRevision: workspace.State.Revision + 1, cancellationToken: TestContext.Current.CancellationToken));
        Assert.Throws<OperationCanceledException>(() => workspace.SearchWinningRecords(
            "Keyword",
            null,
            0,
            1,
            cancellationToken: canceled.Token));
        var page = workspace.SearchWinningRecords("Keyword", null, 0, 1, workspace.State.Revision, TestContext.Current.CancellationToken);

        Assert.Contains("not cursor revision", stale.Message, StringComparison.Ordinal);
        Assert.Equal(workspace.State.Revision, page.Revision);
        Assert.Empty(page.Records);
    }

    /// <summary>Returns the revision observed inside the read and compare gates and rejects a stale cursor.</summary>
    [Fact]
    public void ReadAndCompareObserveTheGatedRevision()
    {
        using var directory = new TemporaryDirectory();
        using var workspace = OpenEmptyStarfield(directory);
        var created = workspace.Records.Apply(new RecordChangeSet(
            workspace.State.Revision,
            [RecordMutation.Create("Keyword", [SetEditorId("Observed")])])
            ).Records.Single();
        var locator = new RecordLocator("Keyword", created.FormKey, created.ContainingModKey);

        var (snapshot, readRevision) = workspace.Records.ReadObserved(locator, workspace.State.Revision);
        var (_, compareRevision) = workspace.Records.CompareObserved(locator, locator, workspace.State.Revision);
        var staleRead = Assert.Throws<PluginWorkspaceException>(() =>
            workspace.Records.ReadObserved(locator, workspace.State.Revision + 1));
        var staleCompare = Assert.Throws<PluginWorkspaceException>(() =>
            workspace.Records.CompareObserved(locator, locator, workspace.State.Revision + 1));
        var reread = workspace.Records.ReadObserved(locator, workspace.State.Revision);

        Assert.Equal(created.FormKey, snapshot.FormKey);
        Assert.Equal(workspace.State.Revision, readRevision);
        Assert.Equal(workspace.State.Revision, compareRevision);
        Assert.Equal(workspace.State.Revision, reread.Revision);
        Assert.Contains("not cursor revision", staleRead.Message, StringComparison.Ordinal);
        Assert.Contains("not cursor revision", staleCompare.Message, StringComparison.Ordinal);
    }

    /// <summary>Maps a stale search, read, or compare cursor to stale_revision and still serves the current revision.</summary>
    [Fact]
    public async Task StaleRecordCursorsMapToStaleRevision()
    {
        using var directory = new TemporaryDirectory();
        using var workspace = OpenEmptyStarfield(directory);
        var created = workspace.Records.Apply(new RecordChangeSet(
            workspace.State.Revision,
            [RecordMutation.Create("Keyword", [SetEditorId("Cursor")])])
            ).Records.Single();
        var (service, workspaceId) = Attach(workspace, directory.Path);
        var formKey = created.FormKey.ToString();
        var containing = created.ContainingModKey.ToString();
        var staleRevision = workspace.State.Revision + 1;

        var search = await service.RecordsSearchAsync(
            workspaceId,
            "Keyword",
            null,
            Cursor(McpAuthoringProjection.SearchCursor, workspaceId, staleRevision, "Keyword"),
            CancellationToken.None);
        var read = await service.RecordReadAsync(
            workspaceId,
            "Keyword",
            formKey,
            containing,
            Cursor(McpAuthoringProjection.FieldsCursor, workspaceId, staleRevision, "Keyword", formKey, containing),
            CancellationToken.None);
        var compare = await service.RecordCompareAsync(
            workspaceId,
            "Keyword",
            formKey,
            containing,
            formKey,
            containing,
            CompareCursor(workspaceId, staleRevision, "Keyword", formKey, containing),
            CancellationToken.None);
        var current = await service.RecordsSearchAsync(
            workspaceId,
            "Keyword",
            null,
            null,
            CancellationToken.None);

        Assert.Equal("stale_revision", ErrorCode(search));
        Assert.Equal("stale_revision", ErrorCode(read));
        Assert.Equal("stale_revision", ErrorCode(compare));
        Assert.False(current.IsError);
        Assert.Equal(workspace.State.Revision, current.StructuredContent!.Value.GetProperty("revision").GetUInt64());
    }

    /// <summary>Pages pending preview records before projecting the returned page.</summary>
    [Fact]
    public async Task PreviewReturnsOnePendingPage()
    {
        using var directory = new TemporaryDirectory();
        using var workspace = OpenEmptyStarfield(directory);
        var count = PluginRecordSearchPage.MaximumTake + 1;
        var mutations = new List<RecordMutation>(count);
        for (var index = 0; index < count; index++)
        {
            mutations.Add(RecordMutation.Create("Keyword", [SetEditorId($"Paged{index:D2}")]));
        }

        workspace.Records.Apply(new RecordChangeSet(workspace.State.Revision, mutations));
        var (service, workspaceId) = Attach(workspace, directory.Path);
        var first = await service.WorkspacePreviewAsync(workspaceId, null, CancellationToken.None);
        Assert.False(first.IsError);
        var firstKeys = FormKeys(first);
        var cursor = first.StructuredContent!.Value.GetProperty("nextCursor").GetString();
        var second = await service.WorkspacePreviewAsync(workspaceId, cursor, CancellationToken.None);

        Assert.Equal(PluginRecordSearchPage.MaximumTake, firstKeys.Length);
        Assert.False(string.IsNullOrWhiteSpace(cursor));
        Assert.False(second.IsError);
        var secondKeys = FormKeys(second);
        Assert.Single(secondKeys);
        Assert.Empty(firstKeys.Intersect(secondKeys, StringComparer.Ordinal));
        Assert.False(second.StructuredContent!.Value.TryGetProperty("nextCursor", out var nextCursor) &&
            nextCursor.ValueKind == JsonValueKind.String &&
            !string.IsNullOrWhiteSpace(nextCursor.GetString()));
    }

    private static PluginWorkspace OpenEmptyStarfield(TemporaryDirectory directory)
    {
        var masterModKey = ModKey.FromNameAndExtension("Starfield.esm");
        WriteEmptyPlugin(Path.Combine(directory.Path, masterModKey.ToString()), masterModKey, GameRelease.Starfield);
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        return CreateFactory().Open(CreateNewRequest(directory.Path, GameRelease.Starfield, [], outputModKey));
    }

    private static (McpAuthoringService Service, string WorkspaceId) Attach(PluginWorkspace workspace, string directory)
    {
        var sessions = new McpSessionRegistry();
        var service = new McpAuthoringService([], new PluginWorkspaceFactory([]), sessions, new McpOperationReplay());
        var session = new McpAuthoringSession("review-session", GameRelease.Starfield, directory, []);
        Assert.Null(sessions.TryCreate(session));
        sessions.CompleteOutput(session, workspace);
        return (service, session.Id);
    }

    private static RecordFieldChange SetEditorId(string editorId) =>
        new("EditorID", RecordCollectionOperation.Set, RecordValue.FromString(editorId));

    private static string Cursor(
        string kind,
        string workspaceId,
        ulong revision,
        string familyId,
        string? formKey = null,
        string? containingModKey = null)
    {
        return McpCursors.Encode(new McpCursorPayload
        {
            Kind = kind,
            WorkspaceId = workspaceId,
            Revision = revision,
            FamilyId = familyId,
            FormKey = formKey,
            ContainingModKey = containingModKey,
        });
    }

    private static string CompareCursor(string workspaceId, ulong revision, string familyId, string formKey, string containing)
    {
        return McpCursors.Encode(new McpCursorPayload
        {
            Kind = McpAuthoringProjection.CompareCursor,
            WorkspaceId = workspaceId,
            Revision = revision,
            FamilyId = familyId,
            FormKey = formKey,
            ContainingModKey = containing,
            RightFormKey = formKey,
            RightContainingModKey = containing,
        });
    }

    private static string[] FormKeys(CallToolResult result)
    {
        return result.StructuredContent!.Value.GetProperty("records").EnumerateArray()
            .Select(record => record.GetProperty("formKey").GetString() ?? string.Empty)
            .ToArray();
    }

    private static string ErrorCode(CallToolResult result)
    {
        Assert.True(result.IsError);
        return result.StructuredContent?.GetProperty("code").GetString()
            ?? throw new InvalidOperationException("The structured error did not contain a code.");
    }
}
