# Database

## Schema Source

The application uses a local SQLite database. The schema is defined by embedded DbUp scripts in
`CreationsForge.Migrations/Sql`:

- `001_CreateMultiGameImportSchema.sql` creates the application tables, keys, indexes, constraints, and views.
- `002_AddAssetArchiveIndex.sql` adds the metadata-only asset archive index/cache tables, the `Statics` and
  `Containers` typed record tables, `ContainerItems`, and `RawRecordPayloads`.
- `003_Migrations003.sql` renames `MiscObjects` to `MiscItems` and adds the `Books`, `Doors`, `Terminals`, and
  `TerminalMarkerParameters` tables.

DbUp creates and owns its `SchemaVersions` migration-history table. `SchemaVersions` is the migration-state source of
truth. The application does not define a hardcoded schema-version constant.

The application schema contains thirty-seven tables:

- `Games`
- `Plugins`
- `StarfieldPlugins`
- `Fallout4Plugins`
- `SkyrimPlugins`
- `PluginMasterReferences`
- `RecordInstances`
- `FormLists`
- `FormListItems`
- `GameSettings`
- `Globals`
- `MiscItems`
- `Keywords`
- `ActorValueInformation`
- `NPCs`
- `MagicEffects`
- `Perks`
- `Statics`
- `Books`
- `Doors`
- `Containers`
- `ContainerItems`
- `Terminals`
- `TerminalMarkerParameters`
- `RecordKeywords`
- `PerkRanks`
- `PerkRankEffects`
- `PerkBackgroundSkills`
- `Models`
- `ModelMaterialSwaps`
- `RecordSounds`
- `ScriptingAdapters`
- `ScriptingAdapterProperties`
- `ScriptingAdapterPropertyListItems`
- `RawRecordPayloads`
- `AssetArchiveFiles`
- `AssetArchiveEntries`

The application schema also contains these read views:

- `StarfieldPluginDetails`
- `Fallout4PluginDetails`
- `SkyrimPluginDetails`

See [ERD.md](ERD.md) for the relationship diagram.

## Database Location

`ApplicationConfigurationStore` defines the default application-data location:

- Linux directory: `~/.CreationsForge`
- Other platforms directory: `<CommonApplicationData>/CreationsForge`
- Database file: `CreationsForge.sqlite`
- Log directory: `<ApplicationDataDirectory>/Logs`

`ApplicationConfiguration` can store custom application data, database, and logging directories.

The Reset & Import All workflow deletes the configured `CreationsForge.sqlite` database file and SQLite sidecar files
with `-wal` and `-shm` suffixes before running DbUp migrations and full imports for every supported game. Reset only
deletes the expected `CreationsForge.sqlite` file name. Paths under the default CreationsForge application-data
directory are trusted reset targets. Custom database paths must already contain a recognizable CreationsForge SQLite
database marker, such as expected application tables or DbUp's `SchemaVersions` table, before reset deletes the main
database file or its sidecars. This changes database contents but does not change the application schema shape.

## Connection Behavior

`SqliteConnectionFactory.OpenDatabase` creates the database directory, builds a SQLite connection string, and returns
an NPoco `IDatabase`.

Connection settings include:

- SQLite foreign keys enabled in the connection string.
- WAL journal mode.
- Pooling disabled.
- `PRAGMA foreign_keys = ON` executed after opening the NPoco database.

`DatabaseMigrationRunner` also disables pooling while DbUp applies migrations. Migration connection strings are built
with a connection-string builder so configured database paths are treated as data-source values.

## Import Transaction Behavior

The shared game import workflow wraps database writes in one NPoco transaction. The transaction covers game row,
plugin row, plugin extension, master-reference, typed-record, and stale-cleanup writes for the import run. The
transaction is completed only after the shared import workflow finishes successfully.

This keeps large imports from paying SQLite autocommit cost for every saved row. If an uncaught exception or
cancellation escapes the import workflow, the transaction is disposed without completion.

## Runtime SQL Parameterization

Application runtime SQL must use named parameterized queries. Repository and service SQL must pass runtime values with
named parameter objects such as anonymous objects or typed parameter DTOs. Positional NPoco placeholders such as `@0`,
`@1`, and `@2` are not used for application runtime SQL because they are harder to review and easier to bind
incorrectly during query maintenance.

This rule applies to runtime SQL executed by the application. Embedded DbUp migration scripts are schema scripts and
do not bind runtime values.

## Common Typed Record Shape

Typed record parent tables use this composite primary key:

- `Game`
- `ModKey_Name`
- `ModKey_Type`
- `ModKey_FileName`
- `FormKey_ModKey_Name`
- `FormKey_ModKey_Type`
- `FormKey_ModKey_FileName`
- `FormKey_ID`

The `Game` plus `ModKey_*` columns identify the plugin containing the imported record row. The
`FormKey_ModKey_*` columns plus `FormKey_ID` identify the record's origin FormKey.

Application-schema columns store ModKey and FormKey values as primitive component columns. Raw string storage of
FormKey or ModKey is not used by this schema.

