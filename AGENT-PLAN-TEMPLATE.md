# PLAN: <Concise Title>

Task type: Simple / Standard / Database / Architecture / Documentation-only

## Goal

- <1-3 bullets describing the problem and intended outcome>

## Approved Scope

Files to add/update/remove:

- <exact path>
- <exact path>

Out of scope:

- <anything intentionally not touched>

## Implementation Checklist

1. <specific code/doc step>
2. <specific code/doc step>
3. <specific code/doc step>

## Impact Summary

Documentation impacts: None / Add / Update
Database/schema impacts: None / Add / Update
Config/environment impacts: None / Add / Update
Autofac/DI impacts: None / Add / Update
Serilog/logging impacts: None / Add / Update
Public interface/workflow impacts: None / Add / Update

## Tests & Validation

Automated validation:

- dotnet restore ./CreationsForge.sln
- dotnet build ./CreationsForge.sln --no-restore
- dotnet test ./CreationsForge.UnitTests/CreationsForge.UnitTests.csproj --no-build

Unit tests:

- Add/update: <paths or None>
- Not added because: <only if applicable>

Manual validation:

- <manual checks, if any>

## Risks & Rollback

Risks:

- <real risks only>

Rollback:

- Revert the approved file changes from this plan.

Approval required before EXECUTE: Yes
No files will be edited until this plan is approved.
