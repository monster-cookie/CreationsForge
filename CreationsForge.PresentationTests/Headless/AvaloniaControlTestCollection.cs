namespace CreationsForge.PresentationTests.Headless;

/// <summary>
/// Serializes tests that construct real Avalonia controls because headless sessions share process-wide application and dispatcher state.
/// </summary>
[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class AvaloniaControlTestCollection
{
    /// <summary>The collection name used by tests that require exclusive ownership of Avalonia process state.</summary>
    public const string Name = "Avalonia control tests";
}
