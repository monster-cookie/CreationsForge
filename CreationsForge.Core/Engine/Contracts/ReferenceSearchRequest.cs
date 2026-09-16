using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Requests a bounded reference search without materializing a persistent index.</summary>
public sealed class ReferenceSearchRequest
{
    /// <summary>The largest accepted deterministic page size.</summary>
    public const int MaximumPageSize = 250;

    /// <summary>The largest accepted search query length after surrounding whitespace is removed.</summary>
    public const int MaximumQueryLength = 256;

    /// <summary>Initializes a bounded reference search.</summary>
    /// <param name="query">The non-empty record identity or EditorID search text.</param>
    /// <param name="maximumResults">The positive maximum number of matches to return.</param>
    /// <param name="continuationToken">An optional adapter-issued token for the next deterministic page.</param>
    /// <param name="scope">The record contexts to search.</param>
    /// <param name="containingModKey">An optional containing plugin filter for non-winning scopes.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="query"/> is empty or whitespace.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the query is too long, the page size is outside the supported bounds, or <paramref name="scope"/> is undefined.</exception>
    public ReferenceSearchRequest(
        string query,
        int maximumResults,
        string? continuationToken = null,
        RecordScope scope = RecordScope.WinningOverrides,
        ModKey? containingModKey = null)
        : this(query, maximumResults, continuationToken, scope, containingModKey, allowEmptyQuery: false)
    { }

    /// <summary>Initializes a search request, optionally reserving an empty query for internal match-all listing.</summary>
    /// <param name="query">The search text, or empty text only for an internal listing request.</param>
    /// <param name="maximumResults">The positive maximum number of matches.</param>
    /// <param name="continuationToken">The optional continuation token.</param>
    /// <param name="scope">The record contexts to search.</param>
    /// <param name="containingModKey">The optional containing plugin filter.</param>
    /// <param name="allowEmptyQuery">Whether an empty query represents match-all listing.</param>
    private ReferenceSearchRequest(
        string query,
        int maximumResults,
        string? continuationToken,
        RecordScope scope,
        ModKey? containingModKey,
        bool allowEmptyQuery)
    {
        ArgumentNullException.ThrowIfNull(query);
        var normalizedQuery = query.Trim();
        if (!allowEmptyQuery && normalizedQuery.Length == 0)
        {
            throw new ArgumentException("Reference search queries cannot be empty or whitespace.", nameof(query));
        }
        if (normalizedQuery.Length > MaximumQueryLength)
        {
            throw new ArgumentOutOfRangeException(
                nameof(query),
                $"Reference search queries cannot exceed {MaximumQueryLength} characters.");
        }

        if (maximumResults <= 0 || maximumResults > MaximumPageSize)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maximumResults),
                $"Reference search page size must be between 1 and {MaximumPageSize}.");
        }

        if (!Enum.IsDefined(scope))
        {
            throw new ArgumentOutOfRangeException(nameof(scope));
        }

        if (scope == RecordScope.WinningOverrides && containingModKey.HasValue)
        {
            throw new ArgumentException(
                "A containing plugin cannot be combined with the winning-override view.",
                nameof(containingModKey));
        }

        Query = normalizedQuery;
        MaximumResults = maximumResults;
        ContinuationToken = continuationToken;
        Scope = scope;
        ContainingModKey = containingModKey;
    }

    /// <summary>Creates the internal match-all search used by bounded major-record listing.</summary>
    /// <param name="maximumResults">The positive maximum page size.</param>
    /// <param name="continuationToken">The optional engine-issued continuation token.</param>
    /// <param name="scope">The record contexts to list.</param>
    /// <param name="containingModKey">The optional containing plugin filter.</param>
    /// <returns>A cursor-compatible search request whose empty query matches every record.</returns>
    internal static ReferenceSearchRequest CreateListing(
        int maximumResults,
        string? continuationToken,
        RecordScope scope,
        ModKey? containingModKey)
    {
        return new ReferenceSearchRequest(
            string.Empty,
            maximumResults,
            continuationToken,
            scope,
            containingModKey,
            allowEmptyQuery: true);
    }

    /// <summary>Gets the record identity or EditorID search text.</summary>
    public string Query { get; }

    /// <summary>Gets the maximum number of matches to return.</summary>
    public int MaximumResults { get; }

    /// <summary>Gets the adapter-issued continuation token, or <see langword="null"/> for the first page.</summary>
    public string? ContinuationToken { get; }

    /// <summary>Gets the record contexts to search.</summary>
    public RecordScope Scope { get; }

    /// <summary>Gets the containing plugin filter, or <see langword="null"/> when all contexts in the scope participate.</summary>
    public ModKey? ContainingModKey { get; }
}
