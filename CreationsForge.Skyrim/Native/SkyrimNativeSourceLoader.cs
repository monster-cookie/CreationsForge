using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInputs;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Skyrim;

namespace CreationsForge.Skyrim.Native;

/// <summary>
/// Opens explicitly selected Skyrim Special Edition plugins as an independently owned native source lifetime.
/// </summary>
public sealed class SkyrimNativeSourceLoader
{
    /// <summary>The shared boundary that validates, opens, and baselines explicit native inputs.</summary>
    private readonly NativeSourceInputLoader _inputLoader;

    /// <summary>
    /// Initializes a Skyrim Special Edition native source loader.
    /// </summary>
    /// <param name="inputLoader">The shared service that validates and baselines explicit native inputs.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="inputLoader"/> is <see langword="null"/>.</exception>
    public SkyrimNativeSourceLoader(NativeSourceInputLoader inputLoader)
    {
        ArgumentNullException.ThrowIfNull(inputLoader);
        _inputLoader = inputLoader;
    }

    /// <summary>
    /// Opens every requested Skyrim Special Edition plugin without consulting an installed game or ambient load order.
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

        if (request.Game != SupportedGame.Skyrim || request.Release != GameRelease.SkyrimSE)
        {
            return EngineResult<NativeSourceOpenResult>.Failure(
                new EngineError(
                    EngineErrorCode.UnsupportedGameRelease,
                    $"Skyrim Special Edition native sources require {SupportedGame.Skyrim}/{GameRelease.SkyrimSE}; received {request.Game}/{request.Release}."),
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

        try
        {
            request.Progress?.Report(new WorkspaceOpenProgress(
                WorkspaceOpenStage.OpeningSources,
                $"Prepared {inputs.Plugins.Count} Skyrim Special Edition native plugin inputs."));
            var nativeMods = new List<ISkyrimModGetter>(inputs.Plugins.Count);
            for (var pluginIndex = 0; pluginIndex < inputs.Plugins.Count; pluginIndex++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var plugin = inputs.Plugins[pluginIndex];
                request.Progress?.Report(new WorkspaceOpenProgress(
                    WorkspaceOpenStage.ParsingPlugin,
                    $"Parsing Skyrim Special Edition plugin {pluginIndex + 1} of {inputs.Plugins.Count}: '{plugin.Path}'."));
                using MutagenBinaryReadStream stream = inputs.OpenReadStream(plugin, cancellationToken);
                var nativeMod = SkyrimMod.CreateFromBinary(
                    new MutagenFrame(stream),
                    SkyrimRelease.SkyrimSE,
                    new GroupMask(true));

                if (nativeMod.ModKey != plugin.ModKey)
                {
                    return await FailAndDisposeAsync(
                        inputs,
                        request.WorkspaceId,
                        $"Native plugin identity {nativeMod.ModKey} did not match the admitted input {plugin.ModKey} at '{plugin.Path}'.")
                        .ConfigureAwait(false);
                }

                nativeMods.Add(nativeMod);
                request.Progress?.Report(new WorkspaceOpenProgress(
                    WorkspaceOpenStage.ParsingPlugin,
                    $"Parsed Skyrim Special Edition plugin {pluginIndex + 1} of {inputs.Plugins.Count}: '{plugin.Path}'."));
            }

            request.Progress?.Report(new WorkspaceOpenProgress(
                WorkspaceOpenStage.FinalizingSources,
                $"Verifying the immutable baseline for {inputs.Plugins.Count} Skyrim Special Edition plugins."));
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
            var sourceSet = new SkyrimNativeSourceSet(
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
                    $"Skyrim Special Edition native source parsing failed: {exception.Message}"),
                workspaceId: request.WorkspaceId);
        }
    }

    /// <summary>
    /// Releases a rejected shared input lifetime and returns its stable identity-mismatch failure.
    /// </summary>
    /// <param name="inputs">The prepared input lifetime to release.</param>
    /// <param name="workspaceId">The workspace whose source acquisition failed.</param>
    /// <param name="message">The diagnostic identity-mismatch message.</param>
    /// <returns>A failed native source-open result after the prepared lifetime has been released.</returns>
    private static async Task<EngineResult<NativeSourceOpenResult>> FailAndDisposeAsync(
        NativeSourceInputs inputs,
        Guid workspaceId,
        string message)
    {
        await inputs.DisposeAsync().ConfigureAwait(false);
        return EngineResult<NativeSourceOpenResult>.Failure(
            new EngineError(EngineErrorCode.SourceOpenFailed, message),
            workspaceId: workspaceId);
    }
}
