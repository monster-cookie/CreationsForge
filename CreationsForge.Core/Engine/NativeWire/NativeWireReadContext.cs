using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Core.Engine.NativeWire;

/// <summary>Tracks one bounded native wire traversal and converts expected input failures into typed results.</summary>
public sealed partial class NativeWireReadContext
{
    /// <summary>Stores the immutable per-operation limits.</summary>
    private readonly NativeWireReadLimits Limits;

    /// <summary>Stores the cancellation token observed during traversal.</summary>
    private readonly CancellationToken CancellationToken;

    /// <summary>Tracks the number of JSON values admitted by this context.</summary>
    private int VisitedNodes;

    /// <summary>Initializes one private decode context.</summary>
    /// <param name="limits">The immutable per-operation limits.</param>
    /// <param name="cancellationToken">The token observed during traversal.</param>
    private NativeWireReadContext(
        NativeWireReadLimits limits,
        CancellationToken cancellationToken)
    {
        Limits = limits;
        CancellationToken = cancellationToken;
    }

    /// <summary>Runs one generated decoder with bounded traversal and a typed expected-failure boundary.</summary>
    /// <typeparam name="T">The complete reference type produced after all validation succeeds.</typeparam>
    /// <param name="root">The request-local JSON root.</param>
    /// <param name="limits">The per-operation resource limits.</param>
    /// <param name="cancellationToken">A token observed throughout traversal and before construction.</param>
    /// <param name="decoder">The generated static decoder that consumes the context-owned root.</param>
    /// <returns>A complete value or one path-specific invalid-request failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="limits"/> or <paramref name="decoder"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    public static NativeWireDecodeResult<T> Decode<T>(
        JsonElement root,
        NativeWireReadLimits limits,
        CancellationToken cancellationToken,
        Func<NativeWireReadContext, NativeWireValue, T> decoder)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(limits);
        ArgumentNullException.ThrowIfNull(decoder);
        cancellationToken.ThrowIfCancellationRequested();

        var context = new NativeWireReadContext(limits, cancellationToken);
        try
        {
            var rootValue = context.CreateValue(root, "$", 1);
            var value = decoder(context, rootValue);
            cancellationToken.ThrowIfCancellationRequested();
            if (value is null)
            {
                throw new InvalidOperationException("A native wire decoder returned null without reporting an input failure.");
            }

            return NativeWireDecodeResult<T>.Success(value);
        }
        catch (NativeWireInputException exception)
        {
            return NativeWireDecodeResult<T>.Failure(exception.Error);
        }
    }

    /// <summary>Validates one closed object before generated code reads any property values.</summary>
    /// <param name="value">The context-owned value expected to contain an object.</param>
    /// <param name="shape">The complete accepted required and optional property set.</param>
    /// <returns>A strict object reader bound to the validated shape.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="shape"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    public NativeWireObject ReadObject(NativeWireValue value, NativeWireObjectShape shape)
    {
        ArgumentNullException.ThrowIfNull(shape);
        EnsureOwned(value);
        CancellationToken.ThrowIfCancellationRequested();
        if (value.Element.ValueKind != JsonValueKind.Object)
        {
            Fail(value, $"Expected a {shape.Name} object.");
        }

        var propertyCount = 0;
        foreach (var ignored in value.Element.EnumerateObject())
        {
            CancellationToken.ThrowIfCancellationRequested();
            propertyCount++;
            if (propertyCount > RemainingNodes)
            {
                Fail(value, $"Object property values exceed the remaining node limit of {RemainingNodes}.");
            }
        }

        var seen = new bool[shape.PropertyCount];
        foreach (var property in value.Element.EnumerateObject())
        {
            CancellationToken.ThrowIfCancellationRequested();
            var propertyName = string.Empty;
            try
            {
                propertyName = property.Name;
            }
            catch (InvalidOperationException exception) when (exception is not ObjectDisposedException)
            {
                Fail(value, "An object property name contains incomplete UTF-16 data.");
            }

            if (propertyName.Length > Limits.MaximumStringLength)
            {
                Fail(value, $"Object property name length {propertyName.Length} exceeds the string limit of {Limits.MaximumStringLength}.");
            }

            var ordinal = shape.GetOrdinal(propertyName);
            if (ordinal < 0)
            {
                Fail(value, $"Property '{propertyName}' is not accepted by {shape.Name}.");
            }

            if (seen[ordinal])
            {
                Fail(value, $"Property '{propertyName}' occurs more than once.");
            }

            seen[ordinal] = true;
        }

        for (var ordinal = 0; ordinal < shape.RequiredProperties.Count; ordinal++)
        {
            if (!seen[ordinal])
            {
                Fail(value, $"Required property '{shape.RequiredProperties[ordinal]}' is missing.");
            }
        }

        return new NativeWireObject(
            this,
            value.Element,
            shape,
            value.Path!,
            value.Depth);
    }

    /// <summary>Validates one array and its element count before generated code allocates a native collection.</summary>
    /// <param name="value">The context-owned value expected to contain an array.</param>
    /// <returns>A bounded indexed array reader.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    public NativeWireArray ReadArray(NativeWireValue value)
    {
        EnsureOwned(value);
        CancellationToken.ThrowIfCancellationRequested();
        if (value.Element.ValueKind != JsonValueKind.Array)
        {
            Fail(value, "Expected an array.");
        }

        var count = value.Element.GetArrayLength();
        if (count > Limits.MaximumArrayElements)
        {
            Fail(value, $"Array length {count} exceeds the per-array limit of {Limits.MaximumArrayElements}.");
        }

        if (count > RemainingNodes)
        {
            Fail(value, $"Array elements exceed the remaining node limit of {RemainingNodes}.");
        }

        return new NativeWireArray(this, value.Element, value.Path!, value.Depth);
    }

    /// <summary>Validates one decoded array or rectangular-array dimension before generated code allocates native storage.</summary>
    /// <param name="value">The context-owned numeric value that supplied the length.</param>
    /// <param name="length">The decoded element or dimension count.</param>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    public void ValidateArrayLength(NativeWireValue value, int length)
    {
        EnsureOwned(value);
        CancellationToken.ThrowIfCancellationRequested();
        if (length < 0)
        {
            Fail(value, "Array length cannot be negative.");
        }

        if (length > Limits.MaximumArrayElements)
        {
            Fail(value, $"Array length {length} exceeds the per-array limit of {Limits.MaximumArrayElements}.");
        }
    }

    /// <summary>Determines whether one admitted value is the JSON null literal.</summary>
    /// <param name="value">The context-owned value to inspect.</param>
    /// <returns><see langword="true"/> only for the JSON null literal.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    public bool IsNull(NativeWireValue value)
    {
        EnsureOwned(value);
        CancellationToken.ThrowIfCancellationRequested();
        return value.Element.ValueKind == JsonValueKind.Null;
    }

    /// <summary>Rejects a statically recognized invalid native state at the owning value's path.</summary>
    /// <param name="value">The context-owned value whose state is invalid.</param>
    /// <param name="message">A non-empty description of the violated native invariant.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="message"/> is empty or whitespace.</exception>
    [DoesNotReturn]
    public void Reject(NativeWireValue value, string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        EnsureOwned(value);
        Fail(value, message);
    }

    /// <summary>Gets the number of additional JSON values that may be admitted.</summary>
    private int RemainingNodes => Limits.MaximumNodes - VisitedNodes;

    /// <summary>Creates one child value after enforcing cancellation, depth, and total node limits.</summary>
    /// <param name="element">The request-local child element.</param>
    /// <param name="path">The diagnostic child path.</param>
    /// <param name="depth">The child nesting depth.</param>
    /// <returns>The admitted context-owned child value.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    internal NativeWireValue CreateChild(JsonElement element, string path, int depth)
    {
        return CreateValue(element, path, depth);
    }

    /// <summary>Observes the decode cancellation token during bounded helper traversal.</summary>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    internal void ThrowIfCancellationRequested()
    {
        CancellationToken.ThrowIfCancellationRequested();
    }

    /// <summary>Verifies that one opaque value belongs to this context.</summary>
    /// <param name="value">The value supplied by generated code.</param>
    /// <exception cref="InvalidOperationException">Thrown for a default value or a value owned by another decode context.</exception>
    private void EnsureOwned(NativeWireValue value)
    {
        if (!ReferenceEquals(value.Context, this) || value.Path is null)
        {
            throw new InvalidOperationException("The native wire value does not belong to this decode context.");
        }
    }

    /// <summary>Creates one admitted value and records its resource use.</summary>
    /// <param name="element">The request-local JSON element.</param>
    /// <param name="path">The value-owned diagnostic path.</param>
    /// <param name="depth">The value nesting depth, including the root.</param>
    /// <returns>The admitted context-owned value.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    private NativeWireValue CreateValue(JsonElement element, string path, int depth)
    {
        CancellationToken.ThrowIfCancellationRequested();
        if (depth > Limits.MaximumDepth)
        {
            FailAtPath(path, $"Nesting depth {depth} exceeds the limit of {Limits.MaximumDepth}.");
        }

        if (VisitedNodes >= Limits.MaximumNodes)
        {
            FailAtPath(path, $"Visited JSON values exceed the node limit of {Limits.MaximumNodes}.");
        }

        VisitedNodes++;
        return new NativeWireValue(this, element, path, depth);
    }

    /// <summary>Stops expected decoding with an invalid-request error at one admitted value.</summary>
    /// <param name="value">The context-owned invalid value.</param>
    /// <param name="message">The diagnostic failure message.</param>
    [DoesNotReturn]
    private void Fail(NativeWireValue value, string message)
    {
        EnsureOwned(value);
        FailAtPath(value.Path!, message);
    }

    /// <summary>Stops expected decoding with an invalid-request error at one diagnostic path.</summary>
    /// <param name="path">The exact value-owned diagnostic path.</param>
    /// <param name="message">The diagnostic failure message.</param>
    [DoesNotReturn]
    private static void FailAtPath(string path, string message)
    {
        throw new NativeWireInputException(new EngineError(
            EngineErrorCode.InvalidRequest,
            $"{path}: {message}"));
    }

    /// <summary>Transports an expected input error to the outer decode boundary without exposing an exception contract.</summary>
    private sealed class NativeWireInputException : Exception
    {
        /// <summary>Initializes the private expected-failure sentinel.</summary>
        /// <param name="error">The path-specific stable input error.</param>
        internal NativeWireInputException(EngineError error)
            : base(error.Message)
        {
            Error = error;
        }

        /// <summary>Gets the path-specific stable input error.</summary>
        internal EngineError Error { get; }
    }
}
