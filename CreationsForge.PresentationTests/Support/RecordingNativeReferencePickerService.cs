using CreationsForge.Services;
using CreationsForge.Services.Interfaces;

namespace CreationsForge.PresentationTests.Support;

/// <summary>
/// Records browser-owned picker requests and delegates deterministic selection behavior to each test.
/// </summary>
internal sealed class RecordingNativeReferencePickerService : INativeReferencePickerService
{
    /// <summary>The callback that supplies each picker result.</summary>
    private readonly Func<NativeReferencePickerRequest, CancellationToken, Task<NativeReferencePickerSelection?>> PickAction;

    /// <summary>Initializes a recording picker that treats every interaction as canceled.</summary>
    public RecordingNativeReferencePickerService()
        : this(static (_, _) => Task.FromResult<NativeReferencePickerSelection?>(null))
    {
    }

    /// <summary>Initializes a recording picker with deterministic behavior.</summary>
    /// <param name="pickAction">The callback that supplies each picker result.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="pickAction"/> is <see langword="null"/>.</exception>
    public RecordingNativeReferencePickerService(
        Func<NativeReferencePickerRequest, CancellationToken, Task<NativeReferencePickerSelection?>> pickAction)
    {
        ArgumentNullException.ThrowIfNull(pickAction);
        PickAction = pickAction;
    }

    /// <summary>Gets picker requests in call order.</summary>
    public List<NativeReferencePickerRequest> Requests { get; } = [];

    /// <summary>Gets the cancellation tokens supplied with picker requests in call order.</summary>
    public List<CancellationToken> CancellationTokens { get; } = [];

    /// <inheritdoc />
    public Task<NativeReferencePickerSelection?> PickAsync(
        NativeReferencePickerRequest request,
        CancellationToken cancellationToken = default)
    {
        Requests.Add(request);
        CancellationTokens.Add(cancellationToken);
        return PickAction(request, cancellationToken);
    }
}
