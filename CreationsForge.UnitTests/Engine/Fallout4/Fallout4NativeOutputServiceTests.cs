using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInputs;
using CreationsForge.Core.Engine.NativeOutputs;
using CreationsForge.Fallout4.Native;
using Mutagen.Bethesda.Fallout4;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Fallout4;

/// <summary>Verifies complete Fallout 4 output selection, native preservation, cloning, reopening, and lifecycle.</summary>
public sealed class Fallout4NativeOutputServiceTests
{
    /// <summary>CreateNew produces complete full and small in-memory outputs without writing destination artifacts.</summary>
    /// <returns>A task that completes after both native output states are disposed.</returns>
    [Fact]
    public async Task OpenAsync_CreateNew_ConfiguresNativeHeadersWithoutWritingArtifacts()
    {
        using var fixture = Fallout4NativeOutputTestFixture.Create();
        var sourceBytes = fixture.Sources.SnapshotArtifacts();
        await using var sources = await OpenSourcesAsync(fixture);
        var service = CreateService();

        var fullAssociation = fixture.CreateAssociation("CreatedFull.esm");
        var fullResult = await service.OpenAsync(
            sources,
            new SelectOutputRequest(
                Guid.NewGuid(),
                sources.Revision,
                OutputSelectionMode.CreateNew,
                fullAssociation),
            TestContext.Current.CancellationToken);

        fullResult.Succeeded.ShouldBeTrue(fullResult.Error?.Message);
        await using var fullState = fullResult.Value!.Output.ShouldBeOfType<Fallout4NativeOutputState>();
        var full = fullState.CreateSnapshot(TestContext.Current.CancellationToken);
        full.ModKey.ShouldBe(fullAssociation.ModKey);
        full.IsMaster.ShouldBeTrue();
        full.IsSmallMaster.ShouldBeFalse();
        full.IsMediumMaster.ShouldBeFalse();
        full.UsingLocalization.ShouldBeFalse();
        full.EnumerateMajorRecords().ShouldBeEmpty();
        fullState.CreateOriginalSnapshot(TestContext.Current.CancellationToken)
            .EnumerateMajorRecords().ShouldBeEmpty();
        fullResult.Value.Association.PluginPath.ShouldBe(Path.GetFullPath(fullAssociation.PluginPath));
        fullResult.Value.Baseline.Artifacts.ShouldAllBe(artifact => !artifact.Fingerprint.Exists);
        File.Exists(fullAssociation.PluginPath).ShouldBeFalse();

        var reopenedNew = await service.ReopenAsync(
            sources,
            fullResult.Value.Association,
            fullResult.Value.Baseline,
            TestContext.Current.CancellationToken);
        reopenedNew.Succeeded.ShouldBeTrue(reopenedNew.Error?.Message);
        await using var reopenedNewState = reopenedNew.Value!.Output
            .ShouldBeOfType<Fallout4NativeOutputState>();
        reopenedNewState.CreateSnapshot().EnumerateMajorRecords().ShouldBeEmpty();
        File.Exists(fullAssociation.PluginPath).ShouldBeFalse();

        var smallAssociation = fixture.CreateAssociation(
            "CreatedSmall.esm",
            OutputMasterStyle.Small,
            LocalizedOutputMode.SeparateStringFiles);
        var smallResult = await service.OpenAsync(
            sources,
            new SelectOutputRequest(
                Guid.NewGuid(),
                sources.Revision,
                OutputSelectionMode.CreateNew,
                smallAssociation),
            TestContext.Current.CancellationToken);

        smallResult.Succeeded.ShouldBeTrue(smallResult.Error?.Message);
        await using var smallState = smallResult.Value!.Output.ShouldBeOfType<Fallout4NativeOutputState>();
        var small = smallState.CreateSnapshot(TestContext.Current.CancellationToken);
        small.IsMaster.ShouldBeTrue();
        small.IsSmallMaster.ShouldBeTrue();
        small.IsMediumMaster.ShouldBeFalse();
        small.UsingLocalization.ShouldBeTrue();
        File.Exists(smallAssociation.PluginPath).ShouldBeFalse();
        Directory.Exists(Path.Combine(fixture.OutputDirectory.FullName, "Strings")).ShouldBeFalse();

        var mediumAssociation = fixture.CreateAssociation("CreatedMedium.esm", OutputMasterStyle.Medium);
        var mediumResult = await service.OpenAsync(
            sources,
            new SelectOutputRequest(
                Guid.NewGuid(),
                sources.Revision,
                OutputSelectionMode.CreateNew,
                mediumAssociation),
            TestContext.Current.CancellationToken);
        mediumResult.Succeeded.ShouldBeFalse();
        mediumResult.Error!.Code.ShouldBe(EngineErrorCode.UnsupportedInput);
        File.Exists(mediumAssociation.PluginPath).ShouldBeFalse();
        AssertArtifactsUnchanged(sourceBytes);
    }

