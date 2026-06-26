# Domain Model

## Core Concepts

Game: A supported Bethesda game identity. The current supported values are `Starfield`, `Fallout4`, and `Skyrim`.

Plugin: A Bethesda plugin file within a game. Plugins are identified in persistence by `Game` plus a decomposed Mutagen
ModKey tuple.

ModKey: A Mutagen plugin identifier. Application-schema columns store ModKey values as primitive components:
`*_ModKey_Name`, `*_ModKey_Type`, and `*_ModKey_FileName`.

ModKey name and filename comparisons are case-insensitive for lookup. Bethesda load-order files, plugin headers, and
Mutagen-provided master references can disagree on casing for the same plugin, especially in Starfield load orders.
Persisted casing remains source/display metadata, not a case-sensitive identity boundary.
When a case-insensitive lookup resolves a persisted plugin row, dependent rows use that persisted ModKey tuple so
declared SQLite foreign keys continue to reference the exact stored parent key.

FormKey: A Mutagen record identifier. Application-schema columns store FormKey values as referenced ModKey components
plus a numeric ID: `*_ModKey_Name`, `*_ModKey_Type`, `*_ModKey_FileName`, and `*_FormKey_ID`.

Containing plugin identity: The `Game` plus `ModKey_*` columns on typed record tables identify the plugin containing
the imported row.

Origin record identity: The `FormKey_ModKey_*` columns plus `FormKey_ID` identify the record's origin FormKey. This is
the identity needed to group true overrides for comparison while avoiding collisions on local numeric FormKey IDs.

Record instance: A persisted imported override identity combining the containing game/plugin, record type ID, and
origin record identity. `RecordInstances` is the database parent for typed detail rows and shared child rows such as
models, keywords, sounds, and scripting adapters.

Master reference: A relationship edge from a declaring plugin to a declared master plugin in the same game.

Record type: A Bethesda major-record type identified by a four-character record ID. The current cross-game shared
record import workflow includes FormLists (`FLST`), GameSettings (`GMST`), Globals (`GLOB`), MiscItems (`MISC`),
Classes (`CLAS`), Factions (`FACT`), Keywords (`KYWD`), ActorValueInformation (`AVIF`), NPCs (`NPC_`), MagicEffects
(`MGEF`), Perks (`PERK`), Statics (`STAT`), Books (`BOOK`), Doors (`DOOR`), Containers (`CONT`), and
ConstructibleObjects (`COBJ`). Starfield also persists typed detail rows for ConditionForms (`CNDF`), and Starfield
plus Fallout 4 persist typed detail rows for Terminals (`TERM`).

Record specification: Production metadata that describes a record family's Bethesda record ID, canonical
CreationsForge name, current typed-detail table name, supported game adapters, source field hints, and comparison
field intent. The specification catalog lives in `CreationsForge.Specification` and covers the current imported
record families for Core import dispatch. Reader metadata names each record family's `PluginRecordSetDTO` destination
collection and default Mutagen collection name, but game adapters still own Mutagen-to-DTO mapping. Core import
dispatch consumes specification import metadata to locate the matching `PluginRecordSetDTO` collections and preserve
the approved record-family order. Core comparison consumes pilot comparison metadata for `FLST`, `GMST`, and `GLOB`
simple scalar rows. Complex comparison strategies and the actual game-specific Mutagen mapping remain owned by the
existing Core and game-adapter services until later approved work makes those paths specification-driven.

Starfield master references require special construction through Mutagen's separated-master-aware load-order paths.
The Starfield reader prefers the full Mutagen environment load order's mod objects so split masters, medium masters,
and overlays retain their master-style data for FormID translation. Header-master construction remains a fallback when
environment mod objects are unavailable. Fallout 4 and Skyrim master references use normal plugin construction unless
a future Mutagen or game-specific requirement proves otherwise.

## Shared And Game-Specific Boundaries

Core DTOs and repositories are shared only for the current approved schema: games, plugin metadata, plugin master
references, FormLists, FormListItems, GameSettings, Globals, approved typed parent rows, shared model rows, shared
keyword rows, shared sound rows, and shared scripting adapter rows. Shared child rows are persisted for approved record
types that expose the corresponding Core DTO capability interfaces.

Game-specific projects own Mutagen package references and should own any mapping that depends on a specific game's
record interfaces, header flags, version fields, or available payload fields.

