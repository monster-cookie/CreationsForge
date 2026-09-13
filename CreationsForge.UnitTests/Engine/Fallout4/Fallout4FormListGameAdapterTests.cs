using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.PluginInputs;
using CreationsForge.Core.Engine.PluginOutputs;
using CreationsForge.Fallout4.PluginAdapter;
using CreationsForge.Fallout4.PluginAdapter.Edits;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Strings;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Fallout4;

/// <summary>Verifies complete Fallout 4 adapter composition, output-aware reads, and private staged-write preservation.</summary>
public sealed class Fallout4FormListGameAdapterTests
{
    /// <summary>Selected output participates in plugin enumeration, winning reads, and staged-output FormList enumeration.</summary>
    /// <returns>A task that completes after the generated plugin lifetimes are released.</returns>
    [Fact]
    public async Task AdapterReads_IncludeSelectedOutputWithoutMutatingPluginFiles()
    {
        using var fixture = Fallout4PluginOutputTestFixture.Create();
        var existing = fixture.WriteExistingOutput();
        var sourceArtifacts = fixture.Sources.SnapshotArtifacts();
        var destinationBytes = File.ReadAllBytes(existing.Association.PluginPath);
        var adapter = CreateAdapter(out _);
        await using var sources = await OpenSourcesAsync(adapter, fixture);
        var open = await adapter.OpenOutputAsync(
            sources,
            new SelectOutputRequest(
                Guid.NewGuid(),
                sources.Revision,
                OutputSelectionMode.OpenExisting,
                existing.Association),
            TestContext.Current.CancellationToken);

        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        await using var output = open.Value!.Output;
        var plugins = adapter.ListPlugins(sources, output, TestContext.Current.CancellationToken);
        plugins.Succeeded.ShouldBeTrue(plugins.Error?.Message);
        plugins.Value![^1].ModKey.ShouldBe(existing.Association.ModKey);
        plugins.Value[^1].Role.ShouldBe(PluginRole.Output);

        var staged = adapter.ListFormLists(
            sources,
            output,
            RecordScope.StagedOutput,
            TestContext.Current.CancellationToken);
        staged.Succeeded.ShouldBeTrue(staged.Error?.Message);
        staged.Value.ShouldNotBeNull();
        staged.Value!.Select(summary => summary.FormKey).ShouldContain(existing.OwnListFormKey);
        staged.Value.Select(summary => summary.FormKey).ShouldContain(existing.SourceOverrideFormKey);
        staged.Value.ShouldAllBe(summary => summary.Role == PluginRole.Output);

        var winning = adapter.ReadFormListContext(
            sources,
            output,
            new ReferenceRequest(existing.SourceOverrideFormKey, RecordScope.WinningOverrides),
            TestContext.Current.CancellationToken);
        winning.Succeeded.ShouldBeTrue(winning.Error?.Message);
        winning.Value!.Context.ContainingModKey.ShouldBe(existing.Association.ModKey);
        winning.Value.Context.Role.ShouldBe(PluginRole.Output);
        winning.Value.Record.ShouldBeOfType<FormList>().EditorID.ShouldBe("ExistingSourceOverride");

        AssertArtifactsUnchanged(sourceArtifacts);
        File.ReadAllBytes(existing.Association.PluginPath).ShouldBe(destinationBytes);
    }

