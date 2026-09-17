using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.PluginInputs;
using CreationsForge.Fallout4.PluginAdapter;
using CreationsForge.Fallout4.PluginAdapter.RecordInspection;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Fallout4;

/// <summary>Verifies Fallout 4 source-only FormList enumeration, contextual reads, and comparison inputs.</summary>
public sealed class Fallout4PluginSourceInspectionTests
{
    /// <summary>Verifies plugin and FormList summaries preserve plugin order, scopes, deletion, and chain-wide override counts.</summary>
    /// <returns>A task that completes after the plugin source lifetime is disposed.</returns>
    [Fact]
    public async Task ListOperations_PreservePluginOrderScopesAndOverrideCounts()
    {
        using var fixture = Fallout4PluginTestFixture.Create();
        var open = await CreateLoader().OpenAsync(fixture.CreateOpenRequest());
        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        await using var sources = open.Value!.Sources.ShouldBeOfType<Fallout4PluginSourceSet>();

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
        pluginValues.ShouldAllBe(plugin => plugin.RecordCount.HasValue);
        pluginValues.ShouldAllBe(plugin => plugin.UniqueRecordContributionCount.HasValue);
        pluginValues.Sum(plugin => plugin.RecordCount!.Value).ShouldBeGreaterThan(
            pluginValues.Sum(plugin => plugin.UniqueRecordContributionCount!.Value));

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
    /// <returns>A task that completes after contextual plugin reads are inspected.</returns>
    [Fact]
    public async Task ReadFormListContext_PreservesProvenanceAndReturnsDetachedTypedState()
    {
        using var fixture = Fallout4PluginTestFixture.Create();
        var open = await CreateLoader().OpenAsync(fixture.CreateOpenRequest());
        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        await using var sources = open.Value!.Sources.ShouldBeOfType<Fallout4PluginSourceSet>();

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

        var book = sources.ReadRecordContext(new ReferenceRequest(
            fixture.BookFormKey,
            RecordScope.Source), TestContext.Current.CancellationToken);
        book.Succeeded.ShouldBeTrue(book.Error?.Message);
        book.Value!.Context.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
        book.Value.Context.ContainingModKey.ShouldBe(fixture.SourceModKey);
        book.Value.RecordType.ShouldBe("Book");
        book.Value.Record.ShouldBeOfType<Book>();
        using var bookStream = new MemoryStream();
        using (var writer = new System.Text.Json.Utf8JsonWriter(bookStream))
        {
            new CreationsForge.Core.Engine.RecordInspection.MutagenMajorRecordInspector(
                typeof(Fallout4MajorRecord),
                "Mutagen.Bethesda.Fallout4/0.55.0-alpha.53")
                .WriteReadView(book.Value.Record!, writer, TestContext.Current.CancellationToken);
        }
        using var bookDocument = System.Text.Json.JsonDocument.Parse(bookStream.ToArray());
        bookDocument.RootElement.GetProperty("$type").GetString().ShouldBe(typeof(Book).FullName);

        var unresolved = sources.ReadFormListContext(new ReferenceRequest(
            new FormKey(fixture.SourceModKey, 0x0F01),
            RecordScope.Source), TestContext.Current.CancellationToken);
        unresolved.Value!.Context.Status.ShouldBe(ReferenceResolutionStatus.Unresolved);
        unresolved.Value.Context.ContainingModKey.ShouldBeNull();
        unresolved.Value.Record.ShouldBeNull();
    }

    /// <summary>Verifies missing item references are warnings and contextual reads feed precise plugin comparisons.</summary>
    /// <returns>A task that completes after warning and comparison inspection.</returns>
    [Fact]
    public async Task ReadAndCompare_ReportMissingItemsAndPrecisePluginChanges()
    {
        using var fixture = Fallout4PluginTestFixture.Create();
        var open = await CreateLoader().OpenAsync(fixture.CreateOpenRequest());
        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        await using var sources = open.Value!.Sources.ShouldBeOfType<Fallout4PluginSourceSet>();

        var missing = sources.ReadFormListContext(new ReferenceRequest(
            fixture.MissingReferenceListFormKey,
            RecordScope.Source), TestContext.Current.CancellationToken);

        missing.Succeeded.ShouldBeTrue(missing.Error?.Message);
        missing.Value!.Context.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
        missing.Value.Record.ShouldBeOfType<FormList>().Items.Select(item => item.FormKey)
            .ShouldBe([fixture.BookFormKey, FormKey.Null, fixture.MissingReferenceFormKey]);
        missing.Warnings.ShouldHaveSingleItem().Code.ShouldBe("missing-plugin-reference");
        missing.Warnings[0].Message.ShouldContain(fixture.MissingReferenceFormKey.ToString());
        missing.Warnings[0].Message.ShouldContain("Items[2]");

        var before = sources.ReadFormListContext(new ReferenceRequest(
            fixture.SourceListFormKey,
            RecordScope.AllContexts,
            fixture.SourceModKey), TestContext.Current.CancellationToken);
        var after = sources.ReadFormListContext(new ReferenceRequest(
            fixture.SourceListFormKey,
            RecordScope.WinningOverrides), TestContext.Current.CancellationToken);
        var changes = new Fallout4FormListInspector().Compare(
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

    /// <summary>Creates an isolated production loader for generated plugin fixtures.</summary>
    /// <returns>A Fallout 4 source loader with no installed-game inputs.</returns>
    private static Fallout4PluginSourceLoader CreateLoader()
    {
        return new Fallout4PluginSourceLoader(new PluginSourceInputLoader());
    }
}
