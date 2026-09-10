# Workflow Validation Handoff

This guide records the VWCF-6 baseline and the validation handoff for the proposed VWCF-7 native FormList authoring contract. Approval of the proposed contract is a documentation decision; it does not authorize or imply that the engine is implemented. After contract approval, native engine and per-game acceptance are separate checks requiring implementation, game data, Spriggit extractions, hosted CI, or other listed prerequisites.

## Candidate and prerequisites

The local baseline was executed from source HEAD `46d2d8cad23d61c395c14e4f7397cb700e3e9fa3` on Windows with .NET SDK `10.0.401`, before the documentation and current workflow edits. Run commands from the repository root. The existing console resolves its SQLite path from the application configuration and `SqliteConnectionFactory`; a clone does not necessarily isolate that configured/shared database. Before any reset/import check, inspect the effective database path through the existing configuration/startup diagnostics and use an explicitly disposable configured database, or obtain separate authorization for the selected database.

The portable build/test checks require the repository's existing solution and configured package sources. Spriggit checks additionally require one extraction root per game and a compatible imported SQLite database. Set `SPRIGGIT_STARFIELD_EXTRACTIONS`, `SPRIGGIT_FALLOUT_EXTRACTIONS`, and `SPRIGGIT_SKYRIM_EXTRACTIONS` in the process environment or repository-root `.env`; never commit machine-specific paths. A native authoring acceptance run additionally requires the game installation/load order, representative source and output plugins, a writable disposable output directory, and the engine implementation.

## Executed VWCF-6 baseline

The following commands actually ran successfully on the baseline source snapshot:

1. `dotnet restore ./CreationsForge.sln`. Expected and observed: all 13 projects restored successfully.
2. `dotnet build ./CreationsForge.sln --configuration Release --no-restore`. Expected and observed: build succeeded with 0 warnings and 0 errors.
3. `dotnet test ./CreationsForge.UnitTests/CreationsForge.UnitTests.csproj --configuration Release --no-build --filter "Category!=RequiresStarfield"`. Expected and observed: 402 passed, 0 failed, 0 skipped.
4. `dotnet test ./CreationsForge.PresentationTests/CreationsForge.PresentationTests.csproj --configuration Release --no-build --filter "Category!=RequiresStarfield"`. Expected and observed: 54 passed, 0 failed, 0 skipped.
5. `Invoke-ScriptAnalyzer -Path ./runWithDumps.ps1` with local PSScriptAnalyzer `1.25.0` default rules. Expected and observed: 0 findings. Treat `runWithDumps.ps1` as an analyzer subject only; it also builds and supports `--reset-all`, so do not execute it casually.

The baseline also verified the existing CI dependency in an isolated task temporary location by inspecting its package manifest and import, then cleaning up after release. No shared package configuration was changed. This is dependency/setup evidence only.

## Current workflow checks

The intended workflow contract for the current edits is:

- `.github/workflows/ci.yml` runs on `master` and `mcp-mutagen-base-refactor` pushes, pull requests targeting `master`, and manual dispatch.
- Its PowerShell lint job uses PSScriptAnalyzer `1.25.0`, scans the repository root recursively with default rules, and fails when findings exist.
- Its build/test job restores and builds `CreationsForge.sln` in Release, then runs the filtered UnitTests command. The PresentationTests job remains commented because the hosted headless run has a known random hang history; local presentation results are recorded separately.
- `.github/workflows/package-release.yml` invokes the reusable `./.github/workflows/ci.yml` workflow for release validation before packaging.

The workflow files are owned by the infrastructure task. After those edits are final, inspect their diff and run the focused YAML/actionlint checks available in the checkout. A local workflow syntax check does not prove hosted runner behavior. The package-release caller must resolve `ci.yml`; no `validate-pull-request.yml` caller should remain.

The current workflow edit was checked locally by the infrastructure task with actionlint `1.7.12` against both workflow files, focused workflow assertions, PowerShell parsing of the embedded CI blocks, and a diff check; all passed.

