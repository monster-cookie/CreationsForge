using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

/// <summary>
/// Stores component reflection payloads exported by Spriggit as <c>REFL</c> fields.
/// </summary>
public class ReflectionRepository : IReflectionRepository
{
    private readonly IDatabase Database;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReflectionRepository"/> class.
    /// </summary>
    /// <param name="database">The database used to persist and query reflection rows.</param>
    public ReflectionRepository(IDatabase database)
    {
        Database = database;
    }

    /// <inheritdoc />
    public void Save(ReflectionDTO dto)
    {
        Database.Execute(
            """
            INSERT OR REPLACE INTO Reflection (
                Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                Component_Index, ComponentType, SourcePath, REFL, ImportedAtUTC)
            VALUES (
                @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @RecordType, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                @ComponentIndex, @ComponentType, @SourcePath, @REFL, @ImportedAtUTC);
            """,
            new
            {
                Game = dto.Game.ToString(),
                ModKeyName = dto.ModKey.Name,
                ModKeyType = dto.ModKey.Type,
                ModKeyFileName = dto.ModKey.FileName,
                dto.RecordType,
                FormKeyModKeyName = dto.FormKey.ModKey.Name,
                FormKeyModKeyType = dto.FormKey.ModKey.Type,
                FormKeyModKeyFileName = dto.FormKey.ModKey.FileName,
                FormKeyId = dto.FormKey.Id,
                dto.ComponentIndex,
                dto.ComponentType,
                dto.SourcePath,
                dto.REFL,
                dto.ImportedAtUTC
            });
    }

    /// <inheritdoc />
    public IReadOnlyList<ReflectionDTO> GetByFormKey(SupportedGame game, string recordType, FormKeyDTO formKey)
    {
        return Database.Fetch<ReflectionRow>(
                """
                SELECT
                    Game,
                    ModKey_Name AS ModKeyName,
                    ModKey_Type AS ModKeyType,
                    ModKey_FileName AS ModKeyFileName,
                    RecordType,
                    FormKey_ModKey_Name AS FormKeyModKeyName,
                    FormKey_ModKey_Type AS FormKeyModKeyType,
                    FormKey_ModKey_FileName AS FormKeyModKeyFileName,
                    FormKey_ID AS FormKeyId,
                    Component_Index AS ComponentIndex,
                    ComponentType,
                    SourcePath,
                    REFL,
                    ImportedAtUTC
                FROM Reflection
                WHERE Game = @Game
                  AND RecordType = @RecordType
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY ModKey_FileName COLLATE NOCASE, Component_Index;
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

    /// <inheritdoc />
    public void DeleteByRecord(SupportedGame game, ModKeyDTO modKey, string recordType, FormKeyDTO formKey)
    {
        Database.Execute(
            """
            DELETE FROM Reflection
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

    /// <summary>
    /// Maps a database row to the public reflection DTO shape used by import read-back and comparison.
    /// </summary>
    /// <param name="row">The database row returned by NPoco.</param>
    /// <returns>The equivalent reflection DTO.</returns>
    private static ReflectionDTO ToDTO(ReflectionRow row)
    {
        return new ReflectionDTO
        {
            Game = Enum.Parse<SupportedGame>(row.Game),
            ModKey = new ModKeyDTO
            {
                Name = row.ModKeyName,
                Type = row.ModKeyType,
                FileName = row.ModKeyFileName
            },
            RecordType = row.RecordType,
            FormKey = new FormKeyDTO
            {
                ModKey = new ModKeyDTO
                {
                    Name = row.FormKeyModKeyName,
                    Type = row.FormKeyModKeyType,
                    FileName = row.FormKeyModKeyFileName
                },
                Id = (uint)row.FormKeyId
            },
            ComponentIndex = row.ComponentIndex,
            ComponentType = row.ComponentType,
            SourcePath = row.SourcePath,
            REFL = row.REFL,
            ImportedAtUTC = row.ImportedAtUTC
        };
    }

    private sealed class ReflectionRow
    {
        /// <summary>
        /// Gets or sets the game name stored for the reflection row.
        /// </summary>
        public string Game { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the parent plugin mod key name.
        /// </summary>
        public string ModKeyName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the parent plugin mod key type.
        /// </summary>
        public int ModKeyType { get; set; }

        /// <summary>
        /// Gets or sets the parent plugin file name.
        /// </summary>
        public string ModKeyFileName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the parent Bethesda record type identifier.
        /// </summary>
        public string RecordType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the parent form key mod name.
        /// </summary>
        public string FormKeyModKeyName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the parent form key mod type.
        /// </summary>
        public int FormKeyModKeyType { get; set; }

        /// <summary>
        /// Gets or sets the parent form key plugin file name.
        /// </summary>
        public string FormKeyModKeyFileName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the parent form key identifier.
        /// </summary>
        public long FormKeyId { get; set; }

        /// <summary>
        /// Gets or sets the component index containing the reflection field.
        /// </summary>
        public int ComponentIndex { get; set; }

        /// <summary>
        /// Gets or sets the Spriggit component type name.
        /// </summary>
        public string ComponentType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Spriggit source path.
        /// </summary>
        public string SourcePath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the stored hexadecimal reflection payload.
        /// </summary>
        public string? REFL { get; set; }

        /// <summary>
        /// Gets or sets when the row was imported.
        /// </summary>
        public DateTime ImportedAtUTC { get; set; }
    }
}
