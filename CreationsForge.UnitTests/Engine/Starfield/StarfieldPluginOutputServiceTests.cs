using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.PluginInputs;
using CreationsForge.Core.Engine.PluginOutputs;
using CreationsForge.Starfield.PluginAdapter;
using CreationsForge.Starfield.PluginAdapter.RecordInspection;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Strings;
using Shouldly;
using System.IO.Abstractions;

namespace CreationsForge.UnitTests.Engine.Starfield;

/// <summary>
/// Verifies complete Starfield output admission, cloning, and FormList target creation without destination writes.
/// </summary>
public sealed class StarfieldPluginOutputServiceTests
{
    /// <summary>Verifies every supported Starfield master style for both absent and existing output selection.</summary>
    /// <param name="style">The plugin master style to create and reopen.</param>
    /// <returns>A task that completes after both independently owned output states are released.</returns>
    [Theory]
    [InlineData(OutputMasterStyle.Full)]
    [InlineData(OutputMasterStyle.Small)]
    [InlineData(OutputMasterStyle.Medium)]
    public async Task OpenAsync_SupportsAllMasterStylesForNewAndExistingOutputs(OutputMasterStyle style)
    {
        using var fixture = StarfieldPluginTestFixture.Create();
        await using var sources = await OpenSourcesAsync(fixture);
        var service = new StarfieldPluginOutputService(new PluginOutputInputLoader());
        service.Inspector.ShouldBeOfType<StarfieldFormListInspector>();
        var outputDirectory = fixture.RootDirectory.CreateSubdirectory($"Style-{style}");

        var newPath = Path.Combine(outputDirectory.FullName, $"New{style}.esm");
        var newAssociation = CreateAssociation(newPath, style, LocalizedOutputMode.Embedded);
        var newResult = await service.OpenAsync(
            sources,
            new SelectOutputRequest(Guid.NewGuid(), sources.Revision, OutputSelectionMode.CreateNew, newAssociation),
            TestContext.Current.CancellationToken);
        newResult.Succeeded.ShouldBeTrue(newResult.Error?.Message);
        await using (var newState = newResult.Value!.Output.ShouldBeOfType<StarfieldPluginOutputState>())
        {
            var newSnapshot = newState.CreateSnapshot(TestContext.Current.CancellationToken);
            AssertStyle(newSnapshot, style);
            newSnapshot.ModKey.ShouldBe(newAssociation.ModKey);
            newSnapshot.EnumerateMajorRecords().ShouldBeEmpty();
            newSnapshot.UsingLocalization.ShouldBeFalse();
            File.Exists(newPath).ShouldBeFalse();
            Directory.Exists(Path.Combine(outputDirectory.FullName, "Strings")).ShouldBeFalse();
        }

        var existingPath = WriteOutput(
            outputDirectory,
            $"Existing{style}.esm",
            style,
            localized: false,
            mod => mod.FormLists.Add(new FormList(mod, $"Existing{style}List")));
        var existingAssociation = CreateAssociation(existingPath, style, LocalizedOutputMode.Embedded);
        var existingResult = await service.OpenAsync(
            sources,
            new SelectOutputRequest(Guid.NewGuid(), sources.Revision, OutputSelectionMode.OpenExisting, existingAssociation),
            TestContext.Current.CancellationToken);
        existingResult.Succeeded.ShouldBeTrue(existingResult.Error?.Message);
        await using var existingState = existingResult.Value!.Output.ShouldBeOfType<StarfieldPluginOutputState>();
        var existingSnapshot = existingState.CreateSnapshot(TestContext.Current.CancellationToken);
        AssertStyle(existingSnapshot, style);
        existingSnapshot.FormLists.ShouldHaveSingleItem().EditorID.ShouldBe($"Existing{style}List");
    }

