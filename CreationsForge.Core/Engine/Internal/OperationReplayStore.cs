using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Core.Engine.Internal;

/// <summary>
/// Retains immutable operation results and rejects reuse with a different canonical payload.
/// </summary>
internal sealed class OperationReplayStore
{
    /// <summary>Stores operation fingerprints and their immutable result objects.</summary>
    private readonly Dictionary<Guid, (OperationFingerprint Fingerprint, object Result)> Entries = new();

    /// <summary>Looks up a prior operation result or reports conflicting identifier reuse.</summary>
    /// <typeparam name="T">The immutable result type expected by the operation.</typeparam>
    /// <param name="operationId">The operation identifier.</param>
    /// <param name="fingerprint">The complete canonical request fingerprint.</param>
    /// <param name="result">The prior result when an exact replay exists.</param>
    /// <param name="conflict">Whether the identifier exists with a different payload or result type.</param>
    /// <returns><see langword="true"/> when an exact prior result was found.</returns>
    internal bool TryGet<T>(
        Guid operationId,
        OperationFingerprint fingerprint,
        out T? result,
        out bool conflict)
        where T : class
    {
        if (!Entries.TryGetValue(operationId, out var entry))
        {
            result = null;
            conflict = false;
            return false;
        }

        if (!entry.Fingerprint.Equals(fingerprint) || entry.Result is not T typedResult)
        {
            result = null;
            conflict = true;
            return false;
        }

        result = typedResult;
        conflict = false;
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
        Entries.Add(operationId, (fingerprint, result));
    }
}
