using System.ComponentModel;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordWire;
using CreationsForge.Services;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.RecordEditing.Drafts;

/// <summary>Describes one stable path-specific typed draft issue.</summary>
public sealed class RecordWireDraftIssue
{
    /// <summary>Initializes one immutable issue.</summary>
    /// <param name="code">The stable issue category.</param>
    /// <param name="path">The exact root-based JSON path.</param>
    /// <param name="message">The complete diagnostic message.</param>
    /// <param name="node">The corresponding typed node when exact resolution succeeds.</param>
    public RecordWireDraftIssue(RecordWireDraftIssueCode code, string path, string message, RecordWireDraftNode? node = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        Code = code;
        Path = path;
        Message = message;
        Node = node;
    }

    /// <summary>Gets the stable issue category.</summary>
    public RecordWireDraftIssueCode Code { get; }

    /// <summary>Gets the exact root-based JSON path.</summary>
    public string Path { get; }

    /// <summary>Gets the complete diagnostic message.</summary>
    public string Message { get; }

    /// <summary>Gets the corresponding typed node, or <see langword="null"/> for a banner-level issue.</summary>
    public RecordWireDraftNode? Node { get; }
}