    /// <summary>Verifies complete unrelated output state survives admission, snapshots, cloning, and existing-output edit selection.</summary>
    /// <returns>A task that completes after the output and its clone are released.</returns>
    [Fact]
    public async Task ExistingOutput_PreservesHeadersTranslationsOrderConditionsAndDefensiveSnapshots()
    {
        using var fixture = StarfieldPluginTestFixture.Create();
        var sourceBytes = fixture.SnapshotArtifacts();
        await using var sources = await OpenSourcesAsync(fixture);
        var postSelectionRevision = CreatePostSelectionRevision(sources.Revision);
        var service = new StarfieldPluginOutputService(new PluginOutputInputLoader());
        var outputDirectory = fixture.RootDirectory.CreateSubdirectory("CompleteExisting");
        FormKey outputListFormKey = default;
        FormKey keywordFormKey = default;
        var outputPath = WriteOutput(
            outputDirectory,
            "CompleteExisting.esp",
            OutputMasterStyle.Full,
            localized: true,
            mod =>
            {
                mod.ModHeader.Author = "CreationsForge preservation test";
                var firstKeyword = new Keyword(mod, "FirstKeyword");
                var secondKeyword = new Keyword(mod, "SecondKeyword");
                mod.Keywords.Add(firstKeyword);
                mod.Keywords.Add(secondKeyword);
                keywordFormKey = secondKeyword.FormKey;
                var list = new FormList(mod, "PreservedOutputList")
                {
                    Name = CreateName("Preserved list", "Liste conservée")
                };
                list.Items.Add(new FormLink<IStarfieldMajorRecordGetter>(firstKeyword.FormKey));
                list.Items.Add(new FormLink<IStarfieldMajorRecordGetter>(secondKeyword.FormKey));
                list.ConditionalEntries.Add(new FormListConditionalEntry
                {
                    Index = 9,
                    Conditions = null
                });
                mod.FormLists.Add(list);
                outputListFormKey = list.FormKey;
            });
        var association = CreateAssociation(
            outputPath,
            OutputMasterStyle.Full,
            LocalizedOutputMode.SeparateStringFiles);

        var open = await service.OpenAsync(
            sources,
            new SelectOutputRequest(Guid.NewGuid(), sources.Revision, OutputSelectionMode.OpenExisting, association),
            TestContext.Current.CancellationToken);

        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        await using var state = open.Value!.Output.ShouldBeOfType<StarfieldPluginOutputState>();
        AssertCompleteExistingSnapshot(state.CreateSnapshot(), outputListFormKey, keywordFormKey);

        var callerSnapshot = state.CreateSnapshot();
        callerSnapshot.ShouldBeOfType<StarfieldMod>().FormLists.Single().EditorID = "Caller mutation";
        state.CreateSnapshot().FormLists.Single().EditorID.ShouldBe("PreservedOutputList");

        await using var candidate = service.Clone(state, TestContext.Current.CancellationToken);
        var begin = service.BeginEdit(
            sources,
            candidate,
            new BeginEditRequest(
                Guid.NewGuid(),
                postSelectionRevision,
                FormListEditRole.ExistingOutput,
                targetFormKey: outputListFormKey),
            TestContext.Current.CancellationToken);
        begin.Succeeded.ShouldBeTrue(begin.Error?.Message);
        begin.Value!.FormKey.ShouldBe(outputListFormKey);
        begin.Value.OriginFormKey.ShouldBeNull();
        begin.Value.Role.ShouldBe(FormListEditRole.ExistingOutput);
        AssertCompleteExistingSnapshot(candidate.CreateSnapshot(), outputListFormKey, keywordFormKey);
        AssertArtifactsUnchanged(sourceBytes, fixture.SnapshotArtifacts());
    }

