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

`CreationsForge.Specification` owns production record-family metadata that can gradually drive shared import,
validation, and comparison behavior. The first catalog slice covers FormLists (`FLST`), GameSettings (`GMST`), and
Globals (`GLOB`). `RecordComparisonService` consumes that comparison metadata for the pilot records' simple scalar
rows, while the existing import readers, typed importers, repositories, and complex comparison strategies still own
their current runtime behavior. The specification project does not reference Core, Avalonia, NPoco, Migrations,
Assets, or game-specific Mutagen packages.

`CreationsForge.Bethesda.Assets` owns UI-neutral Bethesda asset IO helpers, local-file resolution result DTOs, an
in-memory asset provider, archive-reader contracts, and temporary extraction session infrastructure. It does not
reference Avalonia, Mutagen, NPoco, game projects, or the database. Archive implementations are intended to be
read-only and preview-focused. The first archive implementations are minimal BA2 general and BSA archive readers that
can list entries and read uncompressed and zlib-compressed entries into memory. BA2 texture archives and Starfield
compression variants that are not zlib are explicit follow-up work. The first NIF implementation is a minimal preview
reader for Fallout 4/Skyrim Special Edition-style `BSTriShape` geometry and a narrow Starfield `BSGeometry` external
`.mesh` preview slice that follows NifSkope's `MeshFile` stream order, scales packed signed 16-bit positions by the
`.mesh` scale field, decodes raw external mesh positions like NifSkope's `BSMesh::updateData`, then bakes the parsed
NIF world transform into the UI-neutral preview mesh because the current renderer does not carry a NIF scene graph.
The reader can also resolve Starfield `.mat` material assets through the same external-asset callback and extract
preview DDS texture references. Starfield layered material preview support is intentionally narrow: it tracks one
primary texture, one overlay/decal texture, additive decal
blending hints, invisible-material skip hints, and a `materialsbeta.cdb` `STRT` string-table fallback for stale or
indirect texture paths. Full NIF scene graph support, full Starfield material parity, full CDB material graph parsing,
skeletons, collision, additional Starfield geometry variants, and unsupported vertex layouts remain follow-up work.

`CreationsForge.Starfield`, `CreationsForge.Fallout4`, and `CreationsForge.Skyrim` isolate
game-specific Mutagen packages, Autofac modules, reader services, and reader facade implementations. These projects
are the intended home for game-specific record mapping when Mutagen APIs or record/header fields diverge.
They also own game installation metadata discovery through Mutagen so Core does not return hardcoded or partial
game metadata. Mutagen plugin and record reads use `GameEnvironment.Typical.*(...).DataFolderPath` inside the game
adapter projects rather than persisted game metadata paths.

`CreationsForge.Migrations` contains DbUp infrastructure and embedded SQL scripts.

`CreationsForge.UnitTests` tests non-database logic only.

`CreationsForge.DataValidationTests` is a manual validation test project. It references Bootstrap and Core so it can
resolve existing repositories and compare imported DTO readback against selected Spriggit YAML samples. It may also
reference the Avalonia presentation project for Spriggit-backed rendered comparison validation, where the validation
subject is imported data flowing through the production comparison UI rather than isolated presentation behavior. Its
JSON configuration lives inside the test project under `Configuration`. It does not own production services or database
schema.

`CreationsForge.PresentationTests` owns headless Avalonia unit and presentation behavior checks that do not require
Spriggit extraction data or imported validation database state.

## Dependency Direction

- CreationsForge depends on Bootstrap and Core.
- Console depends on Bootstrap and Core.
- Bootstrap depends on Core, Migrations, Starfield, Fallout4, and Skyrim.
- Core depends on Assets for asset resolution DTOs, Migrations for migration execution, Specification for
  record-family metadata, and shared Mutagen core primitives for game-agnostic DTO mapping.
