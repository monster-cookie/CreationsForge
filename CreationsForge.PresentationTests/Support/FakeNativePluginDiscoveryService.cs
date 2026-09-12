using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;

namespace CreationsForge.PresentationTests.Support;

/// <summary>Returns a configured detached plugin catalog while recording requested games.</summary>
internal sealed class FakeNativePluginDiscoveryService : INativePluginDiscoveryService
{
    /// <summary>Gets or sets the deterministic discovery result.</summary>
    public EngineResult<NativePluginCatalog> Result { get; set; } = EngineResult<NativePluginCatalog>.Failure(
        new EngineError(EngineErrorCode.SourceOpenFailed, "No fake plugin catalog was configured."));

    /// <summary>Gets requested games in call order.</summary>
    public List<SupportedGame> RequestedGames { get; } = [];

    /// <inheritdoc />
    public ValueTask<EngineResult<NativePluginCatalog>> DiscoverAsync(
        SupportedGame game,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        RequestedGames.Add(game);
        return ValueTask.FromResult(Result);
    }
}
