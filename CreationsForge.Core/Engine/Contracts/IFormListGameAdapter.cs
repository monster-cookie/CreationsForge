using CreationsForge.Core.Enums;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Adapts one game's typed Mutagen records and writers to the shared UI-neutral FormList engine.
/// </summary>
public interface IFormListGameAdapter
{
    /// <summary>Gets the supported CreationsForge game.</summary>
    SupportedGame Game { get; }

    /// <summary>Gets the stateless game-specific typed FormList view writer and semantic comparer.</summary>
    IFormListNativeInspector Inspector { get; }

    /// <summary>Determines whether this adapter supports an exact native release selection.</summary>
    /// <param name="release">The requested Mutagen release.</param>
    /// <returns><see langword="true"/> when this adapter can safely open and write that release.</returns>
    bool SupportsRelease(GameRelease release);

    /// <summary>Opens the explicitly ordered native source, load-order, and string inputs.</summary>
    /// <param name="request">The validated canonical request whose native resources the returned handle owns.</param>
    /// <param name="cancellationToken">A token that cancels source acquisition before ownership transfers to a workspace.</param>
    /// <returns>The independently disposable source handle and deterministic native baseline when opening succeeds.</returns>
    ValueTask<EngineResult<NativeSourceOpenResult>> OpenSourcesAsync(
        WorkspaceOpenRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Opens or creates a separate native output without publishing partial state to the workspace.</summary>
    /// <param name="sources">The borrowed source lifetime against which the output is opened.</param>
    /// <param name="request">The canonical guarded output selection.</param>
    /// <param name="cancellationToken">A token that cancels output acquisition before the returned handle is published.</param>
    /// <returns>An independently disposable complete output state and its observed artifact baseline.</returns>
    ValueTask<EngineResult<NativeOutputOpenResult>> OpenOutputAsync(
        INativeSourceSet sources,
        SelectOutputRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Reopens a selected native output against its exact expected file-set baseline.</summary>
    /// <param name="sources">The borrowed source lifetime against which the output is reopened.</param>
    /// <param name="association">The exact selected plugin identity and output mode.</param>
    /// <param name="expectedBaseline">The complete artifact observation that must still match the destination.</param>
    /// <param name="cancellationToken">A token that cancels reopening before the returned handle is published.</param>
    /// <returns>An independently disposable complete output state when the baseline remains current and native reopen succeeds.</returns>
    ValueTask<EngineResult<NativeOutputOpenResult>> ReopenOutputAsync(
        INativeSourceSet sources,
        OutputAssociation association,
        OutputArtifactSetBaseline expectedBaseline,
        CancellationToken cancellationToken = default);

    /// <summary>Creates an independently disposable deep native copy of complete staged output state.</summary>
    /// <param name="output">The current complete native output state.</param>
    /// <param name="cancellationToken">A token checked immediately before and after the uninterruptible native copy.</param>
    /// <returns>A candidate containing all edited and unrelated native records.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    INativeOutputState CloneOutput(
        INativeOutputState output,
        CancellationToken cancellationToken);

    /// <summary>Allocates, overrides, or selects an existing FormList only inside an unpublished output candidate.</summary>
    /// <param name="sources">The borrowed native source lifetime used to resolve an override origin.</param>
    /// <param name="candidate">The unpublished complete output candidate to mutate.</param>
    /// <param name="request">The guarded new-record, override, or existing-output request.</param>
    /// <param name="cancellationToken">A token checked immediately before and after the uninterruptible native allocation or override copy.</param>
    /// <returns>The stable edit identity assigned inside the candidate, or a typed rejection.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    EngineResult<NativeEditIdentity> BeginEdit(
        INativeSourceSet sources,
        INativeOutputState candidate,
        BeginEditRequest request,
        CancellationToken cancellationToken);

    /// <summary>Defensively copies and canonically fingerprints every reachable field of a typed edit before an asynchronous boundary.</summary>
    /// <param name="edit">The caller-owned typed edit.</param>
    /// <returns>An immutable prepared payload, including deterministic validation failure when invalid.</returns>
    PreparedFormListEdit PrepareEdit(FormListEdit edit);

    /// <summary>Applies one prepared typed edit only to an unpublished output candidate.</summary>
    /// <param name="sources">The borrowed native source lifetime used for reference validation.</param>
    /// <param name="candidate">The unpublished complete output candidate to mutate.</param>
    /// <param name="target">The native FormList identity associated with the staged edit.</param>
    /// <param name="edit">The immutable adapter-prepared typed payload.</param>
    /// <returns>Whether the successful command changed the candidate and any warnings, or a typed rejection that leaves publication to the workspace.</returns>
    EngineResult<NativeEditMutationResult> ApplyEdit(
        INativeSourceSet sources,
        INativeOutputState candidate,
        FormKey target,
        PreparedFormListEdit edit);

