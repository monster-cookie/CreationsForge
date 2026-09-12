using CreationsForge.Core.Enums;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Discovers installed Bethesda plugins without importing or persisting repository state.</summary>
public interface INativePluginDiscoveryService
{
    /// <summary>Discovers one game's installed load order and reads bounded native header metadata.</summary>
    /// <param name="game">The game whose installed plugins are requested.</param>
    /// <param name="cancellationToken">A token observed between filesystem and header reads.</param>
    /// <returns>The detected catalog or a stable typed failure.</returns>
    ValueTask<EngineResult<NativePluginCatalog>> DiscoverAsync(
        SupportedGame game,
        CancellationToken cancellationToken = default);
}
