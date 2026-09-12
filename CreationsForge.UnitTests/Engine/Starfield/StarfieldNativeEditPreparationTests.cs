using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInputs;
using CreationsForge.Core.Engine.NativeOutputs;
using CreationsForge.Starfield.Native;
using CreationsForge.Starfield.Native.Edits;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Strings;
using Noggog;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Starfield;

/// <summary>Verifies Starfield typed command copying, canonical identity, validation, mutation, and no-op detection.</summary>
public sealed class StarfieldNativeEditPreparationTests
{
    /// <summary>Verifies common and Starfield root commands preserve order, duplicates, null links, and typed native header behavior.</summary>
    /// <returns>A task that completes after all native test state is released.</returns>
    [Fact]
    public async Task ApplyEdit_MutatesEveryRootCategoryAndReportsNoOps()
    {
        using var fixture = StarfieldNativeTestFixture.Create();
        await using var sources = await OpenSourcesAsync(fixture);
        var outputService = new StarfieldNativeOutputService(new NativeOutputInputLoader());
        await using var output = await OpenNewOutputAsync(fixture, sources, outputService, "RootEdits.esp");
        await using var candidate = outputService.Clone(output);
        var target = BeginNew(sources, candidate, outputService);
        var editService = new StarfieldNativeEditService();

        var editor = editService.PrepareEdit(new SetEditorIdEdit("EditedList"));
        Apply(editService, sources, candidate, target, editor).Changed.ShouldBeTrue();
        Apply(editService, sources, candidate, target, editor).Changed.ShouldBeFalse();
        Apply(editService, sources, candidate, target, editService.PrepareEdit(new SetVersionControlEdit(0x01020304))).Changed.ShouldBeTrue();
        Apply(editService, sources, candidate, target, editService.PrepareEdit(new SetFormVersionEdit(131))).Changed.ShouldBeTrue();
        Apply(editService, sources, candidate, target, editService.PrepareEdit(new SetVersion2Edit(7))).Changed.ShouldBeTrue();
        Apply(editService, sources, candidate, target, editService.PrepareEdit(new SetCompressedEdit(true))).Changed.ShouldBeTrue();
        Apply(editService, sources, candidate, target, editService.PrepareEdit(new SetDeletedEdit(true))).Changed.ShouldBeTrue();
        Apply(
            editService,
            sources,
            candidate,
            target,
            editService.PrepareEdit(new StarfieldSetMajorFlagsEdit(
                StarfieldMajorRecord.StarfieldMajorRecordFlag.Compressed |
                StarfieldMajorRecord.StarfieldMajorRecordFlag.Deleted |
                StarfieldMajorRecord.StarfieldMajorRecordFlag.InitiallyDisabled))).Changed.ShouldBeTrue();

        var name = new TranslatedString(Language.English, string.Empty);
        name.Set(Language.French, "Nom français");
        Apply(editService, sources, candidate, target, editService.PrepareEdit(new StarfieldSetNameEdit(name))).Changed.ShouldBeTrue();
        Apply(
            editService,
            sources,
            candidate,
            target,
            editService.PrepareEdit(new StarfieldSetAddToListEdit(fixture.SourceListFormKey))).Changed.ShouldBeTrue();
        Apply(
            editService,
            sources,
            candidate,
            target,
            editService.PrepareEdit(new ReplaceItemsEdit(
                [fixture.KeywordFormKey, fixture.KeywordFormKey, FormKey.Null]))).Changed.ShouldBeTrue();
        Apply(editService, sources, candidate, target, editService.PrepareEdit(new MoveItemEdit(2, 0))).Changed.ShouldBeTrue();
        Apply(editService, sources, candidate, target, editService.PrepareEdit(new InsertItemEdit(1, fixture.BookFormKey))).Changed.ShouldBeTrue();
        Apply(editService, sources, candidate, target, editService.PrepareEdit(new RemoveItemEdit(2))).Changed.ShouldBeTrue();

        var snapshot = candidate.CreateSnapshot().FormLists.ShouldHaveSingleItem();
        snapshot.EditorID.ShouldBe("EditedList");
        snapshot.VersionControl.ShouldBe(0x01020304U);
        snapshot.FormVersion.ShouldBe((ushort)131);
        snapshot.Version2.ShouldBe((ushort)7);
        snapshot.IsCompressed.ShouldBeTrue();
        snapshot.IsDeleted.ShouldBeTrue();
        snapshot.StarfieldMajorRecordFlags.ShouldBe(
            StarfieldMajorRecord.StarfieldMajorRecordFlag.Compressed |
            StarfieldMajorRecord.StarfieldMajorRecordFlag.Deleted |
            StarfieldMajorRecord.StarfieldMajorRecordFlag.InitiallyDisabled);
        snapshot.Name!.String.ShouldBe(string.Empty);
        snapshot.Name.TryLookup(Language.French, out var french).ShouldBeTrue();
        french.ShouldBe("Nom français");
        snapshot.AddToList.FormKey.ShouldBe(fixture.SourceListFormKey);
        snapshot.Items.Select(item => item.FormKeyNullable).ShouldBe(
            new FormKey?[] { FormKey.Null, fixture.BookFormKey, fixture.KeywordFormKey });

        Apply(editService, sources, candidate, target, editService.PrepareEdit(new ClearEditorIdEdit())).Changed.ShouldBeTrue();
        Apply(editService, sources, candidate, target, editService.PrepareEdit(new StarfieldClearNameEdit())).Changed.ShouldBeTrue();
        Apply(editService, sources, candidate, target, editService.PrepareEdit(new StarfieldClearAddToListEdit())).Changed.ShouldBeTrue();
        Apply(editService, sources, candidate, target, editService.PrepareEdit(new ClearItemsEdit())).Changed.ShouldBeTrue();
        var cleared = candidate.CreateSnapshot().FormLists.ShouldHaveSingleItem();
        cleared.EditorID.ShouldBeNull();
        cleared.Name.ShouldBeNull();
        cleared.AddToList.IsNull.ShouldBeTrue();
        cleared.Items.ShouldBeEmpty();
    }