    /// <summary>An unchanged existing output returns the explicit zero-write disposition and leaves private staging empty.</summary>
    /// <returns>A task that completes after the generated plugin lifetimes are released.</returns>
    [Fact]
    public async Task WriteAndValidateAsync_UnchangedExistingOutput_PerformsZeroArtifactWrites()
    {
        using var fixture = Fallout4PluginOutputTestFixture.Create();
        var existing = fixture.WriteExistingOutput();
        var sourceArtifacts = fixture.Sources.SnapshotArtifacts();
        var destinationBytes = File.ReadAllBytes(existing.Association.PluginPath);
        var stagingDirectory = fixture.OutputDirectory.CreateSubdirectory("unchanged-stage");
        var adapter = CreateAdapter(out _);
        await using var sources = await OpenSourcesAsync(adapter, fixture);
        var open = await adapter.OpenOutputAsync(
            sources,
            new SelectOutputRequest(
                Guid.NewGuid(),
                sources.Revision,
                OutputSelectionMode.OpenExisting,
                existing.Association),
            TestContext.Current.CancellationToken);

        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        await using var output = open.Value!.Output;
        var result = await adapter.WriteAndValidateAsync(
            sources,
            output,
            new PluginWriteRequest(stagingDirectory.FullName, open.Value.Association, open.Value.Baseline),
            TestContext.Current.CancellationToken);

        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        result.Value!.Disposition.ShouldBe(PluginWriteDisposition.Unchanged);
        result.Value.StagedOutput.ShouldBeNull();
        result.Value.ArtifactMappings.ShouldBeEmpty();
        stagingDirectory.EnumerateFileSystemInfos().ShouldBeEmpty();
        AssertArtifactsUnchanged(sourceArtifacts);
        File.ReadAllBytes(existing.Association.PluginPath).ShouldBe(destinationBytes);
    }

    /// <summary>An identical-byte destination replacement invalidates the old file identity while a fresh baseline remains writable.</summary>
    /// <returns>A task that completes after the selected, replacement, and staged plugin lifetimes are released.</returns>
    [Fact]
    public async Task WriteAndValidateAsync_IdenticalByteReplacement_RejectsOldBaselineAndAcceptsFreshBaseline()
    {
        using var fixture = Fallout4PluginOutputTestFixture.Create();
        var existing = fixture.WriteExistingOutput();
        var sourceArtifacts = fixture.Sources.SnapshotArtifacts();
        var destinationBytes = File.ReadAllBytes(existing.Association.PluginPath);
        var stagingDirectory = fixture.OutputDirectory.CreateSubdirectory("replacement-stale-stage");
        var freshStagingDirectory = fixture.OutputDirectory.CreateSubdirectory("replacement-fresh-stage");
        var adapter = CreateAdapter(out _);
        await using var sources = await OpenSourcesAsync(adapter, fixture);
        var originalOpen = await adapter.OpenOutputAsync(
            sources,
            new SelectOutputRequest(
                Guid.NewGuid(),
                sources.Revision,
                OutputSelectionMode.OpenExisting,
                existing.Association),
            TestContext.Current.CancellationToken);

        originalOpen.Succeeded.ShouldBeTrue(originalOpen.Error?.Message);
        await using var originalOutput = originalOpen.Value!.Output;
        var originalPlugin = originalOpen.Value.Baseline.Artifacts.Single(
            artifact => artifact.Role == PluginArtifactRole.Plugin);
        var originalIdentity = originalPlugin.FileIdentity.ShouldNotBeNull();
        var replacementPath = Path.Combine(
            fixture.OutputDirectory.FullName,
            $"replacement-{Guid.NewGuid():N}.tmp");
        File.Copy(existing.Association.PluginPath, replacementPath);
        File.Move(replacementPath, existing.Association.PluginPath, overwrite: true);

        var freshOpen = await adapter.OpenOutputAsync(
            sources,
            new SelectOutputRequest(
                Guid.NewGuid(),
                sources.Revision,
                OutputSelectionMode.OpenExisting,
                existing.Association),
            TestContext.Current.CancellationToken);
        freshOpen.Succeeded.ShouldBeTrue(freshOpen.Error?.Message);
        await using var freshOutput = freshOpen.Value!.Output;
        var freshPlugin = freshOpen.Value.Baseline.Artifacts.Single(
            artifact => artifact.Role == PluginArtifactRole.Plugin);
        freshPlugin.Fingerprint.ShouldBe(originalPlugin.Fingerprint);
        freshPlugin.FileIdentity.ShouldNotBeNull().ShouldNotBe(originalIdentity);

        var stale = await adapter.WriteAndValidateAsync(
            sources,
            originalOutput,
            new PluginWriteRequest(
                stagingDirectory.FullName,
                originalOpen.Value.Association,
                originalOpen.Value.Baseline),
            TestContext.Current.CancellationToken);

        stale.Succeeded.ShouldBeFalse();
        stale.Error!.Code.ShouldBe(EngineErrorCode.ExternalChangeDetected);
        stagingDirectory.EnumerateFileSystemInfos().ShouldBeEmpty();

        await using var candidate = adapter.CloneOutput(originalOutput, TestContext.Current.CancellationToken);
        candidate.ShouldBeOfType<Fallout4PluginOutputState>()
            .Baseline
            .Artifacts
            .Single(artifact => artifact.Role == PluginArtifactRole.Plugin)
            .FileIdentity
            .ShouldBe(originalIdentity);
        var begin = adapter.BeginEdit(
            sources,
            candidate,
            new BeginEditRequest(
                Guid.NewGuid(),
                sources.Revision,
                FormListEditRole.ExistingOutput,
                targetFormKey: existing.SourceOverrideFormKey),
            TestContext.Current.CancellationToken);
        begin.Succeeded.ShouldBeTrue(begin.Error?.Message);
        adapter.ApplyEdit(
            sources,
            candidate,
            begin.Value!.FormKey,
            adapter.PrepareEdit(new SetEditorIdEdit("ReplacementIdentityAccepted"))).Succeeded.ShouldBeTrue();

        var fresh = await adapter.WriteAndValidateAsync(
            sources,
            candidate,
            new PluginWriteRequest(
                freshStagingDirectory.FullName,
                freshOpen.Value.Association,
                freshOpen.Value.Baseline),
            TestContext.Current.CancellationToken);

        fresh.Succeeded.ShouldBeTrue(fresh.Error?.Message);
        fresh.Value!.Disposition.ShouldBe(PluginWriteDisposition.StagedChanges);
        fresh.Value.ArtifactMappings.ShouldNotBeEmpty();
        await fresh.Value.StagedOutput!.DisposeAsync();
        AssertArtifactsUnchanged(sourceArtifacts);
        File.ReadAllBytes(existing.Association.PluginPath).ShouldBe(destinationBytes);
    }

