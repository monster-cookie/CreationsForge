using CreationsForge.Core.Engine.PluginInputs;

namespace CreationsForge.Core.Engine.Persistence;

/// <summary>Identifies the stable paths owned by one save transaction.</summary>
internal sealed class SaveTransactionPaths
{
    /// <summary>Initializes stable transaction paths.</summary>
    /// <param name="outputDirectoryPath">The canonical leased output directory.</param>
    /// <param name="workspaceId">The original workspace identifier.</param>
    /// <param name="saveOperationId">The original save operation identifier.</param>
    internal SaveTransactionPaths(string outputDirectoryPath, Guid workspaceId, Guid saveOperationId)
    {
        OutputDirectoryPath = outputDirectoryPath;
        MetadataRootPath = Path.Combine(outputDirectoryPath, SaveTransactionStore.MetadataDirectoryName);
        WorkspaceDirectoryPath = Path.Combine(MetadataRootPath, workspaceId.ToString("N"));
        TransactionDirectoryPath = Path.Combine(WorkspaceDirectoryPath, saveOperationId.ToString("N"));
        JournalPath = Path.Combine(TransactionDirectoryPath, SaveTransactionStore.JournalFileName);
        StagingDirectoryPath = Path.Combine(TransactionDirectoryPath, "stage");
        PublishDirectoryPath = Path.Combine(TransactionDirectoryPath, "publish");
        BackupDirectoryPath = Path.Combine(TransactionDirectoryPath, "backup");
        RetiredDirectoryPath = Path.Combine(TransactionDirectoryPath, "retired");
    }

    /// <summary>Gets the canonical leased output directory.</summary>
    internal string OutputDirectoryPath { get; }

    /// <summary>Gets the stable transaction metadata root.</summary>
    internal string MetadataRootPath { get; }

    /// <summary>Gets the original workspace transaction directory.</summary>
    internal string WorkspaceDirectoryPath { get; }

    /// <summary>Gets the exact save-operation transaction directory.</summary>
    internal string TransactionDirectoryPath { get; }

    /// <summary>Gets the durable journal path.</summary>
    internal string JournalPath { get; }

    /// <summary>Gets the adapter-exclusive empty staging directory.</summary>
    internal string StagingDirectoryPath { get; }

    /// <summary>Gets the same-volume owned publication-file directory.</summary>
    internal string PublishDirectoryPath { get; }

    /// <summary>Gets the transaction-owned prior-byte backup directory.</summary>
    internal string BackupDirectoryPath { get; }

    /// <summary>Gets the transaction-owned retired-destination directory.</summary>
    internal string RetiredDirectoryPath { get; }
}

/// <summary>Captures one bounded read of recognized transaction metadata and the directory capacity it consumed.</summary>
internal sealed class SaveTransactionInventory
{
    /// <summary>Initializes one immutable transaction inventory.</summary>
    /// <param name="journals">The recognized canonical journals in stable order.</param>
    /// <param name="workspaceIds">The canonical workspace directory identifiers observed during the read.</param>
    /// <param name="operationDirectoryCount">The total canonical operation and unpublished initialization directory count.</param>
    internal SaveTransactionInventory(
        IReadOnlyList<(SaveTransactionPaths Paths, SaveTransactionJournal Journal)> journals,
        IReadOnlySet<Guid> workspaceIds,
        int operationDirectoryCount)
    {
        ArgumentNullException.ThrowIfNull(journals);
        ArgumentNullException.ThrowIfNull(workspaceIds);
        Journals = journals;
        WorkspaceIds = workspaceIds;
        OperationDirectoryCount = operationDirectoryCount;
    }

    /// <summary>Gets the recognized canonical journals in stable order.</summary>
    internal IReadOnlyList<(SaveTransactionPaths Paths, SaveTransactionJournal Journal)> Journals { get; }

    /// <summary>Gets the canonical workspace directory identifiers observed during the read.</summary>
    internal IReadOnlySet<Guid> WorkspaceIds { get; }

    /// <summary>Gets the total canonical operation and unpublished initialization directory count.</summary>
    internal int OperationDirectoryCount { get; }
}

