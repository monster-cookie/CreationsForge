using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace CreationsForge.Mcp;

/// <summary>Begins one supported non-FormList native record edit through the shared workspace transaction.</summary>
internal sealed class MajorRecordBeginEditTool : McpToolBase
{
    private static readonly IReadOnlySet<string> AllowedArguments = new HashSet<string>(StringComparer.Ordinal)
    {
        "workspaceId", "operationId", "expectedRevision", "recordType", "role", "originFormKey", "originSelection", "targetFormKey"
    };

    private static readonly JsonElement InputSchema = ParseSchema("{\"type\":\"object\",\"properties\":{\"workspaceId\":{\"type\":\"string\",\"format\":\"uuid\"},\"operationId\":{\"type\":\"string\",\"format\":\"uuid\"},\"expectedRevision\":" + McpToolSchema.Revision + ",\"recordType\":{\"type\":\"string\",\"enum\":[\"GameSettingFloat\"]},\"role\":{\"type\":\"string\",\"enum\":[\"new\",\"override\",\"existing_output\"]},\"originFormKey\":{\"type\":\"string\",\"minLength\":1},\"originSelection\":{\"type\":\"object\",\"properties\":{\"formKey\":{\"type\":\"string\"},\"scope\":{\"type\":\"string\",\"enum\":[\"source\",\"winning_overrides\",\"all_contexts\"]},\"containingModKey\":{\"type\":\"string\"}},\"required\":[\"formKey\",\"scope\"],\"additionalProperties\":false},\"targetFormKey\":{\"type\":\"string\",\"minLength\":1}},\"required\":[\"workspaceId\",\"operationId\",\"expectedRevision\",\"recordType\",\"role\"],\"additionalProperties\":false}");
    private static readonly JsonElement OutputSchema = McpToolSchema.Output("{\"type\":\"object\",\"properties\":{\"workspaceId\":{\"type\":\"string\"},\"operationId\":{\"type\":\"string\"},\"editId\":{\"type\":\"string\"},\"formKey\":{\"type\":\"string\"},\"originFormKey\":" + McpToolSchema.NullableString + ",\"recordType\":{\"type\":\"string\"},\"role\":{\"type\":\"string\"},\"revision\":" + McpToolSchema.Revision + ",\"warnings\":{\"type\":\"array\",\"items\":" + McpToolSchema.Warning + "}},\"required\":[\"workspaceId\",\"operationId\",\"editId\",\"formKey\",\"originFormKey\",\"recordType\",\"role\",\"revision\",\"warnings\"],\"additionalProperties\":false}");
    private readonly McpWorkspaceRegistry Registry;
    private readonly McpMetadataStore Store;

    internal MajorRecordBeginEditTool(McpWorkspaceRegistry registry, McpMetadataStore store)
    {
        Registry = registry ?? throw new ArgumentNullException(nameof(registry));
        Store = store ?? throw new ArgumentNullException(nameof(store));
    }

    public override Tool ProtocolTool { get; } = new()
    {
        Name = "creationsforge_major_record_begin_edit",
        Title = "Begin a native major-record edit",
        Description = "Begins a new, override, or existing-output edit for a supported non-FormList native record family.",
        InputSchema = InputSchema,
        OutputSchema = OutputSchema,
        Annotations = new ToolAnnotations { ReadOnlyHint = false, IdempotentHint = true, DestructiveHint = false, OpenWorldHint = false },
    };

