using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;
using ScriptingAdapterModel = CreationsForge.Core.Models.Database.ScriptingAdapter;
using ScriptingAdapterPropertyListItemModel = CreationsForge.Core.Models.Database.ScriptingAdapterPropertyListItem;
using ScriptingAdapterPropertyModel = CreationsForge.Core.Models.Database.ScriptingAdapterProperty;
using ScriptingAdapterPropertyStructMemberModel = CreationsForge.Core.Models.Database.ScriptingAdapterPropertyStructMember;
using ScriptingAdapterPropertyStructModel = CreationsForge.Core.Models.Database.ScriptingAdapterPropertyStruct;

namespace CreationsForge.Core.Repositories;

public class ScriptingAdapterRepository : IScriptingAdapterRepository
{
    private readonly IDatabase Database;

    public ScriptingAdapterRepository(IDatabase database)
    {
        Database = database;
    }

    public void Save(ScriptingAdapterDTO dto)
    {
        Database.Save(new ScriptingAdapterModel(dto));
    }

    public IReadOnlyList<ScriptingAdapterDTO> GetByFormKey(SupportedGame game, string recordType, FormKeyDTO formKey)
    {
        var adapters = Database.Fetch<ScriptingAdapterModel>(
            """
            SELECT *
            FROM ScriptingAdapters
            WHERE Game = @Game
              AND RecordType = @RecordType
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId
            ORDER BY ModKey_FileName COLLATE NOCASE, Script_Index, Name COLLATE NOCASE;
            """,
            new
            {
                Game = game.ToString(),
                RecordType = recordType,
                FormKeyModKeyName = formKey.ModKey.Name,
                FormKeyModKeyType = formKey.ModKey.Type,
                FormKeyModKeyFileName = formKey.ModKey.FileName,
                FormKeyId = formKey.Id
            });
        var properties = Database.Fetch<ScriptingAdapterPropertyModel>(
            """
            SELECT *
            FROM ScriptingAdapterProperties
            WHERE Game = @Game
              AND RecordType = @RecordType
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId
            ORDER BY ModKey_FileName COLLATE NOCASE, ScriptingAdapter_Name COLLATE NOCASE, Property_Index;
            """,
            new
            {
                Game = game.ToString(),
                RecordType = recordType,
                FormKeyModKeyName = formKey.ModKey.Name,
                FormKeyModKeyType = formKey.ModKey.Type,
                FormKeyModKeyFileName = formKey.ModKey.FileName,
                FormKeyId = formKey.Id
            });
        var listItems = Database.Fetch<ScriptingAdapterPropertyListItemModel>(
            """
            SELECT *
            FROM ScriptingAdapterPropertyListItems
            WHERE Game = @Game
              AND RecordType = @RecordType
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId
            ORDER BY ModKey_FileName COLLATE NOCASE, ScriptingAdapter_Name COLLATE NOCASE, Property_Index, ListItem_Index;
            """,
            new
            {
                Game = game.ToString(),
                RecordType = recordType,
                FormKeyModKeyName = formKey.ModKey.Name,
                FormKeyModKeyType = formKey.ModKey.Type,
                FormKeyModKeyFileName = formKey.ModKey.FileName,
                FormKeyId = formKey.Id
            });
        var propertyStructs = Database.Fetch<ScriptingAdapterPropertyStructModel>(
            """
            SELECT *
            FROM ScriptingAdapterPropertyStructs
            WHERE Game = @Game
              AND RecordType = @RecordType
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId
            ORDER BY ModKey_FileName COLLATE NOCASE, ScriptingAdapter_Name COLLATE NOCASE, Property_Index, Struct_Index;
            """,
            new
            {
                Game = game.ToString(),
                RecordType = recordType,
                FormKeyModKeyName = formKey.ModKey.Name,
                FormKeyModKeyType = formKey.ModKey.Type,
                FormKeyModKeyFileName = formKey.ModKey.FileName,
                FormKeyId = formKey.Id
            });
        var structMembers = Database.Fetch<ScriptingAdapterPropertyStructMemberModel>(
            """
            SELECT *
            FROM ScriptingAdapterPropertyStructMembers
            WHERE Game = @Game
              AND RecordType = @RecordType
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId
            ORDER BY ModKey_FileName COLLATE NOCASE, ScriptingAdapter_Name COLLATE NOCASE, Property_Index, Struct_Index, Member_Index;
            """,
            new
            {
                Game = game.ToString(),
                RecordType = recordType,
                FormKeyModKeyName = formKey.ModKey.Name,
                FormKeyModKeyType = formKey.ModKey.Type,
                FormKeyModKeyFileName = formKey.ModKey.FileName,
                FormKeyId = formKey.Id
            });

        var dtos = adapters.Select(adapter => ToDTO(game, adapter)).ToList();
        foreach (var dto in dtos)
        {
            dto.Properties = properties
                .Where(property => IsSameAdapter(dto, property))
                .Select(property => ToDTO(
                    game,
                    property,
                    listItems.Where(listItem => IsSameProperty(property, listItem)).ToList(),
                    propertyStructs.Where(propertyStruct => IsSameProperty(property, propertyStruct)).ToList(),
                    structMembers.Where(member => IsSameProperty(property, member)).ToList()))
                .ToList();
        }

        return dtos;
    }

