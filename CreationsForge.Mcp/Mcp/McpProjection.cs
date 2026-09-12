using System.Globalization;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Mcp;

/// <summary>
/// Projects native engine contracts into stable MCP JSON without changing their identity or order.
/// </summary>
internal static class McpProjection
{
    /// <summary>Projects the exact engine revision while keeping its unsigned sequence out of JSON number space.</summary>
    /// <param name="revision">The exact result revision.</param>
    /// <returns>A closed revision object with a decimal-string sequence.</returns>
    internal static object Revision(WorkspaceRevision revision)
    {
        return new
        {
            baselineId = revision.BaselineId.ToString("D"),
            sequence = revision.Sequence.ToString(CultureInfo.InvariantCulture),
        };
    }

    /// <summary>Projects one exact contextual selection and its native resolution status.</summary>
    /// <param name="context">The engine-owned context result.</param>
    /// <returns>A closed context object that preserves absence as JSON null.</returns>
    internal static object Context(FormListContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return new
        {
            formKey = context.Selection.FormKey.ToString(),
            scope = Scope(context.Selection.Scope),
            requestedContainingModKey = context.Selection.ContainingModKey?.ToString(),
            status = ResolutionStatus(context.Status),
            containingModKey = context.ContainingModKey?.ToString(),
            sourcePath = context.Path,
            loadOrderIndex = context.LoadOrderIndex,
            role = context.Role.HasValue ? PluginRole(context.Role.Value) : null,
        };
    }

    /// <summary>Projects one engine warning without rewriting its stable code or message.</summary>
    /// <param name="warning">The warning to project.</param>
    /// <returns>A closed warning object.</returns>
    internal static object Warning(EngineWarning warning)
    {
        ArgumentNullException.ThrowIfNull(warning);
        return new
        {
            code = warning.Code,
            message = warning.Message,
        };
    }

    /// <summary>Projects one semantic descriptor without interpreting its typed field identifier as a JSON Pointer.</summary>
    /// <param name="change">The ordered engine descriptor.</param>
    /// <returns>A closed descriptor object with unchanged positions.</returns>
    internal static object Change(SemanticChangeDescriptor change)
    {
        ArgumentNullException.ThrowIfNull(change);
        return new
        {
            fieldIdentifier = change.FieldIdentifier,
            kind = SemanticChangeKind(change.Kind),
            beforePosition = change.BeforePosition,
            afterPosition = change.AfterPosition,
        };
    }

    /// <summary>Returns the stable lower-snake-case name for one record scope.</summary>
    /// <param name="scope">The scope to map.</param>
    /// <returns>The MCP scope name.</returns>
    internal static string Scope(RecordScope scope)
    {
        return scope switch
        {
            RecordScope.WinningOverrides => "winning_overrides",
            RecordScope.AllContexts => "all_contexts",
            RecordScope.Source => "source",
            RecordScope.StagedOutput => "staged_output",
            _ => throw new ArgumentOutOfRangeException(nameof(scope)),
        };
    }

    /// <summary>Returns the stable lower-snake-case name for one plugin role.</summary>
    /// <param name="role">The role to map.</param>
    /// <returns>The MCP role name.</returns>
    internal static string PluginRole(CreationsForge.Core.Engine.Contracts.PluginRole role)
    {
        return role switch
        {
            CreationsForge.Core.Engine.Contracts.PluginRole.Source => "source",
            CreationsForge.Core.Engine.Contracts.PluginRole.LoadOrder => "load_order",
            CreationsForge.Core.Engine.Contracts.PluginRole.Output => "output",
            _ => throw new ArgumentOutOfRangeException(nameof(role)),
        };
    }

    /// <summary>Returns the stable lower-snake-case name for one resolution status.</summary>
    /// <param name="status">The status to map.</param>
    /// <returns>The MCP status name.</returns>
    internal static string ResolutionStatus(ReferenceResolutionStatus status)
    {
        return status switch
        {
            ReferenceResolutionStatus.Resolved => "resolved",
            ReferenceResolutionStatus.Unresolved => "unresolved",
            ReferenceResolutionStatus.Unsupported => "unsupported",
            ReferenceResolutionStatus.Deleted => "deleted",
            ReferenceResolutionStatus.Ambiguous => "ambiguous",
            ReferenceResolutionStatus.UnknownFamily => "unknown_family",
            _ => throw new ArgumentOutOfRangeException(nameof(status)),
        };
    }

