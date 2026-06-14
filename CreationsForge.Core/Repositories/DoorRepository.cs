using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

public class DoorRepository : TypedRecordRepositoryBase, IDoorRepository
{
    public DoorRepository(IDatabase database, IRecordInstanceRepository recordInstanceRepository)
        : base(database, recordInstanceRepository)
    { }

    public override string RecordType => RecordTypeCatalog.Door.RecordID;

    protected override string TableName => RecordTypeCatalog.Door.TableName;

    public IReadOnlyList<DoorDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return FetchByFormKey<DoorRow>(
                game,
                formKey,
                [
                    SelectColumn("Version2"),
                    SelectColumn("ObjectBounds_First", "ObjectBoundsFirst"),
                    SelectColumn("ObjectBounds_Second", "ObjectBoundsSecond"),
                    SelectColumn("Name"),
                    SelectColumn("Flags"),
                    SelectColumn("NativeTerminal_ModKey_Name", "NativeTerminalModKeyName"),
                    SelectColumn("NativeTerminal_ModKey_Type", "NativeTerminalModKeyType"),
                    SelectColumn("NativeTerminal_ModKey_FileName", "NativeTerminalModKeyFileName"),
                    SelectColumn("NativeTerminal_FormKey_ID", "NativeTerminalFormKeyId"),
                    SelectColumn("SoundLevel"),
                    SelectColumn("FacingAxisOverride")
                ])
            .Select(record => ToDTO(record, game))
            .ToList();
    }

    public void Save(DoorDTO dto)
    {
        SaveRecordInstance(dto);
        Database.Execute(
            """
            INSERT OR REPLACE INTO Doors (
                Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                EditorID, FormVersion, MajorRecordFlags, ImportedAtUTC, Version2, ObjectBounds_First, ObjectBounds_Second, Name, Flags,
                NativeTerminal_ModKey_Name, NativeTerminal_ModKey_Type, NativeTerminal_ModKey_FileName, NativeTerminal_FormKey_ID,
                SoundLevel, FacingAxisOverride)
            VALUES (
                @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                @EditorId, @FormVersion, @MajorRecordFlags, @ImportedAtUTC, @Version2, @ObjectBoundsFirst, @ObjectBoundsSecond, @Name, @Flags,
                @NativeTerminalModKeyName, @NativeTerminalModKeyType, @NativeTerminalModKeyFileName, @NativeTerminalFormKeyId,
                @SoundLevel, @FacingAxisOverride);
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
                dto.Version2,
                dto.ObjectBoundsFirst,
                dto.ObjectBoundsSecond,
                dto.Name,
                dto.Flags,
                NativeTerminalModKeyName = dto.NativeTerminalFormKey?.ModKey.Name,
                NativeTerminalModKeyType = dto.NativeTerminalFormKey?.ModKey.Type,
                NativeTerminalModKeyFileName = dto.NativeTerminalFormKey?.ModKey.FileName,
                NativeTerminalFormKeyId = dto.NativeTerminalFormKey?.Id,
                dto.SoundLevel,
                dto.FacingAxisOverride
            });
    }

    private static DoorDTO ToDTO(DoorRow record, SupportedGame game)
    {
        var dto = new DoorDTO
        {
            Game = game,
            ModKey = new ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty },
            FormKey = new FormKeyDTO { ModKey = new ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty }, Id = 0 },
            EditorID = string.Empty,
            FormVersion = 0,
            MajorRecordFlags = 0,
            ImportedAtUTC = record.ImportedAtUTC,
            Version2 = record.Version2,
            ObjectBoundsFirst = record.ObjectBoundsFirst,
            ObjectBoundsSecond = record.ObjectBoundsSecond,
            Name = record.Name,
            Flags = record.Flags,
            NativeTerminalFormKey = CreateNullableFormKey(record.NativeTerminalModKeyName, record.NativeTerminalModKeyType, record.NativeTerminalModKeyFileName, record.NativeTerminalFormKeyId),
            SoundLevel = record.SoundLevel,
            FacingAxisOverride = record.FacingAxisOverride
        };
        ApplyCommonFields(dto, record, game);
        return dto;
    }

    private sealed class DoorRow : RecordRow
    {
        public int? Version2 { get; set; }

        public string? ObjectBoundsFirst { get; set; }

        public string? ObjectBoundsSecond { get; set; }

        public string? Name { get; set; }

        public string? Flags { get; set; }

        public string? NativeTerminalModKeyName { get; set; }

        public int? NativeTerminalModKeyType { get; set; }

        public string? NativeTerminalModKeyFileName { get; set; }

        public long? NativeTerminalFormKeyId { get; set; }

        public string? SoundLevel { get; set; }

        public string? FacingAxisOverride { get; set; }
    }
}
