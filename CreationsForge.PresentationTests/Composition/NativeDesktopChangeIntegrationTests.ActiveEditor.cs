using Autofac;
using Avalonia.Headless.XUnit;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;
using CreationsForge.ViewModels;
using Shouldly;

namespace CreationsForge.PresentationTests.Composition;

/// <content>Verifies guarded leave preserves a real successful native Apply when editor cancellation arrives before presentation receives its result.</content>
public sealed partial class NativeDesktopChangeIntegrationTests
{
    /// <summary>Bounds deterministic response and terminal-publication synchronization.</summary>
    private static readonly TimeSpan NativeApplyRaceDeadline = TimeSpan.FromSeconds(15);

    /// <summary>Verifies Starfield retains a successful Apply when cancel-and-wait loses the mutation race.</summary>
    /// <returns>A task that completes after the staged native record and unchanged files have been checked.</returns>
    [AvaloniaFact]
    public Task ProductionChanges_Starfield_CancelAndWaitPreservesSuccessfulApply()
    {
        return VerifySuccessfulApplyCancellationRaceAsync(SupportedGame.Starfield);
    }

    /// <summary>Verifies Fallout 4 retains a successful Apply when cancel-and-wait loses the mutation race.</summary>
    /// <returns>A task that completes after the staged native record and unchanged files have been checked.</returns>
    [AvaloniaFact]
    public Task ProductionChanges_Fallout4_CancelAndWaitPreservesSuccessfulApply()
    {
        return VerifySuccessfulApplyCancellationRaceAsync(SupportedGame.Fallout4);
    }

    /// <summary>Verifies Skyrim retains a successful Apply when cancel-and-wait loses the mutation race.</summary>
    /// <returns>A task that completes after the staged native record and unchanged files have been checked.</returns>
    [AvaloniaFact]
    public Task ProductionChanges_Skyrim_CancelAndWaitPreservesSuccessfulApply()
    {
        return VerifySuccessfulApplyCancellationRaceAsync(SupportedGame.Skyrim);
    }

    /// <summary>Holds an actual successful native Apply response, chooses cancel-and-wait, then verifies the exact receipt and fresh staged review survive.</summary>
    /// <param name="game">The supported game whose real native workspace performs the mutation.</param>
    /// <returns>A task that completes after all borrowed state and response gates have drained.</returns>
    private static async Task VerifySuccessfulApplyCancellationRaceAsync(SupportedGame game)
    {
        using var fixture = NativeDesktopWorkspaceFixture.Create(game);
        var sourceBytes = fixture.SnapshotSourceBytes();
        var outputBytes = File.ReadAllBytes(fixture.ExistingOutput.PluginPath);
        await using var container = fixture.CreateContainer($"ApplyCancellation-{game}");
        var coordinator = container.Resolve<INativeWorkspaceCoordinator>();
        await using var responseGate = new NativeDesktopApplyResultGate(coordinator);
        var dialog = new NativeDesktopChangeDialog();
        await using var viewScope = container.BeginLifetimeScope(builder =>
        {
            builder.RegisterInstance(dialog).As<INativeWorkspaceChangesDialogService>();
            builder.RegisterInstance(responseGate).As<INativeWorkspaceCoordinator>().ExternallyOwned();
        });
        var open = await coordinator.OpenAsync(new NativeWorkspaceOpenRequest(
            fixture.CreateSourceRequest(), OutputSelectionMode.OpenExisting, fixture.ExistingOutput),
            TestContext.Current.CancellationToken);
        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        var browser = viewScope.Resolve<NativeFormListBrowserViewModel>();
        var changes = viewScope.Resolve<NativeWorkspaceChangesViewModel>();
        var arbiter = viewScope.Resolve<INativeWorkspacePresentationOperationArbiter>();
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
            var receipt = await responseGate.HeldReceipt.WaitAsync(NativeApplyRaceDeadline);
            receipt.Revision.ShouldNotBe(previousRevision);
            editor.IsBusy.ShouldBeTrue();
            session.ExpectedRevision.ShouldBe(previousRevision);
            applyTask.IsCompleted.ShouldBeFalse();
            changes.CurrentReview.ShouldBeNull();
            dialog.Enqueue(NativeWorkspaceChangesDialogPurpose.Leave, async (model, cancellationToken) =>
            {
                model.ShowActiveEditorOperationDecision.ShouldBeTrue();
                model.CanCancelEditorOperationAndWait.ShouldBeTrue();
                model.CurrentReview.ShouldBeNull();
                arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeTrue();
                editor.CanBeginNew.ShouldBeFalse();
                var choiceTask = model.ApplyLeaveChoiceAsync(
                    NativeWorkspaceChangesDialogChoice.CancelEditorOperationAndWait,
                    cancellationToken);
                await responseGate.CancellationObserved.WaitAsync(NativeApplyRaceDeadline);
                choiceTask.IsCompleted.ShouldBeFalse();
                applyTask.IsCompleted.ShouldBeFalse();
                editor.IsBusy.ShouldBeTrue();
                responseGate.ReleaseHeldResult();
                (await choiceTask.WaitAsync(NativeApplyRaceDeadline))
                    .ShouldBe(NativeWorkspaceLeaveChoiceOutcome.ContinueDialog);
                await applyTask.WaitAsync(NativeApplyRaceDeadline);

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
                return NativeWorkspaceChangesDialogResult.KeepEditing;
            });

            var reservation = await changes.ReserveLeaveAsync(
                NativeWorkspaceLeaveReason.CloseWorkspace,
                TestContext.Current.CancellationToken).AsTask().WaitAsync(NativeApplyRaceDeadline);
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
            await applyTask.WaitAsync(NativeApplyRaceDeadline);
            await changes.ShutdownAndDrainAsync().WaitAsync(NativeApplyRaceDeadline);
            await coordinator.CloseAsync().AsTask().WaitAsync(NativeApplyRaceDeadline);
        }
    }
}
