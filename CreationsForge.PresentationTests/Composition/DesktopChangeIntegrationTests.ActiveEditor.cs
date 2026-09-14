using Autofac;
using Avalonia.Headless.XUnit;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;
using CreationsForge.ViewModels;
using Shouldly;

namespace CreationsForge.PresentationTests.Composition;

/// <content>Verifies guarded leave preserves a real successful plugin Apply when editor cancellation arrives before presentation receives its result.</content>
public sealed partial class DesktopChangeIntegrationTests
{
    /// <summary>Bounds deterministic response and terminal-publication synchronization.</summary>
    private static readonly TimeSpan ApplyRaceDeadline = TimeSpan.FromSeconds(15);

    /// <summary>Verifies Starfield retains a successful Apply when cancel-and-wait loses the mutation race.</summary>
    /// <returns>A task that completes after the staged record and unchanged files have been checked.</returns>
    [AvaloniaFact]
    public Task ProductionChanges_Starfield_CancelAndWaitPreservesSuccessfulApply()
    {
        return VerifySuccessfulApplyCancellationRaceAsync(SupportedGame.Starfield);
    }

    /// <summary>Verifies Fallout 4 retains a successful Apply when cancel-and-wait loses the mutation race.</summary>
    /// <returns>A task that completes after the staged record and unchanged files have been checked.</returns>
    [AvaloniaFact]
    public Task ProductionChanges_Fallout4_CancelAndWaitPreservesSuccessfulApply()
    {
        return VerifySuccessfulApplyCancellationRaceAsync(SupportedGame.Fallout4);
    }

    /// <summary>Verifies Skyrim retains a successful Apply when cancel-and-wait loses the mutation race.</summary>
    /// <returns>A task that completes after the staged record and unchanged files have been checked.</returns>
    [AvaloniaFact]
    public Task ProductionChanges_Skyrim_CancelAndWaitPreservesSuccessfulApply()
    {
        return VerifySuccessfulApplyCancellationRaceAsync(SupportedGame.Skyrim);
    }

