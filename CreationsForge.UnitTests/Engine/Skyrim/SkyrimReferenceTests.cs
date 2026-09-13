using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.PluginInputs;
using CreationsForge.Skyrim.PluginAdapter;
using Mutagen.Bethesda.Skyrim;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Skyrim;

/// <summary>
/// Verifies Skyrim Special Edition's bounded reference resolution and deterministic search behavior.
/// </summary>
public sealed class SkyrimReferenceTests
{
    /// <summary>
    /// Verifies source, winning, ambiguous, and explicit containing-plugin contexts preserve origin and provenance.
    /// </summary>
    /// <returns>A task that completes after the generated plugin source lifetime is disposed.</returns>
    [Fact]
    public async Task Resolve_PreservesOriginAndContainingContextAcrossScopes()
    {
        using var fixture = SkyrimPluginTestFixture.Create();
        var sources = await OpenAsync(fixture);
        try
        {
            var source = sources.Resolve(
                new ReferenceRequest(fixture.SourceListFormKey, RecordScope.Source),
                TestContext.Current.CancellationToken);
            var winning = sources.Resolve(
                new ReferenceRequest(fixture.SourceListFormKey, RecordScope.WinningOverrides),
                TestContext.Current.CancellationToken);
            var ambiguous = sources.Resolve(
                new ReferenceRequest(fixture.SourceListFormKey, RecordScope.AllContexts),
                TestContext.Current.CancellationToken);
            var selectedContext = sources.Resolve(
                new ReferenceRequest(fixture.SourceListFormKey, RecordScope.AllContexts, fixture.SourceModKey),
                TestContext.Current.CancellationToken);

            source.Succeeded.ShouldBeTrue(source.Error?.Message);
            source.Value!.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
            source.Value.FormKey.ShouldBe(fixture.SourceListFormKey);
            source.Value.ContainingModKey.ShouldBe(fixture.SourceModKey);
            source.Value.SourcePath.ShouldBe(fixture.SourcePluginPath);
            source.Value.Role.ShouldBe(PluginRole.Source);
            source.Value.LoadOrderIndex.ShouldBe(0);

            winning.Succeeded.ShouldBeTrue(winning.Error?.Message);
            winning.Value!.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
            winning.Value.FormKey.ShouldBe(fixture.SourceListFormKey);
            winning.Value.ContainingModKey.ShouldBe(fixture.PatchModKey);
            winning.Value.SourcePath.ShouldBe(fixture.PatchPluginPath);
            winning.Value.Role.ShouldBe(PluginRole.LoadOrder);
            winning.Value.Record!.EditorID.ShouldBe("SharedListOverride");

            ambiguous.Succeeded.ShouldBeTrue(ambiguous.Error?.Message);
            ambiguous.Value!.Status.ShouldBe(ReferenceResolutionStatus.Ambiguous);
            ambiguous.Value.Record.ShouldBeNull();

            selectedContext.Succeeded.ShouldBeTrue(selectedContext.Error?.Message);
            selectedContext.Value!.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
            selectedContext.Value.ContainingModKey.ShouldBe(fixture.SourceModKey);
            selectedContext.Value.Record!.EditorID.ShouldBe("SharedListSmall");
        }
        finally
        {
            await sources.DisposeAsync();
        }
    }

