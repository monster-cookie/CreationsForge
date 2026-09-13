using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.PluginInputs;
using CreationsForge.Core.Engine.RecordReading;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace CreationsForge.Skyrim.PluginAdapter;

/// <summary>
/// Owns fully materialized Skyrim Special Edition source plugins and performs bounded reference operations over them.
/// </summary>
public sealed class SkyrimPluginSourceSet : IPluginSourceSet
{
    /// <summary>The shared validated input lifetime, including Mutagen resource fingerprints.</summary>
    private readonly PluginSourceInputs _inputs;

    /// <summary>The independently materialized Mutagen Skyrim plugins in explicit load-order order.</summary>
    private readonly List<ISkyrimModGetter> _mutagenMods;

    /// <summary>The bounded reader that borrows the materialized plugin handles.</summary>
    private ReferenceReader? _referenceReader;

    /// <summary>The immutable plugin and provenance pairs used by source-only enumeration.</summary>
    private IReadOnlyList<SkyrimFormListSource>? _formListSources;

    /// <summary>Tracks whether the source lifetime has been disposed.</summary>
    private int _disposed;

    /// <summary>
    /// Initializes an owned Skyrim Special Edition source set after parsing and baseline verification complete.
    /// </summary>
    /// <param name="workspaceId">The workspace that owns this source lifetime and its reference cursors.</param>
    /// <param name="inputs">The shared plugin input lifetime transferred to this source set.</param>
    /// <param name="mutagenMods">The fully materialized Skyrim plugins in explicit load-order order.</param>
    /// <param name="baseline">The complete verified baseline established after Mutagen parsing.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="workspaceId"/> is empty or the input and plugin counts disagree.</exception>
    /// <exception cref="ArgumentNullException">Thrown when a required lifetime or collection is <see langword="null"/>.</exception>
    internal SkyrimPluginSourceSet(
        Guid workspaceId,
        PluginSourceInputs inputs,
        IReadOnlyList<ISkyrimModGetter> mutagenMods,
        PluginSourceInputBaseline baseline)
    {
        if (workspaceId == Guid.Empty)
        {
            throw new ArgumentException("A Skyrim plugin source lifetime requires a non-empty workspace identifier.", nameof(workspaceId));
        }

        ArgumentNullException.ThrowIfNull(inputs);
        ArgumentNullException.ThrowIfNull(mutagenMods);
        ArgumentNullException.ThrowIfNull(baseline);

        if (inputs.Plugins.Count != mutagenMods.Count)
        {
            throw new ArgumentException("Every admitted Skyrim plugin must have exactly one materialized plugin source.", nameof(mutagenMods));
        }

        WorkspaceId = workspaceId;
        _inputs = inputs;
        _mutagenMods = mutagenMods.ToList();
        Baseline = baseline;
        Revision = new WorkspaceRevision(baseline.BaselineId, 0);
        var formListSources = Array.AsReadOnly(inputs.Plugins
            .Select((plugin, index) => new SkyrimFormListSource(
                _mutagenMods[index],
                plugin.Path,
                plugin.LoadOrderIndex,
                plugin.Role))
            .ToArray());
        _formListSources = formListSources;
        _referenceReader = new ReferenceReader(
            formListSources
                .Select(source => new ReferenceSource(
                    source.Mod,
                    source.Path,
                    source.LoadOrderIndex,
                    source.Role,
                    CreateDetachedSkyrimRecord))
                .ToArray(),
            EnumerateSkyrimDirectLinks);
    }

    /// <summary>Gets the complete immutable source artifact baseline captured after Mutagen parsing.</summary>
    public PluginSourceInputBaseline Baseline { get; }

    /// <summary>Gets the workspace identifier used to bind reference results and continuation tokens.</summary>
    public Guid WorkspaceId { get; }

    /// <summary>Gets the initial source-only revision used to bind reference-search cursors.</summary>
    public WorkspaceRevision Revision { get; }

