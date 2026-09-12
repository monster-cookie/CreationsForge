using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInputs;
using CreationsForge.Starfield.Native;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Starfield;

/// <summary>
/// Verifies source-only Starfield plugin discovery, FormList enumeration, and contextual detached reads.
/// </summary>
public sealed class StarfieldNativeSourceReadTests
{
    /// <summary>Verifies plugin and FormList listings preserve native context order and override counts.</summary>
    /// <returns>A task that completes after the source lifetime is released.</returns>
    [Fact]
    public async Task Lists_PreserveNativeOrderScopesAndOverrideCounts()
    {
        using var fixture = StarfieldNativeTestFixture.Create();
        var sources = await OpenAsync(fixture);
        try
        {
            var plugins = sources.ListPlugins(TestContext.Current.CancellationToken);
            var sourceLists = sources.ListFormLists(RecordScope.Source, TestContext.Current.CancellationToken);
            var allContexts = sources.ListFormLists(RecordScope.AllContexts, TestContext.Current.CancellationToken);
            var winning = sources.ListFormLists(RecordScope.WinningOverrides, TestContext.Current.CancellationToken);
            var staged = sources.ListFormLists(RecordScope.StagedOutput, TestContext.Current.CancellationToken);

            plugins.Succeeded.ShouldBeTrue(plugins.Error?.Message);
            plugins.WorkspaceId.ShouldBe(sources.WorkspaceId);
            plugins.ResultRevision.ShouldBe(sources.Revision);
            var pluginSummaries = plugins.Value!;
            pluginSummaries.Select(plugin => plugin.ModKey).ShouldBe(new[]
            {
                fixture.SourceModKey,
                fixture.FullModKey,
                fixture.MediumModKey,
                fixture.PatchModKey,
            });
            pluginSummaries.Select(plugin => plugin.LoadOrderIndex).ShouldBe(new[] { 0, 1, 2, 3 });
            pluginSummaries.Select(plugin => plugin.Role).ShouldBe(new[]
            {
                PluginRole.Source,
                PluginRole.LoadOrder,
                PluginRole.LoadOrder,
                PluginRole.LoadOrder,
            });

            sourceLists.Succeeded.ShouldBeTrue(sourceLists.Error?.Message);
            var sourceSummaries = sourceLists.Value!;
            sourceSummaries.Select(summary => summary.FormKey).ShouldBe(new[]
            {
                fixture.SourceListFormKey,
                fixture.DeletedListFormKey,
                fixture.WarningListFormKey,
            });
            sourceSummaries.Select(summary => summary.OverrideCount).ShouldBe(new[] { 1, 1, 0 });

            allContexts.Succeeded.ShouldBeTrue(allContexts.Error?.Message);
            var allContextSummaries = allContexts.Value!;
            allContextSummaries.Select(summary => summary.FormKey).ShouldBe(new[]
            {
                fixture.SourceListFormKey,
                fixture.DeletedListFormKey,
                fixture.WarningListFormKey,
                fixture.FullListFormKey,
                fixture.MediumListFormKey,
                fixture.SourceListFormKey,
                fixture.DeletedListFormKey,
            });
            allContextSummaries.Select(summary => summary.OverrideCount).ShouldBe(new[] { 1, 1, 0, 0, 0, 1, 1 });

            winning.Succeeded.ShouldBeTrue(winning.Error?.Message);
            var winningSummaries = winning.Value!;
            winningSummaries.Select(summary => summary.FormKey).ShouldBe(new[]
            {
                fixture.WarningListFormKey,
                fixture.FullListFormKey,
                fixture.MediumListFormKey,
                fixture.SourceListFormKey,
                fixture.DeletedListFormKey,
            });
            winningSummaries.Select(summary => summary.ContainingModKey).ShouldBe(new ModKey?[]
            {
                fixture.SourceModKey,
                fixture.FullModKey,
                fixture.MediumModKey,
                fixture.PatchModKey,
                fixture.PatchModKey,
            });
            winningSummaries.Select(summary => summary.OverrideCount).ShouldBe(new[] { 0, 0, 0, 1, 1 });

            staged.Succeeded.ShouldBeTrue(staged.Error?.Message);
            staged.Value!.ShouldBeEmpty();
        }
        finally
        {
            await sources.DisposeAsync();
        }
    }