    /// <summary>Verifies all-family allocation and exact override selection preserve complete candidate state and reject unusable origins.</summary>
    /// <returns>A task that completes after all output candidates are released.</returns>
    [Fact]
    public async Task BeginEdit_AllocatesUniqueIdsAndUsesExactLiveOverrideContexts()
    {
        using var fixture = StarfieldPluginTestFixture.Create();
        var sourceBytes = fixture.SnapshotArtifacts();
        await using var sources = await OpenSourcesAsync(fixture);
        var postSelectionRevision = CreatePostSelectionRevision(sources.Revision);
        var service = new StarfieldPluginOutputService(new PluginOutputInputLoader());
        var outputDirectory = fixture.RootDirectory.CreateSubdirectory("BeginEdit");

        FormKey collisionKey = default;
        var collisionPath = WriteOutput(
            outputDirectory,
            "Collision.esp",
            OutputMasterStyle.Full,
            localized: false,
            mod =>
            {
                collisionKey = new FormKey(mod.ModKey, 0x00000800);
                mod.Keywords.Add(new Keyword(collisionKey, StarfieldRelease.Starfield)
                {
                    EditorID = "OccupiedAllocatorId"
                });
                ((IModHeaderCommon)mod.ModHeader).NextFormID = collisionKey.ID;
            },
            preserveNextFormId: true);
        var collisionOpen = await service.OpenAsync(
            sources,
            new SelectOutputRequest(
                Guid.NewGuid(),
                sources.Revision,
                OutputSelectionMode.OpenExisting,
                CreateAssociation(collisionPath, OutputMasterStyle.Full, LocalizedOutputMode.Embedded)),
            TestContext.Current.CancellationToken);
        collisionOpen.Succeeded.ShouldBeTrue(collisionOpen.Error?.Message);
        await using var collisionState = collisionOpen.Value!.Output.ShouldBeOfType<StarfieldPluginOutputState>();
        await using var allocationCandidate = service.Clone(collisionState);
        var allocation = service.BeginEdit(
            sources,
            allocationCandidate,
            new BeginEditRequest(Guid.NewGuid(), postSelectionRevision, FormListEditRole.New),
            TestContext.Current.CancellationToken);
        allocation.Succeeded.ShouldBeFalse();
        allocation.Error!.Code.ShouldBe(EngineErrorCode.ValidationFailed);
        allocation.Error.Message.ShouldContain("not unique");

        var overridePath = Path.Combine(outputDirectory.FullName, "Overrides.esp");
        var overrideOpen = await service.OpenAsync(
            sources,
            new SelectOutputRequest(
                Guid.NewGuid(),
                sources.Revision,
                OutputSelectionMode.CreateNew,
                CreateAssociation(overridePath, OutputMasterStyle.Full, LocalizedOutputMode.Embedded)),
            TestContext.Current.CancellationToken);
        overrideOpen.Succeeded.ShouldBeTrue(overrideOpen.Error?.Message);
        await using var overrideState = overrideOpen.Value!.Output.ShouldBeOfType<StarfieldPluginOutputState>();
        await using var cleanAllocationCandidate = service.Clone(overrideState);
        var successfulAllocation = service.BeginEdit(
            sources,
            cleanAllocationCandidate,
            new BeginEditRequest(Guid.NewGuid(), postSelectionRevision, FormListEditRole.New),
            TestContext.Current.CancellationToken);
        successfulAllocation.Succeeded.ShouldBeTrue(successfulAllocation.Error?.Message);
        var cleanAllocationSnapshot = cleanAllocationCandidate.CreateSnapshot();
        cleanAllocationSnapshot.FormLists.ShouldHaveSingleItem().FormKey.ShouldBe(successfulAllocation.Value!.FormKey);
        cleanAllocationSnapshot.EnumerateMajorRecords().Select(record => record.FormKey).Distinct().Count()
            .ShouldBe(cleanAllocationSnapshot.EnumerateMajorRecords().Count());

        await using var exactSourceCandidate = service.Clone(overrideState);
        var exactSource = service.BeginEdit(
            sources,
            exactSourceCandidate,
            new BeginEditRequest(
                Guid.NewGuid(),
                postSelectionRevision,
                FormListEditRole.Override,
                fixture.SourceListFormKey,
                new ReferenceRequest(fixture.SourceListFormKey, RecordScope.AllContexts, fixture.SourceModKey)),
            TestContext.Current.CancellationToken);
        exactSource.Succeeded.ShouldBeTrue(exactSource.Error?.Message);
        exactSourceCandidate.CreateSnapshot().FormLists.Single().EditorID.ShouldBe("SharedListSmall");

        await using var overrideCandidate = service.Clone(overrideState);
        var winning = service.BeginEdit(
            sources,
            overrideCandidate,
            new BeginEditRequest(Guid.NewGuid(), postSelectionRevision, FormListEditRole.Override, fixture.SourceListFormKey),
            TestContext.Current.CancellationToken);
        winning.Succeeded.ShouldBeTrue(winning.Error?.Message);
        overrideCandidate.CreateSnapshot().FormLists.Single().EditorID.ShouldBe("SharedListOverride");

        var reusedSource = service.BeginEdit(
            sources,
            overrideCandidate,
            new BeginEditRequest(
                Guid.NewGuid(),
                postSelectionRevision,
                FormListEditRole.Override,
                fixture.SourceListFormKey,
                new ReferenceRequest(fixture.SourceListFormKey, RecordScope.AllContexts, fixture.SourceModKey)),
            TestContext.Current.CancellationToken);
        reusedSource.Succeeded.ShouldBeTrue(reusedSource.Error?.Message);
        reusedSource.Value!.FormKey.ShouldBe(winning.Value!.FormKey);
        overrideCandidate.CreateSnapshot().FormLists.Single().EditorID.ShouldBe("SharedListOverride");

        var deleted = service.BeginEdit(
            sources,
            overrideCandidate,
            new BeginEditRequest(Guid.NewGuid(), postSelectionRevision, FormListEditRole.Override, fixture.DeletedListFormKey),
            TestContext.Current.CancellationToken);
        var wrongFamily = service.BeginEdit(
            sources,
            overrideCandidate,
            new BeginEditRequest(Guid.NewGuid(), postSelectionRevision, FormListEditRole.Override, fixture.BookFormKey),
            TestContext.Current.CancellationToken);
        var missing = service.BeginEdit(
            sources,
            overrideCandidate,
            new BeginEditRequest(Guid.NewGuid(), postSelectionRevision, FormListEditRole.Override, fixture.MissingFormKey),
            TestContext.Current.CancellationToken);
        deleted.Error!.Code.ShouldBe(EngineErrorCode.ValidationFailed);
        wrongFamily.Error!.Code.ShouldBe(EngineErrorCode.ValidationFailed);
        missing.Error!.Code.ShouldBe(EngineErrorCode.RecordNotFound);
        overrideCandidate.CreateSnapshot().FormLists.ShouldHaveSingleItem();
        AssertArtifactsUnchanged(sourceBytes, fixture.SnapshotArtifacts());
    }

