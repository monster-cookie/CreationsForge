using Autofac;
using CreationsForge.Core.Engine;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInputs;
using CreationsForge.Core.Engine.NativeOutputs;
using CreationsForge.Core.Engine.Persistence;
using CreationsForge.Core.Engine.NativeWire;
using CreationsForge.Fallout4.Native;
using CreationsForge.Fallout4.Native.Wire;
using CreationsForge.Skyrim.Native;
using CreationsForge.Skyrim.Native.Wire;
using CreationsForge.Starfield.Native;
using CreationsForge.Starfield.Native.Edits;
using CreationsForge.Starfield.Native.Wire;
using Module = Autofac.Module;

namespace CreationsForge.Bootstrap.Composition;

/// <summary>Registers the complete shared native FormList engine for Starfield, Fallout 4, and Skyrim Special Edition.</summary>
public sealed class NativeEngineModule : Module
{
    /// <summary>Registers stateless native services and guarded save infrastructure; callers own every opened workspace lifetime.</summary>
    /// <param name="builder">The host container builder, which must also supply a Serilog logger.</param>
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterModule<EngineModule>();
        builder.RegisterType<NativeSourceInputLoader>().SingleInstance();
        builder.RegisterType<NativeOutputInputLoader>().SingleInstance();
        builder.RegisterType<OutputDirectoryLeaseProvider>()
            .As<IOutputDirectoryLeaseProvider>().SingleInstance();
        builder.RegisterType<WorkspaceSaveCoordinator>()
            .As<IWorkspaceSaveCoordinator>().SingleInstance();

        builder.RegisterType<StarfieldNativeSourceLoader>().SingleInstance();
        builder.RegisterType<StarfieldNativeOutputService>().SingleInstance();
        builder.RegisterType<StarfieldNativeEditService>().SingleInstance();
        builder.RegisterType<StarfieldNativeWriter>().SingleInstance();
        builder.RegisterType<StarfieldFormListGameAdapter>()
            .As<IFormListGameAdapter>().SingleInstance();
        builder.RegisterType<StarfieldFormListEditWireCodec>()
            .As<IFormListEditWireCodec>().SingleInstance();
        builder.RegisterType<StarfieldFormListEditWireSchemaCatalog>()
            .As<IFormListEditWireSchemaCatalog>().SingleInstance();

        builder.RegisterType<Fallout4NativeSourceLoader>().SingleInstance();
        builder.RegisterType<Fallout4NativeOutputService>().SingleInstance();
        builder.Register(context => context.Resolve<Fallout4NativeOutputService>().Inspector).SingleInstance();
        builder.RegisterType<Fallout4NativeEditService>().SingleInstance();
        builder.RegisterType<Fallout4NativeWriteService>().SingleInstance();
        builder.RegisterType<Fallout4FormListGameAdapter>()
            .As<IFormListGameAdapter>().SingleInstance();
        builder.RegisterType<Fallout4FormListEditWireCodec>()
            .As<IFormListEditWireCodec>().SingleInstance();
        builder.RegisterType<Fallout4FormListEditWireSchemaCatalog>()
            .As<IFormListEditWireSchemaCatalog>().SingleInstance();

        builder.RegisterType<SkyrimNativeSourceLoader>().SingleInstance();
        builder.RegisterType<SkyrimNativeOutputService>().SingleInstance();
        builder.Register(context => context.Resolve<SkyrimNativeOutputService>().Inspector).SingleInstance();
        builder.RegisterType<SkyrimNativeEditService>().SingleInstance();
        builder.RegisterType<SkyrimFormListGameAdapter>()
            .As<IFormListGameAdapter>().SingleInstance();
        builder.RegisterType<SkyrimFormListEditWireCodec>()
            .As<IFormListEditWireCodec>().SingleInstance();
        builder.RegisterType<SkyrimFormListEditWireSchemaCatalog>()
            .As<IFormListEditWireSchemaCatalog>().SingleInstance();
    }
}