    public void DeleteByRecord(SupportedGame game, ModKeyDTO modKey, string recordType, FormKeyDTO formKey)
    {
        Database.Execute(
            """
            DELETE FROM ScriptingAdapters
            WHERE Game = @Game
              AND ModKey_Name = @ModKeyName
              AND ModKey_Type = @ModKeyType
              AND ModKey_FileName = @ModKeyFileName
              AND RecordType = @RecordType
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId;
            """,
            new
            {
                Game = game.ToString(),
                ModKeyName = modKey.Name,
                ModKeyType = modKey.Type,
                ModKeyFileName = modKey.FileName,
                RecordType = recordType,
                FormKeyModKeyName = formKey.ModKey.Name,
                FormKeyModKeyType = formKey.ModKey.Type,
                FormKeyModKeyFileName = formKey.ModKey.FileName,
                FormKeyId = formKey.Id
            });
    }

    public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
    {
        Database.Execute(
            """
            DELETE FROM ScriptingAdapters
            WHERE Game = @Game
              AND ModKey_Name = @ModKeyName
              AND ModKey_Type = @ModKeyType
              AND ModKey_FileName = @ModKeyFileName
              AND ImportedAtUTC <> @ImportedAtUTC;
            """,
            new
            {
                Game = game.ToString(),
                ModKeyName = modKey.Name,
                ModKeyType = modKey.Type,
                ModKeyFileName = modKey.FileName,
                ImportedAtUTC = importedAtUTC
            });
    }

    private static ScriptingAdapterDTO ToDTO(SupportedGame game, ScriptingAdapterModel adapter)
    {
        return new ScriptingAdapterDTO
        {
            Game = game,
            ModKey = new ModKeyDTO
            {
                Name = adapter.ModKeyName,
                Type = adapter.ModKeyType,
                FileName = adapter.ModKeyFileName
            },
            RecordType = adapter.RecordType,
            FormKey = new FormKeyDTO
            {
                ModKey = new ModKeyDTO
                {
                    Name = adapter.FormKeyModKeyName,
                    Type = adapter.FormKeyModKeyType,
                    FileName = adapter.FormKeyModKeyFileName
                },
                Id = (uint)adapter.FormKeyId
            },
            Name = adapter.Name,
            ScriptIndex = adapter.ScriptIndex,
            ImportedAtUTC = adapter.ImportedAtUTC
        };
    }

    private static ScriptingAdapterPropertyDTO ToDTO(
        SupportedGame game,
        ScriptingAdapterPropertyModel property,
        IReadOnlyList<ScriptingAdapterPropertyListItemModel> listItems,
        IReadOnlyList<ScriptingAdapterPropertyStructModel> propertyStructs,
        IReadOnlyList<ScriptingAdapterPropertyStructMemberModel> structMembers)
    {
        return new ScriptingAdapterPropertyDTO
        {
            Game = game,
            ModKey = new ModKeyDTO
            {
                Name = property.ModKeyName,
                Type = property.ModKeyType,
                FileName = property.ModKeyFileName
            },
            RecordType = property.RecordType,
            FormKey = new FormKeyDTO
            {
                ModKey = new ModKeyDTO
                {
                    Name = property.FormKeyModKeyName,
                    Type = property.FormKeyModKeyType,
                    FileName = property.FormKeyModKeyFileName
                },
                Id = (uint)property.FormKeyId
            },
            ScriptingAdapterName = property.ScriptingAdapterName,
            PropertyIndex = property.PropertyIndex,
            Name = property.Name,
            MutagenObjectType = property.MutagenObjectType,
            DataBool = property.DataBool,
            DataInt = property.DataInt,
            DataFloat = property.DataFloat,
            DataString = property.DataString,
            ObjectFormKey = CreateOptionalFormKey(property.ObjectModKeyName, property.ObjectModKeyType, property.ObjectModKeyFileName, property.ObjectFormKeyId),
            ObjectAlias = property.ObjectAlias,
            ObjectUnused = property.ObjectUnused.HasValue ? (ushort)property.ObjectUnused.Value : null,
            ImportedAtUTC = property.ImportedAtUTC,
            ListItems = listItems.Select(listItem => ToDTO(game, listItem)).ToList(),
            Structs = propertyStructs.Select(propertyStruct => ToDTO(
                game,
                propertyStruct,
                structMembers.Where(member => IsSameStruct(propertyStruct, member)).ToList())).ToList()
        };
    }

