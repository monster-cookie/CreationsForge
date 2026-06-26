# Design Decisions

## 2026-06-25 - Add Specification Project For Record Metadata

Status: Accepted

Context: The import readers and comparison service are currently wired by hand for every supported record family. That shape is workable for the current approved records, but it does not scale cleanly toward hundreds of major and minor record types across additional Mutagen-supported games. The project also already has validation specs, but those describe sample assertions rather than production record-family metadata.

Decision: Add `CreationsForge.Specification` as a dependency-free production metadata project. The first catalog slice describes `FLST`, `GMST`, and `GLOB` record identity, current game support, source field hints, and comparison field intent. Core references the specification project and registers `IRecordSpecificationProvider`. Later accepted decisions extended the pilot metadata into `RecordComparisonService` and `RecordImportService`; game readers, typed importers, repositories, and complex comparison strategies remain runtime behavior owners for their current responsibilities.

Rationale: A small C# specification foundation gives the project a typed, documented place to grow record metadata without adding dependencies, inventing a file format too early, or forcing a high-risk rewrite of import and comparison behavior. Introducing the project before moving runtime behavior let later work migrate one path at a time while tests guard the catalog shape.

Alternatives considered:

- Keep expanding `RecordTypeCatalog`, `PluginRecordSetDTO`, and `RecordComparisonService` manually for every new record family.
- Move existing DataValidationTests validation specs into production.
- Introduce YAML or JSON production specs immediately.
- Rewrite import and comparison dispatch in the same change as the new project.

Consequences:

- `CreationsForge.Specification` has no project dependencies and uses its own lightweight game identifiers so Core can depend on it without a circular reference.
- Core composition can resolve `IRecordSpecificationProvider` for import, comparison, future validation, and UI-neutral services.
- `RecordTypeCatalog` plus non-pilot comparison/import branches remain transitional and can drift unless future slices intentionally move consumers to the specification provider.
- No database schema, persisted cache shape, UI workflow, or import behavior changes in the foundation slice.

Related files:

- `CreationsForge.Specification/CreationsForge.Specification.csproj`
- `CreationsForge.Specification/Records/RecordSpecification.cs`
- `CreationsForge.Specification/Records/SupportedRecordSpecifications.cs`
- `CreationsForge.Specification/Records/IRecordSpecificationProvider.cs`
- `CreationsForge.Core/CoreModule.cs`
- `CreationsForge.Core/CreationsForge.Core.csproj`
- `CreationsForge.UnitTests/Specifications/RecordSpecificationCatalogTests.cs`
- `CreationsForge.UnitTests/Specifications/RecordSpecificationProviderTests.cs`

## 2026-06-25 - Drive Pilot Comparison Rows From Specifications

Status: Accepted

Context: `RecordComparisonService` built every record-type comparison row through hand-written branches. The first
specification slice added metadata for `FLST`, `GMST`, and `GLOB`, but comparison behavior still ignored that metadata.
The UI consumes `RecordComparisonDTO`, so changing the comparison implementation must preserve the DTO shape, row
ordering, value-state behavior, and localized display behavior.

Decision: Make `RecordComparisonService` consume `IRecordSpecificationProvider` for simple type-specific comparison
rows on the pilot records. `GLOB` simple scalar rows, `GMST` simple rows, and the `FLST` `AddToList` row are produced
from comparison specifications. `FLST` indexed item rows and localized `GMST` `Data` display remain explicit strategy
hooks because those behaviors are not purely source-path-to-display-value mappings.

Rationale: This proves the specification provider can drive production comparison behavior without rewriting the full
comparison engine or changing the Avalonia UI contract. Keeping special cases as hooks avoids pretending complex row
alignment and localization behavior are solved by scalar metadata.

Alternatives considered:

- Keep the pilot comparison metadata inactive until the import engine also consumes specifications.
- Rewrite the full comparison service around specifications in one change.
- Move localized `GMST` display and `FLST` item expansion into declarative metadata immediately.

Consequences:

- `RecordComparisonService` now has an optional constructor dependency on `IRecordSpecificationProvider`.
- The pilot records' simple rows can be changed through specification metadata.
- Existing direct test construction remains source-compatible through a default provider fallback.
- Complex records, shared child rows, and import dispatch remain on the existing hand-written paths.

Related files:

- `CreationsForge.Core/Services/RecordComparisonService.cs`
- `CreationsForge.Specification/Records/SupportedRecordSpecifications.cs`
- `CreationsForge.Specification/Records/RecordComparisonSpecification.cs`
- `CreationsForge.Specification/Records/RecordComparisonFieldSpecification.cs`
- `CreationsForge.UnitTests/Services/RecordComparisonServiceTests.cs`
- `CreationsForge.UnitTests/Specifications/RecordSpecificationCatalogTests.cs`

## 2026-06-26 - Drive Pilot Import Dispatch From Specifications

Status: Accepted

Context: The first production specification catalog described `FLST`, `GMST`, and `GLOB`, and the comparison service
already consumes pilot comparison metadata. `RecordImportService` still hardcoded those first three record families
before continuing through the rest of the explicit record list. The project needs a low-risk path toward
specification-driven import dispatch without changing game readers, typed importers, persistence, or import results.

Decision: Add import metadata to `RecordSpecification` and route the `FLST`, `GMST`, and `GLOB` import loop through
`IRecordSpecificationProvider`. Each pilot specification names the `PluginRecordSetDTO` collection that contains its
mapped DTOs and whether the record type is required. `RecordImportService` resolves those collections generically,
then reuses the existing typed importer lookup, progress reporting, per-record failure handling, and stale cleanup.
Non-pilot record families remain on the existing explicit import calls.

Rationale: This proves production specifications can drive import dispatch while preserving the behavior most likely
to affect users: record order, unsupported importer accounting, result totals, progress messages, and stale cleanup.
Keeping `PluginRecordSetDTO` and the current typed importers in place avoids bundling a reader rewrite into the import
dispatch pilot.

Alternatives considered:

- Keep import dispatch hardcoded until all record types have specifications.
- Replace `PluginRecordSetDTO` with a generic record bag in the same change.
- Move all current record families behind the specification provider immediately.

Consequences:

- `RecordImportService` now has an optional constructor dependency on `IRecordSpecificationProvider`.
- `FLST`, `GMST`, and `GLOB` dispatch order and record-set access are controlled by specification metadata.
- A typo in a pilot `PluginRecordSetDTO` property is guarded by unit tests and fails at import time.
- Non-pilot record families remain transitional and explicitly dispatched.
- No database schema, persisted cache shape, game-reader mapping, or UI behavior changes.

Related files:

- `CreationsForge.Specification/Records/RecordImportSpecification.cs`
- `CreationsForge.Specification/Records/RecordSpecification.cs`
- `CreationsForge.Specification/Records/SupportedRecordSpecifications.cs`
- `CreationsForge.Core/Services/RecordImportService.cs`
- `CreationsForge.UnitTests/Services/RecordImportServiceTests.cs`
- `CreationsForge.UnitTests/Specifications/RecordSpecificationCatalogTests.cs`

## 2026-06-26 - Complete Spec-Driven Import Dispatch For Current Record Families

Status: Accepted

Context: The pilot import-dispatch slice proved that `RecordImportService` could resolve `PluginRecordSetDTO`
collections through `IRecordSpecificationProvider` while preserving typed importer lookup, progress reporting, failure
handling, and stale cleanup. The remaining current record families were still dispatched through explicit service
calls, which left two sources of import order and required/optional record-family policy.

Decision: Expand `CreationsForge.Specification` so the catalog contains import metadata for every currently imported
record family. Add an explicit import order to `RecordImportSpecification`, preserve the existing dispatch order, and
mark optional families through specification metadata. `RecordImportService` now loops over the ordered specification
catalog for all current record families instead of mixing specification-driven pilot records with hardcoded calls.
Import-only specifications do not add declarative comparison fields; record-specific Core comparison methods remain
the runtime authority for those families until approved comparison migration slices move them.

Rationale: Moving the full current import surface behind one catalog removes duplicated dispatch policy without
changing game readers, typed importers, repositories, persisted schema, or UI-facing import results. Keeping comparison
metadata limited to the existing pilot fields avoids implying that complex child-row and localization behavior has
become declarative before the comparison engine is ready.

Alternatives considered:

- Keep only `FLST`, `GMST`, and `GLOB` import dispatch specification-driven until reader mapping also moves.
- Move comparison metadata for all current record families in the same change.
- Replace `PluginRecordSetDTO` with a generic record bag while completing dispatch migration.

Consequences:

- Import order, required record-type result emission, optional record-type omission, and record-set collection lookup
  are now catalog metadata.
- `RecordImportService` treats an injected `IRecordSpecificationProvider` as the complete dispatch source.
- Tests now guard catalog coverage, contiguous import order, and valid `PluginRecordSetDTO` collection names.
- No database schema, persisted data shape, game-reader mapping, or Avalonia UI behavior changes.
- Existing imported SQLite data is not stale because this changes dispatch metadata only.

Related files:

- `CreationsForge.Specification/Records/RecordImportSpecification.cs`
- `CreationsForge.Specification/Records/SupportedRecordSpecifications.cs`
- `CreationsForge.Core/Services/RecordImportService.cs`
- `CreationsForge.UnitTests/Services/RecordImportServiceTests.cs`
- `CreationsForge.UnitTests/Specifications/RecordSpecificationCatalogTests.cs`
- `Documentation/ARCHITECTURE.md`
- `Documentation/DOMAIN-MODEL.md`
- `Documentation/DESIGN-DECISIONS.md`

## 2026-06-26 - Add Reader Metadata To Record Specifications

Status: Accepted

Context: Import dispatch now consumes specification metadata for the current record families, but the Starfield,
Fallout 4, and Skyrim reader services still encode their reader targets and Mutagen collection choices directly in
game-adapter code. A full reader rewrite would be too broad for one step because mapping logic still differs by game
and record family.

Decision: Add reader-facing metadata to each record specification. The new metadata names the destination
`PluginRecordSetDTO` collection, the default Mutagen mod collection name, and whether current behavior still relies on
game-adapter mapping code. Populate the catalog for every current imported record family and add catalog tests that
guard valid DTO destination names, import-reader destination alignment, and populated Mutagen collection names. Do not
change the runtime reader services in this slice.

Rationale: This gives the next reader-dispatch migration a typed target without forcing game-specific Mutagen APIs or
mapping code into `CreationsForge.Specification`. It also prevents the catalog from becoming import-only metadata when
the long-term direction is spec-driven reader, import, comparison, and validation behavior.

Alternatives considered:

- Start rewriting the three game reader services immediately.
- Keep reader targets implicit until every record family has declarative field mappings.
- Store reader collection names only in `RecordGameSupportSpecification`.

Consequences:

- `RecordSpecification` now exposes reader metadata alongside import and comparison metadata.
- Runtime reader behavior is unchanged; game adapters still map Mutagen records into Core DTOs.
- Catalog tests now fail when reader metadata points at a missing `PluginRecordSetDTO` collection.
- No database schema, persisted data shape, dependency injection, or UI workflow changes.

Related files:

- `CreationsForge.Specification/Records/RecordReaderSpecification.cs`
- `CreationsForge.Specification/Records/RecordSpecification.cs`
- `CreationsForge.Specification/Records/SupportedRecordSpecifications.cs`
- `CreationsForge.UnitTests/Specifications/RecordSpecificationCatalogTests.cs`
- `Documentation/ARCHITECTURE.md`
- `Documentation/DOMAIN-MODEL.md`
- `Documentation/DESIGN-DECISIONS.md`

## 2026-06-25 - Keep Spriggit-Backed Rendered UI Validation With Data Validation

Status: Accepted

Context: The comparison UI needs validation that imported DTO readback can be rendered into the Avalonia comparison
grid and still match representative Spriggit samples. `CreationsForge.PresentationTests` already supports headless
Avalonia tests, but its responsibility is isolated presentation behavior rather than Spriggit/data-backed validation.
The existing `CreationsForge.DataValidationTests` specs already identify the game, record type, sample, form key, and
DTO/Spriggit field mappings needed for rendered validation.

Decision: Keep Spriggit-backed rendered comparison UI validation in `CreationsForge.DataValidationTests`. Add optional
comparison UI expectations to the existing validation specs so DTO validation and headless rendered UI validation can
share the same sample identity and expected value source. `CreationsForge.PresentationTests` remains focused on
headless Avalonia unit and presentation behavior tests.

Rationale: The rendered comparison UI checks are a data-validation slice: they depend on imported validation database
state, Spriggit extraction roots, and spec-driven expected values. Keeping them with DataValidationTests avoids a second
Spriggit sample catalog and makes failures easier to interpret alongside DTO readback validation failures.

Alternatives considered:

- Keep Spriggit-backed UI validation in `CreationsForge.PresentationTests`.
- Duplicate a separate UI validation spec catalog under `CreationsForge.PresentationTests`.
- Validate only `IRecordComparisonService` output without rendering Avalonia controls.

Consequences:

- `CreationsForge.DataValidationTests` references the Avalonia presentation project and `Avalonia.Headless.XUnit`.
- Specs can opt into rendered UI validation incrementally through explicit comparison row expectations.
- Existing imported SQLite data must be current for rendered validation to be meaningful, just like DTO validation.
- PresentationTests remains available for UI behavior that does not need Spriggit or imported validation data.

Related files:

- `CreationsForge.DataValidationTests/CreationsForge.DataValidationTests.csproj`
- `CreationsForge.DataValidationTests/Validation/Specs/ValidationSpec.cs`
- `CreationsForge.DataValidationTests/Validation/Specs/ValidationUiComparisonExpectation.cs`
- `CreationsForge.DataValidationTests/Validation/UI/SpriggitComparisonUiSpecRunner.cs`
- `CreationsForge.DataValidationTests/Validation/UI/SpriggitComparisonUiValidationTests.cs`

## 2026-06-25 - Separate Numeric Storage Precision From Display Precision

Status: Accepted

Context: Imported numeric DTO values need to preserve the precision exposed by Mutagen and Spriggit, but some user-facing values such as NPC height and weight are coarse sliders where comparison display should be friendlier. Data validation also found single-precision float readback noise, such as a Starfield NPC face morph blend where Spriggit prints `0.1386505` and DTO readback prints `0.13865050673484802`.

Decision: Keep imported and persisted numeric values exact by default. Add `NumericDisplayPrecisionAttribute` for DTO properties that should use reduced decimal precision when comparison builds display values and comparable state. Use targeted validation normalization for known float-backed source values by parsing them as single-precision floats and formatting with stable `G8` text.

Rationale: Display precision is presentation metadata, not an import or storage rule. Keeping it opt-in avoids hiding meaningful numeric differences across unrelated fields. Float-backed validation normalization handles binary readback noise without rounding source evidence down to coarse UI precision.

Alternatives considered:

- Round all numeric comparison values to three decimals.
- Store rounded values during import.
- Add broad validation tolerance for all decimal values.

Consequences:

- Existing imported SQLite data remains valid because storage/readback values are unchanged.
- Comparison rows for attributed fields may be marked identical when their displayed values match at the declared precision.
- Validation specs must choose float-backed normalization only for fields known to be sourced from single-precision values.

Related files:

- `CreationsForge.Core/DTOs/Records/Metadata/NumericDisplayPrecisionAttribute.cs`
- `CreationsForge.Core/DTOs/Records/NPCDTO.cs`
- `CreationsForge.Core/DTOs/Records/NPCWeightDTO.cs`
- `CreationsForge.Core/Services/RecordComparisonService.cs`
- `CreationsForge.DataValidationTests/Validation/Specs/ValidationSpecRunner.cs`
- `CreationsForge.DataValidationTests/Validation/Specs/NPC/NPCValidationSpecs.cs`

## 2026-06-19 - Persist Localized Record Text

Status: Accepted

Context: Mutagen can expose localized text values through translation-table-backed strings, but CreationsForge was
collapsing imported string data to English scalar values. Manual Spriggit validation showed the same problem anywhere
Spriggit preserves language-specific values: English-only DTOs can report false differences and would corrupt plugin
text on future edit/export paths by writing English into translated fields.

Decision: Add shared localized string persistence under `LocalizedStrings`, owned by `RecordInstances`. `RecordDTO`
exposes localized strings as shared child data, imported records replace those rows through
`IRecordLocalizedStringImportService`, and DTO fields that map directly to Mutagen translation-table-backed strings
use `TranslatedStringDTO`. GameSetting import stores localized `Data` values when available while keeping the scalar
`Data` DTO value because the field also carries setting-type semantics. The Settings screen stores the preferred
record text language. Comparison resolves localized fields through the selected language, then English, then the DTO or
scalar database fallback. The command bar does not expose a language selector.

Rationale: Localized text is record-owned data and needs the same stale-row behavior as other shared child payloads.
Keeping language selection in Settings avoids crowding the command bar and gives comparison a stable persisted display
preference.

Alternatives considered:

- Continue storing only English scalar strings.
- Add per-record-type translation tables.
- Add a command-bar language dropdown.

Consequences:

- Plugins must be reimported after migration 005 so localized string rows are populated.
- Comparison can display localized record text without direct Mutagen access from the UI.
- Translated DTO fields preserve the Mutagen/Spriggit language-table shape instead of exposing English as the public
  contract.
- Additional localized fields can use the shared child table without adding new schema tables.

Related files:

- `CreationsForge.Migrations/Sql/005_Migrations005.sql`
- `CreationsForge.Core/DTOs/Records/LocalizedStringDTO.cs`
- `CreationsForge.Core/DTOs/Records/TranslatedStringDTO.cs`
- `CreationsForge.Core/DTOs/Records/TranslatedStringValueDTO.cs`
- `CreationsForge.Core/Repositories/RecordLocalizedStringRepository.cs`
- `CreationsForge.Core/Services/RecordLocalizedStringImportService.cs`
- `CreationsForge.Core/Utilities/LocalizedStringDTOMapper.cs`
- `CreationsForge.Core/Services/RecordComparisonService.cs`
- `CreationsForge/ViewModels/SettingsViewModel.cs`
- `CreationsForge/Views/SettingsView.cs`

## 2026-06-19 - Add Manual Spriggit DTO Validation Tests

Status: Accepted

Context: Creations Forge maps Bethesda records through Mutagen into project DTOs, repositories, and comparison
surfaces. Existing unit and presentation tests cover focused behavior, but the project also needs a manual engineering
tool that can compare representative Spriggit YAML extraction data against current DTO output across supported games
and record types.

Decision: Add `CreationsForge.DataValidationTests` as a manual xUnit/Shouldly test project rather than a console app.
The project stores validation JSON under `CreationsForge.DataValidationTests/Configuration`, reads Spriggit extraction
roots from environment variables with a read-only `.env` fallback, and compares selected samples against imported DTOs
read back through repositories from the currently configured database.

Rationale: Keeping the harness as a test project gives it Shouldly assertions, test filtering, and normal .NET test
runner ergonomics while avoiding a second command-line app surface. Reusing the CLI import path validates Mutagen
mapping, persistence, and repository readback without a parallel Bethesda parser.

Alternatives considered:

- Add a standalone console validation harness.
- Add validation commands to `CreationsForge.Console`.
- Store validation sample configuration under `Documentation`.

Consequences:

- Manual validation can fail independently of normal unit and presentation test runs.
- Spriggit validation reads the configured local database; schema or import changes should be validated after a
  manual CLI reset/import.
- Sample and approved-difference configuration stays close to the validation code that consumes it.
- Spriggit extraction files remain external local inputs and are not copied into the repository.
- Avalonia comparison-row validation remains a separate validation slice.

Related files:

- `CreationsForge.DataValidationTests/CreationsForge.DataValidationTests.csproj`
- `CreationsForge.DataValidationTests/Configuration/SpriggitValidationSamples.json`
- `CreationsForge.DataValidationTests/Configuration/SpriggitApprovedDifferences.json`
- `Documentation/Instructions/SpriggitManualValidation.md`

## 2026-06-12 - Harden Local Asset And Database Trust Boundaries

Status: Accepted

Context: Asset preview paths come from imported plugin data, and reset/import workflows operate on configured local
database paths. The preview pipeline also reads loose files and archive entries into memory for rendering. These are
local desktop workflows, but they still cross trust boundaries from mod metadata and user-editable configuration into
OS shell launching, file deletion, and large memory allocations.

Decision: Treat external asset opening as a resolved loose-file operation. Imported model paths can preview through the
asset resolver, but shell-open behavior only uses a validated local file under the selected game's data folder. URI-like
paths, rooted paths, device paths, parent traversal, missing files, unsupported extensions, and archive-backed entries
are rejected for external open. Reset only deletes the expected `CreationsForge.sqlite` filename and sidecars, and
custom database paths must contain recognizable CreationsForge database markers before deletion. Loose-file and
archive-backed preview reads enforce a maximum preview asset byte limit before whole-file reads, byte-array allocation,
or decompression.

Rationale: Imported plugins and asset archives are local files but not necessarily trusted files. Keeping validation in
the Core/Assets services preserves UI boundaries while preventing untrusted metadata from directly controlling shell
launches, arbitrary reset deletes, or unbounded preview memory allocation.

Alternatives considered:

- Continue relying on extension checks before external open.
- Allow any configured database path because configuration is local.
- Stream all preview assets immediately instead of adding a bounded in-memory read limit.

Consequences:

- External open works only for real loose files resolved under the selected game's data folder.
- Archive-backed entries remain previewable in memory when supported, but they are not shell-opened directly.
- Custom database reset targets are stricter and may require a valid existing CreationsForge database marker.
- Very large loose files or archive entries return controlled oversized-asset failures instead of being read into
  memory.

