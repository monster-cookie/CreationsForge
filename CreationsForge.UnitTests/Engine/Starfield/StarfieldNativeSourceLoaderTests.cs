using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInputs;
using CreationsForge.Starfield.Native;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Strings;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Starfield;

/// <summary>
/// Verifies Starfield's explicit native source acquisition, baseline, and owned-lifetime behavior.
/// </summary>
public sealed class StarfieldNativeSourceLoaderTests
{
    /// <summary>
    /// Verifies full, small, and medium native identities, localized strings, and exact source-byte preservation.
    /// </summary>
    /// <returns>A task that completes after the native source lifetime and fixture are released.</returns>
    [Fact]
    public async Task OpenAsync_LoadsMixedMasterStylesAndPreservesSourceArtifacts()
    {
        using var fixture = StarfieldNativeTestFixture.Create();
        var before = fixture.SnapshotArtifacts();
        var request = fixture.CreateOpenRequest();
        var loader = new StarfieldNativeSourceLoader(new NativeSourceInputLoader());

        var result = await loader.OpenAsync(request, TestContext.Current.CancellationToken);

        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        result.WorkspaceId.ShouldBe(request.WorkspaceId);
        var openResult = result.Value!;
        var sources = openResult.Sources.ShouldBeOfType<StarfieldNativeSourceSet>();
        try
        {
            sources.WorkspaceId.ShouldBe(request.WorkspaceId);
            sources.Revision.BaselineId.ShouldBe(openResult.BaselineId);
            sources.Revision.Sequence.ShouldBe(0UL);
            result.ResultRevision.ShouldBe(sources.Revision);
            sources.Baseline.Artifacts.ShouldNotBeEmpty();

            var small = sources.Resolve(
                new ReferenceRequest(fixture.SourceListFormKey, RecordScope.Source),
                TestContext.Current.CancellationToken);
            var full = sources.Resolve(
                new ReferenceRequest(fixture.FullListFormKey, RecordScope.WinningOverrides),
                TestContext.Current.CancellationToken);
            var medium = sources.Resolve(
                new ReferenceRequest(fixture.MediumListFormKey, RecordScope.WinningOverrides),
                TestContext.Current.CancellationToken);

            small.Succeeded.ShouldBeTrue(small.Error?.Message);
            full.Succeeded.ShouldBeTrue(full.Error?.Message);
            medium.Succeeded.ShouldBeTrue(medium.Error?.Message);
            var smallResolution = small.Value.ShouldNotBeNull();
            var fullResolution = full.Value.ShouldNotBeNull();
            var mediumResolution = medium.Value.ShouldNotBeNull();
            smallResolution.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
            fullResolution.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
            mediumResolution.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
            smallResolution.ContainingModKey.ShouldBe(fixture.SourceModKey);
            fullResolution.ContainingModKey.ShouldBe(fixture.FullModKey);
            mediumResolution.ContainingModKey.ShouldBe(fixture.MediumModKey);

            fixture.SourceListFormKey.ID.ShouldBe(fixture.FullListFormKey.ID);
            fixture.SourceListFormKey.ID.ShouldBe(fixture.MediumListFormKey.ID);

            var sourceList = smallResolution.Record.ShouldBeOfType<FormList>();
            var sourceListName = sourceList.Name.ShouldNotBeNull();
            sourceListName.StringsKey.ShouldBeNull();
            sourceListName.TryLookup(Language.English, out var english).ShouldBeTrue();
            sourceListName.TryLookup(Language.French, out var french).ShouldBeTrue();
            english.ShouldBe("Small source list");
            french.ShouldBe("Liste source petite");
            sourceList.Items.Select(item => item.FormKey).ShouldBe(new[] { fixture.BookFormKey, fixture.KeywordFormKey });
        }
        finally
        {
            await sources.DisposeAsync();
        }

        var after = fixture.SnapshotArtifacts();
        after.Keys.ShouldBe(before.Keys, ignoreOrder: false);
        foreach (var artifact in before)
        {
            after[artifact.Key].ShouldBe(artifact.Value);
        }
    }

