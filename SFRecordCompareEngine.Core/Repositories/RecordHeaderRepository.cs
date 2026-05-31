using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Enums;
using SFRecordCompareEngine.Core.Models.Database;

namespace SFRecordCompareEngine.Core.Repositories;

public abstract class RecordHeaderRepository<TModel, TRecordDTO>
    where TModel : RecordHeader
    where TRecordDTO : RecordHeaderDTO
{
    private readonly IDatabase Database;
    private readonly string TableName;

    protected RecordHeaderRepository(IDatabase database, string tableName)
    {
        Database = database;
        TableName = tableName;
    }

    public IList<TRecordDTO> GetByModKey(ModKey modKey)
    {
        return Database.Fetch<TModel>(
                $"""
                SELECT *
                FROM {TableName}
                WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName COLLATE NOCASE
                ORDER BY FormKey_ID;
                """,
                new { ModKeyName = modKey.Name, ModKeyType = (int)modKey.Type, ModKeyFileName = modKey.FileName })
            .Select(CreateDTO)
            .ToList();
    }

    public IList<TRecordDTO> GetByFormKeyID(uint formKeyID)
    {
        return Database.Fetch<TModel>(
                $"""
                SELECT {TableName}.*
                FROM {TableName}
                INNER JOIN Plugins
                    ON Plugins.ModKey_Name = {TableName}.ModKey_Name
                    AND Plugins.ModKey_Type = {TableName}.ModKey_Type
                    AND Plugins.ModKey_FileName = {TableName}.ModKey_FileName
                WHERE {TableName}.FormKey_ID = @FormKeyID
                  AND Plugins.Enabled = 1
                  AND Plugins.ExistsOnDisk = 1
                  AND Plugins.ImportState = @ImportState
                ORDER BY Plugins.LoadOrderIndex;
                """,
                new { FormKeyID = formKeyID, ImportState = nameof(PluginImportState.Current) })
            .Select(CreateDTO)
            .ToList();
    }

    public void Save(TRecordDTO dto)
    {
        Database.Save(CreateModel(dto));
    }

    protected abstract TRecordDTO CreateDTO(TModel model);
    protected abstract TModel CreateModel(TRecordDTO dto);
}