    /// <summary>
    /// Verifies non-FormList references resolve and every returned getter is an isolated plugin copy.
    /// </summary>
    /// <returns>A task that completes after plugin copy-isolation checks and fixture cleanup.</returns>
    [Fact]
    public async Task Resolve_ReturnsDetachedCopiesForCrossFamilyRecords()
    {
        using var fixture = SkyrimPluginTestFixture.Create();
        var sources = await OpenAsync(fixture);
        try
        {
            var firstList = sources.Resolve(
                new ReferenceRequest(fixture.SourceListFormKey, RecordScope.Source),
                TestContext.Current.CancellationToken);
            var book = sources.Resolve(
                new ReferenceRequest(fixture.BookFormKey, RecordScope.Source),
                TestContext.Current.CancellationToken);
            var keyword = sources.Resolve(
                new ReferenceRequest(fixture.KeywordFormKey, RecordScope.Source),
                TestContext.Current.CancellationToken);

            firstList.Succeeded.ShouldBeTrue(firstList.Error?.Message);
            var detachedList = firstList.Value!.Record.ShouldBeOfType<FormList>();
            detachedList.Items.Select(item => item.FormKey).ShouldBe(
                [fixture.BookFormKey, fixture.MissingFormKey, fixture.KeywordFormKey, fixture.MissingFormKey]);
            detachedList.EditorID = "Mutated detached copy";
            detachedList.Items.RemoveAt(0);
            book.Succeeded.ShouldBeTrue(book.Error?.Message);
            book.Value!.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
            book.Value.Record.ShouldBeAssignableTo<IBookGetter>();
            keyword.Succeeded.ShouldBeTrue(keyword.Error?.Message);
            keyword.Value!.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
            keyword.Value.Record.ShouldBeAssignableTo<IKeywordGetter>();

            var secondList = sources.Resolve(
                new ReferenceRequest(fixture.SourceListFormKey, RecordScope.Source),
                TestContext.Current.CancellationToken);

            secondList.Succeeded.ShouldBeTrue(secondList.Error?.Message);
            secondList.Value!.Record!.EditorID.ShouldBe("SharedListSmall");
            secondList.Value.Record.ShouldBeOfType<FormList>()
                .Items.Select(item => item.FormKey)
                .ShouldBe([fixture.BookFormKey, fixture.MissingFormKey, fixture.KeywordFormKey, fixture.MissingFormKey]);
            secondList.Value.Record.ShouldNotBeSameAs(firstList.Value.Record);
        }
        finally
        {
            await sources.DisposeAsync();
        }
    }

    /// <summary>
    /// Verifies search pages are stable, scope-aware, provenance-rich, and bound to the source revision.
    /// </summary>
    /// <returns>A task that completes after deterministic page checks and source cleanup.</returns>
    [Fact]
    public async Task Search_ReturnsStableBoundedPagesWithPluginProvenance()
    {
        using var fixture = SkyrimPluginTestFixture.Create();
        var sources = await OpenAsync(fixture);
        try
        {
            var request = new ReferenceSearchRequest(
                "Shared",
                2,
                scope: RecordScope.AllContexts);
            var first = sources.Search(request, TestContext.Current.CancellationToken);
            var repeated = sources.Search(request, TestContext.Current.CancellationToken);

            first.Succeeded.ShouldBeTrue(first.Error?.Message);
            repeated.Succeeded.ShouldBeTrue(repeated.Error?.Message);
            first.WorkspaceId.ShouldBe(sources.WorkspaceId);
            first.ResultRevision.ShouldBe(sources.Revision);
            first.Value!.Matches.Count.ShouldBe(2);
            first.Value.ContinuationToken.ShouldNotBeNull();
            repeated.Value!.ContinuationToken.ShouldBe(first.Value.ContinuationToken);
            repeated.Value.Matches.Select(MatchIdentity).ShouldBe(first.Value.Matches.Select(MatchIdentity));

            var second = sources.Search(
                new ReferenceSearchRequest(
                    "Shared",
                    2,
                    first.Value.ContinuationToken,
                    RecordScope.AllContexts),
                TestContext.Current.CancellationToken);

            second.Succeeded.ShouldBeTrue(second.Error?.Message);
            second.Value!.Matches.ShouldNotBeEmpty();
            second.Value.Matches.Select(MatchIdentity)
                .Intersect(first.Value.Matches.Select(MatchIdentity))
                .ShouldBeEmpty();
            foreach (var match in first.Value.Matches.Concat(second.Value.Matches))
            {
                match.ContainingModKey.ShouldNotBeNull();
                match.SourcePath.ShouldNotBeNullOrWhiteSpace();
                match.LoadOrderIndex.ShouldNotBeNull();
                match.Role.ShouldNotBeNull();
            }

            var sourceOnly = sources.Search(
                new ReferenceSearchRequest("Shared", 20, scope: RecordScope.Source),
                TestContext.Current.CancellationToken);
            sourceOnly.Succeeded.ShouldBeTrue(sourceOnly.Error?.Message);
            sourceOnly.Value!.Matches.ShouldNotBeEmpty();
            sourceOnly.Value.Matches.ShouldAllBe(match => match.ContainingModKey == fixture.SourceModKey);
            sourceOnly.Value.Matches.ShouldAllBe(match => match.Role == PluginRole.Source);

            var stagedOutput = sources.Search(
                new ReferenceSearchRequest("Shared", 20, scope: RecordScope.StagedOutput),
                TestContext.Current.CancellationToken);
            stagedOutput.Succeeded.ShouldBeTrue(stagedOutput.Error?.Message);
            stagedOutput.Value!.Matches.ShouldBeEmpty();
        }
        finally
        {
            await sources.DisposeAsync();
        }
    }