Runtime ModKey lookups compare `*_ModKey_Name` and `*_ModKey_FileName` case-insensitively. The stored casing is
preserved, but casing is not part of plugin identity for lookup because load-order entries, plugin headers, and master
references can differ by case for the same file.
Rows with declared foreign keys to `Plugins` use the resolved persisted plugin ModKey tuple when saving dependent
relationships.

`RecordInstances` is the shared persisted parent identity for imported record overrides. Typed record parent tables
reference `RecordInstances` by game, containing plugin ModKey, and origin FormKey. `ScriptingAdapters` references the
full `RecordInstances` key including `RecordType`, so scripting adapters remain record-owned without needing one
adapter table per record type.
`Models` also references the full `RecordInstances` key including `RecordType`, plus a model slot and model gender so
direct, slotted, and gendered model payloads can share one table family.
`RecordKeywords` references the full `RecordInstances` key including `RecordType`, so keyword lists can be shared by
record types that expose the same indexed keyword payload.
`RecordSounds` references the full `RecordInstances` key including `RecordType`, so named and indexed sound payloads
can be shared by record types that expose the same Spriggit-style sound data.
`RawRecordPayloads` references the full `RecordInstances` key including `RecordType`, so opaque payload bytes or
strings can be retained for future parsing without adding one table per record type.

## Tables

### Games

Columns:

- `Game` (`TEXT`, `NOT NULL`, primary key)
- `DisplayName` (`TEXT`, `NOT NULL`)
- `InstallationFolder` (`TEXT`, nullable)
- `DataFolder` (`TEXT`, nullable)
- `ImportedAtUTC` (`TEXT`, `NOT NULL`)

Constraints:

- `Game` must be `Starfield`, `Fallout4`, or `Skyrim`.

### Plugins

Columns:

- `Game` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `ModKey_Name` (`TEXT`, `NOT NULL`, primary key)
- `ModKey_Type` (`INTEGER`, `NOT NULL`, primary key)
- `ModKey_FileName` (`TEXT`, `NOT NULL`, primary key)
- `LoadOrderIndex` (`INTEGER`, `NOT NULL`)
- `Enabled` (`INTEGER`, `NOT NULL`, default `1`)
- `ExistsOnDisk` (`INTEGER`, `NOT NULL`, default `1`)
- `ImportState` (`TEXT`, `NOT NULL`, default `Current`)
- `HeaderFlags` (`INTEGER`, `NOT NULL`)
- `FormVersion` (`INTEGER`, `NOT NULL`)
- `Author` (`TEXT`, nullable)
- `Description` (`TEXT`, nullable)
- `RecordCount` (`INTEGER`, `NOT NULL`, default `0`)
- `SourceLastWriteUTCTicks` (`INTEGER`, `NOT NULL`)
- `SourceFileSizeBytes` (`INTEGER`, `NOT NULL`)
- `LastCheckedUTC` (`TEXT`, `NOT NULL`)
- `LastImportedUTC` (`TEXT`, nullable)
- `InvalidatedAtUTC` (`TEXT`, nullable)

Foreign keys:

- `Game` references `Games.Game` with `ON DELETE CASCADE`.

Constraints:

- `ImportState` must be `Current`, `Changed`, `PartiallyImported`, `Missing`, `Failed`, or `Unsupported`.

Indexes:

- `IX_Plugins_Game_LoadOrderIndex` on `Game` and `LoadOrderIndex`
- `IX_Plugins_Game_ImportState` on `Game` and `ImportState`

### StarfieldPlugins

Columns:

- `Game` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `ModKey_Name` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `ModKey_Type` (`INTEGER`, `NOT NULL`, primary key, foreign key)
- `ModKey_FileName` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `Branch` (`TEXT`, `NOT NULL`)
- `InteriorCellCount` (`INTEGER`, nullable)
- `Intv` (`INTEGER`, nullable)

Foreign keys:

- `Game` plus `ModKey_*` references `Plugins` with `ON DELETE CASCADE`.

Constraints:

- `Game` must be `Starfield`.
- `InteriorCellCount` must be null or greater than or equal to zero.

### Fallout4Plugins

Columns:

- `Game` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `ModKey_Name` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `ModKey_Type` (`INTEGER`, `NOT NULL`, primary key, foreign key)
- `ModKey_FileName` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `Incc` (`INTEGER`, nullable)

Foreign keys:

- `Game` plus `ModKey_*` references `Plugins` with `ON DELETE CASCADE`.

Constraints:

- `Game` must be `Fallout4`.
- `Incc` must be null or greater than or equal to zero.

### SkyrimPlugins

Columns:

- `Game` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `ModKey_Name` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `ModKey_Type` (`INTEGER`, `NOT NULL`, primary key, foreign key)
- `ModKey_FileName` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `Incc` (`INTEGER`, nullable)
- `Intv` (`INTEGER`, nullable)

Foreign keys:

- `Game` plus `ModKey_*` references `Plugins` with `ON DELETE CASCADE`.

Constraints:

- `Game` must be `Skyrim`.
- `Incc` must be null or greater than or equal to zero.

### PluginMasterReferences

Columns:

