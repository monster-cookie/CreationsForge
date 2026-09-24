using System.Diagnostics;
using CreationsForge.Engine.Records;
using CreationsForge.Engine.Workspaces;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

namespace CreationsForge.Engine.Persistence;

/// <summary>Coordinates guarded Mutagen export, semantic staged verification, publication, and final reopen.</summary>
internal sealed class PluginSaveService
{
    private readonly IPluginPersistenceBackend _backend;
    private readonly PluginFilePublisher _publisher;

    /// <summary>Initializes persistence coordination over the supplied native IO boundary.</summary>
    public PluginSaveService(IPluginPersistenceBackend backend)
    {
        ArgumentNullException.ThrowIfNull(backend);
        _backend = backend;
        _publisher = new PluginFilePublisher(backend);
    }

    /// <summary>Runs one guarded native save for the supplied workspace.</summary>
    public async Task<PluginSaveResult> SaveAsync(
        PluginWorkspace workspace,
        IProgress<PluginSaveProgress>? progress,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var revision = workspace.State.Revision;
        var destinationPath = workspace.State.OutputPath;
        var phase = PluginSavePhase.Preflight;
        Report(progress, phase, destinationPath, revision, stopwatch.Elapsed, "Save started.");

        if (workspace.State.RequiresReopen)
        {
            return Result(
                PluginSaveStatus.Failed,
                PluginPublicationState.Unchanged,
                phase,
                workspace,
                [],
                [],
                stopwatch,
                "The workspace requires a fresh reopen before another persistence operation.",
                requiresWorkspaceReopen: true,
                progress);
        }

        if (!DestinationMatches(workspace))
        {
            return Result(
                PluginSaveStatus.Failed,
                PluginPublicationState.Unchanged,
                phase,
                workspace,
                [],
                [],
                stopwatch,
                $"Destination '{destinationPath}' changed outside this workspace; close and reopen before saving.",
                requiresWorkspaceReopen: false,
                progress);
        }

        if (!workspace.State.IsDirty)
        {
            return Result(
                PluginSaveStatus.NoChanges,
                PluginPublicationState.Unchanged,
                PluginSavePhase.Complete,
                workspace,
                [],
                [],
                stopwatch,
                "The workspace is already clean and the destination is unchanged.",
                requiresWorkspaceReopen: false,
                progress);
        }

        cancellationToken.ThrowIfCancellationRequested();
        var destinationRoot = Path.GetDirectoryName(destinationPath)
            ?? throw new PluginWorkspaceException($"Output path '{destinationPath}' does not have a parent directory.");
        var operationRoot = Path.Combine(destinationRoot, $".CreationsForge-save-{Guid.NewGuid():N}");
        var stagingRoot = Path.Combine(operationRoot, "stage");
        var backupRoot = Path.Combine(operationRoot, "backup");
        var stagedPluginPath = Path.Combine(stagingRoot, workspace.State.OutputModKey.ToString());
        var expectedRecordCounts = CountRecordIdentities(workspace.Output);
        var expectedMasters = workspace.Output.MasterReferences.Select(reference => reference.Master).ToArray();
        var expectedSnapshots = workspace.PendingSaveSnapshots.ToArray();
        IMod? stagedOutput = null;

        try
        {
            _backend.CreateDirectory(stagingRoot);
            _backend.CreateDirectory(backupRoot);

            phase = PluginSavePhase.Staging;
            Report(progress, phase, destinationPath, revision, stopwatch.Elapsed, "Exporting the native output to same-filesystem staging.");
            await _backend.ExportAsync(
                workspace.Output,
                stagedPluginPath,
                workspace.DataDirectory,
                workspace.SourceMasters).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();

            phase = PluginSavePhase.StagedReopen;
            Report(progress, phase, destinationPath, revision, stopwatch.Elapsed, "Reopening and verifying the staged output.");
            stagedOutput = _backend.OpenOutput(
                workspace.Integration,
                new ModPath(workspace.State.OutputModKey, stagedPluginPath),
                workspace.SourceMasters);
            VerifyOutput(workspace, stagedOutput, expectedRecordCounts, expectedMasters, expectedSnapshots);
            if (!TryDisposeResource(stagedOutput))
            {
                throw new PluginWorkspaceException("The verified staged output did not close cleanly before publication.");
            }
            stagedOutput = null;

            if (!DestinationMatches(workspace))
            {
                return ResultAfterCleanup(
                    PluginSaveStatus.Failed,
                    PluginPublicationState.Unchanged,
                    phase,
                    workspace,
                    [],
                    [],
                    operationRoot,
                    stopwatch,
                    $"Destination '{destinationPath}' changed while the save was staging; nothing was published.",
                    requiresWorkspaceReopen: false,
                    progress);
            }

            cancellationToken.ThrowIfCancellationRequested();
            var publication = _publisher.Publish(
                workspace.State.OutputModKey,
                stagingRoot,
                stagedPluginPath,
                destinationPath,
                backupRoot,
                (reportedPhase, diagnostic) =>
                {
                    phase = reportedPhase;
                    Report(progress, reportedPhase, destinationPath, revision, stopwatch.Elapsed, diagnostic);
                });
            if (!publication.Succeeded)
            {
                var requiresReopen = publication.State == PluginPublicationState.PartiallyPublished;
                if (requiresReopen)
                {
                    workspace.MarkRequiresReopen();
                    return Result(
                        PluginSaveStatus.Failed,
                        publication.State,
                        phase,
                        workspace,
                        publication.PublishedPaths,
                        publication.BackupPaths,
                        stopwatch,
                        publication.Diagnostic,
                        requiresWorkspaceReopen: true,
                        progress);
                }

                return ResultAfterCleanup(
                    PluginSaveStatus.Failed,
                    publication.State,
                    phase,
                    workspace,
                    publication.PublishedPaths,
                    publication.BackupPaths,
                    operationRoot,
                    stopwatch,
                    publication.Diagnostic,
                    requiresWorkspaceReopen: false,
                    progress);
            }

            phase = PluginSavePhase.PublishedReopen;
            Report(progress, phase, destinationPath, revision, stopwatch.Elapsed, "Reopening and verifying the published destination.");
            IMod? publishedOutput = null;
            var adoptedWithoutCleanupFailures = false;
            try
            {
                publishedOutput = _backend.OpenOutput(
                    workspace.Integration,
                    new ModPath(workspace.State.OutputModKey, destinationPath),
                    workspace.SourceMasters);
                VerifyOutput(workspace, publishedOutput, expectedRecordCounts, expectedMasters, expectedSnapshots);
                var publishedStamp = _backend.CaptureStamp(workspace.State.OutputModKey, destinationPath)
                    ?? throw new PluginWorkspaceException($"Published output '{destinationPath}' is missing after publication.");
                adoptedWithoutCleanupFailures = workspace.AcceptSavedOutput(publishedOutput, publishedStamp);
                publishedOutput = null;
            }
            catch (Exception exception)
            {
                TryDisposeResource(publishedOutput);
                workspace.MarkRequiresReopen();
                return Result(
                    PluginSaveStatus.PublishedButReopenFailed,
                    PluginPublicationState.Published,
                    phase,
                    workspace,
                    publication.PublishedPaths,
                    publication.BackupPaths,
                    stopwatch,
                    $"The file set was published, but the published output could not be reopened and adopted. Close and reopen the workspace instead of retrying blindly: {exception.Message}",
                    requiresWorkspaceReopen: true,
                    progress);
            }

            var operationDirectoryRemoved = TryDeleteOperationDirectory(operationRoot);
            var cleanupDiagnostic = (adoptedWithoutCleanupFailures, operationDirectoryRemoved) switch
            {
                (true, true) => "The native output was published, reopened, and adopted as the saved baseline.",
                (false, true) => "The native output was saved and adopted, but one or more replaced in-memory resources did not close cleanly.",
                (true, false) => $"The native output was saved, but temporary operation directory '{operationRoot}' could not be removed.",
                (false, false) => $"The native output was saved, but replaced in-memory resources and temporary operation directory '{operationRoot}' did not close cleanly.",
            };
            var retainedBackups = _backend.DirectoryExists(operationRoot)
                ? publication.BackupPaths.Where(_backend.FileExists).ToArray()
                : [];
            return Result(
                PluginSaveStatus.Succeeded,
                PluginPublicationState.Published,
                PluginSavePhase.Complete,
                workspace,
                publication.PublishedPaths,
                retainedBackups,
                stopwatch,
                cleanupDiagnostic,
                requiresWorkspaceReopen: false,
                progress);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            TryDisposeResource(stagedOutput);
            return ResultAfterCleanup(
                PluginSaveStatus.Canceled,
                PluginPublicationState.Unchanged,
                phase,
                workspace,
                [],
                [],
                operationRoot,
                stopwatch,
                "The save was canceled before destination publication began; staged edits remain in memory.",
                requiresWorkspaceReopen: false,
                progress);
        }
        catch (Exception exception)
        {
            TryDisposeResource(stagedOutput);
            return ResultAfterCleanup(
                PluginSaveStatus.Failed,
                PluginPublicationState.Unchanged,
                phase,
                workspace,
                [],
                [],
                operationRoot,
                stopwatch,
                $"Save failed before destination publication: {exception.Message}",
                requiresWorkspaceReopen: false,
                progress);
        }
    }