    /// <summary>OpenExisting and Clone retain complete headers and record families while exposing only defensive copies.</summary>
    /// <returns>A task that completes after both independent output states are disposed.</returns>
    [Fact]
    public async Task OpenExistingAndClone_PreserveCompleteNativeStateAndIsolation()
    {
        using var fixture = Fallout4NativeOutputTestFixture.Create();
        var existing = fixture.WriteExistingOutput();
        var outputBytes = File.ReadAllBytes(existing.Association.PluginPath);
        var sourceBytes = fixture.Sources.SnapshotArtifacts();
        await using var sources = await OpenSourcesAsync(fixture);
        var service = CreateService();

        var open = await service.OpenAsync(
            sources,
            new SelectOutputRequest(
                Guid.NewGuid(),
                sources.Revision,
                OutputSelectionMode.OpenExisting,
                existing.Association),
            TestContext.Current.CancellationToken);

        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        var state = open.Value!.Output.ShouldBeOfType<Fallout4NativeOutputState>();
        var current = state.CreateSnapshot(TestContext.Current.CancellationToken);
        AssertCompleteExistingState(current, existing);
        AssertCompleteExistingState(
            state.CreateOriginalSnapshot(TestContext.Current.CancellationToken),
            existing);

        current.ModHeader.Author = "Caller mutation";
        current.FormLists.Clear();
        current.Keywords.Clear();
        AssertCompleteExistingState(
            state.CreateSnapshot(TestContext.Current.CancellationToken),
            existing);

        var clone = service.Clone(state, TestContext.Current.CancellationToken);
        var begin = service.BeginEdit(
            sources,
            clone,
            new BeginEditRequest(Guid.NewGuid(), sources.Revision, FormListEditRole.New),
            TestContext.Current.CancellationToken);
        begin.Succeeded.ShouldBeTrue(begin.Error?.Message);
        clone.CreateSnapshot(TestContext.Current.CancellationToken).FormLists.Count.ShouldBe(4);
        state.CreateSnapshot(TestContext.Current.CancellationToken).FormLists.Count.ShouldBe(3);

        await state.DisposeAsync();
        AssertCompleteExistingState(
            clone.CreateOriginalSnapshot(TestContext.Current.CancellationToken),
            existing);
        await clone.DisposeAsync();
        File.ReadAllBytes(existing.Association.PluginPath).ShouldBe(outputBytes);
        AssertArtifactsUnchanged(sourceBytes);
    }

    /// <summary>Reopen accepts the exact artifact baseline and rejects a valid native rewrite with a different baseline.</summary>
    /// <returns>A task that completes after reopened states and sources are disposed.</returns>
    [Fact]
    public async Task ReopenAsync_RequiresExactOutputArtifactBaseline()
    {
        using var fixture = Fallout4NativeOutputTestFixture.Create();
        var existing = fixture.WriteExistingOutput();
        var sourceBytes = fixture.Sources.SnapshotArtifacts();
        await using var sources = await OpenSourcesAsync(fixture);
        var service = CreateService();
        var open = await service.OpenAsync(
            sources,
            new SelectOutputRequest(
                Guid.NewGuid(),
                sources.Revision,
                OutputSelectionMode.OpenExisting,
                existing.Association),
            TestContext.Current.CancellationToken);
        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        await using var initial = open.Value!.Output.ShouldBeOfType<Fallout4NativeOutputState>();

        var unchanged = await service.ReopenAsync(
            sources,
            open.Value.Association,
            open.Value.Baseline,
            TestContext.Current.CancellationToken);
        unchanged.Succeeded.ShouldBeTrue(unchanged.Error?.Message);
        await using var unchangedState = unchanged.Value!.Output.ShouldBeOfType<Fallout4NativeOutputState>();
        AssertCompleteExistingState(unchangedState.CreateSnapshot(), existing);

        var forgedArtifacts = open.Value.Baseline.Artifacts
            .Select(artifact => artifact.Role == NativeArtifactRole.Plugin
                ? new NativeArtifactAssociation(
                    artifact.Path,
                    artifact.Role,
                    artifact.Language,
                    new NativeArtifactFingerprint(
                        exists: true,
                        artifact.Fingerprint.Length + 1,
                        artifact.Fingerprint.Sha256),
                    artifact.FileIdentity)
                : artifact)
            .ToArray();
        var forgedBaseline = new OutputArtifactSetBaseline(
            open.Value.Baseline.BaselineId,
            forgedArtifacts);
        var forged = await service.ReopenAsync(
            sources,
            open.Value.Association,
            forgedBaseline,
            TestContext.Current.CancellationToken);
        forged.Succeeded.ShouldBeFalse();
        forged.Error!.Code.ShouldBe(EngineErrorCode.ExternalChangeDetected);

        fixture.WriteExistingOutput(author: "Rewritten output author");
        var changed = await service.ReopenAsync(
            sources,
            open.Value.Association,
            open.Value.Baseline,
            TestContext.Current.CancellationToken);
        changed.Succeeded.ShouldBeFalse();
        changed.Error!.Code.ShouldBe(EngineErrorCode.ExternalChangeDetected);
        AssertArtifactsUnchanged(sourceBytes);
    }