- `Game` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `Master_ModKey_Name` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `Master_ModKey_Type` (`INTEGER`, `NOT NULL`, primary key, foreign key)
- `Master_ModKey_FileName` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `Plugin_ModKey_Name` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `Plugin_ModKey_Type` (`INTEGER`, `NOT NULL`, primary key, foreign key)
- `Plugin_ModKey_FileName` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `ImportedAtUTC` (`TEXT`, `NOT NULL`)

Foreign keys:

- `Game` plus `Master_ModKey_*` references `Plugins` with `ON DELETE CASCADE`.
- `Game` plus `Plugin_ModKey_*` references `Plugins` with `ON DELETE CASCADE`.

Persistence behavior:

- Current imported rows are upserted.
- Rows for the same game/plugin whose `ImportedAtUTC` was not refreshed by the current successful master-reference
  import batch are deleted as stale.

### RecordInstances

Columns:

- Common typed record key and metadata columns listed above
- `RecordType` (`TEXT`, `NOT NULL`, primary key)

Foreign keys:

- `Game` plus containing `ModKey_*` references `Plugins` with `ON DELETE CASCADE`.

Constraints:

- Full common typed record key plus `RecordType` is the primary key.
- Full common typed record key without `RecordType` is unique so typed detail tables can reference the shared record
  identity without duplicating a constant `RecordType` column in every typed table.

Indexes:

- `IX_RecordInstances_FormKey` on `Game`, origin FormKey ModKey columns, and `FormKey_ID`
- `IX_RecordInstances_Game_RecordType_Plugin` on `Game`, `RecordType`, containing plugin ModKey columns, `EditorID`,
  and `FormKey_ID`
- `IX_RecordInstances_Game_RecordType_FormKey` on `Game`, `RecordType`, origin FormKey ModKey columns, and
  `FormKey_ID`

Persistence behavior:

- Current imported rows are upserted before typed detail rows and scripting adapters.
- Rows for the same game/plugin/record type whose `ImportedAtUTC` was not refreshed by the current successful
  typed-record import batch are deleted as stale after stale typed detail rows are removed.

### FormLists

Columns:

- Common typed record key and metadata columns listed above
- `AddToList_ModKey_Name` (`TEXT`, nullable)
- `AddToList_ModKey_Type` (`INTEGER`, nullable)
- `AddToList_ModKey_FileName` (`TEXT`, nullable)
- `AddToList_FormKey_ID` (`INTEGER`, nullable)

Foreign keys:

- `Game` plus containing `ModKey_*` references `Plugins` with `ON DELETE CASCADE`.
- Full common typed record key references `RecordInstances` with `ON DELETE CASCADE`.

Indexes:

- `IX_FormLists_FormKey` on `Game`, origin FormKey ModKey columns, and `FormKey_ID`
- `IX_FormLists_Game_Plugin` on `Game`, containing plugin ModKey columns, `EditorID`, and `FormKey_ID`
- `IX_FormLists_Game_FormKey_Collated` on `Game`, origin FormKey ModKey columns, and `FormKey_ID`

Persistence behavior:

- Current imported rows are upserted.
- Rows for the same game/plugin whose `ImportedAtUTC` was not refreshed by the current successful FormList import
  batch are deleted as stale.
- Stale parent FormList deletion cascades to child `FormListItems` rows through the declared foreign key.

### FormListItems

Columns:

- `Game` (`TEXT`, `NOT NULL`, primary key, foreign key)
- containing `ModKey_*` columns (`NOT NULL`, primary key, foreign key)
- parent `FormKey_ModKey_*` plus `FormKey_ID` (`NOT NULL`, primary key, foreign key)
- `Item_ModKey_Name` (`TEXT`, `NOT NULL`, primary key)
- `Item_ModKey_Type` (`INTEGER`, `NOT NULL`, primary key)
- `Item_ModKey_FileName` (`TEXT`, `NOT NULL`, primary key)
- `Item_FormKey_ID` (`INTEGER`, `NOT NULL`, primary key)
- `Item_Index` (`INTEGER`, `NOT NULL`, primary key)
- `ImportedAtUTC` (`TEXT`, `NOT NULL`)

Foreign keys:

- Full parent key references `FormLists` with `ON DELETE CASCADE`.

Persistence behavior:

- Current imported rows are upserted.
- Rows for the same game/plugin whose `ImportedAtUTC` was not refreshed by the current successful FormList import
  batch are deleted as stale.

### GameSettings

Columns:

- Common typed record key and metadata columns listed above
- `SettingType` (`TEXT`, nullable)
- `Data` (`TEXT`, nullable)
- `NumericData` (`REAL`, nullable)
- `IntegerData` (`INTEGER`, nullable)
- `BooleanData` (`INTEGER`, nullable)

Foreign keys:

- `Game` plus containing `ModKey_*` references `Plugins` with `ON DELETE CASCADE`.
- Full common typed record key references `RecordInstances` with `ON DELETE CASCADE`.

Indexes:

- `IX_GameSettings_FormKey` on `Game`, origin FormKey ModKey columns, and `FormKey_ID`
- `IX_GameSettings_Game_Plugin` on `Game`, containing plugin ModKey columns, `EditorID`, and `FormKey_ID`
- `IX_GameSettings_Game_FormKey_Collated` on `Game`, origin FormKey ModKey columns, and `FormKey_ID`

