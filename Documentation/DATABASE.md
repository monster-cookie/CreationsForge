# Database

## Database Location

`SqliteDatabaseOptions` defines the default database location:

- Directory: `ApplicationConfigurationStore.DefaultApplicationDataDirectory`
- File name: `SFRecordCompareEngine.sqlite`
- Full path: `<CommonApplicationData>/SFRecordCompareEngine/SFRecordCompareEngine.sqlite`
- Log directory: `<CommonApplicationData>/SFRecordCompareEngine/Logs`

`ApplicationConfigurationStore.DefaultApplicationDataDirectory` is based on `Environment.SpecialFolder.CommonApplicationData`.

## Connection Behavior

`SqliteConnectionFactory.OpenDatabase` creates the database directory, builds a SQLite connection string, and returns an 
NPoco `IDatabase`.

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

Stores relationships between plugins and their header masters.

Primary key:

- child plugin key columns
- parent plugin key columns

Foreign keys reference `Plugins` for both child and parent plugins with cascade delete.

Indexes support parent lookup and ordering by parent load order. A unique index prevents duplicate master reference 
indexes per plugin.

### FormList

Stores Starfield `FLST` record detail rows.

Primary key:

- plugin key columns
- `FormKey_ID`

Foreign key:

- owning plugin key references `Plugins` with cascade delete.

### FormListItems

Stores item references inside form lists.

Primary key:

- owning plugin key columns
- item plugin key columns
- `FormKey_ID`

Foreign key:

- owning plugin key plus `FormKey_ID` references `FormList` with cascade delete.

### GameSetting

Stores planned `GMST` record detail fields. The table exists in the initial migration, but `GameSettingImporter` 
is not implemented.

## Repository Boundary

Repositories use NPoco database models with `[TableName]`, `[PrimaryKey]`, and `[Column]` attributes. Repositories 
translate DTOs to database models and should not own business workflow, UI behavior, or logging decisions.

Runtime SQL values should be parameterized. Existing query methods in repositories use NPoco parameters for runtime values.