    /// <summary>
    /// Verifies that unadmitted native masters and duplicate embedded plugin identities fail the source open.
    /// </summary>
    /// <returns>A task that completes after both rejected opens release their temporary input state.</returns>
    [Fact]
    public async Task OpenAsync_RejectsMissingMasterAndAmbiguousPluginIdentity()
    {
        using var fixture = StarfieldNativeTestFixture.Create();
        var loader = new StarfieldNativeSourceLoader(new NativeSourceInputLoader());
        var missingMasterRequest = fixture.CreateOpenRequest(
            fixture.PatchPluginPath,
            [fixture.PatchPluginPath]);

        var missingMaster = await loader.OpenAsync(
            missingMasterRequest,
            TestContext.Current.CancellationToken);

        missingMaster.Succeeded.ShouldBeFalse();
        missingMaster.Error.ShouldNotBeNull();
        missingMaster.Error.Message.ShouldContain(fixture.SourceModKey.FileName.ToString());

        var duplicatePath = fixture.CreateAmbiguousSourceCopy();
        var ambiguousRequest = fixture.CreateOpenRequest(
            fixture.SourcePluginPath,
            [fixture.SourcePluginPath, duplicatePath]);

        var ambiguous = await loader.OpenAsync(
            ambiguousRequest,
            TestContext.Current.CancellationToken);

        ambiguous.Succeeded.ShouldBeFalse();
        ambiguous.Error.ShouldNotBeNull();
        ambiguous.Error.Message.ShouldContain(fixture.SourceModKey.FileName.ToString());
    }

    /// <summary>
    /// Verifies mismatched game/release pairs and source-open cancellation do not publish a partial native lifetime.
    /// </summary>
    /// <returns>A task that completes after cancellation is observed.</returns>
    [Fact]
    public async Task OpenAsync_RejectsOtherGamesAndPropagatesCancellation()
    {
        using var fixture = StarfieldNativeTestFixture.Create();
        var loader = new StarfieldNativeSourceLoader(new NativeSourceInputLoader());
        var wrongGameRequest = new WorkspaceOpenRequest(
            Guid.NewGuid(),
            CreationsForge.Core.Enums.SupportedGame.Fallout4,
            Mutagen.Bethesda.GameRelease.Starfield,
            fixture.SourcePluginPath,
            [fixture.SourcePluginPath],
            fixture.DataDirectory.FullName,
            [fixture.StringsDirectory.FullName]);

        var wrongGame = await loader.OpenAsync(wrongGameRequest, TestContext.Current.CancellationToken);

        wrongGame.Succeeded.ShouldBeFalse();
        wrongGame.Error!.Code.ShouldBe(EngineErrorCode.UnsupportedGameRelease);

        using var cancellationSource = new CancellationTokenSource();
        cancellationSource.Cancel();

        await Should.ThrowAsync<OperationCanceledException>(async () =>
            await loader.OpenAsync(fixture.CreateOpenRequest(), cancellationSource.Token));
    }

    /// <summary>
    /// Verifies input drift is reported and disposed lifetimes reject later source operations.
    /// </summary>
    /// <returns>A task that completes after drift detection and idempotent disposal.</returns>
    [Fact]
    public async Task SourceSet_DetectsChangedInputAndRejectsOperationsAfterDisposal()
    {
        using var fixture = StarfieldNativeTestFixture.Create();
        var loader = new StarfieldNativeSourceLoader(new NativeSourceInputLoader());
        var open = await loader.OpenAsync(fixture.CreateOpenRequest(), TestContext.Current.CancellationToken);
        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        var sources = open.Value!.Sources.ShouldBeOfType<StarfieldNativeSourceSet>();

        File.AppendAllText(fixture.PatchPluginPath, "changed");
        var verification = await sources.VerifyUnchangedAsync(TestContext.Current.CancellationToken);

        verification.Succeeded.ShouldBeFalse();
        verification.Error!.Code.ShouldBe(EngineErrorCode.ExternalChangeDetected);

        await sources.DisposeAsync();
        await sources.DisposeAsync();

        var disposedResolution = sources.Resolve(
            new ReferenceRequest(fixture.SourceListFormKey, RecordScope.Source),
            TestContext.Current.CancellationToken);
        var disposedVerification = await sources.VerifyUnchangedAsync(TestContext.Current.CancellationToken);
        disposedResolution.Succeeded.ShouldBeFalse();
        disposedResolution.Error!.Code.ShouldBe(EngineErrorCode.WorkspaceDisposed);
        disposedVerification.Succeeded.ShouldBeFalse();
        disposedVerification.Error!.Code.ShouldBe(EngineErrorCode.WorkspaceDisposed);
    }
}
