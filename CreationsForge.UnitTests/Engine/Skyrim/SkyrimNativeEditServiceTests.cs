using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInputs;
using CreationsForge.Core.Engine.NativeOutputs;
using CreationsForge.Skyrim.Native;
using CreationsForge.Skyrim.Native.NativeInspection;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Skyrim;

/// <summary>Verifies immutable preparation, atomic application, provenance, and exact Skyrim preview behavior.</summary>
public sealed class SkyrimNativeEditServiceTests
{
    /// <summary>Every supported typed edit preserves Skyrim values, nulls, duplicates, and final-position move semantics.</summary>
    /// <returns>A task that completes after all generated native lifetimes are released.</returns>
    [Fact]
    public async Task ApplyEdit_AllSupportedFields_PreservesTypedValuesAndOrder()
    {
        using var fixture = SkyrimNativeOutputTestFixture.Create();
        await using var sources = await OpenSourcesAsync(fixture);
        var outputService = CreateOutputService();
        var editService = new SkyrimNativeEditService(outputService.Inspector);
        await using var state = await CreateNewStateAsync(fixture, sources, outputService, "TypedEdits.esm");
        var target = BeginNew(outputService, sources, state);

        Apply(editService, sources, state, target, new SetEditorIdEdit("EditedList")).Changed.ShouldBeTrue();
        Apply(editService, sources, state, target, new SetVersionControlEdit(0x10203040)).Changed.ShouldBeTrue();
        Apply(editService, sources, state, target, new SetFormVersionEdit(55)).Changed.ShouldBeTrue();
        Apply(editService, sources, state, target, new SetVersion2Edit(7)).Changed.ShouldBeTrue();
        Apply(
            editService,
            sources,
            state,
            target,
            new SkyrimSetMajorRecordFlagsEdit(SkyrimMajorRecord.SkyrimMajorRecordFlag.NotPlayable))
            .Changed.ShouldBeTrue();
        Apply(editService, sources, state, target, new SetCompressedEdit(true)).Changed.ShouldBeTrue();
        Apply(
            editService,
            sources,
            state,
            target,
            new ReplaceItemsEdit([fixture.Sources.BookFormKey, fixture.Sources.BookFormKey]))
            .Changed.ShouldBeTrue();
        Apply(editService, sources, state, target, new MoveItemEdit(0, 1)).Changed.ShouldBeFalse();
        Apply(
            editService,
            sources,
            state,
            target,
            new ReplaceItemsEdit([fixture.Sources.BookFormKey, FormKey.Null, fixture.Sources.BookFormKey]))
            .Changed.ShouldBeTrue();
        Apply(
            editService,
            sources,
            state,
            target,
            new InsertItemEdit(1, fixture.Sources.KeywordFormKey)).Changed.ShouldBeTrue();
        Apply(editService, sources, state, target, new MoveItemEdit(0, 3)).Changed.ShouldBeTrue();
        Apply(editService, sources, state, target, new RemoveItemEdit(1)).Changed.ShouldBeTrue();

        var record = state.CreateSnapshot().FormLists.ShouldHaveSingleItem();
        record.EditorID.ShouldBe("EditedList");
        record.VersionControl.ShouldBe(0x10203040U);
        record.FormVersion.ShouldBe((ushort)55);
        record.Version2.ShouldBe((ushort)7);
        record.SkyrimMajorRecordFlags.HasFlag(SkyrimMajorRecord.SkyrimMajorRecordFlag.NotPlayable).ShouldBeTrue();
        record.IsCompressed.ShouldBeTrue();
        record.Items.Select(item => item.FormKey).ShouldBe(
            [fixture.Sources.KeywordFormKey, fixture.Sources.BookFormKey, fixture.Sources.BookFormKey],
            ignoreOrder: false);

        Apply(editService, sources, state, target, new SetEditorIdEdit("EditedList")).Changed.ShouldBeFalse();
        Apply(editService, sources, state, target, new SetFormVersionEdit(55)).Changed.ShouldBeFalse();
        Apply(editService, sources, state, target, new ClearItemsEdit()).Changed.ShouldBeTrue();
        Apply(editService, sources, state, target, new ClearItemsEdit()).Changed.ShouldBeFalse();
        Apply(editService, sources, state, target, new ClearEditorIdEdit()).Changed.ShouldBeTrue();
        Apply(editService, sources, state, target, new SetDeletedEdit(true)).Changed.ShouldBeTrue();

        var cleared = state.CreateSnapshot().FormLists.ShouldHaveSingleItem();
        cleared.EditorID.ShouldBeNull();
        cleared.Items.ShouldBeEmpty();
        cleared.IsDeleted.ShouldBeTrue();
    }

