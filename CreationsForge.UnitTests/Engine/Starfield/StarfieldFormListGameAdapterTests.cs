using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.PluginInputs;
using CreationsForge.Core.Engine.PluginOutputs;
using CreationsForge.Starfield.PluginAdapter;
using CreationsForge.Starfield.PluginAdapter.Edits;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Strings;
using Noggog;
using Shouldly;
using System.IO.Abstractions;

namespace CreationsForge.UnitTests.Engine.Starfield;

/// <summary>Verifies complete Starfield adapter composition, output-aware reads, and private staged-write preservation.</summary>
public sealed class StarfieldFormListGameAdapterTests
{
    /// <summary>Selected output participates in plugin enumeration, staged enumeration, and winning contextual reads.</summary>
    /// <returns>A task that completes after generated plugin lifetimes are released.</returns>
    [Fact]
    public async Task AdapterReads_IncludeSelectedOutputWithoutMutatingArtifacts()
    {
        using var fixture = StarfieldPluginTestFixture.Create();
        var sourceArtifacts = fixture.SnapshotArtifacts();
        var outputDirectory = fixture.RootDirectory.CreateSubdirectory("AdapterReads");
        FormKey outputFormKey = default;
        var outputPath = WriteOutput(
            outputDirectory,
            "AdapterReads.esp",
            OutputMasterStyle.Full,
            localized: false,
            mod =>
            {
                var outputList = new FormList(fixture.SourceListFormKey, StarfieldRelease.Starfield)
                {
                    EditorID = "AdapterWinningOverride",
                };
                mod.FormLists.Add(outputList);
                outputFormKey = outputList.FormKey;
            },
            [new KeyedMasterStyle(fixture.SourceModKey, MasterStyle.Small)]);
        var destinationBytes = File.ReadAllBytes(outputPath);
        var adapter = CreateAdapter(out _);
        await using var sources = await OpenSourcesAsync(adapter, fixture);
        var association = CreateAssociation(outputPath, OutputMasterStyle.Full, LocalizedOutputMode.Embedded);
        var open = await adapter.OpenOutputAsync(
            sources,
            new SelectOutputRequest(Guid.NewGuid(), sources.Revision, OutputSelectionMode.OpenExisting, association),
            TestContext.Current.CancellationToken);

        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        await using var output = open.Value!.Output;
        var plugins = adapter.ListPlugins(sources, output, TestContext.Current.CancellationToken);
        plugins.Succeeded.ShouldBeTrue(plugins.Error?.Message);
        plugins.WorkspaceId.ShouldBe(sources.WorkspaceId);
        plugins.ResultRevision.ShouldBe(sources.Revision);
        plugins.Value![^1].ModKey.ShouldBe(association.ModKey);
        plugins.Value[^1].Role.ShouldBe(PluginRole.Output);

        var staged = adapter.ListFormLists(
            sources,
            output,
            RecordScope.StagedOutput,
            TestContext.Current.CancellationToken);
        staged.Succeeded.ShouldBeTrue(staged.Error?.Message);
        staged.Value!.ShouldHaveSingleItem().FormKey.ShouldBe(outputFormKey);
        staged.Value.ShouldAllBe(summary => summary.Role == PluginRole.Output);

        var winning = adapter.ReadFormListContext(
            sources,
            output,
            new ReferenceRequest(fixture.SourceListFormKey, RecordScope.WinningOverrides),
            TestContext.Current.CancellationToken);
        winning.Succeeded.ShouldBeTrue(winning.Error?.Message);
        winning.WorkspaceId.ShouldBe(sources.WorkspaceId);
        winning.Value!.Context.ContainingModKey.ShouldBe(association.ModKey);
        winning.Value.Context.Role.ShouldBe(PluginRole.Output);
        winning.Value.Record.ShouldBeOfType<FormList>().EditorID.ShouldBe("AdapterWinningOverride");

        AssertArtifactsUnchanged(sourceArtifacts);
        File.ReadAllBytes(outputPath).ShouldBe(destinationBytes);
    }

