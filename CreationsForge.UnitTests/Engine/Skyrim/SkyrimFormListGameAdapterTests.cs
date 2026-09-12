using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInputs;
using CreationsForge.Core.Engine.NativeOutputs;
using CreationsForge.Skyrim.Native;
using CreationsForge.Skyrim.Native.NativeInspection;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Skyrim;

/// <summary>
/// Verifies complete Skyrim adapter composition, output-aware reads, private serialization, and strict native reopen preservation.
/// </summary>
public sealed class SkyrimFormListGameAdapterTests
{
    /// <summary>Verifies staged output participates after sources in listing, winning selection, resolution, and bounded search.</summary>
    /// <returns>A task that completes after all native source and output lifetimes are released.</returns>
    [Fact]
    public async Task Adapter_ReadsStagedOutputAcrossCompleteParticipatingLoadOrder()
    {
        using var fixture = SkyrimNativeOutputTestFixture.Create();
        var adapter = CreateAdapter();
        await using var sources = await OpenSourcesAsync(adapter, fixture.Sources);
        await using var output = await OpenOutputAsync(
            adapter,
            sources,
            fixture.CreateExistingAssociation(),
            OutputSelectionMode.OpenExisting);

        var plugins = adapter.ListPlugins(sources, output, TestContext.Current.CancellationToken);
        plugins.Succeeded.ShouldBeTrue(plugins.Error?.Message);
        plugins.Value.ShouldNotBeNull();
        plugins.Value!.Last().ModKey.ShouldBe(fixture.ExistingOutputModKey);
        plugins.Value.Last().Role.ShouldBe(PluginRole.Output);
        plugins.Value.Last().LoadOrderIndex.ShouldBe(plugins.Value.Count - 1);

        var stagedLists = adapter.ListFormLists(
            sources,
            output,
            RecordScope.StagedOutput,
            TestContext.Current.CancellationToken);
        stagedLists.Succeeded.ShouldBeTrue(stagedLists.Error?.Message);
        stagedLists.Value.ShouldNotBeNull();
        stagedLists.Value!.ShouldContain(summary => summary.FormKey == fixture.OutputOwnListFormKey);
        stagedLists.Value.ShouldAllBe(summary => summary.Role == PluginRole.Output);

        var winners = adapter.ListFormLists(
            sources,
            output,
            RecordScope.WinningOverrides,
            TestContext.Current.CancellationToken);
        winners.Succeeded.ShouldBeTrue(winners.Error?.Message);
        var outputWinner = winners.Value!.Single(summary =>
            summary.FormKey == fixture.Sources.SourceListFormKey);
        outputWinner.ContainingModKey.ShouldBe(fixture.ExistingOutputModKey);
        outputWinner.Role.ShouldBe(PluginRole.Output);

        var read = adapter.ReadFormListContext(
            sources,
            output,
            new ReferenceRequest(fixture.Sources.SourceListFormKey, RecordScope.WinningOverrides),
            TestContext.Current.CancellationToken);
        read.Succeeded.ShouldBeTrue(read.Error?.Message);
        read.Value!.Context.ContainingModKey.ShouldBe(fixture.ExistingOutputModKey);
        read.Value.Context.Role.ShouldBe(PluginRole.Output);
        read.Value.Record.ShouldBeOfType<FormList>().EditorID.ShouldBe("ExistingStagedOverride");

        var resolution = adapter.ResolveReference(
            sources,
            output,
            new ReferenceRequest(
                fixture.OutputKeywordFormKey,
                RecordScope.StagedOutput,
                fixture.ExistingOutputModKey),
            TestContext.Current.CancellationToken);
        resolution.Succeeded.ShouldBeTrue(resolution.Error?.Message);
        resolution.Value!.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
        resolution.Value.Record.ShouldBeOfType<Keyword>().EditorID.ShouldBe("OutputKeyword");

        var liveRevision = sources.Revision.Next();
        var search = adapter.SearchReferences(
            sources,
            output,
            new ReferenceSearchRequest(
                "ExistingOwnList",
                maximumResults: 10,
                scope: RecordScope.StagedOutput,
                containingModKey: fixture.ExistingOutputModKey),
            sources.WorkspaceId,
            liveRevision,
            TestContext.Current.CancellationToken);
        search.Succeeded.ShouldBeTrue(search.Error?.Message);
        search.ResultRevision.ShouldBe(liveRevision);
        search.Value!.Matches.ShouldHaveSingleItem().FormKey.ShouldBe(fixture.OutputOwnListFormKey);
    }