/// <summary>Persists and inspects bounded save journals under the leased output directory.</summary>
internal sealed class SaveTransactionStore
{
    /// <summary>The stable transaction metadata directory directly under an output directory.</summary>
    internal const string MetadataDirectoryName = ".creationsforge-transactions";

    /// <summary>The stable complete journal filename inside one transaction directory.</summary>
    internal const string JournalFileName = "journal.bin";

    /// <summary>The maximum accepted journal size.</summary>
    private const int MaximumJournalBytes = 16 * 1024 * 1024;

    /// <summary>The production maximum for each bounded metadata directory or file collection.</summary>
    private const int DefaultMaximumMetadataEntryCount = 4096;

    /// <summary>The exact prefix reserved for unpublished initialization directories.</summary>
    internal const string InitializationDirectoryPrefix = ".initializing-";

    /// <summary>The maximum accepted count for each bounded metadata collection.</summary>
    private readonly int MaximumMetadataEntryCount;

    /// <summary>The allowed owned transaction subdirectory names.</summary>
    private static readonly HashSet<string> AllowedTransactionDirectories = new(StringComparer.Ordinal)
    {
        "stage",
        "publish",
        "backup",
        "retired"
    };

    /// <summary>Initializes the stateless transaction store.</summary>
    internal SaveTransactionStore()
        : this(DefaultMaximumMetadataEntryCount)
    { }

