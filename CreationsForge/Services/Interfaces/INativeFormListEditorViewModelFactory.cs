using CreationsForge.ViewModels;

namespace CreationsForge.Services.Interfaces;

/// <summary>Creates one browser-owned native FormList editor without introducing a composition cycle.</summary>
public interface INativeFormListEditorViewModelFactory
{
    /// <summary>Creates an editor bound to the supplied browser host for selection and post-mutation refresh.</summary>
    /// <param name="host">The browser-owned selection and refresh boundary.</param>
    /// <returns>The navigation-scope editor view model.</returns>
    NativeFormListEditorViewModel Create(INativeFormListEditorHost host);
}
