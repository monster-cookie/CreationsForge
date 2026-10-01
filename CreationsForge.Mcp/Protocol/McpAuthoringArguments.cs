using CreationsForge.Engine.Records;
using CreationsForge.Engine.Workspaces;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Mcp.Protocol;

/// <summary>Parses closed authoring arguments before they reach the engine.</summary>
internal static class McpAuthoringArguments
{
    /// <summary>The longest accepted client operation identifier.</summary>
    public const int MaximumOperationIdLength = 128;

    /// <summary>Requires a non-empty operation identifier within the replay limit.</summary>
    /// <param name="operationId">The client operation identifier.</param>
    /// <returns>The unchanged identifier.</returns>
    /// <exception cref="McpContractException">Thrown when the identifier is missing or too long.</exception>
    public static string RequireOperationId(string? operationId)
    {
        if (string.IsNullOrWhiteSpace(operationId) || operationId.Length > MaximumOperationIdLength)
        {
            throw new McpContractException(
                "invalid_input",
                "operationId is required and must be at most 128 characters.");
        }

        return operationId;
    }

    /// <summary>Parses a supported game release.</summary>
    /// <param name="release">The client release name.</param>
    /// <param name="supported">Releases with a registered integration.</param>
    /// <returns>The parsed release.</returns>
    /// <exception cref="McpContractException">Thrown when the release is missing or unsupported.</exception>
    public static GameRelease ParseRelease(string? release, IReadOnlySet<GameRelease> supported)
    {
        if (string.IsNullOrWhiteSpace(release)
            || !TryParseDefined<GameRelease>(release, ignoreCase: false, out var parsed)
            || !supported.Contains(parsed))
        {
            throw new McpContractException("invalid_input", $"Release '{release}' is not supported.");
        }

        return parsed;
    }

    /// <summary>Parses a required non-empty text argument.</summary>
    /// <param name="value">The client text.</param>
    /// <param name="name">The argument name used in the failure.</param>
    /// <returns>The unchanged text.</returns>
    /// <exception cref="McpContractException">Thrown when the text is missing.</exception>
    public static string RequireText(string? value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new McpContractException("invalid_input", $"{name} is required.");
        }

