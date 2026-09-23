namespace CreationsForge.Engine.Records;

/// <summary>Reports an invalid, conflicting, or failed record authoring operation.</summary>
public sealed class RecordEditingException : Exception
{
    /// <summary>Initializes a record editing failure.</summary>
    /// <param name="message">The actionable failure message.</param>
    public RecordEditingException(string message)
        : base(message)
    {
    }

    /// <summary>Initializes a record editing failure with its underlying Mutagen cause.</summary>
    /// <param name="message">The actionable failure message.</param>
    /// <param name="innerException">The underlying Mutagen cause.</param>
    public RecordEditingException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