Related files:

- `CreationsForge.Core/Services/AssetPreviewPathResolverService.cs`
- `CreationsForge/Services/ExternalAssetOpenService.cs`
- `CreationsForge/ViewModels/AssetPreviewPaneViewModel.cs`
- `CreationsForge.Core/Database/DatabaseResetService.cs`
- `CreationsForge.Bethesda.Assets/Resources/BethesdaAssetProvider.cs`
- `CreationsForge.Bethesda.Assets/Archives/Ba2/Ba2ArchiveReader.cs`
- `CreationsForge.Bethesda.Assets/Archives/Bsa/BsaArchiveReader.cs`

## 2026-06-11 - Probe Starfield Material Files For Preview Textures

Status: Accepted

Context: Starfield `BSLightingShaderProperty` blocks can reference `.mat` material assets instead of direct
`BSShaderTextureSet` DDS paths. Digipick preview geometry was loading through the owned Starfield external `.mesh`
slice, but the render path stayed gray because the NIF reader only understood inline texture paths and texture-set
blocks. Starfield material files can contain richer shader and layered-material behavior than the preview renderer can
model today.

Decision: Keep material parsing inside the owned `CreationsForge.Bethesda.Assets` preview reader and add a narrow
`.mat` probe that resolves material bytes through the existing external-asset resolver. The probe scans material data
for DDS texture references, prefers diffuse/base/color candidates over normal or mask-style maps, and reports
unsupported material feature hints in diagnostics. When a `.mat` texture path appears stale, the reader can also probe
known Starfield `materialsbeta.cdb` files for `BETH`/`STRT` string-table texture paths with the same DDS filename. The
probe does not alter geometry or claim full Starfield material or CDB graph semantics.

Rationale: This reuses the current archive-backed asset resolver and keeps the UI/Core boundaries intact while making
Starfield previews texture-aware enough for practical inspection. It avoids taking a dependency on Nifly and leaves
advanced material behavior for a later, explicit renderer/material-system slice.

Alternatives considered:

- Keep Starfield `.mat` references as material names only and continue rendering gray previews.
- Add Starfield material parsing to the Avalonia renderer.
- Treat `.mat` files as geometry authority and change preview shape from material metadata.

Consequences:

- Starfield NIF preview meshes can receive DDS texture paths from archive-backed material files.
- Stale `.mat` texture paths can be corrected from the Starfield material database string table when a matching DDS
  filename is present.
- Advanced `.mat` behavior such as decals, glass/effects, opacity, layered edge falloff, and shape-affecting render
  semantics remains diagnostic-only, and full CDB object graph parsing remains deferred.
- Material, texture, and geometry asset lookup still flows through the existing asset-file resolver.

Related files:

- `CreationsForge.Bethesda.Assets/Nif/NifPreviewModelReader.cs`
- `CreationsForge/Services/BethesdaAssetPreviewGeometryReader.cs`
- `CreationsForge.UnitTests/Services/NifPreviewModelReaderTests.cs`

## 2026-06-11 - Add First Starfield External Geometry Preview Slice

Status: Accepted

Context: Starfield `BSGeometry` NIFs such as `Meshes\Items\digipic\DigiPic.nif` do not store their full vertex and
index data inline. The NIF block stores object metadata plus a reference to an external `geometries/**/*.mesh`
payload. The existing owned NIF reader could find the `BSGeometry` block, but it still fell back to sample geometry
because there was no external geometry resolution path.

Decision: Keep the owned parser in `CreationsForge.Bethesda.Assets` and add a narrow external-geometry resolver
contract to `NifPreviewReadRequest`. The presentation geometry adapter resolves referenced geometry payloads through
the existing asset-file resolver, then passes the bytes back to the NIF reader. The first `.mesh` decoder supports the
observed Starfield preview shape: versioned mesh payloads with ushort triangle indices and a stride-18 signed
quantized position buffer. Normals and UVs are defaulted for this first preview slice.

Rationale: This keeps asset lookup in the existing application workflow while leaving parsing in the UI-neutral Assets
project. It avoids Nifly and gives CreationsForge a small, testable path for Starfield split geometry without claiming
complete `.mesh` support.

Alternatives considered:

- Keep Starfield `BSGeometry` NIFs on generated fallback geometry until a full mesh parser exists.
- Put external geometry resolution directly inside the NIF parser.
- Add a third-party NIF or Starfield mesh parser dependency.

Consequences:

- Starfield `BSGeometry` NIFs with supported external `.mesh` payloads can now produce real preview silhouettes.
- The first external `.mesh` slice does not yet decode materials, textures, UVs, normals, skinning, or every Starfield
  vertex-buffer variant.
- Archive and loose-file lookup remain owned by the existing asset resolver; the Assets NIF reader receives only
  resolved external bytes.

Related files:

- `CreationsForge.Bethesda.Assets/Nif/NifPreviewReadRequest.cs`
- `CreationsForge.Bethesda.Assets/Nif/NifPreviewModelReader.cs`
- `CreationsForge/Services/BethesdaAssetPreviewGeometryReader.cs`
- `CreationsForge.UnitTests/Services/NifPreviewModelReaderTests.cs`

## 2026-06-09 - Add Read-Only BA2 Archive Reading To Bethesda Assets

Status: Accepted

Context: Real preview paths for Fallout 4, Skyrim, and Starfield are usually archive-backed rather than loose files.
Third-party NIF parser project shape and Starfield support did not make it a good fit for CreationsForge's long-term
asset IO path. The UI also should not own Bethesda archive parsing.

Decision: Keep archive IO in `CreationsForge.Bethesda.Assets` and add a minimal `Ba2ArchiveReader` implementing the
existing `IAssetArchiveReader` contract. The first reader is read-only, targets BA2 general archives, lists archive
entries, and reads uncompressed and zlib-compressed entries into memory. Core registers the reader through Autofac so
`BethesdaAssetProvider` can resolve archive-backed asset bytes without adding archive parsing to the UI.

Rationale: A small owned reader keeps the asset pipeline UI-neutral and testable while avoiding a dependency on a
tool-specific NIF project. It also creates a narrow place to port additional behavior from fo76utils/NifSkope-style C++
code as the preview path matures.

Alternatives considered:

- Fork an existing NIF parser and place BA2/BSA loading inside that fork.
- Keep archive-backed paths as sample-geometry placeholders until all NIF parsing is ready.
- Put BA2 parsing directly in the Avalonia preview services.

Consequences:

- Uncompressed and zlib-compressed entries from BA2 general archives can now be read into memory for later NIF parsing.
- BA2 texture files, Starfield compression variants that are not zlib, BSA archives, material lookup, texture lookup,
  and real NIF mesh extraction remain follow-up work.
- The presentation project remains responsible only for rendering, NIF-to-render-mesh conversion, and external-open
  behavior.

Related files:

- `CreationsForge.Bethesda.Assets/Archives/Ba2/Ba2ArchiveReader.cs`
- `CreationsForge.Bethesda.Assets/Archives/IAssetArchiveReader.cs`
- `CreationsForge.Bethesda.Assets/Resources/BethesdaAssetProvider.cs`
- `CreationsForge.Core/CoreModule.cs`
- `CreationsForge.UnitTests/Services/Ba2ArchiveReaderTests.cs`

## 2026-06-09 - Add First Owned NIF Preview Reader

Status: Accepted

Context: CreationsForge removed the experimental third-party NIF reader path and needs the asset previewer to move
from sample geometry toward real archive-backed mesh previews without putting NIF parsing in the Avalonia project.

Decision: Add a minimal `INifPreviewModelReader` implementation in `CreationsForge.Bethesda.Assets`. The first slice
reads Fallout 4/Skyrim Special Edition-style NIF headers and extracts simple `BSTriShape` vertex/index data into
UI-neutral preview models. The Avalonia project maps those preview models into existing Core preview DTOs and keeps
rendering in the presentation layer.

Rationale: Keeping the first NIF parser in the asset pipeline preserves the UI/Core boundaries and gives the project a
testable place to grow NIF support from fo76utils/NifSkope-style reference behavior without adopting a parser layout
that does not fit CreationsForge.

Alternatives considered:

- Reintroduce the third-party NIF reader path.
- Keep archive-backed NIF previews on generated sample geometry until a complete NIF parser exists.
- Put NIF parsing directly in the Avalonia preview service.

Consequences:

- Simple `BSTriShape` mesh payloads can now produce real preview vertices and triangle indices.
- Unsupported NIF versions, block types, and vertex layouts still return parser status messages instead of rendering.
- Materials, textures, skeletons, collision, Starfield-specific NIF variants, BSA archives, and texture BA2 files
  remain follow-up work.

Related files:

- `CreationsForge.Bethesda.Assets/Nif/NifPreviewModelReader.cs`
- `CreationsForge.Bethesda.Assets/Nif/INifPreviewModelReader.cs`
- `CreationsForge/Services/BethesdaAssetPreviewGeometryReader.cs`
- `CreationsForge.Core/CoreModule.cs`
- `CreationsForge.UnitTests/Services/NifPreviewModelReaderTests.cs`

## 2026-06-08 - Keep Asset Preview Rendering In Presentation

Status: Accepted

Context: CreationsForge needs an experimental asset preview pane for records that persist model paths, but Core must
not reference UI frameworks and the presentation project must not call Mutagen directly. Real NIF parsing is not yet
implemented.

Decision: Add Core asset-preview DTOs and `IAssetPreviewPathResolverService` for UI-neutral candidate resolution from
persisted model rows. Add `CreationsForge.Bethesda.Assets` for UI-neutral Bethesda asset IO result DTOs, in-memory
asset reads, archive-reader contracts, and temporary extraction infrastructure. Add `IAssetFileResolverService` for
UI-neutral local-file resolution that checks absolute paths, game data-folder loose files, normalized `Meshes` paths,
and registered archive readers. The Avalonia presentation project owns the preview pane, an Avalonia
`OpenGlControlBase` renderer using Silk.NET, generated sample geometry, external file launching, and
unsupported-preview logging.

Rationale: This proves the UI workflow without weakening the Core/presentation boundary. It also leaves a stable DTO
shape for future NIF readers or mesh importers to populate with real geometry.

Alternatives considered:

- Put Avalonia or HelixToolkit types directly in Core preview contracts.
- Delay the preview pane until real NIF parsing is available.
- Have the presentation project query model repositories directly.

Consequences:

- Selecting a model-bearing record can show preview candidates and render generated sample geometry through the native
  OpenGL preview control.
- Archive-backed model paths depend on registered archive readers before falling back to generated sample geometry.
- The asset provider can read loose files into memory and dispatch archive reads through registered readers.
- Unsupported, missing, or unreadable preview cases are logged through the UI service/view-model path.
- External opening depends on OS file associations for NifSkope, Blender, or compatible tools.
- Real Starfield archive-backed NIF mesh parsing, unsupported archive compression variants, BSA files, and texture
  loading remain follow-up work.

Related files:

- `CreationsForge.Core/DTOs/Assets/AssetPreviewCandidateDTO.cs`
- `CreationsForge.Core/DTOs/Assets/AssetPreviewModelDTO.cs`
- `CreationsForge.Bethesda.Assets/Files/AssetFileResolutionDTO.cs`
- `CreationsForge.Bethesda.Assets/Resources/IBethesdaAssetProvider.cs`
- `CreationsForge.Bethesda.Assets/Archives/IAssetArchiveReader.cs`
- `CreationsForge.Bethesda.Assets/Temp/IAssetTempFileSession.cs`
- `CreationsForge.Core/Services/AssetFileResolverService.cs`
- `CreationsForge.Core/Services/AssetPreviewPathResolverService.cs`
- `CreationsForge/Views/AssetPreviewOpenGlControl.cs`
- `CreationsForge/ViewModels/AssetPreviewPaneViewModel.cs`
- `CreationsForge/Views/AssetPreviewPaneView.cs`

## 2026-06-08 - Keep Shared Typed Record Support Synchronized Across Games

Status: Accepted

Context: CreationsForge supports Starfield, Fallout 4, and Skyrim as current first-class games. The approved shared
typed record set had drifted: Starfield imported `MISC`, `KYWD`, `AVIF`, `NPC_`, `MGEF`, and `PERK`, while Fallout 4
and Skyrim still imported only `FLST`, `GMST`, and `GLOB`. That made the UI and persisted record comparison surface
look inconsistent across games even when the Core schema and DTOs could represent the same record categories.

Decision: Treat the approved typed record set as synchronized across Starfield, Fallout 4, and Skyrim by default.
Fallout 4 and Skyrim now map `MISC`, `KYWD`, `AVIF`, `NPC_`, `MGEF`, and `PERK` into the existing Core DTOs and shared
typed importers. Future shared typed record import, comparison, persistence, or UI browsing changes must update all
three games in the same task unless the plan explicitly documents the behavior as game-specific.

Rationale: Keeping shared record support in sync makes the multi-game UI predictable and prevents one game adapter
from silently falling behind when Core adds a shared DTO, repository, comparison row, or browser category. Game-specific
adapters still own Mutagen mapping details, so unavailable fields can remain null or empty without changing the shared
database schema.

Alternatives considered:

- Leave Fallout 4 and Skyrim on the smaller `FLST`, `GMST`, and `GLOB` subset.
- Move game-specific mapping into Core to remove repeated adapter code.
- Add separate schemas for Fallout 4 and Skyrim variants of the newly synchronized record types.

Consequences:

- Fallout 4 and Skyrim active plugins can display the same approved record categories as Starfield after reimport.
- Core typed importers for `MISC`, `KYWD`, `AVIF`, `NPC_`, `MGEF`, and `PERK` now support all current games.
- Game adapters may conservatively map unavailable Mutagen fields to null, empty lists, or compatible string values.
- New shared typed record work carries an explicit all-current-games implementation expectation.

Related files:

- `AGENTS.md`
- `CreationsForge.Fallout4/Fallout4RecordReaderService.cs`
- `CreationsForge.Skyrim/SkyrimRecordReaderService.cs`
- `CreationsForge.Core/Importers/MiscItemImporter.cs`
- `CreationsForge.Core/Importers/KeywordImporter.cs`
- `CreationsForge.Core/Importers/ActorValueInformationImporter.cs`
- `CreationsForge.Core/Importers/NPCImporter.cs`
- `CreationsForge.Core/Importers/MagicEffectImporter.cs`
- `CreationsForge.Core/Importers/PerkImporter.cs`
- `CreationsForge.UnitTests/Importers/TypedRecordImporterSupportedGamesTests.cs`

## 2026-06-07 - Persist Shared IModelGetter Data By Record Instance

Status: Accepted

Context: Starfield `MISC` records expose model data through Mutagen's `IModelGetter`. The Creation Kit presents
similar model fields on several major records, but some records wrap those model fields in custom slot or gender
containers.

Decision: Add shared `Models` and `ModelMaterialSwaps` tables keyed to `RecordInstances` plus `ModelSlot` and
`ModelGender`. The first populated record type is Starfield `MISC`, using `ModelSlot = Model` and an empty
`ModelGender`. The shared model payload stores `IModelGetter` fields: file, texture file hashes, light layer, flags,
color remapping index, vestigial flags, and material swaps.

Rationale: `MiscItem`, `Static`, `Book`, `Door`, `Container`, and `Terminal` expose a direct `Model : IModelGetter`
shape, so one shared table family avoids one model table per record type. `Terminal.MarkerModel` is a separate
terminal-specific scalar and does not belong in the shared model table.

Alternatives considered:

- Keep model data in one `MiscItemModels` table.
- Add one model table per record type.
- Delay model persistence until every model-bearing record type is implemented.

Consequences:

- Shared model persistence can be reused by future direct `IModelGetter` record types such as `STAT`, `BOOK`, `DOOR`,
  `CONT`, and `TERM`.
- Starfield `ARMO` and `ARMA` need custom mapping because armor uses gendered world and first-person model wrappers.
- Starfield `WEAP` needs custom mapping because weapons have a direct `Model` plus additional first-person/custom
  model data.
- Future armor, armor-addon, and weapon import work should map their custom slots into `ModelSlot` and `ModelGender`
  deliberately rather than assuming the direct `Model` slot is enough.

Related files:

- `CreationsForge.Migrations/Sql/001_CreateMultiGameImportSchema.sql`
- `CreationsForge.Core/DTOs/Records/ModelDTO.cs`
- `CreationsForge.Core/Services/ModelImportService.cs`
- `CreationsForge.Starfield/StarfieldRecordReaderService.cs`

## 2026-06-07 - Highlight Shared Comparison Value States

Status: Accepted

Context: CreationsForge now renders shared record comparisons in the Avalonia UI, but the initial TreeDataGrid view
did not preserve the predecessor app's visible comparison states or legend.

Decision: Add `RecordComparisonValueState` to Core comparison DTOs and calculate neutral, identical, conflict, and
winning-override states in `RecordComparisonService`. Comparable rows are identical when all visible values match and
conflicting when any visible value differs. Blank values count as values. Single-column comparisons and non-comparable
informational rows stay neutral. The far-right visible value in a conflicting row is treated as the displayed winning
override. The presentation layer maps states to green, red, and yellow cell backgrounds, gives the active plugin
column a gold border, and shows a persistent legend in the status area.

Rationale: Keeping state calculation in Core makes comparison semantics reusable and testable while keeping Avalonia
brushes and layout in the presentation project. The far-right displayed winner matches the predecessor behavior
without implying recursive master-resolution logic that CreationsForge does not yet calculate.

Alternatives considered:

- Keep all color state as presentation-only logic.
- Treat blank values as neutral.
- Highlight all later load-order values as winning overrides.
- Wait for full conflict-resolution workflows before adding comparison colors.

Consequences:

- Shared comparison rows now carry deterministic value states.
- The UI visually distinguishes identical, conflicting, and displayed winning-override values.
- Game-specific comparison sections and true conflict-resolution workflows remain follow-up work.

Related files:

- `CreationsForge.Core/DTOs/Records/RecordComparisonValueState.cs`
- `CreationsForge.Core/DTOs/Records/RecordComparisonFieldDTO.cs`
- `CreationsForge.Core/DTOs/Records/RecordComparisonValueDTO.cs`
- `CreationsForge.Core/Services/RecordComparisonService.cs`
- `CreationsForge/Views/MainView.cs`
- `CreationsForge/ViewModels/MainViewModel.cs`

## 2026-06-07 - Add Initial Shared Record Comparison Contract

Status: Accepted

Context: The predecessor Starfield-only app rendered a broad record comparison view directly from presentation view
models and Starfield-specific DTOs. CreationsForge needs a multi-game comparison path that preserves the UI boundary
and starts from the shared typed records already persisted for Starfield, Fallout 4, and Skyrim.

Decision: Add `IRecordComparisonService` in Core with DTOs for comparison columns, field rows, and values. Shared
repositories expose query methods that fetch all persisted overrides for a selected origin FormKey. The service builds
the first comparison slice for FormLists (`FLST`), GameSettings (`GMST`), and Globals (`GLOB`). The Avalonia UI renders
the comparison DTOs in a `TreeDataGrid` with one field per row and one plugin override per dynamic column.

Rationale: This ports the useful comparison shape without moving Mutagen or database access into the presentation
project. Starting with a simple field/column table keeps the UI focused while leaving richer grouping as future options
after the comparison contract proves useful.

Alternatives considered:

- Port the Starfield-only comparison view wholesale.
- Add TreeDataGrid before validating the shared comparison DTO shape.
- Query repositories directly from the Avalonia view model.
- Wait for patch generation before showing any comparison UI.

Consequences:

- Selecting an imported record leaf can load a first shared comparison view.
- Core owns comparison query and row-building behavior.
- The initial UI compares only approved shared DTO fields.
- Starfield-only sections, conflict-resolution state, winning override calculation, and patch generation remain
  deferred.

Related files:

- `CreationsForge.Core/Services/Interfaces/IRecordComparisonService.cs`
- `CreationsForge.Core/Services/RecordComparisonService.cs`
- `CreationsForge.Core/DTOs/Records/RecordComparisonDTO.cs`
- `CreationsForge.Core/Repositories/FormListRepository.cs`
- `CreationsForge.Core/Repositories/GameSettingRepository.cs`
- `CreationsForge.Core/Repositories/GlobalRepository.cs`
- `CreationsForge/ViewModels/MainViewModel.cs`
- `CreationsForge/Views/MainView.cs`

## 2026-06-06 - Expose Import Control And Progress Through Core Contracts

Status: Accepted

Context: The UI can start first imports and full imports, and the CLI remains useful for headless refreshes. The
shared importer previously had cancellation support, but progress was mostly stage-level and the CLI could not request
a forced full reimport through a first-class parser result.

Decision: Thread force-full-reimport and progress through the Core import contracts. `GameArgumentParser` accepts
`--force` and `--full`, `GameImportDispatcher` and `IGameImporter` accept the force flag, and the shared import
workflow reports `GameImportProgressDTO` snapshots for load-order, plugin, master-reference, record-type, and
record-detail work.

Rationale: This gives both app surfaces the same import control model without moving UI concepts into Core. Progress
remains DTO-based and can be consumed by the Uno UI, CLI output, tests, or future automation.

Alternatives considered:

- Keep force import as a UI-only concept.
- Add UI binding primitives to Core progress objects.
- Leave progress stage-only until the comparison UI exists.

Consequences:

- CLI callers can request a full reimport with `--force` or `--full`.
- UI callers can render richer import progress without direct importer access.
- Core import contracts changed, so importer fakes in tests must implement the progress-aware signature.

Related files:

- `CreationsForge.Console/CommandLine/GameArgumentParser.cs`
- `CreationsForge.Console/CommandLine/GameArgumentParseResult.cs`
- `CreationsForge.Console/Program.cs`
- `CreationsForge.Core/DTOs/Results/GameImportProgressDTO.cs`
- `CreationsForge.Core/Importers/GameImportDispatcher.cs`
- `CreationsForge.Core/Importers/GameImporter.cs`
- `CreationsForge.Core/Importers/Interfaces/IGameImporter.cs`
- `CreationsForge.Core/Services/RecordImportService.cs`
- `CreationsForge.Core/Services/GameImportWorkflowService.cs`

## 2026-06-06 - Use Environment Mod Object Load Order For Starfield Reads

Status: Accepted

Context: Starfield imports still failed for `aurie_terranarmadadelayed.esm` and `looterbot_dlc1.esm` with
`MissingModException sfbgs00d.esm` even though metadata and record reads used `WithLoadOrderFromHeaderMasters()` and
the Starfield environment data folder. Mutagen's separated-master guidance recommends providing a load order with mod
objects when master-style data is needed for Starfield FormID translation.

Decision: Centralize Starfield mod construction in `StarfieldModConstruction`. The helper prefers the full Mutagen
environment load order's mod objects and passes the Starfield environment data folder. If no environment mod objects
are available, it falls back to `WithLoadOrderFromHeaderMasters()` with the same data folder.

Rationale: Environment mod objects carry master-style information that a header-master-only load order can miss. This
keeps Starfield split-master handling inside the Starfield adapter and avoids unsafe `WithNoLoadOrder()` construction.

Alternatives considered:

- Continue using only `WithLoadOrderFromHeaderMasters()`.
- Use `WithNoLoadOrder()` for failing plugins.
- Move Mutagen construction helpers into Core.

Consequences:

- Starfield metadata, master-reference, and record reads use one centralized construction helper.
- Starfield reads prefer separated-master-safe environment mod-object load order construction.
- Core and presentation projects remain free of game-specific Mutagen APIs.

Related files:

- `CreationsForge.Starfield/StarfieldModConstruction.cs`
- `CreationsForge.Starfield/StarfieldPluginReaderService.cs`
- `CreationsForge.Starfield/StarfieldRecordReaderService.cs`

## 2026-06-06 - Wrap Shared Imports In One Database Transaction

Status: Accepted

Context: SFRecordCompareEngine wrapped the full plugin import in an NPoco transaction. CreationsForge initially saved
plugins, master references, and typed record rows without an equivalent transaction, which made large imports pay
SQLite write overhead for many small `Save` calls. FLST imports were especially affected because large plugins can
contain thousands of child item rows.

Decision: Wrap the shared `GameImporter` write workflow in one NPoco transaction. Register database-backed
repositories, importers, and workflow services per lifetime scope so they share the same scoped `IDatabase` and avoid
capturing database connections in application-wide singletons.

Rationale: This ports the proven SFRecordCompareEngine performance behavior without adding bulk-insert APIs or schema
changes. It keeps repository code simple while giving SQLite a single transaction for the import write batch.

Alternatives considered:

- Add bulk insert APIs for individual tables before restoring transaction behavior.
- Keep singleton repositories and rely on WAL mode alone.
- Wrap each plugin in its own transaction.

Consequences:

- Successful imports call `Complete()` on the NPoco transaction after all shared phases finish.
- Uncaught exceptions and cancellation dispose the transaction without completion.
- Database-backed Core repositories, Core importers/services, and game-specific plugin extension repositories are
  lifetime-scoped rather than singletons.

Related files:

- `CreationsForge.Core/Importers/GameImporter.cs`
- `CreationsForge.Core/CoreModule.cs`
- `CreationsForge.Starfield/StarfieldModule.cs`
- `CreationsForge.Fallout4/Fallout4Module.cs`
- `CreationsForge.Skyrim/SkyrimModule.cs`

## 2026-06-06 - Use Case-Insensitive ModKey Name And Filename Lookup

Status: Accepted

Context: Starfield imports logged missing master references for plugins that existed in the load order, including
official masters with casing differences such as `starfield.esm` versus `Starfield.esm` and `sfbgs00d.esm` versus
`SFBGS00D.esm`. SFRecordCompareEngine had already established that Bethesda load-order data, plugin headers, and
Mutagen master references can disagree on casing for the same plugin identity.

Decision: Preserve source casing when storing ModKey components, but treat ModKey name and filename comparisons as
case-insensitive for runtime plugin lookup. When lookup resolves a persisted plugin row, dependent rows use the
persisted ModKey tuple so SQLite foreign keys reference the exact stored parent key. `ModKey_Type` remains exact in
this pass.

Rationale: Casing differences should not cause valid master references to be skipped as missing. Keeping persisted
casing avoids destructive normalization while making lookup behavior match the domain.

Alternatives considered:

- Normalize ModKey names and filenames before persistence.
- Keep filename lookup case-insensitive but leave ModKey name lookup case-sensitive.
- Ignore `ModKey_Type` during lookup.

Consequences:

- `PluginRepository.GetByModKey` compares both `ModKey_Name` and `ModKey_FileName` with SQLite `COLLATE NOCASE`.
- Master-reference persistence uses the resolved master plugin ModKey casing after lookup.
- Persisted plugin identity still includes `ModKey_Type`.
- A future pass may add a scoped fallback if Starfield master references also disagree on `ModKey_Type`.

Related files:

- `CreationsForge.Core/Repositories/PluginRepository.cs`
- `CreationsForge.Core/Importers/GameImporter.cs`
- `CreationsForge.UnitTests/Importers/GameImporterTests.cs`

## 2026-06-06 - Refresh Plugin Import Batches With Stale Row Cleanup

Status: Accepted

Context: Changed and forced plugin imports upserted current master-reference and typed-record rows, but rows removed
from the source plugin could remain in the database. Record import also loaded the same Mutagen plugin once for each
approved record type, and UI cancellation did not reach the expensive import loops.

Decision: Keep the existing schema and add import-batch cleanup behavior. Master references and each typed record type
use a single `ImportedAtUTC` batch timestamp for the current plugin import. After a successful refresh, stale rows for
the same game/plugin whose timestamp was not refreshed are deleted. Typed record cleanup runs only when that record
type has no per-record import failures. Game adapter record readers now return one bundled `PluginRecordSetDTO` for
the approved shared record types so the Core-facing path loads each Mutagen plugin once. Import dispatch, importer
loops, record reads, and record-detail loops accept cancellation tokens.

Rationale: This keeps persisted comparison inputs aligned with the current plugin contents without weakening foreign
keys or adding schema. Guarding typed cleanup on per-record success avoids deleting previously valid data after a
partial import failure. Bundled record reads remove repeated Mutagen construction work while preserving the game
adapter boundary.

Alternatives considered:

- Keep upsert-only behavior and tolerate stale rows.
- Delete all typed rows for a plugin before importing current rows.
- Add new import batch tables or schema version fields.
- Keep one Core record-reader method per record type and accept repeated Mutagen loads.

Consequences:

- Changed and forced imports remove stale master references and approved shared typed rows after successful refreshes.
- Partial typed record failures can leave existing rows in place for that record type until the next successful import.
- `IGameRecordReader` now returns bundled approved records instead of exposing one method per record type.
- UI cancellation can stop between plugins, master references, record-type phases, and record-detail rows.
- Log enrichment no longer includes environment usernames by default.

Related files:

- `CreationsForge.Core/Importers/GameImporter.cs`
- `CreationsForge.Core/Services/RecordImportService.cs`
- `CreationsForge.Core/DTOs/Records/PluginRecordSetDTO.cs`
- `CreationsForge.Core/Importers/Interfaces/IGameRecordReader.cs`
- `CreationsForge.Core/Repositories/PluginMasterReferenceRepository.cs`
- `CreationsForge.Core/Repositories/FormListRepository.cs`
- `CreationsForge.Core/Repositories/GameSettingRepository.cs`
- `CreationsForge.Core/Repositories/GlobalRepository.cs`
- `CreationsForge.Starfield/StarfieldRecordReaderService.cs`
- `CreationsForge.Fallout4/Fallout4RecordReaderService.cs`
- `CreationsForge.Skyrim/SkyrimRecordReaderService.cs`

## 2026-06-06 - Run FormList Item Cleanup Once Per Plugin Batch

Status: Accepted

Context: The initial safe FormList item stale cleanup avoided SQLite expression-depth failures by deleting stale items
with an import-batch timestamp, but it still ran a cleanup statement after every imported FormList. Large plugins with
many FormLists paid that cleanup cost repeatedly and imported much slower than SFRecordCompareEngine.

Decision: Keep the import-batch timestamp strategy, but run FormList item stale cleanup once per successful plugin
FormList batch. Individual FormList imports upsert current parent and child rows only. After the FLST record type
finishes without per-record failures, stale FormListItems and stale FormLists for the same game/plugin are deleted by
batch timestamp.

Rationale: This preserves removed-item cleanup and the partial-failure safety rule while removing repeated per-record
SQLite cleanup work.

Alternatives considered:

- Keep per-FormList cleanup and add another index.
- Delete all FormListItems before importing current rows.
- Add staging tables for current FormList item identities.

Consequences:

- FormList item stale cleanup runs once after a successful plugin FLST import.
- Partial FLST failures leave previous stale rows in place until the next successful import, matching typed record
  cleanup safety behavior.
- No schema changes are required.

Related files:

- `CreationsForge.Core/Importers/FormListImporter.cs`
- `CreationsForge.Core/Repositories/FormListItemRepository.cs`
- `CreationsForge.Core/Repositories/FormListRepository.cs`

## 2026-06-06 - Keep Mutagen Out Of UI And MVVM

Status: Accepted

Context: The predecessor Starfield-only UI called Mutagen directly from presentation and MVVM code. CreationsForge
must support Starfield, Fallout 4, and Skyrim without making the UI depend on one game's Mutagen APIs or record shapes.

Decision: `CreationsForge` owns presentation concerns only. UI and MVVM code consume Core DTOs, result objects, enums,
and UI-neutral application services. `CreationsForge.Core` may use shared Mutagen primitives internally for
game-agnostic mapping, but game-specific Mutagen API usage remains in `CreationsForge.Starfield`,
`CreationsForge.Fallout4`, and `CreationsForge.Skyrim`.

Rationale: This keeps the presentation layer multi-game, testable, and insulated from game-specific Mutagen package
references. It also keeps game-specific mapping close to the packages and APIs that define each game's record
behavior.

Alternatives considered:

- Port the Starfield UI directly and generalize later.
- Move all Mutagen usage into Core.

Consequences:

- UI features may require additional Core DTOs or workflow/query services before they can be displayed.
- Game-specific adapters remain responsible for mapping Mutagen records into approved shared DTOs.
- The first UI phase displays import workflow status and summaries while deeper comparison contracts are designed.

Related files:

- `CreationsForge/App.xaml.cs`
- `CreationsForge/ViewModels/MainViewModel.cs`
- `CreationsForge.Core/Services/Interfaces/IGameSelectionService.cs`
- `CreationsForge.Core/Services/Interfaces/IGameImportWorkflowService.cs`
- `CreationsForge.Starfield/StarfieldRecordReaderService.cs`
- `CreationsForge.Fallout4/Fallout4RecordReaderService.cs`
- `CreationsForge.Skyrim/SkyrimRecordReaderService.cs`

## 2026-06-06 - Keep UI And CLI As Separate App Surfaces

Status: Accepted

Context: CreationsForge needs a cross-platform UI, but a long-term CLI remains useful for scripted imports, CI
validation, batch refreshes, diagnostics, and headless workflows. Sharing startup code by making the CLI reference the
UI project would pull Uno and presentation dependencies into the headless app.

Decision: Keep `CreationsForge` and `CreationsForge.Console` as separate app surfaces. Add `CreationsForge.Bootstrap`
for shared Autofac module registration and Serilog setup. The UI adds only presentation registrations on top of
Bootstrap, and the CLI adds only command-line registrations on top of Bootstrap.

Rationale: This preserves clean dependency direction while removing duplicated composition and logging setup. It also
keeps UI/MVVM code out of the CLI and command-line parsing out of the UI.

Alternatives considered:

- Remove the console app after adding the UI.
- Make `CreationsForge.Console` reference the Uno UI project to reuse startup helpers.
- Keep duplicated composition and logging code in both app surfaces.

Consequences:

- Bootstrap references Core, Migrations, and game adapter projects.
- UI and CLI no longer duplicate shared Autofac module registration or Serilog file logging configuration.
- UI-only types remain in `CreationsForge`; CLI-only types remain in `CreationsForge.Console`.

Related files:

- `CreationsForge.Bootstrap/Composition/AutofacConfigurator.cs`
- `CreationsForge.Bootstrap/Logging/SerilogConfigurator.cs`
- `CreationsForge/App.xaml.cs`
- `CreationsForge.Console/Program.cs`
- `CreationsForge.sln`

## 2026-06-06 - Use Guarded Startup And Import Progress Flow

Status: Superseded by "Use Direct Main Window And Guarded Import Progress Flow"

Context: First and full imports can take 5-15 minutes depending on load-order size. A main-window-only import button
makes that operation too easy to start casually and does not handle first-run state well. The predecessor UI used a
startup game/import flow and progress screen before the Starfield-only main workspace.

Decision: `CreationsForge` starts with a startup flow that asks for a game when no active game is configured, checks
whether the selected game has imported plugin data, warns before first or full imports, and shows an import progress
view. The main window keeps game selection and import actions, but those actions reuse the same warning/progress flow
instead of importing directly from the main view model.

Rationale: This makes long-running imports explicit while still allowing users to switch games without restarting the
app. It also keeps the UI multi-game and routes all import work through Core workflow services.

Alternatives considered:

- Keep only the main-window import buttons.
- Force users to restart the app to switch games.
- Port the Starfield-only startup flow without adapting it for multiple games.

Consequences:

- The UI has presentation-only navigation, window, and dialog services.
- Core exposes stage-level import progress and import-readiness services.
- Rich per-plugin progress remains future work because the current shared import workflow does not yet expose
  per-plugin progress events.

Related files:

- `CreationsForge/ViewModels/StartupFlowViewModel.cs`
- `CreationsForge/ViewModels/ImportProgressViewModel.cs`
- `CreationsForge/ViewModels/MainViewModel.cs`
- `CreationsForge/Views/StartupFlowView.xaml`
- `CreationsForge/Views/ImportProgressView.xaml`
- `CreationsForge.Core/Services/Interfaces/IGameImportReadinessService.cs`
- `CreationsForge.Core/Services/Interfaces/IGameImportWorkflowService.cs`

## 2026-06-06 - Use Direct Main Window And Guarded Import Progress Flow

Status: Accepted

Context: The initial guarded startup flow protected long-running imports, but it delayed access to the main window and
kept game selection in a pre-main experience. The UI now needs active game and active plugin controls in the main
workspace command bar while still ensuring that imports do not run inline in the main workspace.

Decision: `CreationsForge` starts directly in the main view and initializes the database schema during GUI startup
before view-model database queries run. The main view owns active-game and active-plugin autocomplete controls. If a
valid active game is configured, or if the user selects a different active game, the UI navigates to the import
progress view for the import and returns to the main view afterward. New and full imports continue to show the
long-running import warning before the progress view. Active plugin choices are refreshed from imported/openable
plugin rows for the active game after import completes.

Rationale: This keeps the main workspace immediately visible while preserving the explicit long-running import
experience. It also gives active plugin selection a persistent home in the main command bar and keeps all import work
routed through Core workflow services.

Alternatives considered:

- Keep the startup flow and add plugin selection only after startup completes.
- Run imports inline inside the main view while updating status text.
- Make active plugin selection a modal dialog instead of a command-bar autocomplete.

Consequences:

- The startup flow view and view model are removed.
- GUI startup initializes the database schema before main-window content is resolved.
- Active game selection can trigger navigation away from the main workspace to the import progress view.
- Active plugin selection is presentation state backed by Core plugin-query services.

Related files:

- `CreationsForge/App.xaml.cs`
- `CreationsForge/MainWindow.xaml.cs`
- `CreationsForge/ViewModels/MainViewModel.cs`
- `CreationsForge/ViewModels/ImportProgressViewModel.cs`
- `CreationsForge/Views/MainView.xaml`
- `CreationsForge/Views/ImportProgressView.xaml`
- `CreationsForge.Core/Services/Interfaces/IPluginSelectionService.cs`
- `CreationsForge.Core/Services/Interfaces/IGameImportReadinessService.cs`
- `CreationsForge.Core/Services/Interfaces/IGameImportWorkflowService.cs`

## 2026-06-06 - Use Mutagen Environment Data Folders For Reads

Status: Accepted

Context: A Starfield load order imported successfully in SFRecordCompareEngine but failed in CreationsForge with
missing header-master errors such as `sfbgs00d.esm`. The Starfield builder chain already used
`WithLoadOrderFromHeaderMasters()` and `WithDataFolder(...)`, but CreationsForge used persisted `GameDTO.DataFolder`
values as the read-path authority while the predecessor used `GameEnvironment.Typical.*(...).DataFolderPath`.

Decision: Game adapter plugin and record reads use Mutagen environment data folder paths directly. Persisted game
folder metadata can still be saved and displayed, but it is not the source of truth for Mutagen plugin construction.

Rationale: Mutagen environment resolution matches the proven predecessor behavior and keeps load-order, data folder,
and game-release assumptions together inside each game adapter.

Alternatives considered:

- Continue using persisted `GameDTO.DataFolder` for reads.
- Add fallback logic only for Starfield.
- Upgrade Mutagen packages before fixing path authority.

Consequences:

- Starfield reads use `GameEnvironment.Typical.Starfield(StarfieldRelease.Starfield).DataFolderPath`.
- Fallout 4 and Skyrim readers also use their corresponding Mutagen environment data folders.
- Persisted installation and data-folder metadata remains informational.

Related files:

- `CreationsForge.Starfield/StarfieldPluginReaderService.cs`
- `CreationsForge.Starfield/StarfieldRecordReaderService.cs`
- `CreationsForge.Fallout4/Fallout4PluginReaderService.cs`
- `CreationsForge.Fallout4/Fallout4RecordReaderService.cs`
- `CreationsForge.Skyrim/SkyrimPluginReaderService.cs`
- `CreationsForge.Skyrim/SkyrimRecordReaderService.cs`

## 2026-06-05 - Use Shared Core Only For Proven Shared Import Shape

Status: Accepted

Context: CreationsForge needs to evaluate multi-game import feasibility without assuming that every Bethesda
game has identical record headers, flags, fields, or Mutagen APIs. The initial plan overreached by implying all
database models and repositories could live in Core.

Decision: Keep Core responsible for configuration, DI-independent orchestration contracts, database initialization,
shared key DTOs, shared Mutagen primitive mapping, and repositories for the explicitly approved shared schema. Core may
reference shared Mutagen packages such as `Mutagen.Bethesda.Core`, but game-specific Mutagen reader and mapping code
stays in the Starfield, Fallout4, and Skyrim projects. At the time, the console app remained the composition root.

Rationale: This preserves a real game-agnostic center without forcing false sameness onto game-specific record
details. The current schema proves the multi-game key shape while leaving room for game-specific persistence if later
record fields diverge.

Alternatives considered:

- Put all record database models and repositories in Core.
- Duplicate the entire importer and repository stack per game immediately.
- Store raw ModKey and FormKey strings to avoid shared component modeling.

Consequences:

- Core repositories are limited to the approved shared application schema.
- Core may map shared Mutagen primitives such as `ModKey`.
- Game projects are the home for Mutagen API differences and future divergent mapping.
- `Game` is part of plugin and record primary keys.
- New application-schema columns store ModKey and FormKey values as primitive components.
- DbUp `SchemaVersions` remains the migration-state source of truth.
- No hardcoded application schema-version constants are added.

Related files:

- `CreationsForge.Console/Program.cs`
- `CreationsForge.Core/CoreModule.cs`
- `CreationsForge.Core/Configuration/ApplicationConfigurationStore.cs`
- `CreationsForge.Core/Importers/GameImporter.cs`
- `CreationsForge.Migrations/Sql/001_CreateMultiGameImportSchema.sql`
- `CreationsForge.Starfield/StarfieldModule.cs`
- `CreationsForge.Fallout4/Fallout4Module.cs`
- `CreationsForge.Skyrim/SkyrimModule.cs`

## 2026-06-05 - Keep Game Metadata Discovery In Game Adapters

Status: Accepted

Context: Core contained a `GameMetadataService` that returned hardcoded display names and left installation metadata
partial. Real game installation metadata depends on Mutagen APIs and game-specific package references, which are owned
by the Starfield, Fallout4, and Skyrim adapter projects.

Decision: Keep the shared `IGameMetadataService` contract and `GameDTO` shape in Core, but implement metadata
discovery in each game adapter project. Plugin readers use their local metadata service before returning the selected
`GameDTO` to the shared importer. The implemented Mutagen package version exposes installation and data folder lookup
through `GameLocations`, so the adapters use that API for persisted folder metadata.

Rationale: This keeps Core as a loose shared wrapper and prevents it from inventing partial game data. It also
preserves the existing dependency direction: game projects can reference Mutagen game packages, while Core remains
free of game-specific Mutagen dependencies.

Alternatives considered:

- Keep hardcoded Core metadata and leave folder fields null.
- Add game-specific Mutagen package references to Core.
- Put metadata discovery directly in each plugin reader without a dedicated service.

Consequences:

- Core no longer contains a concrete metadata service.
- Game adapters own Mutagen-backed game metadata discovery.
- `Games.InstallationFolder` and `Games.DataFolder` can now be populated from Mutagen location lookup.
- DbUp `SchemaVersions` remains the migration-state source of truth.
- No hardcoded application schema-version constants are added.

