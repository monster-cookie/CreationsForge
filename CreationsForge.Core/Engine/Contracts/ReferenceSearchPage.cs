namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Carries one bounded page of native reference-search matches.
/// </summary>
public sealed class ReferenceSearchPage
{
    /// <summary>Initializes an immutable reference-search page.</summary>
    /// <param name="matches">The ordered matches to snapshot.</param>
    /// <param name="continuationToken">The token for the next page, or <see langword="null"/> when complete.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="matches"/> is <see langword="null"/>.</exception>
    public ReferenceSearchPage(IReadOnlyList<ReferenceSearchMatch> matches, string? continuationToken)
    {
        ArgumentNullException.ThrowIfNull(matches);
        Matches = Array.AsReadOnly(matches.ToArray());
        ContinuationToken = continuationToken;
    }

    /// <summary>Gets the immutable ordered matches in this page.</summary>
    public IReadOnlyList<ReferenceSearchMatch> Matches { get; }

    /// <summary>Gets the token for the next deterministic page, or <see langword="null"/> when complete.</summary>
    public string? ContinuationToken { get; }
}
