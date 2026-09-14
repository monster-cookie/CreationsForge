using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.PluginInputs;
using CreationsForge.Skyrim.PluginAdapter;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Skyrim;

/// <summary>
/// Verifies Skyrim's plugin, FormList enumeration, and contextual FormList read facades.
/// </summary>
public sealed class SkyrimFormListReadingTests
{
    /// <summary>
    /// Verifies plugin and FormList enumeration preserve supplied order, plugin group order, provenance, winners, and override counts.
    /// </summary>
    /// <returns>A task that completes after the plugin source lifetime is disposed.</returns>
    [Fact]
    public async Task ListFacades_ReturnDeterministicScopesProvenanceAndOverrideCounts()
    {
        using var fixture = SkyrimPluginTestFixture.Create();
        var sources = await OpenAsync(fixture);
        try
        {
            var plugins = sources.ListPlugins(TestContext.Current.CancellationToken);
            var source = sources.ListFormLists(RecordScope.Source, TestContext.Current.CancellationToken);
            var all = sources.ListFormLists(RecordScope.AllContexts, TestContext.Current.CancellationToken);
            var winning = sources.ListFormLists(RecordScope.WinningOverrides, TestContext.Current.CancellationToken);
            var staged = sources.ListFormLists(RecordScope.StagedOutput, TestContext.Current.CancellationToken);

            plugins.Succeeded.ShouldBeTrue(plugins.Error?.Message);
            plugins.WorkspaceId.ShouldBe(sources.WorkspaceId);
            plugins.ResultRevision.ShouldBe(sources.Revision);
            var pluginValues = plugins.Value.ShouldNotBeNull();
            pluginValues.Select(plugin => plugin.ModKey)
                .ShouldBe([fixture.SourceModKey, fixture.FullModKey, fixture.PatchModKey]);
            pluginValues.Select(plugin => plugin.LoadOrderIndex).ShouldBe([0, 1, 2]);
            pluginValues.Select(plugin => plugin.Role)
                .ShouldBe([PluginRole.Source, PluginRole.LoadOrder, PluginRole.LoadOrder]);
            pluginValues.Select(plugin => plugin.Path)
                .ShouldBe(fixture.OrderedPluginPaths);
            pluginValues.ShouldAllBe(plugin => plugin.RecordCount.HasValue);
            pluginValues.ShouldAllBe(plugin => plugin.UniqueRecordContributionCount.HasValue);
            pluginValues.Sum(plugin => plugin.RecordCount!.Value).ShouldBeGreaterThan(
                pluginValues.Sum(plugin => plugin.UniqueRecordContributionCount!.Value));

            source.Succeeded.ShouldBeTrue(source.Error?.Message);
            var sourceValues = source.Value.ShouldNotBeNull();
            sourceValues.Select(summary => summary.FormKey)
                .ShouldBe([fixture.SourceListFormKey, fixture.DeletedListFormKey]);
            sourceValues.ShouldAllBe(summary => summary.Scope == RecordScope.Source);
            sourceValues.ShouldAllBe(summary => summary.ContainingModKey == fixture.SourceModKey);
            sourceValues.ShouldAllBe(summary => summary.SourcePath == fixture.SourcePluginPath);
            sourceValues.ShouldAllBe(summary => summary.Role == PluginRole.Source);
            sourceValues.ShouldAllBe(summary => summary.OverrideCount == 1);

            all.Succeeded.ShouldBeTrue(all.Error?.Message);
            var allValues = all.Value.ShouldNotBeNull();
            allValues.Select(summary => SummaryIdentity(summary)).ShouldBe(
            [
                $"{fixture.SourceListFormKey}|{fixture.SourceModKey}",
                $"{fixture.DeletedListFormKey}|{fixture.SourceModKey}",
                $"{fixture.FullListFormKey}|{fixture.FullModKey}",
                $"{fixture.SourceListFormKey}|{fixture.PatchModKey}",
                $"{fixture.DeletedListFormKey}|{fixture.PatchModKey}",
            ]);
            allValues.Where(summary => summary.FormKey == fixture.SourceListFormKey)
                .ShouldAllBe(summary => summary.OverrideCount == 1);
            allValues.Where(summary => summary.FormKey == fixture.DeletedListFormKey)
                .ShouldAllBe(summary => summary.OverrideCount == 1);
            allValues.Single(summary => summary.FormKey == fixture.FullListFormKey)
                .OverrideCount.ShouldBe(0);

            winning.Succeeded.ShouldBeTrue(winning.Error?.Message);
            var winningValues = winning.Value.ShouldNotBeNull();
            winningValues.Select(summary => SummaryIdentity(summary)).ShouldBe(
            [
                $"{fixture.FullListFormKey}|{fixture.FullModKey}",
                $"{fixture.SourceListFormKey}|{fixture.PatchModKey}",
                $"{fixture.DeletedListFormKey}|{fixture.PatchModKey}",
            ]);
            winningValues.Select(summary => summary.OverrideCount).ShouldBe([0, 1, 1]);

            staged.Succeeded.ShouldBeTrue(staged.Error?.Message);
            staged.Value.ShouldBeEmpty();
        }
        finally
        {
            await sources.DisposeAsync();
        }
    }

