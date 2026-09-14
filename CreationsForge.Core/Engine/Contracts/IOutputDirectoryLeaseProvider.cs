namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Acquires process-wide and cooperating cross-process leases for canonical output directories.</summary>
public interface IOutputDirectoryLeaseProvider
{
    /// <summary>Acquires exclusive access to one output directory under the requested metadata policy.</summary>
    /// <param name="outputDirectoryPath">The canonical output directory whose stable guard is protected.</param>
    /// <param name="mode">Whether absent stable guard metadata may be created.</param>
    /// <param name="cancellationToken">A token that cancels in-process lease waiting and guard acquisition.</param>
    /// <returns>An acquired lease, normal existing-only absence, or a typed validation or contention failure.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    ValueTask<EngineResult<OutputDirectoryLeaseAcquisition>> AcquireAsync(
        string outputDirectoryPath,
        OutputDirectoryLeaseMode mode,
        CancellationToken cancellationToken = default);
}
