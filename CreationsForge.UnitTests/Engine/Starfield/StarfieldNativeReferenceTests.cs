using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInputs;
using CreationsForge.Starfield.Native;
using Mutagen.Bethesda.Starfield;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Starfield;

/// <summary>
/// Verifies Starfield's bounded native reference resolution and deterministic search behavior.
/// </summary>
public sealed class StarfieldNativeReferenceTests
{
    /// <summary>
    /// Verifies source, winning, ambiguous, and explicit containing-plugin contexts preserve origin and provenance.
    /// </summary>
    /// <returns>A task that completes after the generated native source lifetime is disposed.</returns>
    [Fact]
    public async Task Resolve_PreservesOriginAndContainingContextAcrossScopes()
    {
        using var fixture = StarfieldNativeTestFixture.Create();
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

            source.Value!.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
            source.Value.FormKey.ShouldBe(fixture.SourceListFormKey);
            source.Value.ContainingModKey.ShouldBe(fixture.SourceModKey);
            source.Value.Role.ShouldBe(PluginRole.Source);
            source.Value.LoadOrderIndex.ShouldBe(0);

            winning.Value!.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
            winning.Value.FormKey.ShouldBe(fixture.SourceListFormKey);
            winning.Value.ContainingModKey.ShouldBe(fixture.PatchModKey);
            winning.Value.Role.ShouldBe(PluginRole.LoadOrder);
            winning.Value.Record!.EditorID.ShouldBe("SharedListOverride");

            ambiguous.Value!.Status.ShouldBe(ReferenceResolutionStatus.Ambiguous);
            ambiguous.Value.Record.ShouldBeNull();

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
    /// Verifies non-FormList references resolve and every returned getter is an isolated native copy.
    /// </summary>
    /// <returns>A task that completes after native copy-isolation checks and fixture cleanup.</returns>
    [Fact]
    public async Task Resolve_ReturnsDetachedCopiesForCrossFamilyRecords()
    {
        using var fixture = StarfieldNativeTestFixture.Create();
        var sources = await OpenAsync(fixture);
        FormList? retainedAfterDisposal = null;
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

            var detachedList = firstList.Value!.Record.ShouldBeOfType<FormList>();
            detachedList.EditorID = "Mutated detached copy";
            detachedList.Name = "Mutated detached name";
            detachedList.Items.Clear();
            book.Value!.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
            book.Value.Record.ShouldBeAssignableTo<IBookGetter>();
            keyword.Value!.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
            keyword.Value.Record.ShouldBeAssignableTo<IKeywordGetter>();

            var secondList = sources.Resolve(
                new ReferenceRequest(fixture.SourceListFormKey, RecordScope.Source),
                TestContext.Current.CancellationToken);

            secondList.Value!.Record!.EditorID.ShouldBe("SharedListSmall");
            secondList.Value.Record.ShouldNotBeSameAs(firstList.Value.Record);
            var unchangedList = secondList.Value.Record.ShouldBeOfType<FormList>();
            var unchangedName = unchangedList.Name.ShouldNotBeNull();
            unchangedName.String.ShouldBe("Small source list");
            unchangedList.Items.Select(item => item.FormKey).ShouldBe(new[] { fixture.BookFormKey, fixture.KeywordFormKey });
            retainedAfterDisposal = unchangedList;
        }
        finally
        {
            await sources.DisposeAsync();
        }

        retainedAfterDisposal.ShouldNotBeNull();
        retainedAfterDisposal.EditorID.ShouldBe("SharedListSmall");
        retainedAfterDisposal.Items.Select(item => item.FormKey).ShouldBe(new[] { fixture.BookFormKey, fixture.KeywordFormKey });
    }