    /// <summary>An unchanged existing output returns the explicit zero-write result and leaves private staging empty.</summary>
    /// <returns>A task that completes after generated plugin lifetimes are released.</returns>
    [Fact]
    public async Task WriteAndValidateAsync_UnchangedExistingOutput_PerformsZeroArtifactWrites()
    {
        using var fixture = StarfieldPluginTestFixture.Create();
        var sourceArtifacts = fixture.SnapshotArtifacts();
        var outputDirectory = fixture.RootDirectory.CreateSubdirectory("UnchangedOutput");
        var outputPath = WriteOutput(
            outputDirectory,
            "UnchangedOutput.esp",
            OutputMasterStyle.Full,
            localized: false,
            mod => mod.FormLists.Add(new FormList(mod, "UnchangedList")));
        var destinationBytes = File.ReadAllBytes(outputPath);
        var stagingDirectory = fixture.RootDirectory.CreateSubdirectory("UnchangedStage");
        var adapter = CreateAdapter(out _);
        await using var sources = await OpenSourcesAsync(adapter, fixture);
        var association = CreateAssociation(outputPath, OutputMasterStyle.Full, LocalizedOutputMode.Embedded);
        var open = await adapter.OpenOutputAsync(
            sources,
            new SelectOutputRequest(Guid.NewGuid(), sources.Revision, OutputSelectionMode.OpenExisting, association),
            TestContext.Current.CancellationToken);

        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        await using var output = open.Value!.Output;
        var result = await adapter.WriteAndValidateAsync(
            sources,
            output,
            CreateWriteRequest(stagingDirectory.FullName, open.Value),
            TestContext.Current.CancellationToken);

        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        result.WorkspaceId.ShouldBe(sources.WorkspaceId);
        result.ResultRevision.ShouldBe(sources.Revision);
        result.Value!.Disposition.ShouldBe(PluginWriteDisposition.Unchanged);
        result.Value.StagedOutput.ShouldBeNull();
        result.Value.ArtifactMappings.ShouldBeEmpty();
        stagingDirectory.EnumerateFileSystemInfos().ShouldBeEmpty();
        AssertArtifactsUnchanged(sourceArtifacts);
        File.ReadAllBytes(outputPath).ShouldBe(destinationBytes);
    }

    /// <summary>An identical destination restored as another file identity rejects the stale baseline and stages the next edit with a fresh baseline.</summary>
    /// <returns>A task that completes after every independently opened plugin lifetime is released.</returns>
    [Fact]
    public async Task WriteAndValidateAsync_RestoredIdenticalDestination_RequiresFreshExpectedBaseline()
    {
        using var fixture = StarfieldPluginTestFixture.Create();
        var outputDirectory = fixture.RootDirectory.CreateSubdirectory("RestoredDestination");
        FormKey listKey = default;
        var outputPath = WriteOutput(
            outputDirectory,
            "RestoredDestination.esp",
            OutputMasterStyle.Full,
            localized: false,
            mod =>
            {
                var list = new FormList(mod, "RestoredList");
                mod.FormLists.Add(list);
                listKey = list.FormKey;
            });
        var association = CreateAssociation(outputPath, OutputMasterStyle.Full, LocalizedOutputMode.Embedded);
        var adapter = CreateAdapter(out _);
        await using var sources = await OpenSourcesAsync(adapter, fixture);
        var selected = await adapter.OpenOutputAsync(
            sources,
            new SelectOutputRequest(Guid.NewGuid(), sources.Revision, OutputSelectionMode.OpenExisting, association),
            TestContext.Current.CancellationToken);

        selected.Succeeded.ShouldBeTrue(selected.Error?.Message);
        await using var output = selected.Value!.Output;
        var replacementPath = Path.Combine(outputDirectory.FullName, "replacement.bin");
        File.Copy(outputPath, replacementPath);
        File.Move(replacementPath, outputPath, overwrite: true);

        var staleStage = fixture.RootDirectory.CreateSubdirectory("RestoredDestinationStaleStage");
        var stale = await adapter.WriteAndValidateAsync(
            sources,
            output,
            new PluginWriteRequest(staleStage.FullName, selected.Value.Association, selected.Value.Baseline),
            TestContext.Current.CancellationToken);
        stale.Succeeded.ShouldBeFalse();
        stale.Error!.Code.ShouldBe(EngineErrorCode.ExternalChangeDetected);
        staleStage.EnumerateFileSystemInfos().ShouldBeEmpty();

        var refreshed = await adapter.OpenOutputAsync(
            sources,
            new SelectOutputRequest(Guid.NewGuid(), sources.Revision, OutputSelectionMode.OpenExisting, association),
            TestContext.Current.CancellationToken);
        refreshed.Succeeded.ShouldBeTrue(refreshed.Error?.Message);
        await refreshed.Value!.Output.DisposeAsync();
        refreshed.Value.Baseline.BaselineId.ShouldNotBe(selected.Value.Baseline.BaselineId);

        await using var candidate = adapter.CloneOutput(output, TestContext.Current.CancellationToken);
        var candidateState = candidate.ShouldBeOfType<StarfieldPluginOutputState>();
        candidateState.Baseline.BaselineId.ShouldBe(selected.Value.Baseline.BaselineId);
        candidateState.Baseline.BaselineId.ShouldNotBe(refreshed.Value.Baseline.BaselineId);
        var begin = adapter.BeginEdit(
            sources,
            candidate,
            new BeginEditRequest(
                Guid.NewGuid(),
                sources.Revision,
                FormListEditRole.ExistingOutput,
                targetFormKey: listKey),
            TestContext.Current.CancellationToken);
        begin.Succeeded.ShouldBeTrue(begin.Error?.Message);
        Apply(adapter, sources, candidate, listKey, new SetEditorIdEdit("EditedAfterRestore"));

        var freshStage = fixture.RootDirectory.CreateSubdirectory("RestoredDestinationFreshStage");
        var freshRequest = new PluginWriteRequest(
            freshStage.FullName,
            selected.Value.Association,
            refreshed.Value.Baseline);
        freshRequest.ExpectedOutputBaseline.BaselineId.ShouldBe(refreshed.Value.Baseline.BaselineId);
        var accepted = await adapter.WriteAndValidateAsync(
            sources,
            candidate,
            freshRequest,
            TestContext.Current.CancellationToken);
        accepted.Succeeded.ShouldBeTrue(accepted.Error?.Message);
        accepted.Value!.Disposition.ShouldBe(PluginWriteDisposition.StagedChanges);
        accepted.Value.ArtifactMappings.Single(mapping => mapping.StagedArtifact.Role == PluginArtifactRole.Plugin)
            .DestinationPath.ShouldBe(Path.GetFullPath(outputPath));
        await accepted.Value.StagedOutput!.DisposeAsync();
    }

