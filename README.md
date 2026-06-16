# Creations Forge

![Creations Forge](./MarketingSites/Images/SFRecordCompareEngine-Header.png)

Creations Forge is a cross-platform desktop and command-line application for importing, browsing, and comparing
Bethesda plugin records. It imports selected plugin metadata and supported record details into a local SQLite cache,
then provides workflows for browsing records, reviewing overrides, previewing selected assets, and comparing values
across plugins.

Creations Forge is the multi-game replacement for Starfield Record Compare Engine. The Avalonia desktop application
currently targets Windows and Linux packaging, and the CLI harness remains available for import and validation
workflows.

![Screen Shot of Record Comparison](./Documentation/Images/RecordCompare.png)

## Current Features

1. Discovers local load-order plugins for supported games through game-specific Mutagen adapters
2. Imports plugin metadata, declared master references, and supported record details into a local SQLite cache
3. Skips unchanged plugins during later imports using source fingerprints
4. Persists one multi-game schema for imported games, plugins, master references, supported typed records, and shared
   child data
5. Browses records owned by a selected plugin in a filterable record tree
6. Filters records by FormID and EditorID
7. Compares matching records across imported plugins in load-order order
8. Highlights matching values in green, conflicts in red, and the visible winning override in yellow
9. Displays supported child comparison rows, including models, keywords, sounds, scripts, raw payloads, container
   items, constructible object components, perk ranks, condition rows, and terminal marker parameters
10. Provides an experimental asset preview pane for persisted model paths
11. Provides a hexadecimal/string viewer for retained binary reflection payloads
12. Supports light and dark desktop themes
13. Provides CLI imports for one selected game, forced reimport, and reset/import-all workflows

## Supported Games

1. Fallout 4
2. Skyrim
3. Starfield

## Currently Supported Record Types

Cross-game record types:

1. Actor Value Information (AVIF)
2. Books (BOOK)
3. Constructible Objects (COBJ)
4. Containers (CONT)
5. Doors (DOOR)
6. Form Lists (FLST)
7. Game Settings (GMST)
8. Globals (GLOB)
9. Keywords (KYWD)
10. Magic Effects (MGEF)
11. Miscellaneous Items (MISC)
12. NPCs (NPC_)
13. Perks (PERK)
14. Statics (STAT)
15. Terminals (TERM)

Additional Starfield record types:

1. Condition Forms (CNDF)

## Planned Roadmap

1. Expand supported record details for Starfield, Fallout 4, and Skyrim
2. Add Spriggit-compatible plugin and record export/import
3. Validate supported record types against Spriggit and xEdit
4. Add record editing, plugin saving, plugin creation, and patch creation workflows

Long-term goals include local LLM-assisted patch creation.

## Current Limitations

1. Only a subset of Bethesda record details are currently supported.
2. Patch generation and conflict resolution workflows do not exist yet.
3. Oblivion is not implemented.
4. `BlueprintShips*.esm` Starfield plugins are intentionally skipped during import.
5. Mod Organizer 2 can currently break Starfield split-master assembly through Mutagen, so Starfield imports through
   MO2 are not supported at this time.

## Installation

1. Windows users can download the x64 desktop ZIP archive or installer.
2. Linux users can download the x64 desktop ZIP archive, CLI ZIP archive, Debian package, or Arch package.
3. Do not install the application into a game's Data folder.
4. The selected game must be installed and discoverable on your system, including Linux installations running through
   Proton where supported by Mutagen's game discovery.
5. If you used Starfield Record Compare Engine, uninstall or delete it before installing Creations Forge. Do not reuse
   the old SFRecordCompareEngine application folder or cache/log directory.

Application data and logs are stored under:

1. Windows: `C:\ProgramData\CreationsForge`
2. Linux/macOS: `~/.CreationsForge`

## CLI Usage

Run the console harness from the repository with:

```powershell
dotnet run --project ./CreationsForge.Console/CreationsForge.Console.csproj -- --game Starfield
dotnet run --project ./CreationsForge.Console/CreationsForge.Console.csproj -- --game Fallout4
dotnet run --project ./CreationsForge.Console/CreationsForge.Console.csproj -- --game Skyrim
```

Useful import options:

1. `--force` or `--full` forces a full reimport for the selected game.
2. `--reset-all` deletes the current application database and imports every supported game.

## Required Development Environment

1. .NET 10 SDK
2. Visual Studio 2022 or later, VS Code, or JetBrains Rider
3. A supported Bethesda game installation for runtime import testing

## References

- [Mutagen Documentation](https://mutagen-modding.github.io/Mutagen/)
- [Mutagen GitHub Repository](https://github.com/Mutagen-Modding/Mutagen)
- [Spriggit GitHub Repository](https://github.com/Mutagen-Modding/Spriggit)

## Source Code

The source code is available at [monster-cookie/CreationsForge](https://github.com/monster-cookie/CreationsForge).

## Social Presence

1. I can be found as Venpi hanging out in the Quarter Onion Games Discord server.
2. You can follow me on X as [@monstercookiebd](https://x.com/monstercookiebd).
3. You can follow me on Threads as [@monstercookiebd](https://www.threads.net/@monstercookiebd).
