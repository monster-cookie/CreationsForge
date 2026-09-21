using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;
using Noggog;

namespace CreationsForge.Core.Engine.PluginInputs;

/// <summary>
/// Owns one prepared explicit plugin source-input lifetime and the read locks that keep its physical sources immutable.
/// </summary>
public sealed class PluginSourceInputs : IPluginSourceSet
{
    /// <summary>The immutable master-style lookup retained behind plugin parsing setup.</summary>
    private readonly IReadOnlyCache<IModMasterStyledGetter, Mutagen.Bethesda.Plugins.ModKey> MasterFlagsLookup;

    /// <summary>The explicit plugin data directory used for archive-backed localized strings.</summary>
    private readonly string DataDirectoryPath;

    /// <summary>The explicit loose localized-string directories in caller priority order.</summary>
    private readonly IReadOnlyList<string> StringDirectoryPaths;

    /// <summary>The plugin-specific plugin localized-string lookups.</summary>
    private readonly IReadOnlyDictionary<Mutagen.Bethesda.Plugins.ModKey, PluginStringsFolderLookup> StringsLookups;

    /// <summary>The retained read locks that deny source writers and deletion for this lifetime.</summary>
    private readonly PluginSourceFileLockSet SourceLocks;

    /// <summary>The initial observations captured while preparing the input set.</summary>
    private readonly IReadOnlyList<PluginArtifactAssociation> InitialArtifacts;

    /// <summary>Serializes baseline establishment and verification.</summary>
    private readonly SemaphoreSlim VerificationGate = new(1, 1);

    /// <summary>The completed immutable baseline, or <see langword="null"/> until parsing is confirmed.</summary>
    private PluginSourceInputBaseline? CompletedBaseline;

    /// <summary>Tracks whether ownership has been released.</summary>
    private int IsDisposed;

    /// <summary>Initializes a prepared plugin source-input lifetime.</summary>
    /// <param name="release">The selected plugin release.</param>
    /// <param name="plugins">The explicit plugin descriptors in load-order order.</param>
    /// <param name="dataDirectoryPath">The explicit plugin data directory used for archive lookup.</param>
    /// <param name="stringDirectoryPaths">The explicit loose localized-string directories in lookup-priority order.</param>
    /// <param name="recordTextLanguage">The explicit language used for localized record text.</param>
    /// <param name="masterFlagsLookup">The private immutable-surface master-style lookup.</param>
    /// <param name="stringsLookups">The plugin-specific strings lookups.</param>
    /// <param name="sourceLocks">The retained read locks for every existing source artifact.</param>
    /// <param name="initialArtifacts">The complete physical observations captured during preparation.</param>
    internal PluginSourceInputs(
        GameRelease release,
        IReadOnlyList<PluginSourcePluginInput> plugins,
        string dataDirectoryPath,
        IReadOnlyList<string> stringDirectoryPaths,
        Language recordTextLanguage,
        IReadOnlyCache<IModMasterStyledGetter, Mutagen.Bethesda.Plugins.ModKey> masterFlagsLookup,
        IReadOnlyDictionary<Mutagen.Bethesda.Plugins.ModKey, PluginStringsFolderLookup> stringsLookups,
        PluginSourceFileLockSet sourceLocks,
        IReadOnlyList<PluginArtifactAssociation> initialArtifacts)
    {
        Release = release;
        Plugins = Array.AsReadOnly(plugins.ToArray());
        DataDirectoryPath = dataDirectoryPath;
        StringDirectoryPaths = Array.AsReadOnly(stringDirectoryPaths.ToArray());
        RecordTextLanguage = recordTextLanguage;
        MasterFlagsLookup = masterFlagsLookup;
        StringsLookups = stringsLookups;
        SourceLocks = sourceLocks;
        InitialArtifacts = Array.AsReadOnly(initialArtifacts.ToArray());
    }

    /// <summary>Gets the exact plugin game release selected by the explicit open request.</summary>
    public GameRelease Release { get; }

    /// <summary>Gets the explicit language used for localized record text.</summary>
    public Language RecordTextLanguage { get; }

    /// <summary>Gets the completed immutable baseline for every physical source artifact.</summary>
    /// <exception cref="InvalidOperationException">Thrown before plugin opening establishes the complete baseline.</exception>
    /// <exception cref="ObjectDisposedException">Thrown after this source-input lifetime is disposed.</exception>
    public PluginSourceInputBaseline Baseline
    {
        get
        {
            ThrowIfDisposed();
            return CompletedBaseline
                ?? throw new InvalidOperationException("The plugin source baseline is unavailable before opening is completed.");
        }
    }

    /// <summary>Gets the immutable plugin descriptors in caller-supplied load-order order.</summary>
    public IReadOnlyList<PluginSourcePluginInput> Plugins { get; }

