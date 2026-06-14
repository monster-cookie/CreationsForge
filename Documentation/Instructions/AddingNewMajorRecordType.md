# Adding Major Record Types

This document describes the human process for adding a new Bethesda major record type to Creations Forge.

The goal is to make new record support complete, consistent, and boring in the best possible way. A new record type should not be mapped for only one game unless that is an intentional, documented limitation. Starfield, Fallout 4, and Skyrim support must be handled together or explicitly called out as unsupported.

Use GameSetting (GMST), Global (GLOB), and ActorValueInformation (AVIF) as reference patterns.

## Scope

A normal typed major record usually requires changes in these areas:

* Record type catalog
* Core DTO
* Plugin record set DTO
* Database migration
* Repository interface
* Repository implementation
* Importer
* Game-specific record reader mapping for Starfield
* Game-specific record reader mapping for Fallout 4
* Game-specific record reader mapping for Skyrim
* Record import service dispatch
* Record comparison service
* Unit tests
* Database documentation

A new record is not considered done until it imports, persists, appears in the record tree, compares correctly, and has been mapped or intentionally excluded for each supported game.

## Non-negotiable rules

Use named SQL parameters everywhere.

Do not use NPoco positional placeholders like `@0`, `@1`, `@2`, through `@9`.

Good:

```csharp
Database.Execute(
    """
    DELETE FROM SomeRecords
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
```

Bad:

```csharp
Database.Execute(
    """
    DELETE FROM SomeRecords
    WHERE Game = @0
      AND ModKey_Name = @1
      AND ModKey_Type = @2
      AND ModKey_FileName = @3
      AND ImportedAtUTC <> @4;
    """,
    game.ToString(),
    modKey.Name,
    modKey.Type,
    modKey.FileName,
    importedAtUTC);
```

Map every supported game.

When adding a record type, update all three game reader services unless the record truly does not exist for that game:

```text
CreationsForge.Starfield/StarfieldRecordReaderService.cs
CreationsForge.Fallout4/Fallout4RecordReaderService.cs
CreationsForge.Skyrim/SkyrimRecordReaderService.cs
```

If support is intentionally omitted for a game, document why and make sure the import service treats it as optional or unsupported in a controlled way.

Do not use C# primary constructors for classes. Use explicit constructors.

Use one class per file.

Keep Core game-agnostic. Direct game-specific Mutagen calls belong in the game-specific projects.

Do not add repository or migration execution unit tests. Test importers, services, DTO behavior, comparison behavior, and mapping where practical.

When the schema changes, update the database documentation:

```text
Documentation/Database/DATABASE.md
Documentation/Database/ERD.md
```

## Reference examples

### GameSetting (GMST)

GameSetting is the simplest example.

It has:

* A catalog entry for `GMST`
* A `GameSettingDTO`
* A `GameSettings` database table
* A `GameSettingRepository`
* A `GameSettingImporter`
* Reader mappings in Starfield, Fallout 4, and Skyrim
* Import service dispatch
* Comparison service support

GameSetting stores scalar values:

```csharp
public class GameSettingDTO : RecordDTO
{
    public string? SettingType { get; set; }

    public string? Data { get; set; }

    public double? NumericData { get; set; }

    public int? IntegerData { get; set; }

    public bool? BooleanData { get; set; }
}
```

Use GMST when you need a simple scalar record example.

Do not blindly copy the older repository style unless needed. Newer records should usually follow the AVIF repository pattern.

### Global (GLOB)

Global is another small scalar record.

It has:

* A catalog entry for `GLOB`
* A `GlobalDTO`
* A `Globals` database table
* A `GlobalRepository`
* A `GlobalImporter`
* Reader mappings in Starfield, Fallout 4, and Skyrim
* Import service dispatch
* Comparison service support

Global stores a numeric value and can support scripting adapters:

```csharp
public class GlobalDTO : RecordDTO, IHasScriptingAdaptersRecordDTO
{
    public double? Data { get; set; }

    public IList<ScriptingAdapterDTO> ScriptingAdapters { get; set; } = new List<ScriptingAdapterDTO>();
}
```

Use GLOB when you need an example of a simple typed record that can also import child data.

### ActorValueInformation (AVIF)

ActorValueInformation is the best current template for new typed records.

It has:

* A catalog entry for `AVIF`
* An `ActorValueInformationDTO`
* An `ActorValueInformation` database table
* An `IActorValueInformationRepository`
* An `ActorValueInformationRepository` using `TypedRecordRepositoryBase`
* An `ActorValueInformationImporter`
* Reader mappings in Starfield, Fallout 4, and Skyrim
* Import service dispatch
* Comparison service support
* Supported-games importer test coverage

AVIF stores several typed fields and supports scripting adapters:

```csharp
public class ActorValueInformationDTO : RecordDTO, IHasScriptingAdaptersRecordDTO
{
    public string? Name { get; set; }

    public string? Abbreviation { get; set; }

    public string? ContextNotes { get; set; }

    public double? DefaultValue { get; set; }

    public string? Flags { get; set; }

    public string? Type { get; set; }

    public double? Min { get; set; }

    public double? Max { get; set; }

    public IList<ScriptingAdapterDTO> ScriptingAdapters { get; set; } = new List<ScriptingAdapterDTO>();
}
```

Use AVIF as the preferred model for new records.

## Recommended implementation order

### 1. Research the record shape

Before writing code, inspect the record in Mutagen and Spriggit output for all three supported games:

```text
C:\StarfieldExtractions\Spriggit\Starfield.esm
C:\FalloutExtractions\Spriggit\Fallout4.esm
C:\SkyrimExtractions\Spriggit\Skyrim.esm
```

Answer these questions before creating the DTO:

* What is the four-character major record ID?
* What is the Mutagen collection name in Starfield?
* What is the Mutagen collection name in Fallout 4?
* What is the Mutagen collection name in Skyrim?
* Are the field names the same across games?
* Are the field types the same across games?
* Which fields are safe to import now?
* Which fields should be deferred?
* Does the record have models?
* Does the record have keywords?
* Does the record have sounds?
* Does the record have scripting adapters?
* Does the record contain opaque or hard-to-parse payloads that should be stored as raw payloads?

Create a mini mapping table before coding:

```markdown
| Game | Mutagen collection | Direct properties | Reflection helpers needed | Supported now |
| ---- | ------------------ | ----------------- | ------------------------- | ------------- |
| Starfield | mod.SomeRecords | Yes | No | Yes |
| Fallout 4 | GetRecordCollection(mod, "SomeRecords", "AlternateName") | Partial | Yes | Yes |
| Skyrim | GetRecordCollection(mod, "SomeRecords", "AlternateName") | Partial | Yes | Yes |
```

This table prevents the common failure where Starfield is implemented and Fallout 4 or Skyrim quietly vanish into the marsh.

### 2. Add the RecordTypeCatalog entry

File:

```text
CreationsForge.Core/Helpers/RecordTypeCatalog.cs
```

Add an alphabetized entry:

```csharp
public static readonly RecordTypeData SomeRecord = new()
{
    TableName = "SomeRecords",
    RecordType = "SomeRecord",
    RecordID = "XXXX"
};
```

Use the four-character Bethesda major record ID for `RecordID`.

Examples:

```csharp
public static readonly RecordTypeData GameSetting = new()
{
    TableName = "GameSettings",
    RecordType = "GameSetting",
    RecordID = "GMST"
};

public static readonly RecordTypeData Global = new()
{
    TableName = "Globals",
    RecordType = "Global",
    RecordID = "GLOB"
};

public static readonly RecordTypeData ActorValueInformation = new()
{
    TableName = "ActorValueInformation",
    RecordType = "ActorValueInformation",
    RecordID = "AVIF"
};
```

Do not use display names here. This catalog is used by import dispatch, repositories, comparison, child import services, and tests.

### 3. Add the DTO

Folder:

```text
CreationsForge.Core/DTOs/Records/
```

Create:

```text
SomeRecordDTO.cs
```

Start with:

```csharp
namespace CreationsForge.Core.DTOs.Records;

public class SomeRecordDTO : RecordDTO
{
}
```

Add only fields that are useful, stable, and safe to compare.

Do not mirror the entire Mutagen object. The DTO should represent the subset Creations Forge actually imports and compares.

If the record has child data, implement the matching child interfaces:

```csharp
IHasModelsRecordDTO
IHasKeywordsRecordDTO
IHasSoundsRecordDTO
IHasRawRecordPayloadsRecordDTO
IHasScriptingAdaptersRecordDTO
```

Example with child data:

```csharp
using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.DTOs.Records;

public class SomeRecordDTO : RecordDTO, IHasKeywordsRecordDTO, IHasScriptingAdaptersRecordDTO
{
    public string? Name { get; set; }

    public IList<RecordKeywordDTO> Keywords { get; set; } = new List<RecordKeywordDTO>();

    public IList<ScriptingAdapterDTO> ScriptingAdapters { get; set; } = new List<ScriptingAdapterDTO>();
}
```

The shared `RecordChildImportService` will detect these interfaces and import child rows automatically when the importer calls:

```csharp
RecordChildImportService.ReplaceRecordChildren(record, RecordTypeCatalog.SomeRecord.RecordID);
```

### 4. Add the DTO collection to PluginRecordSetDTO

File:

```text
CreationsForge.Core/DTOs/Records/PluginRecordSetDTO.cs
```

Add:

```csharp
public IReadOnlyList<SomeRecordDTO> SomeRecords { get; set; } = [];
```

This is the handoff object between game-specific readers and the core import service.

If this is missing, the reader can map the records but the import service will never see them.

### 5. Add the database migration

Folder:

```text
CreationsForge.Migrations/Sql/
```

Current database work should go into the current unreleased migration.

At the time this instruction was written, the intended migration file was documented as:

```text
CreationsForge.Migrations\Sql\003_Migrations003.sql
```

If that file does not exist locally, create it only if this is still the current agreed migration. Otherwise use the current unreleased migration file.

The table should follow the standard typed record table pattern:

```sql
CREATE TABLE SomeRecords
(
    Game                    TEXT    NOT NULL,
    ModKey_Name             TEXT    NOT NULL,
    ModKey_Type             INTEGER NOT NULL,
    ModKey_FileName         TEXT    NOT NULL,
    FormKey_ModKey_Name     TEXT    NOT NULL,
    FormKey_ModKey_Type     INTEGER NOT NULL,
    FormKey_ModKey_FileName TEXT    NOT NULL,
    FormKey_ID              INTEGER NOT NULL,
    EditorID                TEXT    NOT NULL,
    FormVersion             INTEGER NOT NULL,
    MajorRecordFlags        INTEGER NOT NULL,
    ImportedAtUTC           TEXT    NOT NULL,

    -- Typed fields go here.
    Name                    TEXT    NULL,
    Value                   REAL    NULL,

    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (Game, ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES RecordInstances (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);

CREATE INDEX IX_SomeRecords_FormKey ON SomeRecords (Game, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);
```

Use this naming style:

```text
Table: SomeRecords
Index: IX_SomeRecords_FormKey
```

When adding nullable linked form keys, use the existing four-column pattern:

```text
SomeReference_ModKey_Name
SomeReference_ModKey_Type
SomeReference_ModKey_FileName
SomeReference_FormKey_ID
```

Add a check constraint that either all four linked form key columns are null or all four are populated when that relationship needs integrity.

### 6. Update database documentation

Required files:

```text
Documentation/Database/DATABASE.md
Documentation/Database/ERD.md
```

Update these whenever the migration changes:

* New table
* New column
* Removed column
* Renamed column
* Changed type
* Changed nullability
* Changed default
* Changed constraint
* Changed index
* Changed foreign key behavior

The docs should describe the final migrated schema, not just the first migration where a table was created.

Do not document inferred relationships as real SQLite foreign keys. Only declared SQLite foreign keys belong in the ERD relationship lines.

### 7. Add the repository interface

Folder:

```text
CreationsForge.Core/Repositories/Interfaces/
```

Create:

```text
ISomeRecordRepository.cs
```

Preferred shape:

```csharp
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.Repositories.Interfaces;

public interface ISomeRecordRepository : IRecordTreeRepository
{
    void Save(SomeRecordDTO dto);

    IReadOnlyList<SomeRecordDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey);

    void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC);
}
```

Use this AVIF-style pattern for new records.

### 8. Add the repository

Folder:

```text
CreationsForge.Core/Repositories/
```

Create:

```text
SomeRecordRepository.cs
```

Prefer extending `TypedRecordRepositoryBase`.

Skeleton:

```csharp
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

public class SomeRecordRepository : TypedRecordRepositoryBase, ISomeRecordRepository
{
    public SomeRecordRepository(IDatabase database, IRecordInstanceRepository recordInstanceRepository)
        : base(database, recordInstanceRepository)
    { }

    public override string RecordType => RecordTypeCatalog.SomeRecord.RecordID;

    protected override string TableName => RecordTypeCatalog.SomeRecord.TableName;

    public IReadOnlyList<SomeRecordDTO> GetByFormKey(CreationsForge.Core.Enums.SupportedGame game, CreationsForge.Core.DTOs.Plugins.FormKeyDTO formKey)
    {
        return FetchByFormKey<SomeRecordRow>(
                game,
                formKey,
                [
                    SelectColumn("Name"),
                    SelectColumn("Value")
                ])
            .Select(record => ToDTO(record, game))
            .ToList();
    }

    public void Save(SomeRecordDTO dto)
    {
        SaveRecordInstance(dto);
        Database.Execute(
            """
            INSERT OR REPLACE INTO SomeRecords (
                Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                EditorID, FormVersion, MajorRecordFlags, ImportedAtUTC, Name, Value)
            VALUES (
                @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                @EditorId, @FormVersion, @MajorRecordFlags, @ImportedAtUTC, @Name, @Value);
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
                dto.Name,
                dto.Value
            });
    }

    private static SomeRecordDTO ToDTO(SomeRecordRow record, CreationsForge.Core.Enums.SupportedGame game)
    {
        var dto = new SomeRecordDTO
        {
            Game = game,
            ModKey = new CreationsForge.Core.DTOs.Plugins.ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty },
            FormKey = new CreationsForge.Core.DTOs.Plugins.FormKeyDTO { ModKey = new CreationsForge.Core.DTOs.Plugins.ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty }, Id = 0 },
            EditorID = string.Empty,
            FormVersion = 0,
            MajorRecordFlags = 0,
            ImportedAtUTC = record.ImportedAtUTC,
            Name = record.Name,
            Value = record.Value
        };
        ApplyCommonFields(dto, record, game);
        return dto;
    }

    private sealed class SomeRecordRow : RecordRow
    {
        public string? Name { get; set; }

        public double? Value { get; set; }
    }
}
```

Also update:

```text
CreationsForge.Core/Repositories/TypedRecordRepositoryBase.cs
```

Add the new table name to `AllowedTableNames`:

```csharp
RecordTypeCatalog.SomeRecord.TableName,
```

This is required because `TypedRecordRepositoryBase` validates table names before injecting them into SQL. If the table name is missing, repository reads and stale cleanup will fail.

### 9. Add the importer

Folder:

```text
CreationsForge.Core/Importers/
```

Create:

```text
SomeRecordImporter.cs
```

Skeleton:

```csharp
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Importers;

public class SomeRecordImporter : ITypedRecordImporter
{
    private readonly ISomeRecordRepository SomeRecordRepository;
    private readonly IRecordChildImportService RecordChildImportService;

    public SomeRecordImporter(
        ISomeRecordRepository someRecordRepository,
        IRecordChildImportService recordChildImportService)
    {
        SomeRecordRepository = someRecordRepository;
        RecordChildImportService = recordChildImportService;
    }

    public string RecordType => RecordTypeCatalog.SomeRecord.RecordID;

    public string TableName => RecordTypeCatalog.SomeRecord.TableName;

    public IReadOnlySet<SupportedGame> SupportedGames { get; } = new HashSet<SupportedGame>
    {
        SupportedGame.Starfield,
        SupportedGame.Fallout4,
        SupportedGame.Skyrim
    };

    public void Import(object recordDTO, RecordTypeImportResultDTO result, DateTime importedAtUTC)
    {
        if (recordDTO is not SomeRecordDTO someRecord)
        {
            throw new ArgumentException($"Expected {nameof(SomeRecordDTO)}.", nameof(recordDTO));
        }

        someRecord.ImportedAtUTC = importedAtUTC;
        SomeRecordRepository.Save(someRecord);
        RecordChildImportService.ReplaceRecordChildren(someRecord, RecordTypeCatalog.SomeRecord.RecordID);
        result.DetailRowsImported++;
    }

    public void DeleteStaleRecords(PluginDTO plugin, DateTime importedAtUTC)
    {
        SomeRecordRepository.DeleteStaleByPlugin(plugin.Game, plugin.ModKey, importedAtUTC);
    }
}
```

Do not skip `RecordChildImportService.ReplaceRecordChildren`. It is safe even when the DTO has no child interfaces, and it keeps the importer pattern consistent.