- Specification has no project dependencies.
- Assets has no project dependencies.
- Game projects depend on Core.
- Migrations does not depend on Core or game projects.
- UnitTests depend on Core and the console project for parser tests.

## Composition

`CreationsForge.Bootstrap` provides shared Autofac module registration.

- `CoreModule` registers configuration, SQLite options, connection factory, NPoco `IDatabase`, schema initializer,
  shared services, UI-neutral workflow services, shared typed importers, the asset archive readers, the record
  specification provider, and shared repositories.
- `MigrationsModule` registers `DatabaseMigrationRunner`.
- Each game module registers that game's plugin reader service, plugin reader facade, record reader, and one
  `IGameImporter`. Game reader facades are keyed by `SupportedGame` so the shared `GameImporter` can be constructor
  wired with the matching plugin and record reader pair while shared repositories and services remain normal Autofac
  dependencies.

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
work without depending on UI binding primitives. The importer saves the selected game row before archive indexing,
then scopes plugin metadata, extension rows, master references, typed records, and stale cleanup to one short NPoco
transaction per plugin.

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
(`GMST`), Globals (`GLOB`), Classes (`CLAS`), Factions (`FACT`), MiscItems (`MISC`), Keywords (`KYWD`),
ActorValueInformation (`AVIF`), NPCs (`NPC_`), MagicEffects (`MGEF`), Perks (`PERK`), Statics (`STAT`),
Books (`BOOK`), Doors (`DOOR`), Containers (`CONT`), and ConstructibleObjects (`COBJ`).
Starfield, Fallout 4, and Skyrim map approved shared records inside their game adapters after loading the Mutagen
plugin once for the Core-facing record-read call. Starfield also imports ConditionForms (`CNDF`), and Starfield plus
Fallout 4 import Terminals (`TERM`) through the same typed-record pipeline with type-specific detail tables and
comparison fields. CNDF, FACT, COBJ, and PERK condition lists use shared condition-rule rows and generic condition-data
parameter rows, not raw condition payload rows, when Mutagen exposes the condition list as typed condition objects.
All typed record importers save the record's parent row before dispatching shared child import by DTO capability.
Records that expose models, keywords, condition rules, record components, sounds, or scripting adapters persist those
child rows through the common `RecordInstances` identity instead of game-specific child-table paths. Starfield FACT
components use the shared record-component child path; Fallout 4 and Skyrim FACT records currently have no component
payload to map.

The `CreationsForge.Specification` catalog is a transitional source of production record metadata for the first
`FLST`, `GMST`, and `GLOB` slice. Runtime import dispatch still uses `PluginRecordSetDTO`, `RecordTypeCatalog`, and
registered `ITypedRecordImporter` implementations until later approved work moves discovery and dispatch behind the
specification provider.

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

The main view owns active-game and active-plugin selection state, but selection is initiated through an Open Plugin
dialog rather than command-bar autocomplete controls. `IPluginSelectionService` exposes UI-neutral queries for openable
plugins and imported record totals by game. The dialog filters, sorts, and presents those plugin rows in presentation
code, including persisted plugin import diagnostics. Selecting an active plugin updates presentation status and loads
left-side record-type sections through `IRecordTreeService`. Each section is rendered as an expander with a grid
populated from persisted shared record rows for the approved typed record set; the grids show per-record plugin usage
counts and do not call Mutagen directly from presentation code. Plugins with large header record counts use a dedicated
active-plugin loading screen before returning to the main view with a prebuilt record browser tree.
That loading screen, and the main view's asynchronous record-tree refresh path, create child Autofac lifetime scopes on
worker paths so database-backed record tree services are resolved and disposed with the background load instead of
reusing the main view's scoped connection. Active-plugin record tree entries are loaded from the shared
`RecordInstances` parent table so browsing does not fan out through every typed detail table.

