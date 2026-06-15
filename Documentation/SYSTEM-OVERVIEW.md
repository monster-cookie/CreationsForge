# System Overview

## Purpose

CreationsForge is a .NET feasibility project for testing whether selected Bethesda plugin metadata and record details
can be imported into one SQLite schema across multiple games.

The current slice supports game selection, persisted active-game configuration, Serilog logging, Autofac composition,
DbUp schema migration, and shared import orchestration for:

- Starfield
- Fallout 4
- Skyrim Special Edition

The current game-specific reader services establish the project and dependency boundaries and use Mutagen to discover
selected game installation metadata, load-order plugins, source fingerprints, header-level plugin metadata, and
declared plugin master references. Thin game plugin readers expose the shared Core import contract over those services.
Plugin metadata import avoids typed record enumeration; record counts are read from header stats. Game-specific plugin
extension importers persist audited scalar plugin header fields into extension tables. Starfield, Fallout 4, and
Skyrim map the currently approved cross-game typed records: FormLists, GameSettings, Globals, MiscItems, Keywords,
ActorValueInformation, NPCs, MagicEffects, Perks, Statics, Containers, and ConstructibleObjects. Starfield also maps
ConditionForms, Books, Doors, and Terminals into typed detail rows. ConditionForms include structured condition rows
and generic condition-data parameter rows, not raw condition payload rows. Imports currently create/update the selected
`Games`, `Plugins`, `PluginMasterReferences`, game-specific plugin extension rows, and approved typed record rows.

## Projects

- `CreationsForge` is the cross-platform Avalonia presentation project. Its first UI slice owns
  MVVM state for direct main-window game selection, active plugin selection, guarded import flow, and import progress
  through Core workflow services.
- `CreationsForge.Console` is the command-line import harness. It owns
  command-line parsing, terminal output, exit codes, schema initialization trigger, and game import dispatch.
- `CreationsForge.Bootstrap` owns shared startup helpers used by both app surfaces. It registers Core, Migrations,
  and game adapter Autofac modules and configures shared Serilog file logging, with optional console logging for CLI
  callers.
- `CreationsForge.Core` owns shared configuration storage, shared DTOs, database initialization, importer
  contracts, shared import orchestration, UI-neutral workflow services, shared repositories for the approved common
  schema, metadata contracts, and Core Autofac registrations.
- `CreationsForge.Starfield` owns Starfield-specific Mutagen package references, metadata discovery, reader
  services, reader facade, and module types.
- `CreationsForge.Fallout4` owns Fallout 4-specific Mutagen package references, metadata discovery, reader
  services, reader facade, and module types.
- `CreationsForge.Skyrim` owns Skyrim-specific Mutagen package references, metadata discovery, reader services,
  reader facade, and module types.
- `CreationsForge.Migrations` owns DbUp migration execution and embedded SQL scripts.
- `CreationsForge.UnitTests` owns xUnit tests for parser, dispatch, and shared service behavior.

## Runtime Flow

1. `Program` loads configuration and configures Serilog.
2. `Program` builds the Autofac container by registering Core, Migrations, and all game modules.
3. `GameArgumentParser` reads `--game`, `-g`, or `-game`; when no game is supplied, it falls back to the configured
   active game. `--force` or `--full` requests a full reimport instead of skipping unchanged plugins. `--reset-all`
   deletes the current database and imports every supported game.
4. The selected single-game import is saved back to `ApplicationConfigurationStore`.
5. `IDatabaseSchemaInitializer` runs DbUp migrations.
6. If migrations were applied during a single-game import, the import is forced so existing cached plugin data for
   the selected game is refreshed for the updated schema. Migrations can also invalidate existing plugin rows by
   setting `ImportState` to `Changed`; invalidated rows are reimported on the next import for their game even when the
   source fingerprint has not changed.
7. `GameImportDispatcher` selects the registered game importer.
8. `GameImporter` saves the selected game row and reads the selected game's load order.
9. `GameImporter` evaluates each plugin source fingerprint, preserving missing, unsupported, unchanged, changed, and
   failed import states before expensive metadata or record work.
10. Current plugin rows and matching game-specific plugin extension rows are persisted before master references.
11. Declared plugin masters are mapped to shared `PluginMasterReferenceDTO` rows after plugin rows exist.
12. `RecordImportService` runs last for plugins that were imported in the current run. It discovers approved shared
    record types, resolves registered typed detail importers, records unsupported typed detail importers, and isolates
    per-record failures.

The Avalonia UI uses `IGameSelectionService` to list and persist supported games, `IGameImportReadinessService` to
detect whether the selected game already has imported plugin data, `IPluginSelectionService` to list imported/openable
plugins for the active game, and `IGameImportWorkflowService` to run the same schema initialization and import workflow
through Core. UI and MVVM code consume Core DTOs and result objects only; direct Mutagen usage remains outside the
presentation project.

