# Creations Forge

![Creations Forge](./MarketingSites/Images/SFRecordCompareEngine-Header.png)

Creations Forge is a cross-platform Bethesda plugin authoring application for Starfield, Fallout 4, and Skyrim Special Edition. It gives mod authors desktop and MCP-compatible workflows for creating new plugins and opening, inspecting, comparing, editing, and saving existing plugins.

The application is currently undergoing a complete rebuild. Untagged development builds may contain only part of the product workflow and should not be used for production mod authoring.

## Features

1. Opens existing Bethesda plugins and creates new plugin files for Starfield, Fallout 4, and Skyrim Special Edition.
2. Browses records, resolves references, and compares record data across plugin contexts.
3. Edits records through typed game-aware workflows and saves changes to an explicitly selected output plugin.
4. Reviews staged changes, validation warnings, and save outcomes before replacing output files.
5. Supports both a desktop experience and local MCP-compatible automation without requiring an application database as the record authority.
6. Keeps source plugins separate from editable output and provides guarded save and recovery workflows.

## Installation

Install a published release from the [GitHub Releases](https://github.com/monster-cookie/CreationsForge/releases) page. Development branches are not end-user packages.

| Platform | Desktop package | MCP package | Native package |
| --- | --- | --- | --- |
| Windows x64 | `CreationsForge-Desktop-win-x64-<version>.zip` | `CreationsForge-Mcp-win-x64-<version>.zip` | `CreationsForge-Setup-<version>.exe`, containing both applications |
| Ubuntu/Debian x64 | `CreationsForge-Desktop-linux-x64-<version>.zip` | `CreationsForge-Mcp-linux-x64-<version>.zip` | `CreationsForge_<version>_amd64.deb`, containing both applications and the `creationsforge` and `creationsforge-mcp` launchers |
| Arch Linux x64 | The `linux-x64` desktop archive can be extracted directly | The `linux-x64` MCP archive can be extracted directly | `creationsforge-<version>-1-x86_64.pkg.tar.zst`, containing both applications and the `creationsforge` and `creationsforge-mcp` launchers |
| macOS Intel | `CreationsForge-Desktop-osx-x64-<version>.zip`, containing `CreationsForge.app` | `CreationsForge-Mcp-osx-x64-<version>.tar.gz` | None |
| macOS Apple Silicon | `CreationsForge-Desktop-osx-arm64-<version>.zip`, containing `CreationsForge.app` | `CreationsForge-Mcp-osx-arm64-<version>.tar.gz` | None |

The macOS archives are unsigned and unnotarized. macOS may warn about or block their first launch according to the system's security settings. Intel and Apple Silicon are separate native packages; no universal binary is provided.

1. Download the package for your platform and install or extract it outside the game's `Data` directory.
2. Keep the game's installed plugins and required masters available through the game's normal data path or supported mod-manager setup.
3. Launch the desktop application for an interactive workflow or configure the MCP executable in a compatible client.
4. Keep source plugins backed up and select a separate output path until you have reviewed and tested saved changes.

## Development and Contribution

Install the .NET 10 SDK, create a focused working branch from `master`, and keep changes aligned with the existing project boundaries. New behavior should include focused automated coverage, and user-visible changes should update the applicable public documentation.

Restore, build, test, and verify formatting from the repository root:

```powershell
dotnet restore .\CreationsForge.sln
dotnet build .\CreationsForge.sln --configuration Release --no-restore
dotnet test .\CreationsForge.UnitTests\CreationsForge.UnitTests.csproj --configuration Release --no-build --no-restore
dotnet format .\CreationsForge.sln --no-restore --verify-no-changes
```

Before opening a pull request, review the complete diff, confirm that no generated output or machine-specific files are included, and make the pull request ready for review rather than a draft. Pull requests run CI; release packaging runs only for a newly created `v<major>.<minor>.<patch>` tag that points to the current `master` commit.

## MCP Development Host

Configure a stdio-capable MCP client to launch the dedicated MCP executable without command-line arguments. The production host exposes `server_info`, `record_types`, `record_schema`, `workspace_open`, `workspace_state`, `workspace_close`, `records_search`, `record_read`, `record_compare`, `output_create`, `output_open`, `record_create`, `record_override`, `record_apply`, `workspace_preview`, `workspace_save`, and `workspace_discard`. Mutating tools require an `operationId` of at most 128 characters. An identical operation id and payload is replayed for the life of the process and is not persisted across a restart. Reusing an operation id with a different payload returns `replay_conflict`. An evicted operation id returns `replay_expired` and is not executed again. A new operation id is refused with `replay_capacity` when the replay cache cannot record it, and that refusal is not executed.

From the repository root after a Release build:

```powershell
dotnet .\CreationsForge.Mcp\bin\Release\net10.0\CreationsForge.Mcp.dll
```

This starts a protocol server, not an interactive shell. Protocol messages use standard input and output; diagnostics use standard error. The executable rejects command-line arguments with exit code 2.

## MCP Workbench

The standalone Workbench provides a human-launchable desktop shell around the production MCP executable. It keeps the executable, game release, source data directory, selected plugins, output path, master style, text storage, and language explicit; connection and workspace failures remain visible in the diagnostics panel. Build it with the solution and launch `CreationsForge.Workbench\bin\Release\net10.0\CreationsForgeWorkbench.exe`. A saved MCP executable is reused, and a blank one is filled from the installed `Mcp` folder or the development build beside the Workbench. A blank or missing Data directory is filled from the installed game for the selected release. The final Creations Forge GUI keeps the executable name `CreationsForge.exe` and will later add a setting and button that launches this Workbench. That entry is not part of the standalone shell. A missing executable or failed startup leaves the shell disconnected and usable. Disconnect and window close cancel the active call, stop the Workbench-owned child process, and remove the output sidecar lock file (`<plugin>.creationsforge.lock`). The plugin file stays. A killed process can leave that sidecar behind. The operating-system lock ends when the process exits, and the next close of a workspace that opens the same output removes the file.

The initial shell connects to the production MCP executable. Connect negotiates protocol `2025-06-18`, requires the tools capability, and lists every discovered tool with its input schema. Protocol frames stay on the child standard output; host diagnostics stay on standard error and in the Workbench log. Successful results store the session and revision identifiers, and later calls fill `workspaceId` or `expectedRevision` when the discovered schema names them and the call omitted them. Cancel stops the active call and leaves a connected host running. Cancel during startup, Disconnect, and window close stop only the Workbench-owned child. Connect waits up to 15 seconds and each later call waits up to 30 seconds; a timeout stays on the usable screen. If the host process exits, the shell clears those identifiers and leaves Connect available. Create output calls `output_create` and then `workspace_save`, so the new plugin file is written to the selected path. Open output attaches a plugin that already exists and does not rewrite it. A second create or open stays disabled while that session has an output attached. The shell does not duplicate engine or record state; later authoring controls call the discovered production MCP tools through the same child process.

The Workbench writes a Serilog file and shows its path above the diagnostics box. On Windows the preferred directory is `%ProgramData%\CreationsForge\Logs`. Other systems use `~/.CreationsForge/Logs`. Each launch creates `CreationsForgeWorkbench-*.log` and records connection, workspace, output, and MCP host stderr there. If that directory cannot be created, the file falls forward to the user or temporary logs directory.

Selections are stored in the historical `CreationsForge.Config.json` file in the parent of that logs directory (`%ProgramData%\CreationsForge` on Windows, `~/.CreationsForge` otherwise). The file keeps earlier fields such as theme settings. Language and the MCP executable are shared. Each release remembers its own Data directory, source plugins, output path, master style, and text storage. If the preferred directory is not writable, the file falls forward to the user or temporary Creations Forge directory.

## Social Presence

1. I can be found as Venpi hanging out in the Quarter Onion Games Discord server.
2. You can follow me on X as [@monstercookiebd](https://x.com/monstercookiebd).
3. You can follow me on Threads as [@monstercookiebd](https://www.threads.net/@monstercookiebd).
