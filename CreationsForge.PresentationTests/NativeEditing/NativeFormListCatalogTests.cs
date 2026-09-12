using CreationsForge.Core.Engine.NativeWire;
using CreationsForge.Core.Enums;
using CreationsForge.Fallout4.Native.Wire;
using CreationsForge.NativeEditing;
using CreationsForge.Services;
using CreationsForge.Skyrim.Native.Wire;
using CreationsForge.Starfield.Native.Wire;
using Mutagen.Bethesda;
using Shouldly;

namespace CreationsForge.PresentationTests.NativeEditing;

/// <summary>Verifies exact three-game catalog resolution, command admission, and presentation metadata.</summary>
public sealed class NativeFormListCatalogTests
{
    /// <summary>Verifies every exact command has one seed policy and presentation entry without an invented clear-components command.</summary>
    [Fact]
    public void ResolveAll_ProductionCatalogs_AdmitsExactlyFiftyOneCommands()
    {
        var contexts = CreateContexts();

        contexts.SelectMany(context => NativeFormListCommandPresentationCatalog.Resolve(context).Value!).Count().ShouldBe(51);
        contexts.SelectMany(context => NativeFormListCommandSeedCatalog.ResolveAll(context).Value!).Count().ShouldBe(51);
        NativeFormListCommandPresentationCatalog.Resolve(contexts[0]).Value!.Count.ShouldBe(23);
        NativeFormListCommandPresentationCatalog.Resolve(contexts[1]).Value!.Count.ShouldBe(15);
        NativeFormListCommandPresentationCatalog.Resolve(contexts[2]).Value!.Count.ShouldBe(13);
        contexts.SelectMany(context => NativeFormListCommandPresentationCatalog.Resolve(context).Value!)
            .Select(entry => entry.CommandName)
            .ShouldNotContain("starfield.form-list.clear-components");
    }

    /// <summary>Verifies the resolver requires one exact codec and catalog for the requested pair.</summary>
    [Fact]
    public void Resolve_ExactProductionRegistrations_ReturnsMatchingWholeIdentity()
    {
        var catalogs = CreateCatalogs();
        var codecs = CreateCodecs();
        var resolver = new NativeFormListWireCatalogResolver(codecs, catalogs);

        var result = resolver.Resolve(SupportedGame.Starfield, GameRelease.Starfield);

        result.Succeeded.ShouldBeTrue();
        result.Value!.SchemaCatalog.ShouldBeSameAs(catalogs[0]);
        result.Value.Codec.ShouldBeSameAs(codecs[0]);
        result.Value.Identity.CatalogId.ShouldBe(catalogs[0].Identity.CatalogId);
    }

    /// <summary>Verifies duplicate exact codec registrations fail closed.</summary>
    [Fact]
    public void Resolve_DuplicateCodecRegistration_FailsClosed()
    {
        var catalogs = CreateCatalogs();
        var codecs = CreateCodecs().Concat(new[] { new StarfieldFormListEditWireCodec() });

        var result = new NativeFormListWireCatalogResolver(codecs, catalogs)
            .Resolve(SupportedGame.Starfield, GameRelease.Starfield);

        result.Succeeded.ShouldBeFalse();
        result.Error!.Message.ShouldContain("found 2 codec(s) and 1 catalog(s)");
    }

    /// <summary>Creates exact production contexts in Starfield, Fallout 4, and Skyrim order.</summary>
    internal static IReadOnlyList<NativeFormListWireCatalogContext> CreateContexts()
    {
        var catalogs = CreateCatalogs();
        var codecs = CreateCodecs();
        return Array.AsReadOnly(new[]
        {
            new NativeFormListWireCatalogContext(catalogs[0], codecs[0]),
            new NativeFormListWireCatalogContext(catalogs[1], codecs[1]),
            new NativeFormListWireCatalogContext(catalogs[2], codecs[2]),
        });
    }

    /// <summary>Creates the exact production schema catalogs.</summary>
    private static IFormListEditWireSchemaCatalog[] CreateCatalogs()
    {
        return
        [
            new StarfieldFormListEditWireSchemaCatalog(),
            new Fallout4FormListEditWireSchemaCatalog(),
            new SkyrimFormListEditWireSchemaCatalog(),
        ];
    }

    /// <summary>Creates the exact production codecs.</summary>
    private static IFormListEditWireCodec[] CreateCodecs()
    {
        return
        [
            new StarfieldFormListEditWireCodec(),
            new Fallout4FormListEditWireCodec(),
            new SkyrimFormListEditWireCodec(),
        ];
    }
}