`IRecordTreeService` reads active-plugin record tree entries through `IRecordInstanceRepository`. Repository query
methods return Core `RecordTreeEntryDTO` values scoped by game and plugin `ModKeyDTO`, preserving the UI boundary and
allowing the presentation project to group and filter records without knowing database table details. Plugin usage
counts are calculated in the same `RecordInstances` query by grouping peers by record type and origin FormKey.

`IRecordComparisonService` exposes the first game-agnostic comparison contract for imported typed record rows.
It reads all persisted overrides for a selected origin FormKey from shared repositories and returns comparison DTOs
with plugin columns, field rows, and display values. The presentation project renders those DTOs with an Avalonia
`TreeDataGrid` and does not query repositories, database tables, or Mutagen directly. The active plugin record browser
renders record-type groups as expander sections with flat `TreeDataGrid` controls for record rows. The comparison
slice covers common record header fields plus scalar persisted fields for `FLST`, `GMST`, `GLOB`, `CLAS`, `FACT`,
`MISC`, `KYWD`, `AVIF`, `NPC_`, `MGEF`, `PERK`, `STAT`, and `CONT`. Global comparison displays `MutagenObjectType`,
named `MajorFlags`, and `Data` when those values are persisted. GameSetting comparison displays the generic
`Data` row instead of duplicating the Mutagen-derived typed data helper fields. MISC, NPC_, and MGEF comparison
includes shared keyword rows.
MISC and MGEF comparison includes shared sound rows. MISC comparison also includes persisted model rows, component
display indices, destructible data and stages, and scripting adapter rows as hierarchical child rows in the comparison
`TreeDataGrid`. NPC_ comparison includes persisted actor configuration, template, appearance, head part, package,
property, perk, inventory, face morph, face dial, morph blend, tint, and player-skill rows. AVIF comparison includes
Skyrim layout rows, perk-tree rows, optional perk references, and
connection-line target indices. PERK comparison includes root effect rows, rank rows, nested rank-effect rows, rank
activity rows, progression evaluator rows, condition-tab rows, background skill rows, shared condition rows, shared
sound rows, shared scripting adapter rows, and script fragment rows. STAT comparison includes scalar fields, shared
keyword rows, shared model rows, navmesh geometry, and first-class reflection rows for component `REFL` data.
BOOK comparison includes scalar fields plus shared models, keywords, sounds, scripting adapters, and first-class
reflection rows. DOOR comparison includes scalar fields, direct animation component fields, shared models, keywords,
sounds, scripting adapters, and first-class reflection rows. CONT comparison includes scalar fields, direct animation
component fields, item rows, shared keyword rows, shared model rows, shared sound rows, scripting adapters, and
first-class reflection rows. TERM comparison includes scalar fields, direct animation component fields, shared models,
keywords, scripting adapters, script fragments, first-class reflection rows, forced locations, marker parameters, body
texts, condition rows, and menu items.
CNDF, FACT, and COBJ
comparison includes structured condition rows and condition-data parameter rows. COBJ comparison also includes
the scalar `CreatedObjectCount` field. Reflection and raw payload values are
compared by their retained full value but are summarized in the grid as `[UNPARSEABLE REFLECTION DATA]`; the
presentation layer opens the full value in a hex-view dialog when the user selects the summarized value. MGEF DATA
fields follow Mutagen/Spriggit's flattened record shape and display as flat comparison rows.
GameSetting comparison resolves localized `Data` through persisted localized string rows using the Settings-selected
record text language, then falls back to English and finally the scalar `Data` value.
Core assigns comparison value states for neutral, identical, conflicting, and displayed winning-override values; the
presentation layer maps those states to the green, red, and yellow comparison colors and shows the legend in the status
area.

The specification catalog now drives simple comparison rows for the first `FLST`, `GMST`, and `GLOB` pilot records.
`RecordComparisonService` reads type-specific comparison fields from `IRecordSpecificationProvider`, resolves simple
DTO source paths generically, and still uses explicit strategy hooks where behavior is not purely declarative. The
current hooks include indexed `FLST` item rows and localized `GMST` `Data` display. Complex record families and shared
child groups remain on the existing record-specific comparison methods until later approved slices move them behind
specification metadata.

