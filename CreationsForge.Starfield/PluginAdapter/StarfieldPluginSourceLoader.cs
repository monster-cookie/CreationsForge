using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.PluginInputs;
using CreationsForge.Core.Engine.RecordReading;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Starfield;

namespace CreationsForge.Starfield.PluginAdapter;

/// <summary>
/// Opens explicitly selected Starfield source plugins as independently owned records.
/// </summary>
public sealed class StarfieldPluginSourceLoader
{
    /// <summary>The shared boundary that validates and fingerprints explicit plugin inputs.</summary>
    private readonly PluginSourceInputLoader InputLoader;

    /// <summary>
    /// Initializes a Starfield plugin source loader.
    /// </summary>
    /// <param name="inputLoader">The shared plugin input boundary used for all paths and parsing metadata.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="inputLoader"/> is <see langword="null"/>.</exception>
    public StarfieldPluginSourceLoader(PluginSourceInputLoader inputLoader)
    {
        ArgumentNullException.ThrowIfNull(inputLoader);
        InputLoader = inputLoader;
    }

    /// <summary>
    /// Opens every admitted Starfield plugin in explicit load-order order and records the complete source baseline.
    /// </summary>
    /// <param name="request">The explicit source, load order, data directory, and localized-string inputs.</param>
    /// <param name="cancellationToken">A token that cancels validation or Mutagen parsing before the open completes.</param>
    /// <returns>A successful owned plugin source lifetime and baseline, or a typed open failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public async Task<EngineResult<PluginSourceOpenResult>> OpenAsync(
        WorkspaceOpenRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        if (request.Game != SupportedGame.Starfield || request.Release != GameRelease.Starfield)
        {
            return EngineResult<PluginSourceOpenResult>.Failure(
                new EngineError(
                    EngineErrorCode.UnsupportedGameRelease,
                    $"Starfield plugin source loading does not support {request.Game} with release {request.Release}."),
                request.WorkspaceId);
        }

        var inputResult = await InputLoader.PrepareAsync(request, cancellationToken).ConfigureAwait(false);
        if (!inputResult.Succeeded)
        {
            return EngineResult<PluginSourceOpenResult>.Failure(
                inputResult.Error!,
                request.WorkspaceId,
                warnings: inputResult.Warnings);
        }

        var inputs = inputResult.Value!;
        var sourceMods = new List<IStarfieldModGetter>(inputs.Plugins.Count);
        var referenceMods = new List<IStarfieldModGetter>(inputs.Plugins.Count);
        var ownedOverlayResources = new List<IDisposable>();