On startup, the UI opens the main window immediately and initializes the database schema before view-model queries run.
The main toolbar exposes `Open Plugin` for active game/plugin selection. When a configured active game exists, startup
can run that game's import through the import progress screen before returning to the main workspace. New and full
imports warn that import can take 5-15 minutes. The toolbar also exposes `Reimport` for a full import of the active
game and `Reset & Import All` for deleting the current database before importing every supported game. Selecting an
active plugin updates the main-window status bar and loads the imported-record tree for the current persisted record
types.

## Current Capabilities

- Accepts `Starfield`, `Fallout4`, and `Skyrim` as supported game values.
- Accepts `--force` and `--full` to force a full reimport from the CLI.
- Accepts `--reset-all` to delete the current database and force a full import for every supported game.
- Rejects unsupported game values with a clear error and non-zero exit code.
- Persists active game and app data paths in a JSON configuration file.
- Writes logs to console and the configured `Logs` directory.
- Creates and migrates a SQLite database through DbUp.
- Uses DbUp `SchemaVersions` as the migration-state source of truth.
- Creates a multi-game application schema for `Games`, `Plugins`, `PluginMasterReferences`, `FormLists`,
  `FormListItems`, `GameSettings`, `Globals`, `MiscItems`, `Keywords`, `ActorValueInformation`, `NPCs`,
  `MagicEffects`, `Perks`, `Statics`, `ConditionForms`, `ConstructibleObjects`, `Books`, `Doors`, `Containers`,
  `Terminals`, `TerminalMarkerParameters`, shared model data, shared keyword lists, shared sounds, shared raw payload
  data, and shared scripting adapter data.
- Preserves plugin source-fingerprint behavior for unchanged, changed, missing, failed, and unsupported plugin states.
- Preserves record import accounting for the approved typed record types.
- Provides an initial Avalonia UI with an Open Plugin dialog for active game/plugin selection, warning before long
  first/full imports, toolbar commands for active-game reimport and Reset & Import All, running all imports through
  Core services with a progress screen, and browsing imported typed records in a left-side tree with category counts,
  per-record
  plugin usage counts, scalar comparison rows, and supported child comparison rows such as CNDF condition rows. Long
  binary raw payload comparison values are summarized as `[UNPARSEABLE REFLECTION DATA]` and can be opened in a
  hex-view dialog from the comparison grid.
- Provides an experimental asset preview pane in the Avalonia UI. Core resolves persisted model-path candidates through
  UI-neutral DTOs and services, while the presentation project owns Silk.NET-backed OpenGL rendering and external file
  launching.

## Current Limitations

- Game-specific reader services currently return selected game metadata, load-order plugin metadata, header-stat
  record counts, declared master references, and audited scalar game-specific plugin header fields.
- Starfield, Fallout 4, and Skyrim share `FLST`, `GMST`, `GLOB`, `MISC`, `KYWD`, `AVIF`, `NPC_`, `MGEF`, `PERK`,
  `STAT`, and `CONT` typed-record mapping. Additional typed record types and deeper game-specific fields are follow-up
  work.
- Shared plugin, plugin-master-reference, and typed-record repositories use NPoco database models for save behavior.
  Repository delete/query SQL remains parameterized where explicit SQL is used.
- Oblivion is not implemented.
- The UI shows imported records in a tree plus scalar and supported child comparison rows for approved persisted
  record types. Patch generation and conflict resolution behavior do not exist yet.
- The asset preview pane can load an early subset of NIF mesh geometry, with many model types and visual details still
  pending.

## Shared Record Child Import

Starfield, Fallout 4, and Skyrim import typed record parent rows for MiscItems (`MISC`), Keywords (`KYWD`),
ActorValueInformation (`AVIF`), NPCs (`NPC_`), MagicEffects (`MGEF`), Perks (`PERK`), Statics (`STAT`), and
Containers (`CONT`). Starfield also imports ConditionForms (`CNDF`), Books (`BOOK`), Doors (`DOOR`), and Terminals
(`TERM`) into typed detail tables. These records are mapped in their game adapter projects and persisted through Core
DTOs, repositories, and
typed importers. Scripting adapters are persisted for `GLOB`, `MISC`, `KYWD`, `AVIF`, `NPC_`, `MGEF`, `PERK`, `BOOK`,
and `TERM` when the source record exposes virtual-machine adapter data; `FLST`, `GMST`, and `DOOR` remain flat
records without scripting adapter persistence. Shared child rows for models, keywords, sounds, raw payloads,
scripting adapters, and terminal marker parameters are dispatched by Core DTO capability interfaces and linked through
the owning `RecordInstances` row or terminal parent row.

The current `MISC` implementation persists the parent scalar row plus shared model, keyword, sound, and scripting
adapter rows. `BOOK`, `DOOR`, `CONT`, and `TERM` now persist typed parent scalar rows in addition to their shared
children, and `TERM` also persists marker parameter child rows.