### 10. Add Starfield reader mapping

File:

```text
CreationsForge.Starfield/StarfieldRecordReaderService.cs
```

In `ReadPluginRecords`, add:

```csharp
var someRecords = MapSomeRecords(plugin, mod);
cancellationToken.ThrowIfCancellationRequested();
```

Then return it:

```csharp
return new PluginRecordSetDTO
{
    SomeRecords = someRecords
};
```

Add the mapper:

```csharp
private static IReadOnlyList<SomeRecordDTO> MapSomeRecords(PluginDTO plugin, IStarfieldModGetter mod)
{
    return mod.SomeRecords
        .Select(record => new SomeRecordDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = plugin.ModKey,
            FormKey = MapFormKey(record.FormKey),
            EditorID = record.EditorID ?? string.Empty,
            FormVersion = record.FormVersion,
            MajorRecordFlags = (int)record.StarfieldMajorRecordFlags,
            ImportedAtUTC = DateTime.UtcNow,
            Name = record.Name?.Lookup(Language.English),
            Value = record.Value
        })
        .ToList();
}
```

Use the actual Mutagen collection name and fields.

Starfield often allows direct strongly typed access, but verify the collection and property names instead of guessing.

### 11. Add Fallout 4 reader mapping

File:

```text
CreationsForge.Fallout4/Fallout4RecordReaderService.cs
```

In `ReadPluginRecords`, add:

```csharp
var someRecords = MapSomeRecords(plugin, mod);
cancellationToken.ThrowIfCancellationRequested();
```

Return it through `PluginRecordSetDTO`.

Mapper pattern:

```csharp
private static IReadOnlyList<SomeRecordDTO> MapSomeRecords(PluginDTO plugin, IFallout4ModGetter mod)
{
    return GetRecordCollection(mod, "SomeRecords", "AlternateCollectionName")
        .Select(record => new SomeRecordDTO
        {
            Game = SupportedGame.Fallout4,
            ModKey = plugin.ModKey,
            FormKey = GetRequiredFormKey(record),
            EditorID = GetPropertyString(record, "EditorID"),
            FormVersion = GetPropertyInt(record, "FormVersion"),
            MajorRecordFlags = GetPropertyInt(record, "Fallout4MajorRecordFlags"),
            ImportedAtUTC = DateTime.UtcNow,
            Name = GetLocalizedEnglishText(record, "Name"),
            Value = GetPropertyNullableDouble(record, "Value")
        })
        .ToList();
}
```

Use reflection helpers when field or collection names differ across Mutagen versions or games.

Do not assume the Starfield property names are valid for Fallout 4.

### 12. Add Skyrim reader mapping

File:

```text
CreationsForge.Skyrim/SkyrimRecordReaderService.cs
```

In `ReadPluginRecords`, add:

```csharp
var someRecords = MapSomeRecords(plugin, mod);
cancellationToken.ThrowIfCancellationRequested();
```

Return it through `PluginRecordSetDTO`.

Mapper pattern:

```csharp
private static IReadOnlyList<SomeRecordDTO> MapSomeRecords(PluginDTO plugin, ISkyrimModGetter mod)
{
    return GetRecordCollection(mod, "SomeRecords", "AlternateCollectionName")
        .Select(record => new SomeRecordDTO
        {
            Game = SupportedGame.Skyrim,
            ModKey = plugin.ModKey,
            FormKey = GetRequiredFormKey(record),
            EditorID = GetPropertyString(record, "EditorID"),
            FormVersion = GetPropertyInt(record, "FormVersion"),
            MajorRecordFlags = GetPropertyInt(record, "SkyrimMajorRecordFlags"),
            ImportedAtUTC = DateTime.UtcNow,
            Name = GetLocalizedEnglishText(record, "Name"),
            Value = GetPropertyNullableDouble(record, "Value")
        })
        .ToList();
}
```

Do not copy the Fallout 4 mapper without checking Skyrim property names.

### 13. Add import dispatch

File:

```text
CreationsForge.Core/Services/RecordImportService.cs
```

Add one dispatch call after related records:

```csharp
ImportPluginRecordType(plugin, result, RecordTypeCatalog.SomeRecord, recordSet.SomeRecords, progress, pluginIndex, pluginCount, cancellationToken);
```

For records that are not guaranteed to be supported in every game yet, prefer:

```csharp
ImportOptionalPluginRecordType(plugin, result, RecordTypeCatalog.SomeRecord, recordSet.SomeRecords, progress, pluginIndex, pluginCount, cancellationToken);
```

Use `ImportPluginRecordType` when the record is a normal supported record type and should appear in import results even when there are zero records.

Use `ImportOptionalPluginRecordType` when the record is game-specific, experimental, or intentionally absent for some games.

Current required-style examples include GMST, GLOB, and AVIF.

Current optional-style examples include STAT, CONT, BOOK, DOOR, and TERM.

### 14. Add comparison support

File:

```text
CreationsForge.Core/Services/RecordComparisonService.cs
```

Add the repository field:

```csharp
private readonly ISomeRecordRepository SomeRecordRepository;
```

Add it to the constructor.

Add a branch in `GetRecordComparison`:

```csharp
if (recordType == RecordTypeCatalog.SomeRecord.RecordID)
{
    return CreateSomeRecordComparison(game, formKey);
}
```

Add the comparison method:

```csharp
private RecordComparisonDTO CreateSomeRecordComparison(SupportedGame game, FormKeyDTO formKey)
{
    var records = SomeRecordRepository.GetByFormKey(game, formKey);
    var fields = CreateCommonFields(records.Cast<RecordDTO>().ToList());
    fields.Add(CreateField("Name", records, record => record.Name ?? string.Empty));
    fields.Add(CreateField("Value", records, record => record.Value?.ToString() ?? string.Empty));

    return CreateComparison(RecordTypeCatalog.SomeRecord.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
}
```

For child records, also add the relevant child groups:

```csharp
AddKeywordGroup(fields, records.Cast<RecordDTO>().ToList(), RecordKeywordRepository.GetByFormKey(game, RecordTypeCatalog.SomeRecord.RecordID, formKey));
AddModelGroups(fields, records.Cast<RecordDTO>().ToList(), ModelRepository.GetByFormKey(game, RecordTypeCatalog.SomeRecord.RecordID, formKey));
AddSoundGroups(fields, records.Cast<RecordDTO>().ToList(), RecordSoundRepository.GetByFormKey(game, RecordTypeCatalog.SomeRecord.RecordID, formKey));
AddScriptingAdapterGroups(fields, records.Cast<RecordDTO>().ToList(), ScriptingAdapterRepository.GetByFormKey(game, RecordTypeCatalog.SomeRecord.RecordID, formKey));
```

Only add groups that are actually used by the DTO and importer.

### 15. Check dependency injection

Most normal records do not require manual DI registration.

`CoreModule` automatically registers core types ending in:

```text
Importer
Service
Initializer
Repository
```

as implemented interfaces.

Game modules already register game-specific reader services and keyed game readers.

Only update game modules for special support such as model-only record import/tree support.

### 16. Add importer tests

Folder:

```text
CreationsForge.UnitTests/Importers/
```

Create:

```text
SomeRecordImporterTests.cs
```

Test:

* `RecordType`
* `TableName`
* `SupportedGames`
* Repository `Save` was called
* Child import service was called
* `ImportedAtUTC` was assigned
* `DetailRowsImported` was incremented
* Wrong DTO type throws `ArgumentException`, if desired
* `DeleteStaleRecords` calls the repository, if desired

Base the shape on `GameSettingImporterTests`.

### 17. Update supported-games importer tests

File:

```text
CreationsForge.UnitTests/Importers/TypedRecordImporterSupportedGamesTests.cs
```

If the importer supports all three current games, add it to `ExpandedRecordImporters`.

Example:

```csharp
yield return [new SomeRecordImporter(
    Mock.Of<ISomeRecordRepository>(),
    Mock.Of<IRecordChildImportService>())];
```

This protects against accidentally adding a cross-game record importer that only claims one or two games.

### 18. Update RecordImportService tests

File:

```text
CreationsForge.UnitTests/Services/RecordImportServiceTests.cs
```

Update the test reader to carry the new DTO list.

Add:

* Private field for `IReadOnlyList<SomeRecordDTO>`
* Constructor parameter
* Default empty list
* Assignment to `PluginRecordSetDTO.SomeRecords`

Add a helper:

```csharp
private static SomeRecordDTO CreateSomeRecord(PluginDTO plugin, uint id)
{
    return new SomeRecordDTO
    {
        Game = plugin.Game,
        ModKey = plugin.ModKey,
        FormKey = CreateFormKey(plugin.ModKey, id),
        EditorID = $"XXXX{id}",
        FormVersion = 1,
        MajorRecordFlags = 0,
        ImportedAtUTC = default
    };
}
```

Update import dispatch tests:

* Add a test record
* Add a test importer
* Assert the record type appears in the expected import order
* Assert header count
* Assert detail row count
* Assert stale cleanup
* Assert imported record list

This is one of the best defenses against the record being mapped but never imported.

### 19. Update RecordComparisonService tests

File:

```text
CreationsForge.UnitTests/Services/RecordComparisonServiceTests.cs
```

Add or update tests that verify the comparison service returns expected fields for the new record type.

At minimum, verify:

* The new `RecordTypeCatalog` branch is used
* Common fields are included
* Typed fields are included
* Child groups appear when applicable

### 20. Add reader mapping tests when practical

If existing reader tests cover GMST, GLOB, or AVIF patterns, add equivalent coverage for the new record.

Use these files as reference locations:

```text
CreationsForge.UnitTests/Services/StarfieldRecordReaderTests.cs
CreationsForge.UnitTests/Services/Fallout4RecordReaderTests.cs
CreationsForge.UnitTests/Services/SkyrimRecordReaderTests.cs
```

The key goal is not perfect Mutagen testing. The key goal is catching the repeated mistake where one game reader is updated and the others are forgotten.

### 21. Validate cross-game mapping before calling the work complete

Use this checklist for every new record:

```markdown
| Requirement | Done |
| ----------- | ---- |
| RecordTypeCatalog entry added |  |
| DTO added |  |
| PluginRecordSetDTO property added |  |
| Migration table added |  |
| FormKey index added |  |
| Repository interface added |  |
| Repository implementation added |  |
| New table added to TypedRecordRepositoryBase.AllowedTableNames |  |
| Importer added |  |
| Starfield reader maps the record |  |
| Fallout 4 reader maps the record |  |
| Skyrim reader maps the record |  |
| RecordImportService imports the record |  |
| RecordComparisonService compares the record |  |
| Importer tests added |  |
| RecordImportService tests updated |  |
| RecordComparisonService tests updated |  |
| Supported-games test updated |  |
| DATABASE.md updated |  |
| ERD.md updated |  |
| SQL uses named parameters only |  |
| No positional @0 through @9 SQL placeholders |  |
```

### 22. Search checks before review

Run these searches before review.

Search for the record name:

```powershell
rg "SomeRecord|SomeRecords|XXXX" .
```

Search for positional SQL parameters:

```powershell
rg "@[0-9]" CreationsForge.Core CreationsForge.Migrations CreationsForge.Starfield CreationsForge.Fallout4 CreationsForge.Skyrim
```

Any `@0`, `@1`, `@2`, etc. in runtime SQL should be replaced with named parameters.

Search for incomplete game mapping:

```powershell
rg "MapSomeRecords|SomeRecords" CreationsForge.Starfield CreationsForge.Fallout4 CreationsForge.Skyrim
```

All three game projects should show mapping work unless there is a documented reason not to support one.

Search for import dispatch:

```powershell
rg "RecordTypeCatalog\.SomeRecord|recordSet\.SomeRecords" CreationsForge.Core/Services
```

Search for repository allow-list registration:

```powershell
rg "SomeRecords|RecordTypeCatalog\.SomeRecord" CreationsForge.Core/Repositories/TypedRecordRepositoryBase.cs
```

### 23. Build and test

Run:

```powershell
dotnet build
dotnet test
```

If the record reader depends on installed game files, also run a real import smoke test for each game that is available locally.

The minimum intended validation is:

```powershell
dotnet run --project .\CreationsForge.Console\CreationsForge.Console.csproj -- -game Starfield
dotnet run --project .\CreationsForge.Console\CreationsForge.Console.csproj -- -game Fallout4
dotnet run --project .\CreationsForge.Console\CreationsForge.Console.csproj -- -game Skyrim
```

Adjust arguments if the console harness changes.

After import, inspect the SQLite database and confirm:

* Rows exist in `RecordInstances` for the new record type
* Rows exist in the typed table
* Child tables have rows when applicable
* Stale rows are removed after reimport
* The record appears in the tree
* The comparison view shows the typed fields

### 24. When a record is not supported by every game

Sometimes a record truly is not available for every supported game.

In that case:

1. Document the unsupported game and reason.
2. Do not fake empty support in the reader.
3. Keep the importer `SupportedGames` accurate.
4. Use `ImportOptionalPluginRecordType` if missing support should not create noisy unsupported results.
5. Add test coverage proving the unsupported game is handled intentionally.

Example note:

```markdown
Support notes:

| Game | Supported | Reason |
| ---- | --------- | ------ |
| Starfield | Yes | Mutagen exposes mod.SomeRecords |
| Fallout 4 | No | Record type does not exist in Fallout 4 |
| Skyrim | Yes | Mutagen exposes the collection as AlternateSomeRecords |
```

Do not leave this implicit. Future humans should not need a lantern and a séance to understand why one game is missing.

## Preferred pattern summary

For new normal typed records, prefer the AVIF pattern:

```text
DTO inherits RecordDTO
Repository extends TypedRecordRepositoryBase
Repository uses named SQL parameters
Importer implements ITypedRecordImporter
Importer saves typed record and replaces children
Reader maps all three games
RecordImportService imports the PluginRecordSetDTO list
RecordComparisonService compares typed fields
Database docs are updated with schema changes
```

Use GMST only as a simple scalar reference.

Use GLOB as a simple scalar plus child-data reference.

Use AVIF as the primary implementation reference.

## Common failure points

### Starfield mapped, Fallout 4 and Skyrim forgotten

Symptom:

* Starfield import works
* Fallout 4 and Skyrim silently do not import the new record
* RecordImportService tests may not catch it unless updated

Prevention:

* Fill out the cross-game mapping table before coding
* Update all three reader services in the same change
* Add or update tests for all three game readers when practical
* Search all three game projects before review

### DTO exists but PluginRecordSetDTO is missing the list

Symptom:

* Reader can map the record locally
* Import service never receives the records

Prevention:

* Add `IReadOnlyList<SomeRecordDTO> SomeRecords { get; set; } = [];`
* Update test readers in unit tests

### Importer exists but RecordImportService does not dispatch it

Symptom:

* Importer is registered in DI
* Records are read
* Nothing is imported

Prevention:

* Add `ImportPluginRecordType` or `ImportOptionalPluginRecordType`
* Update `RecordImportServiceTests`

### Repository extends TypedRecordRepositoryBase but table is not allow-listed

Symptom:

* Runtime failure when fetching tree entries, fetching by form key, or deleting stale records

Prevention:

* Add `RecordTypeCatalog.SomeRecord.TableName` to `AllowedTableNames`

### SQL uses positional parameters

Symptom:

* Runtime SQL works today but violates project conventions
* Future edits are harder to maintain
* Parameter ordering bugs become more likely

Prevention:

* Use named placeholders like `@Game`, `@ModKeyName`, and `@ImportedAtUTC`
* Pass anonymous objects or typed parameter objects
* Run `rg "@[0-9]"` before review

### Comparison support is missing

Symptom:

* Record imports into SQLite
* Record appears in the tree
* Comparison view shows little or no useful typed data

Prevention:

* Add repository dependency to `RecordComparisonService`
* Add record-type branch
* Add `CreateSomeRecordComparison`
* Add child comparison groups when needed

### Database docs are stale

Symptom:

* Migration and code are correct
* Documentation lies with great confidence

Prevention:

* Update `Documentation/Database/DATABASE.md`
* Update `Documentation/Database/ERD.md`
* Keep docs aligned to the final migrated schema

## Final reviewer checklist

Before merging a new major record type, confirm:

* The record imports for every intended game.
* Unsupported games are explicitly documented.
* All runtime SQL uses named parameters.
* There are no `@0` through `@9` positional SQL placeholders in touched runtime SQL.
* The typed table has the standard composite primary key.
* The typed table references `Plugins`.
* The typed table references `RecordInstances`.
* The typed table has a FormKey index.
* The repository saves `RecordInstances`.
* The repository deletes stale typed rows and stale `RecordInstances`.
* The importer calls `RecordChildImportService.ReplaceRecordChildren`.
* The import service dispatches the new record.
* The comparison service displays useful typed fields.
* Tests cover importer behavior.
* Tests cover import dispatch.
* Tests cover comparison behavior.
* Cross-game reader mapping was checked for Starfield, Fallout 4, and Skyrim.
* Database documentation was updated.

