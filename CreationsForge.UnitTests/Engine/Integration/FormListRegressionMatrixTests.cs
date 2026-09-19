using System.Text.Json;
using CreationsForge.Bootstrap.Composition;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using CreationsForge.Fallout4.PluginAdapter.Edits;
using CreationsForge.Skyrim.PluginAdapter;
using CreationsForge.Starfield.PluginAdapter.Edits;
using CreationsForge.Starfield.PluginAdapter.RecordInspection;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Strings;
using Noggog;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Integration;

/// <summary>Closes deterministic cross-game FormList regression coverage through the production guarded-save composition.</summary>
public sealed class FormListRegressionMatrixTests
{
    /// <summary>The exact common version-control value expected after plugin save and reopen.</summary>
    private const uint ExpectedVersionControl = 0x10203040;

    /// <summary>The exact common FormVersion value expected after plugin save and reopen.</summary>
    private const ushort ExpectedFormVersion = 55;

    /// <summary>The exact common Version2 value expected after plugin save and reopen.</summary>
    private const ushort ExpectedVersion2 = 7;

    /// <summary>The raw plugin compressed-record bit set by every supported game adapter.</summary>
    private const int CompressedRecordFlag = 0x00040000;

    /// <summary>Verifies a rejected small-master save leaves the staged record discardable without creating an output artifact.</summary>
    /// <returns>A task that completes after the original empty output has been reopened.</returns>
    [Fact]
    public async Task StarfieldRejectedEmbeddedTranslationSave_CanDiscardNewOutput()
    {
        using var fixture = WorkspaceIntegrationFixture.Create(SupportedGame.Starfield);
        var outputDirectory = fixture.RootDirectory.CreateSubdirectory("RejectedSmallMaster");
        var outputPath = Path.Combine(outputDirectory.FullName, "RejectedSmallMaster.esm");
        var association = new OutputAssociation(
            outputPath,
            ModKey.FromNameAndExtension("RejectedSmallMaster.esm"),
            LocalizedOutputMode.Embedded,
            OutputMasterStyle.Small);
        using var logger = new LoggerConfiguration().CreateLogger();
        await using var services = EngineComposition.Create(logger);
        var opened = await services.WorkspaceFactory.OpenAsync(fixture.CreateOpenRequest(), TestContext.Current.CancellationToken);
        opened.Succeeded.ShouldBeTrue(DescribeError(opened.Error));
        await using var workspace = opened.Value!;
        var selected = await SelectOutputAsync(workspace, association, OutputSelectionMode.CreateNew);
        var edit = await BeginEditAsync(workspace, FormListEditRole.New);
        var name = new TranslatedString(Language.English, "English name");
        name.Set(Language.French, "Nom francais");
        await ApplyEditAsync(workspace, edit.EditId, new StarfieldSetNameEdit(name));

        var save = await workspace.SaveAsync(
            new SaveRequest(Guid.NewGuid(), workspace.Revision, selected.Baseline),
            TestContext.Current.CancellationToken);
        save.Status.ShouldBe(SaveCommitStatus.NotCommitted);
        save.Error!.Message.ShouldContain("changed field 'Name'");
        File.Exists(outputPath).ShouldBeFalse();

        var discarded = await workspace.DiscardChangesAsync(
            new DiscardChangesRequest(Guid.NewGuid(), workspace.Revision, selected.Baseline),
            TestContext.Current.CancellationToken);
        discarded.Succeeded.ShouldBeTrue(DescribeError(discarded.Error));
        var preview = await workspace.PreviewAsync(TestContext.Current.CancellationToken);
        preview.Succeeded.ShouldBeTrue(DescribeError(preview.Error));
        preview.Value!.HasStagedChanges.ShouldBeFalse();
        File.Exists(outputPath).ShouldBeFalse();
    }

    /// <summary>Identifies each owner-derived representation exercised for a Starfield link-or-index condition parameter.</summary>
    public enum StarfieldConditionParameterMode
    {
        /// <summary>Uses a concrete plugin FormKey link.</summary>
        Link,

        /// <summary>Uses the plugin null FormKey representation while link mode remains active.</summary>
        NullLink,

        /// <summary>Uses an alias index selected by the condition-data owner.</summary>
        Alias,

        /// <summary>Uses a package-data index selected by the condition-data owner.</summary>
        PackageData,
    }

