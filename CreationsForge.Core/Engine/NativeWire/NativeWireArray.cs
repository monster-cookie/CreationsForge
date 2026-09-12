using System.Text.Json;

namespace CreationsForge.Core.Engine.NativeWire;

/// <summary>Provides indexed access to one bounded native wire array.</summary>
public sealed class NativeWireArray
{
    /// <summary>Stores the context that admitted this array.</summary>
    private readonly NativeWireReadContext Context;

    /// <summary>Stores request-local array elements captured by one bounded linear enumeration.</summary>
    private readonly JsonElement[] Elements;

    /// <summary>Stores the value-owned diagnostic path.</summary>
    private readonly string Path;

    /// <summary>Stores the admitted array depth.</summary>
    private readonly int Depth;

    /// <summary>Initializes a bounded context-owned array reader.</summary>
    /// <param name="context">The context that admitted this array.</param>
    /// <param name="element">The request-local array element.</param>
    /// <param name="path">The diagnostic path of the array.</param>
    /// <param name="depth">The admitted nesting depth of the array.</param>
    internal NativeWireArray(
        NativeWireReadContext context,
        JsonElement element,
        string path,
        int depth)
    {
        Context = context;
        Path = path;
        Depth = depth;
        Count = element.GetArrayLength();
        context.ThrowIfCancellationRequested();
        Elements = new JsonElement[Count];
        var index = 0;
        foreach (var child in element.EnumerateArray())
        {
            context.ThrowIfCancellationRequested();
            Elements[index] = child;
            index++;
        }
    }

    /// <summary>Gets the validated number of array elements.</summary>
    public int Count { get; }

    /// <summary>Gets one indexed child value with node and depth accounting applied.</summary>
    /// <param name="index">The zero-based array index.</param>
    /// <returns>The context-owned child value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is outside this array.</exception>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    public NativeWireValue GetElement(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        if (index >= Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        return Context.CreateChild(Elements[index], $"{Path}[{index}]", Depth + 1);
    }
}
