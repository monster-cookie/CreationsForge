using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;
using Noggog;

namespace CreationsForge.Core.Engine.NativeInputs;

/// <summary>
/// Owns one prepared explicit native source-input lifetime and verifies that its complete physical baseline remains unchanged.
/// </summary>
public sealed class NativeSourceInputs : INativeSourceSet
{
    /// <summary>The immutable master-style lookup retained behind native parsing setup.</summary>
    private readonly IReadOnlyCache<IModMasterStyledGetter, Mutagen.Bethesda.Plugins.ModKey> MasterFlagsLookup;

    /// <summary>The plugin-specific native localized-string lookups.</summary>
    private readonly IReadOnlyDictionary<Mutagen.Bethesda.Plugins.ModKey, NativeStringsFolderLookup> StringsLookups;

    /// <summary>The collector used to recapture the exact physical artifact set.</summary>
    private readonly NativeSourceArtifactCollector ArtifactCollector;

    /// <summary>The initial observations captured while preparing the input set.</summary>
    private readonly IReadOnlyList<NativeArtifactAssociation> InitialArtifacts;

    /// <summary>Serializes baseline establishment and verification.</summary>
    private readonly SemaphoreSlim VerificationGate = new(1, 1);

    /// <summary>The completed immutable baseline, or <see langword="null"/> until parsing is confirmed.</summary>
    private NativeSourceInputBaseline? CompletedBaseline;

    /// <summary>Tracks whether ownership has been released.</summary>
    private int IsDisposed;

    /// <summary>Initializes a prepared native source-input lifetime.</summary>
    /// <param name="release">The selected native release.</param>
    /// <param name="plugins">The explicit plugin descriptors in load-order order.</param>
    /// <param name="masterFlagsLookup">The private immutable-surface master-style lookup.</param>
    /// <param name="stringsLookups">The plugin-specific strings lookups.</param>
    /// <param name="artifactCollector">The collector used for later source verification.</param>
    /// <param name="initialArtifacts">The complete physical observations captured during preparation.</param>
    internal NativeSourceInputs(
        GameRelease release,
        IReadOnlyList<NativeSourcePluginInput> plugins,
        IReadOnlyCache<IModMasterStyledGetter, Mutagen.Bethesda.Plugins.ModKey> masterFlagsLookup,
        IReadOnlyDictionary<Mutagen.Bethesda.Plugins.ModKey, NativeStringsFolderLookup> stringsLookups,
        NativeSourceArtifactCollector artifactCollector,
        IReadOnlyList<NativeArtifactAssociation> initialArtifacts)
    {
        Release = release;
        Plugins = Array.AsReadOnly(plugins.ToArray());
        MasterFlagsLookup = masterFlagsLookup;
        StringsLookups = stringsLookups;
        ArtifactCollector = artifactCollector;
        InitialArtifacts = Array.AsReadOnly(initialArtifacts.ToArray());
    }

    /// <summary>Gets the exact native game release selected by the explicit open request.</summary>
    public GameRelease Release { get; }

    /// <summary>Gets the completed immutable baseline for every physical source artifact.</summary>
    /// <exception cref="InvalidOperationException">Thrown before native opening establishes the complete baseline.</exception>
    /// <exception cref="ObjectDisposedException">Thrown after this source-input lifetime is disposed.</exception>
    public NativeSourceInputBaseline Baseline
    {
        get
        {
            ThrowIfDisposed();
            return CompletedBaseline
                ?? throw new InvalidOperationException("The native source baseline is unavailable before opening is completed.");
        }
    }

    /// <summary>Gets the immutable plugin descriptors in caller-supplied load-order order.</summary>
    public IReadOnlyList<NativeSourcePluginInput> Plugins { get; }

