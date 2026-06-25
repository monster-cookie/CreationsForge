using CreationsForge.Core.DTOs.Records;

namespace CreationsForge.Core.Repositories.Interfaces;

/// <summary>
/// Persists VMAD script property struct member rows.
/// </summary>
public interface IScriptingAdapterPropertyStructMemberRepository
{
    /// <summary>
    /// Saves or replaces one VMAD script property struct member row.
    /// </summary>
    /// <param name="dto">The struct member row to persist.</param>
    void Save(ScriptingAdapterPropertyStructMemberDTO dto);
}