    /// <summary>
    /// Verifies contextual reads distinguish ambiguity, deletion, unsupported families, and unresolved identities while retaining exact provenance.
    /// </summary>
    /// <returns>A task that completes after detached-copy and missing-reference checks.</returns>
    [Fact]
    public async Task ReadFormListContext_ReturnsDetachedSelectionAndMissingReferenceWarnings()
    {
        using var fixture = SkyrimPluginTestFixture.Create();
        var sources = await OpenAsync(fixture);
        try
        {
            var sourceRequest = new ReferenceRequest(fixture.SourceListFormKey, RecordScope.Source);
            var source = sources.ReadFormListContext(sourceRequest, TestContext.Current.CancellationToken);

            source.Succeeded.ShouldBeTrue(source.Error?.Message);
            source.WorkspaceId.ShouldBe(sources.WorkspaceId);
            source.ResultRevision.ShouldBe(sources.Revision);
            source.Value!.Context.Selection.ShouldBeSameAs(sourceRequest);
            source.Value.Context.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
            source.Value.Context.ContainingModKey.ShouldBe(fixture.SourceModKey);
            source.Value.Context.Path.ShouldBe(fixture.SourcePluginPath);
            source.Value.Context.LoadOrderIndex.ShouldBe(0);
            source.Value.Context.Role.ShouldBe(PluginRole.Source);
            source.Value.RecordType.ShouldBe("FormList");
            var detached = source.Value.Record.ShouldBeOfType<FormList>();
            detached.Items.Select(item => item.FormKey).ShouldBe(
                [fixture.BookFormKey, fixture.MissingFormKey, fixture.KeywordFormKey, fixture.MissingFormKey]);
            source.Warnings.Select(warning => warning.Code)
                .ShouldBe(["missing-plugin-reference", "missing-plugin-reference"]);
            source.Warnings[0].Message.ShouldContain(fixture.MissingFormKey.ToString());
            source.Warnings[0].Message.ShouldContain("Items[1]");
            source.Warnings[1].Message.ShouldContain(fixture.MissingFormKey.ToString());
            source.Warnings[1].Message.ShouldContain("Items[3]");

            detached.EditorID = "Changed detached copy";
            detached.Items.Clear();
            var repeated = sources.ReadFormListContext(sourceRequest, TestContext.Current.CancellationToken);
            repeated.Value!.Record.ShouldBeOfType<FormList>().EditorID.ShouldBe("SharedListSmall");
            repeated.Value.Record.ShouldBeOfType<FormList>().Items.Count.ShouldBe(4);
            repeated.Value.Record.ShouldNotBeSameAs(detached);

            var ambiguous = sources.ReadFormListContext(
                new ReferenceRequest(fixture.SourceListFormKey, RecordScope.AllContexts),
                TestContext.Current.CancellationToken);
            ambiguous.Succeeded.ShouldBeTrue(ambiguous.Error?.Message);
            ambiguous.Value!.Context.Status.ShouldBe(ReferenceResolutionStatus.Ambiguous);
            ambiguous.Value.Context.ContainingModKey.ShouldBeNull();
            ambiguous.Value.Record.ShouldBeNull();

            var explicitPatch = sources.ReadFormListContext(
                new ReferenceRequest(fixture.SourceListFormKey, RecordScope.AllContexts, fixture.PatchModKey),
                TestContext.Current.CancellationToken);
            explicitPatch.Succeeded.ShouldBeTrue(explicitPatch.Error?.Message);
            explicitPatch.Value!.Context.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
            explicitPatch.Value.Context.ContainingModKey.ShouldBe(fixture.PatchModKey);
            explicitPatch.Value.Context.Path.ShouldBe(fixture.PatchPluginPath);
            explicitPatch.Value.Record.ShouldBeOfType<FormList>().EditorID.ShouldBe("SharedListOverride");

            var deleted = sources.ReadFormListContext(
                new ReferenceRequest(fixture.DeletedListFormKey, RecordScope.WinningOverrides),
                TestContext.Current.CancellationToken);
            deleted.Succeeded.ShouldBeTrue(deleted.Error?.Message);
            deleted.Value!.Context.Status.ShouldBe(ReferenceResolutionStatus.Deleted);
            deleted.Value.Context.ContainingModKey.ShouldBe(fixture.PatchModKey);
            deleted.Value.Record.ShouldBeOfType<FormList>().IsDeleted.ShouldBeTrue();

            var unresolved = sources.ReadFormListContext(
                new ReferenceRequest(new FormKey(fixture.SourceModKey, 0x0FFE), RecordScope.WinningOverrides),
                TestContext.Current.CancellationToken);
            unresolved.Succeeded.ShouldBeTrue(unresolved.Error?.Message);
            unresolved.Value!.Context.Status.ShouldBe(ReferenceResolutionStatus.Unresolved);
            unresolved.Value.Record.ShouldBeNull();

            var otherFamily = sources.ReadFormListContext(
                new ReferenceRequest(fixture.BookFormKey, RecordScope.Source),
                TestContext.Current.CancellationToken);
            otherFamily.Succeeded.ShouldBeTrue(otherFamily.Error?.Message);
            otherFamily.Value!.Context.Status.ShouldBe(ReferenceResolutionStatus.Unsupported);
            otherFamily.Value.Context.ContainingModKey.ShouldBe(fixture.SourceModKey);
            otherFamily.Value.Record.ShouldBeNull();
        }
        finally
        {
            await sources.DisposeAsync();
        }
    }

