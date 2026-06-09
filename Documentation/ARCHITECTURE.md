# Architecture

## Layering

The solution is split into UI, console, core, game adapter, migrations, and tests.

`CreationsForge` is the cross-platform Avalonia presentation project. It owns views, view models,
presentation commands, and UI-specific coordination. It references Bootstrap and Core, but UI and MVVM code must
consume Core contracts and DTOs instead of Mutagen APIs or game-specific reader services directly.

`CreationsForge.Console` is the command-line harness. It references Bootstrap and Core. It owns command-line parsing,
terminal output, exit codes, and console-only registrations.

`CreationsForge.Bootstrap` owns shared app startup helpers for UI and CLI surfaces. It references Core, Migrations,
and the game projects so app surfaces can share common Autofac module registration and Serilog logging setup without
depending on each other.

`CreationsForge.Core` is game-agnostic only where the implemented behavior is truly shared. Core owns
configuration, database connection setup, schema initialization, common DTO identity shapes, importer contracts,
shared import orchestration, shared Mutagen primitive mapping, and repositories for the approved shared schema. Core
may reference shared Mutagen packages such as `Mutagen.Bethesda.Core`, but it must not reference game-specific Mutagen
packages.

`CreationsForge.Bethesda.Assets` owns UI-neutral Bethesda asset IO helpers, local-file resolution result DTOs, an
in-memory asset provider, archive-reader contracts, and temporary extraction session infrastructure. It does not
reference Avalonia, Mutagen, NPoco, game projects, or the database. Archive implementations are intended to be
read-only and preview-focused.

`CreationsForge.Starfield`, `CreationsForge.Fallout4`, and `CreationsForge.Skyrim` isolate
game-specific Mutagen packages, Autofac modules, reader services, and reader facade implementations. These projects
are the intended home for game-specific record mapping when Mutagen APIs or record/header fields diverge.
They also own game installation metadata discovery through Mutagen so Core does not return hardcoded or partial
game metadata. Mutagen plugin and record reads use `GameEnvironment.Typical.*(...).DataFolderPath` inside the game
adapter projects rather than persisted game metadata paths.

`CreationsForge.Migrations` contains DbUp infrastructure and embedded SQL scripts.

`CreationsForge.UnitTests` tests non-database logic only.

## Dependency Direction

- CreationsForge depends on Bootstrap and Core.
- Console depends on Bootstrap and Core.
- Bootstrap depends on Core, Migrations, Starfield, Fallout4, and Skyrim.
- Core depends on Assets for asset resolution DTOs, Migrations for migration execution, and shared Mutagen core
  primitives for game-agnostic DTO mapping.
- Assets has no project dependencies.
- Game projects depend on Core.
- Migrations does not depend on Core or game projects.
- UnitTests depend on Core and the console project for parser tests.

## Composition

`CreationsForge.Bootstrap` provides shared Autofac module registration.

- `CoreModule` registers configuration, SQLite options, connection factory, NPoco `IDatabase`, schema initializer,
  shared services, UI-neutral workflow services, shared typed importers, and shared repositories.
- `MigrationsModule` registers `DatabaseMigrationRunner`.
- Each game module registers that game's plugin reader service, plugin reader facade, record reader, and one
  `IGameImporter` wired to those readers.

`CreationsForge` builds a presentation container in `App` by calling Bootstrap and then registering presentation-only
windows, views, and view models. `CreationsForge.Console` builds a CLI container in `Program` by calling Bootstrap and
then registering `GameArgumentParser`.

## Import Architecture

`GameImportDispatcher` selects an `IGameImporter` by `SupportedGame`.

