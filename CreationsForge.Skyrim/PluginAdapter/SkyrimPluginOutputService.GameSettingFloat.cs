using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace CreationsForge.Skyrim.PluginAdapter;

/// <summary>Owns native Skyrim GameSettingFloat allocation and exact-context override selection.</summary>
public sealed partial class SkyrimPluginOutputService
{
    /// <summary>Replaces every independent native field of one staged float game setting.</summary>
    public EngineResult<RecordEditMutationResult> ApplyGameSettingFloatEdit(
        SkyrimPluginOutputState candidate,
        FormKey target,
        GameSettingFloatEditRequest request,
        CancellationToken cancellationToken)
    {
        if (!request.EditorId.StartsWith('f') || request.Xalg is not null || request.Data is { } data && !float.IsFinite(data))
        {
            return EngineResult<RecordEditMutationResult>.Failure(new EngineError(
                EngineErrorCode.ValidationFailed,
                "Skyrim float game settings require an EditorID beginning with f, a finite value, and no XALG."));
        }

        var found = FindGameSettingFloat(candidate.GetMutableMod(), target, cancellationToken);
        if (!found.Succeeded || found.Value?.Record is null)
        {
            return EngineResult<RecordEditMutationResult>.Failure(
                found.Error ?? new EngineError(EngineErrorCode.RecordNotFound, $"Skyrim output has no GameSettingFloat {target}."));
        }

        var record = found.Value.Record;
        var changed = record.EditorID != request.EditorId
            || record.Data != request.Data
            || record.MajorRecordFlagsRaw != request.MajorRecordFlagsRaw
            || record.FormVersion != request.FormVersion
            || record.Version2 != request.Version2
            || record.VersionControl != request.VersionControl;
        record.EditorID = request.EditorId;
        record.Data = request.Data;
        record.MajorRecordFlagsRaw = request.MajorRecordFlagsRaw;
        record.FormVersion = request.FormVersion;
        record.Version2 = request.Version2;
        record.VersionControl = request.VersionControl;
        return EngineResult<RecordEditMutationResult>.Success(new RecordEditMutationResult(changed));
    }

    /// <summary>Begins one GameSettingFloat edit inside a complete unpublished output candidate.</summary>
    /// <param name="sources">The exact immutable source load order.</param>
    /// <param name="candidate">The complete mutable output candidate.</param>
    /// <param name="request">The new, override, or existing-output selection.</param>
    /// <param name="cancellationToken">A token observed around native record scans and copying.</param>
    /// <returns>The selected edit identity, or a typed rejection without publishing the candidate.</returns>
    private static EngineResult<RecordEditIdentity> BeginGameSettingFloat(
        SkyrimPluginSourceSet sources,
        SkyrimPluginOutputState candidate,
        BeginEditRequest request,
        CancellationToken cancellationToken)
    {
        var mod = candidate.GetMutableMod();
        if (request.Role == FormListEditRole.New)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var created = new GameSettingFloat(mod);
            mod.GameSettings.Add(created);
            var uniqueness = FindGameSettingFloat(mod, created.FormKey, cancellationToken);
            if (!uniqueness.Succeeded)
            {
                return EngineResult<RecordEditIdentity>.Failure(uniqueness.Error!);
            }

            var identity = new RecordEditIdentity(Guid.NewGuid(), created.FormKey, null, request.Role, request.RecordType);
            candidate.RegisterEditProvenance(new RecordEditProvenance(
                identity.EditId,
                identity.FormKey,
                EditBaselineKind.Absent,
                recordType: request.RecordType));
            return EngineResult<RecordEditIdentity>.Success(identity);
        }

        if (request.Role == FormListEditRole.Override)
        {
            var formKey = request.OriginFormKey!.Value;
            var selection = request.OriginSelection ?? new ReferenceRequest(formKey, RecordScope.WinningOverrides);
            var readResult = sources.ReadRecordContext(selection, null, cancellationToken);
            if (!readResult.Succeeded || readResult.Value is null)
            {
                return EngineResult<RecordEditIdentity>.Failure(
                    readResult.Error ?? new EngineError(EngineErrorCode.RecordNotFound, "The Skyrim override origin could not be read."),
                    warnings: readResult.Warnings);
            }

            if (readResult.Value.Context.Status != ReferenceResolutionStatus.Resolved ||
                readResult.Value.Record is not IGameSettingFloatGetter origin)
            {
                return EngineResult<RecordEditIdentity>.Failure(new EngineError(
                    EngineErrorCode.ValidationFailed,
                    $"Skyrim override origin {formKey} is not one live GameSettingFloat in the selected context."),
                    warnings: readResult.Warnings);
            }

            var current = FindGameSettingFloat(mod, formKey, cancellationToken);
            if (!current.Succeeded)
            {
                return EngineResult<RecordEditIdentity>.Failure(current.Error!, warnings: readResult.Warnings);
            }

            if (current.Value?.Record is null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                mod.GameSettings.Add((GameSettingFloat)origin.DeepCopy());
                current = FindGameSettingFloat(mod, formKey, cancellationToken);
                if (!current.Succeeded || current.Value?.Record is null)
                {
                    return EngineResult<RecordEditIdentity>.Failure(
                        current.Error ?? new EngineError(EngineErrorCode.ValidationFailed, "The Skyrim override was not retained in output."),
                        warnings: readResult.Warnings);
                }
            }

            var identity = new RecordEditIdentity(Guid.NewGuid(), formKey, formKey, request.Role, request.RecordType);
            candidate.RegisterEditProvenance(CreateGameSettingFloatProvenance(
                sources,
                candidate,
                identity,
                readResult.Value.Context));
            return EngineResult<RecordEditIdentity>.Success(identity, warnings: readResult.Warnings);
        }

