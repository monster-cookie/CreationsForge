using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Owns one isolated native source/output lifetime and serializes all operations through that workspace.
/// </summary>
public interface IFormListWorkspace : IAsyncDisposable
{
    /// <summary>Gets the caller-assigned workspace identifier.</summary>
    Guid WorkspaceId { get; }

    /// <summary>Gets the current exact native baseline and monotonic mutation sequence.</summary>
    WorkspaceRevision Revision { get; }

    /// <summary>Gets an atomic snapshot indicating whether ordinary output operations are ready or await recovery or reopen.</summary>
    OutputSynchronizationState OutputSynchronization { get; }

    /// <summary>Reads game, release, output metadata, synchronization, and revision as one serialized atomic snapshot.</summary>
    /// <param name="cancellationToken">A token that cancels before the snapshot is returned.</param>
    /// <returns>The immutable workspace state, including blocked synchronization metadata without native record traversal.</returns>
    ValueTask<EngineResult<WorkspaceState>> ReadStateAsync(
        CancellationToken cancellationToken = default);

    /// <summary>Selects or creates a separate native output transactionally.</summary>
    /// <param name="request">The idempotent output selection guarded by the current revision.</param>
    /// <param name="cancellationToken">A token that cancels before newly opened state is published.</param>
    /// <returns>The selected output association, exact artifact baseline, and resulting revision.</returns>
    ValueTask<EngineResult<OutputSelectionReceipt>> SelectOutputAsync(
        SelectOutputRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Lists participating native plugins and their source, load-order, or output roles.</summary>
    /// <param name="cancellationToken">A token that cancels the serialized native read.</param>
    /// <returns>Immutable plugin summaries with the unchanged workspace revision.</returns>
    ValueTask<EngineResult<IReadOnlyList<PluginSummary>>> ListPluginsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>Lists native FormLists in the requested scope.</summary>
    /// <param name="scope">The source, winning-override, or staged-output view to enumerate.</param>
    /// <param name="cancellationToken">A token that cancels the serialized native read.</param>
    /// <returns>Immutable lightweight FormList summaries with the unchanged workspace revision.</returns>
    ValueTask<EngineResult<IReadOnlyList<FormListSummary>>> ListFormListsAsync(
        RecordScope scope,
        CancellationToken cancellationToken = default);

    /// <summary>Reads one FormList as a detached native deep copy.</summary>
    /// <param name="formKey">The exact native FormList identity to read.</param>
    /// <param name="scope">The native view from which to resolve the record.</param>
    /// <param name="cancellationToken">A token that cancels the serialized native read.</param>
    /// <returns>A detached native getter or a typed resolution failure with the unchanged revision.</returns>
    ValueTask<EngineResult<IMajorRecordGetter>> ReadFormListAsync(
        FormKey formKey,
        RecordScope scope,
        CancellationToken cancellationToken = default);

    /// <summary>Reads one explicitly selected FormList context as a detached complete typed JSON view.</summary>
    /// <param name="request">The exact native identity, scope, and optional containing-plugin selection.</param>
    /// <param name="cancellationToken">A token that cancels the serialized native read and typed view traversal.</param>
    /// <returns>The contextual detached view with typed fields for resolved or deleted contexts and explicit status without fields for unavailable or unresolved contexts.</returns>
    ValueTask<EngineResult<FormListReadView>> ReadFormListViewAsync(
        ReferenceRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Searches native references in bounded deterministic pages.</summary>
    /// <param name="request">The bounded query and optional continuation token.</param>
    /// <param name="cancellationToken">A token that cancels the serialized native search.</param>
    /// <returns>One immutable page and continuation state with the unchanged revision.</returns>
    ValueTask<EngineResult<ReferenceSearchPage>> SearchReferencesAsync(
        ReferenceSearchRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Resolves a native identity and returns a detached native copy when supported.</summary>
    /// <param name="request">The exact native identity and record view to resolve.</param>
    /// <param name="cancellationToken">A token that cancels the serialized native resolution.</param>
    /// <returns>The resolution status and optional detached native getter with the unchanged revision.</returns>
    ValueTask<EngineResult<ReferenceResolution>> ResolveReferenceAsync(
        ReferenceRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Begins an allocation, override, or existing-output edit against an unpublished output candidate.</summary>
    /// <param name="request">The idempotent allocation, override, or existing-output selection guarded by the current revision.</param>
    /// <param name="cancellationToken">A token that cancels before the candidate is published.</param>
    /// <returns>The stable staged edit identity and resulting revision, or a typed rejection. Re-selecting a target already staged in the workspace returns that session's original role and origin identity so its preview baseline remains unchanged.</returns>
    ValueTask<EngineResult<EditReceipt>> BeginEditAsync(
        BeginEditRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Applies one prepared named typed edit against an unpublished output candidate.</summary>
    /// <param name="request">The idempotent typed command and staged edit identity guarded by the current revision.</param>
    /// <param name="cancellationToken">A token that cancels before the candidate is published.</param>
    /// <returns>The operation receipt and resulting revision, or a typed rejection.</returns>
    ValueTask<EngineResult<OperationReceipt>> ApplyFormListEditAsync(
        FormListEditRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Compares detached native before-and-after copies for one FormList.</summary>
    /// <param name="request">The explicit prior and resulting context selections for the same FormList identity.</param>
    /// <param name="cancellationToken">A token that cancels the serialized native comparison.</param>
    /// <returns>Detached typed before-and-after views, exact contexts, and immutable semantic changes with the unchanged revision.</returns>
    ValueTask<EngineResult<FormListComparison>> CompareFormListAsync(
        CompareFormListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Previews all staged FormList changes without writing destination files.</summary>
    /// <param name="cancellationToken">A token that cancels the serialized native preview.</param>
    /// <returns>An immutable preview with the unchanged workspace revision.</returns>
    ValueTask<EngineResult<WorkspacePreview>> PreviewAsync(
        CancellationToken cancellationToken = default);

    /// <summary>Saves the complete selected plugin-and-strings set under guarded recoverable semantics.</summary>
    /// <param name="request">The idempotent save request guarded by the current revision and exact output baseline.</param>
    /// <param name="cancellationToken">A token honored before destination mutation; after that boundary the result reports exact commit knowledge.</param>
    /// <returns>The committed, rejected, incomplete, or unknown outcome and any recovery evidence.</returns>
    ValueTask<SaveResult> SaveAsync(
        SaveRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Explicitly adopts terminal save-recovery evidence after revalidating it under the output-directory lease.</summary>
    /// <param name="request">The idempotent resume-or-reopen decision guarded by the current live workspace revision.</param>
    /// <param name="cancellationToken">A token that cancels before recovered state is published.</param>
    /// <returns>The resumed or reopened output association, terminal baseline, and resulting revision.</returns>
    ValueTask<EngineResult<OutputSelectionReceipt>> ResolveOutputRecoveryAsync(
        ResolveOutputRecoveryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Reopens the selected output from its exact expected baseline and replaces staged state transactionally.</summary>
    /// <param name="request">The idempotent reopen request guarded by the current revision and output baseline.</param>
    /// <param name="cancellationToken">A token that cancels before reopened state is published.</param>
    /// <returns>The reopened output association, exact artifact baseline, and resulting revision.</returns>
    ValueTask<EngineResult<OutputSelectionReceipt>> ReopenOutputAsync(
        ReopenOutputRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Discards staged changes by reopening the selected output baseline transactionally.</summary>
    /// <param name="request">The idempotent discard request guarded by the current revision and output baseline.</param>
    /// <param name="cancellationToken">A token that cancels before reopened destination state is published.</param>
    /// <returns>The operation receipt and resulting revision, or a typed reopen failure that preserves staged state.</returns>
    ValueTask<EngineResult<OperationReceipt>> DiscardChangesAsync(
        DiscardChangesRequest request,
        CancellationToken cancellationToken = default);
}
