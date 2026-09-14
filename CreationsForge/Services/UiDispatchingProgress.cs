using CreationsForge.Services.Interfaces;

namespace CreationsForge.Services;

/// <summary>
/// Projects background progress callbacks onto the presentation dispatcher.
/// </summary>
/// <typeparam name="T">The progress value type.</typeparam>
internal sealed class UiDispatchingProgress<T> : IProgress<T>
{
    /// <summary>The dispatcher that owns presentation state.</summary>
    private readonly IUiDispatcher UiDispatcher;

    /// <summary>The state update that consumes each progress value.</summary>
    private readonly Action<T> Handler;

    /// <summary>Initializes a dispatcher-backed progress observer.</summary>
    /// <param name="uiDispatcher">The dispatcher that owns presentation state.</param>
    /// <param name="handler">The state update that consumes each progress value.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required dependency is <see langword="null"/>.</exception>
    public UiDispatchingProgress(IUiDispatcher uiDispatcher, Action<T> handler)
    {
        ArgumentNullException.ThrowIfNull(uiDispatcher);
        ArgumentNullException.ThrowIfNull(handler);
        UiDispatcher = uiDispatcher;
        Handler = handler;
    }

    /// <inheritdoc />
    public void Report(T value)
    {
        UiDispatcher.Post(() => Handler(value));
    }
}