    /// <summary>
    /// Verifies an unchanged embedded output produces no artifacts, while a material edit stages an exact complete plugin without touching sources or destination.
    /// </summary>
    /// <returns>A task that completes after every staged and opened native lifetime is released.</returns>
    [Fact]
    public async Task WriteAndValidateAsync_ExistingEmbeddedOutputPreservesCompleteNativeState()
    {
        using var fixture = SkyrimNativeOutputTestFixture.Create(LocalizedOutputMode.Embedded);
        var sourceBefore = fixture.Sources.SnapshotArtifacts();
        var destinationBefore = fixture.SnapshotOutputArtifacts();
        var adapter = CreateAdapter();
        await using var sources = await OpenSourcesAsync(adapter, fixture.Sources);
        await using var output = await OpenOutputAsync(
            adapter,
            sources,
            fixture.CreateExistingAssociation(),
            OutputSelectionMode.OpenExisting);

        var noOpDirectory = fixture.Sources.RootDirectory.CreateSubdirectory("Stage-Existing-NoOp");
        var noOp = await adapter.WriteAndValidateAsync(
            sources,
            output,
            new NativeWriteRequest(noOpDirectory.FullName, output.Association, output.Baseline),
            TestContext.Current.CancellationToken);
        noOp.Succeeded.ShouldBeTrue(noOp.Error?.Message);
        noOp.Value!.Disposition.ShouldBe(NativeWriteDisposition.Unchanged);
        noOp.Value.StagedOutput.ShouldBeNull();
        noOp.Value.ArtifactMappings.ShouldBeEmpty();
        noOpDirectory.EnumerateFileSystemInfos().ShouldBeEmpty();

        var editIdentity = BeginExisting(adapter, sources, output, fixture.OutputOwnListFormKey);
        Apply(adapter, sources, output, editIdentity.FormKey, new ClearEditorIdEdit()).Changed.ShouldBeTrue();
        Apply(adapter, sources, output, editIdentity.FormKey, new SetVersionControlEdit(0xA1B2C3D4)).Changed.ShouldBeTrue();
        Apply(adapter, sources, output, editIdentity.FormKey, new SetFormVersionEdit(55)).Changed.ShouldBeTrue();
        Apply(adapter, sources, output, editIdentity.FormKey, new SetVersion2Edit(19)).Changed.ShouldBeTrue();
        Apply(
            adapter,
            sources,
            output,
            editIdentity.FormKey,
            new SkyrimSetMajorRecordFlagsEdit(
                SkyrimMajorRecord.SkyrimMajorRecordFlag.NotPlayable
                | SkyrimMajorRecord.SkyrimMajorRecordFlag.InitiallyDisabled)).Changed.ShouldBeTrue();
        var expectedItems = new[]
        {
            fixture.Sources.BookFormKey,
            FormKey.Null,
            fixture.OutputKeywordFormKey,
            fixture.OutputKeywordFormKey,
            fixture.Sources.MissingFormKey,
        };
        Apply(
            adapter,
            sources,
            output,
            editIdentity.FormKey,
            new ReplaceItemsEdit(expectedItems)).Changed.ShouldBeTrue();

        var stagingDirectory = fixture.Sources.RootDirectory.CreateSubdirectory("Stage-Existing-Changed");
        var stagedResult = await adapter.WriteAndValidateAsync(
            sources,
            output,
            new NativeWriteRequest(stagingDirectory.FullName, output.Association, output.Baseline),
            TestContext.Current.CancellationToken);
        stagedResult.Succeeded.ShouldBeTrue(stagedResult.Error?.Message);
        var staged = stagedResult.Value.ShouldNotBeNull();
        try
        {
            staged.Disposition.ShouldBe(NativeWriteDisposition.StagedChanges);
            staged.StagedOutput.ShouldNotBeNull();
            var pluginMapping = GetPluginMapping(staged);
            pluginMapping.StagedArtifact.Fingerprint.Exists.ShouldBeTrue();
            pluginMapping.DestinationPath.ShouldBe(Path.GetFullPath(fixture.ExistingOutputPath));
            staged.ArtifactMappings
                .Where(mapping => mapping.StagedArtifact.Role != NativeArtifactRole.Plugin)
                .ShouldAllBe(mapping => !mapping.StagedArtifact.Fingerprint.Exists);

            await using var reopened = await OpenOutputAsync(
                adapter,
                sources,
                new OutputAssociation(
                    pluginMapping.StagedArtifact.Path,
                    fixture.ExistingOutputModKey,
                    LocalizedOutputMode.Embedded,
                    OutputMasterStyle.Full),
                OutputSelectionMode.OpenExisting);
            var snapshot = reopened.CreateSnapshot(TestContext.Current.CancellationToken);
            var formList = snapshot.FormLists.Single(record => record.FormKey == fixture.OutputOwnListFormKey);
            formList.EditorID.ShouldBeNull();
            formList.VersionControl.ShouldBe(0xA1B2C3D4U);
            formList.FormVersion.ShouldBe((ushort)55);
            formList.Version2.ShouldBe((ushort)19);
            formList.SkyrimMajorRecordFlags.HasFlag(
                SkyrimMajorRecord.SkyrimMajorRecordFlag.NotPlayable).ShouldBeTrue();
            formList.SkyrimMajorRecordFlags.HasFlag(
                SkyrimMajorRecord.SkyrimMajorRecordFlag.InitiallyDisabled).ShouldBeTrue();
            (formList.MajorRecordFlagsRaw & unchecked((int)0x40000000U)).ShouldNotBe(0);
            formList.Items.Select(item => item.FormKey).ShouldBe(expectedItems, ignoreOrder: false);
            snapshot.Keywords.ShouldContain(record => record.FormKey == fixture.OutputKeywordFormKey);
            snapshot.Books.ShouldContain(record => record.FormKey == fixture.OutputBookFormKey);
            snapshot.FormLists.Single(record => record.FormKey == fixture.OutputDeletedListFormKey)
                .IsDeleted.ShouldBeTrue();
        }
        finally
        {
            if (staged.StagedOutput is not null)
            {
                await staged.StagedOutput.DisposeAsync();
            }
        }

        AssertArtifactsEqual(sourceBefore, fixture.Sources.SnapshotArtifacts());
        AssertArtifactsEqual(destinationBefore, fixture.SnapshotOutputArtifacts());
    }

