using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInputs;
using CreationsForge.Core.Engine.NativeOutputs;
using CreationsForge.Fallout4.Native;
using CreationsForge.Fallout4.Native.Edits;
using CreationsForge.Fallout4.Native.NativeInspection;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Strings;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Fallout4;

/// <summary>Verifies immutable preparation, atomic typed application, provenance, and exact Fallout 4 preview behavior.</summary>
public sealed class Fallout4NativeEditServiceTests
{
    /// <summary>Every supported typed field edit mutates native candidate state with exact ordered item semantics.</summary>
    /// <returns>A task that completes after the generated native lifetimes are released.</returns>
    [Fact]
    public async Task ApplyEdit_AllSupportedFields_PreservesTypedValuesAndOrder()
    {
        using var fixture = Fallout4NativeOutputTestFixture.Create();
        var sourceArtifacts = fixture.Sources.SnapshotArtifacts();
        await using var sources = await OpenSourcesAsync(fixture);
        var outputService = CreateOutputService();
        var editService = new Fallout4NativeEditService(outputService.Inspector);
        await using var state = await CreateNewStateAsync(fixture, sources, outputService, "TypedEdits.esm");
        var target = BeginNew(outputService, sources, state);

        Apply(editService, sources, state, target, new SetEditorIdEdit("EditedList")).Changed.ShouldBeTrue();
        Apply(editService, sources, state, target, new SetVersionControlEdit(0x10203040)).Changed.ShouldBeTrue();
        Apply(editService, sources, state, target, new SetFormVersionEdit(60000)).Changed.ShouldBeTrue();
        Apply(editService, sources, state, target, new SetVersion2Edit(7)).Changed.ShouldBeTrue();
        Apply(
            editService,
            sources,
            state,
            target,
            new Fallout4SetMajorRecordFlagsEdit(Fallout4MajorRecord.Fallout4MajorRecordFlag.NotPlayable))
            .Changed.ShouldBeTrue();
        Apply(editService, sources, state, target, new SetCompressedEdit(true)).Changed.ShouldBeTrue();

        var name = CreateName("Prepared name", "Nom prepare");
        Apply(editService, sources, state, target, new Fallout4SetNameEdit(name)).Changed.ShouldBeTrue();
        Apply(
            editService,
            sources,
            state,
            target,
            new ReplaceItemsEdit([fixture.Sources.BookFormKey, fixture.Sources.BookFormKey]))
            .Changed.ShouldBeTrue();
        Apply(editService, sources, state, target, new MoveItemEdit(0, 1)).Changed.ShouldBeFalse();
        var replacement = new[]
        {
            fixture.Sources.BookFormKey,
            FormKey.Null,
            fixture.Sources.BookFormKey,
        };
        Apply(editService, sources, state, target, new ReplaceItemsEdit(replacement)).Changed.ShouldBeTrue();
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
        record.FormVersion.ShouldBe((ushort)60000);
        record.Version2.ShouldBe((ushort)7);
        record.Fallout4MajorRecordFlags
            .HasFlag(Fallout4MajorRecord.Fallout4MajorRecordFlag.NotPlayable)
            .ShouldBeTrue();
        record.IsCompressed.ShouldBeTrue();
        record.Name!.String.ShouldBe("Prepared name");
        record.Name.Lookup(Language.French).ShouldBe("Nom prepare");
        record.Items.Select(item => item.FormKey).ShouldBe(
            [fixture.Sources.KeywordFormKey, fixture.Sources.BookFormKey, fixture.Sources.BookFormKey],
            ignoreOrder: false);

        Apply(editService, sources, state, target, new SetEditorIdEdit("EditedList")).Changed.ShouldBeFalse();
        Apply(editService, sources, state, target, new Fallout4ClearNameEdit()).Changed.ShouldBeTrue();
        Apply(editService, sources, state, target, new Fallout4ClearNameEdit()).Changed.ShouldBeFalse();
        Apply(
            editService,
            sources,
            state,
            target,
            new Fallout4SetNameEdit(new TranslatedString(Language.English, string.Empty)))
            .Changed.ShouldBeTrue();
        state.CreateSnapshot().FormLists.ShouldHaveSingleItem().Name!.String.ShouldBe(string.Empty);
        Apply(editService, sources, state, target, new Fallout4ClearNameEdit()).Changed.ShouldBeTrue();
        Apply(editService, sources, state, target, new ClearItemsEdit()).Changed.ShouldBeTrue();
        Apply(editService, sources, state, target, new ClearItemsEdit()).Changed.ShouldBeFalse();
        Apply(editService, sources, state, target, new ClearEditorIdEdit()).Changed.ShouldBeTrue();
        Apply(editService, sources, state, target, new SetDeletedEdit(true)).Changed.ShouldBeTrue();

        var cleared = state.CreateSnapshot().FormLists.ShouldHaveSingleItem();
        cleared.EditorID.ShouldBeNull();
        cleared.Name.ShouldBeNull();
        cleared.Items.ShouldBeEmpty();
        cleared.IsDeleted.ShouldBeTrue();
        File.Exists(state.Association.PluginPath).ShouldBeFalse();
        AssertArtifactsUnchanged(sourceArtifacts);
    }

