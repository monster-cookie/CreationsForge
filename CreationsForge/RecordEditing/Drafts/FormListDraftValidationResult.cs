using System.ComponentModel;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordWire;
using CreationsForge.Services;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.RecordEditing.Drafts;

/// <summary>Contains all immutable issues from one typed draft validation pass.</summary>
public sealed class FormListDraftValidationResult
{
    /// <summary>Initializes one validation result.</summary>
    /// <param name="issues">The complete issue set in deterministic traversal order.</param>
    public FormListDraftValidationResult(IEnumerable<RecordWireDraftIssue> issues)
    {
        ArgumentNullException.ThrowIfNull(issues);
        Issues = Array.AsReadOnly(issues.ToArray());
    }

    /// <summary>Gets whether no validation issue prevents serialization or Apply.</summary>
    public bool IsValid => Issues.Count == 0;

    /// <summary>Gets the immutable complete issue set.</summary>
    public IReadOnlyList<RecordWireDraftIssue> Issues { get; }
}
