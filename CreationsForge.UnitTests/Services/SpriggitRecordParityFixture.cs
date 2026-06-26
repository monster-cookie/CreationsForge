using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Fallout4;
using CreationsForge.Skyrim;
using CreationsForge.Starfield;

namespace CreationsForge.UnitTests.Services;

public class SpriggitRecordParityFixture
{
    private readonly Lazy<PluginRecordSetDTO> fallout4RecordSet;
    private readonly Lazy<PluginRecordSetDTO> skyrimRecordSet;
    private readonly Lazy<PluginRecordSetDTO> starfieldRecordSet;
    private readonly Dictionary<string, SpriggitYamlDocument> sampleCache = new(StringComparer.OrdinalIgnoreCase);

    public SpriggitRecordParityFixture()
    {
        fallout4RecordSet = new Lazy<PluginRecordSetDTO>(() => new Fallout4RecordReaderService(new Fallout4GameMetadataService())
            .ReadPluginRecords(CreatePlugin(SupportedGame.Fallout4, "Fallout4.esm")));
        skyrimRecordSet = new Lazy<PluginRecordSetDTO>(() => new SkyrimRecordReaderService(new SkyrimGameMetadataService())
            .ReadPluginRecords(CreatePlugin(SupportedGame.Skyrim, "Skyrim.esm")));
        starfieldRecordSet = new Lazy<PluginRecordSetDTO>(() => new StarfieldRecordReaderService(new StarfieldGameMetadataService())
            .ReadPluginRecords(CreatePlugin(SupportedGame.Starfield, "Starfield.esm")));
    }

    public PluginRecordSetDTO GetRecordSet(SupportedGame game)
    {
        return game switch
        {
            SupportedGame.Fallout4 => fallout4RecordSet.Value,
            SupportedGame.Skyrim => skyrimRecordSet.Value,
            SupportedGame.Starfield => starfieldRecordSet.Value,
            _ => throw new InvalidOperationException($"Unsupported game '{game}'.")
        };
    }

    public SpriggitYamlDocument GetSample(SupportedGame game, string folderName, IReadOnlyList<string> requiredPaths)
    {
        var cacheKey = $"{game}|{folderName}|{string.Join('|', requiredPaths)}";
        if (sampleCache.TryGetValue(cacheKey, out var cachedDocument))
        {
            return cachedDocument;
        }

        var folderPath = Path.Combine(GetSpriggitRootPath(game), folderName);
        if (!Directory.Exists(folderPath))
        {
            throw new DirectoryNotFoundException($"Spriggit folder '{folderPath}' does not exist.");
        }

        foreach (var filePath in Directory.EnumerateFiles(folderPath, "*.yaml").OrderBy(static path => path, StringComparer.OrdinalIgnoreCase))
        {
            var document = SpriggitYamlDocument.Load(filePath);
            if (requiredPaths.All(document.HasPath))
            {
                sampleCache[cacheKey] = document;
                return document;
            }
        }

        var fallbackFilePath = Directory.EnumerateFiles(folderPath, "*.yaml")
            .OrderBy(static path => path, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault();
        if (fallbackFilePath == null)
        {
            throw new InvalidOperationException($"Unable to find any Spriggit samples in '{folderPath}'.");
        }

        var fallbackDocument = SpriggitYamlDocument.Load(fallbackFilePath);
        sampleCache[cacheKey] = fallbackDocument;
        return fallbackDocument;
    }

    public RecordDTO GetRecord(SupportedGame game, string recordType, string rawFormKey)
    {
        var expectedFormKey = ParseFormKey(rawFormKey);
        var record = GetRecords(GetRecordSet(game), recordType)
            .FirstOrDefault(candidate => FormKeysMatch(candidate.FormKey, expectedFormKey));

        return record ?? throw new InvalidOperationException(
            $"Unable to find record '{rawFormKey}' for record type '{recordType}' in game '{game}'.");
    }

    private static IEnumerable<RecordDTO> GetRecords(PluginRecordSetDTO recordSet, string recordType)
    {
        return recordType switch
        {
            "AVIF" => recordSet.ActorValueInformation,
            "BOOK" => recordSet.Books,
            "CONT" => recordSet.Containers,
            "CNDF" => recordSet.ConditionForms,
            "COBJ" => recordSet.ConstructibleObjects,
            "DOOR" => recordSet.Doors,
            "FLST" => recordSet.FormLists,
            "GMST" => recordSet.GameSettings,
            "GLOB" => recordSet.Globals,
            "KYWD" => recordSet.Keywords,
            "MGEF" => recordSet.MagicEffects,
            "MISC" => recordSet.MiscItems,
            "NPC_" => recordSet.NPCs,
            "PERK" => recordSet.Perks,
            "STAT" => recordSet.Statics,
            "TERM" => recordSet.Terminals,
            _ => throw new InvalidOperationException($"Unsupported record type '{recordType}'.")
        };
    }

    private static string GetSpriggitRootPath(SupportedGame game)
    {
        return game switch
        {
            SupportedGame.Fallout4 => @"C:\FalloutExtractions\Spriggit\Fallout4.esm",
            SupportedGame.Skyrim => @"C:\SkyrimExtractions\Spriggit\Skyrim.esm",
            SupportedGame.Starfield => @"C:\StarfieldExtractions\Spriggit\Starfield.esm",
            _ => throw new InvalidOperationException($"Unsupported game '{game}'.")
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

        return new FormKeyDTO
        {
            Id = Convert.ToUInt32(rawFormKey[..separatorIndex], 16),
            ModKey = new ModKeyDTO
            {
                FileName = rawFormKey[(separatorIndex + 1)..],
                Name = Path.GetFileNameWithoutExtension(rawFormKey[(separatorIndex + 1)..]),
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
