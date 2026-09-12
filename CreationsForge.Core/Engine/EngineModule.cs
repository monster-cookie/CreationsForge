using Autofac;
using CreationsForge.Core.Engine.Contracts;
using Module = Autofac.Module;

namespace CreationsForge.Core.Engine;

/// <summary>
/// Registers the UI-neutral FormList workspace factory without legacy Core infrastructure.
/// </summary>
public sealed class EngineModule : Module
{
    /// <summary>Adds the transient workspace factory registration to an Autofac container.</summary>
    /// <param name="builder">The Autofac container builder receiving the engine registration.</param>
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<FormListWorkspaceFactory>()
            .As<IFormListWorkspaceFactory>()
            .InstancePerDependency();
    }
}