When in doubt, copy AVIF first, then simplify. That path has fewer trapdoors.
# Adding Major Record Types

This document describes the human process for adding a new Bethesda major record type to Creations Forge.

The goal is to make new record support complete, consistent, and boring in the best possible way. A new record type should not be mapped for only one game unless that is an intentional, documented limitation. Starfield, Fallout 4, and Skyrim support must be handled together or explicitly called out as unsupported.

Use GameSetting (GMST), Global (GLOB), and ActorValueInformation (AVIF) as reference patterns.

## Scope

A normal typed major record usually requires changes in these areas:

* Record type catalog
* Core DTO
* Plugin record set DTO
* Database migration
* Repository interface
* Repository implementation
* Importer
* Game-specific record reader mapping for Starfield
* Game-specific record reader mapping for Fallout 4
* Game-specific record reader mapping for Skyrim
* Record import service dispatch
* Record comparison service
* Unit tests
* Database documentation

A new record is not considered done until it imports, persists, appears in the record tree, compares correctly, and has been mapped or intentionally excluded for each supported game.

## Non-negotiable rules

Use named SQL parameters everywhere.

Do not use NPoco positional placeholders like `@0`, `@1`, `@2`, through `@9`.

Good:

```csharp
Database.Execute(
    """
    DELETE FROM SomeRecords
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
```

Bad:

```csharp
Database.Execute(
    """
    DELETE FROM SomeRecords
    WHERE Game = @0
      AND ModKey_Name = @1
      AND ModKey_Type = @2
      AND ModKey_FileName = @3
      AND ImportedAtUTC <> @4;
    """,
    game.ToString(),
    modKey.Name,
    modKey.Type,
    modKey.FileName,
    importedAtUTC);
```

Map every supported game.

When adding a record type, update all three game reader services unless the record truly does not exist for that game:

```text
CreationsForge.Starfield/StarfieldRecordReaderService.cs
CreationsForge.Fallout4/Fallout4RecordReaderService.cs
CreationsForge.Skyrim/SkyrimRecordReaderService.cs
```

If support is intentionally omitted for a game, document why and make sure the import service treats it as optional or unsupported in a controlled way.

Do not use C# primary constructors for classes. Use explicit constructors.

Use one class per file.

Keep Core game-agnostic. Direct game-specific Mutagen calls belong in the game-specific projects.

Do not add repository or migration execution unit tests. Test importers, services, DTO behavior, comparison behavior, and mapping where practical.

When the schema changes, update the database documentation:

```text
Documentation/Database/DATABASE.md
Documentation/Database/ERD.md
```

## Reference examples

### GameSetting (GMST)

GameSetting is the simplest example.

It has:

* A catalog entry for `GMST`
* A `GameSettingDTO`
* A `GameSettings` database table
* A `GameSettingRepository`
* A `GameSettingImporter`
* Reader mappings in Starfield, Fallout 4, and Skyrim
* Import service dispatch
* Comparison service support

GameSetting stores scalar values:

```csharp
public class GameSettingDTO : RecordDTO
{
    public string? SettingType { get; set; }

    public string? Data { get; set; }

    public double? NumericData { get; set; }

    public int? IntegerData { get; set; }

    public bool? BooleanData { get; set; }
}
```

Use GMST when you need a simple scalar record example.

Do not blindly copy the older repository style unless needed. Newer records should usually follow the AVIF repository pattern.

### Global (GLOB)

Global is another small scalar record.

It has:

* A catalog entry for `GLOB`
* A `GlobalDTO`
* A `Globals` database table
* A `GlobalRepository`
* A `GlobalImporter`
* Reader mappings in Starfield, Fallout 4, and Skyrim
* Import service dispatch
* Comparison service support

Global stores a numeric value and can support scripting adapters:

```csharp
public class GlobalDTO : RecordDTO, IHasScriptingAdaptersRecordDTO
{
    public double? Data { get; set; }

    public IList<ScriptingAdapterDTO> ScriptingAdapters { get; set; } = new List<ScriptingAdapterDTO>();
}
```

Use GLOB when you need an example of a simple typed record that can also import child data.

### ActorValueInformation (AVIF)

ActorValueInformation is the best current template for new typed records.

It has:

* A catalog entry for `AVIF`
* An `ActorValueInformationDTO`
* An `ActorValueInformation` database table
* An `IActorValueInformationRepository`
* An `ActorValueInformationRepository` using `TypedRecordRepositoryBase`
* An `ActorValueInformationImporter`
* Reader mappings in Starfield, Fallout 4, and Skyrim
* Import service dispatch
* Comparison service support
* Supported-games importer test coverage

AVIF stores several typed fields and supports scripting adapters:

```csharp
public class ActorValueInformationDTO : RecordDTO, IHasScriptingAdaptersRecordDTO
{
    public string? Name { get; set; }

    public string? Abbreviation { get; set; }

    public string? ContextNotes { get; set; }

    public double? DefaultValue { get; set; }

    public string? Flags { get; set; }

    public string? Type { get; set; }

    public double? Min { get; set; }

    public double? Max { get; set; }

    public IList<ScriptingAdapterDTO> ScriptingAdapters { get; set; } = new List<ScriptingAdapterDTO>();
}
```

Use AVIF as the preferred model for new records.

## Recommended implementation order

### 1. Research the record shape

Before writing code, inspect the record in Mutagen and Spriggit output for all three supported games:

```text
C:\StarfieldExtractions\Spriggit\Starfield.esm
C:\FalloutExtractions\Spriggit\Fallout4.esm
C:\SkyrimExtractions\Spriggit\Skyrim.esm
```

Answer these questions before creating the DTO:

* What is the four-character major record ID?
* What is the Mutagen collection name in Starfield?
* What is the Mutagen collection name in Fallout 4?
* What is the Mutagen collection name in Skyrim?
* Are the field names the same across games?
* Are the field types the same across games?
* Which fields are safe to import now?
* Which fields should be deferred?
* Does the record have models?
* Does the record have keywords?
* Does the record have sounds?
* Does the record have scripting adapters?
* Does the record contain opaque or hard-to-parse payloads that should be stored as raw payloads?

Create a mini mapping table before coding:

```markdown
| Game | Mutagen collection | Direct properties | Reflection helpers needed | Supported now |
| ---- | ------------------ | ----------------- | ------------------------- | ------------- |
| Starfield | mod.SomeRecords | Yes | No | Yes |
| Fallout 4 | GetRecordCollection(mod, "SomeRecords", "AlternateName") | Partial | Yes | Yes |
| Skyrim | GetRecordCollection(mod, "SomeRecords", "AlternateName") | Partial | Yes | Yes |
```

This table prevents the common failure where Starfield is implemented and Fallout 4 or Skyrim quietly vanish into the marsh.

### 2. Add the RecordTypeCatalog entry

File:

```text
CreationsForge.Core/Helpers/RecordTypeCatalog.cs
```

Add an alphabetized entry:

```csharp
public static readonly RecordTypeData SomeRecord = new()
{
    TableName = "SomeRecords",
    RecordType = "SomeRecord",
    RecordID = "XXXX"
};
```

Use the four-character Bethesda major record ID for `RecordID`.

Examples:

```csharp
public static readonly RecordTypeData GameSetting = new()
{
    TableName = "GameSettings",
    RecordType = "GameSetting",
    RecordID = "GMST"
};

public static readonly RecordTypeData Global = new()
{
    TableName = "Globals",
    RecordType = "Global",
    RecordID = "GLOB"
};

public static readonly RecordTypeData ActorValueInformation = new()
{
    TableName = "ActorValueInformation",
    RecordType = "ActorValueInformation",
    RecordID = "AVIF"
};
```

Do not use display names here. This catalog is used by import dispatch, repositories, comparison, child import services, and tests.

### 3. Add the DTO

Folder:

```text
CreationsForge.Core/DTOs/Records/
```

Create:

```text
SomeRecordDTO.cs
```

Start with:

```csharp
namespace CreationsForge.Core.DTOs.Records;

public class SomeRecordDTO : RecordDTO
{
}
```

Add only fields that are useful, stable, and safe to compare.

Do not mirror the entire Mutagen object. The DTO should represent the subset Creations Forge actually imports and compares.

If the record has child data, implement the matching child interfaces:

```csharp
IHasModelsRecordDTO
IHasKeywordsRecordDTO
IHasSoundsRecordDTO
IHasRawRecordPayloadsRecordDTO
IHasScriptingAdaptersRecordDTO
```

Example with child data:

