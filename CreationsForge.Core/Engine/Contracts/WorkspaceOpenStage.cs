namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Reports a coarse-grained stage while a workspace is being opened.
/// </summary>
public enum WorkspaceOpenStage
{
    /// <summary>The request is being validated and canonicalized.</summary>
    Validating,

    /// <summary>A compatible game adapter has been selected.</summary>
    SelectingAdapter,

    /// <summary>The game adapter is validating and fingerprinting the explicit native inputs.</summary>
    PreparingInputs,

    /// <summary>The game adapter is opening the explicitly supplied native inputs.</summary>
    OpeningSources,

    /// <summary>The game adapter is parsing one explicitly supplied native plugin.</summary>
    ParsingPlugin,

    /// <summary>The game adapter is completing immutable source baseline verification.</summary>
    FinalizingSources,

    /// <summary>The independently owned workspace is ready for use.</summary>
    Completed
}
