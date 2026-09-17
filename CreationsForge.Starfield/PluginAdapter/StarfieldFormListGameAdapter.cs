using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordReading;
using CreationsForge.Core.Engine.RecordInspection;
using CreationsForge.Core.Enums;
using CreationsForge.Starfield.PluginAdapter.Edits;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Starfield;

namespace CreationsForge.Starfield.PluginAdapter;

/// <summary>Composes the complete Starfield FormList engine behind the shared game-adapter contract.</summary>
public sealed class StarfieldFormListGameAdapter : IFormListGameAdapter
{
    /// <summary>The Starfield source acquisition boundary.</summary>
    private readonly StarfieldPluginSourceLoader SourceLoader;

    /// <summary>The Starfield output acquisition and edit-target boundary.</summary>
    private readonly StarfieldPluginOutputService OutputService;

    /// <summary>The Starfield typed edit and preview boundary.</summary>
    private readonly StarfieldRecordEditService EditService;

    /// <summary>The Starfield private staged-write and Mutagen-reopen boundary.</summary>
    private readonly StarfieldPluginWriter Writer;

    /// <summary>Initializes the adapter from the complete Starfield Mutagen services.</summary>
    /// <param name="sourceLoader">The source acquisition service.</param>
    /// <param name="outputService">The output acquisition and target-selection service.</param>
    /// <param name="editService">The typed edit and preview service.</param>
    /// <param name="writer">The private staged-write and Mutagen-reopen service.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required service is <see langword="null"/>.</exception>
    public StarfieldFormListGameAdapter(
        StarfieldPluginSourceLoader sourceLoader,
        StarfieldPluginOutputService outputService,
        StarfieldRecordEditService editService,
        StarfieldPluginWriter writer)
    {
        ArgumentNullException.ThrowIfNull(sourceLoader);
        ArgumentNullException.ThrowIfNull(outputService);
        ArgumentNullException.ThrowIfNull(editService);
        ArgumentNullException.ThrowIfNull(writer);
        SourceLoader = sourceLoader;
        OutputService = outputService;
        EditService = editService;
        Writer = writer;
    }

    /// <inheritdoc />
    public SupportedGame Game => SupportedGame.Starfield;

    /// <inheritdoc />
    public IFormListInspector Inspector => OutputService.Inspector;

    /// <inheritdoc />
    public IMajorRecordInspector MajorRecordInspector => OutputService.MajorRecordInspector;

    /// <inheritdoc />
    public bool SupportsRelease(GameRelease release)
    {
        return release == GameRelease.Starfield;
    }

