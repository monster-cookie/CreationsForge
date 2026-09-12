using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeOutputs;
using CreationsForge.Fallout4.Native.NativeInspection;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Records;
using NativeMasterStyle = Mutagen.Bethesda.Plugins.MasterStyle;

namespace CreationsForge.Fallout4.Native;

/// <summary>
/// Opens, clones, and begins transactional edits against complete Fallout 4 native output state.
/// </summary>
public sealed class Fallout4NativeOutputService
{
    /// <summary>The shared boundary that admits and baselines explicit output artifacts.</summary>
    private readonly NativeOutputInputLoader _inputLoader;

    /// <summary>Initializes a Fallout 4 native output service.</summary>
    /// <param name="inputLoader">The shared output admission and baseline service.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="inputLoader"/> is <see langword="null"/>.</exception>
    public Fallout4NativeOutputService(NativeOutputInputLoader inputLoader)
    {
        ArgumentNullException.ThrowIfNull(inputLoader);
        _inputLoader = inputLoader;
        Inspector = new Fallout4FormListNativeInspector();
    }

    /// <summary>Gets the stateless complete Fallout 4 FormList inspector used for detached snapshots.</summary>
    public Fallout4FormListNativeInspector Inspector { get; }

    /// <summary>Opens an existing Fallout 4 output or creates a complete in-memory output without writing files.</summary>
    /// <param name="sources">The borrowed Fallout 4 source lifetime.</param>
    /// <param name="request">The guarded output selection and expected source revision.</param>
    /// <param name="cancellationToken">A token observed during admission, parsing, copying, and verification.</param>
    /// <returns>The independently owned complete output state and exact artifact baseline, or a typed failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public async Task<EngineResult<NativeOutputOpenResult>> OpenAsync(
        Fallout4NativeSourceSet sources,
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

