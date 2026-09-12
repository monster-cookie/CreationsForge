namespace CreationsForge.Core.Engine.NativeWire;

/// <summary>Defines the maximum resources one native wire decode may consume before constructing a typed edit.</summary>
public sealed class NativeWireReadLimits
{
    /// <summary>Gets the default maximum nesting depth, including the root value.</summary>
    public const int DefaultMaximumDepth = 64;

    /// <summary>Gets the default maximum number of JSON values visited during one decode.</summary>
    public const int DefaultMaximumNodes = 262144;

    /// <summary>Gets the default maximum number of elements accepted in one JSON array.</summary>
    public const int DefaultMaximumArrayElements = 65536;

    /// <summary>Gets the default maximum number of UTF-16 code units accepted in one ordinary string.</summary>
    public const int DefaultMaximumStringLength = 1048576;

    /// <summary>Gets the default maximum number of bytes decoded from one Base64 payload.</summary>
    public const int DefaultMaximumDecodedByteLength = 8388608;

    /// <summary>Initializes a bounded native wire decode policy.</summary>
    /// <param name="maximumDepth">The maximum nesting depth, including the root value.</param>
    /// <param name="maximumNodes">The maximum number of JSON values visited during one decode.</param>
    /// <param name="maximumArrayElements">The maximum number of elements accepted in one JSON array.</param>
    /// <param name="maximumStringLength">The maximum number of UTF-16 code units accepted in one ordinary string.</param>
    /// <param name="maximumDecodedByteLength">The maximum number of bytes decoded from one Base64 payload.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any limit is not positive.</exception>
    public NativeWireReadLimits(
        int maximumDepth,
        int maximumNodes,
        int maximumArrayElements,
        int maximumStringLength,
        int maximumDecodedByteLength)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maximumDepth);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maximumNodes);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maximumArrayElements);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maximumStringLength);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maximumDecodedByteLength);
        MaximumDepth = maximumDepth;
        MaximumNodes = maximumNodes;
        MaximumArrayElements = maximumArrayElements;
        MaximumStringLength = maximumStringLength;
        MaximumDecodedByteLength = maximumDecodedByteLength;
    }

    /// <summary>Gets the shared default policy for bounded native edit decoding.</summary>
    public static NativeWireReadLimits Default { get; } = new(
        DefaultMaximumDepth,
        DefaultMaximumNodes,
        DefaultMaximumArrayElements,
        DefaultMaximumStringLength,
        DefaultMaximumDecodedByteLength);

    /// <summary>Gets the maximum nesting depth, including the root value.</summary>
    public int MaximumDepth { get; }

    /// <summary>Gets the maximum number of JSON values visited during one decode.</summary>
    public int MaximumNodes { get; }

    /// <summary>Gets the maximum number of elements accepted in one JSON array.</summary>
    public int MaximumArrayElements { get; }

    /// <summary>Gets the maximum number of UTF-16 code units accepted in one ordinary string.</summary>
    public int MaximumStringLength { get; }

    /// <summary>Gets the maximum number of bytes decoded from one Base64 payload.</summary>
    public int MaximumDecodedByteLength { get; }
}
