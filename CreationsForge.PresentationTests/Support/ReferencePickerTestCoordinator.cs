using System.ComponentModel;
using System.Runtime.CompilerServices;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;

namespace CreationsForge.PresentationTests.Support;

/// <summary>Provides deterministic borrowed workspace access and replacement notifications to picker tests.</summary>
internal sealed class ReferencePickerTestCoordinator : INativeWorkspaceCoordinator
{
    /// <summary>The test workspace borrowed by picker operations.</summary>
    private readonly IFormListWorkspace Workspace;

    /// <summary>Initializes a picker test coordinator with one active workspace.</summary>
    /// <param name="workspace">The workspace supplied to borrowed operations.</param>
    /// <param name="descriptor">The initial active workspace descriptor.</param>
    public ReferencePickerTestCoordinator(IFormListWorkspace workspace, NativeWorkspaceDescriptor descriptor)
    {
        Workspace = workspace;
        CurrentWorkspace = descriptor;
    }

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public NativeWorkspaceDescriptor? CurrentWorkspace { get; private set; }

    /// <summary>Publishes a replacement or closed workspace to subscribed presentation state.</summary>
    /// <param name="descriptor">The new workspace descriptor, or <see langword="null"/> after close.</param>
    public void PublishWorkspace(NativeWorkspaceDescriptor? descriptor)
    {
        CurrentWorkspace = descriptor;
        OnPropertyChanged(nameof(CurrentWorkspace));
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<T>> ExecuteAsync<T>(
        Func<IFormListWorkspace, CancellationToken, ValueTask<EngineResult<T>>> operation,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return operation(Workspace, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<NativeWorkspaceDescriptor>> OpenAsync(
        NativeWorkspaceOpenRequest request,
        CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public ValueTask CloseAsync()
    {
        PublishWorkspace(null);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        PublishWorkspace(null);
        return ValueTask.CompletedTask;
    }

    /// <summary>Raises a deterministic workspace property-change event.</summary>
    /// <param name="propertyName">The changed property name.</param>
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
