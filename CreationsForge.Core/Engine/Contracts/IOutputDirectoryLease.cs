namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Owns exclusive in-process and cooperating cross-process access to one canonical output directory.
/// </summary>
public interface IOutputDirectoryLease : IAsyncDisposable
{
    /// <summary>Gets the canonical output directory protected by this lease.</summary>
    string OutputDirectoryPath { get; }
}