    /// <summary>Game-native output operations accept the workspace's composite revision after output selection.</summary>
    /// <returns>A task that completes after the simulated post-selection edit candidate is released.</returns>
    [Fact]
    public async Task OutputOperations_AcceptWorkspaceOwnedCompositeRevisions()
    {
        using var fixture = Fallout4NativeOutputTestFixture.Create();
        await using var sources = await OpenSourcesAsync(fixture);
        var service = CreateService();
        var selectionRevision = new WorkspaceRevision(Guid.NewGuid(), 4);
        var association = fixture.CreateAssociation("CompositeRevision.esm");
        var openRequest = new SelectOutputRequest(
            Guid.NewGuid(),
            selectionRevision,
            OutputSelectionMode.CreateNew,
            association);

        var open = await service.OpenAsync(
            sources,
            openRequest,
            TestContext.Current.CancellationToken);

        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        open.BaseRevision.ShouldBe(selectionRevision);
        open.ResultRevision.ShouldBe(selectionRevision);
        await using var state = open.Value!.Output.ShouldBeOfType<Fallout4NativeOutputState>();
        var stagedRevision = selectionRevision.Next();
        var beginRequest = new BeginEditRequest(
            Guid.NewGuid(),
            stagedRevision,
            FormListEditRole.New);
        var begin = service.BeginEdit(
            sources,
            state,
            beginRequest,
            TestContext.Current.CancellationToken);

        begin.Succeeded.ShouldBeTrue(begin.Error?.Message);
        begin.BaseRevision.ShouldBe(stagedRevision);
        begin.ResultRevision.ShouldBe(stagedRevision);
        state.CreateSnapshot().FormLists.ShouldHaveSingleItem()
            .FormKey.ShouldBe(begin.Value!.FormKey);
        File.Exists(association.PluginPath).ShouldBeFalse();
    }

    /// <summary>Cancellation and disposal close output operations without taking ownership of source artifacts.</summary>
    /// <returns>A task that completes after cancellation and disposal paths are observed.</returns>
    [Fact]
    public async Task OutputLifecycle_ObservesCancellationAndIndependentDisposal()
    {
        using var fixture = Fallout4NativeOutputTestFixture.Create();
        var association = fixture.CreateAssociation("Lifecycle.esm");
        await using var sources = await OpenSourcesAsync(fixture);
        var service = CreateService();
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Should.ThrowAsync<OperationCanceledException>(() => service.OpenAsync(
            sources,
            new SelectOutputRequest(
                Guid.NewGuid(),
                sources.Revision,
                OutputSelectionMode.CreateNew,
                association),
            cancellation.Token));
        File.Exists(association.PluginPath).ShouldBeFalse();

        var open = await service.OpenAsync(
            sources,
            new SelectOutputRequest(
                Guid.NewGuid(),
                sources.Revision,
                OutputSelectionMode.CreateNew,
                association),
            TestContext.Current.CancellationToken);
        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        var state = open.Value!.Output.ShouldBeOfType<Fallout4NativeOutputState>();
        await state.DisposeAsync();
        await state.DisposeAsync();
        Should.Throw<ObjectDisposedException>(() => state.CreateSnapshot());
        Should.Throw<ObjectDisposedException>(() => service.Clone(state, CancellationToken.None));

        var disposedBegin = service.BeginEdit(
            sources,
            state,
            new BeginEditRequest(Guid.NewGuid(), sources.Revision, FormListEditRole.New),
            CancellationToken.None);
        disposedBegin.Succeeded.ShouldBeFalse();
        disposedBegin.Error!.Code.ShouldBe(EngineErrorCode.WorkspaceDisposed);

        var sourceVerification = await sources.VerifyUnchangedAsync(TestContext.Current.CancellationToken);
        sourceVerification.Succeeded.ShouldBeTrue(sourceVerification.Error?.Message);
        await sources.DisposeAsync();
        var disposedOpen = await service.OpenAsync(
            sources,
            new SelectOutputRequest(
                Guid.NewGuid(),
                sources.Revision,
                OutputSelectionMode.CreateNew,
                fixture.CreateAssociation("DisposedSource.esm")),
            TestContext.Current.CancellationToken);
        disposedOpen.Succeeded.ShouldBeFalse();
        disposedOpen.Error!.Code.ShouldBe(EngineErrorCode.WorkspaceDisposed);
    }

