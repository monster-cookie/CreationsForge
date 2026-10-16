using System.Text.Json;
using System.Text.Json.Nodes;

namespace CreationsForge.Workbench;

/// <summary>One tool returned by the production host's <c>tools/list</c> response.</summary>
public sealed class WorkbenchMcpTool
{
    private static readonly JsonSerializerOptions IndentedJson = new() { WriteIndented = true };

    /// <summary>Initializes a discovered tool.</summary>
    /// <param name="name">The production tool name.</param>
    /// <param name="description">The host description, or an empty string when the host omitted one.</param>
    /// <param name="inputSchema">The tool input schema. The instance keeps this object and does not copy it again.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is blank.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="inputSchema"/> is <see langword="null"/>.</exception>
    public WorkbenchMcpTool(string name, string description, JsonObject inputSchema)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(inputSchema);
        Name = name;
        Description = description ?? string.Empty;
        InputSchema = inputSchema;
        InputSchemaJson = inputSchema.ToJsonString(IndentedJson);
    }

    /// <summary>Gets the production tool name.</summary>
    public string Name { get; }

    /// <summary>Gets the host description, or an empty string when the host omitted one.</summary>
    public string Description { get; }

    /// <summary>Gets the input schema as indented JSON for the diagnostic view.</summary>
    public string InputSchemaJson { get; }

    /// <summary>Gets the input schema used to decide which session identifiers a call still needs.</summary>
    internal JsonObject InputSchema { get; }
}
