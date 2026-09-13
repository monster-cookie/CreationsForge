using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

namespace CreationsForge.Core.Engine.NativeReading;

/// <summary>
/// Performs bounded reference lookup directly over explicitly ordered native plugin groups without a LinkCache or record index.
/// </summary>
public sealed class NativeReferenceReader
{
    /// <summary>The immutable snapshot of borrowed native plugin handles and their provenance.</summary>
    private readonly IReadOnlyList<NativeReferenceSource> Sources;

    /// <summary>The game-aware direct native link visitor used for missing-reference diagnostics.</summary>
    private readonly Func<IMajorRecordGetter, IReadOnlyList<NativeFormLinkReference>> EnumerateDirectLinks;

    /// <summary>
    /// Initializes a shared reader over an explicit native load order.
    /// </summary>
    /// <param name="sources">The ordered borrowed native plugin handles and their provenance.</param>
    /// <param name="enumerateDirectLinks">An optional game-aware visitor that returns direct native links with typed field paths.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="sources"/> or one of its entries is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when source load-order positions decrease relative to their supplied order.</exception>
    public NativeReferenceReader(
        IReadOnlyList<NativeReferenceSource> sources,
        Func<IMajorRecordGetter, IReadOnlyList<NativeFormLinkReference>>? enumerateDirectLinks = null)
    {
        ArgumentNullException.ThrowIfNull(sources);
        var snapshot = sources.ToArray();
        for (var index = 0; index < snapshot.Length; index++)
        {
            ArgumentNullException.ThrowIfNull(snapshot[index]);
            if (index > 0 && snapshot[index - 1].LoadOrderIndex > snapshot[index].LoadOrderIndex)
            {
                throw new ArgumentException(
                    "Native reference sources must retain non-decreasing explicit load-order positions.",
                    nameof(sources));
            }
        }

        Sources = Array.AsReadOnly(snapshot);
        EnumerateDirectLinks = enumerateDirectLinks ?? EnumerateDefaultDirectLinks;
    }

