# Creations Forge

![Creations Forge](./MarketingSites/Images/SFRecordCompareEngine-Header.png)

Creations Forge browses and compares complete typed fields for major records in Bethesda plugins through a desktop application and a local MCP server. FormList authoring remains available in both interfaces. The desktop discovers installed plugins for the selected game so you can open the plugin you intend to inspect or edit, or create a new one.

This README describes the current source implementation. Automated plugin checks and real Codex MCP save/reopen runs on generated fixtures have passed. Installed-game and live desktop/Codex conflict acceptance checks are not complete. Package CI exercises extracted MCP executables against generated fixtures, but does not establish installed-game or graphical desktop runtime acceptance. See [Known Issues](./Documentation/KNOWN-ISSUES.md) before choosing inputs.

## Current Features

1. Opens plugin files directly, without importing records into an application database.
2. Loads all major records from the opened plugin and its admitted masters into a family-grouped desktop tree, then compares complete typed fields across plugin contexts. MCP listings remain paged.
3. Creates FormLists, overrides selected FormLists, and edits staged output through typed edit controls.
4. Separates unapplied form input, staged workspace changes, and saved output files.
5. Reviews changes and warnings before saving or discarding staged changes.
6. Checks for unfinished operations and unsaved changes before leaving a workspace.
7. Provides save-outcome inspection and recovery actions.
8. Provides a local MCP server over standard input and standard output.
9. Supports light and dark desktop themes.

## Supported Games and Record Types

Read-only browsing and comparison cover the concrete major-record families exposed by the installed Mutagen packages for Starfield, Fallout 4, and Skyrim Special Edition. Generated coverage manifests verify the supported families and their indexed fields against those packages. FormList (FLST) authoring is implemented for the same three games; other record families are read-only. This workflow does not provide general plugin editing, automatic conflict resolution, or asset preview.

Desktop and MCP workspaces are independent. Changes do not synchronize live between them. Save guards check for changed input or output files; resolve a reported conflict instead of assuming another workspace's changes have been adopted.

## Installation

Use a desktop or MCP build containing the plugin replacement. Earlier releases may still contain the retired import workflow; these instructions do not describe those releases. Pull requests build the complete package matrix for inspection, while newly created `v<major>.<minor>.<patch>` tags on the current `master` head create releases containing the same standalone archives and native installers.

| Platform | Desktop package | MCP package | Native package |
| --- | --- | --- | --- |
| Windows x64 | `CreationsForge-Desktop-win-x64-<version>.zip` | `CreationsForge-Mcp-win-x64-<version>.zip` | `CreationsForge-Setup-<version>.exe`, containing both applications |
| Ubuntu/Debian x64 | `CreationsForge-Desktop-linux-x64-<version>.zip` | `CreationsForge-Mcp-linux-x64-<version>.zip` | `CreationsForge_<version>_amd64.deb`, containing both applications and the `creationsforge` and `creationsforge-mcp` launchers |
| Arch Linux x64 | The `linux-x64` desktop archive can be extracted directly | The `linux-x64` MCP archive can be extracted directly | `creationsforge-<version>-1-x86_64.pkg.tar.zst`, containing both applications and the `creationsforge` and `creationsforge-mcp` launchers |
| macOS Intel | `CreationsForge-Desktop-osx-x64-<version>.zip`, containing `CreationsForge.app` | `CreationsForge-Mcp-osx-x64-<version>.tar.gz` | None |
| macOS Apple Silicon | `CreationsForge-Desktop-osx-arm64-<version>.zip`, containing `CreationsForge.app` | `CreationsForge-Mcp-osx-arm64-<version>.tar.gz` | None |

The macOS archives are unsigned and unnotarized. macOS may warn about or block their first launch according to the system's security settings. Intel and Apple Silicon are separate native packages; no universal binary is provided.

1. Install or extract the application outside the game's Data folder.
2. Keep the game's installed plugins and required masters available in its Data directory.
3. Open a plugin read-only to inspect it, explicitly open an eligible plugin for editing, or choose a path for a new plugin. Creations Forge derives the dependency context from the installed load order and plugin headers.

Application configuration and logs use these default locations:

1. Windows configuration: `C:\ProgramData\CreationsForge`; logs: `C:\ProgramData\CreationsForge\Logs\CreationsForge-<startup timestamp>.log`
2. Linux/macOS: `~/.CreationsForge`

Desktop logs are flushed to disk at least once per second. Workspace opening records validation, input preparation, each plugin parse, baseline finalization, completion or failure, and periodic warnings while an open remains incomplete.

Package validation publishes and inspects each desktop archive but does not launch the graphical application. A successful package build therefore does not establish interactive desktop behavior on Windows, Linux, or macOS.

## Desktop Usage

