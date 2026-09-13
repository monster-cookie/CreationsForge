using System.Text.Json;

namespace CreationsForge.Core.Engine.RecordWire;

/// <summary>Provides strict access to one validated closed record wire object.</summary>
public sealed class RecordWireObject
{
    /// <summary>Stores the context that validated and owns this object.</summary>
    private readonly RecordWireReadContext Context;

    /// <summary>Stores the request-local object element.</summary>
    private readonly JsonElement Element;

    /// <summary>Stores the complete accepted property shape.</summary>
    private readonly RecordWireObjectShape Shape;

    /// <summary>Stores the value-owned diagnostic path.</summary>
    private readonly string Path;

    /// <summary>Stores the admitted object depth.</summary>
    private readonly int Depth;

    /// <summary>Initializes a validated context-owned object reader.</summary>
    /// <param name="context">The context that validated this object.</param>
    /// <param name="element">The request-local object element.</param>
    /// <param name="shape">The complete accepted property shape.</param>
    /// <param name="path">The diagnostic path of the object.</param>
    /// <param name="depth">The admitted nesting depth of the object.</param>
    internal RecordWireObject(
        RecordWireReadContext context,
        JsonElement element,
        RecordWireObjectShape shape,
        string path,
        int depth)
    {
        Context = context;
        Element = element;
        Shape = shape;
        Path = path;
        Depth = depth;
    }

    /// <summary>Gets one property that the validated shape requires to be present exactly once.</summary>
    /// <param name="name">The exact required property name.</param>
    /// <returns>The context-owned child value with node and depth accounting applied.</returns>
    /// <exception cref="InvalidOperationException">Thrown when generated code requests a property not declared as required by the shape.</exception>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    public RecordWireValue GetRequiredProperty(string name)
    {
        var ordinal = Shape.GetOrdinal(name);
        if (!Shape.IsRequired(ordinal))
        {
            throw new InvalidOperationException(
                $"Property '{name}' is not required by record wire object shape '{Shape.Name}'.");
        }

        if (!Element.TryGetProperty(name, out var child))
        {
            throw new InvalidOperationException(
                $"Validated record wire object shape '{Shape.Name}' no longer contains required property '{name}'.");
        }

        return Context.CreateChild(child, AppendPropertyPath(Path, name), Depth + 1);
    }

    /// <summary>Gets one optional property when it occurs in the validated object.</summary>
    /// <param name="name">The exact optional property name.</param>
    /// <param name="value">Receives the context-owned child value when present.</param>
    /// <returns><see langword="true"/> when the optional property is present.</returns>
    /// <exception cref="InvalidOperationException">Thrown when generated code requests a property not declared as optional by the shape.</exception>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    public bool TryGetOptionalProperty(string name, out RecordWireValue value)
    {
        var ordinal = Shape.GetOrdinal(name);
        if (ordinal < Shape.RequiredProperties.Count || ordinal >= Shape.PropertyCount)
        {
            throw new InvalidOperationException(
                $"Property '{name}' is not optional in record wire object shape '{Shape.Name}'.");
        }

        if (!Element.TryGetProperty(name, out var child))
        {
            value = default;
            return false;
        }

        value = Context.CreateChild(child, AppendPropertyPath(Path, name), Depth + 1);
        return true;
    }

    /// <summary>Appends one known property name to a diagnostic path.</summary>
    /// <param name="path">The owning object path.</param>
    /// <param name="name">The exact property name.</param>
    /// <returns>The child diagnostic path.</returns>
    private static string AppendPropertyPath(string path, string name)
    {
        return $"{path}.{name}";
    }
}