Hosted CI passed for committed candidate `5858aabb4d37ae8b03c302a39c2e51dbd0d19854`: the [push run](https://github.com/monster-cookie/CreationsForge/actions/runs/34435351715) and [pull-request run](https://github.com/monster-cookie/CreationsForge/actions/runs/34435464898) both completed successfully, with `Lint PowerShell` and `Unit Tests` passing. The associated [PR #42](https://github.com/monster-cookie/CreationsForge/pull/42) is open and ready for review from `mcp-mutagen-base-refactor` into `master`. These hosted results cover that committed candidate; they do not establish validation for later documentation commits.

## Native FormList acceptance procedure

Run this procedure only after the engine implementation exists and a disposable source/output fixture is available. The commands below are contract-level steps; the exact engine command or test names must be supplied by the implementation task rather than invented here.

1. Build the final solution with `dotnet restore ./CreationsForge.sln` and `dotnet build ./CreationsForge.sln --configuration Release --no-restore`. Expected: 0 errors and no task-caused warnings.
2. For Starfield, open a representative ESM/source load order and a separate output plugin using native Mutagen. Enumerate FormLists and verify `Components`, `Name`, `Items`, `ConditionalEntries`, and scalar `AddToList` when present. Expected: native typed values, concrete component/condition payloads, list order, duplicates, and nulls are visible.
3. For Fallout 4, repeat enumeration and staged save for `Name` and `Items`. Expected: the native translated name and ordered item links round-trip; unsupported fields are reported instead of invented.
4. For Skyrim Special Edition, repeat enumeration and staged save for `Items`. Expected: ordered item links round-trip and the absence of a native `Name` property in the exact target Mutagen version is respected.
5. Create one new native FormList record and one override in each game. Expected: allocated identities are unique, source records remain unchanged, output records reopen through native Mutagen, and unrelated output records/data remain byte/native-data equivalent.
6. Resolve FormList item references into other supported major-record families. Expected: native typed resolution succeeds where a native getter/group exposes the target and returns an explicit unresolved result otherwise; no other-family authoring is performed.
7. Open two independent workspaces against the same source/output, save one, then attempt the stale save from the other. Expected: the second save returns a revision or baseline conflict without silent overwrite.
8. Cancel before the commit boundary and inject or observe a save/validation failure. Expected: staged files and native handles are cleaned up, output is unchanged where atomic replacement guarantees it, and post-commit cancellation or ambiguity requires reopen inspection.
9. Reopen each successful output and dispose each workspace twice. Expected: committed records and fields remain present, no workspace remains usable after disposal, and no lock/temp-handle leak is observed.
10. Deliberately discard a `Save` response after a known commit and call `RecoverSave` with the workspace ID, predispatch operation ID, and attempted output association. Expected: recovery returns `Committed`; replaying the same operation ID/payload returns the receipt without a second replacement. Repeat with a precommit failure and expect `NotCommitted` before one same-payload retry; any unprovable outcome remains `StillUnknown` and blocks automatic retry.

## Spriggit and imported-data checks

When extraction roots and a fresh compatible database are available, and reset is explicitly authorized for the verified configured database path, run the existing manual harness:

```powershell
dotnet run --project ./CreationsForge.Console/CreationsForge.Console.csproj -- --reset-all
dotnet test ./CreationsForge.DataValidationTests/CreationsForge.DataValidationTests.csproj --filter "Category=SpriggitDataValidation&RecordType=FLST"
```

Expected: reset applies migrations and imports all supported games, then the filtered FormList validation compares selected Spriggit samples against DTOs read back from the resulting database with no task-caused unmatched fields. The reset deletes the configured `CreationsForge.sqlite` and its `-wal`/`-shm` sidecars after the service's target checks; use only an explicitly disposable or verified database path. Do not run this casually: `--reset-all` is destructive to the selected application database, and a disposable clone does not isolate an application-data database automatically.

Current evidence gap: the three extraction variables are unset and no `.env` is present, so no Spriggit import or data validation ran for VWCF-6. Existing DataValidationTests also validate SQLite DTO readback, not the future native authoring engine.

The installed Spriggit CLI reports `0.40.1+Branch.main.Sha.da8152cdfd0313fbf08b217acffc6ac0b6b1b5b5`; its embedded baseline references an older Mutagen version and no compatible local translation package was established. Treat YAML property spelling, default omission, and parity as unverified until a future task pins a compatible translator and supplies disposable per-game fixtures.

## Evidence status

Executed and passed: local restore, Release build, filtered UnitTests, filtered PresentationTests, and local PSScriptAnalyzer baseline on source snapshot `46d2d8c` as listed above. Hosted CI push and pull-request runs for committed candidate `5858aabb4d37ae8b03c302a39c2e51dbd0d19854` also passed both `Lint PowerShell` and `Unit Tests`, including analyzer installation/analysis and restore/build/test steps.

Not run: hosted package-release workflow, hosted PresentationTests (the hosted headless test job remains disabled), Spriggit extraction/import validation, database reset/reimport, native FormList authoring, cross-game ESM output acceptance, stale independent workspace conflicts, cancellation at the commit boundary, save failure/reopen recovery, application/game runtime smoke tests, packaging, and gameplay acceptance. These checks require their listed environment or a later implementation task.

The local presentation pass does not resolve the hosted random-hang history. A successful build, documentation check, or package artifact is not runtime or target-game evidence. Native save acceptance must also fail closed when the selected writer cannot preserve untouched data, must distinguish expected writer-header updates from semantic record-data changes, and must distinguish a known committed output followed by reopen failure from an output that was never committed.

## Related contracts

- [FormList MVP contracts](../Engine/FORMLIST-MVP-CONTRACTS.md)
- [Legacy backend migration](../Engine/LEGACY-BACKEND-MIGRATION.md)
- [Spriggit manual validation](SpriggitManualValidation.md)
- [Architecture](../ARCHITECTURE.md)
