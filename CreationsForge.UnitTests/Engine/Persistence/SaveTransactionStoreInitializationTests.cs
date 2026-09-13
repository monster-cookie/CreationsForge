using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.PluginInputs;
using CreationsForge.Core.Engine.Persistence;
using Mutagen.Bethesda;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Persistence;

/// <summary>Exercises atomic initial journal publication and bounded initialization-remnant admission.</summary>
public sealed partial class WorkspaceSaveCoordinatorTests
{
    /// <summary>Contains focused tests for atomic initial journal publication and interruption debris.</summary>
    public sealed class SaveTransactionStoreInitializationBoundaryTests
    {
        /// <summary>Verifies the flushed initial journal becomes visible only at its canonical operation path.</summary>
        [Fact]
        public async Task InitializeAsyncPublishesCompleteCanonicalJournalWithoutInitializationRemnant()
        {
            using var directory = new TestDirectory();
            var setup = await CreatePreparingJournalAsync(directory.FullName);
            var store = new SaveTransactionStore();
    
            await store.InitializeAsync(setup.Paths, setup.Journal, CancellationToken.None);
    
            var parsed = await store.ReadAsync(setup.Paths, CancellationToken.None);
            parsed.ShouldNotBeNull();
            parsed.WorkspaceId.ShouldBe(setup.Journal.WorkspaceId);
            parsed.SaveOperationId.ShouldBe(setup.Journal.SaveOperationId);
            Directory.EnumerateDirectories(setup.Paths.WorkspaceDirectoryPath).Single()
                .ShouldBe(setup.Paths.TransactionDirectoryPath);
        }
    
        /// <summary>Verifies interruption before journal creation remains unpublished and does not block admission.</summary>
        [Fact]
        public async Task EmptyInitializationRemnantIsBoundedAndIgnored()
        {
            using var directory = new TestDirectory();
            var workspaceId = Guid.NewGuid();
            CreateInitializationRemnant(directory.FullName, workspaceId, Guid.NewGuid(), journalBytes: null);
    
            var inventory = await new SaveTransactionStore().ReadAllAsync(directory.FullName, CancellationToken.None);
    
            inventory.Journals.ShouldBeEmpty();
            inventory.OperationDirectoryCount.ShouldBe(1);
            inventory.WorkspaceIds.ShouldContain(workspaceId);
        }
    
        /// <summary>Verifies interruption during journal bytes remains unpublished without parsing partial content.</summary>
        [Fact]
        public async Task PartialInitializationJournalIsBoundedAndIgnored()
        {
            using var directory = new TestDirectory();
            CreateInitializationRemnant(directory.FullName, Guid.NewGuid(), Guid.NewGuid(), [1, 2, 3]);
    
            var inventory = await new SaveTransactionStore().ReadAllAsync(directory.FullName, CancellationToken.None);
    
            inventory.Journals.ShouldBeEmpty();
            inventory.OperationDirectoryCount.ShouldBe(1);
        }
    
        /// <summary>Verifies interruption after a complete journal flush but before publication remains unpublished.</summary>
        [Fact]
        public async Task CompleteInitializationJournalIsBoundedAndIgnored()
        {
            using var directory = new TestDirectory();
            var setup = await CreatePreparingJournalAsync(directory.FullName);
            CreateInitializationRemnant(
                directory.FullName,
                setup.Journal.WorkspaceId,
                setup.Journal.SaveOperationId,
                SaveTransactionJournalCodec.Write(setup.Journal));
    
            var inventory = await new SaveTransactionStore().ReadAllAsync(directory.FullName, CancellationToken.None);
    
            inventory.Journals.ShouldBeEmpty();
            inventory.OperationDirectoryCount.ShouldBe(1);
        }
    
