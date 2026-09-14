using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.PluginInputs;
using CreationsForge.Core.Engine.PluginOutputs;
using CreationsForge.Skyrim.PluginAdapter;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Skyrim;

/// <summary>
/// Verifies complete Skyrim output selection, materialization, cloning, baseline, and disposal behavior.
/// </summary>
public sealed class SkyrimPluginOutputServiceTests
{
    /// <summary>
    /// Verifies new full and small .esm outputs have exact plugin flags and create no destination artifacts.
    /// </summary>
    /// <param name="masterStyle">The requested Skyrim-supported plugin master style.</param>
    /// <param name="expectedSmallMaster">Whether the in-memory plugin header must carry the small-master flag.</param>
    /// <returns>A task that completes after every plugin lifetime is released.</returns>
    [Theory]
    [InlineData(OutputMasterStyle.Full, false)]
    [InlineData(OutputMasterStyle.Small, true)]
    public async Task OpenAsync_CreateNewBuildsExactInMemoryEsmWithoutWriting(
        OutputMasterStyle masterStyle,
        bool expectedSmallMaster)
    {
        using var fixture = SkyrimPluginOutputTestFixture.Create();
        var sourceBefore = fixture.Sources.SnapshotArtifacts();
        var outputBefore = fixture.SnapshotOutputArtifacts();
        var sources = await OpenSourcesAsync(fixture.Sources);
        try
        {
            var association = fixture.CreateNewAssociation(
                $"New{masterStyle}.esm",
                LocalizedOutputMode.SeparateStringFiles,
                masterStyle);
            var request = new SelectOutputRequest(
                Guid.NewGuid(),
                sources.Revision,
                OutputSelectionMode.CreateNew,
                association);
            var service = new SkyrimPluginOutputService(new PluginOutputInputLoader());

            var result = await service.OpenAsync(
                sources,
                request,
                TestContext.Current.CancellationToken);

            result.Succeeded.ShouldBeTrue(result.Error?.Message);
            result.WorkspaceId.ShouldBe(sources.WorkspaceId);
            result.OperationId.ShouldBe(request.OperationId);
            result.BaseRevision.ShouldBe(request.ExpectedRevision);
            result.ResultRevision.ShouldBe(request.ExpectedRevision);
            var state = result.Value!.Output.ShouldBeOfType<SkyrimPluginOutputState>();
            try
            {
                var snapshot = state.CreateSnapshot(TestContext.Current.CancellationToken);
                snapshot.ModKey.ShouldBe(association.ModKey);
                snapshot.IsMaster.ShouldBeTrue();
                snapshot.IsSmallMaster.ShouldBe(expectedSmallMaster);
                snapshot.IsMediumMaster.ShouldBeFalse();
                snapshot.UsingLocalization.ShouldBeTrue();
                snapshot.EnumerateMajorRecords().ShouldBeEmpty();
                state.CreateOriginalSnapshot(TestContext.Current.CancellationToken)
                    .EnumerateMajorRecords().ShouldBeEmpty();
                state.Association.PluginPath.ShouldBe(Path.GetFullPath(association.PluginPath));
                state.Baseline.Artifacts.Single(artifact => artifact.Role == PluginArtifactRole.Plugin)
                    .Fingerprint.Exists.ShouldBeFalse();
                File.Exists(association.PluginPath).ShouldBeFalse();
            }
            finally
            {
                await state.DisposeAsync();
            }
        }
        finally
        {
            await sources.DisposeAsync();
        }

        AssertArtifactsEqual(sourceBefore, fixture.Sources.SnapshotArtifacts());
        AssertArtifactsEqual(outputBefore, fixture.SnapshotOutputArtifacts());
    }