Game adapter plugin and record reads use Mutagen environment data folders, such as
`GameEnvironment.Typical.Starfield(StarfieldRelease.Starfield).DataFolderPath`, as the read-path source of truth.
Persisted game folder metadata remains informational and is not used as the authority for Mutagen plugin construction.

## Active Game Configuration

`ApplicationConfiguration` stores the active game as a string matching `SupportedGame`. Passing a game argument updates
the stored active game. Running without a game argument uses the stored active game when available.

The UI uses the same active-game configuration through `IGameSelectionService`. Supported-game display labels are
presentation-safe Core DTOs and do not expose Mutagen types.

The main-window Open Plugin workflow defaults to the configured active game when one is stored in the configuration
file. Selecting a game/plugin from the dialog updates presentation active-selection state, and configured startup or
explicit import commands continue to run through the shared import workflow.

## Active Plugin Selection

An active plugin is a presentation selection from imported/openable plugin rows for a selected active game. The UI uses
an Open Plugin dialog to choose both the active game and active plugin from Core DTOs. Plugin choices are scoped to the
selected game and can be filtered/sorted in presentation code. Selecting an active plugin updates the status bar with
active game, plugin, and record-count context; it does not perform direct Mutagen reads in the presentation layer.
Plugin rows can carry persisted import diagnostics for failed, missing, unsupported, changed, or partially imported
states so the UI can show details without reading logs.

## Imported Record Tree

The main workspace includes a left-side imported-record tree for the active plugin. The current tree includes the
approved persisted record types: `FLST`, `GMST`, `GLOB`, `MISC`, `KYWD`, `AVIF`, `NPC_`, `MGEF`, `PERK`, `STAT`,
`CLAS`, `FACT`, `CNDF`, `BOOK`, `DOOR`, `CONT`, `COBJ`, and `TERM`. Tree
entries are read from Core repository data through `IRecordTreeService` and grouped by record ID. Each record-type
group shows its visible record count, and each record row shows how many imported plugins in the active game contain
that same origin FormKey.

The tree displays a FormID-style value from the persisted `FormKeyDTO.Id` and the imported `EditorID`. It does not use
presentation-layer Mutagen APIs or Starfield separated-master translation. Exact game-specific display FormID
translation can be added later through Core/game-adapter services if needed.

## Record Comparison

Record comparison groups persisted records by origin FormKey across imported plugins in the active game. Comparison
columns represent plugin overrides, and comparison rows represent fields exposed by Core DTOs.

The first comparison slice displays common fields (`EditorID`, `FormVersion`, and `MajorRecordFlags`) for all approved
records. FormLists also display `AddToList` and indexed `Items[n]` rows. GameSettings display `MutagenObjectType`
and the active typed `Data` value. Localized text rows use the Settings-selected record text language when a persisted
localized value exists, then fall back to English and the DTO or scalar database fallback. Globals display
`MutagenObjectType`, named `MajorFlags`, and `Data`.
The specification catalog now drives the simple comparison fields for `FLST`, `GMST`, and `GLOB`. The Core comparison
service remains the runtime authority for generated comparison DTOs, including row state, plugin column ordering,
indexed `FLST` item expansion, and localized `GMST` `Data` display.
`MISC`, `KYWD`, `AVIF`, `NPC_`, `MGEF`, `PERK`, `STAT`, `CLAS`, `FACT`, `BOOK`, `DOOR`, `CONT`, `COBJ`, and `TERM`
comparisons display their currently persisted parent fields and record-reference fields. CLAS comparison displays
class property rows and skill-weight or stat-weight
rows when those child rows are present. FACT comparison displays relation, rank, shared condition-rule, and Starfield
component rows when those payloads are persisted. PERK comparison displays rank rows, nested rank-effect
rows, background skill rows, and shared scripting adapter rows. `MISC` comparison displays component display indices
and destructible data/stage rows when those payloads are persisted. `NPC_` comparison displays persisted actor
configuration, template, appearance, head part, package, property, perk, inventory, face morph, face dial, morph blend,
tint, and player-skill rows when those payloads are persisted. `MISC`, `NPC_`, `MGEF`, `BOOK`, `DOOR`, `CONT`,
`COBJ`, and `TERM` comparisons display shared child rows when those payloads are persisted. `CNDF` and `COBJ`
comparison displays shared condition-rule rows and
generic condition-data parameter rows when those payloads are persisted. `COBJ` comparison displays component,
Fallout 4 category, and Starfield recipe-filter rows.
`AVIF` comparison displays Skyrim perk-tree rows, including associated skill, grid placement, perk references, and
connection-line target indices when they are present. `TERM` comparison also displays forced locations, marker
parameter rows, body-text rows, and menu-item rows when present.
MGEF DATA follows Mutagen/Spriggit's flattened record shape and displays as flat rows. Child comparison data such as
keywords, models, sounds, scripts, script fragments, binary raw payloads, items, shared condition rules,
condition-rule parameters,
constructible object components, Fallout 4 COBJ category links, Starfield COBJ recipe-filter links, perk ranks, perk
rank effects, perk background skills, and terminal child rows is represented as hierarchical rows in the comparison
TreeDataGrid instead of flattened dotted field names.