    /// <summary>Verifies complete concrete component and conditional-entry payloads are copied, replaced, removed, and cleared.</summary>
    /// <returns>A task that completes after all native test state is released.</returns>
    [Fact]
    public async Task PrepareAndApply_DeepCopiesNestedPayloadsAndSupportsStructuralMutations()
    {
        using var fixture = StarfieldNativeTestFixture.Create();
        await using var sources = await OpenSourcesAsync(fixture);
        var outputService = new StarfieldNativeOutputService(new NativeOutputInputLoader());
        await using var output = await OpenNewOutputAsync(fixture, sources, outputService, "NestedEdits.esp");
        await using var candidate = outputService.Clone(output);
        var target = BeginNew(sources, candidate, outputService);
        var editService = new StarfieldNativeEditService();

        var component = CreateComponent(FormKey.Null, 1.25f);
        var preparedAdd = editService.PrepareEdit(new StarfieldAddComponentEdit(0, component));
        component.Properties.ShouldNotBeNull()[0].Value = 99.0f;
        Apply(editService, sources, candidate, target, preparedAdd).Changed.ShouldBeTrue();
        var appliedComponent = candidate.CreateSnapshot().FormLists.Single().Components
            .ShouldHaveSingleItem().ShouldBeOfType<PropertySheetComponent>();
        appliedComponent.Properties.ShouldNotBeNull()[0].Value.ShouldBe(1.25f);

        var replacement = CreateComponent(FormKey.Null, -0.0f);
        Apply(
            editService,
            sources,
            candidate,
            target,
            editService.PrepareEdit(new StarfieldReplaceComponentEdit(0, replacement))).Changed.ShouldBeTrue();
        var invalidReplace = editService.ApplyEdit(
            sources,
            candidate,
            target,
            editService.PrepareEdit(new StarfieldReplaceComponentEdit(4, replacement)));
        invalidReplace.Succeeded.ShouldBeFalse();
        invalidReplace.Error!.Code.ShouldBe(EngineErrorCode.ValidationFailed);
        candidate.CreateSnapshot().FormLists.Single().Components.Count.ShouldBe(1);

        Apply(
            editService,
            sources,
            candidate,
            target,
            editService.PrepareEdit(new StarfieldReplaceComponentsEdit(
                [CreateComponent(FormKey.Null, 2.0f), CreateComponent(FormKey.Null, 3.0f)]))).Changed.ShouldBeTrue();
        Apply(editService, sources, candidate, target, editService.PrepareEdit(new StarfieldRemoveComponentEdit(0))).Changed.ShouldBeTrue();
        candidate.CreateSnapshot().FormLists.Single().Components.Count.ShouldBe(1);

        var nullConditions = new FormListConditionalEntry
        {
            Index = null,
            Conditions = null
        };
        var populatedConditions = CreateConditionalEntry(7);
        var preparedConditions = editService.PrepareEdit(new StarfieldSetConditionalEntriesEdit(
            [nullConditions, populatedConditions]));
        nullConditions.Index = 42;
        populatedConditions.Index = 99;
        Apply(editService, sources, candidate, target, preparedConditions).Changed.ShouldBeTrue();
        var entries = candidate.CreateSnapshot().FormLists.Single().ConditionalEntries;
        entries.Count.ShouldBe(2);
        entries[0].Index.ShouldBeNull();
        entries[0].Conditions.ShouldBeNull();
        entries[1].Index.ShouldBe(7U);
        entries[1].Conditions.ShouldNotBeNull().ShouldHaveSingleItem().ShouldBeOfType<ConditionFloat>();

        Apply(
            editService,
            sources,
            candidate,
            target,
            editService.PrepareEdit(new StarfieldClearConditionalEntriesEdit())).Changed.ShouldBeTrue();
        candidate.CreateSnapshot().FormLists.Single().ConditionalEntries.ShouldBeEmpty();
    }

