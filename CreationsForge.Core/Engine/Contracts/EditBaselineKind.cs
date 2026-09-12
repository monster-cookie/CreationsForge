namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Identifies the immutable native state used as the before-view for one staged edit session.</summary>
public enum EditBaselineKind
{
    /// <summary>The target did not exist before the edit session allocated it.</summary>
    Absent,

    /// <summary>The target existed in the originally selected output.</summary>
    OriginalOutput,

    /// <summary>The target originated from one exact immutable source or load-order context.</summary>
    SourceContext
}
