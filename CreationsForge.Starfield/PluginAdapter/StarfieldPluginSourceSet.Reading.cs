using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.PluginInputs;
using CreationsForge.Core.Engine.RecordReading;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;

namespace CreationsForge.Starfield.PluginAdapter;

/// <summary>
/// Provides source-only Starfield FormList discovery and contextual Mutagen reads.
/// </summary>
public sealed partial class StarfieldPluginSourceSet
{
    /// <summary>Lists every participating source plugin in the admitted Mutagen load-order order.</summary>
    /// <param name="cancellationToken">A token observed before every plugin summary.</param>
    /// <returns>The immutable source plugin summaries, or a typed disposed failure.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public EngineResult<IReadOnlyList<PluginSummary>> ListPlugins(
        CancellationToken cancellationToken = default)
    {
        var snapshotResult = GetReadSnapshot();
        if (!snapshotResult.Succeeded)
        {
            return EngineResult<IReadOnlyList<PluginSummary>>.Failure(
                snapshotResult.Error!,
                WorkspaceId,
                resultRevision: Revision);
        }

        return BindResult(PluginSummaryBuilder.Build(
            GetReferenceMods(),
            BorrowInputs().Plugins,
            cancellationToken: cancellationToken));
    }

    /// <summary>Lists source FormLists in deterministic containing-plugin and Mutagen group order.</summary>
    /// <param name="scope">The source-only Mutagen view to enumerate.</param>
    /// <param name="cancellationToken">A token observed while scanning every FormList context and override.</param>
    /// <returns>Immutable FormList summaries, or a typed request/disposed failure.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public EngineResult<IReadOnlyList<FormListSummary>> ListFormLists(
        RecordScope scope,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.IsDefined(scope))
        {
            return EngineResult<IReadOnlyList<FormListSummary>>.Failure(
                new EngineError(EngineErrorCode.InvalidRequest, $"Unknown record scope '{scope}'."),
                WorkspaceId,
                resultRevision: Revision);
        }

        var snapshotResult = GetReadSnapshot();
        if (!snapshotResult.Succeeded)
        {
            return EngineResult<IReadOnlyList<FormListSummary>>.Failure(
                snapshotResult.Error!,
                WorkspaceId,
                resultRevision: Revision);
        }

        if (scope == RecordScope.StagedOutput)
        {
            return EngineResult<IReadOnlyList<FormListSummary>>.Success(
                Array.Empty<FormListSummary>(),
                WorkspaceId,
                resultRevision: Revision);
        }

        var snapshot = snapshotResult.Value!;
        var readerResult = GetReferenceReader();
        if (!readerResult.Succeeded)
        {
            return EngineResult<IReadOnlyList<FormListSummary>>.Failure(
                readerResult.Error!,
                WorkspaceId,
                resultRevision: Revision);
        }

        var reader = readerResult.Value!;
        var summaries = new List<FormListSummary>();
        for (var sourceIndex = 0; sourceIndex < snapshot.Mods.Count; sourceIndex++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var plugin = snapshot.Plugins[sourceIndex];
            if (scope == RecordScope.Source && plugin.Role != PluginRole.Source)
            {
                continue;
            }

            foreach (var formList in snapshot.Mods[sourceIndex].FormLists)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (scope == RecordScope.WinningOverrides
                    && HasLaterFormListContext(snapshot, sourceIndex, formList.FormKey, cancellationToken))
                {
                    continue;
                }

                summaries.Add(new FormListSummary(
                    formList.FormKey,
                    formList.EditorID,
                    reader.CountOverrides(formList.FormKey, typeof(IFormListGetter), cancellationToken),
                    scope,
                    plugin.ModKey,
                    plugin.Path,
                    plugin.LoadOrderIndex,
                    plugin.Role));
            }
        }

        return EngineResult<IReadOnlyList<FormListSummary>>.Success(
            Array.AsReadOnly(summaries.ToArray()),
            WorkspaceId,
            resultRevision: Revision);
    }

    /// <summary>Reads one exact source FormList context as detached Mutagen state, including deleted records.</summary>
    /// <param name="request">The FormList identity, scope, and optional containing-plugin selection.</param>
    /// <param name="cancellationToken">A token observed during selection, copying, and direct-link diagnostics.</param>
    /// <returns>The contextual Mutagen read and missing-reference warnings, or a typed disposed failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public EngineResult<RecordRead> ReadFormListContext(
        ReferenceRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var readerResult = GetReferenceReader();
        if (!readerResult.Succeeded)
        {
            return EngineResult<RecordRead>.Failure(
                readerResult.Error!,
                WorkspaceId,
                resultRevision: Revision);
        }

        var readResult = readerResult.Value!.Read(request, cancellationToken);
        if (!readResult.Succeeded)
        {
            return BindResult(readResult);
        }

        var read = readResult.Value!;
        if (read.Record is null || read.Record is IFormListGetter)
        {
            return BindResult(readResult);
        }

        var context = read.Context;
        var unsupportedContext = new FormListContext(
            context.Selection,
            ReferenceResolutionStatus.Unsupported,
            context.ContainingModKey,
            context.Path,
            context.LoadOrderIndex,
            context.Role);
        return BindResult(EngineResult<RecordRead>.Success(
            new RecordRead(unsupportedContext, read.RecordType, null),
            warnings: readResult.Warnings));
    }

    /// <summary>Checks whether a later containing plugin has a FormList context for the same record identity.</summary>
    /// <param name="snapshot">The immutable aligned plugin and Mutagen-mod view.</param>
    /// <param name="sourceIndex">The current containing plugin position.</param>
    /// <param name="formKey">The FormList identity.</param>
    /// <param name="cancellationToken">A token observed during every later Mutagen group scan.</param>
    /// <returns><see langword="true"/> when a later FormList context overrides the current context.</returns>
    private bool HasLaterFormListContext(
        StarfieldMutagenReadSnapshot snapshot,
        int sourceIndex,
        FormKey formKey,
        CancellationToken cancellationToken)
    {
        for (var laterIndex = sourceIndex + 1; laterIndex < snapshot.Mods.Count; laterIndex++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            foreach (var laterFormList in snapshot.Mods[laterIndex].FormLists)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (laterFormList.FormKey == formKey)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>Gets the immutable source-listing snapshot while the source lifetime is active.</summary>
    /// <returns>The source snapshot, or a typed disposed failure.</returns>
    private EngineResult<StarfieldMutagenReadSnapshot> GetReadSnapshot()
    {
        var snapshot = Volatile.Read(ref ReadSnapshot);
        if (Volatile.Read(ref IsDisposed) != 0 || snapshot is null)
        {
            return EngineResult<StarfieldMutagenReadSnapshot>.Failure(
                new EngineError(EngineErrorCode.WorkspaceDisposed, "The Starfield plugin source lifetime has been disposed."),
                WorkspaceId,
                resultRevision: Revision);
        }

        return EngineResult<StarfieldMutagenReadSnapshot>.Success(snapshot);
    }

    /// <summary>Retains immutable arrays for concurrent source listing after an operation obtains the snapshot.</summary>
    private sealed class StarfieldMutagenReadSnapshot
    {
        /// <summary>Initializes immutable aligned plugin and Mutagen-mod arrays.</summary>
        /// <param name="plugins">The admitted plugin descriptors.</param>
        /// <param name="mods">The materialized Mutagen Starfield plugins in matching order.</param>
        /// <exception cref="ArgumentNullException">Thrown when either collection is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when the collection counts differ.</exception>
        internal StarfieldMutagenReadSnapshot(
            IReadOnlyList<PluginSourcePluginInput> plugins,
            IReadOnlyList<IStarfieldModGetter> mods)
        {
            ArgumentNullException.ThrowIfNull(plugins);
            ArgumentNullException.ThrowIfNull(mods);
            if (plugins.Count != mods.Count)
            {
                throw new ArgumentException("Starfield source listing requires aligned plugin and Mutagen-mod collections.", nameof(mods));
            }

            Plugins = Array.AsReadOnly(plugins.ToArray());
            Mods = Array.AsReadOnly(mods.ToArray());
        }

        /// <summary>Gets the immutable admitted plugin descriptors.</summary>
        internal IReadOnlyList<PluginSourcePluginInput> Plugins { get; }

        /// <summary>Gets the immutable materialized Mutagen Starfield plugins.</summary>
        internal IReadOnlyList<IStarfieldModGetter> Mods { get; }
    }
}