    /// <summary>Verifies new links resolve output-first, AddToList enforces the FormList family, and failed validation leaves the candidate unchanged.</summary>
    /// <returns>A task that completes after all native test state is released.</returns>
    [Fact]
    public async Task ApplyEdit_ValidatesOnlyIntroducedReferencesBeforeMutation()
    {
        using var fixture = StarfieldNativeTestFixture.Create();
        await using var sources = await OpenSourcesAsync(fixture);
        var outputService = new StarfieldNativeOutputService(new NativeOutputInputLoader());
        await using var output = await OpenNewOutputAsync(fixture, sources, outputService, "References.esp");
        await using var candidate = outputService.Clone(output);
        var target = BeginNew(sources, candidate, outputService);
        var editService = new StarfieldNativeEditService();

        var missingItem = editService.ApplyEdit(
            sources,
            candidate,
            target,
            editService.PrepareEdit(new InsertItemEdit(0, fixture.MissingFormKey)));
        missingItem.Succeeded.ShouldBeFalse();
        missingItem.Error!.Code.ShouldBe(EngineErrorCode.ValidationFailed);
        candidate.CreateSnapshot().FormLists.Single().Items.ShouldBeEmpty();

        var wrongAddToList = editService.ApplyEdit(
            sources,
            candidate,
            target,
            editService.PrepareEdit(new StarfieldSetAddToListEdit(fixture.BookFormKey)));
        wrongAddToList.Succeeded.ShouldBeFalse();
        wrongAddToList.Error!.Message.ShouldContain(nameof(IFormListGetter));
        candidate.CreateSnapshot().FormLists.Single().AddToList.IsNull.ShouldBeTrue();

        var missingNested = editService.ApplyEdit(
            sources,
            candidate,
            target,
            editService.PrepareEdit(new StarfieldAddComponentEdit(
                0,
                CreateComponent(fixture.MissingFormKey, 1.0f))));
        missingNested.Succeeded.ShouldBeFalse();
        candidate.CreateSnapshot().FormLists.Single().Components.ShouldBeEmpty();

        var keywordSlots = new AttachParentArrayComponent
        {
            Slots = new ExtendedList<IFormLinkGetter<IKeywordGetter>>()
        };
        keywordSlots.Slots.Add(new FormLink<IKeywordGetter>(fixture.BookFormKey));
        var liveWrongFamily = editService.ApplyEdit(
            sources,
            candidate,
            target,
            editService.PrepareEdit(new StarfieldAddComponentEdit(0, keywordSlots)));
        liveWrongFamily.Succeeded.ShouldBeFalse();
        liveWrongFamily.Error!.Message.ShouldContain(nameof(IKeywordGetter));
        candidate.CreateSnapshot().FormLists.Single().Components.ShouldBeEmpty();

        Apply(
            editService,
            sources,
            candidate,
            target,
            editService.PrepareEdit(new InsertItemEdit(0, target))).Changed.ShouldBeTrue();
        candidate.CreateSnapshot().FormLists.Single().Items.Single().FormKey.ShouldBe(target);
    }

