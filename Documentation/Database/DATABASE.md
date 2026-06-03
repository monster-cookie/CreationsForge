# Database

## Schema Source

The application uses a local SQLite cache. The schema is defined by embedded DbUp scripts in
`SFRecordCompareEngine.Migrations/Sql`:

- `001_CreatePluginSchema.sql` creates the application tables, keys, indexes, and constraints.

DbUp creates and owns its `SchemaVersions` migration-history table. `SchemaVersions` is the migration state source of
truth. The application does not define a hardcoded schema-version constant.

The application schema contains 18 tables:

- `Plugins`
- `PluginMasterReferences`
- `FormList`
- `FormListItems`
- `GameSetting`
- `Global`
- `MiscItem`
- `Keyword`
- `NPC`
- `ActorValueInformation`
- `MagicEffect`
- `Perk`
- `PerkRanks`
- `PerkRankEffects`
- `PerkBackgroundSkills`
- `ScriptingAdapters`
- `ScriptingAdapterProperties`
- `ScriptingAdapterPropertyListItems`

See [ERD.md](ERD.md) for the relationship diagram.

## Database Location

`SqliteDatabaseOptions` defines the default database location:

- Directory: `ApplicationConfigurationStore.DefaultApplicationDataDirectory`
- File name: `SFRecordCompareEngine.sqlite`
- Linux full path: `~/.SFRecordCompareEngine/SFRecordCompareEngine.sqlite`
- Linux log directory: `~/.SFRecordCompareEngine/Logs`
- Other platforms full path: `<CommonApplicationData>/SFRecordCompareEngine/SFRecordCompareEngine.sqlite`
- Other platforms log directory: `<CommonApplicationData>/SFRecordCompareEngine/Logs`

`ApplicationConfigurationStore.DefaultApplicationDataDirectory` uses the current user's profile directory on Linux
and `Environment.SpecialFolder.CommonApplicationData` on other platforms.

## Connection Behavior

`SqliteConnectionFactory.OpenDatabase` creates the database directory, builds a SQLite connection string, and returns
an NPoco `IDatabase`.

Connection settings include:

- SQLite foreign keys enabled in the connection string.
- WAL journal mode.
- Pooling disabled.
- `PRAGMA foreign_keys = ON` executed after opening the NPoco database.

`DatabaseMigrationRunner` also disables pooling while DbUp applies migrations.

## Common Typed Record Shape

The typed record tables `FormList`, `GameSetting`, `Global`, `MiscItem`, `Keyword`, `NPC`, `ActorValueInformation`,
`MagicEffect`, and `Perk` store imported Starfield record details. Each uses this composite primary key:

- `ModKey_Name`
- `ModKey_Type`
- `ModKey_FileName`
- `FormKey_ModKey_Name`
- `FormKey_ModKey_Type`
- `FormKey_ModKey_FileName`
- `FormKey_ID`

The `ModKey_*` columns identify the plugin containing the imported record row. The `FormKey_ModKey_*` columns plus
`FormKey_ID` identify the record's origin `FormKey`. This lets comparison queries group true overrides while keeping
unrelated records with the same local numeric ID separate.

Application schema columns must store `ModKey` and `FormKey` values as primitive component columns. `ModKey` values
must use `*_ModKey_Name`, `*_ModKey_Type`, and `*_ModKey_FileName`. `FormKey` values must use those same referenced
`ModKey` components plus `*_FormKey_ID`. Raw string storage of `FormKey` or `ModKey` is not an allowed schema pattern
for new application-schema columns. Inferred references remain documented separately from declared SQLite foreign keys.

Each typed record table declares:

- A foreign key from `ModKey_Name`, `ModKey_Type`, and `ModKey_FileName` to the `Plugins` primary key.
- `ON DELETE CASCADE` for its `Plugins` foreign key.
- `CHECK (FormKey_ID >= 0)`.
- A non-unique full-origin `FormKey` index for cross-plugin comparison lookup.

Every typed record table contains these common columns:

- `ModKey_Name` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `ModKey_Type` (`INTEGER`, `NOT NULL`, primary key, foreign key)
- `ModKey_FileName` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `FormKey_ModKey_Name` (`TEXT`, `NOT NULL`, primary key)
- `FormKey_ModKey_Type` (`INTEGER`, `NOT NULL`, primary key)
- `FormKey_ModKey_FileName` (`TEXT`, `NOT NULL`, primary key)
- `FormKey_ID` (`INTEGER`, `NOT NULL`, primary key)
- `EditorID` (`TEXT`, `NOT NULL`)
- `FormVersion` (`INTEGER`, `NOT NULL`)
- `StarfieldMajorRecordFlags` (`INTEGER`, `NOT NULL`)
- `Version2` (`INTEGER`, `NOT NULL`)
- `VersionControl` (`INTEGER`, `NOT NULL`)
- `ImportedAtUTC` (`TEXT`, `NOT NULL`)