    /// <summary>A changed nonlocalized output stages complete preserved plugin state while retaining unedited records and embedded names.</summary>
    /// <returns>A task that completes after all selected and staged output lifetimes are released.</returns>
    [Fact]
    public async Task WriteAndValidateAsync_ExistingOutput_PreservesUneditedPluginStateAndStagesMappings()
    {
        using var fixture = Fallout4PluginOutputTestFixture.Create();
        var existing = fixture.WriteExistingOutput();
        var sourceArtifacts = fixture.Sources.SnapshotArtifacts();
        var destinationBytes = File.ReadAllBytes(existing.Association.PluginPath);
        var stagingDirectory = fixture.OutputDirectory.CreateSubdirectory("existing-stage");
        var adapter = CreateAdapter(out var outputService);
        await using var sources = await OpenSourcesAsync(adapter, fixture);
        var open = await adapter.OpenOutputAsync(
            sources,
            new SelectOutputRequest(
                Guid.NewGuid(),
                sources.Revision,
                OutputSelectionMode.OpenExisting,
                existing.Association),
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
                targetFormKey: existing.SourceOverrideFormKey),
            TestContext.Current.CancellationToken);
        begin.Succeeded.ShouldBeTrue(begin.Error?.Message);
        var name = new TranslatedString(Language.English, "First line\nSecond line");
        var applied = adapter.ApplyEdit(
            sources,
            candidate,
            begin.Value!.FormKey,
            adapter.PrepareEdit(new Fallout4SetNameEdit(name)));
        applied.Succeeded.ShouldBeTrue(applied.Error?.Message);
        applied.Value!.Changed.ShouldBeTrue();

        var result = await adapter.WriteAndValidateAsync(
            sources,
            candidate,
            new PluginWriteRequest(stagingDirectory.FullName, open.Value.Association, open.Value.Baseline),
            TestContext.Current.CancellationToken);

        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        result.Value!.Disposition.ShouldBe(PluginWriteDisposition.StagedChanges);
        result.Value.StagedOutput.ShouldNotBeNull();
        result.Value.ArtifactMappings.ShouldNotBeEmpty();
        var pluginMapping = result.Value.ArtifactMappings.Single(
            mapping => mapping.StagedArtifact.Role == PluginArtifactRole.Plugin);
        pluginMapping.StagedArtifact.Fingerprint.Exists.ShouldBeTrue();
        pluginMapping.DestinationPath.ShouldBe(Path.GetFullPath(existing.Association.PluginPath));
        result.Value.ArtifactMappings
            .Where(mapping => mapping.StagedArtifact.Role != PluginArtifactRole.Plugin)
            .ShouldAllBe(mapping => !mapping.StagedArtifact.Fingerprint.Exists);

