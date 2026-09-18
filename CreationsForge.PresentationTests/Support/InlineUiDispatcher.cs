using CreationsForge.Services.Interfaces;

namespace CreationsForge.PresentationTests.Support;

/// <summary>
/// Executes presentation dispatches synchronously while recording how they were requested.
/// </summary>
internal sealed class InlineUiDispatcher : IUiDispatcher
{
    /// <summary>Gets the number of queued state changes.</summary>
    public int PostCount { get; private set; }

    /// <summary>Gets the number of awaited state changes.</summary>
    public int InvokeCount { get; private set; }

    /// <summary>Gets whether an awaited state change is currently executing through this dispatcher.</summary>
    public bool IsInvoking { get; private set; }

    /// <summary>Gets or sets an exception thrown instead of running an awaited state change.</summary>
    public Exception? InvokeException { get; set; }

    /// <summary>Gets or sets the one-based awaited invocation on which the configured exception is thrown, or <see langword="null"/> for every invocation.</summary>
    public int? ThrowOnInvokeNumber { get; set; }

    /// <inheritdoc />
    public void Post(Action action)
    {
        PostCount++;
        action();
    }

    /// <inheritdoc />
    public Task InvokeAsync(Action action)
    {
        InvokeCount++;
        if (InvokeException is not null
            && (ThrowOnInvokeNumber is null || ThrowOnInvokeNumber == InvokeCount))
        {
            throw InvokeException;
        }

        IsInvoking = true;
        try
        {
            action();
            return Task.CompletedTask;
        }
        finally
        {
            IsInvoking = false;
        }
    }
}
