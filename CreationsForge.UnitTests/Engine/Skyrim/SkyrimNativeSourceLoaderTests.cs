using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInputs;
using CreationsForge.Skyrim.Native;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Skyrim;

/// <summary>
/// Verifies Skyrim Special Edition's explicit native source acquisition, baseline, and owned-lifetime behavior.
/// </summary>
public sealed class SkyrimNativeSourceLoaderTests
{
    /// <summary>
    /// Verifies full and small native identities, localized strings, references, and exact source-byte preservation.
    /// </summary>
    /// <returns>A task that completes after the native source lifetime and fixture are released.</returns>
    [Fact]
    public async Task OpenAsync_LoadsFullAndSmallSourcesAndPreservesArtifacts()
    {
        using var fixture = SkyrimNativeTestFixture.Create();
        var before = fixture.SnapshotArtifacts();
        var request = fixture.CreateOpenRequest();
        var loader = new SkyrimNativeSourceLoader(new NativeSourceInputLoader());

        await VerifyParsedSourceLocalizationAsync(fixture);
        var result = await loader.OpenAsync(request, TestContext.Current.CancellationToken);

        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        result.WorkspaceId.ShouldBe(request.WorkspaceId);
        var openResult = result.Value!;
        var sources = openResult.Sources.ShouldBeOfType<SkyrimNativeSourceSet>();
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
            var book = sources.Resolve(
                new ReferenceRequest(fixture.BookFormKey, RecordScope.Source),
                TestContext.Current.CancellationToken);

            small.Succeeded.ShouldBeTrue(small.Error?.Message);
            full.Succeeded.ShouldBeTrue(full.Error?.Message);
            book.Succeeded.ShouldBeTrue(book.Error?.Message);
            var smallResolution = small.Value.ShouldNotBeNull();
            var fullResolution = full.Value.ShouldNotBeNull();
            var bookResolution = book.Value.ShouldNotBeNull();
            small.WorkspaceId.ShouldBe(request.WorkspaceId);
            small.ResultRevision.ShouldBe(sources.Revision);
            smallResolution.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
            fullResolution.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
            bookResolution.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
            smallResolution.ContainingModKey.ShouldBe(fixture.SourceModKey);
            fullResolution.ContainingModKey.ShouldBe(fixture.FullModKey);
            fixture.SourceListFormKey.ID.ShouldBe(fixture.FullListFormKey.ID);

            var smallRecord = smallResolution.Record
                ?? throw new InvalidOperationException("A resolved source FormList did not contain a detached native record.");
            var sourceList = smallRecord as IFormListGetter
                ?? throw new InvalidOperationException("The detached source record did not retain its Skyrim FormList family.");
            sourceList.Items.Select(item => item.FormKey).ShouldBe(
                [fixture.BookFormKey, fixture.MissingFormKey, fixture.KeywordFormKey, fixture.MissingFormKey]);
            var bookRecord = bookResolution.Record
                ?? throw new InvalidOperationException("A resolved source Book did not contain a detached native record.");
            var localizedBook = bookRecord as IBookGetter
                ?? throw new InvalidOperationException("The detached source record did not retain its Skyrim Book family.");
            var localizedName = localizedBook.Name
                ?? throw new InvalidOperationException("The detached source Book did not retain its localized Name.");
            localizedName.TryLookup(Language.English, out var english).ShouldBeTrue();
            localizedName.TryLookup(Language.French, out var french).ShouldBeTrue();
            english.ShouldBe("Referenced book");
            french.ShouldBe("Livre référence");
            var detachedName = localizedName as TranslatedString
                ?? throw new InvalidOperationException("The detached Book Name did not retain its native translated-string type.");
            detachedName.StringsKey.ShouldBeNull();
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
    /// Verifies a plugin whose required native master is absent fails before a source lifetime is published.
    /// </summary>
    /// <returns>A task that completes after the rejected open releases its temporary input state.</returns>
    [Fact]
    public async Task OpenAsync_RejectsMissingMaster()
    {
        using var fixture = SkyrimNativeTestFixture.Create();
        var loader = new SkyrimNativeSourceLoader(new NativeSourceInputLoader());
        var missingMasterRequest = fixture.CreateOpenRequest(
            fixture.PatchPluginPath,
            [fixture.PatchPluginPath]);

        var missingMaster = await loader.OpenAsync(
            missingMasterRequest,
            TestContext.Current.CancellationToken);

        missingMaster.Succeeded.ShouldBeFalse();
        missingMaster.Error.ShouldNotBeNull();
        missingMaster.Error.Message.ShouldContain(fixture.SourceModKey.FileName.String);
    }

    /// <summary>
    /// Verifies Mutagen rejects medium-master state for Skyrim Special Edition rather than emitting an ambiguous header.
    /// </summary>
    [Fact]
    public void NativeModel_RejectsMediumMasterStyle()
    {
        var mod = new SkyrimMod("UnsupportedMedium.esm", SkyrimRelease.SkyrimSE);

        var exception = Should.Throw<ArgumentException>(() => mod.IsMediumMaster = true);

        exception.Message.ShouldContain("unsupported", Case.Insensitive);
    }

    /// <summary>
    /// Verifies mismatched game/release pairs and source-open cancellation do not publish a partial native lifetime.
    /// </summary>
    /// <returns>A task that completes after validation and cancellation are observed.</returns>
    [Fact]
    public async Task OpenAsync_RejectsOtherGamesAndPropagatesCancellation()
    {
        using var fixture = SkyrimNativeTestFixture.Create();
        var loader = new SkyrimNativeSourceLoader(new NativeSourceInputLoader());
        var wrongGameRequest = new WorkspaceOpenRequest(
            Guid.NewGuid(),
            CreationsForge.Core.Enums.SupportedGame.Fallout4,
            Mutagen.Bethesda.GameRelease.SkyrimSE,
            fixture.SourcePluginPath,
            fixture.OrderedPluginPaths,
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
    /// Verifies source drift is reported and disposed lifetimes reject later source operations.
    /// </summary>
    /// <returns>A task that completes after drift detection and idempotent disposal.</returns>
    [Fact]
    public async Task SourceSet_DetectsChangedInputAndRejectsOperationsAfterDisposal()
    {
        using var fixture = SkyrimNativeTestFixture.Create();
        var loader = new SkyrimNativeSourceLoader(new NativeSourceInputLoader());
        var open = await loader.OpenAsync(fixture.CreateOpenRequest(), TestContext.Current.CancellationToken);
        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        var sources = open.Value!.Sources.ShouldBeOfType<SkyrimNativeSourceSet>();

        fixture.ChangeFullPlugin();
        var verification = await sources.VerifyUnchangedAsync(TestContext.Current.CancellationToken);

        verification.Succeeded.ShouldBeFalse();
        verification.Error!.Code.ShouldBe(EngineErrorCode.ExternalChangeDetected);

        await sources.DisposeAsync();
        await sources.DisposeAsync();

        var disposedResolution = sources.Resolve(
            new ReferenceRequest(fixture.SourceListFormKey, RecordScope.Source),
            TestContext.Current.CancellationToken);
        var disposedSearch = sources.Search(
            new ReferenceSearchRequest("Shared", 1, scope: RecordScope.Source),
            TestContext.Current.CancellationToken);
        var disposedVerification = await sources.VerifyUnchangedAsync(TestContext.Current.CancellationToken);
        disposedResolution.Succeeded.ShouldBeFalse();
        disposedResolution.Error!.Code.ShouldBe(EngineErrorCode.WorkspaceDisposed);
        disposedSearch.Succeeded.ShouldBeFalse();
        disposedSearch.Error!.Code.ShouldBe(EngineErrorCode.WorkspaceDisposed);
        disposedVerification.Succeeded.ShouldBeFalse();
        disposedVerification.Error!.Code.ShouldBe(EngineErrorCode.WorkspaceDisposed);
    }

    /// <summary>
    /// Confirms the parsed source retains its external string-table key before the detached native copy intentionally normalizes that writer-owned metadata.
    /// </summary>
    /// <param name="fixture">The generated localized Skyrim fixture.</param>
    /// <returns>A task that completes after the independently prepared native input lifetime is released.</returns>
    private static async Task VerifyParsedSourceLocalizationAsync(SkyrimNativeTestFixture fixture)
    {
        var inputResult = await new NativeSourceInputLoader().PrepareAsync(
            fixture.CreateOpenRequest(),
            TestContext.Current.CancellationToken);
        inputResult.Succeeded.ShouldBeTrue(inputResult.Error?.Message);
        var inputs = inputResult.Value.ShouldNotBeNull();

        try
        {
            var sourceInput = inputs.Plugins.Single(plugin => plugin.Role == PluginRole.Source);
            using MutagenBinaryReadStream stream = inputs.OpenReadStream(
                sourceInput,
                TestContext.Current.CancellationToken);
            var sourceMod = SkyrimMod.CreateFromBinary(
                new MutagenFrame(stream),
                SkyrimRelease.SkyrimSE,
                new GroupMask(true));
            var sourceBook = sourceMod.Books.Single(book => book.FormKey == fixture.BookFormKey);
            var sourceName = sourceBook.Name as TranslatedString
                ?? throw new InvalidOperationException("The parsed source Book did not retain its native translated Name.");

            sourceName.StringsKey.ShouldBe(1u);
            sourceName.TryLookup(Language.English, out var english).ShouldBeTrue();
            sourceName.TryLookup(Language.French, out var french).ShouldBeTrue();
            english.ShouldBe("Referenced book");
            french.ShouldBe("Livre référence");
        }
        finally
        {
            await inputs.DisposeAsync();
        }
    }
}