1. Choose **Open Plugin...** and select the game. Creations Forge uses Mutagen to locate the installed Data directory and load order.
2. Search or browse the detected plugin list. Choose **Open Read-Only** to inspect the selected plugin and its records without selecting an output. This is the default action and works for Bethesda-supplied plugins when their plugin inputs are readable.
3. Choose **Open for Editing** only when the selected plugin should become the guarded mutable output. For **New Plugin...**, choose the game, plugin file type, and master size in the creation screen, then select a new plugin path; the picker starts in the detected game Data directory. A new plugin initially admits only that game's base plugin as a master; existing plugins retain their declared read-only master dependencies.
4. Read any unavailable editing reason shown for a plugin before continuing. Creations Forge identifies Bethesda-supplied plugins through Mutagen and does not admit them as editable outputs.
5. Use **All Records** to browse the complete family-grouped tree from the opened plugin and its admitted masters. The screen shows the current plugin and record count while loading. Select a record, then choose its before and after plugin contexts to inspect both complete typed field trees and their semantic changes.
6. Use **FormList Authoring** to compare or edit FormLists. **Find Reference...** looks up linked records.
7. In an editing workspace, choose **New FormList**, **Override selected**, or **Edit staged output**. Select an edit action, choose **Open action**, and complete its controls. Available actions depend on the game and current selection.
8. Resolve validation feedback, then choose **Apply to staged output**. This updates staged workspace changes; it does not save the output file.
9. Choose **Review Changes...** to inspect changes and warnings. **Discard form changes** clears only unapplied form input; previously staged changes remain.
10. Choose **Save Changes...** and confirm with **Save and Proceed** when available. To discard staged workspace changes, choose **Discard Changes...** and **Discard and Proceed**. Read disabled-action reasons and the resulting status before leaving.

If an unapplied local draft blocks leaving, choose **Keep Editing** to return and preserve the input. For an exact pending editor operation, use **Return to editor** when offered, then **Retry exact pending operation** if available. During an active editor operation, the leave dialog offers **Wait for operation**, **Cancel operation and wait**, and **Keep Editing**. Cancellation requests do not prove that an operation failed or changes were discarded; wait for its outcome.

If saving reports an uncertain outcome, read **Persistence status** and use **Inspect Save Outcome** when available. Follow the enabled recovery choices and their explanations. Completing a prepared save, restoring previous output, and resuming unsaved changes have different effects; they are not a sequence to run automatically. A committed save whose display refresh failed offers **Retry Refresh**. Do not treat a closed dialog as proof of a successful save.

## MCP Usage

Configure a stdio-capable MCP client to launch the dedicated MCP executable without command-line arguments. Select the executable from your actual build or package. The client supplies workspace inputs through the server's advertised tools; no game or output arguments are accepted by the server.

Use `creationsforge_records_list` to page major records, `creationsforge_record_inspect` to page a typed record tree or its warnings from an exact plugin context, and `creationsforge_record_compare` to page semantic changes and the complete before and after trees. Select the `warnings` section of `creationsforge_record_inspect` to page warnings independently. These tools are read-only and use revision-bound continuation tokens. FormList-specific tools continue to provide authoring and save workflows.

From a folder containing the Windows MCP executable:

```powershell
.\CreationsForge.Mcp.exe
```

From a folder containing the Linux or macOS MCP executable:

```bash
./CreationsForge.Mcp
```

This starts a protocol server, not an interactive import prompt. Protocol messages use standard input/output; logs use standard error. Package CI runs MCP checks against executables extracted from the standalone Windows, Linux, and macOS archives and from the Arch package payload. It inspects the Debian package contents but executes the Linux standalone archive, and it builds the Windows installer without installing or executing it. The executed MCP checks cover initialization, tool discovery, three-game generated-fixture access, save/reopen, standard-stream separation, normal shutdown, and argument rejection.

The MCP executable starts the stdio server when invoked without arguments and rejects command-line arguments with exit code 2. The retired Console import commands `--game`, `--force`, `--full`, and `--reset-all` remain removed with the SQLite backend. The native Debian and Arch packages install `/usr/bin/creationsforge-mcp`; the Windows installer places the executable under its `Mcp` directory. Standalone archives can be configured by passing their extracted executable path directly to the MCP client.

## Updating from the Import Workflow

Open your original plugin from the installed plugin list. Previously imported records are not the source for record editing, and old import databases are not converted into editable plugins. No database reset or reimport is required.

The database-directory setting has been removed. Existing configuration can retain its old JSON property, which is ignored. Removing the import backend does not delete existing database files; do not delete application data as an upgrade step. Keep your configuration, source plugins, and separate output files.

## Current Limitations

Authoring remains limited to FormLists. Major-record browsing and comparison are read-only. Saving edits to an existing localized output that uses separate string files is rejected with `UnsupportedInput` to protect unedited localized data in all three games. Creating a new localized output and editing an existing output with embedded strings are different supported paths; this is not an instruction to convert an existing localized plugin.

Post-removal restore, Release build, automated checks and focused rechecks have passed, with some platform and external-input checks skipped. Real Codex authoring and independent plugin-file verification passed on generated fixtures for all three games. Package CI adds native executable and archive coverage, but hosted results for a particular change must still be checked and graphical applications are not launched. A correction for the installed Starfield parser failure is implemented but has not yet been retested on the affected machine; installed Fallout 4 and Skyrim Special Edition validation inputs have not yet been supplied. Plugin Mod Organizer 2/virtual-file-system compatibility, live desktop/Codex conflict checks, and game-runtime acceptance remain unverified. See [Known Issues](./Documentation/KNOWN-ISSUES.md) for symptoms and recovery guidance.

## Source Code

The source code is available at [monster-cookie/CreationsForge](https://github.com/monster-cookie/CreationsForge).

## Social Presence

1. I can be found as Venpi hanging out in the Quarter Onion Games Discord server.
2. You can follow me on X as [@monstercookiebd](https://x.com/monstercookiebd).
3. You can follow me on Threads as [@monstercookiebd](https://www.threads.net/@monstercookiebd).