Related files:

- `CreationsForge.Core/Services/Interfaces/IGameMetadataService.cs`
- `CreationsForge.Starfield/StarfieldGameMetadataService.cs`
- `CreationsForge.Fallout4/Fallout4GameMetadataService.cs`
- `CreationsForge.Skyrim/SkyrimGameMetadataService.cs`
- `CreationsForge.Starfield/StarfieldPluginReader.cs`
- `CreationsForge.Fallout4/Fallout4PluginReader.cs`
- `CreationsForge.Skyrim/SkyrimPluginReader.cs`

## 2026-06-05 - Preserve Starfield Header-Master Construction

Status: Superseded

Context: Starfield plugin metadata and master-reference reads must account for Starfield-specific master behavior,
including split masters, medium masters, and overlays. Mutagen exposes `WithLoadOrderFromHeaderMasters()` for the
Starfield binary read builder, and this construction path is required to resolve those header masters correctly.
Fallout 4 and Skyrim do not expose the same builder path in the Mutagen package version used by this project, and
their current master-reference behavior does not require it.

Decision: Starfield plugin metadata and master-reference reads must use `WithLoadOrderFromHeaderMasters()`. Fallout 4
and Skyrim readers use their normal construction paths unless future Mutagen or game-specific behavior requires a
different game-specific path.

Rationale: Treating Starfield like the older games can corrupt or omit master-reference information. The Starfield
reader is intentionally different so future refactors do not replace the required header-master construction with a
generic construction path.

Alternatives considered:

- Use generic construction for all games.
- Use load-order import for Starfield metadata and master-reference reads.
- Force the same header-master builder pattern onto Fallout 4 and Skyrim.

Consequences:

- Starfield plugin reads remain intentionally game-specific.
- Future refactors must preserve `WithLoadOrderFromHeaderMasters()` in Starfield metadata and master-reference reads.
- Fallout 4 and Skyrim remain on normal construction paths unless a concrete game-specific need appears.
- DbUp `SchemaVersions` remains the migration-state source of truth.
- No hardcoded application schema-version constants are added.
- Superseded by the later environment mod-object load-order decision for Starfield reads.

Related files:

- `CreationsForge.Starfield/StarfieldPluginReader.cs`
- `CreationsForge.Fallout4/Fallout4PluginReader.cs`
- `CreationsForge.Skyrim/SkyrimPluginReader.cs`

## 2026-06-05 - Read Plugins In Game Adapter Projects

Status: Accepted

Context: The game plugin readers returned empty plugin lists, so imports saved the selected game row but never
persisted the installed game's plugin load order. Plugin discovery depends on Mutagen game packages and local
installed-game state.

Decision: Implement plugin discovery in the Starfield, Fallout4, and Skyrim adapter projects with Mutagen
`LoadOrder.Import<TModGetter>()`. Each adapter maps discovered load-order entries to the shared `PluginDTO` shape and
uses existing Core repositories through the shared importer.

Rationale: Core remains a loose shared wrapper and does not need game-specific Mutagen package references. The game
adapter projects own Mutagen load-order and header access while Core continues to orchestrate shared persistence and
map shared Mutagen primitives.

Alternatives considered:

- Keep plugin readers as empty placeholders.
- Move load-order discovery into Core.
- Add a new shared Mutagen helper project before the repeated adapter mapping proves stable.

Consequences:

- `Plugins` rows can now be populated for each supported game.
- Shared Mutagen primitive mapping can live in Core without adding game-specific package references.
- Master-reference and typed-record imports remain follow-up work.
- DbUp `SchemaVersions` remains the migration-state source of truth.
- No hardcoded application schema-version constants are added.

Related files:

- `CreationsForge.Core/DTOs/Plugins/PluginDTO.cs`
- `CreationsForge.Core/Utilities/ModKeyDTOMapper.cs`
- `CreationsForge.Core/Importers/GameImporter.cs`
- `CreationsForge.Starfield/StarfieldPluginReader.cs`
- `CreationsForge.Fallout4/Fallout4PluginReader.cs`
- `CreationsForge.Skyrim/SkyrimPluginReader.cs`

## 2026-06-05 - Persist Game-Specific Plugin Header Extensions

Status: Accepted

Context: The shared `Plugins` table captures common load-order and plugin header metadata, but Mutagen exposes scalar
header fields that differ by game. Starfield exposes `Branch`, `InteriorCellCount`, and scalar `INTV`; Fallout 4
exposes scalar `INCC`; Skyrim exposes scalar `INCC` and scalar `INTV`. Some additional fields are binary slices or
lists and need a separate persistence decision before they are stored.

Decision: Keep `Plugins` as the shared base table and add game-specific plugin extension tables for audited scalar
header fields. `GameImporter` remains the shared workflow and calls optional `IPluginExtensionImporter`
implementations after saving the base plugin row. Game-specific read views join `Plugins` to the extension tables for
query convenience.

Rationale: This preserves a relational base-plus-extension model without duplicating the import workflow. It also
keeps game-specific Mutagen package usage and field mapping in the adapter projects while allowing Core to coordinate
extension persistence through a game-agnostic interface.

Alternatives considered:

- Add nullable game-specific columns directly to `Plugins`.
- Duplicate the entire importer workflow per game.
- Store all game-specific header values in JSON or binary columns immediately.

Consequences:

- Plugin imports now save base rows before optional game-specific extension rows.
- `StarfieldPlugins`, `Fallout4Plugins`, and `SkyrimPlugins` are keyed to `Plugins` with cascading deletes.
- Binary/list header fields are audited but not persisted in this slice.
- DbUp `SchemaVersions` remains the migration-state source of truth.
- No hardcoded application schema-version constants are added.

Related files:

- `CreationsForge.Core/Importers/Interfaces/IPluginExtensionImporter.cs`
- `CreationsForge.Core/Importers/GameImporter.cs`
- `CreationsForge.Migrations/Sql/002_AddGameSpecificPluginExtensions.sql`
- `CreationsForge.Starfield/DTOs/StarfieldPluginDTO.cs`
- `CreationsForge.Fallout4/DTOs/Fallout4PluginDTO.cs`
- `CreationsForge.Skyrim/DTOs/SkyrimPluginDTO.cs`

## 2026-06-05 - Align Plugin Import Flow With SFRecordCompareEngine

Status: Accepted

Context: CreationsForge was intended to generalize the proven SFRecordCompareEngine import behavior across
Starfield, Fallout 4, and Skyrim, but the initial multi-game implementation drifted into a simpler workflow. It saved
each plugin, immediately saved that plugin's master references, and then invoked record importers. That ordering could
violate `PluginMasterReferences` foreign keys when a master plugin row was not yet saved. The workflow also did not
preserve the SFRecordCompareEngine source-fingerprint behavior for unchanged, changed, missing, failed, and
unsupported plugins, and the plugin repositories hand-wrote upsert SQL instead of using NPoco database models.

Decision: Keep the multi-game project split, but align the plugin import workflow with SFRecordCompareEngine. Game
plugin readers now expose load-order entries, source fingerprints, header-level metadata, and declared master
references separately. `GameImporter` saves all eligible plugin rows before importing master references, skips missing
master endpoints, and runs typed record importers last. Plugin source fingerprints are used to skip unchanged plugins
unless a forced reimport is requested. Missing, unsupported, and failed plugins are persisted with their import states
and skipped for later expensive phases. Core `Plugins` and `PluginMasterReferences` persistence now uses NPoco database
models with `Database.Save`.

Rationale: This restores the working behavior from SFRecordCompareEngine while preserving the multi-game boundaries.
Separating source-info reads from metadata reads avoids unnecessary metadata and record work for unchanged or missing
plugins. Saving all plugin rows before master references satisfies the declared SQLite foreign keys without weakening
the schema.

Alternatives considered:

- Keep the per-plugin save/master-reference/record sequence and remove master-reference foreign keys.
- Continue using explicit SQL upserts for plugin repositories.
- Port all typed record import behavior in the same change.

Consequences:

- `GameImporter` has richer plugin-state behavior and result counts.
- Game plugin readers expose more granular import operations.
- `IPluginRepository` now supports ModKey lookup by game.
- Plugin and master-reference save behavior is model-backed through NPoco.
- Typed record import remains a follow-up; the current typed record readers still return empty lists.
- DbUp `SchemaVersions` remains the migration-state source of truth.
- No hardcoded application schema-version constants are added.

Related files:

- `CreationsForge.Core/Importers/GameImporter.cs`
- `CreationsForge.Core/Importers/Interfaces/IGamePluginReader.cs`
- `CreationsForge.Core/Repositories/PluginRepository.cs`
- `CreationsForge.Core/Repositories/PluginMasterReferenceRepository.cs`
- `CreationsForge.Core/Models/Database/Plugin.cs`
- `CreationsForge.Core/Models/Database/PluginMasterReference.cs`
- `CreationsForge.Starfield/StarfieldPluginReader.cs`
- `CreationsForge.Fallout4/Fallout4PluginReader.cs`
- `CreationsForge.Skyrim/SkyrimPluginReader.cs`

## 2026-06-05 - Split Game Plugin Readers Into Reader Services

Status: Accepted

Context: The game plugin readers were overloaded with multiple responsibilities: installed-game metadata access,
load-order discovery, source fingerprint reads, unsupported-plugin policy, header-level plugin metadata construction,
and declared master-reference reads. SFRecordCompareEngine already uses a reader-service pattern for Starfield that
keeps these Mutagen and file-system operations in a focused service.

Decision: Keep `IGamePluginReader` as the Core-facing importer contract, but make each concrete game plugin reader a
thin facade over a game-specific plugin reader service. The services own load-order, source-info, header-metadata,
unsupported-policy, and master-reference behavior inside their game adapter projects. Starfield service reads preserve
`WithLoadOrderFromHeaderMasters()` for metadata and master-reference construction. Fallout 4 and Skyrim services keep
their normal construction paths.

Rationale: This ports the proven SFRecordCompareEngine shape without moving game-specific Mutagen dependencies into
Core or changing the shared import workflow. It also creates smaller seams for testing source-info behavior and
game-specific unsupported-plugin policy.

Alternatives considered:

- Leave all behavior inside the existing overloaded plugin reader classes.
- Split the Core contract into separate load-order, source-info, metadata, and master-reference interfaces.
- Move game-specific reader services into Core like the original Starfield-only reference project.

Consequences:

- Core importer contracts remain stable.
- Game plugin readers now delegate to game-specific services.
- Autofac modules register both the service and the Core-facing reader facade for each game.
- Starfield-specific header-master construction remains explicit and protected from generic refactors.
- DbUp `SchemaVersions` remains the migration-state source of truth.
- No hardcoded application schema-version constants are added.

Related files:

- `CreationsForge.Core/Importers/Interfaces/IGamePluginReader.cs`
- `CreationsForge.Starfield/Interfaces/IStarfieldPluginReaderService.cs`
- `CreationsForge.Starfield/StarfieldPluginReaderService.cs`
- `CreationsForge.Starfield/StarfieldPluginReader.cs`
- `CreationsForge.Fallout4/Interfaces/IFallout4PluginReaderService.cs`
- `CreationsForge.Fallout4/Fallout4PluginReaderService.cs`
- `CreationsForge.Fallout4/Fallout4PluginReader.cs`
- `CreationsForge.Skyrim/Interfaces/ISkyrimPluginReaderService.cs`
- `CreationsForge.Skyrim/SkyrimPluginReaderService.cs`
- `CreationsForge.Skyrim/SkyrimPluginReader.cs`

## 2026-06-05 - Add Game-Aware Record Import Service

Status: Accepted

Context: `GameImporter` directly invoked each typed record importer for each imported plugin. That preserved the
basic phase ordering but did not match SFRecordCompareEngine's record import workflow for per-record-type discovery,
typed detail importer lookup, unsupported detail importer accounting, or per-record failure isolation.

Decision: Add a Core `RecordImportService` that runs after plugin and master-reference phases. The service discovers
only the approved shared record types, resolves typed detail importers by supported game and record type ID, records
unsupported typed detail importers, and catches per-record failures while continuing the import. The initial approved
record types are FormLists (`FLST`), GameSettings (`GMST`), and Globals (`GLOB`).

Rationale: This ports the proven SFRecordCompareEngine import shape while preserving CreationsForge's multi-game
adapter boundary. Core orchestrates shared DTO import; game-specific Mutagen record mapping remains in the game
adapter projects.

Alternatives considered:

- Keep the direct typed importer loop in `GameImporter`.
- Port every SFRecordCompareEngine record type immediately.
- Move Mutagen record discovery into Core.
- Add progress and cancellation plumbing before the console app has a consumer for it.

Consequences:

- `GameImporter` delegates typed record import to `IRecordImportService`.
- Record import results include per-record-type details and aggregate counters.
- Existing shared typed importers now import one record DTO at a time as typed detail importers.
- Game-specific record readers still return empty typed-record lists until real Mutagen mapping is implemented.
- DbUp `SchemaVersions` remains the migration-state source of truth.
- No hardcoded application schema-version constants are added.

Related files:

- `CreationsForge.Core/Services/RecordImportService.cs`
- `CreationsForge.Core/Services/Interfaces/IRecordImportService.cs`
- `CreationsForge.Core/DTOs/Results/RecordImportResultDTO.cs`
- `CreationsForge.Core/DTOs/Results/RecordTypeImportResultDTO.cs`
- `CreationsForge.Core/Helpers/RecordTypeCatalog.cs`
- `CreationsForge.Core/Importers/GameImporter.cs`
- `CreationsForge.Core/Importers/FormListImporter.cs`
- `CreationsForge.Core/Importers/GameSettingImporter.cs`
- `CreationsForge.Core/Importers/GlobalImporter.cs`

## 2026-06-05 - Implement Starfield Shared Typed Record Readers

Status: Accepted

Context: `RecordImportService` can import approved shared typed records, but Starfield record reads still returned
empty lists. SFRecordCompareEngine already maps Starfield FormLists, GameSettings, and Globals through Mutagen, but it
also stores Starfield-only fields that CreationsForge has not approved in its schema.

Decision: Implement Starfield typed record reading in the Starfield adapter for only the current shared record types:
FormLists (`FLST`), GameSettings (`GMST`), and Globals (`GLOB`). The mapping uses Mutagen inside
`CreationsForge.Starfield` and populates only the fields already present in Core DTOs and the current database
schema.

Rationale: This activates the shared record import pipeline for Starfield without expanding persistence scope or
moving game-specific Mutagen mapping into Core.

Alternatives considered:

- Keep Starfield typed record readers empty.
- Port all SFRecordCompareEngine Starfield fields and schema.
- Move typed record mapping into Core.

Consequences:

- Starfield imports can now produce FormList, FormListItem, GameSetting, and Global rows.
- Extra Starfield-only fields remain out of scope.
- Fallout 4 and Skyrim typed record readers remain placeholders.
- DbUp `SchemaVersions` remains the migration-state source of truth.
- No hardcoded application schema-version constants are added.

Related files:

- `CreationsForge.Starfield/Interfaces/IStarfieldRecordReaderService.cs`
- `CreationsForge.Starfield/StarfieldRecordReaderService.cs`
- `CreationsForge.Starfield/StarfieldRecordReader.cs`
- `CreationsForge.Core/DTOs/Records/FormListDTO.cs`
- `CreationsForge.Core/DTOs/Records/FormListItemDTO.cs`
- `CreationsForge.Core/DTOs/Records/GameSettingDTO.cs`
- `CreationsForge.Core/DTOs/Records/GlobalDTO.cs`

## 2026-06-05 - Sync FormList Items Without Full Child Wipe

Status: Accepted

Context: FormList item import followed SFRecordCompareEngine's delete-all-then-save pattern. That behavior was
correct, but it deleted every existing child row before reinserting current rows even when most items were unchanged.

Decision: FormList item import now saves the parent FormList, assigns current item indexes, upserts current
FormListItems, and then deletes only stale FormListItems for the same parent whose full item identity is no longer
present.

Rationale: The sync keeps removed items from remaining stale while reducing unnecessary delete/reinsert churn. The
existing composite key already includes the parent FormList identity, item FormKey identity, and `Item_Index`, so no
schema change is needed.

Alternatives considered:

- Keep deleting all FormListItems before saving current items.
- Delete stale rows before upserting current rows.
- Add a staging table or import batch marker.

Consequences:

- Current FormListItems are upserted through NPoco save behavior.
- Removed and reordered items are cleaned up by a targeted stale delete.
- SQLite foreign keys and composite keys remain unchanged.
- DbUp `SchemaVersions` remains the migration-state source of truth.
- No hardcoded application schema-version constants are added.

Related files:

- `CreationsForge.Core/Importers/FormListImporter.cs`
- `CreationsForge.Core/Repositories/Interfaces/IFormListItemRepository.cs`
- `CreationsForge.Core/Repositories/FormListItemRepository.cs`

## 2026-06-05 - Implement Skyrim Shared Typed Record Readers

Status: Accepted

Context: `RecordImportService` can import approved shared typed records and Skyrim plugin metadata/master-reference
import validates on the normal Mutagen construction path, but the Skyrim record reader still returned empty lists.

Decision: Implement Skyrim typed record reading in the Skyrim adapter for only the current shared record types:
FormLists (`FLST`), GameSettings (`GMST`), and Globals (`GLOB`). The mapping uses Mutagen inside
`CreationsForge.Skyrim` and populates only the fields already present in Core DTOs and the current database
schema.

Rationale: This activates the shared record import pipeline for Skyrim without expanding persistence scope or moving
game-specific Mutagen mapping into Core.

Alternatives considered:

- Keep Skyrim typed record readers empty.
- Add new Skyrim-only schema fields while implementing the reader.
- Move typed record mapping into Core.

Consequences:

- Skyrim imports can now produce FormList, FormListItem, GameSetting, and Global rows.
- Extra Skyrim-only fields remain out of scope.
- Fallout 4 typed record readers remain placeholders.
- DbUp `SchemaVersions` remains the migration-state source of truth.
- No hardcoded application schema-version constants are added.

Related files:

- `CreationsForge.Skyrim/Interfaces/ISkyrimRecordReaderService.cs`
- `CreationsForge.Skyrim/SkyrimRecordReaderService.cs`
- `CreationsForge.Skyrim/SkyrimRecordReader.cs`
- `CreationsForge.Core/DTOs/Records/FormListDTO.cs`
- `CreationsForge.Core/DTOs/Records/FormListItemDTO.cs`
- `CreationsForge.Core/DTOs/Records/GameSettingDTO.cs`
- `CreationsForge.Core/DTOs/Records/GlobalDTO.cs`

## 2026-06-05 - Implement Fallout 4 Shared Typed Record Readers

Status: Accepted

Context: `RecordImportService` can import approved shared typed records and Fallout 4 plugin metadata/master-reference
import validates on the normal Mutagen construction path, but the Fallout 4 record reader still returned empty lists.

Decision: Implement Fallout 4 typed record reading in the Fallout 4 adapter for only the current shared record types:
FormLists (`FLST`), GameSettings (`GMST`), and Globals (`GLOB`). The mapping uses Mutagen inside
`CreationsForge.Fallout4` and populates only the fields already present in Core DTOs and the current database
schema.

Rationale: This activates the shared record import pipeline for Fallout 4 without expanding persistence scope or
moving game-specific Mutagen mapping into Core.

Alternatives considered:

- Keep Fallout 4 typed record readers empty.
- Add new Fallout 4-only schema fields while implementing the reader.
- Move typed record mapping into Core.

Consequences:

- Fallout 4 imports can now produce FormList, FormListItem, GameSetting, and Global rows.
- Extra Fallout 4-only fields remain out of scope.
- All currently supported games now map the approved shared typed record set.
- DbUp `SchemaVersions` remains the migration-state source of truth.
- No hardcoded application schema-version constants are added.

Related files:

- `CreationsForge.Fallout4/Interfaces/IFallout4RecordReaderService.cs`
- `CreationsForge.Fallout4/Fallout4RecordReaderService.cs`
- `CreationsForge.Fallout4/Fallout4RecordReader.cs`
- `CreationsForge.Core/DTOs/Records/FormListDTO.cs`
- `CreationsForge.Core/DTOs/Records/FormListItemDTO.cs`
- `CreationsForge.Core/DTOs/Records/GameSettingDTO.cs`
- `CreationsForge.Core/DTOs/Records/GlobalDTO.cs`

## 2026-06-05 - Reserve CreationsForge For The Future UI Project

Status: Superseded

Context: The project is moving away from console-only validation toward a cross-platform UI. The current console
application still provides a useful command-line import harness, but the top-level `CreationsForge` project name
needs to remain available for the future UI project.

Decision: Rename the current console project to `CreationsForge.Console` and reserve `CreationsForge` for the future
cross-platform UI project. The console remained the composition root until a UI composition root was added.

Rationale: Keeping the console as a separate harness preserves fast runtime validation while avoiding a name collision
with the planned UI project.

Alternatives considered:

- Delete the console app immediately.
- Keep the console project named `CreationsForge`.
- Add the UI under a different product-level project name.

Consequences:

- Console validation commands use `CreationsForge.Console/CreationsForge.Console.csproj`.
- The solution later added a new `CreationsForge` UI project without moving the console again.
- App data, database, and log identity remain `CreationsForge`.
- DbUp `SchemaVersions` remains the migration-state source of truth.
- No hardcoded application schema-version constants are added.

Related files:

- `CreationsForge.Console/CreationsForge.Console.csproj`
- `CreationsForge.Console/Program.cs`
- `CreationsForge.sln`
- `CreationsForge.UnitTests/CreationsForge.UnitTests.csproj`