    /// <summary>A changed embedded output stages preserved unrelated, duplicate, null, component, and conditional state.</summary>
    /// <returns>A task that completes after selected and staged plugin lifetimes are released.</returns>
    [Fact]
    public async Task WriteAndValidateAsync_ExistingOutput_PreservesCompleteUneditedState()
    {
        using var fixture = StarfieldPluginTestFixture.Create();
        var sourceArtifacts = fixture.SnapshotArtifacts();
        var outputDirectory = fixture.RootDirectory.CreateSubdirectory("CompleteExisting");
        FormKey listKey = default;
        FormKey keywordKey = default;
        FormKey deletedListKey = default;
        var outputPath = WriteOutput(
            outputDirectory,
            "CompleteExisting.esp",
            OutputMasterStyle.Full,
            localized: false,
            mod =>
            {
                mod.ModHeader.Author = "Starfield staged writer test";
                var keyword = new Keyword(mod, "UnrelatedKeyword");
                mod.Keywords.Add(keyword);
                keywordKey = keyword.FormKey;
                var list = new FormList(mod, "CompleteList")
                {
                    Name = "Complete list",
                };
                list.Items.Add(new FormLink<IStarfieldMajorRecordGetter>(keyword.FormKey));
                list.Items.Add(new FormLink<IStarfieldMajorRecordGetter>(keyword.FormKey));
                list.Items.Add(new FormLink<IStarfieldMajorRecordGetter>(FormKey.Null));
                var component = new AttachParentArrayComponent
                {
                    Slots = new ExtendedList<IFormLinkGetter<IKeywordGetter>>(),
                };
                component.Slots.Add(new FormLink<IKeywordGetter>(keyword.FormKey));
                list.Components.Add(component);
                list.ConditionalEntries.Add(new FormListConditionalEntry
                {
                    Index = 11,
                    Conditions = null,
                });
                mod.FormLists.Add(list);
                listKey = list.FormKey;
                var deletedList = new FormList(mod, "RetainedDeletedList")
                {
                    IsDeleted = true,
                };
                mod.FormLists.Add(deletedList);
                deletedListKey = deletedList.FormKey;
            });
        var destinationBytes = File.ReadAllBytes(outputPath);
        var stagingDirectory = fixture.RootDirectory.CreateSubdirectory("CompleteExistingStage");
        var adapter = CreateAdapter(out var outputService);
        await using var sources = await OpenSourcesAsync(adapter, fixture);
        var association = CreateAssociation(outputPath, OutputMasterStyle.Full, LocalizedOutputMode.Embedded);
        var open = await adapter.OpenOutputAsync(
            sources,
            new SelectOutputRequest(Guid.NewGuid(), sources.Revision, OutputSelectionMode.OpenExisting, association),
            TestContext.Current.CancellationToken);

        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        await using var selected = open.Value!.Output;
        await using var candidate = adapter.CloneOutput(selected, TestContext.Current.CancellationToken);
        var begin = adapter.BeginEdit(
            sources,
            candidate,
            new BeginEditRequest(
                Guid.NewGuid(),
                sources.Revision,
                FormListEditRole.ExistingOutput,
                targetFormKey: listKey),
            TestContext.Current.CancellationToken);
        begin.Succeeded.ShouldBeTrue(begin.Error?.Message);
        var applied = adapter.ApplyEdit(
            sources,
            candidate,
            listKey,
            adapter.PrepareEdit(new SetEditorIdEdit("EditedCompleteList")));
        applied.Succeeded.ShouldBeTrue(applied.Error?.Message);
        applied.Value!.Changed.ShouldBeTrue();

        var result = await adapter.WriteAndValidateAsync(
            sources,
            candidate,
            CreateWriteRequest(stagingDirectory.FullName, open.Value),
            TestContext.Current.CancellationToken);

        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        result.Value!.Disposition.ShouldBe(PluginWriteDisposition.StagedChanges);
        var pluginMapping = result.Value.ArtifactMappings.Single(
            mapping => mapping.StagedArtifact.Role == PluginArtifactRole.Plugin);
        pluginMapping.DestinationPath.ShouldBe(Path.GetFullPath(outputPath));
        var stagedOpen = await outputService.OpenAsync(
            sources,
            new SelectOutputRequest(
                Guid.NewGuid(),
                sources.Revision,
                OutputSelectionMode.OpenExisting,
                CreateAssociation(pluginMapping.StagedArtifact.Path, OutputMasterStyle.Full, LocalizedOutputMode.Embedded)),
            TestContext.Current.CancellationToken);
        stagedOpen.Succeeded.ShouldBeTrue(stagedOpen.Error?.Message);
        await using var stagedState = stagedOpen.Value!.Output.ShouldBeOfType<StarfieldPluginOutputState>();
        var stagedMod = stagedState.CreateSnapshot(TestContext.Current.CancellationToken);
        stagedMod.ModHeader.Author.ShouldBe("Starfield staged writer test");
        stagedMod.Keywords.ShouldHaveSingleItem().FormKey.ShouldBe(keywordKey);
        var stagedList = stagedMod.FormLists.Single(record => record.FormKey == listKey);
        stagedList.EditorID.ShouldBe("EditedCompleteList");
        stagedList.Name!.String.ShouldBe("Complete list");
        stagedList.Items.Select(item => item.FormKeyNullable).ShouldBe(
            new FormKey?[] { keywordKey, keywordKey, FormKey.Null });
        stagedList.Components.ShouldHaveSingleItem().ShouldBeOfType<AttachParentArrayComponent>()
            .Slots.ShouldHaveSingleItem().FormKey.ShouldBe(keywordKey);
        stagedList.ConditionalEntries.ShouldHaveSingleItem().Index.ShouldBe(11U);
        stagedList.ConditionalEntries.Single().Conditions.ShouldBeNull();
        stagedMod.FormLists.Single(record => record.FormKey == deletedListKey).IsDeleted.ShouldBeTrue();

        await result.Value.StagedOutput!.DisposeAsync();
        AssertArtifactsUnchanged(sourceArtifacts);
        File.ReadAllBytes(outputPath).ShouldBe(destinationBytes);
    }