```csharp
using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.DTOs.Records;

public class SomeRecordDTO : RecordDTO, IHasKeywordsRecordDTO, IHasScriptingAdaptersRecordDTO
{
    public string? Name { get; set; }

    public IList<RecordKeywordDTO> Keywords { get; set; } = new List<RecordKeywordDTO>();

    public IList<ScriptingAdapterDTO> ScriptingAdapters { get; set; } = new List<ScriptingAdapterDTO>();
}
```

The shared `RecordChildImportService` will detect these interfaces and import child rows automatically when the importer calls:

```csharp
RecordChildImportService.ReplaceRecordChildren(record, RecordTypeCatalog.SomeRecord.RecordID);
```

### 4. Add the DTO collection to PluginRecordSetDTO

File:

```text
CreationsForge.Core/DTOs/Records/PluginRecordSetDTO.cs
```

Add:

```csharp
public IReadOnlyList<SomeRecordDTO> SomeRecords { get; set; } = [];
```

This is the handoff object between game-specific readers and the core import service.

If this is missing, the reader can map the records but the import service will never see them.

### 5. Add the database migration

Folder:

```text
CreationsForge.Migrations/Sql/
```

Current database work should go into the current unreleased migration.

At the time this instruction was written, the intended migration file was documented as:

```text
CreationsForge.Migrations\Sql\003_Migrations003.sql
```

If that file does not exist locally, create it only if this is still the current agreed migration. Otherwise use the current unreleased migration file.

The table should follow the standard typed record table pattern:

```sql
CREATE TABLE SomeRecords
(
    Game                    TEXT    NOT NULL,
    ModKey_Name             TEXT    NOT NULL,
    ModKey_Type             INTEGER NOT NULL,
    ModKey_FileName         TEXT    NOT NULL,
    FormKey_ModKey_Name     TEXT    NOT NULL,
    FormKey_ModKey_Type     INTEGER NOT NULL,
    FormKey_ModKey_FileName TEXT    NOT NULL,
    FormKey_ID              INTEGER NOT NULL,
    EditorID                TEXT    NOT NULL,
    FormVersion             INTEGER NOT NULL,
    MajorRecordFlags        INTEGER NOT NULL,
    ImportedAtUTC           TEXT    NOT NULL,

    -- Typed fields go here.
    Name                    TEXT    NULL,
    Value                   REAL    NULL,

    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (Game, ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES RecordInstances (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);

CREATE INDEX IX_SomeRecords_FormKey ON SomeRecords (Game, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);
```

Use this naming style:

```text
Table: SomeRecords
Index: IX_SomeRecords_FormKey
```

When adding nullable linked form keys, use the existing four-column pattern:

```text
SomeReference_ModKey_Name
SomeReference_ModKey_Type
SomeReference_ModKey_FileName
SomeReference_FormKey_ID
```

Add a check constraint that either all four linked form key columns are null or all four are populated when that relationship needs integrity.

### 6. Update database documentation

Required files:

```text
Documentation/Database/DATABASE.md
Documentation/Database/ERD.md
```

Update these whenever the migration changes:

* New table
* New column
* Removed column
* Renamed column
* Changed type
* Changed nullability
* Changed default
* Changed constraint
* Changed index
* Changed foreign key behavior

The docs should describe the final migrated schema, not just the first migration where a table was created.

Do not document inferred relationships as real SQLite foreign keys. Only declared SQLite foreign keys belong in the ERD relationship lines.

### 7. Add the repository interface

Folder:

```text
CreationsForge.Core/Repositories/Interfaces/
```

Create:

```text
ISomeRecordRepository.cs
```

Preferred shape:

```csharp
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.Repositories.Interfaces;

public interface ISomeRecordRepository : IRecordTreeRepository
{
    void Save(SomeRecordDTO dto);

    IReadOnlyList<SomeRecordDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey);

    void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC);
}
```

Use this AVIF-style pattern for new records.

### 8. Add the repository

Folder:

```text
CreationsForge.Core/Repositories/
```

Create:

```text
SomeRecordRepository.cs
```

Prefer extending `TypedRecordRepositoryBase`.

Skeleton:

```csharp
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

public class SomeRecordRepository : TypedRecordRepositoryBase, ISomeRecordRepository
{
    public SomeRecordRepository(IDatabase database, IRecordInstanceRepository recordInstanceRepository)
        : base(database, recordInstanceRepository)
    { }

    public override string RecordType => RecordTypeCatalog.SomeRecord.RecordID;

    protected override string TableName => RecordTypeCatalog.SomeRecord.TableName;

    public IReadOnlyList<SomeRecordDTO> GetByFormKey(CreationsForge.Core.Enums.SupportedGame game, CreationsForge.Core.DTOs.Plugins.FormKeyDTO formKey)
    {
        return FetchByFormKey<SomeRecordRow>(
                game,
                formKey,
                [
                    SelectColumn("Name"),
                    SelectColumn("Value")
                ])
            .Select(record => ToDTO(record, game))
            .ToList();
    }

    public void Save(SomeRecordDTO dto)
    {
        SaveRecordInstance(dto);
        Database.Execute(
            """
            INSERT OR REPLACE INTO SomeRecords (
                Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                EditorID, FormVersion, MajorRecordFlags, ImportedAtUTC, Name, Value)
            VALUES (
                @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                @EditorId, @FormVersion, @MajorRecordFlags, @ImportedAtUTC, @Name, @Value);
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
                dto.Name,
                dto.Value
            });
    }

    private static SomeRecordDTO ToDTO(SomeRecordRow record, CreationsForge.Core.Enums.SupportedGame game)
    {
        var dto = new SomeRecordDTO
        {
            Game = game,
            ModKey = new CreationsForge.Core.DTOs.Plugins.ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty },
            FormKey = new CreationsForge.Core.DTOs.Plugins.FormKeyDTO { ModKey = new CreationsForge.Core.DTOs.Plugins.ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty }, Id = 0 },
            EditorID = string.Empty,
            FormVersion = 0,
            MajorRecordFlags = 0,
            ImportedAtUTC = record.ImportedAtUTC,
            Name = record.Name,
            Value = record.Value
        };
        ApplyCommonFields(dto, record, game);
        return dto;
    }

    private sealed class SomeRecordRow : RecordRow
    {
        public string? Name { get; set; }

        public double? Value { get; set; }
    }
}
```

Also update:

```text
CreationsForge.Core/Repositories/TypedRecordRepositoryBase.cs
```

Add the new table name to `AllowedTableNames`:

```csharp
RecordTypeCatalog.SomeRecord.TableName,
```

This is required because `TypedRecordRepositoryBase` validates table names before injecting them into SQL. If the table name is missing, repository reads and stale cleanup will fail.

### 9. Add the importer

Folder:

```text
CreationsForge.Core/Importers/
```

Create:

```text
SomeRecordImporter.cs
```

Skeleton:

```csharp
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Importers;

public class SomeRecordImporter : ITypedRecordImporter
{
    private readonly ISomeRecordRepository SomeRecordRepository;
    private readonly IRecordChildImportService RecordChildImportService;

    public SomeRecordImporter(
        ISomeRecordRepository someRecordRepository,
        IRecordChildImportService recordChildImportService)
    {
        SomeRecordRepository = someRecordRepository;
        RecordChildImportService = recordChildImportService;
    }

    public string RecordType => RecordTypeCatalog.SomeRecord.RecordID;

    public string TableName => RecordTypeCatalog.SomeRecord.TableName;

    public IReadOnlySet<SupportedGame> SupportedGames { get; } = new HashSet<SupportedGame>
    {
        SupportedGame.Starfield,
        SupportedGame.Fallout4,
        SupportedGame.Skyrim
    };

    public void Import(object recordDTO, RecordTypeImportResultDTO result, DateTime importedAtUTC)
    {
        if (recordDTO is not SomeRecordDTO someRecord)
        {
            throw new ArgumentException($"Expected {nameof(SomeRecordDTO)}.", nameof(recordDTO));
        }

        someRecord.ImportedAtUTC = importedAtUTC;
        SomeRecordRepository.Save(someRecord);
        RecordChildImportService.ReplaceRecordChildren(someRecord, RecordTypeCatalog.SomeRecord.RecordID);
        result.DetailRowsImported++;
    }

    public void DeleteStaleRecords(PluginDTO plugin, DateTime importedAtUTC)
    {
        SomeRecordRepository.DeleteStaleByPlugin(plugin.Game, plugin.ModKey, importedAtUTC);
    }
}
```

Do not skip `RecordChildImportService.ReplaceRecordChildren`. It is safe even when the DTO has no child interfaces, and it keeps the importer pattern consistent.

### 10. Add Starfield reader mapping

File:

```text
CreationsForge.Starfield/StarfieldRecordReaderService.cs
```

In `ReadPluginRecords`, add:

```csharp
var someRecords = MapSomeRecords(plugin, mod);
cancellationToken.ThrowIfCancellationRequested();
```

Then return it:

```csharp
return new PluginRecordSetDTO
{
    SomeRecords = someRecords
};
```

Add the mapper:

```csharp
private static IReadOnlyList<SomeRecordDTO> MapSomeRecords(PluginDTO plugin, IStarfieldModGetter mod)
{
    return mod.SomeRecords
        .Select(record => new SomeRecordDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = plugin.ModKey,
            FormKey = MapFormKey(record.FormKey),
            EditorID = record.EditorID ?? string.Empty,
            FormVersion = record.FormVersion,
            MajorRecordFlags = (int)record.StarfieldMajorRecordFlags,
            ImportedAtUTC = DateTime.UtcNow,
            Name = record.Name?.Lookup(Language.English),
            Value = record.Value
        })
        .ToList();
}
```

Use the actual Mutagen collection name and fields.

Starfield often allows direct strongly typed access, but verify the collection and property names instead of guessing.

### 11. Add Fallout 4 reader mapping

File:

```text
CreationsForge.Fallout4/Fallout4RecordReaderService.cs
```

In `ReadPluginRecords`, add:

```csharp
var someRecords = MapSomeRecords(plugin, mod);
cancellationToken.ThrowIfCancellationRequested();
```

Return it through `PluginRecordSetDTO`.

Mapper pattern:

```csharp
private static IReadOnlyList<SomeRecordDTO> MapSomeRecords(PluginDTO plugin, IFallout4ModGetter mod)
{
    return GetRecordCollection(mod, "SomeRecords", "AlternateCollectionName")
        .Select(record => new SomeRecordDTO
        {
            Game = SupportedGame.Fallout4,
            ModKey = plugin.ModKey,
            FormKey = GetRequiredFormKey(record),
            EditorID = GetPropertyString(record, "EditorID"),
            FormVersion = GetPropertyInt(record, "FormVersion"),
            MajorRecordFlags = GetPropertyInt(record, "Fallout4MajorRecordFlags"),
            ImportedAtUTC = DateTime.UtcNow,
            Name = GetLocalizedEnglishText(record, "Name"),
            Value = GetPropertyNullableDouble(record, "Value")
        })
        .ToList();
}
```

Use reflection helpers when field or collection names differ across Mutagen versions or games.

Do not assume the Starfield property names are valid for Fallout 4.

### 12. Add Skyrim reader mapping

File:

```text
CreationsForge.Skyrim/SkyrimRecordReaderService.cs
```

In `ReadPluginRecords`, add:

```csharp
var someRecords = MapSomeRecords(plugin, mod);
cancellationToken.ThrowIfCancellationRequested();
```

Return it through `PluginRecordSetDTO`.

Mapper pattern:

```csharp
private static IReadOnlyList<SomeRecordDTO> MapSomeRecords(PluginDTO plugin, ISkyrimModGetter mod)
{
    return GetRecordCollection(mod, "SomeRecords", "AlternateCollectionName")
        .Select(record => new SomeRecordDTO
        {
            Game = SupportedGame.Skyrim,
            ModKey = plugin.ModKey,
            FormKey = GetRequiredFormKey(record),
            EditorID = GetPropertyString(record, "EditorID"),
            FormVersion = GetPropertyInt(record, "FormVersion"),
            MajorRecordFlags = GetPropertyInt(record, "SkyrimMajorRecordFlags"),
            ImportedAtUTC = DateTime.UtcNow,
            Name = GetLocalizedEnglishText(record, "Name"),
            Value = GetPropertyNullableDouble(record, "Value")
        })
        .ToList();
}
```

Do not copy the Fallout 4 mapper without checking Skyrim property names.

### 13. Add import dispatch

File:

```text
CreationsForge.Core/Services/RecordImportService.cs
```

Add one dispatch call after related records:

```csharp
ImportPluginRecordType(plugin, result, RecordTypeCatalog.SomeRecord, recordSet.SomeRecords, progress, pluginIndex, pluginCount, cancellationToken);
```

For records that are not guaranteed to be supported in every game yet, prefer:

```csharp
ImportOptionalPluginRecordType(plugin, result, RecordTypeCatalog.SomeRecord, recordSet.SomeRecords, progress, pluginIndex, pluginCount, cancellationToken);
```

Use `ImportPluginRecordType` when the record is a normal supported record type and should appear in import results even when there are zero records.

Use `ImportOptionalPluginRecordType` when the record is game-specific, experimental, or intentionally absent for some games.

Current required-style examples include GMST, GLOB, and AVIF.

Current optional-style examples include STAT, CONT, BOOK, DOOR, and TERM.

### 14. Add comparison support

File:

```text
CreationsForge.Core/Services/RecordComparisonService.cs
```

Add the repository field:

```csharp
private readonly ISomeRecordRepository SomeRecordRepository;
```

Add it to the constructor.

Add a branch in `GetRecordComparison`:

```csharp
if (recordType == RecordTypeCatalog.SomeRecord.RecordID)
{
    return CreateSomeRecordComparison(game, formKey);
}
```

Add the comparison method:

```csharp
private RecordComparisonDTO CreateSomeRecordComparison(SupportedGame game, FormKeyDTO formKey)
{
    var records = SomeRecordRepository.GetByFormKey(game, formKey);
    var fields = CreateCommonFields(records.Cast<RecordDTO>().ToList());
    fields.Add(CreateField("Name", records, record => record.Name ?? string.Empty));
    fields.Add(CreateField("Value", records, record => record.Value?.ToString() ?? string.Empty));

    return CreateComparison(RecordTypeCatalog.SomeRecord.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
}
```

For child records, also add the relevant child groups:

```csharp
AddKeywordGroup(fields, records.Cast<RecordDTO>().ToList(), RecordKeywordRepository.GetByFormKey(game, RecordTypeCatalog.SomeRecord.RecordID, formKey));
AddModelGroups(fields, records.Cast<RecordDTO>().ToList(), ModelRepository.GetByFormKey(game, RecordTypeCatalog.SomeRecord.RecordID, formKey));
AddSoundGroups(fields, records.Cast<RecordDTO>().ToList(), RecordSoundRepository.GetByFormKey(game, RecordTypeCatalog.SomeRecord.RecordID, formKey));
AddScriptingAdapterGroups(fields, records.Cast<RecordDTO>().ToList(), ScriptingAdapterRepository.GetByFormKey(game, RecordTypeCatalog.SomeRecord.RecordID, formKey));
```

Only add groups that are actually used by the DTO and importer.

### 15. Check dependency injection

Most normal records do not require manual DI registration.

`CoreModule` automatically registers core types ending in:

```text
Importer
Service
Initializer
Repository
```

as implemented interfaces.

Game modules already register game-specific reader services and keyed game readers.

Only update game modules for special support such as model-only record import/tree support.

### 16. Add importer tests

Folder:

```text
CreationsForge.UnitTests/Importers/
```

Create:

```text
SomeRecordImporterTests.cs
```

Test:

* `RecordType`
* `TableName`
* `SupportedGames`
* Repository `Save` was called
* Child import service was called
* `ImportedAtUTC` was assigned
* `DetailRowsImported` was incremented
* Wrong DTO type throws `ArgumentException`, if desired
* `DeleteStaleRecords` calls the repository, if desired

Base the shape on `GameSettingImporterTests`.

### 17. Update supported-games importer tests

File:

```text
CreationsForge.UnitTests/Importers/TypedRecordImporterSupportedGamesTests.cs
```

If the importer supports all three current games, add it to `ExpandedRecordImporters`.

Example:

```csharp
yield return [new SomeRecordImporter(
    Mock.Of<ISomeRecordRepository>(),
    Mock.Of<IRecordChildImportService>())];
```

This protects against accidentally adding a cross-game record importer that only claims one or two games.

### 18. Update RecordImportService tests

File:

```text
CreationsForge.UnitTests/Services/RecordImportServiceTests.cs
```

Update the test reader to carry the new DTO list.

Add:

* Private field for `IReadOnlyList<SomeRecordDTO>`
* Constructor parameter
* Default empty list
* Assignment to `PluginRecordSetDTO.SomeRecords`

Add a helper:

```csharp
private static SomeRecordDTO CreateSomeRecord(PluginDTO plugin, uint id)
{
    return new SomeRecordDTO
    {
        Game = plugin.Game,
        ModKey = plugin.ModKey,
        FormKey = CreateFormKey(plugin.ModKey, id),
        EditorID = $"XXXX{id}",
        FormVersion = 1,
        MajorRecordFlags = 0,
        ImportedAtUTC = default
    };
}
```

