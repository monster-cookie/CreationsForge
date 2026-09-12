using System.Collections.ObjectModel;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.Persistence;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.UnitTests.Engine.Integration;

/// <summary>Contains one complete recognized recovery scenario, any live original owner, and exact physical-snapshot support.</summary>
internal sealed class NativeWorkspaceRecoverySeed : IAsyncDisposable
{
    /// <summary>The live original workspace owned by a real interrupted save, cleared when disposal begins.</summary>
    private IFormListWorkspace? _originalWorkspace;

    /// <summary>Initializes immutable recovery identities, native baselines, expected classification, and optional live ownership.</summary>
    /// <param name="state">The requested journal and physical destination state.</param>
    /// <param name="originalWorkspaceId">The workspace identity recorded by the prior save.</param>
    /// <param name="saveOperationId">The operation identity recorded by the prior save.</param>
    /// <param name="saveBaseRevision">The exact workspace revision recorded by the prior save.</param>
    /// <param name="output">The complete output association shared by all owners.</param>
    /// <param name="beforeBaseline">The complete destination baseline before the prior save.</param>
    /// <param name="preparedBaseline">The complete intended destination baseline, or <see langword="null"/> when the journal has no prepared set.</param>
    /// <param name="formKey">The output FormList identity changed by the prior save.</param>
    /// <param name="beforeEditorId">The EditorID in the original output.</param>
    /// <param name="preparedEditorId">The EditorID in the intended prepared output, or <see langword="null"/> when no prepared set exists.</param>
    /// <param name="expectedRecoveryStatus">The recovery status expected from the seeded physical state.</param>
    /// <param name="expectedRecoveryErrorCode">The recovery error expected from the seeded physical state, or <see langword="null"/> for terminal evidence.</param>
    /// <param name="paths">The canonical transaction paths for the prior save.</param>
    /// <param name="originalSaveResult">The real interrupted save result for a mixed state, or <see langword="null"/> for direct terminal-state seeding.</param>
    /// <param name="originalSynchronization">The original workspace synchronization after a real interrupted save, or <see langword="null"/> for direct seeding.</param>
    /// <param name="originalPreview">The detached staged preview captured before a real interrupted save, or <see langword="null"/> for direct seeding.</param>
    /// <param name="originalWorkspace">The live original workspace after a real interrupted save, or <see langword="null"/> for direct seeding.</param>
    internal NativeWorkspaceRecoverySeed(
        NativeWorkspaceRecoveryPhysicalState state,
        Guid originalWorkspaceId,
        Guid saveOperationId,
        WorkspaceRevision saveBaseRevision,
        OutputAssociation output,
        OutputArtifactSetBaseline beforeBaseline,
        OutputArtifactSetBaseline? preparedBaseline,
        FormKey formKey,
        string beforeEditorId,
        string? preparedEditorId,
        RecoverSaveStatus expectedRecoveryStatus,
        EngineErrorCode? expectedRecoveryErrorCode,
        SaveTransactionPaths paths,
        SaveResult? originalSaveResult,
        OutputSynchronizationState? originalSynchronization,
        WorkspacePreview? originalPreview,
        IFormListWorkspace? originalWorkspace)
    {
        State = state;
        OriginalWorkspaceId = originalWorkspaceId;
        SaveOperationId = saveOperationId;
        SaveBaseRevision = saveBaseRevision;
        Output = output;
        BeforeBaseline = beforeBaseline;
        PreparedBaseline = preparedBaseline;
        FormKey = formKey;
        BeforeEditorId = beforeEditorId;
        PreparedEditorId = preparedEditorId;
        ExpectedRecoveryStatus = expectedRecoveryStatus;
        ExpectedRecoveryErrorCode = expectedRecoveryErrorCode;
        Paths = paths;
        OriginalSaveResult = originalSaveResult;
        OriginalSynchronization = originalSynchronization;
        OriginalPreview = originalPreview;
        _originalWorkspace = originalWorkspace;
    }

    /// <summary>Gets the requested journal and physical destination state.</summary>
    internal NativeWorkspaceRecoveryPhysicalState State { get; }

