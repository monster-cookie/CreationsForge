using CreationsForge.Core.DTOs.Records;

namespace CreationsForge.Core.Repositories.Interfaces;

/// <summary>
/// Persists VMAD script property struct-list entries.
/// </summary>
public interface IScriptingAdapterPropertyStructRepository
{
    /// <summary>
    /// Saves or replaces one VMAD script property struct row.
    /// </summary>
    /// <param name="dto">The struct row to persist.</param>
    void Save(ScriptingAdapterPropertyStructDTO dto);
}