Update import dispatch tests:

* Add a test record
* Add a test importer
* Assert the record type appears in the expected import order
* Assert header count
* Assert detail row count
* Assert stale cleanup
* Assert imported record list

This is one of the best defenses against the record being mapped but never imported.

### 19. Update RecordComparisonService tests

File:

```text
CreationsForge.UnitTests/Services/RecordComparisonServiceTests.cs
```

Add or update tests that verify the comparison service returns expected fields for the new record type.

At minimum, verify:

* The new `RecordTypeCatalog` branch is used
* Common fields are included
* Typed fields are included
* Child groups appear when applicable

### 20. Add reader mapping tests when practical

If existing reader tests cover GMST, GLOB, or AVIF patterns, add equivalent coverage for the new record.

Use these files as reference locations:

```text
CreationsForge.UnitTests/Services/StarfieldRecordReaderTests.cs
CreationsForge.UnitTests/Services/Fallout4RecordReaderTests.cs
CreationsForge.UnitTests/Services/SkyrimRecordReaderTests.cs
```

The key goal is not perfect Mutagen testing. The key goal is catching the repeated mistake where one game reader is updated and the others are forgotten.

### 21. Validate cross-game mapping before calling the work complete

Use this checklist for every new record:

```markdown
| Requirement | Done |
| ----------- | ---- |
| RecordTypeCatalog entry added |  |
| DTO added |  |
| PluginRecordSetDTO property added |  |
| Migration table added |  |
| FormKey index added |  |
| Repository interface added |  |
| Repository implementation added |  |
| New table added to TypedRecordRepositoryBase.AllowedTableNames |  |
| Importer added |  |
| Starfield reader maps the record |  |
| Fallout 4 reader maps the record |  |
| Skyrim reader maps the record |  |
| RecordImportService imports the record |  |
| RecordComparisonService compares the record |  |
| Importer tests added |  |
| RecordImportService tests updated |  |
| RecordComparisonService tests updated |  |
| Supported-games test updated |  |
| DATABASE.md updated |  |
| ERD.md updated |  |
| SQL uses named parameters only |  |
| No positional @0 through @9 SQL placeholders |  |
```

### 22. Search checks before review

Run these searches before review.

Search for the record name:

```powershell
rg "SomeRecord|SomeRecords|XXXX" .
```

Search for positional SQL parameters:

```powershell
rg "@[0-9]" CreationsForge.Core CreationsForge.Migrations CreationsForge.Starfield CreationsForge.Fallout4 CreationsForge.Skyrim
```

Any `@0`, `@1`, `@2`, etc. in runtime SQL should be replaced with named parameters.

Search for incomplete game mapping:

```powershell
rg "MapSomeRecords|SomeRecords" CreationsForge.Starfield CreationsForge.Fallout4 CreationsForge.Skyrim
```

All three game projects should show mapping work unless there is a documented reason not to support one.

Search for import dispatch:

```powershell
rg "RecordTypeCatalog\.SomeRecord|recordSet\.SomeRecords" CreationsForge.Core/Services
```

Search for repository allow-list registration:

```powershell
rg "SomeRecords|RecordTypeCatalog\.SomeRecord" CreationsForge.Core/Repositories/TypedRecordRepositoryBase.cs
```

### 23. Build and test

Run:

```powershell
dotnet build
dotnet test
```

If the record reader depends on installed game files, also run a real import smoke test for each game that is available locally.

The minimum intended validation is:

```powershell
dotnet run --project .\CreationsForge.Console\CreationsForge.Console.csproj -- -game Starfield
dotnet run --project .\CreationsForge.Console\CreationsForge.Console.csproj -- -game Fallout4
dotnet run --project .\CreationsForge.Console\CreationsForge.Console.csproj -- -game Skyrim
```

Adjust arguments if the console harness changes.

After import, inspect the SQLite database and confirm:

* Rows exist in `RecordInstances` for the new record type
* Rows exist in the typed table
* Child tables have rows when applicable
* Stale rows are removed after reimport
* The record appears in the tree
* The comparison view shows the typed fields

### 24. When a record is not supported by every game

Sometimes a record truly is not available for every supported game.

In that case:

1. Document the unsupported game and reason.
2. Do not fake empty support in the reader.
3. Keep the importer `SupportedGames` accurate.
4. Use `ImportOptionalPluginRecordType` if missing support should not create noisy unsupported results.
5. Add test coverage proving the unsupported game is handled intentionally.

Example note:

```markdown
Support notes:

| Game | Supported | Reason |
| ---- | --------- | ------ |
| Starfield | Yes | Mutagen exposes mod.SomeRecords |
| Fallout 4 | No | Record type does not exist in Fallout 4 |
| Skyrim | Yes | Mutagen exposes the collection as AlternateSomeRecords |
```

Do not leave this implicit. Future humans should not need a lantern and a séance to understand why one game is missing.

## Preferred pattern summary

For new normal typed records, prefer the AVIF pattern:

```text
DTO inherits RecordDTO
Repository extends TypedRecordRepositoryBase
Repository uses named SQL parameters
Importer implements ITypedRecordImporter
Importer saves typed record and replaces children
Reader maps all three games
RecordImportService imports the PluginRecordSetDTO list
RecordComparisonService compares typed fields
Database docs are updated with schema changes
```

Use GMST only as a simple scalar reference.

Use GLOB as a simple scalar plus child-data reference.

Use AVIF as the primary implementation reference.

## Common failure points

### Starfield mapped, Fallout 4 and Skyrim forgotten

Symptom:

* Starfield import works
* Fallout 4 and Skyrim silently do not import the new record
* RecordImportService tests may not catch it unless updated

Prevention:

* Fill out the cross-game mapping table before coding
* Update all three reader services in the same change
* Add or update tests for all three game readers when practical
* Search all three game projects before review

### DTO exists but PluginRecordSetDTO is missing the list

Symptom:

* Reader can map the record locally
* Import service never receives the records

Prevention:

* Add `IReadOnlyList<SomeRecordDTO> SomeRecords { get; set; } = [];`
* Update test readers in unit tests

### Importer exists but RecordImportService does not dispatch it

Symptom:

* Importer is registered in DI
* Records are read
* Nothing is imported

Prevention:

* Add `ImportPluginRecordType` or `ImportOptionalPluginRecordType`
* Update `RecordImportServiceTests`

### Repository extends TypedRecordRepositoryBase but table is not allow-listed

Symptom:

* Runtime failure when fetching tree entries, fetching by form key, or deleting stale records

Prevention:

* Add `RecordTypeCatalog.SomeRecord.TableName` to `AllowedTableNames`

### SQL uses positional parameters

Symptom:

* Runtime SQL works today but violates project conventions
* Future edits are harder to maintain
* Parameter ordering bugs become more likely

Prevention:

* Use named placeholders like `@Game`, `@ModKeyName`, and `@ImportedAtUTC`
* Pass anonymous objects or typed parameter objects
* Run `rg "@[0-9]"` before review

### Comparison support is missing

Symptom:

* Record imports into SQLite
* Record appears in the tree
* Comparison view shows little or no useful typed data

Prevention:

* Add repository dependency to `RecordComparisonService`
* Add record-type branch
* Add `CreateSomeRecordComparison`
* Add child comparison groups when needed

### Database docs are stale

Symptom:

* Migration and code are correct
* Documentation lies with great confidence

Prevention:

* Update `Documentation/Database/DATABASE.md`
* Update `Documentation/Database/ERD.md`
* Keep docs aligned to the final migrated schema

## Final reviewer checklist

Before merging a new major record type, confirm:

* The record imports for every intended game.
* Unsupported games are explicitly documented.
* All runtime SQL uses named parameters.
* There are no `@0` through `@9` positional SQL placeholders in touched runtime SQL.
* The typed table has the standard composite primary key.
* The typed table references `Plugins`.
* The typed table references `RecordInstances`.
* The typed table has a FormKey index.
* The repository saves `RecordInstances`.
* The repository deletes stale typed rows and stale `RecordInstances`.
* The importer calls `RecordChildImportService.ReplaceRecordChildren`.
* The import service dispatches the new record.
* The comparison service displays useful typed fields.
* Tests cover importer behavior.
* Tests cover import dispatch.
* Tests cover comparison behavior.
* Cross-game reader mapping was checked for Starfield, Fallout 4, and Skyrim.
* Database documentation was updated.

When in doubt, copy AVIF first, then simplify. That path has fewer trapdoors.
