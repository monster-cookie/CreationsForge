using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInputs;
using CreationsForge.Fallout4.Native;
using Mutagen.Bethesda.Fallout4;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Fallout4;

/// <summary>
/// Verifies Fallout 4 native source loading and bounded reference operations through the production helper.
/// </summary>
public sealed class Fallout4NativeSourceTests
{
    /// <summary>Verifies full/small native loading, immutable baselining, and byte-preserving source ownership.</summary>
    /// <returns>A task that completes after the generated source lifetime is disposed.</returns>
    [Fact]
    public async Task OpenAsync_LoadsFullAndSmallSourcesWithoutChangingArtifacts()
    {
        using var fixture = Fallout4NativeTestFixture.Create();
        var before = fixture.SnapshotArtifacts();

        var result = await CreateLoader().OpenAsync(fixture.CreateOpenRequest());

        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        result.Value.ShouldNotBeNull();
        await using var sources = result.Value.Sources.ShouldBeOfType<Fallout4NativeSourceSet>();
        sources.Baseline.BaselineId.ShouldBe(result.Value.BaselineId);
        sources.Revision.ShouldBe(new WorkspaceRevision(result.Value.BaselineId, 0));
        result.WorkspaceId.ShouldBe((Guid?)sources.WorkspaceId);
        sources.Baseline.Artifacts.Count.ShouldBeGreaterThanOrEqualTo(3);

        var sourceResolution = sources.Resolve(new ReferenceRequest(
            fixture.SourceListFormKey,
            RecordScope.Source));
        var fullResolution = sources.Resolve(new ReferenceRequest(
            fixture.FullListFormKey,
            RecordScope.AllContexts,
            fixture.FullModKey));

        sourceResolution.Succeeded.ShouldBeTrue(sourceResolution.Error?.Message);
        sourceResolution.Value!.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
        sourceResolution.Value.Record.ShouldBeOfType<FormList>();
        var sourceList = (FormList)sourceResolution.Value.Record!;
        sourceList.Name.ShouldNotBeNull();
        sourceList.Name!.String.ShouldBe("Small source list");
        fullResolution.Succeeded.ShouldBeTrue(fullResolution.Error?.Message);
        fullResolution.Value!.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
        fullResolution.Value.Record.ShouldBeOfType<FormList>();
        var fullList = (FormList)fullResolution.Value.Record!;
        fullList.Name.ShouldNotBeNull();
        fullList.Name!.String.ShouldBe("Full list");
        fixture.SourceListFormKey.ID.ShouldBe(fixture.FullListFormKey.ID);
        fixture.SourceListFormKey.ModKey.ShouldNotBe(fixture.FullListFormKey.ModKey);

        var after = fixture.SnapshotArtifacts();
        after.Keys.OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ShouldBe(before.Keys.OrderBy(path => path, StringComparer.OrdinalIgnoreCase));
        foreach (var path in before.Keys)
        {
            after[path].ShouldBe(before[path]);
        }
    }

