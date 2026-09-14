using CreationsForge.Core.Engine.RecordInspection;
using Moq;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.RecordInspection;

/// <summary>Verifies shared plugin semantic equality for owner-derived form-link-or-index unions.</summary>
public sealed class RecordSemanticComparerTests
{
    /// <summary>Verifies nullable union wrappers preserve their exact presence state.</summary>
    [Fact]
    public void FormLinkOrIndexEquals_PreservesWrapperPresence()
    {
        var value = CreateUnion(useAliases: false, usePackageData: false);

        RecordSemanticComparer.FormLinkOrIndexEquals<IMajorRecordGetter>(null, null).ShouldBeTrue();
        RecordSemanticComparer.FormLinkOrIndexEquals(null, value).ShouldBeFalse();
        RecordSemanticComparer.FormLinkOrIndexEquals(value, null).ShouldBeFalse();
    }

    /// <summary>Verifies link mode compares the active plugin FormKey while treating both managed representations of a plugin null link as equal.</summary>
    [Fact]
    public void FormLinkOrIndexEquals_LinkMode_UsesPluginFormKeySemantics()
    {
        var before = CreateUnion(useAliases: false, usePackageData: false);
        var after = CreateUnion(useAliases: false, usePackageData: false);
        before.Link.FormKeyNullable = null;
        after.Link.FormKey = FormKey.Null;
        before.Index = 17;
        after.Index = 18;

        RecordSemanticComparer.FormLinkEquals(before.Link, after.Link).ShouldBeFalse();
        RecordSemanticComparer.FormLinkOrIndexEquals(before, after).ShouldBeTrue();

        var firstLink = new FormKey(ModKey.FromNameAndExtension("Source.esm"), 0x800);
        var secondLink = new FormKey(ModKey.FromNameAndExtension("Source.esm"), 0x801);
        before.Link.FormKey = firstLink;
        after.Link.FormKey = firstLink;
        RecordSemanticComparer.FormLinkOrIndexEquals(before, after).ShouldBeTrue();

        after.Link.FormKey = secondLink;
        RecordSemanticComparer.FormLinkOrIndexEquals(before, after).ShouldBeFalse();
    }

    /// <summary>Verifies alias and package-data modes compare nullable indices exactly and ignore their inactive link projection.</summary>
    /// <param name="useAliases">Whether the union owner selects alias-index mode.</param>
    /// <param name="usePackageData">Whether the union owner selects package-data-index mode.</param>
    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void FormLinkOrIndexEquals_IndexModes_CompareNullableIndex(
        bool useAliases,
        bool usePackageData)
    {
        var before = CreateUnion(useAliases, usePackageData);
        var after = CreateUnion(useAliases, usePackageData);
        before.Link.FormKey = new FormKey(ModKey.FromNameAndExtension("Source.esm"), 0x800);
        after.Link.FormKey = new FormKey(ModKey.FromNameAndExtension("Source.esm"), 0x801);
        before.Index = null;
        after.Index = 0;

        RecordSemanticComparer.FormLinkOrIndexEquals(before, after).ShouldBeFalse();

        before.Index = 17;
        after.Index = 17;
        RecordSemanticComparer.FormLinkOrIndexEquals(before, after).ShouldBeTrue();

        after.Index = 18;
        RecordSemanticComparer.FormLinkOrIndexEquals(before, after).ShouldBeFalse();
    }

    /// <summary>Verifies equal active values do not collapse link, alias, and package-data owner modes.</summary>
    [Fact]
    public void FormLinkOrIndexEquals_PreservesExactOwnerMode()
    {
        var link = CreateUnion(useAliases: false, usePackageData: false);
        var alias = CreateUnion(useAliases: true, usePackageData: false);
        var packageData = CreateUnion(useAliases: false, usePackageData: true);
        alias.Index = 17;
        packageData.Index = 17;

        RecordSemanticComparer.FormLinkOrIndexEquals(link, alias).ShouldBeFalse();
        RecordSemanticComparer.FormLinkOrIndexEquals(alias, packageData).ShouldBeFalse();
        RecordSemanticComparer.FormLinkOrIndexEquals(packageData, link).ShouldBeFalse();
    }

    /// <summary>Creates a mutable union backed by fixed owner-mode flags for focused semantic comparison.</summary>
    /// <param name="useAliases">Whether the owner selects alias-index mode.</param>
    /// <param name="usePackageData">Whether the owner selects package-data-index mode.</param>
    /// <returns>A mutable union whose active representation follows the supplied owner mode.</returns>
    private static FormLinkOrIndex<IMajorRecordGetter> CreateUnion(
        bool useAliases,
        bool usePackageData)
    {
        var owner = new Mock<IFormLinkOrIndexFlagGetter>(MockBehavior.Strict);
        owner.SetupGet(static value => value.UseAliases).Returns(useAliases);
        owner.SetupGet(static value => value.UsePackageData).Returns(usePackageData);
        return new FormLinkOrIndex<IMajorRecordGetter>(owner.Object);
    }
}