    /// <summary>A new localized output retains master style, multilingual text, references, nulls, duplicates, and exact master order.</summary>
    /// <param name="style">The full, small, or medium plugin master style to roundtrip.</param>
    /// <param name="includeName">Whether the plugin writer has real translated entries or must retain an explicit empty table.</param>
    /// <returns>A task that completes after selected and staged plugin lifetimes are released.</returns>
    [Theory]
    [InlineData(OutputMasterStyle.Full, true)]
    [InlineData(OutputMasterStyle.Small, true)]
    [InlineData(OutputMasterStyle.Medium, true)]
    [InlineData(OutputMasterStyle.Full, false)]
    public async Task WriteAndValidateAsync_NewLocalizedOutput_PreservesAllStarfieldFields(OutputMasterStyle style, bool includeName)
    {
        using var fixture = StarfieldPluginTestFixture.Create();
        var sourceArtifacts = fixture.SnapshotArtifacts();
        var outputDirectory = fixture.RootDirectory.CreateSubdirectory("LocalizedOutput");
        var association = CreateAssociation(
            Path.Combine(outputDirectory.FullName, $"Localized{style}.esm"),
            style,
            LocalizedOutputMode.SeparateStringFiles);
        var stagingDirectory = fixture.RootDirectory.CreateSubdirectory($"Localized{style}Stage");
        var adapter = CreateAdapter(out var outputService);
        await using var sources = await OpenSourcesAsync(adapter, fixture);
        var open = await adapter.OpenOutputAsync(
            sources,
            new SelectOutputRequest(Guid.NewGuid(), sources.Revision, OutputSelectionMode.CreateNew, association),
            TestContext.Current.CancellationToken);

        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        await using var output = open.Value!.Output;
        var begin = adapter.BeginEdit(
            sources,
            output,
            new BeginEditRequest(Guid.NewGuid(), sources.Revision, FormListEditRole.New),
            TestContext.Current.CancellationToken);
        begin.Succeeded.ShouldBeTrue(begin.Error?.Message);
        begin.Value.ShouldNotBeNull();
        ((StarfieldPluginOutputState)output).CreateSnapshot(TestContext.Current.CancellationToken)
            .IsMaster.ShouldBeTrue();
        if (includeName)
        {
            var name = new TranslatedString(Language.English, "Localized\nEnglish");
            name.Set(Language.French, "Francais\nLocalise");
            Apply(adapter, sources, output, begin.Value!.FormKey, new StarfieldSetNameEdit(name));
        }
        Apply(
            adapter,
            sources,
            output,
            begin.Value.FormKey,
            new ReplaceItemsEdit([fixture.BookFormKey, fixture.BookFormKey, FormKey.Null]));
        var component = new AttachParentArrayComponent
        {
            Slots = new ExtendedList<IFormLinkGetter<IKeywordGetter>>
            {
                new FormLink<IKeywordGetter>(fixture.KeywordFormKey),
            },
        };
        Apply(adapter, sources, output, begin.Value.FormKey, new StarfieldAddComponentEdit(0, component));
        Apply(
            adapter,
            sources,
            output,
            begin.Value.FormKey,
            new StarfieldSetConditionalEntriesEdit(
                [new FormListConditionalEntry { Index = 17, Conditions = null }]));

        var result = await adapter.WriteAndValidateAsync(
            sources,
            output,
            CreateWriteRequest(stagingDirectory.FullName, open.Value),
            TestContext.Current.CancellationToken);

        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        result.Value!.Disposition.ShouldBe(PluginWriteDisposition.StagedChanges);
        var presentSidecars = result.Value.ArtifactMappings
            .Where(mapping => mapping.StagedArtifact.Role != PluginArtifactRole.Plugin
                && mapping.StagedArtifact.Fingerprint.Exists)
            .ToArray();
        presentSidecars.Select(mapping => mapping.StagedArtifact.Language).ShouldContain(Language.English.ToString());
        if (includeName)
        {
            presentSidecars.Select(mapping => mapping.StagedArtifact.Language).ShouldContain(Language.French.ToString());
        }
        var pluginMapping = result.Value.ArtifactMappings.Single(
            mapping => mapping.StagedArtifact.Role == PluginArtifactRole.Plugin);
        var stagedOpen = await outputService.OpenAsync(
            sources,
            new SelectOutputRequest(
                Guid.NewGuid(),
                sources.Revision,
                OutputSelectionMode.OpenExisting,
                CreateAssociation(
                    pluginMapping.StagedArtifact.Path,
                    style,
                    LocalizedOutputMode.SeparateStringFiles)),
            TestContext.Current.CancellationToken);
        stagedOpen.Succeeded.ShouldBeTrue(stagedOpen.Error?.Message);
        await using var stagedState = stagedOpen.Value!.Output.ShouldBeOfType<StarfieldPluginOutputState>();
        var stagedMod = stagedState.CreateSnapshot(TestContext.Current.CancellationToken);
        stagedMod.IsSmallMaster.ShouldBe(style == OutputMasterStyle.Small);
        stagedMod.IsMediumMaster.ShouldBe(style == OutputMasterStyle.Medium);
        stagedMod.MasterReferences.Select(reference => reference.Master)
            .ShouldBe([fixture.SourceModKey], ignoreOrder: false);
        var stagedList = stagedMod.FormLists.ShouldHaveSingleItem();
        if (includeName)
        {
            stagedList.Name!.String.ShouldBe("Localized\nEnglish");
            stagedList.Name.Lookup(Language.French).ShouldBe("Francais\nLocalise");
        }
        else
        {
            stagedList.Name.ShouldBeNull();
            result.Value.ArtifactMappings
                .Where(mapping => mapping.StagedArtifact.Role != PluginArtifactRole.Plugin
                    && mapping.StagedArtifact.Fingerprint.Exists)
                .ShouldHaveSingleItem().StagedArtifact.Fingerprint.Length.ShouldBe(8L);
        }
        stagedList.Items.Select(item => item.FormKeyNullable).ShouldBe(
            new FormKey?[] { fixture.BookFormKey, fixture.BookFormKey, FormKey.Null });
        stagedList.Components.ShouldHaveSingleItem().ShouldBeOfType<AttachParentArrayComponent>()
            .Slots.ShouldHaveSingleItem().FormKey.ShouldBe(fixture.KeywordFormKey);
        stagedList.ConditionalEntries.ShouldHaveSingleItem().Index.ShouldBe(17U);

        await result.Value.StagedOutput!.DisposeAsync();
        File.Exists(association.PluginPath).ShouldBeFalse();
        AssertArtifactsUnchanged(sourceArtifacts);
    }