Comparable comparison rows are highlighted green when all visible plugin values match and red when any visible plugin
value differs. Blank values count as values. Single-column comparisons and non-comparable informational rows remain
neutral. In a conflicting row, the far-right visible plugin value is highlighted yellow as the winning override within
the displayed load-order-sorted comparison set.
Numeric DTO fields preserve imported values by default. Fields marked with `NumericDisplayPrecisionAttribute` use that
declared decimal precision only when comparison builds display values and comparable row state.

The UI renders comparison DTOs from `IRecordComparisonService` and does not call repositories, database tables, or
Mutagen APIs directly.
Spriggit comparison UI validation includes a coverage audit that flags validation specs with meaningful DTO assertions
but no explicit comparison row expectations, preventing record types from passing headless UI validation through the
default `EditorID`-only fallback.

## Localized Record Text

Localized record text is persisted as record-owned child data in `LocalizedStrings`. Each row identifies the owning
record, source DTO field, language name, translated value, and import timestamp. DTO fields that map directly to
Mutagen translation-table-backed strings use `TranslatedStringDTO`, which preserves the imported language table in the
DTO contract. Type-specific scalar database columns remain a compatibility and fallback persistence layer for existing
repository rows; they are not the authoritative DTO shape for translated fields.

Localized record text uses Mutagen `Language` in core logic; strings are boundary values for configuration,
persistence, and imported/exported data.

The Settings screen stores the preferred record text language. Core comparison services use that setting when
rendering localized comparison rows and fall back to English when the selected language is unavailable. Child
translated text uses dotted or indexed source paths such as `Ranks[0].Description` and
`Ranks[0].Effects[0].ButtonLabel`.

## Current Import Data

The current readers save the selected game row, discover load-order plugins, read plugin source fingerprints, persist
plugin metadata, persist declared master references, and run shared record import orchestration for approved record
types. Starfield, Fallout 4, and Skyrim map `FLST`, `GMST`, `GLOB`, `MISC`, `KYWD`, `AVIF`, `NPC_`, `MGEF`, `PERK`,
`CLAS`, `FACT`, `STAT`, `BOOK`, `DOOR`, `CONT`, and `COBJ` records to shared DTO shapes. Starfield also maps `CNDF`
records, and Starfield plus Fallout 4 map `TERM` records to typed detail DTOs instead of model-only preview rows.
Typed record repositories persist a shared
record
instance before saving type-specific detail rows, and typed importers dispatch shared child persistence from the record
DTO capability interfaces.

`CreationsForge.Specification` currently provides production metadata for the imported record families. That metadata
is registered through Core composition and drives the shared import dispatch loop. The catalog now describes current
reader destination collections and default Mutagen collection names as a reader-migration target. The same catalog
drives simple comparison rows for the `FLST`, `GMST`, and `GLOB` pilot records, but it does not yet change how readers
map Mutagen records or how repositories persist DTOs.

## Presentation Boundary

The UI can select games, trigger imports, and display summaries of imported plugin and record counts. It does not own
Bethesda plugin parsing concepts and does not call Mutagen directly. Game-specific Mutagen record and header mapping
remains in the game adapter projects, and Core exposes only approved DTOs, enums, result objects, and primitive
identity shapes to presentation code.

## Shared Record Children

Typed records currently include `FLST`, `GMST`, `GLOB`, `MISC`, `KYWD`, `AVIF`, `NPC_`, `MGEF`, `PERK`, `STAT`,
`BOOK`, `DOOR`, `CONT`, and `COBJ` across Starfield, Fallout 4, and Skyrim. Starfield also includes `CNDF`, and
Starfield plus Fallout 4 include `TERM`. Core
exposes these through CreationsForge DTOs and primitive `FormKeyDTO`/`ModKeyDTO` identity shapes; direct Mutagen
mapping remains in the game adapter projects.

