using CreationsForge.Core.Engine.PluginInputs;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Represents an opaque, independently owned plugin source/load-order/string lifetime for one workspace.
/// </summary>
public interface IPluginSourceSet : IAsyncDisposable
{
    /// <summary>Gets the completed immutable baseline for every physical source artifact.</summary>
    PluginSourceInputBaseline Baseline { get; }

    /// <summary>Confirms ownership of the completed source lifetime and its retained read locks.</summary>
    /// <param name="cancellationToken">A token that cancels entry to the source-lifetime gate.</param>
    /// <returns>The completed baseline, or a typed source-lifetime failure.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    Task<EngineResult<PluginSourceInputBaseline>> VerifyUnchangedAsync(
        CancellationToken cancellationToken = default);
}
