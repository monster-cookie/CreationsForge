using CreationsForge.Core.Engine.RecordWire;
using CreationsForge.RecordEditing.Drafts;
using CreationsForge.Services.Interfaces;

namespace CreationsForge.ViewModels;

/// <summary>Creates browser-owned FormList editors from the services injected into their navigation scope.</summary>
public sealed class FormListEditorViewModelFactory : IFormListEditorViewModelFactory
{
    /// <summary>The application-wide workspace owner.</summary>
    private readonly IWorkspaceCoordinator WorkspaceCoordinator;

    /// <summary>The navigation-scope admission boundary shared by editor and workspace transitions.</summary>
    private readonly IWorkspacePresentationOperationArbiter OperationArbiter;

    /// <summary>The exact game and release catalog resolver.</summary>
    private readonly IFormListWireCatalogResolver CatalogResolver;

    /// <summary>The typed draft and detached seed factory.</summary>
    private readonly IFormListDraftFactory DraftFactory;

    /// <summary>The typed draft validator.</summary>
    private readonly IFormListDraftValidator DraftValidator;

    /// <summary>The detached command-argument serializer.</summary>
    private readonly IFormListDraftSerializer DraftSerializer;

    /// <summary>The bounded reference picker.</summary>
    private readonly IReferencePickerService ReferencePickerService;

    /// <summary>The presentation dispatcher.</summary>
    private readonly IUiDispatcher UiDispatcher;

    /// <summary>Initializes the editor factory from application and navigation-scope presentation services.</summary>
    /// <param name="workspaceCoordinator">The application-wide workspace owner.</param>
    /// <param name="operationArbiter">The navigation-scope editor and workspace-transition admission boundary.</param>
    /// <param name="catalogResolver">The exact game and release catalog resolver.</param>
    /// <param name="draftFactory">The typed draft and detached seed factory.</param>
    /// <param name="draftValidator">The typed draft validator.</param>
    /// <param name="draftSerializer">The detached command-argument serializer.</param>
    /// <param name="referencePickerService">The bounded reference picker.</param>
    /// <param name="uiDispatcher">The presentation dispatcher.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required dependency is <see langword="null"/>.</exception>
    public FormListEditorViewModelFactory(
        IWorkspaceCoordinator workspaceCoordinator,
        IWorkspacePresentationOperationArbiter operationArbiter,
        IFormListWireCatalogResolver catalogResolver,
        IFormListDraftFactory draftFactory,
        IFormListDraftValidator draftValidator,
        IFormListDraftSerializer draftSerializer,
        IReferencePickerService referencePickerService,
        IUiDispatcher uiDispatcher)
    {
        ArgumentNullException.ThrowIfNull(workspaceCoordinator);
        ArgumentNullException.ThrowIfNull(operationArbiter);
        ArgumentNullException.ThrowIfNull(catalogResolver);
        ArgumentNullException.ThrowIfNull(draftFactory);
        ArgumentNullException.ThrowIfNull(draftValidator);
        ArgumentNullException.ThrowIfNull(draftSerializer);
        ArgumentNullException.ThrowIfNull(referencePickerService);
        ArgumentNullException.ThrowIfNull(uiDispatcher);
        WorkspaceCoordinator = workspaceCoordinator;
        OperationArbiter = operationArbiter;
        CatalogResolver = catalogResolver;
        DraftFactory = draftFactory;
        DraftValidator = draftValidator;
        DraftSerializer = draftSerializer;
        ReferencePickerService = referencePickerService;
        UiDispatcher = uiDispatcher;
    }

    /// <inheritdoc />
    public FormListEditorViewModel Create(IFormListEditorHost host)
    {
        ArgumentNullException.ThrowIfNull(host);
        return new FormListEditorViewModel(
            WorkspaceCoordinator,
            host,
            OperationArbiter,
            CatalogResolver,
            DraftFactory,
            DraftValidator,
            DraftSerializer,
            ReferencePickerService,
            UiDispatcher,
            RecordWireReadLimits.Default);
    }
}
