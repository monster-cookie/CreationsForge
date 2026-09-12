using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeWire;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace CreationsForge.Console.Mcp;

/// <summary>Selects or creates a native output and publishes exact immutable metadata handles.</summary>
internal sealed class OutputSelectTool : McpToolBase
{
    /// <summary>The accepted argument names.</summary>
    private static readonly IReadOnlySet<string> AllowedArguments = new HashSet<string>(StringComparer.Ordinal)
    {
        "workspaceId", "operationId", "expectedRevision", "mode", "output",
    };

    /// <summary>The closed output-selection schema.</summary>
    private static readonly JsonElement InputSchema = ParseSchema("{\"type\":\"object\",\"properties\":{\"workspaceId\":{\"type\":\"string\",\"format\":\"uuid\"},\"operationId\":{\"type\":\"string\",\"format\":\"uuid\"},\"expectedRevision\":" + McpToolSchema.Revision + ",\"mode\":{\"type\":\"string\",\"enum\":[\"create_new\",\"open_existing\"]},\"output\":{\"type\":\"object\",\"properties\":{\"pluginPath\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":32767},\"modKey\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":1024},\"localizedOutputMode\":{\"type\":\"string\",\"enum\":[\"embedded\",\"separate_string_files\"]},\"masterStyle\":{\"type\":\"string\",\"enum\":[\"full\",\"small\",\"medium\"]}},\"required\":[\"pluginPath\",\"modKey\",\"localizedOutputMode\",\"masterStyle\"],\"additionalProperties\":false}},\"required\":[\"workspaceId\",\"operationId\",\"expectedRevision\",\"mode\",\"output\"],\"additionalProperties\":false}");

    /// <summary>The closed selection result schema.</summary>
    private static readonly JsonElement OutputSchema = McpToolSchema.Output("{\"type\":\"object\",\"properties\":{\"workspaceId\":{\"type\":\"string\",\"format\":\"uuid\"},\"operationId\":{\"type\":\"string\",\"format\":\"uuid\"},\"revision\":" + McpToolSchema.Revision + ",\"outputReference\":{\"oneOf\":[" + McpToolSchema.MetadataReference + ",{\"type\":\"null\"}]},\"baselineReference\":{\"oneOf\":[" + McpToolSchema.MetadataReference + ",{\"type\":\"null\"}]},\"detailsUnavailable\":{\"type\":\"boolean\"},\"warnings\":{\"type\":\"array\",\"items\":" + McpToolSchema.Warning + "}},\"required\":[\"workspaceId\",\"operationId\",\"revision\",\"outputReference\",\"baselineReference\",\"detailsUnavailable\",\"warnings\"],\"additionalProperties\":false}");

    /// <summary>The workspace registry.</summary>
    private readonly McpWorkspaceRegistry Registry;

    /// <summary>The host metadata store.</summary>
    private readonly McpMetadataStore Store;

    /// <summary>Initializes the output-selection tool.</summary>
    /// <param name="registry">The host workspace registry.</param>
    /// <param name="store">The host metadata store.</param>
    internal OutputSelectTool(McpWorkspaceRegistry registry, McpMetadataStore store)
    {
        Registry = registry ?? throw new ArgumentNullException(nameof(registry));
        Store = store ?? throw new ArgumentNullException(nameof(store));
    }

    /// <summary>Gets the output-selection protocol descriptor.</summary>
    public override Tool ProtocolTool { get; } = new()
    {
        Name = "creationsforge_output_select",
        Title = "Select a native output",
        Description = "Creates a new output or opens an existing output and returns opaque handles for its exact association and complete artifact baseline.",
        InputSchema = InputSchema,
        OutputSchema = OutputSchema,
        Annotations = new ToolAnnotations { ReadOnlyHint = false, IdempotentHint = true, DestructiveHint = false, OpenWorldHint = false },
    };

    /// <summary>Validates, admits, executes, and publishes one exact output selection.</summary>
    /// <param name="request">The MCP request.</param>
    /// <param name="cancellationToken">The propagated cancellation token.</param>
    /// <returns>The selection receipt and metadata handles.</returns>
    public override async ValueTask<CallToolResult> InvokeAsync(RequestContext<CallToolRequestParams> request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!McpInput.TryGetArguments(request, AllowedArguments, out var arguments, out var error) ||
            !McpInput.TryGetRequiredGuid(arguments, "workspaceId", out var workspaceId, out error) ||
            !McpInput.TryGetRequiredGuid(arguments, "operationId", out var operationId, out error) ||
            !McpInput.TryGetRequiredRevision(arguments, out var expectedRevision, out error) ||
            !McpInput.TryGetRequiredString(arguments, "mode", 16, out var modeText, out error) ||
            !McpAuthoringSupport.TryGetOutputAssociation(arguments, out var association, out error))
        {
            return Error("invalid_arguments", error);
        }

        var mode = modeText switch { "create_new" => OutputSelectionMode.CreateNew, "open_existing" => OutputSelectionMode.OpenExisting, _ => (OutputSelectionMode)(-1) };
        if (!Enum.IsDefined(mode))
        {
            return Error("invalid_arguments", "Argument 'mode' must be create_new or open_existing.");
        }

        await using var admission = await Store.AcquireOperationAsync(workspaceId, operationId, McpMetadataOperationKind.WorkspaceMutation, 2, cancellationToken).ConfigureAwait(false);
        if (admission is null)
        {
            return Error("metadata_capacity_exceeded", "The host cannot reserve metadata capacity for this operation.");
        }

        var result = await Registry.ExecuteAsync(workspaceId, (workspace, token) => workspace.SelectOutputAsync(new SelectOutputRequest(operationId, expectedRevision, mode, association), token), cancellationToken).ConfigureAwait(false);
        if (!result.Succeeded)
        {
            return EngineFailure(result);
        }

        if (result.Value is null)
        {
            return Error("unexpected_failure", "The engine reported success without an output-selection receipt.");
        }

        var outputReference = admission.Publish(result.Value.Output);
        var baselineReference = admission.Publish(result.Value.Baseline);
        var projected = McpProjection.Json(new
        {
            workspaceId = workspaceId.ToString("D"),
            operationId = operationId.ToString("D"),
            revision = McpProjection.Revision(result.Value.Revision),
            outputReference = outputReference is null ? null : McpAuthoringSupport.MetadataReference(outputReference),
            baselineReference = baselineReference is null ? null : McpAuthoringSupport.MetadataReference(baselineReference),
            detailsUnavailable = outputReference is null || baselineReference is null,
            warnings = result.Warnings.Select(McpProjection.Warning).ToArray(),
        });
        return FitsStructuredResultBudget(projected)
            ? Success(projected, $"Selected output for workspace {workspaceId:D} at revision {result.Value.Revision}.")
            : Error("result_too_large", "The output-selection receipt exceeds the 64 KiB structured result budget.");
    }
}

