using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Core.Engine.NativeWire;

/// <summary>Carries either one completely decoded typed value or a stable input failure.</summary>
/// <typeparam name="T">The reference type produced only after decoding succeeds.</typeparam>
public sealed class NativeWireDecodeResult<T>
    where T : class
{
    /// <summary>Initializes an immutable native wire decode result.</summary>
    /// <param name="succeeded">Whether a complete value was decoded.</param>
    /// <param name="value">The complete decoded value, or <see langword="null"/> on failure.</param>
    /// <param name="error">The typed failure, or <see langword="null"/> on success.</param>
    private NativeWireDecodeResult(bool succeeded, T? value, EngineError? error)
    {
        Succeeded = succeeded;
        Value = value;
        Error = error;
    }

    /// <summary>Gets whether decoding produced a complete value.</summary>
    public bool Succeeded { get; }

    /// <summary>Gets the complete decoded value, or <see langword="null"/> when decoding failed.</summary>
    public T? Value { get; }

    /// <summary>Gets the typed failure, or <see langword="null"/> when decoding succeeded.</summary>
    public EngineError? Error { get; }

    /// <summary>Creates a successful decode result containing a complete value.</summary>
    /// <param name="value">The complete decoded value.</param>
    /// <returns>A successful immutable result.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <see langword="null"/>.</exception>
    public static NativeWireDecodeResult<T> Success(T value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new NativeWireDecodeResult<T>(true, value, null);
    }

    /// <summary>Creates a failed decode result without publishing a partial value.</summary>
    /// <param name="error">The stable input failure.</param>
    /// <returns>A failed immutable result.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="error"/> is <see langword="null"/>.</exception>
    public static NativeWireDecodeResult<T> Failure(EngineError error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return new NativeWireDecodeResult<T>(false, null, error);
    }
}
