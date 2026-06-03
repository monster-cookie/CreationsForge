using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Enums;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class KeywordRepository : IKeywordRepository
{
    private readonly IDatabase Database;

    public KeywordRepository(IDatabase database)
    {
        Database = database;
    }

    public IList<KeywordDTO> GetByModKey(ModKey modKey)
    {
        return Database.Fetch<Keyword>("SELECT * FROM Keyword WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName COLLATE NOCASE ORDER BY FormKey_ID;", new { ModKeyName = modKey.Name, ModKeyType = (int)modKey.Type, ModKeyFileName = modKey.FileName }).Select(x => new KeywordDTO(x)).ToList();
    }

    public IList<KeywordDTO> GetByFormKey(FormKey formKey)
    {
        return Database.Fetch<Keyword>("SELECT Keyword.* FROM Keyword INNER JOIN Plugins ON Plugins.ModKey_Name = Keyword.ModKey_Name AND Plugins.ModKey_Type = Keyword.ModKey_Type AND Plugins.ModKey_FileName = Keyword.ModKey_FileName WHERE Keyword.FormKey_ModKey_Name = @FormKeyModKeyName AND Keyword.FormKey_ModKey_Type = @FormKeyModKeyType AND Keyword.FormKey_ModKey_FileName = @FormKeyModKeyFileName AND Keyword.FormKey_ID = @FormKeyID AND Plugins.Enabled = 1 AND Plugins.ExistsOnDisk = 1 AND Plugins.ImportState = @ImportState ORDER BY Plugins.LoadOrderIndex;",
            new { FormKeyModKeyName = formKey.ModKey.Name, FormKeyModKeyType = (int)formKey.ModKey.Type, FormKeyModKeyFileName = formKey.ModKey.FileName, FormKeyID = formKey.ID, ImportState = nameof(PluginImportState.Current) }).Select(x => new KeywordDTO(x)).ToList();
    }

    public void Save(KeywordDTO dto)
    {
        Database.Save(new Keyword(dto));
    }
}