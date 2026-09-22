# Creations Forge

![Creations Forge](./MarketingSites/Images/SFRecordCompareEngine-Header.png)

Creations Forge is being rebuilt as a Mutagen-native authoring system for Starfield, Fallout 4, and Skyrim Special Edition. The current source branch establishes only the clean .NET engine, game-package, production MCP transport, and test boundaries needed by that rebuild.

The baseline does not yet open, inspect, edit, save, or package Bethesda plugins. It does not contain the desktop editor or human MCP Workbench, and it establishes no installed-game or game-runtime acceptance. Those capabilities must be implemented and validated by their governing Linear work before they can be claimed here.

## Current Baseline

1. Targets .NET 10 through a solution containing a UI-neutral engine, one integration assembly for each supported game, a production stdio MCP executable, and unit tests.
2. Loads `Mutagen.Bethesda.Starfield`, `Mutagen.Bethesda.Fallout4`, and `Mutagen.Bethesda.Skyrim` together at the exact centrally managed version recorded in `Directory.Packages.props`.
3. Starts a production MCP server whose standard output is reserved for protocol frames and whose diagnostics use standard error.
4. Keeps Avalonia, database storage, record DTO storage, workspace state, record editing, and persistence outside the engine baseline.

## Supported Game Package Boundaries

Starfield, Fallout 4, and Skyrim Special Edition have compile-time Mutagen integration boundaries. A supported package boundary does not mean that any record family is readable or editable yet.

## Development

Restore, build, and test the baseline from the repository root:

```powershell
dotnet restore .\CreationsForge.sln
dotnet build .\CreationsForge.sln --configuration Release --no-restore
dotnet test .\CreationsForge.UnitTests\CreationsForge.UnitTests.csproj --configuration Release --no-build --no-restore
```

The approved dependency baseline is Mutagen `0.55.0-alpha.54`, ModelContextProtocol `2.2.0`, Microsoft.Extensions.Hosting `10.0.12`, xUnit v3 `4.0.1`, and Microsoft.NET.Test.Sdk `18.10.1`. `Directory.Packages.props` is authoritative for exact package versions.

## MCP Baseline Usage

Configure a stdio-capable MCP client to launch the dedicated MCP executable without command-line arguments. This baseline server initializes but intentionally advertises no workspace, record, editing, or save tools.

From the repository root after a Release build:

```powershell
dotnet .\CreationsForge.Mcp\bin\Release\net10.0\CreationsForge.Mcp.dll
```

This starts a protocol server, not an interactive shell. Protocol messages use standard input and output; diagnostics use standard error. The executable rejects command-line arguments with exit code 2.

## Source Code

The source code is available at [monster-cookie/CreationsForge](https://github.com/monster-cookie/CreationsForge).

## Social Presence

1. I can be found as Venpi hanging out in the Quarter Onion Games Discord server.
2. You can follow me on X as [@monstercookiebd](https://x.com/monstercookiebd).
3. You can follow me on Threads as [@monstercookiebd](https://www.threads.net/@monstercookiebd).