/// <summary>Begins a new, override, or existing-output FormList edit.</summary>
internal sealed class FormListBeginEditTool : McpToolBase
{
    /// <summary>The accepted argument names.</summary>
    private static readonly IReadOnlySet<string> AllowedArguments = new HashSet<string>(StringComparer.Ordinal)
    {
        "workspaceId", "operationId", "expectedRevision", "role", "originFormKey", "originSelection", "targetFormKey",
    };

    /// <summary>The closed begin-edit schema.</summary>
    private static readonly JsonElement InputSchema = CreateInputSchema();

    /// <summary>The closed begin-edit result schema.</summary>
    private static readonly JsonElement OutputSchema = McpToolSchema.Output("{\"type\":\"object\",\"properties\":{\"workspaceId\":{\"type\":\"string\"},\"operationId\":{\"type\":\"string\"},\"editId\":{\"type\":\"string\"},\"formKey\":{\"type\":\"string\"},\"originFormKey\":" + McpToolSchema.NullableString + ",\"role\":{\"type\":\"string\"},\"revision\":" + McpToolSchema.Revision + ",\"warnings\":{\"type\":\"array\",\"items\":" + McpToolSchema.Warning + "}},\"required\":[\"workspaceId\",\"operationId\",\"editId\",\"formKey\",\"originFormKey\",\"role\",\"revision\",\"warnings\"],\"additionalProperties\":false}");

