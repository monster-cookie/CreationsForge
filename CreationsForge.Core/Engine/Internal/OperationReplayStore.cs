using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Core.Engine.Internal;

/// <summary>
/// Retains immutable operation results and rejects reuse with a different canonical payload.
/// </summary>
internal sealed class OperationReplayStore
{
    /// <summary>The maximum number of replay entries retained by a normal workspace.</summary>
    internal const int DefaultMaximumEntryCount = 4096;

    /// <summary>Stores operation fingerprints and their immutable result objects.</summary>
    private readonly Dictionary<Guid, (OperationFingerprint Fingerprint, object Result, bool Evictable)> Entries = new();

    /// <summary>Orders finalization entries so completed or rejected finalization attempts can be evicted safely.</summary>
    private readonly Queue<Guid> EvictableOperationIds = new();

    /// <summary>Retains a bounded identity window for evicted finalization operations.</summary>
    private readonly Dictionary<Guid, OperationFingerprint> ExpiredFinalizationFingerprints = new();

    /// <summary>Orders expired finalization identities for bounded tombstone eviction.</summary>
    private readonly Queue<Guid> ExpiredFinalizationOperationIds = new();

    /// <summary>Tracks ordinary mutation entries that cannot be evicted during the workspace lifetime.</summary>
    private int NonEvictableEntryCount;

    /// <summary>Stores the maximum number of immutable operation results retained by this workspace.</summary>
    private readonly int MaximumEntryCount;

    /// <summary>Initializes a bounded replay store.</summary>
    /// <param name="maximumEntryCount">The positive maximum number of retained operation results.</param>
    internal OperationReplayStore(int maximumEntryCount = DefaultMaximumEntryCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maximumEntryCount);
        MaximumEntryCount = maximumEntryCount;
    }

    /// <summary>Determines whether a fresh operation identifier can be retained while preserving finalization capacity.</summary>
    /// <param name="operationId">The fresh operation identifier.</param>
    /// <param name="reservedEntryCount">The number of remaining entries reserved for save or recovery finalization.</param>
    /// <returns><see langword="true"/> when the identifier already exists or the store has room beyond the reserved entries.</returns>
    internal bool CanStore(Guid operationId, int reservedEntryCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(reservedEntryCount);
        return Entries.ContainsKey(operationId) || NonEvictableEntryCount < MaximumEntryCount - reservedEntryCount;
    }

    /// <summary>Determines whether a finalization result can be retained by reusing bounded finalization capacity when necessary.</summary>
    /// <param name="operationId">The finalization operation identifier.</param>
    /// <returns><see langword="true"/> when the operation already exists or the bounded store has any finalization capacity.</returns>
    internal bool CanStoreFinalization(Guid operationId)
    {
        return Entries.ContainsKey(operationId) || MaximumEntryCount > NonEvictableEntryCount;
    }

    /// <summary>Looks up a prior operation result or reports conflicting identifier reuse.</summary>
    /// <typeparam name="T">The immutable result type expected by the operation.</typeparam>
    /// <param name="operationId">The operation identifier.</param>
    /// <param name="fingerprint">The complete canonical request fingerprint.</param>
    /// <param name="result">The prior result when an exact replay exists.</param>
    /// <param name="conflict">Whether the identifier exists with a different payload or result type.</param>
    /// <param name="expired">Whether the exact identifier and payload left the bounded finalization replay window.</param>
    /// <returns><see langword="true"/> when an exact prior result was found.</returns>
    internal bool TryGet<T>(
        Guid operationId,
        OperationFingerprint fingerprint,
        out T? result,
        out bool conflict,
        out bool expired)
        where T : class
    {
        if (!Entries.TryGetValue(operationId, out var entry))
        {
            if (ExpiredFinalizationFingerprints.TryGetValue(operationId, out var expiredFingerprint))
            {
                result = null;
                conflict = !expiredFingerprint.Equals(fingerprint);
                expired = !conflict;
                return false;
            }

            result = null;
            conflict = false;
            expired = false;
            return false;
        }

        if (!entry.Fingerprint.Equals(fingerprint) || entry.Result is not T typedResult)
        {
            result = null;
            conflict = true;
            expired = false;
            return false;
        }

        result = typedResult;
        conflict = false;
        expired = false;
        return true;
    }

    /// <summary>Stores the first immutable result for an operation identifier.</summary>
    /// <typeparam name="T">The immutable result type.</typeparam>
    /// <param name="operationId">The operation identifier.</param>
    /// <param name="fingerprint">The complete canonical request fingerprint.</param>
    /// <param name="result">The immutable result to replay.</param>
    internal void Store<T>(Guid operationId, OperationFingerprint fingerprint, T result)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(result);
        if (!CanStore(operationId, reservedEntryCount: 0))
        {
            throw new InvalidOperationException("The operation replay store is at capacity.");
        }

        EnsureAvailableSlot();
        Entries.Add(operationId, (fingerprint, result, false));
        NonEvictableEntryCount++;
    }

    /// <summary>Stores a replayable finalization result in bounded evictable capacity.</summary>
    /// <typeparam name="T">The immutable finalization result type.</typeparam>
    /// <param name="operationId">The finalization operation identifier.</param>
    /// <param name="fingerprint">The complete canonical request fingerprint.</param>
    /// <param name="result">The immutable result to replay while retained.</param>
    internal void StoreFinalization<T>(Guid operationId, OperationFingerprint fingerprint, T result)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(result);
        if (!CanStoreFinalization(operationId))
        {
            throw new InvalidOperationException("The operation replay store has no finalization capacity.");
        }

        EnsureAvailableSlot();
        Entries.Add(operationId, (fingerprint, result, true));
        EvictableOperationIds.Enqueue(operationId);
    }

    /// <summary>Evicts the oldest retained finalization result when the bounded store is full.</summary>
    private void EnsureAvailableSlot()
    {
        while (Entries.Count >= MaximumEntryCount && EvictableOperationIds.TryDequeue(out var operationId))
        {
            if (Entries.TryGetValue(operationId, out var entry) && entry.Evictable)
            {
                Entries.Remove(operationId);
                RetainExpiredIdentity(operationId, entry.Fingerprint);
            }
        }

        if (Entries.Count >= MaximumEntryCount)
        {
            throw new InvalidOperationException("The operation replay store is at capacity.");
        }
    }

    /// <summary>Retains an evicted finalization identity within the bounded replay-expiration window.</summary>
    /// <param name="operationId">The evicted operation identifier.</param>
    /// <param name="fingerprint">The operation's canonical request fingerprint.</param>
    private void RetainExpiredIdentity(Guid operationId, OperationFingerprint fingerprint)
    {
        while (ExpiredFinalizationFingerprints.Count >= MaximumEntryCount
            && ExpiredFinalizationOperationIds.TryDequeue(out var expiredOperationId))
        {
            ExpiredFinalizationFingerprints.Remove(expiredOperationId);
        }

        ExpiredFinalizationFingerprints[operationId] = fingerprint;
        ExpiredFinalizationOperationIds.Enqueue(operationId);
    }
}
