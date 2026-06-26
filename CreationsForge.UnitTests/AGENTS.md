# CreationsForge.UnitTests rules

This folder contains unit tests for testable non-UI behavior.

## Test scope

- Use xUnit, Moq, and Shouldly according to existing patterns.
- Prefer tests for services, factories, validators, DTO behavior, pure normalization logic, and business rules.
- Do not add unit tests for database access, repository implementations, DbUp migration execution, or UI-bound code unless
  the root rules are explicitly changed.
- Do not make unit tests depend on local game installations, ProgramData state, user profile paths, real Nexus/Bethesda
  data, or machine-specific configuration.
- Use small deterministic fixtures.
- Avoid sleeps, timing-sensitive tests, and tests that depend on file ordering unless ordering is part of the behavior.

## Test style

- Keep tests readable and focused on one behavior.
- Use descriptive test names that identify the scenario and expected result.
- Avoid over-mocking simple data objects.
- Do not test implementation details when a public service contract can be tested instead.
- If a task only touches migrations, repositories, or workflows, the plan must explain why no unit tests are added and list the manual or integration validation instead.

## Validation

Run the standard test command after build:

```powershell
dotnet test ./CreationsForge.UnitTests/CreationsForge.UnitTests.csproj --no-build
```
