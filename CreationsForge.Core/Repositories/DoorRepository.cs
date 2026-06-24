using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

public class DoorRepository : TypedRecordRepositoryBase, IDoorRepository
{
    private readonly IModelRepository ModelRepository;
    private readonly IKeywordMappingRepository KeywordMappingRepository;
    private readonly ISoundMappingRepository SoundMappingRepository;
    private readonly IScriptingAdapterRepository ScriptingAdapterRepository;
    private readonly IRecordComponentRepository RecordComponentRepository;
    private readonly IRawRecordPayloadRepository RawRecordPayloadRepository;

    public DoorRepository(
        IDatabase database,
        IRecordInstanceRepository recordInstanceRepository,
        IModelRepository modelRepository,
        IKeywordMappingRepository keywordMappingRepository,
        ISoundMappingRepository soundMappingRepository,
        IScriptingAdapterRepository scriptingAdapterRepository,
        IRecordComponentRepository recordComponentRepository,
        IRawRecordPayloadRepository rawRecordPayloadRepository)
        : base(database, recordInstanceRepository)
    {
        ModelRepository = modelRepository;
        KeywordMappingRepository = keywordMappingRepository;
        SoundMappingRepository = soundMappingRepository;
        ScriptingAdapterRepository = scriptingAdapterRepository;
        RecordComponentRepository = recordComponentRepository;
        RawRecordPayloadRepository = rawRecordPayloadRepository;
    }

    public override string RecordType => RecordTypeCatalog.Door.RecordID;

    protected override string TableName => RecordTypeCatalog.Door.TableName;

    public IReadOnlyList<DoorDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        var records = FetchByFormKey<DoorRow>(
                game,
                formKey,
                [
                    SelectColumn("Version2"),
                    SelectColumn("VersionControl"),
                    SelectColumn("ObjectBounds_First", "ObjectBoundsFirst"),
                    SelectColumn("ObjectBounds_Second", "ObjectBoundsSecond"),
                    SelectColumn("Name"),
                    SelectColumn("Flags"),
                    SelectColumn("NativeTerminal_ModKey_Name", "NativeTerminalModKeyName"),
                    SelectColumn("NativeTerminal_ModKey_Type", "NativeTerminalModKeyType"),
                    SelectColumn("NativeTerminal_ModKey_FileName", "NativeTerminalModKeyFileName"),
                    SelectColumn("NativeTerminal_FormKey_ID", "NativeTerminalFormKeyId"),
                    SelectColumn("SoundLevel"),
                    SelectColumn("FacingAxisOverride"),
                    SelectColumn("AnimationGraph"),
                    SelectColumn("AnimationSkeleton"),
                    SelectColumn("AnimationDirectory"),
                    SelectColumn("AnimationFile")
                ])
            .Select(record => ToDTO(record, game))
            .ToList();
        var models = ModelRepository.GetByFormKey(game, RecordTypeCatalog.Door.RecordID, formKey);
        var keywords = KeywordMappingRepository.GetByFormKey(game, RecordTypeCatalog.Door.RecordID, formKey);
        var sounds = SoundMappingRepository.GetByFormKey(game, RecordTypeCatalog.Door.RecordID, formKey);
        var scriptingAdapters = ScriptingAdapterRepository.GetByFormKey(game, RecordTypeCatalog.Door.RecordID, formKey);
        var components = RecordComponentRepository.GetByFormKey(game, RecordTypeCatalog.Door.RecordID, formKey);
        var rawPayloads = RawRecordPayloadRepository.GetByFormKey(game, RecordTypeCatalog.Door.RecordID, formKey);
        foreach (var record in records)
        {
            record.Models = models.Where(model => IsSameModKey(model.ModKey, record.ModKey)).OrderBy(model => model.ModelSlot).ThenBy(model => model.ModelGender).ToList();
            record.Keywords = keywords.Where(keyword => IsSameModKey(keyword.ModKey, record.ModKey)).OrderBy(keyword => keyword.KeywordIndex).ToList();
            record.Sounds = sounds.Where(sound => IsSameModKey(sound.ModKey, record.ModKey)).OrderBy(sound => sound.SoundIndex).ToList();
            record.ScriptingAdapters = scriptingAdapters.Where(adapter => IsSameModKey(adapter.ModKey, record.ModKey)).OrderBy(adapter => adapter.ScriptIndex).ToList();
            record.Components = components.Where(component => IsSameModKey(component.ModKey, record.ModKey)).OrderBy(component => component.ComponentIndex).ToList();
            record.RawPayloads = rawPayloads.Where(payload => IsSameModKey(payload.ModKey, record.ModKey)).OrderBy(payload => payload.PayloadSlot).ThenBy(payload => payload.PayloadIndex).ToList();
        }