    /// <summary>
    /// Verifies a retained candidate rejects its stale original file identity but accepts a freshly adopted identical destination baseline without replacing candidate semantics.
    /// </summary>
    /// <returns>A task that completes after all candidate, fresh-observation, and staged native lifetimes are released.</returns>
    [Fact]
    public async Task WriteAndValidateAsync_RestoredIdenticalDestinationRequiresFreshSuppliedBaseline()
    {
        using var fixture = SkyrimNativeOutputTestFixture.Create(LocalizedOutputMode.Embedded);
        var sourceBefore = fixture.Sources.SnapshotArtifacts();
        var destinationBefore = fixture.SnapshotOutputArtifacts();
        var adapter = CreateAdapter();
        await using var sources = await OpenSourcesAsync(adapter, fixture.Sources);
        await using var candidate = await OpenOutputAsync(
            adapter,
            sources,
            fixture.CreateExistingAssociation(),
            OutputSelectionMode.OpenExisting);

        var originalPlugin = candidate.Baseline.Artifacts.Single(
            artifact => artifact.Role == NativeArtifactRole.Plugin);
        var originalIdentity = originalPlugin.FileIdentity.ShouldNotBeNull();
        var editIdentity = BeginExisting(adapter, sources, candidate, fixture.OutputOwnListFormKey);
        Apply(
            adapter,
            sources,
            candidate,
            editIdentity.FormKey,
            new SetEditorIdEdit("RestoredIdentityEdit")).Changed.ShouldBeTrue();

        var replacementPath = Path.Combine(
            fixture.OutputDirectory.FullName,
            $"replacement-{Guid.NewGuid():N}.tmp");
        File.WriteAllBytes(replacementPath, File.ReadAllBytes(fixture.ExistingOutputPath));
        File.Move(replacementPath, fixture.ExistingOutputPath, overwrite: true);

        OutputArtifactSetBaseline freshBaseline;
        await using (var freshObservation = await OpenOutputAsync(
            adapter,
            sources,
            fixture.CreateExistingAssociation(),
            OutputSelectionMode.OpenExisting))
        {
            freshBaseline = freshObservation.Baseline;
            var freshPlugin = freshBaseline.Artifacts.Single(
                artifact => artifact.Role == NativeArtifactRole.Plugin);
            freshPlugin.Fingerprint.ShouldBe(originalPlugin.Fingerprint);
            freshPlugin.FileIdentity.ShouldNotBeNull().ShouldNotBe(originalIdentity);
        }

        candidate.Baseline.Artifacts.Single(artifact => artifact.Role == NativeArtifactRole.Plugin)
            .FileIdentity.ShouldBe(originalIdentity);

        var staleDirectory = fixture.Sources.RootDirectory.CreateSubdirectory("Stage-Restored-Stale");
        var stale = await adapter.WriteAndValidateAsync(
            sources,
            candidate,
            new NativeWriteRequest(staleDirectory.FullName, candidate.Association, candidate.Baseline),
            TestContext.Current.CancellationToken);
        stale.Succeeded.ShouldBeFalse();
        stale.Error!.Code.ShouldBe(EngineErrorCode.ExternalChangeDetected);
        staleDirectory.EnumerateFileSystemInfos().ShouldBeEmpty();

        var freshDirectory = fixture.Sources.RootDirectory.CreateSubdirectory("Stage-Restored-Fresh");
        var stagedResult = await adapter.WriteAndValidateAsync(
            sources,
            candidate,
            new NativeWriteRequest(freshDirectory.FullName, candidate.Association, freshBaseline),
            TestContext.Current.CancellationToken);
        stagedResult.Succeeded.ShouldBeTrue(stagedResult.Error?.Message);
        var staged = stagedResult.Value.ShouldNotBeNull();
        try
        {
            staged.Disposition.ShouldBe(NativeWriteDisposition.StagedChanges);
            GetPluginMapping(staged).StagedArtifact.Fingerprint.Exists.ShouldBeTrue();
        }
        finally
        {
            if (staged.StagedOutput is not null)
            {
                await staged.StagedOutput.DisposeAsync();
            }
        }

        candidate.Baseline.Artifacts.Single(artifact => artifact.Role == NativeArtifactRole.Plugin)
            .FileIdentity.ShouldBe(originalIdentity);
        AssertArtifactsEqual(sourceBefore, fixture.Sources.SnapshotArtifacts());
        AssertArtifactsEqual(destinationBefore, fixture.SnapshotOutputArtifacts());
    }

