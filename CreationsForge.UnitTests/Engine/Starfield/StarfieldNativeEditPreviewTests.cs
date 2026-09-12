using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInputs;
using CreationsForge.Core.Engine.NativeOutputs;
using CreationsForge.Starfield.Native;
using CreationsForge.Starfield.Native.Edits;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Starfield;
using Noggog;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Starfield;

/// <summary>Verifies Starfield first-baseline provenance, exact-context previews, warnings, cloning, and fresh-state discard behavior.</summary>
public sealed class StarfieldNativeEditPreviewTests
{
    /// <summary>Verifies exact chosen source contexts remain the before-view after edit reuse and candidate cloning.</summary>
    /// <returns>A task that completes after all native state is released.</returns>
    [Fact]
    public async Task Preview_PreservesFirstExactSourceAndAbsentBaselinesAcrossReuseAndClone()
    {
        using var fixture = StarfieldNativeTestFixture.Create();
        await using var sources = await OpenSourcesAsync(fixture);
        var outputService = new StarfieldNativeOutputService(new NativeOutputInputLoader());
        var editService = new StarfieldNativeEditService();
        await using var output = await OpenNewOutputAsync(fixture, sources, outputService, "SourcePreview.esp");
        await using var candidate = outputService.Clone(output);

        var sourceTarget = Begin(
            sources,
            candidate,
            outputService,
            FormListEditRole.Override,
            origin: fixture.SourceListFormKey,
            selection: new ReferenceRequest(
                fixture.SourceListFormKey,
                RecordScope.AllContexts,
                fixture.SourceModKey));
        Apply(editService, sources, candidate, sourceTarget, new SetEditorIdEdit("First staged value"));
        Begin(
            sources,
            candidate,
            outputService,
            FormListEditRole.ExistingOutput,
            target: sourceTarget);
        Apply(editService, sources, candidate, sourceTarget, new SetEditorIdEdit("Second staged value"));

        var newTarget = Begin(sources, candidate, outputService, FormListEditRole.New);
        Apply(editService, sources, candidate, newTarget, new SetEditorIdEdit("New staged value"));

        var warningTarget = Begin(
            sources,
            candidate,
            outputService,
            FormListEditRole.Override,
            origin: fixture.WarningListFormKey,
            selection: new ReferenceRequest(
                fixture.WarningListFormKey,
                RecordScope.AllContexts,
                fixture.SourceModKey));
        Apply(editService, sources, candidate, warningTarget, new SetVersionControlEdit(42));

        var preview = editService.Preview(sources, candidate);
        preview.Succeeded.ShouldBeTrue(preview.Error?.Message);
        preview.Value!.Comparisons.Select(comparison => comparison.FormKey)
            .ShouldBe(new[] { sourceTarget, newTarget, warningTarget });
        var sourceComparison = preview.Value.Comparisons[0];
        sourceComparison.BeforeContext.Selection.Scope.ShouldBe(RecordScope.AllContexts);
        sourceComparison.BeforeContext.Selection.ContainingModKey.ShouldBe(fixture.SourceModKey);
        sourceComparison.BeforeContext.ContainingModKey.ShouldBe(fixture.SourceModKey);
        sourceComparison.Before!.Value.GetProperty(nameof(IMajorRecordGetter.EditorID)).GetString()
            .ShouldBe("SharedListSmall");
        sourceComparison.After!.Value.GetProperty(nameof(IMajorRecordGetter.EditorID)).GetString()
            .ShouldBe("Second staged value");
        preview.Value.Comparisons[1].Before.ShouldBeNull();
        preview.Value.Comparisons[1].BeforeContext.Status.ShouldBe(ReferenceResolutionStatus.Unresolved);
        preview.Value.Comparisons[1].After!.Value.GetProperty(nameof(IMajorRecordGetter.EditorID)).GetString()
            .ShouldBe("New staged value");
        preview.Value.UnresolvedReferenceCount.ShouldBeGreaterThan(0);
        preview.Value.Warnings.ShouldContain(warning => warning.Code == "missing-native-reference");

        await using var clone = outputService.Clone(candidate);
        var clonePreview = editService.Preview(sources, clone);
        clonePreview.Succeeded.ShouldBeTrue(clonePreview.Error?.Message);
        clonePreview.Value!.Comparisons.Select(comparison => comparison.FormKey)
            .ShouldBe(preview.Value.Comparisons.Select(comparison => comparison.FormKey));
        clonePreview.Value.Comparisons[0].Before!.Value.GetProperty(nameof(IMajorRecordGetter.EditorID)).GetString()
            .ShouldBe("SharedListSmall");

        var uneditedPreview = editService.Preview(sources, output);
        uneditedPreview.Succeeded.ShouldBeTrue(uneditedPreview.Error?.Message);
        uneditedPreview.Value!.Comparisons.ShouldBeEmpty();
        uneditedPreview.Value.UnresolvedReferenceCount.ShouldBe(0);
    }

