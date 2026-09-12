using CreationsForge.Core.Engine.NativeWire;
using CreationsForge.NativeEditing.Drafts;
using CreationsForge.Services.Interfaces;

namespace CreationsForge.ViewModels;

/// <summary>Creates browser-owned native FormList editors from the services injected into their navigation scope.</summary>
public sealed class NativeFormListEditorViewModelFactory : INativeFormListEditorViewModelFactory
{
    /// <summary>The application-wide native workspace owner.</summary>
    private readonly INativeWorkspaceCoordinator WorkspaceCoordinator;

    /// <summary>The navigation-scope admission boundary shared by editor and workspace transitions.</summary>
    private readonly INativeWorkspacePresentationOperationArbiter OperationArbiter;

    /// <summary>The exact game and release catalog resolver.</summary>
    private readonly INativeFormListWireCatalogResolver CatalogResolver;

    /// <summary>The typed draft and detached seed factory.</summary>
    private readonly INativeFormListDraftFactory DraftFactory;

    /// <summary>The typed draft validator.</summary>
    private readonly INativeFormListDraftValidator DraftValidator;

    /// <summary>The detached command-argument serializer.</summary>
    private readonly INativeFormListDraftSerializer DraftSerializer;

    /// <summary>The bounded native reference picker.</summary>
    private readonly INativeReferencePickerService ReferencePickerService;

    /// <summary>The presentation dispatcher.</summary>
    private readonly IUiDispatcher UiDispatcher;

    /// <summary>Initializes the editor factory from application and navigation-scope presentation services.</summary>
    /// <param name="workspaceCoordinator">The application-wide native workspace owner.</param>
    /// <param name="operationArbiter">The navigation-scope editor and workspace-transition admission boundary.</param>
    /// <param name="catalogResolver">The exact game and release catalog resolver.</param>
    /// <param name="draftFactory">The typed draft and detached seed factory.</param>
    /// <param name="draftValidator">The typed draft validator.</param>
    /// <param name="draftSerializer">The detached command-argument serializer.</param>
    /// <param name="referencePickerService">The bounded native reference picker.</param>
    /// <param name="uiDispatcher">The presentation dispatcher.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required dependency is <see langword="null"/>.</exception>
    public NativeFormListEditorViewModelFactory(
        INativeWorkspaceCoordinator workspaceCoordinator,
        INativeWorkspacePresentationOperationArbiter operationArbiter,
        INativeFormListWireCatalogResolver catalogResolver,
        INativeFormListDraftFactory draftFactory,
        INativeFormListDraftValidator draftValidator,
        INativeFormListDraftSerializer draftSerializer,
        INativeReferencePickerService referencePickerService,
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
    public NativeFormListEditorViewModel Create(INativeFormListEditorHost host)
    {
        ArgumentNullException.ThrowIfNull(host);
        return new NativeFormListEditorViewModel(
            WorkspaceCoordinator,
            host,
            OperationArbiter,
            CatalogResolver,
            DraftFactory,
            DraftValidator,
            DraftSerializer,
            ReferencePickerService,
            UiDispatcher,
            NativeWireReadLimits.Default);
    }
}
