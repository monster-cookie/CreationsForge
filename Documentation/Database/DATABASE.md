# Database

## Schema Source

The application uses a local SQLite cache. The schema is defined by embedded DbUp scripts in
`SFRecordCompareEngine.Migrations/Sql`:

- `001_CreatePluginSchema.sql` creates the application tables, keys, indexes, and initial constraints.
- `002_AddPluginRecordCount.sql` adds `Plugins.RecordCount`.

DbUp creates and owns its `SchemaVersions` migration-history table. `SchemaVersions` is the migration state source of
truth. The application does not define a hardcoded schema-version constant.

The application schema contains 12 tables:

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
- `FormKey_ID`

The `ModKey_*` columns identify the plugin containing the imported record. `FormKey_ID` identifies the record for
cross-plugin lookup. Multiple plugins can therefore store rows with the same `FormKey_ID`.

Each typed record table declares:

- A foreign key from `ModKey_Name`, `ModKey_Type`, and `ModKey_FileName` to the `Plugins` primary key.
- `ON DELETE CASCADE` for its `Plugins` foreign key.
- `CHECK (FormKey_ID >= 0)`.
- A non-unique `FormKey_ID` index for cross-plugin comparison lookup.

The typed tables also store imported record headers, including `EditorID`, `FormVersion`, `StarfieldMajorRecordFlags`,
`Version2`, `VersionControl`, and `ImportedAtUTC`.

## Tables

### Plugins

Stores plugin metadata and import status.

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
- `RecordCount` was added by `002_AddPluginRecordCount.sql`, defaults to `0`, and must be greater than or equal to `0`.

### PluginMasterReferences

Stores relationships between plugins and the masters declared in their headers.

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

Record-specific column:

- `AddToListFormKey` is nullable text.

### FormListItems

Stores item references inside form lists.

Primary key:

- `ModKey_Name`, `ModKey_Type`, `ModKey_FileName`, and `FormKey_ID`
- `Item_ModKey_Name`, `Item_ModKey_Type`, `Item_ModKey_FileName`, and `Item_FormKey_ID`
- `Item_Index`

Foreign key:

- `ModKey_Name`, `ModKey_Type`, `ModKey_FileName`, and `FormKey_ID` reference the `FormList` primary key with
  `ON DELETE CASCADE`.

Indexes:

- `IX_FormListItems_Item_FormKey_ID_ModKey_FormKey_ID` on `Item_FormKey_ID`, the owning plugin key columns, and
  `FormKey_ID`
- `IX_FormListItems_Item_Index` on `Item_Index`

Constraints:

- All columns are `NOT NULL`.
- `FormKey_ID` must be greater than or equal to `0`.
- `Item_FormKey_ID` must be greater than or equal to `0`.

`Item_Index` preserves source enumeration order. The primary key allows duplicate item references to remain separate
rows when they occur at different indexes.

### GameSetting

Stores Starfield `GMST` record detail rows. The common typed record key, foreign key, index, and non-negative
`FormKey_ID` constraint apply.

Record-specific constraints:

- `IsCompressed` is `NOT NULL` and must be `0` or `1`.
- `IsDeleted` is `NOT NULL` and must be `0` or `1`.

`RawData` and `XALG` remain persisted as diagnostic fields but are not shown in the comparison workspace.

### Global

Stores Starfield `GLOB` record detail rows. The common typed record key, foreign key, index, and non-negative
`FormKey_ID` constraint apply.

### MiscItem

Stores Starfield `MISC` record detail rows. The common typed record key, foreign key, index, and non-negative
`FormKey_ID` constraint apply.

### Keyword

Stores Starfield `KYWD` record detail rows. The common typed record key, foreign key, index, and non-negative
`FormKey_ID` constraint apply.

### NPC

Stores Starfield `NPC_` record detail rows. The common typed record key, foreign key, index, and non-negative
`FormKey_ID` constraint apply.

### ActorValueInformation

Stores Starfield `AVIF` record detail rows. The common typed record key, foreign key, index, and non-negative
`FormKey_ID` constraint apply.

### MagicEffect

Stores Starfield `MGEF` record detail rows. The common typed record key, foreign key, index, and non-negative
`FormKey_ID` constraint apply.

### Perk

Stores Starfield `PERK` record detail rows. The common typed record key, foreign key, index, and non-negative
`FormKey_ID` constraint apply.

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

These references are intentionally not shown as Mermaid relationship lines in the ERD.

## Record Comparison Lookup

The schema supports locating typed rows for the same record across plugins. For example:

```sql
SELECT *
FROM FormList
WHERE FormKey_ID = 0x0003F551;
```

This can return rows from multiple plugins. Each result keeps its containing plugin's `ModKey` columns, while the
shared `FormKey_ID` identifies the record being compared.

Comparison queries should filter typed tables by `FormKey_ID` and use plugin metadata for load-order sorting. Do not
add additional origin-plugin columns or persist `FormKey` as a second `ModKey` tuple for this workflow.

## Repository Boundary

Repositories use NPoco database models with `[TableName]`, `[PrimaryKey]`, and `[Column]` attributes. Repositories
translate DTOs to database models and should not own business workflow, UI behavior, or logging decisions.

Runtime SQL values should be parameterized. Existing query methods in repositories use NPoco parameters for runtime
values.
