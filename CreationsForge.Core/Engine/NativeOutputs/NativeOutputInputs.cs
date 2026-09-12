using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInputs;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;
using Noggog;

namespace CreationsForge.Core.Engine.NativeOutputs;

/// <summary>Owns a short-lived native output admission and parsing scope without owning its borrowed sources.</summary>
public sealed class NativeOutputInputs : IAsyncDisposable
{
    /// <summary>The borrowed source lifetime used for admission and unchanged-state verification.</summary>
    private readonly NativeSourceInputs BorrowedSources;

    /// <summary>The immutable native master-style lookup used for output parsing.</summary>
    private readonly IReadOnlyCache<IModMasterStyledGetter, ModKey> MasterFlagsLookup;

    /// <summary>The bounded loose strings lookup for an existing localized output.</summary>
    private readonly StringsFolderLookupOverlay? StringsLookup;

    /// <summary>The collector used to recapture the complete output artifact set.</summary>
    private readonly NativeOutputArtifactCollector ArtifactCollector;

    /// <summary>The initial output observations captured during admission.</summary>
    private readonly IReadOnlyList<NativeArtifactAssociation> InitialArtifacts;

    /// <summary>Serializes baseline establishment, verification, and disposal.</summary>
    private readonly SemaphoreSlim VerificationGate = new(1, 1);

    /// <summary>The completed immutable output baseline, or <see langword="null"/> until parsing finishes.</summary>
    private OutputArtifactSetBaseline? Baseline;

    /// <summary>Tracks whether the short-lived parsing scope has been released.</summary>
    private int IsDisposed;

    /// <summary>Initializes a prepared output admission and parsing scope.</summary>
    /// <param name="borrowedSources">The caller-owned native source lifetime.</param>
    /// <param name="output">The verified output descriptor.</param>
    /// <param name="masterFlagsLookup">The native master-style lookup.</param>
    /// <param name="stringsLookup">The existing output's loose strings lookup, when needed.</param>
    /// <param name="artifactCollector">The collector used for output verification.</param>
    /// <param name="initialArtifacts">The complete output observations captured during admission.</param>
    internal NativeOutputInputs(
        NativeSourceInputs borrowedSources,
        NativeOutputPluginInput output,
        IReadOnlyCache<IModMasterStyledGetter, ModKey> masterFlagsLookup,
        StringsFolderLookupOverlay? stringsLookup,
        NativeOutputArtifactCollector artifactCollector,
        IReadOnlyList<NativeArtifactAssociation> initialArtifacts)
    {
        BorrowedSources = borrowedSources;
        Output = output;
        MasterFlagsLookup = masterFlagsLookup;
        StringsLookup = stringsLookup;
        ArtifactCollector = artifactCollector;
        InitialArtifacts = Array.AsReadOnly(initialArtifacts.ToArray());
    }

    /// <summary>Gets the exact native release selected by the borrowed source set.</summary>
    public GameRelease Release => BorrowedSources.Release;

    /// <summary>Gets the verified output plugin descriptor.</summary>
    public NativeOutputPluginInput Output { get; }

    /// <summary>Creates strict native parsing metadata for the admitted output.</summary>
    /// <returns>Fresh metadata with verified master flags, strict unknown-subrecord handling, and bounded loose strings lookup.</returns>
    /// <exception cref="ObjectDisposedException">Thrown after this short-lived output parsing scope is disposed.</exception>
    public ParsingMeta CreateParsingMeta()
    {
        ThrowIfDisposed();
        var parameters = new BinaryReadParameters
        {
            MasterFlagsLookup = MasterFlagsLookup,
            ThrowOnUnknownSubrecord = true
        };
        var metadata = ParsingMeta.Factory(parameters, Release, Output.ModPath);
        if (Output.UsesLocalization && StringsLookup is not null)
        {
            metadata.StringsLookup = StringsLookup;
        }

        return metadata;
    }

    /// <summary>Opens a caller-owned cancellation-aware read stream for an existing admitted output.</summary>
    /// <param name="cancellationToken">The token checked at synchronous parser read and seek boundaries.</param>
    /// <returns>A native read stream that owns its underlying read-only file stream.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the selected output is intentionally absent for creation.</exception>
    /// <exception cref="ObjectDisposedException">Thrown after this short-lived output parsing scope is disposed.</exception>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    public MutagenBinaryReadStream OpenReadStream(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        cancellationToken.ThrowIfCancellationRequested();
        if (!Output.Exists)
        {
            throw new InvalidOperationException("A CreateNew output has no plugin stream to open.");
        }

        var fileStream = new FileStream(
            Output.Path,
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
                CreateParsingMeta(),
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

    /// <summary>Confirms full native materialization observed unchanged sources and output, then establishes the output baseline.</summary>
    /// <param name="cancellationToken">The token checked during source verification, discovery, and hashing.</param>
    /// <returns>The immutable completed output baseline or a typed failure.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    /// <exception cref="ObjectDisposedException">Thrown after this short-lived output parsing scope is disposed.</exception>
    public async Task<EngineResult<OutputArtifactSetBaseline>> CompleteOpenAsync(
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        await VerificationGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ThrowIfDisposed();
            var expected = Baseline?.Artifacts ?? InitialArtifacts;
            var currentResult = await CaptureStableAsync(expected, cancellationToken).ConfigureAwait(false);
            if (!currentResult.Succeeded)
            {
                return EngineResult<OutputArtifactSetBaseline>.Failure(currentResult.Error!);
            }

            if (Baseline is null)
            {
                Baseline = new OutputArtifactSetBaseline(
                    NativeArtifactSetUtilities.CreateBaselineId(
                        "CreationsForge.OutputArtifactSetBaseline/v1",
                        currentResult.Value!),
                    currentResult.Value!);
            }

            return EngineResult<OutputArtifactSetBaseline>.Success(Baseline);
        }
        finally
        {
            VerificationGate.Release();
        }
    }

