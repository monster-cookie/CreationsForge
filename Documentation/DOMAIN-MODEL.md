# Domain Model

## Core Concepts

Plugin: A Starfield plugin file discovered from the local load order. Plugins are identified by Mutagen `ModKey` values 
and persisted in the `Plugins` table.

Load order entry: A discovered plugin plus its file name, path, load order index, and enabled state. Represented by 
`PluginLoadOrderEntryDTO`.

ModKey: Mutagen identifier for a plugin. The database stores it as name, type, and file name columns.

FormKey: Mutagen identifier for an individual record. Form list records persist the numeric form ID alongside the owning 
plugin key.

Master reference: A relationship between a plugin and a master plugin declared in the plugin header. Represented by 
`PluginMasterReferenceDTO` and persisted in `PluginMasterReferences`.

Record type: A Starfield major record category. `RecordTypeCatalog` lists known supported and unsupported record type 
names and includes table metadata for `FormList` and `GameSetting`.

## Plugin Import States

`PluginImportState` contains:

- `Current`: the plugin exists and was imported for the current source fingerprint.
- `Changed`: the plugin source differs from the stored fingerprint. The current implementation counts changed plugins during import and saves successfully reimported plugins as `Current`.
- `Missing`: the plugin was present in load order data but the source file was not found on disk.
- `Failed`: plugin metadata import failed.
- `Unsupported`: the plugin is intentionally skipped. Current logic skips `BlueprintShips*.esm`.

## Plugin Metadata

`PluginDTO` carries:

- `ModKey`
- load order index
- enabled and exists-on-disk flags
- import state
- Starfield header flags
- form version
- author and branch
- interior cell count
- source last-write ticks and source file size
- checked, imported, and invalidated timestamps

`StarfieldPluginReaderService` reads metadata from Mutagen using `StarfieldMod.Create(...).FromPath(...).WithLoadOrderFromHeaderMasters().WithDataFolder(...).Construct()`.

## Record Import

`RecordImportService` returns `RecordImportResultDTO` for a plugin. The result aggregates per-record-type counts from `RecordTypeImportResultDTO`.

The active typed detail import path is Starfield `FLST`:

- `StarfieldRecordReaderService.GetFormListFormKeys` reads form list keys from a plugin.
- `FormListImporter` reads each `FormListDTO`.
- `FormListRepository` saves the form list row.
- `FormListItemRepository` saves each item row.

`GameSettingDTO`, the `GameSetting` table, and `GameSettingImporter` exist, but `GameSettingImporter.Import` currently throws `NotImplementedException`.

## Form List Data

`FormListDTO` represents a Starfield form list record with common header fields and form-list-specific data:

- owning `ModKey`
- record `FormKey`
- editor ID
- form version
- Starfield major record flags
- version fields
- imported timestamp
- optional `AddToListFormKey`
- item references

`FormListItemDTO` represents an item reference inside a form list:

- owning plugin `ModKey`
- owning form list `FormKey`
- item plugin `ModKey`
- item `FormKey`
- imported timestamp

## Configuration

`ApplicationConfiguration` currently stores the selected game name. `ApplicationConfigurationStore` loads and saves the 
JSON configuration file and reports whether configuration is required when no selected game is present.
