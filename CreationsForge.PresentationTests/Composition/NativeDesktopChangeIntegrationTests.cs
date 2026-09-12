using System.Text.Json;
using Autofac;
using Avalonia.Headless.XUnit;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using CreationsForge.NativeEditing.Drafts;
using CreationsForge.PresentationTests.Headless;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;
using CreationsForge.ViewModels;
using Mutagen.Bethesda.Plugins;
using Shouldly;

namespace CreationsForge.PresentationTests.Composition;

/// <summary>Exercises the composed editor and change lifecycle against disposable native files for every supported game.</summary>
[Collection(AvaloniaControlTestCollection.Name)]
public sealed partial class NativeDesktopChangeIntegrationTests
{
    /// <summary>Verifies Starfield review, save, independent reopen, and discard preserve complete native records.</summary>
    /// <returns>A task that completes after both native containers and the fixture are released.</returns>
    [AvaloniaFact]
    public Task ProductionChanges_Starfield_SaveAndDiscardPreserveNativeOutput()
    {
        return VerifySaveAndDiscardAsync(SupportedGame.Starfield);
    }

    /// <summary>Verifies Fallout 4 review, save, independent reopen, and discard preserve complete native records.</summary>
    /// <returns>A task that completes after both native containers and the fixture are released.</returns>
    [AvaloniaFact]
    public Task ProductionChanges_Fallout4_SaveAndDiscardPreserveNativeOutput()
    {
        return VerifySaveAndDiscardAsync(SupportedGame.Fallout4);
    }

    /// <summary>Verifies Skyrim review, save, independent reopen, and discard preserve complete native records.</summary>
    /// <returns>A task that completes after both native containers and the fixture are released.</returns>
    [AvaloniaFact]
    public Task ProductionChanges_Skyrim_SaveAndDiscardPreserveNativeOutput()
    {
        return VerifySaveAndDiscardAsync(SupportedGame.Skyrim);
    }

