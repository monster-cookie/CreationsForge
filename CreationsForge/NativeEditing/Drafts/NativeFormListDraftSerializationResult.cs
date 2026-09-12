using System.ComponentModel;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeWire;
using CreationsForge.Services;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.NativeEditing.Drafts;

/// <summary>Owns detached serialized command arguments or immutable blocking issues.</summary>
public sealed class NativeFormListDraftSerializationResult
{
    /// <summary>The internally owned detached serialized arguments.</summary>
    private readonly JsonElement? Arguments;

    /// <summary>Initializes one immutable serialization result.</summary>
    /// <param name="arguments">The detached arguments object on success.</param>
    /// <param name="issues">The complete blocking issue set.</param>
    /// <exception cref="ArgumentException">Thrown when arguments and issues do not describe exactly one outcome.</exception>
    public NativeFormListDraftSerializationResult(JsonElement? arguments, IEnumerable<NativeWireDraftIssue> issues)
    {
        ArgumentNullException.ThrowIfNull(issues);
        var issueCopy = issues.ToArray();
        if ((arguments.HasValue && issueCopy.Length != 0) || (!arguments.HasValue && issueCopy.Length == 0))
        {
            throw new ArgumentException("A draft serialization result must contain either arguments or blocking issues.");
        }

        if (arguments.HasValue && arguments.Value.ValueKind != JsonValueKind.Object)
        {
            throw new ArgumentException("Serialized command arguments must be a detached JSON object.", nameof(arguments));
        }

        Arguments = arguments?.Clone();
        Issues = Array.AsReadOnly(issueCopy);
    }

    /// <summary>Gets whether detached command arguments are available.</summary>
    public bool Succeeded => Arguments.HasValue;

    /// <summary>Gets the immutable blocking issues.</summary>
    public IReadOnlyList<NativeWireDraftIssue> Issues { get; }

    /// <summary>Returns a new detached clone of the successful command arguments.</summary>
    /// <returns>The cloned command arguments.</returns>
    /// <exception cref="InvalidOperationException">Thrown when validation prevented serialization.</exception>
    public JsonElement GetArguments()
    {
        if (!Arguments.HasValue)
        {
            throw new InvalidOperationException("Command arguments are unavailable because draft serialization failed.");
        }

        return Arguments.Value.Clone();
    }
}
