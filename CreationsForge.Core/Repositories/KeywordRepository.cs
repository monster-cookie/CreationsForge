using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

public class KeywordRepository : TypedRecordRepositoryBase, IKeywordRepository
{
    private readonly IRecordLocalizedStringRepository RecordLocalizedStringRepository;
    private readonly IScriptingAdapterRepository ScriptingAdapterRepository;

    public KeywordRepository(
        IDatabase database,
        IRecordInstanceRepository recordInstanceRepository,
        IRecordLocalizedStringRepository recordLocalizedStringRepository,
        IScriptingAdapterRepository scriptingAdapterRepository)
        : base(database, recordInstanceRepository)
    {
        RecordLocalizedStringRepository = recordLocalizedStringRepository;
        ScriptingAdapterRepository = scriptingAdapterRepository;
    }

    public override string RecordType => RecordTypeCatalog.Keyword.RecordID;

    protected override string TableName => RecordTypeCatalog.Keyword.TableName;

    public IReadOnlyList<KeywordDTO> GetByFormKey(CreationsForge.Core.Enums.SupportedGame game, CreationsForge.Core.DTOs.Plugins.FormKeyDTO formKey)
    {
        var records = FetchByFormKey<KeywordRow>(
                game,
                formKey,
                [
                    SelectColumn("Name"),
                    SelectColumn("Color"),
                    SelectColumn("Type"),
                    SelectColumn("Notes"),
                    SelectColumn("FlashLinkageName"),
                    SelectColumn("Version2"),
                    SelectColumn("VersionControl"),
                    SelectColumn("FNAM"),
                    SelectColumn("WAIM"),
                    SelectColumn("WFIR"),
                    SelectColumn("AttractionRule_ModKey_Name", "AttractionRuleModKeyName"),
                    SelectColumn("AttractionRule_ModKey_Type", "AttractionRuleModKeyType"),
                    SelectColumn("AttractionRule_ModKey_FileName", "AttractionRuleModKeyFileName"),
                    SelectColumn("AttractionRule_FormKey_ID", "AttractionRuleFormKeyId")
                ])
            .Select(record => ToDTO(record, game))
            .ToList();
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.Keyword.RecordID, formKey);
        var scriptingAdapters = ScriptingAdapterRepository.GetByFormKey(game, RecordTypeCatalog.Keyword.RecordID, formKey);
        foreach (var record in records)
        {
            ApplyLocalizedStrings(record, localizedStrings.Where(localizedString => RecordModKeysMatch(localizedString.ModKey, record.ModKey)).ToList());
            record.ScriptingAdapters = scriptingAdapters.Where(adapter => RecordModKeysMatch(adapter.ModKey, record.ModKey)).OrderBy(adapter => adapter.ScriptIndex).ToList();
        }

        return records;
    }

    public void Save(KeywordDTO dto)
    {
        SaveRecordInstance(dto);
        Database.Execute(
            """
            INSERT OR REPLACE INTO Keywords (
                Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                EditorID, FormVersion, MajorRecordFlags, ImportedAtUTC, Name, Color, Type, Notes, FlashLinkageName, Version2, VersionControl, FNAM, WAIM, WFIR,
                AttractionRule_ModKey_Name, AttractionRule_ModKey_Type, AttractionRule_ModKey_FileName, AttractionRule_FormKey_ID)
            VALUES (
                @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                @EditorId, @FormVersion, @MajorRecordFlags, @ImportedAtUTC, @Name, @Color, @Type, @Notes, @FlashLinkageName, @Version2, @VersionControl, @FNAM, @WAIM, @WFIR,
                @AttractionRuleModKeyName, @AttractionRuleModKeyType, @AttractionRuleModKeyFileName, @AttractionRuleFormKeyId);
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
                dto.Color,
                dto.Type,
                dto.Notes,
                dto.FlashLinkageName,
                dto.Version2,
                dto.VersionControl,
                dto.FNAM,
                dto.WAIM,
                dto.WFIR,
                AttractionRuleModKeyName = dto.AttractionRule?.ModKey.Name,
                AttractionRuleModKeyType = dto.AttractionRule?.ModKey.Type,
                AttractionRuleModKeyFileName = dto.AttractionRule?.ModKey.FileName,
                AttractionRuleFormKeyId = dto.AttractionRule?.Id
            });
    }

    private static KeywordDTO ToDTO(KeywordRow record, CreationsForge.Core.Enums.SupportedGame game)
    {
        var dto = new KeywordDTO
        {
            Game = game,
            ModKey = new CreationsForge.Core.DTOs.Plugins.ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty },
            FormKey = new CreationsForge.Core.DTOs.Plugins.FormKeyDTO { ModKey = new CreationsForge.Core.DTOs.Plugins.ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty }, Id = 0 },
            EditorID = string.Empty,
            FormVersion = 0,
            MajorRecordFlags = 0,
            ImportedAtUTC = record.ImportedAtUTC,
            Name = FromEnglish(record.Name),
            Color = record.Color,
            Type = record.Type,
            Notes = record.Notes,
            FlashLinkageName = record.FlashLinkageName,
            Version2 = record.Version2,
            VersionControl = record.VersionControl,
            FNAM = record.FNAM,
            WAIM = record.WAIM,
            WFIR = record.WFIR,
            AttractionRule = CreateNullableFormKey(record.AttractionRuleModKeyName, record.AttractionRuleModKeyType, record.AttractionRuleModKeyFileName, record.AttractionRuleFormKeyId)
        };
        ApplyCommonFields(dto, record, game);
        return dto;
    }

    private static void ApplyLocalizedStrings(KeywordDTO record, IReadOnlyList<LocalizedStringDTO> localizedStrings)
    {
        record.LocalizedStrings = localizedStrings.ToList();
        record.Name = BuildTranslatedString(localizedStrings, nameof(KeywordDTO.Name), record.Name);
    }

    private sealed class KeywordRow : RecordRow
    {
        public string? Name { get; set; }

        public string? Color { get; set; }

        public string? Type { get; set; }

        public string? Notes { get; set; }

        public string? FlashLinkageName { get; set; }

        public int? Version2 { get; set; }

        public int? VersionControl { get; set; }

        public string? FNAM { get; set; }

        public string? WAIM { get; set; }

        public string? WFIR { get; set; }

        public string? AttractionRuleModKeyName { get; set; }

        public int? AttractionRuleModKeyType { get; set; }

        public string? AttractionRuleModKeyFileName { get; set; }

        public long? AttractionRuleFormKeyId { get; set; }
    }
}