`IAssetPreviewPathResolverService` resolves UI-neutral asset preview candidates from persisted model rows.
`IAssetFileResolverService` resolves readable local asset files from preview candidates by checking absolute paths,
game data-folder loose files, normalized `Meshes` paths, and the database-backed asset archive index. The archive index
stores archive metadata and normalized entry paths only, not extracted bytes. Game import builds or refreshes archive
indexes only for preview-relevant archive names containing `meshes`, `textures`, `materials`, `misc`, or `main`, so
large non-preview archives such as animation, voice, shader, terrain, and localization packs are skipped during import.
BA2 texture archive indexing reads the archive header and name table only; full texture record and chunk parsing is
reserved for reading the selected texture entry during preview. Preview lookup can still lazily build or refresh one
archive index at a time by comparing the archive file's last-write ticks and size, then reads only the matching archive
entry through the owned read-only archive readers. Lazy fallback lookup scopes archive candidates by asset path:
`Meshes` and Starfield `geometries` paths search mesh/main/misc archives, `Textures` paths search texture/main/misc
archives, `Materials` paths search material/main/misc archives, and other paths search main/misc archives. Core DTOs
describe record-owned candidate paths and optional mesh payloads without referencing Avalonia, OpenGL, Silk.NET,
process launching, or binding primitives.
Bulk game import treats archive indexing and each game boundary as memory-pressure checkpoints: archive reader
directory caches are cleared after each archive index attempt, and the all-games workflow runs an explicit
large-object-heap-compacting collection after each game completes.
Assets DTOs describe local-file resolution, in-memory asset reads, and archive extraction results. The presentation
project owns `AssetPreviewPaneViewModel`, the Avalonia `OpenGlControlBase` renderer, Silk.NET OpenGL calls, render
mesh conversion, sample-geometry fallback, and the external-open command. The presentation preview geometry adapter
uses the owned Assets NIF preview reader when readable `.nif` bytes are available. For Starfield `BSGeometry` NIFs
that reference external `geometries/**/*.mesh` payloads, the adapter passes an external-asset resolver back through
the existing UI-neutral asset-file resolver. The same resolver path is used when the NIF reader probes Starfield
`.mat` material assets for preview texture references. Unsupported preview cases, archive-backed paths that cannot yet
be read, parser gaps, and OpenGL renderer failures are logged through Serilog. Asset preview creation runs on a
presentation background task with a loading state, and stale background results are ignored when selection changes.
The OpenGL preview uses one interactive, bounds-based camera view with full-orientation pointer orbit, pointer pan,
wheel zoom, a reset-view control, explicit X/Y/Z axis view presets, and a small X/Y/Z orientation overlay. Preview
render-space maps Creation Engine/NIF Z-up to render Y-up, so camera defaults, view presets, and the orientation
overlay follow that presentation-space mapping. Render mode and mesh selection remain presentation-only controls.
The simplified Starfield material preview follows NifSkope's CE2 opacity rule at a preview level by treating ordinary
base texture alpha as opaque and using only explicit decal opacity texture handling for preview alpha. Each background
preview load creates its own Autofac lifetime scope before resolving preview scene services, because
archive and database-backed asset resolution are scoped dependencies and must not be shared across overlapping preview
tasks.

## Persistence Architecture

NPoco is used for application database access. Shared plugin, plugin-master-reference, and typed-record repositories
use NPoco database models for save behavior. Explicit runtime SQL uses named parameterized queries and named parameter
objects, not positional NPoco placeholders. Database-backed repositories, importers, and workflow services are
registered per Autofac lifetime scope so they share the same scoped `IDatabase`. Game imports use short transaction
boundaries: one archive-index transaction per refreshed archive and one plugin transaction per imported plugin.