    /// <summary>Verifies original-output and deleted baselines are reconstructed exactly and reopening starts with no provenance.</summary>
    /// <returns>A task that completes after original, candidate, and reopened state are released.</returns>
    [Fact]
    public async Task Preview_UsesOriginalOutputBaselinesAndFreshReopenClearsProvenance()
    {
        using var fixture = StarfieldNativeTestFixture.Create();
        await using var sources = await OpenSourcesAsync(fixture);
        var outputDirectory = fixture.RootDirectory.CreateSubdirectory("OriginalPreview");
        FormKey liveTarget = default;
        FormKey deletedTarget = default;
        var path = WriteExistingOutput(
            outputDirectory,
            "OriginalPreview.esp",
            mod =>
            {
                var missingOutputKey = new FormKey(mod.ModKey, 0x00F00000);
                var live = new FormList(mod, "Original live")
                {
                    MajorRecordFlagsRaw = 0x40000000
                };
                live.Items.Add(new FormLink<IStarfieldMajorRecordGetter>(missingOutputKey));
                mod.FormLists.Add(live);
                liveTarget = live.FormKey;
                var deleted = new FormList(mod, "Original deleted")
                {
                    IsDeleted = true
                };
                mod.FormLists.Add(deleted);
                deletedTarget = deleted.FormKey;
            });
        var association = CreateAssociation(path);
        var outputService = new StarfieldNativeOutputService(new NativeOutputInputLoader());
        var editService = new StarfieldNativeEditService();
        var open = await outputService.OpenAsync(
            sources,
            new SelectOutputRequest(Guid.NewGuid(), sources.Revision, OutputSelectionMode.OpenExisting, association),
            TestContext.Current.CancellationToken);
        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        await using var output = open.Value!.Output.ShouldBeOfType<StarfieldNativeOutputState>();
        await using var candidate = outputService.Clone(output);

        Begin(sources, candidate, outputService, FormListEditRole.ExistingOutput, target: liveTarget);
        Begin(sources, candidate, outputService, FormListEditRole.ExistingOutput, target: deletedTarget);
        Apply(editService, sources, candidate, liveTarget, new SetEditorIdEdit("Edited live"));
        Apply(
            editService,
            sources,
            candidate,
            liveTarget,
            new StarfieldSetMajorFlagsEdit(StarfieldMajorRecord.StarfieldMajorRecordFlag.InitiallyDisabled));
        Apply(editService, sources, candidate, deletedTarget, new SetDeletedEdit(false));

        var preview = editService.Preview(sources, candidate);
        preview.Succeeded.ShouldBeTrue(preview.Error?.Message);
        preview.Value!.Comparisons.Count.ShouldBe(2);
        var live = preview.Value.Comparisons.Single(comparison => comparison.FormKey == liveTarget);
        live.BeforeContext.Selection.Scope.ShouldBe(RecordScope.StagedOutput);
        live.BeforeContext.Role.ShouldBe(PluginRole.Output);
        live.Before!.Value.GetProperty(nameof(IMajorRecordGetter.EditorID)).GetString().ShouldBe("Original live");
        live.After!.Value.GetProperty(nameof(IMajorRecordGetter.EditorID)).GetString().ShouldBe("Edited live");
        candidate.CreateSnapshot().FormLists.Single(record => record.FormKey == liveTarget)
            .MajorRecordFlagsRaw.ShouldBe(0x40000800);
        live.Warnings.ShouldContain(warning => warning.Code == "missing-native-reference");
        var deleted = preview.Value.Comparisons.Single(comparison => comparison.FormKey == deletedTarget);
        deleted.BeforeContext.Status.ShouldBe(ReferenceResolutionStatus.Deleted);
        deleted.AfterContext.Status.ShouldBe(ReferenceResolutionStatus.Resolved);

        var reopen = await outputService.ReopenAsync(
            sources,
            open.Value.Association,
            open.Value.Baseline,
            TestContext.Current.CancellationToken);
        reopen.Succeeded.ShouldBeTrue(reopen.Error?.Message);
        await using var reopened = reopen.Value!.Output.ShouldBeOfType<StarfieldNativeOutputState>();
        var reopenedPreview = editService.Preview(sources, reopened);
        reopenedPreview.Succeeded.ShouldBeTrue(reopenedPreview.Error?.Message);
        reopenedPreview.Value!.Comparisons.ShouldBeEmpty();
    }