    /// <summary>Verifies winning override provenance, source context selection, FormList payloads, and detached-copy isolation.</summary>
    /// <returns>A task that completes after the isolated source lifetime is disposed.</returns>
    [Fact]
    public async Task Resolve_PreservesOriginAndContainingPluginAndReturnsDetachedCopy()
    {
        using var fixture = Fallout4NativeTestFixture.Create();
        var result = await CreateLoader().OpenAsync(fixture.CreateOpenRequest());
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        await using var sources = result.Value!.Sources.ShouldBeOfType<Fallout4NativeSourceSet>();

        var winning = sources.Resolve(new ReferenceRequest(
            fixture.SourceListFormKey,
            RecordScope.WinningOverrides));

        winning.Succeeded.ShouldBeTrue(winning.Error?.Message);
        winning.WorkspaceId.ShouldBe(sources.WorkspaceId);
        winning.ResultRevision.ShouldBe(sources.Revision);
        winning.Value!.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
        winning.Value.FormKey.ShouldBe(fixture.SourceListFormKey);
        winning.Value.ContainingModKey.ShouldBe(fixture.PatchModKey);
        winning.Value.SourcePath.ShouldBe(fixture.PatchPluginPath);
        winning.Value.LoadOrderIndex.ShouldBe(2);
        winning.Value.Role.ShouldBe(PluginRole.LoadOrder);
        winning.Value.Record.ShouldBeOfType<FormList>();
        var detached = (FormList)winning.Value.Record!;
        detached.FormKey.ShouldBe(fixture.SourceListFormKey);
        detached.EditorID.ShouldBe("SharedListOverride");
        detached.Name.ShouldNotBeNull();
        detached.Name!.String.ShouldBe("Winning override");
        detached.Items.Select(item => item.FormKey).ShouldBe([fixture.BookFormKey, fixture.KeywordFormKey]);

        var mutableDetached = detached;
        mutableDetached.EditorID = "CallerMutation";
        mutableDetached.Items.Clear();

        var repeated = sources.Resolve(new ReferenceRequest(
            fixture.SourceListFormKey,
            RecordScope.WinningOverrides));
        repeated.Value!.Record.ShouldBeOfType<FormList>();
        var repeatedList = (FormList)repeated.Value.Record!;
        repeatedList.EditorID.ShouldBe("SharedListOverride");
        repeatedList.Items.Count.ShouldBe(2);

        var ambiguous = sources.Resolve(new ReferenceRequest(
            fixture.SourceListFormKey,
            RecordScope.AllContexts));
        ambiguous.Value!.Status.ShouldBe(ReferenceResolutionStatus.Ambiguous);
        ambiguous.Value.Record.ShouldBeNull();

        var selectedSource = sources.Resolve(new ReferenceRequest(
            fixture.SourceListFormKey,
            RecordScope.AllContexts,
            fixture.SourceModKey));
        selectedSource.Value!.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
        selectedSource.Value.ContainingModKey.ShouldBe(fixture.SourceModKey);
        selectedSource.Value.Role.ShouldBe(PluginRole.Source);
        selectedSource.Value.Record.ShouldBeOfType<FormList>();
        ((FormList)selectedSource.Value.Record!).EditorID.ShouldBe("SharedListSmall");
    }

    /// <summary>Verifies stable bounded paging across FormList and non-FormList native record families.</summary>
    /// <returns>A task that completes after both workspace-bound source lifetimes are disposed.</returns>
    [Fact]
    public async Task Search_PagesCrossFamilyContextsAndRejectsAnotherWorkspaceToken()
    {
        using var fixture = Fallout4NativeTestFixture.Create();
        var firstOpen = await CreateLoader().OpenAsync(fixture.CreateOpenRequest(Guid.NewGuid()));
        firstOpen.Succeeded.ShouldBeTrue(firstOpen.Error?.Message);
        await using var firstSources = firstOpen.Value!.Sources.ShouldBeOfType<Fallout4NativeSourceSet>();

        var firstPageRequest = new ReferenceSearchRequest(
            "Shared",
            2,
            scope: RecordScope.AllContexts);
        var firstPage = firstSources.Search(firstPageRequest);

        firstPage.Succeeded.ShouldBeTrue(firstPage.Error?.Message);
        firstPage.Value!.Matches.Count.ShouldBe(2);
        firstPage.Value.ContinuationToken.ShouldNotBeNull();

        var allMatches = new List<ReferenceSearchMatch>(firstPage.Value.Matches);
        var token = firstPage.Value.ContinuationToken;
        while (token is not null)
        {
            var nextPage = firstSources.Search(new ReferenceSearchRequest(
                "Shared",
                2,
                token,
                RecordScope.AllContexts));
            nextPage.Succeeded.ShouldBeTrue(nextPage.Error?.Message);
            allMatches.AddRange(nextPage.Value!.Matches);
            token = nextPage.Value.ContinuationToken;
        }

        allMatches.Count.ShouldBe(5);
        allMatches.Select(match => match.RecordType).ShouldContain("Book");
        allMatches.Select(match => match.RecordType).ShouldContain("Keyword");
        allMatches.Count(match => match.FormKey == fixture.SourceListFormKey).ShouldBe(2);
        allMatches.Single(match =>
            match.FormKey == fixture.SourceListFormKey && match.ContainingModKey == fixture.PatchModKey)
            .LoadOrderIndex.ShouldBe(2);

        var secondOpen = await CreateLoader().OpenAsync(fixture.CreateOpenRequest(Guid.NewGuid()));
        secondOpen.Succeeded.ShouldBeTrue(secondOpen.Error?.Message);
        await using var secondSources = secondOpen.Value!.Sources.ShouldBeOfType<Fallout4NativeSourceSet>();
        var rejected = secondSources.Search(new ReferenceSearchRequest(
            "Shared",
            2,
            firstPage.Value.ContinuationToken,
            RecordScope.AllContexts));

        rejected.Succeeded.ShouldBeFalse();
        rejected.Error!.Code.ShouldBe(EngineErrorCode.InvalidRequest);
    }