    /// <summary>Preparation freezes translated strings, canonicalizes equivalent values, and fingerprints malformed text before rejection.</summary>
    /// <returns>A task that completes after the generated native lifetimes are released.</returns>
    [Fact]
    public async Task PrepareEdit_CopiesNameAndFingerprintsCompleteRejectedPayloads()
    {
        using var fixture = Fallout4NativeOutputTestFixture.Create();
        await using var sources = await OpenSourcesAsync(fixture);
        var outputService = CreateOutputService();
        var editService = new Fallout4NativeEditService(outputService.Inspector);
        await using var state = await CreateNewStateAsync(fixture, sources, outputService, "PreparedName.esm");
        var target = BeginNew(outputService, sources, state);
        var callerOwned = CreateName("Original", "Original francais");

        var prepared = editService.PrepareEdit(new Fallout4SetNameEdit(callerOwned));
        callerOwned.Set(Language.English, "Mutated");
        callerOwned.Set(Language.French, "Francais modifie");
        editService.ApplyEdit(sources, state, target, prepared).Succeeded.ShouldBeTrue();

        var applied = state.CreateSnapshot().FormLists.ShouldHaveSingleItem().Name!;
        applied.String.ShouldBe("Original");
        applied.Lookup(Language.French).ShouldBe("Original francais");
        var equivalent = editService.PrepareEdit(new Fallout4SetNameEdit(
            new TranslatedString(
                Language.English,
                new KeyValuePair<Language, string>(Language.French, "Original francais"),
                new KeyValuePair<Language, string>(Language.English, "Original"))));
        equivalent.Fingerprint.ShouldBe(prepared.Fingerprint);

        var malformedA = editService.PrepareEdit(new SetEditorIdEdit("bad\uD800value"));
        var malformedB = editService.PrepareEdit(new SetEditorIdEdit("bad\uD801value"));
        malformedA.IsValid.ShouldBeFalse();
        malformedA.Error!.Code.ShouldBe(EngineErrorCode.ValidationFailed);
        malformedB.IsValid.ShouldBeFalse();
        malformedB.Fingerprint.ShouldNotBe(malformedA.Fingerprint);
        editService.ApplyEdit(sources, state, target, malformedA).Succeeded.ShouldBeFalse();
        state.CreateSnapshot().FormLists.ShouldHaveSingleItem().EditorID.ShouldBeNull();

        var supportedFlagBits = Enum.GetValues<Fallout4MajorRecord.Fallout4MajorRecordFlag>()
            .Aggregate(0U, (current, value) => current | unchecked((uint)(int)value));
        var unsupportedBit = Enumerable.Range(0, 32)
            .Select(index => 1U << index)
            .First(bit => (supportedFlagBits & bit) == 0);
        var invalidFlags = editService.PrepareEdit(new Fallout4SetMajorRecordFlagsEdit(
            (Fallout4MajorRecord.Fallout4MajorRecordFlag)unsupportedBit));
        invalidFlags.IsValid.ShouldBeFalse();
        invalidFlags.Error!.Code.ShouldBe(EngineErrorCode.ValidationFailed);

        var invalidHighBitFlags = editService.PrepareEdit(new Fallout4SetMajorRecordFlagsEdit(
            unchecked((Fallout4MajorRecord.Fallout4MajorRecordFlag)(int)0x80000000U)));
        var invalidDistinctHighBitFlags = editService.PrepareEdit(new Fallout4SetMajorRecordFlagsEdit(
            unchecked((Fallout4MajorRecord.Fallout4MajorRecordFlag)(int)0xC0000000U)));
        invalidHighBitFlags.IsValid.ShouldBeFalse();
        invalidHighBitFlags.Error!.Code.ShouldBe(EngineErrorCode.ValidationFailed);
        invalidHighBitFlags.Error.Message.ShouldContain("0x80000000");
        invalidDistinctHighBitFlags.IsValid.ShouldBeFalse();
        invalidDistinctHighBitFlags.Error!.Code.ShouldBe(EngineErrorCode.ValidationFailed);
        invalidDistinctHighBitFlags.Error.Message.ShouldContain("0xC0000000");
        invalidDistinctHighBitFlags.Fingerprint.ShouldNotBe(invalidHighBitFlags.Fingerprint);

        var upperIdentity = new FormKey(ModKey.FromNameAndExtension("CaseIdentity.esm"), 0x123);
        var lowerIdentity = new FormKey(ModKey.FromNameAndExtension("caseidentity.esm"), 0x123);
        var differentIdentity = new FormKey(ModKey.FromNameAndExtension("caseidentity.esm"), 0x124);
        var upperFingerprint = editService.PrepareEdit(new ReplaceItemsEdit([upperIdentity])).Fingerprint;
        editService.PrepareEdit(new ReplaceItemsEdit([lowerIdentity])).Fingerprint.ShouldBe(upperFingerprint);
        editService.PrepareEdit(new ReplaceItemsEdit([differentIdentity])).Fingerprint.ShouldNotBe(upperFingerprint);
    }