        /// <summary>Verifies cancellation before initialization creates no transaction metadata.</summary>
        [Fact]
        public async Task InitializeAsyncCanceledBeforeStartCreatesNoMetadata()
        {
            using var directory = new TestDirectory();
            var setup = await CreatePreparingJournalAsync(directory.FullName);
            using var cancellationSource = new CancellationTokenSource();
            cancellationSource.Cancel();
    
            await Should.ThrowAsync<OperationCanceledException>(async () =>
                await new SaveTransactionStore().InitializeAsync(
                    setup.Paths,
                    setup.Journal,
                    cancellationSource.Token));
    
            Directory.Exists(setup.Paths.MetadataRootPath).ShouldBeFalse();
        }
    
        /// <summary>Verifies a mismatched journal identity fails before any unpublished or canonical path is created.</summary>
        [Fact]
        public async Task InitializeAsyncRejectsMismatchedJournalIdentityBeforeMetadataCreation()
        {
            using var directory = new TestDirectory();
            var setup = await CreatePreparingJournalAsync(directory.FullName);
            var mismatchedPaths = new SaveTransactionPaths(
                directory.FullName,
                setup.Journal.WorkspaceId,
                Guid.NewGuid());
    
            await Should.ThrowAsync<InvalidDataException>(async () =>
                await new SaveTransactionStore().InitializeAsync(
                    mismatchedPaths,
                    setup.Journal,
                    CancellationToken.None));
    
            Directory.Exists(mismatchedPaths.MetadataRootPath).ShouldBeFalse();
        }
    
        /// <summary>Verifies a canonical operation directory without a durable journal is corruption rather than an interruption remnant.</summary>
        [Fact]
        public async Task CanonicalOperationWithoutJournalIsRejected()
        {
            using var directory = new TestDirectory();
            var paths = new SaveTransactionPaths(directory.FullName, Guid.NewGuid(), Guid.NewGuid());
            Directory.CreateDirectory(paths.TransactionDirectoryPath);
    
            await Should.ThrowAsync<InvalidDataException>(async () =>
                await new SaveTransactionStore().ReadAllAsync(directory.FullName, CancellationToken.None));
        }
    
        /// <summary>Verifies a near-match initialization directory name is rejected instead of being treated as unpublished state.</summary>
        [Fact]
        public async Task MalformedInitializationDirectoryNameIsRejected()
        {
            using var directory = new TestDirectory();
            var workspacePath = CreateWorkspaceMetadataDirectory(directory.FullName, Guid.NewGuid());
            Directory.CreateDirectory(Path.Combine(workspacePath, $"{SaveTransactionStore.InitializationDirectoryPrefix}{Guid.NewGuid():N}"));
    
            await Should.ThrowAsync<InvalidDataException>(async () =>
                await new SaveTransactionStore().ReadAllAsync(directory.FullName, CancellationToken.None));
        }
    
        /// <summary>Verifies foreign initialization content is rejected rather than hidden by unpublished-state handling.</summary>
        [Fact]
        public async Task InitializationRemnantWithUnknownContentIsRejected()
        {
            using var directory = new TestDirectory();
            var initializationPath = CreateInitializationRemnant(
                directory.FullName,
                Guid.NewGuid(),
                Guid.NewGuid(),
                journalBytes: null);
            await File.WriteAllBytesAsync(Path.Combine(initializationPath, "foreign.bin"), [1]);
    
            await Should.ThrowAsync<InvalidDataException>(async () =>
                await new SaveTransactionStore().ReadAllAsync(directory.FullName, CancellationToken.None));
        }
    
        /// <summary>Verifies an initialization-directory alias is rejected without inspecting its target as owned metadata.</summary>
        [Fact]
        public async Task InitializationDirectoryAliasIsRejected()
        {
            using var directory = new TestDirectory();
            var workspacePath = CreateWorkspaceMetadataDirectory(directory.FullName, Guid.NewGuid());
            var targetPath = Directory.CreateDirectory(Path.Combine(directory.FullName, "AliasTarget")).FullName;
            var aliasPath = Path.Combine(
                workspacePath,
                $"{SaveTransactionStore.InitializationDirectoryPrefix}{Guid.NewGuid():N}-{Guid.NewGuid():N}");
            try
            {
                Directory.CreateSymbolicLink(aliasPath, targetPath);
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or PlatformNotSupportedException)
            {
                Assert.Skip($"The host could not create a directory symbolic-link fixture: {exception.Message}");
            }
    
            await Should.ThrowAsync<PluginSourceInputException>(async () =>
                await new SaveTransactionStore().ReadAllAsync(directory.FullName, CancellationToken.None));
        }
    