    /// <summary>The registry.</summary>
    private readonly McpWorkspaceRegistry Registry;

    /// <summary>The metadata admission store.</summary>
    private readonly McpMetadataStore Store;

    /// <summary>Initializes the begin-edit tool.</summary>
    /// <param name="registry">The workspace registry.</param>
    /// <param name="store">The metadata admission store.</param>
    internal FormListBeginEditTool(McpWorkspaceRegistry registry, McpMetadataStore store)
    {
        Registry = registry ?? throw new ArgumentNullException(nameof(registry));
        Store = store ?? throw new ArgumentNullException(nameof(store));
    }

    /// <summary>Creates three fully closed role-specific alternatives so discovery prevents ambiguous edit shapes.</summary>
    /// <returns>The detached begin-edit input schema.</returns>
    private static JsonElement CreateInputSchema()
    {
        const string identityProperties = "\"workspaceId\":{\"type\":\"string\",\"format\":\"uuid\"},\"operationId\":{\"type\":\"string\",\"format\":\"uuid\"}";
        const string identityRequired = "\"workspaceId\",\"operationId\",\"expectedRevision\",\"role\"";
        const string originSelection = "{\"type\":\"object\",\"properties\":{\"formKey\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":1024},\"scope\":{\"type\":\"string\",\"enum\":[\"winning_overrides\",\"all_contexts\",\"source\"]},\"containingModKey\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":1024}},\"required\":[\"formKey\",\"scope\"],\"additionalProperties\":false}";
        var common = identityProperties + ",\"expectedRevision\":" + McpToolSchema.Revision;
        return ParseSchema("{\"type\":\"object\",\"properties\":{" + common +
            ",\"role\":{\"type\":\"string\",\"enum\":[\"new\",\"override\",\"existing_output\"]}," +
            "\"originFormKey\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":1024},\"originSelection\":" + originSelection + "," +
            "\"targetFormKey\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":1024}}," +
            "\"required\":[" + identityRequired + "],\"additionalProperties\":false,\"oneOf\":[" +
            "{\"properties\":{\"role\":{\"const\":\"new\"}},\"not\":{\"anyOf\":[{\"required\":[\"originFormKey\"]},{\"required\":[\"originSelection\"]},{\"required\":[\"targetFormKey\"]}]}}," +
            "{\"properties\":{\"role\":{\"const\":\"override\"}},\"required\":[\"originFormKey\"],\"not\":{\"required\":[\"targetFormKey\"]}}," +
            "{\"properties\":{\"role\":{\"const\":\"existing_output\"}},\"required\":[\"targetFormKey\"],\"not\":{\"anyOf\":[{\"required\":[\"originFormKey\"]},{\"required\":[\"originSelection\"]}]}}" +
            "]}");
    }

    /// <summary>Gets the begin-edit descriptor.</summary>
    public override Tool ProtocolTool { get; } = new()
    {
        Name = "creationsforge_formlist_begin_edit", Title = "Begin a FormList edit", Description = "Allocates a new FormList, stages an override, or selects an existing output FormList for typed edits.",
        InputSchema = InputSchema, OutputSchema = OutputSchema,
        Annotations = new ToolAnnotations { ReadOnlyHint = false, IdempotentHint = true, DestructiveHint = false, OpenWorldHint = false },
    };

    /// <summary>Validates and begins one exact edit session.</summary>
    /// <param name="request">The MCP request.</param>
    /// <param name="cancellationToken">The propagated token.</param>
    /// <returns>The staged edit receipt.</returns>
    public override async ValueTask<CallToolResult> InvokeAsync(RequestContext<CallToolRequestParams> request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!McpInput.TryGetArguments(request, AllowedArguments, out var arguments, out var error) ||
            !McpInput.TryGetRequiredGuid(arguments, "workspaceId", out var workspaceId, out error) ||
            !McpInput.TryGetRequiredGuid(arguments, "operationId", out var operationId, out error) ||
            !McpInput.TryGetRequiredRevision(arguments, out var expectedRevision, out error) ||
            !McpInput.TryGetRequiredString(arguments, "role", 32, out var roleText, out error))
        {
            return Error("invalid_arguments", error);
        }

        if (!TryCreateRequest(arguments, operationId, expectedRevision, roleText, out var beginRequest, out error))
        {
            return Error("invalid_arguments", error);
        }

        var metadataCapacityExceeded = false;
        var result = await Registry.ExecuteAsync(
            workspaceId,
            async (workspace, token) =>
            {
                var state = await workspace.ReadStateAsync(token).ConfigureAwait(false);
                if (!state.Succeeded) return McpAuthoringSupport.StateFailure<EditReceipt>(state);
                if (state.Value is null) return EngineResult<EditReceipt>.Failure(new EngineError(EngineErrorCode.UnexpectedFailure, "The engine reported success without workspace state."), workspaceId: workspaceId);
                if (expectedRevision == state.Value.Revision &&
                    state.Value.OutputBaseline is not null &&
                    !Store.TryPublishWorkspaceMetadata(workspaceId, state.Value.OutputBaseline, out _))
                {
                    metadataCapacityExceeded = true;
                    return EngineResult<EditReceipt>.Failure(new EngineError(EngineErrorCode.UnexpectedFailure, "The host could not retain the selected output baseline before staging an edit."), workspaceId: workspaceId);
                }

                return await workspace.BeginEditAsync(beginRequest, token).ConfigureAwait(false);
            },
            cancellationToken).ConfigureAwait(false);
        if (metadataCapacityExceeded) return Error("metadata_capacity_exceeded", "The host cannot retain the selected output baseline required for a later save.");
        if (!result.Succeeded) return EngineFailure(result);
        if (result.Value is null) return Error("unexpected_failure", "The engine reported success without an edit receipt.");
        var projected = McpProjection.Json(new
        {
            workspaceId = workspaceId.ToString("D"), operationId = operationId.ToString("D"), editId = result.Value.EditId.ToString("D"),
            formKey = result.Value.FormKey.ToString(), originFormKey = result.Value.OriginFormKey?.ToString(), role = McpAuthoringSupport.EditRole(result.Value.Role),
            revision = McpProjection.Revision(result.Value.Revision), warnings = result.Warnings.Select(McpProjection.Warning).ToArray(),
        });
        return FitsStructuredResultBudget(projected) ? Success(projected, $"Began FormList edit {result.Value.EditId:D}.") : Error("result_too_large", "The edit receipt exceeds the 64 KiB structured result budget.");
    }

    /// <summary>Creates the role-specific Core request.</summary>
    /// <param name="arguments">The validated closed MCP arguments.</param>
    /// <param name="operationId">The idempotent operation identity.</param>
    /// <param name="revision">The exact expected revision.</param>
    /// <param name="roleText">The stable protocol role.</param>
    /// <param name="request">Receives the complete Core request.</param>
    /// <param name="error">Receives a role-shape or Core validation error.</param>
    /// <returns><see langword="true"/> when exactly one role shape is valid.</returns>
    private static bool TryCreateRequest(IReadOnlyDictionary<string, JsonElement> arguments, Guid operationId, WorkspaceRevision revision, string roleText, out BeginEditRequest request, out string error)
    {
        request = null!;
        try
        {
            switch (roleText)
            {
                case "new" when !arguments.ContainsKey("originFormKey") && !arguments.ContainsKey("originSelection") && !arguments.ContainsKey("targetFormKey"):
                    request = new BeginEditRequest(operationId, revision, FormListEditRole.New);
                    break;
                case "override" when !arguments.ContainsKey("targetFormKey") && McpInput.TryGetFormKey(arguments, "originFormKey", out var origin, out error):
                    ReferenceRequest? selection = null;
                    if (arguments.ContainsKey("originSelection"))
                    {
                        if (!McpInput.TryGetNestedReferenceRequest(arguments, "originSelection", out var parsedSelection, out error)) return false;
                        selection = parsedSelection;
                    }
                    request = new BeginEditRequest(operationId, revision, FormListEditRole.Override, origin, selection);
                    break;
                case "existing_output" when !arguments.ContainsKey("originFormKey") && !arguments.ContainsKey("originSelection") && McpInput.TryGetFormKey(arguments, "targetFormKey", out var target, out error):
                    request = new BeginEditRequest(operationId, revision, FormListEditRole.ExistingOutput, targetFormKey: target);
                    break;
                default:
                    error = "Role-specific arguments must describe exactly one new, override, or existing_output edit shape.";
                    return false;
            }

            error = string.Empty;
            return true;
        }
        catch (ArgumentException exception)
        {
            error = exception.Message;
            return false;
        }
    }
}