    private static ScriptingAdapterPropertyListItemDTO ToDTO(SupportedGame game, ScriptingAdapterPropertyListItemModel listItem)
    {
        return new ScriptingAdapterPropertyListItemDTO
        {
            Game = game,
            ModKey = new ModKeyDTO
            {
                Name = listItem.ModKeyName,
                Type = listItem.ModKeyType,
                FileName = listItem.ModKeyFileName
            },
            RecordType = listItem.RecordType,
            FormKey = new FormKeyDTO
            {
                ModKey = new ModKeyDTO
                {
                    Name = listItem.FormKeyModKeyName,
                    Type = listItem.FormKeyModKeyType,
                    FileName = listItem.FormKeyModKeyFileName
                },
                Id = (uint)listItem.FormKeyId
            },
            ScriptingAdapterName = listItem.ScriptingAdapterName,
            PropertyIndex = listItem.PropertyIndex,
            ListItemIndex = listItem.ListItemIndex,
            Name = listItem.Name,
            MutagenObjectType = listItem.MutagenObjectType,
            DataBool = listItem.DataBool,
            DataInt = listItem.DataInt,
            DataFloat = listItem.DataFloat,
            DataString = listItem.DataString,
            ObjectFormKey = CreateOptionalFormKey(listItem.ObjectModKeyName, listItem.ObjectModKeyType, listItem.ObjectModKeyFileName, listItem.ObjectFormKeyId),
            ObjectAlias = listItem.ObjectAlias,
            ObjectUnused = listItem.ObjectUnused.HasValue ? (ushort)listItem.ObjectUnused.Value : null,
            ImportedAtUTC = listItem.ImportedAtUTC
        };
    }

    private static ScriptingAdapterPropertyStructDTO ToDTO(
        SupportedGame game,
        ScriptingAdapterPropertyStructModel propertyStruct,
        IReadOnlyList<ScriptingAdapterPropertyStructMemberModel> members)
    {
        return new ScriptingAdapterPropertyStructDTO
        {
            Game = game,
            ModKey = new ModKeyDTO
            {
                Name = propertyStruct.ModKeyName,
                Type = propertyStruct.ModKeyType,
                FileName = propertyStruct.ModKeyFileName
            },
            RecordType = propertyStruct.RecordType,
            FormKey = new FormKeyDTO
            {
                ModKey = new ModKeyDTO
                {
                    Name = propertyStruct.FormKeyModKeyName,
                    Type = propertyStruct.FormKeyModKeyType,
                    FileName = propertyStruct.FormKeyModKeyFileName
                },
                Id = (uint)propertyStruct.FormKeyId
            },
            ScriptingAdapterName = propertyStruct.ScriptingAdapterName,
            PropertyIndex = propertyStruct.PropertyIndex,
            StructIndex = propertyStruct.StructIndex,
            ImportedAtUTC = propertyStruct.ImportedAtUTC,
            Members = members.Select(member => ToDTO(game, member)).ToList()
        };
    }

