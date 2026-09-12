using CreationsForge.Services;

namespace CreationsForge.Services.Interfaces;

/// <summary>Presents a bounded native reference search and returns one explicitly resolved selection.</summary>
public interface INativeReferencePickerService
{
    /// <summary>Shows an owner-bound reference picker for one exact workspace revision.</summary>
    /// <param name="request">The workspace, scope, nullability, and presentation context for the picker.</param>
    /// <param name="cancellationToken">A token that cancels and drains the modal picker operation.</param>
    /// <returns>The explicit native or null selection, or <see langword="null"/> when the user cancels.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    Task<NativeReferencePickerSelection?> PickAsync(
        NativeReferencePickerRequest request,
        CancellationToken cancellationToken = default);
}