The table sections below list the additional record-specific columns. Together with this common list, they document
the complete persisted shape of each typed record table.

## Tables

### Plugins

Stores plugin metadata and import status.

Columns:

- `ModKey_Name` (`TEXT`, `NOT NULL`, primary key)
- `ModKey_Type` (`INTEGER`, `NOT NULL`, primary key)
- `ModKey_FileName` (`TEXT`, `NOT NULL`, primary key)
- `LoadOrderIndex` (`INTEGER`, `NOT NULL`)
- `Enabled` (`INTEGER`, `NOT NULL`, defaults to `1`)
- `ExistsOnDisk` (`INTEGER`, `NOT NULL`, defaults to `1`)
- `ImportState` (`TEXT`, `NOT NULL`, defaults to `Current`)
- `HeaderFlags` (`INTEGER`, `NOT NULL`)
- `FormVersion` (`INTEGER`, `NOT NULL`)
- `Author` (`TEXT`, `NOT NULL`)
- `Branch` (`TEXT`, `NOT NULL`)
- `InteriorCellCount` (`INTEGER`, `NOT NULL`)
- `SourceLastWriteUTCTicks` (`INTEGER`, `NOT NULL`)
- `SourceFileSizeBytes` (`INTEGER`, `NOT NULL`)
- `LastCheckedUTC` (`TEXT`, `NOT NULL`)
- `LastImportedUTC` (`TEXT`, nullable)
- `InvalidatedAtUTC` (`TEXT`, nullable)
- `RecordCount` (`INTEGER`, `NOT NULL`, defaults to `0`)

Primary key:

- `ModKey_Name`, `ModKey_Type`, and `ModKey_FileName`

Indexes:

- `IX_Plugins_LoadOrderIndex` on `LoadOrderIndex`
- `IX_Plugins_ImportState` on `ImportState`
- `IX_Plugins_SourceFingerprint` on `SourceLastWriteUTCTicks` and `SourceFileSizeBytes`

Constraints:

- All columns are `NOT NULL` except `LastImportedUTC` and `InvalidatedAtUTC`.
- `Enabled` defaults to `1` and must be `0` or `1`.
- `ExistsOnDisk` defaults to `1` and must be `0` or `1`.
- `ImportState` defaults to `Current` and must be `Current`, `Changed`, `Missing`, `Failed`, or `Unsupported`.
- `RecordCount` is created by `001_CreatePluginSchema.sql`, defaults to `0`, and must be greater than or equal to `0`.

### PluginMasterReferences

Stores relationships between plugins and the masters declared in their headers.

Columns:

- `Master_ModKey_Name` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `Master_ModKey_Type` (`INTEGER`, `NOT NULL`, primary key, foreign key)
- `Master_ModKey_FileName` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `Plugin_ModKey_Name` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `Plugin_ModKey_Type` (`INTEGER`, `NOT NULL`, primary key, foreign key)
- `Plugin_ModKey_FileName` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `ImportedAtUTC` (`TEXT`, `NOT NULL`)

Primary key:

- `Master_ModKey_Name`, `Master_ModKey_Type`, and `Master_ModKey_FileName`
- `Plugin_ModKey_Name`, `Plugin_ModKey_Type`, and `Plugin_ModKey_FileName`

Foreign keys:

- `Master_ModKey_Name`, `Master_ModKey_Type`, and `Master_ModKey_FileName` reference the `Plugins` primary key with
  `ON DELETE CASCADE`.
- `Plugin_ModKey_Name`, `Plugin_ModKey_Type`, and `Plugin_ModKey_FileName` reference the `Plugins` primary key with
  `ON DELETE CASCADE`.

Indexes:

- `IX_PluginMasterReferences_MasterModKey` on the master-plugin key columns
- `IX_PluginMasterReferences_PluginModKey` on the declaring-plugin key columns

Constraints:

- All columns are `NOT NULL`.
- The composite primary key prevents duplicate relationship edges.

Master load-order sorting is derived from `Plugins.LoadOrderIndex` when relationships are read.

### FormList

Stores Starfield `FLST` record detail rows. The common typed record key, foreign key, index, and non-negative
`FormKey_ID` constraint apply.

Additional record-specific column:

- `AddToListFormKey` (`TEXT`, nullable)

### FormListItems

Stores item references inside form lists.

Columns:

- `ModKey_Name` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `ModKey_Type` (`INTEGER`, `NOT NULL`, primary key, foreign key)
- `ModKey_FileName` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `FormKey_ModKey_Name` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `FormKey_ModKey_Type` (`INTEGER`, `NOT NULL`, primary key, foreign key)
- `FormKey_ModKey_FileName` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `FormKey_ID` (`INTEGER`, `NOT NULL`, primary key, foreign key)
- `Item_ModKey_Name` (`TEXT`, `NOT NULL`, primary key)
- `Item_ModKey_Type` (`INTEGER`, `NOT NULL`, primary key)
- `Item_ModKey_FileName` (`TEXT`, `NOT NULL`, primary key)
- `Item_FormKey_ID` (`INTEGER`, `NOT NULL`, primary key)
- `Item_Index` (`INTEGER`, `NOT NULL`, primary key)
- `ImportedAtUTC` (`TEXT`, `NOT NULL`)

Primary key:

- `ModKey_Name`, `ModKey_Type`, `ModKey_FileName`, `FormKey_ModKey_Name`, `FormKey_ModKey_Type`,
  `FormKey_ModKey_FileName`, and `FormKey_ID`
- `Item_ModKey_Name`, `Item_ModKey_Type`, `Item_ModKey_FileName`, and `Item_FormKey_ID`
- `Item_Index`

Foreign key:

- `ModKey_Name`, `ModKey_Type`, `ModKey_FileName`, `FormKey_ModKey_Name`, `FormKey_ModKey_Type`,
  `FormKey_ModKey_FileName`, and `FormKey_ID` reference the `FormList` primary key with `ON DELETE CASCADE`.

Indexes:

- `IX_FormListItems_FormList` on the owning plugin key columns plus the parent origin `FormKey` columns
- `IX_FormListItems_Item_FormKey` on the item `ModKey` columns plus `Item_FormKey_ID`
- `IX_FormListItems_Item_Index` on `Item_Index`

Constraints:

- All columns are `NOT NULL`.
- `FormKey_ID` must be greater than or equal to `0`.
- `Item_FormKey_ID` must be greater than or equal to `0`.
- `Item_Index` must be greater than or equal to `0`.

`Item_Index` preserves source enumeration order. The primary key allows duplicate item references to remain separate
rows when they occur at different indexes.

### GameSetting

Stores Starfield `GMST` record detail rows. The common typed record key, foreign key, index, and non-negative
`FormKey_ID` constraint apply.

Additional record-specific columns:

- `SettingType` (`TEXT`, nullable)
- `Data` (`TEXT`, nullable)
- `RawData` (`REAL`, nullable)
- `XALG` (`INTEGER`, nullable)
- `IsCompressed` (`INTEGER`, `NOT NULL`)
- `IsDeleted` (`INTEGER`, `NOT NULL`)

Record-specific constraints:

- `IsCompressed` is `NOT NULL` and must be `0` or `1`.
- `IsDeleted` is `NOT NULL` and must be `0` or `1`.

`RawData` and `XALG` remain persisted as diagnostic fields but are not shown in the comparison workspace.

### Global

Stores Starfield `GLOB` record detail rows. The common typed record key, foreign key, index, and non-negative
`FormKey_ID` constraint apply.

Additional record-specific column:

- `Data` (`REAL`, nullable)

### MiscItem

Stores Starfield `MISC` record detail rows. The common typed record key, foreign key, index, and non-negative
`FormKey_ID` constraint apply.

Additional record-specific columns:

- `Name` (`TEXT`, nullable)
- `ShortName` (`TEXT`, nullable)
- `Value` (`INTEGER`, nullable)
- `Weight` (`REAL`, nullable)

### Keyword

Stores Starfield `KYWD` record detail rows. The common typed record key, foreign key, index, and non-negative
`FormKey_ID` constraint apply.

Additional record-specific columns:

- `Name` (`TEXT`, nullable)
- `Color` (`TEXT`, `NOT NULL`)
- `Type` (`TEXT`, `NOT NULL`)
- `Notes` (`TEXT`, nullable)
- `FlashLinkageName` (`TEXT`, nullable)
- `AttractionRuleFormKey` (`TEXT`, nullable)

### NPC

Stores Starfield `NPC_` record detail rows. The common typed record key, foreign key, index, and non-negative
`FormKey_ID` constraint apply.

Additional record-specific columns:

- `Name` (`TEXT`, nullable)
- `ShortName` (`TEXT`, nullable)
- `LongName` (`TEXT`, nullable)
- `DispositionBase` (`INTEGER`, `NOT NULL`)
- `Aggression` (`TEXT`, `NOT NULL`)
- `Confidence` (`TEXT`, `NOT NULL`)
- `EnergyLevel` (`INTEGER`, `NOT NULL`)
- `Responsibility` (`TEXT`, `NOT NULL`)
- `Assistance` (`TEXT`, `NOT NULL`)
- `GearedUpWeapons` (`INTEGER`, `NOT NULL`)
- `HeightMin` (`REAL`, `NOT NULL`)
- `HeightMax` (`REAL`, `NOT NULL`)
- `SkinToneIndex` (`INTEGER`, nullable)
- `Pronoun` (`TEXT`, nullable)
- `VoiceFormKey` (`TEXT`, nullable)
- `RaceFormKey` (`TEXT`, nullable)
- `CombatOverridePackageListFormKey` (`TEXT`, nullable)
- `CombatStyleFormKey` (`TEXT`, nullable)
- `DefaultPackageListFormKey` (`TEXT`, nullable)
- `CrimeFactionFormKey` (`TEXT`, nullable)

### ActorValueInformation

Stores Starfield `AVIF` record detail rows. The common typed record key, foreign key, index, and non-negative
`FormKey_ID` constraint apply.

Additional record-specific columns:

- `Name` (`TEXT`, nullable)
- `Abbreviation` (`TEXT`, nullable)
- `ContextNotes` (`TEXT`, nullable)
- `DefaultValue` (`REAL`, nullable)
- `Flags` (`TEXT`, nullable)
- `Type` (`TEXT`, nullable)
- `Min` (`REAL`, nullable)
- `Max` (`REAL`, nullable)

### MagicEffect

Stores Starfield `MGEF` record detail rows. The common typed record key, foreign key, index, and non-negative
`FormKey_ID` constraint apply.

Additional record-specific columns:

- `Name` (`TEXT`, nullable)
- `Description` (`TEXT`, nullable)
- `Flags` (`TEXT`, `NOT NULL`)
- `CastType` (`TEXT`, nullable)
- `TargetType` (`TEXT`, nullable)
- `ActorValue2FormKey` (`TEXT`, nullable)
- `ResistValueFormKey` (`TEXT`, nullable)
- `PerkToApplyFormKey` (`TEXT`, nullable)
- `EquipAbilityFormKey` (`TEXT`, nullable)
- `ExplosionFormKey` (`TEXT`, nullable)
- `CastingArtFormKey` (`TEXT`, nullable)
- `HitEffectArtFormKey` (`TEXT`, nullable)
- `HitShaderFormKey` (`TEXT`, nullable)
- `ImageSpaceModifierFormKey` (`TEXT`, nullable)
- `ImpactDataFormKey` (`TEXT`, nullable)
- `ProjectileFormKey` (`TEXT`, nullable)

### Perk

Stores Starfield `PERK` record detail rows. The common typed record key, foreign key, index, and non-negative
`FormKey_ID` constraint apply.

Additional record-specific columns:

- `Name` (`TEXT`, nullable)
- `Description` (`TEXT`, nullable)
- `Flags` (`TEXT`, `NOT NULL`)
- `SkillGroup` (`TEXT`, nullable)
- `CrewAssignment` (`TEXT`, nullable)
- `PerkIcon` (`TEXT`, nullable)
- `Category` (`TEXT`, nullable)
- `Restriction_ModKey_Name` (`TEXT`, nullable)
- `Restriction_ModKey_Type` (`INTEGER`, nullable)
- `Restriction_ModKey_FileName` (`TEXT`, nullable)
- `Restriction_FormKey_ID` (`INTEGER`, nullable)
- `Training_ModKey_Name` (`TEXT`, nullable)
- `Training_ModKey_Type` (`INTEGER`, nullable)
- `Training_ModKey_FileName` (`TEXT`, nullable)
- `Training_FormKey_ID` (`INTEGER`, nullable)
- `MajorFlags` (`TEXT`, nullable)

`Category` maps Mutagen's generated `Categroy` Perk property. The restriction and training columns are direct Perk
references, not declared SQLite foreign keys. Each nullable reference tuple must be either fully populated or fully
`NULL`. `MajorFlags` is a direct Perk field, not a record reference.

### PerkRanks

Stores ordered rank rows inside Starfield `PERK` records.

Columns:

- `ModKey_Name` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `ModKey_Type` (`INTEGER`, `NOT NULL`, primary key, foreign key)
- `ModKey_FileName` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `FormKey_ModKey_Name` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `FormKey_ModKey_Type` (`INTEGER`, `NOT NULL`, primary key, foreign key)
- `FormKey_ModKey_FileName` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `FormKey_ID` (`INTEGER`, `NOT NULL`, primary key, foreign key)
- `Rank_Index` (`INTEGER`, `NOT NULL`, primary key)
- `Description` (`TEXT`, nullable)
- `UnknownStatic_ModKey_Name` (`TEXT`, nullable)
- `UnknownStatic_ModKey_Type` (`INTEGER`, nullable)
- `UnknownStatic_ModKey_FileName` (`TEXT`, nullable)
- `UnknownStatic_FormKey_ID` (`INTEGER`, nullable)
- `ConditionCount` (`INTEGER`, `NOT NULL`)
- `ActivityCount` (`INTEGER`, `NOT NULL`)
- `ImportedAtUTC` (`TEXT`, `NOT NULL`)

Primary key:

- `ModKey_Name`, `ModKey_Type`, `ModKey_FileName`, `FormKey_ModKey_Name`, `FormKey_ModKey_Type`,
  `FormKey_ModKey_FileName`, `FormKey_ID`, and `Rank_Index`

Foreign key:

- `ModKey_Name`, `ModKey_Type`, `ModKey_FileName`, `FormKey_ModKey_Name`, `FormKey_ModKey_Type`,
  `FormKey_ModKey_FileName`, and `FormKey_ID` reference the `Perk` primary key with `ON DELETE CASCADE`.

Indexes:

- `IX_PerkRanks_Perk` on the owning Perk key columns
- `IX_PerkRanks_RankIndex` on `Rank_Index`

Constraints:

- All columns are `NOT NULL` except `Description` and the nullable unknown-static reference tuple.
- `FormKey_ID`, `Rank_Index`, `ConditionCount`, and `ActivityCount` must be greater than or equal to `0`.
- The unknown-static reference tuple must be either fully populated or fully `NULL`.
- `UnknownStatic_FormKey_ID` must be `NULL` or greater than or equal to `0`.

`Rank_Index` preserves source rank order. `ConditionCount` and `ActivityCount` preserve summary counts for nested rank
data that is not otherwise normalized in this schema.

### PerkRankEffects

Stores ordered supported effect fields inside Perk ranks.

Columns:

- `ModKey_Name` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `ModKey_Type` (`INTEGER`, `NOT NULL`, primary key, foreign key)
- `ModKey_FileName` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `FormKey_ModKey_Name` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `FormKey_ModKey_Type` (`INTEGER`, `NOT NULL`, primary key, foreign key)
- `FormKey_ModKey_FileName` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `FormKey_ID` (`INTEGER`, `NOT NULL`, primary key, foreign key)
- `Rank_Index` (`INTEGER`, `NOT NULL`, primary key, foreign key)
- `Effect_Index` (`INTEGER`, `NOT NULL`, primary key)
- `MutagenObjectType` (`TEXT`, `NOT NULL`)
- `Rank` (`INTEGER`, `NOT NULL`)
- `Priority` (`INTEGER`, `NOT NULL`)
- `PerkEntryID` (`INTEGER`, nullable)
- `Flags` (`TEXT`, nullable)
- `ButtonLabel` (`TEXT`, nullable)
- `ConditionCount` (`INTEGER`, `NOT NULL`)
- `EntryPoint` (`TEXT`, nullable)
- `PerkConditionTabCount` (`INTEGER`, nullable)
- `Modification` (`TEXT`, nullable)
- `Value` (`REAL`, nullable)
- `ImportedAtUTC` (`TEXT`, `NOT NULL`)

Primary key:

- `ModKey_Name`, `ModKey_Type`, `ModKey_FileName`, `FormKey_ModKey_Name`, `FormKey_ModKey_Type`,
  `FormKey_ModKey_FileName`, `FormKey_ID`, `Rank_Index`, and `Effect_Index`

Foreign key:

- `ModKey_Name`, `ModKey_Type`, `ModKey_FileName`, `FormKey_ModKey_Name`, `FormKey_ModKey_Type`,
  `FormKey_ModKey_FileName`, `FormKey_ID`, and `Rank_Index` reference the `PerkRanks` primary key with
  `ON DELETE CASCADE`.

Indexes:

- `IX_PerkRankEffects_PerkRank` on the owning Perk rank key columns
- `IX_PerkRankEffects_EffectIndex` on `Effect_Index`

Constraints:

- `FormKey_ID`, `Rank_Index`, `Effect_Index`, `Rank`, `Priority`, and `ConditionCount` must be greater than or
  equal to `0`.
- `PerkEntryID` and `PerkConditionTabCount` must be `NULL` or greater than or equal to `0`.

Supported effect fields include common Perk effect fields and entry-point modify-value fields. Deeper effect-specific
payloads and nested conditions are not normalized in this schema.

### PerkBackgroundSkills

Stores ordered background skill references inside Starfield `PERK` records.

Columns:

- `ModKey_Name` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `ModKey_Type` (`INTEGER`, `NOT NULL`, primary key, foreign key)
- `ModKey_FileName` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `FormKey_ModKey_Name` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `FormKey_ModKey_Type` (`INTEGER`, `NOT NULL`, primary key, foreign key)
- `FormKey_ModKey_FileName` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `FormKey_ID` (`INTEGER`, `NOT NULL`, primary key, foreign key)
- `Skill_ModKey_Name` (`TEXT`, `NOT NULL`)
- `Skill_ModKey_Type` (`INTEGER`, `NOT NULL`)
- `Skill_ModKey_FileName` (`TEXT`, `NOT NULL`)
- `Skill_FormKey_ID` (`INTEGER`, `NOT NULL`)
- `Skill_Index` (`INTEGER`, `NOT NULL`, primary key)
- `ImportedAtUTC` (`TEXT`, `NOT NULL`)

Primary key:

- `ModKey_Name`, `ModKey_Type`, `ModKey_FileName`, `FormKey_ModKey_Name`, `FormKey_ModKey_Type`,
  `FormKey_ModKey_FileName`, `FormKey_ID`, and `Skill_Index`

Foreign key:

- `ModKey_Name`, `ModKey_Type`, `ModKey_FileName`, `FormKey_ModKey_Name`, `FormKey_ModKey_Type`,
  `FormKey_ModKey_FileName`, and `FormKey_ID` reference the `Perk` primary key with `ON DELETE CASCADE`.

Indexes:

- `IX_PerkBackgroundSkills_Perk` on the owning Perk key columns
- `IX_PerkBackgroundSkills_Skill_FormKey` on the referenced skill `FormKey` columns
- `IX_PerkBackgroundSkills_SkillIndex` on `Skill_Index`

Constraints:

- All columns are `NOT NULL`.
- `FormKey_ID`, `Skill_FormKey_ID`, and `Skill_Index` must be greater than or equal to `0`.

`Skill_Index` preserves source background skill order. The referenced skill columns contain inferred Perk references,
not declared SQLite foreign keys.

### ScriptingAdapters

Stores one VMAD script row attached to an already-supported typed record.

Columns:

- `ModKey_Name` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `ModKey_Type` (`INTEGER`, `NOT NULL`, primary key, foreign key)
- `ModKey_FileName` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `RecordType` (`TEXT`, `NOT NULL`, primary key)
- `FormKey_ModKey_Name` (`TEXT`, `NOT NULL`, primary key)
- `FormKey_ModKey_Type` (`INTEGER`, `NOT NULL`, primary key)
- `FormKey_ModKey_FileName` (`TEXT`, `NOT NULL`, primary key)
- `FormKey_ID` (`INTEGER`, `NOT NULL`, primary key)
- `Name` (`TEXT`, `NOT NULL`, primary key)
- `Script_Index` (`INTEGER`, `NOT NULL`)
- `ImportedAtUTC` (`TEXT`, `NOT NULL`)

Primary key:

- `ModKey_Name`, `ModKey_Type`, `ModKey_FileName`, `RecordType`, `FormKey_ModKey_Name`,
  `FormKey_ModKey_Type`, `FormKey_ModKey_FileName`, `FormKey_ID`, and `Name`

Foreign keys:

- `ModKey_Name`, `ModKey_Type`, and `ModKey_FileName` reference the `Plugins` primary key with
  `ON DELETE CASCADE`.

Indexes:

- `IX_ScriptingAdapters_RecordLookup` on `RecordType` plus the origin `FormKey` columns
- `IX_ScriptingAdapters_ScriptIndex` on `Script_Index`

Constraints:

- All columns are `NOT NULL`.
- `FormKey_ID` must be greater than or equal to `0`.
- `Script_Index` must be greater than or equal to `0`.

`RecordType` is required because the shared VMAD child tables serve multiple typed parent tables. The full origin
`FormKey` is required because `FormKey_ID` is not globally unique across origin plugins.

### ScriptingAdapterProperties

Stores one VMAD property row for a script attached to an already-supported typed record.

Columns:

