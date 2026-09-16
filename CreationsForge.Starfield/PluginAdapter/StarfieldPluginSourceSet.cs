using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.PluginInputs;
using CreationsForge.Core.Engine.RecordReading;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Starfield;

namespace CreationsForge.Starfield.PluginAdapter;

/// <summary>
/// Owns one immutable Starfield source/load-order view and performs bounded reference operations over it.
/// </summary>
public sealed partial class StarfieldPluginSourceSet : IPluginSourceSet
{
    /// <summary>The workspace that owns this source lifetime.</summary>
    private readonly Guid OwningWorkspaceId;

    /// <summary>The shared validated input lifetime, including resource fingerprints.</summary>
    private readonly PluginSourceInputs Inputs;

    /// <summary>The independently materialized FormList-only Starfield plugins in explicit load-order order.</summary>
    private readonly List<IStarfieldModGetter> SourceMods;

    /// <summary>The lazy complete-record views used for general reference discovery.</summary>
    private readonly List<IStarfieldModGetter> ReferenceMods;

    /// <summary>The disposable overlays and caller-owned backing streams retained in creation order.</summary>
    private readonly List<IDisposable> OwnedOverlayResources;

    /// <summary>The bounded reader that borrows the complete lazy or materialized reference views.</summary>
    private ReferenceReader? ReferenceReader;

    /// <summary>The immutable source-listing snapshot retained while this source lifetime is active.</summary>
    private StarfieldMutagenReadSnapshot? ReadSnapshot;

    /// <summary>Tracks whether the source lifetime has been disposed.</summary>
    private int IsDisposed;

    /// <summary>
    /// Initializes an owned Starfield source lifetime after Mutagen parsing and baseline capture succeed.
    /// </summary>
    /// <param name="workspaceId">The non-empty workspace identifier associated with reference cursors.</param>
    /// <param name="inputs">The validated shared plugin input lifetime.</param>
    /// <param name="sourceMods">The materialized FormList-only Mutagen Starfield plugins in admitted order.</param>
    /// <param name="referenceMods">The complete lazy or materialized reference views in admitted order.</param>
    /// <param name="ownedOverlayResources">The overlay objects and backing streams owned by this source lifetime.</param>
    /// <param name="baseline">The complete source baseline established after parsing.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="workspaceId"/> is empty or the plugin source counts disagree.</exception>
    /// <exception cref="ArgumentNullException">Thrown when a required lifetime or collection is <see langword="null"/>.</exception>
    internal StarfieldPluginSourceSet(
        Guid workspaceId,
        PluginSourceInputs inputs,
        IReadOnlyList<IStarfieldModGetter> sourceMods,
        IReadOnlyList<IStarfieldModGetter> referenceMods,
        IReadOnlyList<IDisposable> ownedOverlayResources,
        PluginSourceInputBaseline baseline)
    {
        if (workspaceId == Guid.Empty)
        {
            throw new ArgumentException("A Starfield plugin source lifetime requires a non-empty workspace identifier.", nameof(workspaceId));
        }

        ArgumentNullException.ThrowIfNull(inputs);
        ArgumentNullException.ThrowIfNull(sourceMods);
        ArgumentNullException.ThrowIfNull(referenceMods);
        ArgumentNullException.ThrowIfNull(ownedOverlayResources);
        ArgumentNullException.ThrowIfNull(baseline);

        if (inputs.Plugins.Count != sourceMods.Count || inputs.Plugins.Count != referenceMods.Count)
        {
            throw new ArgumentException("Every admitted Starfield plugin must have aligned authoring and reference sources.", nameof(sourceMods));
        }

        OwningWorkspaceId = workspaceId;
        Inputs = inputs;
        SourceMods = sourceMods.ToList();
        ReferenceMods = referenceMods.ToList();
        OwnedOverlayResources = ownedOverlayResources.ToList();
        Baseline = baseline;
        Revision = new WorkspaceRevision(baseline.BaselineId, 0);
        ReadSnapshot = new StarfieldMutagenReadSnapshot(inputs.Plugins, SourceMods);
        ReferenceReader = new ReferenceReader(
            inputs.Plugins
                .Select((plugin, index) => new ReferenceSource(
                    ReferenceMods[index],
                    plugin.Path,
                    plugin.LoadOrderIndex,
                    plugin.Role,
                    record => CreateDetachedRecord(index, record)))
                .ToArray());
    }