    /// <summary>Canonical preparation retains case-sensitive text, structural identities, copied arrays, and complete rejected payload hashes.</summary>
    [Fact]
    public void PrepareEdit_CanonicalizesNativeIdentityAndHashesRejectedPayloads()
    {
        var service = new SkyrimNativeEditService(new SkyrimFormListNativeInspector());
        var upperKey = new FormKey(new ModKey("CasePlugin", ModType.Plugin), 0x123);
        var lowerKey = new FormKey(new ModKey("caseplugin", ModType.Plugin), 0x123);
        service.PrepareEdit(new ReplaceItemsEdit([upperKey])).Fingerprint
            .ShouldBe(service.PrepareEdit(new ReplaceItemsEdit([lowerKey])).Fingerprint);
        service.PrepareEdit(new SetEditorIdEdit("CaseSensitive")).Fingerprint
            .ShouldNotBe(service.PrepareEdit(new SetEditorIdEdit("casesensitive")).Fingerprint);

        var callerOwned = new[] { upperKey };
        var copiedCommand = new ReplaceItemsEdit(callerOwned);
        var copiedPrepared = service.PrepareEdit(copiedCommand);
        callerOwned[0] = new FormKey(upperKey.ModKey, 0x124);
        copiedPrepared.Fingerprint.ShouldBe(service.PrepareEdit(new ReplaceItemsEdit([upperKey])).Fingerprint);

        var malformedA = service.PrepareEdit(new SetEditorIdEdit("bad\uD800value"));
        var malformedB = service.PrepareEdit(new SetEditorIdEdit("bad\uD801value"));
        malformedA.IsValid.ShouldBeFalse();
        malformedA.Error!.Code.ShouldBe(EngineErrorCode.ValidationFailed);
        malformedB.IsValid.ShouldBeFalse();
        malformedB.Fingerprint.ShouldNotBe(malformedA.Fingerprint);

        var invalidFlags = service.PrepareEdit(new SkyrimSetMajorRecordFlagsEdit(
            unchecked((SkyrimMajorRecord.SkyrimMajorRecordFlag)0x40000000)));
        invalidFlags.IsValid.ShouldBeFalse();
        invalidFlags.Error!.Code.ShouldBe(EngineErrorCode.ValidationFailed);

        var invalidHighBitFlags = service.PrepareEdit(new SkyrimSetMajorRecordFlagsEdit(
            unchecked((SkyrimMajorRecord.SkyrimMajorRecordFlag)(int)0x80000000U)));
        var invalidDistinctHighBitFlags = service.PrepareEdit(new SkyrimSetMajorRecordFlagsEdit(
            unchecked((SkyrimMajorRecord.SkyrimMajorRecordFlag)(int)0xC0000000U)));
        invalidHighBitFlags.IsValid.ShouldBeFalse();
        invalidHighBitFlags.Error!.Code.ShouldBe(EngineErrorCode.ValidationFailed);
        invalidHighBitFlags.Error.Message.ShouldContain("0x80000000");
        invalidDistinctHighBitFlags.IsValid.ShouldBeFalse();
        invalidDistinctHighBitFlags.Error!.Code.ShouldBe(EngineErrorCode.ValidationFailed);
        invalidDistinctHighBitFlags.Error.Message.ShouldContain("0xC0000000");
        invalidDistinctHighBitFlags.Fingerprint.ShouldNotBe(invalidHighBitFlags.Fingerprint);

        var unsupported = service.PrepareEdit(new UnsupportedSkyrimGameFieldEdit());
        unsupported.IsValid.ShouldBeFalse();
        unsupported.Error!.Code.ShouldBe(EngineErrorCode.UnsupportedOperation);
        unsupported.Error.Message.ShouldContain("UnsupportedGameField");
    }

