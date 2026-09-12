using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInputs;
using CreationsForge.Fallout4.Native;
using CreationsForge.Fallout4.Native.NativeInspection;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Fallout4;

/// <summary>Verifies Fallout 4 source-only FormList enumeration, contextual reads, and comparison inputs.</summary>
public sealed class Fallout4NativeSourceInspectionTests
{
    /// <summary>Verifies plugin and FormList summaries preserve native order, scopes, deletion, and chain-wide override counts.</summary>
    /// <returns>A task that completes after the native source lifetime is disposed.</returns>
    [Fact]
    public async Task ListOperations_PreserveNativeOrderScopesAndOverrideCounts()
    {
        using var fixture = Fallout4NativeTestFixture.Create();
        var open = await CreateLoader().OpenAsync(fixture.CreateOpenRequest());
        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        await using var sources = open.Value!.Sources.ShouldBeOfType<Fallout4NativeSourceSet>();

        var plugins = sources.ListPlugins(TestContext.Current.CancellationToken);
        plugins.Succeeded.ShouldBeTrue(plugins.Error?.Message);
        plugins.WorkspaceId.ShouldBe(sources.WorkspaceId);
        plugins.ResultRevision.ShouldBe(sources.Revision);
        var pluginValues = plugins.Value.ShouldNotBeNull();
        pluginValues.Select(plugin => plugin.ModKey)
            .ShouldBe([fixture.SourceModKey, fixture.FullModKey, fixture.PatchModKey]);
        pluginValues.Select(plugin => plugin.LoadOrderIndex).ShouldBe([0, 1, 2]);
        pluginValues.Select(plugin => plugin.Role)
            .ShouldBe([PluginRole.Source, PluginRole.LoadOrder, PluginRole.LoadOrder]);

        var source = sources.ListFormLists(RecordScope.Source, TestContext.Current.CancellationToken);
        var all = sources.ListFormLists(RecordScope.AllContexts, TestContext.Current.CancellationToken);
        var winning = sources.ListFormLists(RecordScope.WinningOverrides, TestContext.Current.CancellationToken);
        var staged = sources.ListFormLists(RecordScope.StagedOutput, TestContext.Current.CancellationToken);

        source.Succeeded.ShouldBeTrue(source.Error?.Message);
        var sourceValues = source.Value.ShouldNotBeNull();
        sourceValues.Select(summary => summary.FormKey).ShouldBe(
        [
            fixture.SourceListFormKey,
            fixture.DeletedListFormKey,
            fixture.MissingReferenceListFormKey,
        ]);
        sourceValues.Select(summary => summary.OverrideCount).ShouldBe([1, 1, 0]);
        sourceValues.ShouldAllBe(summary => summary.Scope == RecordScope.Source);

        all.Succeeded.ShouldBeTrue(all.Error?.Message);
        var allValues = all.Value.ShouldNotBeNull();
        allValues.Select(summary => summary.FormKey).ShouldBe(
        [
            fixture.SourceListFormKey,
            fixture.DeletedListFormKey,
            fixture.MissingReferenceListFormKey,
            fixture.FullListFormKey,
            fixture.SourceListFormKey,
            fixture.DeletedListFormKey,
        ]);
        allValues.Select(summary => summary.OverrideCount).ShouldBe([1, 1, 0, 0, 1, 1]);
        allValues.Select(summary => summary.ContainingModKey).ShouldBe(
        [
            fixture.SourceModKey,
            fixture.SourceModKey,
            fixture.SourceModKey,
            fixture.FullModKey,
            fixture.PatchModKey,
            fixture.PatchModKey,
        ]);

        winning.Succeeded.ShouldBeTrue(winning.Error?.Message);
        var winningValues = winning.Value.ShouldNotBeNull();
        winningValues.Select(summary => summary.FormKey).ShouldBe(
        [
            fixture.MissingReferenceListFormKey,
            fixture.FullListFormKey,
            fixture.SourceListFormKey,
            fixture.DeletedListFormKey,
        ]);
        winningValues.Select(summary => summary.ContainingModKey).ShouldBe(
        [
            fixture.SourceModKey,
            fixture.FullModKey,
            fixture.PatchModKey,
            fixture.PatchModKey,
        ]);
        winningValues[^1].EditorId.ShouldBeNull();
        winningValues[^1].OverrideCount.ShouldBe(1);
        staged.Succeeded.ShouldBeTrue(staged.Error?.Message);
        staged.Value.ShouldNotBeNull().ShouldBeEmpty();
    }

