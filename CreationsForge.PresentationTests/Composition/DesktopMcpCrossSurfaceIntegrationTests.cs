using System.Security.Cryptography;
using System.Text.Json;
using Autofac;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using CreationsForge.Mcp;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using CreationsForge.RecordEditing.Drafts;
using CreationsForge.PresentationTests.Headless;
using CreationsForge.PresentationTests.Support;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;
using CreationsForge.TestSupport;
using CreationsForge.ViewModels;
using CreationsForge.Views;
using Mutagen.Bethesda.Plugins;
using Shouldly;

namespace CreationsForge.PresentationTests.Composition;

/// <summary>Proves first-writer conflict safety between the production desktop and physical production MCP stdio host.</summary>
[Collection(AvaloniaControlTestCollection.Name)]
public sealed class DesktopMcpCrossSurfaceIntegrationTests
{
    /// <summary>The finite deadline shared by every plugin and protocol operation in one case.</summary>
    private static readonly TimeSpan CaseTimeout = TimeSpan.FromSeconds(60);

    /// <summary>Verifies independent application and MCP owners cannot overwrite the first committed plugin output.</summary>
    /// <param name="game">The generated plugin adapter fixture.</param>
    /// <param name="saveOrder">Which production surface commits first.</param>
    /// <returns>A task that completes after both owners, a replacement, and a third plugin reader are released.</returns>
    [AvaloniaTheory]
    [InlineData(SupportedGame.Starfield, CrossSurfaceSaveOrder.ApplicationFirst)]
    [InlineData(SupportedGame.Starfield, CrossSurfaceSaveOrder.McpFirst)]
    [InlineData(SupportedGame.Fallout4, CrossSurfaceSaveOrder.ApplicationFirst)]
    [InlineData(SupportedGame.Fallout4, CrossSurfaceSaveOrder.McpFirst)]
    [InlineData(SupportedGame.Skyrim, CrossSurfaceSaveOrder.ApplicationFirst)]
    [InlineData(SupportedGame.Skyrim, CrossSurfaceSaveOrder.McpFirst)]
    public async Task ProductionDesktopAndPhysicalMcp_FirstWriterWinsAndStaleOwnerReopens(
        SupportedGame game,
        CrossSurfaceSaveOrder saveOrder)
    {
        using var timeout = new CancellationTokenSource(CaseTimeout);
        var cancellationToken = timeout.Token;
        using var fixture = DesktopWorkspaceFixture.Create(game);
        var sourceBefore = fixture.SnapshotSourceBytes();
        var outputBefore = fixture.SnapshotExistingOutputArtifacts();
        await using var container = fixture.CreateContainer($"CrossSurface-{game}-{saveOrder}");
        var changesDialog = new DesktopChangeDialog();
        var selectionDialog = new FakeWorkspaceSelectionDialogService();
        await using var viewScope = container.BeginLifetimeScope(builder =>
        {
            builder.RegisterInstance(changesDialog).As<IWorkspaceChangesDialogService>();
            builder.RegisterInstance(selectionDialog).As<IWorkspaceSelectionDialogService>();
        });
        var coordinator = container.Resolve<IWorkspaceCoordinator>();
        var applicationOpen = await coordinator.OpenAsync(
            new WorkspaceLaunchRequest(
                fixture.CreateSourceRequest(),
                OutputSelectionMode.OpenExisting,
                fixture.ExistingOutput),
            cancellationToken);
        applicationOpen.Succeeded.ShouldBeTrue(applicationOpen.Error?.Message);
        var applicationWorkspaceId = applicationOpen.Value.ShouldNotBeNull().WorkspaceId;
        var browser = viewScope.Resolve<FormListBrowserViewModel>();
        var changes = viewScope.Resolve<WorkspaceChangesViewModel>();
        var arbiter = viewScope.Resolve<IWorkspacePresentationOperationArbiter>();
        var shell = viewScope.Resolve<WorkspaceShellViewModel>();
        await browser.StartAsync();
        var editor = browser.Editor;
        var outputNode = browser.Records.Single(record => record.FormKey == fixture.ExistingOutputFormKey)
            .Children.Single(record => record.Context.Role == PluginRole.Output);
        await browser.SelectRecordAsync(outputNode);
        await editor.BeginExistingOutputAsync(cancellationToken);
        editor.HasError.ShouldBeFalse(editor.ErrorMessage);
        var applicationEditId = editor.Session.ShouldNotBeNull().EditId;
        var orderName = saveOrder == CrossSurfaceSaveOrder.ApplicationFirst ? "UiFirst" : "McpFirst";
        var applicationEditorId = $"CfUi{orderName}{game}";
        SetEditorIdDraft(editor, applicationEditorId);
        await editor.ApplyAsync(cancellationToken);
        AssertApplied(editor);
        var applicationStateBeforeSave = await ReadStateAsync(coordinator, cancellationToken);
        var applicationPreviewBeforeSave = await ReadPreviewAsync(coordinator, cancellationToken);
        AssertApplicationPreview(applicationPreviewBeforeSave, fixture.ExistingOutputFormKey, applicationEditorId);

        await using var process = await McpStdioProcessFixture.StartAsync(
            typeof(McpHostRunner).Assembly.Location,
            cancellationToken);
        var activeBeforeOpen = await PluginMcpWorkspaceSession.ReadActiveWorkspaceIdsAsync(process.Client, cancellationToken);
        activeBeforeOpen.ShouldBeEmpty();
        var mcp = await PluginMcpWorkspaceSession.OpenAsync(process.Client, fixture, cancellationToken);
        mcp.WorkspaceId.ShouldNotBe(applicationWorkspaceId);
        (await PluginMcpWorkspaceSession.ReadActiveWorkspaceIdsAsync(process.Client, cancellationToken))
            .ShouldBe([mcp.WorkspaceId]);
        var mcpEditorId = $"CfMcp{orderName}{game}";
        var mcpEdit = await mcp.StageEditorIdAsync(
            fixture.ExistingOutputFormKey,
            mcpEditorId,
            cancellationToken);
        mcpEdit.EditId.ShouldNotBe(applicationEditId);
        new[] { mcp.SelectionOperationId, mcpEdit.BeginOperationId, mcpEdit.ApplyOperationId }
            .Distinct().Count().ShouldBe(3);
        mcp.SelectionOperationId.ShouldNotBe(Guid.Empty);
        mcpEdit.BeginOperationId.ShouldNotBe(Guid.Empty);
        mcpEdit.ApplyOperationId.ShouldNotBe(Guid.Empty);
        mcp.BaselineHandle.ShouldNotBeNullOrWhiteSpace();
        mcpEdit.Revision.ShouldBe(applicationStateBeforeSave.Revision);
        var mcpPreviewBeforeSave = await mcp.ReadPreviewAsync(cancellationToken);
        AssertMcpPreview(mcpPreviewBeforeSave, fixture.ExistingOutputFormKey, mcpEditorId);

        SaveResult? applicationSave = null;
        PluginMcpSaveRequest? mcpSaveRequest = null;
        PluginMcpSaveOutcome? mcpSave = null;
        IReadOnlyDictionary<string, byte[]>? winnerArtifacts = null;
        changesDialog.Enqueue(WorkspaceChangesDialogPurpose.Save, async (model, dialogToken) =>
        {
            model.ShouldBeSameAs(changes);
            model.CurrentReview.ShouldNotBeNull().WorkspaceId.ShouldBe(applicationWorkspaceId);
            using var attached = AttachChangesView(model, WorkspaceChangesDialogRequest.ForSave());
            AssertReviewView(attached.View, model, fixture.ExistingOutputFormKey);
            if (saveOrder == CrossSurfaceSaveOrder.ApplicationFirst)
            {
                await model.SaveChangesAsync(dialogToken);
                applicationSave = model.LastSaveResult.ShouldNotBeNull();
                AssertCommittedApplicationSave(applicationSave, applicationWorkspaceId, applicationStateBeforeSave);
                (await ReadStateAsync(coordinator, dialogToken)).Revision.ShouldBe(applicationSave.ResultRevision);
                winnerArtifacts = fixture.SnapshotExistingOutputArtifacts();
                model.CurrentReview.ShouldNotBeNull().Revision.ShouldBe(applicationStateBeforeSave.Revision);
                model.IsReviewStale.ShouldBeTrue();
                AssertBoundStatus(attached.View, "Saved and reopened.", stale: true);
            }
            else
            {
                mcpSaveRequest = mcp.CreateSaveRequest();
                mcpSave = await mcp.SaveAsync(mcpSaveRequest, dialogToken);
                AssertCommittedMcpSave(mcpSave, mcpSaveRequest);
                mcp.Revision.ShouldBe(mcpSave.ResultRevision);
                mcp.BaselineHandle.ShouldBe(mcpSave.CommittedBaselineHandle);
                winnerArtifacts = fixture.SnapshotExistingOutputArtifacts();
                await model.SaveChangesAsync(dialogToken);
                applicationSave = model.LastSaveResult.ShouldNotBeNull();
                AssertRejectedApplicationSave(applicationSave, applicationWorkspaceId, applicationStateBeforeSave);
                AssertBoundStatus(
                    attached.View,
                    "The output changed outside CreationsForge. No files were overwritten.",
                    stale: true);
            }

            return WorkspaceChangesDialogResult.KeepEditing;
        });
        await changes.ShowSaveChangesDialogAsync(cancellationToken);
        changesDialog.AssertDrained();
        winnerArtifacts.ShouldNotBeNull();
        AssertArtifactSetChanged(outputBefore, winnerArtifacts);
        fixture.SnapshotSourceBytes().ShouldBe(sourceBefore);

        if (saveOrder == CrossSurfaceSaveOrder.ApplicationFirst)
        {
            mcpSaveRequest = mcp.CreateSaveRequest();
            mcpSave = await mcp.SaveAsync(mcpSaveRequest, cancellationToken);
            AssertRejectedMcpSave(mcpSave, mcpSaveRequest);
            AssertArtifactsEqual(winnerArtifacts, fixture.SnapshotExistingOutputArtifacts());
            var replay = await mcp.SaveAsync(mcpSaveRequest, cancellationToken);
            AssertSameMcpSave(mcpSave, replay);
            AssertArtifactsEqual(winnerArtifacts, fixture.SnapshotExistingOutputArtifacts());
            var newRequest = mcp.CreateSaveRequest();
            newRequest.OperationId.ShouldNotBe(mcpSaveRequest.OperationId);
            var newAttempt = await mcp.SaveAsync(newRequest, cancellationToken);
            AssertRejectedMcpSave(newAttempt, newRequest);
            AssertArtifactsEqual(winnerArtifacts, fixture.SnapshotExistingOutputArtifacts());
            var retainedPreview = await mcp.ReadPreviewAsync(cancellationToken);
            JsonElement.DeepEquals(retainedPreview, mcpPreviewBeforeSave).ShouldBeTrue();
            await mcp.CloseAsync(cancellationToken);
            var staleMcpWorkspaceId = mcp.WorkspaceId;
            var staleBaselineHandle = mcp.BaselineHandle;
            mcp = await PluginMcpWorkspaceSession.OpenAsync(process.Client, fixture, cancellationToken);
            mcp.WorkspaceId.ShouldNotBe(staleMcpWorkspaceId);
            mcp.BaselineHandle.ShouldNotBe(staleBaselineHandle);
        }
        else
        {
            applicationSave.ShouldNotBeNull();
            AssertArtifactsEqual(winnerArtifacts, fixture.SnapshotExistingOutputArtifacts());
            var applicationStateAfterConflict = await ReadStateAsync(coordinator, cancellationToken);
            applicationStateAfterConflict.Revision.ShouldBe(applicationStateBeforeSave.Revision);
            applicationStateAfterConflict.OutputBaseline.ShouldNotBeNull().BaselineId.ShouldBe(
                applicationStateBeforeSave.OutputBaseline.ShouldNotBeNull().BaselineId);
            applicationStateAfterConflict.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.Ready);
            var retainedPreview = await ReadPreviewAsync(coordinator, cancellationToken);
            AssertPreviewsEqual(applicationPreviewBeforeSave, retainedPreview);
            editor.Session.ShouldNotBeNull().EditId.ShouldBe(applicationEditId);
            editor.Session.ExpectedRevision.ShouldBe(applicationStateBeforeSave.Revision);
        }