    /// <summary>Creates plugin parsing metadata for one plugin in this source-input set.</summary>
    /// <param name="plugin">A descriptor obtained from <see cref="Plugins"/>.</param>
    /// <returns>Fresh parsing metadata with the verified master-style cache and ordered localized-string lookup.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="plugin"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when the descriptor does not belong to this input set.</exception>
    /// <exception cref="ObjectDisposedException">Thrown after this source-input lifetime is disposed.</exception>
    public ParsingMeta CreateParsingMeta(PluginSourcePluginInput plugin)
    {
        ThrowIfDisposed();
        ValidatePluginMembership(plugin);
        var parameters = new BinaryReadParameters
        {
            MasterFlagsLookup = MasterFlagsLookup
        };
        var metadata = ParsingMeta.Factory(parameters, Release, plugin.ModPath);
        if (plugin.UsesLocalization)
        {
            metadata.StringsLookup = StringsLookups[plugin.ModKey];
        }

        return metadata;
    }

    /// <summary>Reports whether Mutagen's public overlay API can preserve this plugin's explicit localized-string lookup contract.</summary>
    /// <param name="plugin">A descriptor obtained from <see cref="Plugins"/>.</param>
    /// <returns><see langword="true"/> for nonlocalized plugins or localized plugins with exactly one explicit lookup directory.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="plugin"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when the descriptor does not belong to this input set.</exception>
    /// <exception cref="ObjectDisposedException">Thrown after this source-input lifetime is disposed.</exception>
    public bool SupportsBinaryOverlay(PluginSourcePluginInput plugin)
    {
        ThrowIfDisposed();
        ValidatePluginMembership(plugin);
        return !plugin.UsesLocalization || StringDirectoryPaths.Count == 1;
    }

    /// <summary>Creates plugin binary-read parameters for a disposable read-only overlay without ambient path discovery.</summary>
    /// <param name="plugin">A descriptor obtained from <see cref="Plugins"/>.</param>
    /// <returns>Fresh parameters containing the verified master styles and, when needed, the one explicit strings lookup location.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="plugin"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when the descriptor does not belong to this input set.</exception>
    /// <exception cref="InvalidOperationException">Thrown when a localized plugin requires multiple prioritized loose-string directories that the public overlay API cannot represent.</exception>
    /// <exception cref="ObjectDisposedException">Thrown after this source-input lifetime is disposed.</exception>
    public BinaryReadParameters CreateOverlayReadParameters(PluginSourcePluginInput plugin)
    {
        ThrowIfDisposed();
        ValidatePluginMembership(plugin);
        if (!SupportsBinaryOverlay(plugin))
        {
            throw new InvalidOperationException(
                "Mutagen's public binary overlay API cannot represent multiple prioritized localized-string directories.");
        }

        if (!plugin.UsesLocalization)
        {
            return new BinaryReadParameters
            {
                MasterFlagsLookup = MasterFlagsLookup
            };
        }

        return new BinaryReadParameters
        {
            MasterFlagsLookup = MasterFlagsLookup,
            StringsParam = new StringsReadParameters
            {
                StringsFolderOverride = StringDirectoryPaths[0],
                BsaFolderOverride = DataDirectoryPath,
                TargetLanguage = RecordTextLanguage
            }
        };
    }

    /// <summary>Opens a caller-owned read stream for a lazy overlay under the workspace's retained source read lock.</summary>
    /// <param name="plugin">A descriptor obtained from <see cref="Plugins"/>.</param>
    /// <returns>A seekable source stream that must remain open for the overlay lifetime.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="plugin"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when the descriptor does not belong to this input set.</exception>
    /// <exception cref="ObjectDisposedException">Thrown after this source-input lifetime is disposed.</exception>
    /// <exception cref="IOException">Thrown when the source file cannot be opened for reading.</exception>
    public Stream OpenOverlayStream(PluginSourcePluginInput plugin)
    {
        ThrowIfDisposed();
        ValidatePluginMembership(plugin);
        return new FileStream(
            plugin.Path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            131072,
            FileOptions.RandomAccess);
    }

    /// <summary>Opens a caller-owned plugin read stream whose synchronous parser reads and seeks observe an operation-scoped cancellation token.</summary>
    /// <param name="plugin">A descriptor obtained from <see cref="Plugins"/>.</param>
    /// <param name="cancellationToken">The token applied only to this returned parse stream.</param>
    /// <returns>A plugin read stream that disposes its underlying read-only file stream.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="plugin"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when the descriptor does not belong to this input set.</exception>
    /// <exception cref="ObjectDisposedException">Thrown after this source-input lifetime is disposed.</exception>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is already requested or becomes requested at a read or seek boundary.</exception>
    public MutagenBinaryReadStream OpenReadStream(
        PluginSourcePluginInput plugin,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        ValidatePluginMembership(plugin);
        cancellationToken.ThrowIfCancellationRequested();
        var fileStream = new FileStream(
            plugin.Path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            131072,
            FileOptions.SequentialScan);
        try
        {
            var checkingStream = new CancellationCheckingStream(fileStream, cancellationToken);
            return new MutagenBinaryReadStream(
                checkingStream,
                CreateParsingMeta(plugin),
                bufferSize: 4096,
                dispose: true,
                offsetReference: 0);
        }
        catch
        {
            fileStream.Dispose();
            throw;
        }
    }