    /// <summary>Verifies exact context selection, detached state, deletion, ambiguity, and non-FormList handling.</summary>
    /// <returns>A task that completes after contextual native reads are inspected.</returns>
    [Fact]
    public async Task ReadFormListContext_PreservesProvenanceAndReturnsDetachedTypedState()
    {
        using var fixture = Fallout4NativeTestFixture.Create();
        var open = await CreateLoader().OpenAsync(fixture.CreateOpenRequest());
        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        await using var sources = open.Value!.Sources.ShouldBeOfType<Fallout4NativeSourceSet>();

        var ambiguous = sources.ReadFormListContext(new ReferenceRequest(
            fixture.SourceListFormKey,
            RecordScope.AllContexts), TestContext.Current.CancellationToken);
        ambiguous.Succeeded.ShouldBeTrue(ambiguous.Error?.Message);
        ambiguous.Value!.Context.Status.ShouldBe(ReferenceResolutionStatus.Ambiguous);
        ambiguous.Value.Context.ContainingModKey.ShouldBeNull();
        ambiguous.Value.Record.ShouldBeNull();

        var selectedSource = sources.ReadFormListContext(new ReferenceRequest(
            fixture.SourceListFormKey,
            RecordScope.AllContexts,
            fixture.SourceModKey), TestContext.Current.CancellationToken);
        selectedSource.Succeeded.ShouldBeTrue(selectedSource.Error?.Message);
        selectedSource.Value!.Context.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
        selectedSource.Value.Context.ContainingModKey.ShouldBe(fixture.SourceModKey);
        selectedSource.Value.Context.Path.ShouldBe(fixture.SourcePluginPath);
        selectedSource.Value.Context.LoadOrderIndex.ShouldBe(0);
        selectedSource.Value.Context.Role.ShouldBe(PluginRole.Source);
        selectedSource.Value.RecordType.ShouldBe("FormList");
        selectedSource.Value.Record.ShouldBeOfType<FormList>();
        var sourceRecord = (FormList)selectedSource.Value.Record!;
        sourceRecord.Items.Select(item => item.FormKey)
            .ShouldBe([fixture.BookFormKey, fixture.KeywordFormKey]);
        sourceRecord.EditorID = "CallerMutation";
        sourceRecord.Items.Clear();

        var repeated = sources.ReadFormListContext(new ReferenceRequest(
            fixture.SourceListFormKey,
            RecordScope.Source), TestContext.Current.CancellationToken);
        repeated.Value!.Record.ShouldBeOfType<FormList>();
        ((FormList)repeated.Value.Record!).EditorID.ShouldBe("SharedListSmall");
        ((FormList)repeated.Value.Record!).Items.Count.ShouldBe(2);

        var winning = sources.ReadFormListContext(new ReferenceRequest(
            fixture.SourceListFormKey,
            RecordScope.WinningOverrides), TestContext.Current.CancellationToken);
        winning.Value!.Context.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
        winning.Value.Context.ContainingModKey.ShouldBe(fixture.PatchModKey);
        winning.Value.Context.Path.ShouldBe(fixture.PatchPluginPath);
        winning.Value.Context.LoadOrderIndex.ShouldBe(2);
        ((FormList)winning.Value.Record!).EditorID.ShouldBe("SharedListOverride");

        var deleted = sources.ReadFormListContext(new ReferenceRequest(
            fixture.DeletedListFormKey,
            RecordScope.WinningOverrides), TestContext.Current.CancellationToken);
        deleted.Value!.Context.Status.ShouldBe(ReferenceResolutionStatus.Deleted);
        deleted.Value.Context.ContainingModKey.ShouldBe(fixture.PatchModKey);
        deleted.Value.Record.ShouldBeOfType<FormList>();
        deleted.Value.Record!.IsDeleted.ShouldBeTrue();
        var legacyDeleted = sources.Resolve(new ReferenceRequest(
            fixture.DeletedListFormKey,
            RecordScope.WinningOverrides), TestContext.Current.CancellationToken);
        legacyDeleted.Value!.Status.ShouldBe(ReferenceResolutionStatus.Deleted);
        legacyDeleted.Value.Record.ShouldBeNull();

        var unsupported = sources.ReadFormListContext(new ReferenceRequest(
            fixture.BookFormKey,
            RecordScope.Source), TestContext.Current.CancellationToken);
        unsupported.Value!.Context.Status.ShouldBe(ReferenceResolutionStatus.Unsupported);
        unsupported.Value.Context.ContainingModKey.ShouldBe(fixture.SourceModKey);
        unsupported.Value.RecordType.ShouldBe("Book");
        unsupported.Value.Record.ShouldBeNull();

        var unresolved = sources.ReadFormListContext(new ReferenceRequest(
            new FormKey(fixture.SourceModKey, 0x0F01),
            RecordScope.Source), TestContext.Current.CancellationToken);
        unresolved.Value!.Context.Status.ShouldBe(ReferenceResolutionStatus.Unresolved);
        unresolved.Value.Context.ContainingModKey.ShouldBeNull();
        unresolved.Value.Record.ShouldBeNull();
    }