Persistence behavior:

- Current imported rows are upserted.
- Rows for the same game/plugin whose `ImportedAtUTC` was not refreshed by the current successful GameSetting import
  batch are deleted as stale.

### Globals

Columns:

- Common typed record key and metadata columns listed above
- `Data` (`REAL`, nullable)

Foreign keys:

- `Game` plus containing `ModKey_*` references `Plugins` with `ON DELETE CASCADE`.
- Full common typed record key references `RecordInstances` with `ON DELETE CASCADE`.

Indexes:

- `IX_Globals_FormKey` on `Game`, origin FormKey ModKey columns, and `FormKey_ID`
- `IX_Globals_Game_Plugin` on `Game`, containing plugin ModKey columns, `EditorID`, and `FormKey_ID`
- `IX_Globals_Game_FormKey_Collated` on `Game`, origin FormKey ModKey columns, and `FormKey_ID`

Persistence behavior:

- Current imported rows are upserted.
- Rows for the same game/plugin whose `ImportedAtUTC` was not refreshed by the current successful Global import batch
  are deleted as stale.

### Shared scripted parent records

`MiscItems`, `Keywords`, `ActorValueInformation`, `NPCs`, `MagicEffects`, `Perks`, `Statics`, `Books`, `Doors`,
`Containers`, and `Terminals` use
the common typed record key and metadata columns.

`MiscItems` additional columns:

- `Name` (`TEXT`, nullable)
- `ShortName` (`TEXT`, nullable)
- `Value` (`INTEGER`, nullable)
- `Weight` (`REAL`, nullable)
- `DirtinessScale` (`REAL`, nullable)
- `FeaturedItemMessage_ModKey_Name` (`TEXT`, nullable)
- `FeaturedItemMessage_ModKey_Type` (`INTEGER`, nullable)
- `FeaturedItemMessage_ModKey_FileName` (`TEXT`, nullable)
- `FeaturedItemMessage_FormKey_ID` (`INTEGER`, nullable)
- `FLAG` (`TEXT`, nullable)

`Keywords` additional columns:

- `Name` (`TEXT`, nullable)
- `Color` (`TEXT`, `NOT NULL`)
- `Type` (`TEXT`, `NOT NULL`)
- `Notes` (`TEXT`, nullable)
- `FlashLinkageName` (`TEXT`, nullable)
- `AttractionRule_ModKey_Name` (`TEXT`, nullable)
- `AttractionRule_ModKey_Type` (`INTEGER`, nullable)
- `AttractionRule_ModKey_FileName` (`TEXT`, nullable)
- `AttractionRule_FormKey_ID` (`INTEGER`, nullable)

`ActorValueInformation` additional columns:

- `Name` (`TEXT`, nullable)
- `Abbreviation` (`TEXT`, nullable)
- `ContextNotes` (`TEXT`, nullable)
- `DefaultValue` (`REAL`, nullable)
- `Flags` (`TEXT`, nullable)
- `Type` (`TEXT`, nullable)
- `Min` (`REAL`, nullable)
- `Max` (`REAL`, nullable)

`NPCs` additional columns:

- `Name`, `ShortName`, `LongName`, and `Pronoun` (`TEXT`, nullable)
- `DispositionBase`, `EnergyLevel`, and `GearedUpWeapons` (`INTEGER`, `NOT NULL`)
- `Aggression`, `Confidence`, `Responsibility`, and `Assistance` (`TEXT`, `NOT NULL`)
- `HeightMin` and `HeightMax` (`REAL`, `NOT NULL`)
- `SkinToneIndex` (`INTEGER`, nullable)
- nullable decomposed FormKey columns for `Voice`, `Race`, `CombatOverridePackageList`, `CombatStyle`,
  `DefaultPackageList`, and `CrimeFaction`

`MagicEffects` additional columns:

- `Name`, `Description`, `CastType`, and `TargetType` (`TEXT`, nullable)
- `Flags` (`TEXT`, `NOT NULL`)
- nullable decomposed FormKey columns for `ActorValue2`, `ResistValue`, `PerkToApply`, `EquipAbility`,
  `Explosion`, `CastingArt`, `HitEffectArt`, `HitShader`, `ImageSpaceModifier`, `ImpactData`, and `Projectile`
- `Archetype` (`TEXT`, nullable)
- `UnknownFloat3` (`REAL`, nullable)
- `UnknownInt2` (`INTEGER`, nullable)
- `Unknown` (`TEXT`, nullable)
- `Unknown2` (`TEXT`, nullable)
- `DataTypeState` (`TEXT`, nullable)

`Perks` additional columns:

- `Name`, `Description`, `SkillGroup`, `CrewAssignment`, `PerkIcon`, `Category`, and `MajorFlags`
  (`TEXT`, nullable)
- `Flags` (`TEXT`, `NOT NULL`)
- nullable decomposed FormKey columns for `Restriction` and `Training`

`Statics` additional columns:

- `Version2` (`INTEGER`, nullable)
- `ObjectBounds_First` and `ObjectBounds_Second` (`TEXT`, nullable)
- `MaxAngle`, `UnknownDNAMFloat`, `LeafAmplitude`, and `LeafFrequency` (`REAL`, nullable)
- `Unused` (`TEXT`, nullable)
- `DNAMDataTypeState` (`TEXT`, nullable)

