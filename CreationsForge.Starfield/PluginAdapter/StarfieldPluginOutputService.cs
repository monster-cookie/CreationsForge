using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.PluginOutputs;
using CreationsForge.Core.Engine.RecordInspection;
using CreationsForge.Starfield.PluginAdapter.RecordInspection;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Starfield;

namespace CreationsForge.Starfield.PluginAdapter;

/// <summary>
/// Admits, clones, and begins transactional edits against complete Starfield plugin output state.
/// </summary>
public sealed class StarfieldPluginOutputService
{
    /// <summary>The shared guarded output admission boundary.</summary>
    private readonly PluginOutputInputLoader InputLoader;

    /// <summary>
    /// Initializes the Starfield plugin output service.
    /// </summary>
    /// <param name="inputLoader">The shared output admission and artifact-verification boundary.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="inputLoader"/> is <see langword="null"/>.</exception>
    public StarfieldPluginOutputService(PluginOutputInputLoader inputLoader)
    {
        ArgumentNullException.ThrowIfNull(inputLoader);
        InputLoader = inputLoader;
        Inspector = new StarfieldFormListInspector();
        MajorRecordInspector = new MutagenMajorRecordInspector(
            typeof(StarfieldMajorRecord),
            "Mutagen.Bethesda.Starfield/0.55.0-alpha.53");
    }

    /// <summary>Gets the stateless complete Starfield FormList inspector used for detached snapshots.</summary>
    public StarfieldFormListInspector Inspector { get; }

    /// <summary>Gets the complete native Starfield major-record inspector shared by read and save verification.</summary>
    public IMajorRecordInspector MajorRecordInspector { get; }