- `ModKey_Name` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `ModKey_Type` (`INTEGER`, `NOT NULL`, primary key, foreign key)
- `ModKey_FileName` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `RecordType` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `FormKey_ModKey_Name` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `FormKey_ModKey_Type` (`INTEGER`, `NOT NULL`, primary key, foreign key)
- `FormKey_ModKey_FileName` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `FormKey_ID` (`INTEGER`, `NOT NULL`, primary key, foreign key)
- `ScriptingAdapter_Name` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `Property_Index` (`INTEGER`, `NOT NULL`, primary key)
- `Name` (`TEXT`, `NOT NULL`)
- `MutagenObjectType` (`TEXT`, `NOT NULL`)
- `Data_Bool` (`INTEGER`, nullable)
- `Data_Int` (`INTEGER`, nullable)
- `Data_Float` (`REAL`, nullable)
- `Data_String` (`TEXT`, nullable)
- `Object_ModKey_Name` (`TEXT`, nullable)
- `Object_ModKey_Type` (`INTEGER`, nullable)
- `Object_ModKey_FileName` (`TEXT`, nullable)
- `Object_FormKey_ID` (`INTEGER`, nullable)
- `Object_Alias` (`INTEGER`, nullable)
- `Object_Unused` (`INTEGER`, nullable)
- `ImportedAtUTC` (`TEXT`, `NOT NULL`)

Primary key:

- `ModKey_Name`, `ModKey_Type`, `ModKey_FileName`, `RecordType`, `FormKey_ModKey_Name`,
  `FormKey_ModKey_Type`, `FormKey_ModKey_FileName`, `FormKey_ID`, `ScriptingAdapter_Name`, and
  `Property_Index`

Foreign keys:

- `ModKey_Name`, `ModKey_Type`, `ModKey_FileName`, `RecordType`, `FormKey_ModKey_Name`,
  `FormKey_ModKey_Type`, `FormKey_ModKey_FileName`, `FormKey_ID`, and `ScriptingAdapter_Name` reference the
  `ScriptingAdapters` primary key with `ON DELETE CASCADE`.

Indexes:

- `IX_ScriptingAdapterProperties_RecordLookup` on `RecordType` plus the origin `FormKey` columns
- `IX_ScriptingAdapterProperties_PropertyIndex` on `Property_Index`
- `IX_ScriptingAdapterProperties_ObjectLookup` on the object `ModKey` columns plus `Object_FormKey_ID`

Constraints:

- `FormKey_ID` must be greater than or equal to `0`.
- `Property_Index` must be greater than or equal to `0`.
- `Data_Bool` must be `0`, `1`, or `NULL`.
- `Object_FormKey_ID` must be `NULL` or greater than or equal to `0`.

Supported property shapes in this table are:

- `ScriptProperty`
- `ScriptBoolProperty`
- `ScriptIntProperty`
- `ScriptFloatProperty`
- `ScriptStringProperty`
- `ScriptObjectProperty`
- list-property parents for the supported VMAD list types

List values are stored in `ScriptingAdapterPropertyListItems`, not as JSON.

### ScriptingAdapterPropertyListItems

Stores one VMAD list element row for a supported list-type script property.

Columns:

- `ModKey_Name` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `ModKey_Type` (`INTEGER`, `NOT NULL`, primary key, foreign key)
- `ModKey_FileName` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `RecordType` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `FormKey_ModKey_Name` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `FormKey_ModKey_Type` (`INTEGER`, `NOT NULL`, primary key, foreign key)
- `FormKey_ModKey_FileName` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `FormKey_ID` (`INTEGER`, `NOT NULL`, primary key, foreign key)
- `ScriptingAdapter_Name` (`TEXT`, `NOT NULL`, primary key, foreign key)
- `Property_Index` (`INTEGER`, `NOT NULL`, primary key, foreign key)
- `ListItem_Index` (`INTEGER`, `NOT NULL`, primary key)
- `MutagenObjectType` (`TEXT`, `NOT NULL`)
- `Data_Bool` (`INTEGER`, nullable)
- `Data_Int` (`INTEGER`, nullable)
- `Data_Float` (`REAL`, nullable)
- `Data_String` (`TEXT`, nullable)
- `Object_ModKey_Name` (`TEXT`, nullable)
- `Object_ModKey_Type` (`INTEGER`, nullable)
- `Object_ModKey_FileName` (`TEXT`, nullable)
- `Object_FormKey_ID` (`INTEGER`, nullable)
- `Object_Alias` (`INTEGER`, nullable)
- `Object_Unused` (`INTEGER`, nullable)
- `ImportedAtUTC` (`TEXT`, `NOT NULL`)

Primary key:

- `ModKey_Name`, `ModKey_Type`, `ModKey_FileName`, `RecordType`, `FormKey_ModKey_Name`,
  `FormKey_ModKey_Type`, `FormKey_ModKey_FileName`, `FormKey_ID`, `ScriptingAdapter_Name`, `Property_Index`, and
  `ListItem_Index`

Foreign keys:

- `ModKey_Name`, `ModKey_Type`, `ModKey_FileName`, `RecordType`, `FormKey_ModKey_Name`,
  `FormKey_ModKey_Type`, `FormKey_ModKey_FileName`, `FormKey_ID`, `ScriptingAdapter_Name`, and `Property_Index`
  reference the `ScriptingAdapterProperties` primary key with `ON DELETE CASCADE`.

