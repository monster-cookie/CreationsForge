using CreationsForge.Core.Engine.NativeInputs;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Represents an opaque, independently owned native source/load-order/string lifetime for one workspace.
/// </summary>
public interface INativeSourceSet : IAsyncDisposable
{
    /// <summary>Gets the completed immutable baseline for every physical source artifact.</summary>
    NativeSourceInputBaseline Baseline { get; }

    /// <summary>Verifies that every source artifact still matches the completed baseline.</summary>
    /// <param name="cancellationToken">A token that cancels recapture and hashing.</param>
    /// <returns>The unchanged baseline, or a typed external-change or source verification failure.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    Task<EngineResult<NativeSourceInputBaseline>> VerifyUnchangedAsync(
        CancellationToken cancellationToken = default);
}
