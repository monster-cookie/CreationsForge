namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Carries either a successful engine value or a stable typed failure with operation context.
/// </summary>
/// <typeparam name="T">The successful value type.</typeparam>
public sealed class EngineResult<T>
{
    /// <summary>Initializes an immutable engine result.</summary>
    /// <param name="succeeded">Whether the operation produced a successful value.</param>
    /// <param name="value">The successful value, or <see langword="null"/> for failure.</param>
    /// <param name="error">The typed failure, or <see langword="null"/> for success.</param>
    /// <param name="workspaceId">The associated workspace when available.</param>
    /// <param name="operationId">The associated idempotency identifier when applicable.</param>
    /// <param name="baseRevision">The revision against which the operation ran when available.</param>
    /// <param name="resultRevision">The resulting or unchanged revision when available.</param>
    /// <param name="warnings">The immutable non-fatal warnings.</param>
    private EngineResult(
        bool succeeded,
        T? value,
        EngineError? error,
        Guid? workspaceId,
        Guid? operationId,
        WorkspaceRevision? baseRevision,
        WorkspaceRevision? resultRevision,
        IReadOnlyList<EngineWarning>? warnings)
    {
        Succeeded = succeeded;
        Value = value;
        Error = error;
        WorkspaceId = workspaceId;
        OperationId = operationId;
        BaseRevision = baseRevision;
        ResultRevision = resultRevision;
        Warnings = Array.AsReadOnly(warnings?.ToArray() ?? Array.Empty<EngineWarning>());
    }

    /// <summary>Gets a value indicating whether the operation completed successfully.</summary>
    public bool Succeeded { get; }

    /// <summary>Gets the successful value, or the default value on failure.</summary>
    public T? Value { get; }

    /// <summary>Gets the typed failure, or <see langword="null"/> on success.</summary>
    public EngineError? Error { get; }

    /// <summary>Gets the associated workspace identifier, when available.</summary>
    public Guid? WorkspaceId { get; }

    /// <summary>Gets the mutation operation identifier, when available.</summary>
    public Guid? OperationId { get; }

    /// <summary>Gets the workspace revision used as the operation base, when applicable.</summary>
    public WorkspaceRevision? BaseRevision { get; }

    /// <summary>Gets the workspace revision after the operation, when applicable.</summary>
    public WorkspaceRevision? ResultRevision { get; }

    /// <summary>Gets the immutable collection of non-fatal warnings.</summary>
    public IReadOnlyList<EngineWarning> Warnings { get; }

    /// <summary>Creates a successful result.</summary>
    /// <param name="value">The non-null successful value.</param>
    /// <param name="workspaceId">The associated workspace identifier, when available.</param>
    /// <param name="operationId">The mutation operation identifier, when available.</param>
    /// <param name="baseRevision">The operation base revision, when applicable.</param>
    /// <param name="resultRevision">The resulting revision, when applicable.</param>
    /// <param name="warnings">Optional non-fatal warnings.</param>
    /// <returns>A successful immutable result.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <see langword="null"/>.</exception>
    public static EngineResult<T> Success(
        T value,
        Guid? workspaceId = null,
        Guid? operationId = null,
        WorkspaceRevision? baseRevision = null,
        WorkspaceRevision? resultRevision = null,
        IReadOnlyList<EngineWarning>? warnings = null)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new EngineResult<T>(true, value, null, workspaceId, operationId, baseRevision, resultRevision, warnings);
    }

    /// <summary>Creates a failed result.</summary>
    /// <param name="error">The stable typed failure.</param>
    /// <param name="workspaceId">The associated workspace identifier, when available.</param>
    /// <param name="operationId">The mutation operation identifier, when available.</param>
    /// <param name="baseRevision">The operation base revision, when applicable.</param>
    /// <param name="resultRevision">The unchanged or resulting revision, when applicable.</param>
    /// <param name="warnings">Optional non-fatal warnings.</param>
    /// <returns>A failed immutable result.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="error"/> is <see langword="null"/>.</exception>
    public static EngineResult<T> Failure(
        EngineError error,
        Guid? workspaceId = null,
        Guid? operationId = null,
        WorkspaceRevision? baseRevision = null,
        WorkspaceRevision? resultRevision = null,
        IReadOnlyList<EngineWarning>? warnings = null)
    {
        ArgumentNullException.ThrowIfNull(error);
        return new EngineResult<T>(false, default, error, workspaceId, operationId, baseRevision, resultRevision, warnings);
    }
}