    /// <summary>
    /// Verifies a new localized small-master output writes only the plugin, reopens exactly, and retains ordered cross-master references and FormList values.
    /// </summary>
    /// <returns>A task that completes after every staged and opened native lifetime is released.</returns>
    [Fact]
    public async Task WriteAndValidateAsync_NewLocalizedOutputReopensExactFormListAndMasters()
    {
        using var fixture = SkyrimNativeOutputTestFixture.Create();
        var sourceBefore = fixture.Sources.SnapshotArtifacts();
        var destinationBefore = fixture.SnapshotOutputArtifacts();
        var adapter = CreateAdapter();
        await using var sources = await OpenSourcesAsync(adapter, fixture.Sources);
        var association = fixture.CreateNewAssociation(
            "NewLocalizedSave.esm",
            LocalizedOutputMode.SeparateStringFiles,
            OutputMasterStyle.Small);
        await using var output = await OpenOutputAsync(
            adapter,
            sources,
            association,
            OutputSelectionMode.CreateNew);

        var editIdentity = BeginNew(adapter, sources, output);
        Apply(adapter, sources, output, editIdentity.FormKey, new SetVersionControlEdit(0x0102FEFF)).Changed.ShouldBeTrue();
        Apply(adapter, sources, output, editIdentity.FormKey, new SetFormVersionEdit(55)).Changed.ShouldBeTrue();
        Apply(adapter, sources, output, editIdentity.FormKey, new SetVersion2Edit(23)).Changed.ShouldBeTrue();
        Apply(
            adapter,
            sources,
            output,
            editIdentity.FormKey,
            new SkyrimSetMajorRecordFlagsEdit(
                SkyrimMajorRecord.SkyrimMajorRecordFlag.CantWait
                | SkyrimMajorRecord.SkyrimMajorRecordFlag.NotPlayable)).Changed.ShouldBeTrue();
        var expectedItems = new[]
        {
            fixture.Sources.BookFormKey,
            fixture.Sources.FullListFormKey,
            FormKey.Null,
            fixture.Sources.BookFormKey,
        };
        Apply(
            adapter,
            sources,
            output,
            editIdentity.FormKey,
            new ReplaceItemsEdit(expectedItems)).Changed.ShouldBeTrue();

        var stagingDirectory = fixture.Sources.RootDirectory.CreateSubdirectory("Stage-New-Localized");
        var stagedResult = await adapter.WriteAndValidateAsync(
            sources,
            output,
            new NativeWriteRequest(stagingDirectory.FullName, output.Association, output.Baseline),
            TestContext.Current.CancellationToken);
        stagedResult.Succeeded.ShouldBeTrue(stagedResult.Error?.Message);
        var staged = stagedResult.Value.ShouldNotBeNull();
        try
        {
            staged.Disposition.ShouldBe(NativeWriteDisposition.StagedChanges);
            var pluginMapping = GetPluginMapping(staged);
            var emptyStrings = staged.ArtifactMappings
                .Where(mapping => mapping.StagedArtifact.Role != NativeArtifactRole.Plugin
                    && mapping.StagedArtifact.Fingerprint.Exists)
                .ShouldHaveSingleItem();
            emptyStrings.StagedArtifact.Role.ShouldBe(NativeArtifactRole.Strings);
            emptyStrings.StagedArtifact.Language.ShouldBe("English");
            emptyStrings.StagedArtifact.Fingerprint.Length.ShouldBe(8L);
            File.ReadAllBytes(emptyStrings.StagedArtifact.Path).ShouldBe(new byte[8]);
            stagingDirectory.EnumerateFiles("*", SearchOption.AllDirectories)
                .Count().ShouldBe(2);
            File.Exists(association.PluginPath).ShouldBeFalse();

            await using var reopened = await OpenOutputAsync(
                adapter,
                sources,
                new OutputAssociation(
                    pluginMapping.StagedArtifact.Path,
                    association.ModKey,
                    LocalizedOutputMode.SeparateStringFiles,
                    OutputMasterStyle.Small),
                OutputSelectionMode.OpenExisting);
            var snapshot = reopened.CreateSnapshot(TestContext.Current.CancellationToken);
            snapshot.IsMaster.ShouldBeTrue();
            snapshot.IsSmallMaster.ShouldBeTrue();
            snapshot.UsingLocalization.ShouldBeTrue();
            snapshot.ModHeader.MasterReferences.Select(master => master.Master).ShouldBe(
                [fixture.Sources.SourceModKey, fixture.Sources.FullModKey],
                ignoreOrder: false);
            var formList = snapshot.FormLists.ShouldHaveSingleItem();
            formList.EditorID.ShouldBeNull();
            formList.VersionControl.ShouldBe(0x0102FEFFU);
            formList.FormVersion.ShouldBe((ushort)55);
            formList.Version2.ShouldBe((ushort)23);
            formList.SkyrimMajorRecordFlags.ShouldBe(
                SkyrimMajorRecord.SkyrimMajorRecordFlag.CantWait
                | SkyrimMajorRecord.SkyrimMajorRecordFlag.NotPlayable);
            formList.Items.Select(item => item.FormKey).ShouldBe(expectedItems, ignoreOrder: false);
        }
        finally
        {
            if (staged.StagedOutput is not null)
            {
                await staged.StagedOutput.DisposeAsync();
            }
        }

        AssertArtifactsEqual(sourceBefore, fixture.Sources.SnapshotArtifacts());
        AssertArtifactsEqual(destinationBefore, fixture.SnapshotOutputArtifacts());
    }

