using Autofac;
using CreationsForge.Core.Engine;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.PluginInputs;
using CreationsForge.Core.Engine.PluginOutputs;
using CreationsForge.Core.Engine.Persistence;
using CreationsForge.Core.Engine.RecordWire;
using CreationsForge.Fallout4.PluginAdapter;
using CreationsForge.Fallout4.PluginAdapter.Wire;
using CreationsForge.Skyrim.PluginAdapter;
using CreationsForge.Skyrim.PluginAdapter.Wire;
using CreationsForge.Starfield.PluginAdapter;
using CreationsForge.Starfield.PluginAdapter.Edits;
using CreationsForge.Starfield.PluginAdapter.Wire;
using Module = Autofac.Module;

namespace CreationsForge.Bootstrap.Composition;

/// <summary>Registers the complete shared FormList engine for Starfield, Fallout 4, and Skyrim Special Edition.</summary>
public sealed class FormListEngineModule : Module
{
    /// <summary>Registers stateless engine services and guarded save infrastructure; callers own every opened workspace lifetime.</summary>
    /// <param name="builder">The host container builder, which must also supply a Serilog logger.</param>
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterModule<EngineModule>();
        builder.RegisterType<PluginSourceInputLoader>().SingleInstance();
        builder.RegisterType<PluginOutputInputLoader>().SingleInstance();
        builder.RegisterType<OutputDirectoryLeaseProvider>()
            .As<IOutputDirectoryLeaseProvider>().SingleInstance();
        builder.RegisterType<WorkspaceSaveCoordinator>()
            .As<IWorkspaceSaveCoordinator>().SingleInstance();

        builder.RegisterType<StarfieldPluginSourceLoader>().SingleInstance();
        builder.RegisterType<StarfieldPluginOutputService>().SingleInstance();
        builder.RegisterType<StarfieldRecordEditService>().SingleInstance();
        builder.RegisterType<StarfieldPluginWriter>().SingleInstance();
        builder.RegisterType<StarfieldFormListGameAdapter>()
            .As<IFormListGameAdapter>().SingleInstance();
        builder.RegisterType<StarfieldFormListEditWireCodec>()
            .As<IFormListEditWireCodec>().SingleInstance();
        builder.RegisterType<StarfieldFormListEditWireSchemaCatalog>()
            .As<IFormListEditWireSchemaCatalog>().SingleInstance();

        builder.RegisterType<Fallout4PluginSourceLoader>().SingleInstance();
        builder.RegisterType<Fallout4PluginOutputService>().SingleInstance();
        builder.Register(context => context.Resolve<Fallout4PluginOutputService>().Inspector).SingleInstance();
        builder.RegisterType<Fallout4RecordEditService>().SingleInstance();
        builder.RegisterType<Fallout4PluginWriteService>().SingleInstance();
        builder.RegisterType<Fallout4FormListGameAdapter>()
            .As<IFormListGameAdapter>().SingleInstance();
        builder.RegisterType<Fallout4FormListEditWireCodec>()
            .As<IFormListEditWireCodec>().SingleInstance();
        builder.RegisterType<Fallout4FormListEditWireSchemaCatalog>()
            .As<IFormListEditWireSchemaCatalog>().SingleInstance();

        builder.RegisterType<SkyrimPluginSourceLoader>().SingleInstance();
        builder.RegisterType<SkyrimPluginOutputService>().SingleInstance();
        builder.Register(context => context.Resolve<SkyrimPluginOutputService>().Inspector).SingleInstance();
        builder.RegisterType<SkyrimRecordEditService>().SingleInstance();
        builder.RegisterType<SkyrimFormListGameAdapter>()
            .As<IFormListGameAdapter>().SingleInstance();
        builder.RegisterType<SkyrimFormListEditWireCodec>()
            .As<IFormListEditWireCodec>().SingleInstance();
        builder.RegisterType<SkyrimFormListEditWireSchemaCatalog>()
            .As<IFormListEditWireSchemaCatalog>().SingleInstance();
    }
}
