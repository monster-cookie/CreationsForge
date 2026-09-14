using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using CreationsForge.PresentationTests.Support;
using CreationsForge.Services;
using CreationsForge.ViewModels;
using Serilog;
using Shouldly;

namespace CreationsForge.PresentationTests.ViewModels;

/// <summary>Verifies guarded plugin shell workspace actions, status projection, and navigation.</summary>
public sealed class WorkspaceShellViewModelTests
{
    /// <summary>Verifies each open action holds transition admission through a fresh selection workflow and releases it afterward.</summary>
    [Fact]
    public async Task OpenWorkspaceAsync_WhenRepeated_HoldsReservationThroughFreshSelectionWorkflows()
    {
        await using var context = WorkspaceChangesTestContext.CreateNoWorkspace();
        var dialog = new FakeWorkspaceSelectionDialogService();
        var factoryCount = 0;
        dialog.ShowAction = selection =>
        {
            context.Arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeTrue();
            context.Arbiter.TryBeginEditorOperation().ShouldBeNull();
            selection.OutputModeOptions.ShouldContain(OutputSelectionMode.CreateNew);
            selection.OutputModeOptions.ShouldContain(OutputSelectionMode.OpenExisting);
            return Task.FromResult(false);
        };
        using var viewModel = CreateShellViewModel(
            context,
            dialog,
            new FakeApplicationNavigationService(),
            () =>
            {
                factoryCount++;
                return CreateSelectionViewModel(context);
            });

        (await viewModel.OpenWorkspaceAsync()).ShouldBeFalse();
        context.Arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
        (await viewModel.OpenWorkspaceAsync()).ShouldBeFalse();

        factoryCount.ShouldBe(2);
        dialog.ViewModels.Count.ShouldBe(2);
        dialog.ViewModels[0].ShouldNotBeSameAs(dialog.ViewModels[1]);
        context.Coordinator.ExecuteCount.ShouldBe(0);
        context.Arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
    }

    /// <summary>Verifies a dirty workspace that remains in editing never opens replacement selection.</summary>
    [Fact]
    public async Task OpenWorkspaceAsync_WhenDirtyLeaveKeepsEditing_DoesNotOpenSelection()
    {
        await using var context = WorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        var dialog = new FakeWorkspaceSelectionDialogService();
        using var viewModel = CreateShellViewModel(context, dialog);

        var result = await viewModel.OpenWorkspaceAsync();

        result.ShouldBeFalse();
        dialog.ViewModels.ShouldBeEmpty();
        context.DialogService.Requests.ShouldHaveSingleItem().LeaveReason.ShouldBe(WorkspaceLeaveReason.OpenWorkspace);
        context.Coordinator.CurrentWorkspace.ShouldNotBeNull();
        context.Arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
    }

    /// <summary>Verifies a clean workspace closes under its reservation and publishes the empty shell status.</summary>
    [Fact]
    public async Task CloseWorkspaceAsync_WhenClean_ClosesAndUpdatesShellState()
    {
        await using var context = WorkspaceChangesTestContext.CreateReady();
        using var viewModel = CreateShellViewModel(context);
        var changedProperties = new List<string?>();
        viewModel.PropertyChanged += (_, eventArgs) => changedProperties.Add(eventArgs.PropertyName);

        viewModel.HasWorkspace.ShouldBeTrue();
        viewModel.WorkspaceStatusText.ShouldBe("Starfield: ChangesOutput.esp (Editing)");

        (await viewModel.CloseWorkspaceAsync()).ShouldBeTrue();

        context.Coordinator.CloseCount.ShouldBe(1);
        viewModel.HasWorkspace.ShouldBeFalse();
        viewModel.WorkspaceStatusText.ShouldBe("No plugin is open.");
        changedProperties.ShouldContain(nameof(WorkspaceShellViewModel.HasWorkspace));
        changedProperties.ShouldContain(nameof(WorkspaceShellViewModel.WorkspaceStatusText));
        context.Arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
    }

