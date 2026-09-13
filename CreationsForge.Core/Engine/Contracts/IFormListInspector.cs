using System.Text.Json;
using Mutagen.Bethesda.Plugins.Records;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Writes complete typed FormList read views and compares detached FormList records.</summary>
public interface IFormListInspector
{
    /// <summary>Writes one complete typed FormList view to the caller-owned JSON writer.</summary>
    /// <param name="record">The detached FormList getter to inspect.</param>
    /// <param name="writer">The caller-owned writer that receives exactly one JSON value.</param>
    /// <param name="cancellationToken">A token observed while traversing record fields and collections.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="record"/> or <paramref name="writer"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    void WriteReadView(
        IMajorRecordGetter record,
        Utf8JsonWriter writer,
        CancellationToken cancellationToken);

    /// <summary>Compares complete typed FormList fields without using plugin equality or serialized JSON as authority.</summary>
    /// <param name="before">The detached prior FormList getter, or <see langword="null"/> when absent.</param>
    /// <param name="after">The detached resulting FormList getter, or <see langword="null"/> when absent.</param>
    /// <param name="cancellationToken">A token observed while traversing record fields and collections.</param>
    /// <returns>Immutable semantic change locations in deterministic record field order.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    IReadOnlyList<SemanticChangeDescriptor> Compare(
        IMajorRecordGetter? before,
        IMajorRecordGetter? after,
        CancellationToken cancellationToken);
}
