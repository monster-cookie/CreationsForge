using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

public class NPCRepository : TypedRecordRepositoryBase, INPCRepository
{
    public NPCRepository(IDatabase database, IRecordInstanceRepository recordInstanceRepository)
        : base(database, recordInstanceRepository)
    { }

    public override string RecordType => RecordTypeCatalog.NPC.RecordID;

    protected override string TableName => RecordTypeCatalog.NPC.TableName;

    public IReadOnlyList<NPCDTO> GetByFormKey(CreationsForge.Core.Enums.SupportedGame game, CreationsForge.Core.DTOs.Plugins.FormKeyDTO formKey)
    {
        return FetchByFormKey<NPCRow>(
                game,
                formKey,
                """
                ,
                CurrentRecord.Name,
                CurrentRecord.ShortName,
                CurrentRecord.LongName,
                CurrentRecord.DispositionBase,
                CurrentRecord.Aggression,
                CurrentRecord.Confidence,
                CurrentRecord.EnergyLevel,
                CurrentRecord.Responsibility,
                CurrentRecord.Assistance,
                CurrentRecord.GearedUpWeapons,
                CurrentRecord.HeightMin,
                CurrentRecord.HeightMax,
                CurrentRecord.SkinToneIndex,
                CurrentRecord.Pronoun,
                CurrentRecord.Voice_ModKey_Name AS VoiceModKeyName,
                CurrentRecord.Voice_ModKey_Type AS VoiceModKeyType,
                CurrentRecord.Voice_ModKey_FileName AS VoiceModKeyFileName,
                CurrentRecord.Voice_FormKey_ID AS VoiceFormKeyId,
                CurrentRecord.Race_ModKey_Name AS RaceModKeyName,
                CurrentRecord.Race_ModKey_Type AS RaceModKeyType,
                CurrentRecord.Race_ModKey_FileName AS RaceModKeyFileName,
                CurrentRecord.Race_FormKey_ID AS RaceFormKeyId,
                CurrentRecord.CombatOverridePackageList_ModKey_Name AS CombatOverridePackageListModKeyName,
                CurrentRecord.CombatOverridePackageList_ModKey_Type AS CombatOverridePackageListModKeyType,
                CurrentRecord.CombatOverridePackageList_ModKey_FileName AS CombatOverridePackageListModKeyFileName,
                CurrentRecord.CombatOverridePackageList_FormKey_ID AS CombatOverridePackageListFormKeyId,
                CurrentRecord.CombatStyle_ModKey_Name AS CombatStyleModKeyName,
                CurrentRecord.CombatStyle_ModKey_Type AS CombatStyleModKeyType,
                CurrentRecord.CombatStyle_ModKey_FileName AS CombatStyleModKeyFileName,
                CurrentRecord.CombatStyle_FormKey_ID AS CombatStyleFormKeyId,
                CurrentRecord.DefaultPackageList_ModKey_Name AS DefaultPackageListModKeyName,
                CurrentRecord.DefaultPackageList_ModKey_Type AS DefaultPackageListModKeyType,
                CurrentRecord.DefaultPackageList_ModKey_FileName AS DefaultPackageListModKeyFileName,
                CurrentRecord.DefaultPackageList_FormKey_ID AS DefaultPackageListFormKeyId,
                CurrentRecord.CrimeFaction_ModKey_Name AS CrimeFactionModKeyName,
                CurrentRecord.CrimeFaction_ModKey_Type AS CrimeFactionModKeyType,
                CurrentRecord.CrimeFaction_ModKey_FileName AS CrimeFactionModKeyFileName,
                CurrentRecord.CrimeFaction_FormKey_ID AS CrimeFactionFormKeyId
                """)
            .Select(record => ToDTO(record, game))
            .ToList();
    }