    /// <summary>Verifies a committed source override preserves a previously committed empty output record and every selected record value.</summary>
    /// <param name="game">The plugin game whose complete production composition is exercised.</param>
    /// <returns>A task that completes after guarded commits, fresh reopen, and disposal of both workspaces.</returns>
    [Theory]
    [InlineData(SupportedGame.Starfield)]
    [InlineData(SupportedGame.Fallout4)]
    [InlineData(SupportedGame.Skyrim)]
    public async Task EmbeddedOutput_SourceOverridePreservesAllocatedEmptyRecordAndFreshlyReopens(SupportedGame game)
    {
        using var fixture = WorkspaceIntegrationFixture.Create(game);
        var sourceArtifacts = fixture.SnapshotArtifacts();
        var association = CreateAssociation(fixture, $"{game}RegressionMatrix.esm");
        var logSink = new CollectingLogSink();
        using var logger = new LoggerConfiguration().MinimumLevel.Verbose().WriteTo.Sink(logSink).CreateLogger();
        await using var services = EngineComposition.Create(logger);
        FormKey allocatedFormKey;
        string allocatedRecordBeforeOverride;

        var open = await services.WorkspaceFactory.OpenAsync(
            fixture.CreateOpenRequest(),
            TestContext.Current.CancellationToken);
        open.Succeeded.ShouldBeTrue(DescribeError(open.Error));
        await using (var workspace = open.Value!)
        {
            var selection = await SelectOutputAsync(workspace, association, OutputSelectionMode.CreateNew);
            var allocatedEdit = await BeginEditAsync(workspace, FormListEditRole.New);
            allocatedFormKey = allocatedEdit.FormKey;
            allocatedEdit.OriginFormKey.ShouldBeNull();
            allocatedEdit.FormKey.ModKey.ShouldBe(association.ModKey);
            await ApplyEditAsync(workspace, allocatedEdit.EditId, new SetEditorIdEdit("PreservedEmptyList"));
            await ApplyEditAsync(workspace, allocatedEdit.EditId, new ReplaceItemsEdit([fixture.BookFormKey]));
            await ApplyEditAsync(workspace, allocatedEdit.EditId, new ClearItemsEdit());

            var firstSave = await SaveAsync(workspace, selection.Baseline);
            allocatedRecordBeforeOverride = (await ReadRecordAsync(workspace, association, allocatedFormKey, logSink)).GetRawText();
            using (var allocatedDocument = JsonDocument.Parse(allocatedRecordBeforeOverride))
            {
                allocatedDocument.RootElement.GetProperty("EditorID").GetString().ShouldBe("PreservedEmptyList");
                allocatedDocument.RootElement.GetProperty("Items").GetArrayLength().ShouldBe(0);
            }

            fixture.ReadEmbeddedOutputMasters(association.PluginPath).ShouldBeEmpty();

            var overrideEdit = await BeginEditAsync(
                workspace,
                FormListEditRole.Override,
                fixture.SourceListFormKey);
            overrideEdit.Role.ShouldBe(FormListEditRole.Override);
            overrideEdit.FormKey.ShouldBe(fixture.SourceListFormKey);
            overrideEdit.OriginFormKey.ShouldBe(fixture.SourceListFormKey);
            overrideEdit.FormKey.ShouldNotBe(allocatedFormKey);
            foreach (var edit in CreateOverrideEdits(game, fixture))
            {
                await ApplyEditAsync(workspace, overrideEdit.EditId, edit);
            }

            var preview = await workspace.PreviewAsync(TestContext.Current.CancellationToken);
            preview.Succeeded.ShouldBeTrue(DescribeError(preview.Error));
            var comparison = preview.Value!.Comparisons
                .Single(candidate => candidate.FormKey == fixture.SourceListFormKey);
            comparison.Before.ShouldNotBeNull();
            comparison.After.ShouldNotBeNull();
            var previewRecord = comparison.After!.Value;
            AssertOverrideRecord(previewRecord, game, fixture);

            var secondSave = await SaveAsync(
                workspace,
                firstSave.CommittedBaseline.ShouldNotBeNull(),
                game == SupportedGame.Starfield ? previewRecord : null,
                fixture.SourceListFormKey);
            secondSave.CommittedBaseline!.BaselineId.ShouldNotBe(firstSave.CommittedBaseline.BaselineId);
            var savedOverride = await ReadRecordAsync(workspace, association, fixture.SourceListFormKey, logSink);
            AssertOverrideRecord(savedOverride, game, fixture);
            (await ReadRecordAsync(workspace, association, allocatedFormKey, logSink)).GetRawText()
                .ShouldBe(allocatedRecordBeforeOverride);
        }

        var reopened = await services.WorkspaceFactory.OpenAsync(
            fixture.CreateOpenRequest(),
            TestContext.Current.CancellationToken);
        reopened.Succeeded.ShouldBeTrue(DescribeError(reopened.Error));
        await using (var workspace = reopened.Value!)
        {
            await SelectOutputAsync(workspace, association, OutputSelectionMode.OpenExisting);
            var reopenedOverride = await ReadRecordAsync(workspace, association, fixture.SourceListFormKey, logSink);
            AssertOverrideRecord(reopenedOverride, game, fixture);
            (await ReadRecordAsync(workspace, association, allocatedFormKey, logSink)).GetRawText()
                .ShouldBe(allocatedRecordBeforeOverride);
        }

        fixture.ReadEmbeddedOutputMasters(association.PluginPath)
            .ShouldBe([fixture.SourceModKey], ignoreOrder: false);
        AssertArtifactsUnchanged(sourceArtifacts, fixture.SnapshotArtifacts());
    }

