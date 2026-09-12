namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Requests a detached before-and-after comparison for one FormList.</summary>
public sealed class CompareFormListRequest
{
    /// <summary>Initializes a FormList comparison request.</summary>
    /// <param name="before">The exact prior native context selection.</param>
    /// <param name="after">The exact resulting native context selection.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="before"/> or <paramref name="after"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when the selections identify different native records.</exception>
    public CompareFormListRequest(ReferenceRequest before, ReferenceRequest after)
    {
        ArgumentNullException.ThrowIfNull(before);
        ArgumentNullException.ThrowIfNull(after);
        if (before.FormKey != after.FormKey)
        {
            throw new ArgumentException("A FormList comparison requires before and after selections for the same FormKey.", nameof(after));
        }

        Before = before;
        After = after;
    }

    /// <summary>Gets the exact prior native context selection.</summary>
    public ReferenceRequest Before { get; }

    /// <summary>Gets the exact resulting native context selection.</summary>
    public ReferenceRequest After { get; }
}
