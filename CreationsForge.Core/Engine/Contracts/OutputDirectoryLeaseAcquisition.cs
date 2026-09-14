namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Reports an acquired output-directory lease or normal absence of existing guard metadata.</summary>
public sealed class OutputDirectoryLeaseAcquisition
{
    /// <summary>Initializes a typed output-directory lease acquisition.</summary>
    /// <param name="status">Whether a lease was acquired or an existing guard was absent.</param>
    /// <param name="lease">The owned lease when acquired; otherwise <see langword="null"/>.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="status"/> and <paramref name="lease"/> disagree.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="status"/> is undefined.</exception>
    public OutputDirectoryLeaseAcquisition(
        OutputDirectoryLeaseAcquisitionStatus status,
        IOutputDirectoryLease? lease)
    {
        if (!Enum.IsDefined(status))
        {
            throw new ArgumentOutOfRangeException(nameof(status));
        }

        if ((status == OutputDirectoryLeaseAcquisitionStatus.Acquired) != (lease is not null))
        {
            throw new ArgumentException("An acquired result requires a lease, while a missing guard cannot carry one.", nameof(lease));
        }

        Status = status;
        Lease = lease;
    }

    /// <summary>Gets whether a lease was acquired or existing guard metadata was absent.</summary>
    public OutputDirectoryLeaseAcquisitionStatus Status { get; }

    /// <summary>Gets the exclusively owned lease, or <see langword="null"/> when the existing guard was absent.</summary>
    public IOutputDirectoryLease? Lease { get; }
}