    /// <summary>An existing localized output retains its plugin master flag independently of its filename, permits no-op saves, and rejects material rewrites.</summary>
    /// <returns>A task that completes after generated plugin lifetimes are released.</returns>
    [Fact]
    public async Task WriteAndValidateAsync_ExistingLocalizedOutput_AllowsNoOpAndRejectsChange()
    {
        using var fixture = StarfieldPluginTestFixture.Create();
        var outputDirectory = fixture.RootDirectory.CreateSubdirectory("ExistingLocalized");
        FormKey listKey = default;
        var outputPath = WriteOutput(
            outputDirectory,
            "ExistingLocalized.esm",
            OutputMasterStyle.Full,
            localized: true,
            mod =>
            {
                var list = new FormList(mod, "ExistingLocalizedList")
                {
                    Name = CreateName("Existing English", "Francais existant"),
                };
                mod.FormLists.Add(list);
                listKey = list.FormKey;
            });
        var association = CreateAssociation(
            outputPath,
            OutputMasterStyle.Full,
            LocalizedOutputMode.SeparateStringFiles);
        var adapter = CreateAdapter(out _);
        await using var sources = await OpenSourcesAsync(adapter, fixture);
        var open = await adapter.OpenOutputAsync(
            sources,
            new SelectOutputRequest(Guid.NewGuid(), sources.Revision, OutputSelectionMode.OpenExisting, association),
            TestContext.Current.CancellationToken);

        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        await using var output = open.Value!.Output;
        var destinationArtifacts = SnapshotExistingArtifacts(open.Value.Baseline);
        ((StarfieldPluginOutputState)output).CreateSnapshot(TestContext.Current.CancellationToken)
            .IsMaster.ShouldBeFalse();
        var noOpDirectory = fixture.RootDirectory.CreateSubdirectory("ExistingLocalizedNoOp");
        var noOp = await adapter.WriteAndValidateAsync(
            sources,
            output,
            CreateWriteRequest(noOpDirectory.FullName, open.Value),
            TestContext.Current.CancellationToken);
        noOp.Succeeded.ShouldBeTrue(noOp.Error?.Message);
        noOp.Value!.Disposition.ShouldBe(PluginWriteDisposition.Unchanged);
        noOpDirectory.EnumerateFileSystemInfos().ShouldBeEmpty();

        await using var candidate = adapter.CloneOutput(output, TestContext.Current.CancellationToken);
        var begin = adapter.BeginEdit(
            sources,
            candidate,
            new BeginEditRequest(
                Guid.NewGuid(),
                sources.Revision,
                FormListEditRole.ExistingOutput,
                targetFormKey: listKey),
            TestContext.Current.CancellationToken);
        begin.Succeeded.ShouldBeTrue(begin.Error?.Message);
        Apply(adapter, sources, candidate, listKey, new SetEditorIdEdit("ChangedLocalizedList"));
        var rejectedDirectory = fixture.RootDirectory.CreateSubdirectory("ExistingLocalizedRejected");
        var rejected = await adapter.WriteAndValidateAsync(
            sources,
            candidate,
            CreateWriteRequest(rejectedDirectory.FullName, open.Value),
            TestContext.Current.CancellationToken);

        rejected.Succeeded.ShouldBeFalse();
        rejected.Error!.Code.ShouldBe(EngineErrorCode.UnsupportedInput);
        rejectedDirectory.EnumerateFileSystemInfos().ShouldBeEmpty();
        AssertArtifactsUnchanged(destinationArtifacts);
    }