    /// <summary>Verifies canonical identities normalize native identity spelling while retaining ordinary text and malformed code units exactly.</summary>
    [Fact]
    public void PrepareEdit_CanonicalizesIdentityAndRetainsExactPayloadDifferences()
    {
        var service = new StarfieldNativeEditService();
        var firstIdentity = new FormKey(ModKey.FromNameAndExtension("MixedCase.esm"), 0x00000800);
        var secondIdentity = new FormKey(ModKey.FromNameAndExtension("mixedcase.esm"), 0x00000800);

        service.PrepareEdit(new InsertItemEdit(0, firstIdentity)).Fingerprint
            .ShouldBe(service.PrepareEdit(new InsertItemEdit(0, secondIdentity)).Fingerprint);
        service.PrepareEdit(new SetEditorIdEdit("CaseSensitive")).Fingerprint
            .ShouldNotBe(service.PrepareEdit(new SetEditorIdEdit("casesensitive")).Fingerprint);

        var malformedHigh = new TranslatedString(Language.English, "\uD800");
        var malformedLow = new TranslatedString(Language.English, "\uDC00");
        var highPrepared = service.PrepareEdit(new StarfieldSetNameEdit(malformedHigh));
        var lowPrepared = service.PrepareEdit(new StarfieldSetNameEdit(malformedLow));
        highPrepared.IsValid.ShouldBeFalse();
        highPrepared.Error!.Code.ShouldBe(EngineErrorCode.ValidationFailed);
        lowPrepared.IsValid.ShouldBeFalse();
        highPrepared.Fingerprint.ShouldNotBe(lowPrepared.Fingerprint);

        var invalidHighBitFlags = service.PrepareEdit(new StarfieldSetMajorFlagsEdit(
            (StarfieldMajorRecord.StarfieldMajorRecordFlag)unchecked((int)0x80000000)));
        invalidHighBitFlags.IsValid.ShouldBeFalse();
        invalidHighBitFlags.Error!.Code.ShouldBe(EngineErrorCode.ValidationFailed);
        var invalidDistinctHighBitFlags = service.PrepareEdit(new StarfieldSetMajorFlagsEdit(
            (StarfieldMajorRecord.StarfieldMajorRecordFlag)unchecked((int)0xC0000000)));
        invalidDistinctHighBitFlags.IsValid.ShouldBeFalse();
        invalidDistinctHighBitFlags.Error!.Code.ShouldBe(EngineErrorCode.ValidationFailed);
        invalidDistinctHighBitFlags.Fingerprint.ShouldNotBe(invalidHighBitFlags.Fingerprint);
    }

    /// <summary>Applies one prepared edit and requires a successful native mutation result.</summary>
    /// <param name="service">The Starfield edit service.</param>
    /// <param name="sources">The borrowed immutable source set.</param>
    /// <param name="candidate">The unpublished output candidate.</param>
    /// <param name="target">The exact target FormKey.</param>
    /// <param name="edit">The prepared Starfield command.</param>
    /// <returns>The changed-state result.</returns>
    private static NativeEditMutationResult Apply(
        StarfieldNativeEditService service,
        StarfieldNativeSourceSet sources,
        StarfieldNativeOutputState candidate,
        FormKey target,
        PreparedFormListEdit edit)
    {
        var result = service.ApplyEdit(sources, candidate, target, edit);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value!;
    }

