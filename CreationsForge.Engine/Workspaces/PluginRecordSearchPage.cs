namespace CreationsForge.Engine.Workspaces;

/// <summary>Contains one bounded page of winning-record summaries.</summary>
public sealed class PluginRecordSearchPage
{
    /// <summary>The maximum number of summaries one page may retain.</summary>
    public const int MaximumTake = 50;

    /// <summary>Initializes a search page.</summary>
    /// <param name="revision">The workspace revision captured while the page was read.</param>
    /// <param name="skip">The number of matching summaries skipped before this page.</param>
    /// <param name="take">The requested page size.</param>
    /// <param name="hasMore">Whether another matching summary exists after this page.</param>
    /// <param name="records">The retained summaries, in enumeration order.</param>
    public PluginRecordSearchPage(
        ulong revision,
        int skip,
        int take,
        bool hasMore,
        IReadOnlyList<PluginRecordSummary> records)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(skip);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(take);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(take, MaximumTake);
        ArgumentNullException.ThrowIfNull(records);
        Revision = revision;
        Skip = skip;
        Take = take;
        HasMore = hasMore;
        Records = records;
    }

    /// <summary>Gets the workspace revision captured while the page was read.</summary>
    public ulong Revision { get; }

    /// <summary>Gets the number of matching summaries skipped before this page.</summary>
    public int Skip { get; }

    /// <summary>Gets the requested page size.</summary>
    public int Take { get; }

    /// <summary>Gets whether another matching summary exists after this page.</summary>
    public bool HasMore { get; }

    /// <summary>Gets the retained summaries. The workspace does not keep an unbounded catalog for this query.</summary>
    public IReadOnlyList<PluginRecordSummary> Records { get; }
}
