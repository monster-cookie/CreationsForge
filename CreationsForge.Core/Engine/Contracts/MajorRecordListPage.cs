namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Carries one bounded page of lightweight major-record context summaries.</summary>
public sealed class MajorRecordListPage
{
    /// <summary>Initializes an immutable major-record page.</summary>
    /// <param name="records">The ordered context summaries to snapshot.</param>
    /// <param name="continuationToken">The token for the next page, or <see langword="null"/> when complete.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="records"/> is <see langword="null"/>.</exception>
    public MajorRecordListPage(IReadOnlyList<ReferenceSearchMatch> records, string? continuationToken)
    {
        ArgumentNullException.ThrowIfNull(records);
        Records = Array.AsReadOnly(records.ToArray());
        ContinuationToken = continuationToken;
    }

    /// <summary>Gets ordered lightweight context summaries.</summary>
    public IReadOnlyList<ReferenceSearchMatch> Records { get; }

    /// <summary>Gets the next deterministic page token, or <see langword="null"/> when complete.</summary>
    public string? ContinuationToken { get; }
}
