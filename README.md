# Creations Forge

![Creations Forge](./MarketingSites/Images/SFRecordCompareEngine-Header.png)

Creations Forge is a desktop and command-line application for importing, browsing, and comparing Bethesda plugin records. Browse supported records, review overrides, preview selected assets, and compare values across plugins.

Creations Forge replaces Starfield Record Compare Engine and supports multiple games. Desktop and command-line packages are available for Windows and Linux.

![Screen Shot of Record Comparison](./Documentation/Images/RecordCompare.png)

## Current Features

1. Discovers plugins in your local game load order
2. Imports supported plugin details and records for browsing and comparison
3. Skips unchanged plugins during later imports
4. Keeps imported records from multiple supported games available in one application
5. Browses records owned by a selected plugin in a filterable record tree
6. Filters records by FormID and EditorID
7. Compares matching records across imported plugins in load-order order
8. Highlights matching values in green, conflicts in red, and the visible winning override in yellow
9. Compares supported record details, including models, keywords, sounds, scripts, container items, crafting components, perk ranks, conditions, and terminal parameters
10. Provides an experimental model preview pane
11. Displays retained binary data as hexadecimal values and text
12. Supports light and dark desktop themes
13. Provides command-line imports for a selected game, forced reimport, and reset/import-all workflows

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

Additional Fallout 4 record types:

1. Terminals (TERM)

Additional Starfield record types:

1. Condition Forms (CNDF)
2. Terminals (TERM)

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

On Windows, open PowerShell in the extracted CLI ZIP folder or the installation's `Cli` folder, then run one command for the game you want to import:

```powershell
.\CreationsForge.Console.exe --game Starfield
.\CreationsForge.Console.exe --game Fallout4
.\CreationsForge.Console.exe --game Skyrim
```

On Linux, open a terminal in the extracted CLI ZIP folder and run:

```bash
./CreationsForge.Console --game Starfield
./CreationsForge.Console --game Fallout4
./CreationsForge.Console --game Skyrim
```

Debian and Arch installations also provide the `creationsforge-cli` command; for example, `creationsforge-cli --game Starfield` imports Starfield records.

Useful import options:

1. Add `--force` or `--full` to a game import command to force a full reimport for that game.
2. Use `--reset-all` instead of `--game` only when you intend to delete the current application database and import every supported game. This affects the configured application database, even when the command is run from a different folder.

## Source Code

The source code is available at [monster-cookie/CreationsForge](https://github.com/monster-cookie/CreationsForge).

## Social Presence

1. I can be found as Venpi hanging out in the Quarter Onion Games Discord server.
2. You can follow me on X as [@monstercookiebd](https://x.com/monstercookiebd).
3. You can follow me on Threads as [@monstercookiebd](https://www.threads.net/@monstercookiebd).
