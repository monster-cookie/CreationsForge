using CreationsForge.Services;
using CreationsForge.Services.Interfaces;

namespace CreationsForge.PresentationTests.Support;

/// <summary>
/// Records browser-owned picker requests and delegates deterministic selection behavior to each test.
/// </summary>
internal sealed class RecordingReferencePickerService : IReferencePickerService
{
    /// <summary>The callback that supplies each picker result.</summary>
    private readonly Func<ReferencePickerRequest, CancellationToken, Task<ReferencePickerSelection?>> PickAction;

    /// <summary>Initializes a recording picker that treats every interaction as canceled.</summary>
    public RecordingReferencePickerService()
        : this(static (_, _) => Task.FromResult<ReferencePickerSelection?>(null))
    {
    }

    /// <summary>Initializes a recording picker with deterministic behavior.</summary>
    /// <param name="pickAction">The callback that supplies each picker result.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="pickAction"/> is <see langword="null"/>.</exception>
    public RecordingReferencePickerService(
        Func<ReferencePickerRequest, CancellationToken, Task<ReferencePickerSelection?>> pickAction)
    {
        ArgumentNullException.ThrowIfNull(pickAction);
        PickAction = pickAction;
    }

    /// <summary>Gets picker requests in call order.</summary>
    public List<ReferencePickerRequest> Requests { get; } = [];

    /// <summary>Gets the cancellation tokens supplied with picker requests in call order.</summary>
    public List<CancellationToken> CancellationTokens { get; } = [];

    /// <inheritdoc />
    public Task<ReferencePickerSelection?> PickAsync(
        ReferencePickerRequest request,
        CancellationToken cancellationToken = default)
    {
        Requests.Add(request);
        CancellationTokens.Add(cancellationToken);
        return PickAction(request, cancellationToken);
    }
}