    /// <summary>Verifies that missing declared masters fail before a native source lifetime is returned.</summary>
    /// <returns>A task that completes after native input validation rejects the request.</returns>
    [Fact]
    public async Task OpenAsync_WhenDeclaredMasterIsMissing_ReturnsMissingMaster()
    {
        using var fixture = Fallout4NativeTestFixture.Create();
        var request = fixture.CreateOpenRequest(
            fixture.PatchPluginPath,
            [fixture.PatchPluginPath]);

        var result = await CreateLoader().OpenAsync(request);

        result.Succeeded.ShouldBeFalse();
        result.Error!.Code.ShouldBe(EngineErrorCode.MissingMaster);
        result.Error.Message.ShouldContain("master", Case.Insensitive);
    }

    /// <summary>Verifies the pinned Fallout 4 native model rejects medium-master state instead of emitting an ambiguous header.</summary>
    [Fact]
    public void NativeModel_WhenMediumMasterIsRequested_RejectsUnsupportedStyle()
    {
        var mod = new Fallout4Mod("UnsupportedMedium.esm", Fallout4Release.Fallout4);

        mod.CanBeMediumMaster.ShouldBeFalse();
        var exception = Should.Throw<ArgumentException>(() => mod.IsMediumMaster = true);

        exception.Message.ShouldContain("unsupported", Case.Insensitive);
    }

    /// <summary>Verifies two paths claiming the same native ModKey are rejected as ambiguous explicit input.</summary>
    /// <returns>A task that completes after native input validation rejects the duplicate identity.</returns>
    [Fact]
    public async Task OpenAsync_WhenModKeyIsDuplicated_ReturnsInvalidRequest()
    {
        using var fixture = Fallout4NativeTestFixture.Create();
        var duplicatePath = fixture.CreateAmbiguousSourceCopy();
        var request = fixture.CreateOpenRequest(
            fixture.SourcePluginPath,
            [fixture.SourcePluginPath, duplicatePath]);

        var result = await CreateLoader().OpenAsync(request);

        result.Succeeded.ShouldBeFalse();
        result.Error!.Code.ShouldBe(EngineErrorCode.InvalidRequest);
    }

