using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

public class ActorValueInformationRepository : TypedRecordRepositoryBase, IActorValueInformationRepository
{
    public ActorValueInformationRepository(IDatabase database, IRecordInstanceRepository recordInstanceRepository)
        : base(database, recordInstanceRepository)
    { }

    public override string RecordType => RecordTypeCatalog.ActorValueInformation.RecordID;

    protected override string TableName => RecordTypeCatalog.ActorValueInformation.TableName;

    public IReadOnlyList<ActorValueInformationDTO> GetByFormKey(CreationsForge.Core.Enums.SupportedGame game, CreationsForge.Core.DTOs.Plugins.FormKeyDTO formKey)
    {
        return FetchByFormKey<ActorValueInformationRow>(
                game,
                formKey,
                [
                    SelectColumn("Name"),
                    SelectColumn("Abbreviation"),
                    SelectColumn("ContextNotes"),
                    SelectColumn("DefaultValue"),
                    SelectColumn("Flags"),
                    SelectColumn("Type"),
                    SelectColumn("Min"),
                    SelectColumn("Max")
                ])
            .Select(record => ToDTO(record, game))
            .ToList();
    }

    public void Save(ActorValueInformationDTO dto)
    {
        SaveRecordInstance(dto);
        Database.Execute(
            """
            INSERT OR REPLACE INTO ActorValueInformation (
                Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                EditorID, FormVersion, MajorRecordFlags, ImportedAtUTC, Name, Abbreviation, ContextNotes, DefaultValue, Flags, Type, Min, Max)
            VALUES (
                @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                @EditorId, @FormVersion, @MajorRecordFlags, @ImportedAtUTC, @Name, @Abbreviation, @ContextNotes, @DefaultValue, @Flags, @Type, @Min, @Max);
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
                dto.Name,
                dto.Abbreviation,
                dto.ContextNotes,
                dto.DefaultValue,
                dto.Flags,
                dto.Type,
                dto.Min,
                dto.Max
            });
    }

    private static ActorValueInformationDTO ToDTO(ActorValueInformationRow record, CreationsForge.Core.Enums.SupportedGame game)
    {
        var dto = new ActorValueInformationDTO
        {
            Game = game,
            ModKey = new CreationsForge.Core.DTOs.Plugins.ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty },
            FormKey = new CreationsForge.Core.DTOs.Plugins.FormKeyDTO { ModKey = new CreationsForge.Core.DTOs.Plugins.ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty }, Id = 0 },
            EditorID = string.Empty,
            FormVersion = 0,
            MajorRecordFlags = 0,
            ImportedAtUTC = record.ImportedAtUTC,
            Name = record.Name,
            Abbreviation = record.Abbreviation,
            ContextNotes = record.ContextNotes,
            DefaultValue = record.DefaultValue,
            Flags = record.Flags,
            Type = record.Type,
            Min = record.Min,
            Max = record.Max
        };
        ApplyCommonFields(dto, record, game);
        return dto;
    }

    private sealed class ActorValueInformationRow : RecordRow
    {
        public string? Name { get; set; }

        public string? Abbreviation { get; set; }

        public string? ContextNotes { get; set; }

        public double? DefaultValue { get; set; }

        public string? Flags { get; set; }

        public string? Type { get; set; }

        public double? Min { get; set; }

        public double? Max { get; set; }
    }
}