        if (request.Role == FormListEditRole.ExistingOutput)
        {
            var formKey = request.TargetFormKey!.Value;
            var current = FindGameSettingFloat(mod, formKey, cancellationToken);
            if (!current.Succeeded || current.Value?.Record is null)
            {
                return EngineResult<RecordEditIdentity>.Failure(
                    current.Error ?? new EngineError(EngineErrorCode.RecordNotFound, $"Skyrim output has no GameSettingFloat {formKey}."));
            }

            var identity = new RecordEditIdentity(Guid.NewGuid(), formKey, null, request.Role, request.RecordType);
            candidate.RegisterEditProvenance(CreateGameSettingFloatProvenance(sources, candidate, identity, null));
            return EngineResult<RecordEditIdentity>.Success(identity);
        }

        return EngineResult<RecordEditIdentity>.Failure(new EngineError(
            EngineErrorCode.InvalidRequest,
            $"Unknown Skyrim GameSettingFloat edit role '{request.Role}'."));
    }

    /// <summary>Finds exactly one GameSettingFloat while rejecting cross-family or duplicate output identities.</summary>
    /// <param name="mod">The complete output candidate.</param>
    /// <param name="formKey">The exact record identity.</param>
    /// <param name="cancellationToken">A token observed during the full-record scan.</param>
    /// <returns>The mutable record, absence, or a typed ambiguity failure.</returns>
    private sealed record GameSettingFloatSearch(GameSettingFloat? Record);

    private static EngineResult<GameSettingFloatSearch> FindGameSettingFloat(
        SkyrimMod mod,
        FormKey formKey,
        CancellationToken cancellationToken)
    {
        GameSettingFloat? found = null;
        foreach (var record in mod.EnumerateMajorRecords())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (record.FormKey != formKey)
            {
                continue;
            }

            if (record is not GameSettingFloat gameSetting || found is not null)
            {
                return EngineResult<GameSettingFloatSearch>.Failure(new EngineError(
                    EngineErrorCode.ValidationFailed,
                    $"Skyrim output identity {formKey} is occupied or ambiguous across record families."));
            }

            found = gameSetting;
        }

        return EngineResult<GameSettingFloatSearch>.Success(new GameSettingFloatSearch(found));
    }

    /// <summary>Captures the first exact original-output or source baseline of a GameSettingFloat.</summary>
    /// <param name="sources">The immutable source baseline and load-order position.</param>
    /// <param name="candidate">The unpublished output and independent original output.</param>
    /// <param name="identity">The selected native edit identity.</param>
    /// <param name="sourceContext">The selected source override context, if any.</param>
    /// <returns>Stable provenance for preview and guarded writer verification.</returns>
    private static RecordEditProvenance CreateGameSettingFloatProvenance(
        SkyrimPluginSourceSet sources,
        SkyrimPluginOutputState candidate,
        RecordEditIdentity identity,
        FormListContext? sourceContext)
    {
        var original = candidate.GetOriginalMod().EnumerateMajorRecords()
            .OfType<IGameSettingFloatGetter>()
            .FirstOrDefault(record => record.FormKey == identity.FormKey);
        if (original is not null)
        {
            var context = new FormListContext(
                new ReferenceRequest(identity.FormKey, RecordScope.StagedOutput),
                original.IsDeleted ? ReferenceResolutionStatus.Deleted : ReferenceResolutionStatus.Resolved,
                candidate.Association.ModKey,
                candidate.Association.PluginPath,
                sources.GetMutagenMods().Count,
                PluginRole.Output);
            return new RecordEditProvenance(
                identity.EditId,
                identity.FormKey,
                EditBaselineKind.OriginalOutput,
                context,
                recordType: identity.RecordType);
        }

        if (sourceContext?.ContainingModKey is { } containingModKey)
        {
            var context = new FormListContext(
                new ReferenceRequest(identity.FormKey, RecordScope.AllContexts, containingModKey),
                sourceContext.Status,
                sourceContext.ContainingModKey,
                sourceContext.Path,
                sourceContext.LoadOrderIndex,
                sourceContext.Role);
            return new RecordEditProvenance(
                identity.EditId,
                identity.FormKey,
                EditBaselineKind.SourceContext,
                context,
                sources.Baseline.BaselineId,
                identity.RecordType);
        }

        return new RecordEditProvenance(
            identity.EditId,
            identity.FormKey,
            EditBaselineKind.Absent,
            recordType: identity.RecordType);
    }
}