    /// <summary>Opens the generated Fallout 4 source set used by output tests.</summary>
    /// <param name="fixture">The generated combined output and source fixture.</param>
    /// <returns>The independently owned complete native source set.</returns>
    private static async Task<Fallout4NativeSourceSet> OpenSourcesAsync(
        Fallout4NativeOutputTestFixture fixture)
    {
        var open = await new Fallout4NativeSourceLoader(new NativeSourceInputLoader()).OpenAsync(
            fixture.Sources.CreateOpenRequest(),
            TestContext.Current.CancellationToken);
        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        return open.Value!.Sources.ShouldBeOfType<Fallout4NativeSourceSet>();
    }

    /// <summary>Creates a Fallout 4 output service with the production shared admission boundary.</summary>
    /// <returns>A production output service.</returns>
    private static Fallout4NativeOutputService CreateService()
    {
        return new Fallout4NativeOutputService(new NativeOutputInputLoader());
    }

    /// <summary>Asserts every generated source plugin retains its exact bytes.</summary>
    /// <param name="expected">The source artifact bytes captured before output operations.</param>
    private static void AssertArtifactsUnchanged(IReadOnlyDictionary<string, byte[]> expected)
    {
        var actualPaths = expected.Keys.OrderBy(path => path, StringComparer.OrdinalIgnoreCase).ToArray();
        foreach (var path in actualPaths)
        {
            File.ReadAllBytes(path).ShouldBe(expected[path]);
        }
    }

    /// <summary>Asserts the complete generated existing output survives materialization or cloning.</summary>
    /// <param name="mod">The defensive complete native output snapshot.</param>
    /// <param name="existing">The expected output association and record identities.</param>
    private static void AssertCompleteExistingState(
        Fallout4Mod mod,
        (
            OutputAssociation Association,
            Mutagen.Bethesda.Plugins.FormKey OwnListFormKey,
            Mutagen.Bethesda.Plugins.FormKey SourceOverrideFormKey,
            Mutagen.Bethesda.Plugins.FormKey DeletedListFormKey,
            Mutagen.Bethesda.Plugins.FormKey KeywordFormKey,
            Mutagen.Bethesda.Plugins.FormKey BookFormKey) existing)
    {
        mod.ModKey.ShouldBe(existing.Association.ModKey);
        mod.IsMaster.ShouldBeTrue();
        mod.IsSmallMaster.ShouldBeFalse();
        mod.IsMediumMaster.ShouldBeFalse();
        mod.UsingLocalization.ShouldBeFalse();
        mod.ModHeader.Author.ShouldBe("Output fixture author");
        mod.ModHeader.Description.ShouldBe("Complete Fallout 4 output fixture");
        mod.ModHeader.Version.ShouldBe(13);
        mod.ModHeader.FormVersion.ShouldBe((ushort)44);
        mod.ModHeader.Version2.ShouldBe((ushort)7);
        mod.ModHeader.INCC.ShouldBe(3);

        var ownList = mod.FormLists.Single(record => record.FormKey == existing.OwnListFormKey);
        ownList.Name.ShouldNotBeNull();
        ownList.Name!.TargetLanguage.ShouldBe(Mutagen.Bethesda.Strings.Language.English);
        ownList.Name.String.ShouldBe("Output list");
        ownList.Items.Select(item => item.FormKey).ShouldBe(
        [
            existing.KeywordFormKey,
            existing.KeywordFormKey,
            existing.BookFormKey,
        ], ignoreOrder: false);
        mod.Keywords.ShouldContain(record => record.FormKey == existing.KeywordFormKey);
        mod.FormLists.Single(record => record.FormKey == existing.SourceOverrideFormKey)
            .EditorID.ShouldBe("ExistingSourceOverride");
        mod.FormLists.Single(record => record.FormKey == existing.DeletedListFormKey)
            .IsDeleted.ShouldBeTrue();
    }
}
