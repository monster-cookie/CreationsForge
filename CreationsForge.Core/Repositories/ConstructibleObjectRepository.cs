using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

public class ConstructibleObjectRepository : TypedRecordRepositoryBase, IConstructibleObjectRepository
{
    private readonly IConditionRuleRepository ConditionRuleRepository;

    public ConstructibleObjectRepository(
        IDatabase database,
        IRecordInstanceRepository recordInstanceRepository,
        IConditionRuleRepository conditionRuleRepository)
        : base(database, recordInstanceRepository)
    {
        ConditionRuleRepository = conditionRuleRepository;
    }

    public override string RecordType => RecordTypeCatalog.ConstructibleObject.RecordID;

    protected override string TableName => RecordTypeCatalog.ConstructibleObject.TableName;

    public IReadOnlyList<ConstructibleObjectDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        var records = FetchByFormKey<ConstructibleObjectRow>(
                game,
                formKey,
                [
                    SelectColumn("Version2"),
                    SelectColumn("Description"),
                    SelectColumn("CreatedObject_ModKey_Name", "CreatedObjectModKeyName"),
                    SelectColumn("CreatedObject_ModKey_Type", "CreatedObjectModKeyType"),
                    SelectColumn("CreatedObject_ModKey_FileName", "CreatedObjectModKeyFileName"),
                    SelectColumn("CreatedObject_FormKey_ID", "CreatedObjectFormKeyId"),
                    SelectColumn("WorkbenchKeyword_ModKey_Name", "WorkbenchKeywordModKeyName"),
                    SelectColumn("WorkbenchKeyword_ModKey_Type", "WorkbenchKeywordModKeyType"),
                    SelectColumn("WorkbenchKeyword_ModKey_FileName", "WorkbenchKeywordModKeyFileName"),
                    SelectColumn("WorkbenchKeyword_FormKey_ID", "WorkbenchKeywordFormKeyId"),
                    SelectColumn("CreatedObjectCount"),
                    SelectColumn("AmountProduced"),
                    SelectColumn("MenuSortOrder"),
                    SelectColumn("LearnMethod"),
                    SelectColumn("Flags")
                ])
            .Select(record => ToDTO(record, game))
            .ToList();
        var components = FetchComponentsByFormKey(game, formKey);
        var categories = FetchCategoriesByFormKey(game, formKey);
        var recipeFilters = FetchRecipeFiltersByFormKey(game, formKey);
        var conditions = ConditionRuleRepository.GetByFormKey(game, RecordTypeCatalog.ConstructibleObject.RecordID, formKey);
        foreach (var record in records)
        {
            record.Components = components
                .Where(component => IsSameModKey(component.ModKey, record.ModKey))
                .OrderBy(component => component.ComponentIndex)
                .ToList();
            record.Categories = categories
                .Where(category => IsSameModKey(category.ModKey, record.ModKey))
                .OrderBy(category => category.CategoryIndex)
                .ToList();
            record.RecipeFilters = recipeFilters
                .Where(recipeFilter => IsSameModKey(recipeFilter.ModKey, record.ModKey))
                .OrderBy(recipeFilter => recipeFilter.RecipeFilterIndex)
                .ToList();
            record.Conditions = conditions
                .Where(condition => IsSameModKey(condition.ModKey, record.ModKey) && string.Equals(condition.ConditionSlot, "Conditions", StringComparison.Ordinal))
                .OrderBy(condition => condition.ConditionIndex)
                .ToList();
        }

