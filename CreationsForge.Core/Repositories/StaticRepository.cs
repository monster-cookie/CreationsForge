using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

public class StaticRepository : TypedRecordRepositoryBase, IStaticRepository
{
    public StaticRepository(IDatabase database, IRecordInstanceRepository recordInstanceRepository)
        : base(database, recordInstanceRepository)
    { }

    public override string RecordType => RecordTypeCatalog.Static.RecordID;

    protected override string TableName => RecordTypeCatalog.Static.TableName;

    public IReadOnlyList<StaticDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return FetchByFormKey<StaticRow>(
                game,
                formKey,
                [
                    SelectColumn("Version2"),
                    SelectColumn("ObjectBounds_First", "ObjectBoundsFirst"),
                    SelectColumn("ObjectBounds_Second", "ObjectBoundsSecond"),
                    SelectColumn("MaxAngle"),
                    SelectColumn("UnknownDNAMFloat"),
                    SelectColumn("LeafAmplitude"),
                    SelectColumn("LeafFrequency"),
                    SelectColumn("Unused"),
                    SelectColumn("DNAMDataTypeState")
                ])
            .Select(record => ToDTO(record, game))
            .ToList();
    }

    public void Save(StaticDTO dto)
    {
        SaveRecordInstance(dto);
        Database.Execute(
            """
            INSERT OR REPLACE INTO Statics (
                Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                EditorID, FormVersion, MajorRecordFlags, ImportedAtUTC, Version2, ObjectBounds_First, ObjectBounds_Second, MaxAngle,
                UnknownDNAMFloat, LeafAmplitude, LeafFrequency, Unused, DNAMDataTypeState)
            VALUES (
                @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                @EditorId, @FormVersion, @MajorRecordFlags, @ImportedAtUTC, @Version2, @ObjectBoundsFirst, @ObjectBoundsSecond, @MaxAngle,
                @UnknownDNAMFloat, @LeafAmplitude, @LeafFrequency, @Unused, @DNAMDataTypeState);
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
                dto.MaxAngle,
                dto.UnknownDNAMFloat,
                dto.LeafAmplitude,
                dto.LeafFrequency,
                dto.Unused,
                dto.DNAMDataTypeState
            });
    }

    private static StaticDTO ToDTO(StaticRow record, SupportedGame game)
    {
        var dto = new StaticDTO
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
            MaxAngle = record.MaxAngle,
            UnknownDNAMFloat = record.UnknownDNAMFloat,
            LeafAmplitude = record.LeafAmplitude,
            LeafFrequency = record.LeafFrequency,
            Unused = record.Unused,
            DNAMDataTypeState = record.DNAMDataTypeState
        };
        ApplyCommonFields(dto, record, game);
        return dto;
    }

    private sealed class StaticRow : RecordRow
    {
        public int? Version2 { get; set; }

        public string? ObjectBoundsFirst { get; set; }

        public string? ObjectBoundsSecond { get; set; }

        public double? MaxAngle { get; set; }

        public double? UnknownDNAMFloat { get; set; }

        public double? LeafAmplitude { get; set; }

        public double? LeafFrequency { get; set; }

        public string? Unused { get; set; }

        public string? DNAMDataTypeState { get; set; }
    }
}