    public override async ValueTask<CallToolResult> InvokeAsync(RequestContext<CallToolRequestParams> request, CancellationToken cancellationToken = default)
    {
        if (!McpInput.TryGetArguments(request, AllowedArguments, out var arguments, out var error) ||
            !McpInput.TryGetRequiredGuid(arguments, "workspaceId", out var workspaceId, out error) ||
            !McpInput.TryGetRequiredGuid(arguments, "operationId", out var operationId, out error) ||
            !McpInput.TryGetRequiredRevision(arguments, out var expectedRevision, out error) ||
            !McpInput.TryGetRequiredString(arguments, "recordType", 128, out var recordType, out error) ||
            !McpInput.TryGetRequiredString(arguments, "role", 32, out var roleText, out error))
        {
            return Error("invalid_arguments", error);
        }

        if (recordType != "GameSettingFloat")
        {
            return Error("unsupported_operation", $"Native editing is not yet available for '{recordType}'.");
        }

        BeginEditRequest begin;
        try
        {
            switch (roleText)
            {
                case "new" when !arguments.ContainsKey("originFormKey") && !arguments.ContainsKey("originSelection") && !arguments.ContainsKey("targetFormKey"):
                    begin = new BeginEditRequest(operationId, expectedRevision, FormListEditRole.New, recordType: recordType);
                    break;
                case "override" when !arguments.ContainsKey("targetFormKey") && McpInput.TryGetFormKey(arguments, "originFormKey", out var origin, out error):
                    ReferenceRequest? selection = null;
                    if (arguments.ContainsKey("originSelection"))
                    {
                        if (!McpInput.TryGetNestedReferenceRequest(arguments, "originSelection", out var parsed, out error)) return Error("invalid_arguments", error);
                        selection = parsed;
                    }

                    begin = new BeginEditRequest(operationId, expectedRevision, FormListEditRole.Override, origin, selection, recordType: recordType);
                    break;
                case "existing_output" when !arguments.ContainsKey("originFormKey") && !arguments.ContainsKey("originSelection") && McpInput.TryGetFormKey(arguments, "targetFormKey", out var target, out error):
                    begin = new BeginEditRequest(operationId, expectedRevision, FormListEditRole.ExistingOutput, targetFormKey: target, recordType: recordType);
                    break;
                default:
                    return Error("invalid_arguments", "Role-specific arguments must describe exactly one new, override, or existing_output edit shape.");
            }
        }
        catch (ArgumentException exception)
        {
            return Error("invalid_arguments", exception.Message);
        }

        var metadataCapacityExceeded = false;
        var result = await Registry.ExecuteAsync(workspaceId, async (workspace, token) =>
        {
            var state = await workspace.ReadStateAsync(token).ConfigureAwait(false);
            if (!state.Succeeded) return McpAuthoringSupport.StateFailure<EditReceipt>(state);
            if (state.Value is null) return EngineResult<EditReceipt>.Failure(new EngineError(EngineErrorCode.UnexpectedFailure, "The engine reported success without workspace state."), workspaceId: workspaceId);
            if (expectedRevision == state.Value.Revision && state.Value.OutputBaseline is not null &&
                !Store.TryPublishWorkspaceMetadata(workspaceId, state.Value.OutputBaseline, out _))
            {
                metadataCapacityExceeded = true;
                return EngineResult<EditReceipt>.Failure(new EngineError(EngineErrorCode.UnexpectedFailure, "The host could not retain the selected output baseline before staging an edit."), workspaceId: workspaceId);
            }

            return await workspace.BeginEditAsync(begin, token).ConfigureAwait(false);
        }, cancellationToken).ConfigureAwait(false);
        if (metadataCapacityExceeded) return Error("metadata_capacity_exceeded", "The host cannot retain the selected output baseline required for a later save.");
        if (!result.Succeeded) return EngineFailure(result);
        if (result.Value is null) return Error("unexpected_failure", "The engine reported success without an edit receipt.");
        var projected = McpProjection.Json(new
        {
            workspaceId = workspaceId.ToString("D"), operationId = operationId.ToString("D"), editId = result.Value.EditId.ToString("D"),
            formKey = result.Value.FormKey.ToString(), originFormKey = result.Value.OriginFormKey?.ToString(),
            recordType = result.Value.RecordType, role = McpAuthoringSupport.EditRole(result.Value.Role),
            revision = McpProjection.Revision(result.Value.Revision), warnings = result.Warnings.Select(McpProjection.Warning).ToArray(),
        });
        return FitsStructuredResultBudget(projected) ? Success(projected, $"Began {recordType} edit {result.Value.EditId:D}.") : Error("result_too_large", "The edit receipt exceeds the structured result budget.");
    }
}

