using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

public class ClassRepository : TypedRecordRepositoryBase, IClassRepository
{
    public ClassRepository(IDatabase database, IRecordInstanceRepository recordInstanceRepository)
        : base(database, recordInstanceRepository)
    { }

    public override string RecordType => RecordTypeCatalog.Class.RecordID;

    protected override string TableName => RecordTypeCatalog.Class.TableName;

    public IReadOnlyList<ClassDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        var records = FetchByFormKey<ClassRow>(
                game,
                formKey,
                [
                    SelectColumn("Version2"),
                    SelectColumn("VersionControl"),
                    SelectColumn("Name"),
                    SelectColumn("Description"),
                    SelectColumn("Teaches"),
                    SelectColumn("MaxTrainingLevel"),
                    SelectColumn("BleedoutDefault"),
                    SelectColumn("VoicePoints"),
                    SelectColumn("Unknown"),
                    SelectColumn("Unknown2")
                ])
            .Select(record => ToDTO(record, game))
            .ToList();
        var properties = FetchPropertiesByFormKey(game, formKey);
        var weights = FetchWeightsByFormKey(game, formKey);
        foreach (var record in records)
        {
            record.Properties = properties
                .Where(property => IsSameModKey(property.ModKey, record.ModKey))
                .OrderBy(property => property.PropertyIndex)
                .ToList();
            record.SkillWeights = weights
                .Where(weight => IsSameModKey(weight.ModKey, record.ModKey) && string.Equals(weight.WeightType, "Skill", StringComparison.Ordinal))
                .OrderBy(weight => weight.WeightIndex)
                .ToList();
            record.StatWeights = weights
                .Where(weight => IsSameModKey(weight.ModKey, record.ModKey) && string.Equals(weight.WeightType, "Stat", StringComparison.Ordinal))
                .OrderBy(weight => weight.WeightIndex)
                .ToList();
        }