    /// <summary>Verifies existing localized no-op saves write nothing and material changes fail before private staging.</summary>
    /// <returns>A task that completes after all native source and output lifetimes are released.</returns>
    [Fact]
    public async Task WriteAndValidateAsync_ExistingLocalizedMaterialChangeIsUnsupportedBeforeStaging()
    {
        using var fixture = SkyrimNativeOutputTestFixture.Create();
        var sourceBefore = fixture.Sources.SnapshotArtifacts();
        var destinationBefore = fixture.SnapshotOutputArtifacts();
        var adapter = CreateAdapter();
        await using var sources = await OpenSourcesAsync(adapter, fixture.Sources);
        await using var output = await OpenOutputAsync(
            adapter,
            sources,
            fixture.CreateExistingAssociation(),
            OutputSelectionMode.OpenExisting);

        var noOpDirectory = fixture.Sources.RootDirectory.CreateSubdirectory("Stage-Localized-NoOp");
        var noOp = await adapter.WriteAndValidateAsync(
            sources,
            output,
            new NativeWriteRequest(noOpDirectory.FullName, output.Association, output.Baseline),
            TestContext.Current.CancellationToken);
        noOp.Succeeded.ShouldBeTrue(noOp.Error?.Message);
        noOp.Value!.Disposition.ShouldBe(NativeWriteDisposition.Unchanged);
        noOp.Value.ArtifactMappings.ShouldBeEmpty();
        noOpDirectory.EnumerateFileSystemInfos().ShouldBeEmpty();

        var editIdentity = BeginExisting(adapter, sources, output, fixture.OutputOwnListFormKey);
        Apply(adapter, sources, output, editIdentity.FormKey, new SetEditorIdEdit("LocalizedChanged"))
            .Changed.ShouldBeTrue();
        var rejectedDirectory = fixture.Sources.RootDirectory.CreateSubdirectory("Stage-Localized-Rejected");
        var rejected = await adapter.WriteAndValidateAsync(
            sources,
            output,
            new NativeWriteRequest(rejectedDirectory.FullName, output.Association, output.Baseline),
            TestContext.Current.CancellationToken);

        rejected.Succeeded.ShouldBeFalse();
        rejected.Error!.Code.ShouldBe(EngineErrorCode.UnsupportedInput);
        rejectedDirectory.EnumerateFileSystemInfos().ShouldBeEmpty();
        AssertArtifactsEqual(sourceBefore, fixture.Sources.SnapshotArtifacts());
        AssertArtifactsEqual(destinationBefore, fixture.SnapshotOutputArtifacts());
    }