        /// <summary>Verifies workspace enumeration rejects the first entry beyond the configured test limit.</summary>
        [Fact]
        public async Task ReadAllAsyncBoundsWorkspaceDirectoriesBeforeSorting()
        {
            using var directory = new TestDirectory();
            for (var index = 0; index < 3; index++)
            {
                CreateWorkspaceMetadataDirectory(directory.FullName, Guid.NewGuid());
            }
    
            await Should.ThrowAsync<InvalidDataException>(async () =>
                await new SaveTransactionStore(2).ReadAllAsync(directory.FullName, CancellationToken.None));
        }
    
    }

    /// <summary>Creates a preparing journal and exact physical baseline for store tests.</summary>
    /// <param name="directoryPath">The canonical temporary output directory.</param>
    /// <returns>The journal and its canonical paths.</returns>
    private static async Task<(SaveTransactionPaths Paths, SaveTransactionJournal Journal)> CreatePreparingJournalAsync(
        string directoryPath)
    {
        var output = CreateOutput(directoryPath);
        await File.WriteAllBytesAsync(output.PluginPath, [1]);
        var baseline = await PluginSaveArtifactUtilities.CaptureOutputAsync(
            GameRelease.SkyrimSE,
            output,
            CancellationToken.None);
        var source = new TestSourceSet();
        var workspaceId = Guid.NewGuid();
        var operationId = Guid.NewGuid();
        var journal = CreateJournal(
            workspaceId,
            operationId,
            new WorkspaceRevision(Guid.NewGuid(), 1),
            source.Baseline,
            output,
            baseline,
            PluginWriteDisposition.StagedChanges,
            SaveTransactionPhase.Preparing);
        return (new SaveTransactionPaths(directoryPath, workspaceId, operationId), journal);
    }

    /// <summary>Creates one strict recognized initialization directory with optional journal bytes.</summary>
    /// <param name="outputDirectoryPath">The canonical temporary output directory.</param>
    /// <param name="workspaceId">The owning workspace identifier.</param>
    /// <param name="operationId">The embedded unpublished operation identifier.</param>
    /// <param name="journalBytes">The journal bytes to write, or <see langword="null"/> to leave the directory empty.</param>
    /// <returns>The created initialization directory.</returns>
    private static string CreateInitializationRemnant(
        string outputDirectoryPath,
        Guid workspaceId,
        Guid operationId,
        byte[]? journalBytes)
    {
        var workspacePath = CreateWorkspaceMetadataDirectory(outputDirectoryPath, workspaceId);
        var initializationPath = Directory.CreateDirectory(Path.Combine(
            workspacePath,
            $"{SaveTransactionStore.InitializationDirectoryPrefix}{operationId:N}-{Guid.NewGuid():N}")).FullName;
        if (journalBytes is not null)
        {
            File.WriteAllBytes(Path.Combine(initializationPath, SaveTransactionStore.JournalFileName), journalBytes);
        }

        return initializationPath;
    }

    /// <summary>Creates the canonical transaction metadata root and one workspace directory.</summary>
    /// <param name="outputDirectoryPath">The canonical temporary output directory.</param>
    /// <param name="workspaceId">The workspace directory identifier.</param>
    /// <returns>The canonical workspace metadata directory.</returns>
    private static string CreateWorkspaceMetadataDirectory(string outputDirectoryPath, Guid workspaceId)
    {
        return Directory.CreateDirectory(Path.Combine(
            outputDirectoryPath,
            SaveTransactionStore.MetadataDirectoryName,
            workspaceId.ToString("N"))).FullName;
    }
}
