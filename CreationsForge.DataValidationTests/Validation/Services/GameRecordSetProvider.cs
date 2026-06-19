using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Fallout4;
using CreationsForge.Skyrim;
using CreationsForge.Starfield;

namespace CreationsForge.DataValidationTests.Validation.Services;

public class GameRecordSetProvider
{
    private readonly Lazy<PluginRecordSetDTO> fallout4RecordSet;
    private readonly Lazy<PluginRecordSetDTO> skyrimRecordSet;
    private readonly Lazy<PluginRecordSetDTO> starfieldRecordSet;

    public GameRecordSetProvider()
    {
        fallout4RecordSet = new Lazy<PluginRecordSetDTO>(() => CreateReader(SupportedGame.Fallout4).ReadPluginRecords(CreatePlugin(SupportedGame.Fallout4, "Fallout4.esm")));
        skyrimRecordSet = new Lazy<PluginRecordSetDTO>(() => CreateReader(SupportedGame.Skyrim).ReadPluginRecords(CreatePlugin(SupportedGame.Skyrim, "Skyrim.esm")));
        starfieldRecordSet = new Lazy<PluginRecordSetDTO>(() => CreateReader(SupportedGame.Starfield).ReadPluginRecords(CreatePlugin(SupportedGame.Starfield, "Starfield.esm")));
    }

    public RecordDTO GetRecord(SupportedGame game, string recordType, string rawFormKey)
    {
        var expectedFormKey = ParseFormKey(rawFormKey);
        var record = GetRecords(GetRecordSet(game), recordType)
            .FirstOrDefault(candidate => FormKeysMatch(candidate.FormKey, expectedFormKey));

        return record ?? throw new InvalidOperationException(
            $"Unable to find record '{rawFormKey}' for record type '{recordType}' in game '{game}'.");
    }

    private PluginRecordSetDTO GetRecordSet(SupportedGame game)
    {
        return game switch
        {
            SupportedGame.Fallout4 => fallout4RecordSet.Value,
            SupportedGame.Skyrim => skyrimRecordSet.Value,
            SupportedGame.Starfield => starfieldRecordSet.Value,
            _ => throw new InvalidOperationException($"Unsupported game '{game}'.")
        };
    }

    private static IGameRecordReader CreateReader(SupportedGame game)
    {
        return game switch
        {
            SupportedGame.Fallout4 => new Fallout4RecordReader(new Fallout4RecordReaderService(new Fallout4GameMetadataService())),
            SupportedGame.Skyrim => new SkyrimRecordReader(new SkyrimRecordReaderService(new SkyrimGameMetadataService())),
            SupportedGame.Starfield => new StarfieldRecordReader(new StarfieldRecordReaderService(new StarfieldGameMetadataService())),
            _ => throw new InvalidOperationException($"Unsupported game '{game}'.")
        };
    }

    private static IEnumerable<RecordDTO> GetRecords(PluginRecordSetDTO recordSet, string recordType)
    {
        return recordType switch
        {
            "AVIF" => recordSet.ActorValueInformation,
            "BOOK" => recordSet.Books,
            "CLAS" => recordSet.Classes,
            "CONT" => recordSet.Containers,
            "CNDF" => recordSet.ConditionForms,
            "COBJ" => recordSet.ConstructibleObjects,
            "DOOR" => recordSet.Doors,
            "FACT" => recordSet.Factions,
            "FLST" => recordSet.FormLists,
            "GMST" => recordSet.GameSettings,
            "GLOB" => recordSet.Globals,
            "KYWD" => recordSet.Keywords,
            "MGEF" => recordSet.MagicEffects,
            "MISC" => recordSet.MiscObjects,
            "NPC_" => recordSet.NPCs,
            "PERK" => recordSet.Perks,
            "STAT" => recordSet.Statics,
            "TERM" => recordSet.Terminals,
            _ => throw new InvalidOperationException($"Unsupported record type '{recordType}'.")
        };
    }

    private static PluginDTO CreatePlugin(SupportedGame game, string fileName)
    {
        return new PluginDTO
        {
            Game = game,
            ModKey = new ModKeyDTO
            {
                Name = Path.GetFileNameWithoutExtension(fileName),
                FileName = fileName,
                Type = 0
            },
            LoadOrderIndex = 0,
            Enabled = true,
            ExistsOnDisk = true,
            ImportState = PluginImportState.Current,
            HeaderFlags = 0,
            FormVersion = 0,
            RecordCount = 0,
            SourceLastWriteUTCTicks = 0,
            SourceFileSizeBytes = 0,
            LastCheckedUTC = DateTime.UtcNow
        };
    }

    private static FormKeyDTO ParseFormKey(string rawFormKey)
    {
        var separatorIndex = rawFormKey.IndexOf(':', StringComparison.Ordinal);
        if (separatorIndex <= 0 || separatorIndex >= rawFormKey.Length - 1)
        {
            throw new FormatException($"Invalid Spriggit FormKey '{rawFormKey}'.");
        }

        var fileName = rawFormKey[(separatorIndex + 1)..];
        return new FormKeyDTO
        {
            Id = Convert.ToUInt32(rawFormKey[..separatorIndex], 16),
            ModKey = new ModKeyDTO
            {
                FileName = fileName,
                Name = Path.GetFileNameWithoutExtension(fileName),
                Type = 0
            }
        };
    }

    private static bool FormKeysMatch(FormKeyDTO left, FormKeyDTO right)
    {
        return left.Id == right.Id &&
               string.Equals(left.ModKey.FileName, right.ModKey.FileName, StringComparison.OrdinalIgnoreCase);
    }
}
