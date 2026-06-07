# Change Log

## Unreleased

- Due to supporting all BGS games technically, it has been rebranded to Creations Forge
- Full multi game support (immplemented currently Skyrim, Fallout 4, and Starfield)
- Converted to Avalonia UI after major issues with UNO and WinUI compatibility
- Added support for Virtual Machine Adapters aka Scripts.
- Revamped the status bar to look more modern and speced out.
- Added missing child schemas for Perks (Ranks, Skills, etc.)
- Added missing child schemas for MiscItems (Transforms, models, sounds).

### BREAKING CHANGE/BUG
- This is a completely new appllication and data model. You should uninstall/remove the old SFRecordComparisonEngine version.

## Version 1.0.3 - 2026-06-01 [BETA]

- Replaced the WinUI-only application host with Uno Platform Skia Desktop.
- Changed the Linux application-data default to `~/.SFRecordCompareEngine` so Debian-installed launches can write
  configuration, SQLite database, and log files without elevated permissions.
- Added persisted plugin header record counts and header-flag-based plugin classification.
- Added migration-triggered and manual full plugin reimport support.
- Added total and active-plugin record counts plus plugin type details to the main status area.

## Version 1.0.0 - 2026-05-31 [BETA]

- Initial release of the Starfield Record Compare Engine. 
- This version includes the core features of plugin discovery, record browsing, and record comparison for a subset of
  Starfield record types. 
- Future updates will expand supported record types, add editing capabilities, and improve performance.
