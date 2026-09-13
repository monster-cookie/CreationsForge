using CreationsForge.RecordEditing.Drafts;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;
using CreationsForge.ViewModels;

namespace CreationsForge.PresentationTests.Headless;

/// <summary>Creates production editor view models with deterministic headless presentation dependencies.</summary>
internal static class HeadlessRecordEditorFactory
{
    /// <summary>Creates the browser-owned editor factory used by headless view tests.</summary>
    /// <param name="coordinator">The deterministic workspace coordinator.</param>
    /// <param name="operationArbiter">The shared editor and workspace-transition admission boundary.</param>
    /// <param name="picker">The deterministic reference picker.</param>
    /// <param name="dispatcher">The dispatcher appropriate for the headless fixture.</param>
    /// <returns>The configured production editor factory.</returns>
    internal static IFormListEditorViewModelFactory Create(
        IWorkspaceCoordinator coordinator,
        IWorkspacePresentationOperationArbiter operationArbiter,
        IReferencePickerService picker,
        IUiDispatcher dispatcher)
    {
        var validator = new FormListDraftValidator();
        return new FormListEditorViewModelFactory(
            coordinator,
            operationArbiter,
            new FormListWireCatalogResolver([], []),
            new FormListDraftFactory(),
            validator,
            new FormListDraftSerializer(validator),
            picker,
            dispatcher);
    }
}