    /// <summary>Gets the complete immutable source artifact baseline captured after Mutagen parsing.</summary>
    public PluginSourceInputBaseline Baseline { get; }

    /// <summary>Gets the workspace identifier used to bind reference results and continuation tokens.</summary>
    public Guid WorkspaceId => OwningWorkspaceId;

    /// <summary>Gets the initial source-only workspace revision used to bind reference-search cursors.</summary>
    public WorkspaceRevision Revision { get; }

    /// <summary>
    /// Gets the same-game plugin source handles for later Starfield adapter composition within this assembly.
    /// </summary>
    /// <returns>The immutable ordered collection of owned FormList-only Starfield source getters.</returns>
    internal IReadOnlyList<IStarfieldModGetter> GetMutagenMods()
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref IsDisposed) != 0, this);
        return SourceMods.AsReadOnly();
    }

    /// <summary>Gets the aligned complete-record views used by same-game general reference operations.</summary>
    /// <returns>The immutable ordered collection of owned Starfield reference getters.</returns>
    internal IReadOnlyList<IStarfieldModGetter> GetReferenceMods()
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref IsDisposed) != 0, this);
        return ReferenceMods.AsReadOnly();
    }

    /// <summary>Creates a detached Starfield record while restoring complete FormList localization from the authoring source.</summary>
    /// <param name="sourceIndex">The admitted containing-plugin position.</param>
    /// <param name="record">The borrowed record selected from the aligned reference view.</param>
    /// <returns>A complete detached Starfield record.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="sourceIndex"/> is outside the admitted source sequence.</exception>
    /// <exception cref="ObjectDisposedException">Thrown after this source lifetime is disposed.</exception>
    internal IMajorRecordGetter CreateDetachedRecord(int sourceIndex, IMajorRecordGetter record)
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref IsDisposed) != 0, this);
        ArgumentOutOfRangeException.ThrowIfNegative(sourceIndex);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(sourceIndex, SourceMods.Count);
        return CreateDetachedStarfieldRecord(sourceIndex, record);
    }

    /// <summary>
    /// Borrows the validated plugin source inputs for a same-game output operation while this source lifetime remains active.
    /// </summary>
    /// <returns>The shared input lifetime owned by this source set.</returns>
    /// <exception cref="ObjectDisposedException">Thrown after this source lifetime is disposed.</exception>
    internal PluginSourceInputs BorrowInputs()
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref IsDisposed) != 0, this);
        return Inputs;
    }

    /// <summary>
    /// Resolves one reference against the requested source scope and returns only a detached record copy.
    /// </summary>
    /// <param name="request">The Mutagen FormKey, scope, and optional containing-plugin context to resolve.</param>
    /// <param name="cancellationToken">A token that cancels the bounded Mutagen scan.</param>
    /// <returns>A resolution result, or a typed disposed/request failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public EngineResult<ReferenceResolution> Resolve(
        ReferenceRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var readerResult = GetReferenceReader();
        if (!readerResult.Succeeded)
        {
            return EngineResult<ReferenceResolution>.Failure(
                readerResult.Error!,
                WorkspaceId,
                resultRevision: Revision);
        }

        return BindResult(readerResult.Value!.Resolve(request, cancellationToken));
    }

    /// <summary>
    /// Searches references without retaining a record index and binds paging to this source baseline.
    /// </summary>
    /// <param name="request">The bounded search query, scope, optional containing context, and continuation token.</param>
    /// <param name="cancellationToken">A token that cancels the bounded Mutagen scan.</param>
    /// <returns>A deterministic page of reference metadata or a typed cursor/request failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public EngineResult<ReferenceSearchPage> Search(
        ReferenceSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var readerResult = GetReferenceReader();
        if (!readerResult.Succeeded)
        {
            return EngineResult<ReferenceSearchPage>.Failure(
                readerResult.Error!,
                WorkspaceId,
                resultRevision: Revision);
        }

        return BindResult(readerResult.Value!.Search(request, WorkspaceId, Revision, cancellationToken));
    }

    /// <summary>
    /// Rechecks every admitted plugin and selected localized resource against the recorded source baseline.
    /// </summary>
    /// <param name="cancellationToken">A token that cancels fingerprint verification.</param>
    /// <returns>The unchanged baseline, or a typed external-change/disposed failure.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public Task<EngineResult<PluginSourceInputBaseline>> VerifyUnchangedAsync(
        CancellationToken cancellationToken = default)
    {
        if (Volatile.Read(ref IsDisposed) != 0)
        {
            return Task.FromResult(EngineResult<PluginSourceInputBaseline>.Failure(
                new EngineError(EngineErrorCode.WorkspaceDisposed, "The Starfield plugin source lifetime has been disposed."),
                WorkspaceId,
                resultRevision: Revision));
        }

        return Inputs.VerifyUnchangedAsync(cancellationToken);
    }

    /// <summary>
    /// Releases the shared input lifetime; independently materialized records then become collectible.
    /// </summary>
    /// <returns>A value task that completes after shared plugin input resources are released.</returns>
    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref IsDisposed, 1) != 0)
        {
            return;
        }

        ReferenceReader = null;
        ReadSnapshot = null;
        try
        {
            foreach (var resource in OwnedOverlayResources)
            {
                resource.Dispose();
            }
        }
        finally
        {
            OwnedOverlayResources.Clear();
            ReferenceMods.Clear();
            SourceMods.Clear();
            await Inputs.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Gets the borrowed reference reader while this source lifetime remains active.
    /// </summary>
    /// <returns>The shared reader or a typed disposed failure.</returns>
    private EngineResult<ReferenceReader> GetReferenceReader()
    {
        var referenceReader = Volatile.Read(ref ReferenceReader);
        if (Volatile.Read(ref IsDisposed) != 0 || referenceReader is null)
        {
            return EngineResult<ReferenceReader>.Failure(
                new EngineError(EngineErrorCode.WorkspaceDisposed, "The Starfield plugin source lifetime has been disposed."),
                WorkspaceId,
                resultRevision: Revision);
        }

        return EngineResult<ReferenceReader>.Success(referenceReader);
    }

    /// <summary>
    /// Binds a reference result to the workspace and source revision that produced it.
    /// </summary>
    /// <typeparam name="T">The successful reference result type.</typeparam>
    /// <param name="result">The shared reference result to bind.</param>
    /// <returns>An equivalent result with workspace and revision metadata.</returns>
    private EngineResult<T> BindResult<T>(EngineResult<T> result)
    {
        return result.Succeeded
            ? EngineResult<T>.Success(
                result.Value!,
                WorkspaceId,
                resultRevision: Revision,
                warnings: result.Warnings)
            : EngineResult<T>.Failure(
                result.Error!,
                WorkspaceId,
                resultRevision: Revision,
                warnings: result.Warnings);
    }

    /// <summary>
    /// Creates and validates a complete detached Starfield record rather than accepting another game's Mutagen subtype.
    /// </summary>
    /// <param name="record">The borrowed Mutagen Starfield record selected by the shared reader.</param>
    /// <param name="sourceIndex">The aligned plugin position used to restore complete localized data.</param>
    /// <returns>A complete detached getter retaining its concrete Starfield record family and fields.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the source or copied record is not a Starfield record.</exception>
    private IMajorRecordGetter CreateDetachedStarfieldRecord(
        int sourceIndex,
        IMajorRecordGetter record)
    {
        var authoringSource = SourceMods[sourceIndex];
        if (record is IFormListGetter)
        {
            var authoringFormList = authoringSource.FormLists.FirstOrDefault(candidate => candidate.FormKey == record.FormKey);
            if (authoringFormList is not null)
            {
                return authoringFormList.DeepCopy();
            }
        }

        if (record is not IStarfieldMajorRecordGetter starfieldRecord)
        {
            throw new InvalidOperationException($"Record {record.FormKey} is not a Starfield record.");
        }

        var copy = starfieldRecord.DeepCopy();
        if (copy is not IStarfieldMajorRecordGetter)
        {
            throw new InvalidOperationException($"Detached record {record.FormKey} did not retain its Starfield record family.");
        }

        StarfieldLocalizedStringRepair.Repair(copy, Inputs, Inputs.Plugins[sourceIndex]);
        return copy;
    }
}