    /// <summary>
    /// Verifies the new read facades honor pre-cancellation and return typed failures after source disposal.
    /// </summary>
    /// <returns>A task that completes after the disposed-state checks.</returns>
    [Fact]
    public async Task FormListFacades_PropagateCancellationAndRejectDisposedSources()
    {
        using var fixture = SkyrimPluginTestFixture.Create();
        var sources = await OpenAsync(fixture);
        using var cancellationSource = new CancellationTokenSource();
        cancellationSource.Cancel();

        Should.Throw<OperationCanceledException>(() => sources.ListPlugins(cancellationSource.Token));
        Should.Throw<OperationCanceledException>(() => sources.ListFormLists(RecordScope.Source, cancellationSource.Token));
        Should.Throw<OperationCanceledException>(() => sources.ReadFormListContext(
            new ReferenceRequest(fixture.SourceListFormKey, RecordScope.Source),
            cancellationSource.Token));

        await sources.DisposeAsync();
        var plugins = sources.ListPlugins(TestContext.Current.CancellationToken);
        var formLists = sources.ListFormLists(RecordScope.Source, TestContext.Current.CancellationToken);
        var read = sources.ReadFormListContext(
            new ReferenceRequest(fixture.SourceListFormKey, RecordScope.Source),
            TestContext.Current.CancellationToken);

        plugins.Succeeded.ShouldBeFalse();
        plugins.Error!.Code.ShouldBe(EngineErrorCode.WorkspaceDisposed);
        formLists.Succeeded.ShouldBeFalse();
        formLists.Error!.Code.ShouldBe(EngineErrorCode.WorkspaceDisposed);
        read.Succeeded.ShouldBeFalse();
        read.Error!.Code.ShouldBe(EngineErrorCode.WorkspaceDisposed);
    }

    /// <summary>
    /// Opens the generated source fixture and returns its concrete Skyrim source set.
    /// </summary>
    /// <param name="fixture">The generated plugin fixture.</param>
    /// <returns>The successfully opened Skyrim source lifetime.</returns>
    private static async Task<SkyrimPluginSourceSet> OpenAsync(SkyrimPluginTestFixture fixture)
    {
        var loader = new SkyrimPluginSourceLoader(new PluginSourceInputLoader());
        var result = await loader.OpenAsync(
            fixture.CreateOpenRequest(),
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value!.Sources.ShouldBeOfType<SkyrimPluginSourceSet>();
    }

    /// <summary>
    /// Formats one FormList summary's origin and containing plugin identities.
    /// </summary>
    /// <param name="summary">The FormList summary.</param>
    /// <returns>The stable origin and context identity pair.</returns>
    private static string SummaryIdentity(FormListSummary summary)
    {
        return $"{summary.FormKey}|{summary.ContainingModKey}";
    }
}
