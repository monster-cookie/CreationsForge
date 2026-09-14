namespace CreationsForge.Core.Engine.Persistence;

/// <summary>Provides the narrow per-file publication seam used for deterministic interruption and fault-injection review.</summary>
internal class SaveTransactionFileOperations
{
    /// <summary>Initializes the default BCL-backed file operations.</summary>
    internal SaveTransactionFileOperations()
    { }

    /// <summary>Runs immediately before one destination namespace mutation.</summary>
    /// <param name="saveOperationId">The original save operation identifier.</param>
    /// <param name="artifactIndex">The stable artifact-plan index.</param>
    internal virtual void BeforeDestinationMutation(Guid saveOperationId, int artifactIndex)
    { }

    /// <summary>Atomically moves a flushed same-volume publication file into the destination namespace.</summary>
    /// <param name="publishPath">The transaction-owned publication file.</param>
    /// <param name="destinationPath">The exact output artifact path.</param>
    internal virtual void Publish(string publishPath, string destinationPath)
    {
        File.Move(publishPath, destinationPath, overwrite: true);
    }

    /// <summary>Atomically removes a destination artifact by moving it to a transaction-owned same-volume path.</summary>
    /// <param name="destinationPath">The exact output artifact path.</param>
    /// <param name="retiredPath">The absent transaction-owned retirement path.</param>
    internal virtual void Retire(string destinationPath, string retiredPath)
    {
        File.Move(destinationPath, retiredPath, overwrite: false);
    }

    /// <summary>Runs immediately after one destination namespace mutation and before its progress journal update.</summary>
    /// <param name="saveOperationId">The original save operation identifier.</param>
    /// <param name="artifactIndex">The stable artifact-plan index.</param>
    internal virtual void AfterDestinationMutation(Guid saveOperationId, int artifactIndex)
    { }
}
