namespace CreationsForge.Engine.Persistence;

/// <summary>Reports one UI-neutral save lifecycle event.</summary>
public sealed class PluginSaveProgress
{
    /// <summary>Initializes a save progress event.</summary>
    /// <param name="phase">The save lifecycle phase active when the event was created.</param>
    /// <param name="destinationPath">The absolute output plugin path targeted by the save.</param>
    /// <param name="revision">The exact workspace revision captured for the save.</param>
    /// <param name="elapsed">The elapsed duration from the start of the save through this event.</param>
    /// <param name="diagnostic">A bounded human-readable description of the phase or terminal outcome.</param>
    public PluginSaveProgress(
        PluginSavePhase phase,
        string destinationPath,
        ulong revision,
        TimeSpan elapsed,
        string diagnostic)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(diagnostic);

        Phase = phase;
        DestinationPath = destinationPath;
        Revision = revision;
        Elapsed = elapsed;
        Diagnostic = diagnostic;
    }

    /// <summary>Gets the active save phase.</summary>
    public PluginSavePhase Phase { get; }

    /// <summary>Gets the absolute output plugin path.</summary>
    public string DestinationPath { get; }

    /// <summary>Gets the exact workspace revision captured for this save.</summary>
    public ulong Revision { get; }

    /// <summary>Gets the elapsed save duration at this event.</summary>
    public TimeSpan Elapsed { get; }

    /// <summary>Gets a bounded human-readable diagnostic.</summary>
    public string Diagnostic { get; }
}
