using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.Repositories.Interfaces;

/// <summary>
/// Persists and reads component reflection rows for typed Bethesda records.
/// </summary>
public interface IReflectionRepository
{
    /// <summary>
    /// Saves a reflection row for a parent record.
    /// </summary>
    /// <param name="dto">The reflection row to persist.</param>
    void Save(ReflectionDTO dto);

    /// <summary>
    /// Reads all reflection rows for the specified parent record form key and record type.
    /// </summary>
    /// <param name="game">The game that owns the parent record.</param>
    /// <param name="recordType">The Bethesda record type identifier.</param>
    /// <param name="formKey">The parent record form key.</param>
    /// <returns>The reflection rows for every plugin version of the parent record.</returns>
    IReadOnlyList<ReflectionDTO> GetByFormKey(SupportedGame game, string recordType, FormKeyDTO formKey);

    /// <summary>
    /// Deletes reflection rows for one plugin's version of a parent record before replacement import.
    /// </summary>
    /// <param name="game">The game that owns the parent record.</param>
    /// <param name="modKey">The plugin that contributed the parent record.</param>
    /// <param name="recordType">The Bethesda record type identifier.</param>
    /// <param name="formKey">The parent record form key.</param>
    void DeleteByRecord(SupportedGame game, ModKeyDTO modKey, string recordType, FormKeyDTO formKey);
}