        try
        {
            request.Progress?.Report(new WorkspaceOpenProgress(
                WorkspaceOpenStage.OpeningSources,
                $"Prepared {inputs.Plugins.Count} Starfield plugin inputs."));
            for (var pluginIndex = 0; pluginIndex < inputs.Plugins.Count; pluginIndex++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var plugin = inputs.Plugins[pluginIndex];
                request.Progress?.Report(new WorkspaceOpenProgress(
                    WorkspaceOpenStage.ParsingPlugin,
                    $"Parsing Starfield plugin {pluginIndex + 1} of {inputs.Plugins.Count}: '{plugin.Path}'."));

                using (var formListStream = inputs.OpenReadStream(plugin, cancellationToken))
                {
                    sourceMods.Add(StarfieldMod.CreateFromBinary(
                        new MutagenFrame(formListStream),
                        StarfieldRelease.Starfield,
                        new GroupMask(false) { FormLists = true }));
                }

                IStarfieldModGetter referenceMod;
                if (inputs.SupportsBinaryOverlay(plugin))
                {
                    var overlayStream = inputs.OpenOverlayStream(plugin);
                    try
                    {
                        referenceMod = StarfieldMod.CreateFromBinaryOverlay(
                            overlayStream,
                            StarfieldRelease.Starfield,
                            plugin.ModKey,
                            inputs.CreateOverlayReadParameters(plugin));
                        ownedOverlayResources.Add((IDisposable)referenceMod);
                        ownedOverlayResources.Add(overlayStream);
                    }
                    catch
                    {
                        overlayStream.Dispose();
                        throw;
                    }
                }
                else
                {
                    using var stream = inputs.OpenReadStream(plugin, cancellationToken);
                    var frame = new MutagenFrame(stream);
                    referenceMod = StarfieldMod.CreateFromBinary(
                        frame,
                        StarfieldRelease.Starfield,
                        new GroupMask(true));
                }

                referenceMods.Add(referenceMod);

                if (sourceMods[^1].ModKey != plugin.ModKey || referenceMod.ModKey != plugin.ModKey)
                {
                    try
                    {
                        DisposeResources(ownedOverlayResources);
                    }
                    finally
                    {
                        await inputs.DisposeAsync().ConfigureAwait(false);
                    }

                    return EngineResult<PluginSourceOpenResult>.Failure(
                        new EngineError(
                            EngineErrorCode.SourceOpenFailed,
                            $"Plugin identity {referenceMod.ModKey} did not match the admitted input {plugin.ModKey} at '{plugin.Path}'."),
                        request.WorkspaceId);
                }

                request.Progress?.Report(new WorkspaceOpenProgress(
                    WorkspaceOpenStage.ParsingPlugin,
                    $"Parsed Starfield plugin {pluginIndex + 1} of {inputs.Plugins.Count}: '{plugin.Path}'."));
            }

            request.Progress?.Report(new WorkspaceOpenProgress(
                WorkspaceOpenStage.FinalizingSources,
                $"Verifying the immutable baseline for {inputs.Plugins.Count} Starfield plugins."));
            var baselineResult = await inputs.CompleteOpenAsync(cancellationToken).ConfigureAwait(false);
            if (!baselineResult.Succeeded)
            {
                try
                {
                    DisposeResources(ownedOverlayResources);
                }
                finally
                {
                    await inputs.DisposeAsync().ConfigureAwait(false);
                }

                return EngineResult<PluginSourceOpenResult>.Failure(
                    baselineResult.Error!,
                    request.WorkspaceId,
                    warnings: baselineResult.Warnings);
            }

            var baseline = baselineResult.Value!;
            var sources = new StarfieldPluginSourceSet(
                request.WorkspaceId,
                inputs,
                sourceMods,
                referenceMods,
                ownedOverlayResources,
                baseline);
            var revision = sources.Revision;
            var openResult = new PluginSourceOpenResult(sources, baseline.BaselineId, baseline.Artifacts);
            return EngineResult<PluginSourceOpenResult>.Success(
                openResult,
                request.WorkspaceId,
                resultRevision: revision,
                warnings: inputResult.Warnings.Concat(baselineResult.Warnings).ToArray());
        }
        catch (OperationCanceledException)
        {
            try
            {
                DisposeResources(ownedOverlayResources);
            }
            finally
            {
                await inputs.DisposeAsync().ConfigureAwait(false);
            }

            throw;
        }
        catch (Exception exception)
        {
            try
            {
                DisposeResources(ownedOverlayResources);
            }
            finally
            {
                await inputs.DisposeAsync().ConfigureAwait(false);
            }

            return EngineResult<PluginSourceOpenResult>.Failure(
                new EngineError(
                    EngineErrorCode.SourceOpenFailed,
                    $"Starfield plugin source parsing failed: {exception.Message}"),
                request.WorkspaceId);
        }
    }

    /// <summary>Releases every overlay and backing stream opened before ownership transfers to a plugin source set.</summary>
    /// <param name="resources">The partially opened overlay resources in creation order.</param>
    private static void DisposeResources(IEnumerable<IDisposable> resources)
    {
        foreach (var resource in resources)
        {
            resource.Dispose();
        }
    }
}
