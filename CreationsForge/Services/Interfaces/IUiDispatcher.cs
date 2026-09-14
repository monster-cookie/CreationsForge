namespace CreationsForge.Services.Interfaces;

/// <summary>
/// Schedules presentation state changes on the Avalonia UI thread.
/// </summary>
public interface IUiDispatcher
{
    /// <summary>Queues a state change without blocking the caller.</summary>
    /// <param name="action">The presentation state change to run.</param>
    void Post(Action action);

    /// <summary>Runs a state change on the UI thread and completes after it has run.</summary>
    /// <param name="action">The presentation state change to run.</param>
    /// <returns>A task that completes after the state change has run.</returns>
    Task InvokeAsync(Action action);
}