    /// <summary>Creates a bounded canceled result when a request is canceled before entering the operation gate.</summary>
    public PluginSaveResult CanceledBeforeStart(
        PluginWorkspace workspace,
        TimeSpan duration,
        IProgress<PluginSaveProgress>? progress)
    {
        var result = new PluginSaveResult(
            PluginSaveStatus.Canceled,
            PluginPublicationState.Unchanged,
            PluginSavePhase.Preflight,
            workspace.State.Revision,
            workspace.State.OutputPath,
            [],
            [],
            duration,
            "The save was canceled before it acquired the workspace operation gate.",
            requiresWorkspaceReopen: workspace.State.RequiresReopen);
        Report(
            progress,
            PluginSavePhase.Complete,
            result.DestinationPath,
            result.Revision,
            result.Duration,
            result.Diagnostic);
        return result;
    }

    private bool DestinationMatches(PluginWorkspace workspace)
    {
        return Equals(
            _backend.CaptureStamp(workspace.State.OutputModKey, workspace.State.OutputPath),
            workspace.ExpectedDestinationStamp);
    }

    private static IReadOnlyDictionary<FormKey, int> CountRecordIdentities(IModGetter output)
    {
        return output
            .EnumerateMajorRecords()
            .GroupBy(record => record.FormKey)
            .ToDictionary(group => group.Key, group => group.Count());
    }

