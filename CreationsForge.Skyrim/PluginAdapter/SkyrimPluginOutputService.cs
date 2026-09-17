using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.PluginOutputs;
using CreationsForge.Core.Engine.RecordInspection;
using CreationsForge.Skyrim.PluginAdapter.RecordInspection;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace CreationsForge.Skyrim.PluginAdapter;

/// <summary>
/// Opens, clones, and begins edits against complete Skyrim Special Edition output state without writing destination files.
/// </summary>
public sealed partial class SkyrimPluginOutputService
{
    /// <summary>The shared output admission and physical baseline service.</summary>
    private readonly PluginOutputInputLoader _inputLoader;

    /// <summary>
    /// Initializes a Skyrim Special Edition plugin output service.
    /// </summary>
    /// <param name="inputLoader">The shared output admission service.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="inputLoader"/> is <see langword="null"/>.</exception>
    public SkyrimPluginOutputService(PluginOutputInputLoader inputLoader)
    {
        ArgumentNullException.ThrowIfNull(inputLoader);
        _inputLoader = inputLoader;
        Inspector = new SkyrimFormListInspector();
        MajorRecordInspector = new MutagenMajorRecordInspector(
            typeof(SkyrimMajorRecord),
            "Mutagen.Bethesda.Skyrim/0.55.0-alpha.53");
    }

    /// <summary>Gets the stateless complete Skyrim FormList inspector used for detached snapshots.</summary>
    public SkyrimFormListInspector Inspector { get; }

    /// <summary>Gets the complete native Skyrim major-record inspector shared by read and save verification.</summary>
    public IMajorRecordInspector MajorRecordInspector { get; }