        var guardMcpEditorId = $"CfMcpGuard{orderName}{game}";
        PluginMcpStagedEdit? guardMcpEdit = null;
        var applicationWorkspaceBeforeReplacement = coordinator.CurrentWorkspace.ShouldNotBeNull().WorkspaceId;
        selectionDialog.ShowAction = async selection =>
        {
            arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeTrue();
            editor.CanBeginNew.ShouldBeFalse();
            var stateBeforeDeniedBegin = await ReadStateAsync(coordinator, cancellationToken);
            var sessionBeforeDeniedBegin = editor.Session;
            await editor.BeginNewAsync(cancellationToken);
            (await ReadStateAsync(coordinator, cancellationToken)).Revision.ShouldBe(stateBeforeDeniedBegin.Revision);
            if (sessionBeforeDeniedBegin is null)
            {
                editor.Session.ShouldBeNull();
            }
            else
            {
                editor.Session.ShouldBeSameAs(sessionBeforeDeniedBegin);
            }

            guardMcpEdit = await mcp.StageEditorIdAsync(
                fixture.ExistingOutputFormKey,
                guardMcpEditorId,
                cancellationToken);
            guardMcpEdit.EditId.ShouldNotBe(Guid.Empty);
            AssertMcpPreview(
                await mcp.ReadPreviewAsync(cancellationToken),
                fixture.ExistingOutputFormKey,
                guardMcpEditorId);
            AssertArtifactsEqual(winnerArtifacts, fixture.SnapshotExistingOutputArtifacts());
            arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeTrue();
            ConfigureReplacement(selection, fixture);
            var opened = await selection.OpenWorkspaceAsync(cancellationToken);
            opened.ShouldBeTrue(selection.ErrorText);
            return opened;
        };

