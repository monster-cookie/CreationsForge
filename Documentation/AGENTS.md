# Documentation rules

This folder contains durable project knowledge. Documentation should describe the final approved system, not a wish list.

## Primary documentation files

- /Documentation/NAMING-CONVENTIONS.md - Naming conventions
- /Documentation/ARCHITECTURE.md - Layering rules, Core vs presentation responsibilities, dependency direction, DI composition, persistence boundaries, and logging conventions.
- /Documentation/SYSTEM-OVERVIEW.md - Current system purpose, major workflows, project boundaries, and high-level architecture.
- /Documentation/DESIGN-DECISIONS.md - Important design decisions, tradeoffs, rejected alternatives, and rationale.
- /Documentation/DOMAIN-MODEL.md - Important domain concepts, record comparison terminology, Mutagen concepts used by the app, and project-specific naming.
- /Documentation/Database/DATABASE.md - SQLite, NPoco, DbUp migration behavior, schema ownership, and persistence conventions.
- /Documentation/Database/ERD.md - Entity-Relationship Diagram (ERD) of the database schema, including tables, relationships, and constraints.
- /Documentation/CHANGE-LOG.md - Human-maintained release log. Do not modify this file unless the user explicitly asks.
- /Documentation/KNOWN-ISSUES.md - Human-maintained known issues list. Do not modify this file unless the user explicitly asks.

## General documentation

- Use Markdown.
- Do not wrap documentation prose as Markdown natively handles this.
- Keep documentation concise, factual, and tied to observed repository behavior.
- Do not duplicate large code blocks. Reference paths, classes, interfaces, services, migrations, and tests instead.
- If documentation and code conflict, call out the conflict in the plan before editing either side.
- Do not create new documentation files unless they are listed in the approved plan.
- Do not modify /Documentation/CHANGE-LOG.md or /Documentation/KNOWN-ISSUES.md unless the user explicitly asks.

## Required updates

When a change adds, removes, or meaningfully changes architecture, domain behavior, database schema, persistence behavior, dependency injection, logging behavior, workflow, UI workflow, or public interfaces, the plan must include documentation impacts.

If no documentation update is needed, the plan must explicitly say:

```text
Documentation impacts: None
```

## Design decisions

When documenting design decisions, include:

- Date
- Status: Proposed, Accepted, Superseded, or Rejected
- Context
- Decision
- Rationale
- Alternatives considered
- Consequences
- Related files

## Database documentation

For database schema changes:

- /Documentation/Database/DATABASE.md must describe the complete current persisted schema shape.
- /Documentation/Database/ERD.md must include every application-schema table column in Mermaid entity blocks.
- ERD relationship lines must show only declared SQLite foreign keys.
- Inferred record-reference columns must remain documented separately from declared SQLite foreign keys.
- DbUp-owned migration metadata tables, including SchemaVersions, must not be treated as application-schema tables.
- If a later migration adds a column, the docs must describe the final migrated schema, not only the initial create-table script.
