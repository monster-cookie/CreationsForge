namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Opens independently owned FormList workspaces from explicit native inputs.
/// </summary>
public interface IFormListWorkspaceFactory
{
    /// <summary>Opens a workspace without consulting an installed game load order.</summary>
    /// <param name="request">The explicit game, release, source, load-order, and resource inputs.</param>
    /// <param name="cancellationToken">A token that cancels opening and triggers cleanup of any acquired native state.</param>
    /// <returns>A successful independently owned workspace or a stable typed failure.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is observed before ownership is returned.</exception>
    ValueTask<EngineResult<IFormListWorkspace>> OpenAsync(
        WorkspaceOpenRequest request,
        CancellationToken cancellationToken = default);
}