    /// <summary>Verifies missing item references are warnings and contextual reads feed precise native comparisons.</summary>
    /// <returns>A task that completes after warning and comparison inspection.</returns>
    [Fact]
    public async Task ReadAndCompare_ReportMissingItemsAndPreciseNativeChanges()
    {
        using var fixture = Fallout4NativeTestFixture.Create();
        var open = await CreateLoader().OpenAsync(fixture.CreateOpenRequest());
        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        await using var sources = open.Value!.Sources.ShouldBeOfType<Fallout4NativeSourceSet>();

        var missing = sources.ReadFormListContext(new ReferenceRequest(
            fixture.MissingReferenceListFormKey,
            RecordScope.Source), TestContext.Current.CancellationToken);

        missing.Succeeded.ShouldBeTrue(missing.Error?.Message);
        missing.Value!.Context.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
        missing.Value.Record.ShouldBeOfType<FormList>().Items.Select(item => item.FormKey)
            .ShouldBe([fixture.BookFormKey, FormKey.Null, fixture.MissingReferenceFormKey]);
        missing.Warnings.ShouldHaveSingleItem().Code.ShouldBe("missing-native-reference");
        missing.Warnings[0].Message.ShouldContain(fixture.MissingReferenceFormKey.ToString());
        missing.Warnings[0].Message.ShouldContain("Items[2]");

        var before = sources.ReadFormListContext(new ReferenceRequest(
            fixture.SourceListFormKey,
            RecordScope.AllContexts,
            fixture.SourceModKey), TestContext.Current.CancellationToken);
        var after = sources.ReadFormListContext(new ReferenceRequest(
            fixture.SourceListFormKey,
            RecordScope.WinningOverrides), TestContext.Current.CancellationToken);
        var changes = new Fallout4FormListNativeInspector().Compare(
            before.Value!.Record,
            after.Value!.Record,
            TestContext.Current.CancellationToken);

        changes.Select(change => change.FieldIdentifier).ShouldBe(
        [
            "EditorID",
            "Name",
        ]);
        changes.ShouldAllBe(change => change.Kind == SemanticChangeKind.ValueChanged);
    }

    /// <summary>Creates an isolated production loader for generated native fixtures.</summary>
    /// <returns>A Fallout 4 source loader with no installed-game inputs.</returns>
    private static Fallout4NativeSourceLoader CreateLoader()
    {
        return new Fallout4NativeSourceLoader(new NativeSourceInputLoader());
    }
}
