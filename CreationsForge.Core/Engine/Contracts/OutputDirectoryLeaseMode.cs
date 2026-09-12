namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Controls whether acquiring an output-directory lease may create stable guard metadata.</summary>
public enum OutputDirectoryLeaseMode
{
    /// <summary>Open existing guard metadata or create it when absent.</summary>
    CreateOrOpen,

    /// <summary>Open existing guard metadata without creating a directory or guard file.</summary>
    ExistingOnly
}
