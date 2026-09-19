using CreationsForge.Core.Enums;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Adapts one game's typed Mutagen records and writers to the shared UI-neutral plugin workspace.
/// </summary>
public interface IFormListGameAdapter
{
    /// <summary>Gets the supported CreationsForge game.</summary>
    SupportedGame Game { get; }

    /// <summary>Gets the stateless game-specific typed FormList view writer and semantic comparer.</summary>
    IFormListInspector Inspector { get; }

    /// <summary>Gets the stateless native major-record field inspector and semantic comparer.</summary>
    IMajorRecordInspector MajorRecordInspector { get; }

    /// <summary>Determines whether this adapter supports an exact plugin release selection.</summary>
    /// <param name="release">The requested Mutagen release.</param>
    /// <returns><see langword="true"/> when this adapter can safely open and write that release.</returns>
    bool SupportsRelease(GameRelease release);

    /// <summary>Opens the explicitly ordered plugin source, load-order, and string inputs.</summary>
    /// <param name="request">The validated canonical request whose plugin resources the returned handle owns.</param>
    /// <param name="cancellationToken">A token that cancels source acquisition before ownership transfers to a workspace.</param>
    /// <returns>The independently disposable source handle and deterministic plugin baseline when opening succeeds.</returns>
    ValueTask<EngineResult<PluginSourceOpenResult>> OpenSourcesAsync(
        WorkspaceOpenRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Opens or creates a separate plugin output without publishing partial state to the workspace.</summary>
    /// <param name="sources">The borrowed source lifetime against which the output is opened.</param>
    /// <param name="request">The canonical guarded output selection.</param>
    /// <param name="cancellationToken">A token that cancels output acquisition before the returned handle is published.</param>
    /// <returns>An independently disposable complete output state and its observed artifact baseline.</returns>
    ValueTask<EngineResult<PluginOutputOpenResult>> OpenOutputAsync(
        IPluginSourceSet sources,
        SelectOutputRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Reopens a selected plugin output against its exact expected file-set baseline.</summary>
    /// <param name="sources">The borrowed source lifetime against which the output is reopened.</param>
    /// <param name="association">The exact selected plugin identity and output mode.</param>
    /// <param name="expectedBaseline">The complete artifact observation that must still match the destination.</param>
    /// <param name="cancellationToken">A token that cancels reopening before the returned handle is published.</param>
    /// <returns>An independently disposable complete output state when the baseline remains current and plugin reopen succeeds.</returns>
    ValueTask<EngineResult<PluginOutputOpenResult>> ReopenOutputAsync(
        IPluginSourceSet sources,
        OutputAssociation association,
        OutputArtifactSetBaseline expectedBaseline,
        CancellationToken cancellationToken = default);

    /// <summary>Creates an independently disposable deep plugin copy of complete staged output state.</summary>
    /// <param name="output">The current complete plugin output state.</param>
    /// <param name="cancellationToken">A token checked immediately before and after the uninterruptible plugin copy.</param>
    /// <returns>A candidate containing all edited and unrelated records.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    IPluginOutputState CloneOutput(
        IPluginOutputState output,
        CancellationToken cancellationToken);

    /// <summary>Allocates, overrides, or selects an existing FormList only inside an unpublished output candidate.</summary>
    /// <param name="sources">The borrowed plugin source lifetime used to resolve an override origin.</param>
    /// <param name="candidate">The unpublished complete output candidate to mutate.</param>
    /// <param name="request">The guarded new-record, override, or existing-output request.</param>
    /// <param name="cancellationToken">A token checked immediately before and after the uninterruptible plugin allocation or override copy.</param>
    /// <returns>The stable edit identity assigned inside the candidate, or a typed rejection.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    EngineResult<RecordEditIdentity> BeginEdit(
        IPluginSourceSet sources,
        IPluginOutputState candidate,
        BeginEditRequest request,
        CancellationToken cancellationToken);

    /// <summary>Defensively copies and canonically fingerprints every reachable field of a typed edit before an asynchronous boundary.</summary>
    /// <param name="edit">The caller-owned typed edit.</param>
    /// <returns>An immutable prepared payload, including deterministic validation failure when invalid.</returns>
    PreparedFormListEdit PrepareEdit(FormListEdit edit);