    /// <summary>
    /// Resolves one native FormKey in the requested view and returns only an independent deep copy.
    /// </summary>
    /// <param name="request">The exact native identity, scope, and optional containing-plugin selection.</param>
    /// <param name="cancellationToken">A token observed during native group scans and around deep copying.</param>
    /// <returns>A typed resolved, unresolved, deleted, ambiguous, unknown-family, or unsupported result.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public EngineResult<ReferenceResolution> Resolve(
        ReferenceRequest request,
        CancellationToken cancellationToken = default)
    {
        var readResult = Read(request, cancellationToken);
        if (!readResult.Succeeded || readResult.Value is null)
        {
            return EngineResult<ReferenceResolution>.Failure(
                readResult.Error ?? new EngineError(EngineErrorCode.UnexpectedFailure, "The native contextual read failed without a typed error."),
                warnings: readResult.Warnings);
        }

        var read = readResult.Value;
        var context = read.Context;
        var resolution = new ReferenceResolution(
            context.Status,
            context.Selection.FormKey,
            read.RecordType,
            context.Status == ReferenceResolutionStatus.Resolved ? read.Record : null,
            context.ContainingModKey,
            context.Path,
            context.LoadOrderIndex,
            context.Role);
        return EngineResult<ReferenceResolution>.Success(resolution, warnings: readResult.Warnings);
    }

    /// <summary>Reads one selected native context as an independent copy, including the copy of a deleted context.</summary>
    /// <param name="request">The exact native identity, scope, and optional containing-plugin selection.</param>
    /// <param name="cancellationToken">A token observed during selection, copying, and direct-link diagnostics.</param>
    /// <returns>A contextual detached record read and any missing-reference warnings.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public EngineResult<NativeRecordRead> Read(
        ReferenceRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        var sourceIndices = GetSourceIndices(request.Scope, request.ContainingModKey);
        NativeRecordContext? selected = null;
        foreach (var sourceIndex in sourceIndices)
        {
            var source = Sources[sourceIndex];
            foreach (var record in source.Mod.EnumerateMajorRecords())
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (record.FormKey != request.FormKey)
                {
                    continue;
                }

                if (request.Scope == RecordScope.WinningOverrides)
                {
                    return ReadContext(new NativeRecordContext(source, record), request, cancellationToken);
                }

                if (selected.HasValue)
                {
                    return EngineResult<NativeRecordRead>.Success(new NativeRecordRead(
                        new FormListContext(request, ReferenceResolutionStatus.Ambiguous, null, null, null, null),
                        GetRecordType(selected.Value.Record),
                        null));
                }

                selected = new NativeRecordContext(source, record);
            }
        }

        if (!selected.HasValue)
        {
            return EngineResult<NativeRecordRead>.Success(new NativeRecordRead(
                new FormListContext(request, ReferenceResolutionStatus.Unresolved, null, null, null, null),
                null,
                null));
        }

        return ReadContext(selected.Value, request, cancellationToken);
    }

    /// <summary>Counts every matching override context across the complete participating load order.</summary>
    /// <param name="formKey">The native origin identity whose override contexts are counted.</param>
    /// <param name="recordGetterType">The native getter interface used for direct top-level group lookup.</param>
    /// <param name="cancellationToken">A token observed during every native group lookup and fallback record scan.</param>
    /// <returns>The number of matching contexts whose containing plugin differs from the FormKey origin plugin.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="recordGetterType"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public int CountOverrides(
        FormKey formKey,
        Type recordGetterType,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(recordGetterType);
        var count = 0;
        foreach (var source in Sources)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (source.ModKey == formKey.ModKey)
            {
                continue;
            }

            var topLevelResult = HasTopLevelContext(source.Mod, recordGetterType, formKey);
            if (topLevelResult.HasValue)
            {
                if (topLevelResult.Value)
                {
                    count = checked(count + 1);
                }

                continue;
            }

            // Some native families do not expose a top-level group through the shared interface.
            // Scan only that exceptional source rather than maintaining a parallel record index.
            foreach (var record in source.Mod.EnumerateMajorRecords())
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (record.FormKey == formKey)
                {
                    count = checked(count + 1);
                }
            }
        }

        return count;
    }

    /// <summary>
    /// Searches native FormKeys and EditorIDs in deterministic bounded pages tied to one workspace revision.
    /// </summary>
    /// <param name="request">The normalized query, bounded page size, scope, filter, and optional continuation token.</param>
    /// <param name="workspaceId">The non-empty identity of the workspace that owns continuation state.</param>
    /// <param name="revision">The exact native baseline and in-memory mutation sequence observed for this search.</param>
    /// <param name="cancellationToken">A token observed during every native scan and winner check.</param>
    /// <returns>One immutable page or a typed invalid-request failure for invalid continuation state.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public EngineResult<ReferenceSearchPage> Search(
        ReferenceSearchRequest request,
        Guid workspaceId,
        WorkspaceRevision revision,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();
        if (workspaceId == Guid.Empty)
        {
            return InvalidSearch("A native reference search requires a non-empty workspace identifier.", revision);
        }

        var sourceIndices = GetSourceIndices(request.Scope, request.ContainingModKey);
        var startSourceIndex = sourceIndices.Count == 0 ? 0 : sourceIndices[0];
        var startRecordIndex = 0;
        if (request.ContinuationToken is not null &&
            (!NativeReferenceCursor.TryDecode(
                request.ContinuationToken,
                request,
                workspaceId,
                revision,
                out startSourceIndex,
                out startRecordIndex) ||
             !sourceIndices.Contains(startSourceIndex)))
        {
            return InvalidSearch(
                "The continuation token is invalid for the current workspace, revision, query, scope, filter, or page size.",
                revision);
        }

        var matches = new List<ReferenceSearchMatch>(request.MaximumResults);
        var reachedStart = request.ContinuationToken is null;
        foreach (var sourceIndex in sourceIndices)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!reachedStart)
            {
                if (sourceIndex != startSourceIndex)
                {
                    continue;
                }

                reachedStart = true;
            }

            var recordIndex = 0;
            foreach (var record in Sources[sourceIndex].Mod.EnumerateMajorRecords())
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (sourceIndex == startSourceIndex && recordIndex < startRecordIndex)
                {
                    recordIndex++;
                    continue;
                }

                if (IsSearchMatch(request, sourceIndex, record, cancellationToken))
                {
                    if (matches.Count == request.MaximumResults)
                    {
                        var continuationToken = NativeReferenceCursor.Encode(
                            request,
                            workspaceId,
                            revision,
                            sourceIndex,
                            recordIndex);
                        return EngineResult<ReferenceSearchPage>.Success(
                            new ReferenceSearchPage(matches, continuationToken),
                            resultRevision: revision);
                    }

                    matches.Add(CreateMatch(Sources[sourceIndex], record));
                }

                recordIndex++;
            }

            startRecordIndex = 0;
        }

        if (!reachedStart && request.ContinuationToken is not null)
        {
            return InvalidSearch("The continuation token points outside the current native source sequence.", revision);
        }

        return EngineResult<ReferenceSearchPage>.Success(
            new ReferenceSearchPage(matches, null),
            resultRevision: revision);
    }

    /// <summary>Creates one immutable search match with origin identity and containing-plugin provenance kept separate.</summary>
    /// <param name="source">The plugin containing the context.</param>
    /// <param name="record">The borrowed native record context.</param>
    /// <returns>The immutable common metadata for the match.</returns>
    private static ReferenceSearchMatch CreateMatch(NativeReferenceSource source, IMajorRecordGetter record)
    {
        var recordType = GetRecordType(record);
        return new ReferenceSearchMatch(
            record.FormKey,
            string.IsNullOrWhiteSpace(recordType) ? "Unknown" : recordType,
            record.EditorID,
            source.ModKey,
            source.Path,
            source.LoadOrderIndex,
            source.Role,
            record.IsDeleted);
    }

    /// <summary>Creates a typed invalid-request search failure without discarding the observed revision.</summary>
    /// <param name="message">The diagnostic failure text.</param>
    /// <param name="revision">The unchanged revision observed by the failed request.</param>
    /// <returns>The typed failed page result.</returns>
    private static EngineResult<ReferenceSearchPage> InvalidSearch(string message, WorkspaceRevision revision)
    {
        return EngineResult<ReferenceSearchPage>.Failure(
            new EngineError(EngineErrorCode.InvalidRequest, message),
            resultRevision: revision);
    }

    /// <summary>Determines whether one native context matches the query and requested view.</summary>
    /// <param name="request">The normalized search request.</param>
    /// <param name="sourceIndex">The candidate's position in the ordered source sequence.</param>
    /// <param name="record">The candidate native record.</param>
    /// <param name="cancellationToken">A token observed while checking later override contexts.</param>
    /// <returns><see langword="true"/> when this context belongs in the deterministic result stream.</returns>
    private bool IsSearchMatch(
        ReferenceSearchRequest request,
        int sourceIndex,
        IMajorRecordGetter record,
        CancellationToken cancellationToken)
    {
        var queryMatches = record.FormKey.ToString().Contains(request.Query, StringComparison.OrdinalIgnoreCase) ||
            (record.EditorID?.Contains(request.Query, StringComparison.OrdinalIgnoreCase) ?? false);
        if (!queryMatches)
        {
            return false;
        }

        return request.Scope != RecordScope.WinningOverrides ||
            !HasLaterContext(sourceIndex, record, cancellationToken);
    }

    /// <summary>Checks later native plugins directly so winning resolution does not require a transient record index.</summary>
    /// <param name="sourceIndex">The current source index.</param>
    /// <param name="record">The current native record whose family and origin identity are checked.</param>
    /// <param name="cancellationToken">A token observed during later group scans.</param>
    /// <returns><see langword="true"/> when a later containing plugin overrides the same FormKey.</returns>
    private bool HasLaterContext(
        int sourceIndex,
        IMajorRecordGetter record,
        CancellationToken cancellationToken)
    {
        for (var laterIndex = sourceIndex + 1; laterIndex < Sources.Count; laterIndex++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var laterMod = Sources[laterIndex].Mod;
            var topLevelResult = HasTopLevelContext(laterMod, record);
            if (topLevelResult.HasValue)
            {
                if (topLevelResult.Value)
                {
                    return true;
                }

                continue;
            }

            foreach (var laterRecord in laterMod.EnumerateMajorRecords())
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (laterRecord.FormKey == record.FormKey)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>Uses a native top-level group lookup when the record family exposes one.</summary>
    /// <param name="mod">The later native plugin to inspect.</param>
    /// <param name="record">The earlier native context that supplies the registered family and FormKey.</param>
    /// <returns><see langword="true"/> or <see langword="false"/> for an available group; <see langword="null"/> when direct lookup is unsupported.</returns>
    private static bool? HasTopLevelContext(IModGetter mod, IMajorRecordGetter record)
    {
        return HasTopLevelContext(mod, record.Registration.GetterType, record.FormKey);
    }

    /// <summary>Uses a native top-level group lookup for an explicit getter family and identity.</summary>
    /// <param name="mod">The native plugin to inspect.</param>
    /// <param name="recordGetterType">The registered native getter interface for the requested family.</param>
    /// <param name="formKey">The native identity to locate in the family group.</param>
    /// <returns><see langword="true"/> or <see langword="false"/> for an available group; <see langword="null"/> when direct lookup is unsupported.</returns>
    private static bool? HasTopLevelContext(
        IModGetter mod,
        Type recordGetterType,
        FormKey formKey)
    {
        try
        {
            var topLevelGroup = mod.TryGetTopLevelGroup(recordGetterType);
            return topLevelGroup?.ContainsKey(formKey);
        }
        catch (ArgumentException)
        {
            return null;
        }
        catch (NotSupportedException)
        {
            return null;
        }
    }

    /// <summary>Reads a singular native context while preserving deletion and family status.</summary>
    /// <param name="context">The selected borrowed native context.</param>
    /// <param name="request">The exact context-selection request.</param>
    /// <param name="cancellationToken">A token checked around copying and direct-link diagnostics.</param>
    /// <returns>The contextual detached record read.</returns>
    private EngineResult<NativeRecordRead> ReadContext(
        NativeRecordContext context,
        ReferenceRequest request,
        CancellationToken cancellationToken)
    {
        var recordType = GetRecordType(context.Record);
        if (string.IsNullOrWhiteSpace(recordType))
        {
            return EngineResult<NativeRecordRead>.Success(CreateRead(
                ReferenceResolutionStatus.UnknownFamily,
                context,
                request,
                null,
                null));
        }

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var copy = context.Source.CreateDetachedCopy(context.Record);
            cancellationToken.ThrowIfCancellationRequested();
            var status = context.Record.IsDeleted
                ? ReferenceResolutionStatus.Deleted
                : ReferenceResolutionStatus.Resolved;
            var warnings = CreateMissingReferenceWarnings(context.Record, cancellationToken);
            return EngineResult<NativeRecordRead>.Success(CreateRead(
                status,
                context,
                request,
                recordType,
                copy),
                warnings: warnings);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (NotSupportedException)
        {
            return EngineResult<NativeRecordRead>.Success(CreateRead(
                ReferenceResolutionStatus.Unsupported,
                context,
                request,
                recordType,
                null));
        }
    }

    /// <summary>Creates one contextual native read with containing-plugin provenance.</summary>
    /// <param name="status">The selected resolution status.</param>
    /// <param name="context">The singular native record context.</param>
    /// <param name="request">The exact context-selection request.</param>
    /// <param name="recordType">The registered record family when known.</param>
    /// <param name="record">The detached native record for resolved or deleted status.</param>
    /// <returns>The immutable contextual native record read.</returns>
    private static NativeRecordRead CreateRead(
        ReferenceResolutionStatus status,
        NativeRecordContext context,
        ReferenceRequest request,
        string? recordType,
        IMajorRecordGetter? record)
    {
        return new NativeRecordRead(
            new FormListContext(
                request,
                status,
                context.Source.ModKey,
                context.Source.Path,
                context.Source.LoadOrderIndex,
                context.Source.Role),
            recordType,
            record);
    }

    /// <summary>Collects one warning for each direct native link that has no live winning target.</summary>
    /// <param name="record">The selected borrowed native FormList record.</param>
    /// <param name="cancellationToken">A token observed while visiting links and native targets.</param>
    /// <returns>Immutable missing-reference warnings in direct-link order.</returns>
    private IReadOnlyList<EngineWarning> CreateMissingReferenceWarnings(
        IMajorRecordGetter record,
        CancellationToken cancellationToken)
    {
        var references = EnumerateDirectLinks(record);
        var unresolvedKeys = references
            .Select(reference => reference.FormKey)
            .ToHashSet();
        var liveByFormKey = new Dictionary<FormKey, bool>();
        for (var sourceIndex = Sources.Count - 1; sourceIndex >= 0 && unresolvedKeys.Count > 0; sourceIndex--)
        {
            cancellationToken.ThrowIfCancellationRequested();
            foreach (var candidate in Sources[sourceIndex].Mod.EnumerateMajorRecords())
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!unresolvedKeys.Remove(candidate.FormKey))
                {
                    continue;
                }

                liveByFormKey.Add(candidate.FormKey, !candidate.IsDeleted);
                if (unresolvedKeys.Count == 0)
                {
                    break;
                }
            }
        }

        var warnings = new List<EngineWarning>();
        foreach (var reference in references)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (liveByFormKey.GetValueOrDefault(reference.FormKey))
            {
                continue;
            }

            var location = reference.FieldPath is null ? "an unidentified direct field" : reference.FieldPath;
            warnings.Add(new EngineWarning(
                "missing-native-reference",
                $"Native reference {reference.FormKey} at {location} does not resolve to a live record in the current workspace view."));
        }

        return Array.AsReadOnly(warnings.ToArray());
    }

    /// <summary>Enumerates non-null native link targets with stable fallback positions when no game-aware visitor is supplied.</summary>
    /// <param name="record">The native record whose direct links are inspected.</param>
    /// <returns>Immutable real link targets in native enumeration order; native null sentinels are omitted.</returns>
    private static IReadOnlyList<NativeFormLinkReference> EnumerateDefaultDirectLinks(IMajorRecordGetter record)
    {
        var references = new List<NativeFormLinkReference>();
        var index = 0;
        foreach (var link in record.EnumerateFormLinks(false))
        {
            if (link.FormKeyNullable is { } formKey
                && !formKey.IsNull)
            {
                references.Add(new NativeFormLinkReference(formKey, $"FormLinks[{index}]"));
            }

            index++;
        }

        return Array.AsReadOnly(references.ToArray());
    }

    /// <summary>Reads the generated native record-family registration rather than guessing from runtime implementation names.</summary>
    /// <param name="record">The native record getter.</param>
    /// <returns>The stable registered family name, or <see langword="null"/> when unavailable.</returns>
    private static string? GetRecordType(IMajorRecordGetter record)
    {
        return record.Registration?.Name;
    }

    /// <summary>Selects source positions in the deterministic direction required by the requested scope.</summary>
    /// <param name="scope">The requested native record view.</param>
    /// <param name="containingModKey">An optional concrete containing-plugin filter.</param>
    /// <returns>A private source-index array that cannot expose or reorder caller-owned state.</returns>
    private IReadOnlyList<int> GetSourceIndices(RecordScope scope, ModKey? containingModKey)
    {
        IEnumerable<int> indices = Enumerable.Range(0, Sources.Count);
        indices = scope switch
        {
            RecordScope.WinningOverrides => indices.Reverse(),
            RecordScope.AllContexts => indices,
            RecordScope.Source => indices.Where(index => Sources[index].Role == PluginRole.Source),
            RecordScope.StagedOutput => indices.Where(index => Sources[index].Role == PluginRole.Output),
            _ => Array.Empty<int>()
        };

        if (containingModKey.HasValue)
        {
            indices = indices.Where(index => Sources[index].ModKey == containingModKey.Value);
        }

        return Array.AsReadOnly(indices.ToArray());
    }

    /// <summary>Associates one borrowed record with the native plugin context that contains it.</summary>
    private readonly struct NativeRecordContext
    {
        /// <summary>Initializes a borrowed record context.</summary>
        /// <param name="source">The containing native plugin source.</param>
        /// <param name="record">The borrowed native record.</param>
        internal NativeRecordContext(NativeReferenceSource source, IMajorRecordGetter record)
        {
            Source = source;
            Record = record;
        }

        /// <summary>Gets the containing native plugin source.</summary>
        internal NativeReferenceSource Source { get; }

        /// <summary>Gets the borrowed native record.</summary>
        internal IMajorRecordGetter Record { get; }
    }
}