    /// <summary>Verifies every Starfield link-or-index mode preserves its active record value across save and fresh reopen.</summary>
    /// <param name="mode">The owner-derived plugin condition-parameter mode.</param>
    /// <returns>A task that completes after unrelated output and source preservation are proven.</returns>
    [Theory]
    [InlineData(StarfieldConditionParameterMode.Link)]
    [InlineData(StarfieldConditionParameterMode.NullLink)]
    [InlineData(StarfieldConditionParameterMode.Alias)]
    [InlineData(StarfieldConditionParameterMode.PackageData)]
    public async Task StarfieldConditionParameterModes_SaveAndFreshlyReopen(
        StarfieldConditionParameterMode mode)
    {
        using var fixture = WorkspaceIntegrationFixture.Create(SupportedGame.Starfield);
        var sourceArtifacts = fixture.SnapshotArtifacts();
        var association = CreateAssociation(fixture, $"Starfield{mode}Condition.esm");
        var logSink = new CollectingLogSink();
        using var logger = new LoggerConfiguration().MinimumLevel.Verbose().WriteTo.Sink(logSink).CreateLogger();
        await using var services = EngineComposition.Create(logger);
        FormKey targetFormKey;
        FormKey preservedFormKey;
        string preservedRecord;

        var open = await services.WorkspaceFactory.OpenAsync(
            fixture.CreateOpenRequest(),
            TestContext.Current.CancellationToken);
        open.Succeeded.ShouldBeTrue(DescribeError(open.Error));
        await using (var workspace = open.Value!)
        {
            var selection = await SelectOutputAsync(workspace, association, OutputSelectionMode.CreateNew);
            var targetEdit = await BeginEditAsync(workspace, FormListEditRole.New);
            targetFormKey = targetEdit.FormKey;
            await ApplyEditAsync(workspace, targetEdit.EditId, new SetEditorIdEdit($"{mode}Condition"));
            var preservedEdit = await BeginEditAsync(workspace, FormListEditRole.New);
            preservedFormKey = preservedEdit.FormKey;
            await ApplyEditAsync(workspace, preservedEdit.EditId, new SetEditorIdEdit("PreservedSibling"));
            var firstSave = await SaveAsync(workspace, selection.Baseline);
            preservedRecord = (await ReadRecordAsync(workspace, association, preservedFormKey, logSink)).GetRawText();

            var existingEdit = await BeginEditAsync(
                workspace,
                FormListEditRole.ExistingOutput,
                targetFormKey: targetFormKey);
            await ApplyEditAsync(
                workspace,
                existingEdit.EditId,
                new StarfieldSetConditionalEntriesEdit([
                    CreateStarfieldModeCondition(mode, fixture.KeywordFormKey),
                ]));
            var preview = await workspace.PreviewAsync(TestContext.Current.CancellationToken);
            preview.Succeeded.ShouldBeTrue(DescribeError(preview.Error));
            var expectedCandidateResult = preview.Value!.Comparisons
                .Single(comparison => comparison.FormKey == targetFormKey)
                .After;
            expectedCandidateResult.ShouldNotBeNull();

            var secondSave = await SaveAsync(
                workspace,
                firstSave.CommittedBaseline.ShouldNotBeNull(),
                expectedCandidateResult.Value,
                targetFormKey);
            secondSave.CommittedBaseline!.BaselineId.ShouldNotBe(firstSave.CommittedBaseline.BaselineId);
            AssertStarfieldConditionParameter(
                await ReadRecordAsync(workspace, association, targetFormKey, logSink),
                mode,
                fixture.KeywordFormKey);
            (await ReadRecordAsync(workspace, association, preservedFormKey, logSink)).GetRawText()
                .ShouldBe(preservedRecord);
        }

        var reopened = await services.WorkspaceFactory.OpenAsync(
            fixture.CreateOpenRequest(),
            TestContext.Current.CancellationToken);
        reopened.Succeeded.ShouldBeTrue(DescribeError(reopened.Error));
        await using (var workspace = reopened.Value!)
        {
            await SelectOutputAsync(workspace, association, OutputSelectionMode.OpenExisting);
            AssertStarfieldConditionParameter(
                await ReadRecordAsync(workspace, association, targetFormKey, logSink),
                mode,
                fixture.KeywordFormKey);
            (await ReadRecordAsync(workspace, association, preservedFormKey, logSink)).GetRawText()
                .ShouldBe(preservedRecord);
        }

        var expectedMasters = mode == StarfieldConditionParameterMode.Link
            ? new[] { fixture.SourceModKey }
            : Array.Empty<ModKey>();
        fixture.ReadEmbeddedOutputMasters(association.PluginPath)
            .ShouldBe(expectedMasters, ignoreOrder: false);
        AssertArtifactsUnchanged(sourceArtifacts, fixture.SnapshotArtifacts());
    }

    /// <summary>Creates the exact embedded full-master output association owned by one generated fixture.</summary>
    /// <param name="fixture">The generated cross-game fixture.</param>
    /// <param name="fileName">The absent output plugin file name.</param>
    /// <returns>A create-new association inside the fixture-owned output directory.</returns>
    private static OutputAssociation CreateAssociation(
        WorkspaceIntegrationFixture fixture,
        string fileName)
    {
        var outputDirectory = fixture.RootDirectory.CreateSubdirectory("RegressionMatrixOutput");
        return new OutputAssociation(
            Path.Combine(outputDirectory.FullName, fileName),
            ModKey.FromNameAndExtension(fileName),
            LocalizedOutputMode.Embedded,
            OutputMasterStyle.Full);
    }