    /// <summary>Applies one prepared typed edit only to an unpublished output candidate.</summary>
    /// <param name="sources">The borrowed plugin source lifetime used for reference validation.</param>
    /// <param name="candidate">The unpublished complete output candidate to mutate.</param>
    /// <param name="target">The FormList identity associated with the staged edit.</param>
    /// <param name="edit">The immutable adapter-prepared typed payload.</param>
    /// <returns>Whether the successful command changed the candidate and any warnings, or a typed rejection that leaves publication to the workspace.</returns>
    EngineResult<RecordEditMutationResult> ApplyEdit(
        IPluginSourceSet sources,
        IPluginOutputState candidate,
        FormKey target,
        PreparedFormListEdit edit);

    /// <summary>Applies a complete typed GameSettingFloat edit only inside an unpublished output candidate.</summary>
    /// <param name="sources">The exact immutable source set used for game validation.</param>
    /// <param name="candidate">The complete unpublished native output to mutate.</param>
    /// <param name="target">The exact staged GameSettingFloat identity.</param>
    /// <param name="request">The immutable replacement values.</param>
    /// <param name="cancellationToken">A token observed during native record selection.</param>
    /// <returns>The mutation outcome or a typed rejection without publishing the candidate.</returns>
    EngineResult<RecordEditMutationResult> ApplyGameSettingFloatEdit(
        IPluginSourceSet sources,
        IPluginOutputState candidate,
        FormKey target,
        GameSettingFloatEditRequest request,
        CancellationToken cancellationToken);

    /// <summary>Enumerates participating plugins without consulting persistent imported state.</summary>
    /// <param name="sources">The borrowed plugin source lifetime.</param>
    /// <param name="output">The borrowed selected output state, or <see langword="null"/> before output selection.</param>
    /// <param name="cancellationToken">A token observed while enumerating plugins.</param>
    /// <returns>Immutable plugin summaries in deterministic load-order and role order.</returns>
    EngineResult<IReadOnlyList<PluginSummary>> ListPlugins(
        IPluginSourceSet sources,
        IPluginOutputState? output,
        CancellationToken cancellationToken);

    /// <summary>Enumerates FormLists in the requested scope without creating a persistent index.</summary>
    /// <param name="sources">The borrowed plugin source lifetime.</param>
    /// <param name="output">The borrowed selected output state, or <see langword="null"/> before output selection.</param>
    /// <param name="scope">The source, winning-override, or staged-output view to enumerate.</param>
    /// <param name="cancellationToken">A token observed while enumerating FormLists.</param>
    /// <returns>Immutable lightweight summaries in deterministic plugin order.</returns>
    EngineResult<IReadOnlyList<FormListSummary>> ListFormLists(
        IPluginSourceSet sources,
        IPluginOutputState? output,
        RecordScope scope,
        CancellationToken cancellationToken);

    /// <summary>Reads one exact FormList context as detached plugin state, including a selected deleted context.</summary>
    /// <param name="sources">The borrowed plugin source lifetime.</param>
    /// <param name="output">The borrowed selected output state, or <see langword="null"/> before output selection.</param>
    /// <param name="request">The exact record identity, scope, and optional containing-plugin selection.</param>
    /// <param name="cancellationToken">A token observed while selecting and copying the record.</param>
    /// <returns>The contextual detached plugin read, or a typed adapter failure.</returns>
    EngineResult<RecordRead> ReadFormListContext(
        IPluginSourceSet sources,
        IPluginOutputState? output,
        ReferenceRequest request,
        CancellationToken cancellationToken);

    /// <summary>Reads one exact major-record context without restricting its native Mutagen family.</summary>
    /// <param name="sources">The borrowed plugin source lifetime.</param>
    /// <param name="output">The borrowed selected output state, or <see langword="null"/> before output selection.</param>
    /// <param name="request">The exact record identity, scope, and optional containing-plugin selection.</param>
    /// <param name="cancellationToken">A token observed during selection, copying, and direct-link diagnostics.</param>
    /// <returns>A contextual detached native record for resolved or deleted contexts, or an explicit unavailable status.</returns>
    EngineResult<RecordRead> ReadRecordContext(
        IPluginSourceSet sources,
        IPluginOutputState? output,
        ReferenceRequest request,
        CancellationToken cancellationToken);