    /// <summary>Reopens an existing Fallout 4 output only when its complete artifact baseline still matches.</summary>
    /// <param name="sources">The borrowed Fallout 4 source lifetime.</param>
    /// <param name="association">The exact selected output identity and formatting choices.</param>
    /// <param name="expectedBaseline">The complete plugin-and-strings baseline that must still match.</param>
    /// <param name="cancellationToken">A token observed during admission, parsing, copying, and verification.</param>
    /// <returns>The independently owned reopened state or a typed external-change or native-open failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public Task<EngineResult<NativeOutputOpenResult>> ReopenAsync(
        Fallout4NativeSourceSet sources,
        OutputAssociation association,
        OutputArtifactSetBaseline expectedBaseline,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sources);
        ArgumentNullException.ThrowIfNull(association);
        ArgumentNullException.ThrowIfNull(expectedBaseline);
        var pluginArtifact = expectedBaseline.Artifacts.SingleOrDefault(
            artifact => artifact.Role == NativeArtifactRole.Plugin);
        if (pluginArtifact is null)
        {
            return Task.FromResult(OutputFailure(
                sources,
                operationId: null,
                sources.Revision,
                EngineErrorCode.InvalidRequest,
                "The expected Fallout 4 output baseline does not contain exactly one plugin artifact."));
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

    /// <summary>Creates an independently owned complete native copy of one Fallout 4 output state.</summary>
    /// <param name="output">The complete output state to clone.</param>
    /// <param name="cancellationToken">A token checked immediately before and after both native copies.</param>
    /// <returns>A complete independent candidate retaining the original output baseline.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="output"/> is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown after <paramref name="output"/> is disposed.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public Fallout4NativeOutputState Clone(
        Fallout4NativeOutputState output,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(output);
        cancellationToken.ThrowIfCancellationRequested();
        var mod = CopyMod(output.BorrowMod());
        cancellationToken.ThrowIfCancellationRequested();
        var originalMod = CopyMod(output.BorrowOriginalMod());
        cancellationToken.ThrowIfCancellationRequested();
        var editProvenance = output.GetEditProvenance();
        cancellationToken.ThrowIfCancellationRequested();
        return new Fallout4NativeOutputState(mod, originalMod, output.Association, output.Baseline, editProvenance);
    }

    /// <summary>Allocates, overrides, or selects one FormList only inside an unpublished Fallout 4 candidate.</summary>
    /// <param name="sources">The borrowed source lifetime used to select an override origin.</param>
    /// <param name="candidate">The unpublished complete output candidate to mutate.</param>
    /// <param name="request">The new, override, or existing-output edit request.</param>
    /// <param name="cancellationToken">A token checked around native scans, copies, and allocation.</param>
    /// <returns>The stable native edit identity or a typed selection, family, deletion, or collision failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public EngineResult<NativeEditIdentity> BeginEdit(
        Fallout4NativeSourceSet sources,
        Fallout4NativeOutputState candidate,
        BeginEditRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(sources);
        ArgumentNullException.ThrowIfNull(candidate);
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();
        Fallout4Mod mod;
        try
        {
            mod = candidate.BorrowMod();
        }
        catch (ObjectDisposedException exception)
        {
            return EditFailure(sources, request, EngineErrorCode.WorkspaceDisposed, exception.Message);
        }

        if (mod.ModKey != candidate.Association.ModKey)
        {
            return EditFailure(
                sources,
                request,
                EngineErrorCode.ValidationFailed,
                $"The Fallout 4 candidate identity {mod.ModKey} does not match selected output {candidate.Association.ModKey}.");
        }

        return request.Role switch
        {
            FormListEditRole.New => BeginNewEdit(sources, candidate, mod, request, cancellationToken),
            FormListEditRole.Override => BeginOverrideEdit(sources, candidate, mod, request, cancellationToken),
            FormListEditRole.ExistingOutput => BeginExistingOutputEdit(sources, candidate, mod, request, cancellationToken),
            _ => EditFailure(
                sources,
                request,
                EngineErrorCode.InvalidRequest,
                $"The Fallout 4 FormList edit role '{request.Role}' is undefined.")
        };
    }

    /// <summary>Admits, fully materializes, snapshots, and baselines one output without publishing partial state.</summary>
    /// <param name="sources">The borrowed Fallout 4 sources.</param>
    /// <param name="association">The requested output association.</param>
    /// <param name="mode">Whether the output must be absent or present.</param>
    /// <param name="expectedBaseline">The exact prior baseline required by reopen, or <see langword="null"/> for initial selection.</param>
    /// <param name="operationId">The selection operation identifier, when applicable.</param>
    /// <param name="baseRevision">The source revision associated with the operation.</param>
    /// <param name="cancellationToken">A token observed throughout output acquisition.</param>
    /// <returns>An independently owned native output state or a typed failure.</returns>
    private async Task<EngineResult<NativeOutputOpenResult>> OpenCoreAsync(
        Fallout4NativeSourceSet sources,
        OutputAssociation association,
        OutputSelectionMode mode,
        OutputArtifactSetBaseline? expectedBaseline,
        Guid? operationId,
        WorkspaceRevision baseRevision,
        CancellationToken cancellationToken)
    {
        EngineResult<NativeOutputInputs> preparation;
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
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
        catch (ObjectDisposedException exception)
        {
            return OutputFailure(
                sources,
                operationId,
                baseRevision,
                EngineErrorCode.WorkspaceDisposed,
                exception.Message);
        }

        if (!preparation.Succeeded)
        {
            return OutputFailure(
                sources,
                operationId,
                baseRevision,
                preparation.Error!,
                preparation.Warnings);
        }

        await using var inputs = preparation.Value!;
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (inputs.Release != GameRelease.Fallout4)
            {
                return OutputFailure(
                    sources,
                    operationId,
                    baseRevision,
                    EngineErrorCode.UnsupportedGameRelease,
                    $"Fallout 4 outputs require {GameRelease.Fallout4}; received {inputs.Release}.");
            }

            var modResult = CreateOrReadMod(inputs, cancellationToken);
            if (!modResult.Succeeded)
            {
                return OutputFailure(
                    sources,
                    operationId,
                    baseRevision,
                    modResult.Error!,
                    modResult.Warnings);
            }

            var mod = modResult.Value!;
            cancellationToken.ThrowIfCancellationRequested();
            var originalMod = CopyMod(mod);
            cancellationToken.ThrowIfCancellationRequested();
            var baselineResult = await inputs.CompleteOpenAsync(cancellationToken).ConfigureAwait(false);
            if (!baselineResult.Succeeded)
            {
                return OutputFailure(
                    sources,
                    operationId,
                    baseRevision,
                    baselineResult.Error!,
                    baselineResult.Warnings);
            }

            var baseline = baselineResult.Value!;
            if (expectedBaseline is not null && !BaselinesMatch(expectedBaseline, baseline))
            {
                return OutputFailure(
                    sources,
                    operationId,
                    baseRevision,
                    EngineErrorCode.ExternalChangeDetected,
                    "The Fallout 4 output artifact set no longer matches the expected reopen baseline.");
            }

            var canonicalAssociation = new OutputAssociation(
                inputs.Output.Path,
                inputs.Output.ModKey,
                association.LocalizedOutputMode,
                association.MasterStyle);
            var state = new Fallout4NativeOutputState(mod, originalMod, canonicalAssociation, baseline);
            return EngineResult<NativeOutputOpenResult>.Success(
                new NativeOutputOpenResult(state, canonicalAssociation, baseline),
                workspaceId: sources.WorkspaceId,
                operationId: operationId,
                baseRevision: baseRevision,
                resultRevision: baseRevision,
                warnings: preparation.Warnings
                    .Concat(modResult.Warnings)
                    .Concat(baselineResult.Warnings)
                    .ToArray());
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
                $"Fallout 4 native output materialization failed: {exception.Message}");
        }
    }

    /// <summary>Creates an absent output in memory or strictly parses a complete existing output.</summary>
    /// <param name="inputs">The admitted short-lived native output scope.</param>
    /// <param name="cancellationToken">A token checked around uninterruptible native construction and parsing.</param>
    /// <returns>The complete mutable native mod or a typed header validation failure.</returns>
    private static EngineResult<Fallout4Mod> CreateOrReadMod(
        NativeOutputInputs inputs,
        CancellationToken cancellationToken)
    {
        if (inputs.Output.MasterStyle == NativeMasterStyle.Medium)
        {
            return EngineResult<Fallout4Mod>.Failure(new EngineError(
                EngineErrorCode.UnsupportedInput,
                $"Fallout 4 output '{inputs.Output.ModKey.FileName}' cannot use medium master style."));
        }

        cancellationToken.ThrowIfCancellationRequested();
        Fallout4Mod mod;
        if (inputs.Output.Exists)
        {
            using MutagenBinaryReadStream stream = inputs.OpenReadStream(cancellationToken);
            mod = Fallout4Mod.CreateFromBinary(
                new MutagenFrame(stream),
                Fallout4Release.Fallout4,
                new GroupMask(true));
        }
        else
        {
            mod = new Fallout4Mod(inputs.Output.ModKey, Fallout4Release.Fallout4)
            {
                IsMaster = inputs.Output.ModKey.Type == ModType.Master,
                IsSmallMaster = inputs.Output.MasterStyle == NativeMasterStyle.Small,
                UsingLocalization = inputs.Output.UsesLocalization,
            };
        }

        cancellationToken.ThrowIfCancellationRequested();
        var expectedMasterFlag = inputs.Output.ModKey.Type == ModType.Master;
        var expectedSmallFlag = inputs.Output.MasterStyle == NativeMasterStyle.Small;
        if (mod.ModKey != inputs.Output.ModKey
            || mod.IsMaster != expectedMasterFlag
            || mod.IsSmallMaster != expectedSmallFlag
            || mod.IsMediumMaster
            || mod.UsingLocalization != inputs.Output.UsesLocalization)
        {
            return EngineResult<Fallout4Mod>.Failure(new EngineError(
                EngineErrorCode.OutputOpenFailed,
                $"Fallout 4 output '{inputs.Output.ModKey.FileName}' native identity or header flags do not match the admitted output."));
        }

        return EngineResult<Fallout4Mod>.Success(mod);
    }

    /// <summary>Allocates a native FormList and rejects any resulting cross-family FormKey collision.</summary>
    /// <param name="sources">The borrowed sources used for result metadata.</param>
    /// <param name="candidate">The unpublished output state that owns first-edit provenance.</param>
    /// <param name="mod">The unpublished complete output candidate.</param>
    /// <param name="request">The validated new-record request.</param>
    /// <param name="cancellationToken">A token checked around native allocation and collision scanning.</param>
    /// <returns>The allocated edit identity or a typed collision failure.</returns>
    private static EngineResult<NativeEditIdentity> BeginNewEdit(
        Fallout4NativeSourceSet sources,
        Fallout4NativeOutputState candidate,
        Fallout4Mod mod,
        BeginEditRequest request,
        CancellationToken cancellationToken)
    {
        var existingKeys = new HashSet<FormKey>();
        foreach (var record in mod.EnumerateMajorRecords())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!existingKeys.Add(record.FormKey))
            {
                return EditFailure(
                    sources,
                    request,
                    EngineErrorCode.ValidationFailed,
                    $"Fallout 4 output identity '{record.FormKey}' is duplicated across native record families.");
            }
        }

        cancellationToken.ThrowIfCancellationRequested();
        var formList = mod.FormLists.AddNew();
        cancellationToken.ThrowIfCancellationRequested();
        var matches = FindRecords(mod, formList.FormKey, cancellationToken);
        if (formList.FormKey.ModKey != mod.ModKey
            || existingKeys.Contains(formList.FormKey)
            || matches.Count != 1
            || !ReferenceEquals(matches[0], formList))
        {
            return EditFailure(
                sources,
                request,
                EngineErrorCode.ValidationFailed,
                $"Native Fallout 4 FormList allocation produced duplicate or invalid identity '{formList.FormKey}'.");
        }

        candidate.TryAddEditProvenance(new NativeEditProvenance(
            request.OperationId,
            formList.FormKey,
            EditBaselineKind.Absent));
        return EditSuccess(sources, request, formList.FormKey, originFormKey: null);
    }

    /// <summary>Selects an exact live native source FormList and creates or reuses its output override.</summary>
    /// <param name="sources">The source lifetime used for exact contextual selection.</param>
    /// <param name="candidate">The unpublished output state that owns first-edit provenance.</param>
    /// <param name="mod">The unpublished complete output candidate.</param>
    /// <param name="request">The validated override request.</param>
    /// <param name="cancellationToken">A token observed during source selection, copying, and collision checks.</param>
    /// <returns>The override edit identity or a typed source-selection failure.</returns>
    private static EngineResult<NativeEditIdentity> BeginOverrideEdit(
        Fallout4NativeSourceSet sources,
        Fallout4NativeOutputState candidate,
        Fallout4Mod mod,
        BeginEditRequest request,
        CancellationToken cancellationToken)
    {
        var originFormKey = request.OriginFormKey!.Value;
        var selection = request.OriginSelection
            ?? new ReferenceRequest(originFormKey, RecordScope.WinningOverrides);
        var readResult = sources.ReadFormListContext(selection, cancellationToken);
        if (!readResult.Succeeded)
        {
            return EditFailure(sources, request, readResult.Error!, readResult.Warnings);
        }

        var read = readResult.Value!;
        if (read.Context.Status == ReferenceResolutionStatus.Deleted)
        {
            return EditFailure(
                sources,
                request,
                EngineErrorCode.ValidationFailed,
                $"Fallout 4 FormList override origin '{originFormKey}' is deleted in the selected native context.",
                readResult.Warnings);
        }

        if (read.Context.Status is ReferenceResolutionStatus.Unsupported or ReferenceResolutionStatus.UnknownFamily
            || read.Record is not IFormListGetter sourceFormList)
        {
            var code = read.Context.Status == ReferenceResolutionStatus.Unresolved
                ? EngineErrorCode.RecordNotFound
                : EngineErrorCode.UnsupportedOperation;
            return EditFailure(
                sources,
                request,
                code,
                $"Fallout 4 override origin '{originFormKey}' did not resolve to a supported live FormList; status was {read.Context.Status}.",
                readResult.Warnings);
        }

        var existingMatches = FindRecords(mod, originFormKey, cancellationToken);
        if (existingMatches.Count > 0
            && (existingMatches.Count != 1 || existingMatches[0] is not IFormListGetter))
        {
            return DuplicateOrWrongFamilyFailure(sources, request, originFormKey, existingMatches.Count);
        }

        cancellationToken.ThrowIfCancellationRequested();
        var formList = mod.FormLists.GetOrAddAsOverride(sourceFormList);
        cancellationToken.ThrowIfCancellationRequested();
        var matches = FindRecords(mod, originFormKey, cancellationToken);
        if (matches.Count != 1 || !ReferenceEquals(matches[0], formList))
        {
            return DuplicateOrWrongFamilyFailure(sources, request, originFormKey, matches.Count);
        }

        var provenanceError = TryCaptureExistingProvenance(
            sources,
            candidate,
            request,
            formList.FormKey,
            read.Context);
        if (provenanceError is not null)
        {
            return EditFailure(sources, request, provenanceError, readResult.Warnings);
        }

        return EditSuccess(sources, request, formList.FormKey, originFormKey, readResult.Warnings);
    }

    /// <summary>Selects exactly one FormList already contained in the output without changing its fields.</summary>
    /// <param name="sources">The borrowed sources used for result metadata.</param>
    /// <param name="candidate">The unpublished output state that owns first-edit provenance.</param>
    /// <param name="mod">The unpublished complete output candidate.</param>
    /// <param name="request">The validated existing-output request.</param>
    /// <param name="cancellationToken">A token observed while scanning all native record families.</param>
    /// <returns>The existing-output edit identity or a typed absence, family, or collision failure.</returns>
    private static EngineResult<NativeEditIdentity> BeginExistingOutputEdit(
        Fallout4NativeSourceSet sources,
        Fallout4NativeOutputState candidate,
        Fallout4Mod mod,
        BeginEditRequest request,
        CancellationToken cancellationToken)
    {
        var targetFormKey = request.TargetFormKey!.Value;
        var matches = FindRecords(mod, targetFormKey, cancellationToken);
        if (matches.Count == 0)
        {
            return EditFailure(
                sources,
                request,
                EngineErrorCode.RecordNotFound,
                $"Fallout 4 output FormList '{targetFormKey}' was not found in the selected output.");
        }

        if (matches.Count != 1 || matches[0] is not IFormListGetter)
        {
            return DuplicateOrWrongFamilyFailure(sources, request, targetFormKey, matches.Count);
        }

        var provenanceError = TryCaptureExistingProvenance(
            sources,
            candidate,
            request,
            targetFormKey,
            sourceContext: null);
        if (provenanceError is not null)
        {
            return EditFailure(sources, request, provenanceError);
        }

        return EditSuccess(sources, request, targetFormKey, originFormKey: null);
    }

    /// <summary>Captures the target's exact first baseline from original output or one exact immutable source context.</summary>
    /// <param name="sources">The borrowed source lifetime that supplies source identity and output load-order position.</param>
    /// <param name="candidate">The unpublished output candidate that owns provenance.</param>
    /// <param name="request">The begin-edit request whose identifier becomes the first edit identity.</param>
    /// <param name="targetFormKey">The selected output FormList identity.</param>
    /// <param name="sourceContext">The exact selected source context for an override, or <see langword="null"/> for existing output selection.</param>
    /// <returns>A deterministic provenance failure, or <see langword="null"/> after provenance is present.</returns>
    private static EngineError? TryCaptureExistingProvenance(
        Fallout4NativeSourceSet sources,
        Fallout4NativeOutputState candidate,
        BeginEditRequest request,
        FormKey targetFormKey,
        FormListContext? sourceContext)
    {
        if (candidate.GetEditProvenance().Any(existing => existing.TargetFormKey == targetFormKey))
        {
            return null;
        }

        var originalMatches = candidate.BorrowOriginalMod()
            .EnumerateMajorRecords()
            .Where(record => record.FormKey == targetFormKey)
            .ToArray();
        if (originalMatches.Length == 1 && originalMatches[0] is IFormListGetter originalFormList)
        {
            var selection = new ReferenceRequest(
                targetFormKey,
                RecordScope.StagedOutput,
                candidate.Association.ModKey);
            var context = new FormListContext(
                selection,
                originalFormList.IsDeleted
                    ? ReferenceResolutionStatus.Deleted
                    : ReferenceResolutionStatus.Resolved,
                candidate.Association.ModKey,
                candidate.Association.PluginPath,
                sources.GetNativeMods().Count,
                PluginRole.Output);
            candidate.TryAddEditProvenance(new NativeEditProvenance(
                request.OperationId,
                targetFormKey,
                EditBaselineKind.OriginalOutput,
                context));
            return null;
        }

        if (originalMatches.Length != 0)
        {
            return new EngineError(
                EngineErrorCode.ValidationFailed,
                $"Fallout 4 output baseline identity '{targetFormKey}' is duplicated or belongs to another native record family.");
        }

        if (sourceContext is null || !sourceContext.ContainingModKey.HasValue)
        {
            return new EngineError(
                EngineErrorCode.ValidationFailed,
                $"Fallout 4 FormList '{targetFormKey}' has no exact original-output or source baseline for preview.");
        }

        var exactSourceContext = new FormListContext(
            new ReferenceRequest(targetFormKey, RecordScope.AllContexts, sourceContext.ContainingModKey),
            sourceContext.Status,
            sourceContext.ContainingModKey,
            sourceContext.Path,
            sourceContext.LoadOrderIndex,
            sourceContext.Role);
        candidate.TryAddEditProvenance(new NativeEditProvenance(
            request.OperationId,
            targetFormKey,
            EditBaselineKind.SourceContext,
            exactSourceContext,
            sources.Baseline.BaselineId));
        return null;
    }

    /// <summary>Enumerates the complete output to find every native record sharing one FormKey.</summary>
    /// <param name="mod">The complete output candidate.</param>
    /// <param name="formKey">The native identity to find across every record family.</param>
    /// <param name="cancellationToken">A token observed throughout native enumeration.</param>
    /// <returns>All matching live record objects in native enumeration order.</returns>
    private static IReadOnlyList<IMajorRecordGetter> FindRecords(
        Fallout4Mod mod,
        FormKey formKey,
        CancellationToken cancellationToken)
    {
        var matches = new List<IMajorRecordGetter>();
        foreach (var record in mod.EnumerateMajorRecords())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (record.FormKey == formKey)
            {
                matches.Add(record);
            }
        }

        return Array.AsReadOnly(matches.ToArray());
    }

    /// <summary>Creates a typed collision or wrong-family begin-edit failure.</summary>
    /// <param name="sources">The borrowed sources used for result metadata.</param>
    /// <param name="request">The guarded begin-edit request.</param>
    /// <param name="formKey">The conflicting native identity.</param>
    /// <param name="matchCount">The number of output records sharing the identity.</param>
    /// <returns>A typed failed edit result.</returns>
    private static EngineResult<NativeEditIdentity> DuplicateOrWrongFamilyFailure(
        Fallout4NativeSourceSet sources,
        BeginEditRequest request,
        FormKey formKey,
        int matchCount)
    {
        var message = matchCount > 1
            ? $"Fallout 4 output identity '{formKey}' is duplicated across native record families."
            : $"Fallout 4 output identity '{formKey}' belongs to a native record family other than FormList.";
        return EditFailure(sources, request, EngineErrorCode.ValidationFailed, message);
    }

    /// <summary>Creates a successful begin-edit result with unchanged workspace revision metadata.</summary>
    /// <param name="sources">The borrowed sources used for result metadata.</param>
    /// <param name="request">The guarded begin-edit request.</param>
    /// <param name="formKey">The allocated, overridden, or selected native output identity.</param>
    /// <param name="originFormKey">The source origin for an override, otherwise <see langword="null"/>.</param>
    /// <param name="warnings">Optional native source warnings retained from origin selection.</param>
    /// <returns>A successful native edit identity result.</returns>
    private static EngineResult<NativeEditIdentity> EditSuccess(
        Fallout4NativeSourceSet sources,
        BeginEditRequest request,
        FormKey formKey,
        FormKey? originFormKey,
        IReadOnlyList<EngineWarning>? warnings = null)
    {
        return EngineResult<NativeEditIdentity>.Success(
            new NativeEditIdentity(request.OperationId, formKey, originFormKey, request.Role),
            workspaceId: sources.WorkspaceId,
            operationId: request.OperationId,
            baseRevision: request.ExpectedRevision,
            resultRevision: request.ExpectedRevision,
            warnings: warnings);
    }

    /// <summary>Creates a typed begin-edit failure with stable operation and revision metadata.</summary>
    /// <param name="sources">The borrowed sources used for result metadata.</param>
    /// <param name="request">The guarded begin-edit request.</param>
    /// <param name="code">The stable failure category.</param>
    /// <param name="message">The actionable failure detail.</param>
    /// <param name="warnings">Optional native source warnings retained from origin selection.</param>
    /// <returns>A failed native edit identity result.</returns>
    private static EngineResult<NativeEditIdentity> EditFailure(
        Fallout4NativeSourceSet sources,
        BeginEditRequest request,
        EngineErrorCode code,
        string message,
        IReadOnlyList<EngineWarning>? warnings = null)
    {
        return EditFailure(sources, request, new EngineError(code, message), warnings);
    }

    /// <summary>Propagates a typed begin-edit failure with stable operation and revision metadata.</summary>
    /// <param name="sources">The borrowed sources used for result metadata.</param>
    /// <param name="request">The guarded begin-edit request.</param>
    /// <param name="error">The typed failure to propagate.</param>
    /// <param name="warnings">Optional native source warnings retained from origin selection.</param>
    /// <returns>A failed native edit identity result.</returns>
    private static EngineResult<NativeEditIdentity> EditFailure(
        Fallout4NativeSourceSet sources,
        BeginEditRequest request,
        EngineError error,
        IReadOnlyList<EngineWarning>? warnings = null)
    {
        return EngineResult<NativeEditIdentity>.Failure(
            error,
            workspaceId: sources.WorkspaceId,
            operationId: request.OperationId,
            baseRevision: request.ExpectedRevision,
            resultRevision: request.ExpectedRevision,
            warnings: warnings);
    }

    /// <summary>Creates a failed output-open result from a stable error category and message.</summary>
    /// <param name="sources">The borrowed sources used for result metadata.</param>
    /// <param name="operationId">The selection operation identifier, when applicable.</param>
    /// <param name="baseRevision">The source revision associated with the operation.</param>
    /// <param name="code">The stable failure category.</param>
    /// <param name="message">The actionable failure detail.</param>
    /// <returns>A failed native output result.</returns>
    private static EngineResult<NativeOutputOpenResult> OutputFailure(
        Fallout4NativeSourceSet sources,
        Guid? operationId,
        WorkspaceRevision baseRevision,
        EngineErrorCode code,
        string message)
    {
        return OutputFailure(
            sources,
            operationId,
            baseRevision,
            new EngineError(code, message),
            warnings: null);
    }

    /// <summary>Propagates a typed output-open failure with stable workspace and revision metadata.</summary>
    /// <param name="sources">The borrowed sources used for result metadata.</param>
    /// <param name="operationId">The selection operation identifier, when applicable.</param>
    /// <param name="baseRevision">The source revision associated with the operation.</param>
    /// <param name="error">The typed failure to propagate.</param>
    /// <param name="warnings">Optional warnings produced during output admission or verification.</param>
    /// <returns>A failed native output result.</returns>
    private static EngineResult<NativeOutputOpenResult> OutputFailure(
        Fallout4NativeSourceSet sources,
        Guid? operationId,
        WorkspaceRevision baseRevision,
        EngineError error,
        IReadOnlyList<EngineWarning>? warnings)
    {
        return EngineResult<NativeOutputOpenResult>.Failure(
            error,
            workspaceId: sources.WorkspaceId,
            operationId: operationId,
            baseRevision: baseRevision,
            resultRevision: baseRevision,
            warnings: warnings);
    }

    /// <summary>Creates a complete mutable native copy without retaining the source object.</summary>
    /// <param name="mod">The complete native getter to copy.</param>
    /// <returns>An independent complete mutable Fallout 4 mod.</returns>
    private static Fallout4Mod CopyMod(IFallout4ModGetter mod)
    {
        return (Fallout4Mod)((IModGetter)mod).DeepCopy();
    }

    /// <summary>Compares complete ordered output baselines including canonical paths, content, and physical identity.</summary>
    /// <param name="expected">The prior complete output baseline supplied for reopen.</param>
    /// <param name="actual">The newly observed complete output baseline.</param>
    /// <returns><see langword="true"/> when the identifier and every artifact property match exactly.</returns>
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
                || !string.Equals(expectedArtifact.Language, actualArtifact.Language, StringComparison.Ordinal)
                || !expectedArtifact.Fingerprint.Equals(actualArtifact.Fingerprint)
                || !Equals(expectedArtifact.FileIdentity, actualArtifact.FileIdentity))
            {
                return false;
            }
        }

        return true;
    }
}
