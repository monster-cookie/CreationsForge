namespace CreationsForge.UnitTests.Engine.ExternalSamples;

/// <summary>Reports one source localized-name value proven against its selected native strings table.</summary>
internal sealed class ExternalLocalizedNameEvidence
{
    /// <summary>Initializes an empty serializable localized-name evidence entry.</summary>
    internal ExternalLocalizedNameEvidence()
    { }

    /// <summary>Gets or initializes the selected source FormList identity.</summary>
    public string FormKey { get; init; } = string.Empty;

    /// <summary>Gets or initializes the native language.</summary>
    public string Language { get; init; } = string.Empty;

    /// <summary>Gets or initializes the original source string-table key.</summary>
    public uint StringsKey { get; init; }

    /// <summary>Gets or initializes the physical source path selected by the production strings lookup.</summary>
    public string SelectedSourcePath { get; init; } = string.Empty;

    /// <summary>Gets or initializes the manually opened loose file or archive-entry identity.</summary>
    public string VerifiedTable { get; init; } = string.Empty;

    /// <summary>Gets or initializes the SHA-256 digest of the ordinally verified text.</summary>
    public string TextSha256 { get; init; } = string.Empty;
}
