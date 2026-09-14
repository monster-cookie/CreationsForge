using System.Globalization;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Mcp;

/// <summary>Parses exact save, recovery, repair, and metadata-handle arguments.</summary>
internal static class McpSaveInput
{
    /// <summary>The only properties accepted by an exact nested revision.</summary>
    private static readonly IReadOnlySet<string> RevisionArgumentNames = new HashSet<string>(StringComparer.Ordinal)
    {
        "baselineId",
        "sequence",
    };

    /// <summary>The only properties accepted by an inline output association.</summary>
    private static readonly IReadOnlySet<string> OutputArgumentNames = new HashSet<string>(StringComparer.Ordinal)
    {
        "pluginPath",
        "modKey",
        "localizedOutputMode",
        "masterStyle",
    };

    /// <summary>The maximum accepted opaque metadata-handle length.</summary>
    internal const int MaximumMetadataHandleLength = 128;

    /// <summary>The maximum accepted recovery evidence-token length.</summary>
    internal const int MaximumEvidenceTokenLength = 4096;

    /// <summary>Reads one required exact workspace revision from a named closed object.</summary>
    /// <param name="arguments">The validated top-level arguments.</param>
    /// <param name="name">The required revision property name.</param>
    /// <param name="value">Receives the exact parsed revision.</param>
    /// <param name="error">Receives a stable validation error.</param>
    /// <returns><see langword="true"/> only for canonical UUID and unsigned decimal-string fields.</returns>
    internal static bool TryGetRequiredRevision(
        IReadOnlyDictionary<string, JsonElement> arguments,
        string name,
        out WorkspaceRevision value,
        out string error)
    {
        value = default;
        if (!arguments.TryGetValue(name, out var element))
        {
            error = $"Required argument '{name}' is missing.";
            return false;
        }

        if (!McpInput.TryGetClosedObject(element, name, RevisionArgumentNames, out var revision, out error) ||
            !McpInput.TryGetRequiredString(revision, "baselineId", 36, out var baselineText, out error) ||
            !Guid.TryParseExact(baselineText, "D", out var baselineId) ||
            baselineId == Guid.Empty ||
            !string.Equals(baselineId.ToString("D"), baselineText, StringComparison.Ordinal) ||
            !McpInput.TryGetRequiredString(revision, "sequence", 20, out var sequenceText, out error) ||
            !ulong.TryParse(sequenceText, NumberStyles.None, CultureInfo.InvariantCulture, out var sequence) ||
            !string.Equals(sequence.ToString(CultureInfo.InvariantCulture), sequenceText, StringComparison.Ordinal))
        {
            error = $"Argument '{name}' must be a closed object with canonical baselineId and unsigned decimal-string sequence values.";
            return false;
        }

        value = new WorkspaceRevision(baselineId, sequence);
        error = string.Empty;
        return true;
    }

    /// <summary>Reads one exact inline output association for restart-safe recovery and repair.</summary>
    /// <param name="arguments">The validated top-level arguments.</param>
    /// <param name="name">The required output property name.</param>
    /// <param name="value">Receives the exact association without path normalization.</param>
    /// <param name="error">Receives a stable validation error.</param>
    /// <returns><see langword="true"/> only for a closed canonical association.</returns>
    internal static bool TryGetOutputAssociation(
        IReadOnlyDictionary<string, JsonElement> arguments,
        string name,
        out OutputAssociation value,
        out string error)
    {
        value = null!;
        if (!arguments.TryGetValue(name, out var element) || element.ValueKind != JsonValueKind.Object)
        {
            error = $"Required argument '{name}' must be a closed output association object.";
            return false;
        }

        if (!McpInput.TryGetClosedObject(element, name, OutputArgumentNames, out var output, out error) ||
            !McpInput.TryGetRequiredString(output, "pluginPath", McpInput.MaximumPathLength, out var pluginPath, out error) ||
            !Path.IsPathFullyQualified(pluginPath) ||
            !McpInput.TryGetRequiredString(output, "modKey", 1024, out var modKeyText, out error) ||
            !ModKey.TryFromNameAndExtension(modKeyText, out var modKey, out _) ||
            !string.Equals(modKey.ToString(), modKeyText, StringComparison.Ordinal) ||
            !McpInput.TryGetRequiredString(output, "localizedOutputMode", 32, out var modeText, out error) ||
            !McpInput.TryGetRequiredString(output, "masterStyle", 32, out var styleText, out error))
        {
            error = $"Argument '{name}' must contain exact pluginPath, canonical modKey, localizedOutputMode, and masterStyle fields.";
            return false;
        }

        var mode = modeText switch
        {
            "embedded" => LocalizedOutputMode.Embedded,
            "separate_string_files" => LocalizedOutputMode.SeparateStringFiles,
            _ => (LocalizedOutputMode)(-1),
        };
        var style = styleText switch
        {
            "full" => OutputMasterStyle.Full,
            "small" => OutputMasterStyle.Small,
            "medium" => OutputMasterStyle.Medium,
            _ => (OutputMasterStyle)(-1),
        };
        if (!Enum.IsDefined(mode) || !Enum.IsDefined(style))
        {
            error = $"Argument '{name}' contains an unsupported localizedOutputMode or masterStyle.";
            return false;
        }

        try
        {
            value = new OutputAssociation(pluginPath, modKey, mode, style);
            error = string.Empty;
            return true;
        }
        catch (ArgumentException exception)
        {
            error = exception.Message;
            return false;
        }
    }

    /// <summary>Reads one closed save-repair direction.</summary>
    /// <param name="arguments">The validated arguments.</param>
    /// <param name="value">Receives the repair direction.</param>
    /// <param name="error">Receives a stable validation error.</param>
    /// <returns><see langword="true"/> for a recognized exact wire value.</returns>
    internal static bool TryGetRepairDirection(
        IReadOnlyDictionary<string, JsonElement> arguments,
        out RepairSaveDirection value,
        out string error)
    {
        value = default;
        if (!McpInput.TryGetRequiredString(arguments, "direction", 32, out var text, out error))
        {
            return false;
        }

        value = text switch
        {
            "complete_prepared" => RepairSaveDirection.CompletePrepared,
            "restore_baseline" => RepairSaveDirection.RestoreBaseline,
            _ => (RepairSaveDirection)(-1),
        };
        if (!Enum.IsDefined(value))
        {
            error = "Argument 'direction' must be complete_prepared or restore_baseline.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    /// <summary>Reads one closed output-recovery adoption mode.</summary>
    /// <param name="arguments">The validated arguments.</param>
    /// <param name="value">Receives the adoption mode.</param>
    /// <param name="error">Receives a stable validation error.</param>
    /// <returns><see langword="true"/> for a recognized exact wire value.</returns>
    internal static bool TryGetAdoptionMode(
        IReadOnlyDictionary<string, JsonElement> arguments,
        out OutputRecoveryAdoptionMode value,
        out string error)
    {
        value = default;
        if (!McpInput.TryGetRequiredString(arguments, "mode", 64, out var text, out error))
        {
            return false;
        }

        value = text switch
        {
            "resume_staged_after_not_committed" => OutputRecoveryAdoptionMode.ResumeStagedAfterNotCommitted,
            "reopen_resolved_output" => OutputRecoveryAdoptionMode.ReopenResolvedOutput,
            _ => (OutputRecoveryAdoptionMode)(-1),
        };
        if (!Enum.IsDefined(value))
        {
            error = "Argument 'mode' must be resume_staged_after_not_committed or reopen_resolved_output.";
            return false;
        }

        error = string.Empty;
        return true;
    }

}
