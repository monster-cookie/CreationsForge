using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.PluginInputs;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins.Binary.Streams;

namespace CreationsForge.Fallout4.PluginAdapter;

/// <summary>
/// Opens an explicitly supplied Fallout 4 load order as an independently owned plugin source lifetime.
/// </summary>
public sealed class Fallout4PluginSourceLoader
{
    /// <summary>The shared boundary that validates, fingerprints, and prepares explicit plugin inputs.</summary>
    private readonly PluginSourceInputLoader _inputLoader;

    /// <summary>
    /// Initializes a Fallout 4 plugin source loader.
    /// </summary>
    /// <param name="inputLoader">The shared service that validates and baselines explicit plugin inputs.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="inputLoader"/> is <see langword="null"/>.</exception>
    public Fallout4PluginSourceLoader(PluginSourceInputLoader inputLoader)
    {
        ArgumentNullException.ThrowIfNull(inputLoader);
        _inputLoader = inputLoader;
    }

    /// <summary>
    /// Opens every requested Fallout 4 plugin without consulting an installed game or ambient load order.
    /// </summary>
    /// <param name="request">The explicit workspace source, load order, and localized-resource paths.</param>
    /// <param name="cancellationToken">A token that cancels validation or Mutagen parsing.</param>
    /// <returns>A typed source lifetime and the complete immutable input baseline, or a stable engine failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public async Task<EngineResult<PluginSourceOpenResult>> OpenAsync(
        WorkspaceOpenRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        if (request.Game != SupportedGame.Fallout4 || request.Release != GameRelease.Fallout4)
        {
            return EngineResult<PluginSourceOpenResult>.Failure(
                new EngineError(
                    EngineErrorCode.UnsupportedGameRelease,
                    $"Fallout 4 plugin sources require {SupportedGame.Fallout4}/{GameRelease.Fallout4}; received {request.Game}/{request.Release}."),
                workspaceId: request.WorkspaceId);
        }

        var preparation = await _inputLoader.PrepareAsync(request, cancellationToken).ConfigureAwait(false);
        if (!preparation.Succeeded)
        {
            return EngineResult<PluginSourceOpenResult>.Failure(
                preparation.Error!,
                workspaceId: request.WorkspaceId,
                warnings: preparation.Warnings);
        }

        var inputs = preparation.Value!;
        var mutagenMods = new List<IFallout4ModGetter>(inputs.Plugins.Count);

        try
        {
            request.Progress?.Report(new WorkspaceOpenProgress(
                WorkspaceOpenStage.OpeningSources,
                $"Prepared {inputs.Plugins.Count} Fallout 4 plugin inputs."));
            for (var pluginIndex = 0; pluginIndex < inputs.Plugins.Count; pluginIndex++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var plugin = inputs.Plugins[pluginIndex];
                request.Progress?.Report(new WorkspaceOpenProgress(
                    WorkspaceOpenStage.ParsingPlugin,
                    $"Parsing Fallout 4 plugin {pluginIndex + 1} of {inputs.Plugins.Count}: '{plugin.Path}'."));
                using MutagenBinaryReadStream stream = inputs.OpenReadStream(plugin, cancellationToken);
                var mutagenMod = Fallout4Mod.CreateFromBinary(
                    new MutagenFrame(stream),
                    Fallout4Release.Fallout4,
                    new GroupMask(true));

                if (mutagenMod.ModKey != plugin.ModKey)
                {
                    await inputs.DisposeAsync().ConfigureAwait(false);
                    return EngineResult<PluginSourceOpenResult>.Failure(
                        new EngineError(
                            EngineErrorCode.SourceOpenFailed,
                            $"Plugin identity {mutagenMod.ModKey} did not match the admitted input {plugin.ModKey} at '{plugin.Path}'."),
                        workspaceId: request.WorkspaceId);
                }

                mutagenMods.Add(mutagenMod);
                request.Progress?.Report(new WorkspaceOpenProgress(
                    WorkspaceOpenStage.ParsingPlugin,
                    $"Parsed Fallout 4 plugin {pluginIndex + 1} of {inputs.Plugins.Count}: '{plugin.Path}'."));
            }

            request.Progress?.Report(new WorkspaceOpenProgress(
                WorkspaceOpenStage.FinalizingSources,
                $"Verifying the immutable baseline for {inputs.Plugins.Count} Fallout 4 plugins."));
            var baselineResult = await inputs.CompleteOpenAsync(cancellationToken).ConfigureAwait(false);
            if (!baselineResult.Succeeded)
            {
                await inputs.DisposeAsync().ConfigureAwait(false);
                return EngineResult<PluginSourceOpenResult>.Failure(
                    baselineResult.Error!,
                    workspaceId: request.WorkspaceId,
                    warnings: preparation.Warnings.Concat(baselineResult.Warnings).ToArray());
            }

            var baseline = baselineResult.Value!;
            var sourceSet = new Fallout4PluginSourceSet(
                request.WorkspaceId,
                inputs,
                mutagenMods,
                baseline);

            return EngineResult<PluginSourceOpenResult>.Success(
                new PluginSourceOpenResult(sourceSet, baseline.BaselineId, baseline.Artifacts),
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
            return EngineResult<PluginSourceOpenResult>.Failure(
                new EngineError(
                    EngineErrorCode.SourceOpenFailed,
                    $"Fallout 4 plugin source parsing failed: {exception.Message}"),
                workspaceId: request.WorkspaceId);
        }
    }
}