    /// <summary>Only introduced links are validated, while retained missing links remain editable and visible in preview.</summary>
    /// <returns>A task that completes after the existing output candidate is released.</returns>
    [Fact]
    public async Task ApplyEdit_ValidatesIntroducedReferencesAtomicallyAndRetainsExistingMissingLinks()
    {
        using var fixture = SkyrimNativeOutputTestFixture.Create();
        await using var sources = await OpenSourcesAsync(fixture);
        var outputService = CreateOutputService();
        var editService = new SkyrimNativeEditService(outputService.Inspector);
        await using var state = await OpenExistingStateAsync(fixture, sources, outputService);
        BeginExisting(outputService, sources, state, fixture.OutputOwnListFormKey);

        Apply(
            editService,
            sources,
            state,
            fixture.OutputOwnListFormKey,
            new ReplaceItemsEdit([fixture.OutputKeywordFormKey, fixture.Sources.MissingFormKey]))
            .Changed.ShouldBeTrue();
        var retained = state.CreateSnapshot().FormLists.Single(record => record.FormKey == fixture.OutputOwnListFormKey);

        var introducedDuplicate = editService.ApplyEdit(
            sources,
            state,
            fixture.OutputOwnListFormKey,
            editService.PrepareEdit(new ReplaceItemsEdit(
            [
                fixture.OutputKeywordFormKey,
                fixture.Sources.MissingFormKey,
                fixture.Sources.MissingFormKey,
            ])));
        introducedDuplicate.Succeeded.ShouldBeFalse();
        introducedDuplicate.Error!.Code.ShouldBe(EngineErrorCode.ValidationFailed);

        var deletedReference = editService.ApplyEdit(
            sources,
            state,
            fixture.OutputOwnListFormKey,
            editService.PrepareEdit(new InsertItemEdit(1, fixture.Sources.DeletedListFormKey)));
        deletedReference.Succeeded.ShouldBeFalse();
        deletedReference.Error!.Message.ShouldContain("deleted", Case.Insensitive);

        var invalidIndex = editService.ApplyEdit(
            sources,
            state,
            fixture.OutputOwnListFormKey,
            editService.PrepareEdit(new MoveItemEdit(0, 2)));
        invalidIndex.Succeeded.ShouldBeFalse();
        invalidIndex.Error!.Code.ShouldBe(EngineErrorCode.ValidationFailed);

        var afterFailures = state.CreateSnapshot().FormLists.Single(record => record.FormKey == fixture.OutputOwnListFormKey);
        outputService.Inspector.Compare(
            retained,
            afterFailures,
            TestContext.Current.CancellationToken).ShouldBeEmpty();

        var preview = editService.Preview(sources, state);
        preview.Succeeded.ShouldBeTrue(preview.Error?.Message);
        preview.Value!.UnresolvedReferenceCount.ShouldBe(1);
        preview.Value.Warnings.ShouldHaveSingleItem().Code.ShouldBe("missing-native-reference");
    }

