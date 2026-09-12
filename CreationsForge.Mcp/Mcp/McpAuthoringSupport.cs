using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Mcp;

/// <summary>Shares closed authoring input parsing and projections across the MCP mutation tools.</summary>
internal static class McpAuthoringSupport
{
    /// <summary>The only properties accepted by a native output association.</summary>
    private static readonly IReadOnlySet<string> OutputArgumentNames = new HashSet<string>(StringComparer.Ordinal) { "pluginPath", "modKey", "localizedOutputMode", "masterStyle" };

    /// <summary>The largest accepted serialized command payload in UTF-8 bytes.</summary>
    internal const int MaximumArgumentsJsonBytes = 16 * 1024 * 1024;

    /// <summary>Reads a closed output association object without installed-game discovery.</summary>
    /// <param name="arguments">The request arguments.</param>
    /// <param name="association">Receives the exact output association.</param>
    /// <param name="error">Receives a validation error.</param>
    /// <returns><see langword="true"/> when the association is complete and canonical.</returns>
    internal static bool TryGetOutputAssociation(
        IReadOnlyDictionary<string, JsonElement> arguments,
        out OutputAssociation association,
        out string error)
    {
        association = null!;
        if (!arguments.TryGetValue("output", out var element) || element.ValueKind != JsonValueKind.Object)
        {
            error = "Argument 'output' is required and must be an object.";
            return false;
        }

        if (!McpInput.TryGetClosedObject(element, "output", OutputArgumentNames, out var nested, out error)) return false;
        if (!McpInput.TryGetRequiredString(nested, "pluginPath", McpInput.MaximumPathLength, out var pluginPath, out error) ||
            !McpInput.TryGetRequiredString(nested, "modKey", 1024, out var modKeyText, out error) ||
            !ModKey.TryFromNameAndExtension(modKeyText, out var modKey, out _) ||
            !string.Equals(modKey.ToString(), modKeyText, StringComparison.Ordinal) ||
            !McpInput.TryGetRequiredString(nested, "localizedOutputMode", 32, out var localizedText, out error) ||
            !McpInput.TryGetRequiredString(nested, "masterStyle", 16, out var masterText, out error))
        {
            error = string.IsNullOrEmpty(error) ? "Argument 'output.modKey' must be a canonical Mutagen ModKey string." : error;
            return false;
        }

        var localized = localizedText switch
        {
            "embedded" => LocalizedOutputMode.Embedded,
            "separate_string_files" => LocalizedOutputMode.SeparateStringFiles,
            _ => (LocalizedOutputMode)(-1),
        };
        var master = masterText switch
        {
            "full" => OutputMasterStyle.Full,
            "small" => OutputMasterStyle.Small,
            "medium" => OutputMasterStyle.Medium,
            _ => (OutputMasterStyle)(-1),
        };
        if (!Enum.IsDefined(localized) || !Enum.IsDefined(master))
        {
            error = "Output modes must use embedded or separate_string_files and full, small, or medium.";
            return false;
        }

        try
        {
            association = new OutputAssociation(pluginPath, modKey, localized, master);
            error = string.Empty;
            return true;
        }
        catch (ArgumentException exception)
        {
            error = exception.Message;
            return false;
        }
    }

    /// <summary>Parses a bounded JSON command document and returns a detached root value.</summary>
    /// <param name="json">The serialized command arguments.</param>
    /// <param name="value">Receives the detached parsed root.</param>
    /// <param name="error">Receives a syntax or resource-limit error.</param>
    /// <returns><see langword="true"/> when the payload is valid bounded JSON.</returns>
    internal static bool TryParseArgumentsJson(string json, out JsonElement value, out string error)
    {
        value = default;
        if (json.Length > MaximumArgumentsJsonBytes || System.Text.Encoding.UTF8.GetByteCount(json) > MaximumArgumentsJsonBytes)
        {
            error = $"Argument 'argumentsJson' cannot exceed {MaximumArgumentsJsonBytes} UTF-8 bytes.";
            return false;
        }

        try
        {
            using var document = JsonDocument.Parse(json, new JsonDocumentOptions { MaxDepth = CreationsForge.Core.Engine.NativeWire.NativeWireReadLimits.DefaultMaximumDepth });
            value = document.RootElement.Clone();
            error = string.Empty;
            return true;
        }
        catch (JsonException exception)
        {
            error = $"Argument 'argumentsJson' is not valid bounded JSON: {exception.Message}";
            return false;
        }
    }

    /// <summary>Projects one output metadata reference without exposing retained paths or artifact details.</summary>
    /// <param name="reference">The retained exact metadata reference.</param>
    /// <returns>A closed handle projection.</returns>
    internal static object MetadataReference(McpMetadataReference reference)
    {
        ArgumentNullException.ThrowIfNull(reference);
        return new
        {
            handle = reference.Handle,
            kind = reference.Kind switch
            {
                McpMetadataKind.OutputAssociation => "output_association",
                McpMetadataKind.OutputBaseline => "output_baseline",
                _ => throw new ArgumentOutOfRangeException(nameof(reference)),
            },
            baselineId = reference.BaselineId?.ToString("D"),
        };
    }

    /// <summary>Maps an edit role to its stable protocol name.</summary>
    /// <param name="role">The Core role.</param>
    /// <returns>The lower-snake-case role.</returns>
    internal static string EditRole(FormListEditRole role)
    {
        return role switch
        {
            FormListEditRole.New => "new",
            FormListEditRole.Override => "override",
            FormListEditRole.ExistingOutput => "existing_output",
            _ => throw new ArgumentOutOfRangeException(nameof(role)),
        };
    }

    /// <summary>Maps a supported game to its stable protocol name.</summary>
    /// <param name="game">The supported game.</param>
    /// <returns>The lower-case game name.</returns>
    internal static string Game(SupportedGame game) => game switch
    {
        SupportedGame.Starfield => "starfield",
        SupportedGame.Fallout4 => "fallout4",
        SupportedGame.Skyrim => "skyrim",
        _ => throw new ArgumentOutOfRangeException(nameof(game)),
    };

    /// <summary>Maps an exact release to its stable protocol name.</summary>
    /// <param name="release">The native release.</param>
    /// <returns>The lower-case release name.</returns>
    internal static string Release(GameRelease release) => release switch
    {
        GameRelease.Starfield => "starfield",
        GameRelease.Fallout4 => "fallout4",
        GameRelease.SkyrimSE => "skyrim_se",
        _ => throw new ArgumentOutOfRangeException(nameof(release)),
    };

    /// <summary>Projects a failed atomic workspace-state read into the result type expected by a guarded mutation callback.</summary>
    /// <typeparam name="T">The mutation result type.</typeparam>
    /// <param name="stateResult">The failed state read.</param>
    /// <returns>A failure preserving all available engine context.</returns>
    internal static EngineResult<T> StateFailure<T>(EngineResult<WorkspaceState> stateResult)
    {
        ArgumentNullException.ThrowIfNull(stateResult);
        return EngineResult<T>.Failure(
            stateResult.Error ?? new EngineError(EngineErrorCode.UnexpectedFailure, "The engine returned a failed workspace-state read without an error."),
            stateResult.WorkspaceId,
            stateResult.OperationId,
            stateResult.BaseRevision,
            stateResult.ResultRevision,
            stateResult.Warnings);
    }

}