    /// <summary>Resolves one localized string through the same ordered plugin lookup used by parsing and reports its physical source path.</summary>
    /// <param name="plugin">A descriptor obtained from <see cref="Plugins"/>.</param>
    /// <param name="source">The record strings file category.</param>
    /// <param name="language">The requested plugin language.</param>
    /// <param name="key">The record string key.</param>
    /// <param name="value">The resolved string value, or an empty string when absent.</param>
    /// <param name="sourcePath">The plugin source path reported by Mutagen, or an empty string when absent.</param>
    /// <returns><see langword="true"/> when an explicit record strings input resolves the key.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="plugin"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when the descriptor does not belong to this input set.</exception>
    /// <exception cref="ObjectDisposedException">Thrown after this source-input lifetime is disposed.</exception>
    /// <remarks>Loose sidecar paths are physical file evidence. Archive-internal entry attribution depends on Mutagen and is not asserted by this helper.</remarks>
    public bool TryLookupString(
        PluginSourcePluginInput plugin,
        StringsSource source,
        Language language,
        uint key,
        out string value,
        out string sourcePath)
    {
        ThrowIfDisposed();
        ValidatePluginMembership(plugin);
        if (!plugin.UsesLocalization)
        {
            value = string.Empty;
            sourcePath = string.Empty;
            return false;
        }

        return StringsLookups[plugin.ModKey].TryLookup(source, language, key, out value, out sourcePath);
    }

    /// <summary>Confirms that plugin parsing completed while the prepared source locks remained held and establishes the deterministic baseline.</summary>
    /// <param name="cancellationToken">The token checked while entering the source-lifetime gate.</param>
    /// <returns>The immutable completed baseline.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    /// <exception cref="ObjectDisposedException">Thrown after this source-input lifetime is disposed.</exception>
    public async Task<EngineResult<PluginSourceInputBaseline>> CompleteOpenAsync(
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        await VerificationGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ThrowIfDisposed();
            CompletedBaseline ??= new PluginSourceInputBaseline(
                PluginArtifactSetUtilities.CreateBaselineId(
                    "CreationsForge.PluginSourceInputBaseline/v1",
                    InitialArtifacts),
                InitialArtifacts);
            return EngineResult<PluginSourceInputBaseline>.Success(CompletedBaseline);
        }
        finally
        {
            VerificationGate.Release();
        }
    }

    /// <summary>Returns the completed source baseline while the workspace's retained read locks remain owned.</summary>
    /// <param name="cancellationToken">The token checked while entering the source-lifetime gate.</param>
    /// <returns>The existing immutable baseline, or a typed failure when opening has not completed.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    /// <exception cref="ObjectDisposedException">Thrown after this source-input lifetime is disposed.</exception>
    public async Task<EngineResult<PluginSourceInputBaseline>> VerifyUnchangedAsync(
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        await VerificationGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ThrowIfDisposed();
            return CompletedBaseline is null
                ? EngineResult<PluginSourceInputBaseline>.Failure(new EngineError(
                    EngineErrorCode.InvalidRequest,
                    "The plugin source baseline cannot be verified before opening is completed."))
                : EngineResult<PluginSourceInputBaseline>.Success(CompletedBaseline);
        }
        finally
        {
            VerificationGate.Release();
        }
    }

    /// <summary>Releases the retained source read locks and invalidates further parsing or verification.</summary>
    public async ValueTask DisposeAsync()
    {
        await VerificationGate.WaitAsync().ConfigureAwait(false);
        try
        {
            if (Interlocked.Exchange(ref IsDisposed, 1) == 0)
            {
                await SourceLocks.DisposeAsync().ConfigureAwait(false);
            }
        }
        finally
        {
            VerificationGate.Release();
        }
    }

    /// <summary>Requires that a plugin descriptor is one of this set's immutable descriptor instances.</summary>
    /// <param name="plugin">The descriptor supplied by the caller.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="plugin"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when the descriptor belongs to another input set.</exception>
    private void ValidatePluginMembership(PluginSourcePluginInput plugin)
    {
        ArgumentNullException.ThrowIfNull(plugin);
        if (!Plugins.Contains(plugin))
        {
            throw new ArgumentException("The plugin descriptor does not belong to this source-input set.", nameof(plugin));
        }
    }

    /// <summary>Throws when this independently owned input lifetime has been released.</summary>
    /// <exception cref="ObjectDisposedException">Thrown after <see cref="DisposeAsync"/>.</exception>
    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref IsDisposed) != 0, this);
    }
}