    /// <summary>Verifies source drift is reported and disposal closes every subsequent source operation.</summary>
    /// <returns>A task that completes after drift and disposed-state checks finish.</returns>
    [Fact]
    public async Task VerifyAndDispose_DetectsDriftAndReturnsDisposedFailures()
    {
        using var fixture = Fallout4NativeTestFixture.Create();
        var result = await CreateLoader().OpenAsync(fixture.CreateOpenRequest());
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        var sources = result.Value!.Sources.ShouldBeOfType<Fallout4NativeSourceSet>();

        await using (var stream = new FileStream(
                         fixture.PatchPluginPath,
                         FileMode.Append,
                         FileAccess.Write,
                         FileShare.Read))
        {
            await stream.WriteAsync(new byte[] { 0x42 });
        }
        var drift = await sources.VerifyUnchangedAsync();

        drift.Succeeded.ShouldBeFalse();
        drift.Error!.Code.ShouldBe(EngineErrorCode.ExternalChangeDetected);

        await sources.DisposeAsync();
        await sources.DisposeAsync();
        var resolve = sources.Resolve(new ReferenceRequest(fixture.SourceListFormKey, RecordScope.Source));
        var search = sources.Search(new ReferenceSearchRequest("Shared", 2));
        var plugins = sources.ListPlugins();
        var formLists = sources.ListFormLists(RecordScope.Source);
        var read = sources.ReadFormListContext(new ReferenceRequest(
            fixture.SourceListFormKey,
            RecordScope.Source));
        var verify = await sources.VerifyUnchangedAsync();

        resolve.Succeeded.ShouldBeFalse();
        resolve.Error!.Code.ShouldBe(EngineErrorCode.WorkspaceDisposed);
        search.Succeeded.ShouldBeFalse();
        search.Error!.Code.ShouldBe(EngineErrorCode.WorkspaceDisposed);
        plugins.Succeeded.ShouldBeFalse();
        plugins.Error!.Code.ShouldBe(EngineErrorCode.WorkspaceDisposed);
        formLists.Succeeded.ShouldBeFalse();
        formLists.Error!.Code.ShouldBe(EngineErrorCode.WorkspaceDisposed);
        read.Succeeded.ShouldBeFalse();
        read.Error!.Code.ShouldBe(EngineErrorCode.WorkspaceDisposed);
        verify.Succeeded.ShouldBeFalse();
        verify.Error!.Code.ShouldBe(EngineErrorCode.WorkspaceDisposed);
    }

    /// <summary>Verifies cancellation is observed before both native acquisition and bounded reference scans.</summary>
    /// <returns>A task that completes after all cancellation paths are observed.</returns>
    [Fact]
    public async Task NativeOperations_WhenCancelled_ThrowOperationCanceledException()
    {
        using var fixture = Fallout4NativeTestFixture.Create();
        using var alreadyCancelled = new CancellationTokenSource();
        alreadyCancelled.Cancel();

        await Should.ThrowAsync<OperationCanceledException>(async () =>
            await CreateLoader().OpenAsync(fixture.CreateOpenRequest(), alreadyCancelled.Token));

        var result = await CreateLoader().OpenAsync(fixture.CreateOpenRequest());
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        await using var sources = result.Value!.Sources.ShouldBeOfType<Fallout4NativeSourceSet>();

        Should.Throw<OperationCanceledException>(() => sources.Resolve(
            new ReferenceRequest(fixture.SourceListFormKey, RecordScope.WinningOverrides),
            alreadyCancelled.Token));
        Should.Throw<OperationCanceledException>(() => sources.Search(
            new ReferenceSearchRequest("Shared", 2),
            alreadyCancelled.Token));
        Should.Throw<OperationCanceledException>(() => sources.ListPlugins(alreadyCancelled.Token));
        Should.Throw<OperationCanceledException>(() => sources.ListFormLists(
            RecordScope.Source,
            alreadyCancelled.Token));
        Should.Throw<OperationCanceledException>(() => sources.ReadFormListContext(
            new ReferenceRequest(fixture.SourceListFormKey, RecordScope.Source),
            alreadyCancelled.Token));
    }

    /// <summary>Creates the production Fallout 4 loader with an independent shared input service.</summary>
    /// <returns>A loader that uses only paths supplied by each test request.</returns>
    private static Fallout4NativeSourceLoader CreateLoader()
    {
        return new Fallout4NativeSourceLoader(new NativeSourceInputLoader());
    }
}
