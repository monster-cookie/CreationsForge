# CreationsForge.Migrations rules

This folder owns DbUp migration scripts and migration execution support.

## DbUp and schema ownership

- DbUp SchemaVersions is the source of truth for migration state.
- Do not add hardcoded application schema-version constants.
- Do not return or log app-defined schema versions from migration runners or schema initializers.
- New schema changes must be added as DbUp migrations and validated through SchemaVersions.
- Keep schema creation and migration behavior centralized in this project.

## SQL migration rules

- Use SQLite-compatible SQL only.
- Prefer additive migrations when practical.
- Use IF NOT EXISTS for tables and indexes when the existing migration style allows it.
- Avoid destructive changes unless explicitly approved in the plan with rollback notes.
- Keep foreign key behavior, cascading behavior, nullability, defaults, checks, and collations explicit.
- Use TEXT COLLATE NOCASE consistently for ModKey, plugin filenames, archive paths, and normalized path keys when that
  matches the current schema strategy.
- Avoid index confetti. Add indexes only for known lookup, search, filter, relationship, or import validation paths.
- Run EXPLAIN QUERY PLAN locally before removing or replacing indexes.

## Documentation requirements

For any persisted schema change, the plan must list the database documentation updates:

- /Documentation/Database/DATABASE.md
- /Documentation/Database/ERD.md

DATABASE.md must describe the final migrated schema shape, not only the first create-table script. ERD relationship lines must show declared SQLite foreign keys only. DbUp-owned tables, including SchemaVersions, must not be treated as application-schema tables.

## Validation

When migrations change, validation should include restore/build/tests plus local SQLite checks where practical:

```sql
PRAGMA foreign_key_check;
PRAGMA integrity_check;
PRAGMA index_list('RecordInstances');
```
