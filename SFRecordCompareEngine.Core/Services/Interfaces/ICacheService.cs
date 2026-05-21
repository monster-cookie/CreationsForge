using SFRecordCompareEngine.Core.DTOs.Cache;

namespace SFRecordCompareEngine.Core.Services.Interfaces;

public interface ICacheService
{
    /// <summary>
    ///     Load the persisted reference cache from disk into memory if a valid cache file exists.
    /// </summary>
    void LoadFromDisk();

    /// <summary>
    ///     Save the in-memory reference cache to disk when it has changed.
    /// </summary>
    void SaveToDisk();

    /// <summary>
    ///     Build or update the reference cache for the specified plugins, rebuilding entries whose source plugin files changed.
    /// </summary>
    /// <param name="pluginNames">The plugin file names to validate and cache.</param>
    /// <param name="progress">Optional progress reporter for UI status updates.</param>
    /// <param name="cancellationToken">The cancellation token used to stop cache processing.</param>
    /// <returns>A task that completes when cache validation and rebuild work is finished.</returns>
    Task BuildOrUpdateReferenceCacheAsync(IList<string> pluginNames, IProgress<CacheBuildProgressDTO>? progress, CancellationToken cancellationToken);

    /// <summary>
    ///     Resolve a raw or normalized record reference value to a display value from the cache.
    /// </summary>
    /// <param name="referenceValue">The reference value to resolve, typically a FormKey string.</param>
    /// <returns>The cached display value, or null when the reference is not cached.</returns>
    string? ResolveReferenceDisplayValue(string referenceValue);

    /// <summary>
    ///     Clear all in-memory cache entries and mark the cache for persistence.
    /// </summary>
    void Clear();
}