    /// <summary>
    /// Lists explicitly admitted Skyrim source plugins in their supplied load-order sequence.
    /// </summary>
    /// <param name="cancellationToken">A token observed for every plugin summary.</param>
    /// <returns>Immutable source plugin summaries bound to this workspace and source revision.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public EngineResult<IReadOnlyList<PluginSummary>> ListPlugins(
        CancellationToken cancellationToken = default)
    {
        var sourcesResult = GetFormListSources();
        if (!sourcesResult.Succeeded)
        {
            return EngineResult<IReadOnlyList<PluginSummary>>.Failure(
                sourcesResult.Error!,
                workspaceId: WorkspaceId,
                resultRevision: Revision);
        }

        return BindResult(PluginSummaryBuilder.Build(
            _mutagenMods,
            _inputs.Plugins,
            cancellationToken: cancellationToken));
    }

    /// <summary>
    /// Lists Skyrim FormList contexts directly from Mutagen groups in deterministic containing-plugin and record order.
    /// </summary>
    /// <param name="scope">The requested source-only Mutagen view.</param>
    /// <param name="cancellationToken">A token observed during Mutagen enumeration and override counting.</param>
    /// <returns>Immutable FormList summaries bound to this workspace and source revision.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public EngineResult<IReadOnlyList<FormListSummary>> ListFormLists(
        RecordScope scope,
        CancellationToken cancellationToken = default)
    {
        return ListFormLists(output: null, scope, cancellationToken);
    }

    /// <summary>Lists source and optional staged-output Skyrim FormList contexts in deterministic load-order and group order.</summary>
    /// <param name="output">The borrowed staged output appended after sources, or <see langword="null"/>.</param>
    /// <param name="scope">The requested plugin source, output, all-context, or winning view.</param>
    /// <param name="cancellationToken">A token observed during Mutagen enumeration and override counting.</param>
    /// <returns>Immutable FormList summaries from the complete participating load order.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    internal EngineResult<IReadOnlyList<FormListSummary>> ListFormLists(
        SkyrimPluginOutputState? output,
        RecordScope scope,
        CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(scope))
        {
            return BindResult(EngineResult<IReadOnlyList<FormListSummary>>.Failure(
                new EngineError(EngineErrorCode.InvalidRequest, "The Skyrim FormList enumeration scope is undefined.")));
        }

        var sourcesResult = GetFormListSources(output);
        if (!sourcesResult.Succeeded)
        {
            return EngineResult<IReadOnlyList<FormListSummary>>.Failure(
                sourcesResult.Error!,
                workspaceId: WorkspaceId,
                resultRevision: Revision);
        }

        var readerResult = GetReferenceReader(output);
        if (!readerResult.Succeeded)
        {
            return EngineResult<IReadOnlyList<FormListSummary>>.Failure(
                readerResult.Error!,
                workspaceId: WorkspaceId,
                resultRevision: Revision);
        }

        var contexts = new List<(SkyrimFormListSource Source, IFormListGetter Record)>();
        var sources = sourcesResult.Value!;
        foreach (var source in sources)
        {
            cancellationToken.ThrowIfCancellationRequested();
            foreach (var record in source.Mod.FormLists)
            {
                cancellationToken.ThrowIfCancellationRequested();
                contexts.Add((source, record));
            }
        }

        var winningIndices = scope == RecordScope.WinningOverrides
            ? GetWinningContextIndices(contexts, cancellationToken)
            : null;
        var summaries = new List<FormListSummary>();
        for (var index = 0; index < contexts.Count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var context = contexts[index];
            if (!IncludesContext(scope, context.Source.Role, index, winningIndices))
            {
                continue;
            }

            summaries.Add(new FormListSummary(
                context.Record.FormKey,
                context.Record.EditorID,
                readerResult.Value!.CountOverrides(
                    context.Record.FormKey,
                    typeof(IFormListGetter),
                    cancellationToken),
                scope,
                context.Source.Mod.ModKey,
                context.Source.Path,
                context.Source.LoadOrderIndex,
                context.Source.Role));
        }

        return BindResult(EngineResult<IReadOnlyList<FormListSummary>>.Success(
            Array.AsReadOnly(summaries.ToArray())));
    }

    /// <summary>
    /// Reads one exact Skyrim FormList selection with containing-plugin provenance and a detached record for resolved or deleted contexts.
    /// </summary>
    /// <param name="request">The FormKey, scope, and optional explicit containing plugin to select.</param>
    /// <param name="cancellationToken">A token observed during selection, copying, and direct-link diagnostics.</param>
    /// <returns>The contextual FormList read and missing reference warnings, bound to this workspace and source revision.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public EngineResult<RecordRead> ReadFormListContext(
        ReferenceRequest request,
        CancellationToken cancellationToken = default)
    {
        return ReadFormListContext(request, output: null, cancellationToken);
    }

    /// <summary>Reads one exact Skyrim FormList context from sources and an optional staged output.</summary>
    /// <param name="request">The exact record identity, scope, and optional containing-plugin selection.</param>
    /// <param name="output">The borrowed staged output appended after sources, or <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token observed during selection, copying, and direct-link diagnostics.</param>
    /// <returns>The contextual detached FormList read and any missing-reference warnings.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    internal EngineResult<RecordRead> ReadFormListContext(
        ReferenceRequest request,
        SkyrimPluginOutputState? output,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var readerResult = GetReferenceReader(output);
        if (!readerResult.Succeeded)
        {
            return EngineResult<RecordRead>.Failure(
                readerResult.Error!,
                workspaceId: WorkspaceId,
                resultRevision: Revision);
        }

        var result = readerResult.Value!.Read(request, cancellationToken);
        if (!result.Succeeded || result.Value?.Record is null || result.Value.Record is IFormListGetter)
        {
            return BindResult(result);
        }

        var read = result.Value;
        var context = read.Context;
        var unsupported = new RecordRead(
            new FormListContext(
                request,
                ReferenceResolutionStatus.Unsupported,
                context.ContainingModKey,
                context.Path,
                context.LoadOrderIndex,
                context.Role),
            read.RecordType,
            null);
        return BindResult(EngineResult<RecordRead>.Success(unsupported, warnings: result.Warnings));
    }

    /// <summary>
    /// Resolves one reference against the requested source scope and returns only a detached record copy.
    /// </summary>
    /// <param name="request">The Mutagen FormKey, scope, and optional containing-plugin context to resolve.</param>
    /// <param name="cancellationToken">A token that cancels the bounded Mutagen scan.</param>
    /// <returns>A resolution result bound to this workspace and source revision.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public EngineResult<ReferenceResolution> Resolve(
        ReferenceRequest request,
        CancellationToken cancellationToken = default)
    {
        return Resolve(request, output: null, cancellationToken);
    }

    /// <summary>Resolves one Skyrim reference across sources and an optional staged output.</summary>
    /// <param name="request">The exact record identity, scope, and optional containing-plugin selection.</param>
    /// <param name="output">The borrowed staged output appended after sources, or <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token observed during the bounded Mutagen scan.</param>
    /// <returns>The detached Mutagen resolution or a typed failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    internal EngineResult<ReferenceResolution> Resolve(
        ReferenceRequest request,
        SkyrimPluginOutputState? output,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var readerResult = GetReferenceReader(output);
        if (!readerResult.Succeeded)
        {
            return EngineResult<ReferenceResolution>.Failure(
                readerResult.Error!,
                workspaceId: WorkspaceId,
                resultRevision: Revision);
        }

        var result = readerResult.Value!.Resolve(request, cancellationToken);
        return BindResult(result);
    }

    /// <summary>
    /// Searches references without retaining a record index and binds paging to this source baseline.
    /// </summary>
    /// <param name="request">The bounded search query, scope, optional containing context, and continuation token.</param>
    /// <param name="cancellationToken">A token that cancels the bounded Mutagen scan.</param>
    /// <returns>A deterministic page of reference metadata or a stable cursor/request failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public EngineResult<ReferenceSearchPage> Search(
        ReferenceSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        return Search(request, output: null, WorkspaceId, Revision, cancellationToken);
    }

    /// <summary>Searches Skyrim references across sources and an optional staged output.</summary>
    /// <param name="request">The bounded query, scope, optional containing context, and continuation token.</param>
    /// <param name="output">The borrowed staged output appended after sources, or <see langword="null"/>.</param>
    /// <param name="workspaceId">The live workspace identity used to bind continuation tokens.</param>
    /// <param name="revision">The live workspace revision used to bind continuation tokens.</param>
    /// <param name="cancellationToken">A token that cancels the bounded Mutagen scan.</param>
    /// <returns>A deterministic page of reference metadata or a stable cursor/request failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="workspaceId"/> is empty.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    internal EngineResult<ReferenceSearchPage> Search(
        ReferenceSearchRequest request,
        SkyrimPluginOutputState? output,
        Guid workspaceId,
        WorkspaceRevision revision,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (workspaceId == Guid.Empty)
        {
            throw new ArgumentException("A Skyrim reference search requires a non-empty workspace identifier.", nameof(workspaceId));
        }

        var readerResult = GetReferenceReader(output);
        if (!readerResult.Succeeded)
        {
            return EngineResult<ReferenceSearchPage>.Failure(
                readerResult.Error!,
                workspaceId: WorkspaceId,
                resultRevision: Revision);
        }

        var result = readerResult.Value!.Search(request, workspaceId, revision, cancellationToken);
        return BindResult(result, revision);
    }

    /// <summary>
    /// Rechecks every admitted plugin and selected localized resource against the recorded source baseline.
    /// </summary>
    /// <param name="cancellationToken">A token that cancels fingerprint verification.</param>
    /// <returns>The unchanged baseline or a typed disposed or external-change failure.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public Task<EngineResult<PluginSourceInputBaseline>> VerifyUnchangedAsync(
        CancellationToken cancellationToken = default)
    {
        if (Volatile.Read(ref _disposed) != 0)
        {
            return Task.FromResult(EngineResult<PluginSourceInputBaseline>.Failure(
                new EngineError(EngineErrorCode.WorkspaceDisposed, "The Skyrim plugin source lifetime has been disposed."),
                workspaceId: WorkspaceId,
                resultRevision: Revision));
        }

        return _inputs.VerifyUnchangedAsync(cancellationToken);
    }

    /// <summary>
    /// Gets the same-game plugin source handles for later Skyrim adapter composition within this assembly.
    /// </summary>
    /// <returns>The immutable ordered collection of owned Mutagen Skyrim source getters.</returns>
    /// <exception cref="ObjectDisposedException">Thrown after this source set has been disposed.</exception>
    internal IReadOnlyList<ISkyrimModGetter> GetMutagenMods()
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
        return _mutagenMods.AsReadOnly();
    }

    /// <summary>
    /// Borrows the validated plugin source inputs for same-game output admission within this assembly.
    /// </summary>
    /// <returns>The source input lifetime still owned by this source set.</returns>
    /// <exception cref="ObjectDisposedException">Thrown after this source set has been disposed.</exception>
    internal PluginSourceInputs BorrowInputs()
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
        return _inputs;
    }

    /// <summary>
    /// Releases shared input resources and references to the fully materialized plugins.
    /// </summary>
    /// <returns>A value task that completes after the shared input lifetime is released.</returns>
    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        _referenceReader = null;
        _formListSources = null;
        _mutagenMods.Clear();
        await _inputs.DisposeAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Captures the borrowed shared reader while its plugin source references remain valid for the current operation.
    /// </summary>
    /// <returns>The shared reader or a typed disposed failure.</returns>
    private EngineResult<ReferenceReader> GetReferenceReader()
    {
        var referenceReader = Volatile.Read(ref _referenceReader);
        if (Volatile.Read(ref _disposed) != 0 || referenceReader is null)
        {
            return EngineResult<ReferenceReader>.Failure(
                new EngineError(EngineErrorCode.WorkspaceDisposed, "The Skyrim plugin source lifetime has been disposed."),
                workspaceId: WorkspaceId,
                resultRevision: Revision);
        }

        return EngineResult<ReferenceReader>.Success(referenceReader);
    }

    /// <summary>Creates a complete reference reader that appends the current staged output after all source plugins.</summary>
    /// <param name="output">The borrowed staged output, or <see langword="null"/> for the retained source-only reader.</param>
    /// <returns>The complete borrowed reader or a typed disposed failure.</returns>
    private EngineResult<ReferenceReader> GetReferenceReader(SkyrimPluginOutputState? output)
    {
        if (output is null)
        {
            return GetReferenceReader();
        }

        var sourcesResult = GetFormListSources(output);
        if (!sourcesResult.Succeeded)
        {
            return EngineResult<ReferenceReader>.Failure(
                sourcesResult.Error!,
                workspaceId: WorkspaceId,
                resultRevision: Revision);
        }

        var referenceSources = sourcesResult.Value!
            .Select(source => new ReferenceSource(
                source.Mod,
                source.Path,
                source.LoadOrderIndex,
                source.Role,
                CreateDetachedSkyrimRecord))
            .ToArray();
        return EngineResult<ReferenceReader>.Success(
            new ReferenceReader(referenceSources, EnumerateSkyrimDirectLinks));
    }

    /// <summary>
    /// Captures the immutable plugin and provenance snapshot while plugin source references remain valid.
    /// </summary>
    /// <returns>The source snapshot or a typed disposed failure.</returns>
    private EngineResult<IReadOnlyList<SkyrimFormListSource>> GetFormListSources()
    {
        var sources = Volatile.Read(ref _formListSources);
        if (Volatile.Read(ref _disposed) != 0 || sources is null)
        {
            return EngineResult<IReadOnlyList<SkyrimFormListSource>>.Failure(
                new EngineError(EngineErrorCode.WorkspaceDisposed, "The Skyrim plugin source lifetime has been disposed."),
                workspaceId: WorkspaceId,
                resultRevision: Revision);
        }

        return EngineResult<IReadOnlyList<SkyrimFormListSource>>.Success(sources);
    }

    /// <summary>Creates the complete ordered source and staged-output context list for one adapter operation.</summary>
    /// <param name="output">The borrowed staged output appended after all source plugins, or <see langword="null"/>.</param>
    /// <returns>The immutable participating context list or a typed disposed failure.</returns>
    private EngineResult<IReadOnlyList<SkyrimFormListSource>> GetFormListSources(
        SkyrimPluginOutputState? output)
    {
        var sourcesResult = GetFormListSources();
        if (!sourcesResult.Succeeded || output is null)
        {
            return sourcesResult;
        }

        try
        {
            var sources = sourcesResult.Value!.ToList();
            sources.Add(new SkyrimFormListSource(
                output.GetMutableMod(),
                output.Association.PluginPath,
                sources.Count,
                PluginRole.Output));
            return EngineResult<IReadOnlyList<SkyrimFormListSource>>.Success(
                Array.AsReadOnly(sources.ToArray()));
        }
        catch (ObjectDisposedException exception)
        {
            return EngineResult<IReadOnlyList<SkyrimFormListSource>>.Failure(
                new EngineError(EngineErrorCode.WorkspaceDisposed, exception.Message),
                workspaceId: WorkspaceId,
                resultRevision: Revision);
        }
    }

    /// <summary>
    /// Finds winning FormList positions while preserving the original containing-plugin and Mutagen group order.
    /// </summary>
    /// <param name="contexts">Every FormList context in explicit load-order and group order.</param>
    /// <param name="cancellationToken">A token observed for every reverse winner check.</param>
    /// <returns>The original zero-based positions of each winning context.</returns>
    private static IReadOnlySet<int> GetWinningContextIndices(
        IReadOnlyList<(SkyrimFormListSource Source, IFormListGetter Record)> contexts,
        CancellationToken cancellationToken)
    {
        var winningIndices = new HashSet<int>();
        var seenFormKeys = new HashSet<FormKey>();
        for (var index = contexts.Count - 1; index >= 0; index--)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (seenFormKeys.Add(contexts[index].Record.FormKey))
            {
                winningIndices.Add(index);
            }
        }

        return winningIndices;
    }

    /// <summary>
    /// Determines whether one FormList context belongs in the requested view.
    /// </summary>
    /// <param name="scope">The requested record view.</param>
    /// <param name="role">The containing plugin's workspace role.</param>
    /// <param name="contextIndex">The context's position in the complete ordered sequence.</param>
    /// <param name="winningIndices">Winning positions when required by <paramref name="scope"/>.</param>
    /// <returns><see langword="true"/> when the context belongs in the returned summary list.</returns>
    private static bool IncludesContext(
        RecordScope scope,
        PluginRole role,
        int contextIndex,
        IReadOnlySet<int>? winningIndices)
    {
        return scope switch
        {
            RecordScope.Source => role == PluginRole.Source,
            RecordScope.AllContexts => true,
            RecordScope.WinningOverrides => winningIndices?.Contains(contextIndex) == true,
            RecordScope.StagedOutput => role == PluginRole.Output,
            _ => false,
        };
    }

    /// <summary>
    /// Binds a shared reference result to the workspace and immutable source revision that produced it.
    /// </summary>
    /// <typeparam name="T">The successful reference value type.</typeparam>
    /// <param name="result">The shared reference result to bind.</param>
    /// <returns>An equivalent result carrying this source set's workspace and revision metadata.</returns>
    private EngineResult<T> BindResult<T>(EngineResult<T> result)
    {
        return BindResult(result, Revision);
    }

    /// <summary>Binds a shared reference result to this workspace and the supplied live revision.</summary>
    /// <typeparam name="T">The successful reference value type.</typeparam>
    /// <param name="result">The shared reference result to bind.</param>
    /// <param name="revision">The live revision that produced the result.</param>
    /// <returns>An equivalent result carrying this source set's workspace and supplied revision metadata.</returns>
    private EngineResult<T> BindResult<T>(EngineResult<T> result, WorkspaceRevision revision)
    {
        return result.Succeeded
            ? EngineResult<T>.Success(
                result.Value!,
                workspaceId: WorkspaceId,
                resultRevision: revision,
                warnings: result.Warnings)
            : EngineResult<T>.Failure(
                result.Error!,
                workspaceId: WorkspaceId,
                resultRevision: revision,
                warnings: result.Warnings);
    }

    /// <summary>
    /// Creates and validates a complete detached Skyrim record rather than accepting another game's Mutagen subtype.
    /// </summary>
    /// <param name="record">The borrowed Mutagen Skyrim record selected by the shared reader.</param>
    /// <returns>A complete detached getter retaining its concrete Skyrim record family and fields.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the source or copied record is not a Skyrim record.</exception>
    private static IMajorRecordGetter CreateDetachedSkyrimRecord(IMajorRecordGetter record)
    {
        if (record is not ISkyrimMajorRecordGetter skyrimRecord)
        {
            throw new InvalidOperationException($"Record {record.FormKey} is not a Skyrim record.");
        }

        var copy = skyrimRecord.DeepCopy();
        if (copy is not ISkyrimMajorRecordGetter)
        {
            throw new InvalidOperationException($"Detached record {record.FormKey} did not retain its Skyrim record family.");
        }

        return copy;
    }

    /// <summary>
    /// Enumerates direct Skyrim FormList links with stable field paths for missing-reference diagnostics.
    /// </summary>
    /// <param name="record">The selected borrowed record.</param>
    /// <returns>Immutable non-null direct FormList links in Mutagen item order, or an empty collection for another record family.</returns>
    private static IReadOnlyList<FormLinkReference> EnumerateSkyrimDirectLinks(IMajorRecordGetter record)
    {
        if (record is not IFormListGetter formList)
        {
            return Array.Empty<FormLinkReference>();
        }

        var references = new List<FormLinkReference>();
        for (var index = 0; index < formList.Items.Count; index++)
        {
            if (formList.Items[index].FormKeyNullable is not { } formKey || formKey.IsNull)
            {
                continue;
            }

            references.Add(new FormLinkReference(formKey, $"{nameof(IFormListGetter.Items)}[{index}]"));
        }

        return Array.AsReadOnly(references.ToArray());
    }

    /// <summary>
    /// Associates one borrowed Skyrim plugin with its explicit source provenance.
    /// </summary>
    private sealed class SkyrimFormListSource
    {
        /// <summary>
        /// Initializes one borrowed Mutagen Skyrim plugin context.
        /// </summary>
        /// <param name="mod">The fully materialized plugin.</param>
        /// <param name="path">The canonical source path.</param>
        /// <param name="loadOrderIndex">The explicit zero-based load-order position.</param>
        /// <param name="role">The plugin's workspace role.</param>
        internal SkyrimFormListSource(
            ISkyrimModGetter mod,
            string path,
            int loadOrderIndex,
            PluginRole role)
        {
            Mod = mod;
            Path = path;
            LoadOrderIndex = loadOrderIndex;
            Role = role;
        }

        /// <summary>Gets the borrowed fully materialized Mutagen Skyrim plugin.</summary>
        internal ISkyrimModGetter Mod { get; }

        /// <summary>Gets the canonical plugin path.</summary>
        internal string Path { get; }

        /// <summary>Gets the explicit zero-based load-order position.</summary>
        internal int LoadOrderIndex { get; }

        /// <summary>Gets the plugin's workspace role.</summary>
        internal PluginRole Role { get; }
    }
}