    /// <inheritdoc />
    public async ValueTask<EngineResult<PluginSourceOpenResult>> OpenSourcesAsync(
        WorkspaceOpenRequest request,
        CancellationToken cancellationToken = default)
    {
        return await SourceLoader.OpenAsync(request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask<EngineResult<PluginOutputOpenResult>> OpenOutputAsync(
        IPluginSourceSet sources,
        SelectOutputRequest request,
        CancellationToken cancellationToken = default)
    {
        var sourceResult = RequireSources<PluginOutputOpenResult>(sources);
        if (!sourceResult.Succeeded)
        {
            return EngineResult<PluginOutputOpenResult>.Failure(sourceResult.Error!);
        }

        return await OutputService.OpenAsync(sourceResult.Value!, request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask<EngineResult<PluginOutputOpenResult>> ReopenOutputAsync(
        IPluginSourceSet sources,
        OutputAssociation association,
        OutputArtifactSetBaseline expectedBaseline,
        CancellationToken cancellationToken = default)
    {
        var sourceResult = RequireSources<PluginOutputOpenResult>(sources);
        if (!sourceResult.Succeeded)
        {
            return EngineResult<PluginOutputOpenResult>.Failure(sourceResult.Error!);
        }

        return await OutputService
            .ReopenAsync(sourceResult.Value!, association, expectedBaseline, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public IPluginOutputState CloneOutput(
        IPluginOutputState output,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(output);
        if (output is not StarfieldPluginOutputState starfieldOutput)
        {
            throw new ArgumentException("The Starfield adapter requires Starfield plugin output state.", nameof(output));
        }

        return OutputService.Clone(starfieldOutput, cancellationToken);
    }

    /// <inheritdoc />
    public EngineResult<RecordEditIdentity> BeginEdit(
        IPluginSourceSet sources,
        IPluginOutputState candidate,
        BeginEditRequest request,
        CancellationToken cancellationToken)
    {
        var sourceResult = RequireSources<RecordEditIdentity>(sources);
        if (!sourceResult.Succeeded)
        {
            return EngineResult<RecordEditIdentity>.Failure(sourceResult.Error!);
        }

        var outputResult = RequireOutput<RecordEditIdentity>(candidate);
        if (!outputResult.Succeeded)
        {
            return EngineResult<RecordEditIdentity>.Failure(outputResult.Error!);
        }

        return OutputService.BeginEdit(sourceResult.Value!, outputResult.Value!, request, cancellationToken);
    }

    /// <inheritdoc />
    public PreparedFormListEdit PrepareEdit(FormListEdit edit)
    {
        return EditService.PrepareEdit(edit);
    }

    /// <inheritdoc />
    public EngineResult<RecordEditMutationResult> ApplyEdit(
        IPluginSourceSet sources,
        IPluginOutputState candidate,
        FormKey target,
        PreparedFormListEdit edit)
    {
        var sourceResult = RequireSources<RecordEditMutationResult>(sources);
        if (!sourceResult.Succeeded)
        {
            return EngineResult<RecordEditMutationResult>.Failure(sourceResult.Error!);
        }

        var outputResult = RequireOutput<RecordEditMutationResult>(candidate);
        if (!outputResult.Succeeded)
        {
            return EngineResult<RecordEditMutationResult>.Failure(outputResult.Error!);
        }

        return EditService.ApplyEdit(sourceResult.Value!, outputResult.Value!, target, edit);
    }

    /// <inheritdoc />
    public EngineResult<IReadOnlyList<PluginSummary>> ListPlugins(
        IPluginSourceSet sources,
        IPluginOutputState? output,
        CancellationToken cancellationToken)
    {
        var stateResult = RequireStates<IReadOnlyList<PluginSummary>>(sources, output);
        if (!stateResult.Succeeded)
        {
            return EngineResult<IReadOnlyList<PluginSummary>>.Failure(stateResult.Error!);
        }

        var (starfieldSources, starfieldOutput) = stateResult.Value!;
        if (starfieldOutput is null)
        {
            return starfieldSources.ListPlugins(cancellationToken);
        }

        var result = PluginSummaryBuilder.Build(
            starfieldSources.GetReferenceMods(),
            starfieldSources.BorrowInputs().Plugins,
            starfieldOutput.BorrowMod(),
            starfieldOutput.Association,
            cancellationToken);
        return Success(starfieldSources, result.Value!, result.Warnings);
    }

    /// <inheritdoc />
    public EngineResult<IReadOnlyList<FormListSummary>> ListFormLists(
        IPluginSourceSet sources,
        IPluginOutputState? output,
        RecordScope scope,
        CancellationToken cancellationToken)
    {
        var stateResult = RequireStates<IReadOnlyList<FormListSummary>>(sources, output);
        if (!stateResult.Succeeded)
        {
            return EngineResult<IReadOnlyList<FormListSummary>>.Failure(stateResult.Error!);
        }

        var (starfieldSources, starfieldOutput) = stateResult.Value!;
        if (!Enum.IsDefined(scope))
        {
            return Failure<IReadOnlyList<FormListSummary>>(
                starfieldSources,
                new EngineError(EngineErrorCode.InvalidRequest, $"Unknown record scope '{scope}'."));
        }

        if (starfieldOutput is null)
        {
            return starfieldSources.ListFormLists(scope, cancellationToken);
        }

        try
        {
            var pluginSources = CreateReferenceSources(starfieldSources, starfieldOutput, cancellationToken);
            var reader = new ReferenceReader(pluginSources);
            var summaries = new List<FormListSummary>();
            for (var sourceIndex = 0; sourceIndex < pluginSources.Count; sourceIndex++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var source = pluginSources[sourceIndex];
                if (!IncludesSource(scope, source.Role))
                {
                    continue;
                }

                var mod = sourceIndex < starfieldSources.GetMutagenMods().Count
                    ? starfieldSources.GetMutagenMods()[sourceIndex]
                    : starfieldOutput.BorrowMod();
                foreach (var formList in mod.FormLists)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (scope == RecordScope.WinningOverrides
                        && HasLaterFormListContext(pluginSources, starfieldSources, starfieldOutput, sourceIndex, formList.FormKey, cancellationToken))
                    {
                        continue;
                    }

                    summaries.Add(new FormListSummary(
                        formList.FormKey,
                        formList.EditorID,
                        reader.CountOverrides(formList.FormKey, typeof(IFormListGetter), cancellationToken),
                        scope,
                        source.ModKey,
                        source.Path,
                        source.LoadOrderIndex,
                        source.Role));
                }
            }

            return Success<IReadOnlyList<FormListSummary>>(starfieldSources, Array.AsReadOnly(summaries.ToArray()));
        }
        catch (ObjectDisposedException exception)
        {
            return Failure<IReadOnlyList<FormListSummary>>(
                starfieldSources,
                new EngineError(EngineErrorCode.WorkspaceDisposed, exception.Message));
        }
    }

    /// <inheritdoc />
    public EngineResult<RecordRead> ReadFormListContext(
        IPluginSourceSet sources,
        IPluginOutputState? output,
        ReferenceRequest request,
        CancellationToken cancellationToken)
    {
        var sourceResult = RequireSources<RecordRead>(sources);
        if (!sourceResult.Succeeded)
        {
            return EngineResult<RecordRead>.Failure(sourceResult.Error!);
        }

        if (output is null)
        {
            return sourceResult.Value!.ReadFormListContext(request, cancellationToken);
        }

        var readerResult = CreateReader<RecordRead>(sources, output, cancellationToken);
        if (!readerResult.Succeeded)
        {
            return Failure<RecordRead>(sourceResult.Value!, readerResult.Error!);
        }

        var readResult = readerResult.Value!.Read(request, cancellationToken);
        if (!readResult.Succeeded || readResult.Value is null)
        {
            return Bind(sourceResult.Value!, readResult);
        }

        var read = readResult.Value;
        if (read.Record is null || read.Record is IFormListGetter)
        {
            return Bind(sourceResult.Value!, readResult);
        }

        var context = read.Context;
        var unsupported = new FormListContext(
            context.Selection,
            ReferenceResolutionStatus.Unsupported,
            context.ContainingModKey,
            context.Path,
            context.LoadOrderIndex,
            context.Role);
        return Success(
            sourceResult.Value!,
            new RecordRead(unsupported, read.RecordType, null),
            readResult.Warnings);
    }

    /// <inheritdoc />
    public EngineResult<RecordRead> ReadRecordContext(
        IPluginSourceSet sources,
        IPluginOutputState? output,
        ReferenceRequest request,
        CancellationToken cancellationToken)
    {
        var sourceResult = RequireSources<RecordRead>(sources);
        if (!sourceResult.Succeeded)
        {
            return EngineResult<RecordRead>.Failure(sourceResult.Error!);
        }

        if (output is null)
        {
            return sourceResult.Value!.ReadRecordContext(request, cancellationToken);
        }

        var readerResult = CreateReader<RecordRead>(sources, output, cancellationToken);
        return readerResult.Succeeded
            ? Bind(sourceResult.Value!, readerResult.Value!.Read(request, cancellationToken))
            : Failure<RecordRead>(sourceResult.Value!, readerResult.Error!);
    }

    /// <inheritdoc />
    public EngineResult<ReferenceSearchPage> SearchReferences(
        IPluginSourceSet sources,
        IPluginOutputState? output,
        ReferenceSearchRequest request,
        Guid workspaceId,
        WorkspaceRevision revision,
        CancellationToken cancellationToken)
    {
        var sourceResult = RequireSources<ReferenceSearchPage>(sources);
        if (!sourceResult.Succeeded)
        {
            return EngineResult<ReferenceSearchPage>.Failure(sourceResult.Error!);
        }

        if (output is null)
        {
            return sourceResult.Value!.Search(request, cancellationToken);
        }

        var readerResult = CreateReader<ReferenceSearchPage>(sources, output, cancellationToken);
        return readerResult.Succeeded
            ? Bind(
                sourceResult.Value!,
                readerResult.Value!.Search(request, workspaceId, revision, cancellationToken),
                revision)
            : Failure<ReferenceSearchPage>(sourceResult.Value!, readerResult.Error!);
    }

    /// <inheritdoc />
    public EngineResult<ReferenceResolution> ResolveReference(
        IPluginSourceSet sources,
        IPluginOutputState? output,
        ReferenceRequest request,
        CancellationToken cancellationToken)
    {
        var sourceResult = RequireSources<ReferenceResolution>(sources);
        if (!sourceResult.Succeeded)
        {
            return EngineResult<ReferenceResolution>.Failure(sourceResult.Error!);
        }

        if (output is null)
        {
            return sourceResult.Value!.Resolve(request, cancellationToken);
        }

        var readerResult = CreateReader<ReferenceResolution>(sources, output, cancellationToken);
        return readerResult.Succeeded
            ? Bind(sourceResult.Value!, readerResult.Value!.Resolve(request, cancellationToken))
            : Failure<ReferenceResolution>(sourceResult.Value!, readerResult.Error!);
    }

    /// <inheritdoc />
    public EngineResult<WorkspacePreview> Preview(
        IPluginSourceSet sources,
        IPluginOutputState output)
    {
        var sourceResult = RequireSources<WorkspacePreview>(sources);
        if (!sourceResult.Succeeded)
        {
            return EngineResult<WorkspacePreview>.Failure(sourceResult.Error!);
        }

        var outputResult = RequireOutput<WorkspacePreview>(output);
        if (!outputResult.Succeeded)
        {
            return EngineResult<WorkspacePreview>.Failure(outputResult.Error!);
        }

        return Bind(
            sourceResult.Value!,
            EditService.Preview(sourceResult.Value!, outputResult.Value!));
    }

    /// <inheritdoc />
    public async ValueTask<EngineResult<StagedPluginOutputSet>> WriteAndValidateAsync(
        IPluginSourceSet sources,
        IPluginOutputState output,
        PluginWriteRequest request,
        CancellationToken cancellationToken = default)
    {
        var sourceResult = RequireSources<StagedPluginOutputSet>(sources);
        if (!sourceResult.Succeeded)
        {
            return EngineResult<StagedPluginOutputSet>.Failure(sourceResult.Error!);
        }

        var outputResult = RequireOutput<StagedPluginOutputSet>(output);
        if (!outputResult.Succeeded)
        {
            return EngineResult<StagedPluginOutputSet>.Failure(outputResult.Error!);
        }

        return await Writer
            .WriteAndValidateAsync(sourceResult.Value!, outputResult.Value!, request, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>Creates a complete output-aware reference reader over borrowed Starfield state.</summary>
    /// <typeparam name="T">The eventual adapter result type used for typed failure construction.</typeparam>
    /// <param name="sources">The opaque plugin source handle.</param>
    /// <param name="output">The optional opaque output handle.</param>
    /// <param name="cancellationToken">A token observed while borrowing Mutagen state.</param>
    /// <returns>The reader or a typed game-state mismatch.</returns>
    private static EngineResult<ReferenceReader> CreateReader<T>(
        IPluginSourceSet sources,
        IPluginOutputState? output,
        CancellationToken cancellationToken)
    {
        var states = RequireStates<T>(sources, output);
        if (!states.Succeeded)
        {
            return EngineResult<ReferenceReader>.Failure(states.Error!);
        }

        var (starfieldSources, starfieldOutput) = states.Value!;
        try
        {
            return EngineResult<ReferenceReader>.Success(new ReferenceReader(
                CreateReferenceSources(starfieldSources, starfieldOutput, cancellationToken)));
        }
        catch (ObjectDisposedException exception)
        {
            return EngineResult<ReferenceReader>.Failure(
                new EngineError(EngineErrorCode.WorkspaceDisposed, exception.Message));
        }
    }

    /// <summary>Creates ordered reference-source descriptors with staged output last.</summary>
    /// <param name="sources">The borrowed Starfield source set.</param>
    /// <param name="output">The optional borrowed Starfield output.</param>
    /// <param name="cancellationToken">A token observed between source descriptors.</param>
    /// <returns>The immutable complete source sequence.</returns>
    private static IReadOnlyList<ReferenceSource> CreateReferenceSources(
        StarfieldPluginSourceSet sources,
        StarfieldPluginOutputState? output,
        CancellationToken cancellationToken)
    {
        var mods = sources.GetReferenceMods();
        var plugins = sources.BorrowInputs().Plugins;
        var pluginSources = new List<ReferenceSource>(mods.Count + (output is null ? 0 : 1));
        for (var index = 0; index < mods.Count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var sourceIndex = index;
            pluginSources.Add(new ReferenceSource(
                mods[index],
                plugins[index].Path,
                plugins[index].LoadOrderIndex,
                plugins[index].Role,
                record => sources.CreateDetachedRecord(sourceIndex, record)));
        }

        if (output is not null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            pluginSources.Add(new ReferenceSource(
                output.BorrowMod(),
                output.Association.PluginPath,
                mods.Count,
                PluginRole.Output,
                CreateDetachedRecord));
        }

        return Array.AsReadOnly(pluginSources.ToArray());
    }

    /// <summary>Determines whether a source role belongs to the requested record view.</summary>
    /// <param name="scope">The requested record view.</param>
    /// <param name="role">The candidate plugin role.</param>
    /// <returns><see langword="true"/> when the candidate participates.</returns>
    private static bool IncludesSource(RecordScope scope, PluginRole role)
    {
        return scope switch
        {
            RecordScope.Source => role == PluginRole.Source,
            RecordScope.StagedOutput => role == PluginRole.Output,
            RecordScope.WinningOverrides or RecordScope.AllContexts => true,
            _ => false
        };
    }

    /// <summary>Checks later participating plugins for an overriding FormList context.</summary>
    /// <param name="pluginSources">The ordered plugin source descriptors.</param>
    /// <param name="sources">The borrowed Starfield source set.</param>
    /// <param name="output">The borrowed Starfield output.</param>
    /// <param name="sourceIndex">The current source position.</param>
    /// <param name="formKey">The FormList identity.</param>
    /// <param name="cancellationToken">A token observed while scanning later groups.</param>
    /// <returns><see langword="true"/> when a later FormList context exists.</returns>
    private static bool HasLaterFormListContext(
        IReadOnlyList<ReferenceSource> pluginSources,
        StarfieldPluginSourceSet sources,
        StarfieldPluginOutputState output,
        int sourceIndex,
        FormKey formKey,
        CancellationToken cancellationToken)
    {
        var sourceMods = sources.GetMutagenMods();
        for (var laterIndex = sourceIndex + 1; laterIndex < pluginSources.Count; laterIndex++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var mod = laterIndex < sourceMods.Count ? sourceMods[laterIndex] : output.BorrowMod();
            if (mod.FormLists.Any(formList => formList.FormKey == formKey))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Creates a detached complete Starfield record.</summary>
    /// <param name="record">The borrowed record selected by the shared reader.</param>
    /// <returns>An independent Starfield record copy.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the record is not a Starfield Mutagen family.</exception>
    private static IMajorRecordGetter CreateDetachedRecord(IMajorRecordGetter record)
    {
        if (record is not IStarfieldMajorRecordGetter starfieldRecord)
        {
            throw new InvalidOperationException($"Record {record.FormKey} is not a Starfield record.");
        }

        return starfieldRecord.DeepCopy();
    }

    /// <summary>Validates opaque source and optional output handles as Starfield state.</summary>
    /// <typeparam name="T">The eventual adapter result type used for typed failure construction.</typeparam>
    /// <param name="sources">The opaque source state.</param>
    /// <param name="output">The optional opaque output state.</param>
    /// <returns>The typed Starfield state pair or an invalid-request failure.</returns>
    private static EngineResult<(StarfieldPluginSourceSet Sources, StarfieldPluginOutputState? Output)> RequireStates<T>(
        IPluginSourceSet sources,
        IPluginOutputState? output)
    {
        var sourceResult = RequireSources<T>(sources);
        if (!sourceResult.Succeeded)
        {
            return EngineResult<(StarfieldPluginSourceSet Sources, StarfieldPluginOutputState? Output)>.Failure(sourceResult.Error!);
        }

        if (output is not null && output is not StarfieldPluginOutputState)
        {
            return EngineResult<(StarfieldPluginSourceSet Sources, StarfieldPluginOutputState? Output)>.Failure(
                new EngineError(EngineErrorCode.InvalidRequest, "The Starfield adapter requires Starfield plugin output state."));
        }

        return EngineResult<(StarfieldPluginSourceSet Sources, StarfieldPluginOutputState? Output)>.Success(
            (sourceResult.Value!, output as StarfieldPluginOutputState));
    }

    /// <summary>Validates an opaque source handle as Starfield state.</summary>
    /// <typeparam name="T">The eventual adapter result type used for typed failure construction.</typeparam>
    /// <param name="sources">The opaque source state.</param>
    /// <returns>The Starfield source state or an invalid-request failure.</returns>
    private static EngineResult<StarfieldPluginSourceSet> RequireSources<T>(IPluginSourceSet sources)
    {
        ArgumentNullException.ThrowIfNull(sources);
        return sources is StarfieldPluginSourceSet starfieldSources
            ? EngineResult<StarfieldPluginSourceSet>.Success(starfieldSources)
            : EngineResult<StarfieldPluginSourceSet>.Failure(new EngineError(
                EngineErrorCode.InvalidRequest,
                "The Starfield adapter requires Starfield plugin source state."));
    }

    /// <summary>Validates an opaque output handle as Starfield state.</summary>
    /// <typeparam name="T">The eventual adapter result type used for typed failure construction.</typeparam>
    /// <param name="output">The opaque output state.</param>
    /// <returns>The Starfield output state or an invalid-request failure.</returns>
    private static EngineResult<StarfieldPluginOutputState> RequireOutput<T>(IPluginOutputState output)
    {
        ArgumentNullException.ThrowIfNull(output);
        return output is StarfieldPluginOutputState starfieldOutput
            ? EngineResult<StarfieldPluginOutputState>.Success(starfieldOutput)
            : EngineResult<StarfieldPluginOutputState>.Failure(new EngineError(
                EngineErrorCode.InvalidRequest,
                "The Starfield adapter requires Starfield plugin output state."));
    }

    /// <summary>Creates a successful adapter result bound to the typed source lifetime.</summary>
    /// <typeparam name="T">The successful result value type.</typeparam>
    /// <param name="sources">The typed Starfield source lifetime.</param>
    /// <param name="value">The successful value.</param>
    /// <param name="warnings">Optional nonfatal warnings.</param>
    /// <returns>A successful source-bound engine result.</returns>
    private static EngineResult<T> Success<T>(
        StarfieldPluginSourceSet sources,
        T value,
        IReadOnlyList<EngineWarning>? warnings = null)
    {
        return EngineResult<T>.Success(
            value,
            workspaceId: sources.WorkspaceId,
            resultRevision: sources.Revision,
            warnings: warnings);
    }

    /// <summary>Creates a failed adapter result bound to the typed source lifetime.</summary>
    /// <typeparam name="T">The failed result value type.</typeparam>
    /// <param name="sources">The typed Starfield source lifetime.</param>
    /// <param name="error">The typed engine failure.</param>
    /// <param name="warnings">Optional nonfatal warnings.</param>
    /// <returns>A failed source-bound engine result.</returns>
    private static EngineResult<T> Failure<T>(
        StarfieldPluginSourceSet sources,
        EngineError error,
        IReadOnlyList<EngineWarning>? warnings = null)
    {
        return EngineResult<T>.Failure(
            error,
            workspaceId: sources.WorkspaceId,
            resultRevision: sources.Revision,
            warnings: warnings);
    }

    /// <summary>Binds a shared Mutagen-reader result to the typed source lifetime and selected workspace revision.</summary>
    /// <typeparam name="T">The Mutagen reader result type.</typeparam>
    /// <param name="sources">The typed Starfield source lifetime.</param>
    /// <param name="result">The unbound shared-reader result.</param>
    /// <param name="revision">The selected result revision, or the source-only revision when omitted.</param>
    /// <returns>An equivalent source-bound result.</returns>
    private static EngineResult<T> Bind<T>(
        StarfieldPluginSourceSet sources,
        EngineResult<T> result,
        WorkspaceRevision? revision = null)
    {
        return result.Succeeded
            ? EngineResult<T>.Success(
                result.Value!,
                workspaceId: sources.WorkspaceId,
                resultRevision: revision ?? sources.Revision,
                warnings: result.Warnings)
            : EngineResult<T>.Failure(
                result.Error!,
                workspaceId: sources.WorkspaceId,
                resultRevision: revision ?? sources.Revision,
                warnings: result.Warnings);
    }
}