`GameImporter` is a shared plugin import workflow. It saves the selected game row, reads the selected game's load
order, evaluates source fingerprints and plugin import state, persists all current plugin rows before master-reference
rows, removes stale master-reference rows after a successful master-reference refresh, and delegates typed record
import to `RecordImportService` last. Import dispatch, plugin loops, master-reference loops, and record-detail loops
accept cancellation and report Core `GameImportProgressDTO` snapshots so UI and CLI callers can observe long-running
work without depending on UI binding primitives. The importer wraps the database write workflow in one NPoco
transaction so large imports do not pay per-row SQLite autocommit cost.

The game plugin readers are thin Core-contract facades over game-specific plugin reader services. The services ask
their game-specific metadata services for installed game metadata before returning the selected `GameDTO`. They expose
load-order entries, source fingerprints, header-level plugin metadata, and declared master references separately so
the importer can skip unchanged, missing, unsupported, or failed plugins before expensive metadata or record work.
Plugin metadata mapping uses header-level metadata, including header-stat record counts, and must not enumerate typed
records during plugin import. Optional `IPluginExtensionImporter` implementations persist game-specific scalar plugin
header fields into extension tables after the base `Plugins` row is saved. The game services map declared plugin
masters to shared `PluginMasterReferenceDTO` rows.

`RecordImportService` is the shared typed record import workflow. It discovers the currently approved shared record
types from a bundled `PluginRecordSetDTO`, creates per-record-type results, resolves registered typed detail importers
by `SupportedGame` and record type ID, tracks unsupported typed detail importers, and logs per-record failures without
aborting the full plugin import. The current cross-game shared record types are FormLists (`FLST`), GameSettings
(`GMST`), Globals (`GLOB`), MiscObjects (`MISC`), Keywords (`KYWD`), ActorValueInformation (`AVIF`), NPCs (`NPC_`),
MagicEffects (`MGEF`), and Perks (`PERK`). Starfield, Fallout 4, and Skyrim map approved shared records inside their
game adapters after loading the Mutagen plugin once for the Core-facing record-read call.

Starfield plugin metadata, master-reference, and record reads use a Starfield-only construction helper. The helper
prefers the full Mutagen environment load order's mod objects with the Starfield environment data folder from
`GameEnvironment.Typical.Starfield(StarfieldRelease.Starfield).DataFolderPath`. This preserves Starfield split-master,
medium-master, and overlay master-style data for FormID translation. If no environment mod objects are available, the
helper falls back to `WithLoadOrderFromHeaderMasters()` plus the same data folder. Fallout 4 and Skyrim currently use
their normal construction paths because their current master behavior does not require the Starfield separated-master
path.

## UI Architecture

The presentation layer owns `INotifyPropertyChanged`, `ObservableCollection<T>`, `ICommand`, XAML views, and
presentation commands. Core does not expose UI binding primitives or UI framework types.

`IGameSelectionService` exposes the supported game list and active-game persistence through Core DTOs and
`SupportedGame`. `IGameImportReadinessService` checks whether a selected game has imported plugin data.
`IGameImportWorkflowService` initializes the schema, persists the selected game, reports progress, and dispatches the
existing import workflow asynchronously for UI callers. Progress includes stage text plus current plugin and record
type counters. These services are UI-neutral and do not expose Mutagen types.
`IAllGamesImportWorkflowService` optionally resets the database, initializes schema, and dispatches full imports for
all supported games. The CLI `--reset-all` path and the UI `Reset & Import All` progress flow share this Core service.

`CreationsForge` starts directly in the main view. The app initializes the database schema during GUI startup before
the main view model queries imported plugins or record counts. If no active game is configured, the active-game
autocomplete remains empty and no import runs. If an active game is configured, or if the user selects a different
active game, the main view uses the same warning and import progress flow before returning to the workspace.
Presentation navigation creates a child Autofac lifetime scope for each displayed view and disposes the previous view
scope before replacing it. This keeps scoped database-backed services short-lived and lets the Reset & Import All flow
dispose the main workspace database connection before the reset workflow deletes the SQLite database files.