Typed record repositories upsert a shared `RecordInstances` row before saving type-specific detail rows.
`RecordInstances` is the common persisted parent identity for imported record overrides and lets shared child tables
such as models, keywords, sounds, and scripting adapters declare foreign keys to owning records without creating
per-record-type child tables.

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
Migrations that add typed record support can invalidate existing cached plugin rows by setting `ImportState` to
`Changed`; `GameImporter` only skips source-matching rows when the existing row is still `Current`, so invalidated rows
are reimported when their game is next imported.

## Logging

Serilog is configured through `CreationsForge.Bootstrap.Logging.SerilogConfigurator`. UI logs are written to the
configured application-data `Logs` directory. CLI logs are written to the console and the configured application-data
`Logs` directory. Logs include machine-name enrichment but do not include environment username enrichment by default.
Services log workflow-level progress and failures. Repositories do not log.
`ProcessTerminationDiagnosticsService` writes an application-data session marker with the current PID, log path, last
import heartbeat, memory snapshot, process handle count, thread count, termination-request state, and clean-shutdown
state. UI and CLI startup log an unexpected previous session when the prior marker was not cleanly shut down. Catchable
termination events observed by the app, such as console cancel, request import cancellation and update the session
marker; hard kills such as `SIGKILL`, Windows task termination, and some OS memory kills are diagnosed only on the
next launch from the last heartbeat.

## Shared Scripted Record Extension

Typed records for `MISC`, `KYWD`, `AVIF`, `NPC_`, `MGEF`, `PERK`, `STAT`, `BOOK`, `DOOR`, `CONT`, `CNDF`, `COBJ`,
and `TERM` follow the same Core-facing
import contract as `FLST`, `GMST`, and `GLOB`: the game adapters map Mutagen records into Core DTOs,
`RecordImportService` dispatches by supported game and record type ID, and repositories persist DTO data with named
SQL parameters. The UI continues to consume Core DTOs and record-tree services only.

Scripting adapter persistence is shared in Core through `IScriptingAdapterImportService` and scripting adapter
repositories. `IRecordChildImportService` invokes scripting adapter persistence for any imported `RecordDTO` that
implements the scripting-adapter capability interface. Game adapters populate scripting adapter DTOs for record types
that expose virtual-machine adapters.
The `MISC` slice currently persists parent scalar fields, keyword rows, model rows, sounds, scripts, FO4/Skyrim
components with display indices, Starfield resources, and destructible data/stages when Mutagen exposes them. The
`PERK` slice persists parent scalar fields, root effects, ranks, rank effects, effect condition tabs,
background skills, shared condition rows, sounds, scripts, and script fragments. The `BOOK` slice persists parent
scalar fields, keyword rows, model rows, sounds, scripts, and first-class reflection rows for component `REFL` data.
The `DOOR` slice persists parent scalar fields including direct animation component fields, keyword rows, model rows,
sounds, scripts, and first-class reflection rows. The `CONT` slice persists parent scalar fields including direct
animation component fields, item rows, keyword rows, model rows, sounds, scripts, and first-class reflection rows. The
`CNDF` slice persists parent
scalar fields, shared condition-rule rows, and generic condition-data parameter rows. The `COBJ` slice persists parent
scalar
fields, recipe component rows, Fallout 4 category rows, Starfield recipe-filter rows, shared condition-rule rows,
scripts when present, and scalar `CreatedObjectCount`. The `TERM` slice persists parent scalar fields including direct
animation component fields, keyword rows, model rows, scripts, script fragments, first-class reflection rows, forced
locations, marker parameters, body texts, condition rows, and menu items.
Scripting adapters are persisted against the shared `RecordInstances` parent using record type IDs such as `GLOB`,
`MISC`, `KYWD`, `AVIF`, `NPC_`, `MGEF`, `PERK`, `BOOK`, `DOOR`, and `TERM`.