    /// <summary>
    /// Creates or opens a separate complete Starfield output after guarded source and artifact admission.
    /// </summary>
    /// <param name="sources">The borrowed Starfield source lifetime.</param>
    /// <param name="request">The exact output selection and formatting request.</param>
    /// <param name="cancellationToken">A token that cancels admission or Mutagen parsing before publication.</param>
    /// <returns>An independently owned complete output state and immutable artifact baseline, or a typed failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public Task<EngineResult<PluginOutputOpenResult>> OpenAsync(
        StarfieldPluginSourceSet sources,
        SelectOutputRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sources);
        ArgumentNullException.ThrowIfNull(request);
        return OpenCoreAsync(sources, request.Output, request.Mode, null, cancellationToken);
    }

    /// <summary>
    /// Reopens a complete Starfield output only while its exact prior artifact baseline remains current.
    /// </summary>
    /// <param name="sources">The borrowed Starfield source lifetime.</param>
    /// <param name="association">The exact selected output identity and formatting choices.</param>
    /// <param name="expectedBaseline">The complete output artifact observation that must still match.</param>
    /// <param name="cancellationToken">A token that cancels admission or Mutagen parsing before publication.</param>
    /// <returns>An independently owned reopened output state, or a typed external-change or open failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public Task<EngineResult<PluginOutputOpenResult>> ReopenAsync(
        StarfieldPluginSourceSet sources,
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
            return Task.FromResult(EngineResult<PluginOutputOpenResult>.Failure(new EngineError(
                EngineErrorCode.InvalidRequest,
                "The expected Starfield output baseline does not contain exactly one plugin artifact.")));
        }

        var mode = pluginArtifact.Fingerprint.Exists
            ? OutputSelectionMode.OpenExisting
            : OutputSelectionMode.CreateNew;
        return OpenCoreAsync(sources, association, mode, expectedBaseline, cancellationToken);
    }

    /// <summary>
    /// Creates an independently disposable deep Mutagen copy of complete staged and original Starfield output state.
    /// </summary>
    /// <param name="output">The current complete Starfield output.</param>
    /// <param name="cancellationToken">A token checked immediately before and after each uninterruptible Mutagen copy.</param>
    /// <returns>A complete transactional candidate preserving every edited and unrelated record.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="output"/> is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown after <paramref name="output"/> is disposed.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public StarfieldPluginOutputState Clone(
        StarfieldPluginOutputState output,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(output);
        cancellationToken.ThrowIfCancellationRequested();
        var mod = (StarfieldMod)output.BorrowMod().DeepCopy();
        cancellationToken.ThrowIfCancellationRequested();
        var originalMod = (StarfieldMod)output.BorrowOriginalMod().DeepCopy();
        cancellationToken.ThrowIfCancellationRequested();
        var provenance = output.BorrowProvenance().ToArray();
        cancellationToken.ThrowIfCancellationRequested();
        return new StarfieldPluginOutputState(mod, originalMod, output.Association, output.Baseline, provenance);
    }

    /// <summary>
    /// Allocates, overrides, or selects a FormList only inside an unpublished complete Starfield output candidate.
    /// </summary>
    /// <param name="sources">The borrowed plugin source lifetime used to resolve override origins.</param>
    /// <param name="candidate">The unpublished complete output candidate to mutate.</param>
    /// <param name="request">The guarded record edit-target request.</param>
    /// <param name="cancellationToken">A token checked throughout selection and around Mutagen mutation.</param>
    /// <returns>The newly assigned stable edit identity, or a typed selection or validation failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown after a borrowed plugin lifetime is disposed.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public EngineResult<RecordEditIdentity> BeginEdit(
        StarfieldPluginSourceSet sources,
        StarfieldPluginOutputState candidate,
        BeginEditRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sources);
        ArgumentNullException.ThrowIfNull(candidate);
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        return request.Role switch
        {
            FormListEditRole.New => BeginNew(sources, candidate, cancellationToken),
            FormListEditRole.Override => BeginOverride(sources, candidate, request, cancellationToken),
            FormListEditRole.ExistingOutput => BeginExistingOutput(sources, candidate, request, cancellationToken),
            _ => EngineResult<RecordEditIdentity>.Failure(new EngineError(
                EngineErrorCode.InvalidRequest,
                $"Unknown Starfield FormList edit role '{request.Role}'."))
        };
    }

    /// <summary>Admits, fully materializes, and verifies one new or existing output without retaining parsing resources.</summary>
    /// <param name="sources">The borrowed Starfield source lifetime.</param>
    /// <param name="association">The requested output identity and Mutagen formatting choices.</param>
    /// <param name="mode">Whether the output must be absent or present.</param>
    /// <param name="expectedBaseline">The exact prior baseline required for a reopen, or <see langword="null"/> for initial selection.</param>
    /// <param name="cancellationToken">A token observed until a complete Mutagen state is ready to publish.</param>
    /// <returns>An independently owned output state, or a typed failure.</returns>
    private async Task<EngineResult<PluginOutputOpenResult>> OpenCoreAsync(
        StarfieldPluginSourceSet sources,
        OutputAssociation association,
        OutputSelectionMode mode,
        OutputArtifactSetBaseline? expectedBaseline,
        CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var inputResult = await InputLoader
                .PrepareAsync(sources.BorrowInputs(), association, mode, cancellationToken)
                .ConfigureAwait(false);
            if (!inputResult.Succeeded || inputResult.Value is null)
            {
                return EngineResult<PluginOutputOpenResult>.Failure(
                    inputResult.Error ?? new EngineError(EngineErrorCode.OutputOpenFailed, "The Starfield output could not be admitted."),
                    warnings: inputResult.Warnings);
            }

            await using var inputs = inputResult.Value;
            if (inputs.Release != GameRelease.Starfield)
            {
                return EngineResult<PluginOutputOpenResult>.Failure(new EngineError(
                    EngineErrorCode.UnsupportedGameRelease,
                    $"Starfield plugin output loading does not support release {inputs.Release}."));
            }

            StarfieldMod mod;
            if (inputs.Output.Exists)
            {
                using var stream = inputs.OpenReadStream(cancellationToken);
                var frame = new MutagenFrame(stream);
                mod = StarfieldMod.CreateFromBinary(
                    frame,
                    StarfieldRelease.Starfield,
                    new GroupMask(true));
            }
            else
            {
                mod = CreateNewMod(inputs.Output);
            }

            cancellationToken.ThrowIfCancellationRequested();
            var validationError = ValidateMaterializedMod(mod, inputs.Output);
            if (validationError is not null)
            {
                return EngineResult<PluginOutputOpenResult>.Failure(validationError);
            }

            var baselineResult = await inputs.CompleteOpenAsync(cancellationToken).ConfigureAwait(false);
            if (!baselineResult.Succeeded || baselineResult.Value is null)
            {
                return EngineResult<PluginOutputOpenResult>.Failure(
                    baselineResult.Error ?? new EngineError(EngineErrorCode.OutputOpenFailed, "The Starfield output baseline could not be established."),
                    warnings: inputResult.Warnings.Concat(baselineResult.Warnings).ToArray());
            }

            var baseline = baselineResult.Value;
            if (expectedBaseline is not null && !ArtifactBaselinesMatch(expectedBaseline, baseline))
            {
                return EngineResult<PluginOutputOpenResult>.Failure(new EngineError(
                    EngineErrorCode.ExternalChangeDetected,
                    "The Starfield output artifact set no longer matches the expected reopen baseline."));
            }

            cancellationToken.ThrowIfCancellationRequested();
            var originalMod = mod.DeepCopy();
            cancellationToken.ThrowIfCancellationRequested();
            var canonicalAssociation = new OutputAssociation(
                inputs.Output.Path,
                inputs.Output.ModKey,
                inputs.Output.UsesLocalization
                    ? LocalizedOutputMode.SeparateStringFiles
                    : LocalizedOutputMode.Embedded,
                MapMasterStyle(inputs.Output.MasterStyle));
            var state = new StarfieldPluginOutputState(mod, originalMod, canonicalAssociation, baseline);
            var result = new PluginOutputOpenResult(state, canonicalAssociation, baseline);
            cancellationToken.ThrowIfCancellationRequested();
            return EngineResult<PluginOutputOpenResult>.Success(
                result,
                warnings: inputResult.Warnings.Concat(baselineResult.Warnings).ToArray());
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (ObjectDisposedException exception)
        {
            return EngineResult<PluginOutputOpenResult>.Failure(new EngineError(
                EngineErrorCode.WorkspaceDisposed,
                $"The Starfield plugin lifetime is unavailable: {exception.Message}"));
        }
        catch (Exception exception)
        {
            return EngineResult<PluginOutputOpenResult>.Failure(new EngineError(
                EngineErrorCode.OutputOpenFailed,
                $"Starfield plugin output parsing failed: {exception.Message}"));
        }
    }

    /// <summary>Creates a new in-memory Starfield plugin with exact requested header flags and no filesystem mutation.</summary>
    /// <param name="output">The admitted absent output descriptor.</param>
    /// <returns>An empty complete mutable Starfield plugin.</returns>
    private static StarfieldMod CreateNewMod(PluginOutputPluginInput output)
    {
        var mod = new StarfieldMod(output.ModKey, StarfieldRelease.Starfield)
        {
            IsMaster = output.ModKey.Type == ModType.Master,
            IsSmallMaster = output.MasterStyle == MasterStyle.Small,
            IsMediumMaster = output.MasterStyle == MasterStyle.Medium,
            UsingLocalization = output.UsesLocalization
        };
        return mod;
    }

    /// <summary>Validates fully materialized record identity, master style, and localization state.</summary>
    /// <param name="mod">The complete parsed or newly constructed output.</param>
    /// <param name="output">The exact admitted output descriptor.</param>
    /// <returns>A typed mismatch error, or <see langword="null"/> when every property matches.</returns>
    private static EngineError? ValidateMaterializedMod(
        IStarfieldModGetter mod,
        PluginOutputPluginInput output)
    {
        if (mod.ModKey != output.ModKey)
        {
            return new EngineError(
                EngineErrorCode.OutputOpenFailed,
                $"Plugin Starfield output identity {mod.ModKey} did not match admitted output {output.ModKey}.");
        }

        var actualStyle = GetMasterStyle(mod);
        if (actualStyle is null || actualStyle.Value != output.MasterStyle)
        {
            return new EngineError(
                EngineErrorCode.OutputOpenFailed,
                $"Plugin Starfield output {output.ModKey} did not retain the admitted {output.MasterStyle} master style.");
        }

        if (mod.UsingLocalization != output.UsesLocalization)
        {
            return new EngineError(
                EngineErrorCode.OutputOpenFailed,
                $"Plugin Starfield output {output.ModKey} did not retain the admitted localization mode.");
        }

        return null;
    }

    /// <summary>Allocates one FormList through Mutagen and rejects a cross-family FormKey collision before publication.</summary>
    /// <param name="sources">The borrowed source lifetime used to position the exact staged-output context.</param>
    /// <param name="candidate">The unpublished complete output candidate.</param>
    /// <param name="cancellationToken">A token observed while inspecting occupied identities and around allocation.</param>
    /// <returns>The new edit identity, or a typed uniqueness failure.</returns>
    private static EngineResult<RecordEditIdentity> BeginNew(
        StarfieldPluginSourceSet sources,
        StarfieldPluginOutputState candidate,
        CancellationToken cancellationToken)
    {
        var mod = candidate.BorrowMod();
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

        if (matchingCount != 1)
        {
            return EngineResult<RecordEditIdentity>.Failure(new EngineError(
                EngineErrorCode.ValidationFailed,
                $"Starfield allocated FormKey {formList.FormKey} is not unique across the complete output."));
        }

        var identity = new RecordEditIdentity(
            Guid.NewGuid(),
            formList.FormKey,
            null,
            FormListEditRole.New);
        candidate.AddProvenanceIfAbsent(new RecordEditProvenance(
            identity.EditId,
            identity.FormKey,
            EditBaselineKind.Absent));
        cancellationToken.ThrowIfCancellationRequested();
        return EngineResult<RecordEditIdentity>.Success(identity);
    }

    /// <summary>Resolves one exact live source FormList and copies or reuses its complete output override.</summary>
    /// <param name="sources">The borrowed complete source view.</param>
    /// <param name="candidate">The unpublished complete output candidate.</param>
    /// <param name="request">The override origin and optional exact context selector.</param>
    /// <param name="cancellationToken">A token observed during source resolution and around Mutagen copying.</param>
    /// <returns>The override edit identity and source-read warnings, or a typed selection failure.</returns>
    private static EngineResult<RecordEditIdentity> BeginOverride(
        StarfieldPluginSourceSet sources,
        StarfieldPluginOutputState candidate,
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
                readResult.Error ?? new EngineError(EngineErrorCode.UnexpectedFailure, "The Starfield override origin could not be read."),
                warnings: readResult.Warnings);
        }

        var read = readResult.Value;
        if (read.Context.Status != ReferenceResolutionStatus.Resolved || read.Record is not IFormListGetter sourceFormList)
        {
            return EngineResult<RecordEditIdentity>.Failure(
                CreateOriginSelectionError(originFormKey, read),
                warnings: readResult.Warnings);
        }

        var mod = candidate.BorrowMod();
        foreach (var record in mod.EnumerateMajorRecords())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (record.FormKey == originFormKey && record is not IFormListGetter)
            {
                return EngineResult<RecordEditIdentity>.Failure(new EngineError(
                    EngineErrorCode.ValidationFailed,
                    $"Starfield output {mod.ModKey} already contains non-FormList record {originFormKey}."),
                    warnings: readResult.Warnings);
            }
        }

        cancellationToken.ThrowIfCancellationRequested();
        var formList = mod.FormLists.GetOrAddAsOverride(sourceFormList);
        cancellationToken.ThrowIfCancellationRequested();
        var identity = new RecordEditIdentity(
            Guid.NewGuid(),
            formList.FormKey,
            originFormKey,
            FormListEditRole.Override);
        candidate.AddProvenanceIfAbsent(CreateProvenance(
            sources,
            candidate,
            identity,
            read.Context));
        cancellationToken.ThrowIfCancellationRequested();
        return EngineResult<RecordEditIdentity>.Success(
            identity,
            warnings: readResult.Warnings);
    }

    /// <summary>Selects one exact FormList already contained in the output without copying, resetting, or undeleting it.</summary>
    /// <param name="sources">The borrowed source lifetime used to position the exact staged-output context.</param>
    /// <param name="candidate">The unpublished complete output candidate.</param>
    /// <param name="request">The exact existing-output target request.</param>
    /// <param name="cancellationToken">A token observed while scanning output records.</param>
    /// <returns>The existing-output edit identity, or a typed missing or wrong-family failure.</returns>
    private static EngineResult<RecordEditIdentity> BeginExistingOutput(
        StarfieldPluginSourceSet sources,
        StarfieldPluginOutputState candidate,
        BeginEditRequest request,
        CancellationToken cancellationToken)
    {
        var targetFormKey = request.TargetFormKey!.Value;
        IFormListGetter? selected = null;
        var wrongFamily = false;
        foreach (var record in candidate.BorrowMod().EnumerateMajorRecords())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (record.FormKey != targetFormKey)
            {
                continue;
            }

            if (record is IFormListGetter formList)
            {
                selected = formList;
            }
            else
            {
                wrongFamily = true;
            }
        }

        if (selected is null)
        {
            var code = wrongFamily ? EngineErrorCode.ValidationFailed : EngineErrorCode.RecordNotFound;
            var detail = wrongFamily ? "belongs to another record family" : "is not contained in the selected output";
            return EngineResult<RecordEditIdentity>.Failure(new EngineError(
                code,
                $"Starfield record {targetFormKey} {detail}."));
        }

        if (wrongFamily)
        {
            return EngineResult<RecordEditIdentity>.Failure(new EngineError(
                EngineErrorCode.ValidationFailed,
                $"Starfield output contains ambiguous record families for {targetFormKey}."));
        }

        cancellationToken.ThrowIfCancellationRequested();
        var identity = new RecordEditIdentity(
            Guid.NewGuid(),
            selected.FormKey,
            null,
            FormListEditRole.ExistingOutput);
        candidate.AddProvenanceIfAbsent(CreateProvenance(sources, candidate, identity, null));
        cancellationToken.ThrowIfCancellationRequested();
        return EngineResult<RecordEditIdentity>.Success(identity);
    }

    /// <summary>Creates one target's immutable first baseline, preferring the originally selected output over a source origin.</summary>
    /// <param name="sources">The borrowed immutable Starfield source set.</param>
    /// <param name="candidate">The unpublished output candidate and its independent original baseline.</param>
    /// <param name="identity">The newly assigned edit identity.</param>
    /// <param name="sourceContext">The resolved override origin context, or <see langword="null"/> for a new or existing-output edit.</param>
    /// <returns>Complete metadata that reconstructs the exact Mutagen before-view.</returns>
    private static RecordEditProvenance CreateProvenance(
        StarfieldPluginSourceSet sources,
        StarfieldPluginOutputState candidate,
        RecordEditIdentity identity,
        FormListContext? sourceContext)
    {
        var original = FindFormList(candidate.BorrowOriginalMod(), identity.FormKey);
        if (original is not null)
        {
            var context = CreateOutputContext(sources, candidate, original);
            return new RecordEditProvenance(
                identity.EditId,
                identity.FormKey,
                EditBaselineKind.OriginalOutput,
                context);
        }

        if (sourceContext is not null && sourceContext.ContainingModKey.HasValue)
        {
            var exactSelection = new ReferenceRequest(
                identity.FormKey,
                RecordScope.AllContexts,
                sourceContext.ContainingModKey.Value);
            var exactContext = new FormListContext(
                exactSelection,
                sourceContext.Status,
                sourceContext.ContainingModKey,
                sourceContext.Path,
                sourceContext.LoadOrderIndex,
                sourceContext.Role);
            return new RecordEditProvenance(
                identity.EditId,
                identity.FormKey,
                EditBaselineKind.SourceContext,
                exactContext,
                sources.Baseline.BaselineId);
        }

        return new RecordEditProvenance(
            identity.EditId,
            identity.FormKey,
            EditBaselineKind.Absent);
    }

    /// <summary>Builds one exact staged-output context for a record in the originally selected output.</summary>
    /// <param name="sources">The borrowed source set whose plugin count positions the staged output last.</param>
    /// <param name="candidate">The selected output association.</param>
    /// <param name="record">The original FormList baseline.</param>
    /// <returns>The complete resolved or deleted staged-output context.</returns>
    private static FormListContext CreateOutputContext(
        StarfieldPluginSourceSet sources,
        StarfieldPluginOutputState candidate,
        IFormListGetter record)
    {
        var selection = new ReferenceRequest(record.FormKey, RecordScope.StagedOutput);
        return new FormListContext(
            selection,
            record.IsDeleted ? ReferenceResolutionStatus.Deleted : ReferenceResolutionStatus.Resolved,
            candidate.Association.ModKey,
            candidate.Association.PluginPath,
            sources.GetMutagenMods().Count,
            PluginRole.Output);
    }

    /// <summary>Finds one FormList by exact record identity inside a complete Starfield plugin.</summary>
    /// <param name="mod">The complete plugin to scan.</param>
    /// <param name="formKey">The exact FormKey to locate.</param>
    /// <returns>The matching FormList getter, or <see langword="null"/> when absent.</returns>
    private static IFormListGetter? FindFormList(IStarfieldModGetter mod, FormKey formKey)
    {
        foreach (var formList in mod.FormLists)
        {
            if (formList.FormKey == formKey)
            {
                return formList;
            }
        }

        return null;
    }

    /// <summary>Maps a failed contextual source read to the stable error expected by begin-override.</summary>
    /// <param name="originFormKey">The requested source identity.</param>
    /// <param name="read">The contextual source result.</param>
    /// <returns>A missing-record error for absence or a validation error for unusable record contexts.</returns>
    private static EngineError CreateOriginSelectionError(FormKey originFormKey, RecordRead read)
    {
        var code = read.Context.Status == ReferenceResolutionStatus.Unresolved
            ? EngineErrorCode.RecordNotFound
            : EngineErrorCode.ValidationFailed;
        return new EngineError(
            code,
            $"Starfield override origin {originFormKey} resolved as {read.Context.Status} and cannot be copied as a live FormList.");
    }

    /// <summary>Gets a singular Mutagen master style and rejects contradictory header flags.</summary>
    /// <param name="mod">The complete Starfield plugin.</param>
    /// <returns>The Mutagen style, or <see langword="null"/> when multiple style flags are set.</returns>
    private static MasterStyle? GetMasterStyle(IStarfieldModGetter mod)
    {
        if (mod.IsSmallMaster && mod.IsMediumMaster)
        {
            return null;
        }

        if (mod.IsSmallMaster)
        {
            return MasterStyle.Small;
        }

        return mod.IsMediumMaster ? MasterStyle.Medium : MasterStyle.Full;
    }

    /// <summary>Maps Mutagen style metadata back to the shared output contract.</summary>
    /// <param name="style">The verified Mutagen master style.</param>
    /// <returns>The corresponding shared master style.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown for a Mutagen style unsupported by the shared contract.</exception>
    private static OutputMasterStyle MapMasterStyle(MasterStyle style)
    {
        return style switch
        {
            MasterStyle.Full => OutputMasterStyle.Full,
            MasterStyle.Small => OutputMasterStyle.Small,
            MasterStyle.Medium => OutputMasterStyle.Medium,
            _ => throw new ArgumentOutOfRangeException(nameof(style))
        };
    }

    /// <summary>Compares every ordered artifact property instead of trusting a baseline identifier alone.</summary>
    /// <param name="expected">The prior complete output baseline.</param>
    /// <param name="actual">The newly observed complete output baseline.</param>
    /// <returns><see langword="true"/> when both observations are exactly equal.</returns>
    private static bool ArtifactBaselinesMatch(
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
            var left = expected.Artifacts[index];
            var right = actual.Artifacts[index];
            if (!string.Equals(left.Path, right.Path, pathComparison)
                || left.Role != right.Role
                || !string.Equals(left.Language, right.Language, StringComparison.Ordinal)
                || !left.Fingerprint.Equals(right.Fingerprint)
                || !Equals(left.FileIdentity, right.FileIdentity))
            {
                return false;
            }
        }

        return true;
    }
}
