using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Services;

/// <summary>
/// Combines explicit read-only source inputs with the output that must be admitted before a native workspace becomes active.
/// </summary>
public sealed class NativeWorkspaceOpenRequest
{
    /// <summary>Initializes a complete native workspace activation request.</summary>
    /// <param name="sources">The explicit source, load-order, data, and string inputs.</param>
    /// <param name="outputMode">Whether the selected output must be created or already exist.</param>
    /// <param name="output">The output identity and native formatting choices.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required request component is <see langword="null"/>.</exception>
    public NativeWorkspaceOpenRequest(
        WorkspaceOpenRequest sources,
        OutputSelectionMode outputMode,
        OutputAssociation output)
    {
        ArgumentNullException.ThrowIfNull(sources);
        ArgumentNullException.ThrowIfNull(output);
        Sources = sources;
        OutputMode = outputMode;
        Output = output;
    }

    /// <summary>Gets the explicit native source inputs.</summary>
    public WorkspaceOpenRequest Sources { get; }

    /// <summary>Gets whether the output must be created or already exist.</summary>
    public OutputSelectionMode OutputMode { get; }

    /// <summary>Gets the output identity and native formatting choices.</summary>
    public OutputAssociation Output { get; }
}
