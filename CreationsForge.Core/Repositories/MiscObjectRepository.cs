using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

public class MiscObjectRepository : TypedRecordRepositoryBase, IMiscObjectRepository
{
    public MiscObjectRepository(IDatabase database, IRecordInstanceRepository recordInstanceRepository)
        : base(database, recordInstanceRepository)
    { }

    public override string RecordType => RecordTypeCatalog.MiscObject.RecordID;

    protected override string TableName => RecordTypeCatalog.MiscObject.TableName;

    public IReadOnlyList<MiscObjectDTO> GetByFormKey(CreationsForge.Core.Enums.SupportedGame game, CreationsForge.Core.DTOs.Plugins.FormKeyDTO formKey)
    {
        return FetchByFormKey<MiscObjectRow>(
                game,
                formKey,
                [
                    SelectColumn("Name"),
                    SelectColumn("ShortName"),
                    SelectColumn("Value"),
                    SelectColumn("Weight"),
                    SelectColumn("DirtinessScale"),
                    SelectColumn("FeaturedItemMessage_ModKey_Name", "FeaturedItemMessageModKeyName"),
                    SelectColumn("FeaturedItemMessage_ModKey_Type", "FeaturedItemMessageModKeyType"),
                    SelectColumn("FeaturedItemMessage_ModKey_FileName", "FeaturedItemMessageModKeyFileName"),
                    SelectColumn("FeaturedItemMessage_FormKey_ID", "FeaturedItemMessageFormKeyId"),
                    SelectColumn("FLAG", "Flag")
                ])
            .Select(record => ToDTO(record, game))
            .ToList();
    }

    public void Save(MiscObjectDTO dto)
    {
        SaveRecordInstance(dto);
        Database.Execute(
            """
            INSERT OR REPLACE INTO MiscItems (
                Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                EditorID, FormVersion, MajorRecordFlags, ImportedAtUTC, Name, ShortName, Value, Weight, DirtinessScale,
                FeaturedItemMessage_ModKey_Name, FeaturedItemMessage_ModKey_Type, FeaturedItemMessage_ModKey_FileName, FeaturedItemMessage_FormKey_ID, FLAG)
            VALUES (
                @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                @EditorId, @FormVersion, @MajorRecordFlags, @ImportedAtUTC, @Name, @ShortName, @Value, @Weight, @DirtinessScale,
                @FeaturedItemMessageModKeyName, @FeaturedItemMessageModKeyType, @FeaturedItemMessageModKeyFileName, @FeaturedItemMessageFormKeyId, @Flag);
            """,
            new
            {
                Game = dto.Game.ToString(),
                ModKeyName = dto.ModKey.Name,
                ModKeyType = dto.ModKey.Type,
                ModKeyFileName = dto.ModKey.FileName,
                FormKeyModKeyName = dto.FormKey.ModKey.Name,
                FormKeyModKeyType = dto.FormKey.ModKey.Type,
                FormKeyModKeyFileName = dto.FormKey.ModKey.FileName,
                FormKeyId = dto.FormKey.Id,
                EditorId = dto.EditorID,
                dto.FormVersion,
                dto.MajorRecordFlags,
                dto.ImportedAtUTC,
                Name = GetEnglishText(dto.Name),
                ShortName = GetEnglishText(dto.ShortName),
                dto.Value,
                dto.Weight,
                dto.DirtinessScale,
                FeaturedItemMessageModKeyName = dto.FeaturedItemMessageFormKey?.ModKey.Name,
                FeaturedItemMessageModKeyType = dto.FeaturedItemMessageFormKey?.ModKey.Type,
                FeaturedItemMessageModKeyFileName = dto.FeaturedItemMessageFormKey?.ModKey.FileName,
                FeaturedItemMessageFormKeyId = dto.FeaturedItemMessageFormKey?.Id,
                dto.Flag
            });
    }

    private static MiscObjectDTO ToDTO(MiscObjectRow record, CreationsForge.Core.Enums.SupportedGame game)
    {
        var dto = new MiscObjectDTO
        {
            Game = game,
            ModKey = new CreationsForge.Core.DTOs.Plugins.ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty },
            FormKey = new CreationsForge.Core.DTOs.Plugins.FormKeyDTO { ModKey = new CreationsForge.Core.DTOs.Plugins.ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty }, Id = 0 },
            EditorID = string.Empty,
            FormVersion = 0,
            MajorRecordFlags = 0,
            ImportedAtUTC = record.ImportedAtUTC,
            Name = FromEnglish(record.Name),
            ShortName = FromEnglish(record.ShortName),
            Value = record.Value,
            Weight = record.Weight,
            DirtinessScale = record.DirtinessScale,
            FeaturedItemMessageFormKey = CreateNullableFormKey(record.FeaturedItemMessageModKeyName, record.FeaturedItemMessageModKeyType, record.FeaturedItemMessageModKeyFileName, record.FeaturedItemMessageFormKeyId),
            Flag = record.Flag
        };
        ApplyCommonFields(dto, record, game);
        return dto;
    }

    private sealed class MiscObjectRow : RecordRow
    {
        public string? Name { get; set; }

        public string? ShortName { get; set; }

        public int? Value { get; set; }

        public float? Weight { get; set; }

        public float? DirtinessScale { get; set; }

        public string? FeaturedItemMessageModKeyName { get; set; }

        public int? FeaturedItemMessageModKeyType { get; set; }

        public string? FeaturedItemMessageModKeyFileName { get; set; }

        public long? FeaturedItemMessageFormKeyId { get; set; }

        public string? Flag { get; set; }
    }
}
