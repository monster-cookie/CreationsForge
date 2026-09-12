using System.Text.Json;
using Mutagen.Bethesda.Plugins.Records;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Writes complete typed FormList read views and compares detached native FormList records.</summary>
public interface IFormListNativeInspector
{
    /// <summary>Writes one complete typed native FormList view to the caller-owned JSON writer.</summary>
    /// <param name="record">The detached native FormList getter to inspect.</param>
    /// <param name="writer">The caller-owned writer that receives exactly one JSON value.</param>
    /// <param name="cancellationToken">A token observed while traversing native fields and collections.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="record"/> or <paramref name="writer"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    void WriteReadView(
        IMajorRecordGetter record,
        Utf8JsonWriter writer,
        CancellationToken cancellationToken);

    /// <summary>Compares complete typed native FormList fields without using native equality or serialized JSON as authority.</summary>
    /// <param name="before">The detached prior native FormList getter, or <see langword="null"/> when absent.</param>
    /// <param name="after">The detached resulting native FormList getter, or <see langword="null"/> when absent.</param>
    /// <param name="cancellationToken">A token observed while traversing native fields and collections.</param>
    /// <returns>Immutable semantic change locations in deterministic native field order.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    IReadOnlyList<SemanticChangeDescriptor> Compare(
        IMajorRecordGetter? before,
        IMajorRecordGetter? after,
        CancellationToken cancellationToken);
}