    /// <summary>
    /// Opens or constructs a complete Skyrim output candidate without changing any destination artifact.
    /// </summary>
    /// <param name="sources">The borrowed complete Skyrim source lifetime.</param>
    /// <param name="request">The guarded create-new or open-existing selection.</param>
    /// <param name="cancellationToken">A token that cancels admission and Mutagen materialization before publication.</param>
    /// <returns>The independently owned complete output and its physical baseline, or a typed failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="sources"/> or <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public async Task<EngineResult<PluginOutputOpenResult>> OpenAsync(
        SkyrimPluginSourceSet sources,
        SelectOutputRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sources);
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();
        return await OpenCoreAsync(
            sources,
            request.Output,
            request.Mode,
            expectedBaseline: null,
            request.OperationId,
            request.ExpectedRevision,
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Reopens a complete existing Skyrim output only when its physical state still matches the supplied baseline.
    /// </summary>
    /// <param name="sources">The borrowed complete Skyrim source lifetime.</param>
    /// <param name="association">The canonical selected output association.</param>
    /// <param name="expectedBaseline">The exact output artifact baseline established by the prior selection.</param>
    /// <param name="cancellationToken">A token that cancels admission and Mutagen materialization before publication.</param>
    /// <returns>The independently owned reopened output, or a typed baseline or Mutagen-open failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public Task<EngineResult<PluginOutputOpenResult>> ReopenAsync(
        SkyrimPluginSourceSet sources,
        OutputAssociation association,
        OutputArtifactSetBaseline expectedBaseline,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sources);
        ArgumentNullException.ThrowIfNull(association);
        ArgumentNullException.ThrowIfNull(expectedBaseline);
        var pluginArtifact = expectedBaseline.Artifacts.SingleOrDefault(
            artifact => artifact.Role == PluginArtifactRole.Plugin);
        if (pluginArtifact is null)
        {
            return Task.FromResult(OutputFailure(
                sources,
                operationId: null,
                sources.Revision,
                EngineErrorCode.InvalidRequest,
                "The expected Skyrim output baseline does not contain exactly one plugin artifact."));
        }

        var mode = pluginArtifact.Fingerprint.Exists
            ? OutputSelectionMode.OpenExisting
            : OutputSelectionMode.CreateNew;
        return OpenCoreAsync(
            sources,
            association,
            mode,
            expectedBaseline,
            operationId: null,
            baseRevision: sources.Revision,
            cancellationToken);
    }

    /// <summary>
    /// Creates an independently disposable complete candidate for transactional mutation.
    /// </summary>
    /// <param name="output">The current complete Skyrim output state.</param>
    /// <param name="cancellationToken">A token checked around each uninterruptible Mutagen deep copy.</param>
    /// <returns>An independent candidate retaining current staged state and the original complete baseline.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="output"/> is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown after <paramref name="output"/> is disposed.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public SkyrimPluginOutputState Clone(
        SkyrimPluginOutputState output,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(output);
        return output.Clone(cancellationToken);
    }

    /// <summary>
    /// Allocates, overrides, or selects one Skyrim FormList only inside an unpublished complete output candidate.
    /// </summary>
    /// <param name="sources">The borrowed source lifetime used to resolve an override origin.</param>
    /// <param name="candidate">The unpublished complete output candidate to mutate.</param>
    /// <param name="request">The Mutagen begin-edit request.</param>
    /// <param name="cancellationToken">A token checked around Mutagen selection, allocation, and override copying.</param>
    /// <returns>The stable record edit identity, or a typed rejection.</returns>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public EngineResult<RecordEditIdentity> BeginEdit(
        SkyrimPluginSourceSet sources,
        SkyrimPluginOutputState candidate,
        BeginEditRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(sources);
        ArgumentNullException.ThrowIfNull(candidate);
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var result = request.Role switch
            {
                FormListEditRole.New => BeginNew(candidate, request, cancellationToken),
                FormListEditRole.Override => BeginOverride(sources, candidate, request, cancellationToken),
                FormListEditRole.ExistingOutput => BeginExistingOutput(candidate, request, cancellationToken),
                _ => EngineResult<RecordEditIdentity>.Failure(new EngineError(
                    EngineErrorCode.InvalidRequest,
                    $"The Skyrim FormList edit role '{request.Role}' is undefined."))
            };
            if (result.Succeeded && result.Value is not null)
            {
                result = RegisterEditProvenance(
                    sources,
                    candidate,
                    request,
                    result,
                    cancellationToken);
            }

            return BindEditResult(sources, request, result);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (ObjectDisposedException exception)
        {
            return BindEditResult(
                sources,
                request,
                EngineResult<RecordEditIdentity>.Failure(new EngineError(
                    EngineErrorCode.WorkspaceDisposed,
                    exception.Message)));
        }
        catch (Exception exception)
        {
            return BindEditResult(
                sources,
                request,
                EngineResult<RecordEditIdentity>.Failure(new EngineError(
                    EngineErrorCode.ValidationFailed,
                    $"Skyrim rejected the FormList begin-edit operation: {exception.Message}")));
        }
    }

    /// <summary>Admits and fully materializes one new or existing plugin output before publishing any state.</summary>
    /// <param name="sources">The borrowed complete source lifetime.</param>
    /// <param name="association">The requested output association.</param>
    /// <param name="mode">Whether the output must be absent or present.</param>
    /// <param name="expectedBaseline">The exact required prior baseline for reopen, or <see langword="null"/> for initial selection.</param>
    /// <param name="operationId">The selection operation identifier, when applicable.</param>
    /// <param name="baseRevision">The source revision associated with the operation.</param>
    /// <param name="cancellationToken">The token checked throughout admission and materialization.</param>
    /// <returns>The independently owned complete output state or a typed failure.</returns>
    private async Task<EngineResult<PluginOutputOpenResult>> OpenCoreAsync(
        SkyrimPluginSourceSet sources,
        OutputAssociation association,
        OutputSelectionMode mode,
        OutputArtifactSetBaseline? expectedBaseline,
        Guid? operationId,
        WorkspaceRevision baseRevision,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        EngineResult<PluginOutputInputs> preparation;
        try
        {
            preparation = await _inputLoader.PrepareAsync(
                sources.BorrowInputs(),
                association,
                mode,
                cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (ObjectDisposedException)
        {
            return OutputFailure(
                sources,
                operationId,
                baseRevision,
                EngineErrorCode.WorkspaceDisposed,
                "The Skyrim plugin source lifetime has been disposed.");
        }

        if (!preparation.Succeeded || preparation.Value is null)
        {
            return EngineResult<PluginOutputOpenResult>.Failure(
                preparation.Error ?? new EngineError(EngineErrorCode.OutputOpenFailed, "The Skyrim output could not be admitted."),
                workspaceId: sources.WorkspaceId,
                operationId: operationId,
                baseRevision: baseRevision,
                resultRevision: baseRevision,
                warnings: preparation.Warnings);
        }

        await using var inputs = preparation.Value;
        try
        {
            if (inputs.Release != GameRelease.SkyrimSE)
            {
                return OutputFailure(
                    sources,
                    operationId,
                    baseRevision,
                    EngineErrorCode.UnsupportedGameRelease,
                    $"Skyrim output materialization requires {GameRelease.SkyrimSE}; received {inputs.Release}.");
            }

            if (inputs.Output.MasterStyle == MasterStyle.Medium)
            {
                return OutputFailure(
                    sources,
                    operationId,
                    baseRevision,
                    EngineErrorCode.UnsupportedInput,
                    "Skyrim Special Edition does not support medium-master output state.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            var mod = inputs.Output.Exists
                ? ReadExistingMod(inputs, cancellationToken)
                : CreateNewMod(inputs.Output);
            cancellationToken.ThrowIfCancellationRequested();

            var stateMismatch = ValidatePluginState(mod, inputs.Output);
            if (stateMismatch is not null)
            {
                return OutputFailure(
                    sources,
                    operationId,
                    baseRevision,
                    EngineErrorCode.OutputOpenFailed,
                    stateMismatch);
            }

            var baselineResult = await inputs.CompleteOpenAsync(cancellationToken).ConfigureAwait(false);
            if (!baselineResult.Succeeded || baselineResult.Value is null)
            {
                return EngineResult<PluginOutputOpenResult>.Failure(
                    baselineResult.Error ?? new EngineError(EngineErrorCode.OutputOpenFailed, "The Skyrim output baseline could not be completed."),
                    workspaceId: sources.WorkspaceId,
                    operationId: operationId,
                    baseRevision: baseRevision,
                    resultRevision: baseRevision,
                    warnings: baselineResult.Warnings);
            }

            var baseline = baselineResult.Value;
            if (expectedBaseline is not null && !BaselinesMatch(expectedBaseline, baseline))
            {
                return OutputFailure(
                    sources,
                    operationId,
                    baseRevision,
                    EngineErrorCode.ExternalChangeDetected,
                    "The Skyrim output no longer matches the exact baseline established by its prior selection.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            var original = CopyMod(mod, cancellationToken);
            var canonicalAssociation = new OutputAssociation(
                inputs.Output.Path,
                inputs.Output.ModKey,
                association.LocalizedOutputMode,
                association.MasterStyle);
            var state = new SkyrimPluginOutputState(mod, original, canonicalAssociation, baseline);
            return EngineResult<PluginOutputOpenResult>.Success(
                new PluginOutputOpenResult(state, canonicalAssociation, baseline),
                workspaceId: sources.WorkspaceId,
                operationId: operationId,
                baseRevision: baseRevision,
                resultRevision: baseRevision,
                warnings: preparation.Warnings.Concat(baselineResult.Warnings).ToArray());
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            return OutputFailure(
                sources,
                operationId,
                baseRevision,
                EngineErrorCode.OutputOpenFailed,
                $"Skyrim Special Edition plugin output materialization failed: {exception.Message}");
        }
    }

    /// <summary>Constructs one complete empty Mutagen mod with the exact requested header capabilities.</summary>
    /// <param name="output">The admitted absent output descriptor.</param>
    /// <returns>The new complete in-memory Skyrim mod.</returns>
    private static SkyrimMod CreateNewMod(PluginOutputPluginInput output)
    {
        var mod = new SkyrimMod(output.ModKey, SkyrimRelease.SkyrimSE)
        {
            IsMaster = output.ModKey.Type == ModType.Master,
            IsSmallMaster = output.MasterStyle == MasterStyle.Small,
            UsingLocalization = output.UsesLocalization,
        };
        return mod;
    }

    /// <summary>Fully parses every Mutagen group from an admitted existing output through strict shared metadata.</summary>
    /// <param name="inputs">The short-lived output parsing scope.</param>
    /// <param name="cancellationToken">The token applied to parser read and seek boundaries.</param>
    /// <returns>The complete mutable Mutagen Skyrim mod.</returns>
    private static SkyrimMod ReadExistingMod(
        PluginOutputInputs inputs,
        CancellationToken cancellationToken)
    {
        using MutagenBinaryReadStream stream = inputs.OpenReadStream(cancellationToken);
        return SkyrimMod.CreateFromBinary(
            new MutagenFrame(stream),
            SkyrimRelease.SkyrimSE,
            new GroupMask(true));
    }

    /// <summary>Validates complete record identity, localization, and master flags against admitted metadata.</summary>
    /// <param name="mod">The complete materialized Skyrim mod.</param>
    /// <param name="output">The admitted plugin output descriptor.</param>
    /// <returns>A mismatch description, or <see langword="null"/> when the Mutagen state is exact.</returns>
    private static string? ValidatePluginState(SkyrimMod mod, PluginOutputPluginInput output)
    {
        if (mod.ModKey != output.ModKey)
        {
            return $"Plugin output identity {mod.ModKey} did not match admitted identity {output.ModKey}.";
        }

        var observedStyle = mod.IsMediumMaster
            ? MasterStyle.Medium
            : mod.IsSmallMaster
                ? MasterStyle.Small
                : MasterStyle.Full;
        if (observedStyle != output.MasterStyle)
        {
            return $"Plugin output master style {observedStyle} did not match admitted style {output.MasterStyle}.";
        }

        if (mod.UsingLocalization != output.UsesLocalization)
        {
            return "Plugin output localization state did not match its admitted association.";
        }

        if (output.ModKey.Type == ModType.Master && !mod.IsMaster)
        {
            return "A Skyrim .esm output must retain the Mutagen master header flag.";
        }

        return null;
    }

    /// <summary>Begins a Mutagen new-record edit through Mutagen and rejects any resulting cross-family identity collision.</summary>
    /// <param name="candidate">The unpublished complete output candidate.</param>
    /// <param name="request">The validated new-record request.</param>
    /// <param name="cancellationToken">The token checked around enumeration and allocation.</param>
    /// <returns>The allocated record identity or a typed collision failure.</returns>
    private static EngineResult<RecordEditIdentity> BeginNew(
        SkyrimPluginOutputState candidate,
        BeginEditRequest request,
        CancellationToken cancellationToken)
    {
        var mod = candidate.GetMutableMod();
        var existingKeys = new HashSet<FormKey>();
        foreach (var record in mod.EnumerateMajorRecords())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!existingKeys.Add(record.FormKey))
            {
                return DuplicateRecordFailure<RecordEditIdentity>(record.FormKey);
            }
        }

        cancellationToken.ThrowIfCancellationRequested();
        var formList = mod.FormLists.AddNew();
        cancellationToken.ThrowIfCancellationRequested();
        var matchingCount = 0;
        foreach (var record in mod.EnumerateMajorRecords())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (record.FormKey == formList.FormKey)
            {
                matchingCount++;
            }
        }

        if (formList.FormKey.ModKey != mod.ModKey
            || existingKeys.Contains(formList.FormKey)
            || matchingCount != 1)
        {
            return EngineResult<RecordEditIdentity>.Failure(new EngineError(
                EngineErrorCode.ValidationFailed,
                $"Skyrim Mutagen allocation produced colliding FormKey '{formList.FormKey}'."));
        }

        return EngineResult<RecordEditIdentity>.Success(new RecordEditIdentity(
            request.OperationId,
            formList.FormKey,
            originFormKey: null,
            FormListEditRole.New));
    }

    /// <summary>Begins an override from an exact or source-only winning FormList context.</summary>
    /// <param name="sources">The borrowed plugin source lifetime.</param>
    /// <param name="candidate">The unpublished complete output candidate.</param>
    /// <param name="request">The override request.</param>
    /// <param name="cancellationToken">The token checked around selection and Mutagen copying.</param>
    /// <returns>The override identity or a typed selection failure.</returns>
    private static EngineResult<RecordEditIdentity> BeginOverride(
        SkyrimPluginSourceSet sources,
        SkyrimPluginOutputState candidate,
        BeginEditRequest request,
        CancellationToken cancellationToken)
    {
        var originFormKey = request.OriginFormKey!.Value;
        var selection = request.OriginSelection
            ?? new ReferenceRequest(originFormKey, RecordScope.WinningOverrides);
        var readResult = sources.ReadFormListContext(selection, cancellationToken);
        if (!readResult.Succeeded || readResult.Value is null)
        {
            return EngineResult<RecordEditIdentity>.Failure(
                readResult.Error ?? new EngineError(EngineErrorCode.ValidationFailed, "The Skyrim override origin could not be resolved."),
                warnings: readResult.Warnings);
        }

        var read = readResult.Value;
        if (read.Context.Status == ReferenceResolutionStatus.Deleted)
        {
            return EngineResult<RecordEditIdentity>.Failure(
                new EngineError(
                    EngineErrorCode.ValidationFailed,
                    $"Skyrim FormList '{originFormKey}' resolves to a deleted record context; no older context was substituted."),
                warnings: readResult.Warnings);
        }

        if (read.Context.Status == ReferenceResolutionStatus.Unresolved)
        {
            return EngineResult<RecordEditIdentity>.Failure(
                new EngineError(EngineErrorCode.RecordNotFound, $"Skyrim FormList origin '{originFormKey}' was not found in the selected source context."),
                warnings: readResult.Warnings);
        }

        if (read.Context.Status != ReferenceResolutionStatus.Resolved || read.Record is not IFormListGetter origin)
        {
            return EngineResult<RecordEditIdentity>.Failure(
                new EngineError(
                    EngineErrorCode.ValidationFailed,
                    $"Skyrim FormList origin '{originFormKey}' resolved as {read.Context.Status} instead of one exact supported FormList."),
                warnings: readResult.Warnings);
        }

        var mod = candidate.GetMutableMod();
        var existingResult = FindCandidateRecord(mod, originFormKey, cancellationToken);
        if (existingResult.Succeeded && existingResult.Value is not IFormListGetter)
        {
            return EngineResult<RecordEditIdentity>.Failure(
                WrongFamilyFailure(originFormKey, existingResult.Value!).Error!,
                warnings: readResult.Warnings);
        }

        if (!existingResult.Succeeded && existingResult.Error?.Code != EngineErrorCode.RecordNotFound)
        {
            return EngineResult<RecordEditIdentity>.Failure(
                existingResult.Error!,
                warnings: readResult.Warnings);
        }

        cancellationToken.ThrowIfCancellationRequested();
        var target = mod.FormLists.GetOrAddAsOverride(origin);
        cancellationToken.ThrowIfCancellationRequested();
        var uniqueness = FindCandidateRecord(mod, target.FormKey, cancellationToken);
        if (!uniqueness.Succeeded || uniqueness.Value is not IFormListGetter)
        {
            return EngineResult<RecordEditIdentity>.Failure(
                uniqueness.Error ?? new EngineError(EngineErrorCode.ValidationFailed, "The Skyrim override did not produce one exact FormList target."),
                warnings: readResult.Warnings);
        }

        return EngineResult<RecordEditIdentity>.Success(
            new RecordEditIdentity(request.OperationId, target.FormKey, originFormKey, FormListEditRole.Override),
            warnings: readResult.Warnings);
    }

    /// <summary>Selects one exact FormList already present in the complete output without mutating its Mutagen state.</summary>
    /// <param name="candidate">The unpublished complete output candidate.</param>
    /// <param name="request">The existing-output target request.</param>
    /// <param name="cancellationToken">The token checked around complete Mutagen selection.</param>
    /// <returns>The existing-output edit identity or a typed missing or wrong-family failure.</returns>
    private static EngineResult<RecordEditIdentity> BeginExistingOutput(
        SkyrimPluginOutputState candidate,
        BeginEditRequest request,
        CancellationToken cancellationToken)
    {
        var targetFormKey = request.TargetFormKey!.Value;
        var recordResult = FindCandidateRecord(candidate.GetMutableMod(), targetFormKey, cancellationToken);
        if (!recordResult.Succeeded)
        {
            return EngineResult<RecordEditIdentity>.Failure(recordResult.Error!);
        }

        if (recordResult.Value is not IFormListGetter formList)
        {
            return WrongFamilyFailure(targetFormKey, recordResult.Value!);
        }

        cancellationToken.ThrowIfCancellationRequested();
        return CreateEditIdentity(
            request.OperationId,
            formList.FormKey,
            originFormKey: null,
            FormListEditRole.ExistingOutput);
    }

    /// <summary>Captures the target's first immutable before-view after Mutagen begin-edit succeeds.</summary>
    /// <param name="sources">The borrowed source lifetime used for exact source-context capture.</param>
    /// <param name="candidate">The unpublished complete output candidate that owns provenance.</param>
    /// <param name="request">The successful Mutagen begin-edit request.</param>
    /// <param name="result">The successful record identity and any source-selection warnings.</param>
    /// <param name="cancellationToken">The token checked while finding and copying the exact baseline.</param>
    /// <returns>The original successful result, or a typed failure when the exact baseline cannot be captured.</returns>
    private static EngineResult<RecordEditIdentity> RegisterEditProvenance(
        SkyrimPluginSourceSet sources,
        SkyrimPluginOutputState candidate,
        BeginEditRequest request,
        EngineResult<RecordEditIdentity> result,
        CancellationToken cancellationToken)
    {
        var identity = result.Value!;
        if (candidate.GetEditProvenance().Any(entry => entry.TargetFormKey == identity.FormKey))
        {
            return result;
        }

        var originalResult = FindCandidateRecord(
            candidate.GetOriginalMod(),
            identity.FormKey,
            cancellationToken);
        if (originalResult.Succeeded)
        {
            if (originalResult.Value is not IFormListGetter originalFormList)
            {
                return WrongFamilyFailure(identity.FormKey, originalResult.Value!);
            }

            var originalContext = new FormListContext(
                new ReferenceRequest(identity.FormKey, RecordScope.StagedOutput, candidate.Association.ModKey),
                originalFormList.IsDeleted
                    ? ReferenceResolutionStatus.Deleted
                    : ReferenceResolutionStatus.Resolved,
                candidate.Association.ModKey,
                candidate.Association.PluginPath,
                sources.GetMutagenMods().Count,
                PluginRole.Output);
            candidate.RegisterEditProvenance(
                new RecordEditProvenance(
                    identity.EditId,
                    identity.FormKey,
                    EditBaselineKind.OriginalOutput,
                    originalContext));
            return result;
        }

        if (originalResult.Error?.Code != EngineErrorCode.RecordNotFound)
        {
            return EngineResult<RecordEditIdentity>.Failure(
                originalResult.Error
                    ?? new EngineError(EngineErrorCode.ValidationFailed, "The original Skyrim output baseline could not be selected."),
                warnings: result.Warnings);
        }

        if (identity.Role == FormListEditRole.New)
        {
            candidate.RegisterEditProvenance(
                new RecordEditProvenance(
                    identity.EditId,
                    identity.FormKey,
                    EditBaselineKind.Absent));
            return result;
        }

        if (identity.Role != FormListEditRole.Override)
        {
            return EngineResult<RecordEditIdentity>.Failure(
                new EngineError(
                    EngineErrorCode.ValidationFailed,
                    $"Skyrim output FormList '{identity.FormKey}' has no original or source baseline for existing-output editing."),
                warnings: result.Warnings);
        }

        cancellationToken.ThrowIfCancellationRequested();
        var selection = request.OriginSelection
            ?? new ReferenceRequest(identity.FormKey, RecordScope.WinningOverrides);
        var sourceResult = sources.ReadFormListContext(selection, cancellationToken);
        if (!sourceResult.Succeeded
            || sourceResult.Value?.Context.Status != ReferenceResolutionStatus.Resolved
            || sourceResult.Value.Record is not IFormListGetter)
        {
            return EngineResult<RecordEditIdentity>.Failure(
                sourceResult.Error
                    ?? new EngineError(
                        EngineErrorCode.ValidationFailed,
                        $"Skyrim FormList '{identity.FormKey}' no longer has one exact resolved source baseline."),
                warnings: sourceResult.Warnings.Count == 0 ? result.Warnings : sourceResult.Warnings);
        }

        var sourceContext = sourceResult.Value.Context;
        var exactSourceContext = new FormListContext(
            new ReferenceRequest(
                identity.FormKey,
                RecordScope.AllContexts,
                sourceContext.ContainingModKey),
            sourceContext.Status,
            sourceContext.ContainingModKey,
            sourceContext.Path,
            sourceContext.LoadOrderIndex,
            sourceContext.Role);
        candidate.RegisterEditProvenance(
            new RecordEditProvenance(
                identity.EditId,
                identity.FormKey,
                EditBaselineKind.SourceContext,
                exactSourceContext,
                sources.Baseline.BaselineId));
        return result;
    }

    /// <summary>Finds one exact plugin output record across every materialized record family.</summary>
    /// <param name="mod">The complete plugin output.</param>
    /// <param name="formKey">The exact identity to select.</param>
    /// <param name="cancellationToken">The token checked while traversing records.</param>
    /// <returns>The singular record, or a typed absent or duplicate-state failure.</returns>
    private static EngineResult<IMajorRecordGetter> FindCandidateRecord(
        ISkyrimModGetter mod,
        FormKey formKey,
        CancellationToken cancellationToken)
    {
        IMajorRecordGetter? match = null;
        foreach (var record in mod.EnumerateMajorRecords())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (record.FormKey != formKey)
            {
                continue;
            }

            if (match is not null)
            {
                return DuplicateRecordFailure<IMajorRecordGetter>(formKey);
            }

            match = record;
        }

        return match is null
            ? EngineResult<IMajorRecordGetter>.Failure(new EngineError(
                EngineErrorCode.RecordNotFound,
                $"Plugin output record '{formKey}' was not found."))
            : EngineResult<IMajorRecordGetter>.Success(match);
    }

    /// <summary>Creates a typed duplicate-record failure for an invalid complete plugin output.</summary>
    /// <typeparam name="T">The failed result value type.</typeparam>
    /// <param name="formKey">The duplicated record identity.</param>
    /// <returns>A validation failure describing the duplicate identity.</returns>
    private static EngineResult<T> DuplicateRecordFailure<T>(FormKey formKey)
    {
        return EngineResult<T>.Failure(new EngineError(
            EngineErrorCode.ValidationFailed,
            $"Complete Skyrim output contains duplicate Mutagen FormKey '{formKey}'."));
    }

    /// <summary>Creates a typed wrong-family failure for a record identity already owned by another output record family.</summary>
    /// <param name="formKey">The colliding or selected record identity.</param>
    /// <param name="record">The non-FormList record.</param>
    /// <returns>A validation failure retaining the observed record family.</returns>
    private static EngineResult<RecordEditIdentity> WrongFamilyFailure(FormKey formKey, IMajorRecordGetter record)
    {
        return EngineResult<RecordEditIdentity>.Failure(new EngineError(
            EngineErrorCode.ValidationFailed,
            $"Plugin output identity '{formKey}' belongs to {record.GetType().Name}, not a Skyrim FormList."));
    }

    /// <summary>Creates one successful record edit identity.</summary>
    /// <param name="editId">The caller's stable staged edit identifier.</param>
    /// <param name="formKey">The selected FormList identity.</param>
    /// <param name="originFormKey">The source origin for an override, or <see langword="null"/>.</param>
    /// <param name="role">How the candidate selected or created the target.</param>
    /// <returns>The successful record edit identity result.</returns>
    private static EngineResult<RecordEditIdentity> CreateEditIdentity(
        Guid editId,
        FormKey formKey,
        FormKey? originFormKey,
        FormListEditRole role)
    {
        return EngineResult<RecordEditIdentity>.Success(new RecordEditIdentity(
            editId,
            formKey,
            originFormKey,
            role));
    }

    /// <summary>Creates an independent complete mutable copy around cancellation checks.</summary>
    /// <param name="source">The complete plugin output getter.</param>
    /// <param name="cancellationToken">The token checked before and after Mutagen copying.</param>
    /// <returns>The independent complete mutable copy.</returns>
    private static SkyrimMod CopyMod(ISkyrimModGetter source, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var copy = new SkyrimMod(source.ModKey, SkyrimRelease.SkyrimSE);
        SkyrimModMixIn.DeepCopyIn(copy, source);
        cancellationToken.ThrowIfCancellationRequested();
        return copy;
    }

    /// <summary>Compares two immutable output baselines including canonical paths, content, and physical identities.</summary>
    /// <param name="expected">The previously published exact baseline.</param>
    /// <param name="actual">The newly observed exact baseline.</param>
    /// <returns><see langword="true"/> when every baseline field remains equal.</returns>
    private static bool BaselinesMatch(
        OutputArtifactSetBaseline expected,
        OutputArtifactSetBaseline actual)
    {
        if (expected.BaselineId != actual.BaselineId || expected.Artifacts.Count != actual.Artifacts.Count)
        {
            return false;
        }

        var pathComparison = OperatingSystem.IsWindows()
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;
        for (var index = 0; index < expected.Artifacts.Count; index++)
        {
            var expectedArtifact = expected.Artifacts[index];
            var actualArtifact = actual.Artifacts[index];
            if (!string.Equals(expectedArtifact.Path, actualArtifact.Path, pathComparison)
                || expectedArtifact.Role != actualArtifact.Role
                || !string.Equals(expectedArtifact.Language, actualArtifact.Language, StringComparison.OrdinalIgnoreCase)
                || !expectedArtifact.Fingerprint.Equals(actualArtifact.Fingerprint)
                || !Equals(expectedArtifact.FileIdentity, actualArtifact.FileIdentity))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>Binds a Mutagen begin-edit result to the caller's operation and unchanged source revision.</summary>
    /// <param name="sources">The borrowed source lifetime supplying workspace metadata.</param>
    /// <param name="request">The guarded begin-edit request.</param>
    /// <param name="result">The game-specific begin-edit result.</param>
    /// <returns>An equivalent result with exact operation and revision metadata.</returns>
    private static EngineResult<RecordEditIdentity> BindEditResult(
        SkyrimPluginSourceSet sources,
        BeginEditRequest request,
        EngineResult<RecordEditIdentity> result)
    {
        return result.Succeeded
            ? EngineResult<RecordEditIdentity>.Success(
                result.Value!,
                workspaceId: sources.WorkspaceId,
                operationId: request.OperationId,
                baseRevision: request.ExpectedRevision,
                resultRevision: request.ExpectedRevision,
                warnings: result.Warnings)
            : EngineResult<RecordEditIdentity>.Failure(
                result.Error!,
                workspaceId: sources.WorkspaceId,
                operationId: request.OperationId,
                baseRevision: request.ExpectedRevision,
                resultRevision: request.ExpectedRevision,
                warnings: result.Warnings);
    }

    /// <summary>Creates a source-bound plugin output failure.</summary>
    /// <param name="sources">The borrowed source lifetime.</param>
    /// <param name="operationId">The selection operation identifier, when applicable.</param>
    /// <param name="baseRevision">The source revision associated with the operation.</param>
    /// <param name="code">The stable engine failure category.</param>
    /// <param name="message">The failure description.</param>
    /// <returns>The failed output-open result.</returns>
    private static EngineResult<PluginOutputOpenResult> OutputFailure(
        SkyrimPluginSourceSet sources,
        Guid? operationId,
        WorkspaceRevision baseRevision,
        EngineErrorCode code,
        string message)
    {
        return EngineResult<PluginOutputOpenResult>.Failure(
            new EngineError(code, message),
            workspaceId: sources.WorkspaceId,
            operationId: operationId,
            baseRevision: baseRevision,
            resultRevision: baseRevision);
    }
}