    private static void VerifyOutput(
        PluginWorkspace workspace,
        IMod output,
        IReadOnlyDictionary<FormKey, int> expectedRecordCounts,
        IReadOnlyList<ModKey> expectedMasters,
        IReadOnlyList<RecordSnapshot> expectedSnapshots)
    {
        if (output.ModKey != workspace.State.OutputModKey)
        {
            throw new PluginWorkspaceException($"Reopened output reports identity '{output.ModKey}' instead of '{workspace.State.OutputModKey}'.");
        }

        if (((IModMasterStyledGetter)output).MasterStyle != workspace.State.MasterStyle)
        {
            throw new PluginWorkspaceException($"Reopened output '{output.ModKey}' changed master style.");
        }

        var actualTextStorage = output.UsingLocalization
            ? PluginTextStorageMode.Localized
            : PluginTextStorageMode.Embedded;
        if (actualTextStorage != workspace.State.TextStorageMode)
        {
            throw new PluginWorkspaceException($"Reopened output '{output.ModKey}' changed text-storage mode.");
        }

        var actualRecordCounts = CountRecordIdentities(output);
        if (actualRecordCounts.Count != expectedRecordCounts.Count
            || expectedRecordCounts.Any(pair => !actualRecordCounts.TryGetValue(pair.Key, out var count) || count != pair.Value))
        {
            throw new PluginWorkspaceException($"Reopened output '{output.ModKey}' did not preserve its major-record identities.");
        }

        var actualMasters = output.MasterReferences.Select(reference => reference.Master).ToArray();
        if (expectedMasters.Any(master => !actualMasters.Contains(master)))
        {
            throw new PluginWorkspaceException($"Reopened output '{output.ModKey}' did not preserve every declared master.");
        }

        var sourceOrder = workspace.Sources
            .Select((source, index) => (source.ModKey, index))
            .ToDictionary(pair => pair.ModKey, pair => pair.index);
        var masterRanks = actualMasters.Select(master => sourceOrder.TryGetValue(master, out var rank) ? rank : -1).ToArray();
        if (masterRanks.Any(rank => rank < 0) || !masterRanks.SequenceEqual(masterRanks.Order()))
        {
            throw new PluginWorkspaceException($"Reopened output '{output.ModKey}' has masters outside the explicit source closure or in the wrong order.");
        }

        var families = workspace.Integration.RecordFamilies.ToDictionary(
            family => family.Descriptor.FamilyId,
            StringComparer.Ordinal);
        foreach (var expected in expectedSnapshots)
        {
            if (!families.TryGetValue(expected.FamilyId, out var family))
            {
                throw new PluginWorkspaceException($"Save verification cannot resolve declared family '{expected.FamilyId}'.");
            }

            var record = family.FindOutputRecord(output, expected.FormKey)
                ?? throw new PluginWorkspaceException($"Reopened output '{output.ModKey}' is missing changed record '{expected.FormKey}'.");
            var actualValues = family.ReadFields(record);
            if (actualValues.Count != expected.Values.Count
                || expected.Values.Any(pair => !actualValues.TryGetValue(pair.Key, out var value)
                    || !RecordValueComparer.Equals(pair.Value, value)))
            {
                throw new PluginWorkspaceException($"Reopened output '{output.ModKey}' changed registered values for '{expected.FamilyId}' record '{expected.FormKey}'.");
            }
        }
    }

