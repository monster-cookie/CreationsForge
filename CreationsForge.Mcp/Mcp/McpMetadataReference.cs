namespace CreationsForge.Mcp;

/// <summary>Describes an opaque host-local reference to exact immutable Core metadata.</summary>
internal sealed class McpMetadataReference
{
    /// <summary>Initializes an immutable metadata reference.</summary>
    /// <param name="handle">The non-empty opaque handle.</param>
    /// <param name="kind">The retained metadata type.</param>
    /// <param name="baselineId">The informative engine baseline identifier, when the retained value is a baseline.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="handle"/> is empty or <paramref name="kind"/> is undefined.</exception>
    internal McpMetadataReference(string handle, McpMetadataKind kind, Guid? baselineId = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(handle);
        if (!Enum.IsDefined(kind))
        {
            throw new ArgumentOutOfRangeException(nameof(kind));
        }

        Handle = handle;
        Kind = kind;
        BaselineId = baselineId;
    }

    /// <summary>Gets the opaque handle valid only for the issuing MCP host lifetime.</summary>
    internal string Handle { get; }

    /// <summary>Gets the exact retained metadata type.</summary>
    internal McpMetadataKind Kind { get; }

    /// <summary>Gets the informative engine baseline identifier, or <see langword="null"/> for other metadata.</summary>
    internal Guid? BaselineId { get; }
}
