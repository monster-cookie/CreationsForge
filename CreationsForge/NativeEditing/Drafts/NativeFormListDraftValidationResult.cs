using System.ComponentModel;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeWire;
using CreationsForge.Services;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.NativeEditing.Drafts;

/// <summary>Contains all immutable issues from one typed draft validation pass.</summary>
public sealed class NativeFormListDraftValidationResult
{
    /// <summary>Initializes one validation result.</summary>
    /// <param name="issues">The complete issue set in deterministic traversal order.</param>
    public NativeFormListDraftValidationResult(IEnumerable<NativeWireDraftIssue> issues)
    {
        ArgumentNullException.ThrowIfNull(issues);
        Issues = Array.AsReadOnly(issues.ToArray());
    }

    /// <summary>Gets whether no validation issue prevents serialization or Apply.</summary>
    public bool IsValid => Issues.Count == 0;

    /// <summary>Gets the immutable complete issue set.</summary>
    public IReadOnlyList<NativeWireDraftIssue> Issues { get; }
}