    /// <summary>Returns the stable lower-snake-case name for one semantic change kind.</summary>
    /// <param name="kind">The semantic change kind to map.</param>
    /// <returns>The MCP change-kind name.</returns>
    internal static string SemanticChangeKind(CreationsForge.Core.Engine.Contracts.SemanticChangeKind kind)
    {
        return kind switch
        {
            CreationsForge.Core.Engine.Contracts.SemanticChangeKind.ValueChanged => "value_changed",
            CreationsForge.Core.Engine.Contracts.SemanticChangeKind.ItemInserted => "item_inserted",
            CreationsForge.Core.Engine.Contracts.SemanticChangeKind.ItemRemoved => "item_removed",
            CreationsForge.Core.Engine.Contracts.SemanticChangeKind.ItemChanged => "item_changed",
            CreationsForge.Core.Engine.Contracts.SemanticChangeKind.WriterNormalized => "writer_normalized",
            _ => throw new ArgumentOutOfRangeException(nameof(kind)),
        };
    }

    /// <summary>Returns the explicit lower-snake-case mapping for one engine failure category.</summary>
    /// <param name="code">The engine error code.</param>
    /// <returns>The stable MCP code.</returns>
    internal static string ErrorCode(EngineErrorCode code)
    {
        return code switch
        {
            EngineErrorCode.InvalidRequest => "invalid_request",
            EngineErrorCode.UnsupportedGameRelease => "unsupported_game_release",
            EngineErrorCode.SourceOpenFailed => "source_open_failed",
            EngineErrorCode.MissingMaster => "missing_master",
            EngineErrorCode.UnsupportedInput => "unsupported_input",
            EngineErrorCode.OutputOpenFailed => "output_open_failed",
            EngineErrorCode.WorkspaceDisposed => "workspace_disposed",
            EngineErrorCode.RevisionConflict => "revision_conflict",
            EngineErrorCode.OperationIdReuse => "operation_id_reuse",
            EngineErrorCode.OutputNotSelected => "output_not_selected",
            EngineErrorCode.EditNotFound => "edit_not_found",
            EngineErrorCode.RecordNotFound => "record_not_found",
            EngineErrorCode.UnsupportedOperation => "unsupported_operation",
            EngineErrorCode.ValidationFailed => "validation_failed",
            EngineErrorCode.ExternalChangeDetected => "external_change_detected",
            EngineErrorCode.OutputDirectoryBusy => "output_directory_busy",
            EngineErrorCode.CommitOutcomeUnknown => "commit_outcome_unknown",
            EngineErrorCode.RepairRequired => "repair_required",
            EngineErrorCode.NoRecoveryEvidence => "no_recovery_evidence",
            EngineErrorCode.UnexpectedFailure => "unexpected_failure",
            _ => "unexpected_failure",
        };
    }

    /// <summary>Requires the exact result revision attached by the engine operation.</summary>
    /// <typeparam name="T">The engine result value.</typeparam>
    /// <param name="result">The successful or failed engine result.</param>
    /// <param name="revision">Receives the attached exact result revision.</param>
    /// <param name="message">Receives a failure message when the revision is absent.</param>
    /// <returns><see langword="true"/> when the result includes its exact revision.</returns>
    internal static bool TryGetResultRevision<T>(
        EngineResult<T> result,
        out WorkspaceRevision revision,
        out string message)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (!result.ResultRevision.HasValue)
        {
            revision = default;
            message = "The engine result did not include its exact workspace revision.";
            return false;
        }

        revision = result.ResultRevision.Value;
        message = string.Empty;
        return true;
    }

    /// <summary>Serializes an arbitrary closed projection to a detached JSON element.</summary>
    /// <param name="value">The projection to serialize.</param>
    /// <returns>A detached JSON value.</returns>
    internal static JsonElement Json(object value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return JsonSerializer.SerializeToElement(value);
    }
}
