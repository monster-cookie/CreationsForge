using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeWire;

namespace CreationsForge.NativeEditing.Schema;

/// <summary>Indexes one union alternative without materializing its complete nested descriptor.</summary>
public sealed class NativeWireSchemaUnionOption
{
    /// <summary>The bounded lazy option resolver.</summary>
    private readonly Func<NativeWireReadLimits, CancellationToken, EngineResult<NativeWireSchemaDescriptor>> Resolver;

    /// <summary>Initializes one lightweight union alternative.</summary>
    /// <param name="key">The stable option key.</param>
    /// <param name="displayName">The searchable display identity.</param>
    /// <param name="discriminator">The exact <c>$type</c> discriminator, when the option has one.</param>
    /// <param name="resolver">The bounded lazy descriptor resolver.</param>
    public NativeWireSchemaUnionOption(
        string key,
        string displayName,
        string? discriminator,
        Func<NativeWireReadLimits, CancellationToken, EngineResult<NativeWireSchemaDescriptor>> resolver)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        ArgumentNullException.ThrowIfNull(resolver);
        Key = key;
        DisplayName = displayName;
        Discriminator = discriminator;
        Resolver = resolver;
    }

    /// <summary>Gets the stable alternative key.</summary>
    public string Key { get; }

    /// <summary>Gets the searchable catalog display identity.</summary>
    public string DisplayName { get; }

    /// <summary>Gets the exact package-scoped discriminator, or <see langword="null"/> for a structural option.</summary>
    public string? Discriminator { get; }

    /// <summary>Resolves the selected alternative without materializing its siblings.</summary>
    /// <param name="limits">The per-operation resource limits.</param>
    /// <param name="cancellationToken">A token observed during schema traversal.</param>
    /// <returns>The selected resolved shape or a typed schema failure.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    public EngineResult<NativeWireSchemaDescriptor> Resolve(
        NativeWireReadLimits limits,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(limits);
        cancellationToken.ThrowIfCancellationRequested();
        return Resolver(limits, cancellationToken);
    }
}