    /// <summary>Verifies that borrowed sources and every output artifact still match the completed baseline.</summary>
    /// <param name="cancellationToken">The token checked during source verification, discovery, and hashing.</param>
    /// <returns>The existing immutable output baseline or a typed failure.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    /// <exception cref="ObjectDisposedException">Thrown after this short-lived output parsing scope is disposed.</exception>
    public async Task<EngineResult<OutputArtifactSetBaseline>> VerifyUnchangedAsync(
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        await VerificationGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ThrowIfDisposed();
            if (Baseline is null)
            {
                return EngineResult<OutputArtifactSetBaseline>.Failure(new EngineError(
                    EngineErrorCode.InvalidRequest,
                    "The native output baseline cannot be verified before opening is completed."));
            }

            var currentResult = await CaptureStableAsync(Baseline.Artifacts, cancellationToken).ConfigureAwait(false);
            if (!currentResult.Succeeded)
            {
                return EngineResult<OutputArtifactSetBaseline>.Failure(currentResult.Error!);
            }

            return EngineResult<OutputArtifactSetBaseline>.Success(Baseline);
        }
        finally
        {
            VerificationGate.Release();
        }
    }

    /// <summary>Releases this output-only parsing scope without disposing or otherwise changing its borrowed source lifetime.</summary>
    /// <returns>A task that completes after any active output verification leaves the scope.</returns>
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

    /// <summary>Recaptures a stable output observation around an unchanged-source verification.</summary>
    /// <param name="expected">The exact expected output artifact set.</param>
    /// <param name="cancellationToken">The token checked during source verification, discovery, and hashing.</param>
    /// <returns>The matching fresh output artifacts or a typed failure.</returns>
    private async Task<EngineResult<IReadOnlyList<NativeArtifactAssociation>>> CaptureStableAsync(
        IReadOnlyList<NativeArtifactAssociation> expected,
        CancellationToken cancellationToken)
    {
        try
        {
            var sourceBefore = await BorrowedSources.VerifyUnchangedAsync(cancellationToken).ConfigureAwait(false);
            if (!sourceBefore.Succeeded)
            {
                return EngineResult<IReadOnlyList<NativeArtifactAssociation>>.Failure(sourceBefore.Error!);
            }

            var first = await ArtifactCollector.CaptureAsync(cancellationToken).ConfigureAwait(false);
            if (!NativeArtifactSetUtilities.Match(expected, first))
            {
                return ChangedOutputFailure();
            }

            var sourceAfter = await BorrowedSources.VerifyUnchangedAsync(cancellationToken).ConfigureAwait(false);
            if (!sourceAfter.Succeeded)
            {
                return EngineResult<IReadOnlyList<NativeArtifactAssociation>>.Failure(sourceAfter.Error!);
            }

            var second = await ArtifactCollector.CaptureAsync(cancellationToken).ConfigureAwait(false);
            if (!NativeArtifactSetUtilities.Match(first, second))
            {
                return ChangedOutputFailure();
            }

            return EngineResult<IReadOnlyList<NativeArtifactAssociation>>.Success(second);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (ObjectDisposedException exception)
        {
            return EngineResult<IReadOnlyList<NativeArtifactAssociation>>.Failure(new EngineError(
                EngineErrorCode.SourceOpenFailed,
                $"The borrowed native source lifetime is unavailable: {exception.Message}"));
        }
        catch (NativeSourceInputException exception)
        {
            var code = exception.Code is EngineErrorCode.InvalidRequest
                or EngineErrorCode.UnsupportedInput
                or EngineErrorCode.SourceOpenFailed
                or EngineErrorCode.OutputOpenFailed
                    ? EngineErrorCode.ExternalChangeDetected
                    : exception.Code;
            return EngineResult<IReadOnlyList<NativeArtifactAssociation>>.Failure(new EngineError(
                code,
                exception.Message));
        }
        catch (Exception exception)
        {
            return EngineResult<IReadOnlyList<NativeArtifactAssociation>>.Failure(new EngineError(
                EngineErrorCode.OutputOpenFailed,
                $"The explicit native output baseline could not be verified: {exception.Message}"));
        }
    }

    /// <summary>Creates the stable external-change result for a mismatched output artifact set.</summary>
    /// <returns>A typed failed output-artifact result.</returns>
    private static EngineResult<IReadOnlyList<NativeArtifactAssociation>> ChangedOutputFailure()
    {
        return EngineResult<IReadOnlyList<NativeArtifactAssociation>>.Failure(new EngineError(
            EngineErrorCode.ExternalChangeDetected,
            "One or more native output plugin or loose strings artifacts changed after admission."));
    }

    /// <summary>Throws when this short-lived output admission and parsing scope has been released.</summary>
    /// <exception cref="ObjectDisposedException">Thrown after <see cref="DisposeAsync"/>.</exception>
    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref IsDisposed) != 0, this);
    }
}