`Books` additional columns:

- `Version2` (`INTEGER`, nullable)
- `ObjectBounds_First` and `ObjectBounds_Second` (`TEXT`, nullable)
- nullable decomposed FormKey columns for `InventoryTransform`
- `XALG` (`INTEGER`, nullable)
- `Name`, `Text`, `Flags`, `TeachesType`, `TeachesRawContent`, `DataSlateType`, `Description`,
  `DataSlateHeaderLeft`, and `DataSlateHeaderRight` (`TEXT`, nullable)
- `Value` (`INTEGER`, nullable)
- `Weight` (`REAL`, nullable)

`Doors` additional columns:

- `Version2` (`INTEGER`, nullable)
- `ObjectBounds_First` and `ObjectBounds_Second` (`TEXT`, nullable)
- `Name`, `Flags`, `SoundLevel`, and `FacingAxisOverride` (`TEXT`, nullable)
- nullable decomposed FormKey columns for `NativeTerminal`

`Containers` additional columns:

- `Version2` (`INTEGER`, nullable)
- `ObjectBounds_First` and `ObjectBounds_Second` (`TEXT`, nullable)
- `Name`, `Flags`, and `MajorFlags` (`TEXT`, nullable)
- nullable decomposed FormKey columns for `NativeTerminal`

`Terminals` additional columns:

- `Version2` (`INTEGER`, nullable)
- `ObjectBounds_First` and `ObjectBounds_Second` (`TEXT`, nullable)
- nullable decomposed FormKey columns for `Menu` and `FurnitureTemplate`
- `Background`, `Name`, `PNAM`, `FNAM`, `JNAM`, `GNAM`, `WorkbenchData`, and `MarkerModel` (`TEXT`, nullable)
- `MarkerFlags` (`INTEGER`, nullable)

Foreign keys:

- `Game` plus containing `ModKey_*` references `Plugins` with `ON DELETE CASCADE`.
- Full common typed record key references `RecordInstances` with `ON DELETE CASCADE`.

Indexes:

- Each scripted parent table has a form-key index on `Game`, origin FormKey ModKey columns, and `FormKey_ID`.
- Each scripted parent table has an active-plugin browse index on `Game`, containing plugin ModKey columns,
  `EditorID`, and `FormKey_ID`.
- Each scripted parent table has a collated form-key browse index on `Game`, origin FormKey ModKey columns, and
  `FormKey_ID`.

Persistence behavior:

- Current imported rows are upserted.
- Rows for the same game/plugin whose `ImportedAtUTC` was not refreshed by the current successful typed-record import
  batch are deleted as stale.
- `MiscItems` currently persists the parent scalar row, shared keyword rows, shared model rows, shared sound rows,
  and scripting adapters. `Statics` persists parent scalar rows, shared model rows, shared keyword rows when present,
  and raw opaque payload rows. `Books` persist parent scalar rows, shared model rows, shared keyword rows, shared
  sound rows, scripting adapters, and raw payload rows. `Doors` persist parent scalar rows, shared model rows,
  shared keyword rows, shared sound rows, and raw payload rows. `Containers` persist parent scalar rows, child item
  rows, shared model rows, shared keyword rows when present, shared sound rows when present, and raw opaque payload
  rows. `Terminals` persist parent scalar rows, shared model rows, shared keyword rows, scripting adapters, raw
  payload rows, and `TerminalMarkerParameters` rows. `NPCs` and `MagicEffects` persist shared keyword rows.
  `MagicEffects` persists shared sound rows and Spriggit-flattened DATA fields directly on the parent row.

### ContainerItems

Columns:

- Common containing plugin key columns listed above
- typed-record origin FormKey columns listed above (`NOT NULL`, primary key)
- `Item_Index` (`INTEGER`, `NOT NULL`, primary key)
- decomposed `Item_*` FormKey columns (`NOT NULL`)
- `Count` (`INTEGER`, nullable)
- `ImportedAtUTC` (`TEXT`, `NOT NULL`)

Foreign keys:

- Full common typed record key references `Containers` with `ON DELETE CASCADE`.

Constraints:

- `Item_Index`, `Item_FormKey_ID`, and `FormKey_ID` must be greater than or equal to zero.

Indexes:

- `IX_ContainerItems_Game_FormKey` on `Game`, origin FormKey ModKey columns, and `FormKey_ID`

Persistence behavior:

- Current imported rows are upserted after their owning container row is saved.
- Existing item rows for the same container are deleted before replacement so removed items do not remain stale.
- Stale typed-record deletion removes item rows through the declared `Containers` cascade.

### TerminalMarkerParameters

Columns:

- Common containing plugin key columns listed above
- typed-record origin FormKey columns listed above (`NOT NULL`, primary key)
- `Parameter_Index` (`INTEGER`, `NOT NULL`, primary key)
- `Offset`, `EntryTypes`, and `ExitTypes` (`TEXT`, nullable)
- `ImportedAtUTC` (`TEXT`, `NOT NULL`)

Foreign keys:

- Full common typed record key references `Terminals` with `ON DELETE CASCADE`.

Constraints:

- `Parameter_Index` and `FormKey_ID` must be greater than or equal to zero.

Indexes:

- `IX_TerminalMarkerParameters_Game_FormKey` on `Game`, origin FormKey ModKey columns, and `FormKey_ID`

Persistence behavior:

- Current imported rows are upserted after their owning terminal row is saved.
- Existing marker-parameter rows for the same terminal are deleted before replacement so removed parameter slots do not
  remain stale.
- Stale typed-record deletion removes marker-parameter rows through the declared `Terminals` cascade.

### RecordKeywords

Columns:

- Common containing plugin key columns listed above
- `RecordType` (`TEXT`, `NOT NULL`, primary key)
- typed-record origin FormKey columns listed above (`NOT NULL`, primary key)
- decomposed `Keyword_*` FormKey columns (`NOT NULL`)
- `Keyword_Index` (`INTEGER`, `NOT NULL`, primary key)
- `ImportedAtUTC` (`TEXT`, `NOT NULL`)

Foreign keys:

- Full common typed record key plus `RecordType` references `RecordInstances` with `ON DELETE CASCADE`.

Persistence behavior:

- Current imported rows are upserted after their owning typed record row is saved.
- Existing keyword rows for the same record are deleted before replacement so removed keyword slots do not remain
  stale.
- Stale typed-record deletion removes keyword rows through the declared `RecordInstances` cascade.

### Perk child tables

`PerkRanks`, `PerkRankEffects`, and `PerkBackgroundSkills` use the same containing plugin and parent FormKey columns
as `Perks`.

`PerkRanks` additional columns:

- `Rank_Index` (`INTEGER`, `NOT NULL`, primary key)
- `Description` (`TEXT`, nullable)
- nullable decomposed FormKey columns for `UnknownStatic`
- `ConditionCount` (`INTEGER`, `NOT NULL`)
- `ActivityCount` (`INTEGER`, `NOT NULL`)
- `ImportedAtUTC` (`TEXT`, `NOT NULL`)

`PerkRankEffects` additional columns:

- `Rank_Index` (`INTEGER`, `NOT NULL`, primary key, foreign key)
- `Effect_Index` (`INTEGER`, `NOT NULL`, primary key)
- `MutagenObjectType` (`TEXT`, `NOT NULL`)
- `Rank` (`INTEGER`, `NOT NULL`)
- `Priority` (`INTEGER`, `NOT NULL`)
- `PerkEntryID` (`INTEGER`, nullable)
- `Flags`, `ButtonLabel`, `EntryPoint`, and `Modification` (`TEXT`, nullable)
- `ConditionCount` (`INTEGER`, `NOT NULL`)
- `PerkConditionTabCount` (`INTEGER`, nullable)
- `Value` (`REAL`, nullable)
- `ImportedAtUTC` (`TEXT`, `NOT NULL`)

`PerkBackgroundSkills` additional columns:

- decomposed `Skill_*` FormKey columns (`NOT NULL`)
- `Skill_Index` (`INTEGER`, `NOT NULL`, primary key)
- `ImportedAtUTC` (`TEXT`, `NOT NULL`)

Foreign keys:

- `PerkRanks` references `Perks` with `ON DELETE CASCADE`.
- `PerkRankEffects` references `PerkRanks` with `ON DELETE CASCADE`.
- `PerkBackgroundSkills` references `Perks` with `ON DELETE CASCADE`.

### Models

Columns:

- Common containing plugin key columns listed above
- `RecordType` (`TEXT`, `NOT NULL`, primary key)
- typed-record origin FormKey columns listed above (`NOT NULL`, primary key)
- `ModelSlot` (`TEXT`, `NOT NULL`, primary key)
- `ModelGender` (`TEXT`, `NOT NULL`, primary key)
- `File` (`TEXT`, nullable)
- `TextureFileHashes` (`TEXT`, nullable)
- `LightLayer` (`INTEGER`, nullable)
- `Flags` (`TEXT`, nullable)
- `ColorRemappingIndex` (`REAL`, nullable)
- `FlagsVestigial` (`TEXT`, nullable)
- `ImportedAtUTC` (`TEXT`, `NOT NULL`)

Foreign keys:

- Full common typed record key plus `RecordType` references `RecordInstances` with `ON DELETE CASCADE`.

Persistence behavior:

- Current imported rows are upserted after their owning typed record row is saved.
- Existing model rows for the same record are deleted before replacement so removed model slots do not remain stale.
- Stale typed-record deletion removes model rows through the declared `RecordInstances` cascade.

### ModelMaterialSwaps

Columns:

- Full parent model key columns listed above
- decomposed `MaterialSwap_*` FormKey columns (`NOT NULL`)
- `MaterialSwap_Index` (`INTEGER`, `NOT NULL`, primary key)
- `ImportedAtUTC` (`TEXT`, `NOT NULL`)

Foreign keys:

- Full parent key references `Models` with `ON DELETE CASCADE`.

Persistence behavior:

- Current imported rows are upserted after their owning `Models` row is saved.
- Existing material-swap rows for a replaced model are deleted through the parent `Models` row replacement/delete
  behavior.