    /// <summary>Private staging must be empty, and pre-cancellation stops before any plugin artifact is created.</summary>
    /// <returns>A task that completes after the generated plugin lifetimes are released.</returns>
    [Fact]
    public async Task WriteAndValidateAsync_ValidatesStagingAndHonorsCancellationBeforeWrite()
    {
        using var fixture = StarfieldPluginTestFixture.Create();
        var outputDirectory = fixture.RootDirectory.CreateSubdirectory("StagingValidationOutput");
        var association = CreateAssociation(
            Path.Combine(outputDirectory.FullName, "StagingValidation.esp"),
            OutputMasterStyle.Full,
            LocalizedOutputMode.Embedded);
        var adapter = CreateAdapter(out _);
        await using var sources = await OpenSourcesAsync(adapter, fixture);
        var open = await adapter.OpenOutputAsync(
            sources,
            new SelectOutputRequest(Guid.NewGuid(), sources.Revision, OutputSelectionMode.CreateNew, association),
            TestContext.Current.CancellationToken);
        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        await using var output = open.Value!.Output;

        var nonemptyStage = fixture.RootDirectory.CreateSubdirectory("NonemptyStage");
        var markerPath = Path.Combine(nonemptyStage.FullName, "owned.marker");
        File.WriteAllText(markerPath, "preserve");
        var rejected = await adapter.WriteAndValidateAsync(
            sources,
            output,
            CreateWriteRequest(nonemptyStage.FullName, open.Value),
            TestContext.Current.CancellationToken);
        rejected.Succeeded.ShouldBeFalse();
        rejected.Error!.Code.ShouldBe(EngineErrorCode.InvalidRequest);
        File.ReadAllText(markerPath).ShouldBe("preserve");
        File.Exists(association.PluginPath).ShouldBeFalse();

        var canceledStage = fixture.RootDirectory.CreateSubdirectory("CanceledStage");
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        await Should.ThrowAsync<OperationCanceledException>(async () =>
            await adapter.WriteAndValidateAsync(
                sources,
                output,
                CreateWriteRequest(canceledStage.FullName, open.Value),
                cancellation.Token));
        canceledStage.EnumerateFileSystemInfos().ShouldBeEmpty();
        File.Exists(association.PluginPath).ShouldBeFalse();
    }

