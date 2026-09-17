using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.PresentationTests.Support;

/// <summary>
/// Records explicit workspace-open requests and delegates their deterministic result to a test callback.
/// </summary>
internal sealed class FakeFormListWorkspaceFactory : IPluginWorkspaceFactory
{
    /// <summary>The callback that supplies each open result.</summary>
    private readonly Func<WorkspaceOpenRequest, CancellationToken, ValueTask<EngineResult<IPluginWorkspace>>> OpenAction;

    /// <summary>Initializes a recording workspace factory.</summary>
    /// <param name="openAction">The callback that supplies each open result.</param>
    public FakeFormListWorkspaceFactory(
        Func<WorkspaceOpenRequest, CancellationToken, ValueTask<EngineResult<IPluginWorkspace>>> openAction)
    {
        OpenAction = openAction;
    }

    /// <summary>Gets the requests received in call order.</summary>
    public List<WorkspaceOpenRequest> Requests { get; } = [];

    /// <inheritdoc />
    public ValueTask<EngineResult<IPluginWorkspace>> OpenAsync(
        WorkspaceOpenRequest request,
        CancellationToken cancellationToken = default)
    {
        Requests.Add(request);
        return OpenAction(request, cancellationToken);
    }
}
