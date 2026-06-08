# Creations Forge

![Creations Forge](./MarketingSites/Images/SFRecordCompareEngine-Header.png)

Creations Forge is a Mutagen-based plugin import and record comparison prototype for Bethesda games. It imports
selected plugin metadata and supported record details into a local SQLite cache, then provides desktop and command-line
workflows for browsing records, reviewing overrides, and comparing values across plugins.

The Avalonia desktop application currently targets Windows and Linux packaging. A CLI harness is also available for
import and validation workflows.

![Screen Shot of Record Comparison](./Documentation/Images/RecordCompare.png)

## Current Features

1. Discovers local load-order plugins for supported games through game-specific Mutagen adapters
2. Imports plugin metadata, declared master references, and supported record details into a local SQLite cache
3. Skips unchanged plugins during later imports using source fingerprints
4. Persists one multi-game schema for imported games, plugins, master references, and supported typed records
5. Browses records owned by a selected plugin in a filterable record tree
6. Filters records by FormID and EditorID
7. Compares matching records across imported plugins in load-order order
8. Highlights matching values in green, conflicts in red, and the visible winning override in yellow
9. Supports light and dark desktop themes
10. Provides CLI imports for one selected game, forced reimport, and reset/import-all workflows

## Supported Games

1. Fallout 4
2. Skyrim
3. Starfield

## Currently Supported Record Types

Cross-game record types:

1. Actor Value Information (AVIF)
2. Form Lists (FLST)
3. Game Settings (GMST)
4. Globals (GLOB)
5. Keywords (KYWD)
6. Magic Effects (MGEF)
7. Miscellaneous Items (MISC)
8. NPCs (NPC_)
9. Perks (PERK)

## Planned Roadmap

1. Expand supported record details for Starfield, Fallout 4, and Skyrim
2. Add deeper child comparison sections for supported nested structures
3. Add Spriggit-compatible plugin and record export/import
4. Validate supported record types against Spriggit and xEdit
5. Add record editing, plugin saving, plugin creation, and patch creation workflows

Long-term goals include local LLM-assisted patch creation.

## Current Limitations

1. Fallout 4 and Skyrim currently support the shared `FLST`, `GMST`, and `GLOB` typed-record slice.
2. Starfield currently supports the shared typed-record slice plus additional parent rows for `MISC`, `KYWD`, `AVIF`,
   `NPC_`, `MGEF`, and `PERK`.
3. Deep child comparison sections, patch generation, and conflict resolution workflows are deferred.
4. Oblivion is not implemented.
5. `BlueprintShips*.esm` Starfield plugins are intentionally skipped during import.

## Installation

1. Windows users can download the x64 desktop ZIP archive or installer.
2. Linux users can download the x64 desktop ZIP archive, CLI ZIP archive, Debian package, or Arch package.
3. Do not install the application into a game's Data folder.
4. The selected game must be installed and discoverable on your system, including Linux installations running through
   Proton where supported by Mutagen's game discovery.

Application data and logs are stored under:

1. Windows: `C:\ProgramData\CreationsForge`
2. Linux/macOS: `~/.CreationsForge`

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