    /// <summary>
    /// Verifies an existing output is fully parsed with unrelated records, exact FormList fields, and all translated values preserved.
    /// </summary>
    /// <returns>A task that completes after initial and reopened output state are independently released.</returns>
    [Fact]
    public async Task OpenAndReopenAsync_PreserveCompleteExistingOutputAndExactBaseline()
    {
        using var fixture = SkyrimPluginOutputTestFixture.Create();
        var sourceBefore = fixture.Sources.SnapshotArtifacts();
        var outputBefore = fixture.SnapshotOutputArtifacts();
        var sources = await OpenSourcesAsync(fixture.Sources);
        var service = new SkyrimPluginOutputService(new PluginOutputInputLoader());
        try
        {
            var association = fixture.CreateExistingAssociation();
            var open = await service.OpenAsync(
                sources,
                new SelectOutputRequest(
                    Guid.NewGuid(),
                    sources.Revision,
                    OutputSelectionMode.OpenExisting,
                    association),
                TestContext.Current.CancellationToken);

            open.Succeeded.ShouldBeTrue(open.Error?.Message);
            var state = open.Value!.Output.ShouldBeOfType<SkyrimPluginOutputState>();
            try
            {
                var snapshot = state.CreateSnapshot(TestContext.Current.CancellationToken);
                snapshot.ModKey.ShouldBe(fixture.ExistingOutputModKey);
                snapshot.IsMaster.ShouldBeTrue();
                snapshot.IsSmallMaster.ShouldBeFalse();
                snapshot.UsingLocalization.ShouldBeTrue();
                snapshot.Keywords.Single(keyword => keyword.FormKey == fixture.OutputKeywordFormKey)
                    .EditorID.ShouldBe("OutputKeyword");
                var book = snapshot.Books.Single(record => record.FormKey == fixture.OutputBookFormKey);
                var name = book.Name.ShouldNotBeNull();
                name.TryLookup(Language.English, out var english).ShouldBeTrue();
                name.TryLookup(Language.French, out var french).ShouldBeTrue();
                english.ShouldBe("Output book");
                french.ShouldBe("Livre de sortie");

                var formList = snapshot.FormLists.Single(record => record.FormKey == fixture.OutputOwnListFormKey);
                formList.EditorID.ShouldBe("ExistingOwnList");
                formList.FormVersion.ShouldBe((ushort)44);
                formList.Version2.ShouldBe((ushort)7);
                formList.VersionControl.ShouldBe(0x01020304u);
                formList.MajorRecordFlagsRaw.ShouldBe(unchecked((int)0x40080000));
                formList.Items.Select(item => item.FormKey).ShouldBe(
                [
                    fixture.Sources.BookFormKey,
                    fixture.OutputKeywordFormKey,
                    fixture.OutputKeywordFormKey,
                    FormKey.Null,
                    fixture.Sources.MissingFormKey,
                ]);

                snapshot.FormLists.Single(record => record.FormKey == fixture.Sources.SourceListFormKey)
                    .EditorID.ShouldBe("ExistingStagedOverride");
                snapshot.EnumerateMajorRecords().Select(record => record.FormKey).Distinct().Count()
                    .ShouldBe(snapshot.EnumerateMajorRecords().Count());
                formList.EditorID = "MutatedDefensiveCopy";
                state.CreateSnapshot(TestContext.Current.CancellationToken)
                    .FormLists.Single(record => record.FormKey == fixture.OutputOwnListFormKey)
                    .EditorID.ShouldBe("ExistingOwnList");
                state.CreateOriginalSnapshot(TestContext.Current.CancellationToken)
                    .FormLists.Single(record => record.FormKey == fixture.OutputOwnListFormKey)
                    .EditorID.ShouldBe("ExistingOwnList");

                var reopen = await service.ReopenAsync(
                    sources,
                    state.Association,
                    state.Baseline,
                    TestContext.Current.CancellationToken);
                reopen.Succeeded.ShouldBeTrue(reopen.Error?.Message);
                var reopenedState = reopen.Value!.Output.ShouldBeOfType<SkyrimPluginOutputState>();
                try
                {
                    reopenedState.Baseline.BaselineId.ShouldBe(state.Baseline.BaselineId);
                    reopenedState.CreateSnapshot(TestContext.Current.CancellationToken)
                        .EnumerateMajorRecords().Select(record => record.FormKey)
                        .ShouldBe(state.CreateSnapshot(TestContext.Current.CancellationToken)
                            .EnumerateMajorRecords().Select(record => record.FormKey));
                }
                finally
                {
                    await reopenedState.DisposeAsync();
                }
            }
            finally
            {
                await state.DisposeAsync();
            }
        }
        finally
        {
            await sources.DisposeAsync();
        }

        AssertArtifactsEqual(sourceBefore, fixture.Sources.SnapshotArtifacts());
        AssertArtifactsEqual(outputBefore, fixture.SnapshotOutputArtifacts());
    }

