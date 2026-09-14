namespace CreationsForge.RecordEditing.Drafts;

/// <summary>Maps an exact path-prefixed codec message to a currently materialized typed draft node.</summary>
public static class RecordWireDraftErrorMapper
{
    /// <summary>Creates one codec issue while preserving the complete original engine message.</summary>
    /// <param name="draft">The current typed command draft.</param>
    /// <param name="message">The complete codec or engine message.</param>
    /// <returns>A node-bound issue when the leading path resolves exactly; otherwise a banner-level issue.</returns>
    public static RecordWireDraftIssue Map(FormListDraft draft, string message)
    {
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        var node = FormListDraft.EnumerateNodes(draft.Root)
            .Where(candidate => message.StartsWith(candidate.Path, StringComparison.Ordinal))
            .OrderByDescending(candidate => candidate.Path.Length)
            .FirstOrDefault();
        return new RecordWireDraftIssue(RecordWireDraftIssueCode.CodecRejected, node?.Path ?? "$", message, node);
    }
}