    /// <summary>Verifies exact reopen baselines, cancellation, deleted output targets, and disposed-state rejection.</summary>
    /// <returns>A task that completes after reopen and lifecycle checks.</returns>
    [Fact]
    public async Task ReopenAndLifecycle_RejectDriftCancellationAndDisposedState()
    {
        using var fixture = StarfieldPluginTestFixture.Create();
        await using var sources = await OpenSourcesAsync(fixture);
        var postSelectionRevision = CreatePostSelectionRevision(sources.Revision);
        var service = new StarfieldPluginOutputService(new PluginOutputInputLoader());
        var outputDirectory = fixture.RootDirectory.CreateSubdirectory("Reopen");
        FormKey deletedTarget = default;
        var outputPath = WriteOutput(
            outputDirectory,
            "Reopen.esp",
            OutputMasterStyle.Full,
            localized: false,
            mod =>
            {
                var deleted = new FormList(mod, "DeletedOutputList")
                {
                    IsDeleted = true
                };
                mod.FormLists.Add(deleted);
                deletedTarget = deleted.FormKey;
            });
        var association = CreateAssociation(outputPath, OutputMasterStyle.Full, LocalizedOutputMode.Embedded);
        var open = await service.OpenAsync(
            sources,
            new SelectOutputRequest(Guid.NewGuid(), sources.Revision, OutputSelectionMode.OpenExisting, association),
            TestContext.Current.CancellationToken);
        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        var state = open.Value!.Output.ShouldBeOfType<StarfieldPluginOutputState>();

        var reopened = await service.ReopenAsync(
            sources,
            open.Value.Association,
            open.Value.Baseline,
            TestContext.Current.CancellationToken);
        reopened.Succeeded.ShouldBeTrue(reopened.Error?.Message);
        await using var reopenedState = reopened.Value!.Output.ShouldBeOfType<StarfieldPluginOutputState>();
        await using var candidate = service.Clone(reopenedState);
        var beginDeleted = service.BeginEdit(
            sources,
            candidate,
            new BeginEditRequest(
                Guid.NewGuid(),
                postSelectionRevision,
                FormListEditRole.ExistingOutput,
                targetFormKey: deletedTarget),
            TestContext.Current.CancellationToken);
        beginDeleted.Succeeded.ShouldBeTrue(beginDeleted.Error?.Message);
        candidate.CreateSnapshot().FormLists.Single().IsDeleted.ShouldBeTrue();

        WriteOutput(
            outputDirectory,
            "Reopen.esp",
            OutputMasterStyle.Full,
            localized: false,
            mod => mod.FormLists.Add(new FormList(mod, "ExternallyChanged")));
        var changed = await service.ReopenAsync(
            sources,
            open.Value.Association,
            open.Value.Baseline,
            TestContext.Current.CancellationToken);
        changed.Succeeded.ShouldBeFalse();
        changed.Error!.Code.ShouldBe(EngineErrorCode.ExternalChangeDetected);

        using var canceledSource = new CancellationTokenSource();
        canceledSource.Cancel();
        await Should.ThrowAsync<OperationCanceledException>(() => service.OpenAsync(
            sources,
            new SelectOutputRequest(
                Guid.NewGuid(),
                sources.Revision,
                OutputSelectionMode.CreateNew,
                CreateAssociation(
                    Path.Combine(outputDirectory.FullName, "Canceled.esp"),
                    OutputMasterStyle.Full,
                    LocalizedOutputMode.Embedded)),
            canceledSource.Token));

        await state.DisposeAsync();
        await state.DisposeAsync();
        Should.Throw<ObjectDisposedException>(() => state.CreateSnapshot());
        Should.Throw<ObjectDisposedException>(() => service.Clone(state));
    }