        return records;
    }

    public void Save(ConstructibleObjectDTO dto)
    {
        SaveRecordInstance(dto);
        Database.Execute(
            """
            INSERT OR REPLACE INTO ConstructibleObjects (
                Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                EditorID, FormVersion, MajorRecordFlags, ImportedAtUTC, Version2, Description, CreatedObject_ModKey_Name, CreatedObject_ModKey_Type,
                CreatedObject_ModKey_FileName, CreatedObject_FormKey_ID, WorkbenchKeyword_ModKey_Name, WorkbenchKeyword_ModKey_Type,
                WorkbenchKeyword_ModKey_FileName, WorkbenchKeyword_FormKey_ID, CreatedObjectCount, AmountProduced, MenuSortOrder, LearnMethod, Flags)
            VALUES (
                @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                @EditorId, @FormVersion, @MajorRecordFlags, @ImportedAtUTC, @Version2, @Description, @CreatedObjectModKeyName, @CreatedObjectModKeyType,
                @CreatedObjectModKeyFileName, @CreatedObjectFormKeyId, @WorkbenchKeywordModKeyName, @WorkbenchKeywordModKeyType,
                @WorkbenchKeywordModKeyFileName, @WorkbenchKeywordFormKeyId, @CreatedObjectCount, @AmountProduced, @MenuSortOrder, @LearnMethod, @Flags);
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
                Description = GetEnglishText(dto.Description),
                CreatedObjectModKeyName = dto.CreatedObjectFormKey?.ModKey.Name,
                CreatedObjectModKeyType = dto.CreatedObjectFormKey?.ModKey.Type,
                CreatedObjectModKeyFileName = dto.CreatedObjectFormKey?.ModKey.FileName,
                CreatedObjectFormKeyId = dto.CreatedObjectFormKey?.Id,
                WorkbenchKeywordModKeyName = dto.WorkbenchKeywordFormKey?.ModKey.Name,
                WorkbenchKeywordModKeyType = dto.WorkbenchKeywordFormKey?.ModKey.Type,
                WorkbenchKeywordModKeyFileName = dto.WorkbenchKeywordFormKey?.ModKey.FileName,
                WorkbenchKeywordFormKeyId = dto.WorkbenchKeywordFormKey?.Id,
                dto.CreatedObjectCount,
                dto.AmountProduced,
                dto.MenuSortOrder,
                dto.LearnMethod,
                dto.Flags
            });
        ReplaceComponents(dto);
        ReplaceCategories(dto);
        ReplaceRecipeFilters(dto);
    }

    private IReadOnlyList<ConstructibleObjectComponentDTO> FetchComponentsByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<ConstructibleObjectComponentRow>(
                """
                SELECT *
                FROM ConstructibleObjectComponents
                WHERE Game = @Game
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY ModKey_FileName COLLATE NOCASE, Component_Index;
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

    private IReadOnlyList<ConstructibleObjectCategoryDTO> FetchCategoriesByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<ConstructibleObjectCategoryRow>(
                """
                SELECT *
                FROM ConstructibleObjectCategories
                WHERE Game = @Game
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY ModKey_FileName COLLATE NOCASE, Category_Index;
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

    private IReadOnlyList<ConstructibleObjectRecipeFilterDTO> FetchRecipeFiltersByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<ConstructibleObjectRecipeFilterRow>(
                """
                SELECT *
                FROM ConstructibleObjectRecipeFilters
                WHERE Game = @Game
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY ModKey_FileName COLLATE NOCASE, RecipeFilter_Index;
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

    private void ReplaceComponents(ConstructibleObjectDTO dto)
    {
        Database.Execute(
            """
            DELETE FROM ConstructibleObjectComponents
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

        foreach (var component in dto.Components)
        {
            component.ImportedAtUTC = dto.ImportedAtUTC;
            Database.Execute(
                """
                INSERT OR REPLACE INTO ConstructibleObjectComponents (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    Component_Index, Component_ModKey_Name, Component_ModKey_Type, Component_ModKey_FileName, Component_FormKey_ID, Count, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @ComponentIndex, @ComponentModKeyName, @ComponentModKeyType, @ComponentModKeyFileName, @ComponentFormKeyId, @Count, @ImportedAtUTC);
                """,
                new
                {
                    Game = component.Game.ToString(),
                    ModKeyName = component.ModKey.Name,
                    ModKeyType = component.ModKey.Type,
                    ModKeyFileName = component.ModKey.FileName,
                    FormKeyModKeyName = component.FormKey.ModKey.Name,
                    FormKeyModKeyType = component.FormKey.ModKey.Type,
                    FormKeyModKeyFileName = component.FormKey.ModKey.FileName,
                    FormKeyId = component.FormKey.Id,
                    component.ComponentIndex,
                    ComponentModKeyName = component.ComponentFormKey.ModKey.Name,
                    ComponentModKeyType = component.ComponentFormKey.ModKey.Type,
                    ComponentModKeyFileName = component.ComponentFormKey.ModKey.FileName,
                    ComponentFormKeyId = component.ComponentFormKey.Id,
                    component.Count,
                    component.ImportedAtUTC
                });
        }
    }

    private void ReplaceCategories(ConstructibleObjectDTO dto)
    {
        Database.Execute(
            """
            DELETE FROM ConstructibleObjectCategories
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

        foreach (var category in dto.Categories)
        {
            category.ImportedAtUTC = dto.ImportedAtUTC;
            Database.Execute(
                """
                INSERT OR REPLACE INTO ConstructibleObjectCategories (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    Category_Index, Category_ModKey_Name, Category_ModKey_Type, Category_ModKey_FileName, Category_FormKey_ID, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @CategoryIndex, @CategoryModKeyName, @CategoryModKeyType, @CategoryModKeyFileName, @CategoryFormKeyId, @ImportedAtUTC);
                """,
                new
                {
                    Game = category.Game.ToString(),
                    ModKeyName = category.ModKey.Name,
                    ModKeyType = category.ModKey.Type,
                    ModKeyFileName = category.ModKey.FileName,
                    FormKeyModKeyName = category.FormKey.ModKey.Name,
                    FormKeyModKeyType = category.FormKey.ModKey.Type,
                    FormKeyModKeyFileName = category.FormKey.ModKey.FileName,
                    FormKeyId = category.FormKey.Id,
                    category.CategoryIndex,
                    CategoryModKeyName = category.CategoryFormKey.ModKey.Name,
                    CategoryModKeyType = category.CategoryFormKey.ModKey.Type,
                    CategoryModKeyFileName = category.CategoryFormKey.ModKey.FileName,
                    CategoryFormKeyId = category.CategoryFormKey.Id,
                    category.ImportedAtUTC
                });
        }
    }

    private void ReplaceRecipeFilters(ConstructibleObjectDTO dto)
    {
        Database.Execute(
            """
            DELETE FROM ConstructibleObjectRecipeFilters
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

        foreach (var recipeFilter in dto.RecipeFilters)
        {
            recipeFilter.ImportedAtUTC = dto.ImportedAtUTC;
            Database.Execute(
                """
                INSERT OR REPLACE INTO ConstructibleObjectRecipeFilters (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    RecipeFilter_Index, RecipeFilter_ModKey_Name, RecipeFilter_ModKey_Type, RecipeFilter_ModKey_FileName, RecipeFilter_FormKey_ID, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @RecipeFilterIndex, @RecipeFilterModKeyName, @RecipeFilterModKeyType, @RecipeFilterModKeyFileName, @RecipeFilterFormKeyId, @ImportedAtUTC);
                """,
                new
                {
                    Game = recipeFilter.Game.ToString(),
                    ModKeyName = recipeFilter.ModKey.Name,
                    ModKeyType = recipeFilter.ModKey.Type,
                    ModKeyFileName = recipeFilter.ModKey.FileName,
                    FormKeyModKeyName = recipeFilter.FormKey.ModKey.Name,
                    FormKeyModKeyType = recipeFilter.FormKey.ModKey.Type,
                    FormKeyModKeyFileName = recipeFilter.FormKey.ModKey.FileName,
                    FormKeyId = recipeFilter.FormKey.Id,
                    recipeFilter.RecipeFilterIndex,
                    RecipeFilterModKeyName = recipeFilter.RecipeFilterFormKey.ModKey.Name,
                    RecipeFilterModKeyType = recipeFilter.RecipeFilterFormKey.ModKey.Type,
                    RecipeFilterModKeyFileName = recipeFilter.RecipeFilterFormKey.ModKey.FileName,
                    RecipeFilterFormKeyId = recipeFilter.RecipeFilterFormKey.Id,
                    recipeFilter.ImportedAtUTC
                });
        }
    }

    private static ConstructibleObjectDTO ToDTO(ConstructibleObjectRow record, SupportedGame game)
    {
        var dto = new ConstructibleObjectDTO
        {
            Game = game,
            ModKey = new ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty },
            FormKey = new FormKeyDTO { ModKey = new ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty }, Id = 0 },
            EditorID = string.Empty,
            FormVersion = 0,
            MajorRecordFlags = 0,
            ImportedAtUTC = record.ImportedAtUTC,
            Version2 = record.Version2,
            Description = FromEnglish(record.Description),
            CreatedObjectFormKey = CreateNullableFormKey(record.CreatedObjectModKeyName, record.CreatedObjectModKeyType, record.CreatedObjectModKeyFileName, record.CreatedObjectFormKeyId),
            WorkbenchKeywordFormKey = CreateNullableFormKey(record.WorkbenchKeywordModKeyName, record.WorkbenchKeywordModKeyType, record.WorkbenchKeywordModKeyFileName, record.WorkbenchKeywordFormKeyId),
            CreatedObjectCount = record.CreatedObjectCount,
            AmountProduced = record.AmountProduced,
            MenuSortOrder = record.MenuSortOrder,
            LearnMethod = record.LearnMethod,
            Flags = record.Flags
        };
        ApplyCommonFields(dto, record, game);
        return dto;
    }

    private static ConstructibleObjectComponentDTO ToDTO(ConstructibleObjectComponentRow row, SupportedGame game)
    {
        return new ConstructibleObjectComponentDTO
        {
            Game = game,
            ModKey = CreateModKey(row.ModKeyName, row.ModKeyType, row.ModKeyFileName),
            FormKey = CreateFormKey(row.FormKeyModKeyName, row.FormKeyModKeyType, row.FormKeyModKeyFileName, row.FormKeyId),
            ComponentFormKey = CreateFormKey(row.ComponentModKeyName, row.ComponentModKeyType, row.ComponentModKeyFileName, row.ComponentFormKeyId),
            ComponentIndex = row.ComponentIndex,
            Count = row.Count,
            ImportedAtUTC = row.ImportedAtUTC
        };
    }

    private static ConstructibleObjectCategoryDTO ToDTO(ConstructibleObjectCategoryRow row, SupportedGame game)
    {
        return new ConstructibleObjectCategoryDTO
        {
            Game = game,
            ModKey = CreateModKey(row.ModKeyName, row.ModKeyType, row.ModKeyFileName),
            FormKey = CreateFormKey(row.FormKeyModKeyName, row.FormKeyModKeyType, row.FormKeyModKeyFileName, row.FormKeyId),
            CategoryFormKey = CreateFormKey(row.CategoryModKeyName, row.CategoryModKeyType, row.CategoryModKeyFileName, row.CategoryFormKeyId),
            CategoryIndex = row.CategoryIndex,
            ImportedAtUTC = row.ImportedAtUTC
        };
    }

    private static ConstructibleObjectRecipeFilterDTO ToDTO(ConstructibleObjectRecipeFilterRow row, SupportedGame game)
    {
        return new ConstructibleObjectRecipeFilterDTO
        {
            Game = game,
            ModKey = CreateModKey(row.ModKeyName, row.ModKeyType, row.ModKeyFileName),
            FormKey = CreateFormKey(row.FormKeyModKeyName, row.FormKeyModKeyType, row.FormKeyModKeyFileName, row.FormKeyId),
            RecipeFilterFormKey = CreateFormKey(row.RecipeFilterModKeyName, row.RecipeFilterModKeyType, row.RecipeFilterModKeyFileName, row.RecipeFilterFormKeyId),
            RecipeFilterIndex = row.RecipeFilterIndex,
            ImportedAtUTC = row.ImportedAtUTC
        };
    }

    private static ModKeyDTO CreateModKey(string name, int type, string fileName)
    {
        return new ModKeyDTO
        {
            Name = name,
            Type = type,
            FileName = fileName
        };
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

    private sealed class ConstructibleObjectRow : RecordRow
    {
        public int? Version2 { get; set; }

        public string? Description { get; set; }

        public string? CreatedObjectModKeyName { get; set; }

        public int? CreatedObjectModKeyType { get; set; }

        public string? CreatedObjectModKeyFileName { get; set; }

        public long? CreatedObjectFormKeyId { get; set; }

        public string? WorkbenchKeywordModKeyName { get; set; }

        public int? WorkbenchKeywordModKeyType { get; set; }

        public string? WorkbenchKeywordModKeyFileName { get; set; }

        public long? WorkbenchKeywordFormKeyId { get; set; }

        public int? CreatedObjectCount { get; set; }

        public int? AmountProduced { get; set; }

        public int? MenuSortOrder { get; set; }

        public string? LearnMethod { get; set; }

        public string? Flags { get; set; }
    }

    private sealed class ConstructibleObjectComponentRow
    {
        public string ModKeyName { get; set; } = string.Empty;

        public int ModKeyType { get; set; }

        public string ModKeyFileName { get; set; } = string.Empty;

        public string FormKeyModKeyName { get; set; } = string.Empty;

        public int FormKeyModKeyType { get; set; }

        public string FormKeyModKeyFileName { get; set; } = string.Empty;

        public long FormKeyId { get; set; }

        public int ComponentIndex { get; set; }

        public string ComponentModKeyName { get; set; } = string.Empty;

        public int ComponentModKeyType { get; set; }

        public string ComponentModKeyFileName { get; set; } = string.Empty;

        public long ComponentFormKeyId { get; set; }

        public int? Count { get; set; }

        public DateTime ImportedAtUTC { get; set; }
    }

    private sealed class ConstructibleObjectCategoryRow
    {
        public string ModKeyName { get; set; } = string.Empty;

        public int ModKeyType { get; set; }

        public string ModKeyFileName { get; set; } = string.Empty;

        public string FormKeyModKeyName { get; set; } = string.Empty;

        public int FormKeyModKeyType { get; set; }

        public string FormKeyModKeyFileName { get; set; } = string.Empty;

        public long FormKeyId { get; set; }

        public int CategoryIndex { get; set; }

        public string CategoryModKeyName { get; set; } = string.Empty;

        public int CategoryModKeyType { get; set; }

        public string CategoryModKeyFileName { get; set; } = string.Empty;

        public long CategoryFormKeyId { get; set; }

        public DateTime ImportedAtUTC { get; set; }
    }

    private sealed class ConstructibleObjectRecipeFilterRow
    {
        public string ModKeyName { get; set; } = string.Empty;

        public int ModKeyType { get; set; }

        public string ModKeyFileName { get; set; } = string.Empty;

        public string FormKeyModKeyName { get; set; } = string.Empty;

        public int FormKeyModKeyType { get; set; }

        public string FormKeyModKeyFileName { get; set; } = string.Empty;

        public long FormKeyId { get; set; }

        public int RecipeFilterIndex { get; set; }

        public string RecipeFilterModKeyName { get; set; } = string.Empty;

        public int RecipeFilterModKeyType { get; set; }

        public string RecipeFilterModKeyFileName { get; set; } = string.Empty;

        public long RecipeFilterFormKeyId { get; set; }

        public DateTime ImportedAtUTC { get; set; }
    }
}
