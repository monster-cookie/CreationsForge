namespace CreationsForge.Engine.Records;

/// <summary>Reports one registered field whose values differ between two exact record versions.</summary>
public sealed class RecordDifference
{
    /// <summary>Initializes a field difference.</summary>
    /// <param name="path">The registered field path.</param>
    /// <param name="left">The value in the left record.</param>
    /// <param name="right">The value in the right record.</param>
    public RecordDifference(string path, RecordValue left, RecordValue right)
    {
        Path = path;
        Left = left;
        Right = right;
    }

    /// <summary>Gets the registered field path.</summary>
    public string Path { get; }

    /// <summary>Gets the left record's value.</summary>
    public RecordValue Left { get; }

    /// <summary>Gets the right record's value.</summary>
    public RecordValue Right { get; }
}