        if (saveOrder == CrossSurfaceSaveOrder.McpFirst)
        {
            changesDialog.Enqueue(WorkspaceChangesDialogPurpose.Leave, async (model, dialogToken) =>
            {
                using var attached = AttachChangesView(
                    model,
                    WorkspaceChangesDialogRequest.ForLeave(WorkspaceLeaveReason.OpenWorkspace));
                await model.SaveChangesAsync(dialogToken);
                var retry = model.LastSaveResult.ShouldNotBeNull();
                retry.OperationId.ShouldNotBe(applicationSave.ShouldNotBeNull().OperationId);
                AssertRejectedApplicationSave(retry, applicationWorkspaceId, applicationStateBeforeSave);
                model.ShowConfirmedAbandonmentForOpen.ShouldBeTrue();
                model.CanConfirmAbandonmentForOpen.ShouldBeTrue();
                Dispatcher.UIThread.RunJobs();
                AssertBoundStatus(
                    attached.View,
                    "The output changed outside CreationsForge. No files were overwritten.",
                    stale: true);
                var abandon = ControlFinder.FindByAutomationId<Button>(
                    attached.View,
                    "WorkspaceConfirmAbandonmentButton").ShouldNotBeNull();
                abandon.IsEffectivelyVisible.ShouldBeTrue();
                abandon.IsEnabled.ShouldBeTrue();
                ControlFinder.FindByAutomationId<TextBlock>(
                    attached.View,
                    "WorkspaceAbandonmentExplanation").ShouldNotBeNull().Text.ShouldNotBeNull().ShouldContain(
                    fixture.ExistingOutput.ModKey.FileName);
                var outcome = await model.ApplyLeaveChoiceAsync(
                    WorkspaceChangesDialogChoice.ConfirmAbandonmentForOpen,
                    dialogToken);
                outcome.ShouldBe(WorkspaceLeaveChoiceOutcome.Proceed);
                return WorkspaceChangesDialogResult.Proceed;
            });
        }