/// <summary>Applies all independent native float-setting fields in one guarded transaction.</summary>
internal sealed class GameSettingFloatApplyEditTool : McpToolBase
{
    private static readonly IReadOnlySet<string> AllowedArguments = new HashSet<string>(StringComparer.Ordinal)
    {
        "workspaceId", "operationId", "expectedRevision", "editId", "editorId", "data", "majorRecordFlagsRaw", "formVersion", "version2", "versionControl", "xalg"
    };
    private static readonly JsonElement InputSchema = ParseSchema("{\"type\":\"object\",\"properties\":{\"workspaceId\":{\"type\":\"string\",\"format\":\"uuid\"},\"operationId\":{\"type\":\"string\",\"format\":\"uuid\"},\"expectedRevision\":" + McpToolSchema.Revision + ",\"editId\":{\"type\":\"string\",\"format\":\"uuid\"},\"editorId\":{\"type\":\"string\",\"minLength\":1,\"maxLength\":1024},\"data\":{\"type\":[\"number\",\"null\"]},\"majorRecordFlagsRaw\":{\"type\":\"integer\",\"minimum\":-2147483648,\"maximum\":2147483647},\"formVersion\":{\"type\":\"integer\",\"minimum\":0,\"maximum\":65535},\"version2\":{\"type\":\"integer\",\"minimum\":0,\"maximum\":65535},\"versionControl\":{\"type\":\"integer\",\"minimum\":0,\"maximum\":4294967295},\"xalg\":{\"type\":[\"integer\",\"null\"],\"minimum\":0}},\"required\":[\"workspaceId\",\"operationId\",\"expectedRevision\",\"editId\",\"editorId\",\"data\",\"majorRecordFlagsRaw\",\"formVersion\",\"version2\",\"versionControl\",\"xalg\"],\"additionalProperties\":false}");
    private static readonly JsonElement OutputSchema = McpToolSchema.Output("{\"type\":\"object\",\"properties\":{\"workspaceId\":{\"type\":\"string\"},\"operationId\":{\"type\":\"string\"},\"editId\":{\"type\":\"string\"},\"revision\":" + McpToolSchema.Revision + ",\"warnings\":{\"type\":\"array\",\"items\":" + McpToolSchema.Warning + "}},\"required\":[\"workspaceId\",\"operationId\",\"editId\",\"revision\",\"warnings\"],\"additionalProperties\":false}");
    private readonly McpWorkspaceRegistry Registry;
    private readonly McpMetadataStore Store;

    internal GameSettingFloatApplyEditTool(McpWorkspaceRegistry registry, McpMetadataStore store)
    {
        Registry = registry ?? throw new ArgumentNullException(nameof(registry));
        Store = store ?? throw new ArgumentNullException(nameof(store));
    }

    public override Tool ProtocolTool { get; } = new()
    {
        Name = "creationsforge_game_setting_float_apply_edit",
        Title = "Save a float game-setting record",
        Description = "Replaces every independent native GameSettingFloat field in one staged transaction.",
        InputSchema = InputSchema,
        OutputSchema = OutputSchema,
        Annotations = new ToolAnnotations { ReadOnlyHint = false, IdempotentHint = true, DestructiveHint = false, OpenWorldHint = false },
    };