    /// <summary>Invalid new links and list indices are rejected before any candidate field is mutated.</summary>
    /// <returns>A task that completes after the generated native lifetimes are released.</returns>
    [Fact]
    public async Task ApplyEdit_InvalidReferenceOrIndex_LeavesCandidateUnchanged()
    {
        using var fixture = Fallout4NativeOutputTestFixture.Create();
        await using var sources = await OpenSourcesAsync(fixture);
        var outputService = CreateOutputService();
        var editService = new Fallout4NativeEditService(outputService.Inspector);
        await using var state = await CreateNewStateAsync(fixture, sources, outputService, "AtomicValidation.esm");
        var target = BeginNew(outputService, sources, state);
        Apply(
            editService,
            sources,
            state,
            target,
            new ReplaceItemsEdit([fixture.Sources.BookFormKey, FormKey.Null]));
        var before = state.CreateSnapshot().FormLists.ShouldHaveSingleItem();

        var missing = new FormKey(fixture.Sources.SourceModKey, 0x0F01);
        var invalidReference = editService.ApplyEdit(
            sources,
            state,
            target,
            editService.PrepareEdit(new ReplaceItemsEdit(
            [
                fixture.Sources.KeywordFormKey,
                missing,
            ])));
        invalidReference.Succeeded.ShouldBeFalse();
        invalidReference.Error!.Code.ShouldBe(EngineErrorCode.ValidationFailed);

        var deletedReference = editService.ApplyEdit(
            sources,
            state,
            target,
            editService.PrepareEdit(new InsertItemEdit(1, fixture.Sources.DeletedListFormKey)));
        deletedReference.Succeeded.ShouldBeFalse();
        deletedReference.Error!.Message.ShouldContain("Deleted");

        var invalidIndex = editService.ApplyEdit(
            sources,
            state,
            target,
            editService.PrepareEdit(new MoveItemEdit(0, 2)));
        invalidIndex.Succeeded.ShouldBeFalse();
        invalidIndex.Error!.Code.ShouldBe(EngineErrorCode.ValidationFailed);

        var after = state.CreateSnapshot().FormLists.ShouldHaveSingleItem();
        outputService.Inspector.Compare(before, after, TestContext.Current.CancellationToken).ShouldBeEmpty();
    }

