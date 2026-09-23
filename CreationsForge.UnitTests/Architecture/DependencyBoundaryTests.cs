using CreationsForge.Engine.Interfaces;

namespace CreationsForge.UnitTests.Architecture;

/// <summary>
/// Verifies that the new engine assembly remains independent from presentation, transport, and database layers.
/// </summary>
public sealed class DependencyBoundaryTests
{
    /// <summary>Requires the engine dependency graph to contain only UI-neutral runtime and Mutagen assemblies.</summary>
    [Fact]
    public void EngineDoesNotReferencePresentationTransportOrDatabaseAssemblies()
    {
        var references = typeof(IGameIntegration).Assembly
            .GetReferencedAssemblies()
            .Select(reference => reference.Name ?? string.Empty)
            .ToArray();

        Assert.DoesNotContain(references, name => name.StartsWith("Avalonia", StringComparison.Ordinal));
        Assert.DoesNotContain(references, name => name.StartsWith("ModelContextProtocol", StringComparison.Ordinal));
        Assert.DoesNotContain(references, name => name.StartsWith("Microsoft.Data.Sqlite", StringComparison.Ordinal));
        Assert.DoesNotContain(references, name => name.StartsWith("System.Data.SQLite", StringComparison.Ordinal));
        Assert.DoesNotContain(references, name => name.StartsWith("NPoco", StringComparison.Ordinal));
        Assert.DoesNotContain(references, name => name.StartsWith("DbUp", StringComparison.Ordinal));
    }
}
