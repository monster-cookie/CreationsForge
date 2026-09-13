using System.Globalization;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Mcp;

/// <summary>
/// Parses closed MCP argument objects into exact native engine request values.
/// </summary>
internal static class McpInput
{
    /// <summary>The only properties accepted by an exact revision object.</summary>
    private static readonly IReadOnlySet<string> RevisionArgumentNames = new HashSet<string>(StringComparer.Ordinal) { "baselineId", "sequence" };

    /// <summary>The only properties accepted by a nested native reference selection.</summary>
    private static readonly IReadOnlySet<string> ReferenceArgumentNames = new HashSet<string>(StringComparer.Ordinal) { "formKey", "scope", "containingModKey" };

    /// <summary>The largest accepted path or JSON Pointer string at the MCP boundary.</summary>
    internal const int MaximumPathLength = 32767;

    /// <summary>The largest accepted JSON Pointer so its complete cursor remains within the decode bound.</summary>
    internal const int MaximumJsonPointerLength = 4096;

    /// <summary>The largest accepted continuation token at the MCP boundary.</summary>
    internal const int MaximumCursorLength = 16384;

    /// <summary>The default number of bounded transport results.</summary>
    internal const int DefaultMaximumResults = 50;

    /// <summary>The largest number of bounded transport results.</summary>
    internal const int MaximumResults = 250;

    /// <summary>Reads a required native localized-record language name.</summary>
    /// <param name="arguments">The request arguments.</param>
    /// <param name="name">The required property name.</param>
    /// <param name="value">Receives the defined Mutagen language.</param>
    /// <param name="error">Receives a validation error.</param>
    /// <returns><see langword="true"/> when the value names a defined language, ignoring case.</returns>
    internal static bool TryGetRequiredLanguage(
        IReadOnlyDictionary<string, JsonElement> arguments,
        string name,
        out Language value,
        out string error)
    {
        value = default;
        if (!TryGetRequiredString(arguments, name, 64, out var text, out error)
            || !Enum.TryParse(text, true, out value)
            || !Enum.IsDefined(value))
        {
            error = $"Argument '{name}' must name a supported record-text language.";
            return false;
        }

        return true;
    }

    /// <summary>Reads and validates the complete closed argument object.</summary>
    /// <param name="request">The MCP request whose arguments are inspected.</param>
    /// <param name="allowedNames">Every property accepted by the tool.</param>
    /// <param name="arguments">Receives the supplied argument dictionary.</param>
    /// <param name="error">Receives a stable human-readable validation error.</param>
    /// <returns><see langword="true"/> when no undeclared argument is present.</returns>
    internal static bool TryGetArguments(
        RequestContext<CallToolRequestParams> request,
        IReadOnlySet<string> allowedNames,
        out IReadOnlyDictionary<string, JsonElement> arguments,
        out string error)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(allowedNames);
        arguments = request.Params?.Arguments is { } suppliedArguments
            ? new Dictionary<string, JsonElement>(suppliedArguments, StringComparer.Ordinal)
            : new Dictionary<string, JsonElement>(StringComparer.Ordinal);
        if (arguments.Keys.Any(name => !IsWellFormedUtf16(name)))
        {
            error = "The request contains an argument name with invalid Unicode string data.";
            return false;
        }