    /// <summary>Source override preview reconstructs the exact selected context and cloned candidates retain only its metadata.</summary>
    /// <returns>A task that completes after original and cloned candidate previews are released.</returns>
    [Fact]
    public async Task Preview_SourceOverride_UsesExactContextAndSurvivesClone()
    {
        using var fixture = SkyrimNativeOutputTestFixture.Create();
        await using var sources = await OpenSourcesAsync(fixture);
        var outputService = CreateOutputService();
        var editService = new SkyrimNativeEditService(outputService.Inspector);
        await using var state = await CreateNewStateAsync(fixture, sources, outputService, "SourcePreview.esm");
        var begin = outputService.BeginEdit(
            sources,
            state,
            new BeginEditRequest(
                Guid.NewGuid(),
                sources.Revision,
                FormListEditRole.Override,
                fixture.Sources.SourceListFormKey,
                new ReferenceRequest(
                    fixture.Sources.SourceListFormKey,
                    RecordScope.AllContexts,
                    fixture.Sources.SourceModKey)),
            TestContext.Current.CancellationToken);
        begin.Succeeded.ShouldBeTrue(begin.Error?.Message);
        Apply(editService, sources, state, begin.Value!.FormKey, new SetEditorIdEdit("Preview change"));

        var preview = editService.Preview(sources, state);
        preview.Succeeded.ShouldBeTrue(preview.Error?.Message);
        var comparison = preview.Value!.Comparisons.ShouldHaveSingleItem();
        comparison.Before!.Value.GetProperty("EditorID").GetString().ShouldBe("SharedListSmall");
        comparison.Before.Value.GetProperty("EditorID").GetString().ShouldNotBe("SharedListOverride");
        comparison.After!.Value.GetProperty("EditorID").GetString().ShouldBe("Preview change");
        comparison.BeforeContext.Selection.Scope.ShouldBe(RecordScope.AllContexts);
        comparison.BeforeContext.Selection.ContainingModKey.ShouldBe(fixture.Sources.SourceModKey);
        comparison.Changes.Select(change => change.FieldIdentifier).ShouldContain("EditorID");

        await using var clone = outputService.Clone(state, TestContext.Current.CancellationToken);
        var clonedPreview = editService.Preview(sources, clone);
        clonedPreview.Succeeded.ShouldBeTrue(clonedPreview.Error?.Message);
        clonedPreview.Value!.Comparisons.ShouldHaveSingleItem()
            .Before!.Value.GetProperty("EditorID").GetString().ShouldBe("SharedListSmall");
    }

