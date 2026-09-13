using System.Text.Json;

namespace CreationsForge.UnitTests.Engine.ClientAcceptance;

/// <summary>Describes retained generated inputs, an exact domain-tool plan, and independently verifiable results for every supported game.</summary>
internal sealed class ClientAcceptanceManifest
{
    /// <summary>Gets or initializes the closed manifest schema version.</summary>
    public int SchemaVersion { get; init; }

    /// <summary>Gets or initializes the UTC time at which the retained fixture was exported.</summary>
    public DateTimeOffset GeneratedAtUtc { get; init; }

    /// <summary>Gets or initializes the absolute repository root used to resolve the server build.</summary>
    public string RepositoryRoot { get; init; } = string.Empty;

    /// <summary>Gets or initializes the absolute retained acceptance root.</summary>
    public string AcceptanceRoot { get; init; } = string.Empty;

    /// <summary>Gets or initializes the real MCP server launch contract.</summary>
    public ClientAcceptanceServer Server { get; init; } = new();

    /// <summary>Gets or initializes the three game-specific acceptance cases.</summary>
    public ClientAcceptanceCase[] Cases { get; init; } = [];
}

/// <summary>Describes how a real client launches the repository-built CreationsForge MCP server.</summary>
internal sealed class ClientAcceptanceServer
{
    /// <summary>Gets or initializes the server process command.</summary>
    public string Command { get; init; } = string.Empty;

    /// <summary>Gets or initializes the ordered server process arguments.</summary>
    public string[] Arguments { get; init; } = [];

    /// <summary>Gets or initializes the server working directory.</summary>
    public string WorkingDirectory { get; init; } = string.Empty;
}

/// <summary>Describes one retained game case from copied input through expected saved output.</summary>
internal sealed class ClientAcceptanceCase
{
    /// <summary>Gets or initializes the stable human-readable case identifier.</summary>
    public string CaseId { get; init; } = string.Empty;

    /// <summary>Gets or initializes the MCP game token.</summary>
    public string Game { get; init; } = string.Empty;

    /// <summary>Gets or initializes the exact MCP release token.</summary>
    public string Release { get; init; } = string.Empty;

    /// <summary>Gets or initializes the first-run workspace identity.</summary>
    public Guid WorkspaceId { get; init; }

    /// <summary>Gets or initializes the independent post-save workspace identity.</summary>
    public Guid ReopenWorkspaceId { get; init; }

    /// <summary>Gets or initializes the exact first-run workspace-open request.</summary>
    public ClientAcceptanceWorkspaceOpen WorkspaceOpen { get; init; } = new();

    /// <summary>Gets or initializes every copied immutable input artifact and its baseline digest.</summary>
    public ClientAcceptanceArtifact[] Artifacts { get; init; } = [];

    /// <summary>Gets or initializes the exact selected source identity and complete native baseline.</summary>
    public ClientAcceptanceSource Source { get; init; } = new();

    /// <summary>Gets or initializes representative non-FormList targets accepted by the common item commands.</summary>
    public ClientAcceptanceCrossFamilyTargets CrossFamilyTargets { get; init; } = new();

    /// <summary>Gets or initializes the distinct writable output association.</summary>
    public ClientAcceptanceOutput Output { get; init; } = new();

    /// <summary>Gets or initializes the ordered real-client domain-tool plan.</summary>
    public ClientAcceptancePlanStep[] AuthoringPlan { get; init; } = [];

    /// <summary>Gets or initializes the independently checked saved-output expectations.</summary>
    public ClientAcceptanceExpected Expected { get; init; } = new();
}

/// <summary>Captures the exact arguments for the MCP workspace-open tool.</summary>
internal sealed class ClientAcceptanceWorkspaceOpen
{
    /// <summary>Gets or initializes the MCP tool name.</summary>
    public string ToolName { get; init; } = string.Empty;

    /// <summary>Gets or initializes the caller-selected workspace identity.</summary>
    public Guid WorkspaceId { get; init; }

    /// <summary>Gets or initializes the MCP game token.</summary>
    public string Game { get; init; } = string.Empty;

    /// <summary>Gets or initializes the exact MCP release token.</summary>
    public string Release { get; init; } = string.Empty;

    /// <summary>Gets or initializes the copied read-only source plugin path.</summary>
    public string SourcePluginPath { get; init; } = string.Empty;

    /// <summary>Gets or initializes copied plugins in exact native load-order order.</summary>
    public string[] LoadOrderPluginPaths { get; init; } = [];

    /// <summary>Gets or initializes the copied Data directory.</summary>
    public string DataDirectoryPath { get; init; } = string.Empty;

    /// <summary>Gets or initializes explicit copied string directories in lookup order.</summary>
    public string[] StringDirectoryPaths { get; init; } = [];

    /// <summary>Gets or initializes the explicit localized record-text language token.</summary>
    public string RecordTextLanguage { get; init; } = string.Empty;
}

