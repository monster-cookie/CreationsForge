using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInputs;
using CreationsForge.Core.Engine.NativeReading;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Starfield;

namespace CreationsForge.Starfield.Native;

/// <summary>
/// Opens explicitly selected Starfield source plugins as independently owned native records.
/// </summary>
public sealed class StarfieldNativeSourceLoader
{
    /// <summary>The shared boundary that validates and fingerprints explicit native inputs.</summary>
    private readonly NativeSourceInputLoader InputLoader;

    /// <summary>
    /// Initializes a Starfield native source loader.
    /// </summary>
    /// <param name="inputLoader">The shared native input boundary used for all paths and parsing metadata.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="inputLoader"/> is <see langword="null"/>.</exception>
    public StarfieldNativeSourceLoader(NativeSourceInputLoader inputLoader)
    {
        ArgumentNullException.ThrowIfNull(inputLoader);
        InputLoader = inputLoader;
    }

    /// <summary>
    /// Opens every admitted Starfield plugin in explicit load-order order and records the complete source baseline.
    /// </summary>
    /// <param name="request">The explicit source, load order, data directory, and localized-string inputs.</param>
    /// <param name="cancellationToken">A token that cancels validation or native parsing before the open completes.</param>
    /// <returns>A successful owned native source lifetime and baseline, or a typed open failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public async Task<EngineResult<NativeSourceOpenResult>> OpenAsync(
        WorkspaceOpenRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        if (request.Game != SupportedGame.Starfield || request.Release != GameRelease.Starfield)
        {
            return EngineResult<NativeSourceOpenResult>.Failure(
                new EngineError(
                    EngineErrorCode.UnsupportedGameRelease,
                    $"Starfield native source loading does not support {request.Game} with release {request.Release}."),
                request.WorkspaceId);
        }

        var inputResult = await InputLoader.PrepareAsync(request, cancellationToken).ConfigureAwait(false);
        if (!inputResult.Succeeded)
        {
            return EngineResult<NativeSourceOpenResult>.Failure(
                inputResult.Error!,
                request.WorkspaceId,
                warnings: inputResult.Warnings);
        }

        var inputs = inputResult.Value!;

        try
        {
            var sourceMods = new List<IStarfieldModGetter>(inputs.Plugins.Count);
            foreach (var plugin in inputs.Plugins)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using var stream = inputs.OpenReadStream(plugin, cancellationToken);
                var frame = new MutagenFrame(stream);
                var mod = StarfieldMod.CreateFromBinary(
                    frame,
                    StarfieldRelease.Starfield,
                    new GroupMask(true));

                if (mod.ModKey != plugin.ModKey)
                {
                    await inputs.DisposeAsync().ConfigureAwait(false);
                    return EngineResult<NativeSourceOpenResult>.Failure(
                        new EngineError(
                            EngineErrorCode.SourceOpenFailed,
                            $"Native plugin identity {mod.ModKey} did not match the admitted input {plugin.ModKey} at '{plugin.Path}'."),
                        request.WorkspaceId);
                }

                sourceMods.Add(mod);
            }

            var baselineResult = await inputs.CompleteOpenAsync(cancellationToken).ConfigureAwait(false);
            if (!baselineResult.Succeeded)
            {
                await inputs.DisposeAsync().ConfigureAwait(false);
                return EngineResult<NativeSourceOpenResult>.Failure(
                    baselineResult.Error!,
                    request.WorkspaceId,
                    warnings: baselineResult.Warnings);
            }

            var baseline = baselineResult.Value!;
            var sources = new StarfieldNativeSourceSet(request.WorkspaceId, inputs, sourceMods, baseline);
            var revision = sources.Revision;
            var openResult = new NativeSourceOpenResult(sources, baseline.BaselineId, baseline.Artifacts);
            return EngineResult<NativeSourceOpenResult>.Success(
                openResult,
                request.WorkspaceId,
                resultRevision: revision,
                warnings: inputResult.Warnings.Concat(baselineResult.Warnings).ToArray());
        }
        catch (OperationCanceledException)
        {
            await inputs.DisposeAsync().ConfigureAwait(false);
            throw;
        }
        catch (Exception exception)
        {
            await inputs.DisposeAsync().ConfigureAwait(false);
            return EngineResult<NativeSourceOpenResult>.Failure(
                new EngineError(
                    EngineErrorCode.SourceOpenFailed,
                    $"Starfield native source parsing failed: {exception.Message}"),
                request.WorkspaceId);
        }
    }
}
