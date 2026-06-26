using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;
using ModelDatabase = CreationsForge.Core.Models.Database.Model;
using ModelMaterialSwapDatabase = CreationsForge.Core.Models.Database.ModelMaterialSwap;

namespace CreationsForge.Core.Repositories;

public class ModelRepository : IModelRepository
{
    private readonly IDatabase Database;

    public ModelRepository(IDatabase database)
    {
        Database = database;
    }

    public void Save(ModelDTO dto)
    {
        Database.Save(new ModelDatabase(dto));
    }

    public IReadOnlyList<ModelDTO> GetByFormKey(SupportedGame game, string recordType, FormKeyDTO formKey)
    {
        var models = Database.Fetch<ModelDatabase>(
            """
            SELECT *
            FROM Models
            WHERE Game = @Game
              AND RecordType = @RecordType
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId
            ORDER BY ModKey_FileName COLLATE NOCASE, ModelSlot COLLATE NOCASE, ModelGender COLLATE NOCASE;
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
        var materialSwaps = Database.Fetch<ModelMaterialSwapDatabase>(
            """
            SELECT *
            FROM ModelMaterialSwaps
            WHERE Game = @Game
              AND RecordType = @RecordType
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId
            ORDER BY ModKey_FileName COLLATE NOCASE, ModelSlot COLLATE NOCASE, ModelGender COLLATE NOCASE, MaterialSwap_Index;
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

        var dtos = models.Select(model => ToDTO(game, model)).ToList();
        foreach (var dto in dtos)
        {
            dto.MaterialSwaps = materialSwaps
                .Where(materialSwap => IsSameModel(dto, materialSwap))
                .Select(materialSwap => ToDTO(game, materialSwap))
                .ToList();
        }

        return dtos;
    }

    public void DeleteByRecord(SupportedGame game, ModKeyDTO modKey, string recordType, FormKeyDTO formKey)
    {
        Database.Execute(
            """
            DELETE FROM Models
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
            DELETE FROM Models
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

    private static ModelDTO ToDTO(SupportedGame game, ModelDatabase model)
    {
        return new ModelDTO
        {
            Game = game,
            ModKey = new ModKeyDTO
            {
                Name = model.ModKeyName,
                Type = model.ModKeyType,
                FileName = model.ModKeyFileName
            },
            RecordType = model.RecordType,
            FormKey = new FormKeyDTO
            {
                ModKey = new ModKeyDTO
                {
                    Name = model.FormKeyModKeyName,
                    Type = model.FormKeyModKeyType,
                    FileName = model.FormKeyModKeyFileName
                },
                Id = (uint)model.FormKeyId
            },
            ModelSlot = model.ModelSlot,
            ModelGender = model.ModelGender,
            File = model.File,
            Data = model.Data,
            TextureFileHashes = model.TextureFileHashes,
            LightLayer = model.LightLayer.HasValue ? (uint)model.LightLayer.Value : null,
            Flags = model.Flags,
            ColorRemappingIndex = model.ColorRemappingIndex,
            FlagsVestigial = model.FlagsVestigial,
            ImportedAtUTC = model.ImportedAtUTC
        };
    }

    private static ModelMaterialSwapDTO ToDTO(SupportedGame game, ModelMaterialSwapDatabase materialSwap)
    {
        return new ModelMaterialSwapDTO
        {
            Game = game,
            ModKey = new ModKeyDTO
            {
                Name = materialSwap.ModKeyName,
                Type = materialSwap.ModKeyType,
                FileName = materialSwap.ModKeyFileName
            },
            RecordType = materialSwap.RecordType,
            FormKey = new FormKeyDTO
            {
                ModKey = new ModKeyDTO
                {
                    Name = materialSwap.FormKeyModKeyName,
                    Type = materialSwap.FormKeyModKeyType,
                    FileName = materialSwap.FormKeyModKeyFileName
                },
                Id = (uint)materialSwap.FormKeyId
            },
            ModelSlot = materialSwap.ModelSlot,
            ModelGender = materialSwap.ModelGender,
            Name = materialSwap.Name,
            MaterialSwapFormKey = new FormKeyDTO
            {
                ModKey = new ModKeyDTO
                {
                    Name = materialSwap.MaterialSwapModKeyName,
                    Type = materialSwap.MaterialSwapModKeyType,
                    FileName = materialSwap.MaterialSwapModKeyFileName
                },
                Id = (uint)materialSwap.MaterialSwapFormKeyId
            },
            MaterialSwapIndex = materialSwap.MaterialSwapIndex,
            ImportedAtUTC = materialSwap.ImportedAtUTC
        };
    }

    private static bool IsSameModel(ModelDTO model, ModelMaterialSwapDatabase materialSwap)
    {
        return model.ModKey.Type == materialSwap.ModKeyType &&
            string.Equals(model.ModKey.Name, materialSwap.ModKeyName, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(model.ModKey.FileName, materialSwap.ModKeyFileName, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(model.ModelSlot, materialSwap.ModelSlot, StringComparison.Ordinal) &&
            string.Equals(model.ModelGender, materialSwap.ModelGender, StringComparison.Ordinal);
    }
}