    /// <summary>Verifies replacement validation subtracts retained unresolved multiplicities and move changed-state follows native semantics.</summary>
    /// <returns>A task that completes after the native fixture and candidate are released.</returns>
    [Fact]
    public async Task ApplyEdit_RetainsExistingUnresolvedLinksButRejectsNewMultiplicity()
    {
        using var fixture = StarfieldNativeTestFixture.Create();
        await using var sources = await OpenSourcesAsync(fixture);
        var outputDirectory = fixture.RootDirectory.CreateSubdirectory("RetainedReferences");
        FormKey unresolvedTarget = default;
        FormKey duplicateTarget = default;
        FormKey expectedTypeTarget = default;
        FormKey localBookTarget = default;
        FormKey missingOutputTarget = default;
        var path = WriteExistingOutput(
            outputDirectory,
            "RetainedReferences.esp",
            mod =>
            {
                var localBook = new Book(mod, "LocalBook");
                mod.Books.Add(localBook);
                localBookTarget = localBook.FormKey;
                var localKeyword = new Keyword(mod, "LocalKeyword");
                mod.Keywords.Add(localKeyword);
                var missingOutputKey = new FormKey(mod.ModKey, 0x00F00000);
                missingOutputTarget = missingOutputKey;
                var unresolved = new FormList(mod, "Unresolved");
                unresolved.Items.Add(new FormLink<IStarfieldMajorRecordGetter>(missingOutputKey));
                mod.FormLists.Add(unresolved);
                unresolvedTarget = unresolved.FormKey;
                var duplicates = new FormList(mod, "Duplicates");
                duplicates.Items.Add(new FormLink<IStarfieldMajorRecordGetter>(localKeyword.FormKey));
                duplicates.Items.Add(new FormLink<IStarfieldMajorRecordGetter>(localKeyword.FormKey));
                mod.FormLists.Add(duplicates);
                duplicateTarget = duplicates.FormKey;
                var expectedType = new FormList(mod, "ExpectedTypeCredit");
                var keywordSlots = new AttachParentArrayComponent
                {
                    Slots = new ExtendedList<IFormLinkGetter<IKeywordGetter>>()
                };
                keywordSlots.Slots.Add(new FormLink<IKeywordGetter>(localBook.FormKey));
                expectedType.Components.Add(keywordSlots);
                mod.FormLists.Add(expectedType);
                expectedTypeTarget = expectedType.FormKey;
            });
        var outputService = new StarfieldNativeOutputService(new NativeOutputInputLoader());
        var open = await outputService.OpenAsync(
            sources,
            new SelectOutputRequest(
                Guid.NewGuid(),
                sources.Revision,
                OutputSelectionMode.OpenExisting,
                CreateAssociation(path)),
            TestContext.Current.CancellationToken);
        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        await using var output = open.Value!.Output.ShouldBeOfType<StarfieldNativeOutputState>();
        await using var candidate = outputService.Clone(output);
        Begin(sources, candidate, outputService, FormListEditRole.ExistingOutput, target: unresolvedTarget);
        Begin(sources, candidate, outputService, FormListEditRole.ExistingOutput, target: duplicateTarget);
        Begin(sources, candidate, outputService, FormListEditRole.ExistingOutput, target: expectedTypeTarget);
        var editService = new StarfieldNativeEditService();

        var retained = editService.ApplyEdit(
            sources,
            candidate,
            unresolvedTarget,
            editService.PrepareEdit(new ReplaceItemsEdit([missingOutputTarget])));
        retained.Succeeded.ShouldBeTrue(retained.Error?.Message);
        retained.Value!.Changed.ShouldBeFalse();

        var newMultiplicity = editService.ApplyEdit(
            sources,
            candidate,
            unresolvedTarget,
            editService.PrepareEdit(new ReplaceItemsEdit([missingOutputTarget, missingOutputTarget])));
        newMultiplicity.Succeeded.ShouldBeFalse();
        candidate.CreateSnapshot().FormLists.Single(record => record.FormKey == unresolvedTarget)
            .Items.ShouldHaveSingleItem();

        var duplicateMove = editService.ApplyEdit(
            sources,
            candidate,
            duplicateTarget,
            editService.PrepareEdit(new MoveItemEdit(0, 1)));
        duplicateMove.Succeeded.ShouldBeTrue(duplicateMove.Error?.Message);
        duplicateMove.Value!.Changed.ShouldBeFalse();

        var replacement = new PropertySheetComponent
        {
            Properties = new ExtendedList<ObjectProperty>
            {
                new ObjectProperty
                {
                    ActorValue = new FormLink<IActorValueInformationGetter>(localBookTarget),
                    Value = 1.0f
                }
            }
        };
        var changedExpectedType = editService.ApplyEdit(
            sources,
            candidate,
            expectedTypeTarget,
            editService.PrepareEdit(new StarfieldReplaceComponentEdit(0, replacement)));
        changedExpectedType.Succeeded.ShouldBeFalse();
        candidate.CreateSnapshot().FormLists.Single(record => record.FormKey == expectedTypeTarget)
            .Components.ShouldHaveSingleItem().ShouldBeOfType<AttachParentArrayComponent>();

        var preview = editService.Preview(sources, candidate);
        preview.Succeeded.ShouldBeTrue(preview.Error?.Message);
        preview.Value!.UnresolvedReferenceCount.ShouldBe(1);
        preview.Value.Warnings.ShouldContain(warning => warning.Code == "missing-native-reference");
    }