    private static ScriptingAdapterPropertyStructMemberDTO ToDTO(
        SupportedGame game,
        ScriptingAdapterPropertyStructMemberModel member)
    {
        return new ScriptingAdapterPropertyStructMemberDTO
        {
            Game = game,
            ModKey = new ModKeyDTO
            {
                Name = member.ModKeyName,
                Type = member.ModKeyType,
                FileName = member.ModKeyFileName
            },
            RecordType = member.RecordType,
            FormKey = new FormKeyDTO
            {
                ModKey = new ModKeyDTO
                {
                    Name = member.FormKeyModKeyName,
                    Type = member.FormKeyModKeyType,
                    FileName = member.FormKeyModKeyFileName
                },
                Id = (uint)member.FormKeyId
            },
            ScriptingAdapterName = member.ScriptingAdapterName,
            PropertyIndex = member.PropertyIndex,
            StructIndex = member.StructIndex,
            MemberIndex = member.MemberIndex,
            Name = member.Name,
            MutagenObjectType = member.MutagenObjectType,
            DataBool = member.DataBool,
            DataInt = member.DataInt,
            DataFloat = member.DataFloat,
            DataString = member.DataString,
            ObjectFormKey = CreateOptionalFormKey(member.ObjectModKeyName, member.ObjectModKeyType, member.ObjectModKeyFileName, member.ObjectFormKeyId),
            ObjectAlias = member.ObjectAlias,
            ObjectUnused = member.ObjectUnused.HasValue ? (ushort)member.ObjectUnused.Value : null,
            ImportedAtUTC = member.ImportedAtUTC
        };
    }

    private static FormKeyDTO? CreateOptionalFormKey(string? modKeyName, int? modKeyType, string? modKeyFileName, long? formKeyId)
    {
        if (modKeyName is null || modKeyType is null || modKeyFileName is null || formKeyId is null)
        {
            return null;
        }

        return new FormKeyDTO
        {
            ModKey = new ModKeyDTO
            {
                Name = modKeyName,
                Type = modKeyType.Value,
                FileName = modKeyFileName
            },
            Id = (uint)formKeyId.Value
        };
    }

    private static bool IsSameAdapter(ScriptingAdapterDTO adapter, ScriptingAdapterPropertyModel property)
    {
        return adapter.ModKey.Type == property.ModKeyType &&
            string.Equals(adapter.ModKey.Name, property.ModKeyName, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(adapter.ModKey.FileName, property.ModKeyFileName, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(adapter.Name, property.ScriptingAdapterName, StringComparison.Ordinal);
    }

    private static bool IsSameProperty(ScriptingAdapterPropertyModel property, ScriptingAdapterPropertyListItemModel listItem)
    {
        return property.ModKeyType == listItem.ModKeyType &&
            string.Equals(property.ModKeyName, listItem.ModKeyName, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(property.ModKeyFileName, listItem.ModKeyFileName, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(property.ScriptingAdapterName, listItem.ScriptingAdapterName, StringComparison.Ordinal) &&
            property.PropertyIndex == listItem.PropertyIndex;
    }

    private static bool IsSameProperty(ScriptingAdapterPropertyModel property, ScriptingAdapterPropertyStructModel propertyStruct)
    {
        return property.ModKeyType == propertyStruct.ModKeyType &&
            string.Equals(property.ModKeyName, propertyStruct.ModKeyName, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(property.ModKeyFileName, propertyStruct.ModKeyFileName, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(property.ScriptingAdapterName, propertyStruct.ScriptingAdapterName, StringComparison.Ordinal) &&
            property.PropertyIndex == propertyStruct.PropertyIndex;
    }

    private static bool IsSameProperty(ScriptingAdapterPropertyModel property, ScriptingAdapterPropertyStructMemberModel member)
    {
        return property.ModKeyType == member.ModKeyType &&
            string.Equals(property.ModKeyName, member.ModKeyName, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(property.ModKeyFileName, member.ModKeyFileName, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(property.ScriptingAdapterName, member.ScriptingAdapterName, StringComparison.Ordinal) &&
            property.PropertyIndex == member.PropertyIndex;
    }

    private static bool IsSameStruct(ScriptingAdapterPropertyStructModel propertyStruct, ScriptingAdapterPropertyStructMemberModel member)
    {
        return propertyStruct.ModKeyType == member.ModKeyType &&
            string.Equals(propertyStruct.ModKeyName, member.ModKeyName, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(propertyStruct.ModKeyFileName, member.ModKeyFileName, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(propertyStruct.ScriptingAdapterName, member.ScriptingAdapterName, StringComparison.Ordinal) &&
            propertyStruct.PropertyIndex == member.PropertyIndex &&
            propertyStruct.StructIndex == member.StructIndex;
    }
}