    /// <summary>Verifies a read-only workspace closes without invoking output review or persistence UI.</summary>
    [Fact]
    public async Task CloseWorkspaceAsync_WhenReadOnly_ClosesAfterSourceStateProof()
    {
        await using var context = WorkspaceChangesTestContext.CreateReadOnly();
        using var viewModel = CreateShellViewModel(context);

        viewModel.WorkspaceStatusText.ShouldBe("Starfield: ChangesSource.esm (Read-Only)");
        context.ViewModel.CanReviewChanges.ShouldBeFalse();
        context.ViewModel.CanOpenPersistenceDialog.ShouldBeFalse();

        (await viewModel.CloseWorkspaceAsync()).ShouldBeTrue();

        context.Coordinator.CloseCount.ShouldBe(1);
        context.DialogService.Requests.ShouldBeEmpty();
        context.Coordinator.CurrentWorkspace.ShouldBeNull();
        context.Arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
    }

    /// <summary>Verifies closing an already empty shell does not borrow a workspace and remains successful.</summary>
    [Fact]
    public async Task CloseWorkspaceAsync_WhenNoWorkspace_IsIdempotentWithoutBorrow()
    {
        await using var context = WorkspaceChangesTestContext.CreateNoWorkspace();
        using var viewModel = CreateShellViewModel(context);

        (await viewModel.CloseWorkspaceAsync()).ShouldBeTrue();

        context.Coordinator.CloseCount.ShouldBe(1);
        context.Coordinator.ExecuteCount.ShouldBe(0);
        viewModel.HasWorkspace.ShouldBeFalse();
        context.Arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
    }

    /// <summary>Verifies Settings navigation is awaited and a concurrent shell transition cannot enter.</summary>
    [Fact]
    public async Task ShowSettingsAsync_WhileNavigationIsActive_RemainsSingleFlight()
    {
        await using var context = WorkspaceChangesTestContext.CreateNoWorkspace();
        var entered = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var navigation = new FakeApplicationNavigationService
        {
            SettingsAction = async _ =>
            {
                entered.TrySetResult(true);
                await release.Task;
                return true;
            }
        };
        using var viewModel = CreateShellViewModel(context, navigation: navigation);

        var first = viewModel.ShowSettingsAsync();
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(5));

        viewModel.IsShellTransitionActive.ShouldBeTrue();
        viewModel.OpenWorkspaceCommand.CanExecute(null).ShouldBeFalse();
        (await viewModel.ShowSettingsAsync()).ShouldBeFalse();
        navigation.SettingsCount.ShouldBe(1);

        release.TrySetResult(true);
        (await first.WaitAsync(TimeSpan.FromSeconds(5))).ShouldBeTrue();
        viewModel.IsShellTransitionActive.ShouldBeFalse();
        viewModel.OpenWorkspaceCommand.CanExecute(null).ShouldBeTrue();
    }

    /// <summary>Creates a shell view model with the shared change lifecycle and deterministic navigation dependencies.</summary>
    /// <param name="context">The shared deterministic change-lifecycle context.</param>
    /// <param name="dialog">The optional complete workspace selection recorder.</param>
    /// <param name="navigation">The optional plugin navigation recorder.</param>
    /// <param name="selectionFactory">The optional selection view-model factory.</param>
    /// <returns>The configured shell view model.</returns>
    private static WorkspaceShellViewModel CreateShellViewModel(
        WorkspaceChangesTestContext context,
        FakeWorkspaceSelectionDialogService? dialog = null,
        FakeApplicationNavigationService? navigation = null,
        Func<WorkspaceSelectionViewModel>? selectionFactory = null)
    {
        return new WorkspaceShellViewModel(
            context.Coordinator,
            selectionFactory ?? (() => CreateSelectionViewModel(context)),
            dialog ?? new FakeWorkspaceSelectionDialogService(),
            context.ViewModel,
            navigation ?? new FakeApplicationNavigationService());
    }

    /// <summary>Creates the complete source-and-output selection workflow used by the shell.</summary>
    /// <param name="context">The context whose coordinator and dispatcher are shared with the shell.</param>
    /// <returns>A fresh selection view model.</returns>
    private static WorkspaceSelectionViewModel CreateSelectionViewModel(WorkspaceChangesTestContext context)
    {
        return new WorkspaceSelectionViewModel(
            context.Coordinator,
            new FakeWorkspacePathPicker(),
            new FakeGameSelectionService(),
            context.Dispatcher,
            new LoggerConfiguration().CreateLogger());
    }
}
