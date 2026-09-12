# Creations Forge

![Creations Forge](./MarketingSites/Images/SFRecordCompareEngine-Header.png)

Creations Forge browses, compares, and edits FormLists in Bethesda plugins through a desktop application and a local MCP server. Open explicit source plugins and their load order, then create or edit a separate output plugin.

This README describes the current source implementation. Automated native checks and real Codex MCP save/reopen runs on generated fixtures have passed. Installed-game, live desktop/Codex conflict, and package acceptance checks are not complete. See [Known Issues](./Documentation/KNOWN-ISSUES.md) before choosing inputs.

## Current Features

1. Opens plugin files directly, without importing records into an application database.
2. Browses and compares FormLists across plugin contexts, with reference lookup for selecting linked records.
3. Creates FormLists, overrides selected FormLists, and edits staged output through typed edit controls.
4. Separates unapplied form input, staged workspace changes, and saved output files.
5. Reviews changes and warnings before saving or discarding staged changes.
6. Checks for unfinished operations and unsaved changes before leaving a workspace.
7. Provides save-outcome inspection and recovery actions.
8. Provides a local MCP server over standard input and standard output.
9. Supports light and dark desktop themes.

## Supported Games and Record Types

Native FormList (FLST) workflows are implemented for Starfield, Fallout 4, and Skyrim Special Edition. Other record families may appear in reference lookup; they are not supported authoring targets. This workflow does not provide general plugin editing, automatic conflict resolution, or asset preview.

Desktop and MCP workspaces are independent. Changes do not synchronize live between them. Save guards check for changed input or output files; resolve a reported conflict instead of assuming another workspace's changes have been adopted.

## Installation

Use a desktop or console build containing the native replacement. Earlier releases may still contain the retired import workflow; these instructions do not describe those releases. Windows and Linux packaging exists in the repository, but the replacement's packaged launches and platform behavior remain unverified.

1. Install or extract the application outside the game's Data folder.
2. Make the source plugin, required masters, explicit load order, and game data directory available. Supply localized-string directories when the inputs require them.
3. Choose a separate output plugin. Keep source plugins available and unchanged while the workspace is open.

Application configuration and logs use these default locations:

1. Windows: `C:\ProgramData\CreationsForge`
2. Linux/macOS: `~/.CreationsForge`

The macOS configuration path does not imply a validated macOS package.

## Desktop Usage

1. Choose **Open Workspace...** and select **Game**.
2. Select **Source plugin** and **Game data directory**. Use **Select load order...** to include the source and required masters. Arrange the **Explicit load order (masters first; source included)** with **Move up** and **Move down**.
3. Add **Localized-string directories (optional)** when needed. Under **Output**, select **Open mode**, **Output plugin**, **Localized strings**, and **Master style**, then choose **Open workspace**. Read any error before continuing. Paths are held for the workspace session.
4. Select a FormList and inspect **Compare**. Use **Find Reference...** to look up linked records.
5. In **Edit**, choose **New FormList**, **Override selected**, or **Edit staged output**. Select an edit action, choose **Open action**, and complete its controls. Available actions depend on the game and current selection.
6. Resolve validation feedback, then choose **Apply to staged output**. This updates staged workspace changes; it does not save the output file.
7. Choose **Review Changes...** to inspect changes and warnings. **Discard form changes** clears only unapplied form input; previously staged changes remain.
8. Choose **Save Changes...** and confirm with **Save and Proceed** when available. To discard staged workspace changes, choose **Discard Changes...** and **Discard and Proceed**. Read disabled-action reasons and the resulting status before leaving.

If an unapplied local draft blocks leaving, choose **Keep Editing** to return and preserve the input. For an exact pending editor operation, use **Return to editor** when offered, then **Retry exact pending operation** if available. During an active editor operation, the leave dialog offers **Wait for operation**, **Cancel operation and wait**, and **Keep Editing**. Cancellation requests do not prove that an operation failed or changes were discarded; wait for its outcome.

If saving reports an uncertain outcome, read **Persistence status** and use **Inspect Save Outcome** when available. Follow the enabled recovery choices and their explanations. Completing a prepared save, restoring previous output, and resuming unsaved changes have different effects; they are not a sequence to run automatically. A committed save whose display refresh failed offers **Retry Refresh**. Do not treat a closed dialog as proof of a successful save.

## CLI and MCP Usage

Configure a stdio-capable MCP client to launch the console executable with the single argument `mcp`. Select the executable from your actual build or package. The client supplies workspace inputs through the server's advertised tools; no game or output arguments are accepted by the `mcp` subcommand.

From a folder containing the Windows console executable:

```powershell
.\CreationsForge.Console.exe mcp
```

From a folder containing the Linux console executable:

```bash
./CreationsForge.Console mcp
```

This starts a protocol server, not an interactive import prompt. Protocol messages use standard input/output; logs use standard error. Real Codex save/reopen checks on generated fixtures have passed; packaged invocation remains unverified.

Running the console with no arguments, `--help`, `-h`, or `help` prints usage and exits with code 0. Unsupported commands print an error and usage to standard error and exit with code 2. The old `--game`, `--force`, `--full`, and `--reset-all` import/reset commands have been removed. Additional arguments after `mcp` are rejected with exit code 2. Source-build smoke checks verified these exit codes; packaged invocation checks remain unverified.

## Updating from the Import Workflow

Open your original plugin files and explicit load order in a native workspace. Previously imported records are not the source for native editing, and old import databases are not converted into editable plugins. No database reset or reimport is required.

The database-directory setting has been removed. Existing configuration can retain its old JSON property, which is ignored. Removing the import backend does not delete existing database files; do not delete application data as an upgrade step. Keep your configuration, source plugins, and separate output files.

## Current Limitations

FormList authoring is the current scope. Saving edits to an existing localized output that uses separate string files is rejected with `UnsupportedInput` to protect unedited localized data in all three games. Creating a new localized output and editing an existing output with embedded strings are different supported paths; this is not an instruction to convert an existing localized plugin.

Post-removal restore, Release build, automated checks and focused rechecks have passed, with some platform and external-input checks skipped. Real Codex authoring and independent native-file verification passed on generated fixtures for all three games. Installed Starfield data has encountered an unresolved parser failure; installed Fallout 4 and Skyrim Special Edition validation inputs have not yet been supplied. Native Mod Organizer 2/virtual-file-system compatibility, packages, live desktop/Codex conflict checks, and game-runtime acceptance remain unverified. See [Known Issues](./Documentation/KNOWN-ISSUES.md) for symptoms and recovery guidance.

## Source Code

The source code is available at [monster-cookie/CreationsForge](https://github.com/monster-cookie/CreationsForge).

## Social Presence

1. I can be found as Venpi hanging out in the Quarter Onion Games Discord server.
2. You can follow me on X as [@monstercookiebd](https://x.com/monstercookiebd).
3. You can follow me on Threads as [@monstercookiebd](https://www.threads.net/@monstercookiebd).