    /// <summary>Holds an actual successful plugin Apply response, chooses cancel-and-wait, then verifies the exact receipt and fresh staged review survive.</summary>
    /// <param name="game">The supported game whose real workspace performs the mutation.</param>
    /// <returns>A task that completes after all borrowed state and response gates have drained.</returns>
    private static async Task VerifySuccessfulApplyCancellationRaceAsync(SupportedGame game)
    {
        using var fixture = DesktopWorkspaceFixture.Create(game);
        var sourceBytes = fixture.SnapshotSourceBytes();
        var outputBytes = File.ReadAllBytes(fixture.ExistingOutput.PluginPath);
        await using var container = fixture.CreateContainer($"ApplyCancellation-{game}");
        var coordinator = container.Resolve<IWorkspaceCoordinator>();
        await using var responseGate = new DesktopApplyResultGate(coordinator);
        var dialog = new DesktopChangeDialog();
        await using var viewScope = container.BeginLifetimeScope(builder =>
        {
            builder.RegisterInstance(dialog).As<IWorkspaceChangesDialogService>();
            builder.RegisterInstance(responseGate).As<IWorkspaceCoordinator>().ExternallyOwned();
        });
        var open = await coordinator.OpenAsync(new WorkspaceLaunchRequest(
            fixture.CreateSourceRequest(), OutputSelectionMode.OpenExisting, fixture.ExistingOutput),
            TestContext.Current.CancellationToken);
        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        var browser = viewScope.Resolve<FormListBrowserViewModel>();
        var changes = viewScope.Resolve<WorkspaceChangesViewModel>();
        var arbiter = viewScope.Resolve<IWorkspacePresentationOperationArbiter>();
        await browser.StartAsync();
        var editor = browser.Editor;
        await editor.BeginNewAsync(TestContext.Current.CancellationToken);
        editor.HasError.ShouldBeFalse(editor.ErrorMessage);
        var session = editor.Session.ShouldNotBeNull();
        var newFormKey = session.FormKey;
        var previousRevision = session.ExpectedRevision;
        var intendedEditorId = $"{game}ApplyWonCancellationRace";
        SetEditorIdDraft(editor, intendedEditorId);
        var applyTask = editor.ApplyAsync(TestContext.Current.CancellationToken);

        try
        {
            var receipt = await responseGate.HeldReceipt.WaitAsync(ApplyRaceDeadline);
            receipt.Revision.ShouldNotBe(previousRevision);
            editor.IsBusy.ShouldBeTrue();
            session.ExpectedRevision.ShouldBe(previousRevision);
            applyTask.IsCompleted.ShouldBeFalse();
            changes.CurrentReview.ShouldBeNull();
            dialog.Enqueue(WorkspaceChangesDialogPurpose.Leave, async (model, cancellationToken) =>
            {
                model.ShowActiveEditorOperationDecision.ShouldBeTrue();
                model.CanCancelEditorOperationAndWait.ShouldBeTrue();
                model.CurrentReview.ShouldBeNull();
                arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeTrue();
                editor.CanBeginNew.ShouldBeFalse();
                var choiceTask = model.ApplyLeaveChoiceAsync(
                    WorkspaceChangesDialogChoice.CancelEditorOperationAndWait,
                    cancellationToken);
                await responseGate.CancellationObserved.WaitAsync(ApplyRaceDeadline);
                choiceTask.IsCompleted.ShouldBeFalse();
                applyTask.IsCompleted.ShouldBeFalse();
                editor.IsBusy.ShouldBeTrue();
                responseGate.ReleaseHeldResult();
                (await choiceTask.WaitAsync(ApplyRaceDeadline))
                    .ShouldBe(WorkspaceLeaveChoiceOutcome.ContinueDialog);
                await applyTask.WaitAsync(ApplyRaceDeadline);

                editor.IsBusy.ShouldBeFalse();
                editor.HasPendingOperation.ShouldBeFalse();
                editor.HasDraftChanges.ShouldBeFalse();
                session.ExpectedRevision.ShouldBe(receipt.Revision);
                model.ShowActiveEditorOperationDecision.ShouldBeFalse();
                model.CanSaveAndProceed.ShouldBeTrue();
                model.CanDiscardAndProceed.ShouldBeTrue();
                var review = model.CurrentReview.ShouldNotBeNull();
                review.WorkspaceId.ShouldBe(session.WorkspaceId);
                review.Revision.ShouldBe(receipt.Revision);
                var comparison = review.Comparisons.ShouldHaveSingleItem();
                comparison.FormKey.ShouldBe(newFormKey);
                comparison.After.ShouldNotBeNull();
                comparison.After!.Value.GetProperty("EditorID").GetString()
                    .ShouldBe(intendedEditorId);
                File.ReadAllBytes(fixture.ExistingOutput.PluginPath).ShouldBe(outputBytes);
                fixture.SnapshotSourceBytes().ShouldBe(sourceBytes);
                return WorkspaceChangesDialogResult.KeepEditing;
            });

            var reservation = await changes.ReserveLeaveAsync(
                WorkspaceLeaveReason.CloseWorkspace,
                TestContext.Current.CancellationToken).AsTask().WaitAsync(ApplyRaceDeadline);
            reservation.ShouldBeNull();
            dialog.AssertDrained();
            arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
            coordinator.CurrentWorkspace.ShouldNotBeNull().WorkspaceId.ShouldBe(session.WorkspaceId);
            (await ReadStateAsync(coordinator)).Revision.ShouldBe(receipt.Revision);
            (await ReadRecordAsync(coordinator, newFormKey, RecordScope.StagedOutput))
                .GetProperty("EditorID").GetString().ShouldBe(intendedEditorId);
            File.ReadAllBytes(fixture.ExistingOutput.PluginPath).ShouldBe(outputBytes);
            fixture.SnapshotSourceBytes().ShouldBe(sourceBytes);
        }
        finally
        {
            responseGate.ReleaseHeldResult();
            await applyTask.WaitAsync(ApplyRaceDeadline);
            await changes.ShutdownAndDrainAsync().WaitAsync(ApplyRaceDeadline);
            await coordinator.CloseAsync().AsTask().WaitAsync(ApplyRaceDeadline);
        }
    }
}