    /// <summary>Gets the workspace identity recorded by the prior save.</summary>
    internal Guid OriginalWorkspaceId { get; }

    /// <summary>Gets the operation identity recorded by the prior save.</summary>
    internal Guid SaveOperationId { get; }

    /// <summary>Gets the exact workspace revision recorded by the prior save.</summary>
    internal WorkspaceRevision SaveBaseRevision { get; }

    /// <summary>Gets the complete output association shared by all owners.</summary>
    internal OutputAssociation Output { get; }

    /// <summary>Gets the complete destination baseline before the prior save.</summary>
    internal OutputArtifactSetBaseline BeforeBaseline { get; }

    /// <summary>Gets the complete intended destination baseline, or <see langword="null"/> when the journal has no prepared set.</summary>
    internal OutputArtifactSetBaseline? PreparedBaseline { get; }

    /// <summary>Gets the output FormList identity changed by the prior save.</summary>
    internal FormKey FormKey { get; }

    /// <summary>Gets the EditorID in the original output.</summary>
    internal string BeforeEditorId { get; }

    /// <summary>Gets the EditorID in the intended prepared output, or <see langword="null"/> when no prepared set exists.</summary>
    internal string? PreparedEditorId { get; }

    /// <summary>Gets the recovery status expected from the seeded physical state.</summary>
    internal RecoverSaveStatus ExpectedRecoveryStatus { get; }

    /// <summary>Gets the recovery error expected from the seeded physical state, or <see langword="null"/> for terminal evidence.</summary>
    internal EngineErrorCode? ExpectedRecoveryErrorCode { get; }

    /// <summary>Gets the canonical transaction paths for the prior save.</summary>
    internal SaveTransactionPaths Paths { get; }

    /// <summary>Gets the real interrupted save result for a mixed state, or <see langword="null"/> for direct terminal-state seeding.</summary>
    internal SaveResult? OriginalSaveResult { get; }

    /// <summary>Gets the original workspace synchronization after a real interrupted save, or <see langword="null"/> for direct seeding.</summary>
    internal OutputSynchronizationState? OriginalSynchronization { get; }

    /// <summary>Gets the detached staged preview captured before a real interrupted save, or <see langword="null"/> for direct seeding.</summary>
    internal WorkspacePreview? OriginalPreview { get; }

    /// <summary>Gets the live original workspace after a real interrupted save, or <see langword="null"/> for direct seeding or after disposal.</summary>
    internal IFormListWorkspace? OriginalWorkspace => _originalWorkspace;

    /// <summary>Captures every expected destination path and every current regular transaction file with exact bytes.</summary>
    /// <returns>A stable path-ordered snapshot whose <see langword="null"/> values represent absent destination artifacts.</returns>
    internal IReadOnlyDictionary<string, byte[]?> SnapshotPhysicalArtifacts()
    {
        var comparer = OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
        var paths = BeforeBaseline.Artifacts.Select(artifact => artifact.Path)
            .Concat(PreparedBaseline?.Artifacts.Select(artifact => artifact.Path) ?? [])
            .Concat(Directory.Exists(Paths.TransactionDirectoryPath)
                ? Directory.EnumerateFiles(Paths.TransactionDirectoryPath, "*", SearchOption.AllDirectories)
                : [])
            .Distinct(comparer)
            .OrderBy(path => path, comparer);
        var snapshot = new Dictionary<string, byte[]?>(comparer);
        foreach (var path in paths)
        {
            snapshot[path] = File.Exists(path) ? File.ReadAllBytes(path) : null;
        }

        return new ReadOnlyDictionary<string, byte[]?>(snapshot);
    }

    /// <summary>Releases the live original workspace retained by a real interrupted-save scenario.</summary>
    /// <returns>The asynchronous workspace disposal, or a completed operation when no live owner remains.</returns>
    public ValueTask DisposeAsync()
    {
        return Interlocked.Exchange(ref _originalWorkspace, null)?.DisposeAsync() ?? ValueTask.CompletedTask;
    }
}