        var stagedAssociation = new OutputAssociation(
            pluginMapping.StagedArtifact.Path,
            existing.Association.ModKey,
            existing.Association.LocalizedOutputMode,
            existing.Association.MasterStyle);
        var stagedOpen = await outputService.OpenAsync(
            sources,
            new SelectOutputRequest(
                Guid.NewGuid(),
                sources.Revision,
                OutputSelectionMode.OpenExisting,
                stagedAssociation),
            TestContext.Current.CancellationToken);
        stagedOpen.Succeeded.ShouldBeTrue(stagedOpen.Error?.Message);
        await using var stagedState = stagedOpen.Value!.Output.ShouldBeOfType<Fallout4PluginOutputState>();
        var stagedMod = stagedState.CreateSnapshot(TestContext.Current.CancellationToken);
        var edited = stagedMod.FormLists.Single(record => record.FormKey == existing.SourceOverrideFormKey);
        edited.Name!.String.ShouldBe("First line\nSecond line");
        edited.Name.Lookup(Language.French).ShouldBeNull();
        var unedited = stagedMod.FormLists.Single(record => record.FormKey == existing.OwnListFormKey);
        unedited.Name!.String.ShouldBe("Output list");
        unedited.Name.Lookup(Language.French).ShouldBeNull();
        stagedMod.Keywords.Single(record => record.FormKey == existing.KeywordFormKey)
            .EditorID.ShouldBe("OutputKeyword");