/// <summary>Decodes and applies one named game-specific typed FormList edit.</summary>
internal sealed class FormListApplyEditTool : McpToolBase
{
    /// <summary>The accepted argument names.</summary>
    private static readonly IReadOnlySet<string> AllowedArguments = new HashSet<string>(StringComparer.Ordinal) { "workspaceId", "operationId", "expectedRevision", "editId", "commandName", "argumentsJson" };

    /// <summary>The closed typed-edit schema.</summary>
    private static readonly JsonElement InputSchema = ParseSchema("{\"type\":\"object\",\"properties\":{\"workspaceId\":{\"type\":\"string\",\"format\":\"uuid\"},\"operationId\":{\"type\":\"string\",\"format\":\"uuid\"},\"expectedRevision\":" + McpToolSchema.Revision + ",\"editId\":{\"type\":\"string\",\"format\":\"uuid\"},\"commandName\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":1024},\"argumentsJson\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":16777216}},\"required\":[\"workspaceId\",\"operationId\",\"expectedRevision\",\"editId\",\"commandName\",\"argumentsJson\"],\"additionalProperties\":false}");

    /// <summary>The closed operation receipt schema.</summary>
    private static readonly JsonElement OutputSchema = McpToolSchema.Output("{\"type\":\"object\",\"properties\":{\"workspaceId\":{\"type\":\"string\"},\"operationId\":{\"type\":\"string\"},\"editId\":{\"type\":\"string\"},\"commandName\":{\"type\":\"string\"},\"revision\":" + McpToolSchema.Revision + ",\"warnings\":{\"type\":\"array\",\"items\":" + McpToolSchema.Warning + "}},\"required\":[\"workspaceId\",\"operationId\",\"editId\",\"commandName\",\"revision\",\"warnings\"],\"additionalProperties\":false}");

    /// <summary>The registry.</summary>
    private readonly McpWorkspaceRegistry Registry;

    /// <summary>The exact per-game codecs.</summary>
    private readonly IReadOnlyList<IFormListEditWireCodec> Codecs;

    /// <summary>The operation admission store.</summary>
    private readonly McpMetadataStore Store;

    /// <summary>Initializes the typed-edit tool.</summary>
    /// <param name="registry">The workspace registry.</param>
    /// <param name="codecs">The exact per-game codecs.</param>
    /// <param name="store">The operation admission store.</param>
    internal FormListApplyEditTool(McpWorkspaceRegistry registry, IReadOnlyList<IFormListEditWireCodec> codecs, McpMetadataStore store)
    {
        Registry = registry ?? throw new ArgumentNullException(nameof(registry));
        Codecs = codecs ?? throw new ArgumentNullException(nameof(codecs));
        Store = store ?? throw new ArgumentNullException(nameof(store));
    }

    /// <summary>Gets the typed-edit descriptor.</summary>
    public override Tool ProtocolTool { get; } = new()
    {
        Name = "creationsforge_formlist_apply_edit", Title = "Apply a typed FormList edit", Description = "Decodes one schema-discovered closed command payload into a native typed edit and applies it to a staged FormList.",
        InputSchema = InputSchema, OutputSchema = OutputSchema,
        Annotations = new ToolAnnotations { ReadOnlyHint = false, IdempotentHint = true, DestructiveHint = false, OpenWorldHint = false },
    };

    /// <summary>Rejects malformed wire data before admitting or invoking the Core operation identifier.</summary>
    /// <param name="request">The MCP request.</param>
    /// <param name="cancellationToken">The propagated token.</param>
    /// <returns>The exact operation receipt.</returns>
    public override async ValueTask<CallToolResult> InvokeAsync(RequestContext<CallToolRequestParams> request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!McpInput.TryGetArguments(request, AllowedArguments, out var arguments, out var error) ||
            !McpInput.TryGetRequiredGuid(arguments, "workspaceId", out var workspaceId, out error) ||
            !McpInput.TryGetRequiredGuid(arguments, "operationId", out var operationId, out error) ||
            !McpInput.TryGetRequiredRevision(arguments, out var expectedRevision, out error) ||
            !McpInput.TryGetRequiredGuid(arguments, "editId", out var editId, out error) ||
            !McpInput.TryGetRequiredString(arguments, "commandName", 1024, out var commandName, out error) ||
            !McpInput.TryGetRequiredString(arguments, "argumentsJson", McpAuthoringSupport.MaximumArgumentsJsonBytes, out var argumentsJson, out error) ||
            !McpAuthoringSupport.TryParseArgumentsJson(argumentsJson, out var commandArguments, out error))
        {
            return Error("invalid_arguments", error);
        }

        var stateResult = await Registry.ExecuteAsync(workspaceId, (workspace, token) => workspace.ReadStateAsync(token), cancellationToken).ConfigureAwait(false);
        if (!stateResult.Succeeded) return EngineFailure(stateResult);
        if (stateResult.Value is null) return Error("unexpected_failure", "The engine reported success without workspace state.");
        var codec = Codecs.SingleOrDefault(candidate => candidate.Game == stateResult.Value.Game && candidate.Release == stateResult.Value.Release);
        if (codec is null) return Error("unsupported_game_release", "No typed FormList edit codec is available for the workspace game and release.");

        var decoded = codec.Decode(commandName, commandArguments, NativeWireReadLimits.Default, cancellationToken);
        if (!decoded.Succeeded || decoded.Value is null)
        {
            var decodeError = decoded.Error ?? new EngineError(EngineErrorCode.InvalidRequest, "The native wire codec rejected the command without an error.");
            return Error(McpProjection.ErrorCode(decodeError.Code), decodeError.Message);
        }

        var editRequest = new FormListEditRequest(operationId, expectedRevision, editId, decoded.Value);
        var metadataCapacityExceeded = false;
        var result = await Registry.ExecuteAsync(
            workspaceId,
            async (workspace, token) =>
            {
                var state = await workspace.ReadStateAsync(token).ConfigureAwait(false);
                if (!state.Succeeded) return McpAuthoringSupport.StateFailure<OperationReceipt>(state);
                if (state.Value is null) return EngineResult<OperationReceipt>.Failure(new EngineError(EngineErrorCode.UnexpectedFailure, "The engine reported success without workspace state."), workspaceId: workspaceId);
                if (expectedRevision == state.Value.Revision &&
                    state.Value.OutputBaseline is not null &&
                    !Store.TryPublishWorkspaceMetadata(workspaceId, state.Value.OutputBaseline, out _))
                {
                    metadataCapacityExceeded = true;
                    return EngineResult<OperationReceipt>.Failure(new EngineError(EngineErrorCode.UnexpectedFailure, "The host could not retain the selected output baseline before staging an edit."), workspaceId: workspaceId);
                }

                return await workspace.ApplyFormListEditAsync(editRequest, token).ConfigureAwait(false);
            },
            cancellationToken).ConfigureAwait(false);
        if (metadataCapacityExceeded) return Error("metadata_capacity_exceeded", "The host cannot retain the selected output baseline required for a later save.");
        if (!result.Succeeded) return EngineFailure(result);
        if (result.Value is null) return Error("unexpected_failure", "The engine reported success without an operation receipt.");
        var projected = McpProjection.Json(new
        {
            workspaceId = workspaceId.ToString("D"), operationId = result.Value.OperationId.ToString("D"), editId = editId.ToString("D"), commandName,
            revision = McpProjection.Revision(result.Value.Revision), warnings = result.Warnings.Select(McpProjection.Warning).ToArray(),
        });
        return FitsStructuredResultBudget(projected) ? Success(projected, $"Applied {commandName} at revision {result.Value.Revision}.") : Error("result_too_large", "The operation receipt exceeds the 64 KiB structured result budget.");
    }
}