    /// <summary>Begins one new FormList and returns its unique staged output identity.</summary>
    /// <param name="sources">The borrowed immutable source set.</param>
    /// <param name="candidate">The unpublished output candidate.</param>
    /// <param name="service">The Starfield output service.</param>
    /// <returns>The allocated FormKey.</returns>
    private static FormKey BeginNew(
        StarfieldNativeSourceSet sources,
        StarfieldNativeOutputState candidate,
        StarfieldNativeOutputService service)
    {
        var result = service.BeginEdit(
            sources,
            candidate,
            new BeginEditRequest(
                Guid.NewGuid(),
                new WorkspaceRevision(Guid.NewGuid(), 1),
                FormListEditRole.New),
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value!.FormKey;
    }

    /// <summary>Creates one representative nested component with a typed native link and exact floating value.</summary>
    /// <param name="actorValue">The native link identity retained by the component.</param>
    /// <param name="value">The exact floating property value.</param>
    /// <returns>A mutable caller-owned component payload.</returns>
    private static PropertySheetComponent CreateComponent(FormKey actorValue, float value)
    {
        return new PropertySheetComponent
        {
            Properties = new ExtendedList<ObjectProperty>
            {
                new ObjectProperty
                {
                    ActorValue = new FormLink<IActorValueInformationGetter>(actorValue),
                    Value = value
                }
            }
        };
    }

    /// <summary>Creates one conditional entry with a complete concrete condition graph.</summary>
    /// <param name="index">The retained optional entry index.</param>
    /// <returns>A mutable caller-owned conditional entry.</returns>
    private static FormListConditionalEntry CreateConditionalEntry(uint index)
    {
        var data = new BiomeHasKeywordConditionData();
        data.FirstParameter.Index = 17;
        return new FormListConditionalEntry
        {
            Index = index,
            Conditions = new ExtendedList<Condition>
            {
                new ConditionFloat
                {
                    ComparisonValue = 1.0f,
                    Data = data
                }
            }
        };
    }

    /// <summary>Opens the deterministic generated Starfield source set.</summary>
    /// <param name="fixture">The generated native test fixture.</param>
    /// <returns>The independently owned source set.</returns>
    private static async Task<StarfieldNativeSourceSet> OpenSourcesAsync(StarfieldNativeTestFixture fixture)
    {
        var result = await new StarfieldNativeSourceLoader(new NativeSourceInputLoader()).OpenAsync(
            fixture.CreateOpenRequest(),
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value!.Sources.ShouldBeOfType<StarfieldNativeSourceSet>();
    }

    /// <summary>Opens a new in-memory output without creating its destination file.</summary>
    /// <param name="fixture">The generated fixture whose root owns the absent destination.</param>
    /// <param name="sources">The borrowed immutable source set.</param>
    /// <param name="service">The Starfield output service.</param>
    /// <param name="fileName">The absent output plugin name.</param>
    /// <returns>The independently owned complete output state.</returns>
    private static async Task<StarfieldNativeOutputState> OpenNewOutputAsync(
        StarfieldNativeTestFixture fixture,
        StarfieldNativeSourceSet sources,
        StarfieldNativeOutputService service,
        string fileName)
    {
        var directory = fixture.RootDirectory.CreateSubdirectory(Path.GetFileNameWithoutExtension(fileName));
        var path = Path.Combine(directory.FullName, fileName);
        var association = new OutputAssociation(
            path,
            ModKey.FromNameAndExtension(fileName),
            LocalizedOutputMode.Embedded,
            OutputMasterStyle.Full);
        var result = await service.OpenAsync(
            sources,
            new SelectOutputRequest(Guid.NewGuid(), sources.Revision, OutputSelectionMode.CreateNew, association),
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value!.Output.ShouldBeOfType<StarfieldNativeOutputState>();
    }
}