        return records;
    }

    public void Save(ClassDTO dto)
    {
        SaveRecordInstance(dto);
        Database.Execute(
            """
            INSERT OR REPLACE INTO Classes (
                Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                EditorID, FormVersion, MajorRecordFlags, ImportedAtUTC, Version2, VersionControl, Name, Description, Teaches, MaxTrainingLevel, BleedoutDefault, VoicePoints, Unknown, Unknown2)
            VALUES (
                @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                @EditorId, @FormVersion, @MajorRecordFlags, @ImportedAtUTC, @Version2, @VersionControl, @Name, @Description, @Teaches, @MaxTrainingLevel, @BleedoutDefault, @VoicePoints, @Unknown, @Unknown2);
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
                dto.VersionControl,
                Name = GetEnglishText(dto.Name),
                Description = GetEnglishText(dto.Description),
                dto.Teaches,
                dto.MaxTrainingLevel,
                dto.BleedoutDefault,
                dto.VoicePoints,
                dto.Unknown,
                dto.Unknown2
            });
        ReplaceProperties(dto);
        ReplaceWeights(dto);
    }

    private IReadOnlyList<ClassPropertyDTO> FetchPropertiesByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<ClassPropertyRow>(
                """
                SELECT *
                FROM ClassProperties
                WHERE Game = @Game
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY ModKey_FileName COLLATE NOCASE, Property_Index;
                """,
                new
                {
                    Game = game.ToString(),
                    FormKeyModKeyName = formKey.ModKey.Name,
                    FormKeyModKeyType = formKey.ModKey.Type,
                    FormKeyModKeyFileName = formKey.ModKey.FileName,
                    FormKeyId = formKey.Id
                })
            .Select(row => ToDTO(row, game))
            .ToList();
    }

    private IReadOnlyList<ClassWeightDTO> FetchWeightsByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<ClassWeightRow>(
                """
                SELECT *
                FROM ClassWeights
                WHERE Game = @Game
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY ModKey_FileName COLLATE NOCASE, WeightType COLLATE NOCASE, Weight_Index;
                """,
                new
                {
                    Game = game.ToString(),
                    FormKeyModKeyName = formKey.ModKey.Name,
                    FormKeyModKeyType = formKey.ModKey.Type,
                    FormKeyModKeyFileName = formKey.ModKey.FileName,
                    FormKeyId = formKey.Id
                })
            .Select(row => ToDTO(row, game))
            .ToList();
    }

    private void ReplaceProperties(ClassDTO dto)
    {
        Database.Execute(
            """
            DELETE FROM ClassProperties
            WHERE Game = @Game
              AND ModKey_Name = @ModKeyName
              AND ModKey_Type = @ModKeyType
              AND ModKey_FileName = @ModKeyFileName
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId;
            """,
            CommonParameters(dto));

        foreach (var property in dto.Properties)
        {
            property.ImportedAtUTC = dto.ImportedAtUTC;
            Database.Execute(
                """
                INSERT OR REPLACE INTO ClassProperties (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    Property_Index, ActorValue_ModKey_Name, ActorValue_ModKey_Type, ActorValue_ModKey_FileName, ActorValue_FormKey_ID, Value, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @PropertyIndex, @ActorValueModKeyName, @ActorValueModKeyType, @ActorValueModKeyFileName, @ActorValueFormKeyId, @Value, @ImportedAtUTC);
                """,
                new
                {
                    Game = property.Game.ToString(),
                    ModKeyName = property.ModKey.Name,
                    ModKeyType = property.ModKey.Type,
                    ModKeyFileName = property.ModKey.FileName,
                    FormKeyModKeyName = property.FormKey.ModKey.Name,
                    FormKeyModKeyType = property.FormKey.ModKey.Type,
                    FormKeyModKeyFileName = property.FormKey.ModKey.FileName,
                    FormKeyId = property.FormKey.Id,
                    property.PropertyIndex,
                    ActorValueModKeyName = property.ActorValueFormKey?.ModKey.Name,
                    ActorValueModKeyType = property.ActorValueFormKey?.ModKey.Type,
                    ActorValueModKeyFileName = property.ActorValueFormKey?.ModKey.FileName,
                    ActorValueFormKeyId = property.ActorValueFormKey?.Id,
                    property.Value,
                    property.ImportedAtUTC
                });
        }
    }

    private void ReplaceWeights(ClassDTO dto)
    {
        Database.Execute(
            """
            DELETE FROM ClassWeights
            WHERE Game = @Game
              AND ModKey_Name = @ModKeyName
              AND ModKey_Type = @ModKeyType
              AND ModKey_FileName = @ModKeyFileName
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId;
            """,
            CommonParameters(dto));

        foreach (var weight in dto.SkillWeights.Concat(dto.StatWeights))
        {
            weight.ImportedAtUTC = dto.ImportedAtUTC;
            Database.Execute(
                """
                INSERT OR REPLACE INTO ClassWeights (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    WeightType, Weight_Index, Key, Value, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @WeightType, @WeightIndex, @Key, @Value, @ImportedAtUTC);
                """,
                new
                {
                    Game = weight.Game.ToString(),
                    ModKeyName = weight.ModKey.Name,
                    ModKeyType = weight.ModKey.Type,
                    ModKeyFileName = weight.ModKey.FileName,
                    FormKeyModKeyName = weight.FormKey.ModKey.Name,
                    FormKeyModKeyType = weight.FormKey.ModKey.Type,
                    FormKeyModKeyFileName = weight.FormKey.ModKey.FileName,
                    FormKeyId = weight.FormKey.Id,
                    weight.WeightType,
                    weight.WeightIndex,
                    weight.Key,
                    weight.Value,
                    weight.ImportedAtUTC
                });
        }
    }

    private static ClassDTO ToDTO(ClassRow record, SupportedGame game)
    {
        var dto = new ClassDTO
        {
            Game = game,
            ModKey = new ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty },
            FormKey = new FormKeyDTO { ModKey = new ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty }, Id = 0 },
            EditorID = string.Empty,
            FormVersion = 0,
            MajorRecordFlags = 0,
            ImportedAtUTC = record.ImportedAtUTC,
            Version2 = record.Version2,
            VersionControl = record.VersionControl,
            Name = FromEnglish(record.Name),
            Description = FromEnglish(record.Description),
            Teaches = record.Teaches,
            MaxTrainingLevel = record.MaxTrainingLevel,
            BleedoutDefault = record.BleedoutDefault,
            VoicePoints = record.VoicePoints,
            Unknown = record.Unknown,
            Unknown2 = record.Unknown2
        };
        ApplyCommonFields(dto, record, game);
        return dto;
    }

    private static ClassPropertyDTO ToDTO(ClassPropertyRow row, SupportedGame game)
    {
        return new ClassPropertyDTO
        {
            Game = game,
            ModKey = CreateModKey(row.ModKeyName, row.ModKeyType, row.ModKeyFileName),
            FormKey = CreateFormKey(row.FormKeyModKeyName, row.FormKeyModKeyType, row.FormKeyModKeyFileName, row.FormKeyId),
            PropertyIndex = row.PropertyIndex,
            ActorValueFormKey = CreateNullableFormKey(row.ActorValueModKeyName, row.ActorValueModKeyType, row.ActorValueModKeyFileName, row.ActorValueFormKeyId),
            Value = row.Value,
            ImportedAtUTC = row.ImportedAtUTC
        };
    }

    private static ClassWeightDTO ToDTO(ClassWeightRow row, SupportedGame game)
    {
        return new ClassWeightDTO
        {
            Game = game,
            ModKey = CreateModKey(row.ModKeyName, row.ModKeyType, row.ModKeyFileName),
            FormKey = CreateFormKey(row.FormKeyModKeyName, row.FormKeyModKeyType, row.FormKeyModKeyFileName, row.FormKeyId),
            WeightType = row.WeightType,
            WeightIndex = row.WeightIndex,
            Key = row.Key,
            Value = row.Value,
            ImportedAtUTC = row.ImportedAtUTC
        };
    }

    private static ModKeyDTO CreateModKey(string name, int type, string fileName)
    {
        return new ModKeyDTO { Name = name, Type = type, FileName = fileName };
    }

    private static FormKeyDTO CreateFormKey(string modKeyName, int modKeyType, string modKeyFileName, long formKeyId)
    {
        return new FormKeyDTO
        {
            ModKey = CreateModKey(modKeyName, modKeyType, modKeyFileName),
            Id = (uint)formKeyId
        };
    }

    private static bool IsSameModKey(ModKeyDTO first, ModKeyDTO second)
    {
        return first.Type == second.Type &&
            string.Equals(first.Name, second.Name, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(first.FileName, second.FileName, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class ClassRow : RecordRow
    {
        public int? Version2 { get; set; }
        public int? VersionControl { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Teaches { get; set; }
        public int? MaxTrainingLevel { get; set; }
        public double? BleedoutDefault { get; set; }
        public double? VoicePoints { get; set; }
        public double? Unknown { get; set; }
        public double? Unknown2 { get; set; }
    }

    private sealed class ClassPropertyRow
    {
        public string ModKeyName { get; set; } = string.Empty;
        public int ModKeyType { get; set; }
        public string ModKeyFileName { get; set; } = string.Empty;
        public string FormKeyModKeyName { get; set; } = string.Empty;
        public int FormKeyModKeyType { get; set; }
        public string FormKeyModKeyFileName { get; set; } = string.Empty;
        public long FormKeyId { get; set; }
        public int PropertyIndex { get; set; }
        public string? ActorValueModKeyName { get; set; }
        public int? ActorValueModKeyType { get; set; }
        public string? ActorValueModKeyFileName { get; set; }
        public long? ActorValueFormKeyId { get; set; }
        public double? Value { get; set; }
        public DateTime ImportedAtUTC { get; set; }
    }

    private sealed class ClassWeightRow
    {
        public string ModKeyName { get; set; } = string.Empty;
        public int ModKeyType { get; set; }
        public string ModKeyFileName { get; set; } = string.Empty;
        public string FormKeyModKeyName { get; set; } = string.Empty;
        public int FormKeyModKeyType { get; set; }
        public string FormKeyModKeyFileName { get; set; } = string.Empty;
        public long FormKeyId { get; set; }
        public string WeightType { get; set; } = string.Empty;
        public int WeightIndex { get; set; }
        public string Key { get; set; } = string.Empty;
        public double? Value { get; set; }
        public DateTime ImportedAtUTC { get; set; }
    }
}