/// <summary>Reopens or discards staged output state against an exact retained baseline.</summary>
internal sealed class OutputResetTool : McpToolBase
{
    /// <summary>The accepted argument names.</summary>
    private static readonly IReadOnlySet<string> AllowedArguments = new HashSet<string>(StringComparer.Ordinal) { "workspaceId", "operationId", "expectedRevision", "baselineHandle" };

    /// <summary>The closed reset schema.</summary>
    private static readonly JsonElement InputSchema = ParseSchema("{\"type\":\"object\",\"properties\":{\"workspaceId\":{\"type\":\"string\",\"format\":\"uuid\"},\"operationId\":{\"type\":\"string\",\"format\":\"uuid\"},\"expectedRevision\":" + McpToolSchema.Revision + ",\"baselineHandle\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":128}},\"required\":[\"workspaceId\",\"operationId\",\"expectedRevision\",\"baselineHandle\"],\"additionalProperties\":false}");

    /// <summary>The closed reset result schema.</summary>
    private static readonly JsonElement OutputSchema = McpToolSchema.Output("{\"type\":\"object\",\"properties\":{\"workspaceId\":{\"type\":\"string\"},\"operationId\":{\"type\":\"string\"},\"revision\":" + McpToolSchema.Revision + ",\"outputReference\":{\"oneOf\":[" + McpToolSchema.MetadataReference + ",{\"type\":\"null\"}]},\"baselineReference\":{\"oneOf\":[" + McpToolSchema.MetadataReference + ",{\"type\":\"null\"}]},\"detailsUnavailable\":{\"type\":\"boolean\"},\"warnings\":{\"type\":\"array\",\"items\":" + McpToolSchema.Warning + "}},\"required\":[\"workspaceId\",\"operationId\",\"revision\",\"outputReference\",\"baselineReference\",\"detailsUnavailable\",\"warnings\"],\"additionalProperties\":false}");

    /// <summary>The registry.</summary>
    private readonly McpWorkspaceRegistry Registry;

    /// <summary>The metadata store.</summary>
    private readonly McpMetadataStore Store;

    /// <summary>Whether the tool performs a reopen rather than discard.</summary>
    private readonly bool Reopen;

    /// <summary>Initializes one reset mode.</summary>
    /// <param name="registry">The registry.</param>
    /// <param name="store">The metadata store.</param>
    /// <param name="reopen">Whether to reopen and publish refreshed metadata.</param>
    internal OutputResetTool(McpWorkspaceRegistry registry, McpMetadataStore store, bool reopen)
    {
        Registry = registry ?? throw new ArgumentNullException(nameof(registry));
        Store = store ?? throw new ArgumentNullException(nameof(store));
        Reopen = reopen;
        ProtocolTool = new Tool
        {
            Name = reopen ? "creationsforge_output_reopen" : "creationsforge_workspace_discard",
            Title = reopen ? "Reopen the selected output" : "Discard staged changes",
            Description = reopen ? "Reopens selected output state from an exact retained artifact baseline." : "Discards staged changes by reopening the exact retained output baseline.",
            InputSchema = InputSchema, OutputSchema = OutputSchema,
            Annotations = new ToolAnnotations { ReadOnlyHint = false, IdempotentHint = true, DestructiveHint = true, OpenWorldHint = false },
        };
    }

    /// <summary>Gets the reopen or discard descriptor.</summary>
    public override Tool ProtocolTool { get; }

    /// <summary>Resolves the exact baseline before operation admission and engine invocation.</summary>
    /// <param name="request">The MCP request.</param>
    /// <param name="cancellationToken">The propagated token.</param>
    /// <returns>The reset receipt and optional refreshed handles.</returns>
    public override async ValueTask<CallToolResult> InvokeAsync(RequestContext<CallToolRequestParams> request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!McpInput.TryGetArguments(request, AllowedArguments, out var arguments, out var error) ||
            !McpInput.TryGetRequiredGuid(arguments, "workspaceId", out var workspaceId, out error) ||
            !McpInput.TryGetRequiredGuid(arguments, "operationId", out var operationId, out error) ||
            !McpInput.TryGetRequiredRevision(arguments, out var expectedRevision, out error) ||
            !McpInput.TryGetRequiredString(arguments, "baselineHandle", 128, out var baselineHandle, out error))
        {
            return Error("invalid_arguments", error);
        }

        if (!Store.TryResolveOutputBaseline(baselineHandle, workspaceId, out var baseline))
        {
            return Error("invalid_metadata_handle", "The baseline handle is unknown, has the wrong kind, or belongs to another workspace.");
        }

        if (Reopen)
        {
            await using var admission = await Store.AcquireOperationAsync(workspaceId, operationId, McpMetadataOperationKind.WorkspaceMutation, 2, cancellationToken).ConfigureAwait(false);
            if (admission is null) return Error("metadata_capacity_exceeded", "The host cannot reserve metadata capacity for this operation.");
            var result = await Registry.ExecuteAsync(workspaceId, (workspace, token) => workspace.ReopenOutputAsync(new ReopenOutputRequest(operationId, expectedRevision, baseline), token), cancellationToken).ConfigureAwait(false);
            if (!result.Succeeded) return EngineFailure(result);
            if (result.Value is null) return Error("unexpected_failure", "The engine reported success without an output-reopen receipt.");
            var outputReference = admission.Publish(result.Value.Output);
            var baselineReference = admission.Publish(result.Value.Baseline);
            return CreateResult(
                workspaceId,
                operationId,
                result.Value.Revision,
                outputReference is null ? null : McpAuthoringSupport.MetadataReference(outputReference),
                baselineReference is null ? null : McpAuthoringSupport.MetadataReference(baselineReference),
                outputReference is null || baselineReference is null,
                result.Warnings);
        }

        var discard = await Registry.ExecuteAsync(workspaceId, (workspace, token) => workspace.DiscardChangesAsync(new DiscardChangesRequest(operationId, expectedRevision, baseline), token), cancellationToken).ConfigureAwait(false);
        if (!discard.Succeeded) return EngineFailure(discard);
        if (discard.Value is null) return Error("unexpected_failure", "The engine reported success without a discard receipt.");
        return CreateResult(workspaceId, discard.Value.OperationId, discard.Value.Revision, null, null, false, discard.Warnings);
    }

    /// <summary>Creates one bounded reset projection.</summary>
    /// <param name="workspaceId">The owning workspace.</param>
    /// <param name="operationId">The completed operation identity.</param>
    /// <param name="revision">The resulting revision.</param>
    /// <param name="outputReference">The refreshed output handle projection, or <see langword="null"/> for discard.</param>
    /// <param name="baselineReference">The refreshed baseline handle projection, or <see langword="null"/> for discard.</param>
    /// <param name="detailsUnavailable">Whether an improbable post-mutation publication defect withheld one or more handles.</param>
    /// <param name="warnings">The exact engine warnings.</param>
    /// <returns>The bounded structured result.</returns>
    private CallToolResult CreateResult(Guid workspaceId, Guid operationId, WorkspaceRevision revision, object? outputReference, object? baselineReference, bool detailsUnavailable, IReadOnlyList<EngineWarning> warnings)
    {
        var projected = McpProjection.Json(new { workspaceId = workspaceId.ToString("D"), operationId = operationId.ToString("D"), revision = McpProjection.Revision(revision), outputReference, baselineReference, detailsUnavailable, warnings = warnings.Select(McpProjection.Warning).ToArray() });
        return FitsStructuredResultBudget(projected) ? Success(projected, $"{(Reopen ? "Reopened output" : "Discarded staged changes")} at revision {revision}.") : Error("result_too_large", "The reset receipt exceeds the 64 KiB structured result budget.");
    }
}
