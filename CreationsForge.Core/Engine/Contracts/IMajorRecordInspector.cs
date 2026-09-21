using System.Text.Json;
using Mutagen.Bethesda.Plugins.Records;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Writes and compares complete detached Mutagen major-record values for one supported game package.</summary>
public interface IMajorRecordInspector
{
    /// <summary>Gets every concrete major-record family exposed by the installed game package in deterministic display order.</summary>
    IReadOnlyList<string> SupportedRecordTypes { get; }

    /// <summary>Writes one complete detached native record as a typed JSON tree in installed field order.</summary>
    /// <param name="record">The detached Mutagen major record to inspect.</param>
    /// <param name="writer">The caller-owned writer that receives exactly one JSON object.</param>
    /// <param name="cancellationToken">A token observed throughout native field and collection traversal.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="record"/> or <paramref name="writer"/> is <see langword="null"/>.</exception>
    /// <exception cref="NotSupportedException">Thrown when an installed native field cannot be inspected losslessly.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    void WriteReadView(
        IMajorRecordGetter record,
        Utf8JsonWriter writer,
        CancellationToken cancellationToken);

    /// <summary>Compares complete detached records through their native typed values in deterministic field order.</summary>
    /// <param name="before">The prior detached record, or <see langword="null"/> when absent.</param>
    /// <param name="after">The resulting detached record, or <see langword="null"/> when absent.</param>
    /// <param name="cancellationToken">A token observed throughout native field and collection traversal.</param>
    /// <returns>Immutable change locations whose paths identify the native fields that differ.</returns>
    /// <exception cref="NotSupportedException">Thrown when an installed native field cannot be compared losslessly.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    IReadOnlyList<SemanticChangeDescriptor> Compare(
        IMajorRecordGetter? before,
        IMajorRecordGetter? after,
        CancellationToken cancellationToken);
}
