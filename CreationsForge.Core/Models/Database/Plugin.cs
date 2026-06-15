using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;
using NPoco;

namespace CreationsForge.Core.Models.Database;

[TableName("Plugins")]
[PrimaryKey("Game, ModKey_Name, ModKey_Type, ModKey_FileName", AutoIncrement = false)]
public class Plugin
{
    public Plugin()
    { }

    public Plugin(PluginDTO dto)
    {
        Game = dto.Game.ToString();
        ModKeyName = dto.ModKey.Name;
        ModKeyType = dto.ModKey.Type;
        ModKeyFileName = dto.ModKey.FileName;
        LoadOrderIndex = dto.LoadOrderIndex;
        Enabled = dto.Enabled;
        ExistsOnDisk = dto.ExistsOnDisk;
        ImportState = dto.ImportState.ToString();
        HeaderFlags = dto.HeaderFlags;
        FormVersion = dto.FormVersion;
        Author = dto.Author;
        Description = dto.Description;
        ImportMessage = dto.ImportMessage;
        ImportDetails = dto.ImportDetails;
        RecordCount = dto.RecordCount;
        SourceLastWriteUTCTicks = dto.SourceLastWriteUTCTicks;
        SourceFileSizeBytes = dto.SourceFileSizeBytes;
        LastCheckedUTC = dto.LastCheckedUTC;
        LastImportedUTC = dto.LastImportedUTC;
        InvalidatedAtUTC = dto.InvalidatedAtUTC;
    }

    [Column("Game")] public string Game { get; set; } = string.Empty;

    [Column("ModKey_Name")] public string ModKeyName { get; set; } = string.Empty;

    [Column("ModKey_Type")] public int ModKeyType { get; set; }

    [Column("ModKey_FileName")] public string ModKeyFileName { get; set; } = string.Empty;

    [Column("LoadOrderIndex")] public int LoadOrderIndex { get; set; }

    [Column("Enabled")] public bool Enabled { get; set; } = true;

    [Column("ExistsOnDisk")] public bool ExistsOnDisk { get; set; } = true;

    [Column("ImportState")] public string ImportState { get; set; } = nameof(PluginImportState.Current);

    [Column("HeaderFlags")] public int HeaderFlags { get; set; }

    [Column("FormVersion")] public int FormVersion { get; set; }

    [Column("Author")] public string? Author { get; set; }

    [Column("Description")] public string? Description { get; set; }

    [Column("ImportMessage")] public string? ImportMessage { get; set; }

    [Column("ImportDetails")] public string? ImportDetails { get; set; }

    [Column("RecordCount")] public int RecordCount { get; set; }

    [Column("SourceLastWriteUTCTicks")] public long SourceLastWriteUTCTicks { get; set; }

    [Column("SourceFileSizeBytes")] public long SourceFileSizeBytes { get; set; }

    [Column("LastCheckedUTC")] public DateTime LastCheckedUTC { get; set; }

    [Column("LastImportedUTC")] public DateTime? LastImportedUTC { get; set; }

    [Column("InvalidatedAtUTC")] public DateTime? InvalidatedAtUTC { get; set; }

    public PluginDTO ToDTO()
    {
        if (!Enum.TryParse<SupportedGame>(Game, out var supportedGame))
        {
            throw new ArgumentOutOfRangeException(nameof(Game), Game, "Invalid supported game value.");
        }

        if (!Enum.TryParse<PluginImportState>(ImportState, out var importState))
        {
            throw new ArgumentOutOfRangeException(nameof(ImportState), ImportState, "Invalid plugin import state value.");
        }

        return new PluginDTO
        {
            Game = supportedGame,
            ModKey = new ModKeyDTO
            {
                Name = ModKeyName,
                Type = ModKeyType,
                FileName = ModKeyFileName
            },
            LoadOrderIndex = LoadOrderIndex,
            Enabled = Enabled,
            ExistsOnDisk = ExistsOnDisk,
            ImportState = importState,
            HeaderFlags = HeaderFlags,
            FormVersion = FormVersion,
            Author = Author,
            Description = Description,
            ImportMessage = ImportMessage,
            ImportDetails = ImportDetails,
            RecordCount = RecordCount,
            SourceLastWriteUTCTicks = SourceLastWriteUTCTicks,
            SourceFileSizeBytes = SourceFileSizeBytes,
            LastCheckedUTC = LastCheckedUTC,
            LastImportedUTC = LastImportedUTC,
            InvalidatedAtUTC = InvalidatedAtUTC
        };
    }
}