        await result.Value.StagedOutput!.DisposeAsync();
        AssertArtifactsUnchanged(sourceArtifacts);
        File.ReadAllBytes(existing.Association.PluginPath).ShouldBe(destinationBytes);
    }

    /// <summary>An embedded output with an unrepresentable nondefault translation fails closed before any destination mutation.</summary>
    /// <returns>A task that completes after all selected and candidate plugin lifetimes are released.</returns>
    [Fact]
    public async Task WriteAndValidateAsync_ExistingEmbeddedOutputWithMultipleLanguages_FailsClosed()
    {
        using var fixture = Fallout4PluginOutputTestFixture.Create();
        var existing = fixture.WriteExistingOutput();
        var sourceArtifacts = fixture.Sources.SnapshotArtifacts();
        var destinationBytes = File.ReadAllBytes(existing.Association.PluginPath);
        var stagingDirectory = fixture.OutputDirectory.CreateSubdirectory("embedded-multilingual-stage");
        var adapter = CreateAdapter(out _);
        await using var sources = await OpenSourcesAsync(adapter, fixture);
        var open = await adapter.OpenOutputAsync(
            sources,
            new SelectOutputRequest(
                Guid.NewGuid(),
                sources.Revision,
                OutputSelectionMode.OpenExisting,
                existing.Association),
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
                targetFormKey: existing.SourceOverrideFormKey),
            TestContext.Current.CancellationToken);
        begin.Succeeded.ShouldBeTrue(begin.Error?.Message);
        var name = new TranslatedString(Language.English, "Embedded English");
        name.Set(Language.French, "Francais non representable");
        var applied = adapter.ApplyEdit(
            sources,
            candidate,
            begin.Value!.FormKey,
            adapter.PrepareEdit(new Fallout4SetNameEdit(name)));
        applied.Succeeded.ShouldBeTrue(applied.Error?.Message);
        applied.Value!.Changed.ShouldBeTrue();

        var result = await adapter.WriteAndValidateAsync(
            sources,
            candidate,
            new PluginWriteRequest(stagingDirectory.FullName, open.Value.Association, open.Value.Baseline),
            TestContext.Current.CancellationToken);

        result.Succeeded.ShouldBeFalse();
        result.Value.ShouldBeNull();
        result.Error!.Code.ShouldBe(EngineErrorCode.ValidationFailed);
        result.Error.Message.ShouldContain("nondefault-language translations");
        AssertArtifactsUnchanged(sourceArtifacts);
        File.ReadAllBytes(existing.Association.PluginPath).ShouldBe(destinationBytes);
    }

    /// <summary>A new localized output retains authored translations, null and duplicate item slots, and only the required plugin masters.</summary>
    /// <returns>A task that completes after all selected and staged output lifetimes are released.</returns>
    [Fact]
    public async Task WriteAndValidateAsync_NewLocalizedOutput_RetainsAllFormListTranslationsAndMasters()
    {
        using var fixture = Fallout4PluginOutputTestFixture.Create();
        var sourceArtifacts = fixture.Sources.SnapshotArtifacts();
        var association = fixture.CreateAssociation(
            "LocalizedOutput.esm",
            OutputMasterStyle.Small,
            LocalizedOutputMode.SeparateStringFiles);
        var stagingDirectory = fixture.OutputDirectory.CreateSubdirectory("localized-stage");
        var adapter = CreateAdapter(out var outputService);
        await using var sources = await OpenSourcesAsync(adapter, fixture);
        var open = await adapter.OpenOutputAsync(
            sources,
            new SelectOutputRequest(
                Guid.NewGuid(),
                sources.Revision,
                OutputSelectionMode.CreateNew,
                association),
            TestContext.Current.CancellationToken);

        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        await using var output = open.Value!.Output;
        var begin = adapter.BeginEdit(
            sources,
            output,
            new BeginEditRequest(Guid.NewGuid(), sources.Revision, FormListEditRole.New),
            TestContext.Current.CancellationToken);
        begin.Succeeded.ShouldBeTrue(begin.Error?.Message);
        var name = new TranslatedString(Language.English, "Localized\nEnglish");
        name.Set(Language.French, "Francais\nLocalise");
        adapter.ApplyEdit(
            sources,
            output,
            begin.Value!.FormKey,
            adapter.PrepareEdit(new Fallout4SetNameEdit(name))).Succeeded.ShouldBeTrue();
        adapter.ApplyEdit(
            sources,
            output,
            begin.Value.FormKey,
            adapter.PrepareEdit(new ReplaceItemsEdit([
                fixture.Sources.BookFormKey,
                FormKey.Null,
                fixture.Sources.BookFormKey]))).Succeeded.ShouldBeTrue();

        var result = await adapter.WriteAndValidateAsync(
            sources,
            output,
            new PluginWriteRequest(stagingDirectory.FullName, open.Value.Association, open.Value.Baseline),
            TestContext.Current.CancellationToken);

        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        result.Value!.Disposition.ShouldBe(PluginWriteDisposition.StagedChanges);
        var existingSidecars = result.Value.ArtifactMappings
            .Where(mapping => mapping.StagedArtifact.Role != PluginArtifactRole.Plugin
                && mapping.StagedArtifact.Fingerprint.Exists)
            .ToArray();
        existingSidecars.ShouldNotBeEmpty();
        existingSidecars.Select(mapping => mapping.StagedArtifact.Language).ShouldContain(Language.English.ToString());
        existingSidecars.Select(mapping => mapping.StagedArtifact.Language).ShouldContain(Language.French.ToString());

        var stagedPlugin = result.Value.ArtifactMappings.Single(
            mapping => mapping.StagedArtifact.Role == PluginArtifactRole.Plugin);
        var stagedAssociation = new OutputAssociation(
            stagedPlugin.StagedArtifact.Path,
            association.ModKey,
            LocalizedOutputMode.SeparateStringFiles,
            OutputMasterStyle.Small);
        var stagedOpen = await outputService.OpenAsync(
            sources,
            new SelectOutputRequest(
                Guid.NewGuid(),
                sources.Revision,
                OutputSelectionMode.OpenExisting,
                stagedAssociation),
            TestContext.Current.CancellationToken);
        stagedOpen.Succeeded.ShouldBeTrue(stagedOpen.Error?.Message);
        await using var stagedState = stagedOpen.Value!.Output.ShouldBeOfType<Fallout4PluginOutputState>();
        var stagedMod = stagedState.CreateSnapshot(TestContext.Current.CancellationToken);
        var formList = stagedMod.FormLists.ShouldHaveSingleItem();
        formList.Name!.String.ShouldBe("Localized\nEnglish");
        formList.Name.Lookup(Language.French).ShouldBe("Francais\nLocalise");
        formList.Items.Select(item => item.FormKey).ShouldBe(
            [fixture.Sources.BookFormKey, FormKey.Null, fixture.Sources.BookFormKey],
            ignoreOrder: false);
        stagedMod.ModHeader.MasterReferences.Select(reference => reference.Master)
            .ShouldBe([fixture.Sources.BookFormKey.ModKey], ignoreOrder: false);

        await result.Value.StagedOutput!.DisposeAsync();
        File.Exists(association.PluginPath).ShouldBeFalse();
        AssertArtifactsUnchanged(sourceArtifacts);
    }

    /// <summary>A blank localized FormList stages an explicit empty English strings table and reopens without inventing plugin data.</summary>
    /// <returns>A task that completes after all selected and staged output lifetimes are released.</returns>
    [Fact]
    public async Task WriteAndValidateAsync_BlankLocalizedOutput_StagesExplicitEmptyStringTable()
    {
        using var fixture = Fallout4PluginOutputTestFixture.Create();
        var sourceArtifacts = fixture.Sources.SnapshotArtifacts();
        var association = fixture.CreateAssociation(
            "BlankLocalizedOutput.esm",
            OutputMasterStyle.Full,
            LocalizedOutputMode.SeparateStringFiles);
        var stagingDirectory = fixture.OutputDirectory.CreateSubdirectory("blank-localized-stage");
        var adapter = CreateAdapter(out var outputService);
        await using var sources = await OpenSourcesAsync(adapter, fixture);
        var open = await adapter.OpenOutputAsync(
            sources,
            new SelectOutputRequest(
                Guid.NewGuid(),
                sources.Revision,
                OutputSelectionMode.CreateNew,
                association),
            TestContext.Current.CancellationToken);

        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        await using var output = open.Value!.Output;
        var begin = adapter.BeginEdit(
            sources,
            output,
            new BeginEditRequest(Guid.NewGuid(), sources.Revision, FormListEditRole.New),
            TestContext.Current.CancellationToken);
        begin.Succeeded.ShouldBeTrue(begin.Error?.Message);

        var result = await adapter.WriteAndValidateAsync(
            sources,
            output,
            new PluginWriteRequest(stagingDirectory.FullName, open.Value.Association, open.Value.Baseline),
            TestContext.Current.CancellationToken);

        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        result.Value!.Disposition.ShouldBe(PluginWriteDisposition.StagedChanges);
        var stringMapping = result.Value.ArtifactMappings
            .Where(mapping => mapping.StagedArtifact.Role != PluginArtifactRole.Plugin
                && mapping.StagedArtifact.Fingerprint.Exists)
            .ShouldHaveSingleItem();
        stringMapping.StagedArtifact.Language.ShouldBe(Language.English.ToString());
        stringMapping.StagedArtifact.Fingerprint.Length.ShouldBe(8L);

        var stagedPlugin = result.Value.ArtifactMappings.Single(
            mapping => mapping.StagedArtifact.Role == PluginArtifactRole.Plugin);
        var stagedAssociation = new OutputAssociation(
            stagedPlugin.StagedArtifact.Path,
            association.ModKey,
            LocalizedOutputMode.SeparateStringFiles,
            OutputMasterStyle.Full);
        var stagedOpen = await outputService.OpenAsync(
            sources,
            new SelectOutputRequest(
                Guid.NewGuid(),
                sources.Revision,
                OutputSelectionMode.OpenExisting,
                stagedAssociation),
            TestContext.Current.CancellationToken);
        stagedOpen.Succeeded.ShouldBeTrue(stagedOpen.Error?.Message);
        await using var stagedState = stagedOpen.Value!.Output.ShouldBeOfType<Fallout4PluginOutputState>();
        var stagedMod = stagedState.CreateSnapshot(TestContext.Current.CancellationToken);
        var formList = stagedMod.FormLists.ShouldHaveSingleItem();
        formList.Name.ShouldBeNull();
        formList.Items.ShouldBeEmpty();
        stagedMod.ModHeader.MasterReferences.ShouldBeEmpty();

        await result.Value.StagedOutput!.DisposeAsync();
        File.Exists(association.PluginPath).ShouldBeFalse();
        AssertArtifactsUnchanged(sourceArtifacts);
    }

    /// <summary>An existing localized output may prove unchanged without writes but rejects a material rewrite before staging.</summary>
    /// <returns>A task that completes after all generated plugin lifetimes are released.</returns>
    [Fact]
    public async Task WriteAndValidateAsync_ExistingLocalizedOutput_AllowsNoOpAndRejectsMaterialRewrite()
    {
        using var fixture = Fallout4PluginOutputTestFixture.Create();
        var sourceArtifacts = fixture.Sources.SnapshotArtifacts();
        var association = fixture.CreateAssociation(
            "ExistingLocalized.esm",
            OutputMasterStyle.Full,
            LocalizedOutputMode.SeparateStringFiles);
        var initialStaging = fixture.OutputDirectory.CreateSubdirectory("localized-initial-stage");
        var adapter = CreateAdapter(out _);
        await using var sources = await OpenSourcesAsync(adapter, fixture);
        var create = await adapter.OpenOutputAsync(
            sources,
            new SelectOutputRequest(
                Guid.NewGuid(),
                sources.Revision,
                OutputSelectionMode.CreateNew,
                association),
            TestContext.Current.CancellationToken);
        create.Succeeded.ShouldBeTrue(create.Error?.Message);
        await using (var newOutput = create.Value!.Output)
        {
            var begin = adapter.BeginEdit(
                sources,
                newOutput,
                new BeginEditRequest(Guid.NewGuid(), sources.Revision, FormListEditRole.New),
                TestContext.Current.CancellationToken);
            begin.Succeeded.ShouldBeTrue(begin.Error?.Message);
            var name = new TranslatedString(Language.English, "Existing English");
            name.Set(Language.French, "Francais existant");
            adapter.ApplyEdit(
                sources,
                newOutput,
                begin.Value!.FormKey,
                adapter.PrepareEdit(new Fallout4SetNameEdit(name))).Succeeded.ShouldBeTrue();
            var staged = await adapter.WriteAndValidateAsync(
                sources,
                newOutput,
                new PluginWriteRequest(initialStaging.FullName, create.Value.Association, create.Value.Baseline),
                TestContext.Current.CancellationToken);
            staged.Succeeded.ShouldBeTrue(staged.Error?.Message);
            CommitStagedArtifacts(staged.Value!.ArtifactMappings);
            await staged.Value.StagedOutput!.DisposeAsync();
        }

        var open = await adapter.OpenOutputAsync(
            sources,
            new SelectOutputRequest(
                Guid.NewGuid(),
                sources.Revision,
                OutputSelectionMode.OpenExisting,
                association),
            TestContext.Current.CancellationToken);
        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        await using var existingOutput = open.Value!.Output;
        var destinationArtifacts = SnapshotExistingArtifacts(open.Value.Baseline);
        var noOpDirectory = fixture.OutputDirectory.CreateSubdirectory("localized-noop-stage");
        var noOp = await adapter.WriteAndValidateAsync(
            sources,
            existingOutput,
            new PluginWriteRequest(noOpDirectory.FullName, open.Value.Association, open.Value.Baseline),
            TestContext.Current.CancellationToken);
        noOp.Succeeded.ShouldBeTrue(noOp.Error?.Message);
        noOp.Value!.Disposition.ShouldBe(PluginWriteDisposition.Unchanged);
        noOpDirectory.EnumerateFileSystemInfos().ShouldBeEmpty();

        await using var candidate = adapter.CloneOutput(existingOutput, TestContext.Current.CancellationToken);
        var target = candidate.ShouldBeOfType<Fallout4PluginOutputState>()
            .CreateSnapshot(TestContext.Current.CancellationToken)
            .FormLists
            .ShouldHaveSingleItem()
            .FormKey;
        var beginExisting = adapter.BeginEdit(
            sources,
            candidate,
            new BeginEditRequest(
                Guid.NewGuid(),
                sources.Revision,
                FormListEditRole.ExistingOutput,
                targetFormKey: target),
            TestContext.Current.CancellationToken);
        beginExisting.Succeeded.ShouldBeTrue(beginExisting.Error?.Message);
        adapter.ApplyEdit(
            sources,
            candidate,
            target,
            adapter.PrepareEdit(new SetEditorIdEdit("ChangedLocalizedList"))).Succeeded.ShouldBeTrue();
        var rejectedDirectory = fixture.OutputDirectory.CreateSubdirectory("localized-rejected-stage");
        var rejected = await adapter.WriteAndValidateAsync(
            sources,
            candidate,
            new PluginWriteRequest(rejectedDirectory.FullName, open.Value.Association, open.Value.Baseline),
            TestContext.Current.CancellationToken);

        rejected.Succeeded.ShouldBeFalse();
        rejected.Error!.Code.ShouldBe(EngineErrorCode.UnsupportedInput);
        rejectedDirectory.EnumerateFileSystemInfos().ShouldBeEmpty();
        AssertArtifactsUnchanged(destinationArtifacts);
        AssertArtifactsUnchanged(sourceArtifacts);
    }

    /// <summary>Creates the concrete adapter graph without a service locator or duplicated plugin state.</summary>
    /// <param name="outputService">Returns the shared typed output service for staged inspection.</param>
    /// <returns>A complete Fallout 4 game adapter.</returns>
    private static Fallout4FormListGameAdapter CreateAdapter(out Fallout4PluginOutputService outputService)
    {
        var sourceLoader = new Fallout4PluginSourceLoader(new PluginSourceInputLoader());
        outputService = new Fallout4PluginOutputService(new PluginOutputInputLoader());
        var editService = new Fallout4RecordEditService(outputService.Inspector);
        var writeService = new Fallout4PluginWriteService(outputService);
        return new Fallout4FormListGameAdapter(sourceLoader, outputService, editService, writeService);
    }

    /// <summary>Opens the generated Fallout 4 source set through the complete adapter.</summary>
    /// <param name="adapter">The complete game adapter.</param>
    /// <param name="fixture">The generated source and output fixture.</param>
    /// <returns>The typed independently owned source lifetime.</returns>
    private static async Task<Fallout4PluginSourceSet> OpenSourcesAsync(
        Fallout4FormListGameAdapter adapter,
        Fallout4PluginOutputTestFixture fixture)
    {
        var result = await adapter.OpenSourcesAsync(
            fixture.Sources.CreateOpenRequest(),
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value!.Sources.ShouldBeOfType<Fallout4PluginSourceSet>();
    }

    /// <summary>Asserts that every generated source plugin and strings artifact retains its exact bytes.</summary>
    /// <param name="expected">The pre-operation artifact bytes keyed by path.</param>
    private static void AssertArtifactsUnchanged(IReadOnlyDictionary<string, byte[]> expected)
    {
        expected.ShouldNotBeEmpty();
        foreach (var artifact in expected)
        {
            File.ReadAllBytes(artifact.Key).ShouldBe(artifact.Value);
        }
    }

    /// <summary>Copies a validated private staged set into initially absent test destinations to create an existing localized fixture.</summary>
    /// <param name="mappings">The explicit verified staged-to-destination mappings.</param>
    private static void CommitStagedArtifacts(IReadOnlyList<StagedPluginArtifactMapping> mappings)
    {
        foreach (var mapping in mappings.Where(mapping => mapping.StagedArtifact.Fingerprint.Exists))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(mapping.DestinationPath)!);
            File.Copy(mapping.StagedArtifact.Path, mapping.DestinationPath, overwrite: false);
        }
    }

    /// <summary>Snapshots every present artifact in one completed existing-output baseline.</summary>
    /// <param name="baseline">The complete output baseline.</param>
    /// <returns>Exact bytes for every present plugin and strings artifact.</returns>
    private static IReadOnlyDictionary<string, byte[]> SnapshotExistingArtifacts(OutputArtifactSetBaseline baseline)
    {
        return baseline.Artifacts
            .Where(artifact => artifact.Fingerprint.Exists)
            .ToDictionary(
                artifact => artifact.Path,
                artifact => File.ReadAllBytes(artifact.Path),
                StringComparer.OrdinalIgnoreCase);
    }
}