    /// <summary>Initializes a transaction store with a bounded metadata-entry limit.</summary>
    /// <param name="maximumMetadataEntryCount">The positive maximum workspace, operation, initialization, or permitted temporary-file count.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="maximumMetadataEntryCount"/> is not positive.</exception>
    internal SaveTransactionStore(int maximumMetadataEntryCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maximumMetadataEntryCount);
        MaximumMetadataEntryCount = maximumMetadataEntryCount;
    }

    /// <summary>Creates the fixed metadata hierarchy for a new normal save or explicit repair.</summary>
    /// <param name="paths">The exact transaction paths.</param>
    /// <exception cref="InvalidDataException">Thrown when existing metadata is malformed or aliases another location.</exception>
    internal void EnsureTransactionDirectory(SaveTransactionPaths paths)
    {
        ArgumentNullException.ThrowIfNull(paths);
        EnsureOwnedDirectory(paths.MetadataRootPath, "save transaction metadata root");
        EnsureOwnedDirectory(paths.WorkspaceDirectoryPath, "save transaction workspace directory");
        EnsureOwnedDirectory(paths.TransactionDirectoryPath, "save transaction operation directory");
        ValidateTransactionContents(paths.TransactionDirectoryPath);
    }

    /// <summary>Publishes the first complete journal and its operation directory through one same-volume directory move.</summary>
    /// <param name="paths">The exact transaction paths whose canonical operation directory must be absent.</param>
    /// <param name="journal">The complete initial preparing journal.</param>
    /// <param name="cancellationToken">The token checked before durable initialization starts.</param>
    /// <returns>A task that completes only after the flushed initialization directory becomes the canonical operation directory.</returns>
    /// <exception cref="InvalidDataException">Thrown when the journal identity is inconsistent or the canonical operation path is occupied.</exception>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested before initialization starts.</exception>
    internal async Task InitializeAsync(
        SaveTransactionPaths paths,
        SaveTransactionJournal journal,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(paths);
        ArgumentNullException.ThrowIfNull(journal);
        cancellationToken.ThrowIfCancellationRequested();
        ValidateJournalIdentity(paths, journal);
        var bytes = SaveTransactionJournalCodec.Write(journal);
        if (bytes.Length > MaximumJournalBytes)
        {
            throw new InvalidDataException("The complete initial save journal exceeds its bounded maximum size.");
        }

        EnsureOwnedDirectory(paths.MetadataRootPath, "save transaction metadata root");
        EnsureOwnedDirectory(paths.WorkspaceDirectoryPath, "save transaction workspace directory");
        if (File.Exists(paths.TransactionDirectoryPath) || Directory.Exists(paths.TransactionDirectoryPath))
        {
            throw new InvalidDataException($"The canonical save transaction operation path is already occupied: '{paths.TransactionDirectoryPath}'.");
        }

        var initializationDirectoryPath = Path.Combine(
            paths.WorkspaceDirectoryPath,
            $"{InitializationDirectoryPrefix}{journal.SaveOperationId:N}-{Guid.NewGuid():N}");
        if (File.Exists(initializationDirectoryPath) || Directory.Exists(initializationDirectoryPath))
        {
            throw new InvalidDataException($"The unpublished save transaction initialization path is already occupied: '{initializationDirectoryPath}'.");
        }

        Directory.CreateDirectory(initializationDirectoryPath);
        PluginFileInspector.VerifyDirectory(initializationDirectoryPath, "unpublished save transaction initialization directory");
        var initializationJournalPath = Path.Combine(initializationDirectoryPath, JournalFileName);
        await using (var stream = new FileStream(
            initializationJournalPath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            65536,
            FileOptions.Asynchronous | FileOptions.WriteThrough))
        {
            await stream.WriteAsync(bytes, CancellationToken.None).ConfigureAwait(false);
            stream.Flush(flushToDisk: true);
        }

        ValidateInitializationContents(initializationDirectoryPath);
        if (File.Exists(paths.TransactionDirectoryPath) || Directory.Exists(paths.TransactionDirectoryPath))
        {
            throw new InvalidDataException($"The canonical save transaction operation path became occupied before publication: '{paths.TransactionDirectoryPath}'.");
        }

        Directory.Move(initializationDirectoryPath, paths.TransactionDirectoryPath);
    }

    /// <summary>Creates and validates an empty transaction-owned subdirectory.</summary>
    /// <param name="path">The fixed transaction subdirectory path.</param>
    /// <param name="description">The diagnostic description.</param>
    /// <exception cref="InvalidDataException">Thrown when the directory exists with content or aliases another location.</exception>
    internal void CreateEmptySubdirectory(string path, string description)
    {
        if (File.Exists(path))
        {
            throw new InvalidDataException($"The {description} path identifies a file: '{path}'.");
        }

        if (Directory.Exists(path))
        {
            PluginFileInspector.VerifyDirectory(path, description);
            if (Directory.EnumerateFileSystemEntries(path).Any())
            {
                throw new InvalidDataException($"The {description} is not empty: '{path}'.");
            }

            return;
        }

        Directory.CreateDirectory(path);
        PluginFileInspector.VerifyDirectory(path, description);
    }

    /// <summary>Reads one exact recognized journal without creating metadata.</summary>
    /// <param name="paths">The exact transaction paths.</param>
    /// <param name="cancellationToken">The token checked before and during bounded file reads.</param>
    /// <returns>The parsed journal, or <see langword="null"/> when no journal exists.</returns>
    internal async Task<SaveTransactionJournal?> ReadAsync(
        SaveTransactionPaths paths,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(paths);
        cancellationToken.ThrowIfCancellationRequested();
        if (File.Exists(paths.MetadataRootPath))
        {
            throw new InvalidDataException($"The save transaction metadata-root path identifies a file: '{paths.MetadataRootPath}'.");
        }

        if (!Directory.Exists(paths.MetadataRootPath))
        {
            return null;
        }

        ValidateExistingHierarchy(paths);
        if (!File.Exists(paths.JournalPath))
        {
            return null;
        }

        VerifyOwnedFile(paths.JournalPath, "save transaction journal");
        var info = new FileInfo(paths.JournalPath);
        if (info.Length <= 0 || info.Length > MaximumJournalBytes)
        {
            throw new InvalidDataException($"The save journal has an invalid bounded length: '{paths.JournalPath}'.");
        }

        await using var stream = new FileStream(
            paths.JournalPath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            65536,
            FileOptions.Asynchronous | FileOptions.SequentialScan);
        var bytes = new byte[checked((int)info.Length)];
        await stream.ReadExactlyAsync(bytes, cancellationToken).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        var journal = SaveTransactionJournalCodec.Read(bytes);
        ValidateJournalIdentity(paths, journal);

        return journal;
    }

    /// <summary>Writes one complete journal snapshot through a flushed same-directory replacement file.</summary>
    /// <param name="paths">The exact transaction paths.</param>
    /// <param name="journal">The complete immutable snapshot.</param>
    /// <param name="cancellationToken">The token checked before the durable write starts.</param>
    /// <returns>A task that completes after the file contents have been flushed and published.</returns>
    internal async Task WriteAsync(
        SaveTransactionPaths paths,
        SaveTransactionJournal journal,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(paths);
        ArgumentNullException.ThrowIfNull(journal);
        cancellationToken.ThrowIfCancellationRequested();
        ValidateJournalIdentity(paths, journal);
        EnsureTransactionDirectory(paths);
        var bytes = SaveTransactionJournalCodec.Write(journal);
        if (bytes.Length > MaximumJournalBytes)
        {
            throw new InvalidDataException("The complete save journal exceeds its bounded maximum size.");
        }

        var temporaryPath = Path.Combine(
            paths.TransactionDirectoryPath,
            $".journal-{Guid.NewGuid():N}.tmp");
        await using (var stream = new FileStream(
            temporaryPath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            65536,
            FileOptions.Asynchronous | FileOptions.WriteThrough))
        {
            await stream.WriteAsync(bytes, CancellationToken.None).ConfigureAwait(false);
            stream.Flush(flushToDisk: true);
        }

        File.Move(temporaryPath, paths.JournalPath, overwrite: true);
    }

    /// <summary>Enumerates and parses every bounded recognized journal for admission.</summary>
    /// <param name="outputDirectoryPath">The canonical leased output directory.</param>
    /// <param name="cancellationToken">The token checked between directory and journal observations.</param>
    /// <returns>The recognized journals and the bounded workspace and operation-directory inventory.</returns>
    internal async Task<SaveTransactionInventory> ReadAllAsync(
        string outputDirectoryPath,
        CancellationToken cancellationToken)
    {
        var root = Path.Combine(outputDirectoryPath, MetadataDirectoryName);
        if (File.Exists(root))
        {
            throw new InvalidDataException($"The save transaction metadata-root path identifies a file: '{root}'.");
        }

        if (!Directory.Exists(root))
        {
            return new SaveTransactionInventory(
                Array.Empty<(SaveTransactionPaths, SaveTransactionJournal)>(),
                new HashSet<Guid>(),
                0);
        }

        PluginFileInspector.VerifyDirectory(root, "save transaction metadata root");
        var rootFiles = EnumerateFilesBounded(root, "save transaction metadata-root file collection");
        if (rootFiles.Count != 0)
        {
            throw new InvalidDataException("The save transaction metadata root contains unrecognized files.");
        }

        var workspaceDirectories = EnumerateDirectoriesBounded(
            root,
            MaximumMetadataEntryCount,
            "save transaction workspace directory collection");
        workspaceDirectories.Sort(PathComparer);
        var workspaceIds = new HashSet<Guid>();
        var canonicalTransactions = new List<(SaveTransactionPaths Paths, string DirectoryPath)>();
        var operationDirectoryCount = 0;
        foreach (var workspaceDirectory in workspaceDirectories)
        {
            cancellationToken.ThrowIfCancellationRequested();
            PluginFileInspector.VerifyDirectory(workspaceDirectory, "save transaction workspace directory");
            if (!TryParseCanonicalGuidDirectory(workspaceDirectory, out var workspaceId))
            {
                throw new InvalidDataException($"The save metadata root contains an unrecognized workspace directory: '{workspaceDirectory}'.");
            }

            workspaceIds.Add(workspaceId);
            var workspaceFiles = EnumerateFilesBounded(
                workspaceDirectory,
                "save transaction workspace file collection");
            if (workspaceFiles.Count != 0)
            {
                throw new InvalidDataException($"The save workspace metadata directory contains unrecognized files: '{workspaceDirectory}'.");
            }

            var transactionDirectories = EnumerateDirectoriesBounded(
                workspaceDirectory,
                MaximumMetadataEntryCount - operationDirectoryCount,
                "save transaction operation and initialization directory collection");
            operationDirectoryCount = checked(operationDirectoryCount + transactionDirectories.Count);
            transactionDirectories.Sort(PathComparer);
            foreach (var transactionDirectory in transactionDirectories)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (TryParseCanonicalGuidDirectory(transactionDirectory, out var operationId))
                {
                    PluginFileInspector.VerifyDirectory(transactionDirectory, "save transaction operation directory");
                    canonicalTransactions.Add((
                        new SaveTransactionPaths(outputDirectoryPath, workspaceId, operationId),
                        transactionDirectory));
                    continue;
                }

                if (TryParseInitializationDirectory(transactionDirectory, out _))
                {
                    ValidateInitializationContents(transactionDirectory);
                    continue;
                }

                throw new InvalidDataException($"The save metadata root contains an unrecognized operation directory: '{transactionDirectory}'.");
            }
        }

        var results = new List<(SaveTransactionPaths, SaveTransactionJournal)>(canonicalTransactions.Count);
        foreach (var transaction in canonicalTransactions)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateTransactionContents(transaction.Paths.TransactionDirectoryPath);
            var journal = await ReadAsync(transaction.Paths, cancellationToken).ConfigureAwait(false);
            if (journal is null)
            {
                throw new InvalidDataException($"The recognized save transaction directory has no durable journal: '{transaction.DirectoryPath}'.");
            }

            results.Add((transaction.Paths, journal));
        }

        return new SaveTransactionInventory(results.AsReadOnly(), workspaceIds, operationDirectoryCount);
    }

    /// <summary>Checks whether one bounded inventory has room to publish a new transaction directory.</summary>
    /// <param name="inventory">The complete inventory captured while the output lease is held.</param>
    /// <param name="workspaceId">The workspace that would own the new transaction.</param>
    /// <returns><see langword="true"/> when both workspace and operation-directory limits admit the transaction.</returns>
    internal bool HasCapacityForNewTransaction(SaveTransactionInventory inventory, Guid workspaceId)
    {
        ArgumentNullException.ThrowIfNull(inventory);
        if (workspaceId == Guid.Empty)
        {
            throw new ArgumentException("The workspace identifier must not be empty.", nameof(workspaceId));
        }

        return inventory.OperationDirectoryCount < MaximumMetadataEntryCount
            && (inventory.WorkspaceIds.Contains(workspaceId)
                || inventory.WorkspaceIds.Count < MaximumMetadataEntryCount);
    }

    /// <summary>Validates an existing bounded metadata hierarchy without creating it.</summary>
    /// <param name="paths">The exact transaction paths.</param>
    private void ValidateExistingHierarchy(SaveTransactionPaths paths)
    {
        PluginFileInspector.VerifyDirectory(paths.MetadataRootPath, "save transaction metadata root");
        if (File.Exists(paths.WorkspaceDirectoryPath))
        {
            throw new InvalidDataException($"The save transaction workspace path identifies a file: '{paths.WorkspaceDirectoryPath}'.");
        }

        if (!Directory.Exists(paths.WorkspaceDirectoryPath))
        {
            return;
        }

        PluginFileInspector.VerifyDirectory(paths.WorkspaceDirectoryPath, "save transaction workspace directory");
        if (File.Exists(paths.TransactionDirectoryPath))
        {
            throw new InvalidDataException($"The save transaction operation path identifies a file: '{paths.TransactionDirectoryPath}'.");
        }

        if (!Directory.Exists(paths.TransactionDirectoryPath))
        {
            return;
        }

        PluginFileInspector.VerifyDirectory(paths.TransactionDirectoryPath, "save transaction operation directory");
        ValidateTransactionContents(paths.TransactionDirectoryPath);
    }

    /// <summary>Creates or verifies one fixed owned metadata directory.</summary>
    /// <param name="path">The fixed path.</param>
    /// <param name="description">The diagnostic description.</param>
    private static void EnsureOwnedDirectory(string path, string description)
    {
        if (File.Exists(path))
        {
            throw new InvalidDataException($"The {description} path identifies a file: '{path}'.");
        }

        Directory.CreateDirectory(path);
        PluginFileInspector.VerifyDirectory(path, description);
    }

    /// <summary>Rejects foreign names inside a recognized transaction directory.</summary>
    /// <param name="transactionDirectoryPath">The exact recognized transaction directory.</param>
    private void ValidateTransactionContents(string transactionDirectoryPath)
    {
        var directories = EnumerateDirectoriesBounded(
            transactionDirectoryPath,
            AllowedTransactionDirectories.Count,
            "save transaction owned-directory collection");
        foreach (var directory in directories)
        {
            PluginFileInspector.VerifyDirectory(directory, "save transaction owned directory");
            if (!AllowedTransactionDirectories.Contains(Path.GetFileName(directory)))
            {
                throw new InvalidDataException($"The save transaction contains an unrecognized directory: '{directory}'.");
            }
        }

        var files = EnumerateFilesBounded(transactionDirectoryPath, "save transaction file collection");
        foreach (var file in files)
        {
            var name = Path.GetFileName(file);
            if (!string.Equals(name, JournalFileName, StringComparison.Ordinal)
                && !TryParseTemporaryJournalFileName(name))
            {
                throw new InvalidDataException($"The save transaction contains an unrecognized file: '{file}'.");
            }

            VerifyOwnedFile(file, "save transaction file");
        }
    }

    /// <summary>Validates the bounded contents of an unpublished initialization remnant.</summary>
    /// <param name="initializationDirectoryPath">The strict recognized initialization directory.</param>
    private void ValidateInitializationContents(string initializationDirectoryPath)
    {
        PluginFileInspector.VerifyDirectory(
            initializationDirectoryPath,
            "unpublished save transaction initialization directory");
        var directories = EnumerateDirectoriesBounded(
            initializationDirectoryPath,
            MaximumMetadataEntryCount,
            "save transaction initialization subdirectory collection");
        if (directories.Count != 0)
        {
            throw new InvalidDataException($"The unpublished save transaction initialization directory contains subdirectories: '{initializationDirectoryPath}'.");
        }

        var files = EnumerateFilesBounded(
            initializationDirectoryPath,
            "save transaction initialization file collection");
        foreach (var file in files)
        {
            if (!string.Equals(Path.GetFileName(file), JournalFileName, StringComparison.Ordinal))
            {
                throw new InvalidDataException($"The unpublished save transaction initialization directory contains an unrecognized file: '{file}'.");
            }

            VerifyOwnedFile(file, "unpublished save transaction initialization journal");
            if (new FileInfo(file).Length > MaximumJournalBytes)
            {
                throw new InvalidDataException($"The unpublished save transaction initialization journal exceeds its bounded maximum size: '{file}'.");
            }
        }
    }

    /// <summary>Enumerates direct child directories into a bounded list without unbounded sorting or materialization.</summary>
    /// <param name="path">The existing parent directory.</param>
    /// <param name="maximumCount">The nonnegative remaining entry capacity.</param>
    /// <param name="description">The collection description used in failures.</param>
    /// <returns>The direct child directory paths in file-system enumeration order.</returns>
    private static List<string> EnumerateDirectoriesBounded(
        string path,
        int maximumCount,
        string description)
    {
        var results = new List<string>(Math.Min(maximumCount, 64));
        foreach (var directory in Directory.EnumerateDirectories(path))
        {
            if (results.Count >= maximumCount)
            {
                throw new InvalidDataException($"The {description} exceeds its bounded maximum count.");
            }

            results.Add(directory);
        }

        return results;
    }

    /// <summary>Enumerates direct child files into a bounded list without unbounded materialization.</summary>
    /// <param name="path">The existing parent directory.</param>
    /// <param name="description">The collection description used in failures.</param>
    /// <returns>The direct child file paths in file-system enumeration order.</returns>
    private List<string> EnumerateFilesBounded(string path, string description)
    {
        var results = new List<string>(Math.Min(MaximumMetadataEntryCount, 64));
        foreach (var file in Directory.EnumerateFiles(path))
        {
            if (results.Count >= MaximumMetadataEntryCount)
            {
                throw new InvalidDataException($"The {description} exceeds its bounded maximum count.");
            }

            results.Add(file);
        }

        return results;
    }

    /// <summary>Rejects symbolic-link and reparse-point aliases for one recognized metadata file.</summary>
    /// <param name="path">The recognized metadata file.</param>
    /// <param name="description">The file description used in failures.</param>
    private static void VerifyOwnedFile(string path, string description)
    {
        var file = new FileInfo(path);
        if (!file.Exists
            || file.LinkTarget is not null
            || (file.Attributes & FileAttributes.ReparsePoint) != 0)
        {
            throw new InvalidDataException($"The {description} is absent or aliases another path: '{path}'.");
        }
    }

    /// <summary>Validates that a journal's durable identity matches its canonical bounded path.</summary>
    /// <param name="paths">The canonical transaction paths.</param>
    /// <param name="journal">The parsed or pending journal.</param>
    private static void ValidateJournalIdentity(SaveTransactionPaths paths, SaveTransactionJournal journal)
    {
        if (journal.WorkspaceId.ToString("N") != Path.GetFileName(paths.WorkspaceDirectoryPath)
            || journal.SaveOperationId.ToString("N") != Path.GetFileName(paths.TransactionDirectoryPath))
        {
            throw new InvalidDataException("The save journal identity does not match its bounded transaction path.");
        }
    }

    /// <summary>Parses the exact initialization directory shape and both non-empty canonical identifiers.</summary>
    /// <param name="path">The directory path.</param>
    /// <param name="operationId">The operation identifier embedded in a recognized name.</param>
    /// <returns><see langword="true"/> only for the strict initialization-directory shape.</returns>
    private static bool TryParseInitializationDirectory(string path, out Guid operationId)
    {
        operationId = Guid.Empty;
        var name = Path.GetFileName(path);
        var expectedLength = InitializationDirectoryPrefix.Length + 32 + 1 + 32;
        if (name.Length != expectedLength
            || !name.StartsWith(InitializationDirectoryPrefix, StringComparison.Ordinal)
            || name[InitializationDirectoryPrefix.Length + 32] != '-')
        {
            return false;
        }

        var operationText = name.Substring(InitializationDirectoryPrefix.Length, 32);
        var nonceText = name[(InitializationDirectoryPrefix.Length + 33)..];
        return Guid.TryParseExact(operationText, "N", out operationId)
            && operationId != Guid.Empty
            && string.Equals(operationText, operationId.ToString("N"), StringComparison.Ordinal)
            && Guid.TryParseExact(nonceText, "N", out var nonce)
            && nonce != Guid.Empty
            && string.Equals(nonceText, nonce.ToString("N"), StringComparison.Ordinal);
    }

    /// <summary>Recognizes only the exact temporary journal filename emitted by durable replacement writes.</summary>
    /// <param name="name">The direct child filename.</param>
    /// <returns><see langword="true"/> only for a lowercase N-format non-empty nonce.</returns>
    private static bool TryParseTemporaryJournalFileName(string name)
    {
        const string prefix = ".journal-";
        const string suffix = ".tmp";
        if (name.Length != prefix.Length + 32 + suffix.Length
            || !name.StartsWith(prefix, StringComparison.Ordinal)
            || !name.EndsWith(suffix, StringComparison.Ordinal))
        {
            return false;
        }

        var nonceText = name.Substring(prefix.Length, 32);
        return Guid.TryParseExact(nonceText, "N", out var nonce)
            && nonce != Guid.Empty
            && string.Equals(nonceText, nonce.ToString("N"), StringComparison.Ordinal);
    }

    /// <summary>Parses an exact lowercase N-format GUID directory name.</summary>
    /// <param name="path">The directory path.</param>
    /// <param name="value">The parsed identifier when recognized.</param>
    /// <returns><see langword="true"/> only for a non-empty canonical identifier name.</returns>
    private static bool TryParseCanonicalGuidDirectory(string path, out Guid value)
    {
        var name = Path.GetFileName(path);
        return Guid.TryParseExact(name, "N", out value)
            && value != Guid.Empty
            && string.Equals(name, value.ToString("N"), StringComparison.Ordinal);
    }

    /// <summary>Gets platform path ordering semantics.</summary>
    private static StringComparer PathComparer => OperatingSystem.IsWindows()
        ? StringComparer.OrdinalIgnoreCase
        : StringComparer.Ordinal;
}