    private PluginSaveResult ResultAfterCleanup(
        PluginSaveStatus status,
        PluginPublicationState publicationState,
        PluginSavePhase phase,
        PluginWorkspace workspace,
        IReadOnlyList<string> publishedPaths,
        IReadOnlyList<string> backupPaths,
        string operationRoot,
        Stopwatch stopwatch,
        string diagnostic,
        bool requiresWorkspaceReopen,
        IProgress<PluginSaveProgress>? progress)
    {
        var cleanupSucceeded = TryDeleteOperationDirectory(operationRoot);
        var retainedBackups = cleanupSucceeded
            ? []
            : backupPaths.Where(_backend.FileExists).ToArray();
        var finalDiagnostic = cleanupSucceeded
            ? diagnostic
            : $"{diagnostic} Temporary operation directory '{operationRoot}' could not be removed.";
        return Result(
            status,
            publicationState,
            phase,
            workspace,
            publishedPaths,
            retainedBackups,
            stopwatch,
            finalDiagnostic,
            requiresWorkspaceReopen,
            progress);
    }

    private PluginSaveResult Result(
        PluginSaveStatus status,
        PluginPublicationState publicationState,
        PluginSavePhase phase,
        PluginWorkspace workspace,
        IReadOnlyList<string> publishedPaths,
        IReadOnlyList<string> backupPaths,
        Stopwatch stopwatch,
        string diagnostic,
        bool requiresWorkspaceReopen,
        IProgress<PluginSaveProgress>? progress)
    {
        stopwatch.Stop();
        var result = new PluginSaveResult(
            status,
            publicationState,
            phase,
            workspace.State.Revision,
            workspace.State.OutputPath,
            publishedPaths,
            backupPaths,
            stopwatch.Elapsed,
            diagnostic,
            requiresWorkspaceReopen);
        Report(
            progress,
            PluginSavePhase.Complete,
            result.DestinationPath,
            result.Revision,
            result.Duration,
            result.Diagnostic);
        return result;
    }

    private bool TryDeleteOperationDirectory(string operationRoot)
    {
        if (!_backend.DirectoryExists(operationRoot))
        {
            return true;
        }

        try
        {
            _backend.DeleteDirectory(operationRoot, recursive: true);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool TryDisposeResource(object? resource)
    {
        try
        {
            if (resource is IDisposable disposable)
            {
                disposable.Dispose();
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    private static void Report(
        IProgress<PluginSaveProgress>? progress,
        PluginSavePhase phase,
        string destinationPath,
        ulong revision,
        TimeSpan elapsed,
        string diagnostic)
    {
        try
        {
            progress?.Report(new PluginSaveProgress(phase, destinationPath, revision, elapsed, diagnostic));
        }
        catch
        {
            // Diagnostics must not change the persistence outcome.
        }
    }
}