    /// <summary>Opens the generated Starfield sources through the complete game-specific source loader.</summary>
    /// <param name="fixture">The generated source fixture.</param>
    /// <returns>The independently owned Starfield source set.</returns>
    private static async Task<StarfieldPluginSourceSet> OpenSourcesAsync(StarfieldPluginTestFixture fixture)
    {
        var open = await new StarfieldPluginSourceLoader(new PluginSourceInputLoader()).OpenAsync(
            fixture.CreateOpenRequest(),
            TestContext.Current.CancellationToken);
        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        return open.Value!.Sources.ShouldBeOfType<StarfieldPluginSourceSet>();
    }

    /// <summary>Creates one exact output association from a canonicalizable plugin path.</summary>
    /// <param name="path">The output plugin path.</param>
    /// <param name="style">The requested plugin master style.</param>
    /// <param name="localizedOutputMode">The requested plugin localization representation.</param>
    /// <returns>The immutable output association.</returns>
    private static OutputAssociation CreateAssociation(
        string path,
        OutputMasterStyle style,
        LocalizedOutputMode localizedOutputMode)
    {
        return new OutputAssociation(
            path,
            ModKey.FromNameAndExtension(Path.GetFileName(path)),
            localizedOutputMode,
            style);
    }

    /// <summary>Creates a distinct revision shaped like the workspace revision established after output selection.</summary>
    /// <param name="sourceRevision">The initial source-only revision.</param>
    /// <returns>A distinct baseline identity with the next monotonic sequence.</returns>
    private static WorkspaceRevision CreatePostSelectionRevision(WorkspaceRevision sourceRevision)
    {
        return new WorkspaceRevision(Guid.NewGuid(), checked(sourceRevision.Sequence + 1));
    }

