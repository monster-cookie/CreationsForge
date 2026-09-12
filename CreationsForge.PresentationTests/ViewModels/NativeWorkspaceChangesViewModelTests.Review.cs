using CreationsForge.Core.Engine.Contracts;
using CreationsForge.PresentationTests.Support;
using CreationsForge.Services;
using Shouldly;

namespace CreationsForge.PresentationTests.ViewModels;

/// <content>Verifies revision-consistent review capture and fresh-preview authority.</content>
public sealed partial class NativeWorkspaceChangesViewModelTests
{
    /// <summary>Verifies review reads state and preview through one coordinator borrow before showing the dialog.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task ReviewChangesAsync_ReadyWorkspace_PublishesDetachedReviewFromOneBorrow()
    {
        await using var context = NativeWorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        context.DialogService.OnShowAsync = (viewModel, request) =>
        {
            request.Purpose.ShouldBe(NativeWorkspaceChangesDialogPurpose.Review);
            viewModel.CurrentReview.ShouldNotBeNull();
            viewModel.CurrentReview.WorkspaceId.ShouldBe(context.Workspace!.WorkspaceId);
            viewModel.CurrentReview.Revision.ShouldBe(context.Workspace.Revision);
            viewModel.CurrentReview.Items.Count.ShouldBe(1);
            return Task.FromResult(NativeWorkspaceChangesDialogResult.KeepEditing);
        };

        await context.ViewModel.ReviewChangesAsync();

        context.Coordinator.ExecuteCount.ShouldBe(1);
        context.Workspace!.ReadStateCount.ShouldBe(1);
        context.Workspace.PreviewCount.ShouldBe(1);
    }

    /// <summary>Verifies a preview that contradicts the captured workspace identity is rejected.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task ReviewChangesAsync_MismatchedPreviewIdentity_RejectsReview()
    {
        await using var context = NativeWorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        context.Workspace!.OnPreviewAsync = _ => ValueTask.FromResult(EngineResult<WorkspacePreview>.Success(
            context.Workspace.Preview,
            Guid.NewGuid(),
            baseRevision: context.Workspace.Revision,
            resultRevision: context.Workspace.Revision));
        context.DialogService.OnShowAsync = (viewModel, _) =>
        {
            viewModel.CurrentReview.ShouldBeNull();
            viewModel.ErrorCode.ShouldBe(EngineErrorCode.UnexpectedFailure);
            return Task.FromResult(NativeWorkspaceChangesDialogResult.KeepEditing);
        };

        await context.ViewModel.ReviewChangesAsync();

        context.DialogService.Requests.Count.ShouldBe(1);
        context.Workspace.PreviewCount.ShouldBe(1);
    }

    /// <summary>Verifies a fresh empty save preview overrides a previously displayed staged review.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task SaveChangesAsync_FreshEmptyPreview_DoesNotUseStaleDisplayedReview()
    {
        await using var context = NativeWorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        context.DialogService.OnShowAsync = async (viewModel, _) =>
        {
            viewModel.CurrentReview!.HasStagedChanges.ShouldBeTrue();
            context.Workspace!.Preview = new WorkspacePreview([], 0, []);
            await viewModel.SaveChangesAsync();
            viewModel.ErrorCode.ShouldBe(EngineErrorCode.InvalidRequest);
            return NativeWorkspaceChangesDialogResult.KeepEditing;
        };

        await context.ViewModel.ShowSaveChangesDialogAsync();

        context.Workspace!.SaveRequests.ShouldBeEmpty();
        context.Workspace.PreviewCount.ShouldBe(2);
    }

    /// <summary>Verifies request-local draft state prevents save dispatch even when staged engine changes exist.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task SaveChangesAsync_RequestLocalDraft_DoesNotDispatchSave()
    {
        await using var context = NativeWorkspaceChangesTestContext.CreateReady(
            hasStagedChanges: true,
            hasDraftChanges: true);
        context.DialogService.OnShowAsync = async (viewModel, _) =>
        {
            viewModel.CanSaveChanges.ShouldBeFalse();
            await viewModel.SaveChangesAsync();
            return NativeWorkspaceChangesDialogResult.KeepEditing;
        };

        await context.ViewModel.ShowSaveChangesDialogAsync();

        context.Workspace!.SaveRequests.ShouldBeEmpty();
    }

    /// <summary>Verifies a coordinator publication outside a transition invalidates the prior review.</summary>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Fact]
    public async Task CurrentWorkspace_WhenRepublishedOutsideTransition_ClearsReview()
    {
        await using var context = NativeWorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);

        await context.ViewModel.ReviewChangesAsync();
        context.ViewModel.CurrentReview.ShouldNotBeNull();

        context.Coordinator.SetWorkspace(null, null);

        context.ViewModel.CurrentReview.ShouldBeNull();
        context.ViewModel.OutputSynchronizationStatus.ShouldBeNull();
        context.ViewModel.StatusText.ShouldBe("No native workspace is open.");
    }
}