### RecordSounds

Columns:

- Common containing plugin key columns listed above
- `RecordType` (`TEXT`, `NOT NULL`, primary key)
- typed-record origin FormKey columns listed above (`NOT NULL`, primary key)
- `SoundSlot` (`TEXT`, `NOT NULL`, primary key)
- `Sound_Index` (`INTEGER`, `NOT NULL`, primary key)
- `Start` (`TEXT`, nullable)
- `Versioning` (`TEXT`, nullable)
- `Unknown` (`TEXT`, nullable)
- `ImportedAtUTC` (`TEXT`, `NOT NULL`)

Foreign keys:

- Full common typed record key plus `RecordType` references `RecordInstances` with `ON DELETE CASCADE`.

Persistence behavior:

- Current imported rows are upserted after their owning typed record row is saved.
- Existing sound rows for the same record are deleted before replacement so removed sound slots do not remain stale.
- Stale typed-record deletion removes sound rows through the declared `RecordInstances` cascade.

### RawRecordPayloads

Columns:

- Common containing plugin key columns listed above
- `RecordType` (`TEXT`, `NOT NULL`, primary key)
- typed-record origin FormKey columns listed above (`NOT NULL`, primary key)
- `PayloadSlot` (`TEXT`, `NOT NULL`, primary key)
- `Payload_Index` (`INTEGER`, `NOT NULL`, primary key)
- `PayloadType` (`TEXT`, `NOT NULL`)
- `PayloadValue` (`TEXT`, nullable)
- `ImportedAtUTC` (`TEXT`, `NOT NULL`)

Foreign keys:

- Full common typed record key plus `RecordType` references `RecordInstances` with `ON DELETE CASCADE`.

Constraints:

- `PayloadSlot` and `PayloadType` must not be empty.
- `Payload_Index` and `FormKey_ID` must be greater than or equal to zero.

Indexes:

- `IX_RawRecordPayloads_Game_Record_FormKey` on `Game`, `RecordType`, origin FormKey ModKey columns, and
  `FormKey_ID`

Persistence behavior:

- Current imported rows are upserted after their owning typed record row is saved.
- Existing raw payload rows for the same record are deleted before replacement so removed payload slots do not remain
  stale.
- Stale typed-record deletion removes raw payload rows through the declared `RecordInstances` cascade.
- Current importers populate raw payload rows for Static model/component reflection payloads and Container
  model/component reflection payloads, including Starfield container `ANAM`, `BNAM`, `CNAM`, and `REFL` component
  subfields when Mutagen exposes them through reflection.

### ScriptingAdapters

Columns:

- Common containing plugin key columns listed above
- `RecordType` (`TEXT`, `NOT NULL`, primary key)
- typed-record origin FormKey columns listed above (`NOT NULL`, primary key)
- `Name` (`TEXT`, `NOT NULL`, primary key)
- `Script_Index` (`INTEGER`, `NOT NULL`)
- `ImportedAtUTC` (`TEXT`, `NOT NULL`)

Foreign keys:

- Full common typed record key plus `RecordType` references `RecordInstances` with `ON DELETE CASCADE`.

### ScriptingAdapterProperties

Columns:

- Full parent scripting adapter key columns listed above
- `Property_Index` (`INTEGER`, `NOT NULL`, primary key)
- `Name` (`TEXT`, `NOT NULL`)
- `MutagenObjectType` (`TEXT`, `NOT NULL`)
- `Data_Bool` (`INTEGER`, nullable)
- `Data_Int` (`INTEGER`, nullable)
- `Data_Float` (`REAL`, nullable)
- `Data_String` (`TEXT`, nullable)
- nullable decomposed `Object_*` FormKey columns
- `Object_Alias` (`INTEGER`, nullable)
- `Object_Unused` (`INTEGER`, nullable)
- `ImportedAtUTC` (`TEXT`, `NOT NULL`)

Foreign keys:

- Full parent key references `ScriptingAdapters` with `ON DELETE CASCADE`.

### ScriptingAdapterPropertyListItems

Columns:

- Full parent scripting adapter property key columns listed above
- `ListItem_Index` (`INTEGER`, `NOT NULL`, primary key)
- `MutagenObjectType` (`TEXT`, `NOT NULL`)
- `Data_Bool` (`INTEGER`, nullable)
- `Data_Int` (`INTEGER`, nullable)
- `Data_Float` (`REAL`, nullable)
- `Data_String` (`TEXT`, nullable)
- nullable decomposed `Object_*` FormKey columns
- `Object_Alias` (`INTEGER`, nullable)
- `Object_Unused` (`INTEGER`, nullable)
- `ImportedAtUTC` (`TEXT`, `NOT NULL`)

Foreign keys:

- Full parent key references `ScriptingAdapterProperties` with `ON DELETE CASCADE`.

### AssetArchiveFiles

Columns:

- `Game` (`TEXT`, `NOT NULL`, primary key)
- `DataFolder` (`TEXT`, `NOT NULL`)
- `ArchivePath` (`TEXT`, `NOT NULL`, primary key)
- `ArchiveFileName` (`TEXT`, `NOT NULL`)
- `ArchiveExtension` (`TEXT`, `NOT NULL`)
- `ArchiveType` (`TEXT`, `NOT NULL`)
- `SourceLastWriteUTCTicks` (`INTEGER`, `NOT NULL`)
- `SourceFileSizeBytes` (`INTEGER`, `NOT NULL`)
- `IndexedAtUTC` (`TEXT`, `NOT NULL`)

Constraints:

- `Game` must be `Starfield`, `Fallout4`, or `Skyrim`.
- Source last-write ticks and file size must be greater than or equal to zero.

Indexes:

- `IX_AssetArchiveFiles_Game_DataFolder` on `Game` and `DataFolder`

Persistence behavior:

- Rows cache archive file metadata for asset lookup acceleration only.
- Cache validity is based on matching archive last-write ticks and file size.
- Cache rows do not store extracted archive bytes.

### AssetArchiveEntries

Columns:

- `Game` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `ArchivePath` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `NormalizedEntryPath` (`TEXT`, `NOT NULL`, primary key)
- `RootFolder` (`TEXT`, `NOT NULL`)
- `Extension` (`TEXT`, `NOT NULL`)
- `PackedSize` (`INTEGER`, `NOT NULL`)
- `UnpackedSize` (`INTEGER`, `NOT NULL`)

Foreign keys:

- `Game` plus `ArchivePath` references `AssetArchiveFiles` with `ON DELETE CASCADE`.

Constraints:

- Packed and unpacked sizes must be greater than or equal to zero.

Indexes:

- `IX_AssetArchiveEntries_Game_NormalizedEntryPath` on `Game` and `NormalizedEntryPath`
- `IX_AssetArchiveEntries_Game_RootFolder_Extension` on `Game`, `RootFolder`, and `Extension`

Persistence behavior:

- Rows cache normalized archive entry paths and size metadata for asset lookup acceleration only.
- Entries are replaced for one archive when its cached file metadata is missing or stale.
- Matching entries are read from the source archive on demand; extracted bytes are not persisted.

## Views

`StarfieldPluginDetails`, `Fallout4PluginDetails`, and `SkyrimPluginDetails` join `Plugins` to their corresponding
game-specific plugin extension table on `Game` plus `ModKey_*`. Each view filters to its game and exposes the shared
plugin columns plus the scalar game-specific extension columns.

## Inferred Relationships

These columns carry record-reference identity but do not declare SQLite foreign keys:

- `FormLists.AddToList_ModKey_Name`, `AddToList_ModKey_Type`, `AddToList_ModKey_FileName`,
  and `AddToList_FormKey_ID`
- `MiscItems.FeaturedItemMessage_ModKey_Name`, `FeaturedItemMessage_ModKey_Type`,
  `FeaturedItemMessage_ModKey_FileName`, and `FeaturedItemMessage_FormKey_ID`
- `Keywords.AttractionRule_ModKey_Name`, `AttractionRule_ModKey_Type`, `AttractionRule_ModKey_FileName`,
  and `AttractionRule_FormKey_ID`
- `NPCs.Voice_*`, `Race_*`, `CombatOverridePackageList_*`, `CombatStyle_*`, `DefaultPackageList_*`,
  and `CrimeFaction_*`
- `MagicEffects.ActorValue2_*`, `ResistValue_*`, `PerkToApply_*`, `EquipAbility_*`, `Explosion_*`,
  `CastingArt_*`, `HitEffectArt_*`, `HitShader_*`, `ImageSpaceModifier_*`, `ImpactData_*`, and `Projectile_*`
- `Perks.Restriction_*` and `Training_*`
- `Books.InventoryTransform_ModKey_Name`, `InventoryTransform_ModKey_Type`,
  `InventoryTransform_ModKey_FileName`, and `InventoryTransform_FormKey_ID`
- `Doors.NativeTerminal_ModKey_Name`, `NativeTerminal_ModKey_Type`, `NativeTerminal_ModKey_FileName`,
  and `NativeTerminal_FormKey_ID`
- `Containers.NativeTerminal_ModKey_Name`, `NativeTerminal_ModKey_Type`, `NativeTerminal_ModKey_FileName`,
  and `NativeTerminal_FormKey_ID`
- `Terminals.Menu_ModKey_Name`, `Menu_ModKey_Type`, `Menu_ModKey_FileName`, and `Menu_FormKey_ID`
- `Terminals.FurnitureTemplate_ModKey_Name`, `FurnitureTemplate_ModKey_Type`,
  `FurnitureTemplate_ModKey_FileName`, and `FurnitureTemplate_FormKey_ID`
- `FormListItems.Item_ModKey_Name`, `Item_ModKey_Type`, `Item_ModKey_FileName`, and `Item_FormKey_ID`
- `ModelMaterialSwaps.MaterialSwap_ModKey_Name`, `MaterialSwap_ModKey_Type`, `MaterialSwap_ModKey_FileName`,
  and `MaterialSwap_FormKey_ID`
- `RecordKeywords.Keyword_ModKey_Name`, `Keyword_ModKey_Type`, `Keyword_ModKey_FileName`, and `Keyword_FormKey_ID`

These inferred references are intentionally not shown as Mermaid relationship lines in the ERD.
