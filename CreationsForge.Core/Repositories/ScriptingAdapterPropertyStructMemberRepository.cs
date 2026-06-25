using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;
using ScriptingAdapterPropertyStructMemberModel = CreationsForge.Core.Models.Database.ScriptingAdapterPropertyStructMember;

namespace CreationsForge.Core.Repositories;

/// <summary>
/// NPoco repository for VMAD script property struct member rows.
/// </summary>
public class ScriptingAdapterPropertyStructMemberRepository : IScriptingAdapterPropertyStructMemberRepository
{
    private readonly IDatabase Database;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptingAdapterPropertyStructMemberRepository"/> class.
    /// </summary>
    /// <param name="database">The NPoco database connection for the current import scope.</param>
    public ScriptingAdapterPropertyStructMemberRepository(IDatabase database)
    {
        Database = database;
    }

    /// <inheritdoc />
    public void Save(ScriptingAdapterPropertyStructMemberDTO dto)
    {
        Database.Save(new ScriptingAdapterPropertyStructMemberModel(dto));
    }
}