    /// <summary>Creates the composed Skyrim adapter used by focused native integration tests.</summary>
    /// <returns>A complete adapter over the real source, output, edit, and inspection services.</returns>
    private static SkyrimFormListGameAdapter CreateAdapter()
    {
        var outputService = new SkyrimNativeOutputService(new NativeOutputInputLoader());
        return new SkyrimFormListGameAdapter(
            new SkyrimNativeSourceLoader(new NativeSourceInputLoader()),
            outputService,
            new SkyrimNativeEditService(new SkyrimFormListNativeInspector()));
    }

    /// <summary>Opens the complete generated Skyrim source set through the composed adapter.</summary>
    /// <param name="adapter">The composed Skyrim adapter.</param>
    /// <param name="fixture">The generated source fixture.</param>
    /// <returns>The independently owned native source set.</returns>
    private static async Task<SkyrimNativeSourceSet> OpenSourcesAsync(
        SkyrimFormListGameAdapter adapter,
        SkyrimNativeTestFixture fixture)
    {
        var result = await adapter.OpenSourcesAsync(
            fixture.CreateOpenRequest(),
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value!.Sources.ShouldBeOfType<SkyrimNativeSourceSet>();
    }

    /// <summary>Opens one new or existing output through the composed adapter.</summary>
    /// <param name="adapter">The composed Skyrim adapter.</param>
    /// <param name="sources">The borrowed source set.</param>
    /// <param name="association">The exact output identity and representation.</param>
    /// <param name="mode">Whether the output must be absent or present.</param>
    /// <returns>The independently owned complete native output state.</returns>
    private static async Task<SkyrimNativeOutputState> OpenOutputAsync(
        SkyrimFormListGameAdapter adapter,
        SkyrimNativeSourceSet sources,
        OutputAssociation association,
        OutputSelectionMode mode)
    {
        var result = await adapter.OpenOutputAsync(
            sources,
            new SelectOutputRequest(Guid.NewGuid(), sources.Revision, mode, association),
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value!.Output.ShouldBeOfType<SkyrimNativeOutputState>();
    }

    /// <summary>Begins editing one exact FormList already present in the output.</summary>
    /// <param name="adapter">The composed Skyrim adapter.</param>
    /// <param name="sources">The borrowed source set.</param>
    /// <param name="output">The mutable test-owned output state.</param>
    /// <param name="target">The exact output-owned FormList identity.</param>
    /// <returns>The native edit identity registered for the target.</returns>
    private static NativeEditIdentity BeginExisting(
        SkyrimFormListGameAdapter adapter,
        SkyrimNativeSourceSet sources,
        SkyrimNativeOutputState output,
        FormKey target)
    {
        var result = adapter.BeginEdit(
            sources,
            output,
            new BeginEditRequest(
                Guid.NewGuid(),
                sources.Revision,
                FormListEditRole.ExistingOutput,
                targetFormKey: target),
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value.ShouldNotBeNull();
    }

    /// <summary>Begins one new output-owned FormList edit.</summary>
    /// <param name="adapter">The composed Skyrim adapter.</param>
    /// <param name="sources">The borrowed source set.</param>
    /// <param name="output">The mutable test-owned output state.</param>
    /// <returns>The newly allocated native edit identity.</returns>
    private static NativeEditIdentity BeginNew(
        SkyrimFormListGameAdapter adapter,
        SkyrimNativeSourceSet sources,
        SkyrimNativeOutputState output)
    {
        var result = adapter.BeginEdit(
            sources,
            output,
            new BeginEditRequest(Guid.NewGuid(), sources.Revision, FormListEditRole.New),
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value.ShouldNotBeNull();
    }

    /// <summary>Prepares and applies one typed edit to the test-owned output.</summary>
    /// <param name="adapter">The composed Skyrim adapter.</param>
    /// <param name="sources">The borrowed source set.</param>
    /// <param name="output">The mutable test-owned output state.</param>
    /// <param name="target">The exact FormList target.</param>
    /// <param name="edit">The caller-owned typed edit.</param>
    /// <returns>The successful native mutation result.</returns>
    private static NativeEditMutationResult Apply(
        SkyrimFormListGameAdapter adapter,
        SkyrimNativeSourceSet sources,
        SkyrimNativeOutputState output,
        FormKey target,
        FormListEdit edit)
    {
        var result = adapter.ApplyEdit(sources, output, target, adapter.PrepareEdit(edit));
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value.ShouldNotBeNull();
    }

    /// <summary>Finds the single present staged plugin mapping.</summary>
    /// <param name="staged">The complete validated staged output set.</param>
    /// <returns>The staged plugin-to-destination mapping.</returns>
    private static NativeStagedArtifactMapping GetPluginMapping(NativeStagedOutputSet staged)
    {
        return staged.ArtifactMappings.Single(mapping =>
            mapping.StagedArtifact.Role == NativeArtifactRole.Plugin);
    }

    /// <summary>Asserts two complete physical artifact snapshots have identical ordered paths and bytes.</summary>
    /// <param name="expected">The snapshot captured before the operation.</param>
    /// <param name="actual">The snapshot captured after the operation.</param>
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