        (await shell.OpenWorkspaceAsync(cancellationToken)).ShouldBeTrue();
        changesDialog.AssertDrained();
        selectionDialog.ViewModels.ShouldHaveSingleItem();
        guardMcpEdit.ShouldNotBeNull();
        coordinator.CurrentWorkspace.ShouldNotBeNull().WorkspaceId.ShouldNotBe(applicationWorkspaceBeforeReplacement);
        arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
        AssertArtifactsEqual(winnerArtifacts, fixture.SnapshotExistingOutputArtifacts());
        var replacementRecord = await ReadRecordAsync(
            coordinator,
            fixture.ExistingOutputFormKey,
            cancellationToken);
        replacementRecord.GetProperty("EditorID").GetString().ShouldBe(
            saveOrder == CrossSurfaceSaveOrder.ApplicationFirst ? applicationEditorId : mcpEditorId);

        await mcp.CloseAsync(cancellationToken);
        (await PluginMcpWorkspaceSession.ReadActiveWorkspaceIdsAsync(process.Client, cancellationToken))
            .ShouldBeEmpty();
        var completion = await process.CompleteAsync(cancellationToken);
        completion.ExitCode.ShouldBe(0, completion.StandardError);
        await changes.ShutdownAndDrainAsync();
        await coordinator.CloseAsync();
        await AssertFreshPluginOutputAsync(
            fixture,
            saveOrder == CrossSurfaceSaveOrder.ApplicationFirst ? applicationEditorId : mcpEditorId,
            cancellationToken);
        fixture.SnapshotSourceBytes().ShouldBe(sourceBefore);
        AssertArtifactsEqual(winnerArtifacts, fixture.SnapshotExistingOutputArtifacts());
    }

    /// <summary>Creates a changed current-value EditorID draft through the production desktop editor.</summary>
    /// <param name="editor">The active existing-output edit session.</param>
    /// <param name="editorId">The exact EditorID to stage.</param>
    private static void SetEditorIdDraft(FormListEditorViewModel editor, string editorId)
    {
        var command = editor.AvailableCommands.Single(item => item.CommandName == "form-list.set-editor-id");
        var created = editor.CreateDraft(command, FormListDraftSeedSelection.CurrentValue());
        created.Succeeded.ShouldBeTrue(created.Error?.Message);
        created.Value.ShouldNotBeNull().Root.ShouldBeOfType<RecordWireObjectDraftNode>()
            .FindProperty("editorId").ShouldBeOfType<RecordWireStringDraftNode>().Value = editorId;
    }

    /// <summary>Requires a successful production Apply to retain an active clean edit session.</summary>
    /// <param name="editor">The editor whose awaited Apply completed.</param>
    private static void AssertApplied(FormListEditorViewModel editor)
    {
        editor.HasError.ShouldBeFalse(editor.ErrorMessage);
        editor.HasPendingOperation.ShouldBeFalse();
        editor.IsSessionActive.ShouldBeTrue();
        editor.HasDraftChanges.ShouldBeFalse();
        editor.IsBusy.ShouldBeFalse();
    }

    /// <summary>Reads the current atomic application workspace state through the coordinator borrow.</summary>
    /// <param name="coordinator">The root application workspace owner.</param>
    /// <param name="cancellationToken">The finite case token.</param>
    /// <returns>The exact successful state.</returns>
    private static async Task<WorkspaceState> ReadStateAsync(
        IWorkspaceCoordinator coordinator,
        CancellationToken cancellationToken)
    {
        var result = await coordinator.ExecuteAsync(
            (workspace, token) => workspace.ReadStateAsync(token),
            cancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value.ShouldNotBeNull();
    }

    /// <summary>Reads the complete application-staged preview through the coordinator borrow.</summary>
    /// <param name="coordinator">The root application workspace owner.</param>
    /// <param name="cancellationToken">The finite case token.</param>
    /// <returns>The exact successful preview.</returns>
    private static async Task<WorkspacePreview> ReadPreviewAsync(
        IWorkspaceCoordinator coordinator,
        CancellationToken cancellationToken)
    {
        var result = await coordinator.ExecuteAsync(
            (workspace, token) => workspace.PreviewAsync(token),
            cancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value.ShouldNotBeNull();
    }

    /// <summary>Reads one complete detached output FormList from the application workspace.</summary>
    /// <param name="coordinator">The root application workspace owner.</param>
    /// <param name="formKey">The exact output record.</param>
    /// <param name="cancellationToken">The finite case token.</param>
    /// <returns>The detached record JSON.</returns>
    private static async Task<JsonElement> ReadRecordAsync(
        IWorkspaceCoordinator coordinator,
        FormKey formKey,
        CancellationToken cancellationToken)
    {
        var result = await coordinator.ExecuteAsync(
            (workspace, token) => workspace.ReadFormListViewAsync(
                new ReferenceRequest(formKey, RecordScope.StagedOutput),
                token),
            cancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value.ShouldNotBeNull().Record.ShouldNotBeNull();
    }

    /// <summary>Attaches the real changes control to a headless window for bound-state assertions.</summary>
    /// <param name="model">The production scoped change lifecycle.</param>
    /// <param name="request">The active dialog purpose mirrored by the attached view.</param>
    /// <returns>The owned visible window and its real changes view.</returns>
    private static AttachedChangesView AttachChangesView(
        WorkspaceChangesViewModel model,
        WorkspaceChangesDialogRequest request)
    {
        var view = new WorkspaceChangesView(model, request, _ => Task.CompletedTask);
        var window = new Window
        {
            Width = 1000,
            Height = 720,
            Content = view,
        };
        window.Show();
        Dispatcher.UIThread.RunJobs();
        return new AttachedChangesView(window, view);
    }

    /// <summary>Requires the attached review to expose the real scoped model and staged record.</summary>
    /// <param name="view">The attached production view.</param>
    /// <param name="model">The scoped production view model.</param>
    /// <param name="formKey">The staged output record identity.</param>
    private static void AssertReviewView(
        WorkspaceChangesView view,
        WorkspaceChangesViewModel model,
        FormKey formKey)
    {
        view.DataContext.ShouldBeSameAs(model);
        model.CurrentReview.ShouldNotBeNull().Items.ShouldHaveSingleItem().FormKey.ShouldBe(formKey);
        ControlFinder.FindByAutomationId<ItemsControl>(view, "WorkspaceChangeItems")
            .ShouldNotBeNull().IsEffectivelyVisible.ShouldBeTrue();
    }

    /// <summary>Requires user-visible persistence status and stale-review binding to match typed state.</summary>
    /// <param name="view">The attached real view.</param>
    /// <param name="status">The expected exact status text.</param>
    /// <param name="stale">Whether the stale-review warning must be visible.</param>
    private static void AssertBoundStatus(WorkspaceChangesView view, string status, bool stale)
    {
        Dispatcher.UIThread.RunJobs();
        ControlFinder.FindByAutomationId<TextBlock>(view, "WorkspaceChangesStatusText")!
            .Text.ShouldBe(status);
        ControlFinder.FindByAutomationId<TextBlock>(view, "WorkspaceStaleReviewWarning")!
            .IsEffectivelyVisible.ShouldBe(stale);
    }

    /// <summary>Requires one complete application preview containing the intended EditorID.</summary>
    /// <param name="preview">The detached production preview.</param>
    /// <param name="formKey">The exact output record.</param>
    /// <param name="editorId">The intended application value.</param>
    private static void AssertApplicationPreview(
        WorkspacePreview preview,
        FormKey formKey,
        string editorId)
    {
        var comparison = preview.Comparisons.ShouldHaveSingleItem();
        comparison.FormKey.ShouldBe(formKey);
        comparison.After.ShouldNotBeNull().GetProperty("EditorID").GetString().ShouldBe(editorId);
    }

    /// <summary>Requires the complete MCP preview to contain the intended staged EditorID.</summary>
    /// <param name="preview">The reconstructed complete protocol preview.</param>
    /// <param name="formKey">The exact output record.</param>
    /// <param name="editorId">The intended MCP value.</param>
    private static void AssertMcpPreview(JsonElement preview, FormKey formKey, string editorId)
    {
        var comparison = preview.GetProperty("comparisons").EnumerateArray().ShouldHaveSingleItem();
        comparison.GetProperty("formKey").GetString().ShouldBe(formKey.ToString());
        comparison.GetProperty("after").GetProperty("EditorID").GetString().ShouldBe(editorId);
    }

    /// <summary>Requires a known committed and reopened application save with complete artifact evidence.</summary>
    /// <param name="result">The typed application save result.</param>
    /// <param name="workspaceId">The exact application workspace.</param>
    /// <param name="before">The state captured before either owner saved.</param>
    private static void AssertCommittedApplicationSave(
        SaveResult result,
        Guid workspaceId,
        WorkspaceState before)
    {
        result.WorkspaceId.ShouldBe(workspaceId);
        result.OperationId.ShouldNotBe(Guid.Empty);
        result.BaseRevision.ShouldBe(before.Revision);
        result.ResultRevision.ShouldNotBe(before.Revision);
        result.Status.ShouldBe(SaveCommitStatus.Committed);
        result.Error.ShouldBeNull();
        result.RecoveryEvidenceToken.ShouldBeNull();
        result.ResolvedEvidence.ShouldBeNull();
        var committed = result.CommittedBaseline.ShouldNotBeNull();
        var prior = before.OutputBaseline.ShouldNotBeNull();
        committed.Artifacts.Count.ShouldBe(prior.Artifacts.Count);
        for (var index = 0; index < committed.Artifacts.Count; index++)
        {
            var expected = prior.Artifacts[index];
            var artifact = committed.Artifacts[index];
            artifact.Path.ShouldBe(expected.Path);
            artifact.Role.ShouldBe(expected.Role);
            artifact.Language.ShouldBe(expected.Language);
            artifact.Fingerprint.Exists.ShouldBe(File.Exists(artifact.Path));
            if (!artifact.Fingerprint.Exists)
            {
                artifact.Fingerprint.Length.ShouldBe(0);
                artifact.Fingerprint.Sha256.ShouldBeNull();
                continue;
            }

            using var stream = new FileStream(
                artifact.Path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read | FileShare.Delete);
            stream.Length.ShouldBe(artifact.Fingerprint.Length);
            Convert.ToHexString(SHA256.HashData(stream)).ShouldBe(
                artifact.Fingerprint.Sha256.ShouldNotBeNull());
        }

        var plugin = committed.Artifacts.Single(artifact => artifact.Role == PluginArtifactRole.Plugin);
        Path.GetExtension(plugin.Path).ToLowerInvariant().ShouldBe(".esm");
        plugin.Fingerprint.Exists.ShouldBeTrue();
    }

    /// <summary>Requires a definitive application external-change rejection that retained local authority.</summary>
    /// <param name="result">The typed application save result.</param>
    /// <param name="workspaceId">The exact application workspace.</param>
    /// <param name="before">The state captured before either owner saved.</param>
    private static void AssertRejectedApplicationSave(
        SaveResult result,
        Guid workspaceId,
        WorkspaceState before)
    {
        result.WorkspaceId.ShouldBe(workspaceId);
        result.OperationId.ShouldNotBe(Guid.Empty);
        result.BaseRevision.ShouldBe(before.Revision);
        result.ResultRevision.ShouldBe(before.Revision);
        result.Status.ShouldBe(SaveCommitStatus.NotCommitted);
        result.Error.ShouldNotBeNull().Code.ShouldBe(EngineErrorCode.ExternalChangeDetected);
        result.CommittedBaseline.ShouldBeNull();
        result.RecoveryEvidenceToken.ShouldBeNull();
        result.ResolvedEvidence.ShouldBeNull();
    }

    /// <summary>Requires a known committed MCP result with exact request and retained metadata identity.</summary>
    /// <param name="result">The structured MCP save result.</param>
    /// <param name="request">The exact request that committed.</param>
    private static void AssertCommittedMcpSave(PluginMcpSaveOutcome result, PluginMcpSaveRequest request)
    {
        result.WorkspaceId.ShouldBe(request.WorkspaceId);
        result.OperationId.ShouldBe(request.OperationId);
        result.BaseRevision.ShouldBe(request.ExpectedRevision);
        result.ResultRevision.ShouldNotBe(request.ExpectedRevision);
        result.Status.ShouldBe("committed");
        result.ErrorCode.ShouldBeNull();
        result.CommittedBaselineHandle.ShouldNotBeNullOrWhiteSpace();
        result.CommittedBaselineHandle.ShouldNotBe(request.ExpectedOutputBaselineHandle);
        result.OutputHandle.ShouldBeNull();
        result.ResolvedEvidenceHandle.ShouldBeNull();
        result.RecoveryEvidenceToken.ShouldBeNull();
        result.WarningCount.ShouldBe(0);
        result.DetailsUnavailable.ShouldBeFalse();
        result.DetailsHandle.ShouldNotBeNullOrWhiteSpace();
        result.DetailsKind.ShouldBe("save_result");
    }

    /// <summary>Requires a definitive MCP external-change rejection with no committed baseline.</summary>
    /// <param name="result">The structured MCP save result.</param>
    /// <param name="request">The exact stale request.</param>
    private static void AssertRejectedMcpSave(PluginMcpSaveOutcome result, PluginMcpSaveRequest request)
    {
        result.WorkspaceId.ShouldBe(request.WorkspaceId);
        result.OperationId.ShouldBe(request.OperationId);
        result.BaseRevision.ShouldBe(request.ExpectedRevision);
        result.ResultRevision.ShouldBe(request.ExpectedRevision);
        result.Status.ShouldBe("not_committed");
        result.ErrorCode.ShouldBe("external_change_detected");
        result.CommittedBaselineHandle.ShouldBeNull();
        result.OutputHandle.ShouldBeNull();
        result.ResolvedEvidenceHandle.ShouldBeNull();
        result.RecoveryEvidenceToken.ShouldBeNull();
        result.WarningCount.ShouldBe(0);
        result.DetailsUnavailable.ShouldBeFalse();
        result.DetailsHandle.ShouldNotBeNullOrWhiteSpace();
        result.DetailsKind.ShouldBe("save_result");
    }

    /// <summary>Requires exact historical replay of one definitive MCP rejection.</summary>
    /// <param name="expected">The original result.</param>
    /// <param name="actual">The replayed result.</param>
    private static void AssertSameMcpSave(PluginMcpSaveOutcome expected, PluginMcpSaveOutcome actual)
    {
        actual.WorkspaceId.ShouldBe(expected.WorkspaceId);
        actual.OperationId.ShouldBe(expected.OperationId);
        actual.BaseRevision.ShouldBe(expected.BaseRevision);
        actual.ResultRevision.ShouldBe(expected.ResultRevision);
        actual.Status.ShouldBe(expected.Status);
        actual.ErrorCode.ShouldBe(expected.ErrorCode);
        actual.CommittedBaselineHandle.ShouldBe(expected.CommittedBaselineHandle);
        actual.OutputHandle.ShouldBe(expected.OutputHandle);
        actual.ResolvedEvidenceHandle.ShouldBe(expected.ResolvedEvidenceHandle);
        actual.RecoveryEvidenceToken.ShouldBe(expected.RecoveryEvidenceToken);
        actual.WarningCount.ShouldBe(expected.WarningCount);
        actual.DetailsUnavailable.ShouldBe(expected.DetailsUnavailable);
        actual.DetailsHandle.ShouldBe(expected.DetailsHandle);
        actual.DetailsKind.ShouldBe(expected.DetailsKind);
    }

    /// <summary>Requires two detached application previews to retain the same complete before-and-after JSON.</summary>
    /// <param name="expected">The preview captured before another owner saved.</param>
    /// <param name="actual">The preview captured after the stale rejection.</param>
    private static void AssertPreviewsEqual(WorkspacePreview expected, WorkspacePreview actual)
    {
        actual.UnresolvedReferenceCount.ShouldBe(expected.UnresolvedReferenceCount);
        actual.Comparisons.Count.ShouldBe(expected.Comparisons.Count);
        for (var index = 0; index < expected.Comparisons.Count; index++)
        {
            var expectedComparison = expected.Comparisons[index];
            var actualComparison = actual.Comparisons[index];
            actualComparison.FormKey.ShouldBe(expectedComparison.FormKey);
            actualComparison.Before.HasValue.ShouldBe(expectedComparison.Before.HasValue);
            actualComparison.After.HasValue.ShouldBe(expectedComparison.After.HasValue);
            if (expectedComparison.Before.HasValue)
            {
                JsonElement.DeepEquals(
                    actualComparison.Before.ShouldNotBeNull(),
                    expectedComparison.Before.Value).ShouldBeTrue();
            }

            if (expectedComparison.After.HasValue)
            {
                JsonElement.DeepEquals(
                    actualComparison.After.ShouldNotBeNull(),
                    expectedComparison.After.Value).ShouldBeTrue();
            }
        }
    }

    /// <summary>Requires an artifact snapshot to differ from its pre-save content without changing membership.</summary>
    /// <param name="before">The initial complete embedded output set.</param>
    /// <param name="after">The winning complete embedded output set.</param>
    private static void AssertArtifactSetChanged(
        IReadOnlyDictionary<string, byte[]> before,
        IReadOnlyDictionary<string, byte[]> after)
    {
        after.Keys.ShouldBe(before.Keys, ignoreOrder: true);
        after.Any(pair => !pair.Value.SequenceEqual(before[pair.Key])).ShouldBeTrue();
    }

    /// <summary>Requires exact artifact membership and bytes.</summary>
    /// <param name="expected">The authoritative complete embedded output set.</param>
    /// <param name="actual">The later snapshot.</param>
    private static void AssertArtifactsEqual(
        IReadOnlyDictionary<string, byte[]> expected,
        IReadOnlyDictionary<string, byte[]> actual)
    {
        actual.Keys.ShouldBe(expected.Keys, ignoreOrder: true);
        foreach (var (path, bytes) in expected)
        {
            actual[path].ShouldBe(bytes);
        }
    }

    /// <summary>Populates the real replacement workflow with explicit fixture paths and existing-output mode.</summary>
    /// <param name="selection">The fresh production selection view model.</param>
    /// <param name="fixture">The generated plugin fixture.</param>
    private static void ConfigureReplacement(
        WorkspaceSelectionViewModel selection,
        DesktopWorkspaceFixture fixture)
    {
        selection.SelectedGame = selection.Games.Single(option => option.Game == fixture.Game);
        selection.SourcePluginPath = fixture.SourcePluginPath;
        selection.LoadOrderPluginPaths.Add(fixture.SourcePluginPath);
        selection.DataDirectoryPath = fixture.DataDirectory.FullName;
        selection.OutputPluginPath = fixture.ExistingOutput.PluginPath;
        selection.OutputMode = OutputSelectionMode.OpenExisting;
        selection.LocalizedOutputMode = fixture.ExistingOutput.LocalizedOutputMode;
        selection.OutputMasterStyle = fixture.ExistingOutput.MasterStyle;
    }

    /// <summary>Uses a third independent production plugin lifetime to verify the committed disk record and clean state.</summary>
    /// <param name="fixture">The generated plugin fixture.</param>
    /// <param name="expectedEditorId">The first writer's committed EditorID.</param>
    /// <param name="cancellationToken">The finite case token.</param>
    /// <returns>A task that completes after the independent workspace closes.</returns>
    private static async Task AssertFreshPluginOutputAsync(
        DesktopWorkspaceFixture fixture,
        string expectedEditorId,
        CancellationToken cancellationToken)
    {
        await using var container = fixture.CreateContainer($"CrossSurface-Final-{fixture.Game}");
        var coordinator = container.Resolve<IWorkspaceCoordinator>();
        var opened = await coordinator.OpenAsync(
            new WorkspaceLaunchRequest(
                fixture.CreateSourceRequest(),
                OutputSelectionMode.OpenExisting,
                fixture.ExistingOutput),
            cancellationToken);
        opened.Succeeded.ShouldBeTrue(opened.Error?.Message);
        var record = await ReadRecordAsync(coordinator, fixture.ExistingOutputFormKey, cancellationToken);
        record.GetProperty("EditorID").GetString().ShouldBe(expectedEditorId);
        (await ReadPreviewAsync(coordinator, cancellationToken)).Comparisons.ShouldBeEmpty();
        await coordinator.CloseAsync();
    }

    /// <summary>Owns one attached headless window and its production changes view.</summary>
    private sealed class AttachedChangesView : IDisposable
    {
        /// <summary>Initializes ownership of one visible headless changes surface.</summary>
        /// <param name="window">The attached headless window.</param>
        /// <param name="view">The production changes view.</param>
        public AttachedChangesView(Window window, WorkspaceChangesView view)
        {
            Window = window;
            View = view;
        }

        /// <summary>Gets the attached production changes view.</summary>
        public WorkspaceChangesView View { get; }

        /// <summary>The headless window that owns the view's visual-tree attachment.</summary>
        private Window Window { get; }

        /// <summary>Closes the headless window and detaches the production view.</summary>
        public void Dispose()
        {
            Window.Close();
        }
    }
}

/// <summary>Identifies which independent production surface attempts the first guarded save.</summary>
public enum CrossSurfaceSaveOrder
{
    /// <summary>The desktop application commits before the MCP workspace attempts its stale save.</summary>
    ApplicationFirst,

    /// <summary>The physical MCP host commits before the desktop application attempts its stale save.</summary>
    McpFirst,
}