    public override async ValueTask<CallToolResult> InvokeAsync(RequestContext<CallToolRequestParams> request, CancellationToken cancellationToken = default)
    {
        if (!McpInput.TryGetArguments(request, AllowedArguments, out var arguments, out var error) ||
            !McpInput.TryGetRequiredGuid(arguments, "workspaceId", out var workspaceId, out error) ||
            !McpInput.TryGetRequiredGuid(arguments, "operationId", out var operationId, out error) ||
            !McpInput.TryGetRequiredRevision(arguments, out var expectedRevision, out error) ||
            !McpInput.TryGetRequiredGuid(arguments, "editId", out var editId, out error) ||
            !McpInput.TryGetRequiredString(arguments, "editorId", 1024, out var editorId, out error))
        {
            return Error("invalid_arguments", error);
        }

        if (!TryReadFloat(arguments, "data", out var data) ||
            !TryReadInt32(arguments, "majorRecordFlagsRaw", out var flags) ||
            !TryReadUInt16(arguments, "formVersion", out var formVersion) ||
            !TryReadUInt16(arguments, "version2", out var version2) ||
            !TryReadUInt32(arguments, "versionControl", out var versionControl) ||
            !TryReadUInt64(arguments, "xalg", out var xalg))
        {
            return Error("invalid_arguments", "Native float-setting fields must have their complete declared number or null shapes and ranges.");
        }

        var edit = new GameSettingFloatEditRequest(operationId, expectedRevision, editId, editorId, data, flags, formVersion, version2, versionControl, xalg);
        var metadataCapacityExceeded = false;
        var result = await Registry.ExecuteAsync(workspaceId, async (workspace, token) =>
        {
            var state = await workspace.ReadStateAsync(token).ConfigureAwait(false);
            if (!state.Succeeded) return McpAuthoringSupport.StateFailure<OperationReceipt>(state);
            if (state.Value is null) return EngineResult<OperationReceipt>.Failure(new EngineError(EngineErrorCode.UnexpectedFailure, "The engine reported success without workspace state."), workspaceId: workspaceId);
            if (expectedRevision == state.Value.Revision && state.Value.OutputBaseline is not null &&
                !Store.TryPublishWorkspaceMetadata(workspaceId, state.Value.OutputBaseline, out _))
            {
                metadataCapacityExceeded = true;
                return EngineResult<OperationReceipt>.Failure(new EngineError(EngineErrorCode.UnexpectedFailure, "The host could not retain the selected output baseline before staging an edit."), workspaceId: workspaceId);
            }

            return await workspace.ApplyGameSettingFloatEditAsync(edit, token).ConfigureAwait(false);
        }, cancellationToken).ConfigureAwait(false);
        if (metadataCapacityExceeded) return Error("metadata_capacity_exceeded", "The host cannot retain the selected output baseline required for a later save.");
        if (!result.Succeeded) return EngineFailure(result);
        if (result.Value is null) return Error("unexpected_failure", "The engine reported success without an operation receipt.");
        var projected = McpProjection.Json(new
        {
            workspaceId = workspaceId.ToString("D"), operationId = operationId.ToString("D"), editId = editId.ToString("D"),
            revision = McpProjection.Revision(result.Value.Revision), warnings = result.Warnings.Select(McpProjection.Warning).ToArray(),
        });
        return FitsStructuredResultBudget(projected) ? Success(projected, $"Staged GameSettingFloat {editId:D} at revision {result.Value.Revision}.") : Error("result_too_large", "The edit receipt exceeds the structured result budget.");
    }

    private static bool TryReadFloat(IReadOnlyDictionary<string, JsonElement> values, string name, out float? value)
    {
        value = null;
        if (!values.TryGetValue(name, out var element)) return false;
        if (element.ValueKind == JsonValueKind.Null) return true;
        if (element.ValueKind != JsonValueKind.Number || !element.TryGetSingle(out var number) || !float.IsFinite(number)) return false;
        value = number;
        return true;
    }

    private static bool TryReadInt32(IReadOnlyDictionary<string, JsonElement> values, string name, out int value)
    {
        value = default;
        return values.TryGetValue(name, out var element) && element.ValueKind == JsonValueKind.Number && element.TryGetInt32(out value);
    }

    private static bool TryReadUInt16(IReadOnlyDictionary<string, JsonElement> values, string name, out ushort value)
    {
        value = default;
        return values.TryGetValue(name, out var element) && element.ValueKind == JsonValueKind.Number && element.TryGetUInt16(out value);
    }

    private static bool TryReadUInt32(IReadOnlyDictionary<string, JsonElement> values, string name, out uint value)
    {
        value = default;
        return values.TryGetValue(name, out var element) && element.ValueKind == JsonValueKind.Number && element.TryGetUInt32(out value);
    }

    private static bool TryReadUInt64(IReadOnlyDictionary<string, JsonElement> values, string name, out ulong? value)
    {
        value = null;
        if (!values.TryGetValue(name, out var element)) return false;
        if (element.ValueKind == JsonValueKind.Null) return true;
        if (element.ValueKind != JsonValueKind.Number || !element.TryGetUInt64(out var parsed)) return false;
        value = parsed;
        return true;
    }
}
