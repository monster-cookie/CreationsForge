using System.Text.Json.Nodes;

namespace CreationsForge.UnitTests.McpHost;

/// <summary>Describes one Keyword version the physical output must contain when a fresh MCP session reopens it.</summary>
internal sealed class ExpectedOutputRecord
{
    /// <summary>Initializes an expectation.</summary>
    /// <param name="label">A short role name, such as <c>created</c>, used in reopen-check problems.</param>
    /// <param name="formKey">The origin FormKey text as the host reports it.</param>
    /// <param name="fields">Registered field values, as host JSON, that must match exactly; other fields are not compared.</param>
    public ExpectedOutputRecord(string label, string formKey, IReadOnlyDictionary<string, JsonNode?> fields)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(label);
        ArgumentException.ThrowIfNullOrWhiteSpace(formKey);
        ArgumentNullException.ThrowIfNull(fields);
        Label = label;
        FormKey = formKey;
        Fields = fields;
    }

    /// <summary>Gets the role name used in reopen-check problems.</summary>
    public string Label { get; }

    /// <summary>Gets the origin FormKey text.</summary>
    public string FormKey { get; }

    /// <summary>Gets the field values that must match exactly.</summary>
    public IReadOnlyDictionary<string, JsonNode?> Fields { get; }

    /// <summary>Returns whether the record originates in the output rather than overriding a master record.</summary>
    /// <param name="outputPlugin">The output plugin file name.</param>
    /// <returns><see langword="true"/> when the FormKey's origin plugin is <paramref name="outputPlugin"/>.</returns>
    public bool OriginatesIn(string outputPlugin)
    {
        return FormKey.EndsWith(":" + outputPlugin, StringComparison.OrdinalIgnoreCase);
    }
}
