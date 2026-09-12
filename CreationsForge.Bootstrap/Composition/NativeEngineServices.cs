using Autofac;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeWire;

namespace CreationsForge.Bootstrap.Composition;

/// <summary>Owns host-level native services while each opened workspace retains its own separate native record lifetime.</summary>
public sealed class NativeEngineServices : IAsyncDisposable
{
    /// <summary>The owned container, cleared when asynchronous service disposal begins.</summary>
    private IContainer? Container;

    /// <summary>Initializes an owned native service lifetime from the resolved Bootstrap graph.</summary>
    /// <param name="container">The container to dispose after all caller-owned workspaces have been closed.</param>
    /// <param name="workspaceFactory">The complete three-game workspace factory.</param>
    /// <param name="saveCoordinator">The shared guarded save and recovery coordinator.</param>
    /// <param name="formListEditWireCodecs">The exact per-game typed edit codecs.</param>
    /// <param name="formListEditWireSchemaCatalogs">The immutable per-game schema catalogs.</param>
    internal NativeEngineServices(
        IContainer container,
        IFormListWorkspaceFactory workspaceFactory,
        IWorkspaceSaveCoordinator saveCoordinator,
        IReadOnlyList<IFormListEditWireCodec> formListEditWireCodecs,
        IReadOnlyList<IFormListEditWireSchemaCatalog> formListEditWireSchemaCatalogs)
    {
        Container = container;
        WorkspaceFactory = workspaceFactory;
        SaveCoordinator = saveCoordinator;
        FormListEditWireCodecs = formListEditWireCodecs;
        FormListEditWireSchemaCatalogs = formListEditWireSchemaCatalogs;
    }

    /// <summary>Gets the factory for independent workspaces, usable while this service lifetime remains open.</summary>
    public IFormListWorkspaceFactory WorkspaceFactory { get; }

    /// <summary>Gets the coordinator for evidence-based save inspection and explicit repair while this lifetime remains open.</summary>
    public IWorkspaceSaveCoordinator SaveCoordinator { get; }

    /// <summary>Gets the complete exact per-game typed FormList edit codecs.</summary>
    public IReadOnlyList<IFormListEditWireCodec> FormListEditWireCodecs { get; }

    /// <summary>Gets the complete immutable per-game FormList edit schema catalogs.</summary>
    public IReadOnlyList<IFormListEditWireSchemaCatalog> FormListEditWireSchemaCatalogs { get; }

    /// <summary>Releases the owned service graph; callers must first close every workspace and finish outstanding recovery operations.</summary>
    /// <returns>The asynchronous container disposal, or a completed operation when disposal has already begun.</returns>
    public ValueTask DisposeAsync()
    {
        return Interlocked.Exchange(ref Container, null)?.DisposeAsync() ?? ValueTask.CompletedTask;
    }
}