Keyword-list persistence is shared in Core through `IKeywordMappingImportService` and `KeywordMappings`.
`IRecordChildImportService` invokes keyword persistence for any imported `RecordDTO` that implements the keyword-list
capability interface. Magic Effect DATA fields are persisted directly on `MagicEffects` because Mutagen/Spriggit
expose them as flattened MGEF properties.

Model persistence is shared in Core through `IModelImportService` and model repositories. `Models` and
`ModelMaterialSwaps` reference `RecordInstances` and include `ModelSlot` plus `ModelGender` so future record types can
map direct, slotted, or gendered `IModelGetter` data into one table family. `IRecordChildImportService` invokes model
persistence for any imported `RecordDTO` that implements the model capability interface. The currently populated
direct-model slices are `MISC`, `STAT`, `BOOK`, `DOOR`, `CONT`, and `TERM`, each using `ModelSlot = Model` and an
empty `ModelGender`.

Script fragments are persisted in Core through `IScriptFragmentRepository` alongside scripting adapters. Game adapters
populate script fragment DTOs from VMAD data for supported `PERK` and `TERM` records that expose script fragments, and
the scripting-adapter import path persists them only for DTOs that implement the script-fragment capability interface.

Readable Bethesda component fields are typed on the consuming records when their meaning varies by record. Starfield
`DOOR`, `CONT`, and `TERM` animation graph component values are stored as direct parent columns. COBJ
created-object counts are stored as the scalar `CreatedObjectCount` parent field. NPC template/appearance leftovers
plus static navmesh geometry are typed columns on their owning parent tables.

Reflection persistence is shared in Core through `IReflectionImportService` and the `Reflection` table.
`IRecordChildImportService` invokes reflection persistence for imported DTOs that implement the reflection capability
interface. Starfield component `REFL` data for supported `STAT`, `BOOK`, `DOOR`, `CONT`, and `TERM` records is stored
as first-class reflection rows with component index, component type, source path, and payload value. Raw payload
persistence remains available through `IRawRecordPayloadImportService` and `RawRecordPayloads` only for opaque
binary-like payloads without a first-class model. Model `Data` is stored on `Models`, sounds are stored on
`SoundMappings`, condition rules are modeled as structured condition and parameter rows, and reflected readable data is
stored in typed parent or child tables. Ordinary keyword rows discovered through nested component-shaped objects remain
`KeywordMappings`; they are not treated as payload rows.
Comparison DTOs keep the full reflection or raw payload value as detail data while exposing a summarized display label
for the UI hex viewer.

Sound persistence is shared in Core through `ISoundMappingImportService` and `SoundMappings`.
`IRecordChildImportService` invokes sound persistence for any imported `RecordDTO` that implements the sound
capability interface. `MISC` maps
named scalar sounds such as crafting, pickup, putdown, and dropdown sounds when present, `PERK` maps its exposed sound
list, and `MGEF` maps indexed typed sound entries such as OnHit, Release, and Charge into the same table shape when
present.

Localized string persistence is shared in Core through `IRecordLocalizedStringImportService` and `LocalizedStrings`.
`RecordDTO` exposes localized strings as a shared child collection, and `IRecordChildImportService` replaces those
rows for imported records. DTO fields that map to Mutagen translation-table-backed strings use `TranslatedStringDTO`
so import and future editing preserve the per-language table shape. GameSetting `Data` still uses its scalar DTO value
with localized child rows because the field also carries setting-type semantics. The Settings screen owns the
persisted record text language selection; the command bar does not expose a language dropdown.

Starfield `MiscItem`, `Static`, `Book`, `Door`, `Container`, and `Terminal` expose a direct `Model : IModelGetter`
shape and currently map that direct model to `ModelSlot = Model`. `Terminal.MarkerModel` is a separate
terminal-specific scalar. Starfield armor, armor addon, and weapon model data need custom mapping: armor and armor
addon use gendered model wrappers, and weapons combine a direct `Model` with additional first-person/custom model
data.
