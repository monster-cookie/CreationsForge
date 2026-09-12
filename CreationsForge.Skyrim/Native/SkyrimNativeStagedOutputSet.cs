using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Skyrim.Native;

/// <summary>
/// Owns the strictly reopened Skyrim output lifetime that proves one private staged artifact set is natively readable.
/// </summary>
internal sealed class SkyrimNativeStagedOutputSet : INativeStagedOutputSet
{
    /// <summary>The independently owned reopened staged output, or <see langword="null"/> after disposal.</summary>
    private SkyrimNativeOutputState? _output;

    /// <summary>Initializes ownership of one validated private staged Skyrim output.</summary>
    /// <param name="output">The independently owned reopened staged output.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="output"/> is <see langword="null"/>.</exception>
    internal SkyrimNativeStagedOutputSet(SkyrimNativeOutputState output)
    {
        ArgumentNullException.ThrowIfNull(output);
        _output = output;
    }

    /// <summary>Releases the reopened staged native state exactly once.</summary>
    /// <returns>A task that completes after the staged native state is released.</returns>
    public async ValueTask DisposeAsync()
    {
        var output = Interlocked.Exchange(ref _output, null);
        if (output is not null)
        {
            await output.DisposeAsync().ConfigureAwait(false);
        }
    }
}
