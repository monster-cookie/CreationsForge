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
    private readonly IRecordLocalizedStringRepository RecordLocalizedStringRepository;
    private readonly IModelRepository ModelRepository;
    private readonly IKeywordMappingRepository KeywordMappingRepository;
    private readonly IScriptingAdapterRepository ScriptingAdapterRepository;
    private readonly IRawRecordPayloadRepository RawRecordPayloadRepository;

    public TerminalRepository(
        IDatabase database,
        IRecordInstanceRepository recordInstanceRepository,
        ITerminalMarkerParameterRepository terminalMarkerParameterRepository,
        IRecordLocalizedStringRepository recordLocalizedStringRepository,
        IModelRepository modelRepository,
        IKeywordMappingRepository keywordMappingRepository,
        IScriptingAdapterRepository scriptingAdapterRepository,
        IRawRecordPayloadRepository rawRecordPayloadRepository)
        : base(database, recordInstanceRepository)
    {
        TerminalMarkerParameterRepository = terminalMarkerParameterRepository;
        RecordLocalizedStringRepository = recordLocalizedStringRepository;
        ModelRepository = modelRepository;
        KeywordMappingRepository = keywordMappingRepository;
        ScriptingAdapterRepository = scriptingAdapterRepository;
        RawRecordPayloadRepository = rawRecordPayloadRepository;
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
                    SelectColumn("VersionControl"),
                    SelectColumn("ObjectBounds_First", "ObjectBoundsFirst"),
                    SelectColumn("ObjectBounds_Second", "ObjectBoundsSecond"),
                    SelectColumn("Menu_ModKey_Name", "MenuModKeyName"),
                    SelectColumn("Menu_ModKey_Type", "MenuModKeyType"),
                    SelectColumn("Menu_ModKey_FileName", "MenuModKeyFileName"),
                    SelectColumn("Menu_FormKey_ID", "MenuFormKeyId"),
                    SelectColumn("Background"),
                    SelectColumn("HeaderText"),
                    SelectColumn("WelcomeText"),
                    SelectColumn("Name"),
                    SelectColumn("PNAM", "Pnam"),
                    SelectColumn("FNAM", "Fnam"),
                    SelectColumn("Flags"),
                    SelectColumn("MajorFlags"),
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
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.Terminal.RecordID, formKey);
        var models = ModelRepository.GetByFormKey(game, RecordTypeCatalog.Terminal.RecordID, formKey);
        var keywords = KeywordMappingRepository.GetByFormKey(game, RecordTypeCatalog.Terminal.RecordID, formKey);
        var scriptingAdapters = ScriptingAdapterRepository.GetByFormKey(game, RecordTypeCatalog.Terminal.RecordID, formKey);
        var rawPayloads = RawRecordPayloadRepository.GetByFormKey(game, RecordTypeCatalog.Terminal.RecordID, formKey);
        var markerParameters = TerminalMarkerParameterRepository.GetByFormKey(game, formKey);
        var forcedLocations = GetForcedLocationsByFormKey(game, formKey);
        var bodyTexts = GetBodyTextsByFormKey(game, formKey);
        var menuItems = GetMenuItemsByFormKey(game, formKey);
        foreach (var record in records)
        {
            var recordLocalizedStrings = localizedStrings.Where(localizedString => RecordModKeysMatch(localizedString.ModKey, record.ModKey)).ToList();
            ApplyLocalizedStrings(record, recordLocalizedStrings);
            record.Models = models.Where(model => RecordModKeysMatch(model.ModKey, record.ModKey)).OrderBy(model => model.ModelSlot).ThenBy(model => model.ModelGender).ToList();
            record.Keywords = keywords.Where(keyword => RecordModKeysMatch(keyword.ModKey, record.ModKey)).OrderBy(keyword => keyword.KeywordIndex).ToList();
            record.ScriptingAdapters = scriptingAdapters.Where(adapter => RecordModKeysMatch(adapter.ModKey, record.ModKey)).OrderBy(adapter => adapter.ScriptIndex).ToList();
            record.RawPayloads = rawPayloads.Where(payload => RecordModKeysMatch(payload.ModKey, record.ModKey)).OrderBy(payload => payload.PayloadSlot).ThenBy(payload => payload.PayloadIndex).ToList();
            record.MarkerParameters = markerParameters
                .Where(parameter => RecordModKeysMatch(parameter.ModKey, record.ModKey))
                .OrderBy(parameter => parameter.ParameterIndex)
                .ToList();
            record.ForcedLocations = forcedLocations
                .Where(location => RecordModKeysMatch(location.ModKey, record.ModKey))
                .OrderBy(location => location.ForcedLocationIndex)
                .Select(location => location.ForcedLocation)
                .ToList();
            record.BodyTexts = bodyTexts
                .Where(bodyText => RecordModKeysMatch(bodyText.ModKey, record.ModKey))
                .OrderBy(bodyText => bodyText.BodyTextIndex)
                .ToList();
            record.MenuItems = menuItems
                .Where(menuItem => RecordModKeysMatch(menuItem.ModKey, record.ModKey))
                .OrderBy(menuItem => menuItem.MenuItemIndex)
                .ToList();

            foreach (var bodyText in record.BodyTexts)
            {
                bodyText.Text = BuildTranslatedString(recordLocalizedStrings, "BodyTexts[" + bodyText.BodyTextIndex + "].Text", bodyText.Text);
            }

            foreach (var menuItem in record.MenuItems)
            {
                menuItem.ItemText = BuildTranslatedString(recordLocalizedStrings, "MenuItems[" + menuItem.MenuItemIndex + "].ItemText", menuItem.ItemText);
                menuItem.DisplayText = BuildTranslatedString(recordLocalizedStrings, "MenuItems[" + menuItem.MenuItemIndex + "].DisplayText", menuItem.DisplayText);
            }
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
                EditorID, FormVersion, MajorRecordFlags, ImportedAtUTC, Version2, VersionControl, ObjectBounds_First, ObjectBounds_Second,
                Menu_ModKey_Name, Menu_ModKey_Type, Menu_ModKey_FileName, Menu_FormKey_ID, Background, HeaderText, WelcomeText, Name, PNAM, FNAM, Flags, MajorFlags, JNAM, MarkerFlags, GNAM,
                WorkbenchData, FurnitureTemplate_ModKey_Name, FurnitureTemplate_ModKey_Type, FurnitureTemplate_ModKey_FileName, FurnitureTemplate_FormKey_ID, MarkerModel)
            VALUES (
                @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                @EditorId, @FormVersion, @MajorRecordFlags, @ImportedAtUTC, @Version2, @VersionControl, @ObjectBoundsFirst, @ObjectBoundsSecond,
                @MenuModKeyName, @MenuModKeyType, @MenuModKeyFileName, @MenuFormKeyId, @Background, @HeaderText, @WelcomeText, @Name, @Pnam, @Fnam, @Flags, @MajorFlags, @Jnam, @MarkerFlags, @Gnam,
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
                dto.VersionControl,
                dto.ObjectBoundsFirst,
                dto.ObjectBoundsSecond,
                MenuModKeyName = dto.MenuFormKey?.ModKey.Name,
                MenuModKeyType = dto.MenuFormKey?.ModKey.Type,
                MenuModKeyFileName = dto.MenuFormKey?.ModKey.FileName,
                MenuFormKeyId = dto.MenuFormKey?.Id,
                dto.Background,
                HeaderText = GetEnglishText(dto.HeaderText),
                WelcomeText = GetEnglishText(dto.WelcomeText),
                Name = GetEnglishText(dto.Name),
                dto.Pnam,
                dto.Fnam,
                dto.Flags,
                dto.MajorFlags,
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
        ReplaceTerminalForcedLocations(dto);
        ReplaceTerminalBodyTexts(dto);
        ReplaceTerminalMenuItems(dto);
    }

    public new void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
    {
        DeleteStaleForcedLocationsByPlugin(game, modKey, importedAtUTC);
        DeleteStaleBodyTextsByPlugin(game, modKey, importedAtUTC);
        DeleteStaleMenuItemsByPlugin(game, modKey, importedAtUTC);
        base.DeleteStaleByPlugin(game, modKey, importedAtUTC);
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
            VersionControl = record.VersionControl,
            ObjectBoundsFirst = record.ObjectBoundsFirst,
            ObjectBoundsSecond = record.ObjectBoundsSecond,
            MenuFormKey = CreateNullableFormKey(record.MenuModKeyName, record.MenuModKeyType, record.MenuModKeyFileName, record.MenuFormKeyId),
            Background = record.Background,
            HeaderText = FromEnglish(record.HeaderText),
            WelcomeText = FromEnglish(record.WelcomeText),
            Name = FromEnglish(record.Name),
            Pnam = record.Pnam,
            Fnam = record.Fnam,
            Flags = record.Flags,
            MajorFlags = record.MajorFlags,
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

    private IReadOnlyList<TerminalForcedLocationRow> GetForcedLocationsByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<TerminalForcedLocationRow>(
                """
                SELECT
                    ModKey_Name AS ModKeyName,
                    ModKey_Type AS ModKeyType,
                    ModKey_FileName AS ModKeyFileName,
                    ForcedLocation_ModKey_Name AS ForcedLocationModKeyName,
                    ForcedLocation_ModKey_Type AS ForcedLocationModKeyType,
                    ForcedLocation_ModKey_FileName AS ForcedLocationModKeyFileName,
                    ForcedLocation_FormKey_ID AS ForcedLocationFormKeyId,
                    ForcedLocation_Index AS ForcedLocationIndex
                FROM TerminalForcedLocations
                WHERE Game = @Game
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY ModKey_FileName COLLATE NOCASE, ForcedLocation_Index;
                """,
                new
                {
                    Game = game.ToString(),
                    FormKeyModKeyName = formKey.ModKey.Name,
                    FormKeyModKeyType = formKey.ModKey.Type,
                    FormKeyModKeyFileName = formKey.ModKey.FileName,
                    FormKeyId = formKey.Id
                })
            .Select(row =>
            {
                row.ModKey = new ModKeyDTO { Name = row.ModKeyName, Type = row.ModKeyType, FileName = row.ModKeyFileName };
                row.ForcedLocation = CreateFormKey(row.ForcedLocationModKeyName, row.ForcedLocationModKeyType, row.ForcedLocationModKeyFileName, row.ForcedLocationFormKeyId);
                return row;
            })
            .ToList();
    }

    private IReadOnlyList<TerminalBodyTextDTO> GetBodyTextsByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<TerminalBodyTextRow>(
                """
                SELECT
                    ModKey_Name AS ModKeyName,
                    ModKey_Type AS ModKeyType,
                    ModKey_FileName AS ModKeyFileName,
                    FormKey_ModKey_Name AS FormKeyModKeyName,
                    FormKey_ModKey_Type AS FormKeyModKeyType,
                    FormKey_ModKey_FileName AS FormKeyModKeyFileName,
                    FormKey_ID AS FormKeyId,
                    BodyText_Index AS BodyTextIndex,
                    Text,
                    ImportedAtUTC
                FROM TerminalBodyTexts
                WHERE Game = @Game
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY ModKey_FileName COLLATE NOCASE, BodyText_Index;
                """,
                new
                {
                    Game = game.ToString(),
                    FormKeyModKeyName = formKey.ModKey.Name,
                    FormKeyModKeyType = formKey.ModKey.Type,
                    FormKeyModKeyFileName = formKey.ModKey.FileName,
                    FormKeyId = formKey.Id
                })
            .Select(row => new TerminalBodyTextDTO
            {
                Game = game,
                ModKey = new ModKeyDTO { Name = row.ModKeyName, Type = row.ModKeyType, FileName = row.ModKeyFileName },
                FormKey = CreateFormKey(row.FormKeyModKeyName, row.FormKeyModKeyType, row.FormKeyModKeyFileName, row.FormKeyId),
                BodyTextIndex = row.BodyTextIndex,
                Text = FromEnglish(row.Text),
                ImportedAtUTC = row.ImportedAtUTC
            })
            .ToList();
    }

    private IReadOnlyList<TerminalMenuItemDTO> GetMenuItemsByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<TerminalMenuItemRow>(
                """
                SELECT
                    ModKey_Name AS ModKeyName,
                    ModKey_Type AS ModKeyType,
                    ModKey_FileName AS ModKeyFileName,
                    FormKey_ModKey_Name AS FormKeyModKeyName,
                    FormKey_ModKey_Type AS FormKeyModKeyType,
                    FormKey_ModKey_FileName AS FormKeyModKeyFileName,
                    FormKey_ID AS FormKeyId,
                    MenuItem_Index AS MenuItemIndex,
                    ItemText,
                    Type,
                    ItemId,
                    Submenu_ModKey_Name AS SubmenuModKeyName,
                    Submenu_ModKey_Type AS SubmenuModKeyType,
                    Submenu_ModKey_FileName AS SubmenuModKeyFileName,
                    Submenu_FormKey_ID AS SubmenuFormKeyId,
                    DisplayText,
                    ImportedAtUTC
                FROM TerminalMenuItems
                WHERE Game = @Game
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY ModKey_FileName COLLATE NOCASE, MenuItem_Index;
                """,
                new
                {
                    Game = game.ToString(),
                    FormKeyModKeyName = formKey.ModKey.Name,
                    FormKeyModKeyType = formKey.ModKey.Type,
                    FormKeyModKeyFileName = formKey.ModKey.FileName,
                    FormKeyId = formKey.Id
                })
            .Select(row => new TerminalMenuItemDTO
            {
                Game = game,
                ModKey = new ModKeyDTO { Name = row.ModKeyName, Type = row.ModKeyType, FileName = row.ModKeyFileName },
                FormKey = CreateFormKey(row.FormKeyModKeyName, row.FormKeyModKeyType, row.FormKeyModKeyFileName, row.FormKeyId),
                MenuItemIndex = row.MenuItemIndex,
                ItemText = FromEnglish(row.ItemText),
                Type = row.Type,
                ItemId = row.ItemId,
                Submenu = CreateNullableFormKey(row.SubmenuModKeyName, row.SubmenuModKeyType, row.SubmenuModKeyFileName, row.SubmenuFormKeyId),
                DisplayText = FromEnglish(row.DisplayText),
                ImportedAtUTC = row.ImportedAtUTC
            })
            .ToList();
    }

    private void ReplaceTerminalForcedLocations(TerminalDTO dto)
    {
        Database.Execute(
            """
            DELETE FROM TerminalForcedLocations
            WHERE Game = @Game
              AND ModKey_Name = @ModKeyName
              AND ModKey_Type = @ModKeyType
              AND ModKey_FileName = @ModKeyFileName
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId;
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
                FormKeyId = dto.FormKey.Id
            });

        for (var forcedLocationIndex = 0; forcedLocationIndex < dto.ForcedLocations.Count; forcedLocationIndex++)
        {
            var forcedLocation = dto.ForcedLocations[forcedLocationIndex];
            Database.Execute(
                """
                INSERT OR REPLACE INTO TerminalForcedLocations (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    ForcedLocation_ModKey_Name, ForcedLocation_ModKey_Type, ForcedLocation_ModKey_FileName, ForcedLocation_FormKey_ID, ForcedLocation_Index, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @ForcedLocationModKeyName, @ForcedLocationModKeyType, @ForcedLocationModKeyFileName, @ForcedLocationFormKeyId, @ForcedLocationIndex, @ImportedAtUTC);
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
                    ForcedLocationModKeyName = forcedLocation.ModKey.Name,
                    ForcedLocationModKeyType = forcedLocation.ModKey.Type,
                    ForcedLocationModKeyFileName = forcedLocation.ModKey.FileName,
                    ForcedLocationFormKeyId = forcedLocation.Id,
                    ForcedLocationIndex = forcedLocationIndex,
                    dto.ImportedAtUTC
                });
        }
    }

    private void ReplaceTerminalBodyTexts(TerminalDTO dto)
    {
        Database.Execute(
            """
            DELETE FROM TerminalBodyTexts
            WHERE Game = @Game
              AND ModKey_Name = @ModKeyName
              AND ModKey_Type = @ModKeyType
              AND ModKey_FileName = @ModKeyFileName
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId;
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
                FormKeyId = dto.FormKey.Id
            });

        foreach (var bodyText in dto.BodyTexts)
        {
            bodyText.ImportedAtUTC = dto.ImportedAtUTC;
            Database.Execute(
                """
                INSERT OR REPLACE INTO TerminalBodyTexts (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    BodyText_Index, Text, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @BodyTextIndex, @Text, @ImportedAtUTC);
                """,
                new
                {
                    Game = bodyText.Game.ToString(),
                    ModKeyName = bodyText.ModKey.Name,
                    ModKeyType = bodyText.ModKey.Type,
                    ModKeyFileName = bodyText.ModKey.FileName,
                    FormKeyModKeyName = bodyText.FormKey.ModKey.Name,
                    FormKeyModKeyType = bodyText.FormKey.ModKey.Type,
                    FormKeyModKeyFileName = bodyText.FormKey.ModKey.FileName,
                    FormKeyId = bodyText.FormKey.Id,
                    bodyText.BodyTextIndex,
                    Text = GetEnglishText(bodyText.Text),
                    bodyText.ImportedAtUTC
                });
        }
    }

    private void ReplaceTerminalMenuItems(TerminalDTO dto)
    {
        Database.Execute(
            """
            DELETE FROM TerminalMenuItems
            WHERE Game = @Game
              AND ModKey_Name = @ModKeyName
              AND ModKey_Type = @ModKeyType
              AND ModKey_FileName = @ModKeyFileName
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId;
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
                FormKeyId = dto.FormKey.Id
            });

        foreach (var menuItem in dto.MenuItems)
        {
            menuItem.ImportedAtUTC = dto.ImportedAtUTC;
            Database.Execute(
                """
                INSERT OR REPLACE INTO TerminalMenuItems (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    MenuItem_Index, ItemText, Type, ItemId, Submenu_ModKey_Name, Submenu_ModKey_Type, Submenu_ModKey_FileName, Submenu_FormKey_ID,
                    DisplayText, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @MenuItemIndex, @ItemText, @Type, @ItemId, @SubmenuModKeyName, @SubmenuModKeyType, @SubmenuModKeyFileName, @SubmenuFormKeyId,
                    @DisplayText, @ImportedAtUTC);
                """,
                new
                {
                    Game = menuItem.Game.ToString(),
                    ModKeyName = menuItem.ModKey.Name,
                    ModKeyType = menuItem.ModKey.Type,
                    ModKeyFileName = menuItem.ModKey.FileName,
                    FormKeyModKeyName = menuItem.FormKey.ModKey.Name,
                    FormKeyModKeyType = menuItem.FormKey.ModKey.Type,
                    FormKeyModKeyFileName = menuItem.FormKey.ModKey.FileName,
                    FormKeyId = menuItem.FormKey.Id,
                    menuItem.MenuItemIndex,
                    ItemText = GetEnglishText(menuItem.ItemText),
                    menuItem.Type,
                    menuItem.ItemId,
                    SubmenuModKeyName = menuItem.Submenu?.ModKey.Name,
                    SubmenuModKeyType = menuItem.Submenu?.ModKey.Type,
                    SubmenuModKeyFileName = menuItem.Submenu?.ModKey.FileName,
                    SubmenuFormKeyId = menuItem.Submenu?.Id,
                    DisplayText = GetEnglishText(menuItem.DisplayText),
                    menuItem.ImportedAtUTC
                });
        }
    }

    private void DeleteStaleForcedLocationsByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
    {
        Database.Execute(
            """
            DELETE FROM TerminalForcedLocations
            WHERE Game = @Game
              AND ModKey_Name = @ModKeyName
              AND ModKey_Type = @ModKeyType
              AND ModKey_FileName = @ModKeyFileName
              AND ImportedAtUTC <> @ImportedAtUTC;
            """,
            new { Game = game.ToString(), ModKeyName = modKey.Name, ModKeyType = modKey.Type, ModKeyFileName = modKey.FileName, ImportedAtUTC = importedAtUTC });
    }

    private void DeleteStaleBodyTextsByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
    {
        Database.Execute(
            """
            DELETE FROM TerminalBodyTexts
            WHERE Game = @Game
              AND ModKey_Name = @ModKeyName
              AND ModKey_Type = @ModKeyType
              AND ModKey_FileName = @ModKeyFileName
              AND ImportedAtUTC <> @ImportedAtUTC;
            """,
            new { Game = game.ToString(), ModKeyName = modKey.Name, ModKeyType = modKey.Type, ModKeyFileName = modKey.FileName, ImportedAtUTC = importedAtUTC });
    }

    private void DeleteStaleMenuItemsByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
    {
        Database.Execute(
            """
            DELETE FROM TerminalMenuItems
            WHERE Game = @Game
              AND ModKey_Name = @ModKeyName
              AND ModKey_Type = @ModKeyType
              AND ModKey_FileName = @ModKeyFileName
              AND ImportedAtUTC <> @ImportedAtUTC;
            """,
            new { Game = game.ToString(), ModKeyName = modKey.Name, ModKeyType = modKey.Type, ModKeyFileName = modKey.FileName, ImportedAtUTC = importedAtUTC });
    }

    private static void ApplyLocalizedStrings(TerminalDTO record, IReadOnlyList<LocalizedStringDTO> localizedStrings)
    {
        record.LocalizedStrings = localizedStrings.ToList();
        record.HeaderText = BuildTranslatedString(localizedStrings, nameof(TerminalDTO.HeaderText), record.HeaderText);
        record.WelcomeText = BuildTranslatedString(localizedStrings, nameof(TerminalDTO.WelcomeText), record.WelcomeText);
        record.Name = BuildTranslatedString(localizedStrings, nameof(TerminalDTO.Name), record.Name);
    }

    private static FormKeyDTO CreateFormKey(string modKeyName, int modKeyType, string modKeyFileName, long formKeyId)
    {
        return new FormKeyDTO
        {
            ModKey = new ModKeyDTO
            {
                Name = modKeyName,
                Type = modKeyType,
                FileName = modKeyFileName
            },
            Id = (uint)formKeyId
        };
    }

    private sealed class TerminalRow : RecordRow
    {
        public int? Version2 { get; set; }

        public int? VersionControl { get; set; }

        public string? ObjectBoundsFirst { get; set; }

        public string? ObjectBoundsSecond { get; set; }

        public string? MenuModKeyName { get; set; }

        public int? MenuModKeyType { get; set; }

        public string? MenuModKeyFileName { get; set; }

        public long? MenuFormKeyId { get; set; }

        public string? Background { get; set; }

        public string? HeaderText { get; set; }

        public string? WelcomeText { get; set; }

        public string? Name { get; set; }

        public string? Pnam { get; set; }

        public string? Fnam { get; set; }

        public string? Flags { get; set; }

        public string? MajorFlags { get; set; }

        public string? Jnam { get; set; }

        public string? MarkerFlags { get; set; }

        public string? Gnam { get; set; }

        public string? WorkbenchData { get; set; }

        public string? FurnitureTemplateModKeyName { get; set; }

        public int? FurnitureTemplateModKeyType { get; set; }

        public string? FurnitureTemplateModKeyFileName { get; set; }

        public long? FurnitureTemplateFormKeyId { get; set; }

        public string? MarkerModel { get; set; }
    }

    private sealed class TerminalForcedLocationRow
    {
        public string ModKeyName { get; set; } = string.Empty;

        public int ModKeyType { get; set; }

        public string ModKeyFileName { get; set; } = string.Empty;

        public ModKeyDTO ModKey { get; set; } = new() { Name = string.Empty, Type = 0, FileName = string.Empty };

        public string ForcedLocationModKeyName { get; set; } = string.Empty;

        public int ForcedLocationModKeyType { get; set; }

        public string ForcedLocationModKeyFileName { get; set; } = string.Empty;

        public long ForcedLocationFormKeyId { get; set; }

        public int ForcedLocationIndex { get; set; }

        public FormKeyDTO ForcedLocation { get; set; } = new() { ModKey = new ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty }, Id = 0 };
    }

    private sealed class TerminalBodyTextRow
    {
        public string ModKeyName { get; set; } = string.Empty;

        public int ModKeyType { get; set; }

        public string ModKeyFileName { get; set; } = string.Empty;

        public string FormKeyModKeyName { get; set; } = string.Empty;

        public int FormKeyModKeyType { get; set; }

        public string FormKeyModKeyFileName { get; set; } = string.Empty;

        public long FormKeyId { get; set; }

        public int BodyTextIndex { get; set; }

        public string? Text { get; set; }

        public DateTime ImportedAtUTC { get; set; }
    }

    private sealed class TerminalMenuItemRow
    {
        public string ModKeyName { get; set; } = string.Empty;

        public int ModKeyType { get; set; }

        public string ModKeyFileName { get; set; } = string.Empty;

        public string FormKeyModKeyName { get; set; } = string.Empty;

        public int FormKeyModKeyType { get; set; }

        public string FormKeyModKeyFileName { get; set; } = string.Empty;

        public long FormKeyId { get; set; }

        public int MenuItemIndex { get; set; }

        public string? ItemText { get; set; }

        public string? Type { get; set; }

        public int? ItemId { get; set; }

        public string? SubmenuModKeyName { get; set; }

        public int? SubmenuModKeyType { get; set; }

        public string? SubmenuModKeyFileName { get; set; }

        public long? SubmenuFormKeyId { get; set; }

        public string? DisplayText { get; set; }

        public DateTime ImportedAtUTC { get; set; }
    }
}