Scripting adapters represent virtual-machine script attachments exposed by Mutagen. They are persisted for Starfield
Fallout 4, and Skyrim records that expose scripting adapters through Core DTO capability interfaces. `FLST` and `GMST`
do not persist scripting adapters. Scripting adapters are linked to their owning `RecordInstances` row, not directly
to the containing plugin.

Models represent Mutagen `IModelGetter` payloads for records that expose model data. Shared model rows are linked to
their owning `RecordInstances` row and are further identified by `ModelSlot` and `ModelGender`. The currently
populated direct-model slices are `MISC`, `STAT`, `BOOK`, `DOOR`, `CONT`, and `TERM`, each with `ModelSlot = Model`
and an empty `ModelGender`. Opaque model `Data` bytes are stored on the model row instead of in raw payload rows.

Asset preview candidates are derived from persisted model rows. Candidate identity includes the selected game, source
plugin, record type, origin FormKey, model slot, model gender, and mesh path. Core can describe preview geometry with
UI-neutral mesh DTOs containing vertices, normals, triangle indices, UVs, material names, and optional texture paths,
but the current Avalonia pane renders generated sample geometry while real NIF parsing remains deferred.

Keyword lists represent indexed keyword FormKey payloads for records that expose keyword data. Shared keyword rows are
linked to their owning `RecordInstances` row by record type and parent FormKey. `MISC`, `NPC_`, and `MGEF` currently
populate this shared keyword shape when the source game exposes keyword lists.

Sounds represent Spriggit-style sound payloads. Shared sound rows are linked to their owning `RecordInstances` row by
record type and parent FormKey. `MISC` maps named scalar sounds such as `CraftingSound`, `PickupSound`, and
`DropdownSound`; `BOOK`, `DOOR`, and `CONT` map record-specific named scalar sounds; `MGEF` maps indexed typed sound
entries such as `OnHit`, `Release`, and `Charge`.

Constructible object components, categories, and recipe filters are stored in COBJ-specific child tables. Skyrim maps
COBJ `Items`, Fallout 4 maps `Components` and `Categories`, and Starfield maps `ConstructableComponents` and
`RecipeFilters`. The COBJ `Components` term is intentionally kept distinct from other Bethesda component-shaped
payloads.

Condition rules represent indexed condition rows plus generic parameter rows for records that expose condition lists.
Current users are `CNDF`, `FACT`, `COBJ`, and terminal body/menu condition lists. The condition data function is
stored as `DataMutagenObjectType`, and parameter values retain a decomposed FormKey when the Mutagen value exposes one.
Condition rules are not raw payloads when Mutagen exposes structured condition fields.

Actor Value Information uses AVIF-specific child tables for Skyrim perk-tree data. Perk-tree rows retain associated
skill references, grid placement values, optional perk references, and indexed connection-line target indices so
imported Skyrim perk graph shape can be read back and compared.

Script fragments represent VMAD script fragment data and are stored as script-owned child rows for supported `PERK`
and `TERM` records, not as generic value or raw payload slots. Script adapters remain the storage shape for tracked
Papyrus scripts and properties.

Readable Bethesda component fields are stored on the consuming record when their meaning varies by record. Starfield
`DOOR`, `CONT`, and `TERM` animation graph component values map to direct animation fields on the parent DTO/table.
Spriggit component `REFL` fields are stored as first-class `Reflection` rows with component index, component type,
source path, and payload value. Raw payload rows are reserved for opaque binary-like payloads that do not have a
first-class model.

COBJ created-object counts use the parent scalar `CreatedObjectCount` field. NPC template/appearance leftovers and
static navmesh geometry are typed fields on their owning record DTO and parent table.

Magic Effect DATA represents flattened Starfield `MGEF` properties exposed by Mutagen/Spriggit. Those fields are
persisted directly on `MagicEffects` and displayed as flat comparison rows.

Starfield `MiscItem`, `Static`, `Book`, `Door`, `Container`, and `Terminal` expose a direct `Model : IModelGetter`.
`Terminal.MarkerModel` is not part of the shared model payload. Starfield armor and armor addon model data is wrapped
by gendered world and first-person model structures, and weapon data combines direct model data with first-person and
other custom model-related fields. Those records should map their model slots deliberately when their typed records are
implemented.
