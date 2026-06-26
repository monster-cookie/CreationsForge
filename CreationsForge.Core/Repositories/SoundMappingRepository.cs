using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;
using SoundMappingDatabase = CreationsForge.Core.Models.Database.SoundMapping;

namespace CreationsForge.Core.Repositories;

public class SoundMappingRepository : ISoundMappingRepository
{
    private readonly IDatabase Database;

    public SoundMappingRepository(IDatabase database)
    {
        Database = database;
    }

    public void Save(SoundMappingDTO dto)
    {
        Database.Save(new SoundMappingDatabase(dto));
    }

    public IReadOnlyList<SoundMappingDTO> GetByFormKey(SupportedGame game, string recordType, FormKeyDTO formKey)
    {
        return Database.Fetch<SoundMappingDatabase>(
                """
                SELECT *
                FROM SoundMappings
                WHERE Game = @Game
                  AND RecordType = @RecordType
                  AND FormKey_ModKey_Name = @FormKeyModKeyName
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
                  AND FormKey_ID = @FormKeyId
                ORDER BY ModKey_FileName COLLATE NOCASE, SoundSlot COLLATE NOCASE, Sound_Index;
                """,
                new
                {
                    Game = game.ToString(),
                    RecordType = recordType,
                    FormKeyModKeyName = formKey.ModKey.Name,
                    FormKeyModKeyType = formKey.ModKey.Type,
                    FormKeyModKeyFileName = formKey.ModKey.FileName,
                    FormKeyId = formKey.Id
                })
            .Select(sound => ToDTO(game, sound))
            .ToList();
    }

    public void DeleteByRecord(SupportedGame game, ModKeyDTO modKey, string recordType, FormKeyDTO formKey)
    {
        Database.Execute(
            """
            DELETE FROM SoundMappings
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
            DELETE FROM SoundMappings
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

    private static SoundMappingDTO ToDTO(SupportedGame game, SoundMappingDatabase sound)
    {
        return new SoundMappingDTO
        {
            Game = game,
            ModKey = new ModKeyDTO
            {
                Name = sound.ModKeyName,
                Type = sound.ModKeyType,
                FileName = sound.ModKeyFileName
            },
            RecordType = sound.RecordType,
            FormKey = new FormKeyDTO
            {
                ModKey = new ModKeyDTO
                {
                    Name = sound.FormKeyModKeyName,
                    Type = sound.FormKeyModKeyType,
                    FileName = sound.FormKeyModKeyFileName
                },
                Id = (uint)sound.FormKeyId
            },
            SoundSlot = sound.SoundSlot,
            SoundIndex = sound.SoundIndex,
            Start = sound.Start,
            Stop = sound.Stop,
            MutagenObjectType = sound.MutagenObjectType,
            InheritsSoundsFrom = sound.InheritsSoundsFrom,
            Versioning = sound.Versioning,
            Unknown = sound.Unknown,
            ImportedAtUTC = sound.ImportedAtUTC
        };
    }
}
