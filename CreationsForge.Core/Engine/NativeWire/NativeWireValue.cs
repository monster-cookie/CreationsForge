using System.Text.Json;

namespace CreationsForge.Core.Engine.NativeWire;

/// <summary>Represents one context-owned JSON value whose path, depth, and node budget have already been recorded.</summary>
public readonly struct NativeWireValue
{
    /// <summary>Initializes one value owned by a native wire read context.</summary>
    /// <param name="context">The context that admitted this value.</param>
    /// <param name="element">The request-local JSON element.</param>
    /// <param name="path">The diagnostic path owned by this value.</param>
    /// <param name="depth">The admitted nesting depth.</param>
    internal NativeWireValue(
        NativeWireReadContext context,
        JsonElement element,
        string path,
        int depth)
    {
        Context = context;
        Element = element;
        Path = path;
        Depth = depth;
    }

    /// <summary>Gets the context that admitted this value.</summary>
    internal NativeWireReadContext? Context { get; }

    /// <summary>Gets the request-local JSON element without exposing it to generated readers.</summary>
    internal JsonElement Element { get; }

    /// <summary>Gets the diagnostic path without requiring generated readers to compose paths.</summary>
    internal string? Path { get; }

    /// <summary>Gets the admitted nesting depth, including this value.</summary>
    internal int Depth { get; }
}
