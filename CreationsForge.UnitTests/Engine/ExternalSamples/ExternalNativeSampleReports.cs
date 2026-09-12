using Mutagen.Bethesda.Plugins;

namespace CreationsForge.UnitTests.Engine.ExternalSamples;

/// <summary>Retains one selected external FormList snapshot outside native workspace ownership.</summary>
internal sealed class ExternalSelectedRecord
{
    /// <summary>Initializes an empty detached selected-record holder.</summary>
    internal ExternalSelectedRecord()
    { }

    /// <summary>Gets or initializes the exact native identity.</summary>
    public FormKey FormKey { get; init; }

    /// <summary>Gets or initializes the selected containing plugin identity.</summary>
    public string ContainingModKey { get; init; } = string.Empty;

    /// <summary>Gets or initializes the selected source artifact path.</summary>
    public string SourcePath { get; init; } = string.Empty;

    /// <summary>Gets or initializes complete detached inspector JSON.</summary>
    public string Json { get; init; } = string.Empty;

    /// <summary>Gets or initializes ordered native item identities including duplicates and null sentinels.</summary>
    public IReadOnlyList<FormKey> Items { get; init; } = [];
}

/// <summary>Reports one external execution across all requested output representations.</summary>
internal sealed class ExternalNativeSampleExecutionReport
{
    /// <summary>Initializes an empty serializable execution report.</summary>
    internal ExternalNativeSampleExecutionReport()
    { }

    /// <summary>Gets or initializes the execution report schema version.</summary>
    public int SchemaVersion { get; init; }

    /// <summary>Gets or initializes execution start time.</summary>
    public DateTimeOffset StartedAtUtc { get; init; }

    /// <summary>Gets or sets execution completion time.</summary>
    public DateTimeOffset CompletedAtUtc { get; set; }

    /// <summary>Gets or initializes the fresh retained run directory.</summary>
    public string RunDirectory { get; init; } = string.Empty;

    /// <summary>Gets or sets Passed, Failed, or Running state.</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>Gets or sets complete failure diagnostics when execution fails.</summary>
    public string? Failure { get; set; }

    /// <summary>Gets or initializes metadata-only evidence captured before parsing.</summary>
    public ExternalNativeSamplePreflightReport PreflightBefore { get; init; } = new();

    /// <summary>Gets or sets metadata-only evidence captured after all native lifetimes close.</summary>
    public ExternalNativeSamplePreflightReport? PreflightAfter { get; set; }

    /// <summary>Gets or sets streaming plugin hashes captured before parsing.</summary>
    public List<ExternalSourceHashEvidence> SourcePluginHashesBefore { get; set; } = [];

    /// <summary>Gets or sets streaming plugin hashes captured after all native lifetimes close.</summary>
    public List<ExternalSourceHashEvidence>? SourcePluginHashesAfter { get; set; }

    /// <summary>Gets mutable serial output-case evidence.</summary>
    public List<ExternalNativeSampleCaseReport> Cases { get; } = [];

    /// <summary>Gets mutable explicit acceptance gaps and preserved fail-closed limitations.</summary>
    public List<string> AcceptanceGaps { get; } = [];
}

/// <summary>Reports streaming identity for one explicit plugin input.</summary>
internal sealed class ExternalSourceHashEvidence
{
    /// <summary>Initializes an empty serializable source-hash entry.</summary>
    internal ExternalSourceHashEvidence()
    { }

    /// <summary>Gets or initializes the canonical source path.</summary>
    public string Path { get; init; } = string.Empty;

    /// <summary>Gets or initializes whether the plugin existed.</summary>
    public bool Exists { get; init; }

    /// <summary>Gets or initializes the exact byte length.</summary>
    public long Length { get; init; }

    /// <summary>Gets or initializes the uppercase streaming SHA-256 digest.</summary>
    public string Sha256 { get; init; } = string.Empty;
}

/// <summary>Reports one serial embedded or localized save-and-reopen case.</summary>
internal sealed class ExternalNativeSampleCaseReport
{
    /// <summary>Initializes an empty serializable output-case report.</summary>
    internal ExternalNativeSampleCaseReport()
    { }

    /// <summary>Gets or initializes the output representation.</summary>
    public string OutputMode { get; init; } = string.Empty;

    /// <summary>Gets or initializes the retained output plugin path.</summary>
    public string OutputPluginPath { get; init; } = string.Empty;

    /// <summary>Gets or sets the initial engine-owned complete source baseline identity.</summary>
    public Guid InitialSourceBaselineId { get; set; }

    /// <summary>Gets or sets the fresh engine-owned complete source baseline identity.</summary>
    public Guid FreshSourceBaselineId { get; set; }

    /// <summary>Gets or sets the first committed output baseline identity.</summary>
    public Guid FirstCommittedBaselineId { get; set; }

    /// <summary>Gets or sets the final committed output baseline identity.</summary>
    public Guid FinalCommittedBaselineId { get; set; }

    /// <summary>Gets mutable exact selected-record evidence.</summary>
    public List<ExternalSelectedRecordReport> SelectedRecords { get; } = [];

    /// <summary>Gets or sets present committed output artifacts.</summary>
    public string[] PresentOutputArtifacts { get; set; } = [];

    /// <summary>Gets or sets exact expected masters in load-order order.</summary>
    public string[] ExpectedMasters { get; set; } = [];

    /// <summary>Gets or sets whether the existing localized material rewrite failed closed.</summary>
    public bool MaterialRewriteFailClosed { get; set; }

    /// <summary>Gets or sets selected source localized-name values proven against explicit physical tables.</summary>
    public IReadOnlyList<ExternalLocalizedNameEvidence> LocalizedNameEvidence { get; set; } = [];

    /// <summary>Gets or sets the expected localized material-rewrite failure message.</summary>
    public string? MaterialRewriteFailure { get; set; }

    /// <summary>Gets or sets independent direct-reader evidence.</summary>
    public ExternalNativeVerificationResult? DirectVerification { get; set; }
}

/// <summary>Reports the exact selected identity and available native field categories without retaining full source JSON.</summary>
internal sealed class ExternalSelectedRecordReport
{
    /// <summary>Initializes an empty serializable selected-record report.</summary>
    internal ExternalSelectedRecordReport()
    { }

    /// <summary>Gets or initializes the exact selected FormKey.</summary>
    public string FormKey { get; init; } = string.Empty;

    /// <summary>Gets or initializes the selected containing plugin.</summary>
    public string ContainingModKey { get; init; } = string.Empty;

    /// <summary>Gets or initializes the selected source artifact path.</summary>
    public string SourcePath { get; init; } = string.Empty;

    /// <summary>Gets or initializes present, absent, and not-applicable field categories.</summary>
    public string[] FieldCategories { get; init; } = [];

    /// <summary>Gets or initializes a digest of complete selected inspector JSON.</summary>
    public string InspectorJsonSha256 { get; init; } = string.Empty;
}