    /// <summary>Authors a new record and source override through real typed controls, then verifies persistence and a later discard.</summary>
    /// <param name="game">The game whose complete native plugin is written and reopened.</param>
    /// <returns>A task that completes after all deterministic UI workflows and file preservation assertions finish.</returns>
    private static async Task VerifySaveAndDiscardAsync(SupportedGame game)
    {
        using var fixture = NativeDesktopWorkspaceFixture.Create(game);
        var sourceBytes = fixture.SnapshotSourceBytes();
        var initialOutputBytes = File.ReadAllBytes(fixture.ExistingOutput.PluginPath);
        var initialRecords = NativeDesktopOutputAssertions.ReadAllFormLists(fixture, 1, []);
        var originalUnrelated = initialRecords[fixture.ExistingOutputFormKey];
        await using var container = fixture.CreateContainer($"Changes-{game}");
        var dialog = new NativeDesktopChangeDialog();
        await using var viewScope = container.BeginLifetimeScope(builder =>
            builder.RegisterInstance(dialog).As<INativeWorkspaceChangesDialogService>());
        var coordinator = container.Resolve<INativeWorkspaceCoordinator>();
        var open = await coordinator.OpenAsync(new NativeWorkspaceOpenRequest(
            fixture.CreateSourceRequest(), OutputSelectionMode.OpenExisting, fixture.ExistingOutput),
            TestContext.Current.CancellationToken);
        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        var browser = viewScope.Resolve<NativeFormListBrowserViewModel>();
        var changes = viewScope.Resolve<NativeWorkspaceChangesViewModel>();
        var arbiter = viewScope.Resolve<INativeWorkspacePresentationOperationArbiter>();
        await browser.StartAsync();
        var originalSource = await ReadRecordAsync(coordinator, fixture.SourceFormKey, RecordScope.Source);
        var editor = browser.Editor;
        await editor.BeginNewAsync(TestContext.Current.CancellationToken);
        editor.HasError.ShouldBeFalse(editor.ErrorMessage);
        var newFormKey = editor.Session.ShouldNotBeNull().FormKey;
        newFormKey.ModKey.ShouldBe(fixture.ExistingOutput.ModKey);
        var newEditorId = $"{game}UiNewList";
        var overrideEditorId = $"{game}UiSourceOverride";
        var idDraft = SetEditorIdDraft(editor, newEditorId);

        changes.CurrentReview.ShouldBeNull();
        dialog.Enqueue(NativeWorkspaceChangesDialogPurpose.Leave, async (model, cancellationToken) =>
        {
            model.CurrentReview.ShouldNotBeNull();
            model.HasDraftChanges.ShouldBeTrue();
            model.CanSaveChanges.ShouldBeFalse();
            model.CanDiscardChanges.ShouldBeTrue();
            editor.CanBeginNew.ShouldBeFalse();
            editor.CanApply.ShouldBeFalse();
            await model.SaveChangesAsync(cancellationToken);
            File.ReadAllBytes(fixture.ExistingOutput.PluginPath).ShouldBe(initialOutputBytes);
            editor.Draft.ShouldBeSameAs(idDraft);
            editor.HasDraftChanges.ShouldBeTrue();
            return NativeWorkspaceChangesDialogResult.KeepEditing;
        });
        var canceledLeave = await changes.ReserveLeaveAsync(
            NativeWorkspaceLeaveReason.CloseWorkspace, TestContext.Current.CancellationToken);
        canceledLeave.ShouldBeNull();
        coordinator.CurrentWorkspace.ShouldNotBeNull().WorkspaceId.ShouldBe(open.Value.ShouldNotBeNull().WorkspaceId);
        dialog.AssertDrained();
        arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
        await editor.ApplyAsync(TestContext.Current.CancellationToken);
        AssertApplied(editor);
        var intendedItems = new[] { fixture.SourceFormKey, fixture.ExistingOutputFormKey, fixture.SourceFormKey };
        await ApplyItemsAsync(editor, intendedItems);

        var sourceNode = browser.Records.Single(record => record.FormKey == fixture.SourceFormKey)
            .Children.Single(record => record.Context.Role == PluginRole.Source);
        await browser.SelectRecordAsync(sourceNode);
        await editor.BeginOverrideAsync(TestContext.Current.CancellationToken);
        editor.HasError.ShouldBeFalse(editor.ErrorMessage);
        editor.Session.ShouldNotBeNull().FormKey.ShouldBe(fixture.SourceFormKey);
        SetEditorIdDraft(editor, overrideEditorId);
        await editor.ApplyAsync(TestContext.Current.CancellationToken);
        AssertApplied(editor);

        dialog.Enqueue(NativeWorkspaceChangesDialogPurpose.Save, async (model, cancellationToken) =>
        {
            model.CurrentReview.ShouldNotBeNull();
            model.HasDraftChanges.ShouldBeFalse();
            var beforeAttempt = await ReadStateAsync(coordinator);
            editor.CanBeginNew.ShouldBeFalse();
            await editor.BeginNewAsync(cancellationToken);
            (await ReadStateAsync(coordinator)).Revision.ShouldBe(beforeAttempt.Revision);
            var preview = await ReadPreviewAsync(coordinator);
            preview.Comparisons.Count.ShouldBe(2);
            AssertIntendedRecord(preview.Comparisons.Single(item => item.FormKey == newFormKey).After!.Value,
                newEditorId, intendedItems);
            preview.Comparisons.Single(item => item.FormKey == fixture.SourceFormKey)
                .After!.Value.GetProperty("EditorID").GetString().ShouldBe(overrideEditorId);
            await model.SaveChangesAsync(cancellationToken);
            var saved = model.LastSaveResult.ShouldNotBeNull();
            saved.WorkspaceId.ShouldBe(open.Value.ShouldNotBeNull().WorkspaceId);
            saved.OperationId.ShouldNotBe(Guid.Empty);
            saved.BaseRevision.ShouldBe(beforeAttempt.Revision);
            saved.Status.ShouldBe(SaveCommitStatus.Committed);
            saved.Error.ShouldBeNull();
            var committedBaseline = saved.CommittedBaseline.ShouldNotBeNull();
            var currentState = await ReadStateAsync(coordinator);
            currentState.Revision.ShouldBe(saved.ResultRevision);
            currentState.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.Ready);
            currentState.OutputBaseline.ShouldNotBeNull().BaselineId.ShouldBe(committedBaseline.BaselineId);
            model.PendingRefreshEnvelope.ShouldBeNull();
            model.StatusText.ShouldBe("Saved and reopened.");
            editor.IsSessionActive.ShouldBeFalse();
            return NativeWorkspaceChangesDialogResult.KeepEditing;
        });
        await changes.ShowSaveChangesDialogAsync(TestContext.Current.CancellationToken);
        dialog.AssertDrained();
        arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
        var savedBytes = File.ReadAllBytes(fixture.ExistingOutput.PluginPath);
        savedBytes.SequenceEqual(initialOutputBytes).ShouldBeFalse();
        fixture.SnapshotSourceBytes().ShouldBe(sourceBytes);
        var savedRecords = NativeDesktopOutputAssertions.ReadAllFormLists(fixture, 3, [fixture.SourceFormKey.ModKey]);
        AssertIntendedRecord(savedRecords[newFormKey], newEditorId, intendedItems);
        savedRecords[fixture.SourceFormKey].GetProperty("EditorID").GetString().ShouldBe(overrideEditorId);
        AssertUneditedFields(originalSource, savedRecords[fixture.SourceFormKey], "EditorID");
        JsonElement.DeepEquals(originalUnrelated, savedRecords[fixture.ExistingOutputFormKey]).ShouldBeTrue();
        await AssertIndependentReopenAsync(fixture, savedRecords);