    /// <summary>Preview distinguishes absent, original, and deleted baselines while preserving unrelated complete output state and raw bits.</summary>
    /// <returns>A task that completes after new and existing output candidates are released.</returns>
    [Fact]
    public async Task Preview_NewExistingAndDeletedBaselines_PreserveCompleteOutputAndWarn()
    {
        using var fixture = SkyrimNativeOutputTestFixture.Create();
        var artifactsBefore = fixture.SnapshotOutputArtifacts();
        await using var sources = await OpenSourcesAsync(fixture);
        var outputService = CreateOutputService();
        var editService = new SkyrimNativeEditService(outputService.Inspector);
        await using var newState = await CreateNewStateAsync(fixture, sources, outputService, "NewPreview.esm");
        var newTarget = BeginNew(outputService, sources, newState);
        Apply(editService, sources, newState, newTarget, new SetEditorIdEdit("Allocated"));
        var newComparison = editService.Preview(sources, newState).Value!.Comparisons.ShouldHaveSingleItem();
        newComparison.Before.ShouldBeNull();
        newComparison.BeforeContext.Status.ShouldBe(ReferenceResolutionStatus.Unresolved);
        newComparison.AfterContext.Status.ShouldBe(ReferenceResolutionStatus.Resolved);

        await using var existingState = await OpenExistingStateAsync(fixture, sources, outputService);
        BeginExisting(outputService, sources, existingState, fixture.OutputOwnListFormKey);
        BeginExisting(outputService, sources, existingState, fixture.OutputDeletedListFormKey);
        Apply(
            editService,
            sources,
            existingState,
            fixture.OutputOwnListFormKey,
            new ReplaceItemsEdit([fixture.OutputKeywordFormKey, fixture.Sources.MissingFormKey]));
        Apply(editService, sources, existingState, fixture.OutputOwnListFormKey, new SetFormVersionEdit(55));
        Apply(
            editService,
            sources,
            existingState,
            fixture.OutputOwnListFormKey,
            new SkyrimSetMajorRecordFlagsEdit(SkyrimMajorRecord.SkyrimMajorRecordFlag.NotPlayable));
        BeginExisting(outputService, sources, existingState, fixture.OutputOwnListFormKey);
        Apply(editService, sources, existingState, fixture.OutputDeletedListFormKey, new SetDeletedEdit(false));

        var current = existingState.CreateSnapshot();
        current.IsMaster.ShouldBeTrue();
        current.UsingLocalization.ShouldBeTrue();
        current.Keywords.ShouldContain(record => record.FormKey == fixture.OutputKeywordFormKey);
        current.Books.ShouldContain(record => record.FormKey == fixture.OutputBookFormKey);
        current.FormLists.Single(record => record.FormKey == fixture.OutputOwnListFormKey)
            .MajorRecordFlagsRaw.ShouldBe(unchecked((int)0x40000004));

        var preview = editService.Preview(sources, existingState);
        preview.Succeeded.ShouldBeTrue(preview.Error?.Message);
        preview.Value!.Comparisons.Count.ShouldBe(2);
        var own = preview.Value.Comparisons.Single(item => item.FormKey == fixture.OutputOwnListFormKey);
        own.BeforeContext.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
        own.BeforeContext.Selection.Scope.ShouldBe(RecordScope.StagedOutput);
        own.Before!.Value.GetProperty("FormVersion").GetUInt16().ShouldBe((ushort)44);
        own.After!.Value.GetProperty("FormVersion").GetUInt16().ShouldBe((ushort)55);
        own.Warnings.ShouldHaveSingleItem().Code.ShouldBe("missing-native-reference");
        var deleted = preview.Value.Comparisons.Single(item => item.FormKey == fixture.OutputDeletedListFormKey);
        deleted.BeforeContext.Status.ShouldBe(ReferenceResolutionStatus.Deleted);
        deleted.AfterContext.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
        preview.Value.UnresolvedReferenceCount.ShouldBe(1);
        preview.Value.Warnings.ShouldHaveSingleItem().Code.ShouldBe("missing-native-reference");

        AssertArtifactsEqual(artifactsBefore, fixture.SnapshotOutputArtifacts());
        File.Exists(Path.Combine(fixture.OutputDirectory.FullName, "NewPreview.esm")).ShouldBeFalse();
    }

    /// <summary>Applies one caller-owned command through immutable preparation and requires native success.</summary>
    /// <param name="service">The Skyrim edit service.</param>
    /// <param name="sources">The immutable native sources.</param>
    /// <param name="state">The unpublished output candidate.</param>
    /// <param name="target">The exact target FormKey.</param>
    /// <param name="edit">The typed caller-owned command.</param>
    /// <returns>The successful immediate native mutation result.</returns>
    private static NativeEditMutationResult Apply(
        SkyrimNativeEditService service,
        SkyrimNativeSourceSet sources,
        SkyrimNativeOutputState state,
        FormKey target,
        FormListEdit edit)
    {
        var result = service.ApplyEdit(sources, state, target, service.PrepareEdit(edit));
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value!;
    }

    /// <summary>Allocates one new Skyrim FormList and records an absent first-edit baseline.</summary>
    /// <param name="service">The native output service.</param>
    /// <param name="sources">The immutable native sources.</param>
    /// <param name="state">The unpublished output candidate.</param>
    /// <returns>The allocated native identity.</returns>
    private static FormKey BeginNew(
        SkyrimNativeOutputService service,
        SkyrimNativeSourceSet sources,
        SkyrimNativeOutputState state)
    {
        var begin = service.BeginEdit(
            sources,
            state,
            new BeginEditRequest(Guid.NewGuid(), sources.Revision, FormListEditRole.New),
            TestContext.Current.CancellationToken);
        begin.Succeeded.ShouldBeTrue(begin.Error?.Message);
        return begin.Value!.FormKey;
    }