    /// <summary>
    /// Verifies search pages are stable, scope-aware, provenance-rich, and bound to the source revision.
    /// </summary>
    /// <returns>A task that completes after deterministic page checks and source cleanup.</returns>
    [Fact]
    public async Task Search_ReturnsStableBoundedPagesWithNativeProvenance()
    {
        using var fixture = StarfieldNativeTestFixture.Create();
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

            var completeFirst = ReadAllMatchIdentities(sources, "Shared", RecordScope.AllContexts);
            var completeSecond = ReadAllMatchIdentities(sources, "Shared", RecordScope.AllContexts);
            completeSecond.ShouldBe(completeFirst);
            completeFirst.Distinct(StringComparer.Ordinal).Count().ShouldBe(completeFirst.Count);

            var sourceOnly = sources.Search(
                new ReferenceSearchRequest("Shared", 20, scope: RecordScope.Source),
                TestContext.Current.CancellationToken);
            sourceOnly.Succeeded.ShouldBeTrue(sourceOnly.Error?.Message);
            sourceOnly.Value!.Matches.ShouldNotBeEmpty();
            foreach (var match in sourceOnly.Value.Matches)
            {
                match.ContainingModKey.ShouldBe(fixture.SourceModKey);
                match.Role.ShouldBe(PluginRole.Source);
            }

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
    /// Verifies continuation tokens cannot cross query or workspace boundaries.
    /// </summary>
    /// <returns>A task that completes after invalid cross-context cursor checks.</returns>
    [Fact]
    public async Task Search_RejectsContinuationTokenOutsideItsRequestContext()
    {
        using var fixture = StarfieldNativeTestFixture.Create();
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
    /// Verifies pre-canceled reference operations stop at the bounded native-reader boundary.
    /// </summary>
    /// <returns>A task that completes after both cancellation paths are observed.</returns>
    [Fact]
    public async Task ReferenceOperations_PropagateCancellation()
    {
        using var fixture = StarfieldNativeTestFixture.Create();
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
    /// Opens the generated source fixture and returns its concrete Starfield native source lifetime.
    /// </summary>
    /// <param name="fixture">The generated fixture whose lifetime remains owned by the calling test.</param>
    /// <param name="workspaceId">An optional workspace identity used for cursor-isolation checks.</param>
    /// <returns>The successfully opened Starfield source set.</returns>
    private static async Task<StarfieldNativeSourceSet> OpenAsync(
        StarfieldNativeTestFixture fixture,
        Guid? workspaceId = null)
    {
        var loader = new StarfieldNativeSourceLoader(new NativeSourceInputLoader());
        var result = await loader.OpenAsync(
            fixture.CreateOpenRequest(workspaceId),
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value!.Sources.ShouldBeOfType<StarfieldNativeSourceSet>();
    }

    /// <summary>
    /// Creates a stable comparable identity for one native reference-search context.
    /// </summary>
    /// <param name="match">The native search match.</param>
    /// <returns>A string containing origin, context, type, and EditorID.</returns>
    private static string MatchIdentity(ReferenceSearchMatch match)
    {
        return $"{match.FormKey}|{match.ContainingModKey}|{match.RecordType}|{match.EditorId}";
    }

    /// <summary>
    /// Reads every bounded page for one query and records its complete deterministic context order.
    /// </summary>
    /// <param name="sources">The open native source lifetime.</param>
    /// <param name="query">The EditorID query to search.</param>
    /// <param name="scope">The record contexts to enumerate.</param>
    /// <returns>The ordered identities from all pages.</returns>
    private static IReadOnlyList<string> ReadAllMatchIdentities(
        StarfieldNativeSourceSet sources,
        string query,
        RecordScope scope)
    {
        var identities = new List<string>();
        string? continuationToken = null;

        for (var pageNumber = 0; pageNumber < 100; pageNumber++)
        {
            var result = sources.Search(
                new ReferenceSearchRequest(query, 2, continuationToken, scope),
                TestContext.Current.CancellationToken);
            result.Succeeded.ShouldBeTrue(result.Error?.Message);
            identities.AddRange(result.Value!.Matches.Select(MatchIdentity));
            continuationToken = result.Value.ContinuationToken;
            if (continuationToken is null)
            {
                return identities;
            }
        }

        throw new InvalidOperationException("The bounded native reference search did not terminate within the fixture's expected page count.");
    }
}