    /// <summary>Selects an absent or existing output through the public production workspace contract.</summary>
    /// <param name="workspace">The independently owned workspace.</param>
    /// <param name="association">The exact output identity and representation.</param>
    /// <param name="mode">Whether the output must be absent or present.</param>
    /// <returns>The successful output-selection receipt.</returns>
    private static async Task<OutputSelectionReceipt> SelectOutputAsync(
        IPluginWorkspace workspace,
        OutputAssociation association,
        OutputSelectionMode mode)
    {
        var result = await workspace.SelectOutputAsync(
            new SelectOutputRequest(Guid.NewGuid(), workspace.Revision, mode, association),
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(DescribeError(result.Error));
        return result.Value!;
    }

    /// <summary>Begins and publishes one new-record or source-override edit session.</summary>
    /// <param name="workspace">The selected workspace.</param>
    /// <param name="role">The requested record edit role.</param>
    /// <param name="originFormKey">The source FormList identity for an override, otherwise <see langword="null"/>.</param>
    /// <param name="targetFormKey">The output FormList identity for an existing-output edit, otherwise <see langword="null"/>.</param>
    /// <returns>The successful stable edit identity.</returns>
    private static async Task<EditReceipt> BeginEditAsync(
        IPluginWorkspace workspace,
        FormListEditRole role,
        FormKey? originFormKey = null,
        FormKey? targetFormKey = null)
    {
        var result = await workspace.BeginEditAsync(
            new BeginEditRequest(
                Guid.NewGuid(),
                workspace.Revision,
                role,
                originFormKey,
                targetFormKey: targetFormKey),
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(DescribeError(result.Error));
        return result.Value!;
    }

    /// <summary>Applies and publishes one typed FormList mutation through the public workspace contract.</summary>
    /// <param name="workspace">The workspace that owns the edit session.</param>
    /// <param name="editId">The stable edit-session identity.</param>
    /// <param name="edit">The typed domain edit to apply.</param>
    /// <returns>A task that completes when the candidate is published or the assertion fails.</returns>
    private static async Task ApplyEditAsync(
        IPluginWorkspace workspace,
        Guid editId,
        FormListEdit edit)
    {
        var result = await workspace.ApplyFormListEditAsync(
            new FormListEditRequest(Guid.NewGuid(), workspace.Revision, editId, edit),
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(DescribeError(result.Error));
        result.ResultRevision.ShouldBe(workspace.Revision);
    }

    /// <summary>Runs one guarded save and asserts that the workspace adopted the reopened committed output.</summary>
    /// <param name="workspace">The dirty workspace.</param>
    /// <param name="expectedBaseline">The exact currently selected output baseline.</param>
    /// <param name="expectedStarfieldRecord">The expected complete Starfield read view used only to diagnose a staged-reopen rejection.</param>
    /// <param name="expectedStarfieldFormKey">The Starfield FormList identity corresponding to <paramref name="expectedStarfieldRecord"/>.</param>
    /// <returns>The committed save result with its adopted baseline.</returns>
    private static async Task<SaveResult> SaveAsync(
        IPluginWorkspace workspace,
        OutputArtifactSetBaseline expectedBaseline,
        JsonElement? expectedStarfieldRecord = null,
        FormKey? expectedStarfieldFormKey = null)
    {
        var operationId = Guid.NewGuid();
        var result = await workspace.SaveAsync(
            new SaveRequest(operationId, workspace.Revision, expectedBaseline),
            TestContext.Current.CancellationToken);
        var stagedDifference = result.Status != SaveCommitStatus.Committed
            && expectedStarfieldRecord.HasValue
            && expectedStarfieldFormKey.HasValue
            ? DescribeStarfieldStagedDifference(
                workspace,
                operationId,
                expectedBaseline,
                expectedStarfieldFormKey.Value,
                expectedStarfieldRecord.Value)
            : "Starfield staged difference: <not requested>.";
        result.Status.ShouldBe(
            SaveCommitStatus.Committed,
            $"{DescribeSaveFailure(result, workspace)}; {stagedDifference}");
        result.Error.ShouldBeNull();
        result.CommittedBaseline.ShouldNotBeNull();
        result.ResultRevision.ShouldBe(workspace.Revision);
        workspace.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.Ready);
        return result;
    }

    /// <summary>Reads one exact output FormList through the detached plugin inspector view.</summary>
    /// <param name="workspace">The selected workspace.</param>
    /// <param name="association">The selected output identity.</param>
    /// <param name="formKey">The exact output FormList identity.</param>
    /// <param name="logSink">The test-owned structured log sink used to expose an underlying plugin reader exception.</param>
    /// <returns>The complete detached record JSON record.</returns>
    private static async Task<JsonElement> ReadRecordAsync(
        IPluginWorkspace workspace,
        OutputAssociation association,
        FormKey formKey,
        CollectingLogSink logSink)
    {
        var result = await workspace.ReadFormListViewAsync(
            new ReferenceRequest(formKey, RecordScope.StagedOutput, association.ModKey),
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue($"{DescribeError(result.Error)}; {logSink.DescribeExceptions()}");
        result.Value!.Record.ShouldNotBeNull();
        return result.Value.Record!.Value;
    }

    /// <summary>Builds the exact typed edits applied to the source override for one game.</summary>
    /// <param name="game">The game that owns the mutable FormList.</param>
    /// <param name="fixture">The generated source identities used by the typed edits.</param>
    /// <returns>The ordered common and game-specific edit sequence.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="game"/> is undefined.</exception>
    private static IReadOnlyList<FormListEdit> CreateOverrideEdits(
        SupportedGame game,
        WorkspaceIntegrationFixture fixture)
    {
        var edits = new List<FormListEdit>
        {
            new SetEditorIdEdit($"{game}MatrixOverride"),
            new SetVersionControlEdit(ExpectedVersionControl),
            new SetFormVersionEdit(ExpectedFormVersion),
            new SetVersion2Edit(ExpectedVersion2),
            new ReplaceItemsEdit([fixture.BookFormKey, fixture.BookFormKey, FormKey.Null]),
        };

        switch (game)
        {
            case SupportedGame.Starfield:
                edits.Add(new StarfieldSetMajorFlagsEdit(
                    StarfieldMajorRecord.StarfieldMajorRecordFlag.InitiallyDisabled));
                edits.Add(new StarfieldSetNameEdit(
                    new TranslatedString(Language.English, "Starfield matrix name")));
                edits.Add(new StarfieldSetAddToListEdit(fixture.SourceListFormKey));
                edits.Add(new StarfieldAddComponentEdit(0, CreateStarfieldComponent(fixture.KeywordFormKey)));
                edits.Add(new StarfieldSetConditionalEntriesEdit([
                    CreateStarfieldConditionalEntry(fixture.KeywordFormKey),
                ]));
                break;
            case SupportedGame.Fallout4:
                edits.Add(new Fallout4SetMajorRecordFlagsEdit(
                    Fallout4MajorRecord.Fallout4MajorRecordFlag.NotPlayable));
                edits.Add(new Fallout4SetNameEdit(
                    new TranslatedString(Language.English, "Fallout 4 matrix name")));
                break;
            case SupportedGame.Skyrim:
                edits.Add(new SkyrimSetMajorRecordFlagsEdit(
                    SkyrimMajorRecord.SkyrimMajorRecordFlag.NotPlayable));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(game), game, "The regression matrix requires a supported plugin game.");
        }

        edits.Add(new SetCompressedEdit(true));
        return edits;
    }

    /// <summary>Creates one representative Starfield component with a nested typed Keyword link.</summary>
    /// <param name="keywordFormKey">The resolvable source Keyword identity retained by the component.</param>
    /// <returns>A complete caller-owned component payload.</returns>
    private static AttachParentArrayComponent CreateStarfieldComponent(FormKey keywordFormKey)
    {
        return new AttachParentArrayComponent
        {
            Slots = new ExtendedList<IFormLinkGetter<Mutagen.Bethesda.Starfield.IKeywordGetter>>
            {
                new FormLink<Mutagen.Bethesda.Starfield.IKeywordGetter>(keywordFormKey),
            },
        };
    }

    /// <summary>Creates one representative Starfield conditional entry with a concrete link-mode condition-data graph.</summary>
    /// <param name="keywordFormKey">The resolvable source Keyword retained by the active plugin condition parameter.</param>
    /// <returns>A complete caller-owned conditional entry.</returns>
    private static FormListConditionalEntry CreateStarfieldConditionalEntry(FormKey keywordFormKey)
    {
        var data = new BiomeHasKeywordConditionData();
        data.FirstParameter.Link.SetTo(keywordFormKey);
        return new FormListConditionalEntry
        {
            Index = 17,
            Conditions = new ExtendedList<Mutagen.Bethesda.Starfield.Condition>
            {
                new Mutagen.Bethesda.Starfield.ConditionFloat
                {
                    ComparisonValue = 1.0f,
                    Data = data,
                },
            },
        };
    }

    /// <summary>Creates a Starfield condition whose owner selects the requested active link-or-index representation.</summary>
    /// <param name="mode">The plugin owner mode to encode.</param>
    /// <param name="keywordFormKey">The resolvable source Keyword used by concrete link mode.</param>
    /// <returns>A complete caller-owned conditional entry with one active parameter value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="mode"/> is undefined.</exception>
    private static FormListConditionalEntry CreateStarfieldModeCondition(
        StarfieldConditionParameterMode mode,
        FormKey keywordFormKey)
    {
        var data = new BiomeHasKeywordConditionData
        {
            UseAliases = mode == StarfieldConditionParameterMode.Alias,
            UsePackageData = mode == StarfieldConditionParameterMode.PackageData,
        };
        switch (mode)
        {
            case StarfieldConditionParameterMode.Link:
                data.FirstParameter.Link.SetTo(keywordFormKey);
                break;
            case StarfieldConditionParameterMode.NullLink:
                data.FirstParameter.Link.SetTo(FormKey.Null);
                break;
            case StarfieldConditionParameterMode.Alias:
            case StarfieldConditionParameterMode.PackageData:
                data.FirstParameter.Index = 17;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, "The regression matrix requires a supported Starfield condition-parameter mode.");
        }

        return new FormListConditionalEntry
        {
            Index = 17,
            Conditions = new ExtendedList<Mutagen.Bethesda.Starfield.Condition>
            {
                new Mutagen.Bethesda.Starfield.ConditionFloat
                {
                    ComparisonValue = 1.0f,
                    Data = data,
                },
            },
        };
    }

    /// <summary>Asserts the active owner-derived Starfield condition parameter survives plugin save and reopen.</summary>
    /// <param name="record">The complete detached FormList JSON record.</param>
    /// <param name="mode">The plugin owner mode expected in the condition data.</param>
    /// <param name="keywordFormKey">The exact source Keyword expected by concrete link mode.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="mode"/> is undefined.</exception>
    private static void AssertStarfieldConditionParameter(
        JsonElement record,
        StarfieldConditionParameterMode mode,
        FormKey keywordFormKey)
    {
        var entry = record.GetProperty("ConditionalEntries").EnumerateArray().ToArray().ShouldHaveSingleItem();
        entry.GetProperty("Index").GetUInt32().ShouldBe(17U);
        var condition = entry.GetProperty("Conditions").EnumerateArray().ToArray().ShouldHaveSingleItem();
        condition.GetProperty("$type").GetString().ShouldNotBeNull()
            .ShouldEndWith(":Mutagen.Bethesda.Starfield.ConditionFloat");
        condition.GetProperty("ComparisonValue").GetProperty("bits").GetString()
            .ShouldBe("0x3F800000");
        var data = condition.GetProperty("Data");
        data.GetProperty("$type").GetString().ShouldNotBeNull()
            .ShouldEndWith(":Mutagen.Bethesda.Starfield.BiomeHasKeywordConditionData");
        data.GetProperty("UseAliases").GetBoolean()
            .ShouldBe(mode == StarfieldConditionParameterMode.Alias);
        data.GetProperty("UsePackageData").GetBoolean()
            .ShouldBe(mode == StarfieldConditionParameterMode.PackageData);

        var parameter = data.GetProperty("FirstParameter");
        parameter.GetProperty("usesLink").GetBoolean()
            .ShouldBe(mode is StarfieldConditionParameterMode.Link or StarfieldConditionParameterMode.NullLink);
        parameter.GetProperty("usesAlias").GetBoolean()
            .ShouldBe(mode == StarfieldConditionParameterMode.Alias);
        parameter.GetProperty("usesPackageData").GetBoolean()
            .ShouldBe(mode == StarfieldConditionParameterMode.PackageData);

        switch (mode)
        {
            case StarfieldConditionParameterMode.Link:
                var link = parameter.GetProperty("link");
                link.GetProperty("isNull").GetBoolean().ShouldBeFalse();
                link.GetProperty("formKey").GetString().ShouldBe(keywordFormKey.ToString());
                break;
            case StarfieldConditionParameterMode.NullLink:
                var nullLink = parameter.GetProperty("link");
                nullLink.GetProperty("isNull").GetBoolean().ShouldBeTrue();
                nullLink.GetProperty("formKey").GetString().ShouldBe(FormKey.Null.ToString());
                break;
            case StarfieldConditionParameterMode.Alias:
            case StarfieldConditionParameterMode.PackageData:
                parameter.GetProperty("index").GetUInt32().ShouldBe(17U);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, "The regression matrix requires a supported Starfield condition-parameter mode.");
        }
    }

