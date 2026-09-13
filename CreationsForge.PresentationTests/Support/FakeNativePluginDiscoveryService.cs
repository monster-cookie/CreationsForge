using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;

namespace CreationsForge.PresentationTests.Support;

/// <summary>Returns a configured detached plugin catalog while recording requested games.</summary>
internal sealed class FakeNativePluginDiscoveryService : INativePluginDiscoveryService
{
    /// <summary>Gets or sets the deterministic discovery result.</summary>
    public EngineResult<NativePluginCatalog> Result { get; set; } = EngineResult<NativePluginCatalog>.Failure(
        new EngineError(EngineErrorCode.SourceOpenFailed, "No fake plugin catalog was configured."));

    /// <summary>Gets or sets an optional asynchronous discovery handler used for cancellation and ordering tests.</summary>
    public Func<SupportedGame, CancellationToken, ValueTask<EngineResult<NativePluginCatalog>>>? Handler { get; set; }

    /// <summary>Gets requested games in call order.</summary>
    public List<SupportedGame> RequestedGames { get; } = [];

    /// <inheritdoc />
    public ValueTask<EngineResult<NativePluginCatalog>> DiscoverAsync(
        SupportedGame game,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        RequestedGames.Add(game);
        return Handler is null ? ValueTask.FromResult(Result) : Handler(game, cancellationToken);
    }
}