The main view owns active-game and active-plugin selector state. `IPluginSelectionService` exposes UI-neutral
queries for openable plugins and imported record totals by game. Selecting an active plugin updates presentation
status and loads left-side record-type sections through `IRecordTreeService`. Each section is rendered as an expander
with a grid populated from persisted shared record rows for the approved typed record set; the grids show per-record
plugin usage counts and do not call Mutagen directly from presentation code. Plugins with large header record counts
use a dedicated active-plugin loading screen before returning to the main view with a prebuilt record browser tree.
That loading screen creates a child Autofac lifetime scope on the worker path so database-backed record tree
repositories are resolved and disposed with the background load instead of reusing the main view's scoped connection.

`IRecordTreeService` aggregates record-tree entries from shared record repositories. Repository query methods return
Core `RecordTreeEntryDTO` values scoped by game and plugin `ModKeyDTO`, preserving the UI boundary and allowing the
presentation project to group and filter records without knowing database table details. Plugin usage counts are
queried with grouped SQL per shared record table and joined to active-plugin tree entries in memory.

`IRecordComparisonService` exposes the first game-agnostic comparison contract for imported typed record rows.
It reads all persisted overrides for a selected origin FormKey from shared repositories and returns comparison DTOs
with plugin columns, field rows, and display values. The presentation project renders those DTOs with an Avalonia
`TreeDataGrid` and does not query repositories, database tables, or Mutagen directly. The active plugin record browser
renders record-type groups as expander sections with flat `TreeDataGrid` controls for record rows. The comparison
slice covers common record header fields plus scalar persisted fields for `FLST`, `GMST`, `GLOB`, `MISC`, `KYWD`,
`AVIF`, `NPC_`, `MGEF`, and `PERK`. GameSetting comparison displays the generic `Data` row instead of duplicating the
Mutagen-derived typed data helper fields. MISC, NPC_, and MGEF comparison includes shared keyword rows. MISC and MGEF
comparison includes shared sound rows. MISC comparison also includes persisted model rows and scripting adapter rows
as hierarchical child rows in the comparison `TreeDataGrid`. MGEF DATA fields follow Mutagen/Spriggit's flattened
record shape and display as flat comparison rows.
Core assigns comparison value states for neutral, identical, conflicting, and displayed winning-override values; the
presentation layer maps those states to the green, red, and yellow comparison colors and shows the legend in the status
area. Deeper child sections such as perk ranks, patch generation, and conflict resolution workflows remain deferred.

`IAssetPreviewPathResolverService` resolves UI-neutral asset preview candidates from persisted model rows.
`IAssetFileResolverService` resolves readable local asset files from preview candidates by checking absolute paths,
game data-folder loose files, and normalized `Meshes` paths before reporting archive-backed assets as unsupported.
Core DTOs describe record-owned candidate paths and optional mesh payloads without referencing Avalonia, OpenGL,
Silk.NET, process launching, or binding primitives. Assets DTOs describe local-file resolution, in-memory asset reads,
and future archive extraction results. The presentation project owns `AssetPreviewPaneViewModel`, the Avalonia
`OpenGlControlBase` renderer, Silk.NET OpenGL calls, optional Nifly-backed NIF geometry reads, render mesh conversion,
sample-geometry fallback, and the external-open command. Unsupported preview cases, archive-backed paths without
BA2/BSA extraction, Nifly failures, and OpenGL renderer failures are logged through Serilog.

## Persistence Architecture

NPoco is used for application database access. Shared plugin, plugin-master-reference, and typed-record repositories
use NPoco database models for save behavior. Explicit runtime SQL uses named parameterized queries and named parameter
objects, not positional NPoco placeholders. Database-backed repositories, importers, and workflow services are
registered per Autofac lifetime scope so they share the same scoped `IDatabase` and import transaction.

Typed record repositories upsert a shared `RecordInstances` row before saving type-specific detail rows.
`RecordInstances` is the common persisted parent identity for imported record overrides and lets generic scripting
adapter tables declare foreign keys to owning records without creating per-record-type adapter tables.