    /// <summary>Enumerates participating native plugins without consulting persistent imported state.</summary>
    /// <param name="sources">The borrowed native source lifetime.</param>
    /// <param name="output">The borrowed selected output state, or <see langword="null"/> before output selection.</param>
    /// <param name="cancellationToken">A token observed while enumerating native plugins.</param>
    /// <returns>Immutable plugin summaries in deterministic load-order and role order.</returns>
    EngineResult<IReadOnlyList<PluginSummary>> ListPlugins(
        INativeSourceSet sources,
        INativeOutputState? output,
        CancellationToken cancellationToken);

    /// <summary>Enumerates native FormLists in the requested scope without creating a persistent index.</summary>
    /// <param name="sources">The borrowed native source lifetime.</param>
    /// <param name="output">The borrowed selected output state, or <see langword="null"/> before output selection.</param>
    /// <param name="scope">The source, winning-override, or staged-output view to enumerate.</param>
    /// <param name="cancellationToken">A token observed while enumerating native FormLists.</param>
    /// <returns>Immutable lightweight summaries in deterministic native order.</returns>
    EngineResult<IReadOnlyList<FormListSummary>> ListFormLists(
        INativeSourceSet sources,
        INativeOutputState? output,
        RecordScope scope,
        CancellationToken cancellationToken);

    /// <summary>Reads one exact FormList context as detached native state, including a selected deleted context.</summary>
    /// <param name="sources">The borrowed native source lifetime.</param>
    /// <param name="output">The borrowed selected output state, or <see langword="null"/> before output selection.</param>
    /// <param name="request">The exact native identity, scope, and optional containing-plugin selection.</param>
    /// <param name="cancellationToken">A token observed while selecting and copying the native record.</param>
    /// <returns>The contextual detached native read, or a typed adapter failure.</returns>
    EngineResult<NativeRecordRead> ReadFormListContext(
        INativeSourceSet sources,
        INativeOutputState? output,
        ReferenceRequest request,
        CancellationToken cancellationToken);

    /// <summary>Searches native references in bounded deterministic pages without building a persistent index.</summary>
    /// <param name="sources">The borrowed native source lifetime.</param>
    /// <param name="output">The borrowed selected output state, or <see langword="null"/> before output selection.</param>
    /// <param name="request">The bounded query and optional continuation token.</param>
    /// <param name="workspaceId">The workspace identity used to bind continuation tokens to one native state owner.</param>
    /// <param name="revision">The current workspace revision used to bind continuation tokens to one exact staged state.</param>
    /// <param name="cancellationToken">A token observed while enumerating native records.</param>
    /// <returns>One immutable deterministic page and a continuation token when more matches remain.</returns>
    EngineResult<ReferenceSearchPage> SearchReferences(
        INativeSourceSet sources,
        INativeOutputState? output,
        ReferenceSearchRequest request,
        Guid workspaceId,
        WorkspaceRevision revision,
        CancellationToken cancellationToken);

    /// <summary>Resolves a native identity and returns only a detached native deep copy when supported.</summary>
    /// <param name="sources">The borrowed native source lifetime.</param>
    /// <param name="output">The borrowed selected output state, or <see langword="null"/> before output selection.</param>
    /// <param name="request">The exact native identity and record view to resolve.</param>
    /// <param name="cancellationToken">A token observed while enumerating native records.</param>
    /// <returns>The resolution status and optional detached native getter.</returns>
    EngineResult<ReferenceResolution> ResolveReference(
        INativeSourceSet sources,
        INativeOutputState? output,
        ReferenceRequest request,
        CancellationToken cancellationToken);

    /// <summary>Builds a detached preview of all staged FormList edits without writing destination files.</summary>
    /// <param name="sources">The borrowed native source lifetime.</param>
    /// <param name="output">The borrowed complete staged output state.</param>
    /// <returns>An immutable preview of comparisons, unresolved references, and warnings.</returns>
    EngineResult<WorkspacePreview> Preview(
        INativeSourceSet sources,
        INativeOutputState output);

    /// <summary>Writes a private complete output set and reopens it natively before any destination commit.</summary>
    /// <param name="sources">The borrowed native source lifetime needed for writing and reopen validation.</param>
    /// <param name="output">The borrowed complete staged output state to serialize.</param>
    /// <param name="request">The private staging directory and exact output identity.</param>
    /// <param name="cancellationToken">A token that cancels private staging and validation before destination mutation.</param>
    /// <returns>An independently disposable validated changed set with explicit staged-to-destination mappings, an unchanged result with no staged lifetime or mappings, or a typed pre-commit failure.</returns>
    /// <remarks>An unchanged result is valid only for an existing output after complete native and FormList multilingual equality is proved. A new output must produce a changed set. Existing localized material that cannot be rewritten losslessly fails with <see cref="EngineErrorCode.UnsupportedInput"/>.</remarks>
    ValueTask<EngineResult<NativeStagedOutputSet>> WriteAndValidateAsync(
        INativeSourceSet sources,
        INativeOutputState output,
        NativeWriteRequest request,
        CancellationToken cancellationToken = default);
}
