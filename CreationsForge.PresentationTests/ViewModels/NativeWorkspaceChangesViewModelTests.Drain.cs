using CreationsForge.PresentationTests.Support;
using CreationsForge.ViewModels;
using Shouldly;

namespace CreationsForge.PresentationTests.ViewModels;

/// <content>Verifies admitted-operation cleanup remains drainable when presentation dispatch fails.</content>
public sealed partial class NativeWorkspaceChangesViewModelTests
{
    /// <summary>Verifies startup and terminal dispatcher failures clear the operation and release the transition lease.</summary>
    /// <param name="throwOnInvokeNumber">The awaited dispatcher call that fails.</param>
    /// <returns>A task that completes after the scenario assertions.</returns>
    [Theory]
    [InlineData(1)]
    [InlineData(6)]
    public async Task ReviewChangesAsync_DispatcherFailure_DoesNotLeakOperationOrTransition(int throwOnInvokeNumber)
    {
        await using var context = NativeWorkspaceChangesTestContext.CreateReady(hasStagedChanges: true);
        context.Dispatcher.InvokeException = new InvalidOperationException("Injected dispatcher failure.");
        context.Dispatcher.ThrowOnInvokeNumber = throwOnInvokeNumber;

        await Should.ThrowAsync<InvalidOperationException>(() => context.ViewModel.ReviewChangesAsync());

        context.ViewModel.IsBusy.ShouldBeFalse();
        context.ViewModel.OperationState.ShouldBe(NativeWorkspaceChangesOperationState.Idle);
        context.Arbiter.IsWorkspaceTransitionPendingOrReserved.ShouldBeFalse();
        context.ViewModel.CanReviewChanges.ShouldBeTrue();

        context.Dispatcher.InvokeException = null;
        context.Dispatcher.ThrowOnInvokeNumber = null;
        await context.ViewModel.ReviewChangesAsync();
        context.ViewModel.CurrentReview.ShouldNotBeNull();
    }
}
