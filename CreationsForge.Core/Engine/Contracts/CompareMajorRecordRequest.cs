namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Requests a detached native-field comparison for two contexts of one major record.</summary>
public sealed class CompareMajorRecordRequest
{
    /// <summary>Initializes a major-record comparison request.</summary>
    /// <param name="before">The exact prior record context selection.</param>
    /// <param name="after">The exact resulting record context selection.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="before"/> or <paramref name="after"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when the selections identify different FormKeys.</exception>
    public CompareMajorRecordRequest(ReferenceRequest before, ReferenceRequest after)
    {
        ArgumentNullException.ThrowIfNull(before);
        ArgumentNullException.ThrowIfNull(after);
        if (before.FormKey != after.FormKey)
        {
            throw new ArgumentException("A major-record comparison requires selections for the same FormKey.", nameof(after));
        }

        Before = before;
        After = after;
    }

    /// <summary>Gets the exact prior record context selection.</summary>
    public ReferenceRequest Before { get; }

    /// <summary>Gets the exact resulting record context selection.</summary>
    public ReferenceRequest After { get; }
}
