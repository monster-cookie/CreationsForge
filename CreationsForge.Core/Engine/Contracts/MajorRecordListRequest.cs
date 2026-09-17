using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Requests one bounded deterministic page of major-record contexts without a persistent index.</summary>
public sealed class MajorRecordListRequest
{
    /// <summary>Initializes a bounded major-record listing request.</summary>
    /// <param name="maximumResults">The positive maximum number of contexts to return.</param>
    /// <param name="continuationToken">An optional engine-issued token for the next page.</param>
    /// <param name="scope">The source, winning-override, all-context, or staged-output view.</param>
    /// <param name="containingModKey">An optional containing plugin filter for non-winning scopes.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the page size or scope is invalid.</exception>
    /// <exception cref="ArgumentException">Thrown when a containing plugin is combined with winning overrides.</exception>
    public MajorRecordListRequest(
        int maximumResults,
        string? continuationToken = null,
        RecordScope scope = RecordScope.WinningOverrides,
        ModKey? containingModKey = null)
    {
        if (maximumResults <= 0 || maximumResults > ReferenceSearchRequest.MaximumPageSize)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maximumResults),
                $"Major-record list page size must be between 1 and {ReferenceSearchRequest.MaximumPageSize}.");
        }
        if (!Enum.IsDefined(scope))
        {
            throw new ArgumentOutOfRangeException(nameof(scope));
        }
        if (scope == RecordScope.WinningOverrides && containingModKey.HasValue)
        {
            throw new ArgumentException("A containing plugin cannot be combined with the winning-override view.", nameof(containingModKey));
        }

        MaximumResults = maximumResults;
        ContinuationToken = continuationToken;
        Scope = scope;
        ContainingModKey = containingModKey;
    }

    /// <summary>Gets the maximum number of contexts in the page.</summary>
    public int MaximumResults { get; }

    /// <summary>Gets the engine-issued continuation token, or <see langword="null"/> for the first page.</summary>
    public string? ContinuationToken { get; }

    /// <summary>Gets the record context view to list.</summary>
    public RecordScope Scope { get; }

    /// <summary>Gets the containing plugin filter, or <see langword="null"/>.</summary>
    public ModKey? ContainingModKey { get; }

    /// <summary>Creates the internal match-all search contract shared with deterministic reference paging.</summary>
    /// <returns>A revision-bound cursor-compatible search request.</returns>
    internal ReferenceSearchRequest ToSearchRequest()
    {
        return ReferenceSearchRequest.CreateListing(MaximumResults, ContinuationToken, Scope, ContainingModKey);
    }
}
