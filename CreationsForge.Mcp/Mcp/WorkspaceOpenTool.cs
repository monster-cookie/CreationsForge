using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Mcp;

/// <summary>
/// Opens one independently owned workspace from explicit caller-supplied inputs.
/// </summary>
public sealed class WorkspaceOpenTool : McpToolBase
{
    /// <summary>The exact accepted argument names.</summary>
    private static readonly IReadOnlySet<string> AllowedArguments = new HashSet<string>(StringComparer.Ordinal)
    {
        "workspaceId",
        "game",
        "release",
        "sourcePluginPath",
        "loadOrderPluginPaths",
        "dataDirectoryPath",
        "stringDirectoryPaths",
        "recordTextLanguage",
    };

    /// <summary>The closed lower-case names accepted for localized record text.</summary>
    private static readonly string RecordTextLanguageSchema = JsonSerializer.Serialize(
        Enum.GetNames<Language>().Select(name => name.ToLowerInvariant()).ToArray());

    /// <summary>The explicit installed-discovery-free input schema.</summary>
    private static readonly JsonElement InputSchema = ParseSchema(
        "{\"type\":\"object\",\"properties\":{\"workspaceId\":{\"type\":\"string\",\"format\":\"uuid\"},\"game\":{\"type\":\"string\",\"enum\":[\"starfield\",\"fallout4\",\"skyrim\"]},\"release\":{\"type\":\"string\",\"enum\":[\"starfield\",\"fallout4\",\"skyrim_se\"]},\"sourcePluginPath\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":32767},\"loadOrderPluginPaths\":{\"type\":\"array\",\"maxItems\":4096,\"items\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":32767}},\"dataDirectoryPath\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":32767},\"stringDirectoryPaths\":{\"type\":\"array\",\"maxItems\":4096,\"items\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":32767}},\"recordTextLanguage\":{\"type\":\"string\",\"enum\":" + RecordTextLanguageSchema + "}},\"required\":[\"workspaceId\",\"game\",\"release\",\"sourcePluginPath\",\"loadOrderPluginPaths\",\"dataDirectoryPath\",\"stringDirectoryPaths\",\"recordTextLanguage\"],\"additionalProperties\":false}");

    /// <summary>The exact success and engine-error output schema.</summary>
    private static readonly JsonElement OutputSchema = McpToolSchema.Output(
        "{\"type\":\"object\",\"properties\":{\"workspaceId\":{\"type\":\"string\",\"format\":\"uuid\"},\"revision\":" + McpToolSchema.Revision + ",\"warnings\":{\"type\":\"array\",\"items\":" + McpToolSchema.Warning + "}},\"required\":[\"workspaceId\",\"revision\",\"warnings\"],\"additionalProperties\":false}");

    /// <summary>The real workspace factory supplied by host composition.</summary>
    private readonly IPluginWorkspaceFactory WorkspaceFactory;

    /// <summary>The host-owned registry that receives successful workspace ownership.</summary>
    private readonly McpWorkspaceRegistry WorkspaceRegistry;

    /// <summary>Initializes the explicit workspace-open tool.</summary>
    /// <param name="workspaceFactory">The real engine factory selected by host composition.</param>
    /// <param name="workspaceRegistry">The host-owned workspace registry.</param>
    public WorkspaceOpenTool(
        IPluginWorkspaceFactory workspaceFactory,
        McpWorkspaceRegistry workspaceRegistry)
    {
        ArgumentNullException.ThrowIfNull(workspaceFactory);
        ArgumentNullException.ThrowIfNull(workspaceRegistry);
        WorkspaceFactory = workspaceFactory;
        WorkspaceRegistry = workspaceRegistry;
    }

    /// <summary>Gets the closed workspace-open protocol descriptor.</summary>
    public override Tool ProtocolTool { get; } = new Tool
    {
        Name = "creationsforge_workspace_open",
        Title = "Open a CreationsForge workspace",
        Description = "Opens an isolated workspace from an explicit source, load order, data directory, and ordered string directories without installed-game discovery.",
        InputSchema = InputSchema,
        OutputSchema = OutputSchema,
        Annotations = new ToolAnnotations
        {
            ReadOnlyHint = false,
            IdempotentHint = false,
            DestructiveHint = false,
            OpenWorldHint = false,
        },
    };

    /// <summary>Validates the closed request, forwards it exactly once, and publishes successful registry ownership.</summary>
    /// <param name="request">The MCP request.</param>
    /// <param name="cancellationToken">The token propagated through engine acquisition.</param>
    /// <returns>The exact engine result revision and warnings.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is observed.</exception>
    public override async ValueTask<CallToolResult> InvokeAsync(
        RequestContext<CallToolRequestParams> request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!McpInput.TryGetArguments(request, AllowedArguments, out var arguments, out var error) ||
            !McpInput.TryGetRequiredGuid(arguments, "workspaceId", out var workspaceId, out error) ||
            !McpInput.TryGetGameRelease(arguments, out var game, out var release, out error) ||
            !McpInput.TryGetRequiredString(arguments, "sourcePluginPath", McpInput.MaximumPathLength, out var sourcePath, out error) ||
            !McpInput.TryGetRequiredStringArray(arguments, "loadOrderPluginPaths", 4096, out var loadOrderPaths, out error) ||
            !McpInput.TryGetRequiredString(arguments, "dataDirectoryPath", McpInput.MaximumPathLength, out var dataDirectoryPath, out error) ||
            !McpInput.TryGetRequiredStringArray(arguments, "stringDirectoryPaths", 4096, out var stringDirectoryPaths, out error) ||
            !McpInput.TryGetRequiredLanguage(arguments, "recordTextLanguage", out var recordTextLanguage, out error))
        {
            return Error("invalid_arguments", error);
        }

        var openRequest = new WorkspaceOpenRequest(
            workspaceId,
            game,
            release,
            sourcePath,
            loadOrderPaths,
            dataDirectoryPath,
            stringDirectoryPaths,
            recordTextLanguage: recordTextLanguage);
        EngineResult<WorkspaceRevision> result;
        try
        {
            result = await WorkspaceRegistry.OpenAsync(
                WorkspaceFactory,
                openRequest,
                cancellationToken).ConfigureAwait(false);
        }
        catch (ObjectDisposedException)
        {
            return Error("workspace_disposed", "The MCP workspace registry is shutting down.");
        }

        if (!result.Succeeded)
        {
            return EngineFailure(result);
        }

        if (!McpProjection.TryGetResultRevision(result, out var revision, out error))
        {
            return Error("unexpected_failure", error);
        }

        var projected = McpProjection.Json(new
        {
            workspaceId = workspaceId.ToString("D"),
            revision = McpProjection.Revision(revision),
            warnings = result.Warnings.Select(McpProjection.Warning).ToArray(),
        });
        if (!FitsStructuredResultBudget(projected))
        {
            return Error("result_too_large", "The workspace-open metadata exceeds the 64 KiB structured result budget.");
        }

        return Success(projected, $"Opened CreationsForge workspace {workspaceId:D} at revision {revision}.");
    }
}