    /// <summary>
    /// Verifies continuation tokens are bound to the exact query, workspace, and source revision.
    /// </summary>
    /// <returns>A task that completes after invalid cross-request cursor checks.</returns>
    [Fact]
    public async Task Search_RejectsContinuationTokenOutsideItsRequestContext()
    {
        using var fixture = SkyrimPluginTestFixture.Create();
        var firstSources = await OpenAsync(fixture, Guid.NewGuid());
        var secondSources = await OpenAsync(fixture, Guid.NewGuid());
        try
        {
            var first = firstSources.Search(
                new ReferenceSearchRequest("Shared", 1, scope: RecordScope.AllContexts),
                TestContext.Current.CancellationToken);
            first.Succeeded.ShouldBeTrue(first.Error?.Message);
            first.Value!.ContinuationToken.ShouldNotBeNull();

            var otherQuery = firstSources.Search(
                new ReferenceSearchRequest("Book", 1, first.Value.ContinuationToken, RecordScope.AllContexts),
                TestContext.Current.CancellationToken);
            var otherWorkspace = secondSources.Search(
                new ReferenceSearchRequest("Shared", 1, first.Value.ContinuationToken, RecordScope.AllContexts),
                TestContext.Current.CancellationToken);

            otherQuery.Succeeded.ShouldBeFalse();
            otherQuery.Error!.Code.ShouldBe(EngineErrorCode.InvalidRequest);
            otherWorkspace.Succeeded.ShouldBeFalse();
            otherWorkspace.Error!.Code.ShouldBe(EngineErrorCode.InvalidRequest);
        }
        finally
        {
            await firstSources.DisposeAsync();
            await secondSources.DisposeAsync();
        }
    }

    /// <summary>
    /// Verifies pre-canceled reference operations stop at the bounded plugin-reader boundary.
    /// </summary>
    /// <returns>A task that completes after both cancellation paths are observed.</returns>
    [Fact]
    public async Task ReferenceOperations_PropagateCancellation()
    {
        using var fixture = SkyrimPluginTestFixture.Create();
        var sources = await OpenAsync(fixture);
        try
        {
            using var cancellationSource = new CancellationTokenSource();
            cancellationSource.Cancel();

            Should.Throw<OperationCanceledException>(() => sources.Resolve(
                new ReferenceRequest(fixture.BookFormKey, RecordScope.Source),
                cancellationSource.Token));
            Should.Throw<OperationCanceledException>(() => sources.Search(
                new ReferenceSearchRequest("Shared", 1, scope: RecordScope.Source),
                cancellationSource.Token));
        }
        finally
        {
            await sources.DisposeAsync();
        }
    }

    /// <summary>
    /// Opens the generated source fixture and returns its concrete Skyrim plugin source lifetime.
    /// </summary>
    /// <param name="fixture">The generated fixture whose lifetime remains owned by the calling test.</param>
    /// <param name="workspaceId">An optional workspace identity used for cursor-isolation checks.</param>
    /// <returns>The successfully opened Skyrim plugin source set.</returns>
    private static async Task<SkyrimPluginSourceSet> OpenAsync(
        SkyrimPluginTestFixture fixture,
        Guid? workspaceId = null)
    {
        var loader = new SkyrimPluginSourceLoader(new PluginSourceInputLoader());
        var result = await loader.OpenAsync(
            fixture.CreateOpenRequest(workspaceId),
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value!.Sources.ShouldBeOfType<SkyrimPluginSourceSet>();
    }

    /// <summary>
    /// Creates a stable comparable identity for one reference-search context.
    /// </summary>
    /// <param name="match">The plugin search match.</param>
    /// <returns>A string containing origin, context, type, and EditorID.</returns>
    private static string MatchIdentity(ReferenceSearchMatch match)
    {
        return $"{match.FormKey}|{match.ContainingModKey}|{match.RecordType}|{match.EditorId}";
    }
}