    /// <summary>Verifies exact context selection, deleted snapshots, ambiguity, unsupported families, and missing-link warnings.</summary>
    /// <returns>A task that completes after detached record and provenance checks.</returns>
    [Fact]
    public async Task ReadFormListContext_PreservesSelectionProvenanceAndWarnings()
    {
        using var fixture = StarfieldNativeTestFixture.Create();
        var sources = await OpenAsync(fixture);
        try
        {
            var sourceRequest = new ReferenceRequest(
                fixture.SourceListFormKey,
                RecordScope.AllContexts,
                fixture.SourceModKey);
            var source = sources.ReadFormListContext(sourceRequest, TestContext.Current.CancellationToken);
            var ambiguous = sources.ReadFormListContext(
                new ReferenceRequest(fixture.SourceListFormKey, RecordScope.AllContexts),
                TestContext.Current.CancellationToken);
            var deleted = sources.ReadFormListContext(
                new ReferenceRequest(fixture.DeletedListFormKey, RecordScope.WinningOverrides),
                TestContext.Current.CancellationToken);
            var unresolved = sources.ReadFormListContext(
                new ReferenceRequest(new FormKey(fixture.SourceModKey, 0x00000FFE), RecordScope.WinningOverrides),
                TestContext.Current.CancellationToken);
            var unsupported = sources.ReadFormListContext(
                new ReferenceRequest(fixture.BookFormKey, RecordScope.Source),
                TestContext.Current.CancellationToken);
            var warning = sources.ReadFormListContext(
                new ReferenceRequest(fixture.WarningListFormKey, RecordScope.Source),
                TestContext.Current.CancellationToken);

            source.Succeeded.ShouldBeTrue(source.Error?.Message);
            source.WorkspaceId.ShouldBe(sources.WorkspaceId);
            source.ResultRevision.ShouldBe(sources.Revision);
            var sourceRead = source.Value!;
            sourceRead.Context.Selection.ShouldBeSameAs(sourceRequest);
            sourceRead.Context.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
            sourceRead.Context.ContainingModKey.ShouldBe(fixture.SourceModKey);
            sourceRead.Context.Path.ShouldBe(fixture.SourcePluginPath);
            sourceRead.Context.LoadOrderIndex.ShouldBe(0);
            sourceRead.Context.Role.ShouldBe(PluginRole.Source);
            sourceRead.Record.ShouldBeOfType<FormList>();

            ambiguous.Succeeded.ShouldBeTrue(ambiguous.Error?.Message);
            var ambiguousRead = ambiguous.Value!;
            ambiguousRead.Context.Status.ShouldBe(ReferenceResolutionStatus.Ambiguous);
            ambiguousRead.Record.ShouldBeNull();
            ambiguousRead.Context.ContainingModKey.ShouldBeNull();

            deleted.Succeeded.ShouldBeTrue(deleted.Error?.Message);
            var deletedRead = deleted.Value!;
            deletedRead.Context.Status.ShouldBe(ReferenceResolutionStatus.Deleted);
            deletedRead.Context.ContainingModKey.ShouldBe(fixture.PatchModKey);
            deletedRead.Record.ShouldBeOfType<FormList>().IsDeleted.ShouldBeTrue();

            unresolved.Succeeded.ShouldBeTrue(unresolved.Error?.Message);
            var unresolvedRead = unresolved.Value!;
            unresolvedRead.Context.Status.ShouldBe(ReferenceResolutionStatus.Unresolved);
            unresolvedRead.Record.ShouldBeNull();

            unsupported.Succeeded.ShouldBeTrue(unsupported.Error?.Message);
            var unsupportedRead = unsupported.Value!;
            unsupportedRead.Context.Status.ShouldBe(ReferenceResolutionStatus.Unsupported);
            unsupportedRead.Record.ShouldBeNull();
            unsupportedRead.RecordType.ShouldBe("Book");
            unsupportedRead.Context.ContainingModKey.ShouldBe(fixture.SourceModKey);

            warning.Succeeded.ShouldBeTrue(warning.Error?.Message);
            warning.Value!.Context.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
            warning.Warnings.Count.ShouldBe(1);
            warning.Warnings[0].Code.ShouldBe("missing-native-reference");
            warning.Warnings[0].Message.ShouldContain(fixture.MissingFormKey.ToString());
            warning.Warnings[0].Message.ShouldContain("FormLinks[0]");
        }
        finally
        {
            await sources.DisposeAsync();
        }
    }

    /// <summary>Verifies contextual records are detached and listing and reading observe cancellation and disposal.</summary>
    /// <returns>A task that completes after cancellation, isolation, and disposal checks.</returns>
    [Fact]
    public async Task ReadFacades_ReturnDetachedRecordsAndRejectCanceledOrDisposedOperations()
    {
        using var fixture = StarfieldNativeTestFixture.Create();
        var sources = await OpenAsync(fixture);
        var request = new ReferenceRequest(fixture.SourceListFormKey, RecordScope.Source);
        var first = sources.ReadFormListContext(request, TestContext.Current.CancellationToken);
        first.Succeeded.ShouldBeTrue(first.Error?.Message);
        var firstRecord = first.Value!.Record.ShouldBeOfType<FormList>();
        firstRecord.EditorID = "Changed detached read";
        firstRecord.Items.Clear();

        var second = sources.ReadFormListContext(request, TestContext.Current.CancellationToken);
        second.Succeeded.ShouldBeTrue(second.Error?.Message);
        var secondRead = second.Value!;
        secondRead.Record!.EditorID.ShouldBe("SharedListSmall");
        secondRead.Record.ShouldBeOfType<FormList>().Items.Count.ShouldBe(2);

        using var cancellationSource = new CancellationTokenSource();
        cancellationSource.Cancel();
        Should.Throw<OperationCanceledException>(() => sources.ListPlugins(cancellationSource.Token));
        Should.Throw<OperationCanceledException>(() => sources.ListFormLists(RecordScope.AllContexts, cancellationSource.Token));
        Should.Throw<OperationCanceledException>(() => sources.ReadFormListContext(request, cancellationSource.Token));

        await sources.DisposeAsync();
        sources.ListPlugins().Error!.Code.ShouldBe(EngineErrorCode.WorkspaceDisposed);
        sources.ListFormLists(RecordScope.Source).Error!.Code.ShouldBe(EngineErrorCode.WorkspaceDisposed);
        sources.ReadFormListContext(request).Error!.Code.ShouldBe(EngineErrorCode.WorkspaceDisposed);
    }

    /// <summary>Opens one generated fixture and returns its concrete Starfield source lifetime.</summary>
    /// <param name="fixture">The generated fixture owned by the calling test.</param>
    /// <returns>The successfully opened Starfield source set.</returns>
    private static async Task<StarfieldNativeSourceSet> OpenAsync(StarfieldNativeTestFixture fixture)
    {
        var loader = new StarfieldNativeSourceLoader(new NativeSourceInputLoader());
        var result = await loader.OpenAsync(
            fixture.CreateOpenRequest(),
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value!.Sources.ShouldBeOfType<StarfieldNativeSourceSet>();
    }
}
