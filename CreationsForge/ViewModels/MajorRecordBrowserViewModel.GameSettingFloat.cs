using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.ViewModels;

/// <summary>One detached revision-bound native float-setting form shown beside the record tree.</summary>
public sealed record GameSettingFloatEditorSession(
    Guid EditId,
    FormKey FormKey,
    WorkspaceRevision Revision,
    string EditorId,
    float? Data,
    int MajorRecordFlagsRaw,
    ushort FormVersion,
    ushort Version2,
    uint VersionControl,
    ulong? Xalg,
    bool SupportsXalg);

/// <summary>Coordinates direct native float-setting forms through the shared workspace owner.</summary>
public sealed partial class MajorRecordBrowserViewModel
{
    /// <summary>Begins a new, exact source override, or staged-output float-setting form.</summary>
    public async Task<EngineResult<GameSettingFloatEditorSession>> BeginGameSettingFloatAsync(MajorRecordViewModel? row)
    {
        using var admission = OperationArbiter.TryBeginEditorOperation();
        if (admission is null)
        {
            return EditFailure("Another edit or workspace transition is in progress.");
        }

        var descriptor = WorkspaceCoordinator.CurrentWorkspace;
        if (descriptor?.Output is null)
        {
            return EditFailure("Open an output plugin for editing first.");
        }

        if (row is not null && (!RecordsValue.Contains(row) || row.RecordType != "GameSettingFloat"))
        {
            return EditFailure("Refresh the record tree and choose a current GameSettingFloat row.");
        }

        try
        {
            return await WorkspaceCoordinator.ExecuteAsync(async (workspace, token) =>
            {
                var state = await workspace.ReadStateAsync(token).ConfigureAwait(false);
                if (!state.Succeeded || state.Value is null)
                {
                    return EditFailure(state.Error?.Message ?? "The workspace state could not be read.");
                }

                if (row is not null && RevisionValue != state.Value.Revision)
                {
                    return EditFailure("The record tree is stale. Refresh it before editing this record.");
                }

                var role = row is null ? FormListEditRole.New : row.Role == PluginRole.Output
                    ? FormListEditRole.ExistingOutput : FormListEditRole.Override;
                ReferenceRequest? selection = null;
                if (role == FormListEditRole.Override)
                {
                    selection = new ReferenceRequest(row!.FormKey, RecordScope.AllContexts, row.ContainingModKey);
                    var source = await workspace.ReadRecordContextAsync(selection, token).ConfigureAwait(false);
                    if (!source.Succeeded || source.Value is null ||
                        source.Value.RecordType != "GameSettingFloat" ||
                        source.Value.Context.LoadOrderIndex != row.LoadOrderIndex ||
                        source.Value.Context.Role != row.Role)
                    {
                        return EditFailure("The selected plugin context changed. Refresh the record tree and try again.");
                    }
                }

                var begin = await workspace.BeginEditAsync(new BeginEditRequest(
                    Guid.NewGuid(), state.Value.Revision, role,
                    originFormKey: role == FormListEditRole.Override ? row!.FormKey : null,
                    originSelection: selection,
                    targetFormKey: role == FormListEditRole.ExistingOutput ? row!.FormKey : null,
                    recordType: "GameSettingFloat"), token).ConfigureAwait(false);
                if (!begin.Succeeded || begin.Value is null)
                {
                    return EditFailure(begin.Error?.Message ?? "The GameSettingFloat edit could not begin.");
                }

                return await ReadGameSettingFloatSessionAsync(workspace, begin.Value.EditId, begin.Value.FormKey, begin.Value.Revision, token).ConfigureAwait(false);
            }, admission.CancellationToken);
        }
        catch (OperationCanceledException)
        {
            return EditFailure("The record edit was interrupted by a workspace transition. Refresh the record tree before retrying.");
        }
    }

    /// <summary>Saves the complete visible float-setting form as one staged native operation.</summary>
    public async Task<EngineResult<GameSettingFloatEditorSession>> SaveGameSettingFloatAsync(
        GameSettingFloatEditorSession session,
        GameSettingFloatEditRequest request)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(request);
        using var admission = OperationArbiter.TryBeginEditorOperation();
        if (admission is null)
        {
            return EditFailure("Another edit or workspace transition is in progress.");
        }

        try
        {
            var result = await WorkspaceCoordinator.ExecuteAsync(async (workspace, token) =>
            {
                var applied = await workspace.ApplyGameSettingFloatEditAsync(request, token).ConfigureAwait(false);
                if (!applied.Succeeded || applied.Value is null)
                {
                    return EditFailure(applied.Error?.Message ?? "The record fields could not be saved.");
                }

                return await ReadGameSettingFloatSessionAsync(workspace, session.EditId, session.FormKey, applied.Value.Revision, token).ConfigureAwait(false);
            }, admission.CancellationToken);
            if (result.Succeeded)
            {
                await RefreshAsync().ConfigureAwait(false);
            }

            return result;
        }
        catch (OperationCanceledException)
        {
            return EditFailure("The record save was interrupted. Inspect the staged record before retrying.");
        }
    }

    private static async ValueTask<EngineResult<GameSettingFloatEditorSession>> ReadGameSettingFloatSessionAsync(
        IPluginWorkspace workspace,
        Guid editId,
        FormKey formKey,
        WorkspaceRevision revision,
        CancellationToken token)
    {
        var read = await workspace.ReadMajorRecordViewAsync(
            new ReferenceRequest(formKey, RecordScope.StagedOutput), token).ConfigureAwait(false);
        if (!read.Succeeded || read.Value?.Record is not { } record || read.Value.RecordType != "GameSettingFloat")
        {
            return EditFailure(read.Error?.Message ?? "The staged GameSettingFloat could not be read.");
        }

        try
        {
            return EngineResult<GameSettingFloatEditorSession>.Success(new GameSettingFloatEditorSession(
                editId,
                formKey,
                revision,
                record.TryGetProperty("EditorID", out var editorId) && editorId.ValueKind == JsonValueKind.String ? editorId.GetString()! : string.Empty,
                record.TryGetProperty("Data", out var data) && data.ValueKind == JsonValueKind.Object ? data.GetProperty("number").GetSingle() : null,
                record.GetProperty("MajorRecordFlagsRaw").GetInt32(),
                record.GetProperty("FormVersion").GetUInt16(),
                record.GetProperty("Version2").GetUInt16(),
                record.GetProperty("VersionControl").GetUInt32(),
                record.TryGetProperty("XALG", out var xalg) && xalg.ValueKind == JsonValueKind.Number ? xalg.GetUInt64() : null,
                record.TryGetProperty("XALG", out _)));
        }
        catch (Exception exception) when (exception is JsonException or InvalidOperationException or KeyNotFoundException)
        {
            return EditFailure($"The installed Mutagen GameSettingFloat fields could not be displayed: {exception.Message}");
        }
    }

    private static EngineResult<GameSettingFloatEditorSession> EditFailure(string message) =>
        EngineResult<GameSettingFloatEditorSession>.Failure(new EngineError(EngineErrorCode.ValidationFailed, message));
}