    /// <summary>Selects one existing output FormList while retaining its first original-output baseline.</summary>
    /// <param name="service">The native output service.</param>
    /// <param name="sources">The immutable native sources.</param>
    /// <param name="state">The unpublished output candidate.</param>
    /// <param name="target">The exact output-contained FormKey.</param>
    private static void BeginExisting(
        SkyrimNativeOutputService service,
        SkyrimNativeSourceSet sources,
        SkyrimNativeOutputState state,
        FormKey target)
    {
        var begin = service.BeginEdit(
            sources,
            state,
            new BeginEditRequest(
                Guid.NewGuid(),
                sources.Revision,
                FormListEditRole.ExistingOutput,
                targetFormKey: target),
            TestContext.Current.CancellationToken);
        begin.Succeeded.ShouldBeTrue(begin.Error?.Message);
    }

    /// <summary>Opens generated Skyrim sources through the production native loader.</summary>
    /// <param name="fixture">The generated complete source and output fixture.</param>
    /// <returns>The independently owned source lifetime.</returns>
    private static async Task<SkyrimNativeSourceSet> OpenSourcesAsync(
        SkyrimNativeOutputTestFixture fixture)
    {
        var open = await new SkyrimNativeSourceLoader(new NativeSourceInputLoader()).OpenAsync(
            fixture.Sources.CreateOpenRequest(),
            TestContext.Current.CancellationToken);
        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        return open.Value!.Sources.ShouldBeOfType<SkyrimNativeSourceSet>();
    }

    /// <summary>Creates and selects one absent native Skyrim output without writing it.</summary>
    /// <param name="fixture">The generated complete source and output fixture.</param>
    /// <param name="sources">The immutable native sources.</param>
    /// <param name="service">The native output service.</param>
    /// <param name="fileName">The absent output file name.</param>
    /// <returns>The independently owned empty output state.</returns>
    private static async Task<SkyrimNativeOutputState> CreateNewStateAsync(
        SkyrimNativeOutputTestFixture fixture,
        SkyrimNativeSourceSet sources,
        SkyrimNativeOutputService service,
        string fileName)
    {
        var result = await service.OpenAsync(
            sources,
            new SelectOutputRequest(
                Guid.NewGuid(),
                sources.Revision,
                OutputSelectionMode.CreateNew,
                fixture.CreateNewAssociation(fileName)),
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value!.Output.ShouldBeOfType<SkyrimNativeOutputState>();
    }

    /// <summary>Opens the generated complete existing Skyrim output without changing its artifacts.</summary>
    /// <param name="fixture">The generated complete source and output fixture.</param>
    /// <param name="sources">The immutable native sources.</param>
    /// <param name="service">The native output service.</param>
    /// <returns>The independently owned complete existing output state.</returns>
    private static async Task<SkyrimNativeOutputState> OpenExistingStateAsync(
        SkyrimNativeOutputTestFixture fixture,
        SkyrimNativeSourceSet sources,
        SkyrimNativeOutputService service)
    {
        var result = await service.OpenAsync(
            sources,
            new SelectOutputRequest(
                Guid.NewGuid(),
                sources.Revision,
                OutputSelectionMode.OpenExisting,
                fixture.CreateExistingAssociation()),
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value!.Output.ShouldBeOfType<SkyrimNativeOutputState>();
    }

    /// <summary>Creates the production Skyrim output service with shared native input admission.</summary>
    /// <returns>A new output service instance.</returns>
    private static SkyrimNativeOutputService CreateOutputService()
    {
        return new SkyrimNativeOutputService(new NativeOutputInputLoader());
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

    /// <summary>Represents a recognized caller command outside Skyrim's supported game-specific field surface.</summary>
    private sealed class UnsupportedSkyrimGameFieldEdit : FormListEdit
    {
        /// <summary>Initializes a deterministic unsupported game-field command.</summary>
        internal UnsupportedSkyrimGameFieldEdit()
            : base("other-game.form-list.set-name")
        { }
    }
}