    /// <summary>
    /// Verifies unsupported master style, cancellation, and released output state fail without leaving artifacts or live state.
    /// </summary>
    /// <returns>A task that completes after cancellation and idempotent output disposal are observed.</returns>
    [Fact]
    public async Task OutputLifecycle_RejectsMediumMasterAndPropagatesCancellationAndDisposal()
    {
        using var fixture = SkyrimPluginOutputTestFixture.Create();
        var sources = await OpenSourcesAsync(fixture.Sources);
        var service = new SkyrimPluginOutputService(new PluginOutputInputLoader());
        try
        {
            var mediumAssociation = fixture.CreateNewAssociation(
                "UnsupportedMedium.esm",
                masterStyle: OutputMasterStyle.Medium);
            var medium = await service.OpenAsync(
                sources,
                new SelectOutputRequest(
                    Guid.NewGuid(),
                    sources.Revision,
                    OutputSelectionMode.CreateNew,
                    mediumAssociation),
                TestContext.Current.CancellationToken);
            medium.Succeeded.ShouldBeFalse();
            medium.Error!.Code.ShouldBe(EngineErrorCode.UnsupportedInput);
            File.Exists(mediumAssociation.PluginPath).ShouldBeFalse();

            using var cancellationSource = new CancellationTokenSource();
            cancellationSource.Cancel();
            var canceledAssociation = fixture.CreateNewAssociation("Canceled.esm");
            await Should.ThrowAsync<OperationCanceledException>(async () =>
                await service.OpenAsync(
                    sources,
                    new SelectOutputRequest(
                        Guid.NewGuid(),
                        sources.Revision,
                        OutputSelectionMode.CreateNew,
                        canceledAssociation),
                    cancellationSource.Token));
            File.Exists(canceledAssociation.PluginPath).ShouldBeFalse();

            var open = await service.OpenAsync(
                sources,
                new SelectOutputRequest(
                    Guid.NewGuid(),
                    sources.Revision,
                    OutputSelectionMode.OpenExisting,
                    fixture.CreateExistingAssociation()),
                TestContext.Current.CancellationToken);
            open.Succeeded.ShouldBeTrue(open.Error?.Message);
            var state = open.Value!.Output.ShouldBeOfType<SkyrimPluginOutputState>();
            Should.Throw<OperationCanceledException>(() => service.Clone(state, cancellationSource.Token));
            await state.DisposeAsync();
            await state.DisposeAsync();
            Should.Throw<ObjectDisposedException>(() => state.CreateSnapshot());
            Should.Throw<ObjectDisposedException>(() => service.Clone(state, TestContext.Current.CancellationToken));
        }
        finally
        {
            await sources.DisposeAsync();
        }
    }

    /// <summary>Opens the complete explicit Skyrim source set for one output test.</summary>
    /// <param name="fixture">The generated source fixture.</param>
    /// <returns>The independently disposable materialized Skyrim source set.</returns>
    private static async Task<SkyrimPluginSourceSet> OpenSourcesAsync(SkyrimPluginTestFixture fixture)
    {
        var result = await new SkyrimPluginSourceLoader(new PluginSourceInputLoader()).OpenAsync(
            fixture.CreateOpenRequest(),
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value!.Sources.ShouldBeOfType<SkyrimPluginSourceSet>();
    }

    /// <summary>Asserts two canonical artifact snapshots have identical paths and bytes.</summary>
    /// <param name="expected">The prior exact snapshot.</param>
    /// <param name="actual">The resulting exact snapshot.</param>
    private static void AssertArtifactsEqual(
        IReadOnlyDictionary<string, byte[]> expected,
        IReadOnlyDictionary<string, byte[]> actual)
    {
        actual.Keys.ShouldBe(expected.Keys, ignoreOrder: false);
        foreach (var artifact in expected)
        {
            actual[artifact.Key].ShouldBe(artifact.Value);
        }
    }
}
