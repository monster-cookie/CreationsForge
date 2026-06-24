using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;
using ScriptFragmentModel = CreationsForge.Core.Models.Database.ScriptFragment;

namespace CreationsForge.Core.Repositories;

public class ScriptFragmentRepository : IScriptFragmentRepository
{
    private readonly IDatabase Database;

    public ScriptFragmentRepository(IDatabase database)
    {
        Database = database;
    }

    public void Save(ScriptFragmentDTO dto)
    {
        Database.Save(new ScriptFragmentModel(dto));
    }

    public IReadOnlyList<ScriptFragmentDTO> GetByFormKey(SupportedGame game, string recordType, FormKeyDTO formKey)
    {
        return Database.Fetch<ScriptFragmentModel>(
                """
                SELECT *
                FROM ScriptFragments
                WHERE Game = @Game
                  AND RecordType = @RecordType
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY ModKey_FileName COLLATE NOCASE, FragmentSlot COLLATE NOCASE, Fragment_Index;
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
            .Select(ToDTO)
            .ToList();
    }

    public void DeleteByRecord(SupportedGame game, ModKeyDTO modKey, string recordType, FormKeyDTO formKey)
    {
        Database.Execute(
            """
            DELETE FROM ScriptFragments
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

    private static ScriptFragmentDTO ToDTO(ScriptFragmentModel model)
    {
        return new ScriptFragmentDTO
        {
            Game = Enum.Parse<SupportedGame>(model.Game),
            ModKey = new ModKeyDTO { Name = model.ModKeyName, Type = model.ModKeyType, FileName = model.ModKeyFileName },
            RecordType = model.RecordType,
            FormKey = new FormKeyDTO { ModKey = new ModKeyDTO { Name = model.FormKeyModKeyName, Type = model.FormKeyModKeyType, FileName = model.FormKeyModKeyFileName }, Id = (uint)model.FormKeyId },
            FragmentSlot = model.FragmentSlot,
            FragmentIndex = model.FragmentIndex,
            MutagenObjectType = model.MutagenObjectType,
            ScriptName = model.ScriptName,
            FragmentName = model.FragmentName,
            Unknown2 = model.Unknown2,
            ExtraBindDataVersion = model.ExtraBindDataVersion,
            ImportedAtUTC = model.ImportedAtUTC
        };
    }
}
