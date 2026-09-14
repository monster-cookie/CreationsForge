using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Skyrim.PluginAdapter;

/// <summary>
/// Owns the strictly reopened Skyrim output lifetime that proves one private staged artifact set is readable through Mutagen.
/// </summary>
internal sealed class SkyrimStagedPluginOutputSet : IStagedPluginOutputSet
{
    /// <summary>The independently owned reopened staged output, or <see langword="null"/> after disposal.</summary>
    private SkyrimPluginOutputState? _output;

    /// <summary>Initializes ownership of one validated private staged Skyrim output.</summary>
    /// <param name="output">The independently owned reopened staged output.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="output"/> is <see langword="null"/>.</exception>
    internal SkyrimStagedPluginOutputSet(SkyrimPluginOutputState output)
    {
        ArgumentNullException.ThrowIfNull(output);
        _output = output;
    }

    /// <summary>Releases the reopened staged Mutagen state exactly once.</summary>
    /// <returns>A task that completes after the staged Mutagen state is released.</returns>
    public async ValueTask DisposeAsync()
    {
        var output = Interlocked.Exchange(ref _output, null);
        if (output is not null)
        {
            await output.DisposeAsync().ConfigureAwait(false);
        }
    }
}
