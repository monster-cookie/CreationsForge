namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Describes whether output-directory lease acquisition returned an owned lease.</summary>
public enum OutputDirectoryLeaseAcquisitionStatus
{
    /// <summary>The caller exclusively owns the in-process and cross-process output-directory lease.</summary>
    Acquired,

    /// <summary>Existing-only acquisition found no stable guard metadata and created nothing.</summary>
    GuardNotFound
}
