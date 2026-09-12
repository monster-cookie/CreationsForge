using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInputs;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins.Binary.Streams;

namespace CreationsForge.Fallout4.Native;

/// <summary>
/// Opens an explicitly supplied Fallout 4 load order as an independently owned native source lifetime.
/// </summary>
public sealed class Fallout4NativeSourceLoader
{
    /// <summary>The shared boundary that validates, fingerprints, and prepares explicit native inputs.</summary>
    private readonly NativeSourceInputLoader _inputLoader;

    /// <summary>
    /// Initializes a Fallout 4 native source loader.
    /// </summary>
    /// <param name="inputLoader">The shared service that validates and baselines explicit native inputs.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="inputLoader"/> is <see langword="null"/>.</exception>
    public Fallout4NativeSourceLoader(NativeSourceInputLoader inputLoader)
    {
        ArgumentNullException.ThrowIfNull(inputLoader);
        _inputLoader = inputLoader;
    }

    /// <summary>
    /// Opens every requested Fallout 4 plugin without consulting an installed game or ambient load order.
    /// </summary>
    /// <param name="request">The explicit workspace source, load order, and localized-resource paths.</param>
    /// <param name="cancellationToken">A token that cancels validation or native parsing.</param>
    /// <returns>A typed source lifetime and the complete immutable input baseline, or a stable engine failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public async Task<EngineResult<NativeSourceOpenResult>> OpenAsync(
        WorkspaceOpenRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        if (request.Game != SupportedGame.Fallout4 || request.Release != GameRelease.Fallout4)
        {
            return EngineResult<NativeSourceOpenResult>.Failure(
                new EngineError(
                    EngineErrorCode.UnsupportedGameRelease,
                    $"Fallout 4 native sources require {SupportedGame.Fallout4}/{GameRelease.Fallout4}; received {request.Game}/{request.Release}."),
                workspaceId: request.WorkspaceId);
        }

        var preparation = await _inputLoader.PrepareAsync(request, cancellationToken).ConfigureAwait(false);
        if (!preparation.Succeeded)
        {
            return EngineResult<NativeSourceOpenResult>.Failure(
                preparation.Error!,
                workspaceId: request.WorkspaceId,
                warnings: preparation.Warnings);
        }

        var inputs = preparation.Value!;
        var nativeMods = new List<IFallout4ModGetter>(inputs.Plugins.Count);

        try
        {
            foreach (var plugin in inputs.Plugins)
            {
                cancellationToken.ThrowIfCancellationRequested();
                using MutagenBinaryReadStream stream = inputs.OpenReadStream(plugin, cancellationToken);
                var nativeMod = Fallout4Mod.CreateFromBinary(
                    new MutagenFrame(stream),
                    Fallout4Release.Fallout4,
                    new GroupMask(true));

                if (nativeMod.ModKey != plugin.ModKey)
                {
                    await inputs.DisposeAsync().ConfigureAwait(false);
                    return EngineResult<NativeSourceOpenResult>.Failure(
                        new EngineError(
                            EngineErrorCode.SourceOpenFailed,
                            $"Native plugin identity {nativeMod.ModKey} did not match the admitted input {plugin.ModKey} at '{plugin.Path}'."),
                        workspaceId: request.WorkspaceId);
                }

                nativeMods.Add(nativeMod);
            }

            var baselineResult = await inputs.CompleteOpenAsync(cancellationToken).ConfigureAwait(false);
            if (!baselineResult.Succeeded)
            {
                await inputs.DisposeAsync().ConfigureAwait(false);
                return EngineResult<NativeSourceOpenResult>.Failure(
                    baselineResult.Error!,
                    workspaceId: request.WorkspaceId,
                    warnings: preparation.Warnings.Concat(baselineResult.Warnings).ToArray());
            }

            var baseline = baselineResult.Value!;
            var sourceSet = new Fallout4NativeSourceSet(
                request.WorkspaceId,
                inputs,
                nativeMods,
                baseline);

            return EngineResult<NativeSourceOpenResult>.Success(
                new NativeSourceOpenResult(sourceSet, baseline.BaselineId, baseline.Artifacts),
                workspaceId: request.WorkspaceId,
                resultRevision: sourceSet.Revision,
                warnings: preparation.Warnings.Concat(baselineResult.Warnings).ToArray());
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
                    $"Fallout 4 native source parsing failed: {exception.Message}"),
                workspaceId: request.WorkspaceId);
        }
    }
}
