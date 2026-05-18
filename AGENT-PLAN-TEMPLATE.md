# PLAN: <Concise Title>

## 1 - Context & Goal

- Problem / Motivation: <why this change is needed>
- Outcome: <what success looks like>

## 2 - Scope

- Files to add/update/remove:
  - /SFRecordCompareEngine/...
  - /SFRecordCompareEngine.Core/...
  - /SFRecordCompareEngine.UnitTests/...
- Non-goals: <explicitly out-of-scope items>

## 3 - Data Model, Persistence & Config

- Models/DTOs affected:
- Persistence/file format/schema impacts:
- Config/AppSettings/environment impacts:
- Migrations or rollback steps, if applicable:

## 4 - Tech & Implementation

- Interfaces/DTOs first, if applicable:
- Services:
- Factories:
- Stores:
- Repositories/Data access:
- WPF/UI:
- Autofac registrations:
- Serilog observability:

## 5 - Risks & Rollback

- Risks:
- Mitigations:
- Rollback plan:

## 6 - Test Plan

- Unit tests (/SFRecordCompareEngine.UnitTests):
- Edge cases:
- Data/fixture updates:
- Manual validation:

## 7 - Telemetry & Logging

- Key logs:
- Diagnostics to verify after running the app:

## 8 - Acceptance Criteria

- [ ] Criteria 1
- [ ] Criteria 2
- [ ] Criteria 3

## 9 - Execution Steps After Approval

1) Implement contracts, models, DTOs, validators, and tests where applicable
2) Implement services/factories/stores/repositories
3) Wire Autofac registrations and configuration
4) Update WPF UI/XAML/view models
5) Add or adjust persistence/database scripts if applicable
6) Validate: restore, build, tests, analyzer clean
7) Summarize results and any public interface, config, persistence, or UI workflow impacts

## 10 - Out-of-Scope / Follow-ups

- <items intentionally not addressed here>

Approval required before EXECUTE: Yes
No files will be edited until this plan is approved.