        return value;
    }

    /// <summary>Parses plugin file names without accepting paths.</summary>
    /// <param name="plugins">The selected plugin file names.</param>
    /// <returns>A copy of the validated file names.</returns>
    /// <exception cref="McpContractException">Thrown when a name is missing, duplicated, or a path.</exception>
    public static string[] ParsePlugins(IReadOnlyList<string>? plugins)
    {
        if (plugins is null)
        {
            throw new McpContractException("invalid_input", "selectedPlugins is required.");
        }

        var parsed = new string[plugins.Count];
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        for (var index = 0; index < plugins.Count; index++)
        {
            parsed[index] = ParseModKey(plugins[index]).ToString();
            if (!seen.Add(parsed[index]))
            {
                throw new McpContractException("invalid_input", $"Plugin '{parsed[index]}' is selected more than once.");
            }
        }

        return parsed;
    }

    /// <summary>Parses a plugin file name into a ModKey.</summary>
    /// <param name="text">The plugin file name.</param>
    /// <returns>The parsed ModKey.</returns>
    /// <exception cref="McpContractException">Thrown when the name is missing, a path, or not a ModKey.</exception>
    public static ModKey ParseModKey(string? text)
    {
        if (string.IsNullOrWhiteSpace(text) || text.IndexOfAny(['\\', '/', ':']) >= 0)
        {
            throw new McpContractException("invalid_input", $"Plugin '{text}' must be a file name, not a path.");
        }

        try
        {
            return ModKey.FromNameAndExtension(text);
        }
        catch (ArgumentException exception)
        {
            throw new McpContractException("invalid_input", exception.Message);
        }
    }

    /// <summary>Parses a non-null FormKey.</summary>
    /// <param name="text">The FormKey text.</param>
    /// <returns>The parsed FormKey.</returns>
    /// <exception cref="McpContractException">Thrown when the FormKey is missing or null.</exception>
    public static FormKey ParseFormKey(string? text)
    {
        if (string.IsNullOrWhiteSpace(text) || !FormKey.TryFactory(text.AsSpan(), out var formKey) || formKey.IsNull)
        {
            throw new McpContractException("invalid_input", $"FormKey '{text}' is not valid.");
        }

        return formKey;
    }

    /// <summary>Parses a master style.</summary>
    /// <param name="text">The client style name.</param>
    /// <returns>The parsed style.</returns>
    /// <exception cref="McpContractException">Thrown when the style is not recognized.</exception>
    public static MasterStyle ParseMasterStyle(string? text)
    {
        if (string.IsNullOrWhiteSpace(text) || !TryParseDefined<MasterStyle>(text, ignoreCase: false, out var style))
        {
            throw new McpContractException("invalid_input", $"Master style '{text}' is not recognized.");
        }

        return style;
    }

    /// <summary>Parses a text storage mode.</summary>
    /// <param name="text">The client storage mode.</param>
    /// <returns>The parsed mode.</returns>
    /// <exception cref="McpContractException">Thrown when the mode is not recognized.</exception>
    public static PluginTextStorageMode ParseTextStorage(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)
            || !TryParseDefined<PluginTextStorageMode>(text, ignoreCase: false, out var mode))
        {
            throw new McpContractException("invalid_input", $"Text storage mode '{text}' is not recognized.");
        }

        return mode;
    }

    /// <summary>Parses an optional language, defaulting to English.</summary>
    /// <param name="text">The client language, or <see langword="null"/> for English.</param>
    /// <returns>The parsed language.</returns>
    /// <exception cref="McpContractException">Thrown when the language is not recognized.</exception>
    public static Language ParseLanguage(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return Language.English;
        }

        if (!TryParseDefined<Language>(text, ignoreCase: false, out var language))
        {
            throw new McpContractException("invalid_input", $"Language '{text}' is not recognized.");
        }

        return language;
    }

    /// <summary>Requires an explicit expected revision.</summary>
    /// <param name="expectedRevision">The client revision.</param>
    /// <returns>The revision.</returns>
    /// <exception cref="McpContractException">Thrown when the revision was omitted.</exception>
    public static ulong RequireRevision(ulong? expectedRevision)
    {
        if (expectedRevision is null)
        {
            throw new McpContractException("invalid_input", "expectedRevision is required.");
        }

        return expectedRevision.Value;
    }

    /// <summary>Converts transport field changes into engine changes.</summary>
    /// <param name="changes">The client changes.</param>
    /// <returns>The engine changes.</returns>
    /// <exception cref="McpContractException">Thrown when a change is incomplete or uses an unknown operation.</exception>
    public static IReadOnlyList<RecordFieldChange> ParseChanges(IReadOnlyList<McpFieldChangeDto>? changes)
    {
        if (changes is null)
        {
            throw new McpContractException("invalid_input", "Field changes are required.");
        }

        var parsed = new List<RecordFieldChange>(changes.Count);
        foreach (var change in changes)
        {
            if (change is null || string.IsNullOrWhiteSpace(change.Path))
            {
                throw new McpContractException("invalid_input", "Each field change requires a path.");
            }

            if (!TryParseDefined<RecordCollectionOperation>(change.Operation, ignoreCase: false, out var operation))
            {
                throw new McpContractException("invalid_input", $"Field operation '{change.Operation}' is not recognized.");
            }

            parsed.Add(new RecordFieldChange(
                change.Path,
                operation,
                change.Value is null ? null : McpValueMapper.ToRecordValue(change.Value),
                change.Index));
        }

        return parsed;
    }

    /// <summary>Converts one transport mutation into an engine mutation.</summary>
    /// <param name="mutation">The client mutation.</param>
    /// <returns>The engine mutation.</returns>
    /// <exception cref="McpContractException">Thrown when the kind or identity is not legal.</exception>
    public static RecordMutation ParseMutation(McpMutationDto? mutation)
    {
        if (mutation is null)
        {
            throw new McpContractException("invalid_input", "A mutation is required.");
        }

        var changes = ParseChanges(mutation.Changes);
        if (string.Equals(mutation.Kind, nameof(RecordMutationKind.Create), StringComparison.Ordinal))
        {
            if (!string.IsNullOrWhiteSpace(mutation.FormKey) || !string.IsNullOrWhiteSpace(mutation.ContainingModKey))
            {
                throw new McpContractException(
                    "invalid_input",
                    "A create mutation cannot include a FormKey or containing plugin.");
            }

            return RecordMutation.Create(RequireText(mutation.FamilyId, "familyId"), changes);
        }

        if (string.Equals(mutation.Kind, nameof(RecordMutationKind.Override), StringComparison.Ordinal))
        {
            return RecordMutation.Override(
                RequireText(mutation.FamilyId, "familyId"),
                ParseFormKey(mutation.FormKey),
                ParseModKey(mutation.ContainingModKey),
                changes);
        }

        throw new McpContractException("invalid_input", $"Mutation kind '{mutation.Kind}' is not recognized.");
    }

    /// <summary>Decodes a cursor of one expected kind.</summary>
    /// <param name="cursor">The client cursor, if any.</param>
    /// <param name="expectedKind">The cursor kind this request accepts.</param>
    /// <returns>The payload, or <see langword="null"/> when no cursor was supplied.</returns>
    /// <exception cref="McpContractException">Thrown when the cursor is present but not valid for the request.</exception>
    public static McpCursorPayload? DecodeCursor(string? cursor, string expectedKind)
    {
        if (string.IsNullOrWhiteSpace(cursor))
        {
            return null;
        }

        if (!McpCursors.TryDecode(cursor, out var payload)
            || payload is null
            || !string.Equals(payload.Kind, expectedKind, StringComparison.Ordinal)
            || payload.Skip < 0)
        {
            throw new McpContractException("invalid_input", "The cursor is not valid for this request.");
        }

        return payload;
    }

    /// <summary>Decodes a cursor of one of the expected kinds.</summary>
    /// <param name="cursor">The client cursor, if any.</param>
    /// <param name="expectedKinds">The cursor kinds this request accepts.</param>
    /// <returns>The payload, or <see langword="null"/> when no cursor was supplied.</returns>
    /// <exception cref="McpContractException">Thrown when the cursor is present but not valid for the request.</exception>
    public static McpCursorPayload? DecodeCursor(string? cursor, params string[] expectedKinds)
    {
        ArgumentNullException.ThrowIfNull(expectedKinds);
        if (string.IsNullOrWhiteSpace(cursor))
        {
            return null;
        }

        if (!McpCursors.TryDecode(cursor, out var payload)
            || payload is null
            || payload.Skip < 0
            || !expectedKinds.Contains(payload.Kind, StringComparer.Ordinal))
        {
            throw new McpContractException("invalid_input", "The cursor is not valid for this request.");
        }

        return payload;
    }

    /// <summary>Parses a named enum member and rejects undefined numeric values.</summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <param name="text">The member name.</param>
    /// <param name="ignoreCase">Whether member names are matched without case.</param>
    /// <param name="value">The defined member when parsing succeeds.</param>
    /// <returns><see langword="true"/> when <paramref name="text"/> names a defined member.</returns>
    public static bool TryParseDefined<TEnum>(string? text, bool ignoreCase, out TEnum value)
        where TEnum : struct, Enum
    {
        if (Enum.TryParse(text, ignoreCase, out value) && Enum.IsDefined(value))
        {
            return true;
        }

        value = default;
        return false;
    }

    /// <summary>Requires a workspace cursor to name the same session and revision.</summary>
    /// <param name="payload">The decoded cursor.</param>
    /// <param name="workspaceId">The requested session.</param>
    /// <param name="revision">The live workspace revision.</param>
    /// <exception cref="McpContractException">Thrown when the cursor belongs to another session or revision.</exception>
    public static void BindWorkspace(McpCursorPayload payload, string workspaceId, ulong revision)
    {
        ArgumentNullException.ThrowIfNull(payload);
        if (!string.Equals(payload.WorkspaceId, workspaceId, StringComparison.Ordinal))
        {
            throw new McpContractException("invalid_input", "The cursor workspace does not match this request.");
        }

        if (payload.Revision is null)
        {
            throw new McpContractException("invalid_input", "The cursor is missing a workspace revision.");
        }

        if (payload.Revision != revision)
        {
            throw new McpContractException(
                "stale_revision",
                $"Workspace revision is '{revision}', not cursor revision '{payload.Revision}'.");
        }
    }

    /// <summary>Requires a cursor to name this session and returns its revision without reading live state.</summary>
    /// <param name="payload">The decoded cursor.</param>
    /// <param name="workspaceId">The requested session.</param>
    /// <returns>The revision named by the cursor.</returns>
    /// <exception cref="McpContractException">Thrown when the cursor belongs to another session or has no revision.</exception>
    public static ulong RequireCursorIdentity(McpCursorPayload payload, string workspaceId)
    {
        ArgumentNullException.ThrowIfNull(payload);
        if (!string.Equals(payload.WorkspaceId, workspaceId, StringComparison.Ordinal))
        {
            throw new McpContractException("invalid_input", "The cursor workspace does not match this request.");
        }

        if (payload.Revision is null)
        {
            throw new McpContractException("invalid_input", "The cursor is missing a workspace revision.");
        }

        return payload.Revision.Value;
    }

    /// <summary>Requires two cursor identities to match exactly.</summary>
    /// <param name="actual">The cursor value.</param>
    /// <param name="expected">The request value.</param>
    /// <param name="name">The identity name used in the failure.</param>
    /// <exception cref="McpContractException">Thrown when the values differ.</exception>
    public static void Match(string? actual, string? expected, string name)
    {
        if (!string.Equals(actual ?? string.Empty, expected ?? string.Empty, StringComparison.Ordinal))
        {
            throw new McpContractException("invalid_input", $"The cursor {name} does not match this request.");
        }
    }
}