    /// <summary>Creates the complete typed Starfield adapter graph without a service locator.</summary>
    /// <param name="outputService">Returns the shared output service used for staged inspection.</param>
    /// <returns>The complete Starfield game adapter.</returns>
    private static StarfieldFormListGameAdapter CreateAdapter(out StarfieldPluginOutputService outputService)
    {
        var sourceLoader = new StarfieldPluginSourceLoader(new PluginSourceInputLoader());
        outputService = new StarfieldPluginOutputService(new PluginOutputInputLoader());
        var editService = new StarfieldRecordEditService();
        var writer = new StarfieldPluginWriter(outputService);
        return new StarfieldFormListGameAdapter(sourceLoader, outputService, editService, writer);
    }

    /// <summary>Opens the generated Starfield source set through the complete adapter.</summary>
    /// <param name="adapter">The complete Starfield adapter.</param>
    /// <param name="fixture">The generated source fixture.</param>
    /// <returns>The independently owned typed source lifetime.</returns>
    private static async Task<StarfieldPluginSourceSet> OpenSourcesAsync(
        StarfieldFormListGameAdapter adapter,
        StarfieldPluginTestFixture fixture)
    {
        var result = await adapter.OpenSourcesAsync(
            fixture.CreateOpenRequest(),
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value!.Sources.ShouldBeOfType<StarfieldPluginSourceSet>();
    }

    /// <summary>Prepares and applies one typed edit through the complete adapter.</summary>
    /// <param name="adapter">The complete Starfield adapter.</param>
    /// <param name="sources">The borrowed immutable plugin sources.</param>
    /// <param name="output">The unpublished complete output candidate.</param>
    /// <param name="target">The target FormList identity.</param>
    /// <param name="edit">The typed caller-owned edit.</param>
    private static void Apply(
        StarfieldFormListGameAdapter adapter,
        StarfieldPluginSourceSet sources,
        IPluginOutputState output,
        FormKey target,
        FormListEdit edit)
    {
        var result = adapter.ApplyEdit(sources, output, target, adapter.PrepareEdit(edit));
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
    }

    /// <summary>Creates a canonical output association for one generated destination.</summary>
    /// <param name="path">The output plugin path.</param>
    /// <param name="style">The requested plugin master style.</param>
    /// <param name="localizedOutputMode">The requested localization representation.</param>
    /// <returns>The immutable output association.</returns>
    private static OutputAssociation CreateAssociation(
        string path,
        OutputMasterStyle style,
        LocalizedOutputMode localizedOutputMode)
    {
        return new OutputAssociation(
            Path.GetFullPath(path),
            ModKey.FromNameAndExtension(Path.GetFileName(path)),
            localizedOutputMode,
            style);
    }

    /// <summary>Creates a staged-write request using the current selected output baseline.</summary>
    /// <param name="stagingDirectoryPath">The dedicated empty private staging directory.</param>
    /// <param name="open">The current selected output and artifact baseline.</param>
    /// <returns>The guarded plugin staged-write request.</returns>
    private static PluginWriteRequest CreateWriteRequest(
        string stagingDirectoryPath,
        PluginOutputOpenResult open)
    {
        return new PluginWriteRequest(stagingDirectoryPath, open.Association, open.Baseline);
    }

    /// <summary>Writes one deterministic complete output fixture through Mutagen's public writer.</summary>
    /// <param name="outputDirectory">The existing output directory.</param>
    /// <param name="fileName">The output plugin file name.</param>
    /// <param name="style">The plugin master style.</param>
    /// <param name="localized">Whether record strings use loose sidecars.</param>
    /// <param name="configure">The caller that builds representative plugin content.</param>
    /// <param name="sourceMasterStyles">Optional exact styles for source masters referenced by the generated fixture.</param>
    /// <returns>The canonical generated plugin path.</returns>
    private static string WriteOutput(
        DirectoryInfo outputDirectory,
        string fileName,
        OutputMasterStyle style,
        bool localized,
        Action<StarfieldMod> configure,
        IReadOnlyList<KeyedMasterStyle>? sourceMasterStyles = null)
    {
        var mod = new StarfieldMod(ModKey.FromNameAndExtension(fileName), StarfieldRelease.Starfield)
        {
            IsSmallMaster = style == OutputMasterStyle.Small,
            IsMediumMaster = style == OutputMasterStyle.Medium,
            UsingLocalization = localized,
        };
        configure(mod);
        var outputPath = Path.GetFullPath(Path.Combine(outputDirectory.FullName, fileName));
        var writeParameters = BinaryWriteParameters.Default;
        if (sourceMasterStyles is not null)
        {
            var masterFlags = new Cache<IModMasterStyledGetter, ModKey>(metadata => metadata.ModKey);
            foreach (var metadata in sourceMasterStyles)
            {
                masterFlags.Set(metadata);
            }

            masterFlags.Set(mod);
            writeParameters = writeParameters with
            {
                MasterFlagsLookup = masterFlags,
                MastersListOrdering = new MastersListOrderingByLoadOrder(
                    sourceMasterStyles.Select(metadata => metadata.ModKey).Append(mod.ModKey).ToArray())
                {
                    Strict = true,
                },
            };
        }

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

    /// <summary>Creates a deterministic English and French record translated string.</summary>
    /// <param name="english">The default English text.</param>
    /// <param name="french">The French translation.</param>
    /// <returns>A mutable translated string containing both languages.</returns>
    private static TranslatedString CreateName(string english, string french)
    {
        var name = new TranslatedString(Language.English, english);
        name.Set(Language.French, french);
        return name;
    }

    /// <summary>Snapshots every present artifact from one existing output baseline.</summary>
    /// <param name="baseline">The complete selected output artifact baseline.</param>
    /// <returns>The exact bytes for every present artifact keyed by canonical path.</returns>
    private static IReadOnlyDictionary<string, byte[]> SnapshotExistingArtifacts(OutputArtifactSetBaseline baseline)
    {
        return baseline.Artifacts
            .Where(artifact => artifact.Fingerprint.Exists)
            .ToDictionary(
                artifact => artifact.Path,
                artifact => File.ReadAllBytes(artifact.Path),
                StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>Asserts that every expected artifact retains its exact bytes.</summary>
    /// <param name="expected">The original artifact bytes keyed by canonical path.</param>
    private static void AssertArtifactsUnchanged(IReadOnlyDictionary<string, byte[]> expected)
    {
        expected.ShouldNotBeEmpty();
        foreach (var artifact in expected)
        {
            File.ReadAllBytes(artifact.Key).ShouldBe(artifact.Value);
        }
    }
}