    /// <summary>Asserts every common and game-specific value selected for the committed override matrix.</summary>
    /// <param name="record">The complete detached FormList JSON record.</param>
    /// <param name="game">The game that owns the record.</param>
    /// <param name="fixture">The generated plugin identities expected in links and nested payloads.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="game"/> is undefined.</exception>
    private static void AssertOverrideRecord(
        JsonElement record,
        SupportedGame game,
        WorkspaceIntegrationFixture fixture)
    {
        record.GetProperty("FormKey").GetString().ShouldBe(fixture.SourceListFormKey.ToString());
        record.GetProperty("EditorID").GetString().ShouldBe($"{game}MatrixOverride");
        record.GetProperty("VersionControl").GetUInt32().ShouldBe(ExpectedVersionControl);
        record.GetProperty("FormVersion").GetUInt16().ShouldBe(ExpectedFormVersion);
        record.GetProperty("Version2").GetUInt16().ShouldBe(ExpectedVersion2);
        (record.GetProperty("MajorRecordFlagsRaw").GetInt32() & CompressedRecordFlag)
            .ShouldBe(CompressedRecordFlag);

        var items = record.GetProperty("Items").EnumerateArray().ToArray();
        items.Select(item => item.GetProperty("isNull").GetBoolean())
            .ShouldBe([false, false, true]);
        items.Select(item => item.GetProperty("formKey").GetString())
            .ShouldBe([
                fixture.BookFormKey.ToString(),
                fixture.BookFormKey.ToString(),
                FormKey.Null.ToString(),
            ]);

        switch (game)
        {
            case SupportedGame.Starfield:
                AssertFlag(record, "StarfieldMajorRecordFlags", (int)StarfieldMajorRecord.StarfieldMajorRecordFlag.InitiallyDisabled);
                AssertEmbeddedName(record, "Starfield matrix name");
                record.GetProperty("AddToList").GetProperty("formKey").GetString()
                    .ShouldBe(fixture.SourceListFormKey.ToString());
                var component = record.GetProperty("Components").EnumerateArray().ToArray().ShouldHaveSingleItem();
                component.GetProperty("$type").GetString().ShouldNotBeNull()
                    .ShouldEndWith(":Mutagen.Bethesda.Starfield.AttachParentArrayComponent");
                component.GetProperty("Slots")[0].GetProperty("formKey").GetString()
                    .ShouldBe(fixture.KeywordFormKey.ToString());
                var conditionalEntry = record.GetProperty("ConditionalEntries").EnumerateArray().ToArray().ShouldHaveSingleItem();
                conditionalEntry.GetProperty("Index").GetUInt32().ShouldBe(17U);
                var condition = conditionalEntry.GetProperty("Conditions").EnumerateArray().ToArray().ShouldHaveSingleItem();
                condition.GetProperty("$type").GetString().ShouldNotBeNull()
                    .ShouldEndWith(":Mutagen.Bethesda.Starfield.ConditionFloat");
                condition.GetProperty("ComparisonValue").GetProperty("bits").GetString()
                    .ShouldBe("0x3F800000");
                condition.GetProperty("Data").GetProperty("$type").GetString().ShouldNotBeNull()
                    .ShouldEndWith(":Mutagen.Bethesda.Starfield.BiomeHasKeywordConditionData");
                var firstParameter = condition.GetProperty("Data").GetProperty("FirstParameter");
                firstParameter.GetProperty("usesLink").GetBoolean().ShouldBeTrue();
                firstParameter.GetProperty("usesAlias").GetBoolean().ShouldBeFalse();
                firstParameter.GetProperty("usesPackageData").GetBoolean().ShouldBeFalse();
                firstParameter.GetProperty("link").GetProperty("formKey").GetString()
                    .ShouldBe(fixture.KeywordFormKey.ToString());
                break;
            case SupportedGame.Fallout4:
                AssertFlag(record, "Fallout4MajorRecordFlags", (int)Fallout4MajorRecord.Fallout4MajorRecordFlag.NotPlayable);
                AssertEmbeddedName(record, "Fallout 4 matrix name");
                break;
            case SupportedGame.Skyrim:
                AssertFlag(record, "SkyrimMajorRecordFlags", (int)SkyrimMajorRecord.SkyrimMajorRecordFlag.NotPlayable);
                record.TryGetProperty("Name", out _).ShouldBeFalse();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(game), game, "The regression matrix requires a supported plugin game.");
        }
    }

    /// <summary>Asserts one expected typed major-record bit remains set after plugin serialization.</summary>
    /// <param name="record">The complete detached FormList JSON record.</param>
    /// <param name="propertyName">The game-specific typed flag property.</param>
    /// <param name="expectedFlag">The exact bit that must remain set.</param>
    private static void AssertFlag(JsonElement record, string propertyName, int expectedFlag)
    {
        (record.GetProperty(propertyName).GetInt32() & expectedFlag).ShouldBe(expectedFlag);
    }

    /// <summary>Asserts an embedded plugin Name retains its exact English value.</summary>
    /// <param name="record">The complete detached FormList JSON record.</param>
    /// <param name="expectedValue">The exact English text expected after reopen.</param>
    private static void AssertEmbeddedName(JsonElement record, string expectedValue)
    {
        var name = record.GetProperty("Name");
        name.GetProperty("targetLanguage").GetString().ShouldBe(Language.English.ToString());
        name.GetProperty("value").GetString().ShouldBe(expectedValue);
        name.GetProperty("translations").EnumerateArray()
            .Select(translation => translation.GetProperty("value").GetString())
            .ShouldBe([expectedValue]);
    }

    /// <summary>Asserts every source plugin and localized sidecar retains its exact path and bytes.</summary>
    /// <param name="expected">The source artifact snapshot captured before authoring.</param>
    /// <param name="actual">The source artifact snapshot captured after fresh reopen.</param>
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

    /// <summary>Reopens a retained failed Starfield stage and reports exact inspector JSON paths that changed from preview.</summary>
    /// <param name="workspace">The workspace that owns the failed save transaction.</param>
    /// <param name="operationId">The exact failed save operation identity.</param>
    /// <param name="expectedBaseline">The destination baseline that locates the transaction root.</param>
    /// <param name="formKey">The edited Starfield FormList identity.</param>
    /// <param name="expectedRecord">The complete expected read view captured immediately before save.</param>
    /// <returns>A bounded diagnostic describing exact changed, missing, or added JSON paths.</returns>
    private static string DescribeStarfieldStagedDifference(
        IPluginWorkspace workspace,
        Guid operationId,
        OutputArtifactSetBaseline expectedBaseline,
        FormKey formKey,
        JsonElement expectedRecord)
    {
        try
        {
            var destinationPath = expectedBaseline.Artifacts
                .Single(artifact => artifact.Role == PluginArtifactRole.Plugin)
                .Path;
            var stagedPath = Path.Combine(
                Path.GetDirectoryName(destinationPath)!,
                ".creationsforge-transactions",
                workspace.WorkspaceId.ToString("N"),
                operationId.ToString("N"),
                "stage",
                Path.GetFileName(destinationPath));
            if (!File.Exists(stagedPath))
            {
                return $"Starfield staged difference: retained plugin was absent at '{stagedPath}'.";
            }

            var stagedModKey = ModKey.FromNameAndExtension(Path.GetFileName(stagedPath));
            var masterFlags = new Cache<IModMasterStyledGetter, ModKey>(metadata => metadata.ModKey);
            masterFlags.Set(new KeyedMasterStyle(formKey.ModKey, Mutagen.Bethesda.Plugins.MasterStyle.Small));
            masterFlags.Set(new KeyedMasterStyle(stagedModKey, Mutagen.Bethesda.Plugins.MasterStyle.Full));
            var parameters = new BinaryReadParameters
            {
                MasterFlagsLookup = masterFlags,
                ThrowOnUnknownSubrecord = true,
            };
            var metadata = ParsingMeta.Factory(
                parameters,
                GameRelease.Starfield,
                new ModPath(stagedModKey, stagedPath));
            using var fileStream = File.OpenRead(stagedPath);
            using var binaryStream = new MutagenBinaryReadStream(
                fileStream,
                metadata,
                bufferSize: 4096,
                dispose: true,
                offsetReference: 0);
            var frame = new MutagenFrame(binaryStream);
            var stagedMod = StarfieldMod.CreateFromBinary(
                frame,
                StarfieldRelease.Starfield,
                new Mutagen.Bethesda.Starfield.GroupMask(true));
            var stagedRecord = stagedMod.FormLists.Single(record => record.FormKey == formKey);
            var actualRecord = WriteStarfieldReadView(stagedRecord);
            var differences = new List<string>();
            CollectJsonDifferences(expectedRecord, actualRecord, "$", differences);
            return differences.Count == 0
                ? "Starfield staged difference: complete inspector JSON was identical."
                : $"Starfield staged differences: {string.Join(" | ", differences)}";
        }
        catch (Exception exception)
        {
            return $"Starfield staged difference inspection failed: {exception}";
        }
    }

    /// <summary>Writes one detached Starfield FormList through the production complete plugin inspector.</summary>
    /// <param name="record">The strictly reopened staged FormList.</param>
    /// <returns>A detached complete inspector JSON value.</returns>
    private static JsonElement WriteStarfieldReadView(Mutagen.Bethesda.Starfield.IFormListGetter record)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            new StarfieldFormListInspector().WriteReadView(
                record,
                writer,
                TestContext.Current.CancellationToken);
        }

        using var document = JsonDocument.Parse(stream.ToArray());
        return document.RootElement.Clone();
    }

    /// <summary>Collects a bounded set of exact JSON-path differences from two complete inspector values.</summary>
    /// <param name="expected">The expected preview value.</param>
    /// <param name="actual">The strictly reopened staged value.</param>
    /// <param name="path">The current JSON path.</param>
    /// <param name="differences">The caller-owned difference collection.</param>
    private static void CollectJsonDifferences(
        JsonElement expected,
        JsonElement actual,
        string path,
        ICollection<string> differences)
    {
        const int maximumDifferences = 32;
        if (differences.Count >= maximumDifferences)
        {
            return;
        }

        if (expected.ValueKind != actual.ValueKind)
        {
            differences.Add($"{path}: expected {expected.ValueKind} {FormatJsonValue(expected)}, actual {actual.ValueKind} {FormatJsonValue(actual)}");
            return;
        }

        if (expected.ValueKind == JsonValueKind.Object)
        {
            foreach (var expectedProperty in expected.EnumerateObject())
            {
                if (!actual.TryGetProperty(expectedProperty.Name, out var actualProperty))
                {
                    differences.Add($"{path}.{expectedProperty.Name}: missing after staged reopen");
                    continue;
                }

                CollectJsonDifferences(
                    expectedProperty.Value,
                    actualProperty,
                    $"{path}.{expectedProperty.Name}",
                    differences);
            }

            foreach (var actualProperty in actual.EnumerateObject())
            {
                if (!expected.TryGetProperty(actualProperty.Name, out _))
                {
                    differences.Add($"{path}.{actualProperty.Name}: added after staged reopen as {FormatJsonValue(actualProperty.Value)}");
                }
            }

            return;
        }

        if (expected.ValueKind == JsonValueKind.Array)
        {
            var expectedItems = expected.EnumerateArray().ToArray();
            var actualItems = actual.EnumerateArray().ToArray();
            if (expectedItems.Length != actualItems.Length)
            {
                differences.Add($"{path}: expected length {expectedItems.Length}, actual length {actualItems.Length}");
            }

            for (var index = 0; index < Math.Min(expectedItems.Length, actualItems.Length); index++)
            {
                CollectJsonDifferences(expectedItems[index], actualItems[index], $"{path}[{index}]", differences);
            }

            return;
        }

        if (!string.Equals(expected.GetRawText(), actual.GetRawText(), StringComparison.Ordinal))
        {
            differences.Add($"{path}: expected {FormatJsonValue(expected)}, actual {FormatJsonValue(actual)}");
        }
    }

    /// <summary>Formats one scalar or composite JSON value without allowing diagnostics to grow without bound.</summary>
    /// <param name="value">The complete JSON value.</param>
    /// <returns>The raw JSON text, truncated to 256 characters.</returns>
    private static string FormatJsonValue(JsonElement value)
    {
        const int maximumLength = 256;
        var text = value.GetRawText();
        return text.Length <= maximumLength ? text : $"{text[..maximumLength]}...";
    }

    /// <summary>Formats every typed engine-error member available to the integration-test contract.</summary>
    /// <param name="error">The optional engine error returned by an operation.</param>
    /// <returns>A diagnostic containing the stable error code and full message.</returns>
    private static string DescribeError(EngineError? error)
    {
        return error is null
            ? "EngineError: <null>."
            : $"EngineError: Code={error.Code}; Message={error.Message}";
    }

    /// <summary>Formats the complete typed failure context returned by a guarded save.</summary>
    /// <param name="result">The guarded save result.</param>
    /// <param name="workspace">The workspace whose synchronization state was updated by the save.</param>
    /// <returns>A diagnostic containing status, typed error, recovery evidence, warnings, and synchronization state.</returns>
    private static string DescribeSaveFailure(SaveResult result, IPluginWorkspace workspace)
    {
        var warnings = string.Join(
            " | ",
            result.Warnings.Select(warning => $"{warning.Code}: {warning.Message}"));
        return $"SaveStatus={result.Status}; {DescribeError(result.Error)}; "
            + $"RecoveryEvidenceToken={result.RecoveryEvidenceToken?.Value ?? "<null>"}; "
            + $"Warnings={warnings}; OutputSynchronization={workspace.OutputSynchronization.Status}";
    }

    /// <summary>Collects structured exceptions emitted by one independently owned engine composition.</summary>
    private sealed class CollectingLogSink : ILogEventSink
    {
        /// <summary>Serializes access because plugin reads run on worker threads.</summary>
        private readonly object _gate = new();

        /// <summary>Stores the complete structured events emitted during the test.</summary>
        private readonly List<LogEvent> _events = [];

        /// <inheritdoc />
        public void Emit(LogEvent logEvent)
        {
            ArgumentNullException.ThrowIfNull(logEvent);
            lock (_gate)
            {
                _events.Add(logEvent);
            }
        }

        /// <summary>Formats every captured exception together with its rendered structured message.</summary>
        /// <returns>A diagnostic containing complete exception text, or an explicit empty marker.</returns>
        public string DescribeExceptions()
        {
            lock (_gate)
            {
                var exceptions = _events
                    .Where(logEvent => logEvent.Exception is not null)
                    .Select(logEvent => $"{logEvent.RenderMessage()}: {logEvent.Exception}")
                    .ToArray();
                return exceptions.Length == 0
                    ? "Captured plugin exceptions: <none>."
                    : $"Captured plugin exceptions: {string.Join(" | ", exceptions)}";
            }
        }
    }
}
