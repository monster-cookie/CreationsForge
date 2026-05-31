# Database

## Database Location

`SqliteDatabaseOptions` defines the default database location:

- Directory: `ApplicationConfigurationStore.DefaultApplicationDataDirectory`
- File name: `SFRecordCompareEngine.sqlite`
- Full path: `<CommonApplicationData>/SFRecordCompareEngine/SFRecordCompareEngine.sqlite`
- Log directory: `<CommonApplicationData>/SFRecordCompareEngine/Logs`

`ApplicationConfigurationStore.DefaultApplicationDataDirectory` is based on
`Environment.SpecialFolder.CommonApplicationData`.

## Connection Behavior

`SqliteConnectionFactory.OpenDatabase` creates the database directory, builds a SQLite connection string, and returns
an NPoco `IDatabase`.

Connection settings include:

- SQLite foreign keys enabled in the connection string.
- WAL journal mode.
- Pooling disabled.
- `PRAGMA foreign_keys = ON` executed after opening the NPoco database.

## Migrations

Schema migrations are executed by DbUp through `DatabaseMigrationRunner`. SQL scripts are embedded resources from 
`SFRecordCompareEngine.Migrations/Sql`.

Current migration script:

- `001_CreatePluginSchema.sql`

DbUp's `SchemaVersions` table is the migration state source of truth. The application does not define a hardcoded 
schema-version constant.

`DatabaseSchemaInitializer.Initialize` logs schema initialization and delegates migration execution to 
`IDatabaseMigrationRunner`.

## Tables

### Plugins

Stores plugin metadata and import status.

Primary key:

- `ModKey_Name`
- `ModKey_Type`
- `ModKey_FileName`

Important columns:

- `LoadOrderIndex`
- `Enabled`
- `ExistsOnDisk`
- `ImportState`
- `HeaderFlags`
- `FormVersion`
- `Author`
- `Branch`
- `InteriorCellCount`
- `SourceLastWriteUTCTicks`
- `SourceFileSizeBytes`
- `LastCheckedUTC`
- `LastImportedUTC`
- `InvalidatedAtUTC`

Indexes support load-order, import-state, and source-fingerprint lookups.

### PluginMasterReferences

Stores relationships between plugins and the masters declared in their headers.

Primary key:

- `Master_ModKey_Name`, `Master_ModKey_Type`, and `Master_ModKey_FileName`
- `Plugin_ModKey_Name`, `Plugin_ModKey_Type`, and `Plugin_ModKey_FileName`

Foreign keys reference `Plugins` for both the declared master and the declaring plugin with cascade delete.

The table stores only relationship edges. Master load-order sorting is derived from `Plugins.LoadOrderIndex` when
relationships are read. The composite primary key prevents duplicate relationships.

### FormList

Stores Starfield `FLST` record detail rows.

Primary key:

- plugin key columns
- `FormKey_ID`

Foreign key:

- owning plugin key references `Plugins` with cascade delete.

The plugin key columns identify the plugin containing the imported row. `FormKey_ID` identifies the record for
cross-plugin lookup. Multiple plugins can therefore store rows with the same `FormKey_ID`.

### FormListItems

Stores item references inside form lists.

Primary key:

- owning plugin key columns
- `FormKey_ID`
- item plugin key columns
- `Item_FormKey_ID`
- `Item_Index`

Foreign key:

- owning plugin key plus `FormKey_ID` references `FormList` with cascade delete.

`Item_Index` preserves source enumeration order and allows duplicate references to remain separate rows. Reads for a
specific form list use `ORDER BY Item_Index`.

### GameSetting

Stores Starfield `GMST` record detail rows. Game settings use the same owning plugin key plus `FormKey_ID` primary key 
shape as `FormList`.

As with `FormList`, the plugin key columns identify the containing plugin and `FormKey_ID` supports cross-plugin record
lookup.

Game-setting rows do not include `TitleString` because Mutagen's Starfield game-setting records do not expose that
field. `RawData` and `XALG` remain persisted as diagnostic fields but are not shown in the comparison workspace.

### Additional Supported Record Tables

`Global`, `MiscObject`, `Keyword`, `NPC`, `ActorValueInformation`, `MagicEffect`, and `Perk` store Starfield `GLOB`,
`MISC`, `KYWD`, `NPC_`, `AVIF`, `MGEF`, and `PERK` typed detail rows.

Each table uses the same owning plugin key plus `FormKey_ID` primary key shape as `FormList` and `GameSetting`.
`FormKey_ID` indexes support cross-plugin comparison lookup. Clearly understood scalar fields and direct `FormKey`
references are stored as typed columns. English localized text is stored as nullable `TEXT`. Complex child objects are
not stored as JSON and remain deferred for normalized modeling.

## Record Comparison Lookup

The existing schema already supports locating typed rows for the same record across plugins. For example:

```sql
SELECT *
FROM FormList
WHERE FormKey_ID = 0x0003F551;
```

This can return rows from both `Starfield.esm` and `venworks-myexperiments.esm`. Each result keeps its containing
plugin's `ModKey` columns, while the shared `FormKey_ID` identifies the record being compared.

Comparison queries should filter typed tables by `FormKey_ID` and use plugin metadata for load-order sorting. Do not
add additional origin-plugin columns or persist `FormKey` as a second `ModKey` tuple for this workflow.

## Repository Boundary

Repositories use NPoco database models with `[TableName]`, `[PrimaryKey]`, and `[Column]` attributes. Repositories 
translate DTOs to database models and should not own business workflow, UI behavior, or logging decisions.

Runtime SQL values should be parameterized. Existing query methods in repositories use NPoco parameters for runtime
values.