    /// <summary>Writes a deterministic complete plugin output fixture with caller-selected content.</summary>
    /// <param name="outputDirectory">The existing output parent directory.</param>
    /// <param name="fileName">The output plugin file name.</param>
    /// <param name="style">The plugin master style.</param>
    /// <param name="localized">Whether the output uses loose localized strings.</param>
    /// <param name="configure">The content builder invoked before serialization.</param>
    /// <param name="preserveNextFormId">Whether serialization must retain the deliberately colliding allocator value.</param>
    /// <returns>The written output plugin path.</returns>
    private static string WriteOutput(
        DirectoryInfo outputDirectory,
        string fileName,
        OutputMasterStyle style,
        bool localized,
        Action<StarfieldMod> configure,
        bool preserveNextFormId = false)
    {
        var mod = new StarfieldMod(ModKey.FromNameAndExtension(fileName), StarfieldRelease.Starfield)
        {
            IsSmallMaster = style == OutputMasterStyle.Small,
            IsMediumMaster = style == OutputMasterStyle.Medium,
            UsingLocalization = localized
        };
        configure(mod);
        var outputPath = Path.Combine(outputDirectory.FullName, fileName);
        var writeParameters = preserveNextFormId
            ? BinaryWriteParameters.Default with { NextFormID = NextFormIDOption.NoCheck }
            : BinaryWriteParameters.Default;
        if (localized)
        {
            var stringsDirectory = Directory.CreateDirectory(Path.Combine(outputDirectory.FullName, "Strings"));
            using var stringsWriter = new StringsWriter(
                GameRelease.Starfield,
                mod.ModKey,
                stringsDirectory.FullName,
                new StarfieldTestEncodingProvider(),
                new FileSystem());
            ((IModGetter)mod).WriteToBinary(
                outputPath,
                writeParameters with { StringsWriter = stringsWriter });
        }
        else
        {
            ((IModGetter)mod).WriteToBinary(outputPath, writeParameters);
        }

        return outputPath;
    }

    /// <summary>Asserts exact plugin style flags without conflating full, small, and medium outputs.</summary>
    /// <param name="mod">The detached complete Starfield output.</param>
    /// <param name="style">The expected plugin style.</param>
    private static void AssertStyle(IStarfieldModGetter mod, OutputMasterStyle style)
    {
        mod.IsSmallMaster.ShouldBe(style == OutputMasterStyle.Small);
        mod.IsMediumMaster.ShouldBe(style == OutputMasterStyle.Medium);
    }

    /// <summary>Asserts representative header, unrelated family, translation, ordering, and conditional state.</summary>
    /// <param name="mod">The detached complete Starfield output.</param>
    /// <param name="outputListFormKey">The expected output-owned FormList identity.</param>
    /// <param name="keywordFormKey">The second ordered keyword identity.</param>
    private static void AssertCompleteExistingSnapshot(
        IStarfieldModGetter mod,
        FormKey outputListFormKey,
        FormKey keywordFormKey)
    {
        mod.ModHeader.Author.ShouldBe("CreationsForge preservation test");
        mod.Keywords.Count().ShouldBe(2);
        mod.Keywords.Last().FormKey.ShouldBe(keywordFormKey);
        var list = mod.FormLists.ShouldHaveSingleItem();
        list.FormKey.ShouldBe(outputListFormKey);
        list.EditorID.ShouldBe("PreservedOutputList");
        list.Items.Select(item => item.FormKey).ShouldBe(mod.Keywords.Select(keyword => keyword.FormKey));
        var name = list.Name.ShouldNotBeNull();
        name.TryLookup(Language.English, out var english).ShouldBeTrue();
        name.TryLookup(Language.French, out var french).ShouldBeTrue();
        english.ShouldBe("Preserved list");
        french.ShouldBe("Liste conservée");
        var conditionalEntry = list.ConditionalEntries.ShouldHaveSingleItem();
        conditionalEntry.Index.ShouldBe(9U);
        conditionalEntry.Conditions.ShouldBeNull();
    }

    /// <summary>Creates a deterministic two-language record translated string.</summary>
    /// <param name="english">The English text.</param>
    /// <param name="french">The French text.</param>
    /// <returns>A mutable translated string containing both languages.</returns>
    private static TranslatedString CreateName(string english, string french)
    {
        var name = new TranslatedString(Language.English, english);
        name.Set(Language.French, french);
        return name;
    }

    /// <summary>Asserts exact source plugin and strings bytes remain unchanged.</summary>
    /// <param name="expected">The original source artifact bytes.</param>
    /// <param name="actual">The source artifact bytes after output operations.</param>
    private static void AssertArtifactsUnchanged(
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
