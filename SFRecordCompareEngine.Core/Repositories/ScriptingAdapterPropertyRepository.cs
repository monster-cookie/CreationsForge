using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class ScriptingAdapterPropertyRepository : IScriptingAdapterPropertyRepository
{
    private readonly IDatabase Database;

    public ScriptingAdapterPropertyRepository(IDatabase database)
    {
        Database = database;
    }

    public IList<ScriptingAdapterPropertyDTO> GetByRecord(ModKey modKey, string recordType, FormKey formKey)
    {
        return Database.Fetch<ScriptingAdapterProperty>(
                """
                SELECT *
                FROM ScriptingAdapterProperties
                WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName COLLATE NOCASE
                  AND RecordType = @RecordType
                  AND FormKey_ID = @FormKeyID
                ORDER BY ScriptingAdapter_Name, Property_Index;
                """,
                new { ModKeyName = modKey.Name, ModKeyType = (int)modKey.Type, ModKeyFileName = modKey.FileName, RecordType = recordType, FormKeyID = formKey.ID })
            .Select(model => new ScriptingAdapterPropertyDTO(model))
            .ToList();
    }

    public void Save(ScriptingAdapterPropertyDTO dto)
    {
        Database.Save(new ScriptingAdapterProperty(dto));
    }
}
