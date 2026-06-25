using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;
using ScriptingAdapterPropertyStructModel = CreationsForge.Core.Models.Database.ScriptingAdapterPropertyStruct;

namespace CreationsForge.Core.Repositories;

/// <summary>
/// NPoco repository for VMAD script property struct-list entries.
/// </summary>
public class ScriptingAdapterPropertyStructRepository : IScriptingAdapterPropertyStructRepository
{
    private readonly IDatabase Database;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptingAdapterPropertyStructRepository"/> class.
    /// </summary>
    /// <param name="database">The NPoco database connection for the current import scope.</param>
    public ScriptingAdapterPropertyStructRepository(IDatabase database)
    {
        Database = database;
    }

    /// <inheritdoc />
    public void Save(ScriptingAdapterPropertyStructDTO dto)
    {
        Database.Save(new ScriptingAdapterPropertyStructModel(dto));
    }
}
