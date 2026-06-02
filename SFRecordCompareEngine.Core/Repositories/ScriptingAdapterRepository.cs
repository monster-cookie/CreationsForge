using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class ScriptingAdapterRepository : IScriptingAdapterRepository
{
    private readonly IDatabase Database;

    public ScriptingAdapterRepository(IDatabase database)
    {
        Database = database;
    }

    public IList<ScriptingAdapterDTO> GetByRecord(ModKey modKey, string recordType, FormKey formKey)
    {
        return Database.Fetch<ScriptingAdapter>(
                """
                SELECT *
                FROM ScriptingAdapters
                WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName COLLATE NOCASE
                  AND RecordType = @RecordType
                  AND FormKey_ID = @FormKeyID
                ORDER BY Script_Index;
                """,
                new { ModKeyName = modKey.Name, ModKeyType = (int)modKey.Type, ModKeyFileName = modKey.FileName, RecordType = recordType, FormKeyID = formKey.ID })
            .Select(model => new ScriptingAdapterDTO
            {
                ModKey = new ModKey(model.ModKeyName, (ModType)model.ModKeyType),
                RecordType = model.RecordType,
                FormKey = new FormKey(new ModKey(model.ModKeyName, (ModType)model.ModKeyType), (uint)model.FormKeyId),
                Name = model.Name,
                ScriptIndex = model.ScriptIndex,
                ImportedAtUTC = model.ImportedAtUTC
            })
            .ToList();
    }

    public void DeleteByRecord(ModKey modKey, string recordType, FormKey formKey)
    {
        Database.Delete<ScriptingAdapter>(
            """
            WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName COLLATE NOCASE
              AND RecordType = @RecordType
              AND FormKey_ID = @FormKeyID
            """,
            new { ModKeyName = modKey.Name, ModKeyType = (int)modKey.Type, ModKeyFileName = modKey.FileName, RecordType = recordType, FormKeyID = formKey.ID });
    }

    public void Save(ScriptingAdapterDTO dto)
    {
        Database.Save(new ScriptingAdapter(dto));
    }
}
