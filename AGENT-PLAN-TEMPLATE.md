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

## Token Budget Gate

Before implementation, classify the task:

- Tiny: expected under 10k tokens
- Small: expected 10k-50k tokens
- Medium: expected 50k-150k tokens
- Large: expected 150k-500k tokens
- Very Large: expected over 500k tokens

If a one-file or small targeted change is estimated above 50k tokens, explain the expected token drivers before implementation and propose a lower-token approach.

Default low-token approach:

- inspect fewer files
- use targeted search
- avoid full test suites
- avoid broad repo summaries
- avoid large log dumps
- perform one focused change only

## Token / Usage Budget Estimate

Before implementation, estimate expected token usage for the full task.

Include:

- Estimated task size: Tiny / Small / Medium / Large / Very Large
- Expected token range:
  - Input/context tokens:
  - Cached input tokens, if likely:
  - Output tokens:
  - Total rough range:
- Expected Codex credit impact: Low / Medium / High / Very High
- Confidence: Low / Medium / High
- Primary token drivers:
  - Number of files likely to inspect
  - Expected command/test output volume
  - Expected diff size
  - Risk of retries or exploratory work
- Budget risks:
  - Large generated files
  - Noisy logs
  - Broad repository scans
  - Repeated full-file reads
  - Failing tests or build loops

If the estimate is Large or Very Large, pause before implementation and propose a smaller first slice.

Prefer targeted file reads over broad repository scans. Avoid dumping large command output unless it is necessary. Summarize findings instead of pasting huge logs.

Approval required before EXECUTE: Yes
No files will be edited until this plan is approved.