    /// <summary>Source override preview retains the exact context chosen at begin even when another plugin wins.</summary>
    /// <returns>A task that completes after original and cloned candidate previews are inspected.</returns>
    [Fact]
    public async Task Preview_SourceOverride_UsesCapturedExactContextAndSurvivesClone()
    {
        using var fixture = Fallout4NativeOutputTestFixture.Create();
        await using var sources = await OpenSourcesAsync(fixture);
        var outputService = CreateOutputService();
        var editService = new Fallout4NativeEditService(outputService.Inspector);
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

    /// <summary>Preview distinguishes absent and deleted baselines, preserves unrepresented raw flag bits, and reports unresolved links.</summary>
    /// <returns>A task that completes after new and existing native output states are released.</returns>
    [Fact]
    public async Task Preview_NewExistingAndDeletedBaselines_ReportExactContextsAndWarnings()
    {
        using var fixture = Fallout4NativeOutputTestFixture.Create();
        await using var sources = await OpenSourcesAsync(fixture);
        var outputService = CreateOutputService();
        var editService = new Fallout4NativeEditService(outputService.Inspector);
        await using var newState = await CreateNewStateAsync(fixture, sources, outputService, "NewPreview.esm");
        var newTarget = BeginNew(outputService, sources, newState);
        Apply(editService, sources, newState, newTarget, new SetEditorIdEdit("Allocated"));
        var newComparison = editService.Preview(sources, newState).Value!.Comparisons.ShouldHaveSingleItem();
        newComparison.Before.ShouldBeNull();
        newComparison.BeforeContext.Status.ShouldBe(ReferenceResolutionStatus.Unresolved);
        newComparison.AfterContext.Status.ShouldBe(ReferenceResolutionStatus.Resolved);

        var existing = fixture.WriteExistingOutput(includeMissingReference: true);
        await using var existingState = await OpenExistingStateAsync(sources, outputService, existing.Association);
        BeginExisting(outputService, sources, existingState, existing.OwnListFormKey);
        BeginExisting(outputService, sources, existingState, existing.DeletedListFormKey);
        var retainedMissingReference = new FormKey(existing.Association.ModKey, 0x0F01);
        Apply(
            editService,
            sources,
            existingState,
            existing.OwnListFormKey,
            new ReplaceItemsEdit(
            [
                existing.KeywordFormKey,
                existing.BookFormKey,
                retainedMissingReference,
            ])).Changed.ShouldBeTrue();
        Apply(editService, sources, existingState, existing.OwnListFormKey, new SetFormVersionEdit(55));
        Apply(
            editService,
            sources,
            existingState,
            existing.OwnListFormKey,
            new Fallout4SetMajorRecordFlagsEdit(Fallout4MajorRecord.Fallout4MajorRecordFlag.NotPlayable));
        Apply(editService, sources, existingState, existing.DeletedListFormKey, new SetDeletedEdit(false));

        existingState.CreateSnapshot().FormLists
            .Single(record => record.FormKey == existing.OwnListFormKey)
            .MajorRecordFlagsRaw.ShouldBe(unchecked((int)0x40000004));

        var preview = editService.Preview(sources, existingState);
        preview.Succeeded.ShouldBeTrue(preview.Error?.Message);
        preview.Value!.Comparisons.Count.ShouldBe(2);
        var own = preview.Value.Comparisons.Single(item => item.FormKey == existing.OwnListFormKey);
        own.BeforeContext.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
        own.BeforeContext.Selection.Scope.ShouldBe(RecordScope.StagedOutput);
        own.Warnings.ShouldHaveSingleItem().Code.ShouldBe("missing-native-reference");
        var deleted = preview.Value.Comparisons.Single(item => item.FormKey == existing.DeletedListFormKey);
        deleted.BeforeContext.Status.ShouldBe(ReferenceResolutionStatus.Deleted);
        deleted.AfterContext.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
        preview.Value.UnresolvedReferenceCount.ShouldBe(1);
        preview.Value.Warnings.ShouldHaveSingleItem().Code.ShouldBe("missing-native-reference");
    }

    /// <summary>Applies one caller-owned command through immutable preparation and requires native success.</summary>
    /// <param name="service">The Fallout 4 edit service.</param>
    /// <param name="sources">The immutable native sources.</param>
    /// <param name="state">The unpublished output candidate.</param>
    /// <param name="target">The exact target FormKey.</param>
    /// <param name="edit">The typed caller-owned edit.</param>
    /// <returns>The successful native mutation result.</returns>
    private static NativeEditMutationResult Apply(
        Fallout4NativeEditService service,
        Fallout4NativeSourceSet sources,
        Fallout4NativeOutputState state,
        FormKey target,
        FormListEdit edit)
    {
        var result = service.ApplyEdit(sources, state, target, service.PrepareEdit(edit));
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value!;
    }

    /// <summary>Allocates one new FormList and returns its output-owned identity.</summary>
    /// <param name="service">The native output service.</param>
    /// <param name="sources">The immutable native sources.</param>
    /// <param name="state">The unpublished output candidate.</param>
    /// <returns>The allocated FormKey.</returns>
    private static FormKey BeginNew(
        Fallout4NativeOutputService service,
        Fallout4NativeSourceSet sources,
        Fallout4NativeOutputState state)
    {
        var begin = service.BeginEdit(
            sources,
            state,
            new BeginEditRequest(Guid.NewGuid(), sources.Revision, FormListEditRole.New),
            TestContext.Current.CancellationToken);
        begin.Succeeded.ShouldBeTrue(begin.Error?.Message);
        return begin.Value!.FormKey;
    }

    /// <summary>Selects one existing output FormList and records its first original-output baseline.</summary>
    /// <param name="service">The native output service.</param>
    /// <param name="sources">The immutable native sources.</param>
    /// <param name="state">The unpublished output candidate.</param>
    /// <param name="target">The exact output-contained FormKey.</param>
    private static void BeginExisting(
        Fallout4NativeOutputService service,
        Fallout4NativeSourceSet sources,
        Fallout4NativeOutputState state,
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

    /// <summary>Opens generated Fallout 4 sources through the production native loader.</summary>
    /// <param name="fixture">The generated fixture.</param>
    /// <returns>The independently owned source lifetime.</returns>
    private static async Task<Fallout4NativeSourceSet> OpenSourcesAsync(Fallout4NativeOutputTestFixture fixture)
    {
        var open = await new Fallout4NativeSourceLoader(new NativeSourceInputLoader()).OpenAsync(
            fixture.Sources.CreateOpenRequest(),
            TestContext.Current.CancellationToken);
        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        return open.Value!.Sources.ShouldBeOfType<Fallout4NativeSourceSet>();
    }

    /// <summary>Creates and selects one absent native output without writing it.</summary>
    /// <param name="fixture">The generated fixture.</param>
    /// <param name="sources">The immutable native sources.</param>
    /// <param name="service">The native output service.</param>
    /// <param name="fileName">The absent output file name.</param>
    /// <returns>The independently owned complete output state.</returns>
    private static async Task<Fallout4NativeOutputState> CreateNewStateAsync(
        Fallout4NativeOutputTestFixture fixture,
        Fallout4NativeSourceSet sources,
        Fallout4NativeOutputService service,
        string fileName)
    {
        var result = await service.OpenAsync(
            sources,
            new SelectOutputRequest(
                Guid.NewGuid(),
                sources.Revision,
                OutputSelectionMode.CreateNew,
                fixture.CreateAssociation(fileName)),
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value!.Output.ShouldBeOfType<Fallout4NativeOutputState>();
    }

    /// <summary>Opens one generated existing native output.</summary>
    /// <param name="sources">The immutable native sources.</param>
    /// <param name="service">The native output service.</param>
    /// <param name="association">The exact existing-output association.</param>
    /// <returns>The independently owned complete output state.</returns>
    private static async Task<Fallout4NativeOutputState> OpenExistingStateAsync(
        Fallout4NativeSourceSet sources,
        Fallout4NativeOutputService service,
        OutputAssociation association)
    {
        var result = await service.OpenAsync(
            sources,
            new SelectOutputRequest(
                Guid.NewGuid(),
                sources.Revision,
                OutputSelectionMode.OpenExisting,
                association),
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value!.Output.ShouldBeOfType<Fallout4NativeOutputState>();
    }

    /// <summary>Creates the production output service.</summary>
    /// <returns>A Fallout 4 output service using shared native artifact admission.</returns>
    private static Fallout4NativeOutputService CreateOutputService()
    {
        return new Fallout4NativeOutputService(new NativeOutputInputLoader());
    }

    /// <summary>Creates a two-language native translated string.</summary>
    /// <param name="english">The selected English value.</param>
    /// <param name="french">The retained French translation.</param>
    /// <returns>A mutable caller-owned translated string.</returns>
    private static TranslatedString CreateName(string english, string french)
    {
        var name = new TranslatedString(Language.English, english);
        name.Set(Language.French, french);
        return name;
    }

    /// <summary>Asserts that every generated source artifact retained its exact original bytes.</summary>
    /// <param name="expected">The canonical-path-keyed artifact snapshot.</param>
    private static void AssertArtifactsUnchanged(IReadOnlyDictionary<string, byte[]> expected)
    {
        foreach (var artifact in expected.OrderBy(item => item.Key, StringComparer.OrdinalIgnoreCase))
        {
            File.ReadAllBytes(artifact.Key).ShouldBe(artifact.Value);
        }
    }
}