Indexes:

- `IX_ScriptingAdapterPropertyListItems_RecordLookup` on `RecordType` plus the origin `FormKey` columns
- `IX_ScriptingAdapterPropertyListItems_ListItemIndex` on `ListItem_Index`
- `IX_ScriptingAdapterPropertyListItems_ObjectLookup` on the object `ModKey` columns plus `Object_FormKey_ID`

Constraints:

- `FormKey_ID` must be greater than or equal to `0`.
- `Property_Index` must be greater than or equal to `0`.
- `ListItem_Index` must be greater than or equal to `0`.
- `Data_Bool` must be `0`, `1`, or `NULL`.
- `Object_FormKey_ID` must be `NULL` or greater than or equal to `0`.

Supported list item shapes are:

- `ScriptBoolListProperty`
- `ScriptIntListProperty`
- `ScriptFloatListProperty`
- `ScriptStringListProperty`
- `ScriptObjectListProperty`

## Inferred Relationships

The following columns carry record-reference data but do not declare SQLite foreign keys:

- `FormList.AddToListFormKey`
- `FormListItems.Item_ModKey_Name`, `Item_ModKey_Type`, `Item_ModKey_FileName`, and `Item_FormKey_ID`
- `Keyword.AttractionRuleFormKey`
- `NPC.VoiceFormKey`, `RaceFormKey`, `CombatOverridePackageListFormKey`, `CombatStyleFormKey`,
  `DefaultPackageListFormKey`, and `CrimeFactionFormKey`
- `MagicEffect.ActorValue2FormKey`, `ResistValueFormKey`, `PerkToApplyFormKey`, `EquipAbilityFormKey`,
  `ExplosionFormKey`, `CastingArtFormKey`, `HitEffectArtFormKey`, `HitShaderFormKey`, `ImageSpaceModifierFormKey`,
  `ImpactDataFormKey`, and `ProjectileFormKey`
- `Perk.Restriction_ModKey_Name`, `Restriction_ModKey_Type`, `Restriction_ModKey_FileName`, and
  `Restriction_FormKey_ID`
- `Perk.Training_ModKey_Name`, `Training_ModKey_Type`, `Training_ModKey_FileName`, and `Training_FormKey_ID`
- `PerkRanks.UnknownStatic_ModKey_Name`, `UnknownStatic_ModKey_Type`, `UnknownStatic_ModKey_FileName`, and
  `UnknownStatic_FormKey_ID`
- `PerkBackgroundSkills.Skill_ModKey_Name`, `Skill_ModKey_Type`, `Skill_ModKey_FileName`, and `Skill_FormKey_ID`
- Typed-record and VMAD `FormKey_ModKey_Name`, `FormKey_ModKey_Type`, `FormKey_ModKey_FileName`, and `FormKey_ID`
  persist origin `FormKey` identity but do not declare a SQLite foreign key to `Plugins`
- `ScriptingAdapters.RecordType` and the origin `FormKey` columns identify the owning typed record table row but do
  not declare a SQLite foreign key to a specific typed record table
- `ScriptingAdapterProperties.Object_ModKey_Name`, `Object_ModKey_Type`, `Object_ModKey_FileName`,
  and `Object_FormKey_ID`
- `ScriptingAdapterPropertyListItems.Object_ModKey_Name`, `Object_ModKey_Type`, `Object_ModKey_FileName`,
  and `Object_FormKey_ID`

These references are intentionally not shown as Mermaid relationship lines in the ERD.

## Record Comparison Lookup

The schema supports locating typed rows for the same record across plugins. For example:

```sql
SELECT *
FROM FormList
WHERE FormKey_ModKey_Name = 'BasePlugin'
  AND FormKey_ModKey_Type = 1
  AND FormKey_ModKey_FileName = 'BasePlugin.esm'
  AND FormKey_ID = 0x0003F551;
```

This can return rows from multiple containing plugins when those plugins override the same origin record. Each result
keeps its containing plugin's `ModKey` columns for display and load-order sorting.

Comparison queries must filter typed tables by the full origin `FormKey` tuple. Filtering by `FormKey_ID` alone can
incorrectly group unrelated records from different origin plugins that happen to share the same local numeric ID.

## Repository Boundary

Repositories use NPoco database models with `[TableName]`, `[PrimaryKey]`, and `[Column]` attributes. Repositories
translate DTOs to database models and should not own business workflow, UI behavior, or logging decisions.

Runtime SQL values should be parameterized. Existing query methods in repositories use NPoco parameters for runtime
values.
