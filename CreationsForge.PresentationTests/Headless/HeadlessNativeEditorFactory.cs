using CreationsForge.NativeEditing.Drafts;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;
using CreationsForge.ViewModels;

namespace CreationsForge.PresentationTests.Headless;

/// <summary>Creates production editor view models with deterministic headless presentation dependencies.</summary>
internal static class HeadlessNativeEditorFactory
{
    /// <summary>Creates the browser-owned editor factory used by headless view tests.</summary>
    /// <param name="coordinator">The deterministic workspace coordinator.</param>
    /// <param name="operationArbiter">The shared editor and workspace-transition admission boundary.</param>
    /// <param name="picker">The deterministic native reference picker.</param>
    /// <param name="dispatcher">The dispatcher appropriate for the headless fixture.</param>
    /// <returns>The configured production editor factory.</returns>
    internal static INativeFormListEditorViewModelFactory Create(
        INativeWorkspaceCoordinator coordinator,
        INativeWorkspacePresentationOperationArbiter operationArbiter,
        INativeReferencePickerService picker,
        IUiDispatcher dispatcher)
    {
        var validator = new NativeFormListDraftValidator();
        return new NativeFormListEditorViewModelFactory(
            coordinator,
            operationArbiter,
            new NativeFormListWireCatalogResolver([], []),
            new NativeFormListDraftFactory(),
            validator,
            new NativeFormListDraftSerializer(validator),
            picker,
            dispatcher);
    }
}
