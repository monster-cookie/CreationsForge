using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

public class TerminalRepository : TypedRecordRepositoryBase, ITerminalRepository
{
    private readonly ITerminalMarkerParameterRepository TerminalMarkerParameterRepository;

    public TerminalRepository(IDatabase database, IRecordInstanceRepository recordInstanceRepository, ITerminalMarkerParameterRepository terminalMarkerParameterRepository)
        : base(database, recordInstanceRepository)
    {
        TerminalMarkerParameterRepository = terminalMarkerParameterRepository;
    }

    public override string RecordType => RecordTypeCatalog.Terminal.RecordID;

    protected override string TableName => RecordTypeCatalog.Terminal.TableName;

    public IReadOnlyList<TerminalDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        var records = FetchByFormKey<TerminalRow>(
                game,
                formKey,
                [
                    SelectColumn("Version2"),
                    SelectColumn("ObjectBounds_First", "ObjectBoundsFirst"),
                    SelectColumn("ObjectBounds_Second", "ObjectBoundsSecond"),
                    SelectColumn("Menu_ModKey_Name", "MenuModKeyName"),
                    SelectColumn("Menu_ModKey_Type", "MenuModKeyType"),
                    SelectColumn("Menu_ModKey_FileName", "MenuModKeyFileName"),
                    SelectColumn("Menu_FormKey_ID", "MenuFormKeyId"),
                    SelectColumn("Background"),
                    SelectColumn("Name"),
                    SelectColumn("PNAM", "Pnam"),
                    SelectColumn("FNAM", "Fnam"),
                    SelectColumn("JNAM", "Jnam"),
                    SelectColumn("MarkerFlags"),
                    SelectColumn("GNAM", "Gnam"),
                    SelectColumn("WorkbenchData"),
                    SelectColumn("FurnitureTemplate_ModKey_Name", "FurnitureTemplateModKeyName"),
                    SelectColumn("FurnitureTemplate_ModKey_Type", "FurnitureTemplateModKeyType"),
                    SelectColumn("FurnitureTemplate_ModKey_FileName", "FurnitureTemplateModKeyFileName"),
                    SelectColumn("FurnitureTemplate_FormKey_ID", "FurnitureTemplateFormKeyId"),
                    SelectColumn("MarkerModel")
                ])
            .Select(record => ToDTO(record, game))
            .ToList();
        var markerParameters = TerminalMarkerParameterRepository.GetByFormKey(game, formKey);
        foreach (var record in records)
        {
            record.MarkerParameters = markerParameters
                .Where(parameter => IsSameModKey(parameter.ModKey, record.ModKey))
                .OrderBy(parameter => parameter.ParameterIndex)
                .ToList();
        }

        return records;
    }

    public void Save(TerminalDTO dto)
    {
        SaveRecordInstance(dto);
        Database.Execute(
            """
            INSERT OR REPLACE INTO Terminals (
                Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                EditorID, FormVersion, MajorRecordFlags, ImportedAtUTC, Version2, ObjectBounds_First, ObjectBounds_Second,
                Menu_ModKey_Name, Menu_ModKey_Type, Menu_ModKey_FileName, Menu_FormKey_ID, Background, Name, PNAM, FNAM, JNAM, MarkerFlags, GNAM,
                WorkbenchData, FurnitureTemplate_ModKey_Name, FurnitureTemplate_ModKey_Type, FurnitureTemplate_ModKey_FileName, FurnitureTemplate_FormKey_ID, MarkerModel)
            VALUES (
                @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                @EditorId, @FormVersion, @MajorRecordFlags, @ImportedAtUTC, @Version2, @ObjectBoundsFirst, @ObjectBoundsSecond,
                @MenuModKeyName, @MenuModKeyType, @MenuModKeyFileName, @MenuFormKeyId, @Background, @Name, @Pnam, @Fnam, @Jnam, @MarkerFlags, @Gnam,
                @WorkbenchData, @FurnitureTemplateModKeyName, @FurnitureTemplateModKeyType, @FurnitureTemplateModKeyFileName, @FurnitureTemplateFormKeyId, @MarkerModel);
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
                MenuModKeyName = dto.MenuFormKey?.ModKey.Name,
                MenuModKeyType = dto.MenuFormKey?.ModKey.Type,
                MenuModKeyFileName = dto.MenuFormKey?.ModKey.FileName,
                MenuFormKeyId = dto.MenuFormKey?.Id,
                dto.Background,
                Name = GetEnglishText(dto.Name),
                dto.Pnam,
                dto.Fnam,
                dto.Jnam,
                dto.MarkerFlags,
                dto.Gnam,
                dto.WorkbenchData,
                FurnitureTemplateModKeyName = dto.FurnitureTemplateFormKey?.ModKey.Name,
                FurnitureTemplateModKeyType = dto.FurnitureTemplateFormKey?.ModKey.Type,
                FurnitureTemplateModKeyFileName = dto.FurnitureTemplateFormKey?.ModKey.FileName,
                FurnitureTemplateFormKeyId = dto.FurnitureTemplateFormKey?.Id,
                dto.MarkerModel
            });
    }

    private static TerminalDTO ToDTO(TerminalRow record, SupportedGame game)
    {
        var dto = new TerminalDTO
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
            MenuFormKey = CreateNullableFormKey(record.MenuModKeyName, record.MenuModKeyType, record.MenuModKeyFileName, record.MenuFormKeyId),
            Background = record.Background,
            Name = FromEnglish(record.Name),
            Pnam = record.Pnam,
            Fnam = record.Fnam,
            Jnam = record.Jnam,
            MarkerFlags = record.MarkerFlags,
            Gnam = record.Gnam,
            WorkbenchData = record.WorkbenchData,
            FurnitureTemplateFormKey = CreateNullableFormKey(record.FurnitureTemplateModKeyName, record.FurnitureTemplateModKeyType, record.FurnitureTemplateModKeyFileName, record.FurnitureTemplateFormKeyId),
            MarkerModel = record.MarkerModel
        };
        ApplyCommonFields(dto, record, game);
        return dto;
    }

    private static bool IsSameModKey(ModKeyDTO first, ModKeyDTO second)
    {
        return first.Type == second.Type &&
            string.Equals(first.Name, second.Name, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(first.FileName, second.FileName, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class TerminalRow : RecordRow
    {
        public int? Version2 { get; set; }

        public string? ObjectBoundsFirst { get; set; }

        public string? ObjectBoundsSecond { get; set; }

        public string? MenuModKeyName { get; set; }

        public int? MenuModKeyType { get; set; }

        public string? MenuModKeyFileName { get; set; }

        public long? MenuFormKeyId { get; set; }

        public string? Background { get; set; }

        public string? Name { get; set; }

        public string? Pnam { get; set; }

        public string? Fnam { get; set; }

        public string? Jnam { get; set; }

        public long? MarkerFlags { get; set; }

        public string? Gnam { get; set; }

        public string? WorkbenchData { get; set; }

        public string? FurnitureTemplateModKeyName { get; set; }

        public int? FurnitureTemplateModKeyType { get; set; }

        public string? FurnitureTemplateModKeyFileName { get; set; }

        public long? FurnitureTemplateFormKeyId { get; set; }

        public string? MarkerModel { get; set; }
    }
}