        var unknownName = arguments.Keys.FirstOrDefault(name => !allowedNames.Contains(name));
        if (unknownName is not null)
        {
            error = "The request contains an undeclared argument.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    /// <summary>Reads a required canonical UUID string.</summary>
    /// <param name="arguments">The request arguments.</param>
    /// <param name="name">The required property name.</param>
    /// <param name="value">Receives the parsed UUID.</param>
    /// <param name="error">Receives a validation error.</param>
    /// <returns><see langword="true"/> when the property is a canonical lowercase UUID.</returns>
    internal static bool TryGetRequiredGuid(
        IReadOnlyDictionary<string, JsonElement> arguments,
        string name,
        out Guid value,
        out string error)
    {
        value = Guid.Empty;
        if (!TryGetRequiredString(arguments, name, 36, out var text, out error) ||
            !Guid.TryParseExact(text, "D", out value) ||
            value == Guid.Empty ||
            !string.Equals(value.ToString("D"), text, StringComparison.Ordinal))
        {
            error = $"Argument '{name}' must be a canonical lowercase UUID string.";
            return false;
        }

        return true;
    }

    /// <summary>Reads a required non-empty string with an explicit character bound.</summary>
    /// <param name="arguments">The request arguments.</param>
    /// <param name="name">The required property name.</param>
    /// <param name="maximumLength">The largest accepted character count.</param>
    /// <param name="value">Receives the exact string.</param>
    /// <param name="error">Receives a validation error.</param>
    /// <returns><see langword="true"/> when the property is a bounded non-empty string.</returns>
    internal static bool TryGetRequiredString(
        IReadOnlyDictionary<string, JsonElement> arguments,
        string name,
        int maximumLength,
        out string value,
        out string error)
    {
        value = string.Empty;
        if (!arguments.TryGetValue(name, out var element) || element.ValueKind != JsonValueKind.String)
        {
            error = $"Argument '{name}' is required and must be a string.";
            return false;
        }

        try
        {
            value = element.GetString() ?? string.Empty;
        }
        catch (InvalidOperationException)
        {
            error = $"Argument '{name}' contains invalid Unicode string data.";
            return false;
        }
        if (!IsWellFormedUtf16(value))
        {
            error = $"Argument '{name}' contains invalid Unicode string data.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(value) || value.Length > maximumLength)
        {
            error = $"Argument '{name}' must contain between 1 and {maximumLength} characters.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    /// <summary>Reads an optional bounded string without trimming or otherwise changing it.</summary>
    /// <param name="arguments">The request arguments.</param>
    /// <param name="name">The optional property name.</param>
    /// <param name="maximumLength">The largest accepted character count.</param>
    /// <param name="defaultValue">The value used when the property is absent.</param>
    /// <param name="value">Receives the supplied or default string.</param>
    /// <param name="error">Receives a validation error.</param>
    /// <returns><see langword="true"/> when the optional property is valid.</returns>
    internal static bool TryGetOptionalString(
        IReadOnlyDictionary<string, JsonElement> arguments,
        string name,
        int maximumLength,
        string? defaultValue,
        out string? value,
        out string error)
    {
        value = defaultValue;
        if (!arguments.TryGetValue(name, out var element))
        {
            error = string.Empty;
            return true;
        }

        if (element.ValueKind != JsonValueKind.String)
        {
            error = $"Argument '{name}' must be a string when supplied.";
            return false;
        }

        try
        {
            value = element.GetString() ?? string.Empty;
        }
        catch (InvalidOperationException)
        {
            error = $"Argument '{name}' contains invalid Unicode string data.";
            return false;
        }
        if (!IsWellFormedUtf16(value))
        {
            error = $"Argument '{name}' contains invalid Unicode string data.";
            return false;
        }

        if (value.Length > maximumLength)
        {
            error = $"Argument '{name}' cannot exceed {maximumLength} characters.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    /// <summary>Reads a required array of bounded non-empty strings while preserving order and duplicates.</summary>
    /// <param name="arguments">The request arguments.</param>
    /// <param name="name">The required property name.</param>
    /// <param name="maximumItems">The largest accepted item count.</param>
    /// <param name="values">Receives the immutable string snapshot.</param>
    /// <param name="error">Receives a validation error.</param>
    /// <returns><see langword="true"/> when every array entry is a bounded non-empty string.</returns>
    internal static bool TryGetRequiredStringArray(
        IReadOnlyDictionary<string, JsonElement> arguments,
        string name,
        int maximumItems,
        out IReadOnlyList<string> values,
        out string error)
    {
        values = Array.Empty<string>();
        if (!arguments.TryGetValue(name, out var element) || element.ValueKind != JsonValueKind.Array)
        {
            error = $"Argument '{name}' is required and must be an array of strings.";
            return false;
        }

        var items = element.EnumerateArray().ToArray();
        string?[] materialized;
        try
        {
            materialized = items.Select(item => item.ValueKind == JsonValueKind.String ? item.GetString() : null).ToArray();
        }
        catch (InvalidOperationException)
        {
            error = $"Argument '{name}' contains invalid Unicode string data.";
            return false;
        }

        if (items.Length > maximumItems || materialized.Any(item =>
                string.IsNullOrWhiteSpace(item) || !IsWellFormedUtf16(item) || item.Length > MaximumPathLength))
        {
            error = $"Argument '{name}' must contain at most {maximumItems} non-empty path strings, each no longer than {MaximumPathLength} characters.";
            return false;
        }

        values = Array.AsReadOnly(materialized.Select(item => item!).ToArray());
        error = string.Empty;
        return true;
    }

    /// <summary>Reads an optional bounded positive page size.</summary>
    /// <param name="arguments">The request arguments.</param>
    /// <param name="value">Receives the requested or default page size.</param>
    /// <param name="error">Receives a validation error.</param>
    /// <returns><see langword="true"/> when the page size is between one and 250.</returns>
    internal static bool TryGetMaximumResults(
        IReadOnlyDictionary<string, JsonElement> arguments,
        out int value,
        out string error)
    {
        value = DefaultMaximumResults;
        if (!arguments.TryGetValue("maxResults", out var element))
        {
            error = string.Empty;
            return true;
        }

        if (element.ValueKind != JsonValueKind.Number ||
            !element.TryGetInt32(out value) ||
            value <= 0 ||
            value > MaximumResults)
        {
            error = $"Argument 'maxResults' must be an integer between 1 and {MaximumResults}.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    /// <summary>Reads a canonical FormKey string without accepting alternate spellings.</summary>
    /// <param name="arguments">The request arguments.</param>
    /// <param name="name">The required property name.</param>
    /// <param name="value">Receives the parsed native FormKey.</param>
    /// <param name="error">Receives a validation error.</param>
    /// <returns><see langword="true"/> when the value round-trips through Mutagen's canonical form.</returns>
    internal static bool TryGetFormKey(
        IReadOnlyDictionary<string, JsonElement> arguments,
        string name,
        out FormKey value,
        out string error)
    {
        value = default;
        if (!TryGetRequiredString(arguments, name, 1024, out var text, out error) ||
            !FormKey.TryFactory(text.AsSpan(), out value) ||
            !string.Equals(value.ToString(), text, StringComparison.Ordinal))
        {
            error = $"Argument '{name}' must be a canonical Mutagen FormKey string.";
            return false;
        }

        return true;
    }

    /// <summary>Reads an optional canonical ModKey string.</summary>
    /// <param name="arguments">The request arguments.</param>
    /// <param name="name">The optional property name.</param>
    /// <param name="value">Receives the parsed ModKey or <see langword="null"/>.</param>
    /// <param name="error">Receives a validation error.</param>
    /// <returns><see langword="true"/> when the value is absent or round-trips through Mutagen's canonical form.</returns>
    internal static bool TryGetOptionalModKey(
        IReadOnlyDictionary<string, JsonElement> arguments,
        string name,
        out ModKey? value,
        out string error)
    {
        value = null;
        if (!arguments.TryGetValue(name, out var element))
        {
            error = string.Empty;
            return true;
        }

        if (element.ValueKind != JsonValueKind.String)
        {
            error = $"Argument '{name}' must be a canonical Mutagen ModKey string when supplied.";
            return false;
        }

        string text;
        try
        {
            text = element.GetString() ?? string.Empty;
        }
        catch (InvalidOperationException)
        {
            error = $"Argument '{name}' contains invalid Unicode string data.";
            return false;
        }
        if (!ModKey.TryFromNameAndExtension(text, out var parsed, out _) ||
            !string.Equals(parsed.ToString(), text, StringComparison.Ordinal))
        {
            error = $"Argument '{name}' must be a canonical Mutagen ModKey string when supplied.";
            return false;
        }

        value = parsed;
        error = string.Empty;
        return true;
    }

    /// <summary>Reads one exact closed record-scope name.</summary>
    /// <param name="arguments">The request arguments.</param>
    /// <param name="name">The required property name.</param>
    /// <param name="value">Receives the engine scope.</param>
    /// <param name="error">Receives a validation error.</param>
    /// <returns><see langword="true"/> when the scope is recognized.</returns>
    internal static bool TryGetScope(
        IReadOnlyDictionary<string, JsonElement> arguments,
        string name,
        out RecordScope value,
        out string error)
    {
        value = default;
        if (!TryGetRequiredString(arguments, name, 32, out var text, out error))
        {
            return false;
        }

        value = text switch
        {
            "winning_overrides" => RecordScope.WinningOverrides,
            "all_contexts" => RecordScope.AllContexts,
            "source" => RecordScope.Source,
            "staged_output" => RecordScope.StagedOutput,
            _ => (RecordScope)(-1),
        };
        if (!Enum.IsDefined(value))
        {
            error = $"Argument '{name}' must be one of winning_overrides, all_contexts, source, or staged_output.";
            return false;
        }

        return true;
    }

    /// <summary>Reads an optional exact workspace revision whose sequence is a decimal string.</summary>
    /// <param name="arguments">The request arguments.</param>
    /// <param name="value">Receives the revision or <see langword="null"/>.</param>
    /// <param name="error">Receives a validation error.</param>
    /// <returns><see langword="true"/> when the revision object is closed and canonical.</returns>
    internal static bool TryGetOptionalRevision(
        IReadOnlyDictionary<string, JsonElement> arguments,
        out WorkspaceRevision? value,
        out string error)
    {
        value = null;
        if (!arguments.TryGetValue("expectedRevision", out var element))
        {
            error = string.Empty;
            return true;
        }

        if (!TryGetClosedObject(element, "expectedRevision", RevisionArgumentNames, out var revisionArguments, out error))
        {
            return false;
        }

        if (!TryGetRequiredString(revisionArguments, "baselineId", 36, out var baselineText, out error) ||
            !Guid.TryParseExact(baselineText, "D", out var baselineId) ||
            baselineId == Guid.Empty ||
            !string.Equals(baselineId.ToString("D"), baselineText, StringComparison.Ordinal) ||
            !TryGetRequiredString(revisionArguments, "sequence", 20, out var sequenceText, out error) ||
            !ulong.TryParse(sequenceText, NumberStyles.None, CultureInfo.InvariantCulture, out var sequence) ||
            !string.Equals(sequence.ToString(CultureInfo.InvariantCulture), sequenceText, StringComparison.Ordinal))
        {
            error = "Argument 'expectedRevision' must be a closed object with canonical baselineId and unsigned decimal-string sequence values.";
            return false;
        }

        value = new WorkspaceRevision(baselineId, sequence);
        error = string.Empty;
        return true;
    }

    /// <summary>Reads a required exact workspace revision whose sequence is a decimal string.</summary>
    /// <param name="arguments">The request arguments.</param>
    /// <param name="value">Receives the required revision.</param>
    /// <param name="error">Receives a validation error.</param>
    /// <returns><see langword="true"/> when the required revision is present, closed, and canonical.</returns>
    internal static bool TryGetRequiredRevision(
        IReadOnlyDictionary<string, JsonElement> arguments,
        out WorkspaceRevision value,
        out string error)
    {
        value = default;
        if (!arguments.ContainsKey("expectedRevision"))
        {
            error = "Argument 'expectedRevision' is required.";
            return false;
        }

        if (!TryGetOptionalRevision(arguments, out var parsed, out error) || !parsed.HasValue)
        {
            return false;
        }

        value = parsed.Value;
        return true;
    }

    /// <summary>Maps one closed game and release pair to the exact engine enums.</summary>
    /// <param name="arguments">The request arguments.</param>
    /// <param name="game">Receives the supported game.</param>
    /// <param name="release">Receives the Mutagen release.</param>
    /// <param name="error">Receives a validation error.</param>
    /// <returns><see langword="true"/> when the pair is one of the three supported native releases.</returns>
    internal static bool TryGetGameRelease(
        IReadOnlyDictionary<string, JsonElement> arguments,
        out SupportedGame game,
        out GameRelease release,
        out string error)
    {
        game = default;
        release = default;
        if (!TryGetRequiredString(arguments, "game", 32, out var gameText, out error) ||
            !TryGetRequiredString(arguments, "release", 32, out var releaseText, out error))
        {
            return false;
        }

        var supported = (gameText, releaseText) switch
        {
            ("starfield", "starfield") => (SupportedGame.Starfield, GameRelease.Starfield, true),
            ("fallout4", "fallout4") => (SupportedGame.Fallout4, GameRelease.Fallout4, true),
            ("skyrim", "skyrim_se") => (SupportedGame.Skyrim, GameRelease.SkyrimSE, true),
            _ => (default, default, false),
        };
        game = supported.Item1;
        release = supported.Item2;
        if (!supported.Item3)
        {
            error = "Arguments 'game' and 'release' must be a supported pair: starfield/starfield, fallout4/fallout4, or skyrim/skyrim_se.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    /// <summary>Creates an exact engine reference selection from canonical MCP arguments.</summary>
    /// <param name="arguments">The request arguments.</param>
    /// <param name="prefix">An optional property prefix such as <c>before</c> or <c>after</c>.</param>
    /// <param name="request">Receives the engine selection.</param>
    /// <param name="error">Receives a validation error.</param>
    /// <returns><see langword="true"/> when the selection is valid.</returns>
    internal static bool TryGetReferenceRequest(
        IReadOnlyDictionary<string, JsonElement> arguments,
        string prefix,
        out ReferenceRequest request,
        out string error)
    {
        request = null!;
        var formKeyName = string.IsNullOrEmpty(prefix) ? "formKey" : $"{prefix}FormKey";
        var scopeName = string.IsNullOrEmpty(prefix) ? "scope" : $"{prefix}Scope";
        var containingName = string.IsNullOrEmpty(prefix) ? "containingModKey" : $"{prefix}ContainingModKey";
        if (!TryGetFormKey(arguments, formKeyName, out var formKey, out error) ||
            !TryGetScope(arguments, scopeName, out var scope, out error) ||
            !TryGetOptionalModKey(arguments, containingName, out var containingModKey, out error))
        {
            return false;
        }

        try
        {
            request = new ReferenceRequest(formKey, scope, containingModKey);
            return true;
        }
        catch (ArgumentException exception)
        {
            error = exception.Message;
            return false;
        }
    }

    /// <summary>Creates an exact engine reference selection from a closed nested MCP object.</summary>
    /// <param name="arguments">The request arguments.</param>
    /// <param name="name">The required nested selection property.</param>
    /// <param name="request">Receives the engine selection.</param>
    /// <param name="error">Receives a validation error.</param>
    /// <returns><see langword="true"/> when the nested object contains only formKey, scope, and optional containingModKey.</returns>
    internal static bool TryGetNestedReferenceRequest(
        IReadOnlyDictionary<string, JsonElement> arguments,
        string name,
        out ReferenceRequest request,
        out string error)
    {
        request = null!;
        if (!arguments.TryGetValue(name, out var element) || element.ValueKind != JsonValueKind.Object)
        {
            error = $"Argument '{name}' is required and must be a source-selection object.";
            return false;
        }

        return TryGetClosedObject(element, name, ReferenceArgumentNames, out var nested, out error) &&
            TryGetReferenceRequest(nested, string.Empty, out request, out error);
    }

    /// <summary>Materializes a JSON object only after validating every lazy property name and its closed shape.</summary>
    /// <param name="element">The JSON value expected to be an object.</param>
    /// <param name="argumentName">The enclosing argument name used in diagnostics.</param>
    /// <param name="allowedNames">The exact allowed property names.</param>
    /// <param name="properties">Receives the safely materialized property dictionary.</param>
    /// <param name="error">Receives a shape or invalid-Unicode error.</param>
    /// <returns><see langword="true"/> when the object is closed, duplicate-free, and safely materialized.</returns>
    internal static bool TryGetClosedObject(
        JsonElement element,
        string argumentName,
        IReadOnlySet<string> allowedNames,
        out IReadOnlyDictionary<string, JsonElement> properties,
        out string error)
    {
        properties = new Dictionary<string, JsonElement>(StringComparer.Ordinal);
        if (element.ValueKind != JsonValueKind.Object)
        {
            error = $"Argument '{argumentName}' is required and must be an object.";
            return false;
        }

        try
        {
            var values = new Dictionary<string, JsonElement>(StringComparer.Ordinal);
            foreach (var property in element.EnumerateObject())
            {
                var name = property.Name;
                if (!IsWellFormedUtf16(name))
                {
                    error = $"Argument '{argumentName}' contains a property name with invalid Unicode string data.";
                    return false;
                }

                if (!allowedNames.Contains(name))
                {
                    error = $"Argument '{argumentName}' contains an undeclared property.";
                    return false;
                }

                if (!values.TryAdd(name, property.Value))
                {
                    error = $"Argument '{argumentName}' contains a duplicate property.";
                    return false;
                }
            }

            properties = values;
            error = string.Empty;
            return true;
        }
        catch (InvalidOperationException)
        {
            error = $"Argument '{argumentName}' contains a property name with invalid Unicode string data.";
            return false;
        }
    }

    /// <summary>Checks that every UTF-16 surrogate participates in one valid pair.</summary>
    /// <param name="value">The materialized JSON string.</param>
    /// <returns><see langword="true"/> when the string contains only complete Unicode scalar encodings.</returns>
    private static bool IsWellFormedUtf16(string value)
    {
        for (var index = 0; index < value.Length; index++)
        {
            if (char.IsHighSurrogate(value[index]))
            {
                if (++index >= value.Length || !char.IsLowSurrogate(value[index]))
                {
                    return false;
                }
            }
            else if (char.IsLowSurrogate(value[index]))
            {
                return false;
            }
        }

        return true;
    }
}
