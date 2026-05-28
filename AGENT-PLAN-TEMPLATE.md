# PLAN: <Concise Title>

## 1 - Context & Goal

- Problem / Motivation: <why this change is needed>
- Outcome: <what success looks like>

## 2 - Scope

- Files to add/update/remove:

    - /SFRecordCompareEngine/...
    - /SFRecordCompareEngine.Core/...
    - /SFRecordCompareEngine.Migrations/...
    - /SFRecordCompareEngine.UnitTests/...
    - /Documentation/...

- Documentation files to read before implementation:

    - [Mutagen Documentation](https://mutagen-modding.github.io/Mutagen/)
    - /Documentation/SYSTEM-OVERVIEW.md
    - /Documentation/ARCHITECTURE.md
    - /Documentation/DESIGN-DECISIONS.md
    - /Documentation/DOMAIN-MODEL.md
    - /Documentation/DATABASE.md
    - /Documentation/UI-MVVM.md

- Non-goals: <explicitly out-of-scope items>

## 3 - Documentation & Project Knowledge

- Documentation impact:
    - None / Add / Update / Supersede

- Documentation files affected:
    - /Documentation/...

- Design decision update required:
    - Yes / No

- Design decision entry:

    - Date:
    - Status: Proposed / Accepted / Superseded / Rejected
    - Context:
    - Decision:
    - Rationale:
    - Alternatives considered:
    - Consequences:
    - Related files:

- System knowledge updates:

    - Architecture/layering:
    - Domain model/terminology:
    - Database/persistence:
    - UI/MVVM workflow:
    - Dependency injection:
    - Logging/observability:

- Documentation/code conflicts found:
    - None / <describe conflict and proposed resolution>

- If no documentation update is needed, state exactly:
    - Documentation impacts: None

## 4 - Data Model, Persistence & Config

- Models/DTOs affected:
- Persistence/file format/schema impacts:
- Config/AppSettings/environment impacts:
- Database migrations:
- DbUp SchemaVersions source-of-truth statement, if migration code is touched:
- Hardcoded schema-version constants added: No
- Migrations or rollback steps, if applicable:

## 5 - Tech & Implementation

- Interfaces/DTOs first, if applicable:
- Services:
- Factories:
- Stores:
- Repositories/Data access:
- UI/MVVM boundary:
    - Core UI framework references added: No
    - Core view models/UI commands/dialog/navigation abstractions added: No
    - Presentation-only changes:
    - UI-neutral Core changes:
- MAUI/UI:
- Autofac registrations:
- Serilog observability:

## 6 - Risks & Rollback

- Risks:
- Mitigations:
- Rollback plan:

## 7 - Test Plan

- Unit tests (/SFRecordCompareEngine.UnitTests):
    - Add/update:
    - Not added because:
- Excluded test areas:
    - Database access:
    - Repository implementations:
    - DbUp migration execution:
    - UI-bound code:
- Edge cases:
- Data/fixture updates:
- Manual validation:
    - Verify SFRecordCompareEngine.Core has no UI framework package references and no project references to presentation/UI projects.
    - Verify SFRecordCompareEngine.Core does not contain view models, UI commands, pages, views, dialog services, navigation services, or UI-specific binding helpers.
    - Verify documentation updates, if any, reflect the implemented design and do not conflict with code.

## 8 - Telemetry & Logging

- Key logs:
- Diagnostics to verify after running the app:

## 9 - Acceptance Criteria

- [ ] Approved code changes are implemented.
- [ ] Approved documentation updates are implemented, or Documentation impacts is explicitly listed as None.
- [ ] Any design decision that changed architecture, persistence, dependency direction, public interfaces, or UI workflow is captured in /docs/DESIGN-DECISIONS.md.
- [ ] Documentation reflects the final implemented behavior, not speculative or rejected design.
- [ ] Restore, build, tests, and analyzer validation complete or exact environment failure is reported.

## 10 - Execution Steps After Approval

1. Read the relevant /docs files listed in this plan before editing code.
2. Implement contracts, models, DTOs, validators, and applicable tests, excluding database access, repository implementations, DbUp migration execution, and UI-bound code.
3. Implement services/factories/stores/repositories.
4. Wire Autofac registrations and configuration.
5. Update MAUI UI/view models.
6. Add or adjust persistence/database scripts if applicable.
7. Update approved documentation files so they reflect the implemented design.
8. Validate: restore, build, tests, analyzer clean.
9. Summarize results and any public interface, config, persistence, documentation, or UI workflow impacts.

## 11 - Out-of-Scope / Follow-ups

- <items intentionally not addressed here>

Approval required before EXECUTE: Yes
No files will be edited until this plan is approved.