    /// <summary>Begins one staged edit through the exact requested native role.</summary>
    /// <param name="sources">The borrowed immutable source set.</param>
    /// <param name="candidate">The unpublished complete output candidate.</param>
    /// <param name="service">The Starfield output service.</param>
    /// <param name="role">The requested new, override, or existing-output role.</param>
    /// <param name="origin">The source origin for an override.</param>
    /// <param name="selection">The optional exact source context selector.</param>
    /// <param name="target">The exact existing-output target.</param>
    /// <returns>The selected or allocated output FormKey.</returns>
    private static FormKey Begin(
        StarfieldNativeSourceSet sources,
        StarfieldNativeOutputState candidate,
        StarfieldNativeOutputService service,
        FormListEditRole role,
        FormKey? origin = null,
        ReferenceRequest? selection = null,
        FormKey? target = null)
    {
        var result = service.BeginEdit(
            sources,
            candidate,
            new BeginEditRequest(
                Guid.NewGuid(),
                new WorkspaceRevision(Guid.NewGuid(), 1),
                role,
                origin,
                selection,
                target),
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value!.FormKey;
    }

    /// <summary>Prepares and applies one command, requiring a successful result.</summary>
    /// <param name="service">The Starfield edit service.</param>
    /// <param name="sources">The borrowed immutable source set.</param>
    /// <param name="candidate">The unpublished complete output candidate.</param>
    /// <param name="target">The exact edited FormKey.</param>
    /// <param name="command">The caller-owned typed command.</param>
    private static void Apply(
        StarfieldNativeEditService service,
        StarfieldNativeSourceSet sources,
        StarfieldNativeOutputState candidate,
        FormKey target,
        FormListEdit command)
    {
        var result = service.ApplyEdit(sources, candidate, target, service.PrepareEdit(command));
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
    }

    /// <summary>Writes one deterministic complete existing native output fixture.</summary>
    /// <param name="directory">The existing task-owned parent directory.</param>
    /// <param name="fileName">The native output plugin file name.</param>
    /// <param name="configure">The typed native record builder.</param>
    /// <returns>The written plugin path.</returns>
    private static string WriteExistingOutput(
        DirectoryInfo directory,
        string fileName,
        Action<StarfieldMod> configure)
    {
        var mod = new StarfieldMod(ModKey.FromNameAndExtension(fileName), StarfieldRelease.Starfield);
        configure(mod);
        var path = Path.Combine(directory.FullName, fileName);
        ((IModGetter)mod).WriteToBinary(path, BinaryWriteParameters.Default);
        return path;
    }

    /// <summary>Creates the exact full-master embedded output association for one fixture path.</summary>
    /// <param name="path">The canonicalizable output plugin path.</param>
    /// <returns>The immutable output association.</returns>
    private static OutputAssociation CreateAssociation(string path)
    {
        return new OutputAssociation(
            path,
            ModKey.FromNameAndExtension(Path.GetFileName(path)),
            LocalizedOutputMode.Embedded,
            OutputMasterStyle.Full);
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
        var result = await service.OpenAsync(
            sources,
            new SelectOutputRequest(
                Guid.NewGuid(),
                sources.Revision,
                OutputSelectionMode.CreateNew,
                CreateAssociation(path)),
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value!.Output.ShouldBeOfType<StarfieldNativeOutputState>();
    }
}