        return records;
    }

    public void Save(DoorDTO dto)
    {
        SaveRecordInstance(dto);
        Database.Execute(
            """
            INSERT OR REPLACE INTO Doors (
                Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                EditorID, FormVersion, MajorRecordFlags, ImportedAtUTC, Version2, VersionControl, ObjectBounds_First, ObjectBounds_Second, Name, Flags,
                NativeTerminal_ModKey_Name, NativeTerminal_ModKey_Type, NativeTerminal_ModKey_FileName, NativeTerminal_FormKey_ID,
                SoundLevel, FacingAxisOverride, AnimationGraph, AnimationSkeleton, AnimationDirectory, AnimationFile)
            VALUES (
                @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                @EditorId, @FormVersion, @MajorRecordFlags, @ImportedAtUTC, @Version2, @VersionControl, @ObjectBoundsFirst, @ObjectBoundsSecond, @Name, @Flags,
                @NativeTerminalModKeyName, @NativeTerminalModKeyType, @NativeTerminalModKeyFileName, @NativeTerminalFormKeyId,
                @SoundLevel, @FacingAxisOverride, @AnimationGraph, @AnimationSkeleton, @AnimationDirectory, @AnimationFile);
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
                dto.ObjectBoundsFirst,
                dto.ObjectBoundsSecond,
                Name = GetEnglishText(dto.Name),
                dto.Flags,
                NativeTerminalModKeyName = dto.NativeTerminalFormKey?.ModKey.Name,
                NativeTerminalModKeyType = dto.NativeTerminalFormKey?.ModKey.Type,
                NativeTerminalModKeyFileName = dto.NativeTerminalFormKey?.ModKey.FileName,
                NativeTerminalFormKeyId = dto.NativeTerminalFormKey?.Id,
                dto.SoundLevel,
                dto.FacingAxisOverride,
                dto.AnimationGraph,
                dto.AnimationSkeleton,
                dto.AnimationDirectory,
                dto.AnimationFile
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
            VersionControl = record.VersionControl,
            ObjectBoundsFirst = record.ObjectBoundsFirst,
            ObjectBoundsSecond = record.ObjectBoundsSecond,
            Name = FromEnglish(record.Name),
            Flags = record.Flags,
            NativeTerminalFormKey = CreateNullableFormKey(record.NativeTerminalModKeyName, record.NativeTerminalModKeyType, record.NativeTerminalModKeyFileName, record.NativeTerminalFormKeyId),
            SoundLevel = record.SoundLevel,
            FacingAxisOverride = record.FacingAxisOverride,
            AnimationGraph = record.AnimationGraph,
            AnimationSkeleton = record.AnimationSkeleton,
            AnimationDirectory = record.AnimationDirectory,
            AnimationFile = record.AnimationFile
        };
        ApplyCommonFields(dto, record, game);
        return dto;
    }

    private static bool IsSameModKey(ModKeyDTO first, ModKeyDTO second)
    {
        return string.Equals(first.Name, second.Name, StringComparison.Ordinal) &&
               first.Type == second.Type &&
               string.Equals(first.FileName, second.FileName, StringComparison.Ordinal);
    }

    private sealed class DoorRow : RecordRow
    {
        public int? Version2 { get; set; }

        public int? VersionControl { get; set; }

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

        public string? AnimationGraph { get; set; }

        public string? AnimationSkeleton { get; set; }

        public string? AnimationDirectory { get; set; }

        public string? AnimationFile { get; set; }
    }
}