    /// <summary>Creates native parsing metadata for one plugin in this source-input set.</summary>
    /// <param name="plugin">A descriptor obtained from <see cref="Plugins"/>.</param>
    /// <returns>Fresh parsing metadata with the verified master-style cache and ordered localized-string lookup.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="plugin"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when the descriptor does not belong to this input set.</exception>
    /// <exception cref="ObjectDisposedException">Thrown after this source-input lifetime is disposed.</exception>
    public ParsingMeta CreateParsingMeta(NativeSourcePluginInput plugin)
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

    /// <summary>Opens a caller-owned native read stream whose synchronous parser reads and seeks observe an operation-scoped cancellation token.</summary>
    /// <param name="plugin">A descriptor obtained from <see cref="Plugins"/>.</param>
    /// <param name="cancellationToken">The token applied only to this returned parse stream.</param>
    /// <returns>A native read stream that disposes its underlying read-only file stream.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="plugin"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when the descriptor does not belong to this input set.</exception>
    /// <exception cref="ObjectDisposedException">Thrown after this source-input lifetime is disposed.</exception>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is already requested or becomes requested at a read or seek boundary.</exception>
    public MutagenBinaryReadStream OpenReadStream(
        NativeSourcePluginInput plugin,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        ValidatePluginMembership(plugin);
        cancellationToken.ThrowIfCancellationRequested();
        var fileStream = new FileStream(
            plugin.Path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read | FileShare.Delete,
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

    /// <summary>Resolves one localized string through the same ordered native lookup used by parsing and reports its physical source path.</summary>
    /// <param name="plugin">A descriptor obtained from <see cref="Plugins"/>.</param>
    /// <param name="source">The native strings file category.</param>
    /// <param name="language">The requested native language.</param>
    /// <param name="key">The native string key.</param>
    /// <param name="value">The resolved string value, or an empty string when absent.</param>
    /// <param name="sourcePath">The native source path reported by Mutagen, or an empty string when absent.</param>
    /// <returns><see langword="true"/> when an explicit native strings input resolves the key.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="plugin"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when the descriptor does not belong to this input set.</exception>
    /// <exception cref="ObjectDisposedException">Thrown after this source-input lifetime is disposed.</exception>
    /// <remarks>Loose sidecar paths are physical file evidence. Archive-internal entry attribution depends on Mutagen and is not asserted by this helper.</remarks>
    public bool TryLookupString(
        NativeSourcePluginInput plugin,
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

    /// <summary>Confirms that native parsing observed the prepared physical source state and establishes its deterministic baseline.</summary>
    /// <param name="cancellationToken">The token checked during recapture and hashing.</param>
    /// <returns>The immutable completed baseline, or a typed external-change or source-open failure.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    /// <exception cref="ObjectDisposedException">Thrown after this source-input lifetime is disposed.</exception>
    public async Task<EngineResult<NativeSourceInputBaseline>> CompleteOpenAsync(
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        await VerificationGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ThrowIfDisposed();
            var expected = CompletedBaseline?.Artifacts ?? InitialArtifacts;
            var currentResult = await CaptureAndCompareAsync(expected, cancellationToken).ConfigureAwait(false);
            if (!currentResult.Succeeded)
            {
                return EngineResult<NativeSourceInputBaseline>.Failure(currentResult.Error!);
            }

            if (CompletedBaseline is null)
            {
                CompletedBaseline = new NativeSourceInputBaseline(
                    NativeArtifactSetUtilities.CreateBaselineId(
                        "CreationsForge.NativeSourceInputBaseline/v1",
                        currentResult.Value!),
                    currentResult.Value!);
            }

            return EngineResult<NativeSourceInputBaseline>.Success(CompletedBaseline);
        }
        finally
        {
            VerificationGate.Release();
        }
    }

    /// <summary>Verifies that every prepared native input and selected inventory entry still matches the completed baseline.</summary>
    /// <param name="cancellationToken">The token checked during recapture and hashing.</param>
    /// <returns>The existing immutable baseline on success, or a typed failure describing why verification could not be completed.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    /// <exception cref="ObjectDisposedException">Thrown after this source-input lifetime is disposed.</exception>
    public async Task<EngineResult<NativeSourceInputBaseline>> VerifyUnchangedAsync(
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        await VerificationGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ThrowIfDisposed();
            if (CompletedBaseline is null)
            {
                return EngineResult<NativeSourceInputBaseline>.Failure(new EngineError(
                    EngineErrorCode.InvalidRequest,
                    "The native source baseline cannot be verified before opening is completed."));
            }

            var currentResult = await CaptureAndCompareAsync(CompletedBaseline.Artifacts, cancellationToken).ConfigureAwait(false);
            if (!currentResult.Succeeded)
            {
                return EngineResult<NativeSourceInputBaseline>.Failure(currentResult.Error!);
            }

            return EngineResult<NativeSourceInputBaseline>.Success(CompletedBaseline);
        }
        finally
        {
            VerificationGate.Release();
        }
    }

    /// <summary>Releases this independently owned input lifetime and invalidates further parsing or verification.</summary>
    /// <returns>A completed task because the input set retains no open source handles between operations.</returns>
    public async ValueTask DisposeAsync()
    {
        await VerificationGate.WaitAsync().ConfigureAwait(false);
        try
        {
            Interlocked.Exchange(ref IsDisposed, 1);
        }
        finally
        {
            VerificationGate.Release();
        }
    }

    /// <summary>Recaptures the physical source set and compares every ordered artifact observation.</summary>
    /// <param name="expected">The expected complete artifact set.</param>
    /// <param name="cancellationToken">The token checked during discovery and hashing.</param>
    /// <returns>The fresh matching artifact set or a typed failure.</returns>
    private async Task<EngineResult<IReadOnlyList<NativeArtifactAssociation>>> CaptureAndCompareAsync(
        IReadOnlyList<NativeArtifactAssociation> expected,
        CancellationToken cancellationToken)
    {
        try
        {
            var current = await ArtifactCollector.CaptureAsync(cancellationToken).ConfigureAwait(false);
            if (!NativeArtifactSetUtilities.Match(expected, current))
            {
                return EngineResult<IReadOnlyList<NativeArtifactAssociation>>.Failure(new EngineError(
                    EngineErrorCode.ExternalChangeDetected,
                    "One or more explicit native source files, localized strings sidecars, or applicable archives changed after preparation."));
            }

            return EngineResult<IReadOnlyList<NativeArtifactAssociation>>.Success(current);
        }
        catch (NativeSourceInputException exception)
        {
            var code = exception.Code is EngineErrorCode.InvalidRequest or EngineErrorCode.UnsupportedInput
                ? EngineErrorCode.ExternalChangeDetected
                : exception.Code;
            return EngineResult<IReadOnlyList<NativeArtifactAssociation>>.Failure(new EngineError(
                code,
                exception.Message));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            return EngineResult<IReadOnlyList<NativeArtifactAssociation>>.Failure(new EngineError(
                EngineErrorCode.SourceOpenFailed,
                $"The explicit native source baseline could not be verified: {exception.Message}"));
        }
    }

    /// <summary>Requires that a plugin descriptor is one of this set's immutable descriptor instances.</summary>
    /// <param name="plugin">The descriptor supplied by the caller.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="plugin"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when the descriptor belongs to another input set.</exception>
    private void ValidatePluginMembership(NativeSourcePluginInput plugin)
    {
        ArgumentNullException.ThrowIfNull(plugin);
        if (!Plugins.Contains(plugin))
        {
            throw new ArgumentException("The native plugin descriptor does not belong to this source-input set.", nameof(plugin));
        }
    }

    /// <summary>Throws when this independently owned input lifetime has been released.</summary>
    /// <exception cref="ObjectDisposedException">Thrown after <see cref="DisposeAsync"/>.</exception>
    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref IsDisposed) != 0, this);
    }
}