Changed and forced plugin imports refresh master references and typed record rows with an import-batch timestamp.
When a master-reference refresh or typed record-type import completes without per-record failures, rows for that same
game/plugin whose `ImportedAtUTC` was not refreshed by the current batch are deleted as stale. FormList parent cleanup
uses the declared `FormListItems` foreign key cascade to remove child rows for stale parent FormLists, and FormList
item cleanup runs once per successful plugin FormList batch instead of once per parent FormList.
Typed record stale cleanup deletes stale type-specific detail rows before deleting stale `RecordInstances` rows, so
record-owned scripting adapters are removed by the declared `RecordInstances` cascade.
ModKey name and filename lookup is case-insensitive so load-order, header, and master-reference casing differences do
not make an existing plugin appear missing.

Schema creation and migration are centralized through:

- `DatabaseSchemaInitializer` in Core
- `DatabaseMigrationRunner` in Migrations
- embedded SQL scripts in `CreationsForge.Migrations/Sql`

DbUp's `SchemaVersions` table is the migration-state source of truth. The application does not define a hardcoded
schema-version constant.

## Logging

Serilog is configured through `CreationsForge.Bootstrap.Logging.SerilogConfigurator`. UI logs are written to the
configured application-data `Logs` directory. CLI logs are written to the console and the configured application-data
`Logs` directory. Logs include machine-name enrichment but do not include environment username enrichment by default.
Services log workflow-level progress and failures. Repositories do not log.

## Shared Scripted Record Extension

Typed records for `MISC`, `KYWD`, `AVIF`, `NPC_`, `MGEF`, and `PERK` follow the same Core-facing import contract as
shared records: the game adapters map Mutagen records into Core DTOs, `RecordImportService` dispatches by supported
game and record type ID, and repositories persist DTO data with named SQL parameters. The UI continues to consume Core
DTOs and record-tree services only.

Scripting adapter persistence is shared in Core through `IScriptingAdapterImportService` and scripting adapter
repositories. Game adapters populate scripting adapter DTOs for record types that expose virtual-machine adapters.
The `MISC` slice currently persists parent scalar fields, keyword rows, model rows, and scripts; the old single-game
app's deeper MiscObject child-detail tables are still a separate follow-up.
Scripting adapters are persisted against the shared `RecordInstances` parent using record type IDs such as `GLOB`,
`MISC`, `KYWD`, `AVIF`, `NPC_`, `MGEF`, and `PERK`.

Keyword-list persistence is shared in Core through `IRecordKeywordImportService` and `RecordKeywords`. `MISC`, `NPC_`,
and `MGEF` populate that shared table when the source game exposes keyword lists. Magic Effect DATA fields are
persisted directly on `MagicEffects` because Mutagen/Spriggit expose them as flattened MGEF properties.

Model persistence is shared in Core through `IModelImportService` and model repositories. `Models` and
`ModelMaterialSwaps` reference `RecordInstances` and include `ModelSlot` plus `ModelGender` so future record types can
map direct, slotted, or gendered `IModelGetter` data into one table family. The first populated model slice is
Starfield `MISC`, which uses `ModelSlot = Model` and an empty `ModelGender`.

Sound persistence is shared in Core through `IRecordSoundImportService` and `RecordSounds`. `MISC` maps named scalar
sounds such as crafting, pickup, putdown, and dropdown sounds when present, while `MGEF` maps indexed typed sound
entries such as OnHit, Release, and Charge into the same table shape when present.

Starfield `MiscItem`, `Static`, `Book`, `Door`, `Container`, and `Terminal` expose a direct `Model : IModelGetter`
shape. `Terminal.MarkerModel` is a separate terminal-specific scalar. Starfield armor, armor addon, and weapon model
data need custom mapping: armor and armor addon use gendered model wrappers, and weapons combine a direct `Model` with
additional first-person/custom model data.
