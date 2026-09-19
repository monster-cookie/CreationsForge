using Autofac;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordWire;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda;
using Serilog;

namespace CreationsForge.Bootstrap.Composition;

/// <summary>Builds an independently owned engine service lifetime for a headless host.</summary>
public static class EngineComposition
{
    /// <summary>Creates the real three-game engine and save coordinator without opening plugins or initializing legacy persistence.</summary>
    /// <param name="logger">The caller-owned structured logger, or <see langword="null"/> to use Serilog's current global logger.</param>
    /// <returns>The owned service lifetime; dispose all workspaces opened through it before disposing these services.</returns>
    /// <exception cref="Autofac.Core.DependencyResolutionException">Thrown when the complete engine service graph cannot be resolved.</exception>
    public static EngineServices Create(ILogger? logger = null)
    {
        var builder = new ContainerBuilder();
        builder.RegisterInstance(logger ?? Log.Logger).As<ILogger>().ExternallyOwned();
        builder.RegisterModule<FormListEngineModule>();
        var container = builder.Build();
        try
        {
            var codecs = container.Resolve<IEnumerable<IFormListEditWireCodec>>().ToArray();
            var schemaCatalogs = container.Resolve<IEnumerable<IFormListEditWireSchemaCatalog>>().ToArray();
            ValidateRecordWireServices(codecs, schemaCatalogs);
            return new EngineServices(
                container,
                container.Resolve<IPluginWorkspaceFactory>(),
                container.Resolve<IWorkspaceSaveCoordinator>(),
                Array.AsReadOnly(codecs),
                Array.AsReadOnly(schemaCatalogs));
        }
        catch
        {
            container.Dispose();
            throw;
        }
    }

    /// <summary>Requires exactly one paired codec and schema catalog for every supported game and release.</summary>
    /// <param name="codecs">The resolved typed edit codecs.</param>
    /// <param name="schemaCatalogs">The resolved immutable schema catalogs.</param>
    /// <exception cref="InvalidOperationException">Thrown when registrations are missing, duplicated, or mismatched.</exception>
    private static void ValidateRecordWireServices(
        IReadOnlyList<IFormListEditWireCodec> codecs,
        IReadOnlyList<IFormListEditWireSchemaCatalog> schemaCatalogs)
    {
        var codecPairs = codecs.Select(codec => (codec.Game, codec.Release)).ToArray();
        var schemaPairs = schemaCatalogs.Select(catalog => (catalog.Identity.Game, catalog.Identity.Release)).ToArray();
        var expectedPairs = new[]
        {
            (SupportedGame.Starfield, GameRelease.Starfield),
            (SupportedGame.Fallout4, GameRelease.Fallout4),
            (SupportedGame.Skyrim, GameRelease.SkyrimSE),
        };
        if (codecPairs.Length != 3 || schemaPairs.Length != 3 ||
            codecPairs.Distinct().Count() != 3 || schemaPairs.Distinct().Count() != 3 ||
            !codecPairs.OrderBy(pair => pair.Game).ThenBy(pair => pair.Release).SequenceEqual(schemaPairs.OrderBy(pair => pair.Game).ThenBy(pair => pair.Release)) ||
            !codecPairs.OrderBy(pair => pair.Game).ThenBy(pair => pair.Release).SequenceEqual(expectedPairs.OrderBy(pair => pair.Item1).ThenBy(pair => pair.Item2)))
        {
            throw new InvalidOperationException("Plugin authoring requires one paired edit codec and schema catalog for each supported game and release.");
        }
    }
}