        var savedNode = browser.Records.Single(record => record.FormKey == newFormKey)
            .Children.Single(record => record.Context.Role == PluginRole.Output);
        await browser.SelectRecordAsync(savedNode);
        await editor.BeginExistingOutputAsync(TestContext.Current.CancellationToken);
        editor.HasError.ShouldBeFalse(editor.ErrorMessage);
        SetEditorIdDraft(editor, $"{game}MustBeDiscarded");
        await editor.ApplyAsync(TestContext.Current.CancellationToken);
        AssertApplied(editor);
        (await ReadPreviewAsync(coordinator)).Comparisons.ShouldHaveSingleItem();
        dialog.Enqueue(NativeWorkspaceChangesDialogPurpose.Discard, async (model, cancellationToken) =>
        {
            model.CurrentReview.ShouldNotBeNull();
            await model.DiscardChangesAsync(cancellationToken);
            editor.IsSessionActive.ShouldBeFalse();
            (await ReadPreviewAsync(coordinator)).Comparisons.ShouldBeEmpty();
            return NativeWorkspaceChangesDialogResult.KeepEditing;
        });
        await changes.ShowDiscardChangesDialogAsync(TestContext.Current.CancellationToken);
        dialog.AssertDrained();
        File.ReadAllBytes(fixture.ExistingOutput.PluginPath).ShouldBe(savedBytes);
        fixture.SnapshotSourceBytes().ShouldBe(sourceBytes);
        JsonElement.DeepEquals(await ReadRecordAsync(coordinator, newFormKey, RecordScope.StagedOutput),
            savedRecords[newFormKey]).ShouldBeTrue();
        arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
        await changes.ShutdownAndDrainAsync();
        await coordinator.CloseAsync();
    }

    /// <summary>Creates a real current-value EditorID draft and changes its typed string control.</summary>
    /// <param name="editor">The current revision-bound editor session.</param>
    /// <param name="editorId">The exact intended EditorID.</param>
    /// <returns>The same changed draft retained by the editor.</returns>
    private static NativeFormListDraft SetEditorIdDraft(NativeFormListEditorViewModel editor, string editorId)
    {
        var command = editor.AvailableCommands.Single(item => item.CommandName == "form-list.set-editor-id");
        var created = editor.CreateDraft(command, NativeFormListDraftSeedSelection.CurrentValue());
        created.Succeeded.ShouldBeTrue(created.Error?.Message);
        var draft = created.Value.ShouldNotBeNull();
        draft.Root.ShouldBeOfType<NativeWireObjectDraftNode>().FindProperty("editorId")
            .ShouldBeOfType<NativeWireStringDraftNode>().Value = editorId;
        editor.HasDraftChanges.ShouldBeTrue();
        return draft;
    }

    /// <summary>Replaces the complete ordered reference array through the actual typed draft and editor Apply path.</summary>
    /// <param name="editor">The active revision-bound editor.</param>
    /// <param name="intendedItems">The exact intended order, including duplicates.</param>
    /// <returns>A task that completes after the native edit and browser refresh.</returns>
    private static async Task ApplyItemsAsync(NativeFormListEditorViewModel editor, IReadOnlyList<FormKey> intendedItems)
    {
        var command = editor.AvailableCommands.Single(item => item.CommandName == "form-list.replace-items");
        var created = editor.CreateDraft(command, NativeFormListDraftSeedSelection.CurrentValue());
        created.Succeeded.ShouldBeTrue(created.Error?.Message);
        var items = created.Value.ShouldNotBeNull().Root.ShouldBeOfType<NativeWireObjectDraftNode>()
            .FindProperty("items").ShouldBeOfType<NativeWireArrayDraftNode>();
        items.Clear();
        foreach (var formKey in intendedItems)
        {
            var inserted = items.Insert(items.Items.Count);
            inserted.Succeeded.ShouldBeTrue(inserted.Error?.Message);
            var link = inserted.Value.ShouldBeOfType<NativeWireFormLinkDraftNode>();
            link.IsNull = false;
            link.FormKey = formKey.ToString();
        }

        await editor.ApplyAsync(TestContext.Current.CancellationToken);
        AssertApplied(editor);
    }

    /// <summary>Requires a completed Apply to leave an active session with no unresolved outcome or unapplied input.</summary>
    /// <param name="editor">The editor whose awaited Apply completed.</param>
    private static void AssertApplied(NativeFormListEditorViewModel editor)
    {
        editor.HasError.ShouldBeFalse(editor.ErrorMessage);
        editor.HasPendingOperation.ShouldBeFalse();
        editor.IsSessionActive.ShouldBeTrue();
        editor.HasDraftChanges.ShouldBeFalse();
        editor.IsBusy.ShouldBeFalse();
    }

    /// <summary>Reads a complete detached record from the currently owned workspace without retaining a native getter.</summary>
    /// <param name="coordinator">The application-owned workspace boundary.</param>
    /// <param name="formKey">The exact record to inspect.</param>
    /// <param name="scope">The selected source or output view.</param>
    /// <returns>The immutable complete record JSON.</returns>
    private static async Task<JsonElement> ReadRecordAsync(INativeWorkspaceCoordinator coordinator, FormKey formKey, RecordScope scope)
    {
        var result = await coordinator.ExecuteAsync((workspace, cancellationToken) =>
            workspace.ReadFormListViewAsync(new ReferenceRequest(formKey, scope), cancellationToken),
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value.ShouldNotBeNull().Record.ShouldNotBeNull();
    }

    /// <summary>Reads the atomic live workspace revision used to detect a forbidden editor entry.</summary>
    /// <param name="coordinator">The application-owned workspace boundary.</param>
    /// <returns>The successful atomic state.</returns>
    private static async Task<WorkspaceState> ReadStateAsync(INativeWorkspaceCoordinator coordinator)
    {
        var result = await coordinator.ExecuteAsync((workspace, cancellationToken) => workspace.ReadStateAsync(cancellationToken),
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        result.WorkspaceId.ShouldBe(coordinator.CurrentWorkspace.ShouldNotBeNull().WorkspaceId);
        var state = result.Value.ShouldNotBeNull();
        result.BaseRevision.ShouldBe(state.Revision);
        result.ResultRevision.ShouldBe(state.Revision);
        return state;
    }

    /// <summary>Reads all workspace-staged comparisons through the production borrowing boundary.</summary>
    /// <param name="coordinator">The application-owned workspace boundary.</param>
    /// <returns>The successful complete workspace preview.</returns>
    private static async Task<WorkspacePreview> ReadPreviewAsync(INativeWorkspaceCoordinator coordinator)
    {
        var result = await coordinator.ExecuteAsync((workspace, cancellationToken) => workspace.PreviewAsync(cancellationToken),
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value.ShouldNotBeNull();
    }

    /// <summary>Requires a newly opened independent production container to read every persisted record with no staged edits.</summary>
    /// <param name="fixture">The source and committed output fixture.</param>
    /// <param name="savedRecords">Complete records independently parsed from the saved binary.</param>
    /// <returns>A task that completes after the fresh container releases its independent workspace.</returns>
    private static async Task AssertIndependentReopenAsync(NativeDesktopWorkspaceFixture fixture, IReadOnlyDictionary<FormKey, JsonElement> savedRecords)
    {
        await using var container = fixture.CreateContainer($"Changes-Reopen-{fixture.Game}");
        var coordinator = container.Resolve<INativeWorkspaceCoordinator>();
        var opened = await coordinator.OpenAsync(new NativeWorkspaceOpenRequest(
            fixture.CreateSourceRequest(), OutputSelectionMode.OpenExisting, fixture.ExistingOutput),
            TestContext.Current.CancellationToken);
        opened.Succeeded.ShouldBeTrue(opened.Error?.Message);
        foreach (var (formKey, savedRecord) in savedRecords)
        {
            var reopenedRecord = await ReadRecordAsync(coordinator, formKey, RecordScope.StagedOutput);
            JsonElement.DeepEquals(savedRecord, reopenedRecord).ShouldBeTrue($"Fresh output record {formKey} changed.");
        }

        (await ReadPreviewAsync(coordinator)).Comparisons.ShouldBeEmpty();
        await coordinator.CloseAsync();
    }

    /// <summary>Checks independently specified EditorID and duplicate-preserving item intentions.</summary>
    /// <param name="record">The complete preview or persisted record.</param>
    /// <param name="editorId">The intended exact EditorID.</param>
    /// <param name="items">The intended exact ordered native identities.</param>
    private static void AssertIntendedRecord(JsonElement record, string editorId, IReadOnlyList<FormKey> items)
    {
        record.GetProperty("EditorID").GetString().ShouldBe(editorId);
        var actualItems = record.GetProperty("Items").EnumerateArray().ToArray();
        actualItems.Length.ShouldBe(items.Count);
        for (var index = 0; index < items.Count; index++)
        {
            actualItems[index].GetProperty("isNull").GetBoolean().ShouldBeFalse();
            actualItems[index].GetProperty("formKey").GetString().ShouldBe(items[index].ToString());
        }
    }

    /// <summary>Checks every unedited root field and complete nested value, including properties missing from either side.</summary>
    /// <param name="before">The independent original record.</param>
    /// <param name="after">The persisted override.</param>
    /// <param name="changedProperty">The sole explicitly changed root field.</param>
    private static void AssertUneditedFields(JsonElement before, JsonElement after, string changedProperty)
    {
        var expected = before.EnumerateObject().Where(property => property.Name != changedProperty)
            .ToDictionary(property => property.Name, property => property.Value);
        var actual = after.EnumerateObject().Where(property => property.Name != changedProperty)
            .ToDictionary(property => property.Name, property => property.Value);
        actual.Keys.OrderBy(name => name, StringComparer.Ordinal).ShouldBe(expected.Keys.OrderBy(name => name, StringComparer.Ordinal));
        foreach (var (name, value) in expected)
        {
            JsonElement.DeepEquals(value, actual[name]).ShouldBeTrue($"Unedited override field {name} changed.");
        }
    }
}