/// <summary>Records one copied immutable input artifact and its exact export baseline.</summary>
internal sealed class ClientAcceptanceArtifact
{
    /// <summary>Gets or initializes the path relative to the acceptance root.</summary>
    public string RelativePath { get; init; } = string.Empty;

    /// <summary>Gets or initializes the artifact role.</summary>
    public string Role { get; init; } = string.Empty;

    /// <summary>Gets or initializes the plugin load-order index, or <see langword="null"/> for a localized string.</summary>
    public int? LoadOrderIndex { get; init; }

    /// <summary>Gets or initializes the exact artifact length in bytes.</summary>
    public long Length { get; init; }

    /// <summary>Gets or initializes the uppercase SHA-256 digest.</summary>
    public string Sha256 { get; init; } = string.Empty;
}

/// <summary>Captures the selected source record identity and complete detached inspector baseline.</summary>
internal sealed class ClientAcceptanceSource
{
    /// <summary>Gets or initializes the selected source plugin identity.</summary>
    public string ModKey { get; init; } = string.Empty;

    /// <summary>Gets or initializes the exact source FormList identity used for the override.</summary>
    public string FormListFormKey { get; init; } = string.Empty;

    /// <summary>Gets or initializes the complete detached native inspector JSON before authoring.</summary>
    public JsonElement Baseline { get; init; }

    /// <summary>Gets or initializes root JSON properties that the requested override is allowed to change.</summary>
    public string[] AllowedChangedProperties { get; init; } = [];
}

/// <summary>Names valid cross-record-family keys used by the common FormList item command.</summary>
internal sealed class ClientAcceptanceCrossFamilyTargets
{
    /// <summary>Gets or initializes the valid Book FormKey.</summary>
    public string BookFormKey { get; init; } = string.Empty;

    /// <summary>Gets or initializes the valid Keyword FormKey.</summary>
    public string KeywordFormKey { get; init; } = string.Empty;
}

/// <summary>Describes the isolated embedded full-master output selected by the client.</summary>
internal sealed class ClientAcceptanceOutput
{
    /// <summary>Gets or initializes the absent writable plugin path.</summary>
    public string PluginPath { get; init; } = string.Empty;

    /// <summary>Gets or initializes the output plugin identity.</summary>
    public string ModKey { get; init; } = string.Empty;

    /// <summary>Gets or initializes the localized output mode token.</summary>
    public string LocalizedOutputMode { get; init; } = string.Empty;

    /// <summary>Gets or initializes the output master-style token.</summary>
    public string MasterStyle { get; init; } = string.Empty;
}

/// <summary>Defines one reusable real-client tool invocation or expected diagnostic probe.</summary>
internal sealed class ClientAcceptancePlanStep
{
    /// <summary>Gets or initializes the stable plan step name.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Gets or initializes the MCP domain-tool name.</summary>
    public string ToolName { get; init; } = string.Empty;

    /// <summary>Gets or initializes immutable static request fields, including any operation identity.</summary>
    public JsonElement StaticArguments { get; init; }

    /// <summary>Gets or initializes dynamic request fields that must be copied from actual prior receipts.</summary>
    public string[] DynamicInputs { get; init; } = [];

    /// <summary>Gets or initializes the required result classification.</summary>
    public string ExpectedResult { get; init; } = string.Empty;

    /// <summary>Gets or initializes the earlier step whose complete request must be replayed byte-for-byte, or <see langword="null"/>.</summary>
    public string? ExactReplayOf { get; init; }
}

/// <summary>Defines the saved output shape that both native and engine reopen paths must observe.</summary>
internal sealed class ClientAcceptanceExpected
{
    /// <summary>Gets or initializes the exact number of FormLists physically present in the output.</summary>
    public int OutputFormListCount { get; init; }

    /// <summary>Gets or initializes the expected newly allocated output FormList.</summary>
    public ClientAcceptanceExpectedRecord NewRecord { get; init; } = new();

    /// <summary>Gets or initializes the expected source override FormList.</summary>
    public ClientAcceptanceExpectedRecord OverrideRecord { get; init; } = new();
}

/// <summary>Defines the independently observable identity and selected values for one saved FormList.</summary>
internal sealed class ClientAcceptanceExpectedRecord
{
    /// <summary>Gets or initializes the exact FormKey, or <see langword="null"/> when the verifier must discover a unique output-owned allocation by EditorID.</summary>
    public string? FormKey { get; init; }

    /// <summary>Gets or initializes the exact EditorID.</summary>
    public string EditorId { get; init; } = string.Empty;

    /// <summary>Gets or initializes the English translated name, or <see langword="null"/> for a game without a FormList name field.</summary>
    public string? Name { get; init; }

    /// <summary>Gets or initializes the exact ordered FormList item identities.</summary>
    public string[] Items { get; init; } = [];
}