    public void Save(NPCDTO dto)
    {
        SaveRecordInstance(dto);
        Database.Execute(
            """
            INSERT OR REPLACE INTO NPCs (
                Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                EditorID, FormVersion, MajorRecordFlags, ImportedAtUTC, Name, ShortName, LongName, DispositionBase, Aggression, Confidence,
                EnergyLevel, Responsibility, Assistance, GearedUpWeapons, HeightMin, HeightMax, SkinToneIndex, Pronoun,
                Voice_ModKey_Name, Voice_ModKey_Type, Voice_ModKey_FileName, Voice_FormKey_ID,
                Race_ModKey_Name, Race_ModKey_Type, Race_ModKey_FileName, Race_FormKey_ID,
                CombatOverridePackageList_ModKey_Name, CombatOverridePackageList_ModKey_Type, CombatOverridePackageList_ModKey_FileName, CombatOverridePackageList_FormKey_ID,
                CombatStyle_ModKey_Name, CombatStyle_ModKey_Type, CombatStyle_ModKey_FileName, CombatStyle_FormKey_ID,
                DefaultPackageList_ModKey_Name, DefaultPackageList_ModKey_Type, DefaultPackageList_ModKey_FileName, DefaultPackageList_FormKey_ID,
                CrimeFaction_ModKey_Name, CrimeFaction_ModKey_Type, CrimeFaction_ModKey_FileName, CrimeFaction_FormKey_ID)
            VALUES (
                @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                @EditorId, @FormVersion, @MajorRecordFlags, @ImportedAtUTC, @Name, @ShortName, @LongName, @DispositionBase, @Aggression, @Confidence,
                @EnergyLevel, @Responsibility, @Assistance, @GearedUpWeapons, @HeightMin, @HeightMax, @SkinToneIndex, @Pronoun,
                @VoiceModKeyName, @VoiceModKeyType, @VoiceModKeyFileName, @VoiceFormKeyId,
                @RaceModKeyName, @RaceModKeyType, @RaceModKeyFileName, @RaceFormKeyId,
                @CombatOverridePackageListModKeyName, @CombatOverridePackageListModKeyType, @CombatOverridePackageListModKeyFileName, @CombatOverridePackageListFormKeyId,
                @CombatStyleModKeyName, @CombatStyleModKeyType, @CombatStyleModKeyFileName, @CombatStyleFormKeyId,
                @DefaultPackageListModKeyName, @DefaultPackageListModKeyType, @DefaultPackageListModKeyFileName, @DefaultPackageListFormKeyId,
                @CrimeFactionModKeyName, @CrimeFactionModKeyType, @CrimeFactionModKeyFileName, @CrimeFactionFormKeyId);
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
                dto.ShortName,
                dto.LongName,
                dto.DispositionBase,
                dto.Aggression,
                dto.Confidence,
                dto.EnergyLevel,
                dto.Responsibility,
                dto.Assistance,
                dto.GearedUpWeapons,
                dto.HeightMin,
                dto.HeightMax,
                dto.SkinToneIndex,
                dto.Pronoun,
                VoiceModKeyName = dto.VoiceFormKey?.ModKey.Name,
                VoiceModKeyType = dto.VoiceFormKey?.ModKey.Type,
                VoiceModKeyFileName = dto.VoiceFormKey?.ModKey.FileName,
                VoiceFormKeyId = dto.VoiceFormKey?.Id,
                RaceModKeyName = dto.RaceFormKey?.ModKey.Name,
                RaceModKeyType = dto.RaceFormKey?.ModKey.Type,
                RaceModKeyFileName = dto.RaceFormKey?.ModKey.FileName,
                RaceFormKeyId = dto.RaceFormKey?.Id,
                CombatOverridePackageListModKeyName = dto.CombatOverridePackageListFormKey?.ModKey.Name,
                CombatOverridePackageListModKeyType = dto.CombatOverridePackageListFormKey?.ModKey.Type,
                CombatOverridePackageListModKeyFileName = dto.CombatOverridePackageListFormKey?.ModKey.FileName,
                CombatOverridePackageListFormKeyId = dto.CombatOverridePackageListFormKey?.Id,
                CombatStyleModKeyName = dto.CombatStyleFormKey?.ModKey.Name,
                CombatStyleModKeyType = dto.CombatStyleFormKey?.ModKey.Type,
                CombatStyleModKeyFileName = dto.CombatStyleFormKey?.ModKey.FileName,
                CombatStyleFormKeyId = dto.CombatStyleFormKey?.Id,
                DefaultPackageListModKeyName = dto.DefaultPackageListFormKey?.ModKey.Name,
                DefaultPackageListModKeyType = dto.DefaultPackageListFormKey?.ModKey.Type,
                DefaultPackageListModKeyFileName = dto.DefaultPackageListFormKey?.ModKey.FileName,
                DefaultPackageListFormKeyId = dto.DefaultPackageListFormKey?.Id,
                CrimeFactionModKeyName = dto.CrimeFactionFormKey?.ModKey.Name,
                CrimeFactionModKeyType = dto.CrimeFactionFormKey?.ModKey.Type,
                CrimeFactionModKeyFileName = dto.CrimeFactionFormKey?.ModKey.FileName,
                CrimeFactionFormKeyId = dto.CrimeFactionFormKey?.Id
            });
    }

    private static NPCDTO ToDTO(NPCRow record, CreationsForge.Core.Enums.SupportedGame game)
    {
        var dto = new NPCDTO
        {
            Game = game,
            ModKey = new CreationsForge.Core.DTOs.Plugins.ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty },
            FormKey = new CreationsForge.Core.DTOs.Plugins.FormKeyDTO { ModKey = new CreationsForge.Core.DTOs.Plugins.ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty }, Id = 0 },
            EditorID = string.Empty,
            FormVersion = 0,
            MajorRecordFlags = 0,
            ImportedAtUTC = record.ImportedAtUTC,
            Name = record.Name,
            ShortName = record.ShortName,
            LongName = record.LongName,
            DispositionBase = record.DispositionBase,
            Aggression = record.Aggression,
            Confidence = record.Confidence,
            EnergyLevel = record.EnergyLevel,
            Responsibility = record.Responsibility,
            Assistance = record.Assistance,
            GearedUpWeapons = record.GearedUpWeapons,
            HeightMin = record.HeightMin,
            HeightMax = record.HeightMax,
            SkinToneIndex = record.SkinToneIndex,
            Pronoun = record.Pronoun,
            VoiceFormKey = CreateNullableFormKey(record.VoiceModKeyName, record.VoiceModKeyType, record.VoiceModKeyFileName, record.VoiceFormKeyId),
            RaceFormKey = CreateNullableFormKey(record.RaceModKeyName, record.RaceModKeyType, record.RaceModKeyFileName, record.RaceFormKeyId),
            CombatOverridePackageListFormKey = CreateNullableFormKey(record.CombatOverridePackageListModKeyName, record.CombatOverridePackageListModKeyType, record.CombatOverridePackageListModKeyFileName, record.CombatOverridePackageListFormKeyId),
            CombatStyleFormKey = CreateNullableFormKey(record.CombatStyleModKeyName, record.CombatStyleModKeyType, record.CombatStyleModKeyFileName, record.CombatStyleFormKeyId),
            DefaultPackageListFormKey = CreateNullableFormKey(record.DefaultPackageListModKeyName, record.DefaultPackageListModKeyType, record.DefaultPackageListModKeyFileName, record.DefaultPackageListFormKeyId),
            CrimeFactionFormKey = CreateNullableFormKey(record.CrimeFactionModKeyName, record.CrimeFactionModKeyType, record.CrimeFactionModKeyFileName, record.CrimeFactionFormKeyId)
        };
        ApplyCommonFields(dto, record, game);
        return dto;
    }

    private sealed class NPCRow : RecordRow
    {
        public string? Name { get; set; }
        public string? ShortName { get; set; }
        public string? LongName { get; set; }
        public int DispositionBase { get; set; }
        public string Aggression { get; set; } = string.Empty;
        public string Confidence { get; set; } = string.Empty;
        public int EnergyLevel { get; set; }
        public string Responsibility { get; set; } = string.Empty;
        public string Assistance { get; set; } = string.Empty;
        public int GearedUpWeapons { get; set; }
        public double HeightMin { get; set; }
        public double HeightMax { get; set; }
        public int? SkinToneIndex { get; set; }
        public string? Pronoun { get; set; }
        public string? VoiceModKeyName { get; set; }
        public int? VoiceModKeyType { get; set; }
        public string? VoiceModKeyFileName { get; set; }
        public long? VoiceFormKeyId { get; set; }
        public string? RaceModKeyName { get; set; }
        public int? RaceModKeyType { get; set; }
        public string? RaceModKeyFileName { get; set; }
        public long? RaceFormKeyId { get; set; }
        public string? CombatOverridePackageListModKeyName { get; set; }
        public int? CombatOverridePackageListModKeyType { get; set; }
        public string? CombatOverridePackageListModKeyFileName { get; set; }
        public long? CombatOverridePackageListFormKeyId { get; set; }
        public string? CombatStyleModKeyName { get; set; }
        public int? CombatStyleModKeyType { get; set; }
        public string? CombatStyleModKeyFileName { get; set; }
        public long? CombatStyleFormKeyId { get; set; }
        public string? DefaultPackageListModKeyName { get; set; }
        public int? DefaultPackageListModKeyType { get; set; }
        public string? DefaultPackageListModKeyFileName { get; set; }
        public long? DefaultPackageListFormKeyId { get; set; }
        public string? CrimeFactionModKeyName { get; set; }
        public int? CrimeFactionModKeyType { get; set; }
        public string? CrimeFactionModKeyFileName { get; set; }
        public long? CrimeFactionFormKeyId { get; set; }
    }
}
