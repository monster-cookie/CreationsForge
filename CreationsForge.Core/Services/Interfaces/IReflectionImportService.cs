using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.Services.Interfaces;

/// <summary>
/// Imports component reflection child rows for records that expose <c>REFL</c> data.
/// </summary>
public interface IReflectionImportService
{
    /// <summary>
    /// Replaces the stored reflection rows for a parent record.
    /// </summary>
    /// <param name="record">The parent record that owns the reflection rows.</param>
    /// <param name="recordType">The Bethesda record type identifier for the parent record.</param>
    void ReplaceReflections(IHasReflectionDTO record, string recordType);
}