    /// <summary>Searches references in bounded deterministic pages without building a persistent index.</summary>
    /// <param name="sources">The borrowed plugin source lifetime.</param>
    /// <param name="output">The borrowed selected output state, or <see langword="null"/> before output selection.</param>
    /// <param name="request">The bounded query and optional continuation token.</param>
    /// <param name="workspaceId">The workspace identity used to bind continuation tokens to one plugin state owner.</param>
    /// <param name="revision">The current workspace revision used to bind continuation tokens to one exact staged state.</param>
    /// <param name="cancellationToken">A token observed while enumerating records.</param>
    /// <returns>One immutable deterministic page and a continuation token when more matches remain.</returns>
    EngineResult<ReferenceSearchPage> SearchReferences(
        IPluginSourceSet sources,
        IPluginOutputState? output,
        ReferenceSearchRequest request,
        Guid workspaceId,
        WorkspaceRevision revision,
        CancellationToken cancellationToken);

    /// <summary>Visits every winning major record's identity metadata without restarting a bounded search for each page.</summary>
    /// <param name="sources">The borrowed plugin source lifetime.</param>
    /// <param name="output">The optional staged output appended after the source load order.</param>
    /// <param name="onRecord">Receives each lightweight winning context.</param>
    /// <param name="onProgress">Receives the current plugin and cumulative record count periodically.</param>
    /// <param name="cancellationToken">A token that cancels the scan.</param>
    /// <returns>The number of delivered summaries or a typed source-state failure.</returns>
    EngineResult<int> VisitWinningRecordSummaries(
        IPluginSourceSet sources,
        IPluginOutputState? output,
        Action<ReferenceSearchMatch> onRecord,
        Action<ModKey, int>? onProgress,
        CancellationToken cancellationToken);

    /// <summary>Resolves a record identity and returns only a detached plugin deep copy when supported.</summary>
    /// <param name="sources">The borrowed plugin source lifetime.</param>
    /// <param name="output">The borrowed selected output state, or <see langword="null"/> before output selection.</param>
    /// <param name="request">The exact record identity and record view to resolve.</param>
    /// <param name="cancellationToken">A token observed while enumerating records.</param>
    /// <returns>The resolution status and optional detached record getter.</returns>
    EngineResult<ReferenceResolution> ResolveReference(
        IPluginSourceSet sources,
        IPluginOutputState? output,
        ReferenceRequest request,
        CancellationToken cancellationToken);

    /// <summary>Builds a detached preview of all staged FormList edits without writing destination files.</summary>
    /// <param name="sources">The borrowed plugin source lifetime.</param>
    /// <param name="output">The borrowed complete staged output state.</param>
    /// <returns>An immutable preview of comparisons, unresolved references, and warnings.</returns>
    EngineResult<WorkspacePreview> Preview(
        IPluginSourceSet sources,
        IPluginOutputState output);

    /// <summary>Writes a private complete output set and reopens it through the game adapter before any destination commit.</summary>
    /// <param name="sources">The borrowed plugin source lifetime needed for writing and reopen validation.</param>
    /// <param name="output">The borrowed complete staged output state to serialize.</param>
    /// <param name="request">The private staging directory and exact output identity.</param>
    /// <param name="cancellationToken">A token that cancels private staging and validation before destination mutation.</param>
    /// <returns>An independently disposable validated changed set with explicit staged-to-destination mappings, an unchanged result with no staged lifetime or mappings, or a typed pre-commit failure.</returns>
    /// <remarks>An unchanged result is valid only for an existing output after complete plugin and FormList multilingual equality is proved. Edited non-FormList targets also receive native field verification after reopen. A new output must produce a changed set. Existing localized material that cannot be rewritten losslessly fails with <see cref="EngineErrorCode.UnsupportedInput"/>.</remarks>
    ValueTask<EngineResult<StagedPluginOutputSet>> WriteAndValidateAsync(
        IPluginSourceSet sources,
        IPluginOutputState output,
        PluginWriteRequest request,
        CancellationToken cancellationToken = default);
}
