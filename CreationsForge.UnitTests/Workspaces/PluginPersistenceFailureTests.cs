using CreationsForge.Engine.Interfaces;
using CreationsForge.Engine.Persistence;
using CreationsForge.Engine.Records;
using CreationsForge.Engine.Workspaces;
using CreationsForge.Fallout4;
using CreationsForge.Skyrim;
using CreationsForge.Starfield;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.UnitTests.Workspaces;

/// <summary>Verifies persistence failure, cancellation, and operation-coordination contracts.</summary>
public sealed partial class PluginWorkspaceTests
{
    /// <summary>Keeps the destination absent and staged edits dirty when native export fails.</summary>
    [Fact]
    public async Task ExportFailurePreservesPendingWorkspace()
    {
        using var directory = new TemporaryDirectory();
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        var backend = new InstrumentedPersistenceBackend { FailExport = true };
        using var workspace = CreateFactory(backend).Open(
            CreateNewRequest(directory.Path, GameRelease.Fallout4, [], outputModKey));

        var result = await workspace.SaveAsync(cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(PluginSaveStatus.Failed, result.Status);
        Assert.Equal(PluginPublicationState.Unchanged, result.PublicationState);
        Assert.Equal(PluginSavePhase.Staging, result.TerminalPhase);
        Assert.True(workspace.State.IsDirty);
        Assert.False(workspace.State.RequiresReopen);
        Assert.False(File.Exists(Path.Combine(directory.Path, outputModKey.ToString())));
        Assert.Empty(Directory.EnumerateDirectories(directory.Path, ".CreationsForge-save-*"));
    }

    /// <summary>Rejects an unverified staged output before any destination publication occurs.</summary>
    [Fact]
    public async Task StagedReopenFailureDoesNotPublish()
    {
        using var directory = new TemporaryDirectory();
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        var backend = new InstrumentedPersistenceBackend { FailOpenCall = 1 };
        using var workspace = CreateFactory(backend).Open(
            CreateNewRequest(directory.Path, GameRelease.Fallout4, [], outputModKey));

        var result = await workspace.SaveAsync(cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(PluginSaveStatus.Failed, result.Status);
        Assert.Equal(PluginPublicationState.Unchanged, result.PublicationState);
        Assert.Equal(PluginSavePhase.StagedReopen, result.TerminalPhase);
        Assert.True(workspace.State.IsDirty);
        Assert.False(workspace.State.RequiresReopen);
        Assert.False(File.Exists(Path.Combine(directory.Path, outputModKey.ToString())));
    }

    /// <summary>Restores the prior destination bytes when publication fails after backup.</summary>
    [Fact]
    public async Task PublicationFailureRestoresExistingDestination()
    {
        using var directory = new TemporaryDirectory();
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        var outputPath = Path.Combine(directory.Path, outputModKey.ToString());
        await CreateSavedFalloutOutputAsync(directory.Path, outputModKey);
        var originalBytes = File.ReadAllBytes(outputPath);
        var backend = new InstrumentedPersistenceBackend { FailMoveCall = 1 };
        using var workspace = CreateFactory(backend).Open(
            CreateExistingRequest(directory.Path, GameRelease.Fallout4, outputModKey));
        workspace.Records.Apply(new RecordChangeSet(0,
        [
            RecordMutation.Create("Keyword", [Set("EditorID", RecordValue.FromString("PendingKeyword"))]),
        ]));

        var result = await workspace.SaveAsync(cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(PluginSaveStatus.Failed, result.Status);
        Assert.True(
            result.PublicationState == PluginPublicationState.Restored,
            $"Expected restored publication state. Actual: {result.PublicationState}. {result.Diagnostic}");
        Assert.Equal(originalBytes, File.ReadAllBytes(outputPath));
        Assert.True(workspace.State.IsDirty);
        Assert.False(workspace.State.RequiresReopen);
        Assert.Empty(result.BackupPaths);
        Assert.Empty(Directory.EnumerateDirectories(directory.Path, ".CreationsForge-save-*"));

        var retry = await workspace.SaveAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(PluginSaveStatus.Succeeded, retry.Status);
        Assert.False(workspace.State.IsDirty);
    }

    /// <summary>Leaves the complete existing destination unchanged when backup cannot be completed.</summary>
    [Fact]
    public async Task BackupFailureDoesNotBeginPublication()
    {
        using var directory = new TemporaryDirectory();
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        var outputPath = Path.Combine(directory.Path, outputModKey.ToString());
        await CreateSavedFalloutOutputAsync(directory.Path, outputModKey);
        var originalBytes = File.ReadAllBytes(outputPath);
        var backend = new InstrumentedPersistenceBackend { FailCopyCall = 1 };
        using var workspace = CreateFactory(backend).Open(
            CreateExistingRequest(directory.Path, GameRelease.Fallout4, outputModKey));
        workspace.Records.Apply(new RecordChangeSet(0,
        [
            RecordMutation.Create("Keyword", [Set("EditorID", RecordValue.FromString("PendingKeyword"))]),
        ]));

        var result = await workspace.SaveAsync(cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(PluginSaveStatus.Failed, result.Status);
        Assert.Equal(PluginPublicationState.Unchanged, result.PublicationState);
        Assert.Equal(originalBytes, File.ReadAllBytes(outputPath));
        Assert.True(workspace.State.IsDirty);
        Assert.False(workspace.State.RequiresReopen);
        Assert.Empty(result.BackupPaths);
    }

    /// <summary>Restores plugin and localized string bytes together after a multi-file publication failure.</summary>
    [Fact]
    public async Task LocalizedPublicationFailureRestoresCompleteFileSet()
    {
        using var directory = new TemporaryDirectory();
        var outputModKey = ModKey.FromNameAndExtension("Localized.esp");
        var formKey = await CreateSavedLocalizedFalloutOutputAsync(directory.Path, outputModKey);
        var outputPath = Path.Combine(directory.Path, outputModKey.ToString());
        var realBackend = new PluginPersistenceBackend();
        var originalFiles = realBackend
            .GetAssociatedFiles(outputModKey, outputPath)
            .ToDictionary(path => path, File.ReadAllBytes, PathComparer);
        Assert.True(originalFiles.Count > 1);
        var backend = new InstrumentedPersistenceBackend { FailMoveCall = 2 };
        var output = new PluginOutputDefinition(
            outputPath,
            outputModKey,
            MasterStyle.Full,
            PluginTextStorageMode.Localized,
            createNew: false);
        using var workspace = CreateFactory(backend).Open(
            new PluginWorkspaceOpenRequest(GameRelease.Fallout4, directory.Path, [], output));
        var translated = RecordValue.FromTranslatedString(
            Language.English,
            new Dictionary<Language, string> { [Language.English] = "Updated localized value" });
        workspace.Records.Apply(new RecordChangeSet(0,
        [
            RecordMutation.Override("Message", formKey, outputModKey,
            [
                Set("Name", translated),
                Set("Description", translated),
            ]),
        ]));

        var result = await workspace.SaveAsync(cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(PluginSaveStatus.Failed, result.Status);
        Assert.Equal(PluginPublicationState.Restored, result.PublicationState);
        foreach (var original in originalFiles)
        {
            Assert.Equal(original.Value, File.ReadAllBytes(original.Key));
        }
    }

    /// <summary>Retains recovery material and disables further mutations when publication restoration fails.</summary>
    [Fact]
    public async Task RestorationFailureRequiresFreshWorkspaceReopen()
    {
        using var directory = new TemporaryDirectory();
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        var outputPath = Path.Combine(directory.Path, outputModKey.ToString());
        await CreateSavedFalloutOutputAsync(directory.Path, outputModKey);
        var backend = new InstrumentedPersistenceBackend
        {
            FailMoveCall = 1,
            FailCopyCall = 2,
        };
        using var workspace = CreateFactory(backend).Open(
            CreateExistingRequest(directory.Path, GameRelease.Fallout4, outputModKey));
        workspace.Records.Apply(new RecordChangeSet(0,
        [
            RecordMutation.Create("Keyword", [Set("EditorID", RecordValue.FromString("PendingKeyword"))]),
        ]));

        var result = await workspace.SaveAsync(cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(PluginSaveStatus.Failed, result.Status);
        Assert.True(
            result.PublicationState == PluginPublicationState.PartiallyPublished,
            $"Expected partial publication state. Actual: {result.PublicationState}. {result.Diagnostic}");
        Assert.True(result.RequiresWorkspaceReopen);
        Assert.True(workspace.State.RequiresReopen);
        Assert.Single(result.BackupPaths);
        Assert.All(result.BackupPaths, path => Assert.True(File.Exists(path), path));
        Assert.Throws<PluginWorkspaceException>(() => workspace.Discard());
        Assert.Throws<RecordEditingException>(() => workspace.Records.Apply(new RecordChangeSet(1,
        [
            RecordMutation.Create("Keyword", [Set("EditorID", RecordValue.FromString("RejectedKeyword"))]),
        ])));
    }

    /// <summary>Reports that publication succeeded but requires a new workspace when final reopen cannot be adopted.</summary>
    [Fact]
    public async Task PublishedReopenFailureRequiresFreshWorkspaceReopen()
    {
        using var directory = new TemporaryDirectory();
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        var outputPath = Path.Combine(directory.Path, outputModKey.ToString());
        var backend = new InstrumentedPersistenceBackend { FailOpenCall = 2 };
        var workspace = CreateFactory(backend).Open(
            CreateNewRequest(directory.Path, GameRelease.Fallout4, [], outputModKey));
        workspace.Records.Apply(new RecordChangeSet(0,
        [
            RecordMutation.Create("Keyword", [Set("EditorID", RecordValue.FromString("PublishedKeyword"))]),
        ]));

        var result = await workspace.SaveAsync(cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(PluginSaveStatus.PublishedButReopenFailed, result.Status);
        Assert.Equal(PluginPublicationState.Published, result.PublicationState);
        Assert.True(result.RequiresWorkspaceReopen);
        Assert.True(workspace.State.RequiresReopen);
        Assert.True(File.Exists(outputPath));
        var retry = await workspace.SaveAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(PluginSaveStatus.Failed, retry.Status);
        Assert.Equal(PluginPublicationState.Unchanged, retry.PublicationState);
        Assert.True(retry.RequiresWorkspaceReopen);
        Assert.Contains("requires a fresh reopen", retry.Diagnostic, StringComparison.Ordinal);
        workspace.Dispose();

        using var reopened = CreateFactory().Open(
            CreateExistingRequest(directory.Path, GameRelease.Fallout4, outputModKey));
        Assert.Single(reopened.Output.EnumerateMajorRecords());
    }

    /// <summary>Cancels after native staging but before publication without changing the destination.</summary>
    [Fact]
    public async Task CancellationBeforePublicationPreservesPendingWorkspace()
    {
        using var directory = new TemporaryDirectory();
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        var pause = new ExportPause();
        var backend = new InstrumentedPersistenceBackend { AfterExportAsync = pause.WaitAsync };
        using var workspace = CreateFactory(backend).Open(
            CreateNewRequest(directory.Path, GameRelease.Fallout4, [], outputModKey));
        using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);

        var saveTask = workspace.SaveAsync(cancellationToken: cancellation.Token);
        await pause.Entered.Task.WaitAsync(TestContext.Current.CancellationToken);
        cancellation.Cancel();
        pause.Release.TrySetResult(true);
        var result = await saveTask;

        Assert.Equal(PluginSaveStatus.Canceled, result.Status);
        Assert.Equal(PluginPublicationState.Unchanged, result.PublicationState);
        Assert.True(workspace.State.IsDirty);
        Assert.False(File.Exists(Path.Combine(directory.Path, outputModKey.ToString())));
        Assert.Empty(Directory.EnumerateDirectories(directory.Path, ".CreationsForge-save-*"));
    }

    /// <summary>Coordinates close with an active save, canceling before publication and releasing output ownership.</summary>
    [Fact]
    public async Task CloseWaitsForActiveSaveAndReleasesOwnership()
    {
        using var directory = new TemporaryDirectory();
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        var pause = new ExportPause();
        var backend = new InstrumentedPersistenceBackend { AfterExportAsync = pause.WaitAsync };
        var workspace = CreateFactory(backend).Open(
            CreateNewRequest(directory.Path, GameRelease.Fallout4, [], outputModKey));

        var saveTask = workspace.SaveAsync(cancellationToken: TestContext.Current.CancellationToken);
        await pause.Entered.Task.WaitAsync(TestContext.Current.CancellationToken);
        var closeTask = Task.Run(workspace.Dispose, TestContext.Current.CancellationToken);
        await Task.Delay(TimeSpan.FromMilliseconds(50), TestContext.Current.CancellationToken);
        Assert.False(closeTask.IsCompleted);
        pause.Release.TrySetResult(true);

        var result = await saveTask;
        await closeTask.WaitAsync(TestContext.Current.CancellationToken);

        Assert.Equal(PluginSaveStatus.Canceled, result.Status);
        Assert.False(File.Exists(Path.Combine(directory.Path, outputModKey.ToString())));
        using var reopened = CreateFactory().Open(
            CreateNewRequest(directory.Path, GameRelease.Fallout4, [], outputModKey));
    }

    /// <summary>Queues editing behind an active save and applies it to the newly adopted live output.</summary>
    [Fact]
    public async Task EditingWaitsForSaveOperationGate()
    {
        using var directory = new TemporaryDirectory();
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        var pause = new ExportPause();
        var backend = new InstrumentedPersistenceBackend { AfterExportAsync = pause.WaitAsync };
        using var workspace = CreateFactory(backend).Open(
            CreateNewRequest(directory.Path, GameRelease.Fallout4, [], outputModKey));

        var saveTask = workspace.SaveAsync(cancellationToken: TestContext.Current.CancellationToken);
        await pause.Entered.Task.WaitAsync(TestContext.Current.CancellationToken);
        var editTask = Task.Run(() => workspace.Records.Apply(new RecordChangeSet(0,
        [
            RecordMutation.Create("Keyword", [Set("EditorID", RecordValue.FromString("QueuedKeyword"))]),
        ])));
        await Task.Delay(TimeSpan.FromMilliseconds(50), TestContext.Current.CancellationToken);
        Assert.False(editTask.IsCompleted);
        pause.Release.TrySetResult(true);

        var save = await saveTask;
        var edit = await editTask.WaitAsync(TestContext.Current.CancellationToken);

        Assert.Equal(PluginSaveStatus.Succeeded, save.Status);
        Assert.Equal(1UL, edit.Revision);
        Assert.True(workspace.State.IsDirty);
        Assert.Single(workspace.Output.EnumerateMajorRecords());
    }

    private static PluginWorkspaceFactory CreateFactory(IPluginPersistenceBackend backend)
    {
        return new PluginWorkspaceFactory(
        [
            new StarfieldGameIntegration(),
            new Fallout4GameIntegration(),
            new SkyrimGameIntegration(),
        ], backend);
    }

    private static async Task CreateSavedFalloutOutputAsync(string dataDirectory, ModKey outputModKey)
    {
        using var workspace = CreateFactory().Open(
            CreateNewRequest(dataDirectory, GameRelease.Fallout4, [], outputModKey));
        workspace.Records.Apply(new RecordChangeSet(0,
        [
            RecordMutation.Create("Keyword", [Set("EditorID", RecordValue.FromString("ExistingKeyword"))]),
        ]));
        var result = await workspace.SaveAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(PluginSaveStatus.Succeeded, result.Status);
    }

    private static async Task<FormKey> CreateSavedLocalizedFalloutOutputAsync(
        string dataDirectory,
        ModKey outputModKey)
    {
        var output = new PluginOutputDefinition(
            Path.Combine(dataDirectory, outputModKey.ToString()),
            outputModKey,
            MasterStyle.Full,
            PluginTextStorageMode.Localized,
            createNew: true);
        using var workspace = CreateFactory().Open(
            new PluginWorkspaceOpenRequest(GameRelease.Fallout4, dataDirectory, [], output));
        var translated = RecordValue.FromTranslatedString(
            Language.English,
            new Dictionary<Language, string> { [Language.English] = "Original localized value" });
        var created = workspace.Records.Apply(new RecordChangeSet(0,
        [
            RecordMutation.Create("Message",
            [
                Set("EditorID", RecordValue.FromString("LocalizedMessage")),
                Set("Name", translated),
                Set("Description", translated),
            ]),
        ])).Records.Single();
        var result = await workspace.SaveAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(PluginSaveStatus.Succeeded, result.Status);
        return created.FormKey;
    }

    private sealed class ExportPause
    {
        public TaskCompletionSource<bool> Entered { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public TaskCompletionSource<bool> Release { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public Task WaitAsync()
        {
            Entered.TrySetResult(true);
            return Release.Task;
        }
    }

    private sealed class InstrumentedPersistenceBackend : IPluginPersistenceBackend
    {
        private readonly PluginPersistenceBackend _inner = new();
        private int _openCalls;
        private int _copyCalls;
        private int _moveCalls;

        public bool FailExport { get; init; }

        public int? FailOpenCall { get; init; }

        public int? FailCopyCall { get; init; }

        public int? FailMoveCall { get; init; }

        public Func<Task>? AfterExportAsync { get; init; }

        public async Task ExportAsync(
            IModGetter output,
            string path,
            string dataDirectory,
            IReadOnlyList<IModMasterStyledGetter> loadOrder)
        {
            if (FailExport)
            {
                throw new IOException("Injected native export failure.");
            }

            await _inner.ExportAsync(output, path, dataDirectory, loadOrder);
            if (AfterExportAsync is not null)
            {
                await AfterExportAsync();
            }
        }

        public IMod OpenOutput(
            IGameIntegration integration,
            ModPath path,
            IReadOnlyList<IModMasterStyledGetter> knownMasters)
        {
            if (Interlocked.Increment(ref _openCalls) == FailOpenCall)
            {
                throw new IOException("Injected reopen failure.");
            }

            return _inner.OpenOutput(integration, path, knownMasters);
        }

        public PluginDestinationStamp? CaptureStamp(ModKey modKey, string pluginPath) =>
            _inner.CaptureStamp(modKey, pluginPath);

        public IReadOnlyList<string> GetAssociatedFiles(ModKey modKey, string pluginPath) =>
            _inner.GetAssociatedFiles(modKey, pluginPath);

        public void CreateDirectory(string path) => _inner.CreateDirectory(path);

        public void CopyFile(string sourcePath, string destinationPath, bool overwrite)
        {
            if (Interlocked.Increment(ref _copyCalls) == FailCopyCall)
            {
                throw new IOException("Injected copy failure.");
            }

            _inner.CopyFile(sourcePath, destinationPath, overwrite);
        }

        public void MoveFile(string sourcePath, string destinationPath, bool overwrite)
        {
            if (Interlocked.Increment(ref _moveCalls) == FailMoveCall)
            {
                throw new IOException("Injected move failure.");
            }

            _inner.MoveFile(sourcePath, destinationPath, overwrite);
        }

        public void DeleteFile(string path) => _inner.DeleteFile(path);

        public void DeleteDirectory(string path, bool recursive) => _inner.DeleteDirectory(path, recursive);

        public bool FileExists(string path) => _inner.FileExists(path);

        public bool DirectoryExists(string path) => _inner.DirectoryExists(path);
    }
}
